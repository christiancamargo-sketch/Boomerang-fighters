// ─────────────────────────────────────────────────────────────
// HUD EN JUEGO (IMGUI / OnGUI: cero configuración).
// Marcador, reloj de ronda y muerte súbita, lata caliente, fila de
// La Reta, letreros de KO de la spec A8 (¡GORRAZO!, ¡DE REGRESO!,
// ¡DE BANDA!, ¡TE LA REGRESO!, ¡ZAZ!, ¡ZAP!, ¡SPLASH!, ¡TIRAJE!) y
// ¡EMPATE!, emote local y controles táctiles.
// v0.3: ¡AL VACÍO! y ¡DROP! (los textos salen de KoCauseInfo.Label) y
// el aviso ¡CAJA! cuando sale la sombra de la caja DROP en la tienda.
// Los menús, el vestidor y los resultados viven en GameMenu.
// La UI final (con el arte de la marca) se hará con UI Toolkit o uGUI.
// ─────────────────────────────────────────────────────────────
using BoomerangFighters.Core;
using UnityEngine;

namespace BoomerangFighters.UnityView
{
    [RequireComponent(typeof(MatchRunner))]
    public sealed class DebugHud : MonoBehaviour
    {
        MatchRunner _r;
        GameMenu _menu;
        string _banner = "", _bannerSub = "";
        Color _bannerColor = Color.white;
        float _bannerT, _emoteT;
        string _emoteText = "";
        int _bannerPriority;

        void Awake()
        {
            _r = GetComponent<MatchRunner>();
            _r.SimEvents += OnSimEvents;
        }

        void Start() => _menu = GetComponent<GameMenu>();

        void OnDestroy() { if (_r != null) _r.SimEvents -= OnSimEvents; }

        void Update()
        {
            _bannerT -= Time.unscaledDeltaTime;
            _emoteT -= Time.unscaledDeltaTime;
            if (_bannerT <= 0f) _bannerPriority = 0;
        }

        string NameOf(int id) => _r.Sim != null && id >= 0 && id < _r.Sim.Players.Count ? _r.Sim.Players[id].Name : "?";
        Color ColorOf(int id) => id >= 0 ? Greybox.PlayerColor(id) : Color.white;

        void OnSimEvents(System.Collections.Generic.IReadOnlyList<SimEvent> events)
        {
            var sim = _r.Sim;
            if (sim == null) return;
            foreach (var e in events)
            {
                switch (e.Type)
                {
                    case SimEventType.Countdown:
                        Banner(e.Extra > 0 ? e.Extra.ToString() : "¡YA!", "", Color.white, 0.7f, 1);
                        break;
                    case SimEventType.KO:
                    {
                        var cause = (KoCause)e.Extra;
                        bool credit = e.B >= 0 && e.B != e.A;
                        string sub;
                        if (cause == KoCause.Vacio) sub = credit ? $"{NameOf(e.B)} tiró a {NameOf(e.A)} al vacío" : $"{NameOf(e.A)} se cayó al vacío";
                        else if (cause == KoCause.Drop) sub = $"A {NameOf(e.A)} le cayó la caja";
                        else sub = credit ? $"{NameOf(e.B)} noqueó a {NameOf(e.A)}" : $"{NameOf(e.A)} quedó fuera";
                        Banner(KoCauseInfo.Label(cause), sub, e.B >= 0 ? ColorOf(e.B) : Greybox.Hex(Greybox.Magenta), 1.0f, 3);
                        break;
                    }
                    case SimEventType.DropWarn:
                        Banner("¡CAJA!", "Viene una caja DROP: sal de la sombra", UiSkin.Gold, Mathf.Clamp(e.Value, 0.8f, 1.5f), 2);
                        break;
                    case SimEventType.RoundEnd:
                    {
                        var reason = (RoundEndReason)e.Extra;
                        if (e.A >= 0) Banner($"PUNTO PARA {NameOf(e.A)}", "", ColorOf(e.A), 1.3f, 4);
                        else Banner(KoCauseInfo.TieLabel, reason == RoundEndReason.TimeUp ? "Se acabó el tiempo: nadie suma" : "Doble KO: nadie suma", UiSkin.Gold, 1.6f, 4);
                        break;
                    }
                    case SimEventType.MatchEnd:
                        Banner(e.A >= 0 ? $"¡GANÓ {NameOf(e.A)}!" : "FIN", "", ColorOf(e.A), 2.5f, 5);
                        break;
                    case SimEventType.SuddenDeathWarn:
                        Banner("¡MUERTE SÚBITA!", sim.Mode == GameMode.LataCaliente ? "Los muros se cierran" : "Los muros se cierran · a los 45 s es empate", Greybox.Hex(Greybox.Magenta), 1.4f, 2);
                        break;
                    case SimEventType.HotCanPass:
                        Banner(e.B >= 0 ? $"¡LATA PARA {NameOf(e.A)}!" : $"LA LATA LA TRAE {NameOf(e.A)}", "", Greybox.Hex(0xD55E00), 0.9f, 2);
                        break;
                    case SimEventType.HotCanExplode:
                        Banner("¡SPLASH!", $"Le explotó la lata a {NameOf(e.A)}", Greybox.Hex(0xD55E00), 1.1f, 3);
                        break;
                    case SimEventType.RetaNext:
                        Banner($"¡ENTRA {NameOf(e.A)}!", $"{NameOf(e.B)} se va a la fila", ColorOf(e.A), 1.4f, 2);
                        break;
                    case SimEventType.PickupTaken:
                        if (e.A == _r.LocalPlayerIndex) Banner(PowerUpInfo.Name((PowerUp)e.Extra), PowerUpInfo.Effect((PowerUp)e.Extra), Greybox.Hex(PowerUpInfo.Color((PowerUp)e.Extra)), 1.2f, 1);
                        break;
                    case SimEventType.Marked:
                        if (e.A == _r.LocalPlayerIndex) Banner("¡TE MARCARON!", "Una gorra con Tatuaje te persigue", Greybox.Hex(Greybox.Magenta), 0.9f, 1);
                        break;
                }
            }
        }

        // Un letrero más importante no lo tapa uno menos importante mientras se ve.
        void Banner(string text, string sub, Color color, float seconds, int priority)
        {
            if (_bannerT > 0f && priority < _bannerPriority) return;
            _banner = text; _bannerSub = sub; _bannerColor = color; _bannerT = seconds; _bannerPriority = priority;
        }

        void OnGUI()
        {
            var sim = _r.Sim;
            if (sim == null || (_menu != null && _menu.Visible)) return;
            UiSkin.Ensure();
            float u = UiSkin.U;
            GUI.depth = 10;

            DrawScoreboard(sim, u);
            DrawClock(sim, u);
            if (sim.Mode == GameMode.LaReta) DrawRetaQueue(sim, u);

            if (_bannerT > 0f)
            {
                var r = new Rect(0, Screen.height * 0.26f, Screen.width, 70 * u);
                UiSkin.Outlined(r, _banner, UiSkin.Banner, _bannerColor);
                if (!string.IsNullOrEmpty(_bannerSub)) UiSkin.Outlined(new Rect(0, r.yMax, Screen.width, 26 * u), _bannerSub, UiSkin.BannerSub, Color.white);
            }

            if (_emoteT > 0f && !string.IsNullOrEmpty(_emoteText) && _r.Cam != null)
            {
                var head = _r.World.PlayerHead(_r.LocalPlayerIndex) + Vector3.up * 0.6f;
                var sp = _r.Cam.WorldToScreenPoint(head);
                if (sp.z > 0f) UiSkin.Outlined(new Rect(sp.x - 120 * u, Screen.height - sp.y - 20 * u, 240 * u, 30 * u), _emoteText, UiSkin.BannerSub, UiSkin.Gold);
            }

            foreach (var input in _r.LocalInputs) if (input.Preset == InputDevicePreset.Touch) DrawTouch(input, u);
        }

        void DrawScoreboard(MatchSimulation sim, float u)
        {
            GUILayout.BeginArea(new Rect(8 * u, 8 * u, Screen.width - 16 * u, 44 * u));
            GUILayout.BeginHorizontal();
            foreach (var p in sim.Players)
            {
                var c = ColorOf(p.Id);
                GUI.color = p.Alive ? c : c * 0.55f;
                string pts;
                if (sim.Mode == GameMode.LaReta) pts = $"{p.Score} vict.{(p.Streak > 0 ? $" · racha {p.Streak}" : "")}";
                else pts = $"{new string('●', Mathf.Min(p.Score, 9))}{new string('○', Mathf.Max(0, sim.Config.PointsToWin - p.Score))}";
                string tag = p.Benched ? " (fila)" : p.Id == _r.LocalPlayerIndex && !p.IsBot ? " (tú)" : "";
                GUILayout.Label($"<b>{p.Name}</b>{tag}\n{pts}", UiSkin.Small, GUILayout.Width(Mathf.Min(110 * u, (Screen.width - 170 * u) / Mathf.Max(1, sim.Players.Count))));
            }
            GUI.color = Color.white;
            GUILayout.FlexibleSpace();
            var eq = ProgressStore.Data.Equipped;
            if (!string.IsNullOrEmpty(eq.Emote) && GUILayout.Button("Emote", UiSkin.ButtonGhost, GUILayout.Width(70 * u)))
            {
                foreach (var em in ProgressRules.EmoteList) if (em.Id == eq.Emote) _emoteText = em.Text;
                _emoteT = 1.6f;
            }
            if (GUILayout.Button("Salir", UiSkin.Button, GUILayout.Width(70 * u))) _r.StopAll();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        void DrawClock(MatchSimulation sim, float u)
        {
            if (sim.Phase != MatchPhase.Play && sim.Phase != MatchPhase.RoundEnd) return;
            float t = sim.RoundTime;
            string clock = $"{(int)(t / 60f)}:{(int)(t % 60f):00}";
            var top = new Rect(0, 52 * u, Screen.width, 26 * u);
            var R = sim.T.Round;
            if (sim.SuddenDeath == SuddenDeathStage.Closing)
            {
                string tie = sim.Mode != GameMode.LataCaliente ? $" · empate en {Mathf.Max(0, Mathf.CeilToInt(R.SuddenDeathTieAt - t))} s" : "";
                UiSkin.Outlined(top, $"MUERTE SÚBITA{tie}", UiSkin.BannerSub, Greybox.Hex(Greybox.Magenta));
            }
            else
            {
                string warn = sim.Rules.SuddenDeath && t > R.SuddenDeathAt - 5f && t < R.SuddenDeathAt + R.SuddenDeathWarn ? "  ¡se cierran los muros!" : "";
                UiSkin.Outlined(top, clock + warn, UiSkin.BannerSub, Color.white);
            }
            if (sim.Mode == GameMode.LataCaliente && sim.HotCanHolder >= 0)
            {
                var rr = new Rect(0, top.yMax, Screen.width, 26 * u);
                UiSkin.Outlined(rr, $"LATA CALIENTE: {NameOf(sim.HotCanHolder)} · {sim.HotCanTimer:0.0} s", UiSkin.BannerSub, Greybox.Hex(0xFF8F70));
            }
        }

        void DrawRetaQueue(MatchSimulation sim, float u)
        {
            float w = 150 * u;
            GUILayout.BeginArea(new Rect(Screen.width - w - 8 * u, 104 * u, w, Screen.height * 0.5f), UiSkin.Card);
            GUILayout.Label("<b>EN LA RETA</b>", UiSkin.Small);
            for (int k = 0; k < 2; k++)
            {
                int id = sim.RetaFighters[k];
                if (id < 0) continue;
                var p = sim.Players[id];
                GUILayout.Label($"<color=#{UiSkin.ColorTag(ColorOf(id))}><b>{p.Name}</b></color>  {p.Score} vict.{(p.Streak > 0 ? $" · racha {p.Streak}/3" : "")}", UiSkin.Small);
            }
            if (sim.RetaQueue.Count > 0)
            {
                GUILayout.Label("<b>FILA</b>", UiSkin.Small);
                for (int i = 0; i < sim.RetaQueue.Count; i++)
                {
                    int id = sim.RetaQueue[i];
                    GUILayout.Label($"{i + 1}. <color=#{UiSkin.ColorTag(ColorOf(id))}>{NameOf(id)}</color>", UiSkin.Small);
                }
            }
            GUILayout.Label($"Ronda {sim.Round}/{sim.T.Modes.RetaMaxRounds}", UiSkin.Small);
            GUILayout.EndArea();
        }

        static Rect ToGui(Rect screenRect) => new Rect(screenRect.x, Screen.height - screenRect.y - screenRect.height, screenRect.width, screenRect.height);

        void DrawTouch(LocalInputReader t, float u)
        {
            GUI.color = new Color(1f, 0.42f, 0.17f, 0.85f); GUI.Box(ToGui(t.ThrowRect), "LANZAR", UiSkin.Button);
            GUI.color = new Color(0.91f, 0.26f, 0.58f, 0.85f); GUI.Box(ToGui(t.MeleeRect), "GOLPE", UiSkin.Button);
            GUI.color = new Color(0.13f, 0.78f, 0.91f, 0.85f); GUI.Box(ToGui(t.DashRect), "DASH", UiSkin.Button);
            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            if (t.StickTouchId >= 0)
            {
                float r = 60 * u;
                var o = new Vector2(t.StickOrigin.x, Screen.height - t.StickOrigin.y);
                var k = new Vector2(t.StickPos.x, Screen.height - t.StickPos.y);
                GUI.Box(new Rect(o.x - r, o.y - r, 2 * r, 2 * r), "");
                GUI.Box(new Rect(k.x - r / 2, k.y - r / 2, r, r), "");
            }
            GUI.color = Color.white;
        }
    }
}
