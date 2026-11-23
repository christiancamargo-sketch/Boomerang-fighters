// ─────────────────────────────────────────────────────────────
// Matemática 2D de la simulación (C# puro, sin UnityEngine).
// El juego corre en el plano del piso: Vec2.x = X de Unity, Vec2.y = Z de Unity.
// ─────────────────────────────────────────────────────────────
using System;

namespace BoomerangFighters.Core
{
    [Serializable]
    public struct Vec2 : IEquatable<Vec2>
    {
        public float x;
        public float y;

        public Vec2(float x, float y) { this.x = x; this.y = y; }

        public static readonly Vec2 Zero = new Vec2(0f, 0f);

        public float Length => MathF.Sqrt(x * x + y * y);
        public float SqrLength => x * x + y * y;
        public float Angle => MathF.Atan2(y, x);
        public Vec2 Perp => new Vec2(-y, x);

        public Vec2 Normalized
        {
            get
            {
                float l = Length;
                return l > 1e-6f ? new Vec2(x / l, y / l) : Zero;
            }
        }

        public static Vec2 FromAngle(float a) => new Vec2(MathF.Cos(a), MathF.Sin(a));
        public static float Dot(Vec2 a, Vec2 b) => a.x * b.x + a.y * b.y;
        public static float Distance(Vec2 a, Vec2 b) => (a - b).Length;

        public static Vec2 operator +(Vec2 a, Vec2 b) => new Vec2(a.x + b.x, a.y + b.y);
        public static Vec2 operator -(Vec2 a, Vec2 b) => new Vec2(a.x - b.x, a.y - b.y);
        public static Vec2 operator -(Vec2 a) => new Vec2(-a.x, -a.y);
        public static Vec2 operator *(Vec2 a, float s) => new Vec2(a.x * s, a.y * s);
        public static Vec2 operator *(float s, Vec2 a) => new Vec2(a.x * s, a.y * s);
        public static Vec2 operator /(Vec2 a, float s) => new Vec2(a.x / s, a.y / s);

        public bool Equals(Vec2 o) => x == o.x && y == o.y;
        public override bool Equals(object obj) => obj is Vec2 o && Equals(o);
        public override int GetHashCode() => (x.GetHashCode() * 397) ^ y.GetHashCode();
        public override string ToString() => $"({x:0.00}, {y:0.00})";
    }

    public static class SimMath
    {
        public const float Pi = MathF.PI;

        public static float Lerp(float a, float b, float t) => a + (b - a) * t;
        public static float Clamp(float v, float min, float max) => v < min ? min : (v > max ? max : v);
        public static float Clamp01(float v) => Clamp(v, 0f, 1f);

        /// <summary>Diferencia de ángulo más corta (de -PI a PI).</summary>
        public static float AngleDelta(float from, float to)
        {
            float d = to - from;
            while (d > Pi) d -= 2f * Pi;
            while (d < -Pi) d += 2f * Pi;
            return d;
        }

        /// <summary>Gira la dirección "dir" hacia "want" como máximo "maxStep" radianes.</summary>
        public static Vec2 RotateTowards(Vec2 dir, Vec2 want, float maxStep)
        {
            float cur = dir.Angle;
            float a = cur + Clamp(AngleDelta(cur, want.Angle), -maxStep, maxStep);
            return Vec2.FromAngle(a);
        }
    }

    /// <summary>
    /// Generador pseudoaleatorio determinista (xorshift32). Con la misma semilla,
    /// el host y los clientes obtienen la misma secuencia (spawns, power-ups).
    /// Nunca usar System.Random ni UnityEngine.Random dentro de la simulación.
    /// </summary>
    [Serializable]
    public struct Rng
    {
        public uint State;

        public Rng(uint seed) { State = seed == 0 ? 0x9E3779B9u : seed; }

        public uint NextUInt()
        {
            uint s = State;
            s ^= s << 13;
            s ^= s >> 17;
            s ^= s << 5;
            State = s;
            return s;
        }

        /// <summary>Número en [0, 1).</summary>
        public float NextFloat() => (NextUInt() >> 8) * (1f / 16777216f);
        public float Range(float min, float max) => min + (max - min) * NextFloat();
        public int NextInt(int maxExclusive) => maxExclusive <= 0 ? 0 : (int)(NextUInt() % (uint)maxExclusive);
    }
}
