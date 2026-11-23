// ─────────────────────────────────────────────────────────────
// Pruebas del progreso local (spec v0.2 §E) y del Reto del Día (§C)
// de la capa Unity. Son C# puro (LocalProgress.cs y DailyChallenge.cs
// no usan UnityEngine), así se prueban sin abrir Unity.
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using BoomerangFighters.Core;
using BoomerangFighters.UnityView;
using Xunit;

namespace BoomerangFighters.Tests
{
    public class ProgressTests
    {
        static readonly DateTime Day1 = new DateTime(2026, 11, 23, 10, 0, 0);

        [Fact]
        public void CurvaDeXpYTopeDeNivel()
        {
            Assert.Equal(150, ProgressRules.XpForLevel(1));
            Assert.Equal(200, ProgressRules.XpForLevel(2));
            Assert.Equal(150 + 50 * 48, ProgressRules.XpForLevel(49));
            var d = new ProgressData { Level = 49, Xp = 0 };
            for (int i = 0; i < 200; i++) LocalProgress.RecordMatch(d, true, 5, 5, Day1.AddDays(i));
            Assert.Equal(ProgressRules.MaxLevel, d.Level);
            Assert.Equal(0, d.Xp);
        }

        [Fact]
        public void XpYGotasPorPartida()
        {
            var d = new ProgressData();
            // ganó, 3 KOs, 5 rondas: 20 + 50 + 18 + 50 + 50 (primera del día) = 188
            var r = LocalProgress.RecordMatch(d, true, 3, 5, Day1);
            Assert.Equal(188, r.TotalXp);
            Assert.True(r.FirstWin);
            Assert.Equal(Math.Min(25, 10 + 8 + 10 + 3), r.Gotas);
            Assert.Equal(1, r.LevelsGained); // 188 ≥ 150
            Assert.Equal(2, d.Level);
            Assert.Equal(38, d.Xp);
            Assert.Contains("Gotita", r.Unlocked); // Gotita se desbloquea en nivel 2

            // segunda victoria del mismo día: ya no hay bono
            var r2 = LocalProgress.RecordMatch(d, true, 0, 0, Day1.AddHours(2));
            Assert.False(r2.FirstWin);
            Assert.Equal(20 + 50, r2.TotalXp);
            // perder solo da lo de jugar
            var r3 = LocalProgress.RecordMatch(d, false, 0, 0, Day1.AddDays(1));
            Assert.Equal(20, r3.TotalXp);
            Assert.Equal(10, r3.Gotas);
            Assert.Equal(3, d.Matches);
            Assert.Equal(2, d.Wins);
        }

        [Fact]
        public void GotasEntre10y25()
        {
            Assert.Equal(10, ProgressRules.MatchGotas(false, 0, 0));
            Assert.Equal(25, ProgressRules.MatchGotas(true, 20, 7));
        }

        [Fact]
        public void PremioDiarioConRachaSuave()
        {
            var d = new ProgressData();
            int gotas0 = d.Gotas;
            var a = LocalProgress.ClaimDaily(d, Day1, "semana-tinta");
            Assert.NotNull(a);
            Assert.Equal(1, a.Day);
            Assert.Equal(gotas0 + 150, d.Gotas);
            Assert.Null(LocalProgress.ClaimDaily(d, Day1.AddHours(5), "semana-tinta")); // una vez por día
            Assert.Equal(1, LocalProgress.NextDailyDay(d, Day1));

            // falta 3 días: la racha se PAUSA (sigue en el día 2, no regresa al 1)
            var later = Day1.AddDays(4);
            Assert.Equal(2, LocalProgress.NextDailyDay(d, later));
            var b = LocalProgress.ClaimDaily(d, later, "semana-tinta");
            Assert.Equal(2, b.Day);
            Assert.Single(d.Stickers);
            Assert.Equal(d.Stickers[0], d.Equipped.Sticker);

            // días 3..7: el 7 da la gorra de la semana
            for (int i = 1; i <= 5; i++) LocalProgress.ClaimDaily(d, later.AddDays(i), "semana-rosa", "INKCAP Rosa");
            Assert.Contains("semana-rosa", d.Caps);
            Assert.Single(d.Emotes);
            Assert.Equal(7, d.DailyClaimed);
            // el ciclo vuelve a empezar en el día 1
            Assert.Equal(1, LocalProgress.NextDailyDay(d, later.AddDays(6)));
        }

        [Fact]
        public void CalendarioDe28DiasDaLaPlayeraDelMes()
        {
            var d = new ProgressData();
            for (int i = 0; i < 27; i++) Assert.False(LocalProgress.ClaimDaily(d, Day1.AddDays(i), "semana-tinta").MonthShirt);
            Assert.DoesNotContain(ProgressRules.MonthShirtId, d.Shirts);
            var last = LocalProgress.ClaimDaily(d, Day1.AddDays(27), "semana-tinta");
            Assert.True(last.MonthShirt);
            Assert.Contains(ProgressRules.MonthShirtId, d.Shirts);
            Assert.Equal(0, d.CalendarStamps);
            Assert.Equal(1, d.CalendarMonths);
        }

        [Fact]
        public void VestidorComprasYMunequitos()
        {
            var d = new ProgressData { Gotas = 250 };
            Assert.False(LocalProgress.TryBuy(d, "rayas-lucha", 500, false)); // no alcanza
            Assert.True(LocalProgress.TryBuy(d, "cuadros", 80, true));
            Assert.False(LocalProgress.TryBuy(d, "cuadros", 80, true));       // ya la tiene
            Assert.Equal(170, d.Gotas);
            Assert.True(LocalProgress.Owns(d, "cuadros", true));
            Assert.True(LocalProgress.SpeciesUnlocked(d, 6));  // Lienzo gratis
            Assert.False(LocalProgress.SpeciesUnlocked(d, 7)); // Axo nivel 15
            d.Level = 15;
            Assert.True(LocalProgress.SpeciesUnlocked(d, 7));
        }

        [Fact]
        public void CanjeSimuladoDeCodigo()
        {
            var d = new ProgressData();
            Assert.False(LocalProgress.Redeem(d, "NO-EXISTE", out _));
            Assert.True(LocalProgress.Redeem(d, " tinta-2026 ", out _));
            Assert.Equal(ProgressRules.DemoCodeShirt, d.Equipped.Shirt);
            Assert.False(LocalProgress.Redeem(d, "TINTA-2026", out var msg));
            Assert.Contains("ya se canjeó", msg);
        }

        [Fact]
        public void TablaDelRetoGuardaElMejorYSoloSieteDias()
        {
            var d = new ProgressData();
            Assert.True(LocalProgress.RecordReto(d, 500, Day1, out int best, out var prev));
            Assert.Null(prev);
            Assert.False(LocalProgress.RecordReto(d, 300, Day1, out best, out prev));
            Assert.Equal(500, best);
            Assert.True(LocalProgress.RecordReto(d, 900, Day1, out best, out _));
            Assert.Equal(900, best);
            for (int i = 1; i <= 8; i++) LocalProgress.RecordReto(d, i, Day1.AddDays(i), out _, out _);
            var rows = LocalProgress.RetoTable(d, Day1.AddDays(8));
            Assert.Equal(7, rows.Length);
            Assert.Equal(20261201, rows[0].Seed);
            Assert.Equal("Hoy", rows[0].Label);
            Assert.Equal(8, rows[0].Best);
            Assert.True(d.Reto.Count <= 7);
            Assert.DoesNotContain(d.Reto, e => e.Seed == 20261123); // el de hace 8 días ya se borró
        }

        [Fact]
        public void NormalizaJsonIncompleto()
        {
            var d = new ProgressData { Shirts = null, Caps = new List<string>(), Equipped = null, Setup = null, Reto = null, Level = 99 };
            d.Normalize();
            Assert.Contains("basica-blanca", d.Shirts);
            Assert.Contains("clasica", d.Caps);
            Assert.NotNull(d.Equipped);
            Assert.NotNull(d.Setup);
            Assert.Equal(ProgressRules.MaxLevel, d.Level);
        }

        [Fact]
        public void SemanaDelAnioComoElPrototipoWeb()
        {
            // 1-ene-2026 es jueves: la semana 0 va del lun 29-dic al dom 4-ene
            Assert.Equal(0, LocalProgress.WeekOfYear(new DateTime(2026, 1, 4)));
            Assert.Equal(1, LocalProgress.WeekOfYear(new DateTime(2026, 1, 5)));
            Assert.Equal(47, LocalProgress.WeekOfYear(new DateTime(2026, 11, 23)));
        }
    }

    public class DailyChallengeTests
    {
        [Theory]
        // valores calculados con prototipo/src/rng.js + DAILY_RULES (mismo Rng que el núcleo)
        [InlineData(2026, 11, 23, 20261123, "azotea", "sin-muerte-subita")]
        [InlineData(2026, 12, 1, 20261201, "callejon", "lluvia-latas")]
        [InlineData(2027, 1, 15, 20270115, "azotea", "sin-empujon")]
        [InlineData(2026, 2, 28, 20260228, "azotea", "aerosol")]
        public void MismaArenaYReglaQueLaWeb(int y, int m, int d, int seed, string arena, string rule)
        {
            var rd = DailyChallenge.For(new DateTime(y, m, d, 18, 30, 0));
            Assert.Equal(seed, rd.Seed);
            Assert.Equal(arena, rd.ArenaId);
            Assert.Equal(rule, rd.Rule.Id);
        }

        [Fact]
        public void ConfigDelRetoEsUnoContraTresDificiles()
        {
            var rd = DailyChallenge.For(new DateTime(2026, 12, 1));
            var cfg = rd.BuildConfig(new PlayerSlot { Name = "TÚ", Species = 2 }, Tuning.Default());
            Assert.Equal(GameMode.RetoDelDia, cfg.Mode);
            Assert.Equal(20261201u, cfg.Seed);
            Assert.Equal(4, cfg.Slots.Count);
            Assert.False(cfg.Slots[0].IsBot);
            for (int i = 1; i < 4; i++) { Assert.True(cfg.Slots[i].IsBot); Assert.Equal(BotDifficulty.Dificil, cfg.Slots[i].Difficulty); }
            Assert.Equal(3, cfg.PointsToWin);
            Assert.Equal(3, cfg.Tuning.Pickups.Max); // lluvia de latas
            Assert.Equal(3f, cfg.Tuning.Pickups.EveryMin);

            // la partida corre y es determinista con la semilla del día
            uint Run()
            {
                var c = rd.BuildConfig(new PlayerSlot { Name = "TÚ" }, Tuning.Default());
                c.Slots[0].IsBot = true; // el "humano" lo maneja un bot para la prueba
                var sim = new MatchSimulation(c);
                var bots = new BotBrain[4];
                for (int i = 0; i < 4; i++) bots[i] = new BotBrain(sim, i, BotDifficulty.Dificil);
                var inputs = new InputCommand[4];
                for (int t = 0; t < 60 * 30; t++) { for (int i = 0; i < 4; i++) inputs[i] = bots[i].Think(MatchSimulation.Dt); sim.Step(inputs); }
                return sim.StateHash();
            }
            Assert.Equal(Run(), Run());
        }

        [Fact]
        public void ReglaDelDiaApagaSuInterruptor()
        {
            var rd = DailyChallenge.For(new DateTime(2026, 11, 23)); // sin muerte súbita
            var rules = rd.BuildRules();
            Assert.False(rules.SuddenDeath);
            Assert.True(rules.Shove && rules.DeflectReturn && rules.GraffitiWall && rules.PowerUps);
        }
    }
}
