// ─────────────────────────────────────────────────────────────
// "Tuning": todos los números que definen cómo se siente el juego.
// Valores probados en el prototipo web (rondas de ~16 s con 4 jugadores).
// En Unity se envuelven en un ScriptableObject (TuningAsset) para
// ajustarlos desde el Inspector sin tocar código.
// ─────────────────────────────────────────────────────────────
using System;

namespace BoomerangFighters.Core
{
    [Serializable]
    public sealed class PlayerTuning
    {
        public float Radius = 0.55f;
        public float Speed = 6.4f;
        public float Accel = 70f;
        public float ChargeMoveMult = 0.55f;
        public float DashSpeed = 18f;
        public float DashTime = 0.15f;
        public float DashCooldown = 0.9f;
        public float MeleeRange = 1.75f;
        public float MeleeArc = MathF.PI * 0.62f;
        public float MeleeActive = 0.13f;
        public float MeleeTime = 0.22f;
        public float MeleeCooldown = 0.45f;
        public float SlowMult = 0.5f;
        public float SpawnInvuln = 0.6f;
        // A3: empujón sin gorra (2.5 m en 0.2 s, recarga 0.8 s; patea 4 m una gorra caída ajena)
        public float ShoveDistance = 2.5f;
        public float ShoveTime = 0.2f;
        public float ShoveCooldown = 0.8f;
        public float ShoveKickDistance = 4f;
    }

    [Serializable]
    public sealed class CapTuning
    {
        public float Radius = 0.38f;
        public float FlyHeight = 1.05f;
        public float SpeedMin = 13f;
        public float SpeedMax = 22f;
        public float ChargeTime = 0.55f;
        public float TapThreshold = 0.12f;
        public float TapCharge = 0.3f;
        public float OutTimeMin = 0.34f;
        public float OutTimeMax = 0.62f;
        public float ReturnSpeed = 17f;
        public float ReturnAccel = 45f;
        public float TurnRate = 8.5f;
        public float RecallSpeed = 15f;
        public float CatchRadius = 0.95f;
        public float EarlyCatchDelay = 0.12f;
        public float GroundFriction = 6f;
        public float AimAssistAngle = 0.6f;
        public float AimAssistRange = 17f;
        public float TirajeGap = 0.85f;      // Serigrafía "Tiraje": separación de las copias en fila
        public float GroundAutoReturn = 3f;  // la gorra tirada regresa sola a los 3 s
        public float HomingTurn = 4.5f;      // Tatuaje
        public float HomingRange = 11f;
        public float ExplosionRadius = 2.0f; // Tinta derramada · Charco: radio final
        public float PuddleGrowTime = 0.4f;  // el charco crece 0.4 s (visible) y luego noquea
        public float GroundFadeTime = 1.2f;
        public float StickerScale = 2f;      // Sticker XL: gorra al doble
        public float StickerSpeedMult = 0.9f; // A5: Sticker XL 10 % más lenta
        public float ClankRadiusMult = 0.85f; // A6: radio de choque gorra-gorra = 85 % de la suma
        // A4: "Te la regreso"
        public float DeflectSpeedMult = 1.2f;
        public float DeflectMaxSpeed = 22f;
        public float DeflectLethalTime = 0.6f;
        // Grafiti · Muro de Tag
        public float TagWallLength = 2.5f;
        public float TagWallThickness = 0.4f;
        public float TagWallLife = 3f;
    }

    [Serializable]
    public sealed class RoundTuning
    {
        public float Countdown = 2.2f;
        public float EndDelay = 1.9f;
        public float BetweenRounds = 0.9f;
        public float SlowMoTime = 0.9f;
        public float SlowMoScale = 0.3f;
        public int HitStopTicks = 5;         // ~80 ms congelado al noquear
        public int DoubleKoTicks = 6;        // A1: si el último cae antes de 6 ticks (0.1 s), empate
        // A2: muerte súbita
        public float SuddenDeathAt = 20f;
        public float SuddenDeathWarn = 1f;
        public float SuddenDeathWallSpeed = 0.5f;
        public float SuddenDeathMinWidth = 8f;
        public float SuddenDeathTieAt = 45f;
    }

    [Serializable]
    public sealed class ModeTuning
    {
        // Lata Caliente
        public float HotCanMin = 10f;
        public float HotCanMax = 20f;
        public float HotCanImmunity = 0.5f;
        // La Reta
        public int RetaStreakToWin = 3;
        public int RetaMaxRounds = 10;
        // Reto del Día: puntaje = KOs × 100 + rondas ganadas × 300 − segundos de juego (fase Play, redondeados como en la web)
        public int RetoKoPoints = 100;
        public int RetoRoundPoints = 300;
    }

    /// <summary>Vacíos (spec v0.3, sección A).</summary>
    [Serializable]
    public sealed class PitTuning
    {
        public float FallInset = 0.15f;   // V1: el centro debe entrar 0.15 m al hueco para caer (perdón en la orilla)
        public float CreditTime = 2f;     // V2: si te empujaron/golpearon/desviaron hace menos de 2 s, el KO es de esa persona
        public float SafeMargin = 0.8f;   // V4/V5: nada aparece a menos de 0.8 m de un vacío; los bots se alejan de esa franja
    }

    [Serializable]
    public sealed class PickupTuning
    {
        public float FirstSpawn = 4f;
        public float EveryMin = 6f;
        public float EveryMax = 10f;
        public int Max = 2;
        public float PickRadius = 1.0f;
        public float PaintSlowRadius = 0.65f;
        public float PaintLife = 4.5f;
        public float PaintInterval = 0.035f;
    }

    [Serializable]
    public sealed class Tuning
    {
        public PlayerTuning Player = new PlayerTuning();
        public CapTuning Cap = new CapTuning();
        public RoundTuning Round = new RoundTuning();
        public PickupTuning Pickups = new PickupTuning();
        public ModeTuning Modes = new ModeTuning();
        public PitTuning Pits = new PitTuning();

        public static Tuning Default() => new Tuning();
    }

    /// <summary>Power-ups = las 6 técnicas de tinta.</summary>
    public enum PowerUp : byte
    {
        None = 0,
        Serigrafia = 1, // "tiraje": 2 copias en fila
        Tatuaje = 2,    // teledirigida
        Tinta = 3,      // charco que crece y noquea
        Grafiti = 4,    // muro de tag (respaldo: rastro que alenta)
        Bordado = 5,    // escudo de un golpe
        Sticker = 6,    // gorra gigante
    }

    public static class PowerUpInfo
    {
        public const int Count = 6;

        public static string Name(PowerUp p)
        {
            switch (p)
            {
                case PowerUp.Serigrafia: return "Serigrafía";
                case PowerUp.Tatuaje: return "Tatuaje";
                case PowerUp.Tinta: return "Tinta derramada";
                case PowerUp.Grafiti: return "Grafiti";
                case PowerUp.Bordado: return "Parche bordado";
                case PowerUp.Sticker: return "Sticker XL";
                default: return "";
            }
        }

        public static string Effect(PowerUp p)
        {
            switch (p)
            {
                case PowerUp.Serigrafia: return "Tiraje";
                case PowerUp.Tatuaje: return "Teledirigida";
                case PowerUp.Tinta: return "Charco";
                case PowerUp.Grafiti: return "Muro de Tag";
                case PowerUp.Bordado: return "Escudo";
                case PowerUp.Sticker: return "Gorra gigante";
                default: return "";
            }
        }

        /// <summary>Color en formato 0xRRGGBB (la vista lo convierte a Color).</summary>
        public static uint Color(PowerUp p)
        {
            switch (p)
            {
                case PowerUp.Serigrafia: return 0xFF4FA3;
                case PowerUp.Tatuaje: return 0x6D83FF;
                case PowerUp.Tinta: return 0x9AA0A6;
                case PowerUp.Grafiti: return 0x00E676;
                case PowerUp.Bordado: return 0xFFB300;
                case PowerUp.Sticker: return 0xFFFFFF;
                default: return 0xFFFFFF;
            }
        }
    }

    public enum BotDifficulty : byte { Facil = 0, Normal = 1, Dificil = 2 }

    public struct BotProfile
    {
        public float Reaction, AimError, DodgeChance, ThrowRate, Lead;
        public float DeflectChance; // D: probabilidad de intentar desviar una gorra que viene

        public static BotProfile For(BotDifficulty d)
        {
            switch (d)
            {
                case BotDifficulty.Facil: return new BotProfile { Reaction = 0.55f, AimError = 0.32f, DodgeChance = 0.18f, ThrowRate = 0.9f, Lead = 0.3f, DeflectChance = 0.15f };
                case BotDifficulty.Dificil: return new BotProfile { Reaction = 0.17f, AimError = 0.07f, DodgeChance = 0.75f, ThrowRate = 2.1f, Lead = 1.0f, DeflectChance = 0.6f };
                default: return new BotProfile { Reaction = 0.32f, AimError = 0.16f, DodgeChance = 0.45f, ThrowRate = 1.4f, Lead = 0.75f, DeflectChance = 0.35f };
            }
        }
    }
}
