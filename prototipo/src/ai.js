// ─────────────────────────────────────────────────────────────
// BOTS
// Un "mando virtual" que decide igual que un humano: moverse,
// apuntar con predicción, lanzar, esquivar con dash, recoger
// power-ups y recuperar su gorra. La dificultad cambia el tiempo
// de reacción, el error de puntería y qué tanto esquiva.
// v0.2 (spec D): desvían gorras ("Te la regreso"), empujan sin gorra
// y en Lata Caliente persiguen (si traen la lata) o huyen.
// Todas sus "tiradas de dado" usan el Rng de la partida.
// ─────────────────────────────────────────────────────────────
import { emptyPad } from './input.js';
import { DIFFICULTY, TUNING } from './config.js';

export class BotController {
  constructor(game, player, difficulty = 'normal') {
    this.kind = 'bot';
    this.label = 'Bot';
    this.game = game;
    this.player = player;
    this.cfg = DIFFICULTY[difficulty] || DIFFICULTY.normal;
    this.target = null;
    this.retarget = 0;
    this.think = 0;
    this.throwCooldown = 1 + this._rand();
    this.chargeGoal = 0;
    this.charging = false;
    this.chargeT = 0;
    this.strafeDir = this._rand() < 0.5 ? 1 : -1;
    this.strafeT = 0;
    this.wander = { x: 0, z: 0 };
    this.dodgeT = 0;
    this.reactT = 0;
    this.pendingDodge = null;
    this.preferredDist = 5 + this._rand() * 3;
    this.deflectRolled = new Set(); // gorras rivales para las que ya se tiró el dado de desvío
  }

  _rand() { return this.game.rng.next(); }

  poll(dt) {
    const g = this.game;
    const me = this.player;
    const p = emptyPad();
    if (!me.alive || g.state !== 'play') { this.charging = false; return p; }

    this.retarget -= dt; this.think -= dt; this.throwCooldown -= dt; this.strafeT -= dt; this.dodgeT -= dt;

    // ── elegir objetivo (el más cercano con algo de "terquedad")
    if (!this.target || !this.target.alive || this.retarget <= 0) {
      let best = null, bd = Infinity;
      for (const o of g.players) {
        if (o === me || !o.alive) continue;
        const d = dist(me.pos, o.pos) + (o === this.target ? -2 : 0) + this._rand() * 1.5;
        if (d < bd) { bd = d; best = o; }
      }
      this.target = best;
      this.retarget = 1.5 + this._rand() * 1.5;
    }
    const tgt = this.target;

    // ── esquivar gorras que vienen hacia mí
    const threat = this._incomingThreat();
    if (threat && !this.pendingDodge && this.dodgeT <= 0) {
      if (this._rand() < this.cfg.dodgeChance) {
        this.pendingDodge = { t: this.cfg.reaction * (0.5 + this._rand() * 0.6), dir: threat.perp };
      }
      this.dodgeT = 0.6;
    }
    let moveX = 0, moveZ = 0;
    if (this.pendingDodge) {
      this.pendingDodge.t -= dt;
      moveX = this.pendingDodge.dir.x; moveZ = this.pendingDodge.dir.z;
      if (this.pendingDodge.t <= 0) {
        p.dashPressed = me.dashCd <= 0;
        p.mx = moveX; p.mz = moveZ;
        this.pendingDodge = null;
        return p;
      }
    }

    // ── "Te la regreso": gorra rival letal que pasará a < 1.8 m en 0.25 s → golpe hacia ella
    // (cargando un tiro no se puede golpear: el bot solo lo intenta con las manos libres)
    if (me.hasCap && me.meleeCd <= 0 && !me.charging) {
      const c = this._deflectCandidate();
      if (c && !this.deflectRolled.has(c)) {
        this.deflectRolled.add(c);
        if (this._rand() < this.cfg.deflectChance) {
          p.meleePressed = true;
          p.ax = c.pos.x - me.pos.x; p.az = c.pos.z - me.pos.z; p.aim = true;
          return p; // el golpe tiene prioridad este cuadro (no empieza a cargar)
        }
      }
    }
    for (const c of this.deflectRolled) if (!c.lethal() || c.dead) this.deflectRolled.delete(c);

    // ── empujón sin gorra: rival a < 1.4 m, probabilidad 0.3 por segundo
    if (!me.hasCap && g.rules.shove && me.shoveCd <= 0) {
      const o = g.nearestEnemy(me, me.pos, 1.4);
      if (o && this._rand() < 0.3 * dt) {
        p.meleePressed = true;
        p.ax = o.pos.x - me.pos.x; p.az = o.pos.z - me.pos.z; p.aim = true;
      }
    }

    // ── Lata Caliente: si la traigo, persigo al más cercano; si no, huyo de quien la trae
    if (g.modeId === 'lataCaliente') return this._hotCan(p, dt, moveX, moveZ);

    // ── láser del museo: alejarse
    const lz = g.arena.laserDanger(me.pos.x, me.pos.z, 1.6);
    if (lz !== null) {
      moveZ += me.pos.z > lz ? 1.5 : -1.5;
    }

    if (me.hasCap) {
      // power-up cercano y seguro → ir por él
      const pu = this._nearestPickup();
      if (pu && dist(me.pos, pu) < 6 && (!tgt || dist(me.pos, pu) < dist(me.pos, tgt.pos))) {
        const d = norm(pu.x - me.pos.x, pu.z - me.pos.z);
        moveX += d.x; moveZ += d.z;
      } else if (tgt) {
        const dx = tgt.pos.x - me.pos.x, dz = tgt.pos.z - me.pos.z;
        const d = Math.hypot(dx, dz) || 1;
        const n = { x: dx / d, z: dz / d };
        // acercarse/alejarse a la distancia preferida + "strafe" lateral
        const radial = d > this.preferredDist + 1 ? 1 : d < this.preferredDist - 1.5 ? -0.8 : 0;
        if (this.strafeT <= 0) { this.strafeDir *= -1; this.strafeT = 0.8 + this._rand() * 1.4; }
        moveX += n.x * radial + -n.z * this.strafeDir * 0.7;
        moveZ += n.z * radial + n.x * this.strafeDir * 0.7;

        // golpe cuerpo a cuerpo si está pegado
        if (d < TUNING.player.meleeRange * 0.95 && me.meleeCd <= 0 && this._rand() < 0.25) {
          p.meleePressed = true;
          p.ax = n.x; p.az = n.z; p.aim = true;
        }

        // lanzar con predicción
        const los = g.arena.lineOfSight(me.pos.x, me.pos.z, tgt.pos.x, tgt.pos.z);
        if (!this.charging && this.throwCooldown <= 0 && los && d < 15 && !this._deflectCandidate()) {
          this.charging = true;
          this.chargeT = 0;
          this.chargeGoal = Math.min(1, Math.max(0.05, (d - 3) / 10 + (this._rand() - 0.5) * 0.3));
          p.throwPressed = true;
        }
        if (this.charging) {
          this.chargeT += dt;
          p.throwHeld = true;
          const aim = this._leadAim(tgt, d);
          p.ax = aim.x; p.az = aim.z; p.aim = true;
          if (this.chargeT >= this.chargeGoal * TUNING.cap.chargeTime) {
            p.throwHeld = false; p.throwReleased = true;
            this.charging = false;
            this.throwCooldown = (1 / this.cfg.throwRate) * (0.6 + this._rand() * 0.8);
          }
        }
      }
    } else {
      this.charging = false;
      const cap = me.cap;
      // gorra en el suelo: llamarla (o ir por ella si está cerca)
      if (cap && cap.state === 'ground') {
        const dc = dist(me.pos, cap.pos);
        if (dc < 3) {
          const d = norm(cap.pos.x - me.pos.x, cap.pos.z - me.pos.z);
          moveX += d.x * 1.5; moveZ += d.z * 1.5;
        } else if (this.think <= 0) {
          p.throwPressed = true; p.throwReleased = true; // "regresa"
          this.think = 0.4;
        }
      }
      // sin gorra: mantener distancia de rivales armados
      let fx = 0, fz = 0;
      for (const o of g.players) {
        if (o === me || !o.alive || !o.hasCap) continue;
        const dx = me.pos.x - o.pos.x, dz = me.pos.z - o.pos.z;
        const d = Math.hypot(dx, dz);
        if (d < 7) { fx += dx / (d * d + 0.1); fz += dz / (d * d + 0.1); }
      }
      moveX += fx * 4; moveZ += fz * 4;
      // con la gorra regresando, moverse un poco hacia ella
      if (cap && (cap.state === 'return' || cap.state === 'recall')) {
        const d = norm(cap.pos.x - me.pos.x, cap.pos.z - me.pos.z);
        moveX += d.x * 0.6; moveZ += d.z * 0.6;
      }
    }

    // vagar un poco para no verse robótico
    if (this.think <= 0) {
      this.wander = { x: (this._rand() - 0.5) * 0.6, z: (this._rand() - 0.5) * 0.6 };
      this.think = 0.5 + this._rand() * 0.5;
    }
    moveX += this.wander.x; moveZ += this.wander.z;

    // evitar obstáculos: sondeo hacia adelante
    const look = 1.4;
    const n0 = norm(moveX, moveZ);
    if (!g.arena.isFree(me.pos.x + n0.x * look, me.pos.z + n0.z * look, TUNING.player.radius * 0.9)) {
      // gira 90° hacia el lado libre
      const l = { x: -n0.z, z: n0.x };
      const leftFree = g.arena.isFree(me.pos.x + l.x * look, me.pos.z + l.z * look, TUNING.player.radius);
      moveX = leftFree ? l.x : -l.x; moveZ = leftFree ? l.z : -l.z;
    }

    const m = norm(moveX, moveZ);
    const mag = Math.min(1, Math.hypot(moveX, moveZ));
    p.mx = m.x * mag; p.mz = m.z * mag;
    return p;
  }

  // Lata Caliente: perseguir (con lata) o huir (sin lata). Lanza y golpea para pasarla.
  _hotCan(p, dt, moveX, moveZ) {
    const g = this.game, me = this.player;
    const hot = g.hot;
    if (hot && hot.holder === me) {
      const o = g.nearestEnemy(me, me.pos, 99);
      if (o) {
        const dx = o.pos.x - me.pos.x, dz = o.pos.z - me.pos.z;
        const d = Math.hypot(dx, dz) || 1;
        moveX += dx / d * 1.5; moveZ += dz / d * 1.5;
        if (d < TUNING.player.meleeRange * 0.95 && me.hasCap && me.meleeCd <= 0) { p.meleePressed = true; p.ax = dx; p.az = dz; p.aim = true; }
        else if (me.hasCap && this.throwCooldown <= 0 && d < 12 && g.arena.lineOfSight(me.pos.x, me.pos.z, o.pos.x, o.pos.z)) {
          p.throwPressed = true; p.throwReleased = true; p.ax = dx; p.az = dz; p.aim = true;
          this.throwCooldown = (1 / this.cfg.throwRate) * (0.6 + this._rand() * 0.8);
        }
        if (d < 3 && me.dashCd <= 0 && this._rand() < 1.5 * dt) { p.dashPressed = true; }
      }
    } else if (hot && hot.holder.alive) {
      const h = hot.holder;
      const dx = me.pos.x - h.pos.x, dz = me.pos.z - h.pos.z;
      const d = Math.hypot(dx, dz) || 1;
      if (d < 8) { moveX += dx / d * 1.6; moveZ += dz / d * 1.6; }
      // en las esquinas, rodear en vez de quedarse atrapado
      moveX += -dz / d * 0.5; moveZ += dx / d * 0.5;
      if (d < 2.5 && me.dashCd <= 0 && this._rand() < this.cfg.dodgeChance * 2 * dt) p.dashPressed = true;
    }
    // recuperar la gorra
    if (!me.hasCap && me.cap.state === 'ground' && this.think <= 0) { p.throwPressed = true; p.throwReleased = true; this.think = 0.4; }
    const lz = g.arena.laserDanger(me.pos.x, me.pos.z, 1.6);
    if (lz !== null) moveZ += me.pos.z > lz ? 1.5 : -1.5;
    const look = 1.4;
    const n0 = norm(moveX, moveZ);
    if (!g.arena.isFree(me.pos.x + n0.x * look, me.pos.z + n0.z * look, TUNING.player.radius * 0.9)) {
      const l = { x: -n0.z, z: n0.x };
      const leftFree = g.arena.isFree(me.pos.x + l.x * look, me.pos.z + l.z * look, TUNING.player.radius);
      moveX = leftFree ? l.x : -l.x; moveZ = leftFree ? l.z : -l.z;
    }
    const m = norm(moveX, moveZ);
    const mag = Math.min(1, Math.hypot(moveX, moveZ));
    p.mx = m.x * mag; p.mz = m.z * mag;
    return p;
  }

  // gorra rival letal que pasará a menos de 1.8 m en los próximos 0.25 s
  _deflectCandidate() {
    const me = this.player;
    for (const c of this.game.caps) {
      if (!c.lethal() || c.dead) continue;
      if (c.state === 'deflect' ? c.deflector === me : c.owner === me) continue;
      const vx = c.vel.x, vz = c.vel.z;
      const sp2 = vx * vx + vz * vz;
      if (sp2 < 4) continue;
      const rx = me.pos.x - c.pos.x, rz = me.pos.z - c.pos.z;
      const t = clamp01((rx * vx + rz * vz) / sp2, 0.25);
      const cx = c.pos.x + vx * t - me.pos.x, cz = c.pos.z + vz * t - me.pos.z;
      if (Math.hypot(cx, cz) < 1.8) return c;
    }
    return null;
  }

  _leadAim(tgt, d) {
    const me = this.player;
    const speed = TUNING.cap.speedMin + (TUNING.cap.speedMax - TUNING.cap.speedMin) * this.chargeGoal;
    const t = (d / speed) * this.cfg.lead;
    let ax = tgt.pos.x + tgt.vel.x * t - me.pos.x;
    let az = tgt.pos.z + tgt.vel.z * t - me.pos.z;
    const a = Math.atan2(az, ax) + (this._rand() - 0.5) * 2 * this.cfg.aimError;
    return { x: Math.cos(a), z: Math.sin(a) };
  }

  _nearestPickup() {
    let best = null, bd = Infinity;
    for (const pu of this.game.pickups) {
      const d = dist(this.player.pos, pu.pos);
      if (d < bd) { bd = d; best = pu.pos; }
    }
    return best;
  }

  _incomingThreat() {
    const me = this.player;
    for (const c of this.game.caps) {
      if (!c.lethal()) continue;
      if (c.state === 'deflect' ? c.deflector === me : c.owner === me) continue;
      const vx = c.vel.x, vz = c.vel.z;
      const sp = Math.hypot(vx, vz);
      if (sp < 2) continue;
      const rx = me.pos.x - c.pos.x, rz = me.pos.z - c.pos.z;
      const t = (rx * vx + rz * vz) / (sp * sp);
      if (t < 0 || t > 0.6) continue;
      const cx = c.pos.x + vx * t - me.pos.x, cz = c.pos.z + vz * t - me.pos.z;
      if (Math.hypot(cx, cz) < 1.3 + c.radius) {
        // esquivar perpendicular a la trayectoria, hacia el lado donde ya estoy
        let px = -vz / sp, pz = vx / sp;
        if (px * -cx + pz * -cz < 0) { px = -px; pz = -pz; }
        return { perp: { x: px, z: pz } };
      }
    }
    return null;
  }
}

function clamp01(t, max) { return Math.max(0, Math.min(max, t)); }
function dist(a, b) { return Math.hypot(a.x - b.x, a.z - b.z); }
function norm(x, z) { const d = Math.hypot(x, z); return d > 1e-6 ? { x: x / d, z: z / d } : { x: 0, z: 0 }; }
