// ─────────────────────────────────────────────────────────────
// BOTS: generan InputCommand igual que un humano. Por eso sirven
// sin red, en LAN y online (solo corren en el host).
// Port de prototipo/src/ai.js. Usan su propio Rng con semilla para
// no alterar la secuencia aleatoria de la partida.
// v0.2 (D): desvían gorras ("Te la regreso"), empujan sin gorra,
// huyen de los charcos y juegan Lata Caliente (persiguen / huyen).
// v0.3 (V5): tratan los vacíos como obstáculos, no hacen dash hacia un
// vacío, se alejan de las orillas (< 0.8 m) y salen de la sombra de la caja DROP.
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

        public static float HUNT = float.Parse(System.Environment.GetEnvironmentVariable("HUNT") ?? "5"), PITP = float.Parse(System.Environment.GetEnvironmentVariable("PITP") ?? "0.05"); public static float SIDE = float.Parse(System.Environment.GetEnvironmentVariable("SIDE") ?? "0.6"); public static float SLIDE = float.Parse(System.Environment.GetEnvironmentVariable("SLIDE") ?? "1.3"); public static bool GOODSIDE = System.Environment.GetEnvironmentVariable("GOODSIDE") == "1", NORECALL = System.Environment.GetEnvironmentVariable("NORECALL") == "1"; public static float HUNTA = float.Parse(System.Environment.GetEnvironmentVariable("HUNTA") ?? "9"), HUNTCD = float.Parse(System.Environment.GetEnvironmentVariable("HUNTCD") ?? "0"), HUNTMAX = float.Parse(System.Environment.GetEnvironmentVariable("HUNTMAX") ?? "1.5"); float _huntCd, _huntT; public static bool USEDASH = System.Environment.GetEnvironmentVariable("USEDASH") == "1";
        public static int DbgCapless, DbgFound, DbgInPlace, DbgNear5;
        int _targetId = -1;
        float _retarget, _think, _throwCooldown, _chargeGoal, _chargeT, _strafeT, _dodgeT, _preferredDist;
        int _strafeDir;
        bool _charging;
        Vec2 _wander;
        bool _dodgePending;
        float _dodgeDelay;
        Vec2 _dodgeDir;
        int _deflectRolledCap = -1;   // gorra que ya evaluamos (una tirada por gorra)
        int _deflectCap = -1;         // gorra que vamos a intentar desviar

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

            // Lata Caliente: con la lata, perseguir al más cercano; sin ella, huir de quien la trae
            bool hotMode = _sim.Mode == GameMode.LataCaliente && _sim.HotCanHolder >= 0;
            bool iHaveCan = hotMode && _sim.HotCanHolder == _id;
            if (iHaveCan)
            {
                PlayerState near = null;
                float nd = float.MaxValue;
                foreach (var o in _sim.Players)
                {
                    if (o.Id == _id || !o.Alive || (o.Id == _sim.HotCanImmuneId && _sim.HotCanImmuneT > 0f)) continue;
                    float d = Vec2.Distance(me.Pos, o.Pos);
                    if (d < nd) { nd = d; near = o; }
                }
                if (near != null) { tgt = near; _targetId = near.Id; }
            }

            // D: desviar una gorra rival letal que va a pasar cerca (con nuestra gorra)
            if (!hotMode && me.HasCap && me.MeleeCd <= 0f && !_charging)
            {
                if (TryDeflect(me, out var aimAt)) { cmd.Buttons |= InputButtons.MeleePressed | InputButtons.HasAim; cmd.Aim = aimAt; return cmd; }
            }

            // esquivar gorras que vienen (en Lata Caliente las gorras no noquean)
            if (!hotMode && _deflectCap < 0 && !_dodgePending && _dodgeT <= 0f && IncomingThreat(me, out var perp))
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
                    // V5: nunca hacer dash hacia un vacío (si el dash no es seguro, esquiva caminando)
                    if (DashIsSafe(me, _dodgeDir))
                    {
                        cmd.Move = _dodgeDir;
                        if (me.DashCd <= 0f) cmd.Buttons |= InputButtons.DashPressed;
                    }
                    else cmd.Move = AvoidPits(me, _dodgeDir);
                    return cmd;
                }
            }

            // láser: alejarse
            var lz = _sim.Arena.LaserDanger(me.Pos, 1.6f);
            if (lz.HasValue) move.y += me.Pos.y > lz.Value ? 1.5f : -1.5f;

            // charcos que crecen: salir del círculo
            foreach (var pd in _sim.Puddles)
            {
                if (pd.OwnerId == _id) continue;
                var dv = me.Pos - pd.Pos;
                float dl = dv.Length;
                if (dl < T.Cap.ExplosionRadius + 0.6f) move += (dl > 1e-3f ? dv / dl : new Vec2(1f, 0f)) * 2.5f;
            }

            // caja DROP: salir de la sombra de aviso
            if (_sim.DropPending && _sim.Arena.Def.Drop != null)
            {
                var dv = me.Pos - _sim.DropPos;
                float dl = dv.Length;
                if (dl < _sim.Arena.Def.Drop.Radius + 0.9f) move += (dl > 1e-3f ? dv / dl : new Vec2(1f, 0f)) * 2.5f;
            }

            // muerte súbita: no quedarse pegado al muro que avanza
            if (_sim.SuddenDeath != SuddenDeathStage.None && MathF.Abs(me.Pos.x) > _sim.Arena.PlayHalfW - 2f)
                move.x += me.Pos.x > 0f ? -1f : 1f;

            if (hotMode && !iHaveCan)
            {
                // sin la lata: huir de quien la tiene
                var h = _sim.Players[_sim.HotCanHolder];
                if (h.Alive)
                {
                    var dv = me.Pos - h.Pos;
                    float d = MathF.Max(dv.Length, 0.1f);
                    if (d < 9f) move += dv / d * (1.6f * (9f - d) / 9f + 0.4f);
                }
            }
            else if (iHaveCan && tgt != null)
            {
                // con la lata: ir encima del más cercano y pasarla con golpe o gorrazo
                var delta = tgt.Pos - me.Pos;
                float d = MathF.Max(delta.Length, 1e-3f);
                var n = delta / d;
                move += n * 1.5f;
                if (me.HasCap && d < T.Player.MeleeRange * 0.95f && me.MeleeCd <= 0f)
                {
                    cmd.Buttons |= InputButtons.MeleePressed | InputButtons.HasAim;
                    cmd.Aim = n;
                }
                else if (me.HasCap && _throwCooldown <= 0f && d < 9f && _sim.Arena.LineOfSight(me.Pos, tgt.Pos))
                {
                    cmd.Buttons |= InputButtons.ThrowPressed | InputButtons.ThrowReleased | InputButtons.HasAim;
                    cmd.Aim = LeadAim(me, tgt, d);
                    _throwCooldown = 1f / _cfg.ThrowRate * (0.6f + _rng.NextFloat() * 0.8f);
                }
                else if (!me.HasCap && me.Cap.Mode == CapMode.Ground && _think <= 0f)
                {
                    cmd.Buttons |= InputButtons.ThrowPressed | InputButtons.ThrowReleased; _think = 0.4f;
                }
            }
            else if (me.HasCap)
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
                    if (GOODSIDE && _sim.Arena.HasPits && GoodSide(me, tgt, n, out int side)) _strafeDir = side;
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
                    bool hunting = NORECALL && _huntCd <= 0f && _sim.Rules.Shove && _sim.Arena.HasPits && FindPitShove(me, out _, out _, out _);
                    if (hunting) { }
                    else if (dc < 3f) move += (cap.Pos - me.Pos).Normalized * 1.5f;
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

                // v0.3: sin gorra, aprovechar los vacíos: empujar al rival que quedaría dentro de uno
                bool pitShove = false;
                DbgCapless++;
                foreach (var o in _sim.Players) if (o.Id != _id && o.Alive && Vec2.Distance(o.Pos, me.Pos) < 5f) { DbgNear5++; break; }
                _huntCd -= dt;
                if (_huntCd <= 0f && _sim.Rules.Shove && _sim.Arena.HasPits && FindPitShove(me, out var shoveAim, out var standAt, out bool inPlace))
                {
                    DbgFound++;
                    _huntT += dt;
                    if (_huntT > HUNTMAX) { _huntCd = HUNTCD; _huntT = 0f; }
                    if (inPlace)
                    {
                        DbgInPlace++;
                        if (me.ShoveCd <= 0f && _rng.NextFloat() < PITP)
                        {
                            cmd.Buttons |= InputButtons.MeleePressed | InputButtons.HasAim;
                            cmd.Aim = shoveAim;
                            pitShove = true;
                            _huntCd = HUNTCD; _huntT = 0f;
                        }
                    }
                    else
                    {
                        var go = standAt - me.Pos;
                        float gl = go.Length;
                        move += (gl > 1e-3f ? go / gl : Vec2.Zero) * 2.2f; // acomodarse detrás del rival (gana a "huir")
                        if (USEDASH && gl > 2.4f && me.DashCd <= 0f && DashIsSafe(me, go / gl)) { cmd.Move = go / gl; cmd.Buttons |= InputButtons.DashPressed; return cmd; }
                    }
                }
                else _huntT = 0f;

                // D: empujón si un rival está a menos de 1.4 m (probabilidad 0.3 por segundo)
                if (!pitShove && _sim.Rules.Shove && me.ShoveCd <= 0f)
                {
                    PlayerState close = null;
                    float cd = 1.4f;
                    foreach (var o in _sim.Players)
                    {
                        if (o.Id == _id || !o.Alive) continue;
                        float d = Vec2.Distance(me.Pos, o.Pos);
                        if (d < cd) { cd = d; close = o; }
                    }
                    if (close != null && _rng.NextFloat() < 0.3f * dt)
                    {
                        cmd.Buttons |= InputButtons.MeleePressed | InputButtons.HasAim;
                        cmd.Aim = (close.Pos - me.Pos).Normalized;
                    }
                }
            }

            if (_think <= 0f)
            {
                _wander = new Vec2((_rng.NextFloat() - 0.5f) * 0.6f, (_rng.NextFloat() - 0.5f) * 0.6f);
                _think = 0.5f + _rng.NextFloat() * 0.5f;
            }
            move += _wander;

            // V5: alejarse de la orilla de un vacío si está a menos de 0.8 m
            if (_sim.Arena.HasPits) move += _sim.Arena.PitRepulsion(me.Pos, T.Pits.SafeMargin) * 3f;

            // evitar obstáculos (IsFree cuenta los vacíos como obstáculo)
            const float look = 1.4f;
            var n0 = move.Normalized;
            if (!_sim.Arena.IsFree(me.Pos + n0 * look, T.Player.Radius * 0.9f))
            {
                var l = n0.Perp;
                bool leftFree = _sim.Arena.IsFree(me.Pos + l * look, T.Player.Radius);
                move = leftFree ? l : -l;
            }

            float mag = MathF.Min(1f, move.Length);
            cmd.Move = AvoidPits(me, move.Normalized * mag);
            return cmd;
        }

        // ── V5: vacíos ───────────────────────────────────────
        /// <summary>¿Dar unos pasos en esta dirección lo dejaría sobre un vacío (o pegado a su orilla)?</summary>
        bool StepIsSafe(PlayerState me, Vec2 dir)
        {
            var arena = _sim.Arena;
            // se cuenta el patinón: la velocidad actual tarda unos cuadros en girar
            var from = me.Pos + me.Vel * 0.06f;
            const float pad = 0.25f; // que el centro no se acerque a menos de 0.25 m de la orilla
            return !arena.InPit(from + dir * 0.4f, -pad) && !arena.InPit(from + dir * 0.8f, -pad);
        }

        /// <summary>
        /// Último filtro del movimiento: si el paso lleva a un vacío, se gira (poco a poco, hacia ambos lados)
        /// hasta encontrar una dirección segura; si no hay, se queda quieto.
        /// </summary>
        Vec2 AvoidPits(PlayerState me, Vec2 move)
        {
            if (!_sim.Arena.HasPits) return move;
            float mag = move.Length;
            if (mag < 1e-4f) return move;
            var dir = move / mag;
            if (StepIsSafe(me, dir)) return move;
            float a = dir.Angle;
            for (int i = 1; i <= 6; i++)
            {
                float turn = i * (MathF.PI / 6f);
                var left = Vec2.FromAngle(a + turn);
                if (StepIsSafe(me, left)) return left * MathF.Max(mag, 0.8f);
                var right = Vec2.FromAngle(a - turn);
                if (StepIsSafe(me, right)) return right * MathF.Max(mag, 0.8f);
            }
            return Vec2.Zero;
        }

        /// <summary>Con gorra: ¿hacia qué lado conviene rodear para dejar al rival entre un vacío cercano y yo?</summary>
        bool GoodSide(PlayerState me, PlayerState tgt, Vec2 n, out int side)
        {
            side = 0;
            float best = 4f;
            Vec2 u = Vec2.Zero;
            foreach (var pit in _sim.Arena.Def.Pits)
            {
                var c = pit.Closest(tgt.Pos) - tgt.Pos;
                float d = c.Length;
                if (d >= best || d < 0.05f) continue;
                best = d; u = c / d;
            }
            if (u.SqrLength < 0.5f) return false;
            // quiero quedar en tgt − u·d: moverme de lado hacia allá
            var want = tgt.Pos - u * Vec2.Distance(me.Pos, tgt.Pos) - me.Pos;
            float s = Vec2.Dot(n.Perp, want);
            if (MathF.Abs(s) < 0.6f) return false; // ya estoy alineado
            side = s > 0f ? 1 : -1;
            return true;
        }

        /// <summary>¿Un empujón (2.5 m) en esa dirección dejaría a "o" dentro de un vacío?</summary>
        bool ShoveEndsInPit(PlayerState o, Vec2 dir)
        {
            var PT = _sim.T.Player;
            return _sim.Arena.InPit(o.Pos + dir * PT.ShoveDistance, _sim.T.Pits.FallInset + 0.08f);
        }

        /// <summary>
        /// Busca un rival al que un empujón mandaría a un vacío. "inPlace" = ya estoy en posición (empujar ya,
        /// apuntando a "aim"); si no, "standAt" es el punto detrás del rival desde donde sí saldría.
        /// Solo cuenta si ya estoy más o menos del lado correcto (no rodea al rival).
        /// </summary>
        bool FindPitShove(PlayerState me, out Vec2 aim, out Vec2 standAt, out bool inPlace)
        {
            aim = Vec2.Zero; standAt = me.Pos; inPlace = false;
            var PT = _sim.T.Player;
            var arena = _sim.Arena;
            float range = PT.MeleeRange + PT.Radius * 0.5f - 0.1f;
            float bestD = float.MaxValue;
            bool found = false;
            foreach (var o in _sim.Players)
            {
                if (o.Id == _id || !o.Alive || o.PushT > 0f) continue;
                var delta = o.Pos - me.Pos;
                float d = delta.Length;
                if (d > (o.HasCap ? HUNTA : HUNT) || d < 1e-3f) continue;
                var dir = delta / d;
                bool straight = ShoveEndsInPit(o, dir);
                if (d <= range && straight)
                {
                    aim = dir; inPlace = true;
                    return true;
                }
                if (d >= bestD) continue;
                Vec2 u = dir;
                bool ok = straight;
                if (!ok)
                {
                    // corregir un poco el ángulo: hacia el punto más cercano del vacío o hacia su centro
                    foreach (var pit in arena.Def.Pits)
                    {
                        if (pit.Distance(o.Pos) > PT.ShoveDistance) continue;
                        for (int k = 0; k < 2 && !ok; k++)
                        {
                            var c = (k == 0 ? pit.Closest(o.Pos) : new Vec2(pit.x, pit.y)) - o.Pos;
                            var cu = c.Normalized;
                            if (cu.SqrLength < 0.5f || Vec2.Dot(cu, dir) < SIDE || !ShoveEndsInPit(o, cu)) continue;
                            u = cu; ok = true;
                        }
                        if (ok) break;
                    }
                }
                if (!ok) continue;
                var spot = o.Pos - u * 1.8f;
                // el punto de apoyo debe ser pisable y no estar pegado a otro vacío
                if (!arena.IsFree(spot, PT.Radius * 0.9f) || arena.NearPit(spot, 0.4f)) continue;
                standAt = spot; bestD = d; found = true;
            }
            return found;
        }

        /// <summary>Un dash (2.7 m) más lo que patina al terminar: seguro solo si en todo el trayecto no hay vacío.</summary>
        bool DashIsSafe(PlayerState me, Vec2 dir)
        {
            if (!_sim.Arena.HasPits) return true;
            var PT = _sim.T.Player;
            float reach = PT.DashSpeed * PT.DashTime + SLIDE;
            return !_sim.Arena.SegmentCrossesPit(me.Pos, me.Pos + dir * reach, 0.3f);
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

        /// <summary>
        /// D: si una gorra rival letal va a pasar a menos de 1.8 m en los próximos 0.25 s, se tira
        /// una vez la probabilidad de la dificultad; si sale, se golpea hacia ella cuando entra al alcance.
        /// </summary>
        bool TryDeflect(PlayerState me, out Vec2 aim)
        {
            aim = Vec2.Zero;
            var PT = _sim.T.Player;
            bool seen = false;
            foreach (var c in _sim.Caps)
            {
                if (!c.Lethal || c.Ghost) continue;
                if (c.Deflected ? c.DeflectBy == _id : c.OwnerId == _id) continue;
                float sp = c.Vel.Length;
                if (sp < 2f) continue;
                var r = me.Pos - c.Pos;
                float t = Vec2.Dot(r, c.Vel) / (sp * sp);
                if (t < 0f || t > 0.25f) continue;
                var closest = c.Pos + c.Vel * t - me.Pos;
                if (closest.Length >= 1.8f) continue;
                seen = true;
                if (_deflectRolledCap != c.Id)
                {
                    _deflectRolledCap = c.Id;
                    _deflectCap = _rng.NextFloat() < _cfg.DeflectChance ? c.Id : -1;
                }
                if (_deflectCap != c.Id) continue;
                // esperar a que entre al alcance del golpe (la ventana activa dura 0.13 s)
                if (r.Length > PT.MeleeRange + 0.3f && t > 0.08f) continue;
                aim = (c.Pos + c.Vel * 0.04f - me.Pos).Normalized;
                _deflectCap = -1;
                return aim.SqrLength > 0f;
            }
            // si ya no viene nada, la próxima gorra (o el regreso) se vuelve a tirar
            if (!seen) { _deflectRolledCap = -1; _deflectCap = -1; }
            return false;
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
