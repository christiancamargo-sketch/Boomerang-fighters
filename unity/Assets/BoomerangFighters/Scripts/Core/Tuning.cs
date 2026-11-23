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
        public float GhostSpread = 0.3f;     // Serigrafía: ángulo de las copias
        public float HomingTurn = 4.5f;      // Tatuaje
        public float HomingRange = 11f;
        public float ExplosionRadius = 2.3f; // Tinta derramada
        public float GroundFadeTime = 1.2f;
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

        public static Tuning Default() => new Tuning();
    }

    /// <summary>Power-ups = las 6 técnicas de tinta.</summary>
    public enum PowerUp : byte
    {
        None = 0,
        Serigrafia = 1, // triple tiro
        Tatuaje = 2,    // teledirigida
        Tinta = 3,      // explosiva
        Grafiti = 4,    // rastro que alenta
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
                case PowerUp.Serigrafia: return "Triple tiro";
                case PowerUp.Tatuaje: return "Teledirigida";
                case PowerUp.Tinta: return "Explosiva";
                case PowerUp.Grafiti: return "Rastro de aerosol";
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

        public static BotProfile For(BotDifficulty d)
        {
            switch (d)
            {
                case BotDifficulty.Facil: return new BotProfile { Reaction = 0.55f, AimError = 0.32f, DodgeChance = 0.18f, ThrowRate = 0.9f, Lead = 0.3f };
                case BotDifficulty.Dificil: return new BotProfile { Reaction = 0.17f, AimError = 0.07f, DodgeChance = 0.75f, ThrowRate = 2.1f, Lead = 1.0f };
                default: return new BotProfile { Reaction = 0.32f, AimError = 0.16f, DodgeChance = 0.45f, ThrowRate = 1.4f, Lead = 0.75f };
            }
        }
    }
}
