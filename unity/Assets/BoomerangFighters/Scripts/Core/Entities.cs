// ─────────────────────────────────────────────────────────────
// Entradas, entidades y eventos de la simulación.
// Regla de oro: los jugadores (humanos, bots o remotos) solo mandan
// InputCommand; la simulación decide todo lo demás.
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;

namespace BoomerangFighters.Core
{
    [Flags]
    public enum InputButtons : byte
    {
        None = 0,
        HasAim = 1 << 0,
        ThrowHeld = 1 << 1,
        ThrowPressed = 1 << 2,
        ThrowReleased = 1 << 3,
        MeleePressed = 1 << 4,
        DashPressed = 1 << 5,
        WantsAssist = 1 << 6,
    }

    /// <summary>Lo que un jugador quiere hacer en un tick. Cabe en 5 bytes para la red.</summary>
    [Serializable]
    public struct InputCommand
    {
        public Vec2 Move;   // -1..1
        public Vec2 Aim;    // dirección (si HasAim)
        public InputButtons Buttons;

        public const int WireSize = 5;

        public bool Has(InputButtons b) => (Buttons & b) != 0;
        public bool HasAim => Has(InputButtons.HasAim);
        public bool ThrowHeld => Has(InputButtons.ThrowHeld);
        public bool ThrowPressed => Has(InputButtons.ThrowPressed);
        public bool ThrowReleased => Has(InputButtons.ThrowReleased);
        public bool MeleePressed => Has(InputButtons.MeleePressed);
        public bool DashPressed => Has(InputButtons.DashPressed);
        public bool WantsAssist => Has(InputButtons.WantsAssist);

        /// <summary>Quita los botones "de un solo cuadro" (para sub-pasos sin entrada nueva).</summary>
        public InputCommand WithoutEdges()
        {
            var c = this;
            c.Buttons &= ~(InputButtons.ThrowPressed | InputButtons.ThrowReleased | InputButtons.MeleePressed | InputButtons.DashPressed);
            return c;
        }

        static sbyte Q(float v) => (sbyte)Math.Round(SimMath.Clamp(v, -1f, 1f) * 127f);
        static float DQ(sbyte v) => v / 127f;

        public void Write(byte[] buf, int offset)
        {
            var aim = Aim.Normalized;
            buf[offset] = (byte)Q(Move.x);
            buf[offset + 1] = (byte)Q(Move.y);
            buf[offset + 2] = (byte)Q(aim.x);
            buf[offset + 3] = (byte)Q(aim.y);
            buf[offset + 4] = (byte)Buttons;
        }

        public static InputCommand Read(byte[] buf, int offset)
        {
            return new InputCommand
            {
                Move = new Vec2(DQ((sbyte)buf[offset]), DQ((sbyte)buf[offset + 1])),
                Aim = new Vec2(DQ((sbyte)buf[offset + 2]), DQ((sbyte)buf[offset + 3])),
                Buttons = (InputButtons)buf[offset + 4],
            };
        }
    }

    public enum CapMode : byte { Held = 0, Out = 1, Return = 2, Ground = 3, Recall = 4, Dead = 5 }

    /// <summary>La gorra-proyectil. Cada jugador tiene una; Serigrafía crea copias "fantasma".</summary>
    [Serializable]
    public sealed class CapState
    {
        public int Id;
        public int OwnerId;
        public bool Ghost;
        public CapMode Mode;
        public Vec2 Pos;
        public Vec2 Vel;
        public float Y;
        public float T;
        public float V0;
        public float OutTime;
        public float RetSpeed;
        public float Radius;
        public float Fade;
        public float TrailT;
        public PowerUp Power;

        public bool Lethal => Mode == CapMode.Out || Mode == CapMode.Return || Mode == CapMode.Recall;
    }

    [Serializable]
    public sealed class PlayerState
    {
        public int Id;
        public string Name;
        public bool IsBot;
        public int Species;
        public string Shirt;

        public Vec2 Pos, Vel, Facing, AimDir, DashDir;
        public bool Alive, HasCap, Charging, Aiming, Shield;
        public float DashT, DashCd, MeleeT, MeleeCd, ChargeT, SlowT, SpawnInvuln;
        public uint MeleeHitMask;
        public PowerUp Power;
        public int Score, Kos, RoundWins;
        public CapState Cap;

        public float Charge01(Tuning t) => Charging ? SimMath.Clamp01(ChargeT / t.Cap.ChargeTime) : 0f;
    }

    [Serializable]
    public sealed class Pickup
    {
        public int Id;
        public PowerUp Type;
        public Vec2 Pos;
        public float T;
    }

    [Serializable]
    public struct PaintPatch
    {
        public Vec2 Pos;
        public float Radius;
        public int OwnerId;
        public float Life;
    }

    public enum SimEventType : byte
    {
        Throw, Catch, Recall, Dash, Melee,
        KO, Clank, Deflect, ShieldBreak, Explode,
        PickupSpawn, PickupTaken, Paint,
        LaserWarn, Countdown, RoundStart, RoundEnd, MatchEnd,
    }

    public enum KoCause : byte { Cap = 0, Ghost = 1, Melee = 2, Ink = 3, Laser = 4 }

    /// <summary>
    /// Algo que pasó en un tick. La presentación los usa para sonido, partículas,
    /// manchas de tinta y vibración; la red los reenvía a los clientes.
    /// </summary>
    [Serializable]
    public struct SimEvent
    {
        public SimEventType Type;
        public sbyte A;      // jugador principal (víctima, quien lanza...) o -1
        public sbyte B;      // jugador secundario (quien noquea...) o -1
        public byte Extra;   // causa, power-up, número de cuenta regresiva...
        public Vec2 Pos;
        public float Value;

        public static SimEvent Make(SimEventType type, int a = -1, int b = -1, Vec2 pos = default, float value = 0f, byte extra = 0)
            => new SimEvent { Type = type, A = (sbyte)a, B = (sbyte)b, Pos = pos, Value = value, Extra = extra };
    }

    public enum MatchPhase : byte { Countdown = 0, Play = 1, RoundEnd = 2, Between = 3, MatchEnd = 4 }

    [Serializable]
    public sealed class PlayerSlot
    {
        public string Name = "Jugador";
        public bool IsBot;
        public BotDifficulty Difficulty = BotDifficulty.Normal;
        public int Species;
        public string Shirt = "basica-blanca";
    }

    [Serializable]
    public sealed class MatchConfig
    {
        public string ArenaId = "callejon";
        public int PointsToWin = 5;
        public uint Seed = 12345;
        public List<PlayerSlot> Slots = new List<PlayerSlot>();
        public Tuning Tuning = Tuning.Default();
    }
}
