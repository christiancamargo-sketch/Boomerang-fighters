// ─────────────────────────────────────────────────────────────
// DISEÑOS DEL VESTIDOR (spec v0.2 §E y §F) y cómo se ven puestos.
// • Playeras: tela de color liso + el estampado como calcomanía al
//   frente y en la espalda (no se estira alrededor del cuerpo). Los
//   patrones corridos de ejemplo (camuflaje, rayas…) sí envuelven.
// • Gorras: copa de color liso + el logo o diseño SOLO en el panel de
//   enfrente; la silueta y el tamaño nunca cambian y el color del
//   jugador vive en el aro, el rastro y el botón.
// Se cargan solos los PNG que el equipo de arte deje en:
//   Assets/BoomerangFighters/Resources/Disenos/Playeras/*.png
//   Assets/BoomerangFighters/Resources/Disenos/Gorras/*.png
// (el nombre del archivo es el nombre del diseño). El color de la tela
// o de la copa se saca de la propia imagen (su color dominante) y la
// calcomanía muestra solo la zona donde está el estampado o el logo:
// ver DesignLook. Además hay unos diseños de ejemplo hechos con código
// para que el vestidor no salga vacío. LOGO: si existe
// Resources/Marca/logo_inkcap se dibuja; si no, se usa texto.
// Aquí NO se inventa ningún logo.
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoomerangFighters.UnityView
{
    public enum DesignUnlock { Shop, DailyOnly, CalendarOnly }

    public sealed class DesignDef
    {
        public string Id, Name, Rarity;
        public int Price;
        public bool IsCap;
        public bool FromArtist;          // PNG de Resources/Disenos
        public bool PlayerColor;         // gorra "Clásica": copa del color del jugador, sin textura
        public DesignUnlock Unlock = DesignUnlock.Shop;
        public uint Visor = 0x121016;    // color de la visera cuando el diseño envuelve la copa
        /// <summary>Patrón corrido (cuadros, camuflaje…): envuelve el cuerpo o la copa en vez de ir como calcomanía.</summary>
        public bool AllOver;
        /// <summary>Color liso ya conocido (básicas): no hace falta leer la textura.</summary>
        public uint? Fabric;
        internal Func<Texture2D> Make;
        Texture2D _tex;
        DesignLook _look;

        /// <summary>Textura del diseño (null en la gorra Clásica).</summary>
        public Texture2D Texture
        {
            get
            {
                if (_tex == null && Make != null) { _tex = Make(); if (_tex != null && string.IsNullOrEmpty(_tex.name)) _tex.name = Id; }
                return _tex;
            }
        }

        public bool Buyable => Unlock == DesignUnlock.Shop;

        /// <summary>Cómo se pone el diseño en el muñequito (tela + calcomanía, o patrón que envuelve). Se calcula una vez.</summary>
        public DesignLook Look => _look ?? (_look = DesignLook.For(this));
    }

    /// <summary>
    /// Cómo se ve un diseño puesto: color liso de la tela (o de la copa), y además un patrón que
    /// envuelve (Wrap) o una calcomanía (Decal) con la zona de la imagen donde está el estampado o el logo.
    /// </summary>
    public sealed class DesignLook
    {
        public Color Solid = Color.white;
        public Texture2D Wrap;
        public Texture2D Decal;
        /// <summary>Zona (UV, origen abajo a la izquierda) que ocupa el estampado o el logo dentro de Decal.</summary>
        public Rect Content = new Rect(0f, 0f, 1f, 1f);
        /// <summary>Ancho / alto de la textura de la calcomanía.</summary>
        public float TexAspect = 1f;

        const int S = 64;               // la imagen se analiza reducida a 64 × 64
        const int ContentDiff = 40;     // qué tanto debe alejarse un píxel del color de la tela para contar como estampado

        /// <summary>
        /// Zona UV que debe mostrar una calcomanía de proporción "aspect" (ancho/alto) para que el
        /// contenido quepa entero con un poco de aire, sin deformarse y sin salirse de la imagen.
        /// </summary>
        public Rect Crop(float aspect, float margin = 1.12f)
        {
            float ta = Mathf.Max(0.01f, TexAspect);
            aspect = Mathf.Max(0.01f, aspect);
            float h = Mathf.Max(Content.height, Content.width * ta / aspect) * margin;
            float w = h * aspect / ta;
            float k = Mathf.Min(1f, Mathf.Min(1f / w, 1f / h));
            w *= k; h *= k;
            float x = Mathf.Clamp(Content.center.x - w / 2f, 0f, 1f - w), y = Mathf.Clamp(Content.center.y - h / 2f, 0f, 1f - h);
            return new Rect(x, y, w, h);
        }

        public static DesignLook For(DesignDef d)
        {
            var look = new DesignLook();
            if (d == null || d.PlayerColor) return look;                     // gorra Clásica: del color del jugador
            if (d.Fabric.HasValue) { look.Solid = Greybox.Hex(d.Fabric.Value); return look; }
            var tex = d.Texture;
            if (tex == null) return look;
            look.TexAspect = tex.width / (float)Mathf.Max(1, tex.height);
            if (d.AllOver) { look.Wrap = tex; return look; }
            // si no se pueden leer los píxeles (sin GPU), el diseño envuelve como en v0.2
            if (!Analyze(tex, d.IsCap, look)) { look.Solid = Color.white; look.Wrap = tex; look.Decal = null; }
            return look;
        }

        static bool Analyze(Texture2D tex, bool cap, DesignLook look)
        {
            var px = BrandArt.ReadPixels(tex, S, S);
            if (px == null) return false;

            // color dominante entre los píxeles opacos: la cubeta más repetida (4 bits por canal) y su promedio
            var buckets = new Dictionary<int, int>();
            int bestKey = -1, bestN = 0, clear = 0, opaque = 0;
            long lum = 0;
            foreach (var c in px)
            {
                if (c.a < 128) { clear++; continue; }
                opaque++;
                lum += (c.r * 3 + c.g * 6 + c.b) / 10;
                int key = ((c.r >> 4) << 8) | ((c.g >> 4) << 4) | (c.b >> 4);
                buckets.TryGetValue(key, out int n);
                buckets[key] = ++n;
                if (n > bestN) { bestN = n; bestKey = key; }
            }
            if (bestKey < 0) return false; // imagen totalmente transparente
            long r = 0, g = 0, b = 0;
            foreach (var c in px)
            {
                if (c.a < 128 || (((c.r >> 4) << 8) | ((c.g >> 4) << 4) | (c.b >> 4)) != bestKey) continue;
                r += c.r; g += c.g; b += c.b;
            }
            var dom = new Color32((byte)(r / bestN), (byte)(g / bestN), (byte)(b / bestN), 255);

            // estampado con fondo transparente: tela negra o blanca, la que más contraste con el estampado
            bool cutout = clear > px.Length / 20;
            if (cutout) dom = lum / opaque > 140 ? new Color32(28, 28, 31, 255) : new Color32(242, 242, 238, 255);
            look.Solid = dom;

            if (!Bounds(px, dom, cutout, 0, S, out var content)) return true; // imagen lisa: solo color
            // gorras con el logo repetido a la izquierda y a la derecha (plantilla de 2 paneles): se usa el de la izquierda
            if (cap && !Bounds(px, dom, cutout, S / 2 - 2, S / 2 + 2, out _)
                && Bounds(px, dom, cutout, 0, S / 2 - 2, out var left) && Bounds(px, dom, cutout, S / 2 + 2, S, out _))
                content = left;
            look.Content = content;
            look.Decal = tex;
            if (cutout)
            {
                int w = Mathf.Min(256, tex.width), h = Mathf.Max(1, Mathf.RoundToInt(w / look.TexAspect));
                var flat = BrandArt.Flatten(tex, w, h, dom);
                if (flat != null) look.Decal = flat;
            }
            return true;
        }

        /// <summary>Caja (UV) de los píxeles "con dibujo" entre las columnas x0 (incluida) y x1 (excluida).</summary>
        static bool Bounds(Color32[] px, Color32 dom, bool cutout, int x0, int x1, out Rect uv)
        {
            int minX = S, maxX = -1, minY = S, maxY = -1;
            for (int y = 0; y < S; y++)
                for (int x = x0; x < x1; x++)
                {
                    var c = px[y * S + x];
                    bool on = cutout ? c.a >= 128 : Mathf.Abs(c.r - dom.r) + Mathf.Abs(c.g - dom.g) + Mathf.Abs(c.b - dom.b) > ContentDiff;
                    if (!on) continue;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            if (maxX < 0) { uv = default; return false; }
            uv = new Rect(minX / (float)S, minY / (float)S, (maxX - minX + 1) / (float)S, (maxY - minY + 1) / (float)S);
            return true;
        }
    }

    public static class DesignLibrary
    {
        public const string ShirtsFolder = "Disenos/Playeras";
        public const string CapsFolder = "Disenos/Gorras";
        public const string LogoPath = "Marca/logo_inkcap";
        public const int ArtistShirtPrice = 300, ArtistCapPrice = 150;

        static readonly List<DesignDef> _shirts = new List<DesignDef>();
        static readonly List<DesignDef> _caps = new List<DesignDef>();
        static readonly Dictionary<string, DesignDef> _byShirt = new Dictionary<string, DesignDef>();
        static readonly Dictionary<string, DesignDef> _byCap = new Dictionary<string, DesignDef>();
        static bool _loaded;
        static Texture2D _logo;

        public static IReadOnlyList<DesignDef> Shirts { get { EnsureLoaded(); return _shirts; } }
        public static IReadOnlyList<DesignDef> Caps { get { EnsureLoaded(); return _caps; } }
        /// <summary>Logo de INKCAP si el archivo existe; null si no (entonces se usa texto).</summary>
        public static Texture2D Logo { get { EnsureLoaded(); return _logo; } }
        public static int ArtistShirtCount { get; private set; }
        public static int ArtistCapCount { get; private set; }

        // Gorras de la semana (premio del día 7): diseño INKCAP en tipografía/colores de marca, sin logo inventado.
        static readonly (string id, string name, uint bg, uint band, uint visor)[] Weekly =
        {
            ("semana-tinta", "INKCAP Tinta", 0x121016, 0x56B4E9, 0xF4F1EA),
            ("semana-naranja", "INKCAP Naranja", 0xE69F00, 0xF4F1EA, 0x121016),
            ("semana-verde", "INKCAP Verde", 0x009E73, 0x121016, 0x121016),
            ("semana-rosa", "INKCAP Rosa", 0xCC79A7, 0xF4F1EA, 0x121016),
        };

        public static void EnsureLoaded()
        {
            if (_loaded) return;
            _loaded = true;

            // ── playeras de ejemplo (hechas con código) ──
            // (las básicas son tela lisa; "allOver" = patrón corrido que envuelve el cuerpo; el resto va como estampado)
            AddShirt("basica-blanca", "Básica Blanca", 0, "Común", () => Solid(0xF2F2EE)).Fabric = 0xF2F2EE;
            AddShirt("basica-negra", "Básica Negra", 0, "Común", () => Solid(0x1C1C1F)).Fabric = 0x1C1C1F;
            AddShirt("camuflaje-tinta", "Camuflaje Tinta", 200, "Común", () => Camo(0x2B2B30, 0x55555E, 0x8A8A94, 7)).AllOver = true;
            AddShirt("tinta-derramada", "Tinta Derramada", 300, "Rara", () => Dots(0xF4F1EA, 0x121016, 5, 0.22f)).AllOver = true;
            AddShirt("neon-grafiti", "Neón Grafiti", 300, "Rara", () => Diagonal(0x121016, 0x00E676, 0xE6007E)).AllOver = true;
            AddShirt("rayas-lucha", "Rayas de Lucha", 500, "Legendaria", () => Stripes(0xD55E00, 0xF4F1EA, 8)).AllOver = true;
            AddShirt("edicion-limitada", "Edición Limitada 001", 600, "Edición limitada · también con la playera física", () => Frame(0x0D0D0F, 0xF6C36B));
            AddShirt(ProgressRules.MonthShirtId, "Playera del Mes", 0, "Calendario de 28 días", () => Checker(0x121016, 0xF6C36B, 4), DesignUnlock.CalendarOnly).AllOver = true;

            // ── gorras de ejemplo ──
            AddCap(new DesignDef { Id = "clasica", Name = "Clásica", Rarity = "Común · de tu color", Price = 0, PlayerColor = true });
            // (los patrones de ejemplo envuelven la copa; los PNG de artistas van con copa lisa + logo al frente)
            AddCap(new DesignDef { Id = "cuadros", Name = "Cuadros de Carrera", Rarity = "Común", Price = 80, AllOver = true, Make = () => Checker(0x121016, 0xF4F1EA, 8) });
            AddCap(new DesignDef { Id = "camuflaje", Name = "Camuflaje", Rarity = "Común", Price = 120, Visor = 0x3F4238, AllOver = true, Make = () => Camo(0x3F4238, 0x6B7052, 0x9AA07A, 11) });
            AddCap(new DesignDef { Id = "gotas-tinta", Name = "Gotas de Tinta", Rarity = "Rara", Price = 150, Visor = 0x0072B2, AllOver = true, Make = () => Dots(0xBFE7FF, 0x0072B2, 6, 0.18f) });
            AddCap(new DesignDef { Id = "flamas", Name = "Flamas", Rarity = "Rara", Price = 200, AllOver = true, Make = Flames });
            AddCap(new DesignDef { Id = "estrellas", Name = "Estrellas", Rarity = "Épica", Price = 250, Visor = 0xCC79A7, AllOver = true, Make = () => Dots(0x121016, 0xCC79A7, 7, 0.12f) });
            foreach (var w in Weekly)
            {
                var ww = w;
                AddCap(new DesignDef { Id = ww.id, Name = ww.name, Rarity = "Gorra de la semana · día 7", Price = 0, Unlock = DesignUnlock.DailyOnly, Visor = ww.visor, AllOver = true, Make = () => Band(ww.bg, ww.band) });
            }

            // ── diseños de artistas (PNG en Resources) ──
            foreach (var tex in Resources.LoadAll<Texture2D>(ShirtsFolder))
            {
                var t = tex;
                t.wrapMode = TextureWrapMode.Repeat;
                t.anisoLevel = 4; // también salen en cuadros y pósters que la cámara ve casi de canto
                AddOrReplace(_shirts, _byShirt, new DesignDef { Id = t.name, Name = t.name, Rarity = "Diseño de artista", Price = ArtistShirtPrice, FromArtist = true, Make = () => t });
                ArtistShirtCount++;
            }
            foreach (var tex in Resources.LoadAll<Texture2D>(CapsFolder))
            {
                var t = tex;
                t.wrapMode = TextureWrapMode.Repeat;
                AddOrReplace(_caps, _byCap, new DesignDef { Id = t.name, Name = t.name, Rarity = "Diseño de artista", Price = ArtistCapPrice, IsCap = true, FromArtist = true, Make = () => t });
                ArtistCapCount++;
            }

            _logo = Resources.Load<Texture2D>(LogoPath);
        }

        static DesignDef AddShirt(string id, string name, int price, string rarity, Func<Texture2D> make, DesignUnlock unlock = DesignUnlock.Shop)
        {
            var d = new DesignDef { Id = id, Name = name, Price = price, Rarity = rarity, Make = make, Unlock = unlock };
            AddOrReplace(_shirts, _byShirt, d);
            return d;
        }

        static void AddCap(DesignDef d) { d.IsCap = true; AddOrReplace(_caps, _byCap, d); }

        // Si un PNG se llama igual que un diseño de ejemplo, gana el PNG.
        static void AddOrReplace(List<DesignDef> list, Dictionary<string, DesignDef> map, DesignDef d)
        {
            if (map.TryGetValue(d.Id, out var old)) list[list.IndexOf(old)] = d;
            else list.Add(d);
            map[d.Id] = d;
        }

        public static DesignDef Shirt(string id) { EnsureLoaded(); return id != null && _byShirt.TryGetValue(id, out var d) ? d : _byShirt["basica-blanca"]; }
        public static DesignDef Cap(string id) { EnsureLoaded(); return id != null && _byCap.TryGetValue(id, out var d) ? d : _byCap["clasica"]; }

        public static string WeeklyCapId(DateTime date) => Weekly[LocalProgress.WeekOfYear(date) % Weekly.Length].id;
        public static string WeeklyCapName(DateTime date) => Weekly[LocalProgress.WeekOfYear(date) % Weekly.Length].name;

        /// <summary>Playeras de ejemplo para bots y jugadores invitados (sin las exclusivas).</summary>
        public static string ShirtForGuest(int i)
        {
            EnsureLoaded();
            var pool = new List<string>();
            foreach (var s in _shirts) if (s.Unlock == DesignUnlock.Shop && s.Id != "edicion-limitada") pool.Add(s.Id);
            return pool[(i * 5 + 2) % pool.Count];
        }

        // El campo PlayerSlot.Shirt del núcleo (que ya viaja por la red LAN) lleva
        // "playera|gorra". El núcleo no lo interpreta; solo la presentación.
        public static string PackOutfit(string shirt, string cap) => string.IsNullOrEmpty(cap) || cap == "clasica" ? shirt : $"{shirt}|{cap}";

        public static void UnpackOutfit(string packed, out string shirt, out string cap)
        {
            shirt = "basica-blanca"; cap = "clasica";
            if (string.IsNullOrEmpty(packed)) return;
            int bar = packed.IndexOf('|');
            if (bar < 0) { shirt = packed; return; }
            shirt = packed.Substring(0, bar);
            cap = packed.Substring(bar + 1);
        }

        // ── Texturas de ejemplo (128×128, se generan una vez) ──
        const int N = 128;

        static Color32 C(uint rgb) => new Color32((byte)((rgb >> 16) & 255), (byte)((rgb >> 8) & 255), (byte)(rgb & 255), 255);

        static Texture2D Build(Func<int, int, Color32> f)
        {
            var px = new Color32[N * N];
            for (int y = 0; y < N; y++) for (int x = 0; x < N; x++) px[y * N + x] = f(x, y);
            var t = new Texture2D(N, N, TextureFormat.RGBA32, true) { wrapMode = TextureWrapMode.Repeat, filterMode = FilterMode.Bilinear };
            t.SetPixels32(px);
            t.Apply(true, true); // sin copia en CPU: ahorra memoria en el celular
            return t;
        }

        static Texture2D Solid(uint rgb) { var c = C(rgb); return Build((x, y) => c); }

        static Texture2D Stripes(uint a, uint b, int count)
        {
            Color32 ca = C(a), cb = C(b);
            return Build((x, y) => (y * count / N) % 2 == 0 ? ca : cb);
        }

        static Texture2D Checker(uint a, uint b, int count)
        {
            Color32 ca = C(a), cb = C(b);
            return Build((x, y) => ((x * count / N) + (y * count / N)) % 2 == 0 ? ca : cb);
        }

        static Texture2D Diagonal(uint bg, uint a, uint b)
        {
            Color32 cbg = C(bg), ca = C(a), cb = C(b);
            return Build((x, y) => { int k = ((x + y) / 10) % 4; return k == 1 ? ca : k == 3 ? cb : cbg; });
        }

        static Texture2D Dots(uint bg, uint dot, int count, float radius01)
        {
            Color32 cbg = C(bg), cd = C(dot);
            float cell = N / (float)count;
            return Build((x, y) =>
            {
                int cx = (int)(x / cell), cy = (int)(y / cell);
                // centro desplazado por celda (patrón fijo, no usa Random)
                float ox = (Hash(cx, cy) - 0.5f) * 0.4f, oy = (Hash(cy, cx + 7) - 0.5f) * 0.4f;
                float dx = x / cell - cx - 0.5f - ox, dy = y / cell - cy - 0.5f - oy;
                return dx * dx + dy * dy < radius01 * radius01 * (0.6f + Hash(cx + 3, cy) * 0.8f) ? cd : cbg;
            });
        }

        static Texture2D Camo(uint a, uint b, uint c, int seed)
        {
            Color32 ca = C(a), cb = C(b), cc = C(c);
            return Build((x, y) =>
            {
                float v = Noise(x / 22f, y / 22f, seed) * 0.65f + Noise(x / 9f, y / 9f, seed + 1) * 0.35f;
                return v < 0.42f ? ca : v < 0.6f ? cb : cc;
            });
        }

        static Texture2D Band(uint bg, uint band)
        {
            Color32 cbg = C(bg), cb = C(band);
            return Build((x, y) => y > N * 0.42f && y < N * 0.58f ? cb : cbg);
        }

        static Texture2D Flames()
        {
            Color32 ink = C(0x121016), o = C(0xE69F00), r = C(0xD55E00);
            return Build((x, y) =>
            {
                float edge = N * 0.45f + Mathf.Sin(x * 0.3f) * 10f + Mathf.Sin(x * 0.11f) * 8f;
                if (y > edge) return ink;
                return y > edge - 14f ? o : r;
            });
        }

        static Texture2D Frame(uint bg, uint gold)
        {
            Color32 cbg = C(bg), cg = C(gold);
            return Build((x, y) =>
            {
                bool border = (x > 14 && x < 24) || (x > N - 24 && x < N - 14) || (y > 14 && y < 24) || (y > N - 24 && y < N - 14);
                bool inside = x >= 14 && x <= N - 14 && y >= 14 && y <= N - 14;
                return border && inside ? cg : cbg;
            });
        }

        static float Hash(int x, int y)
        {
            unchecked
            {
                uint h = (uint)(x * 374761393 + y * 668265263);
                h = (h ^ (h >> 13)) * 1274126177u;
                return ((h ^ (h >> 16)) & 0xFFFF) / 65535f;
            }
        }

        // ruido de valor suave y repetible (que la textura sea "tileable" no importa en greybox)
        static float Noise(float x, float y, int seed)
        {
            int ix = Mathf.FloorToInt(x), iy = Mathf.FloorToInt(y);
            float fx = x - ix, fy = y - iy;
            fx = fx * fx * (3f - 2f * fx); fy = fy * fy * (3f - 2f * fy);
            float a = Hash(ix + seed * 31, iy), b = Hash(ix + 1 + seed * 31, iy);
            float c = Hash(ix + seed * 31, iy + 1), d = Hash(ix + 1 + seed * 31, iy + 1);
            return Mathf.Lerp(Mathf.Lerp(a, b, fx), Mathf.Lerp(c, d, fx), fy);
        }
    }
}
