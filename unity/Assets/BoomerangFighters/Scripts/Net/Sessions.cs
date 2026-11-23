// ─────────────────────────────────────────────────────────────
// SESIONES DE RED (host-autoritativo).
//  • El HOST corre la simulación real, aplica las entradas de todos
//    (locales, bots y remotas) y manda snapshots a 30 Hz.
//  • El CLIENTE manda sus entradas cada tick y dibuja lo que el host dice.
// En LAN la latencia es de 2–20 ms, así que no hace falta predicción.
// Para online (Etapa 2) se agrega predicción del propio movimiento y
// "lag compensation" en el host, sin cambiar este contrato.
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.IO;
using BoomerangFighters.Core;

namespace BoomerangFighters.Net
{
    public sealed class LobbyPlayer
    {
        public int PeerId = -1;      // -1 = local del host
        public string Name;
        public bool IsBot;
        public BotDifficulty Difficulty = BotDifficulty.Normal;
        public int Species;
        public string Shirt = "basica-blanca";
        public float LastHeard;
    }

    // ── HOST ─────────────────────────────────────────────────
    public sealed class HostSession : IDisposable
    {
        public readonly ITransport Transport;
        public readonly List<LobbyPlayer> Lobby = new List<LobbyPlayer>();
        public MatchSimulation Sim { get; private set; }
        public event Action<string> Log;
        public event Action<IReadOnlyList<SimEvent>> EventsThisTick;
        public int MaxPlayers = 6;

        readonly Dictionary<int, int> _playerByPeer = new Dictionary<int, int>();
        readonly Dictionary<int, RemoteInputState> _remote = new Dictionary<int, RemoteInputState>();
        readonly List<BotBrain> _bots = new List<BotBrain>();
        readonly InputCommand[] _inputs = new InputCommand[MatchSimulation.MaxPlayers];
        readonly Queue<(int seq, SimEvent[] events)> _eventBatches = new Queue<(int, SimEvent[])>();
        readonly List<SimEvent> _pendingEvents = new List<SimEvent>();
        int _snapshotSeq;
        float _clock;

        sealed class RemoteInputState
        {
            public InputCommand Latest;
            public InputButtons PendingEdges;
            public int LastSeq = -1;
        }

        public HostSession(ITransport transport, string hostName)
        {
            Transport = transport;
            Transport.Received += OnReceived;
            Lobby.Add(new LobbyPlayer { Name = hostName });
        }

        public void AddBot(string name, BotDifficulty d) => Lobby.Add(new LobbyPlayer { Name = name, IsBot = true, Difficulty = d, Species = Lobby.Count });

        void OnReceived(int peer, byte[] data)
        {
            if (data.Length == 0) return;
            using (var r = new BinaryReader(new MemoryStream(data)))
            {
                var type = (MsgType)r.ReadByte();
                switch (type)
                {
                    case MsgType.Hello: HandleHello(peer, r); break;
                    case MsgType.Input: HandleInput(peer, r); break;
                    case MsgType.Bye: Log?.Invoke($"{Transport.Describe(peer)} salió."); break;
                }
            }
        }

        void HandleHello(int peer, BinaryReader r)
        {
            byte ver = r.ReadByte();
            string name = r.ReadString();
            if (ver != Protocol.Version) { Transport.Send(peer, Wire.Build(MsgType.Reject, w => w.Write("Versión distinta: actualiza el juego."))); return; }
            if (_playerByPeer.TryGetValue(peer, out int existing))
            {
                // reintento de Hello: volver a mandar la bienvenida
                Transport.Send(peer, Wire.Build(MsgType.Welcome, w => w.Write((byte)existing)));
                if (Sim != null) SendStart(peer, existing);
                return;
            }
            if (Sim != null || Lobby.Count >= MaxPlayers) { Transport.Send(peer, Wire.Build(MsgType.Reject, w => w.Write(Sim != null ? "La partida ya empezó." : "Sala llena."))); return; }
            int idx = Lobby.Count;
            Lobby.Add(new LobbyPlayer { PeerId = peer, Name = name, Species = idx, LastHeard = _clock });
            _playerByPeer[peer] = idx;
            _remote[idx] = new RemoteInputState();
            Transport.Send(peer, Wire.Build(MsgType.Welcome, w => w.Write((byte)idx)));
            BroadcastLobby();
            Log?.Invoke($"{name} entró desde {Transport.Describe(peer)}.");
        }

        void HandleInput(int peer, BinaryReader r)
        {
            if (!_playerByPeer.TryGetValue(peer, out int idx)) return;
            Lobby[idx].LastHeard = _clock;
            var st = _remote[idx];
            int firstSeq = r.ReadInt32();
            int count = r.ReadByte();
            var buf = r.ReadBytes(count * InputCommand.WireSize);
            for (int i = 0; i < count; i++)
            {
                int seq = firstSeq + i;
                if (seq <= st.LastSeq) continue; // ya lo procesamos
                var cmd = InputCommand.Read(buf, i * InputCommand.WireSize);
                st.PendingEdges |= cmd.Buttons & (InputButtons.ThrowPressed | InputButtons.ThrowReleased | InputButtons.MeleePressed | InputButtons.DashPressed);
                st.Latest = cmd;
                st.LastSeq = seq;
            }
        }

        void BroadcastLobby()
        {
            var msg = Wire.Build(MsgType.Lobby, w =>
            {
                w.Write((byte)Lobby.Count);
                foreach (var p in Lobby) { w.Write(p.Name); w.Write(p.IsBot); }
            });
            foreach (var p in Lobby) if (p.PeerId >= 0) Transport.Send(p.PeerId, msg);
        }

        public MatchConfig BuildConfig(string arenaId, int pointsToWin, uint seed)
        {
            var cfg = new MatchConfig { ArenaId = arenaId, PointsToWin = pointsToWin, Seed = seed };
            foreach (var p in Lobby) cfg.Slots.Add(new PlayerSlot { Name = p.Name, IsBot = p.IsBot, Difficulty = p.Difficulty, Species = p.Species, Shirt = p.Shirt });
            return cfg;
        }

        public void StartMatch(MatchConfig cfg)
        {
            Sim = new MatchSimulation(cfg);
            _bots.Clear();
            for (int i = 0; i < cfg.Slots.Count; i++)
                _bots.Add(cfg.Slots[i].IsBot ? new BotBrain(Sim, i, cfg.Slots[i].Difficulty) : null);
            foreach (var kv in _playerByPeer) SendStart(kv.Key, kv.Value);
            Log?.Invoke($"Partida iniciada: {cfg.ArenaId}, {cfg.Slots.Count} jugadores.");
        }

        void SendStart(int peer, int idx)
        {
            var cfg = Sim.Config;
            Transport.Send(peer, Wire.Build(MsgType.StartMatch, w => { w.Write((byte)idx); Wire.WriteConfig(w, cfg); }));
        }

        /// <summary>Recibe paquetes. Llamar cada cuadro.</summary>
        public void Poll(float dt)
        {
            _clock += dt;
            Transport.Poll();
        }

        /// <summary>Avanza un tick de simulación. localInput = entrada del jugador del host (índice 0).</summary>
        public void Tick(InputCommand localInput)
        {
            if (Sim == null) return;
            for (int i = 0; i < Sim.Players.Count; i++)
            {
                if (i == 0) { _inputs[i] = localInput; continue; }
                if (_bots[i] != null) { _inputs[i] = _bots[i].Think(MatchSimulation.Dt); continue; }
                if (_remote.TryGetValue(i, out var st))
                {
                    var cmd = st.Latest.WithoutEdges();
                    cmd.Buttons |= st.PendingEdges;
                    st.PendingEdges = InputButtons.None;
                    _inputs[i] = cmd;
                }
                else _inputs[i] = default;
            }
            Sim.Step(_inputs);
            if (Sim.Events.Count > 0)
            {
                _pendingEvents.AddRange(Sim.Events);
                EventsThisTick?.Invoke(Sim.Events);
            }
            if (Sim.Tick % Protocol.SnapshotEveryTicks == 0) SendSnapshot();
        }

        void SendSnapshot()
        {
            _snapshotSeq++;
            _eventBatches.Enqueue((_snapshotSeq, _pendingEvents.ToArray()));
            _pendingEvents.Clear();
            while (_eventBatches.Count > Protocol.EventBatchRedundancy) _eventBatches.Dequeue();
            var msg = Wire.Build(MsgType.Snapshot, w =>
            {
                w.Write(_snapshotSeq);
                w.Write((byte)_eventBatches.Count);
                foreach (var (seq, events) in _eventBatches)
                {
                    w.Write(seq);
                    w.Write((byte)Math.Min(255, events.Length));
                    for (int i = 0; i < events.Length && i < 255; i++) MatchSimulation.WriteEvent(w, events[i]);
                }
                Sim.WriteSnapshot(w);
            });
            foreach (var p in Lobby) if (p.PeerId >= 0) Transport.Send(p.PeerId, msg);
        }

        public IEnumerable<LobbyPlayer> TimedOut()
        {
            foreach (var p in Lobby) if (p.PeerId >= 0 && _clock - p.LastHeard > Protocol.PeerTimeoutSeconds) yield return p;
        }

        public void Dispose()
        {
            var bye = Wire.Build(MsgType.Bye, null);
            foreach (var p in Lobby) if (p.PeerId >= 0) Transport.Send(p.PeerId, bye);
            Transport.Dispose();
        }
    }

    // ── CLIENTE ──────────────────────────────────────────────
    public sealed class ClientSession : IDisposable
    {
        public readonly ITransport Transport;
        public int MyIndex { get; private set; } = -1;
        public MatchSimulation Sim { get; private set; }
        public readonly List<string> LobbyNames = new List<string>();
        public string RejectReason { get; private set; }
        public int LastSnapshotSeq { get; private set; }
        public int SnapshotsReceived { get; private set; }
        public event Action<IReadOnlyList<SimEvent>> EventsReceived;
        public event Action MatchStarted;

        readonly string _name;
        readonly List<InputCommand> _history = new List<InputCommand>();
        readonly List<SimEvent> _eventBuffer = new List<SimEvent>();
        int _inputSeq = -1;
        int _lastEventBatch;
        float _helloTimer;

        public ClientSession(ITransport transport, string name)
        {
            Transport = transport;
            _name = name;
            Transport.Received += OnReceived;
            SendHello();
        }

        void SendHello() => Transport.Send(0, Wire.Build(MsgType.Hello, w => { w.Write(Protocol.Version); w.Write(_name); }));

        void OnReceived(int peer, byte[] data)
        {
            if (data.Length == 0) return;
            using (var r = new BinaryReader(new MemoryStream(data)))
            {
                var type = (MsgType)r.ReadByte();
                switch (type)
                {
                    case MsgType.Welcome: MyIndex = r.ReadByte(); break;
                    case MsgType.Reject: RejectReason = r.ReadString(); break;
                    case MsgType.Lobby:
                    {
                        LobbyNames.Clear();
                        int n = r.ReadByte();
                        for (int i = 0; i < n; i++) { LobbyNames.Add(r.ReadString()); r.ReadBoolean(); }
                        break;
                    }
                    case MsgType.StartMatch:
                    {
                        MyIndex = r.ReadByte();
                        var cfg = Wire.ReadConfig(r);
                        if (Sim == null || Sim.Config.Seed != cfg.Seed)
                        {
                            Sim = new MatchSimulation(cfg); // contenedor de estado (no se simula aquí)
                            _lastEventBatch = 0;
                            LastSnapshotSeq = 0;
                            MatchStarted?.Invoke();
                        }
                        break;
                    }
                    case MsgType.Snapshot: HandleSnapshot(r); break;
                    case MsgType.Bye: RejectReason = "El anfitrión cerró la sala."; break;
                }
            }
        }

        void HandleSnapshot(BinaryReader r)
        {
            if (Sim == null) return;
            int seq = r.ReadInt32();
            int batches = r.ReadByte();
            _eventBuffer.Clear();
            for (int b = 0; b < batches; b++)
            {
                int bseq = r.ReadInt32();
                int count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    var e = MatchSimulation.ReadEvent(r);
                    if (bseq > _lastEventBatch) _eventBuffer.Add(e);
                }
                if (bseq > _lastEventBatch && b == batches - 1) _lastEventBatch = bseq;
            }
            if (_eventBuffer.Count > 0) EventsReceived?.Invoke(_eventBuffer);
            if (seq <= LastSnapshotSeq) return; // llegó tarde: ignorar estado viejo
            Sim.ReadSnapshot(r);
            LastSnapshotSeq = seq;
            SnapshotsReceived++;
        }

        /// <summary>Llamar cada cuadro: recibe paquetes y reintenta el saludo si no hay respuesta.</summary>
        public void Poll(float dt)
        {
            Transport.Poll();
            if (MyIndex < 0 && RejectReason == null)
            {
                _helloTimer += dt;
                if (_helloTimer > 0.25f) { _helloTimer = 0f; SendHello(); }
            }
        }

        /// <summary>Manda la entrada de este tick (con las anteriores repetidas por si se pierde un paquete).</summary>
        public void SendInput(InputCommand cmd)
        {
            if (MyIndex < 0) return;
            _inputSeq++;
            _history.Add(cmd);
            if (_history.Count > Protocol.InputRedundancy) _history.RemoveAt(0);
            int first = _inputSeq - _history.Count + 1;
            var msg = Wire.Build(MsgType.Input, w =>
            {
                w.Write(first);
                w.Write((byte)_history.Count);
                var buf = new byte[InputCommand.WireSize];
                foreach (var c in _history) { c.Write(buf, 0); w.Write(buf); }
            });
            Transport.Send(0, msg);
        }

        public void Dispose()
        {
            Transport.Send(0, Wire.Build(MsgType.Bye, null));
            Transport.Dispose();
        }
    }
}
