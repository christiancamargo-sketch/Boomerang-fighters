// ─────────────────────────────────────────────────────────────
// MENÚS DE GORRAZO (IMGUI / OnGUI, legible en celular).
//   Pantalla de carga · Inicio · Ajustes de partida (4 modos, arena,
//   bots, dificultad, puntos, interruptores de reglas) · Wi‑Fi local
//   (crear sala con la IP en grande, buscar, unirse por IP o bf://) ·
//   Reto del Día (semilla AAAAMMDD, mejor puntaje y tabla de 7 días) ·
//   Vestidor (playeras, gorras, muñequitos, stickers/emotes, canje
//   simulado) · Premio diario (7 días, racha suave, calendario de 28) ·
//   Resultados con la XP y las Gotas ganadas.
// Todo el progreso es local (ProgressStore → JSON en persistentDataPath).
// La UI final (con el arte de la marca) se hará con UI Toolkit o uGUI;
// este menú sirve para probar en el editor y en celulares desde hoy.
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using BoomerangFighters.Core;
using BoomerangFighters.Net;
using UnityEngine;

namespace BoomerangFighters.UnityView
{
    [RequireComponent(typeof(MatchRunner))]
    public sealed class GameMenu : MonoBehaviour
    {
        enum Page { None, Splash, Main, Play, Lan, Reto, Closet, Daily, Results }

        sealed class ResultRow { public int Id; public string Name; public int Score, Kos; public bool IsBot, Winner; }

        sealed class Results
        {
            public GameMode Mode;
            public int WinnerId, Me;
            public string WinnerName;
            public readonly List<ResultRow> Rows = new List<ResultRow>();
            public MatchReward Reward;
            public bool HasReto, RetoIsBest;
            public int RetoScore, RetoBest, RetoKos, RetoRounds, RetoSeconds;
        }

        MatchRunner _r;
        Page _page = Page.Splash;
        float _splashT = 1.8f;
        Vector2 _scroll;
        float _dragDist;
        string _ip = "192.168.1.";
        bool _playForLan;
        int _closetTab;
        string _code = "", _codeMsg = "", _dailyMsg = "";
        MatchSimulation _resultsFor;
        float _resultsDelay = -1f;
        Results _res;
        List<string> _ips = new List<string>();
        float _ipsT;
        DailyChallenge _reto;
        int _retoDay;

        /// <summary>true mientras el menú tapa la pantalla (el HUD no se dibuja).</summary>
        public bool Visible => _r != null && (_r.Sim == null || _page == Page.Results);

        static readonly string[] DifficultyNames = { "Fácil", "Normal", "Difícil" };

        void Awake()
        {
            _r = GetComponent<MatchRunner>();
            _r.SimEvents += OnSimEvents;
            ApplyPrefs();
        }

        void OnDestroy() { if (_r != null) _r.SimEvents -= OnSimEvents; }

        // ── Preferencias del menú (se guardan con el progreso) ──
        void ApplyPrefs()
        {
            var s = ProgressStore.Data.Setup;
            _r.PlayerName = string.IsNullOrEmpty(s.PlayerName) ? "Jugador" : s.PlayerName;
            _r.RoomName = string.IsNullOrEmpty(s.RoomName) ? "Sala de Tinta" : s.RoomName;
            var mode = (GameMode)Mathf.Clamp(s.Mode, 0, 3);
            _r.GameMode = mode == GameMode.RetoDelDia ? GameMode.Gorrazo : mode;
            _r.ArenaId = string.IsNullOrEmpty(s.ArenaId) ? "callejon" : s.ArenaId;
            _r.Humans = Mathf.Clamp(s.Humans, 1, 2);
            _r.Bots = Mathf.Clamp(s.Bots, 0, 7);
            _r.Difficulty = (BotDifficulty)Mathf.Clamp(s.Difficulty, 0, 2);
            _r.PointsToWin = s.Points;
            _r.Rules = new Rules { SuddenDeath = s.SuddenDeath, Shove = s.Shove, DeflectReturn = s.DeflectReturn, GraffitiWall = s.GraffitiWall, PowerUps = s.PowerUps };
            _r.ClampBots();
        }

        void SavePrefs()
        {
            var s = ProgressStore.Data.Setup;
            s.PlayerName = _r.PlayerName; s.RoomName = _r.RoomName;
            s.Mode = (int)_r.GameMode; s.ArenaId = _r.ArenaId;
            s.Humans = _r.Humans; s.Bots = _r.Bots; s.Difficulty = (int)_r.Difficulty; s.Points = _r.PointsToWin;
            s.SuddenDeath = _r.Rules.SuddenDeath; s.Shove = _r.Rules.Shove; s.DeflectReturn = _r.Rules.DeflectReturn;
            s.GraffitiWall = _r.Rules.GraffitiWall; s.PowerUps = _r.Rules.PowerUps;
            ProgressStore.Save();
        }

        // ── Resultados: se calculan UNA vez por partida, un momento después de MatchEnd ──
        // (en el cliente los eventos llegan antes que la foto del estado, así que se espera
        // a que el marcador del snapshot esté al día; el ganador sale del evento).
        MatchSimulation _pendingFor;
        int _pendingWinner = -1, _pendingMe;
        RunMode _pendingRun;

        void OnSimEvents(IReadOnlyList<SimEvent> events)
        {
            var sim = _r.Sim;
            if (sim == null || sim == _resultsFor || sim == _pendingFor) return;
            foreach (var e in events)
            {
                if (e.Type != SimEventType.MatchEnd) continue;
                _pendingFor = sim;
                _pendingWinner = e.A;
                _pendingMe = _r.LocalPlayerIndex;
                _pendingRun = _r.Mode;
                _resultsDelay = 1.8f; // deja ver el letrero de ganador
                break;
            }
        }

        void FlushPending()
        {
            if (_pendingFor == null) return;
            var sim = _pendingFor;
            _pendingFor = null;
            RecordResults(sim, _pendingWinner, _pendingMe, _pendingRun);
        }

        void RecordResults(MatchSimulation sim, int winner, int me, RunMode run)
        {
            _resultsFor = sim;
            if (winner < 0) winner = sim.MatchWinnerId;
            var res = new Results { Mode = sim.Mode, WinnerId = winner, Me = me };
            res.WinnerName = winner >= 0 && winner < sim.Players.Count ? sim.Players[winner].Name : "";
            foreach (var p in sim.Players) res.Rows.Add(new ResultRow { Id = p.Id, Name = p.Name, Score = p.Score, Kos = p.Kos, IsBot = p.IsBot, Winner = p.Id == winner });
            res.Rows.Sort((a, b) => a.Winner != b.Winner ? (a.Winner ? -1 : 1) : b.Score != a.Score ? b.Score - a.Score : b.Kos != a.Kos ? b.Kos - a.Kos : a.Id - b.Id);
            if (me >= 0 && me < sim.Players.Count && !sim.Players[me].IsBot)
            {
                var p = sim.Players[me];
                var d = ProgressStore.Data;
                var now = DateTime.Now;
                res.Reward = LocalProgress.RecordMatch(d, winner == me, p.Kos, p.RoundWins, now);
                if (sim.Mode == GameMode.RetoDelDia && run == RunMode.Offline)
                {
                    res.HasReto = true;
                    res.RetoScore = sim.DailyScore(me);
                    res.RetoKos = p.Kos; res.RetoRounds = p.RoundWins; res.RetoSeconds = sim.DailySeconds;
                    res.RetoIsBest = LocalProgress.RecordReto(d, res.RetoScore, now, out res.RetoBest, out _);
                }
                ProgressStore.Save();
            }
            _res = res;
        }

        void Update()
        {
            float dt = Time.unscaledDeltaTime;
            if (_page == Page.Splash) { _splashT -= dt; if (_splashT <= 0f) Go(Page.Main); }
            if (_resultsDelay > 0f)
            {
                _resultsDelay -= dt;
                if (_resultsDelay <= 0f && _r.Sim == _pendingFor) { FlushPending(); Go(Page.Results); }
            }
            // si salieron antes de ver los resultados, la XP de la partida terminada igual se guarda
            if (_pendingFor != null && _r.Sim != _pendingFor) { FlushPending(); _resultsDelay = -1f; }
            if (_page == Page.Lan && _r.Mode == RunMode.Host)
            {
                _ipsT -= dt;
                if (_ipsT <= 0f || _ips.Count == 0) { _ips = MatchRunner.LocalIps(); _ipsT = 3f; }
            }
            // empezó otra partida (revancha o el anfitrión la arrancó): fuera resultados
            if (_page == Page.Results && _r.Sim != _resultsFor) Go(_r.Sim == null ? HomePage() : Page.None);
            if (_r.Sim == null && (_page == Page.None || _page == Page.Results)) Go(HomePage());
            if (_r.Sim != null && _page != Page.Results && _page != Page.None) Go(Page.None);
        }

        Page HomePage() => _r.Mode == RunMode.Host || _r.Mode == RunMode.Client || _r.Browsing ? Page.Lan : Page.Main;

        void Go(Page p)
        {
            if (_page == p) return;
            _page = p;
            _scroll = Vector2.zero;
            _codeMsg = ""; _dailyMsg = "";
        }

        // ── Dibujo ───────────────────────────────────────────
        void OnGUI()
        {
            if (!Visible) return;
            UiSkin.Ensure();
            float u = UiSkin.U;
            GUI.depth = 0;
            UiSkin.Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(UiSkin.Ink.r, UiSkin.Ink.g, UiSkin.Ink.b, _page == Page.Results ? 0.9f : 1f));

            float w = Mathf.Min(Screen.width - 24f * u, 460f * u);
            var area = new Rect((Screen.width - w) / 2f, 10f * u, w, Screen.height - 20f * u);
            if (_page == Page.Splash) { DrawSplash(area); return; }
            DragScroll(area);
            GUILayout.BeginArea(area);
            _scroll = GUILayout.BeginScrollView(_scroll, GUIStyle.none, GUI.skin.verticalScrollbar);
            switch (_page)
            {
                case Page.Main: DrawMain(u); break;
                case Page.Play: DrawPlay(u); break;
                case Page.Lan: DrawLan(u); break;
                case Page.Reto: DrawReto(u); break;
                case Page.Closet: DrawCloset(u); break;
                case Page.Daily: DrawDaily(u); break;
                case Page.Results: DrawResults(u); break;
            }
            GUILayout.Space(24f * u);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        // En celular se arrastra con el dedo para desplazar; si el dedo se movió,
        // se cancela el botón que estaba bajo él (para no "tocar" sin querer).
        void DragScroll(Rect area)
        {
            var ev = Event.current;
            if (ev.type == EventType.MouseDown) _dragDist = 0f;
            if (ev.type == EventType.MouseDrag && area.Contains(ev.mousePosition))
            {
                _scroll.y = Mathf.Max(0f, _scroll.y - ev.delta.y);
                _dragDist += Mathf.Abs(ev.delta.y);
                if (_dragDist > 10f * UiSkin.U) GUIUtility.hotControl = 0;
                ev.Use();
            }
        }

        bool Btn(string text, bool on = false, params GUILayoutOption[] opts) => GUILayout.Button(text, on ? UiSkin.ButtonOn : UiSkin.Button, opts);

        void Back(Page to = Page.Main)
        {
            GUILayout.Space(6f * UiSkin.U);
            if (GUILayout.Button("Atrás", UiSkin.ButtonGhost)) { SavePrefs(); Go(to); }
        }

        void DrawSplash(Rect area)
        {
            GUILayout.BeginArea(area);
            GUILayout.FlexibleSpace();
            UiSkin.Brand(true, 1.3f);
            GUILayout.Space(16f * UiSkin.U);
            GUILayout.Label("Cargando…", UiSkin.Sub);
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
            if (Event.current.type == EventType.MouseUp) _splashT = 0f; // tocar para saltar
        }

        void LevelCard(float u)
        {
            var d = ProgressStore.Data;
            GUILayout.BeginVertical(UiSkin.Card);
            int need = ProgressRules.XpForLevel(d.Level);
            GUILayout.Label($"<b>Nivel {d.Level}</b>   {(d.Level >= ProgressRules.MaxLevel ? "nivel máximo" : $"{d.Xp}/{need} XP")}   ·   <color=#{UiSkin.ColorTag(UiSkin.Gold)}><b>{d.Gotas} Gotas</b></color>", UiSkin.Body);
            UiSkin.Bar(8f * u, LocalProgress.LevelProgress01(d), UiSkin.Magenta);
            GUILayout.EndVertical();
        }

        void NameField(float u)
        {
            GUILayout.Label("Tu nombre", UiSkin.Small);
            string n = GUILayout.TextField(_r.PlayerName ?? "", 16, UiSkin.Field);
            if (n != _r.PlayerName) _r.PlayerName = n;
        }

        // ── Inicio ───────────────────────────────────────────
        void DrawMain(float u)
        {
            UiSkin.Brand();
            GUILayout.Space(8f * u);
            LevelCard(u);
            NameField(u);
            GUILayout.Space(6f * u);
            if (Btn("JUGAR AQUÍ  ·  vs bots o 2 jugadores")) { _playForLan = false; Go(Page.Play); }
            if (Btn("WI‑FI LOCAL  ·  crear o unirse a una sala")) Go(Page.Lan);
            var now = DateTime.Now;
            var rows = LocalProgress.RetoTable(ProgressStore.Data, now);
            if (Btn($"RETO DEL DÍA  ·  {(rows[0].Best.HasValue ? $"tu mejor: {rows[0].Best}" : "aún sin jugar")}")) Go(Page.Reto);
            if (Btn("VESTIDOR")) Go(Page.Closet);
            bool ready = !LocalProgress.ClaimedToday(ProgressStore.Data, now);
            if (Btn(ready ? "PREMIO DIARIO  ·  ¡listo para reclamar!" : "PREMIO DIARIO", ready)) Go(Page.Daily);
            GUILayout.Space(8f * u);
            GUILayout.Label("Sin internet ni servidores: tu progreso se guarda en este celular. Para jugar con amigos, conéctense al mismo Wi‑Fi o al hotspot de un celular.", UiSkin.Small);
        }

        // ── Ajustes de partida ───────────────────────────────
        void DrawPlay(float u)
        {
            GUILayout.Label(_playForLan ? "Ajustes de la sala" : "Ajustes de partida", UiSkin.H1);

            GUILayout.Label("Modo", UiSkin.H2);
            GUILayout.BeginHorizontal();
            foreach (var m in new[] { GameMode.Gorrazo, GameMode.LataCaliente, GameMode.LaReta })
                if (Btn(MatchRunner.ModeName(m), _r.GameMode == m))
                {
                    _r.GameMode = m;
                    _r.ClampBots();
                    if (_r.Host != null) _r.Host.MaxPlayers = m == GameMode.LaReta ? 8 : 6;
                }
            GUILayout.EndHorizontal();
            if (!_playForLan && Btn("Reto del Día  →")) { SavePrefs(); Go(Page.Reto); }
            GUILayout.Label(MatchRunner.ModeDescription(_r.GameMode), UiSkin.Small);

            GUILayout.Label("Escenario", UiSkin.H2);
            GUILayout.BeginHorizontal();
            foreach (var a in ArenaLibrary.All)
                if (Btn(a.Name, _r.ArenaId == a.Id)) _r.ArenaId = a.Id;
            GUILayout.EndHorizontal();
            if (Btn("Aleatorio", _r.ArenaId == "aleatorio")) _r.ArenaId = "aleatorio";

            if (!_playForLan)
            {
                GUILayout.Label("Jugadores en este celular", UiSkin.H2);
                GUILayout.BeginHorizontal();
                for (int h = 1; h <= 2; h++) if (Btn(h == 1 ? "1" : "2 (control o teclado IJKL)", _r.Humans == h)) { _r.Humans = h; _r.ClampBots(); }
                GUILayout.EndHorizontal();
            }

            MatchRunner.PlayerRange(_r.GameMode, out int min, out int max);
            int humans = _playForLan ? 1 : _r.Humans;
            GUILayout.Label(_playForLan ? "Bots al abrir la sala" : "Bots", UiSkin.H2);
            GUILayout.BeginHorizontal();
            if (Btn("−", false, GUILayout.Width(56f * u))) _r.Bots = Mathf.Max(_playForLan ? 0 : Mathf.Max(0, min - humans), _r.Bots - 1);
            GUILayout.Label($"<b>{_r.Bots}</b>  (total {humans + _r.Bots}; este modo es de {min} a {max})", UiSkin.Center, GUILayout.Height(40f * u));
            if (Btn("+", false, GUILayout.Width(56f * u))) _r.Bots = Mathf.Min(Mathf.Max(0, max - humans), _r.Bots + 1);
            GUILayout.EndHorizontal();

            GUILayout.Label("Dificultad de los bots", UiSkin.H2);
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 3; i++) if (Btn(DifficultyNames[i], (int)_r.Difficulty == i)) _r.Difficulty = (BotDifficulty)i;
            GUILayout.EndHorizontal();

            if (_r.GameMode != GameMode.LaReta)
            {
                GUILayout.Label("Puntos para ganar", UiSkin.H2);
                GUILayout.BeginHorizontal();
                foreach (int pts in new[] { 3, 5, 7 }) if (Btn($"{pts} pts", _r.PointsToWin == pts)) _r.PointsToWin = pts;
                GUILayout.EndHorizontal();
            }

            GUILayout.Label("Reglas", UiSkin.H2);
            var R = _r.Rules;
            R.SuddenDeath = Toggle(R.SuddenDeath, "Muerte súbita", "A los 20 s los muros de los lados se cierran y empujan; a los 45 s es empate.", u);
            R.Shove = Toggle(R.Shove, "Empujón [A PRUEBA]", "Sin gorra, el golpe no noquea: empuja 2.5 m (y patea gorras caídas).", u);
            R.DeflectReturn = Toggle(R.DeflectReturn, "Te la regreso [A PRUEBA]", "Golpear una gorra rival la regresa a su dueño y puede noquearlo.", u);
            R.GraffitiWall = Toggle(R.GraffitiWall, "Muro de Tag", "La lata de Grafiti pinta un muro que bloquea 3 s. Apagado: rastro que alenta.", u);
            R.PowerUps = Toggle(R.PowerUps, "Latas de técnica", _r.GameMode == GameMode.LataCaliente ? "En Lata Caliente no salen latas de técnica." : "Salen latas en la arena.", u);

            GUILayout.Space(8f * u);
            if (_playForLan)
            {
                if (Btn("LISTO", true)) { SavePrefs(); Go(Page.Lan); }
            }
            else
            {
                if (Btn("¡A JUGAR!", true))
                {
                    SavePrefs();
                    _r.StartOffline(_r.Humans);
                    if (_r.Sim != null) Go(Page.None);
                }
                if (!string.IsNullOrEmpty(_r.Status) && _r.Sim == null) GUILayout.Label(_r.Status, UiSkin.Small);
                Back();
            }
        }

        bool Toggle(bool value, string name, string desc, float u)
        {
            GUILayout.BeginHorizontal(UiSkin.Card);
            if (Btn(value ? "SÍ" : "NO", value, GUILayout.Width(60f * u))) value = !value;
            GUILayout.BeginVertical();
            GUILayout.Label($"<b>{name}</b>", UiSkin.Body);
            GUILayout.Label(desc, UiSkin.Small);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            return value;
        }

        string SetupSummary()
        {
            var R = _r.Rules;
            string arena = _r.ArenaId == "aleatorio" ? "Aleatorio" : ArenaLibrary.Get(_r.ArenaId).Name;
            string pts = _r.GameMode == GameMode.LaReta ? "3 seguidas o 10 rondas" : $"{_r.PointsToWin} pts";
            var off = new List<string>();
            if (!R.SuddenDeath) off.Add("sin muerte súbita");
            if (!R.Shove) off.Add("sin empujón");
            if (!R.DeflectReturn) off.Add("desvío clásico");
            if (!R.GraffitiWall) off.Add("rastro de aerosol");
            if (!R.PowerUps) off.Add("sin latas");
            return $"{MatchRunner.ModeName(_r.GameMode)} · {arena} · {pts}{(off.Count > 0 ? " · " + string.Join(", ", off) : "")}";
        }

        // ── Wi‑Fi local ──────────────────────────────────────
        void DrawLan(float u)
        {
            if (_r.Mode == RunMode.Host && _r.Host != null) { DrawHostLobby(u); return; }
            if (_r.Mode == RunMode.Client && _r.Client != null) { DrawClientLobby(u); return; }

            GUILayout.Label("Wi‑Fi local", UiSkin.H1);
            GUILayout.Label("Todos conectados al mismo Wi‑Fi o al hotspot de un celular. No necesita internet ni servidores.", UiSkin.Small);
            NameField(u);
            GUILayout.Label("Nombre de la sala", UiSkin.Small);
            _r.RoomName = GUILayout.TextField(_r.RoomName ?? "", 24, UiSkin.Field);
            GUILayout.BeginVertical(UiSkin.Card);
            GUILayout.Label(SetupSummary(), UiSkin.Small);
            GUILayout.EndVertical();
            if (Btn("Ajustes de la sala")) { _playForLan = true; Go(Page.Play); }
            if (Btn("CREAR SALA", true)) { SavePrefs(); _r.HostLan(); }

            GUILayout.Space(8f * u);
            if (Btn(_r.Browsing ? "Buscando salas… (toca para reiniciar)" : "BUSCAR SALAS")) _r.BrowseLan();
            if (_r.Browsing && _r.Rooms.Count == 0) GUILayout.Label("Aún no aparece ninguna. Si no sale, únete con la IP que muestra el anfitrión.", UiSkin.Small);
            foreach (var room in _r.Rooms)
                if (Btn($"Unirse: {room.Name}  ({room.Players}/{room.MaxPlayers})  {room.HostIp}")) { SavePrefs(); _r.JoinLan(room.HostIp, room.Port); }

            GUILayout.Label("Unirse por IP (o pega el texto bf://)", UiSkin.Small);
            GUILayout.BeginHorizontal();
            _ip = GUILayout.TextField(_ip, 64, UiSkin.Field);
            if (Btn("Unirse", false, GUILayout.Width(96f * u)))
            {
                SavePrefs();
                if (RoomInfo.TryParseJoinUri(_ip.Trim(), out var qip, out var qport)) _r.JoinLan(qip, qport); // texto de un QR
                else _r.JoinLan(_ip.Trim(), _r.GamePort);
            }
            GUILayout.EndHorizontal();
            if (!string.IsNullOrEmpty(_r.Status)) GUILayout.Label(_r.Status, UiSkin.Small);
            GUILayout.Space(4f * u);
            if (GUILayout.Button("Atrás", UiSkin.ButtonGhost)) { _r.StopAll(); SavePrefs(); Go(Page.Main); }
        }

        void DrawHostLobby(float u)
        {
            var host = _r.Host;
            string ip = _ips.Count > 0 ? _ips[0] : "127.0.0.1";

            GUILayout.Label($"Tu sala: {_r.RoomName}", UiSkin.H1);
            GUILayout.Label("Que tus amigos se conecten a tu mismo Wi‑Fi (o a tu hotspot), abran «Wi‑Fi local» y toquen tu sala, o escriban esta IP:", UiSkin.Small);
            GUILayout.BeginVertical(UiSkin.Card);
            GUILayout.Label(ip, UiSkin.BigIp);
            GUILayout.Label($"puerto {_r.GamePort}", UiSkin.Sub);
            if (_ips.Count > 1) GUILayout.Label($"Otras IP de este equipo: {string.Join(", ", _ips.GetRange(1, _ips.Count - 1))}", UiSkin.Small);
            GUILayout.EndVertical();
            GUILayout.Label("O que peguen este texto en «Unirse por IP»:", UiSkin.Small);
            GUILayout.TextField(new RoomInfo { Name = _r.RoomName, HostIp = ip, Port = _r.GamePort }.ToJoinUri(), UiSkin.Field);

            GUILayout.BeginVertical(UiSkin.Card);
            GUILayout.Label(SetupSummary(), UiSkin.Small);
            GUILayout.EndVertical();

            GUILayout.Label($"Jugadores ({host.Lobby.Count}/{host.MaxPlayers})", UiSkin.H2);
            for (int i = 0; i < host.Lobby.Count; i++)
            {
                var p = host.Lobby[i];
                string tag = p.IsBot ? " (bot)" : p.PeerId < 0 ? " (tú)" : "";
                GUILayout.Label($"<color=#{UiSkin.ColorTag(Greybox.PlayerColor(i))}>●</color> {p.Name}{tag}", UiSkin.Body);
            }
            GUILayout.BeginHorizontal();
            if (Btn("+ Bot")) _r.AddHostBot();
            if (Btn("− Bot")) _r.RemoveHostBot();
            GUILayout.EndHorizontal();
            if (Btn("¡EMPEZAR PARTIDA!", true)) { SavePrefs(); _r.StartHostedMatch(); }
            if (Btn("Ajustes de la sala")) { _playForLan = true; Go(Page.Play); }
            if (!string.IsNullOrEmpty(_r.Status)) GUILayout.Label(_r.Status, UiSkin.Small);
            if (GUILayout.Button("Cerrar sala", UiSkin.ButtonGhost)) { _r.StopAll(); Go(Page.Lan); }
        }

        void DrawClientLobby(float u)
        {
            GUILayout.Label("Sala", UiSkin.H1);
            GUILayout.Label(_r.Status, UiSkin.Body);
            foreach (var n in _r.Client.LobbyNames) GUILayout.Label($"• {n}", UiSkin.Body);
            GUILayout.Label("El anfitrión elige el modo, la arena y las reglas.", UiSkin.Small);
            if (GUILayout.Button("Salir", UiSkin.ButtonGhost)) { _r.StopAll(); Go(Page.Lan); }
        }

        // ── Reto del Día ─────────────────────────────────────
        DailyChallenge TodayReto()
        {
            var now = DateTime.Now;
            int seed = ProgressRules.DaySeed(now);
            if (_reto == null || _retoDay != seed) { _reto = DailyChallenge.For(now); _retoDay = seed; }
            return _reto;
        }

        void DrawReto(float u)
        {
            var rd = TodayReto();
            var rows = LocalProgress.RetoTable(ProgressStore.Data, DateTime.Now);
            GUILayout.Label("Reto del Día", UiSkin.H1);
            GUILayout.BeginVertical(UiSkin.Card);
            GUILayout.Label($"Semilla  <b>{rd.Seed}</b>", UiSkin.Body);
            GUILayout.Label($"Arena  <b>{ArenaLibrary.Get(rd.ArenaId).Name}</b>", UiSkin.Body);
            GUILayout.Label($"Regla del día  <b>{rd.Rule.Name}</b>", UiSkin.Body);
            GUILayout.Label($"Tu mejor de hoy  <b>{(rows[0].Best.HasValue ? rows[0].Best.ToString() : "—")}</b>", UiSkin.Body);
            GUILayout.EndVertical();
            GUILayout.Label("Puntaje = KOs × 100 + rondas ganadas × 300 − segundos. Tú contra 3 bots en Difícil, a 3 puntos. Todos los que juegan hoy tienen la misma arena y la misma regla.", UiSkin.Small);
            RetoTable(rows, u);
            if (Btn("JUGAR EL RETO", true))
            {
                SavePrefs();
                _r.StartReto();
                if (_r.Sim != null) Go(Page.None);
            }
            Back();
        }

        void RetoTable(RetoRow[] rows, float u)
        {
            GUILayout.BeginVertical(UiSkin.Card);
            GUILayout.Label("<b>Últimos 7 días</b> (solo en este celular)", UiSkin.Small);
            foreach (var row in rows)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(row.Today ? $"<b>{row.Label}</b>" : row.Label, UiSkin.Body);
                GUILayout.FlexibleSpace();
                GUILayout.Label(row.Best.HasValue ? $"<b>{row.Best}</b>" : "—", UiSkin.Body, GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        // ── Vestidor ─────────────────────────────────────────
        static readonly string[] ClosetTabs = { "Playeras", "Gorras", "Muñequitos", "Extras", "Canje" };

        void DrawCloset(float u)
        {
            var d = ProgressStore.Data;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Vestidor", UiSkin.H1);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"<color=#{UiSkin.ColorTag(UiSkin.Gold)}><b>{d.Gotas} Gotas</b></color>", UiSkin.H2, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            for (int i = 0; i < ClosetTabs.Length; i++) if (GUILayout.Button(ClosetTabs[i], _closetTab == i ? UiSkin.ButtonOn : UiSkin.ButtonGhost)) { _closetTab = i; _codeMsg = ""; }
            GUILayout.EndHorizontal();
            switch (_closetTab)
            {
                case 0: DesignList(DesignLibrary.Shirts, false, u); break;
                case 1: DesignList(DesignLibrary.Caps, true, u); break;
                case 2: SpeciesList(u); break;
                case 3: ExtrasList(u); break;
                default: RedeemTab(u); break;
            }
            Back();
        }

        void DesignList(IReadOnlyList<DesignDef> list, bool caps, float u)
        {
            var d = ProgressStore.Data;
            int artists = caps ? DesignLibrary.ArtistCapCount : DesignLibrary.ArtistShirtCount;
            GUILayout.Label(caps
                ? "El diseño va en la copa; la silueta y el tamaño nunca cambian. Tu color vive en el aro y el rastro."
                : "La playera es la textura del cuerpo de tu muñequito.", UiSkin.Small);
            GUILayout.Label(artists > 0 ? $"{artists} diseño(s) de artistas cargados." : $"Sin diseños de artistas aún (PNG en Resources/{(caps ? DesignLibrary.CapsFolder : DesignLibrary.ShirtsFolder)}).", UiSkin.Small);
            string equipped = caps ? d.Equipped.Cap : d.Equipped.Shirt;
            foreach (var def in list)
            {
                bool owned = LocalProgress.Owns(d, def.Id, caps);
                GUILayout.BeginHorizontal(UiSkin.Card);
                var r = GUILayoutUtility.GetRect(48f * u, 48f * u, GUILayout.Width(48f * u), GUILayout.Height(48f * u));
                if (def.PlayerColor || def.Texture == null) UiSkin.Fill(r, Greybox.PlayerColor(0));
                else GUI.DrawTexture(r, def.Texture, ScaleMode.ScaleAndCrop);
                GUILayout.BeginVertical();
                GUILayout.Label($"<b>{def.Name}</b>", UiSkin.Body);
                GUILayout.Label(def.Rarity, UiSkin.Small);
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                float bw = 128f * u;
                if (owned)
                {
                    bool on = equipped == def.Id;
                    if (Btn(on ? "Puesta" : "Usar", on, GUILayout.Width(bw)) && !on)
                    {
                        if (caps) d.Equipped.Cap = def.Id; else d.Equipped.Shirt = def.Id;
                        ProgressStore.Save();
                    }
                }
                else if (def.Unlock == DesignUnlock.DailyOnly) GUILayout.Label("Premio día 7", UiSkin.Small, GUILayout.Width(bw));
                else if (def.Unlock == DesignUnlock.CalendarOnly) GUILayout.Label("Calendario de 28 días", UiSkin.Small, GUILayout.Width(bw));
                else
                {
                    GUI.enabled = d.Gotas >= def.Price;
                    if (Btn($"{def.Price} Gotas", false, GUILayout.Width(bw)) && LocalProgress.TryBuy(d, def.Id, def.Price, caps))
                    {
                        if (caps) d.Equipped.Cap = def.Id; else d.Equipped.Shirt = def.Id;
                        ProgressStore.Save();
                    }
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();
            }
        }

        void SpeciesList(float u)
        {
            var d = ProgressStore.Data;
            GUILayout.Label("Los muñequitos se desbloquean subiendo de nivel.", UiSkin.Small);
            foreach (int i in ProgressRules.SpeciesOrder)
            {
                bool unlocked = LocalProgress.SpeciesUnlocked(d, i);
                GUILayout.BeginHorizontal(UiSkin.Card);
                var r = GUILayoutUtility.GetRect(40f * u, 40f * u, GUILayout.Width(40f * u), GUILayout.Height(40f * u));
                UiSkin.Fill(r, Greybox.Hex(Greybox.VinylColors[i % Greybox.VinylColors.Length]));
                GUILayout.BeginVertical();
                GUILayout.Label($"<b>{ProgressRules.SpeciesNames[i]}</b>", UiSkin.Body);
                int need = ProgressRules.SpeciesUnlock[i];
                GUILayout.Label(need <= 1 ? "Gratis" : $"Nivel {need}", UiSkin.Small);
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                if (unlocked)
                {
                    bool on = d.Equipped.Species == i;
                    if (Btn(on ? "Elegido" : "Elegir", on, GUILayout.Width(110f * u)) && !on) { d.Equipped.Species = i; ProgressStore.Save(); }
                }
                else GUILayout.Label($"Llega a nivel {need}", UiSkin.Small, GUILayout.Width(110f * u));
                GUILayout.EndHorizontal();
            }
        }

        void ExtrasList(float u)
        {
            var d = ProgressStore.Data;
            GUILayout.Label("Stickers y emotes salen en el premio diario. El emote se muestra sobre tu cabeza con el botón «Emote» en partida.", UiSkin.Small);
            GUILayout.Label("Stickers", UiSkin.H2);
            foreach (var s in ProgressRules.StickerList) CosmeticRow(s, d.Stickers.Contains(s.Id), d.Equipped.Sticker == s.Id, () => d.Equipped.Sticker = s.Id, u);
            GUILayout.Label("Emotes", UiSkin.H2);
            foreach (var e in ProgressRules.EmoteList) CosmeticRow(e, d.Emotes.Contains(e.Id), d.Equipped.Emote == e.Id, () => d.Equipped.Emote = e.Id, u);
        }

        void CosmeticRow(Cosmetic c, bool owned, bool on, Action equip, float u)
        {
            GUILayout.BeginHorizontal(UiSkin.Card);
            GUILayout.BeginVertical();
            GUILayout.Label($"<b>{c.Text}</b>", UiSkin.Body);
            GUILayout.Label(c.Name, UiSkin.Small);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            if (owned) { if (Btn(on ? "Puesto" : "Usar", on, GUILayout.Width(110f * u)) && !on) { equip(); ProgressStore.Save(); } }
            else GUILayout.Label("Premio diario", UiSkin.Small, GUILayout.Width(110f * u));
            GUILayout.EndHorizontal();
        }

        void RedeemTab(float u)
        {
            var d = ProgressStore.Data;
            GUILayout.Label("Canje de código de playera", UiSkin.H2);
            GUILayout.Label("SIMULACIÓN del portal web: en la versión final, el código de la etiqueta de tu playera INKCAP se canjea en el portal web con tu ID de jugador. Aquí puedes probar con el código de demostración TINTA-2026. No hay compras con dinero real.", UiSkin.Small);
            GUILayout.BeginVertical(UiSkin.Card);
            GUILayout.Label($"Tu ID de jugador: <b>{d.PlayerId}</b>", UiSkin.Body);
            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            _code = GUILayout.TextField(_code, 24, UiSkin.Field);
            if (Btn("Canjear", true, GUILayout.Width(110f * u)))
            {
                LocalProgress.Redeem(d, _code, out _codeMsg);
                ProgressStore.Save();
            }
            GUILayout.EndHorizontal();
            if (!string.IsNullOrEmpty(_codeMsg)) GUILayout.Label(_codeMsg, UiSkin.Body);
        }

        // ── Premio diario ────────────────────────────────────
        void DrawDaily(float u)
        {
            var d = ProgressStore.Data;
            var now = DateTime.Now;
            bool claimedToday = LocalProgress.ClaimedToday(d, now);
            int next = LocalProgress.NextDailyDay(d, now);
            int doneInCycle = claimedToday ? next : next - 1;
            GUILayout.Label("Premio diario", UiSkin.H1);
            GUILayout.Label("Racha suave: si faltas un día, se pausa; al volver sigues en el día que te tocaba (no regresas al día 1).", UiSkin.Small);
            for (int day = 1; day <= 7; day++)
            {
                var rw = ProgressRules.Daily[day - 1];
                string text = rw.WeeklyCap ? $"Gorra de la semana: {DesignLibrary.WeeklyCapName(now)}" : rw.Text;
                bool done = day <= doneInCycle;
                bool today = !claimedToday && day == next;
                GUILayout.BeginHorizontal(UiSkin.Card);
                GUILayout.Label(today ? $"<color=#{UiSkin.ColorTag(UiSkin.Magenta)}><b>Día {day}</b></color>" : $"<b>Día {day}</b>", UiSkin.Body, GUILayout.Width(70f * u));
                GUILayout.Label(text, UiSkin.Body);
                GUILayout.FlexibleSpace();
                GUILayout.Label(done ? "Listo" : today ? "<b>HOY</b>" : "", UiSkin.Small, GUILayout.Width(50f * u));
                GUILayout.EndHorizontal();
            }
            GUI.enabled = !claimedToday;
            if (Btn(claimedToday ? "Ya reclamaste hoy · vuelve mañana" : "RECLAMAR", !claimedToday))
            {
                var res = LocalProgress.ClaimDaily(d, now, DesignLibrary.WeeklyCapId(now), DesignLibrary.WeeklyCapName(now));
                if (res != null) { _dailyMsg = $"¡Día {res.Day}! Ganaste: {string.Join(" · ", res.Got)}"; ProgressStore.Save(); }
            }
            GUI.enabled = true;
            if (!string.IsNullOrEmpty(_dailyMsg)) GUILayout.Label(_dailyMsg, UiSkin.Body);
            GUILayout.Space(6f * u);
            GUILayout.BeginVertical(UiSkin.Card);
            GUILayout.Label($"<b>Calendario de 28 días</b>: {d.CalendarStamps}/{ProgressRules.CalendarDays} sellos → Playera del mes", UiSkin.Body);
            UiSkin.Bar(8f * u, d.CalendarStamps / (float)ProgressRules.CalendarDays, UiSkin.Gold);
            GUILayout.EndVertical();
            Back();
        }

        // ── Resultados ───────────────────────────────────────
        void DrawResults(float u)
        {
            var res = _res;
            if (res == null) return;
            UiSkin.BrandLine();
            GUILayout.Label(MatchRunner.ModeName(res.Mode), UiSkin.Sub);
            bool won = res.WinnerId >= 0 && res.WinnerId == res.Me;
            var tc = res.WinnerId >= 0 ? Greybox.PlayerColor(res.WinnerId) : Color.white;
            var old = UiSkin.Title.normal.textColor;
            UiSkin.Title.normal.textColor = tc;
            GUILayout.Label(won ? "¡GANASTE!" : $"GANÓ {res.WinnerName}", UiSkin.Title);
            UiSkin.Title.normal.textColor = old;

            GUILayout.BeginVertical(UiSkin.Card);
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>Jugador</b>", UiSkin.Small);
            GUILayout.FlexibleSpace();
            GUILayout.Label(res.Mode == GameMode.LaReta ? "<b>Vict.</b>" : "<b>Pts</b>", UiSkin.Small, GUILayout.Width(50f * u));
            GUILayout.Label("<b>KOs</b>", UiSkin.Small, GUILayout.Width(50f * u));
            GUILayout.EndHorizontal();
            for (int i = 0; i < res.Rows.Count; i++)
            {
                var row = res.Rows[i];
                GUILayout.BeginHorizontal();
                string me = row.Id == res.Me && !row.IsBot ? " (tú)" : "";
                GUILayout.Label($"{i + 1}. <color=#{UiSkin.ColorTag(Greybox.PlayerColor(row.Id))}>●</color> {(row.Winner ? $"<b>{row.Name}</b>" : row.Name)}{me}", UiSkin.Body);
                GUILayout.FlexibleSpace();
                GUILayout.Label(row.Score.ToString(), UiSkin.Body, GUILayout.Width(50f * u));
                GUILayout.Label(row.Kos.ToString(), UiSkin.Body, GUILayout.Width(50f * u));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            if (res.HasReto)
            {
                GUILayout.BeginVertical(UiSkin.Card);
                GUILayout.Label($"Puntaje del Reto  <b><color=#{UiSkin.ColorTag(UiSkin.Gold)}>{res.RetoScore}</color></b>", UiSkin.H2);
                GUILayout.Label($"{res.RetoKos} KOs × 100 + {res.RetoRounds} rondas × 300 − {res.RetoSeconds} s", UiSkin.Small);
                GUILayout.Label(res.RetoIsBest ? "<b>¡Tu mejor de hoy!</b>" : $"Tu mejor de hoy: {res.RetoBest}", UiSkin.Body);
                GUILayout.EndVertical();
                RetoTable(LocalProgress.RetoTable(ProgressStore.Data, DateTime.Now), u);
            }

            if (res.Reward != null)
            {
                var rw = res.Reward;
                GUILayout.BeginVertical(UiSkin.Card);
                foreach (var l in rw.Lines)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(l.Label, UiSkin.Body);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label($"<b>+{l.Xp} XP</b>", UiSkin.Body, GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("Gotas", UiSkin.Body);
                GUILayout.FlexibleSpace();
                GUILayout.Label($"<color=#{UiSkin.ColorTag(UiSkin.Gold)}><b>+{rw.Gotas}</b></color>", UiSkin.Body, GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();
                var d = ProgressStore.Data;
                GUILayout.Label($"<b>Nivel {d.Level}</b>  ·  total +{rw.TotalXp} XP", UiSkin.Body);
                UiSkin.Bar(10f * u, LocalProgress.LevelProgress01(d), UiSkin.Magenta);
                if (rw.LevelsGained > 0)
                {
                    string unlock = rw.Unlocked.Count > 0 ? $" Desbloqueaste a {string.Join(", ", rw.Unlocked)}." : "";
                    GUILayout.Label($"<color=#{UiSkin.ColorTag(UiSkin.Gold)}><b>¡Subiste a nivel {d.Level}!</b></color>{unlock}", UiSkin.Body);
                }
                GUILayout.EndVertical();
            }

            GUILayout.Space(6f * u);
            if (_r.Mode == RunMode.Offline || _r.Mode == RunMode.Host)
            {
                if (Btn(_r.Mode == RunMode.Host ? "OTRA PARTIDA (toda la sala)" : "REVANCHA", true)) _r.Rematch();
            }
            else GUILayout.Label("Esperando a que el anfitrión empiece otra partida…", UiSkin.Small);
            if (_r.Mode == RunMode.Offline && Btn("Vestidor")) { _r.StopAll(); Go(Page.Closet); }
            if (GUILayout.Button(_r.Mode == RunMode.Offline ? "Menú" : "Salir de la sala", UiSkin.ButtonGhost)) { _r.StopAll(); Go(Page.Main); }
        }
    }
}
