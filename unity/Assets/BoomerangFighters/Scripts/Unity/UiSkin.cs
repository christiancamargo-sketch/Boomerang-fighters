// ─────────────────────────────────────────────────────────────
// Estilo IMGUI legible en celular (menú, HUD y resultados).
// Todo se escala con la unidad U = lado corto de la pantalla / 360,
// así un botón mide ≈ 44 pt en un teléfono y no se ve gigante en PC.
// Colores de marca: negro tinta, blanco hueso y magenta de acento.
// La UI final con el arte de la marca se hará con UI Toolkit o uGUI.
// ─────────────────────────────────────────────────────────────
using UnityEngine;

namespace BoomerangFighters.UnityView
{
    public static class UiSkin
    {
        public static readonly Color Ink = new Color(0.07f, 0.063f, 0.086f);
        public static readonly Color Bone = new Color(0.957f, 0.945f, 0.918f);
        public static readonly Color Magenta = new Color(0.902f, 0f, 0.494f);
        public static readonly Color Gold = new Color(0.965f, 0.765f, 0.42f);
        public static readonly Color Dim = new Color(0.62f, 0.6f, 0.66f);

        public static GUIStyle Title, Sub, H1, H2, Body, Small, Button, ButtonOn, ButtonGhost, Panel, Card, Banner, BannerSub, Field, BigIp, Center;
        static int _w, _h;
        static Texture2D _white;

        /// <summary>Unidad de escala (1 = pantalla de 360 px de lado corto).</summary>
        public static float U => Mathf.Max(1f, Mathf.Min(Screen.width, Screen.height) / 360f);

        public static Texture2D White
        {
            get
            {
                if (_white == null) { _white = new Texture2D(1, 1); _white.SetPixel(0, 0, Color.white); _white.Apply(); }
                return _white;
            }
        }

        static Texture2D Tex(Color c)
        {
            var t = new Texture2D(1, 1) { hideFlags = HideFlags.HideAndDontSave };
            t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }

        static int F(float px) => Mathf.RoundToInt(px * U);

        /// <summary>Llamar al inicio de cada OnGUI: crea (o reescala) los estilos.</summary>
        public static void Ensure()
        {
            if (Title != null && _w == Screen.width && _h == Screen.height) return;
            _w = Screen.width; _h = Screen.height;
            float u = U;
            var baseLabel = new GUIStyle(GUI.skin.label) { wordWrap = true, richText = true };
            baseLabel.normal.textColor = Bone;

            Title = new GUIStyle(baseLabel) { fontSize = F(44), fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, wordWrap = false };
            Sub = new GUIStyle(baseLabel) { fontSize = F(13), alignment = TextAnchor.MiddleCenter };
            Sub.normal.textColor = Dim;
            H1 = new GUIStyle(baseLabel) { fontSize = F(22), fontStyle = FontStyle.Bold };
            H2 = new GUIStyle(baseLabel) { fontSize = F(16), fontStyle = FontStyle.Bold };
            Body = new GUIStyle(baseLabel) { fontSize = F(14) };
            Small = new GUIStyle(baseLabel) { fontSize = F(11) };
            Small.normal.textColor = Dim;
            Center = new GUIStyle(Body) { alignment = TextAnchor.MiddleCenter };

            Button = new GUIStyle(GUI.skin.button) { fontSize = F(15), fontStyle = FontStyle.Bold, fixedHeight = 40 * u, wordWrap = true, alignment = TextAnchor.MiddleCenter };
            Button.padding = new RectOffset(F(8), F(8), F(4), F(4));
            Button.margin = new RectOffset(F(3), F(3), F(3), F(3));
            Paint(Button, Bone, Ink, new Color(1f, 1f, 1f), new Color(0.8f, 0.78f, 0.74f));
            ButtonOn = new GUIStyle(Button);
            Paint(ButtonOn, Magenta, Color.white, new Color(1f, 0.2f, 0.6f), new Color(0.7f, 0f, 0.38f));
            ButtonGhost = new GUIStyle(Button);
            Paint(ButtonGhost, new Color(1f, 1f, 1f, 0.08f), Bone, new Color(1f, 1f, 1f, 0.16f), new Color(1f, 1f, 1f, 0.24f));

            Panel = new GUIStyle(GUI.skin.box) { padding = new RectOffset(F(12), F(12), F(12), F(12)) };
            Panel.normal.background = Tex(new Color(Ink.r, Ink.g, Ink.b, 0.94f));
            Card = new GUIStyle(GUI.skin.box) { padding = new RectOffset(F(8), F(8), F(6), F(6)), margin = new RectOffset(0, 0, F(3), F(3)) };
            Card.normal.background = Tex(new Color(1f, 1f, 1f, 0.07f));

            Banner = new GUIStyle(baseLabel) { fontSize = F(40), fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, wordWrap = false };
            BannerSub = new GUIStyle(baseLabel) { fontSize = F(15), alignment = TextAnchor.MiddleCenter };
            BigIp = new GUIStyle(baseLabel) { fontSize = F(34), fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, wordWrap = false };
            BigIp.normal.textColor = Gold;

            Field = new GUIStyle(GUI.skin.textField) { fontSize = F(15), fixedHeight = 36 * u, alignment = TextAnchor.MiddleLeft };
            Field.padding = new RectOffset(F(8), F(8), F(4), F(4));
        }

        static void Paint(GUIStyle s, Color bg, Color fg, Color hover, Color active)
        {
            s.normal.background = Tex(bg); s.normal.textColor = fg;
            s.hover.background = Tex(hover); s.hover.textColor = fg;
            s.active.background = Tex(active); s.active.textColor = fg;
            s.focused.background = s.normal.background; s.focused.textColor = fg;
            s.onNormal.background = s.normal.background; s.onNormal.textColor = fg;
        }

        /// <summary>Texto con contorno negro (para letreros sobre el juego).</summary>
        public static void Outlined(Rect r, string text, GUIStyle style, Color color)
        {
            var old = style.normal.textColor;
            float o = Mathf.Max(1f, 2f * U);
            style.normal.textColor = new Color(0f, 0f, 0f, 0.85f);
            GUI.Label(new Rect(r.x - o, r.y, r.width, r.height), text, style);
            GUI.Label(new Rect(r.x + o, r.y, r.width, r.height), text, style);
            GUI.Label(new Rect(r.x, r.y - o, r.width, r.height), text, style);
            GUI.Label(new Rect(r.x, r.y + o, r.width, r.height), text, style);
            style.normal.textColor = color;
            GUI.Label(r, text, style);
            style.normal.textColor = old;
        }

        public static void Fill(Rect r, Color c)
        {
            var old = GUI.color;
            GUI.color = c;
            GUI.DrawTexture(r, White);
            GUI.color = old;
        }

        /// <summary>Barra de progreso simple (XP, temporizadores).</summary>
        public static void Bar(float height, float value01, Color fill)
        {
            var r = GUILayoutUtility.GetRect(10f, height, GUILayout.ExpandWidth(true));
            Fill(r, new Color(1f, 1f, 1f, 0.12f));
            Fill(new Rect(r.x, r.y, r.width * Mathf.Clamp01(value01), r.height), fill);
        }

        /// <summary>
        /// Marca: "GORRAZO" + sello "by INKCAP". Si existe Resources/Marca/logo_inkcap se dibuja
        /// el logo en el sello; si no, va el texto. No se inventa ningún logo.
        /// </summary>
        public static void Brand(bool subtitle = true, float scale = 1f)
        {
            float u = U;
            var old = Title.fontSize;
            Title.fontSize = Mathf.RoundToInt(old * scale);
            GUILayout.Label("GORRAZO", Title);
            Title.fontSize = old;
            var logo = DesignLibrary.Logo;
            if (logo != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("by", Sub, GUILayout.ExpandWidth(false));
                float h = 26f * u * scale;
                float w = h * logo.width / Mathf.Max(1f, logo.height);
                var r = GUILayoutUtility.GetRect(w, h, GUILayout.Width(w), GUILayout.Height(h));
                GUI.DrawTexture(r, logo, ScaleMode.ScaleToFit, true);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else GUILayout.Label("<b>by INKCAP</b>", Sub); // sin archivo de logo: sello en texto
            if (subtitle) GUILayout.Label("Lanza tu gorra, mancha a todos", Sub);
        }

        /// <summary>Versión de una línea para encabezados chicos: logo si existe; si no, "GORRAZO · by INKCAP".</summary>
        public static void BrandLine()
        {
            var logo = DesignLibrary.Logo;
            if (logo == null) { GUILayout.Label("<b>GORRAZO · by INKCAP</b>", Sub); return; }
            float h = 20f * U;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("<b>GORRAZO</b> · by", Sub, GUILayout.ExpandWidth(false));
            var r = GUILayoutUtility.GetRect(h * logo.width / Mathf.Max(1f, logo.height), h, GUILayout.Height(h));
            GUI.DrawTexture(r, logo, ScaleMode.ScaleToFit, true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static string ColorTag(Color c) => ColorUtility.ToHtmlStringRGB(c);
    }
}
