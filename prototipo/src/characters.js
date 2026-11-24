// ─────────────────────────────────────────────────────────────
// PERSONAJES "MUÑEQUITOS" ESTILO ART TOY
// Cabeza grande de vinil, cuerpo chiquito con playera (textura
// intercambiable), manos de guante flotantes y la GORRA como arma.
// Sin gorra en la cabeza = estás desarmado (lectura instantánea).
// ─────────────────────────────────────────────────────────────
import * as THREE from 'three';
import { toonRamp, shirtTexture, capTexture, CAP_DESIGNS, shirtLook, capLogoTexture } from './textures.js';

const OUTLINE_MAT = new THREE.MeshBasicMaterial({ color: 0x111111, side: THREE.BackSide });

// Tonos de "vinil" para las cabezas (cada especie tiene el suyo).
// Nombres del GDD §8. Los índices 0–5 no cambian (progreso guardado);
// Lienzo y Axo se agregan al final.
export const SPECIES = [
  { id: 'oso', name: 'Osote', vinyl: 0xf3e4cf, ears: 'round' },
  { id: 'gato', name: 'Michi', vinyl: 0x2f2f36, ears: 'cat' },
  { id: 'gota', name: 'Gotita', vinyl: 0xbfe7ff, ears: 'none' },
  { id: 'conejo', name: 'Seri', vinyl: 0xffd6e5, ears: 'bunny' },
  { id: 'alien', name: 'Zurci', vinyl: 0xb8f28a, ears: 'antenna' },
  { id: 'diablito', name: 'Pegote', vinyl: 0xff8f70, ears: 'horns' },
  { id: 'lienzo', name: 'Lienzo', vinyl: 0xf7f7f2, ears: 'seam' },
  { id: 'ajolote', name: 'Axo', vinyl: 0xf6a9c9, ears: 'gills' },
];
// Orden en el vestidor (gratis primero, luego por nivel)
export const SPECIES_ORDER = [6, 0, 1, 2, 3, 4, 5, 7];

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
  seam: new THREE.TorusGeometry(0.47, 0.018, 6, 32),
  gill: new THREE.CapsuleGeometry(0.035, 0.16, 3, 6),
  capCrown: new THREE.SphereGeometry(0.36, 18, 10, 0, Math.PI * 2, 0, Math.PI / 2),
  capVisor: new THREE.CylinderGeometry(0.3, 0.3, 0.04, 18, 1, false, -Math.PI / 2, Math.PI),
  capButton: new THREE.SphereGeometry(0.05, 8, 6),
  // v0.3: muñequito mejorado (piernas, tenis, mangas, cara)
  leg: new THREE.CylinderGeometry(0.07, 0.08, 0.2, 10),
  sole: new THREE.CapsuleGeometry(0.075, 0.13, 4, 10),
  shoeTop: new THREE.SphereGeometry(0.09, 10, 8),
  sleeve: new THREE.CylinderGeometry(0.095, 0.115, 0.15, 12),
  mouth: new THREE.TorusGeometry(0.055, 0.013, 6, 14, Math.PI),
  blush: new THREE.CircleGeometry(0.055, 14),
  neck: new THREE.TorusGeometry(0.2, 0.02, 6, 24),
  mask: new THREE.SphereGeometry(0.2, 14, 10),
  filter: new THREE.CylinderGeometry(0.07, 0.07, 0.07, 12),
  nose: new THREE.SphereGeometry(0.035, 8, 6),
  tooth: new THREE.BoxGeometry(0.05, 0.05, 0.02),
  stitch: new THREE.BoxGeometry(0.012, 0.09, 0.012),
  drip: new THREE.SphereGeometry(0.06, 10, 8),
  dripTail: new THREE.ConeGeometry(0.045, 0.12, 8),
  arrow: new THREE.ConeGeometry(0.06, 0.12, 4),
  patch: new THREE.CylinderGeometry(0.075, 0.075, 0.012, 20),
  // calcomanías: estampado de playera (frente/espalda) y logo del panel de la gorra
  print: new THREE.CylinderGeometry(0.281, 0.281, 0.4, 18, 1, true, -0.95, 1.9),
  capLogo: new THREE.SphereGeometry(0.364, 18, 8, Math.PI / 2 - 0.66, 1.32, 0.5, 0.78),
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
// design: id de CAP_DESIGNS. El diseño vive SOLO en la copa: la silueta y
// el tamaño nunca cambian, y el color del jugador sigue en el aro y el rastro.
export function buildCap(color, design = 'clasica') {
  const g = new THREE.Group();
  const def = CAP_DESIGNS[design];
  const usesTexture = def && !def.playerColor;
  const decal = def && def.crown != null;            // gorra real: copa lisa + logo bordado al frente
  const mat = decal ? toon(def.crown) : usesTexture ? toon(0xffffff, capTexture(design)) : toon(color);
  const visorColor = usesTexture ? new THREE.Color(def.visor ?? 0x222222) : new THREE.Color(color).multiplyScalar(0.75);
  const crown = withOutline(new THREE.Mesh(G.capCrown, mat), 1.06);
  crown.scale.set(1, 0.72, 1);
  if (decal) {
    const logo = new THREE.Mesh(G.capLogo, new THREE.MeshToonMaterial({
      map: capLogoTexture(design), transparent: true, alphaTest: 0.05, gradientMap: toonRamp(),
      polygonOffset: true, polygonOffsetFactor: -2,
    }));
    crown.add(logo);
  }
  // visera plana de snapback (como las gorras reales de INKCAP)
  const visor = withOutline(new THREE.Mesh(G.capVisor, toon(visorColor)), 1.08);
  visor.position.set(0, -0.005, 0.17);
  visor.scale.set(1.06, 0.8, 1.38);
  const button = new THREE.Mesh(G.capButton, toon(usesTexture ? (def.button ?? visorColor.getHex()) : 0xffffff));
  button.position.y = 0.26;
  g.add(crown, visor, button);
  if (def && def.patch) {
    // parche redondo blanco "diseñado por artistas · hecho en México" sobre la visera
    const patch = new THREE.Mesh(G.patch, toon(0xf4f4f0));
    patch.position.set(0.13, 0.012, 0.43);
    g.add(patch);
  }
  g.userData.mat = mat;
  g.userData.textured = usesTexture;
  return g;
}

// ── El muñequito completo ────────────────────────────────────
export class Toy {
  constructor({ color = 0xff6a2b, speciesIndex = 0, shirt = 'basica-blanca', capDesign = 'clasica' } = {}) {
    this.color = color;
    this.species = SPECIES[speciesIndex % SPECIES.length];
    this.root = new THREE.Group();
    this.pivot = new THREE.Group(); // aquí aplicamos rebote y squash
    this.root.add(this.pivot);

    // piernas de jogger + tenis (suela blanca, parte de arriba negra)
    const pantMat = toon(0x2a2d36);
    const soleMat = toon(0xf4f4f0);
    const shoeMat = toon(0x1c1b20);
    const makeShoe = () => {
      const shoe = new THREE.Group();
      const sole = withOutline(new THREE.Mesh(G.sole, soleMat), 1.08);
      sole.rotation.x = Math.PI / 2; sole.scale.set(1, 1, 0.55); sole.position.y = -0.02;
      const top = new THREE.Mesh(G.shoeTop, shoeMat); top.scale.set(0.9, 0.6, 1.15); top.position.set(0, 0.03, -0.01);
      shoe.add(sole, top);
      return shoe;
    };
    this.footL = makeShoe(); this.footR = makeShoe();
    this.footL.position.set(-0.14, 0.08, 0.03);
    this.footR.position.set(0.14, 0.08, 0.03);
    this.legL = new THREE.Mesh(G.leg, pantMat); this.legR = new THREE.Mesh(G.leg, pantMat);
    this.legL.position.set(-0.13, 0.2, 0); this.legR.position.set(0.13, 0.2, 0);
    this.pivot.add(this.footL, this.footR, this.legL, this.legR);

    // cuerpo con playera
    this.shirtMat = toon(0xffffff);
    this.sleeveMat = toon(0xffffff);
    this.body = withOutline(new THREE.Mesh(G.body, this.shirtMat), 1.08);
    this.body.position.y = 0.48;
    this.pivot.add(this.body);
    // estampado al frente y en la espalda (calcomanía curva, no se estira)
    const printMat = () => new THREE.MeshToonMaterial({ transparent: true, alphaTest: 0.04, gradientMap: toonRamp(), polygonOffset: true, polygonOffsetFactor: -2 });
    this.printFront = new THREE.Mesh(G.print, printMat());
    this.printBack = new THREE.Mesh(G.print, printMat());
    this.printFront.position.y = 0.03; this.printBack.position.y = 0.03;
    this.printBack.rotation.y = Math.PI;
    this.body.add(this.printFront, this.printBack);
    // mangas de playera: hacen que el cuerpo se lea como una playera de verdad
    for (const sx of [-1, 1]) {
      const sl = withOutline(new THREE.Mesh(G.sleeve, this.sleeveMat), 1.1);
      sl.position.set(0.29 * sx, 0.6, 0);
      sl.rotation.z = 0.95 * sx;
      this.pivot.add(sl);
    }

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
    this._addFace();
    this.pivot.add(this.headGroup);
    this.setShirt(shirt);

    // gorra en la cabeza (se oculta cuando la lanzas)
    this.cap = buildCap(color, capDesign);
    this.cap.scale.setScalar(1.32);       // cubre la coronilla como una gorra de verdad
    this.cap.position.set(0, 0.13, 0.0);
    this.cap.rotation.x = -0.1;
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
    } else if (kind === 'seam') {
      // Lienzo: cabeza lisa con la unión del molde marcada (la "plataforma" de los artistas)
      const seam = new THREE.Mesh(G.seam, toon(0xd8d8d0));
      seam.scale.set(1.05, 0.92, 0.98); // anillo de oreja a oreja (por los lados, no por la cara)
      h.add(seam);
    } else if (kind === 'gills') {
      // Axo: branquias rosas como brochas, tres por lado
      const gm = toon(0xe0457b);
      for (const sx of [-1, 1]) for (let k = 0; k < 3; k++) {
        const e = add(new THREE.Mesh(G.gill, gm));
        e.position.set(0.44 * sx, 0.12 - k * 0.12, -0.04);
        e.rotation.z = sx * (1.0 - k * 0.45);
      }
    } else if (kind === 'horns') {
      for (const sx of [-1, 1]) { const e = add(new THREE.Mesh(G.horn, toon(0xfff1e0))); e.position.set(0.3 * sx, 0.36, 0.05); e.rotation.z = -0.5 * sx; }
    }
  }

  // Boca, chapitas y el detalle que hace único a cada muñequito (GDD §6.1)
  _addFace() {
    const h = this.headGroup;
    const id = this.species.id;
    const dark = this.species.vinyl === 0x2f2f36;
    const lineMat = new THREE.MeshBasicMaterial({ color: dark ? 0xfff3b0 : 0x141414 });
    if (id !== 'oso') {
      const m = new THREE.Mesh(G.mouth, lineMat);
      m.position.set(0, -0.13, 0.42); m.rotation.set(0.15, 0, Math.PI);
      if (id === 'ajolote') m.scale.set(1.6, 1, 1);
      h.add(m);
    }
    if (!dark && id !== 'oso') {
      const bm = new THREE.MeshBasicMaterial({ color: 0xff8fb1, transparent: true, opacity: 0.55, depthWrite: false });
      for (const sx of [-1, 1]) { const b = new THREE.Mesh(G.blush, bm); b.position.set(0.25 * sx, -0.08, 0.385); b.rotation.y = 0.55 * sx; h.add(b); }
    }
    const vinyl = toon(this.species.vinyl);
    if (id === 'oso') {
      // Osote: respirador de pintor de grafiti
      const mask = withOutline(new THREE.Mesh(G.mask, toon(0x8a9099)), 1.06);
      mask.scale.set(1.15, 0.62, 0.7); mask.position.set(0, -0.16, 0.33); h.add(mask);
      for (const sx of [-1, 1]) {
        const f = withOutline(new THREE.Mesh(G.filter, toon(0xd55e00)), 1.08);
        f.rotation.x = Math.PI / 2; f.rotation.z = 0.5 * sx; f.position.set(0.17 * sx, -0.18, 0.43); h.add(f);
      }
    } else if (id === 'gato' || id === 'diablito') {
      // Michi y Pegote: cola en "S" (la de Pegote termina en flecha)
      const curve = new THREE.CatmullRomCurve3([
        new THREE.Vector3(0, 0.42, -0.24), new THREE.Vector3(0.12, 0.5, -0.42),
        new THREE.Vector3(-0.08, 0.72, -0.5), new THREE.Vector3(0.05, 0.9, -0.46),
      ]);
      const tail = withOutline(new THREE.Mesh(new THREE.TubeGeometry(curve, 16, 0.035, 6), id === 'gato' ? vinyl : toon(0xc0392b)), 1.15);
      this.pivot.add(tail);
      if (id === 'diablito') {
        const tip = new THREE.Mesh(G.arrow, toon(0xc0392b)); tip.position.set(0.05, 0.98, -0.46); this.pivot.add(tip);
        const st = new THREE.Mesh(G.blush, new THREE.MeshBasicMaterial({ color: 0xf0e442 }));
        st.position.set(-0.27, 0.02, 0.36); st.rotation.y = -0.6; st.scale.setScalar(0.8); h.add(st); // sticker en el cachete
      }
    } else if (id === 'gota') {
      // Gotita: una gota de tinta escurriendo por la cabeza
      const d = new THREE.Mesh(G.drip, toon(0x0072b2)); d.position.set(0.3, -0.08, 0.3); h.add(d);
      const t = new THREE.Mesh(G.dripTail, toon(0x0072b2)); t.position.set(0.3, 0.0, 0.3); h.add(t);
    } else if (id === 'conejo') {
      // Seri: nariz y dientes de conejo
      const n = new THREE.Mesh(G.nose, toon(0xe84393)); n.position.set(0, -0.06, 0.45); h.add(n);
      const tt = new THREE.Mesh(G.tooth, toon(0xffffff)); tt.position.set(0, -0.19, 0.41); h.add(tt);
    } else if (id === 'alien') {
      // Zurci: costuras de bordado en la cabeza
      const sm = toon(0x3b3b3b);
      for (let k = 0; k < 4; k++) {
        const st = new THREE.Mesh(G.stitch, sm);
        st.position.set(-0.36 + k * 0.05, 0.16 - k * 0.03, 0.22); st.rotation.set(0.4, -0.9, 0.9); h.add(st);
      }
    } else if (id === 'lienzo') {
      // Lienzo: unión del cuello visible, como un art toy de vinil sin pintar
      const nk = new THREE.Mesh(G.neck, toon(0xd8d8d0)); nk.rotation.x = Math.PI / 2; nk.position.y = 0.74; this.pivot.add(nk);
    }
  }

  setShirt(id) {
    const look = shirtLook(id);
    this.shirtMat.map = look.wrap;
    this.shirtMat.color.set(look.wrap ? 0xffffff : (look.fabric || '#f2f2ee'));
    this.sleeveMat.map = look.wrap;
    this.sleeveMat.color.set(look.wrap ? 0xffffff : (look.fabric || '#f2f2ee'));
    for (const m of [this.printFront, this.printBack]) {
      m.visible = !!look.print;
      m.material.map = look.print;
      m.material.needsUpdate = true;
    }
    this.shirtMat.needsUpdate = true;
    this.sleeveMat.needsUpdate = true;
  }

  setCapColor(hex) {
    if (this.cap.userData.textured) return; // una gorra con diseño no se tiñe
    this.cap.traverse((o) => { if (o.material && o.material.color && o.material !== OUTLINE_MAT && o.geometry === G.capCrown) o.material.color.setHex(hex); });
  }

  setPower(colorHex) {
    if (colorHex == null) { this.powerRing.material.opacity = 0; return; }
    this.powerRing.material.color.setHex(colorHex);
    this.powerRing.material.opacity = 0.85;
  }

  // Podio: el ganador levanta la taza INKCAP con las dos manos (spec v0.3 §C)
  holdTrophy(obj) {
    this.trophy = obj;
    this.pivot.add(obj);
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
    if (this.legL) {
      this.legL.position.z = this.footL.position.z * 0.6; this.legR.position.z = this.footR.position.z * 0.6;
      this.legL.rotation.x = step * 0.9; this.legR.rotation.x = -step * 0.9;
    }

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
    if (s.cheer) {
      // festejo: las dos manos arriba (sosteniendo el trofeo si lo trae)
      const b = Math.sin(this.t * 6.5) * 0.035;
      this.handL.position.set(-0.24, 1.86 + b, 0.1);
      this.handR.position.set(0.24, 1.86 + b, 0.1);
      if (this.trophy) {
        this.trophy.position.set(0, 2.02 + b, 0.1);
        this.trophy.rotation.z = Math.sin(this.t * 3.2) * 0.12;
      }
    }

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
