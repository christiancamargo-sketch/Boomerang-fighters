// ─────────────────────────────────────────────────────────────
// DISEÑOS DEL VESTIDOR (spec v0.2 §E y §F).
// • Playeras: la textura va en el cuerpo del muñequito.
// • Gorras: el diseño va en la COPA; la silueta y el tamaño nunca
//   cambian y el color del jugador vive en el aro y el rastro.
// Se cargan solos los PNG que el equipo de arte deje en:
//   Assets/BoomerangFighters/Resources/Disenos/Playeras/*.png
//   Assets/BoomerangFighters/Resources/Disenos/Gorras/*.png
// (el nombre del archivo es el nombre del diseño). Además hay unos
// diseños de ejemplo hechos con código para que el vestidor no salga
// vacío. LOGO: si existe Resources/Marca/logo_inkcap se dibuja; si no,
// se usa texto. Aquí NO se inventa ningún logo.
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
        public uint Visor = 0x121016;    // color de la visera cuando la gorra trae diseño
        internal Func<Texture2D> Make;
        Texture2D _tex;

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
            AddShirt("basica-blanca", "Básica Blanca", 0, "Común", () => Solid(0xF2F2EE));
            AddShirt("basica-negra", "Básica Negra", 0, "Común", () => Solid(0x1C1C1F));
            AddShirt("camuflaje-tinta", "Camuflaje Tinta", 200, "Común", () => Camo(0x2B2B30, 0x55555E, 0x8A8A94, 7));
            AddShirt("tinta-derramada", "Tinta Derramada", 300, "Rara", () => Dots(0xF4F1EA, 0x121016, 5, 0.22f));
            AddShirt("neon-grafiti", "Neón Grafiti", 300, "Rara", () => Diagonal(0x121016, 0x00E676, 0xE6007E));
            AddShirt("rayas-lucha", "Rayas de Lucha", 500, "Legendaria", () => Stripes(0xD55E00, 0xF4F1EA, 8));
            AddShirt("edicion-limitada", "Edición Limitada 001", 600, "Edición limitada · también con la playera física", () => Frame(0x0D0D0F, 0xF6C36B));
            AddShirt(ProgressRules.MonthShirtId, "Playera del Mes", 0, "Calendario de 28 días", () => Checker(0x121016, 0xF6C36B, 4), DesignUnlock.CalendarOnly);

            // ── gorras de ejemplo ──
            AddCap(new DesignDef { Id = "clasica", Name = "Clásica", Rarity = "Común · de tu color", Price = 0, PlayerColor = true });
            AddCap(new DesignDef { Id = "cuadros", Name = "Cuadros de Carrera", Rarity = "Común", Price = 80, Make = () => Checker(0x121016, 0xF4F1EA, 8) });
            AddCap(new DesignDef { Id = "camuflaje", Name = "Camuflaje", Rarity = "Común", Price = 120, Visor = 0x3F4238, Make = () => Camo(0x3F4238, 0x6B7052, 0x9AA07A, 11) });
            AddCap(new DesignDef { Id = "gotas-tinta", Name = "Gotas de Tinta", Rarity = "Rara", Price = 150, Visor = 0x0072B2, Make = () => Dots(0xBFE7FF, 0x0072B2, 6, 0.18f) });
            AddCap(new DesignDef { Id = "flamas", Name = "Flamas", Rarity = "Rara", Price = 200, Make = Flames });
            AddCap(new DesignDef { Id = "estrellas", Name = "Estrellas", Rarity = "Épica", Price = 250, Visor = 0xCC79A7, Make = () => Dots(0x121016, 0xCC79A7, 7, 0.12f) });
            foreach (var w in Weekly)
            {
                var ww = w;
                AddCap(new DesignDef { Id = ww.id, Name = ww.name, Rarity = "Gorra de la semana · día 7", Price = 0, Unlock = DesignUnlock.DailyOnly, Visor = ww.visor, Make = () => Band(ww.bg, ww.band) });
            }

            // ── diseños de artistas (PNG en Resources) ──
            foreach (var tex in Resources.LoadAll<Texture2D>(ShirtsFolder))
            {
                var t = tex;
                t.wrapMode = TextureWrapMode.Repeat;
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

        static void AddShirt(string id, string name, int price, string rarity, Func<Texture2D> make, DesignUnlock unlock = DesignUnlock.Shop)
            => AddOrReplace(_shirts, _byShirt, new DesignDef { Id = id, Name = name, Price = price, Rarity = rarity, Make = make, Unlock = unlock });

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
