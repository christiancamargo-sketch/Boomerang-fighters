// ─────────────────────────────────────────────────────────────
// NÚCLEO DEL JUEGO
// Rondas de "último en pie", gorras que van y regresan, golpe,
// empujón, dash, latas de técnica, peligros, muerte súbita, modos
// (¡Gorrazo!, Lata Caliente, La Reta, Reto del Día), bots y
// "game feel" (hit stop, cámara lenta, temblor, partículas, manchas).
// La lógica corre a 60 pasos fijos por segundo con un Rng con semilla:
// es la misma idea (y las mismas reglas de la spec v0.2) que el núcleo C#.
// Spec v0.3: vacíos (caes si tu centro queda dentro sin dash ni empujón,
// con crédito a quien te empujó/golpeó/desvió en los últimos 2 s), caja
// DROP de la Pop-up Store, latas con etiqueta INKCAP y podio con la taza.
// ─────────────────────────────────────────────────────────────
import * as THREE from 'three';
import { TUNING, PLAYER_COLORS, POWERUPS, DEFAULT_RULES, KO_LABELS } from './config.js';
import { Arena, ARENAS } from './arena.js';
import { Toy, buildCap, makeBlobShadow, toon } from './characters.js';
import { sfx } from './audio.js';
import { labelTexture, canLabelTexture } from './textures.js';
import { ESCENAS, skyColors, buildMug } from './escenografia.js';
import { BotController } from './ai.js';
import { emptyPad } from './input.js';
import { Rng } from './rng.js';

const T = TUNING;
const lerp = (a, b, t) => a + (b - a) * t;
const clamp = (v, a, b) => Math.max(a, Math.min(b, v));
const norm = (x, z) => { const d = Math.hypot(x, z); return d > 1e-6 ? { x: x / d, z: z / d } : { x: 0, z: 0 }; };
const angleTo = (from, to) => { let d = to - from; while (d > Math.PI) d -= Math.PI * 2; while (d < -Math.PI) d += Math.PI * 2; return d; };

// ── Gorra (proyectil) ────────────────────────────────────────
// Estados: held · out (ida) · return (regreso) · recall (llamada) ·
// ground (en el piso) · deflect ("Te la regreso": va hacia su dueño y lo puede noquear)
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
    this.mesh = buildCap(owner.color.hex, owner.capDesign);
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
    this.bounced = false;   // ya rebotó en pared → KO "de banda"
    this.deflector = null;  // quien la desvió ("Te la regreso")
    this.tagDone = false;   // ya pintó su Muro de Tag en este tiro
    this.markTarget = null; // Tatuaje: a quién persigue
  }

  lethal() { return this.state === 'out' || this.state === 'return' || this.state === 'recall' || this.state === 'deflect'; }

  launch(x, z, vx, vz, outTime, power) {
    this.state = 'out';
    this.pos.x = x; this.pos.z = z;
    // A5: Sticker XL va 10 % más lenta
    const k = power === 'sticker' ? T.stickerSpeedMult : 1;
    this.vel.x = vx * k; this.vel.z = vz * k;
    this.v0 = Math.hypot(this.vel.x, this.vel.z);
    this.outTime = outTime;
    this.t = 0;
    this.power = power;
    this.bounced = false; this.deflector = null; this.tagDone = false; this.markTarget = null;
    this.radius = T.cap.radius * (power === 'sticker' ? 2 : 1);
    this.mesh.scale.setScalar(power === 'sticker' ? 2 : 1);
    this.mesh.visible = true;
    this.shadow.visible = true;
    this.y = T.cap.flyHeight;
    this.fade = 0;
    this.mesh.traverse((o) => { if (o.material && this.ghost) o.material.opacity = 0.55; });
  }

  // Grafiti · Muro de Tag: al terminar la ida o al rebotar
  _paintTagWall() {
    const g = this.game;
    if (this.power !== 'grafiti' || this.tagDone || !g.rules.graffitiWall) return;
    this.tagDone = true;
    g.arena.addTagWall(this.pos.x, this.pos.z, this.vel.x, this.vel.z, this.owner.color.css, this);
    sfx.clank();
    g.puff(this.pos.x, 0.6, this.pos.z, 0xf4f1ea, 10);
    g._emit('TagWall', { owner: this.owner.idx, x: this.pos.x, z: this.pos.z });
  }

  startReturn() {
    if (this.state !== 'out') return;
    this._paintTagWall();
    this.state = 'return';
    this.retSpeed = Math.max(6, Math.hypot(this.vel.x, this.vel.z));
  }

  // A4 "Te la regreso": sale hacia su dueño a 1.2× (máx. 22 m/s) y 0.6 s puede noquearlo
  deflect(by) {
    const sp = Math.max(6, Math.min(T.deflect.maxSpeed, Math.hypot(this.vel.x, this.vel.z) * T.deflect.speedMult));
    const d = norm(this.owner.pos.x - this.pos.x, this.owner.pos.z - this.pos.z);
    this.state = 'deflect';
    this.deflector = by;
    this.t = 0;
    this.vel.x = d.x * sp; this.vel.z = d.z * sp;
    this.y = T.cap.flyHeight;
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
    this.deflector = null;
    this.vel.x = bvx; this.vel.z = bvz;
    this.t = 0;
  }

  hold() {
    this.state = 'held';
    this.mesh.visible = false;
    this.shadow.visible = false;
    this.power = null;
    this.deflector = null;
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
        const tgt = g.nearestEnemy(this.owner, this.pos, T.tracking.range);
        if (tgt) {
          if (tgt !== this.markTarget) { this.markTarget = tgt; g.onMarked(tgt, this); }
          tgt.markedT = 0.2;
          const want = Math.atan2(tgt.pos.z - this.pos.z, tgt.pos.x - this.pos.x);
          const cur = Math.atan2(d.z, d.x);
          const a = cur + clamp(angleTo(cur, want), -T.tracking.turn * dt, T.tracking.turn * dt);
          d = { x: Math.cos(a), z: Math.sin(a) };
        }
      }
      this.vel.x = d.x * sp; this.vel.z = d.z * sp;
      this._move(dt);
      if (this.state === 'out' && this.t >= this.outTime) {
        if (this.ghost) { this.kill(); return; }
        this.startReturn();
      }
    } else if (this.state === 'deflect') {
      this._move(dt);
      if (this.state === 'deflect' && this.t >= T.deflect.lethalTime) {
        // después vuelve a ser una gorra normal en regreso
        this.deflector = null;
        if (this.ghost) { this.kill(); return; }
        this.state = 'return';
        this.retSpeed = Math.hypot(this.vel.x, this.vel.z);
      }
      // rastro rayado con el color de quien desvió
      if (this.state === 'deflect' && this.deflector) {
        g.trail(this.pos.x, this.y, this.pos.z, Math.floor(this.t * 30) % 2 ? this.deflector.color.hex : 0xffffff);
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
      // V3: en el piso DENTRO de un vacío (su centro en el rectángulo completo) regresa sola
      // de inmediato, como si la llamaran; si su dueño ya no está, se va al fondo
      if (g.arena.voidAt(this.pos.x, this.pos.z, 0)) {
        g._emit('CapVoidReturn', { owner: this.owner.idx });
        if (this.owner.alive && !this.ghost) this.recall(); else this.kill();
        return;
      }
      // regreso automático: nadie se queda desarmado para siempre
      if (this.owner.alive && !this.ghost && this.t > T.cap.groundAutoReturn) { this.recall(); return; }
      if (!this.owner.alive) {
        this.fade += dt;
        if (this.fade > 1.2) { this.kill(); return; }
      }
    }

    // rastro de aerosol (respaldo de Grafiti con el interruptor Muro de Tag apagado)
    if (this.power === 'grafiti' && !g.rules.graffitiWall && this.lethal()) {
      this.trailT -= dt;
      if (this.trailT <= 0) {
        this.trailT = 0.035;
        g.arena.spray(this.pos.x, this.pos.z, this.owner.color.css); // el rastro siempre es del color del jugador
        g.paintPatches.push({ x: this.pos.x, z: this.pos.z, r: 0.65, owner: this.owner, life: 4.5 });
      }
    }

    // visual
    this.mesh.position.set(this.pos.x, this.y, this.pos.z);
    const spinning = this.lethal() ? T.cap.spin : Math.max(0, T.cap.spin * (1 - this.t * 2));
    this.mesh.rotation.y += spinning * dt;
    this.mesh.rotation.x = this.lethal() ? 0.25 : lerp(this.mesh.rotation.x, 0, dt * 6);
    this.shadow.position.set(this.pos.x, 0.025, this.pos.z);
    if (this.lethal() && this.state !== 'deflect' && Math.random() < 0.6) g.trail(this.pos.x, this.y, this.pos.z, this.owner.color.hex);
  }

  _move(dt) {
    const g = this.game;
    this.pos.x += this.vel.x * dt; this.pos.z += this.vel.z * dt;
    const hit = g.arena.capCollision(this.pos, this.radius, this);
    if (!hit) return;
    this.pos.x = hit.x; this.pos.z = hit.z;
    g.sparks(this.pos.x, this.y, this.pos.z, 0xffffff, 5);
    if (this.power === 'tinta') { g.startPuddle(this, this.pos.x, this.pos.z); return; }
    if (this.ghost) { this.kill(); return; }
    if (this.state === 'out') {
      // rebote en la pared: tiros de banda (la gorra desviada no rebota: cae, igual que C#)
      const dot = this.vel.x * hit.nx + this.vel.z * hit.nz;
      this.vel.x -= 2 * dot * hit.nx; this.vel.z -= 2 * dot * hit.nz;
      this._paintTagWall();
      this.bounced = true;
      this.deflector = null;
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
    this.capDesign = slot.capDesign || 'clasica';
    this.toy = new Toy({ color: this.color.hex, speciesIndex: slot.species ?? idx, shirt: slot.shirt || 'basica-blanca', capDesign: this.capDesign });
    if (slot.capColor != null) this.toy.setCapColor(slot.capColor);
    this.toy.root.scale.setScalar(1.15); // un poco más grandes para leerse bien en celular
    game.scene.add(this.toy.root, this.toy.aim);
    // ícono de "marcado" (Tatuaje · Trazo persiguiéndote)
    this.markSprite = new THREE.Sprite(new THREE.SpriteMaterial({ map: labelTexture('✦ MARCA ✦', '#f4f1ea'), depthWrite: false, depthTest: false, transparent: true }));
    this.markSprite.scale.set(2.0, 0.5, 1);
    this.markSprite.visible = false;
    this.markSprite.renderOrder = 5;
    game.scene.add(this.markSprite);
    this.cap = new Cap(game, this);
    this.score = 0; this.kos = 0; this.roundWins = 0; this.streak = 0;
    this.benched = false; // La Reta: esperando en la fila
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
    this.meleeT = 0; this.meleeCd = 0; this.meleeHits = null; this.meleeShove = false;
    this.shoveCd = 0; this.pushT = 0; this.pushV = { x: 0, z: 0 };
    this.charging = false; this.chargeT = 0;
    this.power = null; this.shield = false; this.slowT = 0;
    this.markedT = 0; this.hotImmune = 0;
    this.lastHitBy = null; this.lastHitT = 0; // V2: quién lo empujó/golpeó/desvió y cuántos s le quedan al crédito (se borra cada ronda)
    this.aiming = false;
    this.toy.root.visible = true;
    this.toy.root.scale.setScalar(1.15); // por si venía de caer a un vacío (se encoge)
    this.toy.setPower(null);
    this.toy.root.position.set(this.pos.x, 0, this.pos.z);
    this.toy.root.rotation.y = Math.atan2(this.facing.x, this.facing.z);
    this.spawnInvuln = 0.6;
  }

  // La Reta: fuera de la arena mientras espera turno
  bench() {
    this.benched = true;
    this.alive = false;
    this.hasCap = true;
    this.cap.hold();
    this.toy.root.visible = false;
    this.toy.aim.visible = false;
    this.markSprite.visible = false;
    this.pos = { x: 0, z: 100 };
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
    this.puddles = [];
    this.floaters = [];
    this.fallers = [];     // muñequitos cayendo a un vacío (animación)
    this.drop = null;      // caja DROP con aviso en curso {x, z, landAt}
    this.dropN = 0;        // cuántas cajas han caído en esta ronda
    this._canMats = new Map();
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
    this.modeId = 'gorrazo';
    this.rules = { ...DEFAULT_RULES };
    this.rng = new Rng(1);
    this.eventCounts = {};
    // Partículas: UNA sola malla instanciada para todas (hasta 260): un KO ya no cuesta
    // decenas de llamadas de dibujo extra (importa para los 30 fps en celular de gama media)
    this._particleGeo = new THREE.SphereGeometry(1, 6, 4);
    this._pMax = 260;
    this._pMesh = new THREE.InstancedMesh(this._particleGeo, new THREE.MeshBasicMaterial({ color: 0xffffff }), this._pMax);
    this._pMesh.instanceMatrix.setUsage(THREE.DynamicDrawUsage);
    this._pMesh.count = 0;
    this._pMesh.frustumCulled = false;
    this._pColor = new THREE.Color();
    this._pMesh.setColorAt(0, this._pColor.setHex(0xffffff)); // crea el atributo de color por instancia
    this.scene.add(this._pMesh);
    this._puddleGeo = new THREE.CircleGeometry(1, 40);
    this._puddleRimGeo = new THREE.RingGeometry(0.94, 1, 40);

    this.floatLayer = hooks.floatLayer || null;
    window.addEventListener('resize', () => this.resize());
    this.resize();
    this.loop = this.loop.bind(this);
    requestAnimationFrame(this.loop);
  }

  // Eventos de simulación (mismos nombres que el núcleo C#): la vista y las pruebas los escuchan
  _emit(type, data = {}) {
    this.eventCounts[type] = (this.eventCounts[type] || 0) + 1;
    this.hooks.onEvent?.(type, data);
  }

  // ── Configuración de escena ────────────────────────────────
  setArena(id) {
    if (this.arena) this.arena.dispose();
    const def = ARENAS[id] || ARENAS.callejon;
    this.arena = new Arena(this.scene, def);
    this.arena.rng = this.rng;
    // cielo en degradado (se ve en el menú y el podio; en partida casi todo es escenario)
    if (this._skyTex) this._skyTex.dispose();
    const [top, bottom] = skyColors(def.id);
    const c = document.createElement('canvas'); c.width = 4; c.height = 128;
    const ctx = c.getContext('2d');
    const g = ctx.createLinearGradient(0, 0, 0, 128);
    g.addColorStop(0, top); g.addColorStop(0.75, bottom); g.addColorStop(1, bottom);
    ctx.fillStyle = g; ctx.fillRect(0, 0, 4, 128);
    this._skyTex = new THREE.CanvasTexture(c);
    this._skyTex.colorSpace = THREE.SRGBColorSpace;
    this.scene.background = this._skyTex;
    this.scene.fog = new THREE.Fog(def.fog, 30, 70);
    const L = (ESCENAS[def.id] || ESCENAS.callejon).lights;
    this.hemi.color.setHex(L.sky); this.hemi.groundColor.setHex(L.ground); this.sun.color.setHex(L.sun);
    this.drop = null; this.dropN = 0;
    this.resize();
  }

  // Avanza la simulación a mano, sin esperar al reloj (pruebas y balance):
  // los bots deciden en cada tick y las rondas se encadenan solas.
  fastForward(maxTicks = 600, stopAtMatchEnd = true) {
    let n = 0;
    for (; n < maxTicks; n++) {
      if (this.state === 'matchEnd' && stopAtMatchEnd) break;
      if (this.state === 'between') this.nextRound();
      for (const p of this.players) p.pad = p.controller ? p.controller.poll(T.dtFixed) : emptyPad();
      this.step(T.dtFixed, true);
      if (n % 6 === 5) this._fx(T.dtFixed * 6); // limpia partículas y piezas para no acumular
    }
    return n;
  }

  clearEntities() {
    for (const p of this.players) { this.scene.remove(p.toy.root, p.toy.aim, p.markSprite); p.cap.kill(); }
    for (const c of this.caps) if (!c.dead) c.kill();
    for (const pu of this.pickups) this.scene.remove(pu.group);
    for (const d of this.debris) this.scene.remove(d.obj);
    this._pMesh.count = 0;
    for (const pd of this.puddles) this.scene.remove(pd.group);
    if (this.hotMesh) { this.scene.remove(this.hotMesh); this.hotMesh = null; }
    this.players = []; this.caps = []; this.pickups = []; this.debris = []; this.particles = []; this.paintPatches = []; this.puddles = [];
    this.fallers = [];
    this.drop = null; this.dropN = 0;
    if (this.arena) this.arena.hideDrop();
    this.hot = null;
    for (const f of this.floaters) f.el.remove();
    this.floaters = [];
    if (this.showcase) { this.scene.remove(this.showcase.root); this.showcase = null; }
  }

  // Escena del menú: tu muñequito girando en la Pop-up Store INKCAP (spec v0.3 §C)
  showMenu(outfit) {
    this.clearEntities();
    this.mode = 'menu';
    this.state = 'idle';
    const id = outfit.arena || 'popup';
    // si ya está puesta y limpia, no se reconstruye (entrar y salir del vestidor no da tirones)
    if (!this.arena || this.arena.def.id !== id || this._arenaUsed) this.setArena(id);
    this._arenaUsed = false;
    this._makeShowcase(outfit);
    this.resize();
  }

  _makeShowcase(outfit) {
    this.showcase = new Toy({ color: outfit.color ?? PLAYER_COLORS[0].hex, speciesIndex: outfit.species ?? 0, shirt: outfit.shirt, capDesign: outfit.cap || 'clasica' });
    if (outfit.capColor != null) this.showcase.setCapColor(outfit.capColor);
    this.showcase.root.scale.setScalar(1.6);
    if (outfit.trophy) this.showcase.holdTrophy(buildMug(1.55));
    this.scene.add(this.showcase.root);
  }

  updateShowcase(outfit) {
    if (!this.showcase) return;
    this.scene.remove(this.showcase.root);
    this._makeShowcase(outfit);
    this.showcase.triggerSquash(1);
  }

  // Podio (spec v0.3 §C): el ganador levanta la taza INKCAP en el mismo escenario
  // de la partida, que se queda con todas sus manchas de tinta.
  showPodium(winner) {
    const slot = winner.slot || {};
    const look = { color: winner.color.hex, species: slot.species ?? winner.idx, shirt: slot.shirt || 'basica-blanca', cap: winner.capDesign, capColor: slot.capColor, trophy: true };
    this.clearEntities();
    this.mode = 'podium';
    this.state = 'idle';
    this.podiumT = 0;
    this.podiumColor = winner.color.hex;
    this.arena.setSuddenDeath(0, false);
    this.arena.clearTagWalls();
    this._makeShowcase(look);
    this.resize();
  }

  // slots: [{type:'human'|'bot', name, controller?, difficulty?, shirt?, species?, capDesign?, autopilot?}]
  // mode: 'gorrazo' | 'lataCaliente' | 'laReta' | 'retoDelDia'
  startMatch({ slots, arenaId, pointsToWin = 5, mode = 'gorrazo', rules = {}, seed = null, pickupsEvery = null, pickupsMax = null }) {
    this.clearEntities();
    this.mode = 'match';
    this.modeId = mode;
    this.rules = { ...DEFAULT_RULES, ...rules };
    this.seed = seed != null ? seed >>> 0 : ((Date.now() ^ Math.floor(performance.now() * 1000)) >>> 0);
    this.rng = new Rng(this.seed);
    this.pickupsEvery = pickupsEvery || T.pickups.every;
    this.pickupsMax = pickupsMax || T.pickups.max;
    this.eventCounts = {};
    // "Aleatorio" incluye los 4 escenarios (mismo orden que el núcleo C#)
    const ids = Object.keys(ARENAS);
    const chosen = arenaId === 'aleatorio' ? ids[this.rng.int(ids.length)] : arenaId;
    this.setArena(chosen);
    this._arenaUsed = true;
    this.pointsToWin = mode === 'laReta' ? T.reta.streakToWin : pointsToWin;
    this.round = 0;
    this.totalPlayTime = 0;
    this.roundDurations = [];
    this.players = slots.map((s, i) => new Player(this, i, s));
    for (const p of this.players) {
      if (p.isBot || p.slot.autopilot) p.controller = new BotController(this, p, p.slot.difficulty || 'normal');
      if (p.controller && p.controller.useMouse) p.controller.aimFromMouse = (pad) => this._mouseAim(p, pad);
    }
    // La Reta: fila de retadores (los dos primeros pelean)
    this.queue = this.players.map((p) => p);
    this.hooks.onMatchStart?.(this.players, this.arena.def);
    this.nextRound();
  }

  fighters() { return this.modeId === 'laReta' ? this.queue.slice(0, 2) : this.players; }

  nextRound() {
    this.round++;
    // limpiar gorras fantasma, latas, charcos y muros (las manchas del piso se quedan: el mural crece)
    for (const c of this.caps) if (c.ghost && !c.dead) c.kill();
    this.caps = this.caps.filter((c) => !c.dead && !c.ghost);
    for (const pu of this.pickups) this.scene.remove(pu.group);
    this.pickups = [];
    this.paintPatches = [];
    for (const pd of this.puddles) this.scene.remove(pd.group);
    this.puddles = [];
    this.arena.clearTagWalls();
    this.arena.setSuddenDeath(0, false);
    this.arena.hideDrop();
    this.drop = null; this.dropN = 0;
    for (const f of this.fallers) f.toy.root.visible = false;
    this.fallers = [];
    this.hot = null;
    if (this.hotMesh) this.hotMesh.visible = false;
    // orden de spawns barajado con el Rng (Fisher–Yates)
    const spawns = [...this.arena.def.spawns];
    for (let i = spawns.length - 1; i > 0; i--) { const j = this.rng.int(i + 1); [spawns[i], spawns[j]] = [spawns[j], spawns[i]]; }
    const fighting = this.fighters();
    let k = 0;
    for (const p of this.players) {
      if (p.cap.dead) { p.cap = new Cap(this, p); }
      if (!this.caps.includes(p.cap)) this.caps.push(p.cap);
      if (!fighting.includes(p)) { p.bench(); continue; }
      p.benched = false;
      const s = spawns[k++ % spawns.length];
      p.reset({ x: s[0], z: s[1] });
    }
    this.pickupTimer = T.pickups.firstSpawn;
    this.state = 'countdown';
    this.countdown = T.round.countdown;
    this._lastCount = null;
    this.roundEndT = 0;
    this.endTicks = null;
    this.playT = 0;
    this.sdWarned = false;
    this.sdStarted = false;
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

    if (this.mode === 'menu' || this.mode === 'podium') {
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
    const podium = this.mode === 'podium';
    if (this.showcase) {
      // el muñequito se para en el lugar libre de cada escenario; la cámara (ver _frame)
      // lo deja a la derecha en pantallas anchas para que no quede bajo el menú
      const [sx, sz] = this.arena.def.showcase;
      const root = this.showcase.root;
      if (podium) {
        // el ganador brinca de gusto con la taza en alto, de frente a la cámara
        this.podiumT += dt;
        const t = this.podiumT;
        root.position.set(sx, Math.abs(Math.sin(t * 3.4)) * 0.28, sz);
        root.rotation.y = Math.sin(t * 1.3) * 0.45 - 0.15;
        this._confettiT = (this._confettiT || 0) - dt;
        if (this._confettiT <= 0) {
          this._confettiT = 0.32;
          const cols = [this.podiumColor, 0xccff00, 0xffffff, 0xffd23f];
          this.burst(sx + (Math.random() - 0.5) * 4, 4 + Math.random(), sz + (Math.random() - 0.5) * 2, cols[Math.floor(Math.random() * cols.length)], 7, 3);
        }
      } else {
        root.position.set(sx, 0, sz);
        root.rotation.y += dt * 0.6;
      }
      this.showcase.update(dt, { speed01: 0, hasCap: true, cheer: podium });
    }
    if (this.arena) { this.arena.animate(dt); this.arena.tick(dt); }
    this._fx(dt);
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
        if (this.modeId === 'lataCaliente') this._assignHotCan();
      }
    }

    if (this.state === 'play') {
      this.playT += dt;
      this.totalPlayTime += dt;
      this._suddenDeath(dt);
    }

    const playing = this.state === 'play' || this.state === 'roundEnd';
    for (const p of this.players) {
      const pad = consumeInput ? p.pad : { ...p.pad, throwPressed: false, throwReleased: false, meleePressed: false, dashPressed: false };
      if (p.alive) this._updatePlayer(p, pad, dt, playing && this.state === 'play');
    }
    this._separatePlayers();

    for (const c of this.caps) c.update(dt);
    this.caps = this.caps.filter((c) => !c.dead);
    arena.updateTagWalls(dt);
    if (playing) {
      // mismo orden que el núcleo C#: gorras, golpes, charcos, latas, lata caliente, caja DROP, láser y vacíos
      this._capHits();
      this._meleeHits();
      this._updatePuddles(dt);
      this._pickups(dt);
      this._updateHotCan(dt);
      this._updateDrop();
      this._hazards();
    }
    // charcos de pintura (lentitud)
    for (const pp of this.paintPatches) pp.life -= dt;
    this.paintPatches = this.paintPatches.filter((pp) => pp.life > 0);

    // A1: espera de 6 ticks antes de declarar ganador (doble KO = empate)
    if (this.state === 'play' && this.endTicks != null) {
      this.endTicks--;
      if (this.endTicks <= 0) {
        const alive = this.fighters().filter((p) => p.alive);
        this._endRound(alive.length === 1 ? alive[0] : null, alive.length === 0 ? 'doble' : '');
      }
    }

    if (this.state === 'roundEnd') {
      this.roundEndT -= dt;
      if (this.roundEndT <= 0) this._finishRound();
    }
  }

  // ── A2: muerte súbita ──────────────────────────────────────
  _suddenDeath() {
    const SD = T.suddenDeath;
    if (!this.rules.suddenDeath) return;
    const t = this.playT;
    if (t >= SD.start && !this.sdWarned) {
      this.sdWarned = true;
      this._emit('SuddenDeathWarn');
      this.hooks.onSuddenDeath?.();
      sfx.beep(true);
    }
    const maxInset = Math.max(0, (this.arena.w - SD.minWidth) / 2);
    const inset = clamp((t - SD.start - SD.warn) * SD.speed, 0, maxInset);
    const warn = t >= SD.start && t < SD.start + SD.warn;
    if (inset > 0 || warn) this.arena.setSuddenDeath(inset, warn, t);
    if (inset > 0) {
      // empujan, no matan: las latas del piso también se recorren
      const hw = this.arena.w / 2 - inset - 0.6;
      for (const pu of this.pickups) {
        pu.pos.x = clamp(pu.pos.x, -hw, hw); pu.group.position.x = pu.pos.x;
        if (this.arena.voidAt(pu.pos.x, pu.pos.z, 0)) pu.taken = true; // el muro la tiró a un vacío
      }
    }
    if (t >= SD.drawAt && this.modeId !== 'lataCaliente' && this.state === 'play') this._endRound(null, 'tiempo');
  }

  _updatePlayer(p, pad, dt, canAct) {
    p.dashCd -= dt; p.meleeCd -= dt; p.shoveCd -= dt; p.slowT -= dt; p.spawnInvuln -= dt; p.markedT -= dt; p.hotImmune -= dt;
    if (p.meleeT > 0) p.meleeT -= dt;
    // V2: el crédito de una caída dura 2 s
    if (p.lastHitT > 0) { p.lastHitT -= dt; if (p.lastHitT <= 0) { p.lastHitT = 0; p.lastHitBy = null; } }

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
    if (canAct && pad.dashPressed && p.dashCd <= 0 && p.pushT <= 1e-6) {
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

    // golpe (con gorra) o empujón (sin gorra, A3)
    if (canAct && pad.meleePressed && !p.charging) {
      const shove = !p.hasCap && this.rules.shove && p.shoveCd <= 0;
      if ((p.hasCap && p.meleeCd <= 0) || shove) {
        if (pad.aim) p.facing = norm(pad.ax, pad.az);
        else { const t = this._assistTarget(p, p.facing, 0.9, T.player.meleeRange + 1); if (t) p.facing = norm(t.pos.x - p.pos.x, t.pos.z - p.pos.z); }
        p.meleeT = T.player.meleeTime;
        p.meleeShove = shove;
        if (shove) p.shoveCd = T.shove.cooldown; else p.meleeCd = T.player.meleeCooldown;
        p.meleeHits = new Set();
        p.toy.triggerMelee();
        sfx.melee();
      }
    }

    // movimiento
    let vx, vz;
    let pushEnd = false;
    if (p.pushT > 1e-6) {
      // empujón recibido: dash forzado (2.5 m en 0.2 s); al terminar se frena en seco
      p.pushT -= dt;
      vx = p.pushV.x; vz = p.pushV.z;
      pushEnd = p.pushT <= 1e-6;
      if (Math.random() < 0.6) this.trail(p.pos.x, 0.5, p.pos.z, 0xf4f1ea);
    } else if (p.dashT > 0) {
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
    if (pushEnd) { p.vel.x = 0; p.vel.z = 0; p.pushT = 0; }
    // fuera de la ronda en juego ya nadie puede caer: los vacíos se vuelven sólidos
    // para que el ganador no camine "en el aire" mientras festeja
    this.arena.resolveCircle(p.pos, T.player.radius, this.state !== 'play');

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
      // "Tiraje": copias en fila sobre la misma línea, como impresiones de una plancha
      for (const k of [1, 2]) {
        const back = off - T.cap.tirajeGap * k;
        const g = new Cap(this, p, { ghost: true });
        g.launch(p.pos.x + dir.x * back, p.pos.z + dir.z * back, dir.x * sp, dir.z * sp, out, null);
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

  // Tatuaje · Trazo: la gorra empezó a perseguir a alguien
  onMarked(target, cap) {
    this._emit('Marked', { target: target.idx, owner: cap.owner.idx });
    this.float(target.pos.x, 2.6, target.pos.z, '¡MARCADO!', '#f4f1ea');
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
    // atraparla en pleno dash recarga el dash (premia la habilidad)
    if (p.dashT > 0) { p.dashCd = 0; this.float(p.pos.x, 2.2, p.pos.z, '¡RECARGA!', p.color.css); }
  }

  // A8: causa detallada de un KO por gorra
  _capCause(c) {
    if (c.ghost) return 'tiraje';
    if (c.state === 'deflect') return 'teLaRegreso';
    if (c.bounced) return 'deBanda';
    if (c.state === 'return' || c.state === 'recall') return 'deRegreso';
    return 'gorrazo';
  }

  _capHits() {
    const hotMode = this.modeId === 'lataCaliente';
    // gorra vs jugador
    for (const c of this.caps) {
      if (c.dead) continue;
      const owner = c.owner;
      // atrapar la propia (una gorra desviada no se puede atrapar: te noquea)
      if (!c.ghost && owner.alive && c.state !== 'deflect') {
        const canCatch = c.state === 'return' || c.state === 'recall' || (c.state === 'out' && c.t > T.cap.earlyCatchDelay);
        if (canCatch && Math.hypot(c.pos.x - owner.pos.x, c.pos.z - owner.pos.z) < T.cap.catchRadius + (c.radius - T.cap.radius)) {
          this._catch(owner);
          continue;
        }
      }
      if (!c.lethal()) continue;
      const attacker = c.state === 'deflect' ? c.deflector : owner;
      for (const v of this.players) {
        if (!v.alive || v.spawnInvuln > 0) continue;
        if (c.state === 'deflect') { if (v === c.deflector) continue; } else if (v === owner) continue;
        if (Math.hypot(c.pos.x - v.pos.x, c.pos.z - v.pos.z) < c.radius + T.player.radius * 0.85) {
          if (hotMode) { if (attacker && this.hot && this.hot.holder === attacker) this._passHotCan(attacker, v); continue; }
          if (c.power === 'tinta') { this.startPuddle(c, c.pos.x, c.pos.z); break; }
          if (v.shield) {
            v.shield = false;
            this._credit(v, attacker); // V2: lo tocó con una gorra y le rompió el escudo
            sfx.shieldBreak();
            this.float(v.pos.x, 2.2, v.pos.z, '¡BLOQUEADO!', POWERUPS.bordado.css);
            const n = norm(c.pos.x - v.pos.x, c.pos.z - v.pos.z);
            c.drop(n.x * 6, n.z * 6);
            this.shake = Math.max(this.shake, T.feel.shakeClank);
            break;
          }
          this.kill(v, attacker, this._capCause(c));
        }
      }
    }
    // gorra vs gorra: ¡CLANK! ambas caen (A6: 85 % de la suma de radios)
    const lethal = this.caps.filter((c) => !c.dead && c.lethal());
    for (let i = 0; i < lethal.length; i++) {
      for (let j = i + 1; j < lethal.length; j++) {
        const a = lethal[i], b = lethal[j];
        if (a.owner === b.owner || !a.lethal() || !b.lethal()) continue;
        if (Math.hypot(a.pos.x - b.pos.x, a.pos.z - b.pos.z) < (a.radius + b.radius) * T.clankFactor) {
          const n = norm(a.pos.x - b.pos.x, a.pos.z - b.pos.z);
          a.drop(n.x * 7, n.z * 7); b.drop(-n.x * 7, -n.z * 7);
          sfx.clank();
          this.sparks((a.pos.x + b.pos.x) / 2, T.cap.flyHeight, (a.pos.z + b.pos.z) / 2, 0xfff59d, 14);
          this.float((a.pos.x + b.pos.x) / 2, 2.2, (a.pos.z + b.pos.z) / 2, '¡CLANK!', '#fff59d');
          this.shake = Math.max(this.shake, T.feel.shakeClank);
          this._emit('Clank');
        }
      }
    }
  }

  _meleeHits() {
    const hotMode = this.modeId === 'lataCaliente';
    for (const p of this.players) {
      if (!p.alive || p.meleeT <= T.player.meleeTime - T.player.meleeActive) continue;
      const fa = Math.atan2(p.facing.z, p.facing.x);
      const inArc = (x, z, range, extra = 0) => {
        const dx = x - p.pos.x, dz = z - p.pos.z;
        return Math.hypot(dx, dz) <= range && Math.abs(angleTo(fa, Math.atan2(dz, dx))) <= T.player.meleeArc / 2 + extra;
      };
      for (const v of this.players) {
        if (v === p || !v.alive || p.meleeHits.has(v) || v.spawnInvuln > 0) continue;
        if (!inArc(v.pos.x, v.pos.z, T.player.meleeRange + T.player.radius * 0.5)) continue;
        p.meleeHits.add(v);
        this._credit(v, p); // V2: lo empujó o le pegó con un golpe (si después cae a un vacío, el KO es suyo)
        if (hotMode && this.hot && this.hot.holder === p) this._passHotCan(p, v);
        if (p.meleeShove) {
          // A3: empujón, no noquea
          // 2.5 m exactos (12 ticks), de quien empuja hacia el empujado; le corta el dash
          const sp = T.shove.dist / T.shove.time;
          const pd = norm(v.pos.x - p.pos.x, v.pos.z - p.pos.z);
          const dir = pd.x || pd.z ? pd : p.facing;
          v.pushT = T.shove.time; v.pushV = { x: dir.x * sp, z: dir.z * sp };
          v.dashT = 0;
          v.charging = false;
          sfx.dash();
          this.float(v.pos.x, 2.2, v.pos.z, '¡EMPUJÓN!', '#f4f1ea');
          this._emit('Shove', { by: p.idx, target: v.idx });
          continue;
        }
        if (hotMode) continue; // en Lata Caliente el golpe no noquea
        if (v.shield) {
          v.shield = false; sfx.shieldBreak();
          this.float(v.pos.x, 2.2, v.pos.z, '¡BLOQUEADO!', POWERUPS.bordado.css);
          continue;
        }
        this.kill(v, p, 'zaz');
      }
      for (const c of this.caps) {
        if (c.dead || c.owner === p || p.meleeHits.has(c)) continue;
        if (p.meleeShove) {
          // el empujón patea 4 m una gorra ajena caída
          if (c.state !== 'ground' || !inArc(c.pos.x, c.pos.z, T.player.meleeRange + 0.5, 0.3)) continue;
          p.meleeHits.add(c);
          const v0 = T.shove.capKick * T.cap.groundFriction; // con fricción exponencial recorre v0/fricción
          c.vel.x = p.facing.x * v0; c.vel.z = p.facing.z * v0;
          sfx.clank();
          this.puff(c.pos.x, 0.3, c.pos.z, 0xffffff, 6);
          continue;
        }
        // desviar gorras enemigas con el golpe
        if (!c.lethal() || c.deflector === p) continue;
        if (!inArc(c.pos.x, c.pos.z, T.player.meleeRange + 0.5, 0.3)) continue;
        p.meleeHits.add(c);
        this._credit(c.owner, p); // V2: lo desvió
        sfx.clank();
        this.sparks(c.pos.x, c.y, c.pos.z, 0xfff59d, 12);
        this.shake = Math.max(this.shake, T.feel.shakeClank);
        if (this.rules.deflectReturn && c.owner.alive && !c.ghost && !hotMode) {
          c.deflect(p);
          this.float(c.pos.x, 2.2, c.pos.z, '¡TE LA REGRESO!', p.color.css);
          this._emit('Deflect', { by: p.idx, owner: c.owner.idx });
        } else {
          c.drop(p.facing.x * 9, p.facing.z * 9);
          this.float(c.pos.x, 2.2, c.pos.z, '¡DESVÍO!', '#fff59d');
        }
      }
    }
  }

  // ── Tinta derramada · Charco ───────────────────────────────
  startPuddle(c, x, z) {
    const owner = c.owner;
    c.power = null;
    c.drop();
    const group = new THREE.Group();
    const fill = new THREE.Mesh(this._puddleGeo, new THREE.MeshBasicMaterial({ color: 0x121016, transparent: true, opacity: 0.7, depthWrite: false }));
    const rim = new THREE.Mesh(this._puddleRimGeo, new THREE.MeshBasicMaterial({ color: 0xf4f1ea, transparent: true, opacity: 0.9, depthWrite: false }));
    // borde final (para que se vea hasta dónde va a llegar)
    const goal = new THREE.Mesh(this._puddleRimGeo, new THREE.MeshBasicMaterial({ color: 0xf4f1ea, transparent: true, opacity: 0.35, depthWrite: false }));
    goal.scale.setScalar(T.puddle.radius);
    for (const m of [fill, rim, goal]) { m.rotation.x = -Math.PI / 2; m.renderOrder = 2; }
    fill.position.y = 0.05; rim.position.y = 0.06; goal.position.y = 0.055;
    group.add(fill, rim, goal);
    group.position.set(x, 0, z);
    this.scene.add(group);
    this.puddles.push({ x, z, t: 0, owner, group, fill, rim, done: false, fadeT: 0 });
    sfx.explode();
    this.burst(x, 0.6, z, 0x121016, 14, 5);
    this._emit('PuddleStart', { owner: owner.idx, x, z });
  }

  _puddleRadius(pd) { return T.puddle.radius * clamp(pd.t / T.puddle.grow, 0, 1); }

  _updatePuddles(dt) {
    for (const pd of this.puddles) {
      if (pd.done) {
        pd.fadeT += dt;
        pd.fill.material.opacity = Math.max(0, 0.7 - pd.fadeT);
        pd.rim.material.opacity = Math.max(0, 0.9 - pd.fadeT * 2);
        continue;
      }
      pd.t += dt;
      const r = Math.max(0.05, this._puddleRadius(pd));
      pd.fill.scale.setScalar(r); pd.rim.scale.setScalar(r);
      if (pd.t >= T.puddle.grow) {
        // al terminar de crecer noquea a quien siga dentro (excepto al dueño; el Parche bloquea)
        pd.done = true;
        pd.group.children[2].visible = false;
        this.arena.splat(pd.x, pd.z, T.puddle.radius * 0.8, '#111111', 0.9);
        this.burst(pd.x, 0.8, pd.z, 0x111111, 26, 8);
        this.shake = Math.max(this.shake, T.feel.shakeKO);
        this._emit('Explode', { owner: pd.owner.idx, x: pd.x, z: pd.z });
        for (const v of this.players) {
          if (v === pd.owner || !v.alive || v.spawnInvuln > 0) continue;
          if (Math.hypot(v.pos.x - pd.x, v.pos.z - pd.z) < T.puddle.radius) {
            if (v.shield) { v.shield = false; this._credit(v, pd.owner); sfx.shieldBreak(); this.float(v.pos.x, 2.2, v.pos.z, '¡BLOQUEADO!', POWERUPS.bordado.css); continue; }
            this.kill(v, pd.owner, 'splash');
          }
        }
      }
    }
    for (const pd of this.puddles) if (pd.done && pd.fadeT > 1) this.scene.remove(pd.group);
    this.puddles = this.puddles.filter((pd) => !(pd.done && pd.fadeT > 1));
  }

  // ── Modo Lata Caliente ─────────────────────────────────────
  _ensureHotMesh() {
    if (this.hotMesh) return this.hotMesh;
    const g = new THREE.Group();
    const body = new THREE.Mesh(new THREE.CylinderGeometry(0.2, 0.2, 0.5, 14), toon(0x1c1b20));
    const band = new THREE.Mesh(new THREE.CylinderGeometry(0.205, 0.205, 0.14, 14), toon(0xf4f1ea));
    const fuse = new THREE.Mesh(new THREE.SphereGeometry(0.11, 10, 8), new THREE.MeshBasicMaterial({ color: 0xffffff }));
    fuse.position.y = 0.36;
    const label = new THREE.Sprite(new THREE.SpriteMaterial({ map: labelTexture('LATA CALIENTE', '#f4f1ea'), depthWrite: false, depthTest: false, transparent: true }));
    label.scale.set(2.2, 0.55, 1); label.position.y = 0.85; label.renderOrder = 5;
    g.add(body, band, fuse, label);
    g.userData.fuse = fuse;
    this.scene.add(g);
    this.hotMesh = g;
    return g;
  }

  _assignHotCan() {
    const alive = this.players.filter((p) => p.alive);
    if (alive.length < 2) return;
    const holder = alive[this.rng.int(alive.length)];
    const fuse = this.rng.range(T.hotCan.fuseMin, T.hotCan.fuseMax);
    this.hot = { holder, fuse, max: fuse, t: 0 };
    this._ensureHotMesh().visible = true;
    sfx.pickup();
    this.float(holder.pos.x, 2.8, holder.pos.z, '¡LATA CALIENTE!', '#f4f1ea', true);
    this._emit('HotCanStart', { holder: holder.idx });
    this.hooks.onKO?.(null, null, null); // refresca el HUD
  }

  _passHotCan(from, to) {
    if (!this.hot || this.hot.holder !== from || to === from || !to.alive || to.hotImmune > 0) return;
    this.hot.holder = to;
    from.hotImmune = T.hotCan.passImmunity; // para que no regrese al instante
    sfx.catch();
    this.float(to.pos.x, 2.6, to.pos.z, '¡TE TOCA!', to.color.css);
    this._emit('HotCanPass', { from: from.idx, to: to.idx });
    this.hooks.onKO?.(null, null, null);
  }

  _updateHotCan(dt) {
    if (this.modeId !== 'lataCaliente' || this.state !== 'play' || !this.hot) return;
    this.hot.fuse -= dt;
    this.hot.t += dt;
    if (this.hot.fuse <= 0) {
      const h = this.hot.holder;
      this._emit('HotCanExplode', { holder: h.idx });
      this.arena.splat(h.pos.x, h.pos.z, 1.8, '#111111', 0.9);
      this.burst(h.pos.x, 1.2, h.pos.z, 0x111111, 30, 9);
      sfx.explode();
      this.hot = null;
      if (this.hotMesh) this.hotMesh.visible = false;
      this.kill(h, null, 'splash');
      // luego se pega otra al azar entre los vivos
      if (this.state === 'play' && this.endTicks == null) this._assignHotCan();
    }
  }

  _hazards() {
    for (const p of this.players) {
      if (p.alive && this.arena.laserHits(p.pos.x, p.pos.z, T.player.radius)) this.kill(p, null, 'zap');
    }
    this._voids();
  }

  // ── Vacíos (spec v0.3 §A) ──────────────────────────────────
  // V2: recuerda quién lo empujó / golpeó / desvió, y cuándo
  _credit(victim, by) {
    if (!victim || !by || victim === by || !victim.alive) return;
    victim.lastHitBy = by;
    victim.lastHitT = T.voids.creditTime;
  }

  // V1: cae quien tenga el CENTRO dentro de un vacío (reducido 0.15 m por lado)
  // y no esté en dash ni empujado. Un dash o un empujón sí pueden cruzarlo.
  _voids() {
    for (const p of this.players) {
      if (!p.alive || p.dashT > 0 || p.pushT > 1e-6) continue;
      const v = this.arena.voidAt(p.pos.x, p.pos.z);
      if (!v) continue;
      // V2: el KO es de quien lo tocó en los últimos 2 s; si no, de nadie (como el láser)
      const by = p.lastHitT > 0 && p.lastHitBy ? p.lastHitBy : null;
      this.kill(p, by, 'vacio', v);
    }
  }

  // ── Caja DROP (Pop-up Store, spec v0.3 §B4 y §E1) ──────────
  // Tiempos sobre el reloj de la ronda en juego (desde el "¡YA!"): la caja CAE a los
  // 8 s, 28 s, 48 s…; el aviso (DropWarn) sale 1.5 s antes en un punto libre al azar.
  _updateDrop() {
    const D = this.arena.dropDef;
    if (!D || this.state !== 'play') return;
    const t = this.playT + 1e-6;
    if (!this.drop) {
      const landAt = D.first + this.dropN * D.every;
      if (t >= landAt - D.warn) {
        const pos = this.arena.randomFreePoint(1.0);
        this.drop = { x: pos.x, z: pos.z, landAt, warnAt: this.playT };
        sfx.dropWarn();
        this.float(pos.x, 2.6, pos.z, '¡AGUAS!', '#ccff00');
        this._emit('DropWarn', { x: pos.x, z: pos.z, landAt });
      }
    }
    if (this.drop && t >= this.drop.landAt) this._dropLand();
  }

  // latas de técnica encendidas: no en Lata Caliente ni con la regla "powerUps" apagada (igual que el núcleo C#)
  _pickupsOn() { return this.modeId !== 'lataCaliente' && this.rules.powerUps !== false; }

  _dropLand() {
    const { x, z } = this.drop;
    this.drop = null;
    this.dropN++;
    this.arena.hideDrop();
    sfx.dropLand();
    this.shake = Math.max(this.shake, T.feel.shakeKO);
    this.burst(x, 0.5, z, 0x17161a, 16, 7);
    this.burst(x, 0.5, z, 0xccff00, 12, 6);
    this.puff(x, 0.3, z, 0xd9d2c4, 12);
    this._emit('DropLand', { x, z });
    // noquea a quien esté a ≤ 1.1 m (centro a centro); el escudo bloquea. El KO no es de nadie.
    for (const p of this.players) {
      if (!p.alive || p.spawnInvuln > 0 || Math.hypot(p.pos.x - x, p.pos.z - z) > T.drop.radius) continue;
      if (p.shield) { p.shield = false; sfx.shieldBreak(); this.float(p.pos.x, 2.2, p.pos.z, '¡BLOQUEADO!', POWERUPS.bordado.css); continue; }
      this.kill(p, null, 'drop');
    }
    // deja una lata de técnica en ese lugar aunque ya haya el máximo de latas
    // (en Lata Caliente o con las latas apagadas noquea pero no deja lata)
    if (this._pickupsOn()) this._spawnPickup({ x, z });
  }

  kill(v, killer, cause, hole = null) {
    // ya declarada la ronda, nadie más cae (igual que el núcleo C#): el ganador conserva su punto
    if (!v.alive || this.state !== 'play') return;
    v.alive = false;
    v.charging = false;
    v.toy.root.visible = false;
    v.toy.aim.visible = false;
    v.markSprite.visible = false;
    if (killer && killer !== v) killer.kos++;
    if (cause === 'vacio') { this._fall(v, killer, hole); return; }
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
    // A8: letrero según la causa
    this.float(v.pos.x, 2.4, v.pos.z, KO_LABELS[cause] || '¡KO!', killer ? killer.color.css : '#ff1744', true);
    this._afterKO(v, killer, cause);
  }

  // V7: caída a un vacío. El muñequito no se desarma: se hace chiquito hacia abajo y suena "fuuu".
  _fall(v, killer, hole) {
    const root = v.toy.root;
    root.visible = true;
    // se resbala hacia adentro del hueco para no atravesar la orilla
    const cx = hole ? clamp(v.pos.x, hole.x - Math.max(0, hole.w / 2 - 0.55), hole.x + Math.max(0, hole.w / 2 - 0.55)) : v.pos.x;
    const cz = hole ? clamp(v.pos.z, hole.z - Math.max(0, hole.d / 2 - 0.55), hole.z + Math.max(0, hole.d / 2 - 0.55)) : v.pos.z;
    this.fallers.push({ toy: v.toy, t: 0, x: v.pos.x, z: v.pos.z, cx, cz });
    sfx.fall();
    this.puff(v.pos.x, 0.2, v.pos.z, 0xf4f1ea, 8);
    this.hitStop = T.feel.hitStop * 0.6;
    this.shake = Math.max(this.shake, T.feel.shakeKO * 0.6);
    this.float(v.pos.x, 2.4, v.pos.z, KO_LABELS.vacio, killer ? killer.color.css : '#ff1744', true);
    this._afterKO(v, killer, 'vacio');
  }

  _afterKO(v, killer, cause) {
    // su gorra cae y desaparece
    if (!v.hasCap && v.cap.state === 'ground') v.cap.fade = 0;
    v.power = null; v.shield = false;
    this._emit('KO', { victim: v.idx, killer: killer ? killer.idx : -1, cause });
    this.hooks.onKO?.(v, killer, cause);

    // Lata Caliente: si el que la traía cae por otra cosa (láser), se pega a otro
    if (this.hot && this.hot.holder === v) {
      this.hot = null;
      if (this.hotMesh) this.hotMesh.visible = false;
    }

    const alive = this.fighters().filter((p) => p.alive);
    if (this.state === 'play' && alive.length <= 1 && this.endTicks == null) {
      // A1: no se declara ganador de inmediato; se espera 6 ticks
      this.endTicks = T.round.doubleKoTicks;
    } else if (this.modeId === 'lataCaliente' && this.state === 'play' && !this.hot && alive.length >= 2 && cause !== 'splash') {
      this._assignHotCan();
    }
  }

  // reason: '' (último en pie) · 'doble' (doble KO) · 'tiempo' (45 s de muerte súbita)
  _endRound(winner, reason = '') {
    if (this.state !== 'play') return;
    this.state = 'roundEnd';
    this.roundEndT = T.round.endDelay;
    this.slowMo = T.round.slowMoTime;
    this.roundWinner = winner;
    this.roundEndReason = reason;
    this.endTicks = null;
    if (this.hotMesh) this.hotMesh.visible = false;
    this._emit(winner ? 'RoundWin' : 'RoundDraw', { winner: winner ? winner.idx : -1, reason });
  }

  _finishRound() {
    const w = this.roundWinner;
    // métrica de balance: duración de cada ronda (segundos de juego)
    this.roundDurations.push(+((this.simTime || 0) - this.roundStartSim).toFixed(1));
    if (w && w.alive) { w.score++; w.roundWins++; w.streak++; }
    let matchWinner = null;
    if (this.modeId === 'laReta') {
      const [a, b] = this.queue;
      if (w && w.alive) {
        // el ganador se queda; el perdedor va al final de la fila
        const loser = w === a ? b : a;
        loser.streak = 0;
        this.queue = [w, ...this.queue.filter((p) => p !== w && p !== loser), loser];
      }
      // en empate ambos se quedan
      if (w && w.streak >= T.reta.streakToWin) matchWinner = w;
      else if (this.round >= T.reta.maxRounds) {
        // desempate igual que el núcleo C#: más KOs y luego el asiento más bajo (sort estable)
        matchWinner = [...this.players].sort((x, y) => y.score - x.score || y.kos - x.kos)[0];
      }
    } else if (w && w.score >= this.pointsToWin) matchWinner = w;
    this.hooks.onRoundEnd?.(w, this.players, this.roundEndReason);
    if (matchWinner) {
      this.state = 'matchEnd';
      sfx.win();
      this.hooks.onMatchEnd?.({ winner: matchWinner, players: this.players, rounds: this.round, arena: this.arena.def, mode: this.modeId, seconds: this.totalPlayTime, seed: this.seed });
      return;
    }
    this.state = 'between';
    setTimeout(() => { if (this.mode === 'match' && this.state === 'between') this.nextRound(); }, 900 / Math.max(1, this.debugSpeed));
  }

  // ── Latas de técnica ───────────────────────────────────────
  _pickups(dt) {
    if (this.state !== 'play' || !this._pickupsOn()) return; // sin latas de técnica en Lata Caliente
    this.pickupTimer -= dt;
    if (this.pickupTimer <= 0 && this.pickups.length < this.pickupsMax) {
      this.pickupTimer = this.rng.range(this.pickupsEvery[0], this.pickupsEvery[1]);
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

  // at: posición fija (la lata que deja la caja DROP); si no, un punto libre al azar
  _spawnPickup(at = null) {
    const types = Object.keys(POWERUPS);
    const type = types[this.rng.int(types.length)];
    const def = POWERUPS[type];
    let pos = at || this.arena.randomFreePoint(1.0);
    for (let i = 0; i < 10 && !at; i++) {
      if (this.players.every((p) => !p.alive || Math.hypot(p.pos.x - pos.x, p.pos.z - pos.z) > 2.5)) break;
      pos = this.arena.randomFreePoint(1.0);
    }
    const group = new THREE.Group();
    const can = new THREE.Group();
    // latas negro tinta + banda blanca con el ícono de la técnica y el logo INKCAP (spec v0.3 §C):
    // nunca de los colores de jugador (GDD §9)
    this._canGeo ||= {
      body: new THREE.CylinderGeometry(0.22, 0.22, 0.6, 18),
      top: new THREE.SphereGeometry(0.22, 12, 8, 0, Math.PI * 2, 0, Math.PI / 2),
      nozzle: new THREE.CylinderGeometry(0.06, 0.06, 0.1, 8),
      ring: new THREE.RingGeometry(0.5, 0.62, 28),
      mTop: toon(0xbdbdbd), mNozzle: toon(0x212121),
    };
    if (!this._canMats.has(type)) this._canMats.set(type, toon(0xffffff, canLabelTexture(def.glyph)));
    const body = new THREE.Mesh(this._canGeo.body, this._canMats.get(type));
    const top = new THREE.Mesh(this._canGeo.top, this._canGeo.mTop);
    top.position.y = 0.3;
    const nozzle = new THREE.Mesh(this._canGeo.nozzle, this._canGeo.mNozzle);
    nozzle.position.y = 0.5;
    can.add(body, top, nozzle);
    can.rotation.z = 0.25;
    can.scale.setScalar(1.25);
    const ring = new THREE.Mesh(this._canGeo.ring, new THREE.MeshBasicMaterial({ color: 0xf4f1ea, transparent: true, opacity: 0.8, depthWrite: false, side: THREE.DoubleSide }));
    ring.rotation.x = -Math.PI / 2; ring.position.y = -0.5;
    const label = new THREE.Sprite(new THREE.SpriteMaterial({ map: labelTexture(`${def.glyph} ${def.name.toUpperCase()}`, '#f4f1ea'), depthWrite: false, transparent: true }));
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
      p.toy.setPower(0xf4f1ea); // el poder se lee por el efecto en la gorra y el aro blanco, no por color
    }
    this.float(p.pos.x, 2.6, p.pos.z, `${def.glyph} ${def.name.toUpperCase()} · ${def.effect}`, '#f4f1ea');
    this.hooks.onPowerup?.(p, def);
  }

  // ── Efectos visuales ───────────────────────────────────────

  _spawnParticle(x, y, z, color, vx, vy, vz, size, life, gravity = 18) {
    if (this.particles.length >= this._pMax) return;
    this.particles.push({ x, y, z, color, vx, vy, vz, life, max: life, size, gravity });
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
    // evitar amontonamiento: el mismo letrero no se repite en 0.6 s y hay máximo 5 en pantalla
    const now = performance.now();
    this._lastFloat ||= {};
    if (!big && now - (this._lastFloat[text] || 0) < 600) return;
    this._lastFloat[text] = now;
    while (this.floaters.length >= 5) { const old = this.floaters.shift(); old.el.remove(); }
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
      // ícono de marcado (Tatuaje)
      p.markSprite.visible = p.markedT > 0;
      if (p.markSprite.visible) p.markSprite.position.set(p.pos.x, 2.75 + Math.sin(this.simTime * 10) * 0.06, p.pos.z);
    }
    // lata caliente sobre la cabeza de quien la trae: late más rápido al acercarse la explosión
    if (this.hotMesh && this.hot && this.hot.holder.alive) {
      const h = this.hot.holder;
      this.hotMesh.visible = true;
      this.hotMesh.position.set(h.pos.x, 2.05, h.pos.z);
      this.hotMesh.rotation.y += dt * 3;
      const rate = 3 + this.hot.t * 1.2;
      const on = Math.sin(this.hot.t * rate * Math.PI * 2) > 0;
      this.hotMesh.userData.fuse.material.color.setHex(on ? 0xffffff : 0xd55e00);
      this.hotMesh.userData.fuse.scale.setScalar(on ? 1.3 : 0.9);
    } else if (this.hotMesh) this.hotMesh.visible = false;
    // caja DROP: sombra de aviso que crece y la caja bajando
    if (this.drop && this.state === 'play') {
      const D = T.drop;
      this.arena.showDrop(this.drop.x, this.drop.z, clamp(1 - (this.drop.landAt - this.playT) / D.warn, 0, 1), this.simTime);
    } else this.arena.hideDrop();
    this._fx(dt);
  }

  // Efectos que también corren en el menú y el podio: caídas, partículas y piezas
  _fx(dt) {
    // V7: caída a un vacío (el muñequito se encoge hacia abajo, girando)
    for (const f of this.fallers) {
      f.t += dt;
      const k = Math.min(1, f.t / 0.75);
      const root = f.toy.root;
      const slide = Math.min(1, k * 3);
      root.position.set(lerp(f.x, f.cx, slide), -k * k * 3.4, lerp(f.z, f.cz, slide));
      root.scale.setScalar(1.15 * (1 - 0.88 * k));
      root.rotation.y += dt * 11;
      root.rotation.z = Math.sin(f.t * 14) * 0.35 * (1 - k);
      if (k >= 1) { root.visible = false; root.rotation.z = 0; }
    }
    this.fallers = this.fallers.filter((f) => f.t < 0.75);
    // partículas
    for (const pa of this.particles) {
      pa.life -= dt;
      pa.vy -= pa.gravity * dt;
      pa.x += pa.vx * dt; pa.y += pa.vy * dt; pa.z += pa.vz * dt;
      if (pa.y < 0.05) { pa.y = 0.05; pa.vy *= -0.3; pa.vx *= 0.6; pa.vz *= 0.6; }
    }
    this.particles = this.particles.filter((pa) => pa.life > 0);
    // se escriben todas en la malla instanciada (matriz = escala uniforme + posición)
    const im = this._pMesh, arr = im.instanceMatrix.array, col = im.instanceColor.array;
    const n = Math.min(this.particles.length, this._pMax);
    for (let i = 0; i < n; i++) {
      const pa = this.particles[i], sc = pa.size * Math.max(0, pa.life / pa.max), o = i * 16;
      arr[o] = sc; arr[o + 1] = 0; arr[o + 2] = 0; arr[o + 3] = 0;
      arr[o + 4] = 0; arr[o + 5] = sc; arr[o + 6] = 0; arr[o + 7] = 0;
      arr[o + 8] = 0; arr[o + 9] = 0; arr[o + 10] = sc; arr[o + 11] = 0;
      arr[o + 12] = pa.x; arr[o + 13] = pa.y; arr[o + 14] = pa.z; arr[o + 15] = 1;
      this._pColor.setHex(pa.color);
      col[i * 3] = this._pColor.r; col[i * 3 + 1] = this._pColor.g; col[i * 3 + 2] = this._pColor.b;
    }
    im.count = n;
    im.instanceMatrix.needsUpdate = true;
    im.instanceColor.needsUpdate = true;
    // piezas del juguete
    for (const d of this.debris) {
      d.life -= dt;
      d.vy -= 22 * dt;
      d.obj.position.x += d.vx * dt; d.obj.position.y += d.vy * dt; d.obj.position.z += d.vz * dt;
      d.obj.rotation.x += d.rx * dt; d.obj.rotation.z += d.rz * dt;
      // las piezas rebotan en el piso… salvo que caigan en un vacío: ahí se van al fondo
      const overVoid = d.obj.position.y < d.r + 0.2 && this.arena.voidAt(d.obj.position.x, d.obj.position.z, 0);
      if (overVoid) { if (d.obj.position.y < -3.2) d.life = 0; }
      else if (d.obj.position.y < d.r && d.obj.position.y > -0.5) { d.obj.position.y = d.r; d.vy = Math.abs(d.vy) * 0.45; d.vx *= 0.7; d.vz *= 0.7; d.rx *= 0.7; d.rz *= 0.7; }
      else if (d.obj.position.y <= -0.5) { if (d.obj.position.y < -3.2) d.life = 0; }
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
    if (this.mode !== 'match' || !this.arena) {
      // menú y podio: cámara baja frente al muñequito; en pantallas anchas queda a la
      // derecha (el menú o la tabla de resultados van a la izquierda)
      const [sx, sz] = this.arena ? this.arena.def.showcase : [0, 3];
      const wide = this.camera.aspect > 1.15;
      const k = wide ? Math.min(3.2, 1.2 + this.camera.aspect) : 0;
      // en vertical, el podio aleja la cámara y sube al ganador: la tabla ocupa la parte
      // de abajo y arriba debe caber el muñequito con la taza en alto
      const tall = this.mode === 'podium' && !wide;
      const back = wide ? 8.5 : tall ? 15 : 10;
      this.camBase.set(sx - k, 3.4, sz + back);
      this.camTarget.set(sx - k, tall ? 0.36 : 1.4, sz + back - 10);
      return;
    }
    const vfov = THREE.MathUtils.degToRad(this.camera.fov);
    const hfov = 2 * Math.atan(Math.tan(vfov / 2) * this.camera.aspect);
    const W = this.arena.w + 1.5, H = this.arena.h + 2.2;
    const dW = (W / 2) / Math.tan(hfov / 2);
    const dH = (H / 2) * Math.sin(elev) / Math.tan(vfov / 2) + H * 0.12;
    const D = Math.max(dW, dH) * 1.03;
    // se mira un poco hacia el fondo: el muro del fondo (mural, logo) queda completo
    // debajo de las fichas del HUD y se aprovecha el margen de abajo
    const tz = -0.3;
    this.camTarget.set(0, 0, tz);
    this.camBase.set(0, Math.sin(elev) * D, Math.cos(elev) * D + tz);
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
      this.camBase.x + (tx - this.camTarget.x) + (Math.random() - 0.5) * s * 0.8,
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
    this.hot = null;
  }
}
