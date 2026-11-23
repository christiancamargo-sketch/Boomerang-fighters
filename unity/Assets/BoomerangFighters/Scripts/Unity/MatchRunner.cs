// ─────────────────────────────────────────────────────────────
// MATCH RUNNER: el único MonoBehaviour que necesitas en la escena.
// Agrega este componente a un GameObject vacío, dale Play y tendrás:
//   • Partida local (1–2 jugadores con teclado/control/táctil + bots)
//     en los modos ¡Gorrazo!, Lata Caliente, La Reta y Reto del Día
//   • Crear sala LAN (host) o buscar/unirse a una sala en el mismo Wi‑Fi
// Corre la simulación a 60 ticks fijos por segundo (nunca usa
// Time.deltaTime dentro de las reglas) y dibuja con las vistas greybox.
// Agrega solo: DebugHud (HUD en juego) y GameMenu (menús, vestidor,
// premio diario, Reto del Día y resultados con XP).
// ─────────────────────────────────────────────────────────────
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using BoomerangFighters.Core;
using BoomerangFighters.Net;
using UnityEngine;

namespace BoomerangFighters.UnityView
{
    public enum RunMode { Menu, Offline, Host, Client }

    [DisallowMultipleComponent]
    public sealed class MatchRunner : MonoBehaviour
    {
        [Header("Partida")]
        public GameMode GameMode = GameMode.Gorrazo;
        [Tooltip("callejon, museo, azotea o aleatorio")] public string ArenaId = "callejon";
        [Range(1, 9)] public int PointsToWin = 5;
        [Range(1, 2)] public int Humans = 1;
        [Range(0, 7)] public int Bots = 3;
        public BotDifficulty Difficulty = BotDifficulty.Normal;
        public Rules Rules = new Rules();
        [Tooltip("Opcional: ajusta números del juego sin tocar código")] public TuningAsset Tuning;

        [Header("Red local")]
        public string PlayerName = "Jugador";
        public string RoomName = "Sala de Tinta";
        public int GamePort = Protocol.DefaultGamePort;

        [Header("Presentación")]
        public SfxClips Sounds = new SfxClips();

        public RunMode Mode { get; private set; } = RunMode.Menu;
        public MatchSimulation Sim { get; private set; }
        public HostSession Host { get; private set; }
        public ClientSession Client { get; private set; }
        public List<RoomInfo> Rooms { get; private set; } = new List<RoomInfo>();
        public string Status = "";
        public int LocalPlayerIndex => Mode == RunMode.Client ? (Client?.MyIndex ?? 0) : 0;
        /// <summary>La partida actual es el Reto del Día (1 humano contra 3 bots, semilla del día).</summary>
        public bool IsReto => Sim != null && Sim.Mode == GameMode.RetoDelDia;
        public DailyChallenge LastReto { get; private set; }
        public readonly List<LocalInputReader> LocalInputs = new List<LocalInputReader>();
        /// <summary>Eventos de la simulación (en los 3 modos) para HUD, sonido, vibración, analítica.</summary>
        public event System.Action<IReadOnlyList<SimEvent>> SimEvents;
        public WorldView World => _world;
        public Camera Cam => _cam;

        readonly WorldView _world = new WorldView();
        FxDirector _fx;
        CameraRig _rig;
        Camera _cam;
        BotBrain[] _bots;
        InputCommand[] _inputs;
        float _acc;
        LanBeaconSender _beacon;
        LanBeaconListener _listener;
        UdpTransport _transport;
        bool _lastWasReto;
        readonly List<SimEvent> _clientEvents = new List<SimEvent>();

        void Awake()
        {
            Application.targetFrameRate = 60;
            _cam = Camera.main;
            if (_cam == null)
            {
                var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
                _cam = camGo.AddComponent<Camera>();
            }
            _cam.fieldOfView = 38f;
            _cam.clearFlags = CameraClearFlags.SolidColor;
            _cam.backgroundColor = Greybox.Hex(0x1B1530);
            if (FindAnyObjectByType<Light>() == null)
            {
                var l = new GameObject("Sol").AddComponent<Light>();
                l.type = LightType.Directional;
                l.transform.rotation = Quaternion.Euler(55f, -30f, 0f);
                l.intensity = 1.2f;
            }
            _rig = new CameraRig(_cam);
            var audio = gameObject.AddComponent<AudioSource>();
            _fx = new FxDirector(transform, audio, Sounds);
            if (GetComponent<DebugHud>() == null) gameObject.AddComponent<DebugHud>();
            if (GetComponent<GameMenu>() == null) gameObject.AddComponent<GameMenu>();
        }

        // ── Configuración ────────────────────────────────────
        /// <summary>Mínimo y máximo de jugadores (humanos + bots) por modo.</summary>
        public static void PlayerRange(GameMode mode, out int min, out int max)
        {
            switch (mode)
            {
                case GameMode.LataCaliente: min = 3; max = 6; break;
                case GameMode.LaReta: min = 2; max = 8; break;
                case GameMode.RetoDelDia: min = 4; max = 4; break;
                default: min = 2; max = 6; break;
            }
        }

        /// <summary>Ajusta los bots para que el total quede dentro del rango del modo.</summary>
        public void ClampBots()
        {
            PlayerRange(GameMode, out int min, out int max);
            Bots = Mathf.Clamp(Bots, Mathf.Max(0, min - Humans), Mathf.Max(0, max - Humans));
        }

        string ResolveArena()
        {
            if (ArenaId == "aleatorio" || System.Array.FindIndex(ArenaLibrary.All, a => a.Id == ArenaId) < 0)
                return ArenaLibrary.All[Random.Range(0, ArenaLibrary.All.Length)].Id; // elección de menú, no de la simulación
            return ArenaId;
        }

        /// <summary>Copia propia del tuning (el Reto del Día lo modifica con la regla del día).</summary>
        Core.Tuning CloneTuning()
            => Tuning != null ? JsonUtility.FromJson<Core.Tuning>(JsonUtility.ToJson(Tuning.Values)) : Core.Tuning.Default();

        PlayerSlot LocalSlot()
        {
            var eq = ProgressStore.Data.Equipped;
            return new PlayerSlot { Name = PlayerName, Species = eq.Species, Shirt = DesignLibrary.PackOutfit(eq.Shirt, eq.Cap) };
        }

        MatchConfig BuildOfflineConfig(int humans)
        {
            if (GameMode == GameMode.RetoDelDia) GameMode = GameMode.Gorrazo; // el reto se arranca con StartReto()
            Humans = Mathf.Clamp(humans, 1, 2);
            ClampBots();
            var cfg = new MatchConfig
            {
                ArenaId = ResolveArena(), PointsToWin = PointsToWin, Seed = (uint)System.Environment.TickCount,
                Mode = GameMode, Rules = (Rules ?? new Rules()).Clone(), Tuning = CloneTuning(),
            };
            int sp = ProgressStore.Data.Equipped.Species;
            cfg.Slots.Add(LocalSlot());
            for (int i = 1; i < Humans; i++)
                cfg.Slots.Add(new PlayerSlot { Name = $"J{i + 1}", Species = (sp + i) % ProgressRules.SpeciesNames.Length, Shirt = DesignLibrary.ShirtForGuest(i) });
            for (int i = 0; i < Bots; i++)
            {
                int idx = Humans + i;
                cfg.Slots.Add(new PlayerSlot { Name = $"Bot {i + 1}", IsBot = true, Difficulty = Difficulty, Species = (sp + idx) % ProgressRules.SpeciesNames.Length, Shirt = DesignLibrary.ShirtForGuest(idx) });
            }
            while (cfg.Slots.Count < 2) cfg.Slots.Add(new PlayerSlot { Name = "Bot", IsBot = true, Difficulty = Difficulty, Species = 5, Shirt = DesignLibrary.ShirtForGuest(5) });
            return cfg;
        }

        // ── Arranques ────────────────────────────────────────
        public void StartOffline(int humans = 1)
        {
            StopAll();
            var cfg = BuildOfflineConfig(humans);
            StartLocal(cfg, cfg.Slots.FindAll(s => !s.IsBot).Count);
            _lastWasReto = false;
        }

        /// <summary>Reto del Día: semilla AAAAMMDD (hora local), arena y regla salen de la semilla.</summary>
        public void StartReto()
        {
            StopAll();
            LastReto = DailyChallenge.For(System.DateTime.Now);
            var cfg = LastReto.BuildConfig(LocalSlot(), CloneTuning());
            for (int i = 1; i < cfg.Slots.Count; i++) cfg.Slots[i].Shirt = DesignLibrary.ShirtForGuest(i);
            StartLocal(cfg, 1);
            _lastWasReto = true;
        }

        void StartLocal(MatchConfig cfg, int humans)
        {
            SetupLocalInputs(humans);
            try { Sim = new MatchSimulation(cfg); }
            catch (System.ArgumentException e) { Status = e.Message; Sim = null; return; }
            _bots = new BotBrain[Sim.Players.Count];
            for (int i = 0; i < Sim.Players.Count; i++) if (Sim.Players[i].IsBot) _bots[i] = new BotBrain(Sim, i, Sim.Config.Slots[i].Difficulty);
            _inputs = new InputCommand[Sim.Players.Count];
            BindWorld();
            Mode = RunMode.Offline;
        }

        /// <summary>Otra partida con lo mismo (local o anfitrión).</summary>
        public void Rematch()
        {
            if (Mode == RunMode.Host) { StartHostedMatch(); return; }
            if (_lastWasReto) StartReto();
            else StartOffline(Mathf.Max(1, LocalInputs.Count));
        }

        public void HostLan()
        {
            StopAll();
            SetupLocalInputs(1);
            try { _transport = UdpTransport.Host(GamePort); }
            catch (SocketException e) { Status = $"No se pudo abrir el puerto {GamePort}: {e.Message}"; return; }
            if (GameMode == GameMode.RetoDelDia) GameMode = GameMode.Gorrazo; // el reto es de un jugador
            Host = new HostSession(_transport, PlayerName);
            Host.MaxPlayers = GameMode == GameMode.LaReta ? 8 : 6;
            var me = LocalSlot();
            Host.Lobby[0].Species = me.Species;
            Host.Lobby[0].Shirt = me.Shirt; // viaja en el StartMatch: los demás ven tu playera y tu gorra
            Host.Log += s => Status = s;
            Host.EventsThisTick += Dispatch;
            for (int i = 0; i < Bots && Host.Lobby.Count < Host.MaxPlayers; i++) AddHostBot();
            LanPlatform.AcquireMulticast(); // Android: sin esto no salen los anuncios por broadcast
            _beacon = new LanBeaconSender();
            Mode = RunMode.Host;
            Status = $"Sala abierta. Tu IP: {LocalIp()}  puerto {GamePort}";
        }

        public void AddHostBot()
        {
            if (Host == null || Host.Lobby.Count >= Host.MaxPlayers) return;
            int n = 1; foreach (var p in Host.Lobby) if (p.IsBot) n++;
            Host.AddBot($"Bot {n}", Difficulty);
            var bot = Host.Lobby[Host.Lobby.Count - 1];
            bot.Shirt = DesignLibrary.ShirtForGuest(Host.Lobby.Count - 1);
        }

        /// <summary>Quita el último bot (solo si es el último de la lista, para no mover los índices de los humanos).</summary>
        public void RemoveHostBot()
        {
            if (Host == null || Sim != null || Host.Lobby.Count == 0) return;
            if (Host.Lobby[Host.Lobby.Count - 1].IsBot) Host.Lobby.RemoveAt(Host.Lobby.Count - 1);
        }

        public void StartHostedMatch()
        {
            if (Host == null) return;
            var cfg = Host.BuildConfig(ResolveArena(), PointsToWin, (uint)System.Environment.TickCount, GameMode, Rules);
            cfg.Tuning = CloneTuning();
            PlayerRange(GameMode, out int min, out int max);
            if (cfg.Slots.Count < min || cfg.Slots.Count > max) { Status = $"{ModeName(GameMode)} es de {min} a {max} jugadores (hay {cfg.Slots.Count})."; return; }
            try { Host.StartMatch(cfg); }
            catch (System.ArgumentException e) { Status = e.Message; return; }
            Sim = Host.Sim;
            BindWorld();
        }

        public void BrowseLan()
        {
            StopAll();
            LanPlatform.AcquireMulticast();
            try { _listener = new LanBeaconListener(); }
            catch (SocketException e) { Status = $"No se pudo escuchar salas: {e.Message}"; }
            Mode = RunMode.Menu;
        }

        public bool Browsing => _listener != null;

        public void JoinLan(string ip, int port)
        {
            var keepListener = _listener;
            _listener = null;
            StopAll();
            keepListener?.Dispose();
            SetupLocalInputs(1);
            try { _transport = UdpTransport.Connect(ip, port); }
            catch (System.Exception e) { Status = $"No se pudo conectar a {ip}: {e.Message}"; return; }
            Client = new ClientSession(_transport, PlayerName);
            Client.MatchStarted += () => { Sim = Client.Sim; BindWorld(); };
            Client.EventsReceived += evs => { _clientEvents.Clear(); _clientEvents.AddRange(evs); Dispatch(_clientEvents); };
            Mode = RunMode.Client;
            Status = $"Conectando a {ip}:{port}...";
        }

        public void StopAll()
        {
            _world.Clear();
            _fx?.ClearSplats();
            Host?.Dispose(); Host = null;       // también cierra su transporte
            Client?.Dispose(); Client = null;
            _transport?.Dispose(); _transport = null;
            _beacon?.Dispose(); _beacon = null;
            _listener?.Dispose(); _listener = null;
            LanPlatform.ReleaseMulticast();
            Rooms = new List<RoomInfo>();
            Sim = null;
            Mode = RunMode.Menu;
            _acc = 0f;
        }

        void SetupLocalInputs(int humans)
        {
            LocalInputs.Clear();
            bool touch = Application.isMobilePlatform;
            int pads = UnityEngine.InputSystem.Gamepad.all.Count;
            int padIdx = 0;
            for (int i = 0; i < humans; i++)
            {
                LocalInputReader r;
                if (i == 0) r = touch && pads == 0 ? new LocalInputReader(InputDevicePreset.Touch) : (touch ? new LocalInputReader(InputDevicePreset.Gamepad, padIdx++) : new LocalInputReader(InputDevicePreset.KeyboardWASD));
                else if (padIdx < pads) r = new LocalInputReader(InputDevicePreset.Gamepad, padIdx++);
                else r = new LocalInputReader(InputDevicePreset.KeyboardIJKL);
                int idx = i;
                r.AimCamera = _cam;
                r.PlayerWorldPos = () => _world.PlayerPosition(idx == 0 ? LocalPlayerIndex : idx);
                LocalInputs.Add(r);
            }
        }

        void BindWorld()
        {
            _fx.ClearSplats();
            _world.Bind(Sim, OutfitFor);
            _acc = 0f;
        }

        /// <summary>Playera y gorra de cada jugador, a partir de PlayerSlot.Shirt ("playera|gorra").</summary>
        Outfit OutfitFor(int i)
        {
            if (Sim == null || i < 0 || i >= Sim.Config.Slots.Count) return default;
            DesignLibrary.UnpackOutfit(Sim.Config.Slots[i].Shirt, out var shirt, out var cap);
            return new Outfit { Shirt = DesignLibrary.Shirt(shirt).Texture, Cap = DesignLibrary.Cap(cap) };
        }

        // ── Bucle ────────────────────────────────────────────
        void Update()
        {
            float dt = Time.deltaTime;
            foreach (var r in LocalInputs) r.Sample();

            switch (Mode)
            {
                case RunMode.Menu:
                    if (_listener != null) Rooms = _listener.Update(dt);
                    break;
                case RunMode.Host:
                    Host.Poll(dt);
                    _beacon?.Update(dt, RoomName, GamePort, Host.Lobby.Count, Host.MaxPlayers);
                    if (Sim != null) RunTicks(dt, () => Host.Tick(LocalInputs[0].ConsumeTick()));
                    break;
                case RunMode.Client:
                    Client.Poll(dt);
                    if (Client.RejectReason != null) Status = Client.RejectReason;
                    else if (Client.MyIndex >= 0 && Client.Sim == null) Status = $"En la sala ({Client.LobbyNames.Count} jugadores). Esperando al anfitrión...";
                    if (Client.MyIndex >= 0) RunTicks(dt, () => Client.SendInput(LocalInputs[0].ConsumeTick()));
                    break;
                case RunMode.Offline:
                    RunTicks(dt, OfflineTick);
                    break;
            }

            _world.Sync(dt);
            _fx.Tick(dt);
            if (Sim != null) _rig.Frame(Sim.Arena.Def, _fx.Shake);
        }

        void RunTicks(float dt, System.Action tick)
        {
            float scale = 1f;
            if (Sim != null && Sim.SlowMo > 0f)
            {
                scale = Sim.T.Round.SlowMoScale;
                if (Mode != RunMode.Client) Sim.SlowMo -= dt; // en cliente lo controla el snapshot
            }
            _acc += dt * scale;
            int steps = 0;
            while (_acc >= MatchSimulation.Dt && steps < 5)
            {
                tick();
                _acc -= MatchSimulation.Dt;
                steps++;
            }
            if (steps == 5) _acc = 0f;
        }

        void OfflineTick()
        {
            for (int i = 0; i < Sim.Players.Count; i++)
            {
                if (_bots[i] != null) _inputs[i] = _bots[i].Think(MatchSimulation.Dt);
                else _inputs[i] = i < LocalInputs.Count ? LocalInputs[i].ConsumeTick() : default;
            }
            Sim.Step(_inputs);
            if (Sim.Events.Count > 0) Dispatch(Sim.Events);
        }

        void Dispatch(IReadOnlyList<SimEvent> events)
        {
            _fx.Handle(events);
            SimEvents?.Invoke(events);
        }

        void OnApplicationPause(bool paused) { if (paused) ProgressStore.Save(); }
        void OnDestroy() => StopAll();
        void OnApplicationQuit() { ProgressStore.Save(); StopAll(); }

        public static string ModeName(GameMode m)
        {
            switch (m)
            {
                case GameMode.LataCaliente: return "Lata Caliente";
                case GameMode.LaReta: return "La Reta";
                case GameMode.RetoDelDia: return "Reto del Día";
                default: return "¡Gorrazo!";
            }
        }

        public static string ModeDescription(GameMode m)
        {
            switch (m)
            {
                case GameMode.LataCaliente: return "Una lata se pega a alguien y explota entre 10 y 20 s. Gorrazos y golpes no noquean: pasan la lata. 3 a 6 jugadores.";
                case GameMode.LaReta: return "1 contra 1: el ganador se queda y entra el siguiente de la fila. Gana quien encadene 3 o tenga más victorias a las 10 rondas.";
                case GameMode.RetoDelDia: return "Tú contra 3 bots difíciles. Misma arena y regla para todos hoy.";
                default: return "Último en pie gana la ronda.";
            }
        }

        // ── IP local (la sala la muestra en grande) ───────────
        /// <summary>IPv4 de este dispositivo en la red local (Wi‑Fi o hotspot), las privadas primero.</summary>
        public static List<string> LocalIps()
        {
            var list = new List<string>();
            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus != OperationalStatus.Up || ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
                    foreach (var ua in ni.GetIPProperties().UnicastAddresses)
                    {
                        var a = ua.Address;
                        if (a.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(a) && !list.Contains(a.ToString())) list.Add(a.ToString());
                    }
                }
            }
            catch (System.Exception) { /* algunas plataformas no dejan listar interfaces */ }
            if (list.Count == 0)
            {
                try
                {
                    // truco: "conectar" un socket UDP no manda nada, pero dice qué IP local usaría
                    using (var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                    {
                        s.Connect("10.255.255.255", 9);
                        if (s.LocalEndPoint is IPEndPoint ep && !IPAddress.IsLoopback(ep.Address)) list.Add(ep.Address.ToString());
                    }
                }
                catch (System.Exception) { }
            }
            if (list.Count == 0)
            {
                try
                {
                    foreach (var a in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                        if (a.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(a)) list.Add(a.ToString());
                }
                catch (System.Exception) { }
            }
            list.Sort((x, y) => Rank(x).CompareTo(Rank(y)));
            return list;
        }

        static int Rank(string ip)
        {
            if (ip.StartsWith("192.168.")) return 0;
            if (ip.StartsWith("10.")) return 1;
            if (ip.StartsWith("172.")) return 2;
            if (ip.StartsWith("169.254.")) return 9; // sin DHCP: casi nunca sirve
            return 5;
        }

        public static string LocalIp()
        {
            var ips = LocalIps();
            return ips.Count > 0 ? ips[0] : "127.0.0.1";
        }
    }
}
