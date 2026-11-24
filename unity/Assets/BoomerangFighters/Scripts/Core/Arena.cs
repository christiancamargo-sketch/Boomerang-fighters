// ─────────────────────────────────────────────────────────────
// Escenarios: datos + consultas de colisión + peligros (láser).
// Mismos datos que el prototipo web (prototipo/src/arena.js).
// v0.3 (spec docs/diseno/spec-v0.3-escenarios.md): escenarios más
// grandes (26 × 16), VACÍOS (huecos por donde te caes), el escenario
// nuevo "Pop-up Store INKCAP" y su peligro: la caja DROP.
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

    /// <summary>
    /// Vacío: hueco rectangular en el piso (centro, ancho, fondo). Quien queda encima sin dash
    /// ni empujón, cae. Las gorras en vuelo pasan por encima.
    /// </summary>
    [Serializable]
    public struct Pit
    {
        public float x, y, w, d;
        public string Kind;
        public Pit(float x, float y, float w, float d, string kind) { this.x = x; this.y = y; this.w = w; this.d = d; Kind = kind; }

        /// <summary>¿El punto está dentro del hueco? "inset" lo reduce por cada lado (V1: 0.15 m de perdón).</summary>
        public bool Contains(Vec2 p, float inset = 0f)
            => MathF.Abs(p.x - x) < w / 2f - inset && MathF.Abs(p.y - y) < d / 2f - inset;

        /// <summary>Punto del hueco más cercano a "p" (el mismo "p" si está dentro).</summary>
        public Vec2 Closest(Vec2 p)
            => new Vec2(SimMath.Clamp(p.x, x - w / 2f, x + w / 2f), SimMath.Clamp(p.y, y - d / 2f, y + d / 2f));

        /// <summary>Distancia del punto a la orilla del hueco (0 si está dentro).</summary>
        public float Distance(Vec2 p) => Vec2.Distance(p, Closest(p));
    }

    public enum HazardType : byte { Laser = 0 }

    [Serializable]
    public sealed class HazardDef
    {
        public HazardType Type;
        public float Period, Warn, Active, Offset;
    }

    /// <summary>
    /// Peligro "caja DROP" (Pop-up Store): cae del techo cada "Every" segundos de ronda (la primera
    /// a los "First"), con una sombra de aviso "Warn" segundos antes. Noquea a quien esté a "Radius"
    /// o menos del punto de caída y deja una lata de técnica.
    /// </summary>
    [Serializable]
    public sealed class DropBoxDef
    {
        public float First = 8f;
        public float Every = 20f;
        public float Warn = 1.5f;
        public float Radius = 1.1f;
    }

    [Serializable]
    public sealed class ArenaDef
    {
        public string Id, Name, Description;
        public float Width, Depth;
        public Box[] Obstacles;
        public Vec2[] Spawns;
        public HazardDef[] Hazards = Array.Empty<HazardDef>();
        /// <summary>Vacíos del escenario (v0.3).</summary>
        public Pit[] Pits = Array.Empty<Pit>();
        /// <summary>Caja DROP (null = en este escenario no cae nada del techo).</summary>
        public DropBoxDef Drop;
    }

    public static class ArenaLibrary
    {
        static Box B(float x, float y, float w, float d, float h, string k) => new Box(x, y, w, d, h, k);
        static Pit P(float x, float y, float w, float d, string k) => new Pit(x, y, w, d, k);
        static Vec2 V(float x, float y) => new Vec2(x, y);
        static float PX = float.Parse(System.Environment.GetEnvironmentVariable("PX") ?? "9.6"), PY = float.Parse(System.Environment.GetEnvironmentVariable("PY") ?? "4.6");

        // B1 · Callejón Grafiti (26 × 16): coladeras abiertas y zanjas de obra
        public static readonly ArenaDef Callejon = new ArenaDef
        {
            Id = "callejon", Name = "Callejón Grafiti", Description = "Contenedores, cajas, coladeras abiertas y muros llenos de tags.",
            Width = 26f, Depth = 16f,
            Obstacles = new[]
            {
                B(-6.5f, -3.2f, 2.6f, 1.3f, 1.4f, "dumpster"), B(6.5f, 3.2f, 2.6f, 1.3f, 1.4f, "dumpster"),
                B(0f, 0f, 1.2f, 1.2f, 1.2f, "crate"),
                B(-2.4f, 4.6f, 1.0f, 1.0f, 0.7f, "crate-low"), B(2.4f, -4.6f, 1.0f, 1.0f, 0.7f, "crate-low"),
                B(-10.2f, 3.8f, 1.0f, 2.4f, 1.3f, "crate"), B(10.2f, -3.8f, 1.0f, 2.4f, 1.3f, "crate"),
            },
            Pits = new[]
            {
                P(-5.6f, 4.9f, 1.4f, 1.4f, "coladera"), P(5.6f, -4.9f, 1.4f, 1.4f, "coladera"),
                P(0f, 3.0f, 3.4f, 0.9f, "zanja"), P(0f, -3.0f, 3.4f, 0.9f, "zanja"),
            },
            Spawns = new[] { V(-11f, -6.3f), V(11f, 6.3f), V(11f, -6.3f), V(-11f, 6.3f), V(-3.5f, 0f), V(3.5f, 0f) },
        };

        // B2 · Museo de Arte · "Exposición INKCAP" (26 × 16): fosos de instalación + láseres
        public static readonly ArenaDef Museo = new ArenaDef
        {
            Id = "museo", Name = "Museo de Arte", Description = "Exposición INKCAP: pedestales, fosos de instalación y láseres de seguridad.",
            Width = 26f, Depth = 16f,
            Obstacles = new[]
            {
                B(-5.5f, -2.8f, 1.4f, 1.4f, 1.5f, "pedestal"), B(5.5f, 2.8f, 1.4f, 1.4f, 1.5f, "pedestal"),
                B(-5.5f, 2.8f, 1.4f, 1.4f, 1.5f, "pedestal"), B(5.5f, -2.8f, 1.4f, 1.4f, 1.5f, "pedestal"),
                B(0f, 0f, 3.2f, 0.7f, 0.55f, "bench"),
            },
            Pits = new[] { P(-9.2f, 0f, 1.8f, 3.2f, "foso"), P(9.2f, 0f, 1.8f, 3.2f, "foso") },
            Spawns = new[] { V(-11f, -6.3f), V(11f, 6.3f), V(11f, -6.3f), V(-11f, 6.3f), V(0f, -6.6f), V(0f, 6.6f) },
            Hazards = new[] { new HazardDef { Type = HazardType.Laser, Period = 9f, Warn = 1.4f, Active = 1.6f, Offset = 4f } },
        };

        // B3 · Azotea CDMX (26 × 16): dos azoteas con el hueco entre edificios y un puente de tablón de 3 m
        public static readonly ArenaDef Azotea = new ArenaDef
        {
            Id = "azotea", Name = "Azotea CDMX", Description = "Dos azoteas, un tablón de puente y el hueco entre edificios.",
            Width = 26f, Depth = 16f,
            Obstacles = new[]
            {
                B(-6.5f, 0f, 1.6f, 1.6f, 1.8f, "tinaco"), B(6.5f, 0f, 1.6f, 1.6f, 1.8f, "tinaco"),
                B(-6.5f, -4.4f, 2.6f, 0.5f, 0.6f, "planter"), B(6.5f, 4.4f, 2.6f, 0.5f, 0.6f, "planter"),
                B(-10.8f, -5.6f, 1.1f, 1.1f, 1.0f, "gas"), B(10.8f, 5.6f, 1.1f, 1.1f, 1.0f, "gas"),
            },
            Pits = new[] { P(0f, -4.75f, 1.8f, 6.5f, "hueco"), P(0f, 4.75f, 1.8f, 6.5f, "hueco") },
            Spawns = new[] { V(-10f, 0f), V(10f, 0f), V(-4f, -6.2f), V(4f, 6.2f), V(-4f, 6.2f), V(4f, -6.2f) },
        };

        // B4 · Pop-up Store INKCAP (24 × 15): racks de playeras, mesa de gorras, escaleras al sótano y la caja DROP.
        // Los spawns 3 y 4 van en (6.5, −5.5) y (−6.5, 5.5): en las esquinas caerían dentro de las escaleras (V4).
        public static readonly ArenaDef Popup = new ArenaDef
        {
            Id = "popup", Name = "Pop-up Store INKCAP", Description = "Racks de playeras, escaleras al sótano y la caja DROP que cae del techo.",
            Width = 24f, Depth = 15f,
            Obstacles = new[]
            {
                B(-5f, -3.2f, 2.6f, 0.6f, 1.4f, "rack"), B(5f, 3.2f, 2.6f, 0.6f, 1.4f, "rack"),
                B(-5f, 3.2f, 2.6f, 0.6f, 1.4f, "rack"), B(5f, -3.2f, 2.6f, 0.6f, 1.4f, "rack"),
                B(0f, 0f, 2.2f, 1.2f, 0.9f, "table"), // mesa de gorras: baja, las gorras pasan
            },
            Pits = new[] { P(-PX, PY, 1.6f, 2.2f, "escalera"), P(PX, -PY, 1.6f, 2.2f, "escalera") },
            Spawns = new[] { V(-9.5f, -5.5f), V(9.5f, 5.5f), V(6.5f, -5.5f), V(-6.5f, 5.5f), V(0f, -5.8f), V(0f, 5.8f) },
            Drop = new DropBoxDef { First = 8f, Every = 20f, Warn = 1.5f, Radius = 1.1f },
        };

        /// <summary>El orden importa: "Aleatorio" y el Reto del Día eligen por índice.</summary>
        public static readonly ArenaDef[] All = { Callejon, Museo, Azotea, Popup };

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
        /// <summary>Muros de Tag (Grafiti) vivos: bloquean gorras y jugadores.</summary>
        public readonly List<TagWall> TagWalls = new List<TagWall>();
        /// <summary>Muerte súbita: cuánto avanzó hacia el centro cada muro de los lados cortos (eje X).</summary>
        public float WallInset;
        public float HalfW => Def.Width * 0.5f;
        public float HalfD => Def.Depth * 0.5f;
        /// <summary>Media anchura jugable (se encoge en muerte súbita).</summary>
        public float PlayHalfW => HalfW - WallInset;

        /// <summary>V4: nada aparece dentro de un vacío ni a menos de esta distancia de su orilla.</summary>
        public float PitMargin = 0.8f;
        public bool HasPits => Def.Pits.Length > 0;

        /// <summary>
        /// ¿El punto está sobre un vacío? "inset" reduce cada hueco por lado: V1 usa 0.15 m para los
        /// jugadores (perdón en la orilla); las gorras en el piso usan el rectángulo completo (0).
        /// </summary>
        public bool InPit(Vec2 p, float inset = 0f)
        {
            foreach (var pit in Def.Pits) if (pit.Contains(p, inset)) return true;
            return false;
        }

        /// <summary>Distancia a la orilla del vacío más cercano (0 si está dentro; enorme si no hay vacíos).</summary>
        public float PitDistance(Vec2 p)
        {
            float best = float.MaxValue;
            foreach (var pit in Def.Pits)
            {
                float d = pit.Distance(p);
                if (d < best) best = d;
            }
            return best;
        }

        public bool NearPit(Vec2 p, float margin) => PitDistance(p) < margin;

        /// <summary>¿El tramo de "a" a "b" pasa por encima de un vacío (agrandado "pad" por lado)?</summary>
        public bool SegmentCrossesPit(Vec2 a, Vec2 b, float pad = 0f)
        {
            foreach (var pit in Def.Pits)
                if (SegmentHitsBox(a, b, new Box(pit.x, pit.y, pit.w, pit.d, 0f, pit.Kind), pad)) return true;
            return false;
        }

        /// <summary>
        /// Para los bots (V5): empuje que aleja de las orillas que estén a menos de "margin".
        /// Crece de 0 (a "margin" de la orilla) a 1 (en la orilla); dentro del hueco apunta a la salida más corta.
        /// </summary>
        public Vec2 PitRepulsion(Vec2 p, float margin)
        {
            var push = Vec2.Zero;
            foreach (var pit in Def.Pits)
            {
                var c = pit.Closest(p);
                var dv = p - c;
                float d = dv.Length;
                if (d >= margin) continue;
                if (d > 1e-4f) { push += dv / d * ((margin - d) / margin); continue; }
                // está encima: salir por el lado más cercano
                float left = p.x - (pit.x - pit.w / 2f), right = (pit.x + pit.w / 2f) - p.x;
                float top = p.y - (pit.y - pit.d / 2f), bot = (pit.y + pit.d / 2f) - p.y;
                float m = MathF.Min(MathF.Min(left, right), MathF.Min(top, bot));
                if (m == left) push.x -= 1f;
                else if (m == right) push.x += 1f;
                else if (m == top) push.y -= 1f;
                else push.y += 1f;
            }
            return push;
        }

        /// <summary>El obstáculo quedó del otro lado de un muro de muerte súbita (ya no cuenta).</summary>
        bool BeyondWalls(in Box o)
        {
            if (WallInset <= 0f) return false;
            float hw = PlayHalfW;
            return o.x - o.w / 2f >= hw || o.x + o.w / 2f <= -hw;
        }

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
            bool hit = ClampToWalls(ref p, r);
            foreach (var o in Def.Obstacles)
            {
                if (BeyondWalls(o)) continue;
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
            // muros de tag
            foreach (var w in TagWalls)
            {
                if (!CircleVsWall(p, r, w, out var n, out var point)) continue;
                hit = true;
                p = point;
            }
            // los muros de muerte súbita mandan: empujan hacia adentro aunque haya algo detrás
            if (WallInset > 0f) hit |= ClampToWalls(ref p, r);
            return hit;
        }

        bool ClampToWalls(ref Vec2 p, float r)
        {
            bool hit = false;
            float hw = PlayHalfW - r, hh = HalfD - r;
            if (p.x < -hw) { p.x = -hw; hit = true; }
            if (p.x > hw) { p.x = hw; hit = true; }
            if (p.y < -hh) { p.y = -hh; hit = true; }
            if (p.y > hh) { p.y = hh; hit = true; }
            return hit;
        }

        /// <summary>Círculo contra muro de tag (rectángulo orientado). Devuelve la normal y el punto corregido.</summary>
        public static bool CircleVsWall(Vec2 p, float r, TagWall w, out Vec2 normal, out Vec2 point)
        {
            var u = w.Dir; var nn = w.Dir.Perp;
            var d = p - w.Pos;
            float lx = Vec2.Dot(d, u), ly = Vec2.Dot(d, nn);
            float cx = SimMath.Clamp(lx, -w.HalfLen, w.HalfLen), cy = SimMath.Clamp(ly, -w.HalfThick, w.HalfThick);
            float dx = lx - cx, dy = ly - cy;
            float d2 = dx * dx + dy * dy;
            normal = Vec2.Zero; point = p;
            if (d2 >= r * r) return false;
            float nx, ny, px, py;
            if (d2 > 1e-6f)
            {
                float dl = MathF.Sqrt(d2);
                nx = dx / dl; ny = dy / dl;
                px = cx + nx * r; py = cy + ny * r;
            }
            else
            {
                // el centro quedó dentro: salir por el lado más cercano
                float ex = w.HalfLen - MathF.Abs(lx), ey = w.HalfThick - MathF.Abs(ly);
                if (ey <= ex) { nx = 0f; ny = ly >= 0f ? 1f : -1f; px = lx; py = ny * (w.HalfThick + r); }
                else { ny = 0f; nx = lx >= 0f ? 1f : -1f; py = ly; px = nx * (w.HalfLen + r); }
            }
            normal = u * nx + nn * ny;
            point = w.Pos + u * px + nn * py;
            return true;
        }

        /// <summary>Choque de una gorra con muros u obstáculos altos.</summary>
        /// <param name="ignoreOwner">Los muros de tag de este jugador no detienen sus propias gorras (-1 = ninguno).</param>
        /// <param name="from">De dónde venía la gorra: contra un muro de tag rebota del lado por el que llegó.</param>
        public bool CapCollision(Vec2 p, float r, out WallHit hit, int ignoreOwner = -1, Vec2? from = null)
        {
            float hw = PlayHalfW - r, hh = HalfD - r;
            hit = default;
            if (p.x < -hw) { hit.Normal = new Vec2(1, 0); hit.Point = new Vec2(-hw, p.y); return true; }
            if (p.x > hw) { hit.Normal = new Vec2(-1, 0); hit.Point = new Vec2(hw, p.y); return true; }
            if (p.y < -hh) { hit.Normal = new Vec2(0, 1); hit.Point = new Vec2(p.x, -hh); return true; }
            if (p.y > hh) { hit.Normal = new Vec2(0, -1); hit.Point = new Vec2(p.x, hh); return true; }
            foreach (var o in Def.Obstacles)
            {
                if (!o.BlocksCaps || BeyondWalls(o)) continue;
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
            foreach (var w in TagWalls)
            {
                if (w.OwnerId == ignoreOwner) continue;
                if (!CircleVsWall(p, r, w, out var n, out var point)) continue;
                if (from.HasValue)
                {
                    // el muro es delgado: decidir la cara por el lado de llegada (que no lo atraviese)
                    var nn = w.Dir.Perp;
                    float lx = Vec2.Dot(p - w.Pos, w.Dir);
                    if (MathF.Abs(lx) <= w.HalfLen)
                    {
                        float side = Vec2.Dot(from.Value - w.Pos, nn) >= 0f ? 1f : -1f;
                        n = nn * side;
                        point = w.Pos + w.Dir * lx + nn * (side * (w.HalfThick + r));
                    }
                }
                hit.Normal = n;
                hit.Point = point;
                return true;
            }
            return false;
        }

        public bool LineOfSight(Vec2 a, Vec2 b)
        {
            foreach (var o in Def.Obstacles)
                if (o.BlocksCaps && !BeyondWalls(o) && SegmentHitsBox(a, b, o, 0.25f)) return false;
            foreach (var w in TagWalls)
            {
                // pasar el segmento a coordenadas del muro y usar la misma prueba de caja
                var u = w.Dir; var nn = w.Dir.Perp;
                var la = new Vec2(Vec2.Dot(a - w.Pos, u), Vec2.Dot(a - w.Pos, nn));
                var lb = new Vec2(Vec2.Dot(b - w.Pos, u), Vec2.Dot(b - w.Pos, nn));
                if (SegmentHitsBox(la, lb, new Box(0f, 0f, w.HalfLen * 2f, w.HalfThick * 2f, 1f, "tag"), 0.1f)) return false;
            }
            return true;
        }

        /// <summary>¿Cabe un círculo de radio "r" sin tocar muros, obstáculos ni vacíos? (los vacíos cuentan como obstáculo)</summary>
        public bool IsFree(Vec2 p, float r)
        {
            if (MathF.Abs(p.x) > PlayHalfW - r || MathF.Abs(p.y) > HalfD - r) return false;
            foreach (var o in Def.Obstacles)
                if (!BeyondWalls(o) && MathF.Abs(p.x - o.x) < o.w / 2f + r && MathF.Abs(p.y - o.y) < o.d / 2f + r) return false;
            foreach (var pit in Def.Pits)
                if (MathF.Abs(p.x - pit.x) < pit.w / 2f + r && MathF.Abs(p.y - pit.y) < pit.d / 2f + r) return false;
            foreach (var w in TagWalls)
                if (CircleVsWall(p, r, w, out _, out _)) return false;
            return true;
        }

        /// <summary>Punto válido para que aparezca algo (lata, caja DROP): libre y a más de PitMargin de cualquier vacío (V4).</summary>
        public bool IsSpawnable(Vec2 p, float r) => IsFree(p, r) && !NearPit(p, PitMargin);

        public Vec2 RandomFreePoint(ref Rng rng, float r)
        {
            float usableW = MathF.Max(1f, PlayHalfW * 2f - 3f);
            for (int i = 0; i < 60; i++)
            {
                var p = new Vec2((rng.NextFloat() - 0.5f) * usableW, (rng.NextFloat() - 0.5f) * (Def.Depth - 3f));
                if (IsSpawnable(p, r)) return p;
            }
            // casi imposible llegar aquí: barrido fijo de la zona jugable (nunca devolver un punto sobre un vacío)
            for (int pass = 0; pass < 2; pass++)
            {
                float rr = pass == 0 ? r : 0.3f;
                for (float y = 0f; y <= HalfD - 1f; y += 0.5f)
                    for (float x = 0f; x <= PlayHalfW - 1f; x += 0.5f)
                        for (int q = 0; q < 4; q++)
                        {
                            var p = new Vec2((q & 1) == 0 ? x : -x, (q & 2) == 0 ? y : -y);
                            if (IsSpawnable(p, rr)) return p;
                        }
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
