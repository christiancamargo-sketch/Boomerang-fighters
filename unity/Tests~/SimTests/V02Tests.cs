// ─────────────────────────────────────────────────────────────
// Pruebas de las reglas v0.2 (docs/diseno/spec-v0.2-local.md):
// doble KO, muerte súbita, empujón, "Te la regreso", Sticker,
// ¡CLANK! al 85 %, Charco, Muro de Tag, letreros de KO, modos
// Lata Caliente / La Reta / Reto del Día y bots.
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BoomerangFighters.Core;
using Xunit;

namespace BoomerangFighters.Tests
{
    static class H2
    {
        public static MatchConfig Config(int players, GameMode mode, int bots = 0, string arena = "callejon", uint seed = 777, int points = 5, Rules rules = null)
        {
            var cfg = H.Config(players, bots, arena, seed, points);
            cfg.Mode = mode;
            cfg.Rules = rules ?? new Rules();
            return cfg;
        }

        /// <summary>Pone la gorra de un jugador en vuelo, a mano.</summary>
        public static CapState FlyCap(MatchSimulation sim, int owner, CapMode mode, Vec2 pos, Vec2 vel, float t = 0.2f, float outTime = 1f)
        {
            var p = sim.Players[owner];
            p.HasCap = false;
            var c = p.Cap;
            c.Mode = mode; c.Pos = pos; c.Vel = vel; c.V0 = vel.Length; c.RetSpeed = vel.Length;
            c.T = t; c.OutTime = outTime; c.Y = sim.T.Cap.FlyHeight; c.Radius = sim.T.Cap.Radius;
            c.Power = PowerUp.None; c.Bounced = false; c.DeflectT = 0f; c.DeflectBy = -1;
            return c;
        }

        public static void Put(MatchSimulation sim, int id, Vec2 pos)
        {
            sim.Players[id].Pos = pos;
            sim.Players[id].Vel = Vec2.Zero;
        }

        public static InputCommand[] One(int n, int who, InputCommand cmd)
        {
            var a = new InputCommand[n];
            a[who] = cmd;
            return a;
        }

        public static InputCommand Melee(Vec2 aim) => new InputCommand { Aim = aim, Buttons = InputButtons.HasAim | InputButtons.MeleePressed };

        /// <summary>Corre hasta que termine la ronda actual (fase Between o MatchEnd).</summary>
        public static List<SimEvent> RunToRoundOver(MatchSimulation sim, int maxTicks = 4000)
        {
            var all = new List<SimEvent>();
            var idle = H.Idle(sim.Players.Count);
            for (int i = 0; i < maxTicks && sim.Phase != MatchPhase.Between && sim.Phase != MatchPhase.MatchEnd; i++)
            {
                sim.Step(idle);
                all.AddRange(sim.Events);
            }
            return all;
        }

        public static MatchSimulation BotMatch(GameMode mode, int n, uint seed, BotDifficulty diff, out BotBrain[] bots, string arena = "museo")
        {
            var sim = new MatchSimulation(Config(n, mode, bots: n, arena: arena, seed: seed));
            bots = Enumerable.Range(0, n).Select(i => new BotBrain(sim, i, diff)).ToArray();
            return sim;
        }

        public static void StepBots(MatchSimulation sim, BotBrain[] bots, InputCommand[] buf)
        {
            for (int i = 0; i < bots.Length; i++) buf[i] = bots[i].Think(MatchSimulation.Dt);
            sim.Step(buf);
        }
    }

    public class CombatRulesV02Tests
    {
        // ── A1 ──
        [Fact]
        public void DobleKoMenosDe6TicksEsEmpate()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-6f, -5.5f));
            H2.Put(sim, 1, new Vec2(6f, -5.5f));
            // la gorra de P0 noquea a P1 en ~2 ticks; la de P1 noquea a P0 ~2 ticks de juego después
            H2.FlyCap(sim, 0, CapMode.Out, new Vec2(4.8f, -5.5f), new Vec2(15f, 0f));
            H2.FlyCap(sim, 1, CapMode.Out, new Vec2(-7.5f, -5.5f), new Vec2(15f, 0f));
            var ev = H2.RunToRoundOver(sim);
            Assert.Equal(2, ev.Count(e => e.Type == SimEventType.KO));
            var end = ev.Single(e => e.Type == SimEventType.RoundEnd);
            Assert.Equal(-1, end.A);
            Assert.Equal((byte)RoundEndReason.DoubleKo, end.Extra);
            Assert.Equal(0, sim.Players[0].Score + sim.Players[1].Score);
        }

        [Fact]
        public void SiElUltimoCaeDespuesDeLaVentanaGanaLaRonda()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-6f, -5.5f));
            H2.Put(sim, 1, new Vec2(6f, -5.5f));
            H2.FlyCap(sim, 0, CapMode.Out, new Vec2(4.8f, -5.5f), new Vec2(15f, 0f));
            H2.FlyCap(sim, 1, CapMode.Out, new Vec2(-10f, -5.5f), new Vec2(15f, 0f)); // llega tarde
            var ev = H2.RunToRoundOver(sim);
            var end = ev.Single(e => e.Type == SimEventType.RoundEnd);
            Assert.Equal(0, end.A);
            Assert.True(sim.Players[0].Alive, "ya declarada la ronda nadie más cae");
            Assert.Equal(1, sim.Players[0].Score);
        }

        // ── A2 ──
        [Fact]
        public void MuerteSubitaLosMurosEmpujanYALos45sEsEmpate()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-10f, -5.5f));
            H2.Put(sim, 1, new Vec2(10f, 5.5f));
            var ev = new List<SimEvent>();
            float xAt30 = 0f;
            var idle = H.Idle(2);
            for (int i = 0; i < 60 * 50 && sim.Phase == MatchPhase.Play; i++)
            {
                sim.Step(idle);
                ev.AddRange(sim.Events);
                if (MathF.Abs(sim.RoundTime - 30f) < 0.01f) xAt30 = sim.Players[0].Pos.x;
            }
            var warn = ev.Single(e => e.Type == SimEventType.SuddenDeathWarn);
            Assert.Contains(ev, e => e.Type == SimEventType.SuddenDeathStart);
            // a los 30 s: 9 s de avance × 0.5 m/s = 4.5 m → el jugador fue empujado hacia adentro
            Assert.InRange(xAt30, -(11.5f - 4.5f - 0.55f) - 0.05f, -(11.5f - 4.5f - 0.55f) + 0.05f);
            Assert.Equal(11.5f - 4f, sim.Arena.WallInset, 3); // se detuvo al dejar 8 m
            Assert.True(sim.Players[0].Pos.x >= -4f && sim.Players[1].Pos.x <= 4f);
            Assert.True(sim.Players[0].Alive && sim.Players[1].Alive, "los muros empujan, no matan");
            Assert.Equal(MatchPhase.RoundEnd, sim.Phase);
            Assert.InRange(sim.RoundTime, 44.9f, 45.1f);
            ev.AddRange(H2.RunToRoundOver(sim));
            var end = ev.Single(e => e.Type == SimEventType.RoundEnd);
            Assert.Equal(-1, end.A);
            Assert.Equal((byte)RoundEndReason.TimeUp, end.Extra);
            // la siguiente ronda empieza con los muros en su lugar
            H.SkipCountdown(sim);
            Assert.Equal(0f, sim.Arena.WallInset);
        }

        [Fact]
        public void SinMuerteSubitaNoHayMurosNiEmpatePorTiempo()
        {
            var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, rules: new Rules { SuddenDeath = false }));
            H.SkipCountdown(sim);
            var ev = H.Run(sim, 60 * 50);
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.SuddenDeathWarn);
            Assert.Equal(MatchPhase.Play, sim.Phase);
            Assert.Equal(0f, sim.Arena.WallInset);
        }

        // ── A3 ──
        [Fact]
        public void ElEmpujonSinGorraEmpujaYNoNoquea()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-6f, -5.5f));
            H2.Put(sim, 1, new Vec2(-4.9f, -5.5f));
            var cap0 = sim.Players[0].Cap;
            cap0.Mode = CapMode.Ground; cap0.Pos = new Vec2(-6f, 5.5f); cap0.Vel = Vec2.Zero; cap0.T = 0f;
            sim.Players[0].HasCap = false;
            var ev = H.Run(sim, 30, t => t == 0 ? H2.One(2, 0, H2.Melee(new Vec2(1, 0))) : H.Idle(2));
            Assert.Contains(ev, e => e.Type == SimEventType.Shove && e.A == 0 && e.B == 1);
            Assert.True(sim.Players[1].Alive);
            Assert.InRange(sim.Players[1].Pos.x, -4.9f + 2.2f, -4.9f + 2.8f);
            Assert.True(sim.Players[0].ShoveCd > 0f, "recarga de 0.8 s");
        }

        [Fact]
        public void ElEmpujonPateaUnaGorraAjenaCaida()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-6f, -5.5f));
            H2.Put(sim, 1, new Vec2(8f, 5.5f));
            var cap0 = sim.Players[0].Cap;
            cap0.Mode = CapMode.Ground; cap0.Pos = new Vec2(-6f, 5.5f); cap0.T = 0f; sim.Players[0].HasCap = false;
            var cap1 = sim.Players[1].Cap;
            cap1.Mode = CapMode.Ground; cap1.Pos = new Vec2(-5f, -5.5f); cap1.Vel = Vec2.Zero; cap1.T = 0f; sim.Players[1].HasCap = false;
            var ev = H.Run(sim, 60, t => t == 0 ? H2.One(2, 0, H2.Melee(new Vec2(1, 0))) : H.Idle(2));
            Assert.Contains(ev, e => e.Type == SimEventType.Shove && e.Extra == 2);
            Assert.InRange(cap1.Pos.x - (-5f), 3.4f, 4.2f);
        }

        [Fact]
        public void SinInterruptorDeEmpujonNoPasaNada()
        {
            var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, rules: new Rules { Shove = false }));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-6f, -5.5f));
            H2.Put(sim, 1, new Vec2(-4.9f, -5.5f));
            var cap0 = sim.Players[0].Cap;
            cap0.Mode = CapMode.Ground; cap0.Pos = new Vec2(-6f, 5.5f); cap0.T = 0f; sim.Players[0].HasCap = false;
            var ev = H.Run(sim, 30, t => t == 0 ? H2.One(2, 0, H2.Melee(new Vec2(1, 0))) : H.Idle(2));
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.Shove);
            Assert.InRange(sim.Players[1].Pos.x, -5.0f, -4.8f);
        }

        // ── A4 ──
        [Fact]
        public void TeLaRegresoLaGorraDesviadaNoqueaASuDueno()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-6f, -5.5f));
            H2.Put(sim, 1, new Vec2(0f, -5.5f));
            H2.FlyCap(sim, 0, CapMode.Out, new Vec2(-1.6f, -5.5f), new Vec2(15f, 0f));
            var ev = H.Run(sim, 60, t => t == 0 ? H2.One(2, 1, H2.Melee(new Vec2(-1, 0))) : H.Idle(2));
            Assert.Contains(ev, e => e.Type == SimEventType.Deflect && e.A == 1 && e.B == 0 && e.Extra == 1);
            var ko = ev.Single(e => e.Type == SimEventType.KO);
            Assert.Equal(0, ko.A);
            Assert.Equal(1, ko.B);
            Assert.Equal((byte)KoCause.TeLaRegreso, ko.Extra);
            Assert.True(sim.Players[1].Alive);
            Assert.Equal(1, sim.Players[1].Kos);
        }

        [Fact]
        public void SinTeLaRegresoLaGorraDesviadaCae()
        {
            var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, rules: new Rules { DeflectReturn = false }));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-6f, -5.5f));
            H2.Put(sim, 1, new Vec2(0f, -5.5f));
            var cap = H2.FlyCap(sim, 0, CapMode.Out, new Vec2(-1.6f, -5.5f), new Vec2(15f, 0f));
            var ev = H.Run(sim, 2, t => t == 0 ? H2.One(2, 1, H2.Melee(new Vec2(-1, 0))) : H.Idle(2));
            Assert.Contains(ev, e => e.Type == SimEventType.Deflect && e.Extra == 0);
            Assert.Equal(CapMode.Ground, cap.Mode);
            H.Run(sim, 40);
            Assert.True(sim.Players[0].Alive);
        }

        // ── A5 / A6 ──
        [Fact]
        public void StickerXLEsDiezPorCientoMasLento()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-6f, -5.5f));
            H2.Put(sim, 1, new Vec2(6f, 5.5f));
            sim.Players[0].Power = PowerUp.Sticker;
            H.Run(sim, 1, t => new[] { H.Throw(new Vec2(1, 0)), H.Throw(new Vec2(-1, 0)) });
            var big = sim.Players[0].Cap; var normal = sim.Players[1].Cap;
            Assert.Equal(normal.V0 * 0.9f, big.V0, 3);
            Assert.Equal(normal.Radius * 2f, big.Radius, 3);
        }

        [Fact]
        public void ElClankUsaEl85PorCientoDeLaSumaDeRadios()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-9f, 5.5f));
            H2.Put(sim, 1, new Vec2(9f, 5.5f));
            // separadas 0.70 m: antes chocaban (suma 0.76), ahora no (0.85 × 0.76 = 0.646)
            var a = H2.FlyCap(sim, 0, CapMode.Out, new Vec2(-0.35f, -5.5f), Vec2.Zero, outTime: 5f);
            var b = H2.FlyCap(sim, 1, CapMode.Out, new Vec2(0.35f, -5.5f), Vec2.Zero, outTime: 5f);
            var ev = H.Run(sim, 1);
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.Clank);
            a.Pos = new Vec2(-0.3f, -5.5f); b.Pos = new Vec2(0.3f, -5.5f);
            ev = H.Run(sim, 1);
            Assert.Contains(ev, e => e.Type == SimEventType.Clank && e.Extra == 2);
        }

        // ── A8 ──
        [Fact]
        public void LosKoLlevanSuCausaDetallada()
        {
            // gorra de ida = ¡GORRAZO!
            var sim = new MatchSimulation(H.Config(3));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-8f, -5.5f), new Vec2(-3f, -5.5f), new Vec2(9f, 5.5f));
            var ev = H.Run(sim, 60, t => t == 0 ? new[] { H.Throw(new Vec2(1, 0)), default, default } : H.Idle(3));
            Assert.Equal((byte)KoCause.Gorrazo, ev.Single(e => e.Type == SimEventType.KO).Extra);

            // gorra de regreso = ¡DE REGRESO!
            sim = new MatchSimulation(H.Config(3));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-8f, -5.5f), new Vec2(-4f, -5.5f), new Vec2(9f, 5.5f));
            H2.FlyCap(sim, 0, CapMode.Return, new Vec2(-2.5f, -5.5f), new Vec2(-15f, 0f));
            ev = H.Run(sim, 30);
            Assert.Equal((byte)KoCause.DeRegreso, ev.Single(e => e.Type == SimEventType.KO && e.A == 1).Extra);

            // gorra que ya rebotó = ¡DE BANDA!
            sim = new MatchSimulation(H.Config(3));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-7.5f, 6f), new Vec2(-7.5f, -4f), new Vec2(9f, 5.5f));
            H2.FlyCap(sim, 0, CapMode.Out, new Vec2(-7.5f, -6.3f), new Vec2(0f, -12f));
            ev = H.Run(sim, 30);
            Assert.Equal((byte)KoCause.DeBanda, ev.Single(e => e.Type == SimEventType.KO && e.A == 1).Extra);

            // golpe = ¡ZAZ!
            sim = new MatchSimulation(H.Config(3));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-6f, -5.5f), new Vec2(-4.9f, -5.5f), new Vec2(9f, 5.5f));
            ev = H.Run(sim, 20, t => t == 0 ? H2.One(3, 0, H2.Melee(new Vec2(1, 0))) : H.Idle(3));
            Assert.Equal((byte)KoCause.Zaz, ev.Single(e => e.Type == SimEventType.KO).Extra);

            Assert.Equal("¡TE LA REGRESO!", KoCauseInfo.Label(KoCause.TeLaRegreso));
            Assert.Equal("¡SPLASH!", KoCauseInfo.Label(KoCause.Splash));
        }
    }

    public class PowerUpsV02Tests
    {
        [Fact]
        public void ElCharcoCreceYNoqueaAQuienSigaDentro()
        {
            var sim = new MatchSimulation(H.Config(4));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-8f, -5.5f), new Vec2(-2f, -5.5f), new Vec2(-2f, -4.3f), new Vec2(9f, 5.5f));
            sim.Players[0].Power = PowerUp.Tinta;
            float midRadius = -1f;
            int startTick = -1;
            var ev = H.Run(sim, 100, t =>
            {
                var inp = H.Idle(4);
                if (t == 0) inp[0] = H.Throw(new Vec2(1, 0));
                if (sim.Puddles.Count > 0)
                {
                    if (startTick < 0) startTick = t;
                    if (t - startTick == 12) midRadius = sim.Puddles[0].Radius;
                    inp[2] = new InputCommand { Move = new Vec2(0f, 1f) }; // P2 sale corriendo del charco
                }
                return inp;
            });
            Assert.Contains(ev, e => e.Type == SimEventType.PuddleStart && e.A == 0);
            Assert.InRange(midRadius, 0.6f, 1.4f); // creciendo, todavía no a 2 m
            Assert.False(sim.Players[1].Alive);
            Assert.Equal((byte)KoCause.Splash, ev.Single(e => e.Type == SimEventType.KO && e.A == 1).Extra);
            Assert.True(sim.Players[2].Alive, "quien sale del charco a tiempo se salva");
            Assert.True(sim.Players[0].Alive);
            Assert.Empty(sim.Puddles);
        }

        [Fact]
        public void ElTatuajeMarcaASuVictima()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-8f, -5.5f));
            H2.Put(sim, 1, new Vec2(-2f, -4.5f));
            sim.Players[0].Power = PowerUp.Tatuaje;
            var ev = H.Run(sim, 3, t => t == 0 ? new[] { H.Throw(new Vec2(1, 0)), default } : H.Idle(2));
            Assert.Contains(ev, e => e.Type == SimEventType.Marked && e.A == 1 && e.B == 0);
            Assert.Equal(1, sim.Players[0].Cap.MarkedId);
        }

        [Fact]
        public void ElMuroDeTagBloqueaJugadoresYGorras()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-8f, -5.5f));
            H2.Put(sim, 1, new Vec2(2f, -5.5f));
            sim.Players[0].Power = PowerUp.Grafiti;
            var ev = H.Run(sim, 40, t => t == 0 ? new[] { H.Throw(new Vec2(1, 0)), default } : H.Idle(2));
            var tag = ev.Single(e => e.Type == SimEventType.TagWall);
            Assert.Single(sim.TagWalls);
            var wall = sim.TagWalls[0];
            Assert.Equal(0f, Vec2.Dot(wall.Dir, new Vec2(1, 0)), 3); // perpendicular al vuelo
            float wallX = wall.Pos.x;
            Assert.Equal(CapMode.Return, sim.Players[0].Cap.Mode); // la gorra del dueño atraviesa su propio muro (no cae)
            // P1 camina hacia la izquierda y se topa con el muro
            H.Run(sim, 60, t => new[] { default, new InputCommand { Move = new Vec2(-1f, 0f) } });
            Assert.True(sim.Players[1].Pos.x >= wallX + 0.2f + 0.5f, $"P1 cruzó el muro: {sim.Players[1].Pos.x} vs muro {wallX}");
            // la gorra de P1 rebota en el muro y P0 no cae
            ev = H.Run(sim, 30, t => t == 0 ? new[] { default, H.Throw(new Vec2(-1, 0)) } : H.Idle(2));
            Assert.Contains(ev, e => e.Type == SimEventType.Clank && e.A == 1 && e.Extra == 1);
            Assert.True(sim.Players[0].Alive);
            Assert.True(sim.Players[1].Cap.Pos.x > wallX, "la gorra rival se quedó de su lado del muro");
            H.Run(sim, 200);
            Assert.Empty(sim.TagWalls); // dura 3 s
        }

        [Fact]
        public void SinMuroDeTagElGrafitiDejaRastro()
        {
            var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, rules: new Rules { GraffitiWall = false }));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-8f, -5.5f));
            H2.Put(sim, 1, new Vec2(8f, 5.5f));
            sim.Players[0].Power = PowerUp.Grafiti;
            var ev = H.Run(sim, 40, t => t == 0 ? new[] { H.Throw(new Vec2(1, 0)), default } : H.Idle(2));
            Assert.Contains(ev, e => e.Type == SimEventType.Paint);
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.TagWall);
            Assert.Empty(sim.TagWalls);
        }
    }

    public class ModesV02Tests
    {
        [Fact]
        public void LataCalienteElEmpujonDeQuienLaTraeLaPasa()
        {
            var sim = new MatchSimulation(H2.Config(3, GameMode.LataCaliente));
            H.SkipCountdown(sim);
            int h = sim.HotCanHolder, o1 = (h + 1) % 3, o2 = (h + 2) % 3;
            H2.Put(sim, h, new Vec2(-6f, -5.5f));
            H2.Put(sim, o1, new Vec2(-4.9f, -5.5f));
            H2.Put(sim, o2, new Vec2(9f, 5.5f));
            var cap = sim.Players[h].Cap;
            cap.Mode = CapMode.Ground; cap.Pos = new Vec2(-6f, 5.5f); cap.Vel = Vec2.Zero; cap.T = 0f;
            sim.Players[h].HasCap = false;
            var ev = H.Run(sim, 2, t => t == 0 ? H2.One(3, h, H2.Melee(new Vec2(1, 0))) : H.Idle(3));
            Assert.Contains(ev, e => e.Type == SimEventType.Shove && e.A == h && e.B == o1);
            Assert.Contains(ev, e => e.Type == SimEventType.HotCanPass && e.A == o1 && e.B == h);
            Assert.Equal(o1, sim.HotCanHolder);
            Assert.True(sim.Players[o1].Alive);
        }

        [Fact]
        public void LataCalienteSePasaConElGorrazoYExplota()
        {
            var sim = new MatchSimulation(H2.Config(3, GameMode.LataCaliente));
            H.SkipCountdown(sim);
            int h = sim.HotCanHolder;
            Assert.InRange(h, 0, 2);
            Assert.InRange(sim.HotCanTimer, 9f, 20f);
            int o1 = (h + 1) % 3, o2 = (h + 2) % 3;
            H2.Put(sim, h, new Vec2(-8f, -5.5f));
            H2.Put(sim, o1, new Vec2(-3f, -5.5f));
            H2.Put(sim, o2, new Vec2(9f, 5.5f));
            var ev = H.Run(sim, 60, t => t == 0 ? H2.One(3, h, H.Throw(new Vec2(1, 0))) : H.Idle(3));
            Assert.True(sim.Players[o1].Alive, "con la lata en juego el gorrazo no noquea");
            Assert.Contains(ev, e => e.Type == SimEventType.HotCanPass && e.A == o1 && e.B == h);
            Assert.Equal(o1, sim.HotCanHolder);

            // esperar a que explote: queda fuera quien la tiene
            ev = new List<SimEvent>();
            for (int i = 0; i < 60 * 21 && !ev.Any(e => e.Type == SimEventType.HotCanExplode); i++) { sim.Step(H.Idle(3)); ev.AddRange(sim.Events); }
            Assert.Contains(ev, e => e.Type == SimEventType.HotCanExplode && e.A == o1);
            var ko = ev.Single(e => e.Type == SimEventType.KO);
            Assert.Equal(o1, ko.A);
            Assert.Equal((byte)KoCause.Splash, ko.Extra);
            // se pega otra lata al azar entre los vivos
            var again = ev.Last(e => e.Type == SimEventType.HotCanPass);
            Assert.Equal(-1, again.B);
            Assert.True(again.A == h || again.A == o2);
            Assert.InRange(sim.HotCanTimer, 9.9f, 20f);
            Assert.Empty(sim.Pickups); // sin latas de técnica en este modo

            // la segunda explosión deja a uno en pie: gana la ronda
            ev = H2.RunToRoundOver(sim, 60 * 25);
            var end = ev.Single(e => e.Type == SimEventType.RoundEnd);
            Assert.True(end.A == h || end.A == o2);
            Assert.Equal(1, sim.Players[end.A].Score);
        }

        [Fact]
        public void LataCalienteEsDe3A6Jugadores()
        {
            Assert.Throws<ArgumentException>(() => new MatchSimulation(H2.Config(2, GameMode.LataCaliente)));
            Assert.Throws<ArgumentException>(() => new MatchSimulation(H2.Config(7, GameMode.LataCaliente)));
        }

        static void WinRetaRound(MatchSimulation sim, int winner, int loser)
        {
            H.SkipCountdown(sim);
            Assert.True(sim.IsRetaFighter(winner) && sim.IsRetaFighter(loser));
            Assert.Equal(2, sim.AliveCount);
            H2.Put(sim, winner, new Vec2(-8f, -5.5f));
            H2.Put(sim, loser, new Vec2(-3f, -5.5f));
            H.Run(sim, 1, t => H2.One(sim.Players.Count, winner, H.Throw(new Vec2(1, 0))));
            H2.RunToRoundOver(sim);
        }

        [Fact]
        public void LaRetaElGanadorSeQuedaYLaFilaRota()
        {
            var sim = new MatchSimulation(H2.Config(4, GameMode.LaReta));
            Assert.Equal(new[] { 0, 1 }, sim.RetaFighters);
            Assert.Equal(new[] { 2, 3 }, sim.RetaQueue);
            Assert.True(sim.Players[2].Benched && !sim.Players[2].Alive);
            Assert.True(sim.Players[3].Benched && !sim.Players[3].Alive);

            WinRetaRound(sim, 0, 1);
            Assert.Equal(1, sim.Players[0].Streak);
            Assert.Equal(new[] { 3, 1 }, sim.RetaQueue);       // el perdedor va al final
            Assert.True(sim.IsRetaFighter(0) && sim.IsRetaFighter(2)); // entra el siguiente

            WinRetaRound(sim, 0, 2);
            Assert.Equal(2, sim.Players[0].Streak);
            Assert.Equal(new[] { 1, 2 }, sim.RetaQueue);
            Assert.True(sim.IsRetaFighter(3));

            WinRetaRound(sim, 0, 3);
            Assert.Equal(MatchPhase.MatchEnd, sim.Phase);       // 3 seguidas
            Assert.Equal(0, sim.MatchWinnerId);
            Assert.Equal(3, sim.Players[0].Score);
        }

        [Fact]
        public void LaRetaEmpateAmbosSeQuedanYTerminaALas10Rondas()
        {
            // sin rivales que se maten: cada ronda acaba en empate por tiempo (45 s)
            var sim = new MatchSimulation(H2.Config(3, GameMode.LaReta));
            sim.Players[2].Kos = 0;
            int rounds = 0;
            var ev = new List<SimEvent>();
            for (int i = 0; i < 60 * 60 * 10 && sim.Phase != MatchPhase.MatchEnd; i++)
            {
                sim.Step(H.Idle(3));
                ev.AddRange(sim.Events);
            }
            rounds = ev.Count(e => e.Type == SimEventType.RoundEnd);
            Assert.Equal(10, rounds);
            Assert.All(ev.Where(e => e.Type == SimEventType.RoundEnd), e => Assert.Equal(-1, e.A));
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.RetaNext);
            Assert.Equal(new[] { 0, 1 }, sim.RetaFighters);
            Assert.Equal(MatchPhase.MatchEnd, sim.Phase);
            Assert.Equal(0, sim.MatchWinnerId); // desempate: más victorias, más KOs, asiento más bajo
        }

        [Fact]
        public void RetoDelDiaExponeElPuntajeDelJugador0()
        {
            var cfg = H2.Config(4, GameMode.RetoDelDia, bots: 3, seed: 20261123);
            var sim = new MatchSimulation(cfg);
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-8f, -5.5f), new Vec2(-3f, -5.5f), new Vec2(9f, 5.5f), new Vec2(9f, 3.5f));
            H.Run(sim, 60, t => t == 0 ? H2.One(4, 0, H.Throw(new Vec2(1, 0))) : H.Idle(4));
            Assert.Equal(1, sim.Players[0].Kos);
            Assert.Equal(GameMode.RetoDelDia, sim.Mode);
            // se restan los segundos de juego (sin la cuenta regresiva), redondeados como Math.round de la web
            Assert.True(sim.PlayTime < sim.MatchTime);
            Assert.Equal((int)MathF.Floor(sim.PlayTime + 0.5f), sim.DailySeconds);
            Assert.Equal(100 - sim.DailySeconds, sim.DailyScore());
            sim.Players[0].RoundWins = 2;
            Assert.Equal(100 + 600 - sim.DailySeconds, sim.DailyScore(0));
        }
    }

    public class BotsAndDeterminismV02Tests
    {
        [Theory]
        [InlineData(GameMode.Gorrazo, 4)]
        [InlineData(GameMode.LataCaliente, 4)]
        [InlineData(GameMode.LaReta, 5)]
        [InlineData(GameMode.RetoDelDia, 4)]
        public void MismaSemillaMismoResultadoEnCadaModo(GameMode mode, int n)
        {
            var a = H2.BotMatch(mode, n, 4242, BotDifficulty.Dificil, out var ba);
            var b = H2.BotMatch(mode, n, 4242, BotDifficulty.Dificil, out var bb);
            var ia = new InputCommand[n]; var ib = new InputCommand[n];
            for (int t = 0; t < 4000; t++)
            {
                H2.StepBots(a, ba, ia); H2.StepBots(b, bb, ib);
                Assert.Equal(a.StateHash(), b.StateHash());
            }
        }

        [Theory]
        [InlineData(GameMode.LataCaliente, 5, "callejon")]
        [InlineData(GameMode.LaReta, 6, "azotea")]
        [InlineData(GameMode.RetoDelDia, 4, "museo")]
        public void PartidaDeBotsTerminaEnCadaModo(GameMode mode, int n, string arena)
        {
            var sim = H2.BotMatch(mode, n, 99, BotDifficulty.Normal, out var bots, arena);
            var buf = new InputCommand[n];
            while (sim.Phase != MatchPhase.MatchEnd && sim.Tick < 60 * 60 * 25) H2.StepBots(sim, bots, buf);
            Assert.Equal(MatchPhase.MatchEnd, sim.Phase);
            Assert.InRange(sim.MatchWinnerId, 0, n - 1);
        }

        [Fact]
        public void LosBotsDificilesDesvianYEmpujan()
        {
            int deflects = 0, shoves = 0;
            for (uint seed = 1; seed <= 3; seed++)
            {
                var sim = H2.BotMatch(GameMode.Gorrazo, 4, seed * 31, BotDifficulty.Dificil, out var bots, "callejon");
                var buf = new InputCommand[4];
                while (sim.Phase != MatchPhase.MatchEnd && sim.Tick < 60 * 60 * 15)
                {
                    H2.StepBots(sim, bots, buf);
                    foreach (var e in sim.Events)
                    {
                        if (e.Type == SimEventType.Deflect && e.Extra == 1) deflects++;
                        if (e.Type == SimEventType.Shove) shoves++;
                    }
                }
            }
            Assert.True(deflects > 0, "los bots nunca desviaron");
            Assert.True(shoves > 0, "los bots nunca empujaron");
        }
    }

    public class SnapshotV02Tests
    {
        static MatchSimulation RoundTrip(MatchSimulation sim, out long size)
        {
            var ms = new MemoryStream();
            sim.WriteSnapshot(new BinaryWriter(ms));
            size = ms.Length;
            var copy = new MatchSimulation(sim.Config);
            ms.Position = 0;
            copy.ReadSnapshot(new BinaryReader(ms));
            return copy;
        }

        [Fact]
        public void ElSnapshotLlevaElEstadoNuevo()
        {
            var sim = H2.BotMatch(GameMode.LataCaliente, 4, 77, BotDifficulty.Normal, out var bots);
            var buf = new InputCommand[4];
            while (sim.Phase != MatchPhase.Play || sim.RoundTime < 3f) H2.StepBots(sim, bots, buf);
            sim.Arena.WallInset = 2.5f;
            sim.Puddles.Add(new Puddle { Id = 42, Pos = new Vec2(1.5f, -2f), OwnerId = 2, T = 0.1f, Radius = 0.5f });
            sim.TagWalls.Add(new TagWall { Id = 7, Pos = new Vec2(-3f, 1f), Dir = new Vec2(0f, 1f), HalfLen = 1.25f, HalfThick = 0.2f, Life = 2f, OwnerId = 1 });
            var cap = sim.Players[3].Cap;
            cap.Mode = CapMode.Return; cap.DeflectT = 0.3f; cap.DeflectBy = 1; cap.Bounced = true; cap.MarkedId = 2;

            var copy = RoundTrip(sim, out long size);
            Assert.InRange(size, 50, 1400);
            Assert.Equal(GameMode.LataCaliente, copy.Mode);
            Assert.Equal(sim.HotCanHolder, copy.HotCanHolder);
            Assert.True(copy.HotCanHolder >= 0);
            Assert.Equal(sim.HotCanTimer, copy.HotCanTimer);
            Assert.Equal(sim.RoundTime, copy.RoundTime);
            Assert.Equal(2.5f, copy.Arena.WallInset);
            var pd = Assert.Single(copy.Puddles);
            Assert.Equal(new Vec2(1.5f, -2f), pd.Pos); Assert.Equal(0.5f, pd.Radius); Assert.Equal(2, pd.OwnerId);
            var tw = Assert.Single(copy.TagWalls);
            Assert.Equal(new Vec2(-3f, 1f), tw.Pos); Assert.Equal(1, tw.OwnerId); Assert.Equal(2f, tw.Life);
            Assert.Equal(1f, tw.Dir.y, 4);
            var c2 = copy.Players[3].Cap;
            Assert.True(c2.Deflected && c2.Bounced);
            Assert.Equal(1, c2.DeflectBy);
            Assert.Equal(2, c2.MarkedId);
        }

        [Fact]
        public void ElSnapshotLlevaLaFilaDeLaReta()
        {
            var sim = new MatchSimulation(H2.Config(5, GameMode.LaReta));
            H.SkipCountdown(sim);
            sim.Players[0].Streak = 2;
            var copy = RoundTrip(sim, out _);
            Assert.Equal(sim.RetaFighters, copy.RetaFighters);
            Assert.Equal(sim.RetaQueue, copy.RetaQueue);
            Assert.Equal(2, copy.Players[0].Streak);
            for (int i = 0; i < 5; i++) Assert.Equal(sim.Players[i].Benched, copy.Players[i].Benched);
            Assert.True(copy.Players[4].Benched);
        }
    }
}
