// ─────────────────────────────────────────────────────────────
// PERSONAJES "MUÑEQUITOS" ESTILO ART TOY
// Cabeza grande de vinil, cuerpo chiquito con playera (textura
// intercambiable), manos de guante flotantes y la GORRA como arma.
// Sin gorra en la cabeza = estás desarmado (lectura instantánea).
// ─────────────────────────────────────────────────────────────
import * as THREE from 'three';
import { toonRamp, shirtTexture } from './textures.js';

const OUTLINE_MAT = new THREE.MeshBasicMaterial({ color: 0x111111, side: THREE.BackSide });

// Tonos de "vinil" para las cabezas (cada especie tiene el suyo)
export const SPECIES = [
  { id: 'oso', name: 'Oso', vinyl: 0xf3e4cf, ears: 'round' },
  { id: 'gato', name: 'Gato', vinyl: 0x2f2f36, ears: 'cat' },
  { id: 'gota', name: 'Gota', vinyl: 0xbfe7ff, ears: 'none' },
  { id: 'conejo', name: 'Conejo', vinyl: 0xffd6e5, ears: 'bunny' },
  { id: 'alien', name: 'Alien', vinyl: 0xb8f28a, ears: 'antenna' },
  { id: 'diablito', name: 'Diablito', vinyl: 0xff8f70, ears: 'horns' },
];

export function toon(color, map = null) {
  return new THREE.MeshToonMaterial({ color, map, gradientMap: toonRamp() });
}

function withOutline(mesh, thickness = 1.07) {
  const o = new THREE.Mesh(mesh.geometry, OUTLINE_MAT);
  o.scale.setScalar(thickness);
  o.raycast = () => {};
  mesh.add(o);
  return mesh;
}

// Geometrías compartidas (se crean una sola vez → menos memoria)
const G = {
  head: new THREE.SphereGeometry(0.46, 20, 16),
  body: new THREE.CapsuleGeometry(0.27, 0.22, 6, 14),
  foot: new THREE.SphereGeometry(0.13, 10, 8),
  hand: new THREE.SphereGeometry(0.11, 10, 8),
  eye: new THREE.SphereGeometry(0.075, 10, 8),
  shine: new THREE.SphereGeometry(0.025, 6, 4),
  earRound: new THREE.SphereGeometry(0.15, 10, 8),
  earCat: new THREE.ConeGeometry(0.14, 0.26, 4),
  earBunny: new THREE.CapsuleGeometry(0.08, 0.34, 4, 8),
  antenna: new THREE.CylinderGeometry(0.02, 0.02, 0.3, 6),
  antennaTip: new THREE.SphereGeometry(0.07, 8, 6),
  horn: new THREE.ConeGeometry(0.07, 0.22, 8),
  capCrown: new THREE.SphereGeometry(0.36, 18, 10, 0, Math.PI * 2, 0, Math.PI / 2),
  capVisor: new THREE.CylinderGeometry(0.3, 0.3, 0.04, 18, 1, false, -Math.PI / 2, Math.PI),
  capButton: new THREE.SphereGeometry(0.05, 8, 6),
  ring: new THREE.RingGeometry(0.62, 0.74, 32),
  ring2: new THREE.RingGeometry(0.8, 0.86, 32),
};

let _shadowTex = null;
export function blobShadowTexture() {
  if (_shadowTex) return _shadowTex;
  const c = document.createElement('canvas');
  c.width = c.height = 64;
  const ctx = c.getContext('2d');
  const g = ctx.createRadialGradient(32, 32, 2, 32, 32, 30);
  g.addColorStop(0, 'rgba(0,0,0,0.55)');
  g.addColorStop(1, 'rgba(0,0,0,0)');
  ctx.fillStyle = g; ctx.fillRect(0, 0, 64, 64);
  _shadowTex = new THREE.CanvasTexture(c);
  return _shadowTex;
}

export function makeBlobShadow(size = 1.3) {
  const m = new THREE.Mesh(
    new THREE.PlaneGeometry(size, size),
    new THREE.MeshBasicMaterial({ map: blobShadowTexture(), transparent: true, depthWrite: false })
  );
  m.rotation.x = -Math.PI / 2;
  m.position.y = 0.02;
  m.renderOrder = 1;
  return m;
}

// ── La gorra (sirve en la cabeza y como proyectil) ───────────
export function buildCap(color) {
  const g = new THREE.Group();
  const mat = toon(color);
  const crown = withOutline(new THREE.Mesh(G.capCrown, mat), 1.06);
  crown.scale.set(1, 0.72, 1);
  const visor = withOutline(new THREE.Mesh(G.capVisor, toon(new THREE.Color(color).multiplyScalar(0.75))), 1.08);
  visor.position.set(0, 0.0, 0.16);
  visor.scale.set(1, 1, 1.25);
  const button = new THREE.Mesh(G.capButton, toon(0xffffff));
  button.position.y = 0.26;
  g.add(crown, visor, button);
  g.userData.mat = mat;
  return g;
}

// ── El muñequito completo ────────────────────────────────────
export class Toy {
  constructor({ color = 0xff6a2b, speciesIndex = 0, shirt = 'basica-blanca' } = {}) {
    this.color = color;
    this.species = SPECIES[speciesIndex % SPECIES.length];
    this.root = new THREE.Group();
    this.pivot = new THREE.Group(); // aquí aplicamos rebote y squash
    this.root.add(this.pivot);

    // pies
    const footMat = toon(0x26262b);
    this.footL = new THREE.Mesh(G.foot, footMat); this.footL.scale.set(1, 0.6, 1.3);
    this.footR = this.footL.clone();
    this.footL.position.set(-0.16, 0.08, 0.02);
    this.footR.position.set(0.16, 0.08, 0.02);
    this.pivot.add(withOutline(this.footL), withOutline(this.footR));

    // cuerpo con playera
    this.shirtMat = toon(0xffffff, shirtTexture(shirt));
    this.body = withOutline(new THREE.Mesh(G.body, this.shirtMat), 1.08);
    this.body.position.y = 0.48;
    this.pivot.add(this.body);

    // cabeza de vinil
    this.headGroup = new THREE.Group();
    this.headGroup.position.y = 1.08;
    const headMat = toon(this.species.vinyl);
    this.head = withOutline(new THREE.Mesh(G.head, headMat), 1.05);
    this.head.scale.set(1.05, 0.92, 0.98);
    this.headGroup.add(this.head);
    this._addEars(headMat);

    // ojos grandes (lo más importante para que se vea "tierno")
    const eyeMat = new THREE.MeshBasicMaterial({ color: this.species.vinyl === 0x2f2f36 ? 0xfff3b0 : 0x141414 });
    const shineMat = new THREE.MeshBasicMaterial({ color: 0xffffff });
    this.eyes = [];
    for (const sx of [-0.16, 0.16]) {
      const e = new THREE.Mesh(G.eye, eyeMat);
      e.scale.set(0.85, 1.25, 0.6);
      e.position.set(sx, 0.02, 0.41);
      const s = new THREE.Mesh(G.shine, shineMat);
      s.position.set(0.025, 0.035, 0.06);
      e.add(s);
      this.headGroup.add(e);
      this.eyes.push(e);
    }
    this.pivot.add(this.headGroup);

    // gorra en la cabeza (se oculta cuando la lanzas)
    this.cap = buildCap(color);
    this.cap.position.set(0, 0.3, -0.02);
    this.cap.rotation.x = -0.12;
    this.headGroup.add(this.cap);

    // manos tipo guante flotante
    const handMat = toon(0xffffff);
    this.handL = withOutline(new THREE.Mesh(G.hand, handMat));
    this.handR = withOutline(new THREE.Mesh(G.hand, handMat));
    this.handL.position.set(-0.4, 0.5, 0.05);
    this.handR.position.set(0.4, 0.5, 0.05);
    this.pivot.add(this.handL, this.handR);

    // aro de color del jugador en el piso + aro de power-up
    this.ring = new THREE.Mesh(G.ring, new THREE.MeshBasicMaterial({ color, transparent: true, opacity: 0.9, depthWrite: false }));
    this.ring.rotation.x = -Math.PI / 2; this.ring.position.y = 0.03;
    this.powerRing = new THREE.Mesh(G.ring2, new THREE.MeshBasicMaterial({ color: 0xffffff, transparent: true, opacity: 0.0, depthWrite: false }));
    this.powerRing.rotation.x = -Math.PI / 2; this.powerRing.position.y = 0.035;
    this.root.add(this.ring, this.powerRing, makeBlobShadow(1.4));

    // escudo (parche bordado)
    this.shield = new THREE.Mesh(
      new THREE.SphereGeometry(0.95, 18, 12),
      new THREE.MeshBasicMaterial({ color: 0xffb300, transparent: true, opacity: 0.22, depthWrite: false, wireframe: true })
    );
    this.shield.position.y = 0.8;
    this.shield.visible = false;
    this.root.add(this.shield);

    // indicador de apuntado / carga (vive en la escena, no dentro del root,
    // para no heredar la rotación del personaje). El juego lo agrega a la escena.
    const aimGeo = new THREE.PlaneGeometry(0.24, 1);
    aimGeo.translate(0, -0.5, 0);
    aimGeo.rotateX(-Math.PI / 2); // ahora se extiende hacia +Z
    this.aim = new THREE.Mesh(
      aimGeo,
      new THREE.MeshBasicMaterial({ color, transparent: true, opacity: 0.55, depthWrite: false })
    );
    this.aim.visible = false;
    this.aim.renderOrder = 2;

    this.t = Math.random() * 10;
    this.walkPhase = 0;
    this.blinkT = 2 + Math.random() * 3;
    this.throwAnim = 0;
    this.meleeAnim = 0;
    this.squash = 0;
  }

  _addEars(mat) {
    const h = this.headGroup;
    const kind = this.species.ears;
    const add = (m) => { h.add(withOutline(m)); return m; };
    if (kind === 'round') {
      for (const sx of [-1, 1]) { const e = add(new THREE.Mesh(G.earRound, mat)); e.position.set(0.33 * sx, 0.3, -0.05); e.scale.set(1, 1, 0.6); }
    } else if (kind === 'cat') {
      for (const sx of [-1, 1]) { const e = add(new THREE.Mesh(G.earCat, mat)); e.position.set(0.3 * sx, 0.38, -0.02); e.rotation.z = -0.35 * sx; }
    } else if (kind === 'bunny') {
      for (const sx of [-1, 1]) { const e = add(new THREE.Mesh(G.earBunny, mat)); e.position.set(0.17 * sx, 0.62, -0.08); e.rotation.z = -0.18 * sx; e.rotation.x = -0.15; }
    } else if (kind === 'antenna') {
      for (const sx of [-1, 1]) {
        const a = add(new THREE.Mesh(G.antenna, toon(0x333333))); a.position.set(0.18 * sx, 0.52, 0); a.rotation.z = -0.4 * sx;
        const tip = add(new THREE.Mesh(G.antennaTip, toon(0xfff35c))); tip.position.set(0.25 * sx, 0.66, 0);
      }
    } else if (kind === 'horns') {
      for (const sx of [-1, 1]) { const e = add(new THREE.Mesh(G.horn, toon(0xfff1e0))); e.position.set(0.3 * sx, 0.36, 0.05); e.rotation.z = -0.5 * sx; }
    }
  }

  setShirt(id) {
    this.shirtMat.map = shirtTexture(id);
    this.shirtMat.needsUpdate = true;
  }

  setCapColor(hex) {
    this.cap.traverse((o) => { if (o.material && o.material.color && o.material !== OUTLINE_MAT && o.geometry === G.capCrown) o.material.color.setHex(hex); });
  }

  setPower(colorHex) {
    if (colorHex == null) { this.powerRing.material.opacity = 0; return; }
    this.powerRing.material.color.setHex(colorHex);
    this.powerRing.material.opacity = 0.85;
  }

  triggerThrow() { this.throwAnim = 1; }
  triggerMelee() { this.meleeAnim = 1; }
  triggerSquash(v = 1) { this.squash = v; }

  // estado: { speed01, dashing, charging, charge, hasCap, aimDir:{x,z}|null, alive }
  update(dt, s) {
    this.t += dt;
    const moving = s.speed01 > 0.08;
    this.walkPhase += dt * (moving ? 16 * Math.max(0.4, s.speed01) : 0);
    const bob = moving ? Math.abs(Math.sin(this.walkPhase)) * 0.09 : Math.sin(this.t * 2.2) * 0.02;

    // squash & stretch
    this.squash = Math.max(0, this.squash - dt * 5);
    let sy = 1 + Math.sin(this.t * 2.2) * 0.015;
    let sxz = 1;
    if (s.dashing) { sy = 0.8; sxz = 1.15; }
    if (this.squash > 0) { sy -= this.squash * 0.25; sxz += this.squash * 0.18; }
    if (s.charging) { sy -= s.charge * 0.1; sxz += s.charge * 0.06; }
    this.pivot.scale.set(sxz, sy, sxz);
    this.pivot.position.y = bob;
    this.pivot.rotation.x = s.dashing ? 0.35 : (moving ? 0.1 * s.speed01 : 0);

    // pies alternados
    const step = moving ? Math.sin(this.walkPhase) * 0.18 : 0;
    this.footL.position.z = 0.02 + step;
    this.footR.position.z = 0.02 - step;
    this.footL.position.y = 0.08 + Math.max(0, Math.sin(this.walkPhase)) * 0.06;
    this.footR.position.y = 0.08 + Math.max(0, -Math.sin(this.walkPhase)) * 0.06;

    // manos: balanceo, carga (mano atrás), lanzamiento y golpe
    this.throwAnim = Math.max(0, this.throwAnim - dt * 6);
    this.meleeAnim = Math.max(0, this.meleeAnim - dt * 5);
    const swing = moving ? Math.sin(this.walkPhase) * 0.12 : 0;
    this.handL.position.set(-0.4, 0.5 + Math.sin(this.t * 3) * 0.02, 0.05 - swing);
    let rz = 0.05 + swing, ry = 0.5, rx = 0.4;
    if (s.charging) { rz = -0.25 - s.charge * 0.2; ry = 0.95; rx = 0.35; }
    if (this.throwAnim > 0) { rz = 0.6 * this.throwAnim; ry = 0.9; rx = 0.25; }
    if (this.meleeAnim > 0) {
      const a = (1 - this.meleeAnim) * Math.PI * 1.1 - 0.6;
      rx = Math.sin(a) * 0.75; rz = Math.cos(a) * 0.75; ry = 0.7;
    }
    this.handR.position.set(rx, ry, rz);

    // parpadeo
    this.blinkT -= dt;
    const blink = this.blinkT < 0.1;
    if (this.blinkT < 0) this.blinkT = 2 + Math.random() * 4;
    for (const e of this.eyes) e.scale.y = blink ? 0.15 : 1.25;

    // gorra visible solo si la tienes
    this.cap.visible = s.hasCap;
    this.shield.visible = !!s.shield;
    if (s.shield) this.shield.rotation.y += dt * 1.5;

    // aro del jugador late suavecito
    this.ring.material.opacity = 0.65 + Math.sin(this.t * 4) * 0.2;
    if (this.powerRing.material.opacity > 0) this.powerRing.rotation.z += dt * 2;

    // línea de apuntado
    if (s.aimDir && (s.charging || s.aiming) && s.alive !== false) {
      this.aim.visible = true;
      const len = 2.2 + (s.charge || 0) * 5.5;
      this.aim.scale.set(1, 1, len);
      this.aim.position.set(this.root.position.x, 0.06, this.root.position.z);
      this.aim.rotation.y = Math.atan2(s.aimDir.x, s.aimDir.z);
      this.aim.material.opacity = 0.3 + (s.charge || 0) * 0.5;
    } else {
      this.aim.visible = false;
    }
  }

  // Piezas que salen volando al quedar KO (el juguete "se desarma")
  makeDebris() {
    const pieces = [];
    this.root.updateMatrixWorld(true);
    const make = (src) => {
      const clone = src.clone(true);
      src.getWorldPosition(clone.position);
      src.getWorldQuaternion(clone.quaternion);
      const ws = new THREE.Vector3(); src.getWorldScale(ws); clone.scale.copy(ws);
      clone.visible = true;
      return clone;
    };
    pieces.push({ obj: make(this.headGroup), r: 0.42 });
    pieces.push({ obj: make(this.body), r: 0.3 });
    pieces.push({ obj: make(this.handL), r: 0.1 });
    pieces.push({ obj: make(this.handR), r: 0.1 });
    return pieces;
  }
}
