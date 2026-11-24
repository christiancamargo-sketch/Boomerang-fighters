// ─────────────────────────────────────────────────────────────
// BOTS
// Un "mando virtual" que decide igual que un humano: moverse,
// apuntar con predicción, lanzar, esquivar con dash, recoger
// power-ups y recuperar su gorra. La dificultad cambia el tiempo
// de reacción, el error de puntería y qué tanto esquiva.
// v0.2 (spec D): desvían gorras ("Te la regreso"), empujan sin gorra
// y en Lata Caliente persiguen (si traen la lata) o huyen.
// Todas sus "tiradas de dado" usan el Rng de la partida.
// v0.3 (spec V5, igual que BotBrain.cs del núcleo C#): tratan los vacíos como
// obstáculos al moverse, no hacen dash hacia un vacío, se alejan si quedan a
// menos de 0.8 m de una orilla y salen de la sombra de la caja DROP.
// También los aprovechan: sin gorra, van por el rival al que un empujón
// dejaría dentro de un vacío (de ahí salen casi todas las caídas con crédito).
// ─────────────────────────────────────────────────────────────
import { emptyPad } from './input.js';
import { DIFFICULTY, TUNING } from './config.js';

// "Empujón al vacío" (mismos valores que el núcleo C#)
const HUNT_COOLDOWN = 5;        // s de descanso después de un intento
const HUNT_MAX_TIME = 1.5;      // si en 1.5 s no salió el empujón, lo deja
const HUNT_ARMED_FACTOR = 2 / 3; // a un rival con gorra solo se le busca más de cerca
const HUNT_SIDE_MIN_DOT = -0.5; // no rodea por completo al rival para buscar el lado bueno
// lo que avanzan de verdad un dash (10 ticks) y un empujón (12 ticks)
const DASH_REACH = 10 * TUNING.player.dashSpeed * TUNING.dtFixed;
const SHOVE_REACH = TUNING.shove.dist;

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
    this.huntCd = 0;  // "empujón al vacío": descanso entre intentos
    this.huntT = 0;   // segundos que lleva el intento actual
  }

  _rand() { return this.game.rng.next(); }

  poll(dt) {
    const g = this.game;
    const me = this.player;
    const p = emptyPad();
    if (!me.alive || g.state !== 'play') { this.charging = false; return p; }

    this.retarget -= dt; this.think -= dt; this.throwCooldown -= dt; this.strafeT -= dt; this.dodgeT -= dt; this.huntCd -= dt;

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
        // V5: nunca un dash hacia un vacío (si el dash no es seguro, esquiva caminando)
        const d = this.pendingDodge.dir;
        this.pendingDodge = null;
        if (this._dashSafe(d.x, d.z)) {
          p.dashPressed = me.dashCd <= 0;
          p.mx = d.x; p.mz = d.z;
        } else {
          const m = this._avoidPits(d.x, d.z);
          p.mx = m.x; p.mz = m.z;
        }
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
      // (si hay un vacío de por medio no es "seguro": mejor seguir peleando)
      if (pu && dist(me.pos, pu) < 6 && (!tgt || dist(me.pos, pu) < dist(me.pos, tgt.pos)) && !g.arena.voidOnPath(me.pos.x, me.pos.z, pu.x, pu.z)) {
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
      // v0.3: sin gorra, aprovechar los vacíos: ir por el rival al que un empujón dejaría dentro de uno
      let hunting = false;
      const hunt = this.huntCd <= 0 && this.cfg.pitHunt > 0 && g.rules.shove && g.arena.voids.length ? this._findPitShove() : null;
      if (hunt) {
        hunting = true;
        this.huntT += dt;
        if (this.huntT > HUNT_MAX_TIME) this._endHunt(); // no salió: dejarlo por un rato
        if (hunt.inPlace) {
          if (me.shoveCd <= 0 && this._rand() < this.cfg.pitShoveChance) {
            p.meleePressed = true;
            p.ax = hunt.aim.x; p.az = hunt.aim.z; p.aim = true;
            this._endHunt();
          }
        } else {
          // acomodarse detrás del rival (gana a "huir"); si está lejos, llegar con dash
          const gx = hunt.standAt.x - me.pos.x, gz = hunt.standAt.z - me.pos.z;
          const gl = Math.hypot(gx, gz);
          if (gl > 1e-3) {
            const gd = { x: gx / gl, z: gz / gl };
            moveX += gd.x * 2.2; moveZ += gd.z * 2.2;
            if (gl > 2.4 && me.dashCd <= 0 && this._dashSafe(gd.x, gd.z)) {
              p.mx = gd.x; p.mz = gd.z; p.dashPressed = true;
              return p;
            }
          }
        }
      } else this.huntT = 0;
      // gorra en el suelo: llamarla (o ir por ella si está cerca); mientras busca el empujón no la llama
      if (cap && cap.state === 'ground' && !hunting) {
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
    return this._steer(p, moveX, moveZ);
  }

  // Dirección final de movimiento (igual que el final de BotBrain.Think en C#):
  // peligros, vacíos tratados como obstáculos (V5) y último filtro para no pisar un hueco.
  _steer(p, moveX, moveZ) {
    const g = this.game, me = this.player, a = g.arena;
    const R = TUNING.player.radius;
    // charcos que crecen: salir del círculo
    for (const pd of g.puddles) {
      if (pd.owner === me || pd.done) continue;
      const dx = me.pos.x - pd.x, dz = me.pos.z - pd.z;
      const d = Math.hypot(dx, dz);
      if (d < TUNING.puddle.radius + 0.6) { const n = d > 1e-3 ? { x: dx / d, z: dz / d } : { x: 1, z: 0 }; moveX += n.x * 2.5; moveZ += n.z * 2.5; }
    }
    // caja DROP: salir de la sombra de aviso
    if (g.drop) {
      const dx = me.pos.x - g.drop.x, dz = me.pos.z - g.drop.z;
      const d = Math.hypot(dx, dz);
      if (d < TUNING.drop.radius + 0.9) { const n = d > 1e-3 ? { x: dx / d, z: dz / d } : { x: 1, z: 0 }; moveX += n.x * 2.5; moveZ += n.z * 2.5; }
    }
    // muerte súbita: no quedarse pegado al muro que avanza
    if (g.sdWarned && Math.abs(me.pos.x) > a.w / 2 - a.inset - 2) moveX += me.pos.x > 0 ? -1 : 1;
    // V5: alejarse de la orilla de un vacío si está a menos de 0.8 m
    const away = a.voids.length ? a.voidRepulsion(me.pos.x, me.pos.z, TUNING.voids.botEdge) : { x: 0, z: 0 };
    moveX += away.x * 3; moveZ += away.z * 3;
    // evitar obstáculos: sondeo hacia adelante (isFree cuenta los vacíos como obstáculo)
    const look = 1.4;
    const n0 = norm(moveX, moveZ);
    if (!a.isFree(me.pos.x + n0.x * look, me.pos.z + n0.z * look, R * 0.9)) {
      // gira 90° hacia el lado libre
      const l = { x: -n0.z, z: n0.x };
      const leftFree = a.isFree(me.pos.x + l.x * look, me.pos.z + l.z * look, R);
      moveX = leftFree ? l.x : -l.x; moveZ = leftFree ? l.z : -l.z;
    }
    // dentro de esa franja de 0.8 m nunca se acerca más a la orilla: se quita la parte del paso que va hacia el hueco
    if (away.x * away.x + away.z * away.z > 1e-6) {
      const an = norm(away.x, away.z);
      const toward = -(moveX * an.x + moveZ * an.z);
      if (toward > 0) { moveX += an.x * toward; moveZ += an.z * toward; }
      if (moveX * moveX + moveZ * moveZ < 0.04) { moveX = an.x * 0.6; moveZ = an.z * 0.6; }
    }
    const m = norm(moveX, moveZ);
    const mag = Math.min(1, Math.hypot(moveX, moveZ));
    const out = this._avoidPits(m.x * mag, m.z * mag);
    p.mx = out.x; p.mz = out.z;
    // V5: nada de dash hacia un vacío
    if (p.dashPressed) {
      const ol = Math.hypot(out.x, out.z);
      const d = ol > 0.2 ? { x: out.x / ol, z: out.z / ol } : me.facing;
      if (!this._dashSafe(d.x, d.z)) p.dashPressed = false;
    }
    return p;
  }

  // ── V5: vacíos ───────────────────────────────────────────
  // ¿Dar unos pasos en esta dirección lo dejaría sobre un vacío (o pegado a su orilla)?
  _stepSafe(dx, dz) {
    const a = this.game.arena, me = this.player;
    // se cuenta el patinón: la velocidad actual tarda unos cuadros en girar
    const fx = me.pos.x + me.vel.x * 0.06, fz = me.pos.z + me.vel.z * 0.06;
    const pad = 0.25; // que el centro no se acerque a menos de 0.25 m de la orilla
    return !a.voidAt(fx + dx * 0.4, fz + dz * 0.4, -pad) && !a.voidAt(fx + dx * 0.8, fz + dz * 0.8, -pad);
  }

  // Último filtro del movimiento: si el paso lleva a un vacío, se gira (poco a poco, hacia
  // ambos lados) hasta encontrar una dirección segura; si no hay, se queda quieto.
  _avoidPits(mx, mz) {
    if (!this.game.arena.voids.length) return { x: mx, z: mz };
    const mag = Math.hypot(mx, mz);
    if (mag < 1e-4) return { x: mx, z: mz };
    const dx = mx / mag, dz = mz / mag;
    if (this._stepSafe(dx, dz)) return { x: mx, z: mz };
    const ang = Math.atan2(dz, dx), k = Math.max(mag, 0.8);
    for (let i = 1; i <= 6; i++) {
      const turn = i * (Math.PI / 6);
      const lx = Math.cos(ang + turn), lz = Math.sin(ang + turn);
      if (this._stepSafe(lx, lz)) return { x: lx * k, z: lz * k };
      const rx = Math.cos(ang - turn), rz = Math.sin(ang - turn);
      if (this._stepSafe(rx, rz)) return { x: rx * k, z: rz * k };
    }
    return { x: 0, z: 0 };
  }

  // Un dash (3 m) más lo que patina al terminar (≈1 m): seguro solo si en todo el trayecto no hay vacío.
  _dashSafe(dx, dz) {
    const a = this.game.arena, me = this.player;
    if (!a.voids.length) return true;
    const L = DASH_REACH + 1;
    return !a.voidOnPath(me.pos.x, me.pos.z, me.pos.x + dx * L, me.pos.z + dz * L, 0.3);
  }

  // ¿Un empujón (2.5 m) en esa dirección dejaría a "o" dentro de un vacío?
  _shoveEndsInPit(o, dx, dz) {
    return !!this.game.arena.voidAt(o.pos.x + dx * SHOVE_REACH, o.pos.z + dz * SHOVE_REACH, TUNING.voids.shrink + 0.08);
  }

  _endHunt() { this.huntCd = HUNT_COOLDOWN; this.huntT = 0; }

  // Busca un rival al que un empujón mandaría a un vacío. inPlace = ya estoy en posición
  // (empujar ya, apuntando a "aim"); si no, standAt es el punto detrás del rival (del lado
  // contrario al vacío) desde donde sí saldría el empujón.
  _findPitShove() {
    const g = this.game, me = this.player, a = g.arena;
    if (me.cap.state === 'recall') return null; // mi gorra ya viene en camino: mejor esperarla
    const PT = TUNING.player;
    const range = PT.meleeRange + PT.radius * 0.5 - 0.1;
    let bestD = Infinity, found = null;
    for (const o of g.players) {
      if (o === me || !o.alive || o.pushT > 1e-6) continue;
      const dx = o.pos.x - me.pos.x, dz = o.pos.z - me.pos.z;
      const d = Math.hypot(dx, dz);
      if (d > (o.hasCap ? this.cfg.pitHunt * HUNT_ARMED_FACTOR : this.cfg.pitHunt) || d < 1e-3) continue;
      const dir = { x: dx / d, z: dz / d };
      const straight = this._shoveEndsInPit(o, dir.x, dir.z);
      if (d <= range && straight) return { aim: dir, standAt: me.pos, inPlace: true };
      if (d >= bestD) continue;
      let u = dir, ok = straight;
      if (!ok) {
        // desde aquí no sale: buscar el lado bueno (hacia el punto más cercano del vacío o hacia su centro)
        for (const v of a.voids) {
          const cx = Math.max(v.x - v.w / 2, Math.min(o.pos.x, v.x + v.w / 2));
          const cz = Math.max(v.z - v.d / 2, Math.min(o.pos.z, v.z + v.d / 2));
          if (Math.hypot(o.pos.x - cx, o.pos.z - cz) > SHOVE_REACH) continue;
          for (let k = 0; k < 2 && !ok; k++) {
            const c = norm((k === 0 ? cx : v.x) - o.pos.x, (k === 0 ? cz : v.z) - o.pos.z);
            if (c.x * c.x + c.z * c.z < 0.5 || c.x * dir.x + c.z * dir.z < HUNT_SIDE_MIN_DOT || !this._shoveEndsInPit(o, c.x, c.z)) continue;
            u = c; ok = true;
          }
          if (ok) break;
        }
      }
      if (!ok) continue;
      const spot = { x: o.pos.x - u.x * 1.8, z: o.pos.z - u.z * 1.8 };
      // el punto de apoyo debe ser pisable y no estar pegado a otro vacío
      if (!a.isFree(spot.x, spot.z, PT.radius * 0.9) || a.voidNear(spot.x, spot.z, 0.4)) continue;
      found = { aim: dir, standAt: spot, inPlace: false }; bestD = d;
    }
    return found;
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
    return this._steer(p, moveX, moveZ);
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
