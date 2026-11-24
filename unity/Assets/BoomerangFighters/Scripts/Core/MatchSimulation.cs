// ─────────────────────────────────────────────────────────────
// SIMULACIÓN DE LA PARTIDA (C# puro, tick fijo de 60 Hz).
// Es la "verdad" del juego: en local corre en el celular; en red corre
// en el anfitrión (host) o en un servidor dedicado, sin gráficos.
// Port directo de prototipo/src/game.js (ya balanceado).
// v0.2 (spec docs/diseno/spec-v0.2-local.md): doble KO = empate,
// muerte súbita, empujón, "Te la regreso", Charco, Muro de Tag,
// letreros de KO y los modos Lata Caliente, La Reta y Reto del Día.
// v0.3 (spec docs/diseno/spec-v0.3-escenarios.md): vacíos (caída, crédito
// del KO, gorras que regresan solas) y la caja DROP de la Pop-up Store.
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;

namespace BoomerangFighters.Core
{
    /// <summary>Etapa de la muerte súbita en la ronda actual.</summary>
    public enum SuddenDeathStage : byte { None = 0, Warn = 1, Closing = 2 }

    public sealed partial class MatchSimulation
    {
        public const int TickRate = 60;
        public const float Dt = 1f / TickRate;
        public const int MaxPlayers = 8;
        /// <summary>Margen de redondeo para dar por terminado un empujón (mismo valor que el prototipo web).</summary>
        public const float PushEps = 1e-6f;

        public readonly MatchConfig Config;
        public readonly Tuning T;
        public readonly Rules Rules;
        public readonly Arena Arena;
        public Rng Rng;
        public GameMode Mode => Config.Mode;

        public readonly List<PlayerState> Players = new List<PlayerState>();
        public readonly List<CapState> Caps = new List<CapState>();
        public readonly List<Pickup> Pickups = new List<Pickup>();
        public readonly List<PaintPatch> Paint = new List<PaintPatch>();
        public readonly List<Puddle> Puddles = new List<Puddle>();
        public readonly List<SimEvent> Events = new List<SimEvent>();
        /// <summary>Muros de Tag vivos (viven en la arena porque bloquean como obstáculos).</summary>
        public List<TagWall> TagWalls => Arena.TagWalls;

        public MatchPhase Phase { get; private set; }
        public int Round { get; private set; }
        public int Tick { get; private set; }
        public int FreezeTicks { get; private set; }
        /// <summary>Segundos de cámara lenta que la presentación debe aplicar (solo estética).</summary>
        public float SlowMo { get; set; }
        public int RoundWinnerId { get; private set; } = -1;
        public int MatchWinnerId { get; private set; } = -1;
        public float PhaseTimer { get; private set; }
        public readonly List<float> RoundDurations = new List<float>();
        /// <summary>Por qué terminó la última ronda (ganador, doble KO o tiempo).</summary>
        public RoundEndReason LastRoundEndReason { get; private set; }

        /// <summary>Segundos de ronda en juego (desde el "¡YA!"). Mueve la muerte súbita.</summary>
        public float RoundTime { get; private set; }
        public SuddenDeathStage SuddenDeath { get; private set; }
        /// <summary>Segundos totales de partida (Reto del Día los resta del puntaje).</summary>
        public float MatchTime => _simTime;
        /// <summary>Segundos de juego (fase Play, sin cuentas ni pausas entre rondas). El Reto del Día los resta del puntaje.</summary>
        public float PlayTime { get; private set; }

        // Lata Caliente
        public int HotCanHolder { get; private set; } = -1;
        public float HotCanTimer { get; private set; }
        public int HotCanImmuneId { get; private set; } = -1;
        public float HotCanImmuneT { get; private set; }

        // Caja DROP (Pop-up Store): mientras hay sombra de aviso, dónde va a caer y cuánto falta
        /// <summary>Hay una caja por caer: la vista dibuja la sombra en DropPos.</summary>
        public bool DropPending { get; private set; }
        public Vec2 DropPos { get; private set; }
        /// <summary>Segundos que faltan para que caiga (solo vale con DropPending).</summary>
        public float DropT { get; private set; }

        // La Reta: los 2 que pelean y la fila (en orden)
        public readonly int[] RetaFighters = { -1, -1 };
        public readonly List<int> RetaQueue = new List<int>();

        int _nextCapId = 1, _nextPickupId = 1, _lastCount = -1, _nextPuddleId = 1, _nextWallId = 1;
        float _pickupTimer, _roundStartTime, _simTime;
        float _dropNext; // segundo de la ronda (RoundTime) en que cae la siguiente caja DROP
        int _graceTicks = -1; // A1: ticks desde que quedó 1 en pie (-1 = aún no)
        RoundEndReason _endReason;

        public MatchSimulation(MatchConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            if (config.Slots.Count < 2 || config.Slots.Count > MaxPlayers) throw new ArgumentException("Se necesitan de 2 a 8 jugadores.");
            if (config.Mode == GameMode.LataCaliente && (config.Slots.Count < 3 || config.Slots.Count > 6))
                throw new ArgumentException("Lata Caliente es de 3 a 6 jugadores.");
            T = config.Tuning ?? Tuning.Default();
            if (T.Modes == null) T.Modes = new ModeTuning();
            if (T.Pits == null) T.Pits = new PitTuning();
            Rules = config.Rules ?? new Rules();
            Arena = new Arena(ArenaLibrary.Get(config.ArenaId)) { PitMargin = T.Pits.SafeMargin };
            Rng = new Rng(config.Seed);
            for (int i = 0; i < config.Slots.Count; i++)
            {
                var s = config.Slots[i];
                var p = new PlayerState { Id = i, Name = s.Name, IsBot = s.IsBot, Species = s.Species, Shirt = s.Shirt };
                p.Cap = NewCap(i, false);
                Players.Add(p);
                Caps.Add(p.Cap);
            }
            if (config.Mode == GameMode.LaReta)
            {
                RetaFighters[0] = 0; RetaFighters[1] = 1;
                for (int i = 2; i < Players.Count; i++) RetaQueue.Add(i);
            }
            NextRound();
        }

        public bool IsRetaFighter(int id) => RetaFighters[0] == id || RetaFighters[1] == id;
        bool PickupsOn => Rules.PowerUps && Mode != GameMode.LataCaliente;

        CapState NewCap(int owner, bool ghost) => new CapState
        {
            Id = _nextCapId++, OwnerId = owner, Ghost = ghost, Mode = CapMode.Held,
            Radius = T.Cap.Radius, Y = T.Cap.FlyHeight,
        };

        void Emit(SimEventType type, int a = -1, int b = -1, Vec2 pos = default, float value = 0f, byte extra = 0)
            => Events.Add(SimEvent.Make(type, a, b, pos, value, extra));

        // ── Rondas ───────────────────────────────────────────
        void NextRound()
        {
            Round++;
            Caps.RemoveAll(c => c.Ghost || c.Mode == CapMode.Dead);
            Pickups.Clear();
            Paint.Clear();
            Puddles.Clear();
            Arena.TagWalls.Clear();
            Arena.WallInset = 0f;          // los muros de muerte súbita regresan
            SuddenDeath = SuddenDeathStage.None;
            RoundTime = 0f;
            _graceTicks = -1;
            HotCanHolder = -1; HotCanTimer = 0f; HotCanImmuneId = -1; HotCanImmuneT = 0f;
            DropPending = false; DropPos = Vec2.Zero; DropT = 0f;
            _dropNext = Arena.Def.Drop != null ? Arena.Def.Drop.First : 0f;
            // barajar spawns de forma determinista
            var spawns = (Vec2[])Arena.Def.Spawns.Clone();
            for (int i = spawns.Length - 1; i > 0; i--)
            {
                int j = Rng.NextInt(i + 1);
                var tmp = spawns[i]; spawns[i] = spawns[j]; spawns[j] = tmp;
            }
            int retaSwap = Mode == GameMode.LaReta ? Rng.NextInt(2) : 0;
            int benchIdx = 0;
            for (int i = 0; i < Players.Count; i++)
            {
                var p = Players[i];
                if (p.Cap.Mode == CapMode.Dead) p.Cap = NewCap(p.Id, false);
                if (!Caps.Contains(p.Cap)) Caps.Add(p.Cap);
                if (Mode != GameMode.LaReta) { ResetPlayer(p, spawns[i % spawns.Length]); continue; }
                // La Reta: los 2 que pelean salen de esquinas opuestas; los demás esperan fuera del área
                int slot = RetaFighters[0] == i ? 0 : (RetaFighters[1] == i ? 1 : -1);
                if (slot >= 0) { ResetPlayer(p, Arena.Def.Spawns[slot ^ retaSwap]); continue; }
                ResetPlayer(p, new Vec2(-Arena.HalfW + 1f + benchIdx++ * 1.4f, -Arena.HalfD - 1.5f));
                p.Alive = false;
                p.Benched = true;
            }
            _pickupTimer = T.Pickups.FirstSpawn;
            Phase = MatchPhase.Countdown;
            PhaseTimer = T.Round.Countdown;
            _lastCount = -1;
            RoundWinnerId = -1;
            _roundStartTime = _simTime;
            Emit(SimEventType.RoundStart, value: Round);
        }

        void ResetPlayer(PlayerState p, Vec2 spawn)
        {
            p.Pos = spawn;
            p.Vel = Vec2.Zero;
            var c = (-spawn).Normalized;
            p.Facing = c.SqrLength > 0 ? c : new Vec2(1, 0);
            p.AimDir = p.Facing;
            p.Alive = true;
            p.HasCap = true;
            HoldCap(p.Cap);
            p.DashT = p.DashCd = p.MeleeT = p.MeleeCd = p.ChargeT = p.SlowT = 0f;
            p.Charging = p.Aiming = p.Shield = false;
            p.Power = PowerUp.None;
            p.MeleeHitMask = 0;
            p.SpawnInvuln = T.Player.SpawnInvuln;
            p.ShoveCd = p.PushT = 0f;
            p.PushVel = Vec2.Zero;
            p.Benched = false;
            p.LastHitBy = -1;
            p.LastHitT = 0f;
        }

        // ── Paso principal ───────────────────────────────────
        /// <summary>Avanza un tick. "inputs" debe tener una entrada por jugador (índice = Id).</summary>
        public void Step(IReadOnlyList<InputCommand> inputs)
        {
            Events.Clear();
            Tick++;
            if (FreezeTicks > 0) { FreezeTicks--; return; } // hit stop
            float dt = Dt;
            _simTime += dt;

            if (Arena.UpdateHazards(dt, ref Rng)) Emit(SimEventType.LaserWarn, value: Arena.Hazards[0].Z);

            if (Phase == MatchPhase.Countdown)
            {
                PhaseTimer -= dt;
                int n = (int)MathF.Ceiling(PhaseTimer / (T.Round.Countdown / 3f));
                if (n != _lastCount && n > 0) { _lastCount = n; Emit(SimEventType.Countdown, extra: (byte)n); }
                if (PhaseTimer <= 0f) { Phase = MatchPhase.Play; Emit(SimEventType.Countdown, extra: 0); OnPlayStart(); }
            }
            else if (Phase == MatchPhase.Play) UpdateSuddenDeath(dt);

            bool canAct = Phase == MatchPhase.Play;
            for (int i = 0; i < Players.Count; i++)
            {
                var p = Players[i];
                if (!p.Alive) continue;
                var input = i < inputs.Count ? inputs[i] : default;
                UpdatePlayer(p, input, dt, canAct);
            }
            SeparatePlayers();

            for (int i = 0; i < Caps.Count; i++) UpdateCap(Caps[i], dt);
            Caps.RemoveAll(c => c.Mode == CapMode.Dead && c.Ghost);

            if (Phase == MatchPhase.Play || Phase == MatchPhase.RoundEnd)
            {
                CapHits();
                MeleeHits();
                UpdatePuddles(dt);
                if (Phase == MatchPhase.Play)
                {
                    if (PickupsOn) UpdatePickups(dt);
                    UpdateHotCan(dt);
                    UpdateDropBox();
                }
                for (int i = 0; i < Players.Count; i++)
                {
                    var p = Players[i];
                    if (p.Alive && Arena.LaserHits(p.Pos, T.Player.Radius)) Kill(p, null, KoCause.Zap);
                }
                PitFalls();
                CheckRoundOver();
            }

            for (int i = Paint.Count - 1; i >= 0; i--)
            {
                var pp = Paint[i];
                pp.Life -= dt;
                if (pp.Life <= 0f) Paint.RemoveAt(i); else Paint[i] = pp;
            }
            for (int i = Arena.TagWalls.Count - 1; i >= 0; i--)
            {
                Arena.TagWalls[i].Life -= dt;
                if (Arena.TagWalls[i].Life <= 0f) Arena.TagWalls.RemoveAt(i);
            }

            if (Phase == MatchPhase.RoundEnd)
            {
                PhaseTimer -= dt;
                if (PhaseTimer <= 0f) FinishRound();
            }
            else if (Phase == MatchPhase.Between)
            {
                PhaseTimer -= dt;
                if (PhaseTimer <= 0f) NextRound();
            }
        }

        void UpdatePlayer(PlayerState p, InputCommand inp, float dt, bool canAct)
        {
            var PT = T.Player;
            p.DashCd -= dt; p.MeleeCd -= dt; p.SlowT -= dt; p.SpawnInvuln -= dt; p.ShoveCd -= dt;
            if (p.MeleeT > 0f) p.MeleeT -= dt;
            if (p.LastHitT > 0f) { p.LastHitT -= dt; if (p.LastHitT <= 0f) { p.LastHitT = 0f; p.LastHitBy = -1; } }

            var move = inp.Move;
            float mlen = move.Length;
            if (mlen > 1f) { move /= mlen; mlen = 1f; }
            if (inp.HasAim && inp.Aim.SqrLength > 1e-6f) { p.AimDir = inp.Aim.Normalized; p.Aiming = true; }
            else { p.Aiming = false; if (mlen > 0.2f) p.AimDir = move.Normalized; }
            if (mlen > 0.2f || inp.HasAim) p.Facing = p.AimDir;
            if (!canAct) { move = Vec2.Zero; mlen = 0f; }

            // dash (mientras lo empujan no puede: es un "dash forzado" que no controla; igual que en el prototipo web)
            if (canAct && inp.DashPressed && p.DashCd <= 0f && p.PushT <= 0f)
            {
                p.DashT = PT.DashTime;
                p.DashCd = PT.DashCooldown;
                p.DashDir = mlen > 0.2f ? move.Normalized : p.Facing;
                Emit(SimEventType.Dash, p.Id, pos: p.Pos);
            }

            // lanzar / regresar
            if (canAct && inp.ThrowPressed)
            {
                if (p.HasCap) { p.Charging = true; p.ChargeT = 0f; }
                else if (p.Cap.Mode == CapMode.Ground) Recall(p.Cap);
                else if (p.Cap.Mode == CapMode.Out) StartReturn(p.Cap);
            }
            if (p.Charging)
            {
                p.ChargeT += dt;
                if (!canAct) p.Charging = false;
                else if (inp.ThrowReleased || !inp.ThrowHeld)
                {
                    float charge = p.ChargeT < T.Cap.TapThreshold ? T.Cap.TapCharge : SimMath.Clamp01(p.ChargeT / T.Cap.ChargeTime);
                    Throw(p, charge, inp);
                    p.Charging = false;
                }
            }

            // golpe cuerpo a cuerpo
            if (canAct && inp.MeleePressed && p.HasCap && p.MeleeCd <= 0f && !p.Charging)
            {
                if (inp.HasAim && inp.Aim.SqrLength > 1e-6f) p.Facing = inp.Aim.Normalized;
                else
                {
                    var t = AssistTarget(p, p.Facing, 0.9f, PT.MeleeRange + 1f);
                    if (t != null) p.Facing = (t.Pos - p.Pos).Normalized;
                }
                p.MeleeT = PT.MeleeTime;
                p.MeleeCd = PT.MeleeCooldown;
                p.MeleeHitMask = 0;
                Emit(SimEventType.Melee, p.Id, pos: p.Pos);
            }
            // A3: GOLPE sin gorra = empujón (no noquea)
            else if (canAct && inp.MeleePressed && !p.HasCap && Rules.Shove && p.ShoveCd <= 0f)
            {
                Shove(p, inp);
            }

            // movimiento
            Vec2 v;
            bool pushed = p.PushT > 0f;
            if (pushed)
            {
                // lo empujaron: "dash forzado" que no controla
                p.PushT -= dt;
                if (p.PushT <= PushEps) p.PushT = 0f; // sin el margen, el redondeo de float daba un tick de más (2.71 m en vez de 2.5 m)
                v = p.PushVel;
            }
            else if (p.DashT > 0f)
            {
                p.DashT -= dt;
                v = p.DashDir * PT.DashSpeed;
            }
            else
            {
                float speed = PT.Speed;
                if (p.Charging) speed *= PT.ChargeMoveMult;
                if (p.SlowT > 0f) speed *= PT.SlowMult;
                var target = move * speed;
                float k = 1f - MathF.Exp(-PT.Accel * dt / speed);
                v = new Vec2(SimMath.Lerp(p.Vel.x, target.x, k), SimMath.Lerp(p.Vel.y, target.y, k));
            }
            p.Vel = v;
            p.Pos += v * dt;
            if (pushed && p.PushT <= 0f) p.Vel = Vec2.Zero; // el empujón recorre 2.5 m exactos, sin patinar
            // con la ronda ya decidida (o en la cuenta regresiva) nadie cae: los vacíos se vuelven sólidos
            Arena.ResolveCircle(ref p.Pos, PT.Radius, Phase != MatchPhase.Play);

            // pintura enemiga alenta
            for (int i = 0; i < Paint.Count; i++)
            {
                var pp = Paint[i];
                if (pp.OwnerId != p.Id && Vec2.Distance(pp.Pos, p.Pos) < pp.Radius) { p.SlowT = 0.25f; break; }
            }

            // recoger la gorra del piso caminando
            if (!p.HasCap && p.Cap.Mode == CapMode.Ground && Vec2.Distance(p.Cap.Pos, p.Pos) < 1.0f) Catch(p);
        }

        void Throw(PlayerState p, float charge, InputCommand inp)
        {
            var CT = T.Cap;
            var dir = p.AimDir;
            if (!inp.HasAim || inp.WantsAssist)
            {
                var t = AssistTarget(p, dir, CT.AimAssistAngle, CT.AimAssistRange);
                if (t != null)
                {
                    float d = Vec2.Distance(t.Pos, p.Pos);
                    float spd = SimMath.Lerp(CT.SpeedMin, CT.SpeedMax, charge);
                    float lead = d / spd * 0.5f;
                    dir = (t.Pos + t.Vel * lead - p.Pos).Normalized;
                }
            }
            float sp = SimMath.Lerp(CT.SpeedMin, CT.SpeedMax, charge);
            if (p.Power == PowerUp.Sticker) sp *= CT.StickerSpeedMult; // A5: Sticker XL 10 % más lenta
            float outT = SimMath.Lerp(CT.OutTimeMin, CT.OutTimeMax, charge);
            float off = T.Player.Radius + CT.Radius * 0.6f;
            Launch(p.Cap, p.Pos + dir * off, dir * sp, outT, p.Power);
            KeepThrowerSide(p.Cap, p.Pos);
            if (!Caps.Contains(p.Cap)) Caps.Add(p.Cap);
            p.HasCap = false;
            p.Facing = dir;
            Emit(SimEventType.Throw, p.Id, pos: p.Pos, value: charge);
            if (p.Power == PowerUp.Serigrafia)
            {
                // "Tiraje": copias en fila sobre la misma línea (no en abanico)
                for (int k = 1; k <= 2; k++)
                {
                    var g = NewCap(p.Id, true);
                    Launch(g, p.Pos + dir * (off - CT.TirajeGap * k), dir * sp, outT, PowerUp.None);
                    KeepThrowerSide(g, p.Pos);
                    Caps.Add(g);
                }
            }
        }

        /// <summary>Si la gorra aparece dentro de un muro de tag rival, se queda del lado de quien la lanza.</summary>
        void KeepThrowerSide(CapState c, Vec2 thrower)
        {
            if (Arena.TagWalls.Count == 0) return;
            if (Arena.CapCollision(c.Pos, c.Radius, out var hit, c.OwnerId, thrower)) c.Pos = hit.Point;
        }

        PlayerState AssistTarget(PlayerState p, Vec2 dir, float maxAngle, float maxDist)
        {
            PlayerState best = null;
            float bestScore = float.MaxValue;
            float da = dir.Angle;
            foreach (var o in Players)
            {
                if (o == p || !o.Alive) continue;
                var delta = o.Pos - p.Pos;
                float d = delta.Length;
                if (d > maxDist) continue;
                float ang = MathF.Abs(SimMath.AngleDelta(da, delta.Angle));
                if (ang > maxAngle) continue;
                if (!Arena.LineOfSight(p.Pos, o.Pos)) continue;
                float score = ang * 6f + d * 0.15f;
                if (score < bestScore) { bestScore = score; best = o; }
            }
            return best;
        }

        /// <summary>A3: empuja a los rivales del arco 2.5 m en 0.2 s y patea gorras ajenas caídas 4 m.</summary>
        void Shove(PlayerState p, InputCommand inp)
        {
            var PT = T.Player;
            if (inp.HasAim && inp.Aim.SqrLength > 1e-6f) p.Facing = inp.Aim.Normalized;
            else
            {
                var t = AssistTarget(p, p.Facing, 0.9f, PT.MeleeRange + 1f);
                if (t != null) p.Facing = (t.Pos - p.Pos).Normalized;
            }
            p.ShoveCd = PT.ShoveCooldown;
            float fa = p.Facing.Angle;
            float range = PT.MeleeRange + PT.Radius * 0.5f;
            bool any = false;
            foreach (var v in Players)
            {
                if (v == p || !v.Alive) continue;
                var d = v.Pos - p.Pos;
                if (d.Length > range) continue;
                if (MathF.Abs(SimMath.AngleDelta(fa, d.Angle)) > PT.MeleeArc / 2f) continue;
                var dir = d.SqrLength > 1e-6f ? d.Normalized : p.Facing;
                v.PushT = PT.ShoveTime;
                v.PushVel = dir * (PT.ShoveDistance / PT.ShoveTime);
                v.DashT = 0f;
                Credit(v, p); // V2: si el empujón lo manda a un vacío, el KO es de quien empujó
                Emit(SimEventType.Shove, p.Id, v.Id, v.Pos, extra: 1);
                if (Mode == GameMode.LataCaliente && p.Id == HotCanHolder) PassHotCan(p, v); // el golpe sin gorra también pasa la lata
                any = true;
            }
            foreach (var c in Caps)
            {
                if (c.OwnerId == p.Id || c.Mode != CapMode.Ground) continue;
                var d = c.Pos - p.Pos;
                if (d.Length > range) continue;
                if (MathF.Abs(SimMath.AngleDelta(fa, d.Angle)) > PT.MeleeArc / 2f + 0.3f) continue;
                var dir = d.SqrLength > 1e-6f ? d.Normalized : p.Facing;
                // con fricción exponencial la distancia recorrida es v0 / fricción
                c.Vel = dir * (PT.ShoveKickDistance * T.Cap.GroundFriction);
                Emit(SimEventType.Shove, p.Id, c.OwnerId, c.Pos, extra: 2);
                any = true;
            }
            if (!any) Emit(SimEventType.Shove, p.Id, -1, p.Pos, extra: 0);
        }

        public PlayerState NearestEnemy(int ownerId, Vec2 pos, float maxDist)
        {
            PlayerState best = null;
            float bd = maxDist;
            foreach (var o in Players)
            {
                if (o.Id == ownerId || !o.Alive) continue;
                float d = Vec2.Distance(o.Pos, pos);
                if (d < bd) { bd = d; best = o; }
            }
            return best;
        }

        void SeparatePlayers()
        {
            float r2 = T.Player.Radius * 2f;
            for (int i = 0; i < Players.Count; i++)
            {
                var a = Players[i]; if (!a.Alive) continue;
                for (int j = i + 1; j < Players.Count; j++)
                {
                    var b = Players[j]; if (!b.Alive) continue;
                    var d = b.Pos - a.Pos;
                    float len = d.Length;
                    if (len < r2 && len > 1e-4f)
                    {
                        var push = d / len * ((r2 - len) / 2f);
                        a.Pos -= push; b.Pos += push;
                    }
                }
            }
        }

        // ── Gorras ───────────────────────────────────────────
        void Launch(CapState c, Vec2 pos, Vec2 vel, float outTime, PowerUp power)
        {
            c.Mode = CapMode.Out;
            c.Pos = pos; c.Vel = vel;
            c.V0 = vel.Length;
            c.OutTime = outTime;
            c.T = 0f;
            c.Power = power;
            c.Radius = T.Cap.Radius * (power == PowerUp.Sticker ? T.Cap.StickerScale : 1f);
            c.Y = T.Cap.FlyHeight;
            c.Fade = 0f;
            c.TrailT = 0f;
            c.Bounced = false;
            ClearDeflect(c);
        }

        static void ClearDeflect(CapState c)
        {
            c.DeflectT = 0f;
            c.DeflectBy = -1;
            c.MarkedId = -1;
        }

        void StartReturn(CapState c)
        {
            if (c.Mode != CapMode.Out) return;
            c.Mode = CapMode.Return;
            c.RetSpeed = MathF.Max(6f, c.Vel.Length);
        }

        void Recall(CapState c)
        {
            c.Mode = CapMode.Recall;
            c.T = 0f;
            var owner = Players[c.OwnerId];
            c.Vel = (owner.Pos - c.Pos).Normalized * 4f;
            Emit(SimEventType.Recall, c.OwnerId, pos: c.Pos);
        }

        void Drop(CapState c, Vec2 bounce)
        {
            ClearDeflect(c);
            if (c.Ghost) { c.Mode = CapMode.Dead; return; }
            c.Mode = CapMode.Ground;
            c.Vel = bounce;
            c.T = 0f;
        }

        void HoldCap(CapState c)
        {
            c.Mode = CapMode.Held;
            c.Power = PowerUp.None;
            c.Radius = T.Cap.Radius;
            ClearDeflect(c);
        }

        void Catch(PlayerState p)
        {
            p.HasCap = true;
            HoldCap(p.Cap);
            bool dashCatch = p.DashT > 0f;
            if (dashCatch) p.DashCd = 0f; // atraparla en pleno dash recarga el dash
            Emit(SimEventType.Catch, p.Id, pos: p.Pos, extra: (byte)(dashCatch ? 1 : 0));
        }

        void UpdateCap(CapState c, float dt)
        {
            if (c.Mode == CapMode.Held || c.Mode == CapMode.Dead) return;
            var CT = T.Cap;
            var owner = Players[c.OwnerId];
            c.T += dt;

            switch (c.Mode)
            {
                case CapMode.Out:
                {
                    float k = SimMath.Clamp01(1f - c.T / c.OutTime);
                    float sp = c.V0 * (0.3f + 0.7f * k);
                    var d = c.Vel.Normalized;
                    if (c.Power == PowerUp.Tatuaje)
                    {
                        var tgt = NearestEnemy(c.OwnerId, c.Pos, CT.HomingRange);
                        if (tgt != null)
                        {
                            // B: avisar a la vista a quién persigue (ícono sobre la víctima)
                            if (tgt.Id != c.MarkedId) { c.MarkedId = tgt.Id; Emit(SimEventType.Marked, tgt.Id, c.OwnerId, tgt.Pos); }
                            d = SimMath.RotateTowards(d, tgt.Pos - c.Pos, CT.HomingTurn * dt);
                        }
                        else c.MarkedId = -1;
                    }
                    c.Vel = d * sp;
                    MoveCap(c, dt);
                    if (c.Mode == CapMode.Out && c.T >= c.OutTime)
                    {
                        if (c.Ghost) { c.Mode = CapMode.Dead; return; }
                        if (c.Power == PowerUp.Grafiti && Rules.GraffitiWall) PaintTagWall(c);
                        c.MarkedId = -1;
                        StartReturn(c);
                    }
                    break;
                }
                case CapMode.Return:
                {
                    if (!owner.Alive) { Drop(c, c.Vel * 0.3f); return; }
                    if (c.DeflectT > 0f)
                    {
                        // A4: sale recta hacia su dueño a velocidad fija; mientras dure puede noquearlo
                        c.DeflectT -= dt;
                        c.Vel = c.Vel.Normalized * c.RetSpeed;
                        MoveCap(c, dt);
                        if (c.DeflectT <= 0f) { c.DeflectT = 0f; c.DeflectBy = -1; }
                        break;
                    }
                    c.RetSpeed = MathF.Min(CT.ReturnSpeed, c.RetSpeed + CT.ReturnAccel * dt);
                    var d = SimMath.RotateTowards(c.Vel, owner.Pos - c.Pos, CT.TurnRate * dt);
                    c.Vel = d * c.RetSpeed;
                    MoveCap(c, dt);
                    break;
                }
                case CapMode.Recall:
                {
                    if (!owner.Alive) { Drop(c, Vec2.Zero); return; }
                    // al llamarla, se eleva y pasa por encima de los obstáculos
                    float sp = MathF.Min(CT.RecallSpeed, 4f + c.T * 40f);
                    c.Vel = (owner.Pos - c.Pos).Normalized * sp;
                    c.Pos += c.Vel * dt;
                    c.Y = SimMath.Lerp(c.Y, 1.6f, MathF.Min(1f, dt * 8f));
                    break;
                }
                case CapMode.Ground:
                {
                    float f = MathF.Exp(-CT.GroundFriction * dt);
                    c.Vel *= f;
                    c.Pos += c.Vel * dt;
                    Arena.ResolveCircle(ref c.Pos, c.Radius);
                    c.Y = SimMath.Lerp(c.Y, 0.14f, MathF.Min(1f, dt * 10f));
                    if (Arena.InPit(c.Pos))
                    {
                        // V3: la gorra en el piso que queda sobre un vacío regresa sola de inmediato
                        // (si su dueño ya cayó, se pierde en el hueco)
                        if (owner.Alive) Recall(c); else c.Mode = CapMode.Dead;
                        break;
                    }
                    if (owner.Alive && c.T > CT.GroundAutoReturn) { Recall(c); break; } // nadie se queda desarmado para siempre
                    if (!owner.Alive)
                    {
                        c.Fade += dt;
                        if (c.Fade > CT.GroundFadeTime) c.Mode = CapMode.Dead;
                    }
                    break;
                }
            }

            // rastro de aerosol (Grafiti, respaldo si el Muro de Tag está apagado)
            if (c.Power == PowerUp.Grafiti && c.Lethal && !Rules.GraffitiWall)
            {
                c.TrailT -= dt;
                if (c.TrailT <= 0f)
                {
                    c.TrailT = T.Pickups.PaintInterval;
                    Paint.Add(new PaintPatch { Pos = c.Pos, Radius = T.Pickups.PaintSlowRadius, OwnerId = c.OwnerId, Life = T.Pickups.PaintLife });
                    Emit(SimEventType.Paint, c.OwnerId, pos: c.Pos);
                }
            }
        }

        void MoveCap(CapState c, float dt)
        {
            var from = c.Pos;
            c.Pos += c.Vel * dt;
            if (!Arena.CapCollision(c.Pos, c.Radius, out var hit, c.OwnerId, from)) return;
            c.Pos = hit.Point;
            if (c.Power == PowerUp.Tinta) { StartPuddle(c); return; }
            if (c.Ghost) { c.Mode = CapMode.Dead; return; }
            Emit(SimEventType.Clank, c.OwnerId, pos: c.Pos, extra: 1);
            if (c.Mode == CapMode.Out)
            {
                if (c.Power == PowerUp.Grafiti && Rules.GraffitiWall) PaintTagWall(c);
                // rebote en pared: tiros de banda
                float dot = Vec2.Dot(c.Vel, hit.Normal);
                c.Vel -= hit.Normal * (2f * dot);
                c.Mode = CapMode.Return;
                c.RetSpeed = c.Vel.Length * 0.8f;
                c.Bounced = true;
                c.MarkedId = -1;
            }
            else
            {
                Drop(c, hit.Normal * 3f);
            }
        }

        void CapHits()
        {
            var PT = T.Player;
            for (int ci = 0; ci < Caps.Count; ci++)
            {
                var c = Caps[ci];
                if (c.Mode == CapMode.Dead) continue;
                var owner = Players[c.OwnerId];
                if (!c.Ghost && owner.Alive && !c.Deflected)
                {
                    bool canCatch = c.Mode == CapMode.Return || c.Mode == CapMode.Recall || (c.Mode == CapMode.Out && c.T > T.Cap.EarlyCatchDelay);
                    if (canCatch && Vec2.Distance(c.Pos, owner.Pos) < T.Cap.CatchRadius + (c.Radius - T.Cap.Radius))
                    {
                        Catch(owner);
                        continue;
                    }
                }
                if (!c.Lethal) continue;
                for (int vi = 0; vi < Players.Count; vi++)
                {
                    var v = Players[vi];
                    // la gorra desviada sí puede noquear a su dueño, pero no a quien la desvió
                    if ((c.Deflected ? v.Id == c.DeflectBy : v.Id == c.OwnerId) || !v.Alive || v.SpawnInvuln > 0f) continue;
                    if (Vec2.Distance(c.Pos, v.Pos) >= c.Radius + PT.Radius * 0.85f) continue;
                    if (Mode == GameMode.LataCaliente)
                    {
                        // con la lata en juego los gorrazos no noquean: la pasan
                        if (owner.Id == HotCanHolder) PassHotCan(owner, v);
                        continue;
                    }
                    if (c.Power == PowerUp.Tinta) { StartPuddle(c); break; }
                    if (v.Shield)
                    {
                        v.Shield = false;
                        Credit(v, c.Deflected && c.DeflectBy >= 0 ? Players[c.DeflectBy] : owner);
                        Emit(SimEventType.ShieldBreak, v.Id, c.OwnerId, v.Pos);
                        Drop(c, (c.Pos - v.Pos).Normalized * 6f);
                        break;
                    }
                    var killer = c.Deflected && c.DeflectBy >= 0 ? Players[c.DeflectBy] : owner;
                    Kill(v, killer, CapCause(c));
                }
            }

            // gorra contra gorra: ¡CLANK!, ambas caen (A6: radio de choque al 85 %)
            for (int i = 0; i < Caps.Count; i++)
            {
                var a = Caps[i];
                if (!a.Lethal) continue;
                for (int j = i + 1; j < Caps.Count; j++)
                {
                    var b = Caps[j];
                    if (!b.Lethal || a.OwnerId == b.OwnerId) continue;
                    if (Vec2.Distance(a.Pos, b.Pos) >= (a.Radius + b.Radius) * T.Cap.ClankRadiusMult) continue;
                    var n = (a.Pos - b.Pos).Normalized;
                    Drop(a, n * 7f); Drop(b, -n * 7f);
                    Emit(SimEventType.Clank, a.OwnerId, b.OwnerId, (a.Pos + b.Pos) * 0.5f, extra: 2);
                    if (!a.Lethal) break;
                }
            }
        }

        /// <summary>A8: qué letrero lleva un KO de gorra.</summary>
        static KoCause CapCause(CapState c)
        {
            if (c.Ghost) return KoCause.Tiraje;
            if (c.Deflected) return KoCause.TeLaRegreso;
            if (c.Bounced) return KoCause.DeBanda;
            if (c.Mode == CapMode.Return || c.Mode == CapMode.Recall) return KoCause.DeRegreso;
            return KoCause.Gorrazo;
        }

        void MeleeHits()
        {
            var PT = T.Player;
            foreach (var p in Players)
            {
                if (!p.Alive || p.MeleeT <= PT.MeleeTime - PT.MeleeActive) continue;
                float fa = p.Facing.Angle;
                foreach (var v in Players)
                {
                    if (v == p || !v.Alive || (p.MeleeHitMask & (1u << v.Id)) != 0 || v.SpawnInvuln > 0f) continue;
                    var d = v.Pos - p.Pos;
                    if (d.Length > PT.MeleeRange + PT.Radius * 0.5f) continue;
                    if (MathF.Abs(SimMath.AngleDelta(fa, d.Angle)) > PT.MeleeArc / 2f) continue;
                    p.MeleeHitMask |= 1u << v.Id;
                    Credit(v, p); // V2: le pegó con un golpe (aunque no lo noquee: escudo o Lata Caliente)
                    if (Mode == GameMode.LataCaliente)
                    {
                        if (p.Id == HotCanHolder) PassHotCan(p, v);
                        continue;
                    }
                    if (v.Shield) { v.Shield = false; Emit(SimEventType.ShieldBreak, v.Id, p.Id, v.Pos); continue; }
                    Kill(v, p, KoCause.Zaz);
                }
                // el golpe desvía gorras enemigas
                foreach (var c in Caps)
                {
                    if (!c.Lethal) continue;
                    if (c.Deflected ? c.DeflectBy == p.Id : c.OwnerId == p.Id) continue;
                    var d = c.Pos - p.Pos;
                    if (d.Length > PT.MeleeRange + 0.5f) continue;
                    if (MathF.Abs(SimMath.AngleDelta(fa, d.Angle)) > PT.MeleeArc / 2f + 0.3f) continue;
                    Credit(Players[c.OwnerId], p); // V2: le desvió la gorra
                    if (Rules.DeflectReturn && !c.Ghost && Mode != GameMode.LataCaliente && Players[c.OwnerId].Alive)
                    {
                        DeflectReturn(c, p);
                        continue;
                    }
                    Drop(c, p.Facing * 9f);
                    Emit(SimEventType.Deflect, p.Id, c.OwnerId, c.Pos);
                }
            }
        }

        /// <summary>A4 "Te la regreso": la gorra sale hacia su dueño a 1.2× (máx. 22 m/s) y 0.6 s puede noquearlo.</summary>
        void DeflectReturn(CapState c, PlayerState by)
        {
            var CT = T.Cap;
            var owner = Players[c.OwnerId];
            float sp = MathF.Max(6f, MathF.Min(CT.DeflectMaxSpeed, c.Vel.Length * CT.DeflectSpeedMult));
            var dir = (owner.Pos - c.Pos).Normalized;
            if (dir.SqrLength < 1e-6f) dir = (-c.Vel).Normalized;
            c.Mode = CapMode.Return;
            c.Vel = dir * sp;
            c.RetSpeed = sp;
            c.Y = CT.FlyHeight;
            c.Power = PowerUp.None; // pierde la lata (el tamaño del Sticker se queda)
            c.MarkedId = -1;
            c.DeflectT = CT.DeflectLethalTime;
            c.DeflectBy = by.Id;
            Emit(SimEventType.Deflect, by.Id, c.OwnerId, c.Pos, extra: 1);
        }

        // ── Latas con efecto en la arena ─────────────────────
        /// <summary>Tinta derramada · Charco: deja un charco que crece y noquea al terminar de crecer.</summary>
        void StartPuddle(CapState c)
        {
            var pd = new Puddle { Id = _nextPuddleId++, Pos = c.Pos, OwnerId = c.OwnerId, T = 0f, Radius = 0f };
            Puddles.Add(pd);
            Emit(SimEventType.PuddleStart, c.OwnerId, pos: c.Pos, value: T.Cap.ExplosionRadius);
            c.Power = PowerUp.None;
            Drop(c, Vec2.Zero);
        }

        void UpdatePuddles(float dt)
        {
            var CT = T.Cap;
            for (int i = Puddles.Count - 1; i >= 0; i--)
            {
                var pd = Puddles[i];
                pd.T += dt;
                pd.Radius = CT.ExplosionRadius * SimMath.Clamp01(pd.T / CT.PuddleGrowTime);
                if (pd.T < CT.PuddleGrowTime) continue;
                // terminó de crecer: ¡SPLASH! a quien siga dentro (menos al dueño; el Parche bloquea)
                Emit(SimEventType.Explode, pd.OwnerId, pos: pd.Pos, value: CT.ExplosionRadius);
                var owner = Players[pd.OwnerId];
                foreach (var v in Players)
                {
                    if (v.Id == pd.OwnerId || !v.Alive || v.SpawnInvuln > 0f) continue;
                    if (Vec2.Distance(v.Pos, pd.Pos) >= CT.ExplosionRadius) continue;
                    if (v.Shield) { v.Shield = false; Credit(v, owner); Emit(SimEventType.ShieldBreak, v.Id, pd.OwnerId, v.Pos); continue; }
                    Kill(v, owner, KoCause.Splash);
                }
                Puddles.RemoveAt(i);
            }
        }

        /// <summary>Grafiti · Muro de Tag: muro de esténcil perpendicular al vuelo, centrado en la gorra.</summary>
        void PaintTagWall(CapState c)
        {
            var CT = T.Cap;
            var fd = c.Vel.Normalized;
            if (fd.SqrLength < 1e-6f) fd = new Vec2(1f, 0f);
            var w = new TagWall
            {
                Id = _nextWallId++, Pos = c.Pos, Dir = fd.Perp,
                HalfLen = CT.TagWallLength * 0.5f, HalfThick = CT.TagWallThickness * 0.5f,
                Life = CT.TagWallLife, OwnerId = c.OwnerId,
            };
            Arena.TagWalls.Add(w);
            c.Power = PowerUp.None;
            Emit(SimEventType.TagWall, c.OwnerId, pos: w.Pos, value: w.Dir.Angle);
        }

        // ── Vacíos (spec v0.3, sección A) ────────────────────
        /// <summary>V2: "by" tocó a "v" (empujón, golpe, desvío o gorra contra su escudo). Dura T.Pits.CreditTime.</summary>
        void Credit(PlayerState v, PlayerState by)
        {
            if (v == null || by == null || v == by || !v.Alive) return;
            v.LastHitBy = by.Id;
            v.LastHitT = T.Pits.CreditTime;
        }

        /// <summary>
        /// V1: quien queda con el centro sobre un vacío (reducido 0.15 m por lado) sin dash ni empujón, cae.
        /// Un dash o un empujón pueden cruzarlo; si terminan encima, cae. V6: aplica igual si lo empujó
        /// un muro de muerte súbita. V2: el KO es de quien lo tocó en los últimos 2 s (si nadie, de nadie).
        /// </summary>
        void PitFalls()
        {
            if (Phase != MatchPhase.Play || !Arena.HasPits) return;
            float inset = T.Pits.FallInset;
            for (int i = 0; i < Players.Count; i++)
            {
                var p = Players[i];
                if (!p.Alive || p.DashT > 0f || p.PushT > 0f) continue;
                if (!Arena.InPit(p.Pos, inset)) continue;
                var by = p.LastHitT > 0f && p.LastHitBy >= 0 && p.LastHitBy < Players.Count ? Players[p.LastHitBy] : null;
                Kill(p, by, KoCause.Vacio);
            }
        }

        // ── Caja DROP (Pop-up Store) ─────────────────────────
        /// <summary>
        /// Cada "Every" segundos de ronda (la primera a los "First") cae una caja en un punto libre al azar:
        /// sombra de aviso "Warn" segundos antes (DropWarn), luego cae (DropLand), noquea a quien esté a
        /// "Radius" o menos (el Parche bloquea) y deja una lata de técnica.
        /// </summary>
        void UpdateDropBox()
        {
            var D = Arena.Def.Drop;
            if (D == null) return;
            if (!DropPending)
            {
                if (RoundTime < _dropNext - D.Warn) return;
                DropPending = true;
                DropPos = Arena.RandomFreePoint(ref Rng, 1.0f); // respeta obstáculos y vacíos (V4)
                DropT = MathF.Max(0f, _dropNext - RoundTime);
                Emit(SimEventType.DropWarn, pos: DropPos, value: DropT);
                return;
            }
            DropT = MathF.Max(0f, _dropNext - RoundTime);
            if (RoundTime < _dropNext) return;

            // ¡cae!
            var pos = DropPos;
            DropPending = false;
            DropT = 0f;
            _dropNext += D.Every;
            var leaves = PickupsOn ? (PowerUp)(1 + Rng.NextInt(PowerUpInfo.Count)) : PowerUp.None;
            Emit(SimEventType.DropLand, pos: pos, value: D.Radius, extra: (byte)leaves);
            foreach (var v in Players)
            {
                if (!v.Alive || v.SpawnInvuln > 0f) continue;
                if (Vec2.Distance(v.Pos, pos) > D.Radius) continue;
                if (v.Shield) { v.Shield = false; Emit(SimEventType.ShieldBreak, v.Id, -1, v.Pos); continue; }
                Kill(v, null, KoCause.Drop);
            }
            if (leaves != PowerUp.None)
            {
                // la caja se rompe y deja una lata de técnica en ese lugar (aunque ya haya el máximo de latas)
                var pu = new Pickup { Id = _nextPickupId++, Type = leaves, Pos = pos };
                Pickups.Add(pu);
                Emit(SimEventType.PickupSpawn, pos: pos, extra: (byte)leaves, value: pu.Id);
            }
        }

        // ── Muerte súbita (A2) ───────────────────────────────
        void UpdateSuddenDeath(float dt)
        {
            RoundTime += dt;
            PlayTime += dt;
            if (!Rules.SuddenDeath) return;
            var R = T.Round;
            if (SuddenDeath == SuddenDeathStage.None && RoundTime >= R.SuddenDeathAt)
            {
                SuddenDeath = SuddenDeathStage.Warn;
                Emit(SimEventType.SuddenDeathWarn, value: R.SuddenDeathWarn);
            }
            if (SuddenDeath == SuddenDeathStage.Warn && RoundTime >= R.SuddenDeathAt + R.SuddenDeathWarn)
            {
                SuddenDeath = SuddenDeathStage.Closing;
                Emit(SimEventType.SuddenDeathStart);
            }
            if (SuddenDeath == SuddenDeathStage.Closing)
            {
                // los muros de los lados cortos avanzan; empujan (ResolveCircle), no matan
                float maxInset = MathF.Max(0f, Arena.HalfW - R.SuddenDeathMinWidth * 0.5f);
                Arena.WallInset = MathF.Min(maxInset, Arena.WallInset + R.SuddenDeathWallSpeed * dt);
                float lim = Arena.PlayHalfW - 0.6f;
                foreach (var pu in Pickups) pu.Pos.x = SimMath.Clamp(pu.Pos.x, -lim, lim);
            }
            if (Mode != GameMode.LataCaliente && RoundTime >= R.SuddenDeathTieAt && Phase == MatchPhase.Play)
                EndRound(-1, RoundEndReason.TimeUp);
        }

        // ── Lata Caliente ────────────────────────────────────
        void OnPlayStart()
        {
            RoundTime = 0f;
            if (Mode == GameMode.LataCaliente) AttachHotCan();
        }

        void AttachHotCan()
        {
            int alive = AliveCount;
            if (alive < 2) { HotCanHolder = -1; return; }
            int pick = Rng.NextInt(alive);
            PlayerState h = null;
            foreach (var p in Players) if (p.Alive && pick-- == 0) { h = p; break; }
            HotCanHolder = h.Id;
            HotCanTimer = Rng.Range(T.Modes.HotCanMin, T.Modes.HotCanMax);
            HotCanImmuneId = -1; HotCanImmuneT = 0f;
            Emit(SimEventType.HotCanPass, h.Id, -1, h.Pos, value: HotCanTimer);
        }

        void PassHotCan(PlayerState from, PlayerState to)
        {
            if (from.Id != HotCanHolder || to.Id == from.Id || !to.Alive) return;
            if (to.Id == HotCanImmuneId && HotCanImmuneT > 0f) return; // no regresa al instante
            HotCanHolder = to.Id;
            HotCanImmuneId = from.Id;
            HotCanImmuneT = T.Modes.HotCanImmunity;
            Emit(SimEventType.HotCanPass, to.Id, from.Id, to.Pos, value: HotCanTimer);
        }

        void UpdateHotCan(float dt)
        {
            if (Mode != GameMode.LataCaliente) return;
            if (HotCanImmuneT > 0f) HotCanImmuneT -= dt;
            if (HotCanHolder < 0) { if (AliveCount >= 2 && _graceTicks < 0) AttachHotCan(); return; }
            var h = Players[HotCanHolder];
            if (!h.Alive) { AttachHotCan(); return; } // cayó por otra cosa (láser): se pega otra
            HotCanTimer -= dt;
            if (HotCanTimer > 0f) return;
            Emit(SimEventType.HotCanExplode, h.Id, pos: h.Pos);
            HotCanHolder = -1;
            Kill(h, null, KoCause.Splash);
            AttachHotCan();
        }

        // ── KO y fin de ronda ────────────────────────────────
        void Kill(PlayerState v, PlayerState killer, KoCause cause)
        {
            // ya declarada la ronda, nadie más cae (el ganador conserva su punto)
            if (!v.Alive || Phase != MatchPhase.Play) return;
            v.Alive = false;
            v.Charging = false;
            if (killer != null && killer != v) killer.Kos++;
            if (!v.HasCap && v.Cap.Mode == CapMode.Ground) v.Cap.Fade = 0f;
            v.Power = PowerUp.None;
            v.Shield = false;
            v.PushT = 0f;
            FreezeTicks = T.Round.HitStopTicks;
            Emit(SimEventType.KO, v.Id, killer?.Id ?? -1, v.Pos, extra: (byte)cause);
        }

        /// <summary>
        /// A1: al quedar 1 en pie no se declara ganador de inmediato; si el último cae
        /// antes de 6 ticks (0.1 s de juego, sin contar el hit stop), la ronda es empate.
        /// </summary>
        void CheckRoundOver()
        {
            if (Phase != MatchPhase.Play) return;
            PlayerState last = null;
            int alive = 0;
            foreach (var p in Players) if (p.Alive) { alive++; last = p; }
            if (alive >= 2) { _graceTicks = -1; return; }
            if (alive == 0) { EndRound(-1, RoundEndReason.DoubleKo); return; }
            if (_graceTicks < 0) _graceTicks = 0; else _graceTicks++;
            if (_graceTicks >= T.Round.DoubleKoTicks - 1) EndRound(last.Id, RoundEndReason.Winner);
        }

        void EndRound(int winnerId, RoundEndReason reason)
        {
            Phase = MatchPhase.RoundEnd;
            PhaseTimer = T.Round.EndDelay;
            if (reason != RoundEndReason.TimeUp) SlowMo = T.Round.SlowMoTime;
            RoundWinnerId = winnerId;
            _endReason = reason;
            _graceTicks = -1;
        }

        void FinishRound()
        {
            RoundDurations.Add(_simTime - _roundStartTime);
            LastRoundEndReason = _endReason;
            PlayerState w = RoundWinnerId >= 0 ? Players[RoundWinnerId] : null;
            if (w != null) { w.Score++; w.RoundWins++; }
            Emit(SimEventType.RoundEnd, w?.Id ?? -1, value: Round, extra: (byte)_endReason);
            if (Mode == GameMode.LaReta)
            {
                if (RetaAfterRound(w)) return;
            }
            else if (w != null && w.Score >= Config.PointsToWin) { EndMatch(w.Id); return; }
            Phase = MatchPhase.Between;
            PhaseTimer = T.Round.BetweenRounds;
        }

        void EndMatch(int winnerId)
        {
            Phase = MatchPhase.MatchEnd;
            MatchWinnerId = winnerId;
            Emit(SimEventType.MatchEnd, winnerId);
        }

        /// <summary>La Reta: el ganador se queda, el perdedor va al final de la fila y entra el siguiente. true = terminó la partida.</summary>
        bool RetaAfterRound(PlayerState w)
        {
            var MT = T.Modes;
            if (w != null && IsRetaFighter(w.Id))
            {
                int loserSlot = RetaFighters[0] == w.Id ? 1 : 0;
                var loser = Players[RetaFighters[loserSlot]];
                w.Streak++;
                loser.Streak = 0;
                if (w.Streak >= MT.RetaStreakToWin) { EndMatch(w.Id); return true; }
                RetaQueue.Add(loser.Id);
                int next = RetaQueue[0];
                RetaQueue.RemoveAt(0);
                RetaFighters[loserSlot] = next;
                if (next != loser.Id) Emit(SimEventType.RetaNext, next, loser.Id);
            }
            // empate: ambos se quedan
            if (Round >= MT.RetaMaxRounds)
            {
                // más victorias; desempate: más KOs, luego el asiento más bajo
                PlayerState best = null;
                foreach (var p in Players)
                    if (best == null || p.Score > best.Score || (p.Score == best.Score && p.Kos > best.Kos)) best = p;
                EndMatch(best.Id);
                return true;
            }
            return false;
        }

        /// <summary>Segundos que resta el Reto del Día (redondeo igual que Math.round de la web).</summary>
        public int DailySeconds => (int)MathF.Floor(PlayTime + 0.5f);

        /// <summary>Reto del Día: KOs × 100 + rondas ganadas × 300 − segundos de juego.</summary>
        public int DailyScore(int playerId = 0)
        {
            var p = Players[playerId];
            return p.Kos * T.Modes.RetoKoPoints + p.RoundWins * T.Modes.RetoRoundPoints - DailySeconds;
        }

        // ── Power-ups ────────────────────────────────────────
        void UpdatePickups(float dt)
        {
            _pickupTimer -= dt;
            if (_pickupTimer <= 0f && Pickups.Count < T.Pickups.Max)
            {
                _pickupTimer = Rng.Range(T.Pickups.EveryMin, T.Pickups.EveryMax);
                SpawnPickup();
            }
            for (int i = Pickups.Count - 1; i >= 0; i--)
            {
                var pu = Pickups[i];
                pu.T += dt;
                foreach (var p in Players)
                {
                    if (!p.Alive || Vec2.Distance(p.Pos, pu.Pos) >= T.Pickups.PickRadius) continue;
                    if (pu.Type == PowerUp.Bordado) p.Shield = true; else p.Power = pu.Type;
                    Emit(SimEventType.PickupTaken, p.Id, pos: pu.Pos, extra: (byte)pu.Type, value: pu.Id);
                    Pickups.RemoveAt(i);
                    break;
                }
            }
        }

        void SpawnPickup()
        {
            var type = (PowerUp)(1 + Rng.NextInt(PowerUpInfo.Count));
            var pos = Arena.RandomFreePoint(ref Rng, 1.0f);
            for (int i = 0; i < 10; i++)
            {
                bool ok = true;
                foreach (var p in Players) if (p.Alive && Vec2.Distance(p.Pos, pos) <= 2.5f) { ok = false; break; }
                if (ok) break;
                pos = Arena.RandomFreePoint(ref Rng, 1.0f);
            }
            var pu = new Pickup { Id = _nextPickupId++, Type = type, Pos = pos };
            Pickups.Add(pu);
            Emit(SimEventType.PickupSpawn, pos: pos, extra: (byte)type, value: pu.Id);
        }

        // ── Utilidades ───────────────────────────────────────
        public int AliveCount { get { int n = 0; foreach (var p in Players) if (p.Alive) n++; return n; } }

        /// <summary>Huella del estado (FNV-1a). Sirve para pruebas de determinismo y detectar desincronización.</summary>
        public uint StateHash()
        {
            uint h = 2166136261u;
            void Mix(float f) { unchecked { h ^= (uint)BitConverter.SingleToInt32Bits(f); h *= 16777619u; } }
            void MixI(int i) { unchecked { h ^= (uint)i; h *= 16777619u; } }
            MixI(Tick); MixI((int)Phase); MixI(Round); MixI((int)Rng.State);
            Mix(RoundTime); Mix(Arena.WallInset); MixI(HotCanHolder); Mix(HotCanTimer);
            MixI(DropPending ? 1 : 0); Mix(DropPos.x); Mix(DropPos.y); Mix(DropT);
            MixI(RetaFighters[0]); MixI(RetaFighters[1]);
            foreach (var id in RetaQueue) MixI(id);
            foreach (var p in Players)
            {
                Mix(p.Pos.x); Mix(p.Pos.y); Mix(p.Vel.x); Mix(p.Vel.y);
                MixI(p.Alive ? 1 : 0); MixI(p.HasCap ? 1 : 0); MixI(p.Score); MixI((int)p.Power);
                Mix(p.PushT); MixI(p.Streak); MixI(p.LastHitBy); Mix(p.LastHitT);
            }
            foreach (var c in Caps) { Mix(c.Pos.x); Mix(c.Pos.y); MixI((int)c.Mode); Mix(c.DeflectT); }
            foreach (var pu in Pickups) { Mix(pu.Pos.x); Mix(pu.Pos.y); MixI((int)pu.Type); }
            foreach (var pd in Puddles) { Mix(pd.Pos.x); Mix(pd.Pos.y); Mix(pd.T); }
            foreach (var w in Arena.TagWalls) { Mix(w.Pos.x); Mix(w.Pos.y); Mix(w.Life); }
            return h;
        }
    }
}
