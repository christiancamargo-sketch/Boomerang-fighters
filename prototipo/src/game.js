// ─────────────────────────────────────────────────────────────
// NÚCLEO DEL JUEGO
// Rondas de "último en pie", gorras que van y regresan, golpe,
// dash, power-ups de tinta, peligros, bots y "game feel"
// (hit stop, cámara lenta, temblor, partículas, manchas).
// La lógica corre a 60 pasos fijos por segundo: es la misma idea
// que usaremos en Unity para que el online sea determinista.
// ─────────────────────────────────────────────────────────────
import * as THREE from 'three';
import { TUNING, PLAYER_COLORS, POWERUPS } from './config.js';
import { Arena, ARENAS } from './arena.js';
import { Toy, buildCap, makeBlobShadow, toon } from './characters.js';
import { sfx } from './audio.js';
import { labelTexture } from './textures.js';
import { BotController } from './ai.js';
import { emptyPad } from './input.js';

const T = TUNING;
const lerp = (a, b, t) => a + (b - a) * t;
const clamp = (v, a, b) => Math.max(a, Math.min(b, v));
const norm = (x, z) => { const d = Math.hypot(x, z); return d > 1e-6 ? { x: x / d, z: z / d } : { x: 0, z: 0 }; };
const angleTo = (from, to) => { let d = to - from; while (d > Math.PI) d -= Math.PI * 2; while (d < -Math.PI) d += Math.PI * 2; return d; };

// ── Gorra (proyectil) ────────────────────────────────────────
class Cap {
  constructor(game, owner, { ghost = false } = {}) {
    this.game = game;
    this.owner = owner;
    this.ghost = ghost;
    this.state = 'held';
    this.pos = { x: 0, z: 0 };
    this.vel = { x: 0, z: 0 };
    this.y = T.cap.flyHeight;
    this.t = 0;
    this.radius = T.cap.radius;
    this.power = null;
    this.mesh = buildCap(owner.color.hex);
    this.mesh.visible = false;
    this.shadow = makeBlobShadow(0.9);
    this.shadow.visible = false;
    if (ghost) {
      this.mesh.traverse((o) => { if (o.material && o.material.transparent !== undefined && o.material.color) { o.material = o.material.clone(); o.material.transparent = true; o.material.opacity = 0.55; } });
    }
    game.scene.add(this.mesh, this.shadow);
    this.trailT = 0;
    this.dead = false;
    this.fade = 0;
  }

  lethal() { return this.state === 'out' || this.state === 'return' || this.state === 'recall'; }

  launch(x, z, vx, vz, outTime, power) {
    this.state = 'out';
    this.pos.x = x; this.pos.z = z;
    this.vel.x = vx; this.vel.z = vz;
    this.v0 = Math.hypot(vx, vz);
    this.outTime = outTime;
    this.t = 0;
    this.power = power;
    this.radius = T.cap.radius * (power === 'sticker' ? 2 : 1);
    this.mesh.scale.setScalar(power === 'sticker' ? 2 : 1);
    this.mesh.visible = true;
    this.shadow.visible = true;
    this.y = T.cap.flyHeight;
    this.fade = 0;
    this.mesh.traverse((o) => { if (o.material && this.ghost) o.material.opacity = 0.55; });
  }

  startReturn() {
    if (this.state !== 'out') return;
    this.state = 'return';
    this.retSpeed = Math.max(6, Math.hypot(this.vel.x, this.vel.z));
  }

  recall() {
    this.state = 'recall';
    this.t = 0;
    const d = norm(this.owner.pos.x - this.pos.x, this.owner.pos.z - this.pos.z);
    this.vel.x = d.x * 4; this.vel.z = d.z * 4;
    sfx.recall();
  }

  drop(bvx = 0, bvz = 0) {
    if (this.ghost) { this.kill(); return; }
    this.state = 'ground';
    this.vel.x = bvx; this.vel.z = bvz;
    this.t = 0;
  }

  hold() {
    this.state = 'held';
    this.mesh.visible = false;
    this.shadow.visible = false;
    this.power = null;
  }

  kill() {
    this.dead = true;
    this.game.scene.remove(this.mesh, this.shadow);
    this.game.puff(this.pos.x, this.y, this.pos.z, this.owner.color.hex, 6);
  }

  update(dt) {
    const g = this.game;
    if (this.state === 'held' || this.dead) return;
    this.t += dt;

    if (this.state === 'out') {
      const k = clamp(1 - this.t / this.outTime, 0, 1);
      const sp = this.v0 * (0.3 + 0.7 * k);
      let d = norm(this.vel.x, this.vel.z);
      if (this.power === 'tatuaje') {
        const tgt = g.nearestEnemy(this.owner, this.pos, 11);
        if (tgt) {
          const want = Math.atan2(tgt.pos.z - this.pos.z, tgt.pos.x - this.pos.x);
          const cur = Math.atan2(d.z, d.x);
          const a = cur + clamp(angleTo(cur, want), -4.5 * dt, 4.5 * dt);
          d = { x: Math.cos(a), z: Math.sin(a) };
        }
      }
      this.vel.x = d.x * sp; this.vel.z = d.z * sp;
      this._move(dt);
      if (this.state === 'out' && this.t >= this.outTime) {
        if (this.ghost) { this.kill(); return; }
        this.startReturn();
      }
    } else if (this.state === 'return') {
      if (!this.owner.alive) { this.drop(this.vel.x * 0.3, this.vel.z * 0.3); return; }
      this.retSpeed = Math.min(T.cap.returnSpeed, this.retSpeed + T.cap.returnAccel * dt);
      const want = Math.atan2(this.owner.pos.z - this.pos.z, this.owner.pos.x - this.pos.x);
      const cur = Math.atan2(this.vel.z, this.vel.x);
      const a = cur + clamp(angleTo(cur, want), -T.cap.turnRate * dt, T.cap.turnRate * dt);
      this.vel.x = Math.cos(a) * this.retSpeed; this.vel.z = Math.sin(a) * this.retSpeed;
      this._move(dt);
    } else if (this.state === 'recall') {
      if (!this.owner.alive) { this.drop(); return; }
      // al llamarla, la gorra se eleva y pasa por encima de los obstáculos
      const sp = Math.min(T.cap.recallSpeed, 4 + this.t * 40);
      const d = norm(this.owner.pos.x - this.pos.x, this.owner.pos.z - this.pos.z);
      this.vel.x = d.x * sp; this.vel.z = d.z * sp;
      this.pos.x += this.vel.x * dt; this.pos.z += this.vel.z * dt;
      this.y = lerp(this.y, 1.6, dt * 8);
    } else if (this.state === 'ground') {
      const f = Math.exp(-T.cap.groundFriction * dt);
      this.vel.x *= f; this.vel.z *= f;
      this.pos.x += this.vel.x * dt; this.pos.z += this.vel.z * dt;
      g.arena.resolveCircle(this.pos, this.radius);
      this.y = lerp(this.y, 0.14, dt * 10);
      if (!this.owner.alive) {
        this.fade += dt;
        if (this.fade > 1.2) { this.kill(); return; }
      }
    }

    // rastro de aerosol (power-up Grafiti)
    if (this.power === 'grafiti' && this.lethal()) {
      this.trailT -= dt;
      if (this.trailT <= 0) {
        this.trailT = 0.035;
        g.arena.spray(this.pos.x, this.pos.z, '#00e676');
        g.paintPatches.push({ x: this.pos.x, z: this.pos.z, r: 0.65, owner: this.owner, life: 4.5 });
      }
    }

    // visual
    this.mesh.position.set(this.pos.x, this.y, this.pos.z);
    const spinning = this.lethal() ? T.cap.spin : Math.max(0, T.cap.spin * (1 - this.t * 2));
    this.mesh.rotation.y += spinning * dt;
    this.mesh.rotation.x = this.lethal() ? 0.25 : lerp(this.mesh.rotation.x, 0, dt * 6);
    this.shadow.position.set(this.pos.x, 0.025, this.pos.z);
    if (this.lethal() && Math.random() < 0.6) g.trail(this.pos.x, this.y, this.pos.z, this.owner.color.hex);
  }

  _move(dt) {
    const g = this.game;
    this.pos.x += this.vel.x * dt; this.pos.z += this.vel.z * dt;
    const hit = g.arena.capCollision(this.pos, this.radius);
    if (!hit) return;
    this.pos.x = hit.x; this.pos.z = hit.z;
    g.sparks(this.pos.x, this.y, this.pos.z, 0xffffff, 5);
    if (this.power === 'tinta') { g.explode(this); return; }
    if (this.ghost) { this.kill(); return; }
    if (this.state === 'out') {
      // rebote en la pared: tiros de banda
      const dot = this.vel.x * hit.nx + this.vel.z * hit.nz;
      this.vel.x -= 2 * dot * hit.nx; this.vel.z -= 2 * dot * hit.nz;
      this.state = 'return';
      this.retSpeed = Math.hypot(this.vel.x, this.vel.z) * 0.8;
      sfx.clank();
    } else {
      // de regreso, chocar con algo alto la tumba al piso
      this.drop(hit.nx * 3, hit.nz * 3);
      sfx.clank();
    }
  }
}

// ── Jugador ──────────────────────────────────────────────────
class Player {
  constructor(game, idx, slot) {
    this.game = game;
    this.idx = idx;
    this.slot = slot;
    this.name = slot.name;
    this.color = PLAYER_COLORS[idx % PLAYER_COLORS.length];
    this.isBot = slot.type === 'bot';
    this.controller = slot.controller;
    this.toy = new Toy({ color: this.color.hex, speciesIndex: slot.species ?? idx, shirt: slot.shirt || 'basica-blanca' });
    if (slot.capColor != null) this.toy.setCapColor(slot.capColor);
    this.toy.root.scale.setScalar(1.15); // un poco más grandes para leerse bien en celular
    game.scene.add(this.toy.root, this.toy.aim);
    this.cap = new Cap(game, this);
    this.score = 0; this.kos = 0; this.roundWins = 0;
    this.pad = emptyPad();
    this.reset({ x: 0, z: 0 });
  }

  reset(spawn) {
    this.pos = { x: spawn.x, z: spawn.z };
    this.vel = { x: 0, z: 0 };
    const c = norm(-spawn.x, -spawn.z);
    this.facing = { x: c.x || 1, z: c.z };
    this.aimDir = { ...this.facing };
    this.alive = true;
    this.hasCap = true;
    this.cap.hold();
    this.dashT = 0; this.dashCd = 0; this.dashDir = { x: 0, z: 0 };
    this.meleeT = 0; this.meleeCd = 0; this.meleeHits = null;
    this.charging = false; this.chargeT = 0;
    this.power = null; this.shield = false; this.slowT = 0;
    this.aiming = false;
    this.toy.root.visible = true;
    this.toy.setPower(null);
    this.toy.root.position.set(this.pos.x, 0, this.pos.z);
    this.toy.root.rotation.y = Math.atan2(this.facing.x, this.facing.z);
    this.spawnInvuln = 0.6;
  }
}

// ── Juego ────────────────────────────────────────────────────
export class Game {
  constructor(canvas, hooks = {}) {
    this.canvas = canvas;
    this.hooks = hooks;
    this.renderer = new THREE.WebGLRenderer({ canvas, antialias: true, powerPreference: 'high-performance' });
    this.renderer.setPixelRatio(Math.min(window.devicePixelRatio || 1, 1.75));
    this.renderer.outputColorSpace = THREE.SRGBColorSpace;
    this.scene = new THREE.Scene();
    this.camera = new THREE.PerspectiveCamera(38, 1, 0.1, 200);
    this.hemi = new THREE.HemisphereLight(0xffffff, 0x444466, 1.1);
    this.sun = new THREE.DirectionalLight(0xffffff, 1.5);
    this.sun.position.set(6, 14, 8);
    this.scene.add(this.hemi, this.sun);

    this.players = [];
    this.caps = [];
    this.pickups = [];
    this.particles = [];
    this.debris = [];
    this.paintPatches = [];
    this.floaters = [];
    this.state = 'idle';
    this.timeScale = 1;
    this.debugSpeed = 1;
    this.hitStop = 0;
    this.slowMo = 0;
    this.shake = 0;
    this.acc = 0;
    this.last = performance.now();
    this.camTarget = new THREE.Vector3();
    this.camBase = new THREE.Vector3();
    this.mode = 'menu';
    this._particleGeo = new THREE.SphereGeometry(1, 6, 4);
    this._particleMats = new Map();

    this.floatLayer = hooks.floatLayer || null;
    window.addEventListener('resize', () => this.resize());
    this.resize();
    this.loop = this.loop.bind(this);
    requestAnimationFrame(this.loop);
  }

  // ── Configuración de escena ────────────────────────────────
  setArena(id) {
    if (this.arena) this.arena.dispose();
    const def = ARENAS[id] || ARENAS.callejon;
    this.arena = new Arena(this.scene, def);
    this.scene.background = new THREE.Color(def.sky);
    this.scene.fog = new THREE.Fog(def.fog, 30, 70);
    if (id === 'azotea') { this.hemi.color.setHex(0xffe0c0); this.hemi.groundColor.setHex(0x8a4b3a); this.sun.color.setHex(0xffd2a1); }
    else if (id === 'museo') { this.hemi.color.setHex(0xe8f0ff); this.hemi.groundColor.setHex(0x30344a); this.sun.color.setHex(0xffffff); }
    else { this.hemi.color.setHex(0xd8c8ff); this.hemi.groundColor.setHex(0x3a2e4a); this.sun.color.setHex(0xfff0e0); }
    this.resize();
  }

  clearEntities() {
    for (const p of this.players) { this.scene.remove(p.toy.root, p.toy.aim); p.cap.kill(); }
    for (const c of this.caps) if (!c.dead) c.kill();
    for (const pu of this.pickups) this.scene.remove(pu.group);
    for (const d of this.debris) this.scene.remove(d.obj);
    for (const pa of this.particles) this.scene.remove(pa.mesh);
    this.players = []; this.caps = []; this.pickups = []; this.debris = []; this.particles = []; this.paintPatches = [];
    for (const f of this.floaters) f.el.remove();
    this.floaters = [];
    if (this.showcase) { this.scene.remove(this.showcase.root); this.showcase = null; }
  }

  // Escena del menú: tu muñequito girando en el callejón
  showMenu(outfit) {
    this.clearEntities();
    this.mode = 'menu';
    this.state = 'idle';
    this.setArena(outfit.arena || 'callejon');
    this.showcase = new Toy({ color: PLAYER_COLORS[0].hex, speciesIndex: outfit.species ?? 0, shirt: outfit.shirt });
    if (outfit.capColor != null) this.showcase.setCapColor(outfit.capColor);
    this.showcase.root.scale.setScalar(1.6);
    this.scene.add(this.showcase.root);
    this.resize();
  }

  updateShowcase(outfit) {
    if (!this.showcase) return;
    this.scene.remove(this.showcase.root);
    this.showcase = new Toy({ color: PLAYER_COLORS[0].hex, speciesIndex: outfit.species ?? 0, shirt: outfit.shirt });
    if (outfit.capColor != null) this.showcase.setCapColor(outfit.capColor);
    this.showcase.root.scale.setScalar(1.6);
    this.showcase.triggerSquash(1);
    this.scene.add(this.showcase.root);
  }

  // slots: [{type:'human'|'bot', name, controller?, difficulty?, shirt?, species?, capColor?}]
  startMatch({ slots, arenaId, pointsToWin = 5 }) {
    this.clearEntities();
    this.mode = 'match';
    const ids = Object.keys(ARENAS);
    const chosen = arenaId === 'aleatorio' ? ids[Math.floor(Math.random() * ids.length)] : arenaId;
    this.setArena(chosen);
    this.pointsToWin = pointsToWin;
    this.round = 0;
    this.players = slots.map((s, i) => new Player(this, i, s));
    for (const p of this.players) {
      if (p.isBot) p.controller = new BotController(this, p, p.slot.difficulty);
      if (p.controller && p.controller.useMouse) p.controller.aimFromMouse = (pad) => this._mouseAim(p, pad);
    }
    this.hooks.onMatchStart?.(this.players, this.arena.def);
    this.nextRound();
  }

  nextRound() {
    this.round++;
    // limpiar gorras fantasma, power-ups y charcos (las manchas del piso se quedan: el mural crece)
    for (const c of this.caps) if (c.ghost && !c.dead) c.kill();
    this.caps = this.caps.filter((c) => !c.dead && !c.ghost);
    for (const pu of this.pickups) this.scene.remove(pu.group);
    this.pickups = [];
    this.paintPatches = [];
    const spawns = [...this.arena.def.spawns].sort(() => Math.random() - 0.5);
    this.players.forEach((p, i) => {
      if (p.cap.dead) { p.cap = new Cap(this, p); }
      const s = spawns[i % spawns.length];
      p.reset({ x: s[0], z: s[1] });
      if (!this.caps.includes(p.cap)) this.caps.push(p.cap);
    });
    this.pickupTimer = T.pickups.firstSpawn;
    this.state = 'countdown';
    this.countdown = T.round.countdown;
    this._lastCount = null;
    this.roundEndT = 0;
    this.slowMo = 0;
    this.roundStartSim = this.simTime || 0;
    this.hooks.onRoundStart?.(this.round, this.players);
  }

  // ── Bucle principal ────────────────────────────────────────
  loop(now) {
    requestAnimationFrame(this.loop);
    let dt = Math.min(0.05, (now - this.last) / 1000);
    this.last = now;
    dt *= this.debugSpeed;

    if (this.mode === 'menu') {
      this._menuTick(dt);
    } else if (this.mode === 'match') {
      // leer controles una vez por cuadro
      for (const p of this.players) p.pad = p.controller ? p.controller.poll(dt) : emptyPad();
      this.hooks.endInputFrame?.();

      if (this.hitStop > 0) {
        this.hitStop -= dt;
      } else {
        let scale = 1;
        if (this.slowMo > 0) { this.slowMo -= dt; scale = T.round.slowMoScale; }
        this.acc += dt * scale;
        let first = true;
        let steps = 0;
        const maxSteps = 5 * Math.max(1, Math.ceil(this.debugSpeed));
        while (this.acc >= T.dtFixed && steps < maxSteps) {
          this.step(T.dtFixed, first);
          first = false;
          this.acc -= T.dtFixed;
          steps++;
        }
        if (steps === maxSteps) this.acc = 0;
      }
      this._visuals(dt);
    }
    this._camera(dt);
    this._floaters();
    this.renderer.render(this.scene, this.camera);
  }

  _menuTick(dt) {
    if (this.showcase) {
      // en pantallas anchas el muñequito se recorre a la derecha para no quedar bajo el menú
      const wide = this.camera.aspect > 1.15;
      this.showcase.root.position.set(wide ? Math.min(3.2, 1.2 + this.camera.aspect) : 0, 0, wide ? 3 : 1.5);
      this.showcase.root.rotation.y += dt * 0.6;
      this.showcase.update(dt, { speed01: 0, hasCap: true });
    }
    if (this.arena) this.arena.animate(dt);
  }

  step(dt, consumeInput) {
    const arena = this.arena;
    this.simTime = (this.simTime || 0) + dt;
    arena.updateHazards(dt, () => sfx.beep(false));

    if (this.state === 'countdown') {
      this.countdown -= dt;
      const n = Math.ceil(this.countdown / (T.round.countdown / 3));
      if (n !== this._lastCount && n > 0) {
        this._lastCount = n;
        sfx.beep(false);
        this.hooks.onCountdown?.(String(n));
      }
      if (this.countdown <= 0) {
        this.state = 'play';
        sfx.beep(true);
        this.hooks.onCountdown?.('¡YA!');
      }
    }

    const playing = this.state === 'play' || this.state === 'roundEnd';
    for (const p of this.players) {
      const pad = consumeInput ? p.pad : { ...p.pad, throwPressed: false, throwReleased: false, meleePressed: false, dashPressed: false };
      if (p.alive) this._updatePlayer(p, pad, dt, playing && this.state === 'play');
    }
    this._separatePlayers();

    for (const c of this.caps) c.update(dt);
    this.caps = this.caps.filter((c) => !c.dead);
    if (playing) {
      this._capHits();
      this._meleeHits();
      this._pickups(dt);
      this._hazards();
    }
    // charcos de pintura (lentitud)
    for (const pp of this.paintPatches) pp.life -= dt;
    this.paintPatches = this.paintPatches.filter((pp) => pp.life > 0);

    if (this.state === 'roundEnd') {
      this.roundEndT -= dt;
      if (this.roundEndT <= 0) this._finishRound();
    }
  }

  _updatePlayer(p, pad, dt, canAct) {
    p.dashCd -= dt; p.meleeCd -= dt; p.slowT -= dt; p.spawnInvuln -= dt;
    if (p.meleeT > 0) p.meleeT -= dt;

    // dirección de movimiento y de apuntado
    let mx = pad.mx, mz = pad.mz;
    const mlen = Math.hypot(mx, mz);
    if (mlen > 1) { mx /= mlen; mz /= mlen; }
    if (pad.aim) {
      const a = norm(pad.ax, pad.az);
      p.aimDir = a; p.aiming = true;
    } else {
      p.aiming = false;
      if (mlen > 0.2) p.aimDir = norm(mx, mz);
    }
    if (mlen > 0.2 || pad.aim) p.facing = { ...p.aimDir };

    if (!canAct) { mx = 0; mz = 0; }

    // dash
    if (canAct && pad.dashPressed && p.dashCd <= 0) {
      p.dashT = T.player.dashTime;
      p.dashCd = T.player.dashCooldown;
      p.dashDir = mlen > 0.2 ? norm(mx, mz) : { ...p.facing };
      p.toy.triggerSquash(0.6);
      sfx.dash();
      this.puff(p.pos.x, 0.3, p.pos.z, 0xffffff, 6);
    }

    // lanzar / regresar
    if (canAct && pad.throwPressed) {
      if (p.hasCap) { p.charging = true; p.chargeT = 0; }
      else if (p.cap.state === 'ground') p.cap.recall();
      else if (p.cap.state === 'out') p.cap.startReturn();
    }
    if (p.charging) {
      p.chargeT += dt;
      if (!canAct) p.charging = false;
      else if (pad.throwReleased || !pad.throwHeld) {
        const charge = p.chargeT < T.cap.tapThreshold ? 0.3 : clamp(p.chargeT / T.cap.chargeTime, 0, 1);
        this._throw(p, charge, pad);
        p.charging = false;
      }
    }

    // golpe
    if (canAct && pad.meleePressed && p.hasCap && p.meleeCd <= 0 && !p.charging) {
      if (pad.aim) p.facing = norm(pad.ax, pad.az);
      else { const t = this._assistTarget(p, p.facing, 0.9, T.player.meleeRange + 1); if (t) p.facing = norm(t.pos.x - p.pos.x, t.pos.z - p.pos.z); }
      p.meleeT = T.player.meleeTime;
      p.meleeCd = T.player.meleeCooldown;
      p.meleeHits = new Set();
      p.toy.triggerMelee();
      sfx.melee();
    }

    // movimiento
    let vx, vz;
    if (p.dashT > 0) {
      p.dashT -= dt;
      vx = p.dashDir.x * T.player.dashSpeed; vz = p.dashDir.z * T.player.dashSpeed;
      if (Math.random() < 0.7) this.trail(p.pos.x, 0.5, p.pos.z, p.color.hex);
    } else {
      let speed = T.player.speed;
      if (p.charging) speed *= T.player.chargeMoveMult;
      if (p.slowT > 0) speed *= T.player.slowMult;
      const tx = mx * speed, tz = mz * speed;
      const k = 1 - Math.exp(-T.player.accel * dt / speed);
      vx = lerp(p.vel.x, tx, k); vz = lerp(p.vel.z, tz, k);
    }
    p.vel.x = vx; p.vel.z = vz;
    p.pos.x += vx * dt; p.pos.z += vz * dt;
    this.arena.resolveCircle(p.pos, T.player.radius);

    // pintura de grafiti enemiga: te alenta
    for (const pp of this.paintPatches) {
      if (pp.owner !== p && Math.hypot(pp.x - p.pos.x, pp.z - p.pos.z) < pp.r) { p.slowT = 0.25; break; }
    }

    // recoger tu gorra del suelo caminando sobre ella
    if (!p.hasCap && p.cap.state === 'ground' && Math.hypot(p.cap.pos.x - p.pos.x, p.cap.pos.z - p.pos.z) < 1.0) this._catch(p);
  }

  _throw(p, charge, pad) {
    let dir = { ...p.aimDir };
    // ayuda de apuntado si no apuntó a mano (táctil / teclado)
    if (!pad.aim || pad.wantsAssist) {
      const t = this._assistTarget(p, dir, T.cap.aimAssistAngle, 17);
      if (t) {
        const d = Math.hypot(t.pos.x - p.pos.x, t.pos.z - p.pos.z);
        const sp = lerp(T.cap.speedMin, T.cap.speedMax, charge);
        const lead = (d / sp) * 0.5;
        dir = norm(t.pos.x + t.vel.x * lead - p.pos.x, t.pos.z + t.vel.z * lead - p.pos.z);
      }
    }
    const sp = lerp(T.cap.speedMin, T.cap.speedMax, charge);
    const out = lerp(T.cap.outTimeMin, T.cap.outTimeMax, charge);
    const off = T.player.radius + T.cap.radius * 0.6;
    p.cap.launch(p.pos.x + dir.x * off, p.pos.z + dir.z * off, dir.x * sp, dir.z * sp, out, p.power);
    if (!this.caps.includes(p.cap)) this.caps.push(p.cap);
    p.hasCap = false;
    p.facing = dir;
    p.toy.triggerThrow();
    sfx.throw(charge);
    if (p.power === 'serigrafia') {
      for (const s of [-0.3, 0.3]) {
        const a = Math.atan2(dir.z, dir.x) + s;
        const g = new Cap(this, p, { ghost: true });
        g.launch(p.pos.x + Math.cos(a) * off, p.pos.z + Math.sin(a) * off, Math.cos(a) * sp, Math.sin(a) * sp, out, null);
        this.caps.push(g);
      }
    }
  }

  _assistTarget(p, dir, maxAngle, maxDist) {
    let best = null, bestScore = Infinity;
    const da = Math.atan2(dir.z, dir.x);
    for (const o of this.players) {
      if (o === p || !o.alive) continue;
      const dx = o.pos.x - p.pos.x, dz = o.pos.z - p.pos.z;
      const d = Math.hypot(dx, dz);
      if (d > maxDist) continue;
      const ang = Math.abs(angleTo(da, Math.atan2(dz, dx)));
      if (ang > maxAngle) continue;
      if (!this.arena.lineOfSight(p.pos.x, p.pos.z, o.pos.x, o.pos.z)) continue;
      const score = ang * 6 + d * 0.15;
      if (score < bestScore) { bestScore = score; best = o; }
    }
    return best;
  }

  nearestEnemy(owner, pos, maxDist) {
    let best = null, bd = maxDist;
    for (const o of this.players) {
      if (o === owner || !o.alive) continue;
      const d = Math.hypot(o.pos.x - pos.x, o.pos.z - pos.z);
      if (d < bd) { bd = d; best = o; }
    }
    return best;
  }

  _separatePlayers() {
    const r2 = T.player.radius * 2;
    for (let i = 0; i < this.players.length; i++) {
      const a = this.players[i]; if (!a.alive) continue;
      for (let j = i + 1; j < this.players.length; j++) {
        const b = this.players[j]; if (!b.alive) continue;
        const dx = b.pos.x - a.pos.x, dz = b.pos.z - a.pos.z;
        const d = Math.hypot(dx, dz);
        if (d < r2 && d > 1e-4) {
          const push = (r2 - d) / 2;
          a.pos.x -= (dx / d) * push; a.pos.z -= (dz / d) * push;
          b.pos.x += (dx / d) * push; b.pos.z += (dz / d) * push;
        }
      }
    }
  }

  _catch(p) {
    p.hasCap = true;
    p.cap.hold();
    p.toy.triggerSquash(0.5);
    sfx.catch();
  }

  _capHits() {
    // gorra vs jugador
    for (const c of this.caps) {
      if (c.dead) continue;
      const owner = c.owner;
      // atrapar la propia
      if (!c.ghost && owner.alive) {
        const canCatch = c.state === 'return' || c.state === 'recall' || (c.state === 'out' && c.t > T.cap.earlyCatchDelay);
        if (canCatch && Math.hypot(c.pos.x - owner.pos.x, c.pos.z - owner.pos.z) < T.cap.catchRadius + (c.radius - T.cap.radius)) {
          this._catch(owner);
          continue;
        }
      }
      if (!c.lethal()) continue;
      for (const v of this.players) {
        if (v === owner || !v.alive || v.spawnInvuln > 0) continue;
        if (Math.hypot(c.pos.x - v.pos.x, c.pos.z - v.pos.z) < c.radius + T.player.radius * 0.85) {
          if (c.power === 'tinta') { this.explode(c); break; }
          if (v.shield) {
            v.shield = false;
            sfx.shieldBreak();
            this.float(v.pos.x, 2.2, v.pos.z, '¡BLOQUEADO!', POWERUPS.bordado.css);
            const n = norm(c.pos.x - v.pos.x, c.pos.z - v.pos.z);
            c.drop(n.x * 6, n.z * 6);
            this.shake = Math.max(this.shake, T.feel.shakeClank);
            break;
          }
          this.kill(v, owner, c.ghost ? 'triple' : 'gorra');
        }
      }
    }
    // gorra vs gorra: ¡CLANK! ambas caen
    const lethal = this.caps.filter((c) => !c.dead && c.lethal());
    for (let i = 0; i < lethal.length; i++) {
      for (let j = i + 1; j < lethal.length; j++) {
        const a = lethal[i], b = lethal[j];
        if (a.owner === b.owner || !a.lethal() || !b.lethal()) continue;
        if (Math.hypot(a.pos.x - b.pos.x, a.pos.z - b.pos.z) < a.radius + b.radius) {
          const n = norm(a.pos.x - b.pos.x, a.pos.z - b.pos.z);
          a.drop(n.x * 7, n.z * 7); b.drop(-n.x * 7, -n.z * 7);
          sfx.clank();
          this.sparks((a.pos.x + b.pos.x) / 2, T.cap.flyHeight, (a.pos.z + b.pos.z) / 2, 0xfff59d, 14);
          this.float((a.pos.x + b.pos.x) / 2, 2.2, (a.pos.z + b.pos.z) / 2, '¡CLANK!', '#fff59d');
          this.shake = Math.max(this.shake, T.feel.shakeClank);
        }
      }
    }
  }

  _meleeHits() {
    for (const p of this.players) {
      if (!p.alive || p.meleeT <= T.player.meleeTime - T.player.meleeActive) continue;
      const fa = Math.atan2(p.facing.z, p.facing.x);
      for (const v of this.players) {
        if (v === p || !v.alive || p.meleeHits.has(v) || v.spawnInvuln > 0) continue;
        const dx = v.pos.x - p.pos.x, dz = v.pos.z - p.pos.z;
        const d = Math.hypot(dx, dz);
        if (d > T.player.meleeRange + T.player.radius * 0.5) continue;
        if (Math.abs(angleTo(fa, Math.atan2(dz, dx))) > T.player.meleeArc / 2) continue;
        p.meleeHits.add(v);
        if (v.shield) {
          v.shield = false; sfx.shieldBreak();
          this.float(v.pos.x, 2.2, v.pos.z, '¡BLOQUEADO!', POWERUPS.bordado.css);
          continue;
        }
        this.kill(v, p, 'golpe');
      }
      // desviar gorras enemigas con el golpe
      for (const c of this.caps) {
        if (c.owner === p || !c.lethal()) continue;
        const dx = c.pos.x - p.pos.x, dz = c.pos.z - p.pos.z;
        const d = Math.hypot(dx, dz);
        if (d > T.player.meleeRange + 0.5) continue;
        if (Math.abs(angleTo(fa, Math.atan2(dz, dx))) > T.player.meleeArc / 2 + 0.3) continue;
        c.drop(p.facing.x * 9, p.facing.z * 9);
        sfx.clank();
        this.sparks(c.pos.x, c.y, c.pos.z, 0xfff59d, 12);
        this.float(c.pos.x, 2.2, c.pos.z, '¡DESVÍO!', '#fff59d');
        this.shake = Math.max(this.shake, T.feel.shakeClank);
      }
    }
  }

  explode(c) {
    const R = 2.3;
    sfx.explode();
    this.arena.splat(c.pos.x, c.pos.z, 1.6, '#111111', 0.9);
    this.burst(c.pos.x, 0.8, c.pos.z, 0x111111, 30, 9);
    this.shake = Math.max(this.shake, T.feel.shakeKO);
    for (const v of this.players) {
      if (v === c.owner || !v.alive || v.spawnInvuln > 0) continue;
      if (Math.hypot(v.pos.x - c.pos.x, v.pos.z - c.pos.z) < R) {
        if (v.shield) { v.shield = false; sfx.shieldBreak(); continue; }
        this.kill(v, c.owner, 'tinta');
      }
    }
    c.power = null;
    c.drop();
  }

  _hazards() {
    for (const p of this.players) {
      if (p.alive && this.arena.laserHits(p.pos.x, p.pos.z, T.player.radius)) this.kill(p, null, 'laser');
    }
  }

  kill(v, killer, cause) {
    if (!v.alive) return;
    v.alive = false;
    v.charging = false;
    v.toy.root.visible = false;
    v.toy.aim.visible = false;
    if (killer && killer !== v) killer.kos++;
    // el juguete se desarma + mancha de tinta en el piso
    for (const piece of v.toy.makeDebris()) {
      this.scene.add(piece.obj);
      const a = Math.random() * Math.PI * 2;
      const away = killer ? norm(v.pos.x - killer.pos.x, v.pos.z - killer.pos.z) : { x: 0, z: 0 };
      this.debris.push({
        obj: piece.obj, r: piece.r, life: 2.6,
        vx: away.x * 5 + Math.cos(a) * 2.5, vy: 6 + Math.random() * 4, vz: away.z * 5 + Math.sin(a) * 2.5,
        rx: (Math.random() - 0.5) * 14, rz: (Math.random() - 0.5) * 14,
      });
    }
    this.arena.splat(v.pos.x, v.pos.z, 1.1, v.color.css);
    this.burst(v.pos.x, 0.9, v.pos.z, v.color.hex, 26, 8);
    sfx.ko();
    this.hitStop = T.feel.hitStop;
    this.shake = Math.max(this.shake, T.feel.shakeKO);
    const label = cause === 'laser' ? '¡ZAP!' : cause === 'golpe' ? '¡ZAZ!' : cause === 'tinta' ? '¡SPLASH!' : '¡KO!';
    this.float(v.pos.x, 2.4, v.pos.z, label, killer ? killer.color.css : '#ff1744', true);
    // su gorra cae y desaparece
    if (!v.hasCap && v.cap.state === 'ground') v.cap.fade = 0;
    v.power = null; v.shield = false;
    this.hooks.onKO?.(v, killer, cause);

    const alive = this.players.filter((p) => p.alive);
    if (this.state === 'play' && alive.length <= 1) {
      this.state = 'roundEnd';
      this.roundEndT = T.round.endDelay;
      this.slowMo = T.round.slowMoTime;
      this.roundWinner = alive[0] || null;
    }
  }

  _finishRound() {
    const w = this.roundWinner;
    // métrica de balance: duración de cada ronda (segundos de juego)
    (this.roundDurations ||= []).push(+((this.simTime || 0) - this.roundStartSim).toFixed(1));
    if (w && w.alive) { w.score++; w.roundWins++; }
    this.hooks.onRoundEnd?.(w, this.players);
    if (w && w.score >= this.pointsToWin) {
      this.state = 'matchEnd';
      sfx.win();
      this.hooks.onMatchEnd?.({ winner: w, players: this.players, rounds: this.round, arena: this.arena.def });
      return;
    }
    this.state = 'between';
    setTimeout(() => { if (this.mode === 'match' && this.state === 'between') this.nextRound(); }, 900 / this.debugSpeed);
  }

  // ── Power-ups ──────────────────────────────────────────────
  _pickups(dt) {
    if (this.state !== 'play') return;
    this.pickupTimer -= dt;
    if (this.pickupTimer <= 0 && this.pickups.length < T.pickups.max) {
      this.pickupTimer = lerp(T.pickups.every[0], T.pickups.every[1], Math.random());
      this._spawnPickup();
    }
    for (const pu of this.pickups) {
      pu.t += dt;
      pu.group.position.y = 0.55 + Math.sin(pu.t * 3) * 0.12;
      pu.can.rotation.y += dt * 2;
      pu.ring.scale.setScalar(1 + Math.sin(pu.t * 5) * 0.08);
      for (const p of this.players) {
        if (!p.alive) continue;
        if (Math.hypot(p.pos.x - pu.pos.x, p.pos.z - pu.pos.z) < 1.0) {
          this._applyPower(p, pu.type);
          pu.taken = true;
          break;
        }
      }
    }
    for (const pu of this.pickups) if (pu.taken) this.scene.remove(pu.group);
    this.pickups = this.pickups.filter((pu) => !pu.taken);
  }

  _spawnPickup() {
    const types = Object.keys(POWERUPS);
    const type = types[Math.floor(Math.random() * types.length)];
    const def = POWERUPS[type];
    let pos = this.arena.randomFreePoint(1.0);
    for (let i = 0; i < 10; i++) {
      if (this.players.every((p) => !p.alive || Math.hypot(p.pos.x - pos.x, p.pos.z - pos.z) > 2.5)) break;
      pos = this.arena.randomFreePoint(1.0);
    }
    const group = new THREE.Group();
    // lata de aerosol del color de la técnica
    const can = new THREE.Group();
    const body = new THREE.Mesh(new THREE.CylinderGeometry(0.22, 0.22, 0.6, 14), toon(def.color));
    const band = new THREE.Mesh(new THREE.CylinderGeometry(0.225, 0.225, 0.16, 14), toon(0xffffff));
    band.position.y = 0.05;
    const top = new THREE.Mesh(new THREE.SphereGeometry(0.22, 12, 8, 0, Math.PI * 2, 0, Math.PI / 2), toon(0xbdbdbd));
    top.position.y = 0.3;
    const nozzle = new THREE.Mesh(new THREE.CylinderGeometry(0.06, 0.06, 0.1, 8), toon(0x212121));
    nozzle.position.y = 0.5;
    can.add(body, band, top, nozzle);
    can.rotation.z = 0.25;
    const ring = new THREE.Mesh(new THREE.RingGeometry(0.5, 0.62, 28), new THREE.MeshBasicMaterial({ color: def.color === 0x1a1a1a ? 0x9aa0a6 : def.color, transparent: true, opacity: 0.8, depthWrite: false, side: THREE.DoubleSide }));
    ring.rotation.x = -Math.PI / 2; ring.position.y = -0.5;
    const label = new THREE.Sprite(new THREE.SpriteMaterial({ map: labelTexture(def.name.toUpperCase(), def.css), depthWrite: false, transparent: true }));
    label.scale.set(2.2, 0.55, 1); label.position.y = 0.95;
    group.add(can, ring, label);
    group.position.set(pos.x, 0.55, pos.z);
    this.scene.add(group);
    this.pickups.push({ type, pos, group, can, ring, t: 0 });
    this.puff(pos.x, 0.5, pos.z, def.color, 10);
  }

  _applyPower(p, type) {
    const def = POWERUPS[type];
    sfx.pickup();
    if (type === 'bordado') {
      p.shield = true;
    } else {
      p.power = type;
      p.toy.setPower(def.color === 0x1a1a1a ? 0x9aa0a6 : def.color);
    }
    this.float(p.pos.x, 2.6, p.pos.z, `${def.name.toUpperCase()} · ${def.effect}`, def.css);
    this.hooks.onPowerup?.(p, def);
  }

  // ── Efectos visuales ───────────────────────────────────────
  _mat(color) {
    if (!this._particleMats.has(color)) this._particleMats.set(color, new THREE.MeshBasicMaterial({ color }));
    return this._particleMats.get(color);
  }

  _spawnParticle(x, y, z, color, vx, vy, vz, size, life, gravity = 18) {
    if (this.particles.length > 260) return;
    const m = new THREE.Mesh(this._particleGeo, this._mat(color));
    m.position.set(x, y, z);
    m.scale.setScalar(size);
    this.scene.add(m);
    this.particles.push({ mesh: m, vx, vy, vz, life, max: life, size, gravity });
  }

  burst(x, y, z, color, n = 20, speed = 7) {
    for (let i = 0; i < n; i++) {
      const a = Math.random() * Math.PI * 2, e = Math.random() * 1.2;
      const s = speed * (0.4 + Math.random() * 0.8);
      this._spawnParticle(x, y, z, color, Math.cos(a) * s * Math.cos(e), Math.sin(e) * s + 2, Math.sin(a) * s * Math.cos(e), 0.08 + Math.random() * 0.12, 0.5 + Math.random() * 0.5);
    }
  }

  sparks(x, y, z, color, n = 8) {
    for (let i = 0; i < n; i++) {
      const a = Math.random() * Math.PI * 2, s = 4 + Math.random() * 6;
      this._spawnParticle(x, y, z, color, Math.cos(a) * s, 1 + Math.random() * 4, Math.sin(a) * s, 0.04 + Math.random() * 0.04, 0.25 + Math.random() * 0.2, 10);
    }
  }

  puff(x, y, z, color, n = 8) {
    for (let i = 0; i < n; i++) {
      const a = Math.random() * Math.PI * 2, s = 1 + Math.random() * 2;
      this._spawnParticle(x, y, z, color, Math.cos(a) * s, 1 + Math.random() * 1.5, Math.sin(a) * s, 0.1 + Math.random() * 0.1, 0.35 + Math.random() * 0.25, 2);
    }
  }

  trail(x, y, z, color) {
    this._spawnParticle(x + (Math.random() - 0.5) * 0.2, y, z + (Math.random() - 0.5) * 0.2, color, 0, 0.3, 0, 0.07 + Math.random() * 0.05, 0.25, 0);
  }

  float(x, y, z, text, color, big = false) {
    if (!this.floatLayer) return;
    const el = document.createElement('div');
    el.className = 'floater' + (big ? ' big' : '');
    el.textContent = text;
    el.style.color = color;
    this.floatLayer.appendChild(el);
    this.floaters.push({ el, x, y, z, life: big ? 1.1 : 1.0, max: big ? 1.1 : 1.0 });
  }

  _floaters() {
    if (!this.floaters.length) return;
    const v = new THREE.Vector3();
    const w = this.canvas.clientWidth, h = this.canvas.clientHeight;
    for (const f of this.floaters) {
      f.life -= 1 / 60;
      const k = 1 - f.life / f.max;
      v.set(f.x, f.y + k * 1.2, f.z).project(this.camera);
      f.el.style.transform = `translate(-50%,-50%) translate(${(v.x * 0.5 + 0.5) * w}px, ${(-v.y * 0.5 + 0.5) * h}px) scale(${k < 0.15 ? 0.6 + k * 3 : 1})`;
      f.el.style.opacity = String(Math.min(1, f.life * 3));
    }
    for (const f of this.floaters) if (f.life <= 0) f.el.remove();
    this.floaters = this.floaters.filter((f) => f.life > 0);
  }

  _visuals(dt) {
    this.arena.animate(dt);
    this.arena.tick(dt);
    for (const p of this.players) {
      if (!p.alive) continue;
      const tr = p.toy.root;
      tr.position.set(p.pos.x, 0, p.pos.z);
      const want = Math.atan2(p.facing.x, p.facing.z);
      tr.rotation.y += angleTo(tr.rotation.y, want) * Math.min(1, dt * 18);
      const sp = Math.hypot(p.vel.x, p.vel.z) / T.player.speed;
      p.toy.update(dt, {
        speed01: Math.min(1.5, sp), dashing: p.dashT > 0, charging: p.charging,
        charge: p.charging ? clamp(p.chargeT / T.cap.chargeTime, 0, 1) : 0,
        hasCap: p.hasCap, aimDir: p.aimDir, aiming: p.aiming && p.hasCap && !p.isBot,
        shield: p.shield, alive: p.alive,
      });
      // parpadeo de invulnerabilidad al aparecer
      tr.visible = p.spawnInvuln > 0 ? Math.floor(p.spawnInvuln * 20) % 2 === 0 : true;
    }
    // partículas
    for (const pa of this.particles) {
      pa.life -= dt;
      pa.vy -= pa.gravity * dt;
      pa.mesh.position.x += pa.vx * dt; pa.mesh.position.y += pa.vy * dt; pa.mesh.position.z += pa.vz * dt;
      if (pa.mesh.position.y < 0.05) { pa.mesh.position.y = 0.05; pa.vy *= -0.3; pa.vx *= 0.6; pa.vz *= 0.6; }
      pa.mesh.scale.setScalar(pa.size * Math.max(0, pa.life / pa.max));
    }
    for (const pa of this.particles) if (pa.life <= 0) this.scene.remove(pa.mesh);
    this.particles = this.particles.filter((pa) => pa.life > 0);
    // piezas del juguete
    for (const d of this.debris) {
      d.life -= dt;
      d.vy -= 22 * dt;
      d.obj.position.x += d.vx * dt; d.obj.position.y += d.vy * dt; d.obj.position.z += d.vz * dt;
      d.obj.rotation.x += d.rx * dt; d.obj.rotation.z += d.rz * dt;
      if (d.obj.position.y < d.r) { d.obj.position.y = d.r; d.vy = Math.abs(d.vy) * 0.45; d.vx *= 0.7; d.vz *= 0.7; d.rx *= 0.7; d.rz *= 0.7; }
      const hw = this.arena.w / 2 - d.r, hh = this.arena.h / 2 - d.r;
      if (Math.abs(d.obj.position.x) > hw) { d.vx *= -0.6; d.obj.position.x = Math.sign(d.obj.position.x) * hw; }
      if (Math.abs(d.obj.position.z) > hh) { d.vz *= -0.6; d.obj.position.z = Math.sign(d.obj.position.z) * hh; }
      if (d.life < 0.5) d.obj.scale.multiplyScalar(0.9);
    }
    for (const d of this.debris) if (d.life <= 0) this.scene.remove(d.obj);
    this.debris = this.debris.filter((d) => d.life > 0);
  }

  // ── Cámara ─────────────────────────────────────────────────
  resize() {
    const w = this.canvas.clientWidth || window.innerWidth;
    const h = this.canvas.clientHeight || window.innerHeight;
    this.renderer.setSize(w, h, false);
    this.camera.aspect = w / h;
    this.camera.updateProjectionMatrix();
    this._frame();
  }

  _frame() {
    const elev = THREE.MathUtils.degToRad(63);
    if (this.mode === 'menu' || !this.arena) {
      this.camBase.set(0, 3.4, 11.5);
      this.camTarget.set(0, 1.4, 1.5);
      return;
    }
    const vfov = THREE.MathUtils.degToRad(this.camera.fov);
    const hfov = 2 * Math.atan(Math.tan(vfov / 2) * this.camera.aspect);
    const W = this.arena.w + 1.5, H = this.arena.h + 2.2;
    const dW = (W / 2) / Math.tan(hfov / 2);
    const dH = (H / 2) * Math.sin(elev) / Math.tan(vfov / 2) + H * 0.12;
    const D = Math.max(dW, dH) * 1.02;
    this.camTarget.set(0, 0, 0.6);
    this.camBase.set(0, Math.sin(elev) * D, Math.cos(elev) * D + 0.6);
  }

  _camera(dt) {
    this._frame();
    this.shake = Math.max(0, this.shake - dt * 2.2);
    const s = this.shake * this.shake;
    let tx = this.camTarget.x, tz = this.camTarget.z;
    // ligero seguimiento al centro de la acción
    if (this.mode === 'match' && this.players.length) {
      const alive = this.players.filter((p) => p.alive);
      if (alive.length) {
        const cx = alive.reduce((a, p) => a + p.pos.x, 0) / alive.length;
        const cz = alive.reduce((a, p) => a + p.pos.z, 0) / alive.length;
        tx += cx * 0.08; tz += cz * 0.08;
      }
    }
    this.camera.position.set(
      this.camBase.x + tx + (Math.random() - 0.5) * s * 0.8,
      this.camBase.y + (Math.random() - 0.5) * s * 0.8,
      this.camBase.z + (tz - this.camTarget.z) + (Math.random() - 0.5) * s * 0.8
    );
    this.camera.lookAt(tx, this.camTarget.y, tz);
  }

  _mouseAim(p, pad) {
    const { mouse } = this.hooks;
    if (!mouse) return;
    const rect = this.canvas.getBoundingClientRect();
    const ndc = new THREE.Vector2(((mouse.x - rect.left) / rect.width) * 2 - 1, -((mouse.y - rect.top) / rect.height) * 2 + 1);
    const ray = new THREE.Raycaster();
    ray.setFromCamera(ndc, this.camera);
    const plane = new THREE.Plane(new THREE.Vector3(0, 1, 0), 0);
    const hit = new THREE.Vector3();
    if (ray.ray.intersectPlane(plane, hit)) {
      const dx = hit.x - p.pos.x, dz = hit.z - p.pos.z;
      if (Math.hypot(dx, dz) > 0.4) { pad.ax = dx; pad.az = dz; pad.aim = true; }
    }
  }

  exitToMenu() {
    this.mode = 'menu';
    this.state = 'idle';
  }
}
