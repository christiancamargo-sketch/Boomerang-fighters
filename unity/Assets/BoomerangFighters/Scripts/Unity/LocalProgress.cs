// ─────────────────────────────────────────────────────────────
// PROGRESO LOCAL (spec v0.2 §E): todo vive en el celular, sin backend.
// XP y niveles 1–50, Gotas (moneda blanda), primera victoria del día,
// premio diario de 7 días con racha SUAVE, calendario de 28 días
// ("Playera del mes"), inventario y equipado, tabla local del Reto
// del Día (7 días) y las preferencias del menú.
//
// Este archivo es C# puro (sin UnityEngine) para poder probarlo con
// `dotnet test`. Guardar y cargar el JSON lo hace ProgressStore.cs.
// Mismos números que prototipo/src/progress.js y config.js.
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;

namespace BoomerangFighters.UnityView
{
    /// <summary>Lo que trae puesto el jugador.</summary>
    [Serializable]
    public sealed class OutfitData
    {
        public string Shirt = "basica-blanca";
        public string Cap = "clasica";
        public int Species = 0;      // índice de ProgressRules.SpeciesNames
        public string Sticker = "";
        public string Emote = "";
    }

    /// <summary>Mejor puntaje del Reto del Día para una semilla AAAAMMDD.</summary>
    [Serializable]
    public sealed class RetoEntry
    {
        public int Seed;
        public int Best;
    }

    /// <summary>Lo último que eligió el jugador en el menú (para no volver a configurar todo).</summary>
    [Serializable]
    public sealed class SetupPrefs
    {
        public string PlayerName = "Jugador";
        public int Mode = 0;              // Core.GameMode
        public string ArenaId = "callejon"; // o "aleatorio"
        public int Humans = 1;
        public int Bots = 3;
        public int Difficulty = 1;        // Core.BotDifficulty
        public int Points = 5;            // 3 / 5 / 7
        public string RoomName = "Sala de Tinta";
        // interruptores (Core.Rules)
        public bool SuddenDeath = true;
        public bool Shove = true;
        public bool DeflectReturn = true;
        public bool GraffitiWall = true;
        public bool PowerUps = true;
    }

    /// <summary>
    /// Todo el progreso guardado. Solo campos públicos y listas: así lo
    /// entiende JsonUtility de Unity (no usar Dictionary ni propiedades).
    /// </summary>
    [Serializable]
    public sealed class ProgressData
    {
        public int Version = 1;
        public int Level = 1;
        public int Xp = 0;
        public int Gotas = 100;
        public List<string> Shirts = new List<string> { "basica-blanca", "basica-negra" };
        public List<string> Caps = new List<string> { "clasica" };
        public List<string> Stickers = new List<string>();
        public List<string> Emotes = new List<string>();
        public OutfitData Equipped = new OutfitData();
        // premio diario (racha suave): último día reclamado y cuántos premios lleva
        public string DailyLast = "";
        public int DailyClaimed = 0;
        // calendario de 28 días
        public int CalendarStamps = 0;
        public int CalendarMonths = 0;
        public string FirstWinDay = "";
        public int Matches = 0, Wins = 0, Kos = 0;
        public List<string> Redeemed = new List<string>();
        public string PlayerId = "";
        public List<RetoEntry> Reto = new List<RetoEntry>();
        public SetupPrefs Setup = new SetupPrefs();

        /// <summary>Repara listas nulas (JSON viejo o editado a mano).</summary>
        public void Normalize()
        {
            if (Shirts == null || Shirts.Count == 0) Shirts = new List<string> { "basica-blanca", "basica-negra" };
            if (Caps == null || Caps.Count == 0) Caps = new List<string> { "clasica" };
            if (Stickers == null) Stickers = new List<string>();
            if (Emotes == null) Emotes = new List<string>();
            if (Equipped == null) Equipped = new OutfitData();
            if (Redeemed == null) Redeemed = new List<string>();
            if (Reto == null) Reto = new List<RetoEntry>();
            if (Setup == null) Setup = new SetupPrefs();
            if (DailyLast == null) DailyLast = "";
            if (FirstWinDay == null) FirstWinDay = "";
            if (PlayerId == null) PlayerId = "";
            Level = Math.Max(1, Math.Min(ProgressRules.MaxLevel, Level));
            Xp = Math.Max(0, Xp);
            Gotas = Math.Max(0, Gotas);
            DailyClaimed = Math.Max(0, DailyClaimed);
            if (Setup.Points != 3 && Setup.Points != 5 && Setup.Points != 7) Setup.Points = 5;
            if (Equipped.Species < 0 || Equipped.Species >= ProgressRules.SpeciesNames.Length) Equipped.Species = 0;
        }
    }

    public struct XpLine
    {
        public string Label;
        public int Xp;
        public XpLine(string label, int xp) { Label = label; Xp = xp; }
    }

    /// <summary>Lo que ganó el jugador en una partida (para la pantalla de resultados).</summary>
    public sealed class MatchReward
    {
        public readonly List<XpLine> Lines = new List<XpLine>();
        public int TotalXp, Gotas, LevelsGained, LevelBefore, XpBefore;
        public bool FirstWin;
        public readonly List<string> Unlocked = new List<string>();
    }

    public sealed class DailyResult
    {
        public int Day;          // 1..7
        public int Streak;       // premios reclamados en total
        public readonly List<string> Got = new List<string>();
        public bool MonthShirt;
    }

    public struct RetoRow
    {
        public int Seed;
        public string Label;
        public int? Best;
        public bool Today;
    }

    /// <summary>Un premio del ciclo de 7 días.</summary>
    public struct DailyReward
    {
        public int Gotas;
        public bool Sticker, Emote, WeeklyCap;
        public string Text;
    }

    public struct Cosmetic
    {
        public string Id, Name, Text;
        public Cosmetic(string id, string name, string text) { Id = id; Name = name; Text = text; }
    }

    /// <summary>Números y reglas de la progresión local (spec §E, GDD §10).</summary>
    public static class ProgressRules
    {
        public const int MaxLevel = 50;
        public const int XpParticipate = 20, XpRoundWin = 10, XpKo = 6, XpMatchWin = 50, XpFirstWinOfDay = 50;
        public const int GotasBase = 10, GotasMax = 25;
        public const int CalendarDays = 28;
        public const string MonthShirtId = "playera-del-mes";

        /// <summary>XP para pasar del nivel n al n+1: 150 + 50 × (n − 1).</summary>
        public static int XpForLevel(int level) => 150 + 50 * (level - 1);

        /// <summary>Gotas por partida: 10–25 según el desempeño.</summary>
        public static int MatchGotas(bool won, int kos, int roundWins)
            => Math.Min(GotasMax, GotasBase + (won ? 8 : 0) + roundWins * 2 + kos);

        public static readonly DailyReward[] Daily =
        {
            new DailyReward { Gotas = 150, Text = "150 Gotas" },
            new DailyReward { Sticker = true, Text = "Sticker" },
            new DailyReward { Gotas = 250, Text = "250 Gotas" },
            new DailyReward { Gotas = 25, Text = "25 Gotas extra" },
            new DailyReward { Emote = true, Text = "Emote" },
            new DailyReward { Gotas = 100, Text = "100 Gotas" },
            new DailyReward { WeeklyCap = true, Text = "Gorra de la semana" },
        };

        public static readonly Cosmetic[] StickerList =
        {
            new Cosmetic("st-gotita", "Gotita feliz", "GOTITA"),
            new Cosmetic("st-ko", "KO de tinta", "¡KO!"),
            new Cosmetic("st-crew", "Crew", "CREW"),
            new Cosmetic("st-cdmx", "Chilango", "CDMX"),
        };

        public static readonly Cosmetic[] EmoteList =
        {
            new Cosmetic("em-orale", "¡Órale!", "¡ÓRALE!"),
            new Cosmetic("em-quionda", "¿Qué onda?", "¿QUÉ ONDA?"),
            new Cosmetic("em-ahi", "Ahí nos vidrios", "¡AHÍ NOS VIDRIOS!"),
            new Cosmetic("em-chido", "¡Qué chido!", "¡QUÉ CHIDO!"),
        };

        // Muñequitos (mismo orden que prototipo/src/characters.js):
        // Osote, Michi, Gotita nv2, Seri nv4, Zurci nv6, Pegote nv10, Lienzo, Axo nv15
        public static readonly string[] SpeciesNames = { "Osote", "Michi", "Gotita", "Seri", "Zurci", "Pegote", "Lienzo", "Axo" };
        public static readonly int[] SpeciesUnlock = { 1, 1, 2, 4, 6, 10, 1, 15 };
        /// <summary>Orden en el vestidor: gratis primero, luego por nivel.</summary>
        public static readonly int[] SpeciesOrder = { 6, 0, 1, 2, 3, 4, 5, 7 };

        // Códigos de demostración (simulación del portal web; en producción cada
        // código impreso en la etiqueta es único, de un solo uso y se valida en línea).
        public const string DemoCode = "TINTA-2026";
        public const string DemoCodeShirt = "edicion-limitada";

        public static string DayKey(DateTime d) => d.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        /// <summary>Semilla del día con formato AAAAMMDD (hora local del celular).</summary>
        public static int DaySeed(DateTime d) => d.Year * 10000 + d.Month * 100 + d.Day;
    }

    /// <summary>Operaciones sobre ProgressData. "today" se pasa siempre para poder probarlas.</summary>
    public static class LocalProgress
    {
        static int AddXp(ProgressData d, int amount)
        {
            int levels = 0;
            if (d.Level >= ProgressRules.MaxLevel) return 0;
            d.Xp += amount;
            while (d.Level < ProgressRules.MaxLevel && d.Xp >= ProgressRules.XpForLevel(d.Level))
            {
                d.Xp -= ProgressRules.XpForLevel(d.Level);
                d.Level++;
                levels++;
            }
            if (d.Level >= ProgressRules.MaxLevel) d.Xp = 0;
            return levels;
        }

        public static float LevelProgress01(ProgressData d)
            => d.Level >= ProgressRules.MaxLevel ? 1f : Math.Min(1f, d.Xp / (float)ProgressRules.XpForLevel(d.Level));

        /// <summary>Resultado de la partida del jugador local → XP, Gotas y desbloqueos.</summary>
        public static MatchReward RecordMatch(ProgressData d, bool won, int kos, int roundWins, DateTime today)
        {
            var r = new MatchReward { LevelBefore = d.Level, XpBefore = d.Xp };
            r.Lines.Add(new XpLine("Por jugar", ProgressRules.XpParticipate));
            if (roundWins > 0) r.Lines.Add(new XpLine($"Rondas ganadas ×{roundWins}", roundWins * ProgressRules.XpRoundWin));
            if (kos > 0) r.Lines.Add(new XpLine($"Noqueos ×{kos}", kos * ProgressRules.XpKo));
            if (won) r.Lines.Add(new XpLine("¡Ganaste la partida!", ProgressRules.XpMatchWin));
            string key = ProgressRules.DayKey(today);
            if (won && d.FirstWinDay != key)
            {
                r.FirstWin = true;
                d.FirstWinDay = key;
                r.Lines.Add(new XpLine("Primera victoria del día", ProgressRules.XpFirstWinOfDay));
            }
            foreach (var l in r.Lines) r.TotalXp += l.Xp;
            r.Gotas = ProgressRules.MatchGotas(won, kos, roundWins);
            d.Gotas += r.Gotas;
            r.LevelsGained = AddXp(d, r.TotalXp);
            d.Matches++;
            if (won) d.Wins++;
            d.Kos += kos;
            for (int i = 0; i < ProgressRules.SpeciesUnlock.Length; i++)
            {
                int need = ProgressRules.SpeciesUnlock[i];
                if (need > r.LevelBefore && need <= d.Level) r.Unlocked.Add(ProgressRules.SpeciesNames[i]);
            }
            return r;
        }

        // ── Premio diario (racha suave) ──────────────────────
        // Si faltas un día la racha se PAUSA: al volver sigues en el día que te tocaba.
        public static bool ClaimedToday(ProgressData d, DateTime today) => d.DailyLast == ProgressRules.DayKey(today);

        /// <summary>Día del ciclo (1..7) que toca reclamar hoy (o el que ya se reclamó hoy).</summary>
        public static int NextDailyDay(ProgressData d, DateTime today)
            => ClaimedToday(d, today) ? ((d.DailyClaimed - 1) % 7) + 1 : (d.DailyClaimed % 7) + 1;

        /// <summary>Reclama el premio de hoy. null si ya se reclamó. weeklyCapId = diseño de la gorra de la semana.</summary>
        public static DailyResult ClaimDaily(ProgressData d, DateTime today, string weeklyCapId, string weeklyCapName = null)
        {
            if (ClaimedToday(d, today)) return null;
            int claimed = d.DailyClaimed + 1;
            var reward = ProgressRules.Daily[(claimed - 1) % 7];
            int cycle = (claimed - 1) / 7;
            d.DailyLast = ProgressRules.DayKey(today);
            d.DailyClaimed = claimed;
            var res = new DailyResult { Day = ((claimed - 1) % 7) + 1, Streak = claimed };
            if (reward.Gotas > 0) { d.Gotas += reward.Gotas; res.Got.Add($"{reward.Gotas} Gotas"); }
            if (reward.Sticker)
            {
                var s = FirstMissing(ProgressRules.StickerList, d.Stickers, cycle);
                if (!d.Stickers.Contains(s.Id)) d.Stickers.Add(s.Id);
                if (string.IsNullOrEmpty(d.Equipped.Sticker)) d.Equipped.Sticker = s.Id;
                res.Got.Add($"Sticker \"{s.Name}\"");
            }
            if (reward.Emote)
            {
                var e = FirstMissing(ProgressRules.EmoteList, d.Emotes, cycle);
                if (!d.Emotes.Contains(e.Id)) d.Emotes.Add(e.Id);
                if (string.IsNullOrEmpty(d.Equipped.Emote)) d.Equipped.Emote = e.Id;
                res.Got.Add($"Emote \"{e.Name}\"");
            }
            if (reward.WeeklyCap && !string.IsNullOrEmpty(weeklyCapId))
            {
                if (!d.Caps.Contains(weeklyCapId)) d.Caps.Add(weeklyCapId);
                else d.Gotas += 100; // si ya la tenías, te damos Gotas
                res.Got.Add($"Gorra de la semana \"{weeklyCapName ?? weeklyCapId}\"");
            }
            // calendario de 28 días → Playera del mes
            d.CalendarStamps++;
            if (d.CalendarStamps >= ProgressRules.CalendarDays)
            {
                d.CalendarStamps = 0;
                d.CalendarMonths++;
                if (!d.Shirts.Contains(ProgressRules.MonthShirtId)) d.Shirts.Add(ProgressRules.MonthShirtId);
                else d.Gotas += 300;
                res.MonthShirt = true;
                res.Got.Add("¡Playera del mes!");
            }
            return res;
        }

        static Cosmetic FirstMissing(Cosmetic[] list, List<string> owned, int cycle)
        {
            foreach (var c in list) if (!owned.Contains(c.Id)) return c;
            return list[cycle % list.Length];
        }

        // ── Vestidor ─────────────────────────────────────────
        public static bool Owns(ProgressData d, string id, bool cap) => (cap ? d.Caps : d.Shirts).Contains(id);

        /// <summary>Compra con Gotas. false si ya lo tiene o no alcanza.</summary>
        public static bool TryBuy(ProgressData d, string id, int price, bool cap)
        {
            var list = cap ? d.Caps : d.Shirts;
            if (string.IsNullOrEmpty(id) || list.Contains(id) || price < 0 || d.Gotas < price) return false;
            d.Gotas -= price;
            list.Add(id);
            return true;
        }

        public static bool SpeciesUnlocked(ProgressData d, int species)
            => species >= 0 && species < ProgressRules.SpeciesUnlock.Length && d.Level >= ProgressRules.SpeciesUnlock[species];

        /// <summary>Canje de código (simulación del portal web). Devuelve el mensaje para mostrar.</summary>
        public static bool Redeem(ProgressData d, string raw, out string message)
        {
            string code = (raw ?? "").Trim().ToUpperInvariant();
            if (code != ProgressRules.DemoCode) { message = "Ese código no existe. Revisa la etiqueta de tu playera."; return false; }
            if (d.Redeemed.Contains(code)) { message = "Ese código ya se canjeó en este dispositivo."; return false; }
            d.Redeemed.Add(code);
            if (!d.Shirts.Contains(ProgressRules.DemoCodeShirt)) d.Shirts.Add(ProgressRules.DemoCodeShirt);
            d.Equipped.Shirt = ProgressRules.DemoCodeShirt;
            message = "Desbloqueaste \"Edición Limitada 001\" y ya la traes puesta.";
            return true;
        }

        // ── Reto del Día: mejor puntaje del día y tabla de 7 días ──
        public static RetoRow[] RetoTable(ProgressData d, DateTime today)
        {
            var rows = new RetoRow[7];
            for (int i = 0; i < 7; i++)
            {
                var day = today.Date.AddDays(-i);
                int seed = ProgressRules.DaySeed(day);
                int? best = null;
                foreach (var e in d.Reto) if (e.Seed == seed) best = e.Best;
                rows[i] = new RetoRow { Seed = seed, Best = best, Today = i == 0, Label = i == 0 ? "Hoy" : i == 1 ? "Ayer" : $"{day.Day}/{day.Month}" };
            }
            return rows;
        }

        /// <summary>Guarda el puntaje si es el mejor de hoy y borra lo de hace más de 7 días.</summary>
        public static bool RecordReto(ProgressData d, int score, DateTime today, out int best, out int? previous)
        {
            int seed = ProgressRules.DaySeed(today);
            RetoEntry entry = null;
            foreach (var e in d.Reto) if (e.Seed == seed) entry = e;
            previous = entry?.Best;
            bool isBest = entry == null || score > entry.Best;
            if (entry == null) d.Reto.Add(entry = new RetoEntry { Seed = seed, Best = score });
            else if (isBest) entry.Best = score;
            best = entry.Best;
            var keep = new HashSet<int>();
            foreach (var row in RetoTable(d, today)) keep.Add(row.Seed);
            d.Reto.RemoveAll(e => !keep.Contains(e.Seed));
            return isBest;
        }

        /// <summary>Semana del año (lunes a domingo), igual que weeklyCapId() del prototipo web.</summary>
        public static int WeekOfYear(DateTime date)
        {
            var d = date.Date;
            var jan1 = new DateTime(d.Year, 1, 1);
            int offset = ((int)jan1.DayOfWeek + 6) % 7;
            return ((d - jan1).Days + offset) / 7;
        }
    }
}
