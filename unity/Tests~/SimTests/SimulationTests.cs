using System.Collections.Generic;
using System.IO;
using System.Linq;
using BoomerangFighters.Core;
using Xunit;

namespace BoomerangFighters.Tests
{
    static class H
    {
        public static MatchConfig Config(int players, int bots = 0, string arena = "callejon", uint seed = 777, int points = 5)
        {
            var cfg = new MatchConfig { ArenaId = arena, Seed = seed, PointsToWin = points };
            for (int i = 0; i < players; i++) cfg.Slots.Add(new PlayerSlot { Name = $"P{i}", IsBot = i >= players - bots });
            return cfg;
        }

        public static InputCommand[] Idle(int n) => new InputCommand[n];

        public static void SkipCountdown(MatchSimulation sim)
        {
            var idle = Idle(sim.Players.Count);
            for (int i = 0; i < 400 && sim.Phase != MatchPhase.Play; i++) sim.Step(idle);
            Assert.Equal(MatchPhase.Play, sim.Phase);
            // pasar la invulnerabilidad de aparición
            for (int i = 0; i < 40; i++) sim.Step(idle);
        }

        /// <summary>Coloca jugadores en posiciones fijas en una zona sin obstáculos (franja superior del callejón).</summary>
        public static void Place(MatchSimulation sim, params Vec2[] pos)
        {
            for (int i = 0; i < sim.Players.Count; i++)
            {
                var p = sim.Players[i];
                if (i < pos.Length) { p.Pos = pos[i]; p.Vel = Vec2.Zero; }
                else { p.Pos = new Vec2(-10f + i, 6.2f); }
            }
        }

        public static InputCommand Throw(Vec2 aim) => new InputCommand
        {
            Aim = aim,
            Buttons = InputButtons.HasAim | InputButtons.ThrowPressed | InputButtons.ThrowReleased,
        };

        public static List<SimEvent> Run(MatchSimulation sim, int ticks, System.Func<int, InputCommand[]> inputs = null)
        {
            var all = new List<SimEvent>();
            for (int t = 0; t < ticks; t++)
            {
                sim.Step(inputs != null ? inputs(t) : Idle(sim.Players.Count));
                all.AddRange(sim.Events);
            }
            return all;
        }
    }

    public class MechanicsTests
    {
        [Fact]
        public void LaGorraRegresaALaManoDeQuienLaLanza()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            // carril libre: la gorra regresa en curva y no debe toparse con contenedores
            H.Place(sim, new Vec2(-1f, -5.5f), new Vec2(10f, 5.5f));
            var p0 = sim.Players[0];
            var ev = H.Run(sim, 1, t => new[] { H.Throw(new Vec2(1, 0)), default });
            Assert.False(p0.HasCap);
            Assert.Contains(ev, e => e.Type == SimEventType.Throw && e.A == 0);
            var after = H.Run(sim, 180);
            Assert.True(p0.HasCap, $"La gorra no regresó: modo {p0.Cap.Mode} en {p0.Cap.Pos}");
            Assert.Contains(after, e => e.Type == SimEventType.Catch && e.A == 0);
        }

        [Fact]
        public void UnGorrazoNoqueaAlRival()
        {
            var sim = new MatchSimulation(H.Config(3));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-8f, -5.5f), new Vec2(-3f, -5.5f), new Vec2(9f, 5.5f));
            var ev = H.Run(sim, 60, t => t == 0 ? new[] { H.Throw(new Vec2(1, 0)), default, default } : H.Idle(3));
            Assert.False(sim.Players[1].Alive);
            Assert.Equal(1, sim.Players[0].Kos);
            Assert.Contains(ev, e => e.Type == SimEventType.KO && e.A == 1 && e.B == 0);
        }

        [Fact]
        public void ElParcheBordadoAguantaUnGolpe()
        {
            var sim = new MatchSimulation(H.Config(3));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-8f, -5.5f), new Vec2(-3f, -5.5f), new Vec2(9f, 5.5f));
            sim.Players[1].Shield = true;
            var ev = H.Run(sim, 40, t => t == 0 ? new[] { H.Throw(new Vec2(1, 0)), default, default } : H.Idle(3));
            Assert.True(sim.Players[1].Alive);
            Assert.False(sim.Players[1].Shield);
            Assert.Contains(ev, e => e.Type == SimEventType.ShieldBreak && e.A == 1);
        }

        [Fact]
        public void ElGolpeCuerpoACuerpoNoquea()
        {
            var sim = new MatchSimulation(H.Config(3));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-6f, -5.5f), new Vec2(-4.9f, -5.5f), new Vec2(9f, 5.5f));
            var melee = new InputCommand { Aim = new Vec2(1, 0), Buttons = InputButtons.HasAim | InputButtons.MeleePressed };
            H.Run(sim, 20, t => t == 0 ? new[] { melee, default, default } : H.Idle(3));
            Assert.False(sim.Players[1].Alive);
        }

        [Fact]
        public void DosGorrasQueChocanCaenAlPiso()
        {
            var sim = new MatchSimulation(H.Config(3));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-7f, -5.5f), new Vec2(3f, -5.5f), new Vec2(9f, 5.5f));
            var ev = H.Run(sim, 30, t => t == 0 ? new[] { H.Throw(new Vec2(1, 0)), H.Throw(new Vec2(-1, 0)), default } : H.Idle(3));
            Assert.Contains(ev, e => e.Type == SimEventType.Clank && e.Extra == 2);
            Assert.True(sim.Players[0].Alive && sim.Players[1].Alive);
            Assert.Equal(CapMode.Ground, sim.Players[0].Cap.Mode);
        }

        [Fact]
        public void LaGorraEnElPisoSePuedeLlamar()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-6f, -5.5f), new Vec2(10f, 5.5f));
            var cap = sim.Players[0].Cap;
            cap.Mode = CapMode.Ground; cap.Pos = new Vec2(0f, -5.5f); cap.Vel = Vec2.Zero;
            sim.Players[0].HasCap = false;
            var press = new InputCommand { Buttons = InputButtons.ThrowPressed | InputButtons.ThrowReleased };
            var ev = H.Run(sim, 90, t => t == 0 ? new[] { press, default } : H.Idle(2));
            Assert.Contains(ev, e => e.Type == SimEventType.Recall);
            Assert.True(sim.Players[0].HasCap);
        }

        [Fact]
        public void RebotaEnLaParedYRegresa()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-9.5f, -5.5f), new Vec2(10f, 5.5f));
            H.Run(sim, 1, t => new[] { H.Throw(new Vec2(-1, 0)), default });
            var ev = H.Run(sim, 20);
            Assert.Contains(ev, e => e.Type == SimEventType.Clank && e.Extra == 1);
            Assert.True(sim.Players[0].Cap.Mode == CapMode.Return || sim.Players[0].HasCap);
        }

        [Fact]
        public void SerigrafiaLanzaTresGorras()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-6f, -5.5f), new Vec2(10f, 5.5f));
            sim.Players[0].Power = PowerUp.Serigrafia;
            H.Run(sim, 1, t => new[] { H.Throw(new Vec2(1, 0)), default });
            Assert.Equal(2, sim.Caps.Count(c => c.Ghost && c.Lethal));
        }

        [Fact]
        public void TintaDerramadaExplotaYNoqueaEnArea()
        {
            var sim = new MatchSimulation(H.Config(4));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-8f, -5.5f), new Vec2(-2f, -5.5f), new Vec2(-2f, -3.8f), new Vec2(9f, 5.5f));
            sim.Players[0].Power = PowerUp.Tinta;
            var ev = H.Run(sim, 40, t => t == 0 ? new[] { H.Throw(new Vec2(1, 0)), default, default, default } : H.Idle(4));
            Assert.Contains(ev, e => e.Type == SimEventType.Explode);
            Assert.False(sim.Players[1].Alive);
            Assert.False(sim.Players[2].Alive);
        }

        [Fact]
        public void ElUltimoEnPieGanaLaRondaYElMarcadorSube()
        {
            var sim = new MatchSimulation(H.Config(2, points: 3));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-8f, -5.5f), new Vec2(-3f, -5.5f));
            var ev = H.Run(sim, 260, t => t == 0 ? new[] { H.Throw(new Vec2(1, 0)), default } : H.Idle(2));
            Assert.Equal(1, sim.Players[0].Score);
            Assert.Contains(ev, e => e.Type == SimEventType.RoundEnd && e.A == 0);
            Assert.Equal(2, sim.Round);
        }

        [Fact]
        public void PartidaCompletaDeBotsTermina()
        {
            foreach (var arena in new[] { "callejon", "museo", "azotea" })
            {
                var cfg = H.Config(4, bots: 4, arena: arena, seed: 99);
                var sim = new MatchSimulation(cfg);
                var bots = Enumerable.Range(0, 4).Select(i => new BotBrain(sim, i, BotDifficulty.Normal)).ToArray();
                var inputs = new InputCommand[4];
                int ticks = 0;
                while (sim.Phase != MatchPhase.MatchEnd && ticks < 60 * 60 * 15)
                {
                    for (int i = 0; i < 4; i++) inputs[i] = bots[i].Think(MatchSimulation.Dt);
                    sim.Step(inputs);
                    ticks++;
                }
                Assert.Equal(MatchPhase.MatchEnd, sim.Phase);
                Assert.Equal(5, sim.Players[sim.MatchWinnerId].Score);
                Assert.True(sim.RoundDurations.Average() is > 3f and < 90f, $"{arena}: ronda promedio {sim.RoundDurations.Average():0.0}s");
            }
        }
    }

    public class DeterminismAndWireTests
    {
        static (MatchSimulation sim, BotBrain[] bots) BotMatch(uint seed)
        {
            var sim = new MatchSimulation(H.Config(4, bots: 4, arena: "museo", seed: seed));
            return (sim, Enumerable.Range(0, 4).Select(i => new BotBrain(sim, i, BotDifficulty.Dificil)).ToArray());
        }

        [Fact]
        public void MismaSemillaMismasEntradasMismoResultado()
        {
            var (a, ba) = BotMatch(4242);
            var (b, bb) = BotMatch(4242);
            var ia = new InputCommand[4];
            var ib = new InputCommand[4];
            for (int t = 0; t < 4000; t++)
            {
                for (int i = 0; i < 4; i++) { ia[i] = ba[i].Think(MatchSimulation.Dt); ib[i] = bb[i].Think(MatchSimulation.Dt); }
                a.Step(ia); b.Step(ib);
                Assert.Equal(a.StateHash(), b.StateHash());
            }
        }

        [Fact]
        public void InputCommandSobreviveLaRed()
        {
            var cmd = new InputCommand { Move = new Vec2(0.5f, -1f), Aim = new Vec2(0f, 1f), Buttons = InputButtons.ThrowHeld | InputButtons.DashPressed | InputButtons.HasAim };
            var buf = new byte[InputCommand.WireSize];
            cmd.Write(buf, 0);
            var back = InputCommand.Read(buf, 0);
            Assert.Equal(cmd.Buttons, back.Buttons);
            Assert.InRange(back.Move.x, 0.49f, 0.51f);
            Assert.InRange(back.Move.y, -1.001f, -0.99f);
            Assert.InRange(back.Aim.y, 0.99f, 1.001f);
        }

        [Fact]
        public void ElSnapshotReproduceElEstado()
        {
            var (sim, bots) = BotMatch(77);
            var inputs = new InputCommand[4];
            for (int t = 0; t < 900; t++)
            {
                for (int i = 0; i < 4; i++) inputs[i] = bots[i].Think(MatchSimulation.Dt);
                sim.Step(inputs);
            }
            var ms = new MemoryStream();
            sim.WriteSnapshot(new BinaryWriter(ms));
            Assert.InRange(ms.Length, 50, 1400);
            var copy = new MatchSimulation(sim.Config);
            ms.Position = 0;
            copy.ReadSnapshot(new BinaryReader(ms));
            Assert.Equal(sim.Tick, copy.Tick);
            Assert.Equal(sim.Phase, copy.Phase);
            Assert.Equal(sim.Caps.Count, copy.Caps.Count);
            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(sim.Players[i].Pos, copy.Players[i].Pos);
                Assert.Equal(sim.Players[i].Alive, copy.Players[i].Alive);
                Assert.Equal(sim.Players[i].Score, copy.Players[i].Score);
                Assert.Equal(sim.Players[i].Cap.Mode, copy.Players[i].Cap.Mode);
            }
        }
    }
}
