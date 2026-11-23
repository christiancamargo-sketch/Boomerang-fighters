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
        /// <summary>Ya rebotó en una pared en este tiro (para el letrero ¡DE BANDA!).</summary>
        public bool Bounced;
        /// <summary>"Te la regreso": segundos que le quedan a la gorra desviada para poder noquear a su propio dueño.</summary>
        public float DeflectT;
        /// <summary>Quién la desvió (no puede ser noqueado por ella mientras DeflectT &gt; 0). -1 = nadie.</summary>
        public int DeflectBy = -1;
        /// <summary>Tatuaje: a quién persigue ahora (para el ícono de marca). -1 = nadie.</summary>
        public int MarkedId = -1;

        public bool Lethal => Mode == CapMode.Out || Mode == CapMode.Return || Mode == CapMode.Recall;
        public bool Deflected => DeflectT > 0f;
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
        /// <summary>Empujón sin gorra: recarga propia y el "dash forzado" que sufre la víctima.</summary>
        public float ShoveCd, PushT;
        public Vec2 PushVel;
        /// <summary>La Reta: está esperando su turno en la fila (no aparece en la arena).</summary>
        public bool Benched;
        /// <summary>La Reta: victorias seguidas.</summary>
        public int Streak;

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

    /// <summary>Tinta derramada · Charco: crece de 0 a MaxRadius y al terminar noquea a quien siga dentro.</summary>
    [Serializable]
    public sealed class Puddle
    {
        public int Id;
        public Vec2 Pos;
        public int OwnerId;
        public float T;        // segundos desde que empezó a crecer
        public float Radius;   // radio actual (para la vista)
    }

    /// <summary>Grafiti · Muro de Tag: rectángulo orientado que bloquea gorras y jugadores unos segundos.</summary>
    [Serializable]
    public sealed class TagWall
    {
        public int Id;
        public Vec2 Pos;       // centro
        public Vec2 Dir;       // eje largo (unitario)
        public float HalfLen, HalfThick;
        public float Life;     // segundos que le quedan
        public int OwnerId;
    }

    /// <summary>
    /// Tipos de evento. ¡Solo se agregan al FINAL! (el número viaja por la red).
    /// </summary>
    public enum SimEventType : byte
    {
        Throw, Catch, Recall, Dash, Melee,
        KO, Clank, Deflect, ShieldBreak, Explode,
        PickupSpawn, PickupTaken, Paint,
        LaserWarn, Countdown, RoundStart, RoundEnd, MatchEnd,
        // ── v0.2 ──
        SuddenDeathWarn,   // aviso de 1 s antes de que avancen los muros
        SuddenDeathStart,  // los muros empiezan a avanzar
        Shove,             // A = quien empuja, B = empujado (o -1 si solo pateó una gorra)
        Marked,            // Tatuaje: A = víctima marcada, B = dueño de la gorra
        PuddleStart,       // Charco: empieza a crecer (A = dueño, Value = radio final)
        TagWall,           // Muro de Tag pintado (A = dueño, Value = ángulo del eje largo)
        HotCanPass,        // Lata Caliente: A = nuevo portador, B = quien la pasó (-1 = se pegó al azar), Value = segundos que le quedan
        HotCanExplode,     // Lata Caliente: A = portador que queda fuera
        RetaNext,          // La Reta: A = quien entra a pelear, B = quien se va a la fila
    }

    /// <summary>Causa detallada del KO (A8). Cada una tiene su letrero.</summary>
    public enum KoCause : byte
    {
        Gorrazo = 0,     // gorra de ida
        DeRegreso = 1,   // gorra regresando o llamada
        DeBanda = 2,     // gorra que ya rebotó en pared
        TeLaRegreso = 3, // gorra desviada
        Zaz = 4,         // golpe cuerpo a cuerpo
        Zap = 5,         // láser
        Splash = 6,      // Charco (y la Lata Caliente al explotar)
        Tiraje = 7,      // copia de Serigrafía
    }

    /// <summary>Por qué terminó una ronda (va en Extra del evento RoundEnd).</summary>
    public enum RoundEndReason : byte { Winner = 0, DoubleKo = 1, TimeUp = 2 }

    public static class KoCauseInfo
    {
        public const string TieLabel = "¡EMPATE!";

        public static string Label(KoCause c)
        {
            switch (c)
            {
                case KoCause.Gorrazo: return "¡GORRAZO!";
                case KoCause.DeRegreso: return "¡DE REGRESO!";
                case KoCause.DeBanda: return "¡DE BANDA!";
                case KoCause.TeLaRegreso: return "¡TE LA REGRESO!";
                case KoCause.Zaz: return "¡ZAZ!";
                case KoCause.Zap: return "¡ZAP!";
                case KoCause.Splash: return "¡SPLASH!";
                case KoCause.Tiraje: return "¡TIRAJE!";
                default: return "";
            }
        }
    }

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

    /// <summary>Modos de juego (todos locales y gratis).</summary>
    public enum GameMode : byte
    {
        Gorrazo = 0,       // último en pie gana la ronda
        LataCaliente = 1,  // la lata pasa con cada gorrazo y explota entre 10 y 20 s
        LaReta = 2,        // 1 contra 1, el que gana se queda, los demás hacen fila
        RetoDelDia = 3,    // 1 jugador contra bots con semilla del día (la semilla y la tabla viven fuera del núcleo)
    }

    /// <summary>Interruptores de reglas (A2–A4 y latas). Viajan en el StartMatch de la red.</summary>
    [Serializable]
    public sealed class Rules
    {
        public bool SuddenDeath = true;    // A2: muros que avanzan a los 20 s, empate a los 45 s
        public bool Shove = true;          // A3: GOLPE sin gorra = empujón [A PRUEBA]
        public bool DeflectReturn = true;  // A4: "Te la regreso" [A PRUEBA]
        public bool GraffitiWall = true;   // B: Grafiti pinta un Muro de Tag (false = rastro que alenta)
        public bool PowerUps = true;       // latas de técnica en la arena

        public Rules Clone() => (Rules)MemberwiseClone();

        /// <summary>Empaqueta los interruptores en un byte (para la red).</summary>
        public byte ToBits() => (byte)((SuddenDeath ? 1 : 0) | (Shove ? 2 : 0) | (DeflectReturn ? 4 : 0) | (GraffitiWall ? 8 : 0) | (PowerUps ? 16 : 0));

        public static Rules FromBits(byte b) => new Rules
        {
            SuddenDeath = (b & 1) != 0, Shove = (b & 2) != 0, DeflectReturn = (b & 4) != 0,
            GraffitiWall = (b & 8) != 0, PowerUps = (b & 16) != 0,
        };
    }

    [Serializable]
    public sealed class MatchConfig
    {
        public string ArenaId = "callejon";
        public int PointsToWin = 5;        // local: 5 (opciones 3/5/7). La Reta no lo usa.
        public uint Seed = 12345;
        public GameMode Mode = GameMode.Gorrazo;
        public Rules Rules = new Rules();
        public List<PlayerSlot> Slots = new List<PlayerSlot>();
        public Tuning Tuning = Tuning.Default();
    }
}
