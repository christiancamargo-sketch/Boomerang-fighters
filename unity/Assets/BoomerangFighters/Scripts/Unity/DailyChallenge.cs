// ─────────────────────────────────────────────────────────────
// RETO DEL DÍA (spec v0.2 §C): 1 jugador contra 3 bots en Difícil,
// con la semilla del día AAAAMMDD. La arena y la "regla del día" salen
// de esa semilla con el mismo Rng del núcleo y en el mismo orden que el
// prototipo web (primero la arena, luego la regla), así todos juegan
// lo mismo el mismo día. Sin servidor: el mejor puntaje se guarda en
// el celular (LocalProgress.RecordReto).
// C# puro (sin UnityEngine) para poder probarlo con dotnet test.
// ─────────────────────────────────────────────────────────────
using System;
using BoomerangFighters.Core;

namespace BoomerangFighters.UnityView
{
    public sealed class DailyRule
    {
        public string Id, Name;
        public bool NoSuddenDeath, NoShove, NoDeflectReturn, NoGraffitiWall;
        public bool RainOfCans; // "Lluvia de latas": salen cada 3–5 s, hasta 3 a la vez
    }

    public sealed class DailyChallenge
    {
        // Mismo orden que DAILY_RULES en prototipo/src/config.js
        public static readonly DailyRule[] Rules =
        {
            new DailyRule { Id = "todo", Name = "Todo vale" },
            new DailyRule { Id = "sin-empujon", Name = "Sin empujón", NoShove = true },
            new DailyRule { Id = "desvio-clasico", Name = "Desvío clásico", NoDeflectReturn = true },
            new DailyRule { Id = "aerosol", Name = "Rastro de aerosol", NoGraffitiWall = true },
            new DailyRule { Id = "sin-muerte-subita", Name = "Sin muerte súbita", NoSuddenDeath = true },
            new DailyRule { Id = "lluvia-latas", Name = "Lluvia de latas", RainOfCans = true },
        };

        public const int Bots = 3;
        public const int PointsToWin = 3;
        public const BotDifficulty Difficulty = BotDifficulty.Dificil;

        public int Seed;          // AAAAMMDD
        public string ArenaId;
        public DailyRule Rule;

        public static DailyChallenge For(DateTime localDate)
        {
            int seed = ProgressRules.DaySeed(localDate);
            var r = new Rng((uint)seed);
            var arena = ArenaLibrary.All[r.NextInt(ArenaLibrary.All.Length)];
            var rule = Rules[r.NextInt(Rules.Length)];
            return new DailyChallenge { Seed = seed, ArenaId = arena.Id, Rule = rule };
        }

        public Rules BuildRules() => new Rules
        {
            SuddenDeath = !Rule.NoSuddenDeath,
            Shove = !Rule.NoShove,
            DeflectReturn = !Rule.NoDeflectReturn,
            GraffitiWall = !Rule.NoGraffitiWall,
            PowerUps = true,
        };

        /// <summary>
        /// Arma la partida del reto. "tuning" debe ser una copia propia (se modifica
        /// con la regla del día). El jugador humano siempre es el índice 0.
        /// </summary>
        public MatchConfig BuildConfig(PlayerSlot human, Tuning tuning)
        {
            var t = tuning ?? Tuning.Default();
            if (Rule.RainOfCans) { t.Pickups.EveryMin = 3f; t.Pickups.EveryMax = 5f; t.Pickups.Max = 3; }
            var cfg = new MatchConfig
            {
                ArenaId = ArenaId, PointsToWin = PointsToWin, Seed = (uint)Seed,
                Mode = GameMode.RetoDelDia, Rules = BuildRules(), Tuning = t,
            };
            human.IsBot = false;
            cfg.Slots.Add(human);
            for (int i = 0; i < Bots; i++)
                cfg.Slots.Add(new PlayerSlot { Name = $"BOT {i + 1}", IsBot = true, Difficulty = Difficulty, Species = (human.Species + 1 + i) % ProgressRules.SpeciesNames.Length });
            return cfg;
        }
    }
}
