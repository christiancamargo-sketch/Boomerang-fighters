// ─────────────────────────────────────────────────────────────
// VISTAS "GREYBOX": dibujan el estado de la simulación con primitivas
// de Unity (cubos, esferas, cápsulas). Así el proyecto corre desde el
// primer día sin arte. Cuando lleguen los modelos de los muñequitos,
// se reemplaza ToyView por un prefab con animaciones; la simulación
// no cambia.
// Conversión de ejes: Unity = (sim.x, altura, -sim.y).
// ─────────────────────────────────────────────────────────────
using System.Collections.Generic;
using BoomerangFighters.Core;
using UnityEngine;

namespace BoomerangFighters.UnityView
{
    public static class Greybox
    {
        static Material _base;

        public static Vector3 ToUnity(Vec2 v, float height = 0f) => new Vector3(v.x, height, -v.y);
        public static Quaternion Facing(Vec2 dir) => dir.SqrLength > 1e-6f ? Quaternion.LookRotation(new Vector3(dir.x, 0f, -dir.y)) : Quaternion.identity;

        public static Color Hex(uint rgb, float a = 1f) => new Color(((rgb >> 16) & 255) / 255f, ((rgb >> 8) & 255) / 255f, (rgb & 255) / 255f, a);

        /// <summary>Crea una primitiva sin collider, con material propio del color dado (funciona en URP y Built-in).</summary>
        public static GameObject Prim(PrimitiveType type, Transform parent, Color color, Vector3 localPos, Vector3 scale, string name = null)
        {
            var go = GameObject.CreatePrimitive(type);
            if (name != null) go.name = name;
            var col = go.GetComponent<Collider>();
            if (col != null) Object.Destroy(col);
            var r = go.GetComponent<Renderer>();
            if (_base == null) _base = r.sharedMaterial;
            r.material = new Material(_base) { color = color };
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            return go;
        }

        public static readonly uint[] PlayerColors = { 0xFF6A2B, 0x22C7E8, 0xFFD23F, 0xE84393, 0x4CD964, 0x8E6CFF, 0xFFFFFF, 0x888888 };
        public static readonly uint[] VinylColors = { 0xF3E4CF, 0x2F2F36, 0xBFE7FF, 0xFFD6E5, 0xB8F28A, 0xFF8F70 };
    }

    public sealed class ArenaView
    {
        public readonly GameObject Root;
        readonly List<GameObject> _lasers = new List<GameObject>();
        readonly Arena _arena;

        public ArenaView(Arena arena)
        {
            _arena = arena;
            var d = arena.Def;
            Root = new GameObject($"Arena · {d.Name}");
            Color floor = d.Id == "museo" ? Greybox.Hex(0xE9E4DA) : d.Id == "azotea" ? Greybox.Hex(0xB5764F) : Greybox.Hex(0x3A3A42);
            Color wall = d.Id == "museo" ? Greybox.Hex(0xECE6DC) : d.Id == "azotea" ? Greybox.Hex(0xD9A46C) : Greybox.Hex(0x7A4E44);
            Greybox.Prim(PrimitiveType.Cube, Root.transform, floor, new Vector3(0f, -0.05f, 0f), new Vector3(d.Width, 0.1f, d.Depth), "Piso");
            float wh = d.Id == "azotea" ? 1f : 2.4f, t = 0.6f;
            Greybox.Prim(PrimitiveType.Cube, Root.transform, wall, new Vector3(0f, wh / 2f, d.Depth / 2f + t / 2f), new Vector3(d.Width + 2 * t, wh, t), "Muro fondo");
            Greybox.Prim(PrimitiveType.Cube, Root.transform, wall, new Vector3(-d.Width / 2f - t / 2f, wh / 2f, 0f), new Vector3(t, wh, d.Depth), "Muro izq");
            Greybox.Prim(PrimitiveType.Cube, Root.transform, wall, new Vector3(d.Width / 2f + t / 2f, wh / 2f, 0f), new Vector3(t, wh, d.Depth), "Muro der");
            Greybox.Prim(PrimitiveType.Cube, Root.transform, wall * 0.8f, new Vector3(0f, 0.175f, -d.Depth / 2f - t / 2f), new Vector3(d.Width + 2 * t, 0.35f, t), "Muro frente");
            foreach (var o in d.Obstacles)
            {
                Color c = o.Kind switch
                {
                    "dumpster" => Greybox.Hex(0x1F7A4D),
                    "pedestal" => Greybox.Hex(0xF4F1EA),
                    "tinaco" => Greybox.Hex(0x111111),
                    "bench" => Greybox.Hex(0x6D4C41),
                    "planter" => Greybox.Hex(0x8D6E63),
                    "gas" => Greybox.Hex(0xE0E0E0),
                    _ => Greybox.Hex(0xBC8A5F),
                };
                var type = o.Kind == "tinaco" || o.Kind == "gas" ? PrimitiveType.Cylinder : PrimitiveType.Cube;
                var s = type == PrimitiveType.Cylinder ? new Vector3(o.w, o.h / 2f, o.d) : new Vector3(o.w, o.h, o.d);
                Greybox.Prim(type, Root.transform, c, Greybox.ToUnity(new Vec2(o.x, o.y), o.h / 2f), s, o.Kind);
            }
            foreach (var hz in arena.Hazards)
            {
                var beam = Greybox.Prim(PrimitiveType.Cube, Root.transform, new Color(1f, 0.1f, 0.25f), new Vector3(0f, 0.7f, 0f), new Vector3(d.Width, 0.12f, 0.12f), "Láser");
                beam.SetActive(false);
                _lasers.Add(beam);
            }
        }

        public void Sync(float time)
        {
            for (int i = 0; i < _lasers.Count && i < _arena.Hazards.Count; i++)
            {
                var hz = _arena.Hazards[i];
                var beam = _lasers[i];
                bool blink = Mathf.Sin(time * 30f) > 0f;
                beam.SetActive(hz.State == LaserState.Active || (hz.State == LaserState.Warn && blink));
                beam.transform.localPosition = new Vector3(0f, 0.7f, -hz.Z);
                beam.transform.localScale = new Vector3(_arena.Def.Width, hz.State == LaserState.Active ? 0.18f : 0.05f, hz.State == LaserState.Active ? 0.18f : 0.05f);
            }
        }

        public void Destroy() => Object.Destroy(Root);
    }

    /// <summary>Muñequito greybox: cabeza grande, cuerpo chico, gorra (visible solo si la tiene).</summary>
    public sealed class ToyView
    {
        public readonly GameObject Root;
        readonly Transform _pivot, _capOnHead, _shield, _aim;
        readonly Renderer _ring;
        readonly Color _color;
        float _bob, _squash;

        public ToyView(PlayerState p)
        {
            _color = Greybox.Hex(Greybox.PlayerColors[p.Id % Greybox.PlayerColors.Length]);
            Root = new GameObject($"Jugador {p.Id} · {p.Name}");
            _pivot = new GameObject("Pivote").transform;
            _pivot.SetParent(Root.transform, false);
            var vinyl = Greybox.Hex(Greybox.VinylColors[p.Species % Greybox.VinylColors.Length]);
            Greybox.Prim(PrimitiveType.Capsule, _pivot, Color.white, new Vector3(0f, 0.48f, 0f), new Vector3(0.54f, 0.38f, 0.54f), "Cuerpo (playera)");
            var head = Greybox.Prim(PrimitiveType.Sphere, _pivot, vinyl, new Vector3(0f, 1.08f, 0f), new Vector3(0.97f, 0.85f, 0.9f), "Cabeza");
            foreach (float sx in new[] { -0.16f, 0.16f })
                Greybox.Prim(PrimitiveType.Sphere, head.transform, Color.black, new Vector3(sx / 0.97f, 0.02f, 0.45f), new Vector3(0.14f, 0.24f, 0.08f), "Ojo");
            _capOnHead = BuildCap(_pivot, _color).transform;
            _capOnHead.localPosition = new Vector3(0f, 1.38f, 0f);
            var ring = Greybox.Prim(PrimitiveType.Cylinder, Root.transform, _color, new Vector3(0f, 0.02f, 0f), new Vector3(1.4f, 0.01f, 1.4f), "Aro");
            _ring = ring.GetComponent<Renderer>();
            _shield = Greybox.Prim(PrimitiveType.Sphere, Root.transform, new Color(1f, 0.7f, 0f), new Vector3(0f, 0.8f, 0f), Vector3.one * 1.9f, "Escudo").transform;
            _aim = Greybox.Prim(PrimitiveType.Cube, Root.transform, _color, new Vector3(0f, 0.05f, 1.5f), new Vector3(0.2f, 0.02f, 2f), "Mira").transform;
            Root.transform.localScale = Vector3.one * 1.15f;
        }

        public static GameObject BuildCap(Transform parent, Color color)
        {
            var cap = new GameObject("Gorra");
            cap.transform.SetParent(parent, false);
            Greybox.Prim(PrimitiveType.Sphere, cap.transform, color, Vector3.zero, new Vector3(0.72f, 0.42f, 0.72f), "Copa");
            Greybox.Prim(PrimitiveType.Cylinder, cap.transform, color * 0.75f, new Vector3(0f, -0.05f, 0.25f), new Vector3(0.5f, 0.02f, 0.4f), "Visera");
            return cap;
        }

        public void Sync(PlayerState p, Tuning t, float dt)
        {
            Root.SetActive(p.Alive);
            if (!p.Alive) return;
            var target = Greybox.ToUnity(p.Pos);
            // suavizado exponencial: sirve igual a 60 Hz (local) que a 30 Hz (snapshots de red)
            Root.transform.position = Vector3.Lerp(Root.transform.position, target, 1f - Mathf.Exp(-25f * dt));
            Root.transform.rotation = Quaternion.Slerp(Root.transform.rotation, Greybox.Facing(p.Facing), 1f - Mathf.Exp(-18f * dt));
            float speed01 = Mathf.Clamp01(p.Vel.Length / t.Player.Speed);
            _bob += dt * 16f * speed01;
            bool dashing = p.DashT > 0f;
            float charge = p.Charge01(t);
            float sy = dashing ? 0.8f : 1f - charge * 0.1f;
            float sxz = dashing ? 1.15f : 1f + charge * 0.06f;
            _pivot.localScale = new Vector3(sxz, sy, sxz);
            _pivot.localPosition = new Vector3(0f, Mathf.Abs(Mathf.Sin(_bob)) * 0.09f * speed01, 0f);
            _capOnHead.gameObject.SetActive(p.HasCap);
            _shield.gameObject.SetActive(p.Shield);
            _aim.gameObject.SetActive(p.Charging);
            if (p.Charging) _aim.localScale = new Vector3(0.2f, 0.02f, 2f + charge * 5f);
            if (p.Charging) _aim.localPosition = new Vector3(0f, 0.05f, (2f + charge * 5f) / 2f);
            bool blinkInvuln = p.SpawnInvuln > 0f && Mathf.FloorToInt(p.SpawnInvuln * 20f) % 2 == 1;
            _pivot.gameObject.SetActive(!blinkInvuln);
            var pc = p.Power != PowerUp.None ? Greybox.Hex(PowerUpInfo.Color(p.Power)) : _color;
            _ring.material.color = pc;
        }

        public void Destroy() => Object.Destroy(Root);
    }

    public sealed class CapView
    {
        public readonly GameObject Root;
        float _spin;

        public CapView(CapState c)
        {
            var color = Greybox.Hex(Greybox.PlayerColors[c.OwnerId % Greybox.PlayerColors.Length], c.Ghost ? 0.5f : 1f);
            Root = ToyView.BuildCap(null, color);
            Root.name = c.Ghost ? "Gorra fantasma" : $"Gorra de {c.OwnerId}";
            Root.transform.position = Greybox.ToUnity(c.Pos, c.Y);
        }

        public void Sync(CapState c, float dt)
        {
            bool visible = c.Mode != CapMode.Held && c.Mode != CapMode.Dead;
            Root.SetActive(visible);
            if (!visible) return;
            Root.transform.position = Vector3.Lerp(Root.transform.position, Greybox.ToUnity(c.Pos, c.Y), 1f - Mathf.Exp(-30f * dt));
            _spin += (c.Lethal ? 1400f : 200f) * dt;
            Root.transform.rotation = Quaternion.Euler(c.Lethal ? 12f : 0f, _spin, 0f);
            Root.transform.localScale = Vector3.one * (c.Radius / 0.38f);
        }

        public void Destroy() => Object.Destroy(Root);
    }

    public sealed class PickupView
    {
        public readonly GameObject Root;
        float _t;

        public PickupView(Pickup p)
        {
            Root = new GameObject($"Lata · {PowerUpInfo.Name(p.Type)}");
            Greybox.Prim(PrimitiveType.Cylinder, Root.transform, Greybox.Hex(PowerUpInfo.Color(p.Type)), Vector3.zero, new Vector3(0.44f, 0.3f, 0.44f), "Lata");
            Greybox.Prim(PrimitiveType.Sphere, Root.transform, Greybox.Hex(0xBDBDBD), new Vector3(0f, 0.3f, 0f), Vector3.one * 0.44f, "Tapa");
            Root.transform.position = Greybox.ToUnity(p.Pos, 0.55f);
        }

        public void Sync(Pickup p, float dt)
        {
            _t += dt;
            Root.transform.position = Greybox.ToUnity(p.Pos, 0.55f + Mathf.Sin(_t * 3f) * 0.12f);
            Root.transform.rotation = Quaternion.Euler(0f, _t * 120f, 15f);
        }

        public void Destroy() => Object.Destroy(Root);
    }

    /// <summary>Mantiene una vista por entidad y las sincroniza con la simulación cada cuadro.</summary>
    public sealed class WorldView
    {
        public ArenaView Arena { get; private set; }
        readonly List<ToyView> _toys = new List<ToyView>();
        readonly Dictionary<int, CapView> _caps = new Dictionary<int, CapView>();
        readonly Dictionary<int, PickupView> _pickups = new Dictionary<int, PickupView>();
        readonly HashSet<int> _seen = new HashSet<int>();
        readonly List<int> _remove = new List<int>();
        MatchSimulation _sim;

        public void Bind(MatchSimulation sim)
        {
            Clear();
            _sim = sim;
            Arena = new ArenaView(sim.Arena);
            foreach (var p in sim.Players) _toys.Add(new ToyView(p));
        }

        public Vector3 PlayerPosition(int id) => id >= 0 && id < _toys.Count ? _toys[id].Root.transform.position : Vector3.zero;

        public void Sync(float dt)
        {
            if (_sim == null) return;
            Arena.Sync(Time.time);
            for (int i = 0; i < _sim.Players.Count; i++) _toys[i].Sync(_sim.Players[i], _sim.T, dt);

            _seen.Clear();
            foreach (var c in _sim.Caps)
            {
                _seen.Add(c.Id);
                if (!_caps.TryGetValue(c.Id, out var v)) _caps[c.Id] = v = new CapView(c);
                v.Sync(c, dt);
            }
            _remove.Clear();
            foreach (var kv in _caps) if (!_seen.Contains(kv.Key)) _remove.Add(kv.Key);
            foreach (var id in _remove) { _caps[id].Destroy(); _caps.Remove(id); }

            _seen.Clear();
            foreach (var p in _sim.Pickups)
            {
                _seen.Add(p.Id);
                if (!_pickups.TryGetValue(p.Id, out var v)) _pickups[p.Id] = v = new PickupView(p);
                v.Sync(p, dt);
            }
            _remove.Clear();
            foreach (var kv in _pickups) if (!_seen.Contains(kv.Key)) _remove.Add(kv.Key);
            foreach (var id in _remove) { _pickups[id].Destroy(); _pickups.Remove(id); }
        }

        public void Clear()
        {
            Arena?.Destroy(); Arena = null;
            foreach (var t in _toys) t.Destroy();
            _toys.Clear();
            foreach (var c in _caps.Values) c.Destroy();
            _caps.Clear();
            foreach (var p in _pickups.Values) p.Destroy();
            _pickups.Clear();
            _sim = null;
        }
    }
}
