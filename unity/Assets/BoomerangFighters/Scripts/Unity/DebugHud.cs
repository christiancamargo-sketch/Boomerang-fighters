// ─────────────────────────────────────────────────────────────
// HUD y MENÚ DE PRUEBA (IMGUI / OnGUI: cero configuración).
// Sirve para probar en el editor y en celulares:
//   Jugar vs bots · Crear sala LAN · Buscar salas · Unirse por IP
// La UI final (con el arte de la marca) se hará con UI Toolkit o uGUI.
// ─────────────────────────────────────────────────────────────
using BoomerangFighters.Core;
using BoomerangFighters.Net;
using UnityEngine;

namespace BoomerangFighters.UnityView
{
    [RequireComponent(typeof(MatchRunner))]
    public sealed class DebugHud : MonoBehaviour
    {
        MatchRunner _r;
        string _ip = "192.168.1.";
        string _banner = "";
        float _bannerT;
        GUIStyle _big, _mid, _btn;

        void Awake()
        {
            _r = GetComponent<MatchRunner>();
            _r.SimEvents += OnSimEvents;
        }

        void OnDestroy() { if (_r != null) _r.SimEvents -= OnSimEvents; }

        void Styles()
        {
            if (_big != null) return;
            float u = Mathf.Min(Screen.width, Screen.height) / 360f;
            _big = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(42 * u), alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            _mid = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(13 * u), wordWrap = true };
            _btn = new GUIStyle(GUI.skin.button) { fontSize = Mathf.RoundToInt(15 * u), fixedHeight = 34 * u };
        }

        void Update() => _bannerT -= Time.deltaTime;

        void OnSimEvents(System.Collections.Generic.IReadOnlyList<SimEvent> events)
        {
            var sim = _r.Sim;
            if (sim == null) return;
            foreach (var e in events)
            {
                if (e.Type == SimEventType.Countdown) Banner(e.Extra > 0 ? e.Extra.ToString() : "¡YA!", 0.7f);
                if (e.Type == SimEventType.RoundEnd) Banner(e.A >= 0 ? $"PUNTO PARA {sim.Players[e.A].Name}" : "EMPATE", 1.2f);
                if (e.Type == SimEventType.MatchEnd) Banner($"¡GANÓ {sim.Players[e.A].Name}!", 5f);
                if (e.Type == SimEventType.PickupTaken && e.A == _r.LocalPlayerIndex) Banner($"{PowerUpInfo.Name((PowerUp)e.Extra)} · {PowerUpInfo.Effect((PowerUp)e.Extra)}", 1.2f);
            }
        }

        void Banner(string text, float seconds) { _banner = text; _bannerT = seconds; }

        void OnGUI()
        {
            Styles();
            float u = Mathf.Min(Screen.width, Screen.height) / 360f;
            if (_r.Mode == RunMode.Menu || (_r.Mode == RunMode.Host && _r.Sim == null) || (_r.Mode == RunMode.Client && _r.Sim == null))
            {
                DrawMenu(u);
                return;
            }
            var sim = _r.Sim;
            if (sim == null) return;

            // marcador
            GUILayout.BeginArea(new Rect(8 * u, 8 * u, Screen.width - 16 * u, 40 * u));
            GUILayout.BeginHorizontal();
            foreach (var p in sim.Players)
            {
                var c = Greybox.Hex(Greybox.PlayerColors[p.Id % Greybox.PlayerColors.Length]);
                GUI.color = p.Alive ? c : c * 0.5f;
                GUILayout.Label($"{p.Name}  {new string('●', p.Score)}{new string('○', Mathf.Max(0, sim.Config.PointsToWin - p.Score))}", _mid, GUILayout.Width(110 * u));
            }
            GUI.color = Color.white;
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Salir", _btn, GUILayout.Width(70 * u))) _r.StopAll();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (_bannerT > 0f) GUI.Label(new Rect(0, Screen.height * 0.3f, Screen.width, 80 * u), _banner, _big);
            if (sim.Phase == MatchPhase.MatchEnd && GUI.Button(new Rect(Screen.width / 2f - 80 * u, Screen.height * 0.62f, 160 * u, 34 * u), "Revancha", _btn))
            {
                if (_r.Mode == RunMode.Offline) _r.StartOffline(_r.LocalInputs.Count);
                else if (_r.Mode == RunMode.Host) _r.StartHostedMatch();
            }

            foreach (var input in _r.LocalInputs) if (input.Preset == InputDevicePreset.Touch) DrawTouch(input, u);
        }

        void DrawMenu(float u)
        {
            GUILayout.BeginArea(new Rect(16 * u, 16 * u, Mathf.Min(340 * u, Screen.width - 32 * u), Screen.height - 32 * u), GUI.skin.box);
            GUILayout.Label("BOOMERANG FIGHTERS · greybox", _mid);
            GUILayout.Label("Nombre:", _mid);
            _r.PlayerName = GUILayout.TextField(_r.PlayerName, 16);

            if (_r.Mode == RunMode.Menu)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Callejón", _btn)) _r.ArenaId = "callejon";
                if (GUILayout.Button("Museo", _btn)) _r.ArenaId = "museo";
                if (GUILayout.Button("Azotea", _btn)) _r.ArenaId = "azotea";
                GUILayout.EndHorizontal();
                GUILayout.Label($"Escenario: {ArenaLibrary.Get(_r.ArenaId).Name} · Bots: {_r.Bots} ({_r.Difficulty})", _mid);
                if (GUILayout.Button("Jugar vs bots", _btn)) _r.StartOffline(1);
                if (GUILayout.Button("2 jugadores aquí + bots", _btn)) _r.StartOffline(2);
                GUILayout.Space(8 * u);
                GUILayout.Label("Multijugador por Wi‑Fi (misma red o hotspot):", _mid);
                if (GUILayout.Button("Crear sala", _btn)) _r.HostLan();
                if (GUILayout.Button("Buscar salas", _btn)) _r.BrowseLan();
                foreach (var room in _r.Rooms)
                    if (GUILayout.Button($"Unirse: {room}", _btn)) _r.JoinLan(room.HostIp, room.Port);
                GUILayout.BeginHorizontal();
                _ip = GUILayout.TextField(_ip, 21);
                if (GUILayout.Button("Unirse por IP", _btn, GUILayout.Width(130 * u)))
                {
                    if (RoomInfo.TryParseJoinUri(_ip, out var qip, out var qport)) _r.JoinLan(qip, qport); // texto de un QR
                    else _r.JoinLan(_ip.Trim(), _r.GamePort);
                }
                GUILayout.EndHorizontal();
            }
            else if (_r.Mode == RunMode.Host)
            {
                GUILayout.Label($"Sala \"{_r.RoomName}\" — comparte tu IP: {MatchRunner.LocalIp()}", _mid);
                foreach (var p in _r.Host.Lobby) GUILayout.Label($"• {p.Name}{(p.IsBot ? " (bot)" : p.PeerId < 0 ? " (tú)" : "")}", _mid);
                if (GUILayout.Button("¡Empezar partida!", _btn)) _r.StartHostedMatch();
                if (GUILayout.Button("Cerrar sala", _btn)) _r.StopAll();
            }
            else if (_r.Mode == RunMode.Client)
            {
                foreach (var n in _r.Client.LobbyNames) GUILayout.Label($"• {n}", _mid);
                if (GUILayout.Button("Salir", _btn)) _r.StopAll();
            }
            GUILayout.Label(_r.Status, _mid);
            GUILayout.EndArea();
        }

        static Rect ToGui(Rect screenRect) => new Rect(screenRect.x, Screen.height - screenRect.y - screenRect.height, screenRect.width, screenRect.height);

        void DrawTouch(LocalInputReader t, float u)
        {
            GUI.color = new Color(1f, 0.42f, 0.17f, 0.85f); GUI.Box(ToGui(t.ThrowRect), "LANZAR", _btn);
            GUI.color = new Color(0.91f, 0.26f, 0.58f, 0.85f); GUI.Box(ToGui(t.MeleeRect), "GOLPE", _btn);
            GUI.color = new Color(0.13f, 0.78f, 0.91f, 0.85f); GUI.Box(ToGui(t.DashRect), "DASH", _btn);
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
