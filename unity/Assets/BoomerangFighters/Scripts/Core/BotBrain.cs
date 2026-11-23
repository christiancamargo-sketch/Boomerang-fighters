// ─────────────────────────────────────────────────────────────
// BOTS: generan InputCommand igual que un humano. Por eso sirven
// sin red, en LAN y online (solo corren en el host).
// Port de prototipo/src/ai.js. Usan su propio Rng con semilla para
// no alterar la secuencia aleatoria de la partida.
// ─────────────────────────────────────────────────────────────
using System;

namespace BoomerangFighters.Core
{
    public sealed class BotBrain
    {
        readonly MatchSimulation _sim;
        readonly int _id;
        readonly BotProfile _cfg;
        Rng _rng;

        int _targetId = -1;
        float _retarget, _think, _throwCooldown, _chargeGoal, _chargeT, _strafeT, _dodgeT, _preferredDist;
        int _strafeDir;
        bool _charging;
        Vec2 _wander;
        bool _dodgePending;
        float _dodgeDelay;
        Vec2 _dodgeDir;

        public BotBrain(MatchSimulation sim, int playerId, BotDifficulty difficulty)
        {
            _sim = sim;
            _id = playerId;
            _cfg = BotProfile.For(difficulty);
            _rng = new Rng(sim.Config.Seed * 2654435761u + (uint)playerId * 97u + 1u);
            _throwCooldown = 1f + _rng.NextFloat();
            _strafeDir = _rng.NextFloat() < 0.5f ? 1 : -1;
            _preferredDist = 5f + _rng.NextFloat() * 3f;
        }

        public InputCommand Think(float dt)
        {
            var cmd = new InputCommand();
            var me = _sim.Players[_id];
            if (!me.Alive || _sim.Phase != MatchPhase.Play) { _charging = false; return cmd; }
            var T = _sim.T;

            _retarget -= dt; _think -= dt; _throwCooldown -= dt; _strafeT -= dt; _dodgeT -= dt;

            // objetivo: el más cercano (con algo de "terquedad")
            var tgt = _targetId >= 0 ? _sim.Players[_targetId] : null;
            if (tgt == null || !tgt.Alive || _retarget <= 0f)
            {
                PlayerState best = null;
                float bd = float.MaxValue;
                foreach (var o in _sim.Players)
                {
                    if (o.Id == _id || !o.Alive) continue;
                    float d = Vec2.Distance(me.Pos, o.Pos) + (o.Id == _targetId ? -2f : 0f) + _rng.NextFloat() * 1.5f;
                    if (d < bd) { bd = d; best = o; }
                }
                tgt = best;
                _targetId = best?.Id ?? -1;
                _retarget = 1.5f + _rng.NextFloat() * 1.5f;
            }

            // esquivar gorras que vienen
            if (!_dodgePending && _dodgeT <= 0f && IncomingThreat(me, out var perp))
            {
                if (_rng.NextFloat() < _cfg.DodgeChance)
                {
                    _dodgePending = true;
                    _dodgeDelay = _cfg.Reaction * (0.5f + _rng.NextFloat() * 0.6f);
                    _dodgeDir = perp;
                }
                _dodgeT = 0.6f;
            }
            Vec2 move = Vec2.Zero;
            if (_dodgePending)
            {
                _dodgeDelay -= dt;
                move = _dodgeDir;
                if (_dodgeDelay <= 0f)
                {
                    _dodgePending = false;
                    cmd.Move = _dodgeDir;
                    if (me.DashCd <= 0f) cmd.Buttons |= InputButtons.DashPressed;
                    return cmd;
                }
            }

            // láser: alejarse
            var lz = _sim.Arena.LaserDanger(me.Pos, 1.6f);
            if (lz.HasValue) move.y += me.Pos.y > lz.Value ? 1.5f : -1.5f;

            if (me.HasCap)
            {
                var pu = NearestPickup(me.Pos, out float pud);
                if (pu.HasValue && pud < 6f && (tgt == null || pud < Vec2.Distance(me.Pos, tgt.Pos)))
                {
                    move += (pu.Value - me.Pos).Normalized;
                }
                else if (tgt != null)
                {
                    var delta = tgt.Pos - me.Pos;
                    float d = MathF.Max(delta.Length, 1e-3f);
                    var n = delta / d;
                    float radial = d > _preferredDist + 1f ? 1f : (d < _preferredDist - 1.5f ? -0.8f : 0f);
                    if (_strafeT <= 0f) { _strafeDir *= -1; _strafeT = 0.8f + _rng.NextFloat() * 1.4f; }
                    move += n * radial + n.Perp * (_strafeDir * 0.7f);

                    if (d < T.Player.MeleeRange * 0.95f && me.MeleeCd <= 0f && _rng.NextFloat() < 0.25f)
                    {
                        cmd.Buttons |= InputButtons.MeleePressed | InputButtons.HasAim;
                        cmd.Aim = n;
                    }

                    bool los = _sim.Arena.LineOfSight(me.Pos, tgt.Pos);
                    if (!_charging && _throwCooldown <= 0f && los && d < 15f)
                    {
                        _charging = true;
                        _chargeT = 0f;
                        _chargeGoal = SimMath.Clamp((d - 3f) / 10f + (_rng.NextFloat() - 0.5f) * 0.3f, 0.05f, 1f);
                        cmd.Buttons |= InputButtons.ThrowPressed;
                    }
                    if (_charging)
                    {
                        _chargeT += dt;
                        cmd.Buttons |= InputButtons.ThrowHeld | InputButtons.HasAim;
                        cmd.Aim = LeadAim(me, tgt, d);
                        if (_chargeT >= _chargeGoal * T.Cap.ChargeTime)
                        {
                            cmd.Buttons &= ~InputButtons.ThrowHeld;
                            cmd.Buttons |= InputButtons.ThrowReleased;
                            _charging = false;
                            _throwCooldown = 1f / _cfg.ThrowRate * (0.6f + _rng.NextFloat() * 0.8f);
                        }
                    }
                }
            }
            else
            {
                _charging = false;
                var cap = me.Cap;
                if (cap.Mode == CapMode.Ground)
                {
                    float dc = Vec2.Distance(me.Pos, cap.Pos);
                    if (dc < 3f) move += (cap.Pos - me.Pos).Normalized * 1.5f;
                    else if (_think <= 0f) { cmd.Buttons |= InputButtons.ThrowPressed | InputButtons.ThrowReleased; _think = 0.4f; }
                }
                // sin gorra: alejarse de rivales armados
                var flee = Vec2.Zero;
                foreach (var o in _sim.Players)
                {
                    if (o.Id == _id || !o.Alive || !o.HasCap) continue;
                    var dv = me.Pos - o.Pos;
                    float d = dv.Length;
                    if (d < 7f) flee += dv / (d * d + 0.1f);
                }
                move += flee * 4f;
                if (cap.Mode == CapMode.Return || cap.Mode == CapMode.Recall) move += (cap.Pos - me.Pos).Normalized * 0.6f;
            }

            if (_think <= 0f)
            {
                _wander = new Vec2((_rng.NextFloat() - 0.5f) * 0.6f, (_rng.NextFloat() - 0.5f) * 0.6f);
                _think = 0.5f + _rng.NextFloat() * 0.5f;
            }
            move += _wander;

            // evitar obstáculos
            const float look = 1.4f;
            var n0 = move.Normalized;
            if (!_sim.Arena.IsFree(me.Pos + n0 * look, T.Player.Radius * 0.9f))
            {
                var l = n0.Perp;
                bool leftFree = _sim.Arena.IsFree(me.Pos + l * look, T.Player.Radius);
                move = leftFree ? l : -l;
            }

            float mag = MathF.Min(1f, move.Length);
            cmd.Move = move.Normalized * mag;
            return cmd;
        }

        Vec2 LeadAim(PlayerState me, PlayerState tgt, float d)
        {
            var T = _sim.T;
            float speed = SimMath.Lerp(T.Cap.SpeedMin, T.Cap.SpeedMax, _chargeGoal);
            float t = d / speed * _cfg.Lead;
            var aim = tgt.Pos + tgt.Vel * t - me.Pos;
            float a = aim.Angle + (_rng.NextFloat() - 0.5f) * 2f * _cfg.AimError;
            return Vec2.FromAngle(a);
        }

        Vec2? NearestPickup(Vec2 from, out float dist)
        {
            Vec2? best = null;
            dist = float.MaxValue;
            foreach (var pu in _sim.Pickups)
            {
                float d = Vec2.Distance(from, pu.Pos);
                if (d < dist) { dist = d; best = pu.Pos; }
            }
            return best;
        }

        bool IncomingThreat(PlayerState me, out Vec2 perp)
        {
            perp = Vec2.Zero;
            foreach (var c in _sim.Caps)
            {
                if (!c.Lethal || c.OwnerId == _id) continue;
                float sp = c.Vel.Length;
                if (sp < 2f) continue;
                var r = me.Pos - c.Pos;
                float t = Vec2.Dot(r, c.Vel) / (sp * sp);
                if (t < 0f || t > 0.6f) continue;
                var closest = c.Pos + c.Vel * t - me.Pos;
                if (closest.Length >= 1.3f + c.Radius) continue;
                var p = new Vec2(-c.Vel.y / sp, c.Vel.x / sp);
                if (Vec2.Dot(p, -closest) < 0f) p = -p;
                perp = p;
                return true;
            }
            return false;
        }
    }
}
