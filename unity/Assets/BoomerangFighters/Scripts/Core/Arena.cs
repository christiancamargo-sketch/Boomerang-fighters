// ─────────────────────────────────────────────────────────────
// Escenarios: datos + consultas de colisión + peligros (láser).
// Mismos datos que el prototipo web (prototipo/src/arena.js).
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;

namespace BoomerangFighters.Core
{
    /// <summary>Obstáculo rectangular en el piso. Altura ≥ 1 bloquea gorras; menor solo bloquea jugadores.</summary>
    [Serializable]
    public struct Box
    {
        public float x, y, w, d, h;
        public string Kind;
        public Box(float x, float y, float w, float d, float h, string kind) { this.x = x; this.y = y; this.w = w; this.d = d; this.h = h; Kind = kind; }
        public bool BlocksCaps => h >= 1f;
    }

    public enum HazardType : byte { Laser = 0 }

    [Serializable]
    public sealed class HazardDef
    {
        public HazardType Type;
        public float Period, Warn, Active, Offset;
    }

    [Serializable]
    public sealed class ArenaDef
    {
        public string Id, Name, Description;
        public float Width, Depth;
        public Box[] Obstacles;
        public Vec2[] Spawns;
        public HazardDef[] Hazards = Array.Empty<HazardDef>();
    }

    public static class ArenaLibrary
    {
        static Box B(float x, float y, float w, float d, float h, string k) => new Box(x, y, w, d, h, k);
        static Vec2 V(float x, float y) => new Vec2(x, y);

        public static readonly ArenaDef Callejon = new ArenaDef
        {
            Id = "callejon", Name = "Callejón Grafiti", Description = "Contenedores, cajas y muros llenos de tags.",
            Width = 23f, Depth = 14f,
            Obstacles = new[]
            {
                B(-5.6f, -2.8f, 2.4f, 1.2f, 1.3f, "dumpster"), B(5.6f, 2.8f, 2.4f, 1.2f, 1.3f, "dumpster"),
                B(0f, 0f, 1.1f, 1.1f, 1.1f, "crate"),
                B(-2.0f, 3.9f, 1.0f, 1.0f, 0.7f, "crate-low"), B(2.0f, -3.9f, 1.0f, 1.0f, 0.7f, "crate-low"),
                B(-9.0f, 3.1f, 0.9f, 2.2f, 1.2f, "crate"), B(9.0f, -3.1f, 0.9f, 2.2f, 1.2f, "crate"),
            },
            Spawns = new[] { V(-9f, -4.8f), V(9f, 4.8f), V(9f, -4.8f), V(-9f, 4.8f), V(0f, -5.4f), V(0f, 5.4f) },
        };

        public static readonly ArenaDef Museo = new ArenaDef
        {
            Id = "museo", Name = "Museo de Arte", Description = "Pedestales con esculturas y láseres de seguridad.",
            Width = 23f, Depth = 14f,
            Obstacles = new[]
            {
                B(-4.9f, -2.5f, 1.3f, 1.3f, 1.4f, "pedestal"), B(4.9f, 2.5f, 1.3f, 1.3f, 1.4f, "pedestal"),
                B(-4.9f, 2.5f, 1.3f, 1.3f, 1.4f, "pedestal"), B(4.9f, -2.5f, 1.3f, 1.3f, 1.4f, "pedestal"),
                B(0f, 0f, 3.0f, 0.7f, 0.55f, "bench"),
            },
            Spawns = new[] { V(-9.3f, -4.8f), V(9.3f, 4.8f), V(9.3f, -4.8f), V(-9.3f, 4.8f), V(0f, -5.4f), V(0f, 5.4f) },
            Hazards = new[] { new HazardDef { Type = HazardType.Laser, Period = 9f, Warn = 1.4f, Active = 1.6f, Offset = 4f } },
        };

        public static readonly ArenaDef Azotea = new ArenaDef
        {
            Id = "azotea", Name = "Azotea CDMX", Description = "Tinacos, macetas y tendederos al atardecer.",
            Width = 22f, Depth = 13.5f,
            Obstacles = new[]
            {
                B(-5.4f, 0f, 1.5f, 1.5f, 1.5f, "tinaco"), B(5.4f, 0f, 1.5f, 1.5f, 1.5f, "tinaco"),
                B(0f, -3.2f, 2.8f, 0.5f, 0.6f, "planter"), B(0f, 3.2f, 2.8f, 0.5f, 0.6f, "planter"),
                B(-8.6f, -4.1f, 1.1f, 1.1f, 1.0f, "gas"), B(8.6f, 4.1f, 1.1f, 1.1f, 1.0f, "gas"),
            },
            Spawns = new[] { V(-8.8f, 0f), V(8.8f, 0f), V(0f, -5.3f), V(0f, 5.3f), V(-8.2f, 4.9f), V(8.2f, -4.9f) },
        };

        public static readonly ArenaDef[] All = { Callejon, Museo, Azotea };

        public static ArenaDef Get(string id)
        {
            foreach (var a in All) if (a.Id == id) return a;
            return Callejon;
        }
    }

    public enum LaserState : byte { Off = 0, Warn = 1, Active = 2 }

    /// <summary>Estado de un peligro durante la partida.</summary>
    [Serializable]
    public sealed class HazardState
    {
        public HazardDef Def;
        public float T;
        public float Z; // posición del rayo (eje Y de la simulación = Z de Unity)
        public LaserState State;
    }

    public struct WallHit
    {
        public Vec2 Normal;
        public Vec2 Point;
    }

    /// <summary>Escenario en tiempo de juego: geometría + peligros.</summary>
    public sealed class Arena
    {
        public readonly ArenaDef Def;
        public readonly List<HazardState> Hazards = new List<HazardState>();
        public float HalfW => Def.Width * 0.5f;
        public float HalfD => Def.Depth * 0.5f;

        public Arena(ArenaDef def)
        {
            Def = def;
            foreach (var h in def.Hazards) Hazards.Add(new HazardState { Def = h });
        }

        /// <summary>Avanza los peligros. Devuelve true si un láser acaba de entrar en "aviso".</summary>
        public bool UpdateHazards(float dt, ref Rng rng)
        {
            bool warned = false;
            foreach (var hz in Hazards)
            {
                hz.T += dt;
                float cycle = (hz.T + hz.Def.Offset) % hz.Def.Period;
                var prev = hz.State;
                if (cycle < hz.Def.Warn) hz.State = LaserState.Warn;
                else if (cycle < hz.Def.Warn + hz.Def.Active) hz.State = LaserState.Active;
                else hz.State = LaserState.Off;
                if (prev == LaserState.Off && hz.State == LaserState.Warn)
                {
                    hz.Z = (rng.NextFloat() - 0.5f) * (Def.Depth - 3f);
                    warned = true;
                }
            }
            return warned;
        }

        public bool LaserHits(Vec2 p, float r)
        {
            foreach (var hz in Hazards)
                if (hz.Def.Type == HazardType.Laser && hz.State == LaserState.Active && MathF.Abs(p.y - hz.Z) < r + 0.15f) return true;
            return false;
        }

        /// <summary>Si hay un láser encendido o por encenderse cerca, devuelve su posición.</summary>
        public float? LaserDanger(Vec2 p, float margin)
        {
            foreach (var hz in Hazards)
                if (hz.Def.Type == HazardType.Laser && hz.State != LaserState.Off && MathF.Abs(p.y - hz.Z) < margin) return hz.Z;
            return null;
        }

        /// <summary>Empuja un círculo fuera de muros y obstáculos. Devuelve true si chocó.</summary>
        public bool ResolveCircle(ref Vec2 p, float r)
        {
            bool hit = false;
            float hw = HalfW - r, hh = HalfD - r;
            if (p.x < -hw) { p.x = -hw; hit = true; }
            if (p.x > hw) { p.x = hw; hit = true; }
            if (p.y < -hh) { p.y = -hh; hit = true; }
            if (p.y > hh) { p.y = hh; hit = true; }
            foreach (var o in Def.Obstacles)
            {
                float cx = SimMath.Clamp(p.x, o.x - o.w / 2f, o.x + o.w / 2f);
                float cz = SimMath.Clamp(p.y, o.y - o.d / 2f, o.y + o.d / 2f);
                float dx = p.x - cx, dz = p.y - cz;
                float d2 = dx * dx + dz * dz;
                if (d2 >= r * r) continue;
                hit = true;
                if (d2 > 1e-6f)
                {
                    float d = MathF.Sqrt(d2);
                    p.x = cx + dx / d * r; p.y = cz + dz / d * r;
                }
                else
                {
                    float left = p.x - (o.x - o.w / 2f), right = (o.x + o.w / 2f) - p.x;
                    float top = p.y - (o.y - o.d / 2f), bot = (o.y + o.d / 2f) - p.y;
                    float m = MathF.Min(MathF.Min(left, right), MathF.Min(top, bot));
                    if (m == left) p.x = o.x - o.w / 2f - r;
                    else if (m == right) p.x = o.x + o.w / 2f + r;
                    else if (m == top) p.y = o.y - o.d / 2f - r;
                    else p.y = o.y + o.d / 2f + r;
                }
            }
            return hit;
        }

        /// <summary>Choque de una gorra con muros u obstáculos altos.</summary>
        public bool CapCollision(Vec2 p, float r, out WallHit hit)
        {
            float hw = HalfW - r, hh = HalfD - r;
            hit = default;
            if (p.x < -hw) { hit.Normal = new Vec2(1, 0); hit.Point = new Vec2(-hw, p.y); return true; }
            if (p.x > hw) { hit.Normal = new Vec2(-1, 0); hit.Point = new Vec2(hw, p.y); return true; }
            if (p.y < -hh) { hit.Normal = new Vec2(0, 1); hit.Point = new Vec2(p.x, -hh); return true; }
            if (p.y > hh) { hit.Normal = new Vec2(0, -1); hit.Point = new Vec2(p.x, hh); return true; }
            foreach (var o in Def.Obstacles)
            {
                if (!o.BlocksCaps) continue;
                float cx = SimMath.Clamp(p.x, o.x - o.w / 2f, o.x + o.w / 2f);
                float cz = SimMath.Clamp(p.y, o.y - o.d / 2f, o.y + o.d / 2f);
                float dx = p.x - cx, dz = p.y - cz;
                float d2 = dx * dx + dz * dz;
                if (d2 >= r * r) continue;
                Vec2 n;
                if (d2 < 1e-6f) n = new Vec2(MathF.Sign(p.x - o.x) == 0 ? 1 : MathF.Sign(p.x - o.x), 0);
                else { float d = MathF.Sqrt(d2); n = new Vec2(dx / d, dz / d); }
                hit.Normal = n;
                hit.Point = new Vec2(cx + n.x * r, cz + n.y * r);
                return true;
            }
            return false;
        }

        public bool LineOfSight(Vec2 a, Vec2 b)
        {
            foreach (var o in Def.Obstacles)
                if (o.BlocksCaps && SegmentHitsBox(a, b, o, 0.25f)) return false;
            return true;
        }

        public bool IsFree(Vec2 p, float r)
        {
            if (MathF.Abs(p.x) > HalfW - r || MathF.Abs(p.y) > HalfD - r) return false;
            foreach (var o in Def.Obstacles)
                if (MathF.Abs(p.x - o.x) < o.w / 2f + r && MathF.Abs(p.y - o.y) < o.d / 2f + r) return false;
            return true;
        }

        public Vec2 RandomFreePoint(ref Rng rng, float r)
        {
            for (int i = 0; i < 60; i++)
            {
                var p = new Vec2((rng.NextFloat() - 0.5f) * (Def.Width - 3f), (rng.NextFloat() - 0.5f) * (Def.Depth - 3f));
                if (IsFree(p, r)) return p;
            }
            return new Vec2(0f, 2f);
        }

        static bool SegmentHitsBox(Vec2 a, Vec2 b, Box o, float pad)
        {
            float minX = o.x - o.w / 2f - pad, maxX = o.x + o.w / 2f + pad;
            float minZ = o.y - o.d / 2f - pad, maxZ = o.y + o.d / 2f + pad;
            float t0 = 0f, t1 = 1f;
            float dx = b.x - a.x, dz = b.y - a.y;
            bool Clip(float p, float q)
            {
                if (MathF.Abs(p) < 1e-9f) return q >= 0f;
                float t = q / p;
                if (p < 0f) { if (t > t1) return false; if (t > t0) t0 = t; }
                else { if (t < t0) return false; if (t < t1) t1 = t; }
                return true;
            }
            return Clip(-dx, a.x - minX) && Clip(dx, maxX - a.x) && Clip(-dz, a.y - minZ) && Clip(dz, maxZ - a.y) && t0 <= t1;
        }
    }
}
