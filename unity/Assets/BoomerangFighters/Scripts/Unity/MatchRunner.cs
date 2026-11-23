// ─────────────────────────────────────────────────────────────
// MATCH RUNNER: el único MonoBehaviour que necesitas en la escena.
// Agrega este componente a un GameObject vacío, dale Play y tendrás:
//   • Partida contra bots (1–2 jugadores locales con teclado/control/táctil)
//   • Crear sala LAN (host) o buscar/unirse a una sala en el mismo Wi‑Fi
// Corre la simulación a 60 ticks fijos por segundo (nunca usa
// Time.deltaTime dentro de las reglas) y dibuja con las vistas greybox.
// ─────────────────────────────────────────────────────────────
using System.Collections.Generic;
using System.Net;
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
        public string ArenaId = "callejon";
        [Range(1, 9)] public int PointsToWin = 5;
        [Range(0, 5)] public int Bots = 3;
        public BotDifficulty Difficulty = BotDifficulty.Normal;
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
        public readonly List<LocalInputReader> LocalInputs = new List<LocalInputReader>();
        /// <summary>Eventos de la simulación (en los 3 modos) para HUD, sonido, vibración, analítica.</summary>
        public event System.Action<IReadOnlyList<SimEvent>> SimEvents;

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
        }

        // ── Arranques ────────────────────────────────────────
        MatchConfig BuildOfflineConfig(int humans)
        {
            var cfg = new MatchConfig { ArenaId = ArenaId, PointsToWin = PointsToWin, Seed = (uint)System.Environment.TickCount, Tuning = Tuning != null ? Tuning.Values : Core.Tuning.Default() };
            for (int i = 0; i < humans; i++) cfg.Slots.Add(new PlayerSlot { Name = i == 0 ? PlayerName : $"J{i + 1}", Species = i });
            for (int i = 0; i < Bots; i++) cfg.Slots.Add(new PlayerSlot { Name = $"Bot {i + 1}", IsBot = true, Difficulty = Difficulty, Species = humans + i });
            if (cfg.Slots.Count < 2) cfg.Slots.Add(new PlayerSlot { Name = "Bot", IsBot = true, Difficulty = Difficulty, Species = 5 });
            return cfg;
        }

        public void StartOffline(int humans = 1)
        {
            StopAll();
            SetupLocalInputs(humans);
            Sim = new MatchSimulation(BuildOfflineConfig(humans));
            _bots = new BotBrain[Sim.Players.Count];
            for (int i = 0; i < Sim.Players.Count; i++) if (Sim.Players[i].IsBot) _bots[i] = new BotBrain(Sim, i, Sim.Config.Slots[i].Difficulty);
            _inputs = new InputCommand[Sim.Players.Count];
            BindWorld();
            Mode = RunMode.Offline;
        }

        public void HostLan()
        {
            StopAll();
            SetupLocalInputs(1);
            try { _transport = UdpTransport.Host(GamePort); }
            catch (SocketException e) { Status = $"No se pudo abrir el puerto {GamePort}: {e.Message}"; return; }
            Host = new HostSession(_transport, PlayerName);
            Host.Log += s => Status = s;
            Host.EventsThisTick += Dispatch;
            for (int i = 0; i < Bots; i++) Host.AddBot($"Bot {i + 1}", Difficulty);
            _beacon = new LanBeaconSender();
            Mode = RunMode.Host;
            Status = $"Sala abierta. Tu IP: {LocalIp()}  puerto {GamePort}";
        }

        public void StartHostedMatch()
        {
            if (Host == null) return;
            Host.StartMatch(Host.BuildConfig(ArenaId, PointsToWin, (uint)System.Environment.TickCount));
            Sim = Host.Sim;
            BindWorld();
        }

        public void BrowseLan()
        {
            StopAll();
            try { _listener = new LanBeaconListener(); }
            catch (SocketException e) { Status = $"No se pudo escuchar salas: {e.Message}"; }
            Mode = RunMode.Menu;
        }

        public void JoinLan(string ip, int port)
        {
            var keepListener = _listener;
            _listener = null;
            StopAll();
            keepListener?.Dispose();
            SetupLocalInputs(1);
            _transport = UdpTransport.Connect(ip, port);
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
            _world.Bind(Sim);
            _acc = 0f;
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

        void OnDestroy() => StopAll();
        void OnApplicationQuit() => StopAll();

        public static string LocalIp()
        {
            try
            {
                foreach (var a in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                    if (a.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(a)) return a.ToString();
            }
            catch (SocketException) { }
            return "127.0.0.1";
        }
    }
}
