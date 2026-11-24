using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BoomerangFighters.Core;
using BoomerangFighters.Net;
using Xunit;

namespace BoomerangFighters.Tests
{
    public class NetworkTests
    {
        /// <summary>Corre host + cliente en el mismo hilo, avanzando el "reloj" de la red simulada.</summary>
        static void Pump(HostSession host, ClientSession client, LoopbackTransport ht, LoopbackTransport ct, int ticks, System.Func<int, InputCommand> clientInput = null)
        {
            for (int t = 0; t < ticks; t++)
            {
                ht.Advance(MatchSimulation.Dt); ct.Advance(MatchSimulation.Dt);
                host.Poll(MatchSimulation.Dt);
                client.Poll(MatchSimulation.Dt);
                client.SendInput(clientInput != null ? clientInput(t) : default);
                host.Tick(default);
            }
        }

        [Fact]
        public void ClienteEntraALaSalaYRecibeLaPartida()
        {
            var (ht, ct) = LoopbackTransport.Pair(latency: 0.05f);
            var host = new HostSession(ht, "Anfitrión");
            var client = new ClientSession(ct, "Invitada");
            Pump(host, client, ht, ct, 30);
            Assert.Equal(1, client.MyIndex);
            Assert.Equal(new[] { "Anfitrión", "Invitada" }, client.LobbyNames);

            host.AddBot("Bot", BotDifficulty.Normal);
            host.StartMatch(host.BuildConfig("callejon", 3, 2026));
            var events = new List<SimEvent>();
            client.EventsReceived += evs => events.AddRange(evs);
            Pump(host, client, ht, ct, 200);
            Assert.NotNull(client.Sim);
            Assert.True(client.SnapshotsReceived > 50);
            Assert.Contains(events, e => e.Type == SimEventType.Countdown);
            // el estado del cliente va ~50 ms detrás del host, pero es el mismo juego
            Assert.Equal(host.Sim.Players.Count, client.Sim.Players.Count);
            Assert.InRange(host.Sim.Tick - client.Sim.Tick, 0, 12);
        }

        [Fact]
        public void LasEntradasDelClienteMuevenASuMunequitoEnElHost()
        {
            var (ht, ct) = LoopbackTransport.Pair(latency: 0.03f);
            var host = new HostSession(ht, "Anfitrión");
            var client = new ClientSession(ct, "Invitado");
            Pump(host, client, ht, ct, 20);
            host.StartMatch(host.BuildConfig("azotea", 3, 5));
            Pump(host, client, ht, ct, 200); // pasa la cuenta regresiva
            Assert.Equal(MatchPhase.Play, host.Sim.Phase);
            var before = host.Sim.Players[1].Pos;
            Pump(host, client, ht, ct, 40, t => new InputCommand { Move = new Vec2(0f, -1f) });
            var after = host.Sim.Players[1].Pos;
            Assert.True(after.y < before.y - 1f, $"No se movió: {before} → {after}");
        }

        [Fact]
        public void UnLanzamientoNoSePierdeAunqueSePierdanPaquetes()
        {
            var (ht, ct) = LoopbackTransport.Pair(latency: 0.04f, loss: 0.3f);
            var host = new HostSession(ht, "Anfitrión");
            var client = new ClientSession(ct, "Invitado");
            Pump(host, client, ht, ct, 180);
            Assert.Equal(1, client.MyIndex);
            host.StartMatch(host.BuildConfig("callejon", 3, 11));
            Pump(host, client, ht, ct, 220);
            Assert.Equal(MatchPhase.Play, host.Sim.Phase);
            var throwEvents = new List<SimEvent>();
            host.EventsThisTick += evs => throwEvents.AddRange(evs.Where(e => e.Type == SimEventType.Throw && e.A == 1));
            Pump(host, client, ht, ct, 30, t => t == 0
                ? new InputCommand { Aim = new Vec2(1, 0), Buttons = InputButtons.HasAim | InputButtons.ThrowPressed | InputButtons.ThrowReleased }
                : default);
            Assert.Single(throwEvents);
        }

        [Fact]
        public void LanRealPorUdpEnLocalhost()
        {
            using var hostT = UdpTransport.Host(0);
            int port = hostT.LocalPort;
            var host = new HostSession(hostT, "Host UDP");
            host.AddBot("Bot", BotDifficulty.Facil);
            var clientT = UdpTransport.Connect("127.0.0.1", port);
            var client = new ClientSession(clientT, "Cliente UDP");
            for (int i = 0; i < 200 && client.MyIndex < 0; i++) { host.Poll(0.016f); client.Poll(0.016f); Thread.Sleep(2); }
            Assert.Equal(2, client.MyIndex);
            host.StartMatch(host.BuildConfig("museo", 3, 9));
            for (int i = 0; i < 240; i++)
            {
                host.Poll(0.016f); client.Poll(0.016f);
                client.SendInput(new InputCommand { Move = new Vec2(1f, 0f) });
                host.Tick(default);
                if (i % 20 == 0) Thread.Sleep(1);
            }
            for (int i = 0; i < 20; i++) { client.Poll(0.016f); Thread.Sleep(2); }
            Assert.NotNull(client.Sim);
            Assert.True(client.SnapshotsReceived > 60, $"snapshots: {client.SnapshotsReceived}");
            Assert.Equal(host.Sim.Players[2].Pos.x, client.Sim.Players[2].Pos.x, 2);
        }

        [Fact]
        public void LaConfigViajaConModoYReglas()
        {
            Assert.Equal(3, Protocol.Version); // v0.3: snapshot v3 (caja DROP), eventos y escenarios nuevos
            var cfg = new MatchConfig
            {
                ArenaId = "azotea", PointsToWin = 7, Seed = 20261123u, Mode = GameMode.LaReta,
                Rules = new Rules { SuddenDeath = true, Shove = false, DeflectReturn = true, GraffitiWall = false, PowerUps = false },
            };
            for (int i = 0; i < 5; i++) cfg.Slots.Add(new PlayerSlot { Name = $"J{i}", IsBot = i > 2, Difficulty = BotDifficulty.Dificil, Species = i });
            var bytes = Wire.Build(MsgType.StartMatch, w => Wire.WriteConfig(w, cfg));
            var r = new System.IO.BinaryReader(new System.IO.MemoryStream(bytes));
            Assert.Equal((byte)MsgType.StartMatch, r.ReadByte());
            var back = Wire.ReadConfig(r);
            Assert.Equal(GameMode.LaReta, back.Mode);
            Assert.Equal(cfg.Rules.ToBits(), back.Rules.ToBits());
            Assert.False(back.Rules.Shove); Assert.False(back.Rules.GraffitiWall); Assert.False(back.Rules.PowerUps);
            Assert.True(back.Rules.SuddenDeath); Assert.True(back.Rules.DeflectReturn);
            Assert.Equal("azotea", back.ArenaId); Assert.Equal(7, back.PointsToWin); Assert.Equal(20261123u, back.Seed);
            Assert.Equal(5, back.Slots.Count);
        }

        [Fact]
        public void ElClienteRecibeElModoYJuegaLataCaliente()
        {
            var (ht, ct) = LoopbackTransport.Pair(latency: 0.03f);
            var host = new HostSession(ht, "Anfitrión");
            var client = new ClientSession(ct, "Invitado");
            Pump(host, client, ht, ct, 20);
            host.AddBot("Bot", BotDifficulty.Normal);
            host.StartMatch(host.BuildConfig("callejon", 3, 31, GameMode.LataCaliente, new Rules { DeflectReturn = false }));
            Pump(host, client, ht, ct, 260); // pasa la cuenta regresiva: ya hay lata
            Assert.NotNull(client.Sim);
            Assert.Equal(GameMode.LataCaliente, client.Sim.Mode);
            Assert.False(client.Sim.Rules.DeflectReturn);
            Assert.True(host.Sim.HotCanHolder >= 0);
            Assert.Equal(host.Sim.HotCanHolder, client.Sim.HotCanHolder);
        }

        [Fact]
        public void ElAnuncioLanSeVeEnLaLista()
        {
            int beaconPort = 47000 + (System.Environment.ProcessId % 900);
            using var listener = new LanBeaconListener(beaconPort);
            using var sender = new LanBeaconSender(beaconPort, "127.0.0.1");
            List<RoomInfo> rooms = new List<RoomInfo>();
            for (int i = 0; i < 50 && rooms.Count == 0; i++)
            {
                sender.Update(1.1, "Sala de Chris", 7777, 2, 6);
                Thread.Sleep(10);
                rooms = listener.Update(0.01);
            }
            Assert.Single(rooms);
            Assert.Equal("Sala de Chris", rooms[0].Name);
            Assert.True(RoomInfo.TryParseJoinUri(rooms[0].ToJoinUri(), out var ip, out var p));
            Assert.Equal("127.0.0.1", ip);
            Assert.Equal(7777, p);
        }
    }
}
