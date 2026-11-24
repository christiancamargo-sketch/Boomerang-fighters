// ─────────────────────────────────────────────────────────────
// ESCENOGRAFÍA GREYBOX (spec v0.3 §A y §B): lo que ArenaView arma
// alrededor de las reglas, todo con primitivas y sin colliders.
// • VACÍOS: el piso se construye en piezas alrededor de cada hueco;
//   adentro va un tiro oscuro (paredes con degradado y fondo negro) y
//   afuera la orilla de franjas amarillo/negro. Por estilo: neón en los
//   fosos del museo, escalones en la escalera de la tienda y un tablón
//   de puente entre los dos huecos de la azotea.
// • OBSTÁCULOS: los de siempre + racks de playeras y mesa de gorras de
//   la Pop-up Store.
// • MUNDO INKCAP: logo en muros y piso, cuadros y pósters con los
//   diseños de Resources/Disenos/Playeras, espectacular "GORRAZO by
//   INKCAP" en la azotea, pared de gorras y logo grande en la tienda.
//   La decoración no colisiona y va pegada a los muros, pintada en el
//   piso o fuera del área de juego. Si falta un recurso (logo, diseños)
//   se pone un panel de color liso: aquí no se inventa ningún logo.
// Nada de esto decide reglas: los vacíos y obstáculos salen de ArenaDef.
// Conversión de ejes: Unity = (sim.x, altura, -sim.y).
// ─────────────────────────────────────────────────────────────
using System.Collections.Generic;
using BoomerangFighters.Core;
using UnityEngine;

namespace BoomerangFighters.UnityView
{
    /// <summary>Algo que "late" (neón, marco iluminado): ArenaView le aclara el color cada cuadro.</summary>
    public struct Glow
    {
        public Renderer R;
        public Color Color;
        public float Phase;
    }

    /// <summary>Colores y medidas de la vista de cada escenario.</summary>
    public sealed class ArenaLook
    {
        public uint FloorHex = 0x3A3A42, WallHex = 0x7A4E44, PitHex = 0x7C7C88;
        public float WallHeight = 2.4f;
        /// <summary>Orilla de los vacíos en neón (museo) en vez de franjas amarillo/negro.</summary>
        public bool NeonRims;
        public Color Floor => Greybox.Hex(FloorHex);
        public Color Wall => Greybox.Hex(WallHex);
        public Color Pit => Greybox.Hex(PitHex);
    }

    public static class Scenery
    {
        public const float WallThickness = 0.6f;
        /// <summary>Ancho de la franja de precaución alrededor de un vacío.</summary>
        public const float RimWidth = 0.26f;
        // alturas de lo que va pintado en el piso (debajo de manchas de tinta 0.010–0.014, charcos y aros)
        const float FloorDecalY = 0.004f, RimY = 0.008f;
        const float Stuck = 0.012f; // separación de lo que va pegado a un muro (para que no parpadee)

        static readonly uint[] NeonColors = { 0x00E5FF, 0xFF2BD6 };
        static readonly uint[] FlatColors = { Greybox.Ink, Greybox.Magenta, Greybox.Bone };
        const uint Steel = 0x8E8E96;

        public static ArenaLook LookFor(string id)
        {
            switch (id)
            {
                case "museo": return new ArenaLook { FloorHex = 0xE9E4DA, WallHex = 0xECE6DC, PitHex = 0xB9B3AA, NeonRims = true };
                case "azotea": return new ArenaLook { FloorHex = 0xB5764F, WallHex = 0xD9A46C, PitHex = 0xA06A48, WallHeight = 1f };
                case "popup": return new ArenaLook { FloorHex = 0xC9C4BA, WallHex = 0x1D1C21, PitHex = 0x9A958C };
                default: return new ArenaLook();
            }
        }

        static Transform Group(Transform parent, string name, Vector3 localPos)
        {
            var g = new GameObject(name).transform;
            g.SetParent(parent, false);
            g.localPosition = localPos;
            return g;
        }

        // ═════════════════════════════════════════════════════
        // PISO Y VACÍOS
        // ═════════════════════════════════════════════════════

        /// <summary>Rectángulo del vacío en el plano XZ de Unity (Rect.y es la Z; z = −y de la simulación).</summary>
        public static Rect PitRect(Pit p) => new Rect(p.x - p.w / 2f, -p.y - p.d / 2f, p.w, p.d);

        /// <summary>Piso con agujeros + tiro, orilla y detalles de cada vacío.</summary>
        public static void BuildGround(Transform root, ArenaDef d, ArenaLook look, List<Glow> glows)
        {
            var pits = d.Pits ?? System.Array.Empty<Pit>();
            var rects = new Rect[pits.Length];
            for (int i = 0; i < pits.Length; i++) rects[i] = PitRect(pits[i]);

            // Dos "huecos" alineados con piso en medio (azotea) son UN solo tiro entre edificios
            // con un tablón encima: se dibuja el tiro corrido y el puente aparte.
            var shafts = new List<Rect>();
            var kinds = new List<string>();
            var bridges = new List<Rect>();
            var used = new bool[pits.Length];
            for (int i = 0; i < pits.Length; i++)
            {
                if (used[i]) continue;
                var r = rects[i];
                for (int j = i + 1; j < pits.Length; j++)
                {
                    if (used[j] || pits[i].Kind != "hueco" || pits[j].Kind != "hueco") continue;
                    var o = rects[j];
                    if (Mathf.Abs(o.xMin - r.xMin) > 0.01f || Mathf.Abs(o.width - r.width) > 0.01f) continue;
                    float g0 = Mathf.Min(r.yMax, o.yMax), g1 = Mathf.Max(r.yMin, o.yMin);
                    if (g1 - g0 < 0.05f) continue; // se tocan: no hay puente
                    bridges.Add(new Rect(r.xMin, g0, r.width, g1 - g0));
                    r = Rect.MinMaxRect(r.xMin, Mathf.Min(r.yMin, o.yMin), r.xMax, Mathf.Max(r.yMax, o.yMax));
                    used[j] = true;
                }
                shafts.Add(r);
                kinds.Add(pits[i].Kind);
            }

            BuildFloor(root, d, look.Floor, shafts);
            bool reachesFront = false;
            for (int i = 0; i < shafts.Count; i++)
            {
                BuildShaft(root, shafts[i], kinds[i], look);
                reachesFront |= shafts[i].yMin <= -d.Depth / 2f + 0.05f;
            }
            // Un tiro que llega hasta el frente de la arena (azotea) se asomaría por debajo del muro del
            // frente: lo tapa la fachada del edificio, una pared oscura bajo ese muro.
            if (reachesFront)
                Greybox.Prim(PrimitiveType.Cube, root, Greybox.Shade(look.Wall, 0.5f), new Vector3(0f, -3.5f, -d.Depth / 2f - WallThickness / 2f), new Vector3(d.Width + 2f * WallThickness, 7f, WallThickness), "Fachada");
            for (int i = 0; i < pits.Length; i++) BuildRim(root, d, rects[i], bridges, look, i, glows);
            foreach (var b in bridges) BuildBridge(root, b);
        }

        static void AddCut(List<float> cuts, float v, float min, float max)
        {
            v = Mathf.Clamp(v, min, max);
            foreach (float c in cuts) if (Mathf.Abs(c - v) < 0.001f) return;
            cuts.Add(v);
        }

        static bool InHole(List<Rect> holes, float x, float z)
        {
            foreach (var h in holes) if (x > h.xMin && x < h.xMax && z > h.yMin && z < h.yMax) return true;
            return false;
        }

        /// <summary>
        /// El piso es una rejilla cortada en las orillas de los vacíos: las celdas que caen dentro de uno
        /// no se dibujan (queda el agujero) y las demás se juntan por tiras para usar pocas piezas.
        /// </summary>
        static void BuildFloor(Transform root, ArenaDef d, Color color, List<Rect> holes)
        {
            float hw = d.Width / 2f, hd = d.Depth / 2f;
            var xs = new List<float> { -hw, hw };
            var zs = new List<float> { -hd, hd };
            foreach (var h in holes)
            {
                AddCut(xs, h.xMin, -hw, hw); AddCut(xs, h.xMax, -hw, hw);
                AddCut(zs, h.yMin, -hd, hd); AddCut(zs, h.yMax, -hd, hd);
            }
            xs.Sort();
            zs.Sort();
            var floor = Group(root, "Piso", Vector3.zero);
            for (int j = 0; j < zs.Count - 1; j++)
            {
                float z0 = zs[j], z1 = zs[j + 1];
                int start = -1;
                for (int i = 0; i < xs.Count; i++)
                {
                    bool solid = i < xs.Count - 1 && !InHole(holes, (xs[i] + xs[i + 1]) / 2f, (z0 + z1) / 2f);
                    if (solid && start < 0) start = i;
                    if (solid || start < 0) continue;
                    float x0 = xs[start], x1 = xs[i];
                    start = -1;
                    // 2 mm de traslape para que no se vea una rendija entre piezas vecinas
                    Greybox.Prim(PrimitiveType.Cube, floor, color, new Vector3((x0 + x1) / 2f, -0.05f, (z0 + z1) / 2f), new Vector3(x1 - x0 + 0.002f, 0.1f, z1 - z0 + 0.002f), "Pieza de piso");
                }
            }
        }

        static float ShaftDepth(string kind) => kind == "hueco" ? 6f : kind == "escalera" ? 2.7f : 3f;

        /// <summary>El hueco por dentro: 4 paredes que se oscurecen hacia abajo y un fondo negro.</summary>
        static void BuildShaft(Transform root, Rect r, string kind, ArenaLook look)
        {
            var g = Group(root, $"Vacío · {kind}", Vector3.zero);
            float depth = ShaftDepth(kind), top = -0.1f; // empieza bajo la losa del piso (se ve su canto)
            float cx = r.center.x, cz = r.center.y, cy = top - depth / 2f;
            Greybox.Prim(PrimitiveType.Cube, g, Greybox.Hex(0x050507), new Vector3(cx, top - depth - 0.05f, cz), new Vector3(r.width + 0.02f, 0.1f, r.height + 0.02f), "Fondo");
            var tint = look.Pit;
            // cada pared es un plano que mira hacia adentro del hueco (un Quad se ve desde su lado −Z local)
            ShaftWall(g, new Vector3(cx, cy, r.yMax), r.width, depth, 0f, tint, "Pared del fondo");
            ShaftWall(g, new Vector3(cx, cy, r.yMin), r.width, depth, 180f, Greybox.Shade(tint, 0.7f), "Pared del frente");
            ShaftWall(g, new Vector3(r.xMin, cy, cz), r.height, depth, -90f, Greybox.Shade(tint, 0.85f), "Pared izquierda");
            ShaftWall(g, new Vector3(r.xMax, cy, cz), r.height, depth, 90f, Greybox.Shade(tint, 0.85f), "Pared derecha");

            if (kind != "escalera") return;
            // Escalera al sótano: el primer escalón queda al fondo y bajan hacia la cámara (así se ven
            // las huellas y los peraltes); se van oscureciendo hasta perderse.
            const int steps = 6;
            float run = r.height / steps, rise = (depth - 0.5f) / steps, bottom = top - depth;
            for (int k = 0; k < steps; k++)
            {
                float tread = top - rise * (k + 1) + 0.1f;
                var c = Color.Lerp(Greybox.Hex(0xC9C4BA), Greybox.Hex(0x34323A), k / (steps - 1f));
                Greybox.Prim(PrimitiveType.Cube, g, c, new Vector3(cx, (tread + bottom) / 2f, r.yMax - (k + 0.5f) * run), new Vector3(r.width - 0.06f, tread - bottom, run), "Escalón");
            }
        }

        static void ShaftWall(Transform parent, Vector3 pos, float width, float depth, float yaw, Color tint, string name)
        {
            var q = Greybox.Flat(Greybox.Prim(PrimitiveType.Quad, parent, tint, pos, new Vector3(width, depth, 1f), name));
            q.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
            Greybox.ApplyTexture(q, BrandArt.PitGradient, tint);
        }

        /// <summary>
        /// Orilla de aviso alrededor de un vacío. No se pone del lado que da a un muro de la arena
        /// ni del lado por donde sigue el tablón de la azotea.
        /// </summary>
        static void BuildRim(Transform root, ArenaDef d, Rect r, List<Rect> bridges, ArenaLook look, int index, List<Glow> glows)
        {
            float hw = d.Width / 2f, hd = d.Depth / 2f;
            const float e = 0.05f;
            bool left = r.xMin > -hw + e, right = r.xMax < hw - e, near = r.yMin > -hd + e, far = r.yMax < hd - e;
            foreach (var b in bridges)
            {
                if (Mathf.Abs(b.xMin - r.xMin) > 0.01f) continue;
                if (Mathf.Abs(b.yMin - r.yMax) < 0.01f) far = false;
                if (Mathf.Abs(b.yMax - r.yMin) < 0.01f) near = false;
            }
            var g = Group(root, "Orilla de vacío", Vector3.zero);
            float w = RimWidth;
            // las tiras del fondo y del frente cubren también las esquinas
            float x0 = r.xMin - (left ? w : 0f), x1 = r.xMax + (right ? w : 0f);
            if (far) RimStrip(g, (x0 + x1) / 2f, r.yMax + w / 2f, x1 - x0, 0f, look, index, glows);
            if (near) RimStrip(g, (x0 + x1) / 2f, r.yMin - w / 2f, x1 - x0, 0f, look, index, glows);
            if (left) RimStrip(g, r.xMin - w / 2f, r.center.y, r.height, 90f, look, index, glows);
            if (right) RimStrip(g, r.xMax + w / 2f, r.center.y, r.height, 90f, look, index, glows);
        }

        static void RimStrip(Transform parent, float x, float z, float length, float yaw, ArenaLook look, int index, List<Glow> glows)
        {
            // plano acostado en el piso (X 90° = boca arriba); su eje X local corre a lo largo de la tira
            var q = Greybox.Flat(Greybox.Prim(PrimitiveType.Quad, parent, Color.white, new Vector3(x, RimY, z), new Vector3(length, RimWidth, 1f), "Franja"));
            q.transform.localRotation = Quaternion.Euler(90f, yaw, 0f);
            if (look.NeonRims)
            {
                var c = Greybox.Hex(NeonColors[index % NeonColors.Length]);
                var r = q.GetComponent<Renderer>();
                r.material.color = c;
                glows.Add(new Glow { R = r, Color = c, Phase = index * 1.7f });
            }
            else Greybox.ApplyTexture(q, BrandArt.HazardStripes, new Rect(0f, 0f, length / 0.5f, 1f)); // una franja cada 25 cm
        }

        /// <summary>Tablón de puente (azotea): tablas atravesadas sobre el tiro, apoyadas en las dos azoteas.</summary>
        static void BuildBridge(Transform root, Rect b)
        {
            var g = Group(root, "Tablón de puente", Vector3.zero);
            int n = Mathf.Max(1, Mathf.RoundToInt(b.height / 0.5f));
            float bw = b.height / n;
            for (int i = 0; i < n; i++)
                Greybox.Prim(PrimitiveType.Cube, g, Greybox.Hex(i % 2 == 0 ? 0xC89B5Au : 0xB58648u), new Vector3(b.center.x, -0.024f, b.yMin + (i + 0.5f) * bw), new Vector3(b.width + 0.5f, 0.06f, bw - 0.04f), "Tabla");
        }

        // ═════════════════════════════════════════════════════
        // OBSTÁCULOS
        // ═════════════════════════════════════════════════════

        public static void BuildObstacle(Transform root, Box o, int index)
        {
            if (o.Kind == "rack") { BuildRack(root, o, index); return; }
            if (o.Kind == "table") { BuildCapTable(root, o); return; }
            Color c;
            switch (o.Kind)
            {
                case "dumpster": c = Greybox.Hex(0x1F7A4D); break;
                case "pedestal": c = Greybox.Hex(0xF4F1EA); break;
                case "tinaco": c = Greybox.Hex(0x111111); break;
                case "bench": c = Greybox.Hex(0x6D4C41); break;
                case "planter": c = Greybox.Hex(0x8D6E63); break;
                case "gas": c = Greybox.Hex(0xE0E0E0); break;
                default: c = Greybox.Hex(0xBC8A5F); break;
            }
            var type = o.Kind == "tinaco" || o.Kind == "gas" ? PrimitiveType.Cylinder : PrimitiveType.Cube;
            var s = type == PrimitiveType.Cylinder ? new Vector3(o.w, o.h / 2f, o.d) : new Vector3(o.w, o.h, o.d);
            Greybox.Prim(type, root, c, Greybox.ToUnity(new Vec2(o.x, o.y), o.h / 2f), s, o.Kind);
        }

        /// <summary>Rack de playeras: base, postes, tubo y playeras colgadas que llenan todo el volumen que bloquea.</summary>
        static void BuildRack(Transform root, Box o, int index)
        {
            var g = Group(root, "Rack de playeras", Greybox.ToUnity(new Vec2(o.x, o.y)));
            Greybox.Prim(PrimitiveType.Cube, g, Greybox.Hex(Greybox.Ink), new Vector3(0f, 0.04f, 0f), new Vector3(o.w, 0.08f, o.d), "Base");
            foreach (float sx in new[] { -1f, 1f })
                Greybox.Prim(PrimitiveType.Cube, g, Greybox.Hex(Steel), new Vector3(sx * (o.w / 2f - 0.05f), o.h / 2f, 0f), new Vector3(0.08f, o.h, 0.08f), "Poste");
            Greybox.Prim(PrimitiveType.Cube, g, Greybox.Hex(Steel), new Vector3(0f, o.h - 0.04f, 0f), new Vector3(o.w, 0.07f, 0.07f), "Tubo");
            var shirts = ArtistShirts();
            float span = o.w - 0.3f, top = o.h - 0.1f, hang = Mathf.Min(0.95f, top - 0.15f);
            int n = Mathf.Max(3, Mathf.RoundToInt(span / 0.3f));
            float step = span / n;
            for (int i = 0; i < n; i++)
            {
                // de canto se ven como prendas colgadas: cada una del color de la tela de un diseño
                Color c = shirts.Count > 0 ? FabricOf(shirts[(i + index) % shirts.Count]) : Greybox.Hex(FlatColors[(i + index) % FlatColors.Length]);
                Greybox.Prim(PrimitiveType.Cube, g, c, new Vector3(-span / 2f + (i + 0.5f) * step, top - hang / 2f, 0f), new Vector3(step - 0.06f, hang, o.d - 0.1f), "Playera colgada");
            }
            // una playera de frente (hacia la cámara) con su estampado
            if (shirts.Count > 0) HangShirt(g, shirts[index % shirts.Count], new Vector3(0f, top, -o.d / 2f - 0.03f), 0.8f, 0f);
        }

        /// <summary>Mesa de gorras (baja: las gorras pasan por encima): mostrador con 3 gorras de exhibición.</summary>
        static void BuildCapTable(Transform root, Box o)
        {
            var g = Group(root, "Mesa de gorras", Greybox.ToUnity(new Vec2(o.x, o.y)));
            Greybox.Prim(PrimitiveType.Cube, g, Greybox.Hex(Greybox.Ink), new Vector3(0f, (o.h - 0.08f) / 2f, 0f), new Vector3(o.w - 0.3f, o.h - 0.08f, o.d - 0.3f), "Base");
            Greybox.Prim(PrimitiveType.Cube, g, Greybox.Hex(Greybox.Bone), new Vector3(0f, o.h - 0.04f, 0f), new Vector3(o.w, 0.08f, o.d), "Cubierta");
            var caps = ArtistCaps();
            for (int i = 0; i < 3; i++)
            {
                var design = caps.Count > 0 ? caps[(i * 2 + 1) % caps.Count] : null;
                DisplayCap(g, design, new Vector3((i - 1) * Mathf.Min(0.7f, o.w * 0.32f), o.h + 0.035f, -0.05f), 180f + (i - 1) * 18f, 0.8f);
            }
        }

        // ═════════════════════════════════════════════════════
        // UTILERÍA DE MARCA
        // ═════════════════════════════════════════════════════

        static List<DesignDef> ArtistShirts()
        {
            var list = new List<DesignDef>();
            foreach (var s in DesignLibrary.Shirts) if (s.FromArtist && s.Texture != null) list.Add(s);
            return list;
        }

        static List<DesignDef> ArtistCaps()
        {
            var list = new List<DesignDef>();
            foreach (var c in DesignLibrary.Caps) if (c.FromArtist && c.Texture != null) list.Add(c);
            return list;
        }

        static Color FabricOf(DesignDef d)
        {
            var look = d.Look;
            return look.Wrap != null ? Greybox.Hex(Greybox.Bone) : look.Solid;
        }

        static Texture ArtOf(List<DesignDef> art, int i) => art.Count > 0 ? art[i % art.Count].Texture : null;

        /// <summary>Gorra de exhibición (la misma del juego, con el botón negro tinta: no es de ningún jugador).</summary>
        static void DisplayCap(Transform parent, DesignDef design, Vector3 localPos, float yaw, float scale)
        {
            var cap = ToyView.BuildCap(parent, Greybox.Hex(Greybox.Ink), design).transform;
            cap.localPosition = localPos;
            cap.localRotation = Quaternion.Euler(0f, yaw, 0f);
            cap.localScale = Vector3.one * scale;
        }

        /// <summary>
        /// Panel plano en el plano XY local, que se ve desde su lado −Z local: marco de color + imagen.
        /// Sin textura queda el color liso "flat". Devuelve el marco (o la imagen si no hay marco).
        /// </summary>
        static GameObject Panel(Transform parent, Vector3 localPos, float w, float h, Texture tex, Color flat, Color frame, float border, float roll = 0f)
        {
            var g = Group(parent, "Panel", localPos);
            g.localRotation = Quaternion.Euler(0f, 0f, roll);
            GameObject fr = null;
            if (border > 0f) fr = Greybox.Flat(Greybox.Prim(PrimitiveType.Quad, g, frame, Vector3.zero, new Vector3(w + border * 2f, h + border * 2f, 1f), "Marco"));
            var img = Greybox.Flat(Greybox.Prim(PrimitiveType.Quad, g, flat, new Vector3(0f, 0f, -0.006f), new Vector3(w, h, 1f), "Imagen"));
            Greybox.ApplyTexture(img, tex);
            return fr != null ? fr : img;
        }

        /// <summary>
        /// Punto pegado a la cara interior de un muro, girado para que un Panel se vea desde la arena.
        /// wall: 0 = fondo ("along" es X), 1 = izquierdo, 2 = derecho ("along" es Z).
        /// </summary>
        static Transform OnWall(Transform parent, ArenaDef d, int wall, float along, float height, string name)
        {
            float hw = d.Width / 2f, hd = d.Depth / 2f;
            if (wall == 0) return Group(parent, name, new Vector3(along, height, hd - Stuck));
            var g = Group(parent, name, new Vector3(wall == 1 ? -hw + Stuck : hw - Stuck, height, along));
            g.localRotation = Quaternion.Euler(0f, wall == 1 ? -90f : 90f, 0f);
            return g;
        }

        /// <summary>Algo pintado en una superficie horizontal (piso, tapa de un tinaco): plano boca arriba que se lee desde la cámara.</summary>
        static void FlatDecal(Transform parent, Texture tex, Vector3 pos, float width)
        {
            if (tex == null) return;
            var q = Greybox.Flat(Greybox.Prim(PrimitiveType.Quad, parent, Color.white, pos, new Vector3(width, width / BrandArt.Aspect(tex), 1f), "Pintado en el piso"));
            q.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            Greybox.ApplyTexture(q, tex);
        }

        /// <summary>
        /// Playera colgada vista de frente (se ve desde −Z local): tela lisa en forma de "T" y su estampado.
        /// "topCenter" es de donde cuelga; con "tilt" positivo el borde de abajo vuela hacia la cámara.
        /// </summary>
        static void HangShirt(Transform parent, DesignDef d, Vector3 topCenter, float size, float tilt)
        {
            var g = Group(parent, "Playera", topCenter);
            g.localRotation = Quaternion.Euler(tilt, 0f, 0f);
            var look = d.Look;
            Color fabric = look.Wrap != null ? Color.white : look.Solid;
            var body = Greybox.Prim(PrimitiveType.Quad, g, fabric, new Vector3(0f, -size * 0.5f, 0f), new Vector3(size * 0.78f, size, 1f), "Tela");
            var sleeves = Greybox.Flat(Greybox.Prim(PrimitiveType.Quad, g, fabric, new Vector3(0f, -size * 0.17f, 0.002f), new Vector3(size * 1.25f, size * 0.3f, 1f), "Mangas"));
            Greybox.ApplyTexture(body, look.Wrap);
            Greybox.ApplyTexture(sleeves, look.Wrap);
            if (look.Decal == null) return;
            float side = size * 0.6f;
            var print = Greybox.Flat(Greybox.Prim(PrimitiveType.Quad, g, Color.white, new Vector3(0f, -size * 0.52f, -0.006f), new Vector3(side, side, 1f), "Estampado"));
            Greybox.ApplyTexture(print, look.Decal, look.Crop(1f));
        }

        // ═════════════════════════════════════════════════════
        // MUNDO INKCAP POR ESCENARIO
        // ═════════════════════════════════════════════════════

        public static void Decorate(Transform root, ArenaDef d, GameMode mode, ArenaLook look, List<Glow> glows)
        {
            var g = Group(root, "Decoración INKCAP", Vector3.zero);
            switch (d.Id)
            {
                case "callejon": DecorCallejon(g, d, look); break;
                case "museo": DecorMuseo(g, d, look); break;
                case "azotea": DecorAzotea(g, d, mode); break;
                case "popup": DecorPopup(g, d, look, glows); break;
                default: WallLogo(g, d, look, Greybox.Hex(Greybox.Magenta)); break; // escenario nuevo sin adorno propio: al menos el logo
            }
        }

        /// <summary>Logo grande en el muro del fondo (o un panel negro tinta liso si no hay archivo de logo). Devuelve el marco.</summary>
        static GameObject WallLogo(Transform g, ArenaDef d, ArenaLook look, Color frame, uint ink = Greybox.Bone)
        {
            var logo = BrandArt.LogoSign(Greybox.Ink, ink);
            float h = Mathf.Min(2.2f, look.WallHeight - 0.2f), w = h * (logo != null ? BrandArt.Aspect(logo) : 2.9f);
            var spot = OnWall(g, d, 0, 0f, look.WallHeight / 2f, "Logo INKCAP");
            return Panel(spot, Vector3.zero, w, h, logo, Greybox.Hex(Greybox.Ink), frame, 0.08f);
        }

        /// <summary>Pósters o cuadros con los diseños de los artistas a lo largo de un muro.</summary>
        static void WallArt(Transform g, ArenaDef d, int wall, float[] along, float height, float size, List<DesignDef> art, int first, Color frame, float border, bool crooked)
        {
            for (int i = 0; i < along.Length; i++)
            {
                int k = first + i;
                var spot = OnWall(g, d, wall, along[i], height + (crooked ? (k % 2) * 0.1f : 0f), wall == 0 ? "Cuadro" : "Cuadro lateral");
                Panel(spot, Vector3.zero, size, size, ArtOf(art, k), Greybox.Hex(FlatColors[k % FlatColors.Length]), frame, border, crooked ? (k % 2 == 0 ? 3f : -4f) : 0f);
            }
        }

        // B1 · Callejón Grafiti: mural con el logo, paste-ups de los artistas, stickers en los contenedores y esténcil en el piso
        static void DecorCallejon(Transform g, ArenaDef d, ArenaLook look)
        {
            var art = ArtistShirts();
            WallLogo(g, d, look, Greybox.Hex(Greybox.Magenta));
            var paper = Greybox.Hex(Greybox.Bone);
            WallArt(g, d, 0, new[] { -9.9f, -7.7f, -5.5f, 5.5f, 7.7f, 9.9f }, 1.1f, 1.7f, art, 0, paper, 0.06f, true);
            WallArt(g, d, 1, new[] { -2.6f, 2.9f }, 1.15f, 1.6f, art, 1, paper, 0.06f, true);
            WallArt(g, d, 2, new[] { -2.9f, 2.6f }, 1.15f, 1.6f, art, 3, paper, 0.06f, true);

            // sticker blanco con el logo en la cara de cada contenedor que ve la cámara
            var sticker = BrandArt.LogoSign(Greybox.Bone, Greybox.Ink);
            float sw = 1.3f, sh = sw / (sticker != null ? BrandArt.Aspect(sticker) : 2.9f);
            foreach (var o in d.Obstacles)
                if (o.Kind == "dumpster") Panel(g, new Vector3(o.x, o.h * 0.6f, -o.y - o.d / 2f - Stuck), sw, sh, sticker, Greybox.Hex(Greybox.Bone), default, 0f);

            // esténcil gastado del logo en el piso, al frente (zona sin vacíos ni cajas)
            FlatDecal(g, BrandArt.LogoSign(look.FloorHex, Greybox.Bone, 0.4f), new Vector3(0f, FloorDecalY, -d.Depth / 2f + 1.7f), 5.2f);
        }

        // B2 · Museo de Arte: manta "EXPOSICIÓN INKCAP", cuadros de los artistas con su cédula, logo en el piso y vitrina con las gorras
        static void DecorMuseo(Transform g, ArenaDef d, ArenaLook look)
        {
            var art = ArtistShirts();
            var banner = BrandArt.Marquee("EXPOSICIÓN", null, true, Greybox.Ink, Greybox.Bone, Greybox.Magenta);
            const float bh = 1.2f;
            Panel(OnWall(g, d, 0, 0f, 1.65f, "Manta EXPOSICIÓN INKCAP"), Vector3.zero, bh * BrandArt.Aspect(banner), bh, banner, Greybox.Hex(Greybox.Ink), Greybox.Hex(Greybox.Magenta), 0.05f);

            var frame = Greybox.Hex(0x1A1A1E);
            float[] back = { -10.8f, -8.4f, -6f, 6f, 8.4f, 10.8f };
            WallArt(g, d, 0, back, 1.25f, 1.7f, art, 0, frame, 0.09f, false);
            foreach (float x in back) // cédula "Artista · INKCAP" junto a cada cuadro
                Greybox.Flat(Greybox.Prim(PrimitiveType.Quad, g, Greybox.Hex(0x6D6A72), new Vector3(x + 1.08f, 0.5f, d.Depth / 2f - Stuck), new Vector3(0.26f, 0.16f, 1f), "Cédula"));
            WallArt(g, d, 1, new[] { -3f, 3f }, 1.25f, 1.7f, art, 2, frame, 0.09f, false);
            WallArt(g, d, 2, new[] { -3f, 3f }, 1.25f, 1.7f, art, 4, frame, 0.09f, false);

            // logo incrustado en el piso de la sala, tenue (al frente, lejos de pedestales y fosos)
            FlatDecal(g, BrandArt.LogoSign(look.FloorHex, Greybox.Ink, 0.22f), new Vector3(0f, FloorDecalY, -d.Depth / 2f + 1.7f), 5.2f);

            // vitrina fuera del área de juego (detrás del muro izquierdo): las gorras reales sobre un pedestal
            var caps = ArtistCaps();
            // (el pedestal es alto para que las gorras asomen por encima del muro desde la cámara)
            const float ph = 1.6f;
            var v = Group(g, "Vitrina de gorras", new Vector3(-(d.Width / 2f + WallThickness + 0.75f), 0f, 0f));
            Greybox.Prim(PrimitiveType.Cube, v, Greybox.Hex(Greybox.Bone), new Vector3(0f, ph / 2f, 0f), new Vector3(1.1f, ph, 5.4f), "Pedestal");
            Greybox.Prim(PrimitiveType.Cube, v, Greybox.Hex(Greybox.Ink), new Vector3(0f, ph + 0.015f, 0f), new Vector3(1.14f, 0.03f, 5.44f), "Paño");
            for (int i = 0; i < 6; i++)
                DisplayCap(v, caps.Count > 0 ? caps[i % caps.Count] : null, new Vector3(0f, ph + 0.08f, -2.15f + i * 0.86f), 90f, 0.9f);
        }

        // B3 · Azotea CDMX: espectacular "GORRAZO by INKCAP", tendedero con playeras de artistas y logo pintado en los tinacos
        static void DecorAzotea(Transform g, ArenaDef d, GameMode mode)
        {
            float back = d.Depth / 2f + WallThickness;       // cara de atrás del pretil del fondo
            float left = -d.Width / 4f - 0.2f;               // centro de la azotea izquierda
            var sign = BrandArt.Marquee("GORRAZO", "by", true, Greybox.Ink, Greybox.Bone, Greybox.Magenta);
            const float bh = 0.95f, border = 0.06f;
            float bw = bh * BrandArt.Aspect(sign);
            var board = Group(g, "Espectacular GORRAZO by INKCAP", Vector3.zero);
            if (mode == GameMode.LaReta)
            {
                // en La Reta ahí está la grada: el anuncio va pegado a su cara de enfrente
                board.localPosition = new Vector3(left, 1.75f, back - Stuck);
            }
            else
            {
                // detrás del pretil, recostado hacia atrás para que la cámara lo vea de frente (lo sostienen dos postes)
                const float tilt = 45f;
                float reach = (bh / 2f + border) * 0.7071f;
                board.localPosition = new Vector3(left, 1.02f + reach, back + 0.1f + reach);
                board.localRotation = Quaternion.Euler(tilt, 0f, 0f);
                foreach (float sx in new[] { -1f, 1f })
                    Greybox.Prim(PrimitiveType.Cube, g, Greybox.Hex(Steel), new Vector3(left + sx * bw * 0.36f, 0.6f, back + 0.1f + reach + 0.12f), new Vector3(0.14f, 1.2f, 0.14f), "Poste del espectacular");
            }
            Panel(board, Vector3.zero, bw, bh, sign, Greybox.Hex(Greybox.Ink), Greybox.Hex(Greybox.Magenta), border);

            // tendedero en la azotea derecha (en La Reta ese lugar es de la grada)
            if (mode != GameMode.LaReta)
            {
                var shirts = ArtistShirts();
                if (shirts.Count == 0) { shirts.Add(DesignLibrary.Shirt("basica-negra")); shirts.Add(DesignLibrary.Shirt("basica-blanca")); }
                float x0 = 2.6f, x1 = d.Width / 2f - 1.2f, z = back + 0.45f;
                foreach (float x in new[] { x0, x1 })
                    Greybox.Prim(PrimitiveType.Cube, g, Greybox.Hex(Steel), new Vector3(x, 1f, z), new Vector3(0.1f, 2f, 0.1f), "Poste del tendedero");
                Greybox.Prim(PrimitiveType.Cube, g, Greybox.Hex(Greybox.Bone), new Vector3((x0 + x1) / 2f, 1.95f, z), new Vector3(x1 - x0, 0.03f, 0.03f), "Lazo");
                const int n = 6;
                for (int i = 0; i < n; i++)
                    HangShirt(g, shirts[i % shirts.Count], new Vector3(Mathf.Lerp(x0 + 0.8f, x1 - 0.8f, i / (n - 1f)), 1.93f, z), 0.95f, 22f);
            }

            // logo pintado en la tapa de los tinacos (es la cara que ve la cámara)
            var paint = BrandArt.LogoSign(0x111111, Greybox.Bone);
            foreach (var o in d.Obstacles)
                if (o.Kind == "tinaco") FlatDecal(g, paint, new Vector3(o.x, o.h + 0.006f, -o.y), Mathf.Min(o.w, o.d) * 0.84f);
        }

        // B4 · Pop-up Store INKCAP: logo grande iluminado, pared de gorras, cartel ESTRENOS con playeras y logo en el piso
        static void DecorPopup(Transform g, ArenaDef d, ArenaLook look, List<Glow> glows)
        {
            float hd = d.Depth / 2f;
            var art = ArtistShirts();
            var magenta = Greybox.Hex(Greybox.Magenta);

            // el marco del logo "late" como un letrero iluminado
            var frame = WallLogo(g, d, look, magenta, 0xFFFFFF);
            glows.Add(new Glow { R = frame.GetComponent<Renderer>(), Color = magenta, Phase = 0f });

            // pared de gorras (derecha): tablero + repisa alta con las snapbacks viendo a la cámara
            const float cx = 7.6f;
            Panel(OnWall(g, d, 0, cx, 1.2f, "Pared de gorras"), Vector3.zero, 5.4f, 2.1f, null, Greybox.Hex(0x2B2A30), Greybox.Hex(Greybox.Bone), 0.05f);
            Greybox.Prim(PrimitiveType.Cube, g, Greybox.Hex(Greybox.Bone), new Vector3(cx, 1.78f, hd - 0.27f), new Vector3(5.3f, 0.05f, 0.5f), "Repisa");
            var caps = ArtistCaps();
            for (int i = 0; i < 6; i++)
                DisplayCap(g, caps.Count > 0 ? caps[i % caps.Count] : null, new Vector3(cx - 2.1f + i * 0.84f, 1.845f, hd - 0.36f), 180f, 0.95f);

            // cartel "ESTRENOS" (izquierda) con playeras en exhibición debajo
            var sign = BrandArt.Marquee("ESTRENOS", null, false, Greybox.Magenta, 0xFFFFFF, Greybox.Ink);
            const float sh = 0.62f;
            Panel(OnWall(g, d, 0, -cx, 2.02f, "Cartel ESTRENOS"), Vector3.zero, sh * BrandArt.Aspect(sign), sh, sign, magenta, Greybox.Hex(Greybox.Bone), 0.04f);
            var shirts = new List<DesignDef>(art);
            if (shirts.Count == 0) { shirts.Add(DesignLibrary.Shirt("basica-negra")); shirts.Add(DesignLibrary.Shirt("basica-blanca")); }
            for (int i = 0; i < 3; i++)
                HangShirt(g, shirts[i % shirts.Count], new Vector3(-cx + (i - 1) * 1.6f, 1.62f, hd - 0.05f), 1.2f, 0f);

            // pósters en los muros laterales
            var paper = Greybox.Hex(Greybox.Bone);
            WallArt(g, d, 1, new[] { -1.2f, 1.6f }, 1.25f, 1.6f, art, 3, paper, 0.06f, false);
            WallArt(g, d, 2, new[] { -1.6f, 1.2f }, 1.25f, 1.6f, art, 1, paper, 0.06f, false);

            // logo en el piso de la entrada (al frente, entre los racks)
            FlatDecal(g, BrandArt.LogoSign(look.FloorHex, Greybox.Ink, 0.55f), new Vector3(0f, FloorDecalY, -hd + 1.7f), 4.6f);
        }
    }
}
