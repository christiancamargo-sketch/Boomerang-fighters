// ─────────────────────────────────────────────────────────────
// Pruebas de la spec v0.3 (docs/diseno/spec-v0.3-escenarios.md):
// vacíos (caer, cruzar con dash, crédito del KO, gorras, apariciones,
// muerte súbita), las 4 arenas con sus medidas, la caja DROP de la
// Pop-up Store, bots que no se caen solos, determinismo y snapshot.
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BoomerangFighters.Core;
using BoomerangFighters.Net;
using Xunit;

namespace BoomerangFighters.Tests
{
    static class H3
    {
        public static readonly string[] Arenas = { "callejon", "museo", "azotea", "popup" };

        /// <summary>Deja la gorra de un jugador tirada en el piso (sin gorra en la mano).</summary>
        public static CapState GroundCap(MatchSimulation sim, int owner, Vec2 pos)
        {
            var p = sim.Players[owner];
            p.HasCap = false;
            var c = p.Cap;
            c.Mode = CapMode.Ground; c.Pos = pos; c.Vel = Vec2.Zero; c.T = 0f;
            return c;
        }

        /// <summary>Corre hasta que salga un evento del tipo pedido (o se acaben los ticks). Devuelve todos los eventos.</summary>
        public static List<SimEvent> RunUntil(MatchSimulation sim, SimEventType type, int maxTicks, Func<InputCommand[]> inputs = null)
        {
            var all = new List<SimEvent>();
            var idle = H.Idle(sim.Players.Count);
            for (int i = 0; i < maxTicks; i++)
            {
                sim.Step(inputs != null ? inputs() : idle);
                all.AddRange(sim.Events);
                if (sim.Events.Any(e => e.Type == type)) break;
            }
            return all;
        }

        public static SimEvent Ko(IEnumerable<SimEvent> ev, int victim) => ev.Single(e => e.Type == SimEventType.KO && e.A == victim);

        /// <summary>Un punto libre a "dist" metros de "c" (prueba 8 direcciones).</summary>
        public static Vec2 FreeSpotAt(MatchSimulation sim, Vec2 c, float dist)
        {
            for (int k = 0; k < 8; k++)
            {
                var p = c + Vec2.FromAngle(k * MathF.PI / 4f) * dist;
                if (sim.Arena.IsFree(p, 0.6f)) return p;
            }
            throw new InvalidOperationException($"sin lugar libre a {dist} m de {c}");
        }
    }

    // ── A. Vacíos ────────────────────────────────────────────
    public class PitsV03Tests
    {
        // Callejón: coladera (5.6, −4.9, 1.4, 1.4) → x de 4.9 a 6.3; zanja (0, −3.0, 3.4, 0.9) → y de −3.45 a −2.55

        [Fact]
        public void QuienQuedaSobreUnVacioCae()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H2.Put(sim, 1, new Vec2(-8f, 6.5f));
            // 0.10 m adentro de la orilla: todavía no cae (V1: el hueco se reduce 0.15 m por lado)
            H2.Put(sim, 0, new Vec2(5.0f, -4.9f));
            var ev = H.Run(sim, 5);
            Assert.True(sim.Players[0].Alive);
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.KO);
            // 0.20 m adentro: ¡AL VACÍO!
            H2.Put(sim, 0, new Vec2(5.1f, -4.9f));
            ev = H.Run(sim, 1);
            Assert.False(sim.Players[0].Alive);
            var ko = H3.Ko(ev, 0);
            Assert.Equal((byte)KoCause.Vacio, ko.Extra);
            Assert.Equal(-1, ko.B); // nadie lo tocó: el KO no es de nadie
            Assert.Equal(0, sim.Players[1].Kos);
            Assert.Equal("¡AL VACÍO!", KoCauseInfo.Label(KoCause.Vacio));
            // el que queda en pie gana la ronda
            ev = H2.RunToRoundOver(sim);
            Assert.Equal(1, ev.Single(e => e.Type == SimEventType.RoundEnd).A);
        }

        [Fact]
        public void CaminandoHaciaUnVacioSeCae()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(0f, -4.3f));
            H2.Put(sim, 1, new Vec2(-8f, 6.5f));
            var walk = new InputCommand { Move = new Vec2(0f, 1f) };
            var ev = H.Run(sim, 40, t => H2.One(2, 0, walk));
            var ko = H3.Ko(ev, 0);
            Assert.Equal((byte)KoCause.Vacio, ko.Extra);
            Assert.InRange(ko.Pos.y, -3.45f + 0.15f, -3.45f + 0.15f + 0.12f); // cayó apenas pasó el perdón de 0.15 m
        }

        [Fact]
        public void ElDashCruzaUnVacio()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(0f, -4.3f));
            H2.Put(sim, 1, new Vec2(-8f, 6.5f));
            var p0 = sim.Players[0];
            bool wasOverPit = false;
            var ev = H.Run(sim, 40, t =>
            {
                if (sim.Arena.InPit(p0.Pos, sim.T.Pits.FallInset)) wasOverPit = true;
                var cmd = new InputCommand { Move = new Vec2(0f, 1f) };
                if (t == 0) cmd.Buttons = InputButtons.DashPressed;
                return H2.One(2, 0, cmd);
            });
            Assert.True(wasOverPit, "el dash debía pasar por encima de la zanja");
            Assert.True(p0.Alive, "un dash puede cruzar un vacío");
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.KO);
            Assert.True(p0.Pos.y > -2.4f, $"no cruzó: {p0.Pos}");
        }

        [Fact]
        public void SiElDashTerminaEncimaCae()
        {
            // Azotea: desde el puente de tablón, un dash hacia el hueco (0, 4.75, 1.8, 6.5) termina a media caída
            var sim = new MatchSimulation(H.Config(2, arena: "azotea"));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(0f, 0.6f));
            H2.Put(sim, 1, new Vec2(10f, 0f));
            int koTick = -1;
            var all = new List<SimEvent>();
            for (int t = 0; t < 30 && koTick < 0; t++)
            {
                var cmd = new InputCommand { Move = new Vec2(0f, 1f) };
                if (t == 0) cmd.Buttons = InputButtons.DashPressed;
                sim.Step(H2.One(2, 0, cmd));
                all.AddRange(sim.Events);
                if (sim.Events.Any(e => e.Type == SimEventType.KO)) koTick = t;
            }
            var ko = H3.Ko(all, 0);
            Assert.Equal((byte)KoCause.Vacio, ko.Extra);
            Assert.InRange(koTick, 8, 10);      // no cayó a medio dash: cayó al terminar (0.15 s = 9 ticks)
            Assert.InRange(ko.Pos.y, 3.0f, 3.7f); // 2.7 m de dash
        }

        [Fact]
        public void ElEmpujonAlVacioDaElCreditoDelKo()
        {
            var sim = new MatchSimulation(H.Config(2));
            H.SkipCountdown(sim);
            // coladera (−5.6, 4.9): P1 queda justo a 2.5 m (un empujón) de su centro
            H2.Put(sim, 0, new Vec2(-5.6f, 1.3f));
            H2.Put(sim, 1, new Vec2(-5.6f, 2.4f));
            H3.GroundCap(sim, 0, new Vec2(9f, -6.8f));
            var ev = H.Run(sim, 30, t => t == 0 ? H2.One(2, 0, H2.Melee(new Vec2(0f, 1f))) : H.Idle(2));
            Assert.Contains(ev, e => e.Type == SimEventType.Shove && e.A == 0 && e.B == 1);
            var ko = H3.Ko(ev, 1);
            Assert.Equal((byte)KoCause.Vacio, ko.Extra);
            Assert.Equal(0, ko.B);                 // el KO es de quien empujó
            Assert.Equal(1, sim.Players[0].Kos);
            Assert.InRange(ko.Pos.y, 4.8f, 5.0f);  // el empujón lo llevó 2.5 m: cayó al terminar, no antes
            Assert.True(sim.Players[0].Alive);
        }

        [Theory]
        [InlineData(60, 0)]    // 1 s después del empujón: el crédito sigue vivo
        [InlineData(130, -1)]  // 2.2 s después: ya no es de nadie
        public void ElCreditoDura2Segundos(int waitTicks, int expectedKiller)
        {
            var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, rules: new Rules { PowerUps = false }));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-6f, -6.8f));
            H2.Put(sim, 1, new Vec2(-4.9f, -6.8f));
            H3.GroundCap(sim, 0, new Vec2(-6f, -6.8f)); // la recoge enseguida; lo que importa es el empujón del primer tick
            var ev = H.Run(sim, 1, t => H2.One(2, 0, H2.Melee(new Vec2(1f, 0f))));
            Assert.Contains(ev, e => e.Type == SimEventType.Shove && e.B == 1);
            Assert.Equal(0, sim.Players[1].LastHitBy);
            H.Run(sim, waitTicks - 1);
            Assert.True(sim.Players[1].Alive);
            H2.Put(sim, 1, new Vec2(5.6f, -4.9f)); // al centro de la coladera
            ev = H.Run(sim, 1);
            var ko = H3.Ko(ev, 1);
            Assert.Equal((byte)KoCause.Vacio, ko.Extra);
            Assert.Equal(expectedKiller, ko.B);
            Assert.Equal(expectedKiller == 0 ? 1 : 0, sim.Players[0].Kos);
        }

        [Fact]
        public void DesviarLaGorraORomperElEscudoTambienDanCredito()
        {
            // "lo desvió": P1 le desvía la gorra a P0; si P0 cae en menos de 2 s, el KO es de P1
            var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, rules: new Rules { DeflectReturn = false, PowerUps = false }));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-6f, -6.8f));
            H2.Put(sim, 1, new Vec2(0f, -6.8f));
            H2.FlyCap(sim, 0, CapMode.Out, new Vec2(-1.6f, -6.8f), new Vec2(15f, 0f));
            var ev = H.Run(sim, 2, t => t == 0 ? H2.One(2, 1, H2.Melee(new Vec2(-1, 0))) : H.Idle(2));
            Assert.Contains(ev, e => e.Type == SimEventType.Deflect && e.A == 1 && e.B == 0);
            H2.Put(sim, 0, new Vec2(5.6f, -4.9f));
            Assert.Equal(1, H3.Ko(H.Run(sim, 1), 0).B);

            // "lo tocó con una gorra con escudo roto": el Parche aguanta el gorrazo, pero si luego cae el KO es de quien lanzó
            sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, rules: new Rules { PowerUps = false }));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-8f, -6.8f));
            H2.Put(sim, 1, new Vec2(-3f, -6.8f));
            sim.Players[1].Shield = true;
            ev = H.Run(sim, 40, t => t == 0 ? H2.One(2, 0, H.Throw(new Vec2(1, 0))) : H.Idle(2));
            Assert.Contains(ev, e => e.Type == SimEventType.ShieldBreak && e.A == 1);
            Assert.True(sim.Players[1].Alive);
            H2.Put(sim, 1, new Vec2(5.6f, -4.9f));
            var ko = H3.Ko(H.Run(sim, 1), 1);
            Assert.Equal((byte)KoCause.Vacio, ko.Extra);
            Assert.Equal(0, ko.B);
        }

        [Fact]
        public void LaGorraEnElPisoDentroDeUnVacioRegresaSola()
        {
            var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, rules: new Rules { PowerUps = false }));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-6f, -6.8f));
            H2.Put(sim, 1, new Vec2(10f, 6.8f));
            // junto al vacío (0.4 m afuera) se queda en el piso
            var cap = H3.GroundCap(sim, 0, new Vec2(4.5f, -4.9f));
            var ev = H.Run(sim, 30);
            Assert.Equal(CapMode.Ground, cap.Mode);
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.Recall);
            // dentro del vacío (aunque sea en la franja de 0.15 m de la orilla): regresa de inmediato, como un recall
            H3.GroundCap(sim, 0, new Vec2(5.0f, -4.9f));
            ev = H.Run(sim, 1);
            Assert.Contains(ev, e => e.Type == SimEventType.Recall && e.A == 0);
            Assert.Equal(CapMode.Recall, cap.Mode);
            ev = H.Run(sim, 120);
            Assert.True(sim.Players[0].HasCap);
            Assert.Contains(ev, e => e.Type == SimEventType.Catch && e.A == 0);
        }

        [Fact]
        public void LaGorraEnVueloPasaPorEncimaDelVacio()
        {
            var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, rules: new Rules { PowerUps = false }));
            H.SkipCountdown(sim);
            // la zanja (0, −3.0, 3.4, 0.9) queda entre los dos
            H2.Put(sim, 0, new Vec2(-2.6f, -3.0f));
            H2.Put(sim, 1, new Vec2(2.6f, -3.0f));
            bool overPit = false;
            var cap = sim.Players[0].Cap;
            var ev = H.Run(sim, 40, t =>
            {
                if (cap.Mode == CapMode.Out && sim.Arena.InPit(cap.Pos)) overPit = true;
                return t == 0 ? H2.One(2, 0, H.Throw(new Vec2(1, 0))) : H.Idle(2);
            });
            Assert.True(overPit);
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.Recall);
            Assert.Equal((byte)KoCause.Gorrazo, H3.Ko(ev, 1).Extra);
        }

        [Fact]
        public void LaGorraDeQuienYaCayoSePierdeEnElVacio()
        {
            var sim = new MatchSimulation(H2.Config(3, GameMode.Gorrazo, rules: new Rules { PowerUps = false }));
            H.SkipCountdown(sim);
            H.Place(sim, new Vec2(-6f, -6.8f), new Vec2(10f, 6.8f), new Vec2(-10f, 6.8f));
            var cap = H3.GroundCap(sim, 0, new Vec2(5.6f, -4.9f));
            sim.Players[0].Alive = false;
            var ev = H.Run(sim, 2);
            Assert.Equal(CapMode.Dead, cap.Mode);
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.Recall);
        }

        [Fact]
        public void ConLaRondaDecididaLosVaciosSonSolidos()
        {
            // ya declarada la ronda nadie más cae: el ganador no puede quedar "en el aire" sobre un hueco
            var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, rules: new Rules { PowerUps = false }));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(-6f, -6.8f));
            H2.Put(sim, 1, new Vec2(5.6f, -4.9f)); // P1 al vacío
            for (int i = 0; i < 60 && sim.Phase == MatchPhase.Play; i++) sim.Step(H.Idle(2));
            Assert.Equal(MatchPhase.RoundEnd, sim.Phase);
            H2.Put(sim, 0, new Vec2(5.3f, -4.9f)); // el ganador aparece encima del hueco
            var ev = H.Run(sim, 2);
            var p0 = sim.Players[0];
            Assert.True(p0.Alive);
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.KO);
            Assert.False(sim.Arena.InPit(p0.Pos));
            Assert.True(sim.Arena.PitDistance(p0.Pos) >= 0.54f, $"quedó a {sim.Arena.PitDistance(p0.Pos):0.00} m del hueco");
            Assert.Equal(0, H2.RunToRoundOver(sim).Single(e => e.Type == SimEventType.RoundEnd).A); // y se lleva su punto
            Assert.Equal(1, p0.Score);
        }

        [Fact]
        public void LosMurosDeMuerteSubitaEmpujanAlVacio()
        {
            // V6: P0 se queda quieto entre el muro derecho y la coladera (5.6, −4.9); el muro lo empuja al hueco
            var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, rules: new Rules { PowerUps = false }));
            H.SkipCountdown(sim);
            H2.Put(sim, 0, new Vec2(7.5f, -4.9f));
            H2.Put(sim, 1, new Vec2(-3f, 6.8f));
            var ev = H3.RunUntil(sim, SimEventType.KO, 60 * 44);
            Assert.Contains(ev, e => e.Type == SimEventType.SuddenDeathStart);
            var ko = H3.Ko(ev, 0);
            Assert.Equal((byte)KoCause.Vacio, ko.Extra);
            Assert.Equal(-1, ko.B);
            // el muro llega a x = 7.5 a los 30.9 s y lo mete 0.15 m al hueco (x = 6.15) a los 33.6 s
            Assert.InRange(sim.RoundTime, 33.3f, 33.9f);
            Assert.True(sim.Players[1].Alive);
        }
    }

    // ── A (V4) y B. Arenas y apariciones ─────────────────────
    public class ArenasV03Tests
    {
        [Fact]
        public void SonCuatroArenasEnOrdenConSusMedidas()
        {
            Assert.Equal(H3.Arenas, ArenaLibrary.All.Select(a => a.Id).ToArray());
            foreach (var id in new[] { "callejon", "museo", "azotea" })
            {
                Assert.Equal(26f, ArenaLibrary.Get(id).Width);
                Assert.Equal(16f, ArenaLibrary.Get(id).Depth);
            }
            var popup = ArenaLibrary.Get("popup");
            Assert.Equal("Pop-up Store INKCAP", popup.Name);
            Assert.Equal(24f, popup.Width);
            Assert.Equal(15f, popup.Depth);
            Assert.Equal(new[] { 4, 2, 2, 2 }, ArenaLibrary.All.Select(a => a.Pits.Length).ToArray());
            Assert.Equal(new[] { 7, 5, 6, 5 }, ArenaLibrary.All.Select(a => a.Obstacles.Length).ToArray());
            Assert.All(ArenaLibrary.All, a => Assert.Equal(6, a.Spawns.Length));

            // algunas medidas exactas de la spec
            var zanja = ArenaLibrary.Callejon.Pits[2];
            Assert.Equal((0f, 3.0f, 3.4f, 0.9f), (zanja.x, zanja.y, zanja.w, zanja.d));
            var foso = ArenaLibrary.Museo.Pits[0];
            Assert.Equal((-9.2f, 0f, 1.8f, 3.2f), (foso.x, foso.y, foso.w, foso.d));
            var hueco = ArenaLibrary.Azotea.Pits[1];
            Assert.Equal((0f, 4.75f, 1.8f, 6.5f), (hueco.x, hueco.y, hueco.w, hueco.d));
            var escalera = popup.Pits[1];
            Assert.Equal((9.6f, -4.6f, 1.6f, 2.2f), (escalera.x, escalera.y, escalera.w, escalera.d));
            // azotea: entre los dos huecos queda el puente de tablón de 3 m
            Assert.False(new Arena(ArenaLibrary.Azotea).InPit(new Vec2(0f, 1.4f)));
            Assert.True(new Arena(ArenaLibrary.Azotea).InPit(new Vec2(0f, 1.6f)));
            // la mesa de gorras es baja (las gorras pasan); los racks no
            Assert.False(popup.Obstacles.Single(o => o.Kind == "table").BlocksCaps);
            Assert.All(popup.Obstacles.Where(o => o.Kind == "rack"), o => Assert.True(o.BlocksCaps));

            // solo la Pop-up Store tiene caja DROP: cada 20 s, la primera a los 8 s, aviso de 1.5 s, radio 1.1 m
            Assert.Null(ArenaLibrary.Callejon.Drop); Assert.Null(ArenaLibrary.Museo.Drop); Assert.Null(ArenaLibrary.Azotea.Drop);
            Assert.Equal((8f, 20f, 1.5f, 1.1f), (popup.Drop.First, popup.Drop.Every, popup.Drop.Warn, popup.Drop.Radius));
            Assert.Single(ArenaLibrary.Museo.Hazards); // el láser del museo no cambia
        }

        [Fact]
        public void LosEventosYCausasNuevosVanAlFinal()
        {
            // los números viajan por la red: no se reordena nada de lo anterior
            Assert.Equal(26, (int)SimEventType.RetaNext);
            Assert.Equal(27, (int)SimEventType.DropWarn);
            Assert.Equal(28, (int)SimEventType.DropLand);
            Assert.Equal(7, (int)KoCause.Tiraje);
            Assert.Equal(8, (int)KoCause.Vacio);
            Assert.Equal(9, (int)KoCause.Drop);
            Assert.Equal("¡AL VACÍO!", KoCauseInfo.Label(KoCause.Vacio));
            Assert.Equal("¡DROP!", KoCauseInfo.Label(KoCause.Drop));
        }

        [Fact]
        public void NadieApareceEnUnVacioNiCercaDeUno()
        {
            foreach (var def in ArenaLibrary.All)
            {
                var arena = new Arena(def);
                foreach (var s in def.Spawns)
                {
                    Assert.True(arena.PitDistance(s) >= 0.8f, $"{def.Id}: spawn {s} a {arena.PitDistance(s):0.00} m de un vacío");
                    Assert.True(arena.IsFree(s, 0.55f), $"{def.Id}: spawn {s} choca con algo");
                }
                // al empezar cada ronda todos quedan en su spawn, vivos
                var sim = new MatchSimulation(H.Config(6, arena: def.Id));
                H.SkipCountdown(sim);
                Assert.Equal(6, sim.AliveCount);
                Assert.All(sim.Players, p => Assert.True(sim.Arena.PitDistance(p.Pos) >= 0.8f));
            }
        }

        [Fact]
        public void RandomFreePointRespetaLosVacios()
        {
            foreach (var def in ArenaLibrary.All)
            {
                // con la arena completa y con los muros de muerte súbita a medio cerrar y cerrados del todo
                foreach (float inset in new[] { 0f, 4f, def.Width / 2f - 4f })
                {
                    var arena = new Arena(def) { WallInset = inset };
                    var rng = new Rng(2026u + (uint)inset);
                    for (int i = 0; i < 2000; i++)
                    {
                        var p = arena.RandomFreePoint(ref rng, 1.0f);
                        Assert.False(arena.InPit(p), $"{def.Id}: {p} cayó dentro de un vacío");
                        Assert.True(arena.PitDistance(p) >= 0.8f, $"{def.Id}: {p} a {arena.PitDistance(p):0.00} m de un vacío");
                        Assert.True(arena.IsFree(p, 1.0f), $"{def.Id}: {p} no está libre");
                    }
                }
            }
        }

        [Fact]
        public void LasLatasYLaCajaDropNoAparecenEnVacios()
        {
            int pickups = 0, drops = 0;
            foreach (var id in H3.Arenas)
            {
                var sim = H2.BotMatch(GameMode.Gorrazo, 4, 2026, BotDifficulty.Normal, out var bots, id);
                var buf = new InputCommand[4];
                while (sim.Phase != MatchPhase.MatchEnd && sim.Tick < 60 * 60 * 15)
                {
                    H2.StepBots(sim, bots, buf);
                    foreach (var e in sim.Events)
                    {
                        if (e.Type != SimEventType.PickupSpawn && e.Type != SimEventType.DropWarn && e.Type != SimEventType.DropLand) continue;
                        if (e.Type == SimEventType.PickupSpawn) pickups++; else drops++;
                        Assert.False(sim.Arena.InPit(e.Pos), $"{id}: {e.Type} en {e.Pos} dentro de un vacío");
                        Assert.True(sim.Arena.PitDistance(e.Pos) >= 0.8f, $"{id}: {e.Type} en {e.Pos} a {sim.Arena.PitDistance(e.Pos):0.00} m de un vacío");
                    }
                }
            }
            Assert.True(pickups > 20 && drops > 4, $"muy pocas apariciones para probar: {pickups} latas, {drops} avisos/caídas de caja");
        }
    }

    // ── B4. Caja DROP ────────────────────────────────────────
    public class DropBoxV03Tests
    {
        static MatchSimulation Popup(int players, Rules rules = null, GameMode mode = GameMode.Gorrazo, uint seed = 777)
        {
            var sim = new MatchSimulation(H2.Config(players, mode, arena: "popup", seed: seed, rules: rules ?? new Rules()));
            for (int i = 0; i < 400 && sim.Phase != MatchPhase.Play; i++) sim.Step(H.Idle(players));
            Assert.Equal(MatchPhase.Play, sim.Phase);
            return sim;
        }

        /// <summary>El spawn más lejano al punto de caída (para dejar a alguien a salvo).</summary>
        static Vec2 FarFrom(MatchSimulation sim, Vec2 p) => sim.Arena.Def.Spawns.OrderByDescending(s => Vec2.Distance(s, p)).First();

        [Fact]
        public void LaCajaDropAvisaCaeYNoqueaElEscudoBloquea()
        {
            var sim = Popup(4, new Rules { PowerUps = false }); // latas apagadas: tampoco deja lata (eso se prueba abajo)
            var ev = H3.RunUntil(sim, SimEventType.DropWarn, 60 * 10);
            var warn = ev.Single(e => e.Type == SimEventType.DropWarn);
            Assert.InRange(sim.RoundTime, 6.49f, 6.53f);     // 1.5 s antes de los 8 s
            Assert.InRange(warn.Value, 1.47f, 1.51f);
            Assert.True(sim.DropPending);
            Assert.Equal(warn.Pos, sim.DropPos);
            Assert.InRange(sim.DropT, 1.47f, 1.51f);
            Assert.True(sim.Arena.IsFree(warn.Pos, 1.0f));
            Assert.True(sim.Arena.PitDistance(warn.Pos) >= 0.8f);
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.DropLand);

            // P1 debajo de la sombra, P2 debajo pero con Parche, P3 a 1.3 m (fuera del radio de 1.1 m), P0 lejos
            var c = sim.DropPos;
            H2.Put(sim, 0, FarFrom(sim, c));
            sim.Players[2].Shield = true;
            var p3 = H3.FreeSpotAt(sim, c, 1.3f);
            H2.Put(sim, 3, p3);
            // P1 y P2 quedan sobre la recta perpendicular a P3 para no estorbarse
            var side = (p3 - c).Normalized.Perp * 0.55f;
            H2.Put(sim, 1, c + side);
            H2.Put(sim, 2, c - side);
            ev = H3.RunUntil(sim, SimEventType.DropLand, 60 * 3);
            var land = ev.Single(e => e.Type == SimEventType.DropLand);
            Assert.InRange(sim.RoundTime, 7.99f, 8.03f);     // la primera cae a los 8 s
            Assert.Equal(c, land.Pos);
            Assert.Equal(1.1f, land.Value, 3);
            Assert.False(sim.DropPending);

            var ko = ev.Single(e => e.Type == SimEventType.KO);
            Assert.Equal(1, ko.A);
            Assert.Equal(-1, ko.B);
            Assert.Equal((byte)KoCause.Drop, ko.Extra);
            Assert.Equal("¡DROP!", KoCauseInfo.Label(KoCause.Drop));
            Assert.False(sim.Players[1].Alive);
            Assert.True(sim.Players[2].Alive, "el Parche bloquea la caja");
            Assert.False(sim.Players[2].Shield);
            Assert.Contains(ev, e => e.Type == SimEventType.ShieldBreak && e.A == 2);
            Assert.True(sim.Players[3].Alive, "a más de 1.1 m no pasa nada");
            Assert.True(Vec2.Distance(sim.Players[3].Pos, c) > 1.1f);
            Assert.True(sim.Players[0].Alive);
            Assert.Equal(0, land.Extra); // latas apagadas: no deja nada
            Assert.Empty(sim.Pickups);
        }

        [Fact]
        public void LaCajaDropDejaUnaLataYVuelveACaerCada20Segundos()
        {
            // latas encendidas, pero las normales no salen (su primer turno se alarga): la única lata será la de la caja
            var cfg = H.Config(2, arena: "popup", seed: 4321);
            cfg.Tuning.Pickups.FirstSpawn = 999f;
            var sim = new MatchSimulation(cfg);
            for (int i = 0; i < 400 && sim.Phase != MatchPhase.Play; i++) sim.Step(H.Idle(2));

            var ev = H3.RunUntil(sim, SimEventType.DropWarn, 60 * 10);
            var c = sim.DropPos;
            // los dos lejos de la sombra
            var far = sim.Arena.Def.Spawns.OrderByDescending(s => Vec2.Distance(s, c)).Take(2).ToArray();
            H2.Put(sim, 0, far[0]); H2.Put(sim, 1, far[1]);
            ev = H3.RunUntil(sim, SimEventType.DropLand, 60 * 3);
            var land = ev.Single(e => e.Type == SimEventType.DropLand);
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.KO);
            // deja una lata de técnica justo ahí
            var can = Assert.Single(sim.Pickups);
            Assert.Equal(c, can.Pos);
            Assert.InRange((int)can.Type, 1, PowerUpInfo.Count);
            Assert.Equal((byte)can.Type, land.Extra);
            Assert.Contains(ev, e => e.Type == SimEventType.PickupSpawn && e.Pos.Equals(c) && e.Extra == (byte)can.Type);

            // la siguiente: aviso a los 26.5 s, cae a los 28 s (ya con muerte súbita) y en otro punto elegido con Rng
            ev = H3.RunUntil(sim, SimEventType.DropWarn, 60 * 25);
            Assert.InRange(sim.RoundTime, 26.49f, 26.53f);
            Assert.True(MathF.Abs(sim.DropPos.x) < sim.Arena.PlayHalfW - 1f, "la sombra cae dentro de los muros de muerte súbita");
            var c2 = sim.DropPos;
            far = sim.Arena.Def.Spawns.Where(s => MathF.Abs(s.x) < sim.Arena.PlayHalfW - 1.5f).OrderByDescending(s => Vec2.Distance(s, c2)).Take(2).ToArray();
            H2.Put(sim, 0, far[0]); H2.Put(sim, 1, far[1]);
            ev = H3.RunUntil(sim, SimEventType.DropLand, 60 * 3);
            Assert.Contains(ev, e => e.Type == SimEventType.DropLand);
            Assert.InRange(sim.RoundTime, 27.99f, 28.03f);
            Assert.Contains(sim.Pickups, pu => pu.Pos.Equals(c2));
        }

        [Fact]
        public void EnCadaRondaLaCajaEmpiezaDeNuevoALos8Segundos()
        {
            var sim = Popup(2, new Rules { PowerUps = false });
            H3.RunUntil(sim, SimEventType.DropWarn, 60 * 10);
            Assert.True(sim.DropPending);
            // termina la ronda con la sombra puesta: P1 al vacío de la escalera (9.6, −4.6)
            H2.Put(sim, 1, new Vec2(9.6f, -4.6f));
            H2.Put(sim, 0, new Vec2(-9.5f, -5.5f));
            var ev = H2.RunToRoundOver(sim);
            Assert.Equal((byte)KoCause.Vacio, H3.Ko(ev, 1).Extra);
            Assert.DoesNotContain(ev, e => e.Type == SimEventType.DropLand); // con la ronda decidida la caja ya no cae
            for (int i = 0; i < 400 && sim.Phase != MatchPhase.Play; i++) sim.Step(H.Idle(2));
            Assert.False(sim.DropPending);
            ev = H3.RunUntil(sim, SimEventType.DropWarn, 60 * 10);
            Assert.Contains(ev, e => e.Type == SimEventType.DropWarn);
            Assert.InRange(sim.RoundTime, 6.49f, 6.53f);
        }

        [Fact]
        public void EnLasOtrasArenasNoCaeNada()
        {
            foreach (var id in new[] { "callejon", "museo", "azotea" })
            {
                var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, arena: id, rules: new Rules { SuddenDeath = false }));
                bool any = false;
                for (int i = 0; i < 60 * 14; i++)
                {
                    sim.Step(H.Idle(2));
                    any |= sim.DropPending || sim.Events.Any(e => e.Type == SimEventType.DropWarn || e.Type == SimEventType.DropLand);
                }
                Assert.False(any, $"{id}: no debería haber caja DROP");
            }
        }

        [Fact]
        public void EnLataCalienteLaCajaNoqueaPeroNoDejaLata()
        {
            var sim = Popup(3, mode: GameMode.LataCaliente);
            H3.RunUntil(sim, SimEventType.DropWarn, 60 * 10);
            var c = sim.DropPos;
            int victim = (sim.HotCanHolder + 1) % 3; // alguien que no trae la lata
            var far = sim.Arena.Def.Spawns.OrderByDescending(s => Vec2.Distance(s, c)).Take(2).ToArray();
            for (int i = 0, k = 0; i < 3; i++) H2.Put(sim, i, i == victim ? c : far[k++]);
            var ev = H3.RunUntil(sim, SimEventType.DropLand, 60 * 3);
            Assert.Equal((byte)KoCause.Drop, H3.Ko(ev, victim).Extra);
            Assert.Equal(0, ev.Single(e => e.Type == SimEventType.DropLand).Extra);
            Assert.Empty(sim.Pickups);
        }
    }

    // ── V5. Bots ─────────────────────────────────────────────
    public class BotsV03Tests
    {
        [Theory]
        [InlineData(BotDifficulty.Facil)]
        [InlineData(BotDifficulty.Normal)]
        [InlineData(BotDifficulty.Dificil)]
        public void LosBotsNoSeCaenSolos(BotDifficulty diff)
        {
            // spec §D: en 30 partidas de bots, menos del 15 % de los KOs son caídas al vacío sin crédito
            int kos = 0, pitNoCredit = 0, pitCredit = 0;
            for (int g = 0; g < 30; g++)
            {
                var sim = H2.BotMatch(GameMode.Gorrazo, 4, (uint)(500 + g * 7919), diff, out var bots, H3.Arenas[g % 4]);
                var buf = new InputCommand[4];
                while (sim.Phase != MatchPhase.MatchEnd && sim.Tick < 60 * 60 * 20)
                {
                    H2.StepBots(sim, bots, buf);
                    foreach (var e in sim.Events)
                    {
                        if (e.Type != SimEventType.KO) continue;
                        kos++;
                        if ((KoCause)e.Extra != KoCause.Vacio) continue;
                        if (e.B >= 0) pitCredit++; else pitNoCredit++;
                    }
                }
                Assert.Equal(MatchPhase.MatchEnd, sim.Phase);
            }
            Assert.True(kos > 300, $"muy pocos KOs: {kos}");
            double solo = 100.0 * pitNoCredit / kos;
            Assert.True(solo < 15.0, $"{diff}: {pitNoCredit} de {kos} KOs ({solo:0.0} %) fueron caídas sin crédito");
            // y en la práctica mucho menos: los bots tratan los vacíos como obstáculos
            Assert.True(solo < 3.0, $"{diff}: {solo:0.0} % de caídas sin crédito (se esperaba casi cero)");
            if (diff != BotDifficulty.Facil) Assert.True(pitCredit > 0, "los bots nunca empujaron a nadie a un vacío");
        }

        /// <summary>Un "humano" quieto que no puede caer por gorras ni golpes (Parche nuevo cada tick), para aislar al bot.</summary>
        static void StepWithImmortalDummy(MatchSimulation sim, BotBrain bot, InputCommand[] buf)
        {
            sim.Players[0].Shield = true;
            buf[1] = bot.Think(MatchSimulation.Dt);
            sim.Step(buf);
        }

        [Fact]
        public void ElBotSeAlejaDeLaOrillaDeUnVacio()
        {
            // Azotea: el bot empieza a 0.3 m del hueco entre edificios (orilla en x = 0.9) y su rival está del otro lado,
            // así que "quiere" acercarse al hueco. Aun así sale de la franja de 0.8 m y no vuelve a meterse.
            for (uint seed = 1; seed <= 10; seed++)
            {
                var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, bots: 1, arena: "azotea", seed: seed, rules: new Rules { PowerUps = false }));
                H.SkipCountdown(sim);
                H2.Put(sim, 1, new Vec2(1.2f, 4.5f));
                H2.Put(sim, 0, new Vec2(-6f, 4.5f));
                var bot = new BotBrain(sim, 1, BotDifficulty.Normal);
                var buf = new InputCommand[2];
                float minDist = float.MaxValue;
                for (int t = 0; t < 240; t++)
                {
                    StepWithImmortalDummy(sim, bot, buf);
                    Assert.True(sim.Players[1].Alive, $"semilla {seed}: el bot se cayó en el tick {t}");
                    if (t >= 30) minDist = MathF.Min(minDist, sim.Arena.PitDistance(sim.Players[1].Pos));
                }
                Assert.True(minDist >= 0.7f, $"semilla {seed}: el bot se quedó a {minDist:0.00} m de la orilla");
            }
        }

        [Fact]
        public void ElBotNoHaceDashHaciaUnVacio()
        {
            // El bot está junto al hueco de la azotea y le llegan gorras cargadas que lo invitan a esquivar.
            // Puede esquivar con dash hacia el lado libre o caminando, pero ningún dash suyo pasa sobre un vacío.
            int dashes = 0;
            for (uint seed = 1; seed <= 60; seed++)
            {
                // sin empujones: así el bot no manda al "humano" al hueco y la prueba solo mide sus dashes
                var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, bots: 1, arena: "azotea", seed: seed, rules: new Rules { PowerUps = false, Shove = false }));
                H.SkipCountdown(sim);
                H2.Put(sim, 1, new Vec2(2.2f, 5f));     // bot: el hueco queda a su izquierda (x de −0.9 a 0.9)
                H2.Put(sim, 0, new Vec2(2.2f + (seed % 2 == 0 ? 0.5f : -0.5f), -2.5f)); // quien le lanza, un poco a un lado o al otro
                var bot = new BotBrain(sim, 1, BotDifficulty.Dificil);
                var buf = new InputCommand[2];
                for (int t = 0; t < 150 && sim.Phase == MatchPhase.Play; t++)
                {
                    // el "humano" carga a tope (0.55 s) y suelta hacia el bot, una y otra vez
                    var p0 = sim.Players[0];
                    var aim = (sim.Players[1].Pos - p0.Pos).Normalized;
                    if (p0.HasCap && !p0.Charging) buf[0] = new InputCommand { Aim = aim, Buttons = InputButtons.HasAim | InputButtons.ThrowPressed | InputButtons.ThrowHeld };
                    else if (p0.Charging && p0.ChargeT < 0.56f) buf[0] = new InputCommand { Aim = aim, Buttons = InputButtons.HasAim | InputButtons.ThrowHeld };
                    else if (p0.Charging) buf[0] = new InputCommand { Aim = aim, Buttons = InputButtons.HasAim | InputButtons.ThrowReleased };
                    else buf[0] = default;
                    var from = sim.Players[1].Pos;
                    StepWithImmortalDummy(sim, bot, buf);
                    if ((buf[1].Buttons & InputButtons.DashPressed) != 0 && sim.Events.Any(e => e.Type == SimEventType.Dash && e.A == 1))
                    {
                        dashes++;
                        var dir = buf[1].Move.Normalized;
                        Assert.False(sim.Arena.SegmentCrossesPit(from, from + dir * 3.0f), $"semilla {seed}: dash hacia un vacío desde {from} en dirección {dir}");
                    }
                    Assert.DoesNotContain(sim.Events, e => e.Type == SimEventType.KO && e.Extra == (byte)KoCause.Vacio);
                }
            }
            Assert.True(dashes > 5, $"el bot casi nunca esquivó con dash ({dashes}): la prueba no prueba nada");
        }

        [Fact]
        public void ElBotSaleDeLaSombraDeLaCajaDrop()
        {
            int tested = 0;
            for (uint seed = 1; seed <= 6; seed++)
            {
                var sim = new MatchSimulation(H2.Config(2, GameMode.Gorrazo, bots: 1, arena: "popup", seed: seed, rules: new Rules { PowerUps = false, Shove = false }));
                var bot = new BotBrain(sim, 1, BotDifficulty.Normal);
                var buf = new InputCommand[2];
                bool placed = false;
                for (int t = 0; t < 60 * 12; t++)
                {
                    StepWithImmortalDummy(sim, bot, buf);
                    if (sim.Events.Any(e => e.Type == SimEventType.DropWarn))
                    {
                        // el bot justo debajo de la sombra; el humano (quieto) lejos
                        H2.Put(sim, 1, sim.DropPos);
                        H2.Put(sim, 0, sim.Arena.Def.Spawns.OrderByDescending(s => Vec2.Distance(s, sim.DropPos)).First());
                        placed = true;
                    }
                    if (!sim.Events.Any(e => e.Type == SimEventType.DropLand)) continue;
                    Assert.True(placed);
                    Assert.True(sim.Players[1].Alive, $"semilla {seed}: al bot le cayó la caja");
                    Assert.True(Vec2.Distance(sim.Players[1].Pos, sim.Events.First(e => e.Type == SimEventType.DropLand).Pos) > 1.1f);
                    tested++;
                    break;
                }
            }
            Assert.Equal(6, tested);
        }

        [Fact]
        public void PartidaCompletaDeBotsEnCadaUnaDeLasCuatroArenas()
        {
            foreach (var id in H3.Arenas)
            {
                var sim = H2.BotMatch(GameMode.Gorrazo, 4, 99, BotDifficulty.Normal, out var bots, id);
                var buf = new InputCommand[4];
                while (sim.Phase != MatchPhase.MatchEnd && sim.Tick < 60 * 60 * 15) H2.StepBots(sim, bots, buf);
                Assert.Equal(MatchPhase.MatchEnd, sim.Phase);
                Assert.Equal(5, sim.Players[sim.MatchWinnerId].Score);
                Assert.True(sim.RoundDurations.Average() is > 3f and < 45f, $"{id}: ronda promedio {sim.RoundDurations.Average():0.0}s");
            }
        }

        [Theory]
        [InlineData(GameMode.LataCaliente, 5)]
        [InlineData(GameMode.LaReta, 6)]
        [InlineData(GameMode.RetoDelDia, 4)]
        public void LosOtrosModosTambienTerminanEnLaPopupStore(GameMode mode, int n)
        {
            var sim = H2.BotMatch(mode, n, 99, BotDifficulty.Normal, out var bots, "popup");
            var buf = new InputCommand[n];
            while (sim.Phase != MatchPhase.MatchEnd && sim.Tick < 60 * 60 * 25) H2.StepBots(sim, bots, buf);
            Assert.Equal(MatchPhase.MatchEnd, sim.Phase);
            Assert.InRange(sim.MatchWinnerId, 0, n - 1);
        }
    }

    // ── Determinismo, snapshot y red ─────────────────────────
    public class DeterminismAndSnapshotV03Tests
    {
        [Theory]
        [InlineData("callejon")]
        [InlineData("museo")]
        [InlineData("azotea")]
        [InlineData("popup")]
        public void MismaSemillaMismoResultadoEnCadaArena(string arena)
        {
            var a = H2.BotMatch(GameMode.Gorrazo, 4, 4242, BotDifficulty.Dificil, out var ba, arena);
            var b = H2.BotMatch(GameMode.Gorrazo, 4, 4242, BotDifficulty.Dificil, out var bb, arena);
            var ia = new InputCommand[4]; var ib = new InputCommand[4];
            bool sawDrop = false;
            for (int t = 0; t < 5000; t++)
            {
                H2.StepBots(a, ba, ia); H2.StepBots(b, bb, ib);
                Assert.Equal(a.StateHash(), b.StateHash());
                sawDrop |= a.Events.Any(e => e.Type == SimEventType.DropLand);
            }
            Assert.Equal(arena == "popup", sawDrop);
        }

        [Fact]
        public void OtraSemillaOtraCajaDrop()
        {
            // el punto de caída sale del Rng de la partida: con otra semilla cae en otro lado
            Vec2 FirstDrop(uint seed)
            {
                var sim = new MatchSimulation(H.Config(2, arena: "popup", seed: seed));
                return H3.RunUntil(sim, SimEventType.DropWarn, 60 * 12).Single(e => e.Type == SimEventType.DropWarn).Pos;
            }
            Assert.Equal(FirstDrop(5), FirstDrop(5));
            Assert.NotEqual(FirstDrop(5), FirstDrop(6));
        }

        [Fact]
        public void ElSnapshotLlevaLaCajaDrop()
        {
            Assert.Equal(3, MatchSimulation.SnapshotVersion);
            var sim = new MatchSimulation(H.Config(4, arena: "popup"));
            H3.RunUntil(sim, SimEventType.DropWarn, 60 * 12);
            H.Run(sim, 20);
            Assert.True(sim.DropPending);

            var ms = new MemoryStream();
            sim.WriteSnapshot(new BinaryWriter(ms));
            Assert.InRange(ms.Length, 50, 1400);
            var copy = new MatchSimulation(sim.Config);
            Assert.False(copy.DropPending);
            ms.Position = 0;
            copy.ReadSnapshot(new BinaryReader(ms));
            Assert.True(copy.DropPending);
            Assert.Equal(sim.DropPos, copy.DropPos);
            Assert.Equal(sim.DropT, copy.DropT);
            Assert.InRange(copy.DropT, 1.1f, 1.2f); // 20 ticks después del aviso de 1.5 s
            Assert.Equal(sim.RoundTime, copy.RoundTime);
            // los vacíos no viajan: salen de la misma arena en los dos lados
            Assert.Equal(sim.Arena.Def.Pits.Length, copy.Arena.Def.Pits.Length);

            // cuando cae, la foto ya no trae sombra y sí trae la lata que dejó
            H3.RunUntil(sim, SimEventType.DropLand, 60 * 3);
            ms = new MemoryStream();
            sim.WriteSnapshot(new BinaryWriter(ms));
            ms.Position = 0;
            copy.ReadSnapshot(new BinaryReader(ms));
            Assert.False(copy.DropPending);
            Assert.Equal(sim.Pickups.Count, copy.Pickups.Count);
            Assert.Contains(copy.Pickups, pu => pu.Pos.Equals(sim.DropPos));
        }

        [Fact]
        public void UnSnapshotViejoSeRechaza()
        {
            var sim = new MatchSimulation(H.Config(2, arena: "popup"));
            var ms = new MemoryStream();
            sim.WriteSnapshot(new BinaryWriter(ms));
            var bytes = ms.ToArray();
            bytes[0] = 2; // versión anterior
            Assert.Throws<InvalidDataException>(() => sim.ReadSnapshot(new BinaryReader(new MemoryStream(bytes))));
        }

        [Fact]
        public void ElClienteVeLaCajaDropPorLaRed()
        {
            var (ht, ct) = LoopbackTransport.Pair(latency: 0.03f);
            var host = new HostSession(ht, "Anfitrión");
            var client = new ClientSession(ct, "Invitado");
            void Pump(int ticks, Action each = null)
            {
                for (int t = 0; t < ticks; t++)
                {
                    ht.Advance(MatchSimulation.Dt); ct.Advance(MatchSimulation.Dt);
                    host.Poll(MatchSimulation.Dt); client.Poll(MatchSimulation.Dt);
                    client.SendInput(default);
                    host.Tick(default);
                    each?.Invoke();
                }
            }
            Pump(20);
            host.StartMatch(host.BuildConfig("popup", 3, 2026));
            var events = new List<SimEvent>();
            client.EventsReceived += evs => events.AddRange(evs);
            bool sawShadow = false;
            Pump(60 * 12, () =>
            {
                if (client.Sim == null || !client.Sim.DropPending) return;
                sawShadow = true;
                if (host.Sim.DropPending) Assert.Equal(host.Sim.DropPos, client.Sim.DropPos);
            });
            Assert.Equal("popup", client.Sim.Config.ArenaId);
            Assert.Equal(2, client.Sim.Arena.Def.Pits.Length);
            Assert.True(sawShadow, "el cliente nunca vio la sombra de la caja");
            var warn = events.First(e => e.Type == SimEventType.DropWarn);
            var land = events.First(e => e.Type == SimEventType.DropLand);
            Assert.Equal(warn.Pos, land.Pos);
        }
    }
}
