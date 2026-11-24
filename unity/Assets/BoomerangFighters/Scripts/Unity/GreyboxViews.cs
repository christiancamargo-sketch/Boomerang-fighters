// ─────────────────────────────────────────────────────────────
// VISTAS "GREYBOX": dibujan el estado de la simulación con primitivas
// de Unity (cubos, esferas, cápsulas). Así el proyecto corre desde el
// primer día sin arte. Cuando lleguen los modelos de los muñequitos,
// se reemplaza ToyView por un prefab con animaciones; la simulación
// no cambia.
// v0.2: muros de muerte súbita, charco que crece, muro de tag, lata
// caliente sobre la cabeza, marca de Tatuaje, rastro rayado de la gorra
// desviada, grada de La Reta y diseños del vestidor (playera y gorra).
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

        public static Color PlayerColor(int id) => Hex(PlayerColors[((id % PlayerColors.Length) + PlayerColors.Length) % PlayerColors.Length]);

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

        /// <summary>
        /// Destruye un objeto de la vista junto con los materiales propios que le creó Prim
        /// (Object.Destroy del GameObject no borra los materiales: sin esto se acumulan
        /// con cada gorra fantasma, charco, muro o lata que aparece y desaparece).
        /// </summary>
        public static void DestroyTree(GameObject go)
        {
            if (go == null) return;
            foreach (var r in go.GetComponentsInChildren<Renderer>(true))
                foreach (var m in r.sharedMaterials) if (m != null && m != _base) Object.Destroy(m);
            Object.Destroy(go);
        }

        /// <summary>Pone una textura de diseño en la primitiva (mainTexture = _BaseMap en URP, _MainTex en Built-in).</summary>
        public static void ApplyTexture(GameObject go, Texture tex)
        {
            if (go == null || tex == null) return;
            var r = go.GetComponent<Renderer>();
            r.material.mainTexture = tex;
            r.material.color = Color.white;
        }

        // Paleta Okabe-Ito (apta para daltonismo). El color del jugador va en el aro y la gorra, nunca en la ropa.
        public static readonly uint[] PlayerColors = { 0xE69F00, 0x56B4E9, 0xF0E442, 0xCC79A7, 0x009E73, 0x0072B2, 0xD55E00, 0xFFFFFF };
        // Vinil de cada muñequito (orden de ProgressRules.SpeciesNames): Osote, Michi, Gotita, Seri, Zurci, Pegote, Lienzo, Axo
        public static readonly uint[] VinylColors = { 0xF3E4CF, 0x2F2F36, 0xBFE7FF, 0xFFD6E5, 0xB8F28A, 0xFF8F70, 0xF7F7F2, 0xF6A9C9 };

        public const uint Ink = 0x121016, Bone = 0xF4F1EA, Magenta = 0xE6007E;
        /// <summary>Altura de la grada de La Reta (los que esperan su turno se ven ahí).</summary>
        public const float BenchHeight = 2.4f;
    }

    /// <summary>Lo que trae puesto cada jugador en la vista.</summary>
    public struct Outfit
    {
        public Texture Shirt;
        public DesignDef Cap;
    }

    public sealed class ArenaView
    {
        public readonly GameObject Root;
        readonly List<GameObject> _lasers = new List<GameObject>();
        readonly Arena _arena;
        readonly Transform _sdLeft, _sdRight, _sdEdgeL, _sdEdgeR, _sdMarkL, _sdMarkR;
        const float SdWallH = 1.6f;

        public ArenaView(Arena arena, GameMode mode)
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

            // Muerte súbita (A2): muros de tinta que avanzan desde los lados cortos + raya de aviso en el piso
            _sdLeft = Greybox.Prim(PrimitiveType.Cube, Root.transform, Greybox.Hex(Greybox.Ink), Vector3.zero, Vector3.one, "Muro súbito izq").transform;
            _sdRight = Greybox.Prim(PrimitiveType.Cube, Root.transform, Greybox.Hex(Greybox.Ink), Vector3.zero, Vector3.one, "Muro súbito der").transform;
            _sdEdgeL = Greybox.Prim(PrimitiveType.Cube, Root.transform, Greybox.Hex(Greybox.Magenta), Vector3.zero, Vector3.one, "Borde súbito izq").transform;
            _sdEdgeR = Greybox.Prim(PrimitiveType.Cube, Root.transform, Greybox.Hex(Greybox.Magenta), Vector3.zero, Vector3.one, "Borde súbito der").transform;
            _sdMarkL = Greybox.Prim(PrimitiveType.Cube, Root.transform, Greybox.Hex(Greybox.Magenta), Vector3.zero, new Vector3(0.12f, 0.02f, d.Depth), "Aviso súbito izq").transform;
            _sdMarkR = Greybox.Prim(PrimitiveType.Cube, Root.transform, Greybox.Hex(Greybox.Magenta), Vector3.zero, new Vector3(0.12f, 0.02f, d.Depth), "Aviso súbito der").transform;
            foreach (var tr in new[] { _sdLeft, _sdRight, _sdEdgeL, _sdEdgeR, _sdMarkL, _sdMarkR }) tr.gameObject.SetActive(false);

            // La Reta: grada detrás del muro del fondo para los que esperan su turno
            if (mode == GameMode.LaReta)
                Greybox.Prim(PrimitiveType.Cube, Root.transform, Greybox.Hex(0x4A3F5C), new Vector3(0f, Greybox.BenchHeight / 2f, d.Depth / 2f + 1.5f), new Vector3(d.Width + 2 * t, Greybox.BenchHeight, 1.8f), "Grada");
        }

        public void Sync(MatchSimulation sim, float time)
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

            float inset = _arena.WallInset, hw = _arena.HalfW, depth = _arena.Def.Depth;
            bool walls = inset > 0.005f;
            _sdLeft.gameObject.SetActive(walls); _sdRight.gameObject.SetActive(walls);
            _sdEdgeL.gameObject.SetActive(walls); _sdEdgeR.gameObject.SetActive(walls);
            if (walls)
            {
                _sdLeft.localPosition = new Vector3(-hw + inset / 2f, SdWallH / 2f, 0f);
                _sdRight.localPosition = new Vector3(hw - inset / 2f, SdWallH / 2f, 0f);
                _sdLeft.localScale = _sdRight.localScale = new Vector3(inset, SdWallH, depth);
                _sdEdgeL.localPosition = new Vector3(-hw + inset, SdWallH / 2f, 0f);
                _sdEdgeR.localPosition = new Vector3(hw - inset, SdWallH / 2f, 0f);
                _sdEdgeL.localScale = _sdEdgeR.localScale = new Vector3(0.1f, SdWallH + 0.06f, depth + 0.02f);
            }
            // raya de aviso: hasta dónde van a llegar los muros (parpadea en el aviso y mientras avanzan)
            float target = sim.T.Round.SuddenDeathMinWidth * 0.5f;
            float maxInset = Mathf.Max(0f, hw - target);
            bool mark = sim.SuddenDeath == SuddenDeathStage.Warn || (sim.SuddenDeath == SuddenDeathStage.Closing && inset < maxInset - 0.01f);
            bool on = mark && (sim.SuddenDeath == SuddenDeathStage.Closing || Mathf.Sin(time * 24f) > 0f);
            _sdMarkL.gameObject.SetActive(on); _sdMarkR.gameObject.SetActive(on);
            if (on)
            {
                _sdMarkL.localPosition = new Vector3(-target, 0.015f, 0f);
                _sdMarkR.localPosition = new Vector3(target, 0.015f, 0f);
            }
        }

        public void Destroy() => Greybox.DestroyTree(Root);
    }

    /// <summary>Muñequito greybox: cabeza grande, cuerpo chico (playera), gorra (visible solo si la tiene).</summary>
    public sealed class ToyView
    {
        public readonly GameObject Root;
        readonly Transform _pivot, _capOnHead, _shield, _aim, _mark;
        readonly Renderer _ring;
        readonly Color _color;
        float _bob, _markSpin;
        public const float HeadTop = 2.0f; // altura (sin escala) de la parte de arriba de la gorra

        public ToyView(PlayerState p, Outfit outfit)
        {
            _color = Greybox.PlayerColor(p.Id);
            Root = new GameObject($"Jugador {p.Id} · {p.Name}");
            _pivot = new GameObject("Pivote").transform;
            _pivot.SetParent(Root.transform, false);
            var vinyl = Greybox.Hex(Greybox.VinylColors[((p.Species % Greybox.VinylColors.Length) + Greybox.VinylColors.Length) % Greybox.VinylColors.Length]);
            var body = Greybox.Prim(PrimitiveType.Capsule, _pivot, Color.white, new Vector3(0f, 0.48f, 0f), new Vector3(0.54f, 0.38f, 0.54f), "Cuerpo (playera)");
            Greybox.ApplyTexture(body, outfit.Shirt);
            // v0.3: mangas, piernas de jogger y tenis (como el prototipo web)
            foreach (float sx in new[] { -1f, 1f })
            {
                var sleeve = Greybox.Prim(PrimitiveType.Cylinder, _pivot, Color.white, new Vector3(0.29f * sx, 0.6f, 0f), new Vector3(0.22f, 0.075f, 0.22f), "Manga");
                sleeve.transform.localRotation = Quaternion.Euler(0f, 0f, 55f * sx);
                Greybox.ApplyTexture(sleeve, outfit.Shirt);
                Greybox.Prim(PrimitiveType.Cylinder, _pivot, Greybox.Hex(0x2A2D36), new Vector3(0.13f * sx, 0.2f, 0f), new Vector3(0.15f, 0.1f, 0.15f), "Pierna");
                Greybox.Prim(PrimitiveType.Capsule, _pivot, Greybox.Hex(0xF4F4F0), new Vector3(0.14f * sx, 0.06f, 0.03f), new Vector3(0.15f, 0.08f, 0.26f), "Suela");
                Greybox.Prim(PrimitiveType.Sphere, _pivot, Greybox.Hex(0x1C1B20), new Vector3(0.14f * sx, 0.11f, 0.02f), new Vector3(0.16f, 0.11f, 0.22f), "Tenis");
            }
            var head = Greybox.Prim(PrimitiveType.Sphere, _pivot, vinyl, new Vector3(0f, 1.08f, 0f), new Vector3(0.97f, 0.85f, 0.9f), "Cabeza");
            foreach (float sx in new[] { -0.16f, 0.16f })
                Greybox.Prim(PrimitiveType.Sphere, head.transform, Color.black, new Vector3(sx / 0.97f, 0.02f, 0.45f), new Vector3(0.14f, 0.24f, 0.08f), "Ojo");
            // boca y chapitas
            Greybox.Prim(PrimitiveType.Sphere, head.transform, Greybox.Hex(0x141414), new Vector3(0f, -0.15f, 0.47f), new Vector3(0.1f, 0.03f, 0.03f), "Boca");
            foreach (float sx in new[] { -0.27f, 0.27f })
                Greybox.Prim(PrimitiveType.Sphere, head.transform, Greybox.Hex(0xFF8FB1), new Vector3(sx, -0.09f, 0.4f), new Vector3(0.11f, 0.06f, 0.03f), "Chapita");
            _capOnHead = BuildCap(_pivot, _color, outfit.Cap).transform;
            _capOnHead.localPosition = new Vector3(0f, 1.38f, 0f);
            var ring = Greybox.Prim(PrimitiveType.Cylinder, Root.transform, _color, new Vector3(0f, 0.02f, 0f), new Vector3(1.4f, 0.01f, 1.4f), "Aro");
            _ring = ring.GetComponent<Renderer>();
            _shield = Greybox.Prim(PrimitiveType.Sphere, Root.transform, new Color(1f, 0.7f, 0f), new Vector3(0f, 0.8f, 0f), Vector3.one * 1.9f, "Escudo").transform;
            _aim = Greybox.Prim(PrimitiveType.Cube, Root.transform, _color, new Vector3(0f, 0.05f, 1.5f), new Vector3(0.2f, 0.02f, 2f), "Mira").transform;
            // Tatuaje: rombo de "te están persiguiendo" sobre la cabeza
            _mark = new GameObject("Marca Tatuaje").transform;
            _mark.SetParent(Root.transform, false);
            _mark.localPosition = new Vector3(0f, HeadTop + 0.35f, 0f);
            Greybox.Prim(PrimitiveType.Cube, _mark, Greybox.Hex(Greybox.Magenta), Vector3.zero, new Vector3(0.36f, 0.36f, 0.08f), "Rombo").transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            Greybox.Prim(PrimitiveType.Cube, _mark, Greybox.Hex(Greybox.Ink), new Vector3(0f, 0f, -0.01f), new Vector3(0.16f, 0.16f, 0.1f), "Centro").transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            _mark.gameObject.SetActive(false);
            Root.transform.localScale = Vector3.one * 1.15f;
        }

        /// <summary>Gorra greybox. Con diseño: la COPA lleva la textura y la visera su color; sin diseño, todo del color del jugador.</summary>
        public static GameObject BuildCap(Transform parent, Color color, DesignDef design = null)
        {
            var cap = new GameObject("Gorra");
            cap.transform.SetParent(parent, false);
            var tex = design != null && !design.PlayerColor ? design.Texture : null;
            var crown = Greybox.Prim(PrimitiveType.Sphere, cap.transform, color, Vector3.zero, new Vector3(0.72f, 0.42f, 0.72f), "Copa");
            Greybox.ApplyTexture(crown, tex);
            var visor = tex != null ? Greybox.Hex(design.Visor) : color * 0.75f;
            // visera plana de snapback (como las gorras reales de INKCAP)
            Greybox.Prim(PrimitiveType.Cylinder, cap.transform, visor, new Vector3(0f, -0.04f, 0.28f), new Vector3(0.62f, 0.015f, 0.5f), "Visera");
            // parche redondo blanco sobre la visera en gorras con diseño
            if (tex != null) Greybox.Prim(PrimitiveType.Cylinder, cap.transform, Greybox.Hex(0xF4F4F0), new Vector3(0.12f, -0.02f, 0.4f), new Vector3(0.14f, 0.008f, 0.14f), "Parche");
            // botón del color del jugador: aunque la copa tenga diseño, se sabe de quién es
            if (tex != null) Greybox.Prim(PrimitiveType.Sphere, cap.transform, color, new Vector3(0f, 0.2f, 0f), Vector3.one * 0.12f, "Botón");
            return cap;
        }

        /// <summary>Posición (mundo) de arriba de la cabeza, para la lata caliente o letreros.</summary>
        public Vector3 HeadWorld => Root.transform.position + Vector3.up * (HeadTop * Root.transform.localScale.y);
        public bool Visible => Root.activeSelf;

        public void Sync(PlayerState p, Tuning t, float dt, bool marked)
        {
            bool benched = p.Benched && !p.Alive;
            Root.SetActive(p.Alive || benched);
            if (!p.Alive && !benched) return;
            var target = Greybox.ToUnity(p.Pos, benched ? Greybox.BenchHeight : 0f);
            // suavizado exponencial: sirve igual a 60 Hz (local) que a 30 Hz (snapshots de red)
            Root.transform.position = Vector3.Lerp(Root.transform.position, target, 1f - Mathf.Exp(-25f * dt));
            Root.transform.rotation = Quaternion.Slerp(Root.transform.rotation, benched ? Quaternion.LookRotation(Vector3.back) : Greybox.Facing(p.Facing), 1f - Mathf.Exp(-18f * dt));
            Root.transform.localScale = Vector3.one * (benched ? 0.9f : 1.15f);
            float speed01 = benched ? 0f : Mathf.Clamp01(p.Vel.Length / t.Player.Speed);
            _bob += dt * (benched ? 4f : 16f * speed01);
            bool dashing = p.DashT > 0f || p.PushT > 0f;
            float charge = p.Charge01(t);
            float sy = dashing ? 0.8f : 1f - charge * 0.1f;
            float sxz = dashing ? 1.15f : 1f + charge * 0.06f;
            _pivot.localScale = new Vector3(sxz, sy, sxz);
            _pivot.localPosition = new Vector3(0f, Mathf.Abs(Mathf.Sin(_bob)) * (benched ? 0.05f : 0.09f * speed01), 0f);
            _capOnHead.gameObject.SetActive(p.HasCap);
            _shield.gameObject.SetActive(p.Shield && !benched);
            _aim.gameObject.SetActive(p.Charging && !benched);
            if (p.Charging) _aim.localScale = new Vector3(0.2f, 0.02f, 2f + charge * 5f);
            if (p.Charging) _aim.localPosition = new Vector3(0f, 0.05f, (2f + charge * 5f) / 2f);
            bool blinkInvuln = !benched && p.SpawnInvuln > 0f && Mathf.FloorToInt(p.SpawnInvuln * 20f) % 2 == 1;
            _pivot.gameObject.SetActive(!blinkInvuln);
            _ring.gameObject.SetActive(!benched);
            var pc = p.Power != PowerUp.None ? Greybox.Hex(PowerUpInfo.Color(p.Power)) : _color;
            _ring.material.color = pc;
            // marca de Tatuaje: gira y "late" para que se vea en el celular
            _mark.gameObject.SetActive(marked && !benched);
            if (marked)
            {
                _markSpin += dt * 240f;
                _mark.localRotation = Quaternion.Euler(0f, _markSpin, 0f);
                _mark.localScale = Vector3.one * (1f + Mathf.Abs(Mathf.Sin(_markSpin * 0.05f)) * 0.25f);
            }
        }

        public void Destroy() => Greybox.DestroyTree(Root);
    }

    public sealed class CapView
    {
        public readonly GameObject Root;
        float _spin, _trailT;
        // "Te la regreso": rastro rayado con el color de quien desvió
        readonly List<Transform> _trail = new List<Transform>();
        readonly Vector3[] _trailPos = new Vector3[10];
        int _trailHead, _trailCount, _trailOwner = -2;
        GameObject _trailRoot;

        public CapView(CapState c, DesignDef design)
        {
            var color = Greybox.PlayerColor(c.OwnerId);
            if (c.Ghost) color *= 0.6f;
            Root = ToyView.BuildCap(null, color, c.Ghost ? null : design);
            Root.name = c.Ghost ? "Gorra fantasma" : $"Gorra de {c.OwnerId}";
            Root.transform.position = Greybox.ToUnity(c.Pos, c.Y);
        }

        public void Sync(CapState c, float dt)
        {
            bool visible = c.Mode != CapMode.Held && c.Mode != CapMode.Dead;
            Root.SetActive(visible);
            SyncTrail(c, visible && c.Deflected, dt);
            if (!visible) return;
            Root.transform.position = Vector3.Lerp(Root.transform.position, Greybox.ToUnity(c.Pos, c.Y), 1f - Mathf.Exp(-30f * dt));
            _spin += (c.Lethal ? 1400f : 200f) * dt;
            Root.transform.rotation = Quaternion.Euler(c.Lethal ? 12f : 0f, _spin, 0f);
            Root.transform.localScale = Vector3.one * (c.Radius / 0.38f);
        }

        void SyncTrail(CapState c, bool on, float dt)
        {
            if (!on)
            {
                if (_trailRoot != null && _trailRoot.activeSelf) _trailRoot.SetActive(false);
                _trailCount = 0;
                return;
            }
            if (_trailRoot == null) _trailRoot = new GameObject("Rastro te la regreso");
            _trailRoot.SetActive(true);
            if (_trailOwner != c.DeflectBy)
            {
                // cambiar colores: rayas alternadas color de quien desvió / tinta
                _trailOwner = c.DeflectBy;
                foreach (var t in _trail) Greybox.DestroyTree(t.gameObject);
                _trail.Clear();
                var col = Greybox.PlayerColor(c.DeflectBy < 0 ? c.OwnerId : c.DeflectBy);
                for (int i = 0; i < _trailPos.Length; i++)
                    _trail.Add(Greybox.Prim(PrimitiveType.Sphere, _trailRoot.transform, i % 2 == 0 ? col : Greybox.Hex(Greybox.Ink), Vector3.zero, Vector3.one * 0.2f, "Raya").transform);
            }
            _trailT -= dt;
            if (_trailT <= 0f)
            {
                _trailT = 0.025f;
                _trailPos[_trailHead] = Root.transform.position;
                _trailHead = (_trailHead + 1) % _trailPos.Length;
                _trailCount = Mathf.Min(_trailCount + 1, _trailPos.Length);
            }
            for (int i = 0; i < _trail.Count; i++)
            {
                bool has = i < _trailCount;
                _trail[i].gameObject.SetActive(has);
                if (!has) continue;
                int idx = (_trailHead - 1 - i + _trailPos.Length * 2) % _trailPos.Length;
                _trail[i].position = _trailPos[idx];
                _trail[i].localScale = Vector3.one * (0.34f * (1f - i / (float)_trail.Count) + 0.06f);
            }
        }

        public void Destroy()
        {
            Greybox.DestroyTree(Root);
            if (_trailRoot != null) Greybox.DestroyTree(_trailRoot);
        }
    }

    public sealed class PickupView
    {
        public readonly GameObject Root;
        float _t;

        public PickupView(Pickup p)
        {
            // latas negro tinta / blanco hueso con ícono (nunca de color de jugador): el color de la técnica va en la banda
            Root = new GameObject($"Lata · {PowerUpInfo.Name(p.Type)}");
            Greybox.Prim(PrimitiveType.Cylinder, Root.transform, Greybox.Hex(Greybox.Ink), Vector3.zero, new Vector3(0.44f, 0.3f, 0.44f), "Lata");
            Greybox.Prim(PrimitiveType.Cylinder, Root.transform, Greybox.Hex(PowerUpInfo.Color(p.Type)), Vector3.zero, new Vector3(0.46f, 0.08f, 0.46f), "Banda");
            Greybox.Prim(PrimitiveType.Sphere, Root.transform, Greybox.Hex(Greybox.Bone), new Vector3(0f, 0.3f, 0f), Vector3.one * 0.44f, "Tapa");
            Root.transform.position = Greybox.ToUnity(p.Pos, 0.55f);
        }

        public void Sync(Pickup p, float dt)
        {
            _t += dt;
            Root.transform.position = Greybox.ToUnity(p.Pos, 0.55f + Mathf.Sin(_t * 3f) * 0.12f);
            Root.transform.rotation = Quaternion.Euler(0f, _t * 120f, 15f);
        }

        public void Destroy() => Greybox.DestroyTree(Root);
    }

    /// <summary>Tinta derramada · Charco: disco de tinta que crece y "late" justo antes de noquear.</summary>
    public sealed class PuddleView
    {
        public readonly GameObject Root;
        readonly Transform _disc, _rim;
        readonly Renderer _rimR;

        public PuddleView(Puddle pd)
        {
            Root = new GameObject($"Charco de {pd.OwnerId}");
            var owner = Greybox.PlayerColor(pd.OwnerId);
            _rim = Greybox.Prim(PrimitiveType.Cylinder, Root.transform, owner, new Vector3(0f, 0.018f, 0f), Vector3.one, "Orilla").transform;
            _rimR = _rim.GetComponent<Renderer>();
            _disc = Greybox.Prim(PrimitiveType.Cylinder, Root.transform, Greybox.Hex(0x0B0A10), new Vector3(0f, 0.024f, 0f), Vector3.one, "Tinta").transform;
            Root.transform.position = Greybox.ToUnity(pd.Pos);
        }

        public void Sync(Puddle pd, Tuning t, float time)
        {
            Root.transform.position = Greybox.ToUnity(pd.Pos);
            float r = Mathf.Max(0.05f, pd.Radius);
            float grow01 = Mathf.Clamp01(pd.T / Mathf.Max(0.01f, t.Cap.PuddleGrowTime));
            _disc.localScale = new Vector3(r * 2f, 0.004f, r * 2f);
            _rim.localScale = new Vector3(r * 2f + 0.22f, 0.003f, r * 2f + 0.22f);
            // mientras crece la orilla parpadea: "¡sal de ahí!"
            bool blink = grow01 < 1f && Mathf.Sin(time * 40f) > 0f;
            _rimR.material.color = blink ? Color.white : Greybox.PlayerColor(pd.OwnerId);
        }

        public void Destroy() => Greybox.DestroyTree(Root);
    }

    /// <summary>Grafiti · Muro de Tag: esténcil de 2.5 × 0.4 m que bloquea 3 s.</summary>
    public sealed class TagWallView
    {
        public readonly GameObject Root;
        const float H = 1.3f;

        public TagWallView(TagWall w)
        {
            Root = new GameObject($"Muro de Tag de {w.OwnerId}");
            var owner = Greybox.PlayerColor(w.OwnerId);
            Greybox.Prim(PrimitiveType.Cube, Root.transform, Greybox.Hex(Greybox.Bone), new Vector3(0f, H / 2f, 0f), new Vector3(1f, H, 1f), "Muro");
            // franjas del esténcil con el color de quien lo pintó
            Greybox.Prim(PrimitiveType.Cube, Root.transform, owner, new Vector3(0f, H * 0.72f, 0f), new Vector3(1.04f, 0.18f, 1.04f), "Franja");
            Greybox.Prim(PrimitiveType.Cube, Root.transform, Greybox.Hex(Greybox.Ink), new Vector3(0f, H * 0.4f, 0f), new Vector3(1.04f, 0.1f, 1.04f), "Franja tinta");
            Apply(w, 1f);
        }

        void Apply(TagWall w, float k)
        {
            Root.transform.position = Greybox.ToUnity(w.Pos);
            Root.transform.rotation = Greybox.Facing(w.Dir); // eje Z local = eje largo
            Root.transform.localScale = new Vector3(w.HalfThick * 2f, k, w.HalfLen * 2f);
        }

        public void Sync(TagWall w)
        {
            // se "despinta" en los últimos 0.4 s
            Apply(w, Mathf.Clamp01(w.Life / 0.4f) * 0.9f + 0.1f);
        }

        public void Destroy() => Greybox.DestroyTree(Root);
    }

    /// <summary>Lata Caliente: lata con mecha sobre la cabeza de quien la trae; parpadea más rápido al final.</summary>
    public sealed class HotCanView
    {
        public readonly GameObject Root;
        readonly Renderer _body, _spark;
        float _phase;

        public HotCanView()
        {
            Root = new GameObject("Lata caliente");
            _body = Greybox.Prim(PrimitiveType.Cylinder, Root.transform, Greybox.Hex(0xD55E00), Vector3.zero, new Vector3(0.42f, 0.26f, 0.42f), "Lata").GetComponent<Renderer>();
            Greybox.Prim(PrimitiveType.Cylinder, Root.transform, Greybox.Hex(Greybox.Ink), Vector3.zero, new Vector3(0.44f, 0.07f, 0.44f), "Banda");
            Greybox.Prim(PrimitiveType.Cylinder, Root.transform, Greybox.Hex(Greybox.Bone), new Vector3(0f, 0.36f, 0f), new Vector3(0.04f, 0.12f, 0.04f), "Mecha");
            _spark = Greybox.Prim(PrimitiveType.Sphere, Root.transform, Greybox.Hex(0xF0E442), new Vector3(0f, 0.5f, 0f), Vector3.one * 0.14f, "Chispa").GetComponent<Renderer>();
            Root.SetActive(false);
        }

        public void Sync(bool on, Vector3 head, float secondsLeft, float dt)
        {
            Root.SetActive(on);
            if (!on) return;
            Root.transform.position = Vector3.Lerp(Root.transform.position, head + Vector3.up * 0.35f, 1f - Mathf.Exp(-30f * dt));
            float rate = Mathf.Lerp(18f, 3f, Mathf.Clamp01(secondsLeft / 10f)); // < 10 s: cada vez más rápido
            _phase += dt * rate;
            bool hot = Mathf.Sin(_phase * Mathf.PI * 2f) > 0f;
            _body.material.color = hot ? Greybox.Hex(0xFF3B1F) : Greybox.Hex(0xD55E00);
            _spark.transform.localScale = Vector3.one * (hot ? 0.2f : 0.11f);
            Root.transform.rotation = Quaternion.Euler(Mathf.Sin(_phase * 3f) * 8f, 0f, Mathf.Cos(_phase * 2f) * 8f);
        }

        public void Destroy() => Greybox.DestroyTree(Root);
    }

    /// <summary>Mantiene una vista por entidad y las sincroniza con la simulación cada cuadro.</summary>
    public sealed class WorldView
    {
        public ArenaView Arena { get; private set; }
        readonly List<ToyView> _toys = new List<ToyView>();
        readonly List<Outfit> _outfits = new List<Outfit>();
        readonly Dictionary<int, CapView> _caps = new Dictionary<int, CapView>();
        readonly Dictionary<int, PickupView> _pickups = new Dictionary<int, PickupView>();
        readonly Dictionary<int, PuddleView> _puddles = new Dictionary<int, PuddleView>();
        readonly Dictionary<int, TagWallView> _walls = new Dictionary<int, TagWallView>();
        readonly HashSet<int> _seen = new HashSet<int>();
        readonly HashSet<int> _marked = new HashSet<int>();
        readonly List<int> _remove = new List<int>();
        HotCanView _hotCan;
        MatchSimulation _sim;

        /// <summary>outfitFor(i) da la playera y la gorra del jugador i (null = sin diseño).</summary>
        public void Bind(MatchSimulation sim, System.Func<int, Outfit> outfitFor = null)
        {
            Clear();
            _sim = sim;
            Arena = new ArenaView(sim.Arena, sim.Mode);
            foreach (var p in sim.Players)
            {
                var o = outfitFor != null ? outfitFor(p.Id) : default;
                _outfits.Add(o);
                _toys.Add(new ToyView(p, o));
            }
            _hotCan = new HotCanView();
        }

        public Vector3 PlayerPosition(int id) => id >= 0 && id < _toys.Count ? _toys[id].Root.transform.position : Vector3.zero;
        public Vector3 PlayerHead(int id) => id >= 0 && id < _toys.Count ? _toys[id].HeadWorld : Vector3.zero;

        public void Sync(float dt)
        {
            if (_sim == null) return;
            float time = Time.time;
            Arena.Sync(_sim, time);

            _marked.Clear();
            foreach (var c in _sim.Caps) if (c.Lethal && c.MarkedId >= 0) _marked.Add(c.MarkedId);
            for (int i = 0; i < _sim.Players.Count; i++) _toys[i].Sync(_sim.Players[i], _sim.T, dt, _marked.Contains(i));

            int holder = _sim.HotCanHolder;
            bool can = _sim.Mode == GameMode.LataCaliente && holder >= 0 && holder < _toys.Count && _sim.Players[holder].Alive;
            _hotCan.Sync(can, can ? _toys[holder].HeadWorld : Vector3.zero, _sim.HotCanTimer, dt);

            _seen.Clear();
            foreach (var c in _sim.Caps)
            {
                _seen.Add(c.Id);
                if (!_caps.TryGetValue(c.Id, out var v))
                {
                    var design = c.OwnerId >= 0 && c.OwnerId < _outfits.Count ? _outfits[c.OwnerId].Cap : null;
                    _caps[c.Id] = v = new CapView(c, design);
                }
                v.Sync(c, dt);
            }
            Sweep(_caps, cv => cv.Destroy());

            _seen.Clear();
            foreach (var p in _sim.Pickups)
            {
                _seen.Add(p.Id);
                if (!_pickups.TryGetValue(p.Id, out var v)) _pickups[p.Id] = v = new PickupView(p);
                v.Sync(p, dt);
            }
            Sweep(_pickups, pv => pv.Destroy());

            _seen.Clear();
            foreach (var pd in _sim.Puddles)
            {
                _seen.Add(pd.Id);
                if (!_puddles.TryGetValue(pd.Id, out var v)) _puddles[pd.Id] = v = new PuddleView(pd);
                v.Sync(pd, _sim.T, time);
            }
            Sweep(_puddles, pv => pv.Destroy());

            _seen.Clear();
            foreach (var w in _sim.TagWalls)
            {
                _seen.Add(w.Id);
                if (!_walls.TryGetValue(w.Id, out var v)) _walls[w.Id] = v = new TagWallView(w);
                v.Sync(w);
            }
            Sweep(_walls, wv => wv.Destroy());
        }

        void Sweep<T>(Dictionary<int, T> map, System.Action<T> destroy)
        {
            _remove.Clear();
            foreach (var kv in map) if (!_seen.Contains(kv.Key)) _remove.Add(kv.Key);
            foreach (var id in _remove) { destroy(map[id]); map.Remove(id); }
        }

        public void Clear()
        {
            Arena?.Destroy(); Arena = null;
            foreach (var t in _toys) t.Destroy();
            _toys.Clear();
            _outfits.Clear();
            foreach (var c in _caps.Values) c.Destroy();
            _caps.Clear();
            foreach (var p in _pickups.Values) p.Destroy();
            _pickups.Clear();
            foreach (var p in _puddles.Values) p.Destroy();
            _puddles.Clear();
            foreach (var w in _walls.Values) w.Destroy();
            _walls.Clear();
            _hotCan?.Destroy(); _hotCan = null;
            _sim = null;
        }
    }
}
