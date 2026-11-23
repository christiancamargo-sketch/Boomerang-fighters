// ─────────────────────────────────────────────────────────────
// SIMULACIÓN DE LA PARTIDA (C# puro, tick fijo de 60 Hz).
// Es la "verdad" del juego: en local corre en el celular; en red corre
// en el anfitrión (host) o en un servidor dedicado, sin gráficos.
// Port directo de prototipo/src/game.js (ya balanceado).
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;

namespace BoomerangFighters.Core
{
    public sealed partial class MatchSimulation
    {
        public const int TickRate = 60;
        public const float Dt = 1f / TickRate;
        public const int MaxPlayers = 8;

        public readonly MatchConfig Config;
        public readonly Tuning T;
        public readonly Arena Arena;
        public Rng Rng;

        public readonly List<PlayerState> Players = new List<PlayerState>();
        public readonly List<CapState> Caps = new List<CapState>();
        public readonly List<Pickup> Pickups = new List<Pickup>();
        public readonly List<PaintPatch> Paint = new List<PaintPatch>();
        public readonly List<SimEvent> Events = new List<SimEvent>();

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

        int _nextCapId = 1, _nextPickupId = 1, _lastCount = -1;
        float _pickupTimer, _roundStartTime, _simTime;

        public MatchSimulation(MatchConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            if (config.Slots.Count < 2 || config.Slots.Count > MaxPlayers) throw new ArgumentException("Se necesitan de 2 a 8 jugadores.");
            T = config.Tuning ?? Tuning.Default();
            Arena = new Arena(ArenaLibrary.Get(config.ArenaId));
            Rng = new Rng(config.Seed);
            for (int i = 0; i < config.Slots.Count; i++)
            {
                var s = config.Slots[i];
                var p = new PlayerState { Id = i, Name = s.Name, IsBot = s.IsBot, Species = s.Species, Shirt = s.Shirt };
                p.Cap = NewCap(i, false);
                Players.Add(p);
                Caps.Add(p.Cap);
            }
            NextRound();
        }

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
            // barajar spawns de forma determinista
            var spawns = (Vec2[])Arena.Def.Spawns.Clone();
            for (int i = spawns.Length - 1; i > 0; i--)
            {
                int j = Rng.NextInt(i + 1);
                var tmp = spawns[i]; spawns[i] = spawns[j]; spawns[j] = tmp;
            }
            for (int i = 0; i < Players.Count; i++)
            {
                var p = Players[i];
                if (p.Cap.Mode == CapMode.Dead) p.Cap = NewCap(p.Id, false);
                if (!Caps.Contains(p.Cap)) Caps.Add(p.Cap);
                ResetPlayer(p, spawns[i % spawns.Length]);
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
                if (PhaseTimer <= 0f) { Phase = MatchPhase.Play; Emit(SimEventType.Countdown, extra: 0); }
            }

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
                if (Phase == MatchPhase.Play) UpdatePickups(dt);
                for (int i = 0; i < Players.Count; i++)
                {
                    var p = Players[i];
                    if (p.Alive && Arena.LaserHits(p.Pos, T.Player.Radius)) Kill(p, null, KoCause.Laser);
                }
            }

            for (int i = Paint.Count - 1; i >= 0; i--)
            {
                var pp = Paint[i];
                pp.Life -= dt;
                if (pp.Life <= 0f) Paint.RemoveAt(i); else Paint[i] = pp;
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
            p.DashCd -= dt; p.MeleeCd -= dt; p.SlowT -= dt; p.SpawnInvuln -= dt;
            if (p.MeleeT > 0f) p.MeleeT -= dt;

            var move = inp.Move;
            float mlen = move.Length;
            if (mlen > 1f) { move /= mlen; mlen = 1f; }
            if (inp.HasAim && inp.Aim.SqrLength > 1e-6f) { p.AimDir = inp.Aim.Normalized; p.Aiming = true; }
            else { p.Aiming = false; if (mlen > 0.2f) p.AimDir = move.Normalized; }
            if (mlen > 0.2f || inp.HasAim) p.Facing = p.AimDir;
            if (!canAct) { move = Vec2.Zero; mlen = 0f; }

            // dash
            if (canAct && inp.DashPressed && p.DashCd <= 0f)
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

            // movimiento
            Vec2 v;
            if (p.DashT > 0f)
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
            Arena.ResolveCircle(ref p.Pos, PT.Radius);

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
            float outT = SimMath.Lerp(CT.OutTimeMin, CT.OutTimeMax, charge);
            float off = T.Player.Radius + CT.Radius * 0.6f;
            Launch(p.Cap, p.Pos + dir * off, dir * sp, outT, p.Power);
            if (!Caps.Contains(p.Cap)) Caps.Add(p.Cap);
            p.HasCap = false;
            p.Facing = dir;
            Emit(SimEventType.Throw, p.Id, pos: p.Pos, value: charge);
            if (p.Power == PowerUp.Serigrafia)
            {
                foreach (float s in new[] { -CT.GhostSpread, CT.GhostSpread })
                {
                    var gd = Vec2.FromAngle(dir.Angle + s);
                    var g = NewCap(p.Id, true);
                    Launch(g, p.Pos + gd * off, gd * sp, outT, PowerUp.None);
                    Caps.Add(g);
                }
            }
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
            c.Radius = T.Cap.Radius * (power == PowerUp.Sticker ? 2f : 1f);
            c.Y = T.Cap.FlyHeight;
            c.Fade = 0f;
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
        }

        void Catch(PlayerState p)
        {
            p.HasCap = true;
            HoldCap(p.Cap);
            Emit(SimEventType.Catch, p.Id, pos: p.Pos);
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
                        if (tgt != null) d = SimMath.RotateTowards(d, tgt.Pos - c.Pos, CT.HomingTurn * dt);
                    }
                    c.Vel = d * sp;
                    MoveCap(c, dt);
                    if (c.Mode == CapMode.Out && c.T >= c.OutTime)
                    {
                        if (c.Ghost) { c.Mode = CapMode.Dead; return; }
                        StartReturn(c);
                    }
                    break;
                }
                case CapMode.Return:
                {
                    if (!owner.Alive) { Drop(c, c.Vel * 0.3f); return; }
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
                    if (!owner.Alive)
                    {
                        c.Fade += dt;
                        if (c.Fade > CT.GroundFadeTime) c.Mode = CapMode.Dead;
                    }
                    break;
                }
            }

            // rastro de aerosol (Grafiti)
            if (c.Power == PowerUp.Grafiti && c.Lethal)
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
            c.Pos += c.Vel * dt;
            if (!Arena.CapCollision(c.Pos, c.Radius, out var hit)) return;
            c.Pos = hit.Point;
            if (c.Power == PowerUp.Tinta) { Explode(c); return; }
            if (c.Ghost) { c.Mode = CapMode.Dead; return; }
            Emit(SimEventType.Clank, c.OwnerId, pos: c.Pos, extra: 1);
            if (c.Mode == CapMode.Out)
            {
                // rebote en pared: tiros de banda
                float dot = Vec2.Dot(c.Vel, hit.Normal);
                c.Vel -= hit.Normal * (2f * dot);
                c.Mode = CapMode.Return;
                c.RetSpeed = c.Vel.Length * 0.8f;
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
                if (!c.Ghost && owner.Alive)
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
                    if (v.Id == c.OwnerId || !v.Alive || v.SpawnInvuln > 0f) continue;
                    if (Vec2.Distance(c.Pos, v.Pos) >= c.Radius + PT.Radius * 0.85f) continue;
                    if (c.Power == PowerUp.Tinta) { Explode(c); break; }
                    if (v.Shield)
                    {
                        v.Shield = false;
                        Emit(SimEventType.ShieldBreak, v.Id, c.OwnerId, v.Pos);
                        Drop(c, (c.Pos - v.Pos).Normalized * 6f);
                        break;
                    }
                    Kill(v, owner, c.Ghost ? KoCause.Ghost : KoCause.Cap);
                }
            }

            // gorra contra gorra: ¡CLANK!, ambas caen
            for (int i = 0; i < Caps.Count; i++)
            {
                var a = Caps[i];
                if (!a.Lethal) continue;
                for (int j = i + 1; j < Caps.Count; j++)
                {
                    var b = Caps[j];
                    if (!b.Lethal || a.OwnerId == b.OwnerId) continue;
                    if (Vec2.Distance(a.Pos, b.Pos) >= a.Radius + b.Radius) continue;
                    var n = (a.Pos - b.Pos).Normalized;
                    Drop(a, n * 7f); Drop(b, -n * 7f);
                    Emit(SimEventType.Clank, a.OwnerId, b.OwnerId, (a.Pos + b.Pos) * 0.5f, extra: 2);
                    if (!a.Lethal) break;
                }
            }
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
                    if (v.Shield) { v.Shield = false; Emit(SimEventType.ShieldBreak, v.Id, p.Id, v.Pos); continue; }
                    Kill(v, p, KoCause.Melee);
                }
                // el golpe desvía gorras enemigas
                foreach (var c in Caps)
                {
                    if (c.OwnerId == p.Id || !c.Lethal) continue;
                    var d = c.Pos - p.Pos;
                    if (d.Length > PT.MeleeRange + 0.5f) continue;
                    if (MathF.Abs(SimMath.AngleDelta(fa, d.Angle)) > PT.MeleeArc / 2f + 0.3f) continue;
                    Drop(c, p.Facing * 9f);
                    Emit(SimEventType.Deflect, p.Id, c.OwnerId, c.Pos);
                }
            }
        }

        void Explode(CapState c)
        {
            Emit(SimEventType.Explode, c.OwnerId, pos: c.Pos, value: T.Cap.ExplosionRadius);
            foreach (var v in Players)
            {
                if (v.Id == c.OwnerId || !v.Alive || v.SpawnInvuln > 0f) continue;
                if (Vec2.Distance(v.Pos, c.Pos) >= T.Cap.ExplosionRadius) continue;
                if (v.Shield) { v.Shield = false; Emit(SimEventType.ShieldBreak, v.Id, c.OwnerId, v.Pos); continue; }
                Kill(v, Players[c.OwnerId], KoCause.Ink);
            }
            c.Power = PowerUp.None;
            Drop(c, Vec2.Zero);
        }

        void Kill(PlayerState v, PlayerState killer, KoCause cause)
        {
            if (!v.Alive) return;
            v.Alive = false;
            v.Charging = false;
            if (killer != null && killer != v) killer.Kos++;
            if (!v.HasCap && v.Cap.Mode == CapMode.Ground) v.Cap.Fade = 0f;
            v.Power = PowerUp.None;
            v.Shield = false;
            FreezeTicks = T.Round.HitStopTicks;
            Emit(SimEventType.KO, v.Id, killer?.Id ?? -1, v.Pos, extra: (byte)cause);

            if (Phase == MatchPhase.Play)
            {
                PlayerState last = null;
                int alive = 0;
                foreach (var p in Players) if (p.Alive) { alive++; last = p; }
                if (alive <= 1)
                {
                    Phase = MatchPhase.RoundEnd;
                    PhaseTimer = T.Round.EndDelay;
                    SlowMo = T.Round.SlowMoTime;
                    RoundWinnerId = last?.Id ?? -1;
                }
            }
        }

        void FinishRound()
        {
            RoundDurations.Add(_simTime - _roundStartTime);
            PlayerState w = RoundWinnerId >= 0 ? Players[RoundWinnerId] : null;
            if (w != null && w.Alive) { w.Score++; w.RoundWins++; }
            Emit(SimEventType.RoundEnd, w?.Id ?? -1, value: Round);
            if (w != null && w.Score >= Config.PointsToWin)
            {
                Phase = MatchPhase.MatchEnd;
                MatchWinnerId = w.Id;
                Emit(SimEventType.MatchEnd, w.Id);
                return;
            }
            Phase = MatchPhase.Between;
            PhaseTimer = T.Round.BetweenRounds;
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
            foreach (var p in Players)
            {
                Mix(p.Pos.x); Mix(p.Pos.y); Mix(p.Vel.x); Mix(p.Vel.y);
                MixI(p.Alive ? 1 : 0); MixI(p.HasCap ? 1 : 0); MixI(p.Score); MixI((int)p.Power);
            }
            foreach (var c in Caps) { Mix(c.Pos.x); Mix(c.Pos.y); MixI((int)c.Mode); }
            foreach (var pu in Pickups) { Mix(pu.Pos.x); Mix(pu.Pos.y); MixI((int)pu.Type); }
            return h;
        }
    }
}
