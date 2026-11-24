// ─────────────────────────────────────────────────────────────
// ARTE HECHO CON CÓDIGO para la escenografía y la marca (spec v0.3).
// • Lee los píxeles de cualquier textura (aunque no tenga "Read/Write")
//   pasándola por un RenderTexture: sirve para sacar el color de la tela
//   de un diseño y para pintar el logo sobre un fondo liso.
// • Letreros: el logo oficial (Resources/Marca/logo_inkcap) sobre un
//   panel de color, y rótulos con letra de bloques de 5×7 ("GORRAZO",
//   "ESTRENOS"…). AQUÍ NO SE INVENTA NINGÚN LOGO: si falta el archivo,
//   LogoSign devuelve null (quien lo usa pone un panel liso) y los
//   rótulos escriben "INKCAP" con la misma letra de bloques.
// • Utilería: franjas de precaución y el degradado de los vacíos.
// Todas las texturas son OPACAS (no hace falta ningún shader
// transparente: se ve igual en URP y en Built-in) y se crean una sola
// vez; se guardan aquí para toda la sesión.
// ─────────────────────────────────────────────────────────────
using System.Collections.Generic;
using UnityEngine;

namespace BoomerangFighters.UnityView
{
    public static class BrandArt
    {
        /// <summary>Amarillo de precaución (orillas de los vacíos y aviso de la caja DROP).</summary>
        public const uint Yellow = 0xF2C81E;

        static readonly Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();
        static Color32[] _logoPx;          // logo leído una vez (fila 0 = abajo)
        static int _logoW, _logoH;
        static bool _logoTried, _logoOpaque;
        static Texture2D _stripes, _gradient;

        static Color32 C(uint rgb) => new Color32((byte)((rgb >> 16) & 255), (byte)((rgb >> 8) & 255), (byte)(rgb & 255), 255);

        /// <summary>Ancho / alto de una textura (1 si no hay).</summary>
        public static float Aspect(Texture t) => t != null && t.height > 0 ? t.width / (float)t.height : 1f;

        // ── Leer píxeles ─────────────────────────────────────
        /// <summary>
        /// Copia de la textura reducida a w × h (fila 0 = abajo, como GetPixels32). Funciona con texturas
        /// importadas sin "Read/Write". Devuelve null si no se puede (sin GPU, plataforma rara): quien
        /// llama debe tener un plan B.
        /// </summary>
        public static Color32[] ReadPixels(Texture src, int w, int h)
        {
            if (src == null || w <= 0 || h <= 0) return null;
            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null) return null;
            var prev = RenderTexture.active;
            RenderTexture rt = null;
            Texture2D tmp = null;
            try
            {
                rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);
                Graphics.Blit(src, rt);
                RenderTexture.active = rt;
                tmp = new Texture2D(w, h, TextureFormat.RGBA32, false);
                tmp.ReadPixels(new Rect(0f, 0f, w, h), 0, 0, false);
                var px = tmp.GetPixels32();
                return px != null && px.Length == w * h ? px : null;
            }
            catch (System.Exception)
            {
                return null;
            }
            finally
            {
                RenderTexture.active = prev;
                if (rt != null) RenderTexture.ReleaseTemporary(rt);
                if (tmp != null) Object.Destroy(tmp);
            }
        }

        /// <summary>Copia opaca de una textura con transparencia, pintada sobre "bg" (para estampados con fondo transparente).</summary>
        public static Texture2D Flatten(Texture src, int w, int h, Color32 bg)
        {
            var px = ReadPixels(src, w, h);
            if (px == null) return null;
            for (int i = 0; i < px.Length; i++)
            {
                var c = px[i];
                int a = c.a, na = 255 - a;
                px[i] = new Color32((byte)((c.r * a + bg.r * na) / 255), (byte)((c.g * a + bg.g * na) / 255), (byte)((c.b * a + bg.b * na) / 255), 255);
            }
            return Bake(px, w, h, src.name + " (sobre tela)", TextureWrapMode.Clamp);
        }

        static Texture2D Bake(Color32[] px, int w, int h, string name, TextureWrapMode wrap)
        {
            // anisoLevel: los letreros se ven casi de canto desde la cámara (muros, piso) y sin esto salen borrosos
            var t = new Texture2D(w, h, TextureFormat.RGBA32, true) { name = name, wrapMode = wrap, filterMode = FilterMode.Bilinear, anisoLevel = 8 };
            t.SetPixels32(px);
            t.Apply(true, true); // sin copia en CPU: ahorra memoria en el celular
            return t;
        }

        // ── Logo oficial ─────────────────────────────────────
        static bool EnsureLogo()
        {
            if (_logoTried) return _logoPx != null;
            _logoTried = true;
            var logo = DesignLibrary.Logo;
            if (logo == null || logo.width < 2 || logo.height < 2) return false;
            _logoW = 512;
            _logoH = Mathf.Clamp(Mathf.RoundToInt(512f * logo.height / logo.width), 8, 512);
            _logoPx = ReadPixels(logo, _logoW, _logoH);
            if (_logoPx == null) return false;
            // un logo sin transparencia (JPG, PNG de fondo lleno) se copia tal cual en vez de usarse como plantilla
            int solid = 0;
            foreach (var c in _logoPx) if (c.a > 250) solid++;
            _logoOpaque = solid > _logoPx.Length * 0.98f;
            return true;
        }

        static Color SampleLogo(float x, float y)
        {
            int x0 = Mathf.Clamp(Mathf.FloorToInt(x), 0, _logoW - 1), y0 = Mathf.Clamp(Mathf.FloorToInt(y), 0, _logoH - 1);
            int x1 = Mathf.Min(x0 + 1, _logoW - 1), y1 = Mathf.Min(y0 + 1, _logoH - 1);
            float fx = Mathf.Clamp01(x - x0), fy = Mathf.Clamp01(y - y0);
            Color a = _logoPx[y0 * _logoW + x0], b = _logoPx[y0 * _logoW + x1], c = _logoPx[y1 * _logoW + x0], d = _logoPx[y1 * _logoW + x1];
            return Color.Lerp(Color.Lerp(a, b, fx), Color.Lerp(c, d, fx), fy);
        }

        /// <summary>
        /// El logo de INKCAP teñido de "ink" sobre un panel liso "bg" (textura opaca, ≈ 2.9 : 1).
        /// Con "alpha" menor a 1 queda gastado (esténcil en el piso). Devuelve null si no hay archivo de logo.
        /// </summary>
        public static Texture2D LogoSign(uint bg, uint ink, float alpha = 1f)
        {
            if (!EnsureLogo()) return null;
            string key = $"logo|{bg:X6}|{ink:X6}|{Mathf.RoundToInt(alpha * 100f)}";
            if (_cache.TryGetValue(key, out var done) && done != null) return done;
            const int W = 512, mx = 36, my = 22;
            int lw = W - 2 * mx, lh = Mathf.Max(1, Mathf.RoundToInt(lw * _logoH / (float)_logoW));
            var cv = new Lienzo(W, lh + 2 * my, C(bg));
            cv.Logo(mx, my, lw, lh, C(ink), Mathf.Clamp01(alpha));
            return _cache[key] = cv.Texture("Logo INKCAP sobre panel");
        }

        /// <summary>
        /// Rótulo de una línea: TEXTO grande + texto chico opcional ("by") + la marca si "brand"
        /// (el logo oficial; sin archivo de logo, la palabra INKCAP con la misma letra). Nunca devuelve null.
        /// </summary>
        public static Texture2D Marquee(string big, string small, bool brand, uint bg, uint fg, uint accent)
        {
            big = big ?? "";
            string key = $"rotulo|{big}|{small}|{brand}|{bg:X6}|{fg:X6}|{accent:X6}";
            if (_cache.TryGetValue(key, out var done) && done != null) return done;
            const int s = 12;                     // píxeles por cuadrito de letra
            bool logo = brand && EnsureLogo();
            float brandUnits = logo ? GlyphH * _logoW / (float)_logoH : brand ? TextUnits("INKCAP") : 0f;
            bool accents = false;
            foreach (char ch in big) accents |= Accented(ch);
            float my = accents ? 3.5f : 2f, mx = 3f; // margen (en cuadritos); los acentos necesitan aire arriba
            bool hasSmall = !string.IsNullOrEmpty(small);
            float units = TextUnits(big) + (hasSmall ? 3f + TextUnits(small) * 0.5f : 0f) + (brand ? 3f + brandUnits : 0f);
            var cv = new Lienzo(Mathf.CeilToInt((units + mx * 2f) * s), Mathf.RoundToInt((GlyphH + my * 2f) * s), C(bg));
            int x = Mathf.RoundToInt(mx * s), y = Mathf.RoundToInt(my * s);
            cv.Text(big, x, y, s, C(fg));
            x += (TextUnits(big) + 3) * s;
            if (hasSmall)
            {
                int hs = s / 2;
                cv.Text(small, x, y + GlyphH * (s - hs), hs, C(accent)); // alineado abajo con el texto grande
                x += TextUnits(small) * hs + 3 * s;
            }
            if (logo) cv.Logo(x, y, Mathf.RoundToInt(brandUnits * s), GlyphH * s, C(fg), 1f);
            else if (brand) cv.Text("INKCAP", x, y, s, C(fg));
            return _cache[key] = cv.Texture($"Rótulo {big}");
        }

        // ── Utilería de los escenarios ───────────────────────
        /// <summary>Franjas diagonales amarillo/negro que se repiten (orilla de los vacíos).</summary>
        public static Texture2D HazardStripes
        {
            get
            {
                if (_stripes != null) return _stripes;
                const int n = 64;
                var px = new Color32[n * n];
                Color32 a = C(Yellow), b = C(0x15141A);
                for (int y = 0; y < n; y++)
                    for (int x = 0; x < n; x++) px[y * n + x] = ((x + y) / (n / 2)) % 2 == 0 ? a : b;
                return _stripes = Bake(px, n, n, "Franjas de precaución", TextureWrapMode.Repeat);
            }
        }

        /// <summary>Degradado vertical blanco (arriba) → negro (abajo) para las paredes de los vacíos: se ven hondos.</summary>
        public static Texture2D PitGradient
        {
            get
            {
                if (_gradient != null) return _gradient;
                const int w = 4, h = 64;
                var px = new Color32[w * h];
                for (int y = 0; y < h; y++)
                {
                    byte v = (byte)Mathf.RoundToInt(255f * Mathf.Pow(y / (h - 1f), 1.6f));
                    for (int x = 0; x < w; x++) px[y * w + x] = new Color32(v, v, v, 255);
                }
                return _gradient = Bake(px, w, h, "Degradado de vacío", TextureWrapMode.Clamp);
            }
        }

        // ── Letra de bloques 5×7 (solo las letras que se usan en los rótulos) ──
        const int GlyphW = 5, GlyphH = 7;

        static readonly Dictionary<char, string> Glyphs = new Dictionary<char, string>
        {
            ['A'] = ".###." + "#...#" + "#...#" + "#####" + "#...#" + "#...#" + "#...#",
            ['B'] = "####." + "#...#" + "#...#" + "####." + "#...#" + "#...#" + "####.",
            ['C'] = ".####" + "#...." + "#...." + "#...." + "#...." + "#...." + ".####",
            ['D'] = "####." + "#...#" + "#...#" + "#...#" + "#...#" + "#...#" + "####.",
            ['E'] = "#####" + "#...." + "#...." + "####." + "#...." + "#...." + "#####",
            ['G'] = ".####" + "#...." + "#...." + "#.###" + "#...#" + "#...#" + ".###.",
            ['I'] = "#####" + "..#.." + "..#.." + "..#.." + "..#.." + "..#.." + "#####",
            ['K'] = "#...#" + "#..#." + "#.#.." + "##..." + "#.#.." + "#..#." + "#...#",
            ['N'] = "#...#" + "##..#" + "#.#.#" + "#.#.#" + "#..##" + "#...#" + "#...#",
            ['O'] = ".###." + "#...#" + "#...#" + "#...#" + "#...#" + "#...#" + ".###.",
            ['P'] = "####." + "#...#" + "#...#" + "####." + "#...." + "#...." + "#....",
            ['R'] = "####." + "#...#" + "#...#" + "####." + "#.#.." + "#..#." + "#...#",
            ['S'] = ".####" + "#...." + "#...." + ".###." + "....#" + "....#" + "####.",
            ['T'] = "#####" + "..#.." + "..#.." + "..#.." + "..#.." + "..#.." + "..#..",
            ['U'] = "#...#" + "#...#" + "#...#" + "#...#" + "#...#" + "#...#" + ".###.",
            ['X'] = "#...#" + "#...#" + ".#.#." + "..#.." + ".#.#." + "#...#" + "#...#",
            ['Y'] = "#...#" + "#...#" + ".#.#." + "..#.." + "..#.." + "..#.." + "..#..",
            ['Z'] = "#####" + "....#" + "...#." + "..#.." + ".#..." + "#...." + "#####",
        };

        static char Plain(char c)
        {
            c = char.ToUpperInvariant(c);
            switch (c)
            {
                case 'Á': return 'A';
                case 'É': return 'E';
                case 'Í': return 'I';
                case 'Ó': return 'O';
                case 'Ú': return 'U';
                default: return c;
            }
        }

        static bool Accented(char c)
        {
            c = char.ToUpperInvariant(c);
            return c == 'Á' || c == 'É' || c == 'Í' || c == 'Ó' || c == 'Ú';
        }

        /// <summary>Ancho del texto en cuadritos (cada letra mide 5 + 1 de separación; lo desconocido es un espacio de 4).</summary>
        static int TextUnits(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            int u = 0;
            foreach (char raw in s) u += Glyphs.ContainsKey(Plain(raw)) ? GlyphW + 1 : 4;
            return Mathf.Max(0, u - 1);
        }

        /// <summary>Lienzo de píxeles con la fila 0 ARRIBA (se voltea al crear la textura).</summary>
        sealed class Lienzo
        {
            readonly int _w, _h;
            readonly Color32[] _px;

            public Lienzo(int w, int h, Color32 bg)
            {
                _w = Mathf.Max(1, w); _h = Mathf.Max(1, h);
                _px = new Color32[_w * _h];
                for (int i = 0; i < _px.Length; i++) _px[i] = bg;
            }

            void Box(int x, int y, int w, int h, Color32 c)
            {
                int x1 = Mathf.Min(_w, x + w), y1 = Mathf.Min(_h, y + h);
                for (int j = Mathf.Max(0, y); j < y1; j++)
                    for (int i = Mathf.Max(0, x); i < x1; i++) _px[j * _w + i] = c;
            }

            void Mix(int i, int j, Color c, float a)
            {
                if (i < 0 || j < 0 || i >= _w || j >= _h || a <= 0.004f) return;
                Color d = _px[j * _w + i];
                var o = Color.Lerp(d, c, a);
                o.a = 1f;
                _px[j * _w + i] = o;
            }

            /// <summary>El logo escalado al rectángulo: su transparencia hace de plantilla y se pinta del color "ink".</summary>
            public void Logo(int x, int y, int w, int h, Color32 ink, float alpha)
            {
                if (_logoPx == null || w <= 0 || h <= 0) return;
                Color tint = ink;
                for (int j = 0; j < h; j++)
                {
                    float sy = (1f - (j + 0.5f) / h) * _logoH - 0.5f;
                    for (int i = 0; i < w; i++)
                    {
                        var s = SampleLogo((i + 0.5f) / w * _logoW - 0.5f, sy);
                        if (_logoOpaque) Mix(x + i, y + j, s, alpha);
                        else Mix(x + i, y + j, tint, s.a * alpha);
                    }
                }
            }

            public void Text(string s, int x, int y, int scale, Color32 c)
            {
                if (string.IsNullOrEmpty(s)) return;
                int dot = scale + scale / 3; // cada cuadrito se pinta un tercio más grande: trazo grueso que se lee de lejos
                foreach (char raw in s)
                {
                    if (!Glyphs.TryGetValue(Plain(raw), out var g)) { x += 4 * scale; continue; }
                    for (int r = 0; r < GlyphH; r++)
                        for (int k = 0; k < GlyphW; k++)
                            if (g[r * GlyphW + k] == '#') Box(x + k * scale, y + r * scale, dot, dot, c);
                    if (Accented(raw))
                    {
                        // acento agudo: dos cuadritos en diagonal, separados una fila de la letra
                        Box(x + 3 * scale, y - 3 * scale, dot, dot, c);
                        Box(x + 2 * scale, y - 2 * scale, dot, dot, c);
                    }
                    x += (GlyphW + 1) * scale;
                }
            }

            public Texture2D Texture(string name)
            {
                var flipped = new Color32[_px.Length];
                for (int j = 0; j < _h; j++) System.Array.Copy(_px, j * _w, flipped, (_h - 1 - j) * _w, _w);
                return Bake(flipped, _w, _h, name, TextureWrapMode.Clamp);
            }
        }
    }
}
