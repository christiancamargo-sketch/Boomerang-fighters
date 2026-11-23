// ─────────────────────────────────────────────────────────────
// ESCENARIOS
// Cada arena = datos (tamaño, obstáculos, spawns, peligros) + look.
// El piso es un canvas "pintable": cada KO deja una mancha de tinta,
// así que al final de la partida el escenario queda como un mural.
// ─────────────────────────────────────────────────────────────
import * as THREE from 'three';
import { toon } from './characters.js';
import {
  rng, drawSplat, toCanvasTexture,
  graffitiWallTexture, museumWallTexture, rooftopWallTexture,
  stencilTexture, hazardStripeTexture,
} from './textures.js';
import { TUNING } from './config.js';

// Obstáculo: caja AABB en el plano XZ. h >= 1 bloquea gorras; h < 1 solo bloquea jugadores.
const box = (x, z, w, d, h, kind) => ({ x, z, w, d, h, kind });

export const ARENAS = {
  callejon: {
    id: 'callejon',
    name: 'Callejón Grafiti',
    desc: 'Contenedores, cajas y muros llenos de tags.',
    w: 23, h: 14,
    sky: 0x1b1530,
    fog: 0x1b1530,
    obstacles: [
      box(-5.6, -2.8, 2.4, 1.2, 1.3, 'dumpster'),
      box(5.6, 2.8, 2.4, 1.2, 1.3, 'dumpster'),
      box(0, 0, 1.1, 1.1, 1.1, 'crate'),
      box(-2.0, 3.9, 1.0, 1.0, 0.7, 'crate-low'),
      box(2.0, -3.9, 1.0, 1.0, 0.7, 'crate-low'),
      box(-9.0, 3.1, 0.9, 2.2, 1.2, 'crate'),
      box(9.0, -3.1, 0.9, 2.2, 1.2, 'crate'),
    ],
    spawns: [[-9, -4.8], [9, 4.8], [9, -4.8], [-9, 4.8], [0, -5.4], [0, 5.4]],
    hazards: [],
  },
  museo: {
    id: 'museo',
    name: 'Museo de Arte',
    desc: 'Pedestales con esculturas y láseres de seguridad.',
    w: 23, h: 14,
    sky: 0x0f1622,
    fog: 0x0f1622,
    obstacles: [
      box(-4.9, -2.5, 1.3, 1.3, 1.4, 'pedestal'),
      box(4.9, 2.5, 1.3, 1.3, 1.4, 'pedestal'),
      box(-4.9, 2.5, 1.3, 1.3, 1.4, 'pedestal'),
      box(4.9, -2.5, 1.3, 1.3, 1.4, 'pedestal'),
      box(0, 0, 3.0, 0.7, 0.55, 'bench'),
    ],
    spawns: [[-9.3, -4.8], [9.3, 4.8], [9.3, -4.8], [-9.3, 4.8], [0, -5.4], [0, 5.4]],
    hazards: [{ type: 'laser', axis: 'x', period: 9, warn: 1.4, active: 1.6, offset: 4 }],
  },
  azotea: {
    id: 'azotea',
    name: 'Azotea CDMX',
    desc: 'Tinacos, macetas y tendederos al atardecer.',
    w: 22, h: 13.5,
    sky: 0xff9a62,
    fog: 0xffb37e,
    obstacles: [
      box(-5.4, 0, 1.5, 1.5, 1.5, 'tinaco'),
      box(5.4, 0, 1.5, 1.5, 1.5, 'tinaco'),
      box(0, -3.2, 2.8, 0.5, 0.6, 'planter'),
      box(0, 3.2, 2.8, 0.5, 0.6, 'planter'),
      box(-8.6, -4.1, 1.1, 1.1, 1.0, 'gas'),
      box(8.6, 4.1, 1.1, 1.1, 1.0, 'gas'),
    ],
    spawns: [[-8.8, 0], [8.8, 0], [0, -5.3], [0, 5.3], [-8.2, 4.9], [8.2, -4.9]],
    hazards: [],
  },
};

const PX_PER_UNIT = 40;

export class Arena {
  constructor(scene, def) {
    this.scene = scene;
    this.def = def;
    this.w = def.w; this.h = def.h;
    this.group = new THREE.Group();
    scene.add(this.group);
    this.rand = rng(def.id.length * 131 + 7);
    this.obstacles = def.obstacles.map((o) => ({ ...o }));
    this.paintTimer = 0;
    this.rng = null;      // el juego le pasa su Rng (decisiones de simulación)
    this.inset = 0;       // muerte súbita: cuánto avanzó cada muro lateral (m)
    this.tagWalls = [];   // muros de esténcil temporales (Grafiti · Muro de Tag)
    this._buildFloor();
    this._buildWalls();
    this._buildObstacles();
    this._buildHazards();
    this._buildDecor();
    this._buildSuddenDeathWalls();
  }

  _r() { return this.rng ? this.rng.next() : Math.random(); }

  dispose() {
    this.scene.remove(this.group);
    this.group.traverse((o) => {
      if (o.geometry) o.geometry.dispose();
    });
  }

  // ── Piso pintable ──────────────────────────────────────────
  _buildFloor() {
    const W = Math.round(this.w * PX_PER_UNIT), H = Math.round(this.h * PX_PER_UNIT);
    const c = document.createElement('canvas');
    c.width = W; c.height = H;
    const ctx = c.getContext('2d');
    this.floorCanvas = c; this.floorCtx = ctx;
    const r = this.rand;
    const id = this.def.id;
    if (id === 'callejon') {
      ctx.fillStyle = '#3a3a42'; ctx.fillRect(0, 0, W, H);
      for (let i = 0; i < 2500; i++) {
        ctx.fillStyle = `rgba(255,255,255,${r() * 0.05})`;
        ctx.fillRect(r() * W, r() * H, 2, 2);
      }
      // líneas pintadas y coladera
      ctx.strokeStyle = 'rgba(255,214,0,0.55)'; ctx.lineWidth = 10; ctx.setLineDash([60, 40]);
      ctx.beginPath(); ctx.moveTo(0, H / 2); ctx.lineTo(W, H / 2); ctx.stroke();
      ctx.setLineDash([]);
      ctx.fillStyle = '#22222a';
      ctx.beginPath(); ctx.arc(W * 0.72, H * 0.25, 30, 0, Math.PI * 2); ctx.fill();
      ctx.strokeStyle = '#555'; ctx.lineWidth = 3;
      for (let k = -24; k <= 24; k += 8) { ctx.beginPath(); ctx.moveTo(W * 0.72 + k, H * 0.25 - 26); ctx.lineTo(W * 0.72 + k, H * 0.25 + 26); ctx.stroke(); }
      // manchas viejas de pintura
      const cols = ['#ff2bd6', '#39ff14', '#00e5ff', '#ffe600'];
      for (let i = 0; i < 14; i++) drawSplat(ctx, r() * W, r() * H, 10 + r() * 30, cols[i % 4], r, 0.25);
    } else if (id === 'museo') {
      const t = 80;
      for (let y = 0; y < H; y += t) for (let x = 0; x < W; x += t) {
        ctx.fillStyle = ((x + y) / t) % 2 === 0 ? '#e9e4da' : '#2d2f36';
        ctx.fillRect(x, y, t, t);
      }
      // vetas de mármol
      ctx.strokeStyle = 'rgba(140,140,150,0.25)'; ctx.lineWidth = 2;
      for (let i = 0; i < 60; i++) {
        ctx.beginPath(); let x = r() * W, y = r() * H; ctx.moveTo(x, y);
        for (let k = 0; k < 5; k++) { x += (r() - 0.5) * 80; y += (r() - 0.5) * 80; ctx.lineTo(x, y); }
        ctx.stroke();
      }
      // alfombra roja central
      ctx.fillStyle = 'rgba(160,20,30,0.85)';
      ctx.fillRect(W * 0.08, H * 0.42, W * 0.84, H * 0.16);
    } else {
      ctx.fillStyle = '#b5764f'; ctx.fillRect(0, 0, W, H);
      // impermeabilizante rojo en parches
      ctx.fillStyle = '#c0392b';
      for (let i = 0; i < 6; i++) ctx.fillRect(r() * W, r() * H, 120 + r() * 220, 80 + r() * 160);
      for (let i = 0; i < 2500; i++) {
        ctx.fillStyle = `rgba(0,0,0,${r() * 0.06})`;
        ctx.fillRect(r() * W, r() * H, 3, 3);
      }
    }
    this.floorTex = toCanvasTexture(c);
    const floor = new THREE.Mesh(
      new THREE.PlaneGeometry(this.w, this.h),
      new THREE.MeshToonMaterial({ map: this.floorTex, color: 0xffffff })
    );
    floor.rotation.x = -Math.PI / 2;
    this.group.add(floor);

    // suelo exterior (para que no se vea el vacío en pantallas anchas)
    const outer = new THREE.Mesh(
      new THREE.PlaneGeometry(this.w * 4, this.h * 5),
      new THREE.MeshBasicMaterial({ color: new THREE.Color(this.def.sky).multiplyScalar(0.55) })
    );
    outer.rotation.x = -Math.PI / 2; outer.position.y = -0.02;
    this.group.add(outer);
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

  // ── Muros perimetrales ─────────────────────────────────────
  _buildWalls() {
    const id = this.def.id;
    const wallH = id === 'azotea' ? 1.0 : 2.4;
    let tex;
    if (id === 'callejon') tex = graffitiWallTexture(11);
    else if (id === 'museo') tex = museumWallTexture(5);
    else tex = rooftopWallTexture(3);
    const thick = 0.6;
    const mk = (w, d, x, z, rotY, texRepeat) => {
      const t = tex.clone(); t.needsUpdate = true;
      t.wrapS = THREE.RepeatWrapping; t.repeat.set(texRepeat, 1);
      const mats = [toon(0x444444), toon(0x444444), toon(0x555555), toon(0x555555), toon(0xffffff, t), toon(0xffffff, t)];
      const m = new THREE.Mesh(new THREE.BoxGeometry(w, wallH, d), mats);
      m.position.set(x, wallH / 2, z);
      m.rotation.y = rotY;
      this.group.add(m);
    };
    const W = this.w, H = this.h;
    // muro del fondo y frente (el del frente bajito para no tapar la acción)
    mk(W + thick * 2, thick, 0, -H / 2 - thick / 2, 0, W / 13);
    mk(H, thick, -W / 2 - thick / 2, 0, Math.PI / 2, H / 13);
    mk(H, thick, W / 2 + thick / 2, 0, -Math.PI / 2, H / 13);
    const front = new THREE.Mesh(new THREE.BoxGeometry(W + thick * 2, 0.35, thick), toon(id === 'museo' ? 0xb08d57 : 0x555555));
    front.position.set(0, 0.175, H / 2 + thick / 2);
    this.group.add(front);
  }

  // ── Obstáculos ─────────────────────────────────────────────
  _buildObstacles() {
    let pedestalIdx = 0;
    for (const o of this.obstacles) {
      const g = new THREE.Group();
      g.position.set(o.x, 0, o.z);
      if (o.kind === 'dumpster') {
        const b = new THREE.Mesh(new THREE.BoxGeometry(o.w, o.h * 0.85, o.d), toon(0x1f7a4d));
        b.position.y = o.h * 0.425;
        const lid = new THREE.Mesh(new THREE.BoxGeometry(o.w * 1.04, 0.12, o.d * 1.1), toon(0x14593a));
        lid.position.y = o.h * 0.88; lid.rotation.x = -0.08;
        g.add(b, lid);
      } else if (o.kind.startsWith('crate')) {
        const b = new THREE.Mesh(new THREE.BoxGeometry(o.w, o.h, o.d), toon(o.kind === 'crate-low' ? 0xd4a373 : 0xbc8a5f));
        b.position.y = o.h / 2;
        const band = new THREE.Mesh(new THREE.BoxGeometry(o.w * 1.02, o.h * 0.14, o.d * 1.02), toon(0x7f5539));
        band.position.y = o.h * 0.5;
        g.add(b, band);
      } else if (o.kind === 'pedestal') {
        const b = new THREE.Mesh(new THREE.BoxGeometry(o.w, 0.9, o.d), toon(0xf4f1ea));
        b.position.y = 0.45;
        g.add(b);
        // escultura abstracta distinta en cada pedestal
        const k = pedestalIdx++ % 4;
        let s;
        if (k === 0) { s = new THREE.Mesh(new THREE.TorusKnotGeometry(0.32, 0.11, 60, 8), toon(0xffb703)); s.position.y = 1.45; }
        else if (k === 1) { s = new THREE.Mesh(new THREE.IcosahedronGeometry(0.45, 0), toon(0xe63946)); s.position.y = 1.4; }
        else if (k === 2) { s = new THREE.Mesh(new THREE.SphereGeometry(0.42, 16, 12), toon(0x2a9d8f)); s.scale.set(0.8, 1.2, 0.8); s.position.y = 1.45; }
        else {
          // "busto" de muñequito gigante: guiño a los art toys
          s = new THREE.Group();
          const hd = new THREE.Mesh(new THREE.SphereGeometry(0.4, 16, 12), toon(0xf4f1ea)); hd.scale.set(1.05, 0.92, 1);
          const e1 = new THREE.Mesh(new THREE.SphereGeometry(0.06, 8, 6), new THREE.MeshBasicMaterial({ color: 0x111111 })); e1.position.set(-0.13, 0.02, 0.36); e1.scale.y = 1.4;
          const e2 = e1.clone(); e2.position.x = 0.13;
          s.add(hd, e1, e2); s.position.y = 1.35;
        }
        s.userData.spin = true;
        g.add(s);
        const rope = new THREE.Mesh(new THREE.TorusGeometry(1.0, 0.03, 6, 24), toon(0xb08d57));
        rope.rotation.x = Math.PI / 2; rope.position.y = 0.6;
        g.add(rope);
      } else if (o.kind === 'bench') {
        const b = new THREE.Mesh(new THREE.BoxGeometry(o.w, 0.18, o.d), toon(0x6d4c41));
        b.position.y = 0.45;
        const l1 = new THREE.Mesh(new THREE.BoxGeometry(0.2, 0.45, o.d * 0.8), toon(0x3e2723));
        l1.position.set(-o.w / 2 + 0.3, 0.22, 0);
        const l2 = l1.clone(); l2.position.x = o.w / 2 - 0.3;
        g.add(b, l1, l2);
      } else if (o.kind === 'tinaco') {
        const b = new THREE.Mesh(new THREE.CylinderGeometry(o.w * 0.48, o.w * 0.5, o.h, 16), toon(0x111111));
        b.position.y = o.h / 2;
        const top = new THREE.Mesh(new THREE.CylinderGeometry(o.w * 0.3, o.w * 0.45, 0.25, 16), toon(0x222222));
        top.position.y = o.h + 0.1;
        const base = new THREE.Mesh(new THREE.BoxGeometry(o.w * 1.1, 0.3, o.w * 1.1), toon(0x9e9e9e));
        base.position.y = 0.15;
        g.add(base, b, top);
      } else if (o.kind === 'planter') {
        const b = new THREE.Mesh(new THREE.BoxGeometry(o.w, o.h, o.d), toon(0x8d6e63));
        b.position.y = o.h / 2;
        g.add(b);
        for (let i = 0; i < 4; i++) {
          const p = new THREE.Mesh(new THREE.SphereGeometry(0.28, 8, 6), toon(0x43a047));
          p.position.set(-o.w / 2 + 0.4 + i * (o.w - 0.8) / 3, o.h + 0.15, 0);
          g.add(p);
        }
      } else if (o.kind === 'gas') {
        const b = new THREE.Mesh(new THREE.CylinderGeometry(0.42, 0.42, o.h, 14), toon(0xe0e0e0));
        b.position.y = o.h / 2;
        const c = new THREE.Mesh(new THREE.CylinderGeometry(0.18, 0.18, 0.2, 10), toon(0x333333));
        c.position.y = o.h + 0.08;
        g.add(b, c);
      }
      this.group.add(g);
    }
  }

  // ── Peligros ───────────────────────────────────────────────
  _buildHazards() {
    this.hazards = [];
    for (const hz of this.def.hazards) {
      if (hz.type === 'laser') {
        const mat = new THREE.MeshBasicMaterial({ color: 0xff1744, transparent: true, opacity: 0, depthWrite: false });
        const beam = new THREE.Mesh(new THREE.BoxGeometry(this.w, 0.12, 0.12), mat);
        beam.position.y = 0.7;
        const glow = new THREE.Mesh(new THREE.PlaneGeometry(this.w, 0.9), new THREE.MeshBasicMaterial({ color: 0xff1744, transparent: true, opacity: 0, depthWrite: false }));
        glow.rotation.x = -Math.PI / 2; glow.position.y = 0.04;
        const emitters = [];
        for (const sx of [-1, 1]) {
          const e = new THREE.Mesh(new THREE.BoxGeometry(0.4, 1.2, 0.4), toon(0x37474f));
          e.position.set(sx * (this.w / 2 + 0.1), 0.6, 0);
          emitters.push(e);
        }
        const g = new THREE.Group();
        g.add(beam, glow, ...emitters);
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
        hz.beam.scale.y = 0.3;
        hz.glow.material.opacity = 0.12;
      } else if (hz.state === 'active') {
        hz.beam.material.opacity = 0.95;
        hz.beam.scale.y = 1.4;
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

  // ── Decoración (no colisiona) ──────────────────────────────
  _buildDecor() {
    const id = this.def.id;
    this.spinners = [];
    this.group.traverse((o) => { if (o.userData.spin) this.spinners.push(o); });
    if (id === 'azotea') {
      // tendedero con ropa de colores al fondo
      const line = new THREE.Mesh(new THREE.CylinderGeometry(0.02, 0.02, this.w - 2, 4), toon(0x333333));
      line.rotation.z = Math.PI / 2; line.position.set(0, 2.2, -this.h / 2 + 0.6);
      this.group.add(line);
      const cols = [0xff6a2b, 0x22c7e8, 0xffd23f, 0xe84393, 0x4cd964, 0xffffff];
      for (let i = 0; i < 9; i++) {
        const cl = new THREE.Mesh(new THREE.PlaneGeometry(0.8, 0.9), new THREE.MeshToonMaterial({ color: cols[i % 6], side: THREE.DoubleSide, gradientMap: null }));
        cl.position.set(-this.w / 2 + 2 + i * (this.w - 4) / 8, 1.75, -this.h / 2 + 0.6);
        this.group.add(cl);
      }
      // antena
      const ant = new THREE.Mesh(new THREE.CylinderGeometry(0.05, 0.05, 3, 6), toon(0x777777));
      ant.position.set(this.w / 2 - 1, 1.5, -this.h / 2 + 0.6);
      this.group.add(ant);
    }
    if (id === 'callejon') {
      // farol
      const pole = new THREE.Mesh(new THREE.CylinderGeometry(0.08, 0.1, 4, 8), toon(0x333333));
      pole.position.set(-this.w / 2 + 1, 2, -this.h / 2 + 0.6);
      const lamp = new THREE.Mesh(new THREE.SphereGeometry(0.3, 10, 8), new THREE.MeshBasicMaterial({ color: 0xfff1a8 }));
      lamp.position.set(-this.w / 2 + 1, 4, -this.h / 2 + 0.9);
      this.group.add(pole, lamp);
    }
  }

  animate(dt) {
    for (const s of this.spinners) s.rotation.y += dt * 0.6;
  }

  // ── Muerte súbita: muros laterales que avanzan ────────────
  _buildSuddenDeathWalls() {
    const H = this.h;
    const tex = hazardStripeTexture().clone();
    tex.needsUpdate = true;
    tex.wrapS = tex.wrapT = THREE.RepeatWrapping;
    tex.repeat.set(H / 2, 1);
    const mat = toon(0xffffff, tex);
    this.sdWalls = [-1, 1].map((side) => {
      const m = new THREE.Mesh(new THREE.BoxGeometry(0.6, 1.6, H), mat);
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
    const mat = toon(0xffffff, stencilTexture(css));
    const mesh = new THREE.Mesh(new THREE.BoxGeometry(T.len, T.height, T.thick), mat);
    mesh.position.set(x, T.height / 2, z);
    mesh.rotation.y = -Math.atan2(tz, tx);
    mesh.scale.y = 0.05;
    this.group.add(mesh);
    tw.mesh = mesh;
    this.tagWalls.push(tw);
    return tw;
  }

  updateTagWalls(dt) {
    for (const tw of this.tagWalls) {
      tw.life -= dt;
      const age = TUNING.tagWall.life - tw.life;
      tw.mesh.scale.y = Math.min(1, 0.05 + age * 8) * (tw.life < 0.25 ? Math.max(0.05, tw.life / 0.25) : 1);
    }
    for (const tw of this.tagWalls) if (tw.life <= 0) this.group.remove(tw.mesh);
    this.tagWalls = this.tagWalls.filter((tw) => tw.life > 0);
  }

  clearTagWalls() {
    for (const tw of this.tagWalls) this.group.remove(tw.mesh);
    this.tagWalls = [];
  }

  // ── Consultas de colisión ──────────────────────────────────
  // Empuja un círculo fuera de muros y obstáculos (solo los que bloquean jugadores)
  resolveCircle(p, r) {
    const hw = this.w / 2 - this.inset - r, hh = this.h / 2 - r;
    let hit = false;
    if (p.x < -hw) { p.x = -hw; hit = true; }
    if (p.x > hw) { p.x = hw; hit = true; }
    if (p.z < -hh) { p.z = -hh; hit = true; }
    if (p.z > hh) { p.z = hh; hit = true; }
    for (const o of this.obstacles) {
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

  isFree(x, z, r) {
    if (Math.abs(x) > this.w / 2 - this.inset - r || Math.abs(z) > this.h / 2 - r) return false;
    for (const o of this.obstacles) {
      if (Math.abs(x - o.x) < o.w / 2 + r && Math.abs(z - o.z) < o.d / 2 + r) return false;
    }
    for (const tw of this.tagWalls) {
      const c = closestOnSegment(x, z, tw);
      if (Math.hypot(x - c.x, z - c.z) < r + tw.half) return false;
    }
    return true;
  }

  randomFreePoint(r = 0.8) {
    for (let i = 0; i < 60; i++) {
      const x = (this._r() - 0.5) * (this.w - 3 - this.inset * 2);
      const z = (this._r() - 0.5) * (this.h - 3);
      if (this.isFree(x, z, r)) return { x, z };
    }
    return { x: 0, z: 2 };
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
