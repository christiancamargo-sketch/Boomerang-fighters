// ─────────────────────────────────────────────────────────────
// ESCENARIOS (spec v0.3)
// Cada arena = datos (tamaño, obstáculos, VACÍOS, spawns, peligros) + look.
// Este archivo tiene los datos, las consultas de colisión que usa la
// simulación (las mismas reglas que el núcleo C#) y la construcción base:
// piso pintable con huecos, vacíos, muros, peligros y muros de muerte súbita.
// La decoración de cada escenario (marca INKCAP, utilería) vive en escenografia.js.
// El piso es un canvas "pintable": cada KO deja una mancha de tinta,
// así que al final de la partida el escenario queda como un mural.
// ─────────────────────────────────────────────────────────────
import * as THREE from 'three';
import { toon } from './characters.js';
import {
  rng, drawSplat, toCanvasTexture, stencilTexture, hazardStripeTexture,
  pitTexture, rimStripeTexture, glowStripTexture, softDotTexture, dropBoxTexture,
} from './textures.js';
import { TUNING } from './config.js';
import { ESCENAS, buildObstacle } from './escenografia.js';

// Obstáculo: caja AABB en el plano XZ. h >= 1 bloquea gorras; h < 1 solo bloquea jugadores.
const box = (x, z, w, d, h, kind) => ({ x, z, w, d, h, kind });
// Vacío: rectángulo (centroX, centroZ, ancho, fondo) por donde te caes (spec v0.3 §A).
const hole = (x, z, w, d, kind) => ({ x, z, w, d, kind });

// Medidas EXACTAS de la spec v0.3 §B (son las mismas del núcleo C#).
// El orden de las arenas importa: "Aleatorio" y el Reto del Día eligen por índice.
export const ARENAS = {
  callejon: {
    id: 'callejon',
    name: 'Callejón Grafiti',
    desc: 'Contenedores, coladeras abiertas y zanjas de obra entre muros con el mural INKCAP.',
    w: 26, h: 16,
    sky: 0x1b1530,
    fog: 0x1b1530,
    obstacles: [
      box(-6.5, -3.2, 2.6, 1.3, 1.4, 'dumpster'),
      box(6.5, 3.2, 2.6, 1.3, 1.4, 'dumpster'),
      box(0, 0, 1.2, 1.2, 1.2, 'crate'),
      box(-2.4, 4.6, 1.0, 1.0, 0.7, 'crate-low'),
      box(2.4, -4.6, 1.0, 1.0, 0.7, 'crate-low'),
      box(-10.2, 3.8, 1.0, 2.4, 1.3, 'crate-stack'),
      box(10.2, -3.8, 1.0, 2.4, 1.3, 'crate-stack'),
    ],
    voids: [
      hole(-5.6, 4.9, 1.4, 1.4, 'coladera'),
      hole(5.6, -4.9, 1.4, 1.4, 'coladera'),
      hole(0, 3.0, 3.4, 0.9, 'zanja'),
      hole(0, -3.0, 3.4, 0.9, 'zanja'),
    ],
    spawns: [[-11, -6.3], [11, 6.3], [11, -6.3], [-11, 6.3], [-3.5, 0], [3.5, 0]],
    hazards: [],
    showcase: [3.4, 1.4], // dónde se para el muñequito del menú / podio
  },
  museo: {
    id: 'museo',
    name: 'Museo de Arte',
    desc: 'Exposición INKCAP: pedestales, fosos de neón y láseres de seguridad.',
    w: 26, h: 16,
    sky: 0x0f1622,
    fog: 0x0f1622,
    obstacles: [
      box(-5.5, -2.8, 1.4, 1.4, 1.5, 'pedestal'),
      box(5.5, 2.8, 1.4, 1.4, 1.5, 'pedestal'),
      box(-5.5, 2.8, 1.4, 1.4, 1.5, 'pedestal'),
      box(5.5, -2.8, 1.4, 1.4, 1.5, 'pedestal'),
      box(0, 0, 3.2, 0.7, 0.55, 'bench'),
    ],
    voids: [
      hole(-9.2, 0, 1.8, 3.2, 'foso'),
      hole(9.2, 0, 1.8, 3.2, 'foso'),
    ],
    spawns: [[-11, -6.3], [11, 6.3], [11, -6.3], [-11, 6.3], [0, -6.6], [0, 6.6]],
    hazards: [{ type: 'laser', axis: 'x', period: 9, warn: 1.4, active: 1.6, offset: 4 }],
    showcase: [2.6, 4.4],
  },
  azotea: {
    id: 'azotea',
    name: 'Azotea CDMX',
    desc: 'Dos azoteas con un hueco entre edificios y un puente de tablón, al atardecer.',
    w: 26, h: 16,
    sky: 0xff9a62,
    fog: 0xffb37e,
    obstacles: [
      box(-6.5, 0, 1.6, 1.6, 1.8, 'tinaco'),
      box(6.5, 0, 1.6, 1.6, 1.8, 'tinaco'),
      box(-6.5, -4.4, 2.6, 0.5, 0.6, 'planter'),
      box(6.5, 4.4, 2.6, 0.5, 0.6, 'planter'),
      box(-10.8, -5.6, 1.1, 1.1, 1.0, 'gas'),
      box(10.8, 5.6, 1.1, 1.1, 1.0, 'gas'),
    ],
    // hueco entre edificios con un puente de tablón de 3 m en medio
    voids: [
      hole(0, -4.75, 1.8, 6.5, 'hueco'),
      hole(0, 4.75, 1.8, 6.5, 'hueco'),
    ],
    spawns: [[-10, 0], [10, 0], [-4, -6.2], [4, 6.2], [-4, 6.2], [4, -6.2]],
    hazards: [],
    showcase: [3.6, 3.2],
  },
  popup: {
    id: 'popup',
    name: 'Pop-up Store INKCAP',
    desc: 'Racks de playeras, mesa de gorras, escaleras al sótano y la caja DROP que cae del techo.',
    w: 24, h: 15,
    sky: 0x0e0e12,
    fog: 0x0e0e12,
    obstacles: [
      box(-5, -3.2, 2.6, 0.6, 1.4, 'rack'),
      box(5, 3.2, 2.6, 0.6, 1.4, 'rack'),
      box(-5, 3.2, 2.6, 0.6, 1.4, 'rack'),
      box(5, -3.2, 2.6, 0.6, 1.4, 'rack'),
      box(0, 0, 2.2, 1.2, 0.9, 'mesa'), // baja: las gorras pasan
    ],
    voids: [
      hole(-9.6, 4.6, 1.6, 2.2, 'escalera'),
      hole(9.6, -4.6, 1.6, 2.2, 'escalera'),
    ],
    // spawns corregidos (spec §B4): dos caían dentro de los vacíos de la escalera
    spawns: [[-9.5, -5.5], [9.5, 5.5], [6.5, -5.5], [-6.5, 5.5], [0, -5.8], [0, 5.8]],
    // caja DROP: CAE a los 8 s, 28 s, 48 s… de ronda; el aviso sale 1.5 s antes
    hazards: [{ type: 'drop', ...TUNING.drop }],
    showcase: [2.2, 5.2],
  },
};

export const PX_PER_UNIT = 40;

// Geometrías unitarias compartidas por todos los escenarios (se escalan por malla;
// nunca se liberan: son poquitas y así no se recrean al cambiar de arena).
export const GEO = {
  box: new THREE.BoxGeometry(1, 1, 1),
  cyl: new THREE.CylinderGeometry(1, 1, 1, 16),
  cyl8: new THREE.CylinderGeometry(1, 1, 1, 8),
  cone: new THREE.ConeGeometry(1, 1, 12),
  sphere: new THREE.SphereGeometry(1, 14, 10),
  dome: new THREE.SphereGeometry(1, 16, 8, 0, Math.PI * 2, 0, Math.PI / 2),
  plane: new THREE.PlaneGeometry(1, 1),
  // plano acostado sobre el piso (mirando hacia arriba)
  flat: new THREE.PlaneGeometry(1, 1).rotateX(-Math.PI / 2),
  disc: new THREE.CircleGeometry(1, 32).rotateX(-Math.PI / 2),
  ring: new THREE.RingGeometry(0.86, 1, 40).rotateX(-Math.PI / 2),
};
const HIDDEN = new THREE.MeshBasicMaterial({ visible: false });

export class Arena {
  constructor(scene, def) {
    this.scene = scene;
    this.def = def;
    this.sc = ESCENAS[def.id] || ESCENAS.callejon;
    this.w = def.w; this.h = def.h;
    this.group = new THREE.Group();
    scene.add(this.group);
    this.rand = rng(def.id.length * 131 + 7);
    this.obstacles = def.obstacles.map((o) => ({ ...o }));
    this.voids = (def.voids || []).map((v) => ({ ...v }));
    this.paintTimer = 0;
    this.rng = null;      // el juego le pasa su Rng (decisiones de simulación)
    this.inset = 0;       // muerte súbita: cuánto avanzó cada muro lateral (m)
    this.tagWalls = [];   // muros de esténcil temporales (Grafiti · Muro de Tag)
    this.spinners = [];
    this.anims = [];      // funciones (t, dt) de la decoración animada
    this.animT = 0;
    this._owned = [];     // geometrías, materiales y texturas propias de esta arena
    this._mats = new Map();
    this._buildFloor();
    this._buildVoids();
    this._buildWalls();
    this._buildObstacles();
    this._buildHazards();
    this._buildDecor();
    this._buildSuddenDeathWalls();
  }

  _r() { return this.rng ? this.rng.next() : Math.random(); }

  // ── Utilidades de construcción (materiales y geometrías compartidas) ──
  own(x) { this._owned.push(x); return x; }

  // material de caricatura por color (uno por color en toda la arena)
  mat(color) {
    if (!this._mats.has(color)) this._mats.set(color, this.own(toon(color)));
    return this._mats.get(color);
  }

  tmat(tex, color = 0xffffff) { return this.own(toon(color, tex)); }

  // material sin iluminación (luces, neón, letreros iluminados)
  glow(color, opts = {}) { return this.own(new THREE.MeshBasicMaterial({ color, fog: false, ...opts })); }

  add(geo, material, x = 0, y = 0, z = 0, sx = 1, sy = 1, sz = 1, parent = this.group) {
    const m = new THREE.Mesh(geo, typeof material === 'number' ? this.mat(material) : material);
    m.position.set(x, y, z);
    m.scale.set(sx, sy, sz);
    parent.add(m);
    return m;
  }

  // caja con la base en y0 (lo normal para utilería apoyada en el piso)
  box(w, h, d, material, x, y0, z, parent = this.group) { return this.add(GEO.box, material, x, y0 + h / 2, z, w, h, d, parent); }

  cyl(r, h, material, x, y0, z, parent = this.group, geo = GEO.cyl) { return this.add(geo, material, x, y0 + h / 2, z, r, h, r, parent); }

  // canvas en "centímetros" para pintar un muro o letrero de wm × hm metros
  signCanvas(wm, hm, pxW, pxH, paint) {
    const c = document.createElement('canvas');
    c.width = pxW; c.height = pxH;
    const ctx = c.getContext('2d');
    ctx.scale(pxW / (wm * 100), pxH / (hm * 100));
    paint(ctx, wm * 100, hm * 100);
    return this.own(toCanvasTexture(c));
  }

  dispose() {
    this.scene.remove(this.group);
    for (const o of this._owned) o.dispose?.();
    this._owned = [];
    this._mats.clear();
  }

  // ── Piso pintable (con huecos donde hay vacíos) ────────────
  _buildFloor() {
    const W = Math.round(this.w * PX_PER_UNIT), H = Math.round(this.h * PX_PER_UNIT);
    const c = document.createElement('canvas');
    c.width = W; c.height = H;
    const ctx = c.getContext('2d');
    this.floorCanvas = c; this.floorCtx = ctx;
    const F = {
      W, H, px: PX_PER_UNIT, r: this.rand, arena: this,
      X: (x) => (x + this.w / 2) * PX_PER_UNIT,
      Z: (z) => (z + this.h / 2) * PX_PER_UNIT,
      M: (m) => m * PX_PER_UNIT,
      // sombras de contacto de los obstáculos (horneadas en el piso: cero costo por cuadro)
      shadows: (ox = -0.12, oz = -0.16, alpha = 0.5) => this._paintContactShadows(ctx, ox, oz, alpha),
    };
    this.sc.paintFloor(ctx, F);
    this.floorTex = this.own(toCanvasTexture(c));
    const floor = new THREE.Mesh(this.own(this._floorGeometry()), this.own(new THREE.MeshToonMaterial({ map: this.floorTex, color: 0xffffff })));
    this.group.add(floor);

    // suelo exterior: un marco alrededor de la arena (no tapa los vacíos)
    if (this.sc.outer != null) {
      const ext = 46;
      const geo = this.own(ringRectGeometry(this.w / 2, this.h / 2, this.w / 2 + ext, this.h / 2 + ext));
      const outer = new THREE.Mesh(geo, this.own(new THREE.MeshBasicMaterial({ color: this.sc.outer })));
      outer.position.y = -0.012;
      this.group.add(outer);
    }
  }

  // El piso se arma con una rejilla cortada en las orillas de los vacíos:
  // las celdas que caen dentro de un vacío no se dibujan (queda el hueco).
  _floorGeometry() {
    const hw = this.w / 2, hh = this.h / 2;
    const clampX = (x) => Math.max(-hw, Math.min(hw, x)), clampZ = (z) => Math.max(-hh, Math.min(hh, z));
    const xs = new Set([-hw, hw]), zs = new Set([-hh, hh]);
    for (const v of this.voids) {
      xs.add(clampX(v.x - v.w / 2)); xs.add(clampX(v.x + v.w / 2));
      zs.add(clampZ(v.z - v.d / 2)); zs.add(clampZ(v.z + v.d / 2));
    }
    const X = [...xs].sort((a, b) => a - b), Z = [...zs].sort((a, b) => a - b);
    const pos = [], nor = [], uv = [];
    const vert = (x, z) => { pos.push(x, 0, z); nor.push(0, 1, 0); uv.push((x + hw) / this.w, 1 - (z + hh) / this.h); };
    for (let i = 0; i < X.length - 1; i++) {
      for (let j = 0; j < Z.length - 1; j++) {
        const cx = (X[i] + X[i + 1]) / 2, cz = (Z[j] + Z[j + 1]) / 2;
        if (this.voidAt(cx, cz, 0)) continue;
        vert(X[i], Z[j]); vert(X[i], Z[j + 1]); vert(X[i + 1], Z[j + 1]);
        vert(X[i], Z[j]); vert(X[i + 1], Z[j + 1]); vert(X[i + 1], Z[j]);
      }
    }
    const g = new THREE.BufferGeometry();
    g.setAttribute('position', new THREE.Float32BufferAttribute(pos, 3));
    g.setAttribute('normal', new THREE.Float32BufferAttribute(nor, 3));
    g.setAttribute('uv', new THREE.Float32BufferAttribute(uv, 2));
    return g;
  }

  _paintContactShadows(ctx, ox, oz, alpha) {
    const P = PX_PER_UNIT;
    ctx.save();
    ctx.shadowColor = `rgba(0,0,0,${alpha})`;
    ctx.shadowBlur = 0.5 * P;
    ctx.fillStyle = `rgba(0,0,0,${alpha * 0.9})`;
    for (const o of this.obstacles) {
      const [px, py] = this.worldToFloor(o.x, o.z);
      // la sombra se alarga del lado contrario a la luz y más mientras más alto es el objeto
      const k = Math.min(1.6, 0.5 + o.h * 0.6);
      ctx.shadowOffsetX = ox * k * P; ctx.shadowOffsetY = oz * k * P;
      ctx.beginPath();
      if (o.kind === 'tinaco' || o.kind === 'gas') ctx.ellipse(px, py, (o.w / 2) * P, (o.d / 2) * P, 0, 0, Math.PI * 2);
      else ctx.rect(px - (o.w / 2) * P, py - (o.d / 2) * P, o.w * P, o.d * P);
      ctx.fill();
    }
    ctx.restore();
  }

  worldToFloor(x, z) {
    return [(x + this.w / 2) * PX_PER_UNIT, (z + this.h / 2) * PX_PER_UNIT];
  }

  splat(x, z, radius, color, alpha = 0.95) {
    const [px, py] = this.worldToFloor(x, z);
    drawSplat(this.floorCtx, px, py, radius * PX_PER_UNIT, color, Math.random, alpha);
    this.floorTex.needsUpdate = true;
  }

  // pintura de aerosol (rastro del power-up Grafiti)
  spray(x, z, color) {
    const [px, py] = this.worldToFloor(x, z);
    const ctx = this.floorCtx;
    ctx.save();
    ctx.globalAlpha = 0.5;
    ctx.fillStyle = color;
    for (let i = 0; i < 6; i++) {
      ctx.beginPath();
      ctx.arc(px + (Math.random() - 0.5) * 22, py + (Math.random() - 0.5) * 22, 4 + Math.random() * 7, 0, Math.PI * 2);
      ctx.fill();
    }
    ctx.restore();
    this.paintTimer = 0.05;
  }

  tick(dt) {
    if (this.paintTimer > 0) {
      this.paintTimer -= dt;
      if (this.paintTimer <= 0) this.floorTex.needsUpdate = true;
    }
  }

  // ── Vacíos: hueco profundo y oscuro + orilla de aviso (V7) ──
  _buildVoids() {
    const sc = this.sc;
    this._pitBottom = this.own(new THREE.MeshBasicMaterial({ color: 0x000000, side: THREE.BackSide, fog: false }));
    this._pitMats = new Map();
    for (const v of this.voids) {
      // el escenario puede dibujar su propio hueco (la azotea: el hueco entre edificios)
      if (sc.buildVoid && sc.buildVoid(this, v)) continue;
      this.pit(v.x, v.z, v.w, v.d, sc.pitDepth || 3, sc.pit || 'concreto');
      this.rim(v);
    }
  }

  // caja hueca vista por dentro: 4 paredes con degradado a negro + fondo negro
  pit(x, z, w, d, depth, style) {
    if (!this._pitMats.has(style)) this._pitMats.set(style, this.own(new THREE.MeshBasicMaterial({ map: pitTexture(style), side: THREE.BackSide, fog: false })));
    const side = this._pitMats.get(style);
    const m = new THREE.Mesh(GEO.box, [side, side, HIDDEN, this._pitBottom, side, side]);
    m.scale.set(w, depth, d);
    m.position.set(x, -depth / 2, z);
    this.group.add(m);
    return m;
  }

  // orilla del vacío: franjas amarillo/negro, o neón si el escenario lo pide
  rim(v, style = this.sc.rim || 'franjas') {
    const y = 0.014;
    if (style === 'neon') {
      const cols = this.sc.neon || [0x00e5ff, 0xff2bd6];
      const col = cols[this.voids.indexOf(v) % cols.length];
      const glowGeo = this.own(frameGeometry(v, 0.1, 0.95, this, 1));
      const glow = new THREE.Mesh(glowGeo, this.own(new THREE.MeshBasicMaterial({
        map: glowStripTexture(), color: col, transparent: true, opacity: 0.75, depthWrite: false, blending: THREE.AdditiveBlending, fog: false, side: THREE.DoubleSide,
      })));
      glow.position.y = y; glow.renderOrder = 1;
      const tube = new THREE.Mesh(this.own(frameGeometry(v, 0, 0.14, this, 1)), this.glow(col, { side: THREE.DoubleSide }));
      tube.position.y = y + 0.004;
      const core = new THREE.Mesh(this.own(frameGeometry(v, 0.04, 0.05, this, 1)), this.glow(0xffffff, { side: THREE.DoubleSide }));
      core.position.y = y + 0.008;
      this.group.add(glow, tube, core);
      this.anims.push((t) => { glow.material.opacity = 0.6 + Math.sin(t * 2.4 + v.x) * 0.18; });
      return;
    }
    if (!this._rimMat) this._rimMat = this.own(new THREE.MeshBasicMaterial({ map: rimStripeTexture(), fog: false, side: THREE.DoubleSide }));
    const strip = new THREE.Mesh(this.own(frameGeometry(v, 0, 0.26, this, 0.62)), this._rimMat);
    strip.position.y = y;
    this.group.add(strip);
  }

  // ── Muros perimetrales ─────────────────────────────────────
  _buildWalls() {
    if (this.sc.buildWalls) { this.sc.buildWalls(this); return; }
    const W = this.w, H = this.h, t = 0.6;
    const wl = this.sc.walls;
    const cap = this.mat(wl.capColor ?? 0x444444);
    const top = this.mat(wl.topColor ?? 0x555555);
    // muro del fondo: su cara es un canvas propio (mural, cuadros, logo…)
    const wm = W + t * 2;
    const face = this.tmat(this.signCanvas(wm, wl.back, 2048, 256, (ctx, cw, ch) => this.sc.paintBackWall(ctx, cw, ch, this, (x) => (x + wm / 2) * 100)));
    const back = new THREE.Mesh(GEO.box, [cap, cap, top, cap, face, cap]);
    back.scale.set(wm, wl.back, t);
    back.position.set(0, wl.back / 2, -H / 2 - t / 2);
    this.group.add(back);
    // muros laterales
    const sideTex = this.sc.sideTexture ? this.sc.sideTexture(this) : null;
    for (const sx of [-1, 1]) {
      let inner = this.mat(wl.sideColor ?? 0x555555);
      if (sideTex) {
        const tx = this.own(sideTex.clone());
        tx.wrapS = THREE.RepeatWrapping; tx.repeat.set(H / 13, 1); tx.needsUpdate = true;
        inner = this.tmat(tx);
      }
      const mats = sx < 0 ? [inner, cap, top, cap, cap, cap] : [cap, inner, top, cap, cap, cap];
      const m = new THREE.Mesh(GEO.box, mats);
      m.scale.set(t, wl.side, H);
      m.position.set(sx * (W / 2 + t / 2), wl.side / 2, 0);
      this.group.add(m);
    }
    // muro del frente bajito para no tapar la acción
    this.box(wm, wl.front, t, wl.frontColor ?? 0x555555, 0, 0, H / 2 + t / 2);
  }

  // ── Obstáculos ─────────────────────────────────────────────
  _buildObstacles() {
    let idx = 0;
    for (const o of this.obstacles) {
      const g = new THREE.Group();
      g.position.set(o.x, 0, o.z);
      buildObstacle(this, o, g, idx++);
      this.group.add(g);
    }
  }

  // ── Peligros ───────────────────────────────────────────────
  _buildHazards() {
    this.hazards = [];
    this.dropDef = null;
    for (const hz of this.def.hazards) {
      if (hz.type === 'drop') { this.dropDef = hz; continue; } // la caja DROP la maneja el juego (playT de la ronda)
      if (hz.type === 'laser') {
        const mat = this.own(new THREE.MeshBasicMaterial({ color: 0xff1744, transparent: true, opacity: 0, depthWrite: false, fog: false }));
        const beam = new THREE.Mesh(GEO.box, mat);
        beam.scale.set(this.w, 0.12, 0.12);
        beam.position.y = 0.7;
        const glow = new THREE.Mesh(GEO.flat, this.own(new THREE.MeshBasicMaterial({ color: 0xff1744, transparent: true, opacity: 0, depthWrite: false, fog: false })));
        glow.scale.set(this.w, 1, 0.9); glow.position.y = 0.04;
        const g = new THREE.Group();
        g.add(beam, glow);
        for (const sx of [-1, 1]) {
          const e = this.box(0.4, 1.3, 0.5, 0x37474f, sx * (this.w / 2 + 0.12), 0, 0, g);
          e.userData.emitter = true;
          this.add(GEO.box, this.glow(0xff1744), sx * (this.w / 2 - 0.1), 0.7, 0, 0.06, 0.16, 0.16, g);
        }
        this.group.add(g);
        this.hazards.push({ ...hz, group: g, beam, glow, t: 0, z: 0, state: 'off' });
      }
    }
  }

  // Devuelve true si el punto (x,z) está dentro de un láser activo
  updateHazards(dt, onWarn) {
    for (const hz of this.hazards) {
      hz.t += dt;
      const cycle = (hz.t + hz.offset) % hz.period;
      const prev = hz.state;
      if (cycle < hz.warn) hz.state = 'warn';
      else if (cycle < hz.warn + hz.active) hz.state = 'active';
      else hz.state = 'off';
      if (prev === 'off' && hz.state === 'warn') {
        // elige nueva posición del láser
        hz.z = (this._r() - 0.5) * (this.h - 3);
        hz.group.position.z = hz.z;
        if (onWarn) onWarn(hz);
      }
      const blink = Math.sin(hz.t * 30) > 0;
      if (hz.state === 'warn') {
        hz.beam.material.opacity = blink ? 0.35 : 0.1;
        hz.beam.scale.y = 0.036;
        hz.glow.material.opacity = 0.12;
      } else if (hz.state === 'active') {
        hz.beam.material.opacity = 0.95;
        hz.beam.scale.y = 0.17;
        hz.glow.material.opacity = 0.35;
      } else {
        hz.beam.material.opacity = 0;
        hz.glow.material.opacity = 0;
      }
    }
  }

  laserHits(x, z, r) {
    for (const hz of this.hazards) {
      if (hz.type === 'laser' && hz.state === 'active' && Math.abs(z - hz.z) < r + 0.15) return true;
    }
    return false;
  }

  laserDanger(x, z, margin = 1.2) {
    for (const hz of this.hazards) {
      if (hz.type === 'laser' && hz.state !== 'off' && Math.abs(z - hz.z) < margin) return hz.z;
    }
    return null;
  }

  // ── Caja DROP (solo la vista; la regla vive en game.js) ────
  _ensureDrop() {
    if (this._drop) return this._drop;
    const g = new THREE.Group();
    const R = TUNING.drop.radius;
    // sombra de aviso: crece hasta el radio que noquea
    const shadow = new THREE.Mesh(GEO.disc, this.own(new THREE.MeshBasicMaterial({ color: 0x050505, transparent: true, opacity: 0.5, depthWrite: false, fog: false })));
    shadow.position.y = 0.035; shadow.renderOrder = 2;
    const ring = new THREE.Mesh(GEO.ring, this.own(new THREE.MeshBasicMaterial({ color: 0xccff00, transparent: true, opacity: 0.95, depthWrite: false, fog: false })));
    ring.scale.setScalar(R); ring.position.y = 0.04; ring.renderOrder = 3;
    const cross = new THREE.Mesh(GEO.flat, this.own(new THREE.MeshBasicMaterial({ map: rimStripeTexture(), transparent: true, opacity: 0.55, depthWrite: false, fog: false })));
    cross.scale.set(R * 1.15, 1, 0.22); cross.position.y = 0.045; cross.renderOrder = 3;
    const cross2 = cross.clone(); cross2.rotation.y = Math.PI / 2;
    const crate = new THREE.Mesh(GEO.box, this.own(new THREE.MeshToonMaterial({ map: dropBoxTexture() })));
    crate.scale.setScalar(1.25);
    g.add(shadow, ring, cross, cross2, crate);
    g.visible = false;
    this.group.add(g);
    this._drop = { g, shadow, ring, cross, cross2, crate };
    return this._drop;
  }

  // k: 0 (empieza el aviso) → 1 (cae). La caja se ve bajar en el último tramo.
  showDrop(x, z, k, t = 0) {
    const d = this._ensureDrop();
    const R = TUNING.drop.radius;
    d.g.visible = true;
    d.g.position.set(x, 0, z);
    d.shadow.scale.setScalar(R * (0.25 + 0.75 * k));
    d.shadow.material.opacity = 0.3 + 0.4 * k;
    const blink = Math.sin(t * (14 + k * 26)) > 0;
    d.ring.material.opacity = blink ? 1 : 0.45;
    d.cross.material.opacity = d.cross2.material.opacity = blink ? 0.75 : 0.3;
    const fall = Math.max(0, (k - 0.45) / 0.55);
    d.crate.visible = k > 0.45;
    d.crate.position.y = 0.62 + (1 - fall * fall) * 15;
    d.crate.rotation.y = (1 - fall) * 1.4;
  }

  hideDrop() { if (this._drop) this._drop.g.visible = false; }

  // ── Decoración (no colisiona) ──────────────────────────────
  _buildDecor() {
    this.sc.decor?.(this);
    this.group.traverse((o) => { if (o.userData.spin) this.spinners.push(o); });
  }

  animate(dt) {
    this.animT += dt;
    for (const s of this.spinners) s.rotation.y += dt * 0.6;
    for (const f of this.anims) f(this.animT, dt);
  }

  // ── Muerte súbita: muros laterales que avanzan ────────────
  _buildSuddenDeathWalls() {
    const H = this.h;
    const tex = this.own(hazardStripeTexture().clone());
    tex.needsUpdate = true;
    tex.wrapS = tex.wrapT = THREE.RepeatWrapping;
    tex.repeat.set(H / 2, 1);
    const mat = this.tmat(tex);
    this.sdWalls = [-1, 1].map((side) => {
      const m = new THREE.Mesh(GEO.box, mat);
      m.scale.set(0.6, 1.6, H);
      m.position.set(side * (this.w / 2 + 0.3), 0.8, 0);
      m.visible = false;
      m.userData.side = side;
      this.group.add(m);
      return m;
    });
  }

  // warn: true durante el aviso (parpadean); inset: avance de cada muro en metros
  setSuddenDeath(inset, warn = false, t = 0) {
    this.inset = inset;
    for (const m of this.sdWalls) {
      m.position.x = m.userData.side * (this.w / 2 - inset + 0.3);
      m.visible = inset > 0 || (warn && Math.sin(t * 25) > 0);
    }
  }

  // ── Grafiti · Muro de Tag ──────────────────────────────────
  // Muro de esténcil perpendicular a la dirección de vuelo, centrado en la gorra.
  addTagWall(x, z, dirX, dirZ, css, src) {
    const T = TUNING.tagWall;
    const dl = Math.hypot(dirX, dirZ) || 1;
    const nx = dirX / dl, nz = dirZ / dl;      // normal = dirección de vuelo
    const tx = -nz, tz = nx;                   // eje largo del muro
    const half = T.thick / 2;
    const tw = {
      ax: x - tx * T.len / 2, az: z - tz * T.len / 2,
      bx: x + tx * T.len / 2, bz: z + tz * T.len / 2,
      nx, nz, half, life: T.life, src,
    };
    if (!this._tagMats) this._tagMats = new Map();
    if (!this._tagMats.has(css)) this._tagMats.set(css, this.tmat(stencilTexture(css)));
    const mesh = new THREE.Mesh(GEO.box, this._tagMats.get(css));
    mesh.scale.set(T.len, T.height, T.thick);
    mesh.position.set(x, T.height / 2, z);
    mesh.rotation.y = -Math.atan2(tz, tx);
    mesh.scale.y = 0.05 * T.height;
    this.group.add(mesh);
    tw.mesh = mesh;
    this.tagWalls.push(tw);
    return tw;
  }

  updateTagWalls(dt) {
    const H = TUNING.tagWall.height;
    for (const tw of this.tagWalls) {
      tw.life -= dt;
      const age = TUNING.tagWall.life - tw.life;
      const k = Math.min(1, 0.05 + age * 8) * (tw.life < 0.25 ? Math.max(0.05, tw.life / 0.25) : 1);
      tw.mesh.scale.y = k * H;
      tw.mesh.position.y = (k * H) / 2;
    }
    for (const tw of this.tagWalls) if (tw.life <= 0) this.group.remove(tw.mesh);
    this.tagWalls = this.tagWalls.filter((tw) => tw.life > 0);
  }

  clearTagWalls() {
    for (const tw of this.tagWalls) this.group.remove(tw.mesh);
    this.tagWalls = [];
  }

  // ── Vacíos: consultas de la simulación (spec v0.3 §A) ──────
  // V1: ¿el punto está dentro de un vacío reducido `shrink` metros por lado?
  // (jugadores: 0.15 m; gorras en el piso: 0, el rectángulo completo)
  voidAt(x, z, shrink = TUNING.voids.shrink) {
    for (const v of this.voids) {
      if (Math.abs(x - v.x) < v.w / 2 - shrink && Math.abs(z - v.z) < v.d / 2 - shrink) return v;
    }
    return null;
  }

  // Vacío cuya orilla queda a menos de `margin` metros (o null). Devuelve la
  // distancia a la orilla (0 si está dentro) y hacia dónde alejarse.
  voidNear(x, z, margin) {
    let best = null;
    for (const v of this.voids) {
      const cx = Math.max(v.x - v.w / 2, Math.min(x, v.x + v.w / 2));
      const cz = Math.max(v.z - v.d / 2, Math.min(z, v.z + v.d / 2));
      let dx = x - cx, dz = z - cz;
      const d = Math.hypot(dx, dz);
      if (d >= margin || (best && d >= best.d)) continue;
      if (d < 1e-6) {
        // dentro: salir por el lado más cercano
        const left = x - (v.x - v.w / 2), right = (v.x + v.w / 2) - x;
        const top = z - (v.z - v.d / 2), bot = (v.z + v.d / 2) - z;
        const m = Math.min(left, right, top, bot);
        dx = m === left ? -1 : m === right ? 1 : 0;
        dz = m === top ? -1 : m === bot ? 1 : 0;
        if (dx) dz = 0;
        best = { v, d: 0, ax: dx, az: dz };
      } else best = { v, d, ax: dx / d, az: dz / d };
    }
    return best;
  }

  // ── Consultas de colisión ──────────────────────────────────
  // Empuja un círculo fuera de muros y obstáculos (solo los que bloquean jugadores).
  // Los vacíos NO empujan (te caes); con solidVoids=true sí (fuera de la ronda en juego,
  // para que nadie camine "en el aire" cuando ya no se puede caer).
  resolveCircle(p, r, solidVoids = false) {
    const hw = this.w / 2 - this.inset - r, hh = this.h / 2 - r;
    let hit = false;
    if (p.x < -hw) { p.x = -hw; hit = true; }
    if (p.x > hw) { p.x = hw; hit = true; }
    if (p.z < -hh) { p.z = -hh; hit = true; }
    if (p.z > hh) { p.z = hh; hit = true; }
    for (const o of solidVoids ? [...this.obstacles, ...this.voids] : this.obstacles) {
      const cx = Math.max(o.x - o.w / 2, Math.min(p.x, o.x + o.w / 2));
      const cz = Math.max(o.z - o.d / 2, Math.min(p.z, o.z + o.d / 2));
      const dx = p.x - cx, dz = p.z - cz;
      const d2 = dx * dx + dz * dz;
      if (d2 < r * r) {
        hit = true;
        if (d2 > 1e-6) {
          const d = Math.sqrt(d2);
          p.x = cx + (dx / d) * r; p.z = cz + (dz / d) * r;
        } else {
          // centro dentro de la caja: sacarlo por el lado más cercano
          const left = p.x - (o.x - o.w / 2), right = (o.x + o.w / 2) - p.x;
          const top = p.z - (o.z - o.d / 2), bot = (o.z + o.d / 2) - p.z;
          const m = Math.min(left, right, top, bot);
          if (m === left) p.x = o.x - o.w / 2 - r;
          else if (m === right) p.x = o.x + o.w / 2 + r;
          else if (m === top) p.z = o.z - o.d / 2 - r;
          else p.z = o.z + o.d / 2 + r;
        }
      }
    }
    for (const tw of this.tagWalls) {
      const c = closestOnSegment(p.x, p.z, tw);
      const dx = p.x - c.x, dz = p.z - c.z;
      const d = Math.hypot(dx, dz);
      const min = r + tw.half;
      if (d < min) {
        hit = true;
        const n = d > 1e-6 ? { x: dx / d, z: dz / d } : { x: tw.nx, z: tw.nz };
        p.x = c.x + n.x * min; p.z = c.z + n.z * min;
      }
    }
    return hit;
  }

  // Colisión de proyectil (gorra). Devuelve la normal del choque o null.
  // V3: las gorras en vuelo pasan por encima de los vacíos (aquí no cuentan).
  capCollision(p, r, cap = null) {
    const hw = this.w / 2 - this.inset - r, hh = this.h / 2 - r;
    if (p.x < -hw) return { nx: 1, nz: 0, x: -hw, z: p.z };
    if (p.x > hw) return { nx: -1, nz: 0, x: hw, z: p.z };
    if (p.z < -hh) return { nx: 0, nz: 1, x: p.x, z: -hh };
    if (p.z > hh) return { nx: 0, nz: -1, x: p.x, z: hh };
    for (const o of this.obstacles) {
      if (o.h < 1) continue; // los obstáculos bajos no detienen gorras
      const cx = Math.max(o.x - o.w / 2, Math.min(p.x, o.x + o.w / 2));
      const cz = Math.max(o.z - o.d / 2, Math.min(p.z, o.z + o.d / 2));
      const dx = p.x - cx, dz = p.z - cz;
      const d2 = dx * dx + dz * dz;
      if (d2 < r * r) {
        const d = Math.sqrt(d2) || 1;
        let nx = dx / d, nz = dz / d;
        if (d2 < 1e-6) { nx = Math.sign(p.x - o.x) || 1; nz = 0; }
        return { nx, nz, x: cx + nx * r, z: cz + nz * r };
      }
    }
    for (const tw of this.tagWalls) {
      if (tw.src === cap) continue; // la gorra que lo pintó no choca con su propio muro
      const c = closestOnSegment(p.x, p.z, tw);
      const dx = p.x - c.x, dz = p.z - c.z;
      const d = Math.hypot(dx, dz);
      const min = r + tw.half;
      if (d < min) {
        const n = d > 1e-6 ? { x: dx / d, z: dz / d } : { x: tw.nx, z: tw.nz };
        return { nx: n.x, nz: n.z, x: c.x + n.x * min, z: c.z + n.z * min, tagWall: true };
      }
    }
    return null;
  }

  // ¿Hay línea de vista entre a y b? (para la IA)
  lineOfSight(ax, az, bx, bz) {
    for (const o of this.obstacles) {
      if (o.h < 1) continue;
      if (segmentHitsBox(ax, az, bx, bz, o, 0.25)) return false;
    }
    return true;
  }

  // ¿Se puede estar parado ahí? Los vacíos cuentan como obstáculo (V4 y V5).
  isFree(x, z, r) {
    if (Math.abs(x) > this.w / 2 - this.inset - r || Math.abs(z) > this.h / 2 - r) return false;
    for (const o of this.obstacles) {
      if (Math.abs(x - o.x) < o.w / 2 + r && Math.abs(z - o.z) < o.d / 2 + r) return false;
    }
    for (const v of this.voids) {
      if (Math.abs(x - v.x) < v.w / 2 + r && Math.abs(z - v.z) < v.d / 2 + r) return false;
    }
    for (const tw of this.tagWalls) {
      const c = closestOnSegment(x, z, tw);
      if (Math.hypot(x - c.x, z - c.z) < r + tw.half) return false;
    }
    return true;
  }

  // V4: nada aparece dentro de un vacío ni a menos de 0.8 m de uno.
  randomFreePoint(r = 0.8) {
    const m = TUNING.voids.spawnMargin;
    for (let i = 0; i < 60; i++) {
      const x = (this._r() - 0.5) * (this.w - 3 - this.inset * 2);
      const z = (this._r() - 0.5) * (this.h - 3);
      if (this.isFree(x, z, r) && !this.voidNear(x, z, m)) return { x, z };
    }
    // respaldo (casi imposible llegar aquí): un spawn libre, que nunca está en un vacío
    for (const s of this.def.spawns) if (this.isFree(s[0], s[1], 0.5)) return { x: s[0], z: s[1] };
    return { x: this.def.spawns[0][0], z: this.def.spawns[0][1] };
  }
}

function closestOnSegment(px, pz, s) {
  const dx = s.bx - s.ax, dz = s.bz - s.az;
  const l2 = dx * dx + dz * dz || 1;
  const t = Math.max(0, Math.min(1, ((px - s.ax) * dx + (pz - s.az) * dz) / l2));
  return { x: s.ax + dx * t, z: s.az + dz * t };
}

function segmentHitsBox(ax, az, bx, bz, o, pad) {
  // método slab (Liang–Barsky) contra la caja inflada
  const minX = o.x - o.w / 2 - pad, maxX = o.x + o.w / 2 + pad;
  const minZ = o.z - o.d / 2 - pad, maxZ = o.z + o.d / 2 + pad;
  let t0 = 0, t1 = 1;
  const dx = bx - ax, dz = bz - az;
  const clip = (p, q) => {
    if (Math.abs(p) < 1e-9) return q >= 0;
    const t = q / p;
    if (p < 0) { if (t > t1) return false; if (t > t0) t0 = t; }
    else { if (t < t0) return false; if (t < t1) t1 = t; }
    return true;
  };
  return clip(-dx, ax - minX) && clip(dx, maxX - ax) && clip(-dz, az - minZ) && clip(dz, maxZ - az) && t0 <= t1;
}

// Marco rectangular plano (suelo exterior): rectángulo grande con un hueco al centro.
function ringRectGeometry(ix, iz, ox, oz) {
  const pos = [];
  const quad = (x0, z0, x1, z1) => { pos.push(x0, 0, z0, x0, 0, z1, x1, 0, z1, x0, 0, z0, x1, 0, z1, x1, 0, z0); };
  quad(-ox, -oz, ox, -iz); quad(-ox, iz, ox, oz); quad(-ox, -iz, -ix, iz); quad(ix, -iz, ox, iz);
  const g = new THREE.BufferGeometry();
  g.setAttribute('position', new THREE.Float32BufferAttribute(pos, 3));
  g.computeVertexNormals();
  return g;
}

// Marco alrededor de un vacío: empieza a `a` metros de la orilla y mide `wd` de ancho.
// UV: u corre a lo largo (1 repetición cada `period` metros), v=0 pegado al vacío.
// Los lados que tocan el borde de la arena no se dibujan (la azotea).
export function frameGeometry(v, a, wd, arena, period = 0.6) {
  const hw = arena.w / 2, hh = arena.h / 2, e = 1e-3;
  const x0 = v.x - v.w / 2 - a, x1 = v.x + v.w / 2 + a, z0 = v.z - v.d / 2 - a, z1 = v.z + v.d / 2 + a;
  const skipN = v.z - v.d / 2 <= -hh + e, skipS = v.z + v.d / 2 >= hh - e;
  const skipW = v.x - v.w / 2 <= -hw + e, skipE = v.x + v.w / 2 >= hw - e;
  const X0 = skipW ? x0 : x0 - wd, X1 = skipE ? x1 : x1 + wd, Z0 = skipN ? z0 : z0 - wd, Z1 = skipS ? z1 : z1 + wd;
  const pos = [], uv = [];
  // cuadrilátero: orilla interior a→b (v=0), orilla exterior d→c (v=1)
  const quad = (ax, az, bx, bz, cx, cz, dx, dz) => {
    const len = Math.hypot(bx - ax, bz - az) / period;
    pos.push(ax, 0, az, bx, 0, bz, cx, 0, cz, ax, 0, az, cx, 0, cz, dx, 0, dz);
    uv.push(0, 0, len, 0, len, 1, 0, 0, len, 1, 0, 1);
  };
  if (!skipN) quad(x0, z0, x1, z0, X1, Z0, X0, Z0);
  if (!skipS) quad(x1, z1, x0, z1, X0, Z1, X1, Z1);
  if (!skipW) quad(x0, z1, x0, z0, X0, Z0, X0, Z1);
  if (!skipE) quad(x1, z0, x1, z1, X1, Z1, X1, Z0);
  const g = new THREE.BufferGeometry();
  g.setAttribute('position', new THREE.Float32BufferAttribute(pos, 3));
  g.setAttribute('uv', new THREE.Float32BufferAttribute(uv, 2));
  const nor = new Float32Array(pos.length);
  for (let i = 1; i < nor.length; i += 3) nor[i] = 1;
  g.setAttribute('normal', new THREE.BufferAttribute(nor, 3));
  return g;
}
