// ─────────────────────────────────────────────────────────────
// ESCENOGRAFÍA (spec v0.3 §B y §C)
// El "look" de cada escenario: piso pintado, muro del fondo, obstáculos,
// utilería fuera del área de juego y, sobre todo, el mundo INKCAP:
// mural y paste-ups en el callejón, exposición en el museo, espectacular
// y tendedero en la azotea, y la Pop-up Store completa.
// Nada de aquí cambia las reglas: es pura vista. Para que corra a 30 fps
// en un celular de gama media: geometrías unitarias compartidas (geo.js),
// un material por color, casi todo "horneado" en 2 texturas por arena
// (piso y muro del fondo) y cero sombras dinámicas.
// Si la marca no trae logo o diseños de artistas, todo se dibuja con
// texto o colores lisos (no se inventan logos).
// ─────────────────────────────────────────────────────────────
import * as THREE from 'three';
import { GEO } from './geo.js';
import { buildCap } from './characters.js';
import { MARCA } from './marca.js';
import {
  FONTS, drawLogo, drawSplat, drawDrips, TAG_WORDS, makeCanvas, toCanvasTexture, toonRamp,
  graffitiWallTexture, rooftopWallTexture, crateTexture, logoStickerTexture, softDotTexture,
  hangingShirtTexture, brandShirtIds, brandCapIds, mugTexture, dropBoxTexture,
} from './textures.js';

const INK = '#121016', BONE = '#f4f1ea', LIME = '#ccff00', RED = '#cc1111';
const LIME_HEX = 0xccff00;
const brandFont = (px) => `italic 900 ${px}px ${FONTS.brand}`;
const artists = () => (MARCA.artistas || []).filter((a) => a.imgEl);

// ── Utilidades de dibujo en canvas ───────────────────────────
function rr(ctx, x, y, w, h, r) {
  ctx.beginPath();
  if (ctx.roundRect) { ctx.roundRect(x, y, w, h, r); return; }
  ctx.moveTo(x + r, y); ctx.lineTo(x + w - r, y); ctx.quadraticCurveTo(x + w, y, x + w, y + r);
  ctx.lineTo(x + w, y + h - r); ctx.quadraticCurveTo(x + w, y + h, x + w - r, y + h);
  ctx.lineTo(x + r, y + h); ctx.quadraticCurveTo(x, y + h, x, y + h - r);
  ctx.lineTo(x, y + r); ctx.quadraticCurveTo(x, y, x + r, y);
  ctx.closePath();
}

function speckle(ctx, W, H, r, n, alpha, light = true, size = 2) {
  for (let i = 0; i < n; i++) {
    ctx.fillStyle = `rgba(${light ? '255,255,255' : '0,0,0'},${r() * alpha})`;
    ctx.fillRect(r() * W, r() * H, size, size);
  }
}

// charco de luz (lámparas, reflectores): se suma al color del piso o del muro
function lightPool(ctx, x, y, radius, rgb, alpha, sy = 1) {
  ctx.save();
  ctx.globalCompositeOperation = 'lighter';
  ctx.translate(x, y); ctx.scale(1, sy);
  const g = ctx.createRadialGradient(0, 0, 0, 0, 0, radius);
  g.addColorStop(0, `rgba(${rgb},${alpha})`);
  g.addColorStop(0.55, `rgba(${rgb},${alpha * 0.35})`);
  g.addColorStop(1, `rgba(${rgb},0)`);
  ctx.fillStyle = g;
  ctx.fillRect(-radius, -radius, radius * 2, radius * 2);
  ctx.restore();
}

function vignette(ctx, W, H, alpha) {
  const g = ctx.createRadialGradient(W / 2, H / 2, Math.min(W, H) * 0.35, W / 2, H / 2, Math.hypot(W, H) * 0.56);
  g.addColorStop(0, 'rgba(0,0,0,0)');
  g.addColorStop(1, `rgba(0,0,0,${alpha})`);
  ctx.fillStyle = g; ctx.fillRect(0, 0, W, H);
}

// logo gastado (esténcil en el piso): el logo con raspones
function wornLogo(width, color, r, wear = 260) {
  const h = Math.round(width * 364 / 1200) + 8;
  const c = makeCanvas(Math.round(width) + 8, h);
  const ctx = c.getContext('2d');
  drawLogo(ctx, c.width / 2, h / 2, width, color);
  ctx.globalCompositeOperation = 'destination-out';
  for (let i = 0; i < wear; i++) {
    ctx.globalAlpha = 0.25 + r() * 0.6;
    ctx.beginPath(); ctx.arc(r() * c.width, r() * h, 1 + r() * 3.2, 0, Math.PI * 2); ctx.fill();
  }
  for (let i = 0; i < 9; i++) { ctx.globalAlpha = 0.35; ctx.fillRect(r() * c.width, 0, 1 + r() * 2, h); }
  return c;
}

// póster / cuadro con el diseño de un artista (o un póster de la marca si no hay imagen)
function artPanel(ctx, a, x, y, size, { caption = true, paper = BONE, ink = INK } = {}) {
  ctx.fillStyle = paper;
  ctx.fillRect(x, y, size, size + (caption ? size * 0.2 : 0));
  const m = size * 0.045;
  if (a && a.imgEl) ctx.drawImage(a.imgEl, x + m, y + m, size - m * 2, size - m * 2);
  else {
    ctx.fillStyle = INK; ctx.fillRect(x + m, y + m, size - m * 2, size - m * 2);
    ctx.fillStyle = LIME; ctx.fillRect(x + m, y + size * 0.7, size - m * 2, size * 0.08);
    drawLogo(ctx, x + size / 2, y + size * 0.45, size * 0.78, BONE);
  }
  if (caption) {
    ctx.fillStyle = ink; ctx.textAlign = 'left'; ctx.textBaseline = 'alphabetic';
    ctx.font = brandFont(size * 0.105);
    ctx.fillText((a ? a.name : 'INKCAP').toUpperCase(), x + m, y + size + size * 0.1, size - m * 2);
    ctx.font = `600 ${size * 0.062}px ${FONTS.brand}`;
    ctx.fillText(a ? `${a.artist} · INKCAP` : 'diseñado por artistas', x + m, y + size + size * 0.17, size - m * 2);
  }
}

// ── Utilería 3D compartida ───────────────────────────────────
// Gorra de exhibición: la misma gorra del juego sin el contorno (menos mallas).
function displayCap(A, id, parent, x, y, z, s = 1, rotY = 0) {
  const cap = buildCap(0x222222, id);
  const drop = [];
  cap.traverse((o) => {
    if (!o.isMesh) return;
    if (o.material.side === THREE.BackSide) drop.push(o); else A.own(o.material);
  });
  for (const o of drop) o.parent.remove(o);
  cap.position.set(x, y, z);
  cap.scale.setScalar(s);
  cap.rotation.y = rotY;
  parent.add(cap);
  return cap;
}

// Playera colgada (un plano con la silueta y el estampado reales)
function shirtMat(A, id) {
  A._shirtMats ||= new Map();
  if (!A._shirtMats.has(id)) {
    A._shirtMats.set(id, A.own(new THREE.MeshBasicMaterial({ map: hangingShirtTexture(id), alphaTest: 0.5, side: THREE.DoubleSide, color: A.sc.shirtTint ?? 0xffffff })));
  }
  return A._shirtMats.get(id);
}
function hangShirt(A, id, parent, x, yTop, z, size, tilt = -0.25) {
  const pivot = new THREE.Group();
  pivot.position.set(x, yTop, z);
  pivot.rotation.x = tilt;
  const m = new THREE.Mesh(GEO.plane, shirtMat(A, id));
  m.scale.set(size, size, 1);
  m.position.y = -size / 2;
  pivot.add(m);
  parent.add(pivot);
  return pivot;
}

// halo de luz que siempre mira a la cámara (focos, neón)
function halo(A, parent, x, y, z, size, color, opacity = 0.8) {
  const s = new THREE.Sprite(A.own(new THREE.SpriteMaterial({ map: softDotTexture(), color, transparent: true, opacity, depthWrite: false, blending: THREE.AdditiveBlending, fog: false })));
  s.position.set(x, y, z);
  s.scale.set(size, size, 1);
  parent.add(s);
  return s;
}

// calcomanía plana (sticker) acostada o de pie
function sticker(A, tex, parent, x, y, z, w, h, { flat = false, rotY = 0, rotX = 0 } = {}) {
  const m = new THREE.Mesh(flat ? GEO.flat : GEO.plane, A.own(new THREE.MeshBasicMaterial({ map: tex, transparent: true, alphaTest: 0.25, fog: false, polygonOffset: true, polygonOffsetFactor: -2 })));
  m.position.set(x, y, z);
  if (flat) m.scale.set(w, 1, h); else m.scale.set(w, h, 1);
  m.rotation.set(rotX, rotY, 0);
  parent.add(m);
  return m;
}

// Taza INKCAP: trofeo del ganador (game.js) y mostrador de la tienda.
// Primitivas + la textura del logo; materiales y geometrías de módulo (compartidos).
let _mug = null;
export function buildMug(scale = 1) {
  if (!_mug) {
    _mug = {
      body: new THREE.CylinderGeometry(0.115, 0.1, 0.24, 20, 1, true, Math.PI), // la mitad del canvas mira al frente (+z)
      base: new THREE.CylinderGeometry(0.1, 0.1, 0.02, 20),
      coffee: new THREE.CircleGeometry(0.105, 20).rotateX(-Math.PI / 2),
      handle: new THREE.TorusGeometry(0.065, 0.02, 8, 14, Math.PI * 1.15),
      mBody: new THREE.MeshToonMaterial({ map: mugTexture(), gradientMap: toonRamp(), side: THREE.DoubleSide }),
      mWhite: new THREE.MeshToonMaterial({ color: 0xf6f4ee, gradientMap: toonRamp() }),
      mCoffee: new THREE.MeshBasicMaterial({ color: 0x1a120e }),
      mOutline: new THREE.MeshBasicMaterial({ color: 0x111111, side: THREE.BackSide }),
    };
  }
  const g = new THREE.Group();
  const body = new THREE.Mesh(_mug.body, _mug.mBody);
  const outline = new THREE.Mesh(_mug.body, _mug.mOutline); outline.scale.set(1.08, 1.03, 1.08);
  const base = new THREE.Mesh(_mug.base, _mug.mWhite); base.position.y = -0.115;
  const coffee = new THREE.Mesh(_mug.coffee, _mug.mCoffee); coffee.position.y = 0.09;
  const handle = new THREE.Mesh(_mug.handle, _mug.mWhite);
  handle.position.set(0.125, 0, 0); handle.rotation.z = -Math.PI * 0.575;
  g.add(body, outline, base, coffee, handle);
  g.scale.setScalar(scale);
  return g;
}

// ── Obstáculos ───────────────────────────────────────────────
// Cada modelo cabe dentro de la caja de colisión (o.w × o.d × o.h) del obstáculo.
let _knot = null, _ico = null, _tinacoLogo = null;
const OBSTACLES = {
  dumpster(A, o, g) {
    const body = 0x1f7a4d, dark = 0x155a38;
    for (const sx of [-1, 1]) for (const sz of [-1, 1]) {
      const w = A.cyl(0.11, 0.1, 0x1b1b1f, sx * (o.w / 2 - 0.25), 0.01, sz * (o.d / 2 - 0.16), g, GEO.cyl8);
      w.rotation.x = Math.PI / 2; w.scale.set(0.11, 0.1, 0.11); w.position.y = 0.11;
    }
    A.box(o.w * 0.96, o.h * 0.72, o.d * 0.94, body, 0, 0.14, 0, g);
    A.box(o.w, 0.1, o.d, dark, 0, o.h * 0.72 + 0.1, 0, g);
    const lid = A.box(o.w * 0.98, 0.09, o.d * 0.98, dark, 0, o.h - 0.16, 0, g);
    lid.rotation.x = -0.07;
    for (const sx of [-1, 1]) A.box(0.08, 0.1, 0.5, 0x101014, sx * (o.w / 2 + 0.01), o.h * 0.6, 0, g);
    // stickers INKCAP: uno grande en la tapa (se ve desde arriba) y otro al frente
    sticker(A, logoStickerTexture(INK, BONE), g, -0.35, o.h - 0.045, 0.05, 1.5, 0.56, { flat: true, rotY: 0.12 });
    sticker(A, logoStickerTexture(BONE, null), g, 0.45, o.h * 0.45, o.d * 0.47 + 0.012, 1.1, 0.4);
  },
  crate(A, o, g) {
    A._crate ||= A.tmat(crateTexture());
    A.add(GEO.box, A._crate, 0, o.h / 2, 0, o.w, o.h, o.d, g);
    // marca de embarque con el logo, pintada con esténcil sobre la tapa
    sticker(A, logoStickerTexture('#2b1a0e', null), g, 0, o.h + 0.012, 0.05, o.w * 0.86, o.w * 0.32, { flat: true, rotY: -0.08 });
  },
  'crate-low'(A, o, g) {
    A._crateLow ||= A.tmat(crateTexture(), 0xffe9c9);
    A.add(GEO.box, A._crateLow, 0, o.h / 2, 0, o.w, o.h, o.d, g);
  },
  'crate-stack'(A, o, g) {
    // torre de huacales dentro de la huella 1 × 2.4: dos abajo y uno arriba
    A._crate ||= A.tmat(crateTexture());
    A._crateLow ||= A.tmat(crateTexture(), 0xffe9c9);
    const half = o.d / 2;
    A.add(GEO.box, A._crate, 0, 0.39, -half / 2, o.w * 0.96, 0.78, half * 0.96, g);
    A.add(GEO.box, A._crateLow, 0, 0.39, half / 2, o.w * 0.96, 0.78, half * 0.96, g);
    const top = A.add(GEO.box, A._crate, 0, 0.78 + (o.h - 0.78) / 2, half * 0.15, o.w * 0.84, o.h - 0.78, half * 0.84, g);
    top.rotation.y = 0.18;
  },
  pedestal(A, o, g, idx) {
    A.box(o.w, 0.14, o.d, 0xbfb6a6, 0, 0, 0, g);
    A.box(o.w * 0.8, 0.74, o.d * 0.8, 0xf4f1ea, 0, 0.14, 0, g);
    A.box(o.w * 0.92, 0.1, o.d * 0.92, 0xe2dccf, 0, 0.88, 0, g);
    A.box(0.5, 0.2, 0.02, 0xb08d57, 0, 0.42, o.d * 0.4 + 0.01, g); // cédula de latón
    // escultura abstracta distinta en cada pedestal
    const k = idx % 4;
    let s;
    if (k === 0) { _knot ||= new THREE.TorusKnotGeometry(0.3, 0.1, 60, 8); s = A.add(_knot, 0xffb703, 0, 1.5, 0, 1, 1, 1, g); }
    else if (k === 1) { _ico ||= new THREE.IcosahedronGeometry(0.44, 0); s = A.add(_ico, 0xe63946, 0, 1.46, 0, 1, 1, 1, g); }
    else if (k === 2) s = A.add(GEO.sphere, 0x2a9d8f, 0, 1.5, 0, 0.34, 0.5, 0.34, g);
    else {
      // "busto" de muñequito gigante con su gorra INKCAP: guiño a los art toys
      s = new THREE.Group();
      A.add(GEO.sphere, 0xf7f7f2, 0, 0, 0, 0.42, 0.37, 0.4, s);
      for (const sx of [-1, 1]) A.add(GEO.sphere, A.glow(0x111111), sx * 0.13, 0.02, 0.36, 0.055, 0.08, 0.04, s);
      displayCap(A, brandCapIds()[0], s, 0, 0.14, 0, 1.2);
      s.position.y = 1.36;
      g.add(s);
    }
    if (k !== 3) s.userData.spin = true;
  },
  bench(A, o, g) {
    A.box(o.w, 0.12, o.d, 0x6d4c41, 0, o.h - 0.12, 0, g);
    A.box(o.w * 0.96, 0.05, o.d * 0.8, 0x8a2a2a, 0, o.h - 0.02, 0, g); // cojín
    for (const sx of [-1, 1]) A.box(0.16, o.h - 0.12, o.d * 0.8, 0x2f211c, sx * (o.w / 2 - 0.3), 0, 0, g);
  },
  tinaco(A, o, g) {
    const r = o.w * 0.45;
    A.box(o.w * 0.98, 0.2, o.d * 0.98, 0x9e9a94, 0, 0, 0, g);
    A.cyl(r, 1.2, 0x17171b, 0, 0.2, 0, g);
    for (const y of [0.5, 0.85, 1.2]) A.cyl(r * 1.035, 0.07, 0x222228, 0, y, 0, g);
    A.add(GEO.dome, 0x1d1d22, 0, 1.4, 0, r, 0.3, r, g);
    A.cyl(0.2, o.h - 1.66, 0x2c2c33, 0, 1.66, 0, g);
    A.cyl(0.04, 1.3, 0x8d8d8d, r * 0.9, 0.1, -r * 0.5, g, GEO.cyl8);
    // logo INKCAP pintado en el tinaco (calcomanía curva del lado de la cámara)
    _tinacoLogo ||= new THREE.CylinderGeometry(1, 1, 1, 20, 1, true, -0.95, 1.9);
    const logo = new THREE.Mesh(_tinacoLogo, A.own(new THREE.MeshBasicMaterial({ map: logoStickerTexture(BONE, null), transparent: true, alphaTest: 0.25, fog: false, polygonOffset: true, polygonOffsetFactor: -2 })));
    logo.scale.set(r * 1.045, 0.5, r * 1.045);
    logo.position.y = 0.86;
    logo.rotation.x = 0; g.add(logo);
  },
  planter(A, o, g) {
    A.box(o.w, o.h * 0.85, o.d, 0xa8563a, 0, 0, 0, g);
    A.box(o.w * 0.94, 0.06, o.d * 0.8, 0x3b2a20, 0, o.h * 0.85, 0, g);
    const n = 5;
    for (let i = 0; i < n; i++) {
      const x = -o.w / 2 + 0.3 + i * (o.w - 0.6) / (n - 1);
      if (i % 2) { // nopal
        A.add(GEO.sphere, 0x3f8f4a, x, o.h + 0.2, 0, 0.16, 0.26, 0.07, g);
        A.add(GEO.sphere, 0x4aa457, x + 0.12, o.h + 0.42, 0, 0.1, 0.16, 0.06, g);
        A.add(GEO.sphere, A.glow(0xff5d8f), x + 0.13, o.h + 0.6, 0, 0.05, 0.05, 0.05, g);
      } else A.add(GEO.sphere, 0x43a047, x, o.h + 0.12, 0, 0.26, 0.22, 0.2, g);
    }
  },
  gas(A, o, g) {
    // tanque de gas estacionario, acostado sobre su base
    A.box(o.w * 0.94, 0.12, o.d * 0.7, 0x8d8a84, 0, 0, 0, g);
    for (const sx of [-1, 1]) A.box(0.1, 0.3, o.d * 0.5, 0x5d5a55, sx * 0.3, 0.12, 0, g);
    const r = 0.33;
    const body = A.cyl(r, 0.62, 0xe9ecef, 0, 0.35, 0, g);
    body.rotation.z = Math.PI / 2; body.position.y = 0.35 + r;
    for (const sx of [-1, 1]) A.add(GEO.sphere, 0xe9ecef, sx * 0.31, 0.35 + r, 0, r * 0.75, r, r, g);
    const band = A.cyl(r * 1.03, 0.14, 0xc1121f, 0, 0, 0, g);
    band.rotation.z = Math.PI / 2; band.position.y = 0.35 + r;
    A.cyl(0.07, 0.14, 0x333338, 0, o.h - 0.14, 0, g, GEO.cyl8);
    A.cyl(0.11, 0.04, 0xc1121f, 0, o.h - 0.04, 0, g, GEO.cyl8);
  },
  // rack de la tienda: playeras reales (Basic y artistas) de frente a la cámara
  rack(A, o, g, idx) {
    const metal = 0x26252b, chrome = 0xcfd2d6;
    A.box(o.w, 0.06, o.d, metal, 0, 0, 0, g);
    for (const sx of [-1, 1]) A.box(0.08, o.h - 0.1, o.d * 0.9, chrome, sx * (o.w / 2 - 0.05), 0.06, 0, g);
    A.box(o.w * 0.94, o.h - 0.1, 0.05, 0x1b1a1f, 0, 0.06, -o.d * 0.36, g); // respaldo: se lee como mueble sólido
    // repisa de arriba con el logo (es lo que más se ve desde la cámara)
    A.box(o.w, 0.06, o.d, metal, 0, o.h - 0.06, 0, g);
    A.add(GEO.box, A.glow(LIME_HEX), 0, o.h - 0.03, o.d / 2 + 0.012, o.w, 0.05, 0.02, g);
    sticker(A, logoStickerTexture(BONE, null), g, 0, o.h + 0.006, 0, 1.55, 0.47, { flat: true });
    const ids = brandShirtIds();
    const n = 4, size = 1.02;
    for (let i = 0; i < n; i++) {
      const x = -o.w / 2 + 0.42 + i * (o.w - 0.84) / (n - 1);
      const p = hangShirt(A, ids[(i + idx) % ids.length], g, x, o.h - 0.1, o.d / 2 - 0.02 + (i % 2) * 0.03, size, -0.24);
      p.rotation.y = (i % 2 ? 1 : -1) * 0.07;
    }
  },
  // mesa de gorras (baja: las gorras del juego pasan por encima)
  mesa(A, o, g) {
    A.box(o.w * 0.9, o.h - 0.1, o.d * 0.84, 0x1b1a1f, 0, 0, 0, g);
    A.box(o.w, 0.1, o.d, 0xd9bd92, 0, o.h - 0.1, 0, g);
    A.add(GEO.box, A.glow(LIME_HEX), 0, o.h - 0.13, 0, o.w + 0.03, 0.035, o.d + 0.03, g);
    A.box(o.w * 0.92, 0.13, o.d * 0.42, 0x26252b, 0, o.h, -o.d * 0.24, g);
    const ids = brandCapIds();
    for (let i = 0; i < 6; i++) {
      const col = i % 3, back = i < 3;
      displayCap(A, ids[i % ids.length], g, (col - 1) * 0.7, o.h + (back ? 0.13 : 0) + 0.01, back ? -0.26 : 0.3, 0.86);
    }
  },
};

export function buildObstacle(A, o, g, idx) {
  const fn = OBSTACLES[o.kind];
  if (fn) fn(A, o, g, idx);
  else A.box(o.w, o.h, o.d, 0x888888, 0, 0, 0, g);
}

// ═════════════════════════════════════════════════════════════
// B1 · CALLEJÓN GRAFITI
// ═════════════════════════════════════════════════════════════
const NEON = ['#ff2bd6', '#39ff14', '#00e5ff', '#ffe600', '#ff6a2b', '#ffffff', '#8e6cff'];

const callejon = {
  walls: { back: 2.6, side: 2.4, front: 0.35, capColor: 0x3d3a44, topColor: 0x55525c, frontColor: 0x4a4852 },
  outer: 0x1b1a22,
  pit: 'concreto', pitDepth: 3, rim: 'franjas',
  lights: { sky: 0xd8c8ff, ground: 0x3a2e4a, sun: 0xfff0e0 },
  sideTexture: () => graffitiWallTexture(11),

  paintFloor(ctx, F) {
    const { W, H, r, X, Z, M } = F;
    ctx.fillStyle = '#383741'; ctx.fillRect(0, 0, W, H);
    // parches de asfalto reparado y grano
    for (let i = 0; i < 10; i++) {
      ctx.fillStyle = `rgba(${r() < 0.5 ? '255,255,255' : '0,0,0'},${0.03 + r() * 0.05})`;
      ctx.fillRect(r() * W, r() * H, 80 + r() * 260, 60 + r() * 180);
    }
    speckle(ctx, W, H, r, 3200, 0.06);
    speckle(ctx, W, H, r, 1500, 0.12, false, 3);
    // banqueta pegada al mural, con juntas y guarnición amarilla
    const bz = Z(-6.85);
    ctx.fillStyle = '#56545d'; ctx.fillRect(0, 0, W, bz);
    ctx.strokeStyle = 'rgba(0,0,0,0.22)'; ctx.lineWidth = 2;
    for (let x = 0; x < W; x += M(1.3)) { ctx.beginPath(); ctx.moveTo(x, 0); ctx.lineTo(x, bz); ctx.stroke(); }
    ctx.fillStyle = '#c9a227'; ctx.fillRect(0, bz - 3, W, 7);
    ctx.fillStyle = '#383741';
    for (let i = 0; i < 26; i++) ctx.fillRect(r() * W, bz - 3, 6 + r() * 26, 7);
    // grietas
    ctx.strokeStyle = 'rgba(0,0,0,0.3)'; ctx.lineWidth = 1.5;
    for (let i = 0; i < 16; i++) {
      ctx.beginPath(); let x = r() * W, y = r() * H; ctx.moveTo(x, y);
      for (let k = 0; k < 6; k++) { x += (r() - 0.5) * 90; y += (r() - 0.5) * 70; ctx.lineTo(x, y); }
      ctx.stroke();
    }
    // charcos de agua (reflejan el neón)
    for (const [px, pz, rw, col] of [[8.6, 0.2, 1.5, '0,229,255'], [-9.4, -1.6, 1.2, '255,43,214'], [3.2, 6.3, 1.3, '255,230,0']]) {
      ctx.save(); ctx.translate(X(px), Z(pz)); ctx.scale(1, 0.55);
      const g = ctx.createRadialGradient(0, 0, 2, 0, 0, M(rw));
      g.addColorStop(0, `rgba(${col},0.16)`); g.addColorStop(0.8, 'rgba(20,20,30,0.35)'); g.addColorStop(1, 'rgba(20,20,30,0)');
      ctx.fillStyle = g; ctx.beginPath(); ctx.arc(0, 0, M(rw), 0, Math.PI * 2); ctx.fill(); ctx.restore();
    }
    // manchas viejas de pintura
    for (let i = 0; i < 12; i++) drawSplat(ctx, r() * W, bz + r() * (H - bz), 8 + r() * 22, NEON[i % 4], r, 0.2);
    // tierra y marcas de obra alrededor de las zanjas; tapas quitadas junto a las coladeras
    for (const v of F.arena.voids) {
      const x0 = X(v.x - v.w / 2), z0 = Z(v.z - v.d / 2), vw = M(v.w), vd = M(v.d);
      if (v.kind === 'zanja') {
        for (let i = 0; i < 46; i++) {
          ctx.fillStyle = `rgba(${96 + r() * 40},${70 + r() * 25},${45 + r() * 15},${0.25 + r() * 0.3})`;
          ctx.beginPath(); ctx.arc(x0 - 22 + r() * (vw + 44), z0 - 20 + r() * (vd + 40), 3 + r() * 9, 0, Math.PI * 2); ctx.fill();
        }
        ctx.strokeStyle = 'rgba(255,122,26,0.8)'; ctx.lineWidth = 3; ctx.setLineDash([14, 9]);
        ctx.strokeRect(x0 - M(0.55), z0 - M(0.5), vw + M(1.1), vd + M(1.0));
        ctx.setLineDash([]);
        ctx.fillStyle = 'rgba(255,122,26,0.85)'; ctx.font = `${M(0.42)}px ${FONTS.block}`; ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
        ctx.fillText('OBRA', X(v.x + (v.z > 0 ? 2.75 : -2.75)), Z(v.z));
      } else {
        // la tapa de la coladera, botada a un lado
        const sx = v.x < 0 ? -1 : 1;
        ctx.save(); ctx.translate(X(v.x + sx * 1.75), Z(v.z + 0.55 * sx)); ctx.rotate(0.3 * sx);
        ctx.fillStyle = 'rgba(0,0,0,0.35)'; ctx.fillRect(-M(0.62), -M(0.62) + 5, M(1.3), M(1.3));
        ctx.fillStyle = '#4b4a52'; ctx.fillRect(-M(0.65), -M(0.65), M(1.3), M(1.3));
        ctx.strokeStyle = '#27262c'; ctx.lineWidth = 4; ctx.strokeRect(-M(0.65) + 3, -M(0.65) + 3, M(1.3) - 6, M(1.3) - 6);
        ctx.fillStyle = '#1f1e24';
        for (let k = -3; k <= 3; k++) ctx.fillRect(k * 6.2 - 1.6, -M(0.46), 3.2, M(0.92));
        ctx.restore();
      }
    }
    // coladera redonda (cerrada) de adorno
    ctx.save(); ctx.translate(X(9.2), Z(3.6));
    ctx.fillStyle = '#2a2930'; ctx.beginPath(); ctx.arc(0, 0, M(0.5), 0, Math.PI * 2); ctx.fill();
    ctx.strokeStyle = '#55535c'; ctx.lineWidth = 3; ctx.beginPath(); ctx.arc(0, 0, M(0.5) - 3, 0, Math.PI * 2); ctx.stroke();
    for (let k = -2; k <= 2; k++) { ctx.beginPath(); ctx.moveTo(k * 7, -12); ctx.lineTo(k * 7, 12); ctx.stroke(); }
    ctx.restore();
    // esténcil del logo INKCAP en el piso (aerosol blanco con su halo)
    const st = wornLogo(M(6.4), 'rgba(244,241,234,0.92)', r);
    ctx.save();
    ctx.translate(X(-5.7), Z(0.55)); ctx.rotate(-0.035);
    ctx.shadowColor = 'rgba(255,255,255,0.45)'; ctx.shadowBlur = 16;
    ctx.drawImage(st, -st.width / 2, -st.height / 2);
    ctx.restore();
    ctx.save(); ctx.translate(X(-5.7), Z(1.85)); ctx.rotate(-0.035);
    ctx.fillStyle = 'rgba(204,255,0,0.75)'; ctx.font = `${M(0.36)}px ${FONTS.tag}`; ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
    ctx.fillText('diseñado por artistas · hecho en México', 0, 0);
    ctx.restore();
    F.shadows(-0.14, -0.2, 0.55);
    // luz cálida de las lámparas del mural
    for (const lx of [-9, 0, 9]) lightPool(ctx, X(lx), Z(-6.4), M(5.2), '255,214,150', 0.2, 0.8);
    lightPool(ctx, X(-5.7), Z(0.5), M(4.6), '190,160,255', 0.07);
    vignette(ctx, W, H, 0.42);
  },

  // Muro del fondo (cm): mural con el logo, paste-ups de los artistas, tags y una cortina metálica
  paintBackWall(ctx, cw, ch, A, X) {
    const r = A.rand;
    ctx.fillStyle = '#4a3b38'; ctx.fillRect(0, 0, cw, ch);
    for (let y = 0; y < ch; y += 13) {
      for (let x = ((y / 13) % 2) * 19 - 19; x < cw; x += 38) {
        const s = 0.8 + r() * 0.35;
        ctx.fillStyle = `rgb(${Math.floor(116 * s)},${Math.floor(74 * s)},${Math.floor(62 * s)})`;
        ctx.fillRect(x + 1, y + 1, 36, 11);
      }
    }
    // cortina metálica (lado derecho)
    const cx0 = X(8.3), cwid = 310;
    ctx.fillStyle = '#2a2a30'; ctx.fillRect(cx0 - 10, 18, cwid + 20, ch - 18);
    ctx.fillStyle = '#7d8088'; ctx.fillRect(cx0, 28, cwid, ch - 28);
    ctx.fillStyle = 'rgba(0,0,0,0.25)';
    for (let y = 28; y < ch; y += 11) ctx.fillRect(cx0, y, cwid, 3);
    // tags del barrio
    ctx.textAlign = 'left'; ctx.textBaseline = 'alphabetic';
    const tags = [[4.7, 150, 78, 0], [6.1, 232, 54, 1], [8.6, 150, 86, 2], [11.3, 110, 50, 3], [11.2, 226, 62, 4], [-4.9, 250, 34, 6]];
    for (const [wx, y, size, k] of tags) {
      const col = NEON[k % NEON.length];
      ctx.save(); ctx.translate(X(wx), y); ctx.rotate(-0.12 + r() * 0.2);
      ctx.font = `${size}px ${FONTS.tag}`; ctx.lineWidth = size * 0.16; ctx.strokeStyle = '#111'; ctx.lineJoin = 'round';
      const word = TAG_WORDS[(k * 5 + 2) % TAG_WORDS.length];
      ctx.strokeText(word, 0, 0); ctx.fillStyle = col; ctx.fillText(word, 0, 0);
      ctx.restore();
      if (k % 2 === 0) drawDrips(ctx, X(wx) + 10, y + 4, size * 1.6, col, r, 30);
    }
    // MURAL INKCAP (lado izquierdo): bloque negro con sombra neón y el logo en grande
    const mx = X(-12.55), mw = 705;
    ctx.fillStyle = LIME; rr(ctx, mx + 12, 22, mw, 226, 16); ctx.fill();
    ctx.fillStyle = INK; rr(ctx, mx, 12, mw, 226, 16); ctx.fill();
    drawDrips(ctx, mx + 30, 236, mw - 60, INK, r, 22);
    drawDrips(ctx, mx + 200, 244, 300, LIME, r, 14);
    ctx.save(); ctx.shadowColor = 'rgba(255,255,255,0.55)'; ctx.shadowBlur = 18;
    drawLogo(ctx, mx + mw / 2, 112, 610, BONE);
    ctx.restore();
    ctx.fillStyle = LIME; ctx.font = `30px ${FONTS.tag}`; ctx.textAlign = 'center';
    ctx.fillText('diseñado por artistas · hecho en México', mx + mw / 2, 226);
    for (let i = 0; i < 7; i++) drawSplat(ctx, mx + r() * mw, 20 + r() * 210, 4 + r() * 9, i % 2 ? LIME : BONE, r, 0.85);
    // PASTE-UPS: pósters pegados con los diseños de los artistas
    const list = artists();
    const px = [-3.55, -1.4, 0.75, 2.9];
    px.forEach((wx, i) => {
      const a = list.length ? list[i % list.length] : null;
      ctx.save();
      ctx.translate(X(wx), 126); ctx.rotate([-0.035, 0.028, -0.02, 0.04][i]);
      ctx.shadowColor = 'rgba(0,0,0,0.5)'; ctx.shadowBlur = 10; ctx.shadowOffsetY = 4;
      ctx.fillStyle = BONE; ctx.fillRect(-90, -108, 180, 216);
      ctx.shadowColor = 'transparent';
      artPanel(ctx, a, -90, -108, 180);
      // engrudo y esquinas levantadas
      ctx.fillStyle = 'rgba(255,255,255,0.18)'; ctx.fillRect(-90, -108, 180, 9);
      ctx.fillStyle = '#4a3b38'; ctx.beginPath(); ctx.moveTo(90, 108); ctx.lineTo(90 - 22, 108); ctx.lineTo(90, 108 - 18); ctx.fill();
      ctx.restore();
    });
    // stickers chicos de la marca
    for (const [wx, y, rot] of [[5.4, 60, -0.1], [7.6, 96, 0.08], [12.35, 60, 0.05], [-5.15, 64, 0.1]]) {
      ctx.save(); ctx.translate(X(wx), y); ctx.rotate(rot);
      ctx.fillStyle = BONE; rr(ctx, -44, -17, 88, 34, 5); ctx.fill();
      drawLogo(ctx, 0, 0, 76, INK);
      ctx.restore();
    }
    // zoclo de concreto y mugre
    ctx.fillStyle = '#2f2c33'; ctx.fillRect(0, ch - 22, cw, 22);
    const g = ctx.createLinearGradient(0, ch - 90, 0, ch);
    g.addColorStop(0, 'rgba(0,0,0,0)'); g.addColorStop(1, 'rgba(0,0,0,0.4)');
    ctx.fillStyle = g; ctx.fillRect(0, ch - 90, cw, 90);
    // luz de las lámparas de pared
    for (const lx of [-9, 0, 9]) lightPool(ctx, X(lx), 0, 300, '255,210,140', 0.3);
  },

  decor(A) {
    const W = A.w, H = A.h, wl = this.walls;
    // lámparas de cuello de ganso sobre el mural
    for (const lx of [-9, 0, 9]) {
      A.box(0.07, 0.07, 0.6, 0x1c1b20, lx, wl.back + 0.2, -H / 2 + 0.05);
      A.add(GEO.cone, 0x1c1b20, lx, wl.back + 0.2, -H / 2 + 0.36, 0.26, 0.2, 0.26);
      A.add(GEO.sphere, A.glow(0xfff1a8), lx, wl.back + 0.1, -H / 2 + 0.36, 0.1, 0.1, 0.1);
      halo(A, A.group, lx, wl.back + 0.05, -H / 2 + 0.4, 2.2, 0xffd58a, 0.55);
    }
    // azoteas de los edificios vecinos (se ven desde arriba detrás de los muros)
    const roof = 0x2a2733;
    A.box(W + 60, wl.back - 0.12, 20, roof, 0, 0, -H / 2 - 0.6 - 10);
    for (const sx of [-1, 1]) A.box(29.4, wl.side - 0.12, H + 0.6 + 26, roof, sx * (W / 2 + 0.6 + 14.7), 0, -3 + 3);
    const sky = A.glow(0xffc46b), unit = 0x55525c, vent = 0x7a7782;
    // tragaluces encendidos, climas y ductos
    const props = [
      // [x, z, tipo]
      [-10.5, -10.2, 's'], [-3.5, -10.6, 'a'], [4.6, -10.3, 's'], [11.2, -10.4, 'a'],
      [-16.2, -5.2, 's'], [-15.4, -0.8, 'a'], [-16.6, 2.8, 't'], [-15.2, 6.2, 's'], [-18.4, -7.4, 'a'],
      [16.2, -5.6, 'a'], [15.5, -1.4, 's'], [16.8, 2.4, 'a'], [15.3, 6.0, 't'], [18.3, -7.6, 's'],
    ];
    for (const [x, z, k] of props) {
      const y0 = (Math.abs(x) > W / 2 ? wl.side : wl.back) - 0.12;
      if (k === 's') {
        A.box(1.9, 0.14, 1.3, unit, x, y0, z);
        A.add(GEO.flat, sky, x, y0 + 0.15, z, 1.6, 1, 1.0);
        A.box(0.06, 0.03, 1.0, unit, x, y0 + 0.14, z);
      } else if (k === 'a') {
        A.box(1.2, 0.6, 0.9, 0x8d8a92, x, y0, z);
        A.cyl(0.32, 0.05, 0x2a2930, x, y0 + 0.6, z);
        A.cyl(0.12, 0.5, vent, x + 1.0, y0, z + 0.3, A.group, GEO.cyl8);
      } else {
        A.cyl(0.62, 1.1, 0x17171b, x, y0 + 0.2, z);
        A.box(1.3, 0.2, 1.3, 0x77747e, x, y0, z);
        A.add(GEO.dome, 0x1d1d22, x, y0 + 1.3, z, 0.62, 0.24, 0.62);
      }
    }
    // cables con foquitos de un edificio a otro, por detrás del mural
    const bulbs = [0xff2bd6, 0x39ff14, 0x00e5ff, 0xffe600];
    for (let i = 0; i < 9; i++) {
      const x = -17.5 + i * 0.9;
      A.add(GEO.sphere, A.glow(bulbs[i % 4]), x, wl.back + 0.25 - Math.sin((i / 8) * Math.PI) * 0.25, -H / 2 - 1.3 - i * 0.12, 0.09, 0.09, 0.09);
    }
  },
};

// ═════════════════════════════════════════════════════════════
// B2 · MUSEO DE ARTE · "EXPOSICIÓN INKCAP"
// ═════════════════════════════════════════════════════════════
const museo = {
  walls: { back: 2.6, side: 1.3, front: 0.35, capColor: 0xd9d2c4, topColor: 0x8a6a43, sideColor: 0xece6dc, frontColor: 0xb08d57 },
  outer: 0x262733,
  pit: 'neon', pitDepth: 3.4, rim: 'neon', neon: [0x00e5ff, 0xff2bd6],
  lights: { sky: 0xe8f0ff, ground: 0x30344a, sun: 0xffffff },

  paintFloor(ctx, F) {
    const { W, H, r, X, Z, M } = F;
    // mármol en damero suave (claro: los vacíos y las manchas de tinta resaltan)
    const t = M(2);
    for (let y = 0, j = 0; y < H; y += t, j++) for (let x = 0, i = 0; x < W; x += t, i++) {
      ctx.fillStyle = (i + j) % 2 === 0 ? '#e3ddd1' : '#c6bdae';
      ctx.fillRect(x, y, t, t);
    }
    ctx.strokeStyle = 'rgba(120,100,70,0.35)'; ctx.lineWidth = 1.5;
    for (let x = 0; x <= W; x += t) { ctx.beginPath(); ctx.moveTo(x, 0); ctx.lineTo(x, H); ctx.stroke(); }
    for (let y = 0; y <= H; y += t) { ctx.beginPath(); ctx.moveTo(0, y); ctx.lineTo(W, y); ctx.stroke(); }
    // vetas
    ctx.strokeStyle = 'rgba(120,118,128,0.22)'; ctx.lineWidth = 2;
    for (let i = 0; i < 70; i++) {
      ctx.beginPath(); let x = r() * W, y = r() * H; ctx.moveTo(x, y);
      for (let k = 0; k < 5; k++) { x += (r() - 0.5) * 80; y += (r() - 0.5) * 80; ctx.lineTo(x, y); }
      ctx.stroke();
    }
    // alfombra roja de la entrada (frente) a la manta del fondo
    const ax = X(-1.7), aw = M(3.4);
    ctx.fillStyle = '#8c1420'; ctx.fillRect(ax, 0, aw, H);
    ctx.fillStyle = 'rgba(0,0,0,0.16)';
    for (let y = 0; y < H; y += 6) ctx.fillRect(ax, y, aw, 2);
    ctx.fillStyle = '#d7ae5b'; ctx.fillRect(ax + 6, 0, 4, H); ctx.fillRect(ax + aw - 10, 0, 4, H);
    // la exposición se anuncia en la alfombra
    ctx.fillStyle = '#e8c477'; ctx.font = brandFont(M(0.5)); ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
    ctx.fillText('EXPOSICIÓN', X(0), Z(3.55));
    drawLogo(ctx, X(0), Z(4.45), M(2.9), '#e8c477');
    drawLogo(ctx, X(0), Z(-4.6), M(2.9), '#e8c477');
    // zoclo oscuro alrededor de los fosos de instalación (el neón resalta)
    for (const v of F.arena.voids) {
      ctx.fillStyle = '#17161f';
      rr(ctx, X(v.x - v.w / 2 - 0.75), Z(v.z - v.d / 2 - 0.75), M(v.w + 1.5), M(v.d + 1.5), M(0.3)); ctx.fill();
      ctx.strokeStyle = 'rgba(215,174,91,0.8)'; ctx.lineWidth = 2.5; ctx.stroke();
    }
    F.shadows(-0.1, -0.14, 0.42);
    // luz general baja + reflectores sobre cada pedestal
    ctx.fillStyle = 'rgba(14,18,34,0.3)'; ctx.fillRect(0, 0, W, H);
    for (const o of F.arena.obstacles) if (o.kind === 'pedestal') lightPool(ctx, X(o.x), Z(o.z), M(3.2), '255,244,214', 0.3);
    lightPool(ctx, X(0), Z(0), M(4.2), '255,244,214', 0.16);
    for (const v of F.arena.voids) lightPool(ctx, X(v.x), Z(v.z), M(4), v.x < 0 ? '0,229,255' : '255,43,214', 0.14);
    vignette(ctx, W, H, 0.4);
  },

  // Muro del fondo (cm): los cuadros son los diseños de los artistas, con su cédula,
  // y al centro la manta "EXPOSICIÓN INKCAP"
  paintBackWall(ctx, cw, ch, A, X) {
    ctx.fillStyle = '#ebe5da'; ctx.fillRect(0, 0, cw, ch);
    ctx.fillStyle = 'rgba(0,0,0,0.05)';
    for (let x = 0; x < cw; x += 170) ctx.fillRect(x, 0, 2, ch);
    ctx.fillStyle = '#8a6a43'; ctx.fillRect(0, 14, cw, 5);          // riel de cuadros
    ctx.fillStyle = '#d3c8b4'; ctx.fillRect(0, ch - 44, cw, 44);    // lambrín
    ctx.fillStyle = '#4a3a2c'; ctx.fillRect(0, ch - 12, cw, 12);    // zoclo
    const list = artists();
    const fx = [-10.75, -7.35, 7.35, 10.75];
    fx.forEach((wx, i) => {
      const a = list.length ? list[i % list.length] : null;
      const x = X(wx) - 98, y = 24, s = 196;
      lightPool(ctx, X(wx), 10, 210, '255,236,190', 0.28);
      ctx.save(); ctx.shadowColor = 'rgba(0,0,0,0.45)'; ctx.shadowBlur = 12; ctx.shadowOffsetY = 6;
      ctx.fillStyle = '#b08d57'; ctx.fillRect(x, y, s, s);
      ctx.restore();
      ctx.strokeStyle = '#7d6236'; ctx.lineWidth = 3; ctx.strokeRect(x + 4, y + 4, s - 8, s - 8);
      ctx.fillStyle = '#f7f4ec'; ctx.fillRect(x + 13, y + 13, s - 26, s - 26);
      artPanel(ctx, a, x + 22, y + 22, s - 44, { caption: false });
      // cédula: "Obra · Artista · INKCAP"
      const lx = x + (i < 2 ? s + 12 : -70), ly = y + s - 44;
      ctx.fillStyle = '#fbfaf6'; ctx.fillRect(lx, ly, 58, 40);
      ctx.strokeStyle = 'rgba(0,0,0,0.3)'; ctx.lineWidth = 1; ctx.strokeRect(lx, ly, 58, 40);
      ctx.fillStyle = INK; ctx.textAlign = 'left'; ctx.textBaseline = 'alphabetic';
      ctx.font = brandFont(11); ctx.fillText((a ? a.name : 'Obra').toUpperCase(), lx + 4, ly + 13, 50);
      ctx.font = `600 8px ${FONTS.brand}`; ctx.fillText(a ? a.artist : 'Artista invitado', lx + 4, ly + 24, 50);
      drawLogo(ctx, lx + 20, ly + 33, 30, INK);
    });
    // manta "EXPOSICIÓN INKCAP"
    const bx = X(-4.4), bw = 880, by = 8, bh = 240;
    ctx.fillStyle = '#5a3c22'; ctx.fillRect(bx - 16, by, bw + 32, 7);
    ctx.save(); ctx.shadowColor = 'rgba(0,0,0,0.4)'; ctx.shadowBlur = 14; ctx.shadowOffsetY = 6;
    ctx.fillStyle = RED;
    ctx.beginPath(); ctx.moveTo(bx, by + 4); ctx.lineTo(bx + bw, by + 4); ctx.lineTo(bx + bw, by + bh);
    for (let k = 7; k >= 0; k--) { ctx.lineTo(bx + (k + 0.5) * (bw / 8), by + bh - 16); ctx.lineTo(bx + k * (bw / 8), by + bh); }
    ctx.closePath(); ctx.fill();
    ctx.restore();
    for (let k = 0; k < 8; k++) { // pliegues de la tela
      const g = ctx.createLinearGradient(bx + k * (bw / 8), 0, bx + (k + 1) * (bw / 8), 0);
      g.addColorStop(0, 'rgba(0,0,0,0.14)'); g.addColorStop(0.5, 'rgba(255,255,255,0.07)'); g.addColorStop(1, 'rgba(0,0,0,0.14)');
      ctx.fillStyle = g; ctx.fillRect(bx + k * (bw / 8), by + 4, bw / 8, bh - 20);
    }
    ctx.fillStyle = BONE; ctx.fillRect(bx + 26, by + 22, bw - 52, 4); ctx.fillRect(bx + 26, by + bh - 36, bw - 52, 4);
    ctx.fillStyle = BONE; ctx.font = brandFont(64); ctx.textAlign = 'center'; ctx.textBaseline = 'alphabetic';
    ctx.fillText('EXPOSICIÓN', bx + bw / 2, by + 86);
    drawLogo(ctx, bx + bw / 2, by + 146, 400, BONE);
    lightPool(ctx, bx + bw / 2, 0, 460, '255,236,190', 0.16);
  },

  decor(A) {
    const W = A.w, H = A.h, wl = this.walls;
    // riel de reflectores sobre el muro del fondo
    A.box(W, 0.06, 0.06, 0x1c1b20, 0, wl.back + 0.12, -H / 2 + 0.35);
    for (const lx of [-10.75, -7.35, -2.2, 2.2, 7.35, 10.75]) {
      A.cyl(0.09, 0.2, 0x1c1b20, lx, wl.back, -H / 2 + 0.35, A.group, GEO.cyl8);
      A.add(GEO.sphere, A.glow(0xfff4d6), lx, wl.back - 0.02, -H / 2 + 0.38, 0.07, 0.07, 0.07);
    }
    // salas contiguas: tapetes con piezas fuera del área de juego
    const gx = W / 2 + 0.6 + 1.0, gz = -4.6;
    const rug = A.glow(0x5a1018);
    for (const sx of [-1, 1]) A.add(GEO.flat, rug, sx * (gx + 0.2), 0.005, gz, 3.4, 1, 5.4);
    // VITRINA con las 6 gorras reales (decorativa, sala izquierda): 3 escalones de 2 gorras
    const v = new THREE.Group(); v.position.set(-gx, 0, gz); A.group.add(v);
    A.box(1.8, 0.85, 3.5, 0xf4f1ea, 0, 0, 0, v);
    A.box(1.9, 0.08, 3.6, 0xb08d57, 0, 0.85, 0, v);
    const ids = brandCapIds();
    for (let row = 0; row < 3; row++) {
      const y = 0.93 + (2 - row) * 0.3, z = -1.1 + row * 1.1;
      if (row < 2) A.box(1.7, (2 - row) * 0.3, 1.0, 0xe2dccf, 0, 0.93, z, v);
      for (let col = 0; col < 2; col++) displayCap(A, ids[(row * 2 + col) % ids.length], v, (col - 0.5) * 0.84, y + 0.01, z, 1.08);
    }
    const glass = A.add(GEO.box, A.own(new THREE.MeshBasicMaterial({ color: 0xcfe9ff, transparent: true, opacity: 0.12, depthWrite: false })), 0, 1.78, 0, 1.8, 1.7, 3.5, v);
    glass.renderOrder = 4;
    for (const sx of [-1, 1]) for (const sz of [-1, 1]) A.box(0.05, 1.7, 0.05, 0xb08d57, sx * 0.9, 0.93, sz * 1.75, v);
    sticker(A, logoStickerTexture(INK, null), v, 0, 0.45, 1.76, 1.5, 0.47);
    halo(A, v, 0, 1.7, 0.4, 5.4, 0xfff0c8, 0.26);
    // escultura de art toy gigante con gorra INKCAP (sala derecha)
    const b = new THREE.Group(); b.position.set(gx - 0.1, 0, gz); A.group.add(b);
    A.box(1.8, 0.7, 1.8, 0xf4f1ea, 0, 0, 0, b);
    A.box(1.9, 0.08, 1.9, 0xb08d57, 0, 0.7, 0, b);
    A.add(GEO.sphere, 0xf7f7f2, 0, 1.52, 0, 0.84, 0.76, 0.8, b);
    for (const sx of [-1, 1]) A.add(GEO.sphere, A.glow(0x111111), sx * 0.27, 1.54, 0.74, 0.115, 0.165, 0.075, b);
    displayCap(A, ids[1 % ids.length], b, 0, 1.82, 0, 2.42);
    sticker(A, logoStickerTexture(INK, null), b, 0, 0.36, 0.91, 1.4, 0.44);
    halo(A, b, 0, 1.6, 0.6, 4.4, 0xfff0c8, 0.2);
    // postes con cordón de terciopelo frente a cada pieza
    for (const sx of [-1, 1]) {
      const z = gz + 2.55;
      for (const dx of [-1.2, 1.2]) {
        A.cyl(0.06, 0.9, 0xb08d57, sx * (gx + 0.2) + dx, 0, z, A.group, GEO.cyl8);
        A.add(GEO.sphere, 0xb08d57, sx * (gx + 0.2) + dx, 0.95, z, 0.09, 0.09, 0.09);
      }
      A.box(2.4, 0.06, 0.06, 0x8c1420, sx * (gx + 0.2), 0.74, z);
    }
  },
};

// ═════════════════════════════════════════════════════════════
// B3 · AZOTEA CDMX · dos azoteas con hueco entre edificios
// ═════════════════════════════════════════════════════════════
let _streetTex = null, _cityTex = null, _fenceTex = null;
function streetTexture() {
  if (_streetTex) return _streetTex;
  const c = makeCanvas(64, 256);
  const ctx = c.getContext('2d');
  ctx.fillStyle = '#17131c'; ctx.fillRect(0, 0, 64, 256);
  ctx.fillStyle = '#231d29'; ctx.fillRect(0, 0, 9, 256); ctx.fillRect(55, 0, 9, 256); // banquetas
  ctx.fillStyle = 'rgba(240,200,60,0.5)';
  for (let y = 10; y < 256; y += 64) ctx.fillRect(30, y, 4, 30);
  // un coche estacionado y un charco de luz de farol: dan la escala de la altura
  ctx.fillStyle = '#5b2a2a'; ctx.fillRect(12, 150, 16, 34);
  ctx.fillStyle = 'rgba(190,220,255,0.5)'; ctx.fillRect(14, 156, 12, 7);
  const g = ctx.createRadialGradient(46, 60, 1, 46, 60, 26);
  g.addColorStop(0, 'rgba(255,200,120,0.5)'); g.addColorStop(1, 'rgba(255,200,120,0)');
  ctx.fillStyle = g; ctx.fillRect(16, 30, 60, 60);
  _streetTex = toCanvasTexture(c, { repeat: [1, 7] });
  return _streetTex;
}
function cityTexture() {
  if (_cityTex) return _cityTex;
  const c = makeCanvas(256, 256);
  const ctx = c.getContext('2d');
  ctx.fillStyle = '#5a3540'; ctx.fillRect(0, 0, 256, 256);
  let s = 9;
  const r = () => { s ^= s << 13; s ^= s >>> 17; s ^= s << 5; return ((s >>> 0) % 1000) / 1000; };
  for (let y = 6; y < 256; y += 62) for (let x = 6; x < 256; x += 62) {
    ctx.fillStyle = ['#7a4a45', '#6a3f48', '#855544', '#5f3a4a'][Math.floor(r() * 4)];
    ctx.fillRect(x, y, 50, 50);
    ctx.fillStyle = 'rgba(255,214,130,0.75)';
    for (let k = 0; k < 4; k++) ctx.fillRect(x + 4 + r() * 40, y + 4 + r() * 40, 3, 3);
  }
  _cityTex = toCanvasTexture(c, { repeat: [9, 9] });
  return _cityTex;
}
function fenceTexture() {
  if (_fenceTex) return _fenceTex;
  const c = makeCanvas(64, 64);
  const ctx = c.getContext('2d');
  ctx.strokeStyle = '#c9c9c9'; ctx.lineWidth = 3;
  for (let i = -64; i < 128; i += 16) {
    ctx.beginPath(); ctx.moveTo(i, 0); ctx.lineTo(i + 64, 64); ctx.stroke();
    ctx.beginPath(); ctx.moveTo(i + 64, 0); ctx.lineTo(i, 64); ctx.stroke();
  }
  _fenceTex = toCanvasTexture(c, { repeat: [3, 3] });
  return _fenceTex;
}

const azotea = {
  walls: { back: 1.0, side: 1.0, front: 0.35 },
  outer: null, // alrededor no hay piso: es el aire (la ciudad queda abajo)
  rim: 'franjas',
  lights: { sky: 0xffe0c0, ground: 0x8a4b3a, sun: 0xffd2a1 },
  shirtTint: 0xffe9d6,
  sideTexture: () => rooftopWallTexture(3),

  paintFloor(ctx, F) {
    const { W, H, r, X, Z, M } = F;
    // impermeabilizante terracota con parches y traslapes
    ctx.fillStyle = '#b4573d'; ctx.fillRect(0, 0, W, H);
    for (let i = 0; i < 9; i++) {
      ctx.fillStyle = `rgba(${r() < 0.5 ? '214,120,86' : '150,62,44'},${0.25 + r() * 0.3})`;
      ctx.fillRect(r() * W, r() * H, 110 + r() * 240, 70 + r() * 170);
    }
    ctx.strokeStyle = 'rgba(0,0,0,0.1)'; ctx.lineWidth = 2;
    for (let x = M(1); x < W; x += M(1)) { ctx.beginPath(); ctx.moveTo(x, 0); ctx.lineTo(x, H); ctx.stroke(); }
    speckle(ctx, W, H, r, 2600, 0.08, false, 3);
    speckle(ctx, W, H, r, 1200, 0.07, true, 2);
    // manchas de agua junto a los tinacos y bajadas de agua
    for (const o of F.arena.obstacles) if (o.kind === 'tinaco') {
      ctx.fillStyle = 'rgba(70,30,24,0.28)';
      ctx.beginPath(); ctx.ellipse(X(o.x + 0.3), Z(o.z + 1.3), M(1.2), M(0.7), 0.2, 0, Math.PI * 2); ctx.fill();
    }
    // avioncito de gis
    ctx.save(); ctx.translate(X(9.9), Z(-3.6)); ctx.rotate(-0.08);
    ctx.strokeStyle = 'rgba(255,255,255,0.75)'; ctx.fillStyle = 'rgba(255,255,255,0.8)'; ctx.lineWidth = 2.5;
    ctx.font = `${M(0.42)}px ${FONTS.tag}`; ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
    const cells = [[0, 0], [0, 1], [-0.5, 2], [0.5, 2], [0, 3], [-0.5, 4], [0.5, 4], [0, 5]];
    cells.forEach(([cx, cy], i) => {
      const s = M(0.72);
      ctx.strokeRect(cx * s - s / 2, -cy * s * 0.9 + M(1.6), s, s * 0.9);
      ctx.fillText(String(i + 1), cx * s, -cy * s * 0.9 + M(1.6) + s * 0.45);
    });
    ctx.restore();
    ctx.save(); ctx.translate(X(-9.6), Z(4.9)); ctx.rotate(0.06);
    ctx.fillStyle = 'rgba(255,255,255,0.7)'; ctx.font = `${M(0.6)}px ${FONTS.tag}`; ctx.textAlign = 'center';
    ctx.fillText('azotea crew ♥', 0, 0);
    ctx.restore();
    // pretil de concreto a cada lado del hueco entre edificios
    for (const sx of [-1, 1]) {
      ctx.fillStyle = '#9d968d';
      ctx.fillRect(X(sx > 0 ? 0.9 : -1.45), 0, M(0.55), H);
      ctx.fillStyle = 'rgba(0,0,0,0.18)';
      ctx.fillRect(X(sx > 0 ? 1.41 : -1.45), 0, 2, H);
    }
    // PUENTE DE TABLÓN (3 m) sobre el hueco
    const bx = X(-1.3), bw = M(2.6), bz = Z(-1.5), bh = M(3);
    ctx.fillStyle = 'rgba(0,0,0,0.3)'; ctx.fillRect(bx - 3, bz - 4, bw + 6, bh + 8);
    const n = 9;
    for (let i = 0; i < n; i++) {
      const y = bz + (i * bh) / n, h = bh / n;
      const k = 0.85 + r() * 0.3;
      ctx.fillStyle = `rgb(${Math.floor(176 * k)},${Math.floor(132 * k)},${Math.floor(78 * k)})`;
      ctx.fillRect(bx - (i % 2) * 3, y, bw + (i % 3) * 3, h - 1.5);
      ctx.fillStyle = 'rgba(60,36,16,0.45)';
      ctx.fillRect(bx, y + h * 0.5, bw, 1);
      ctx.fillStyle = '#3b2616';
      for (const nx of [0.07, 0.93]) { ctx.beginPath(); ctx.arc(bx + bw * nx, y + h / 2, 1.8, 0, Math.PI * 2); ctx.fill(); }
    }
    // atardecer: sombras largas hacia la derecha y luz naranja desde la izquierda
    F.shadows(0.5, -0.06, 0.42);
    const sh = ctx.createLinearGradient(0, 0, M(1.6), 0);
    sh.addColorStop(0, 'rgba(60,20,30,0.4)'); sh.addColorStop(1, 'rgba(60,20,30,0)');
    ctx.fillStyle = sh; ctx.fillRect(0, 0, M(1.6), H);
    const sun = ctx.createLinearGradient(0, 0, W, 0);
    sun.addColorStop(0, 'rgba(255,170,80,0.22)'); sun.addColorStop(0.6, 'rgba(255,170,80,0.05)'); sun.addColorStop(1, 'rgba(90,40,90,0.16)');
    ctx.fillStyle = sun; ctx.fillRect(0, 0, W, H);
    vignette(ctx, W, H, 0.22);
  },

  // El hueco entre edificios: dos fachadas que bajan hasta la calle, 11 m abajo.
  // Sigue más allá de la azotea (hacia el fondo y hacia la cámara).
  buildVoid(A, v) {
    if (!A._gap) {
      A._gap = true;
      A.pit(0, 0, v.w, 64, 11, 'edificio');
      const street = A.add(GEO.flat, A.own(new THREE.MeshBasicMaterial({ map: streetTexture(), fog: false, color: 0x9a8f9f })), 0, -10.9, 0, v.w, 1, 64);
      street.renderOrder = 0;
      // vigas del puente de tablón (se les ve el canto desde el hueco)
      A.box(v.w, 0.16, 3, 0x5a3c22, 0, -0.165, 0);
      for (const sz of [-1, 1]) A.box(v.w, 0.2, 0.14, 0x3b2616, 0, -0.38, sz * 1.2);
    }
    A.rim(v);
    return true;
  },

  buildWalls(A) {
    const W = A.w, H = A.h, t = 0.6, wl = this.walls;
    const cap = A.mat(0xc7996a), top = A.mat(0xe2c7a0);
    const gap = 0.9; // medio ancho del hueco entre edificios
    const len = W / 2 + t - gap;
    // pretiles del fondo y laterales, con rótulos pintados a mano
    const faceTex = A.signCanvas(W + t * 2, wl.back, 2048, 128, (ctx, cw, ch) => this.paintParapet(ctx, cw, ch, A));
    for (const sx of [-1, 1]) {
      const tx = A.own(faceTex.clone());
      tx.repeat.set(len / (W + t * 2), 1);
      tx.offset.set(sx < 0 ? 0 : 1 - len / (W + t * 2), 0);
      tx.needsUpdate = true;
      const m = new THREE.Mesh(GEO.box, [cap, cap, top, cap, A.tmat(tx), cap]);
      m.scale.set(len, wl.back, t);
      m.position.set(sx * (gap + len / 2), wl.back / 2, -H / 2 - t / 2);
      A.group.add(m);
      // frente bajito de cada edificio
      A.box(len, wl.front, t, 0xb98a5e, sx * (gap + len / 2), 0, H / 2 + t / 2);
    }
    const sideTex = A.own(this.sideTexture());
    for (const sx of [-1, 1]) {
      const tx = A.own(sideTex.clone());
      tx.wrapS = THREE.RepeatWrapping; tx.repeat.set(H / 13, 1); tx.needsUpdate = true;
      const inner = A.tmat(tx);
      const m = new THREE.Mesh(GEO.box, sx < 0 ? [inner, cap, top, cap, cap, cap] : [cap, inner, top, cap, cap, cap]);
      m.scale.set(t, wl.side, H);
      m.position.set(sx * (W / 2 + t / 2), wl.side / 2, 0);
      A.group.add(m);
    }
    // el hueco se cierra con malla ciclónica al fondo y un barandal bajo al frente
    const fence = A.add(GEO.plane, A.own(new THREE.MeshBasicMaterial({ map: fenceTexture(), transparent: true, alphaTest: 0.3, side: THREE.DoubleSide })), 0, 0.85, -H / 2 - 0.05, gap * 2, 1.7, 1);
    fence.renderOrder = 1;
    for (const sx of [-1, 1]) A.cyl(0.05, 1.75, 0x8d8d8d, sx * gap, 0, -H / 2 - 0.05, A.group, GEO.cyl8);
    A.box(gap * 2, 0.05, 0.05, 0x8d8d8d, 0, 1.7, -H / 2 - 0.05);
    for (const y of [0.16, 0.32]) A.box(gap * 2, 0.05, 0.05, 0xd8d24a, 0, y, H / 2 + 0.05);
  },

  paintParapet(ctx, cw, ch, A) {
    const r = A.rand;
    ctx.fillStyle = '#d9a46c'; ctx.fillRect(0, 0, cw, ch);
    ctx.fillStyle = '#3f6fa8'; ctx.fillRect(0, ch - 30, cw, 30); // guardapolvo azul
    for (let i = 0; i < 260; i++) {
      ctx.fillStyle = `rgba(0,0,0,${0.03 + r() * 0.05})`;
      ctx.fillRect(r() * cw, r() * ch, 4 + r() * 22, 3 + r() * 12);
    }
    ctx.textBaseline = 'alphabetic'; ctx.textAlign = 'left';
    ctx.fillStyle = '#c1121f'; ctx.font = `52px ${FONTS.block}`;
    ctx.fillText('SE RENTA CUARTO', 150, 62);
    ctx.fillStyle = '#1d3557'; ctx.font = `44px ${FONTS.block}`;
    ctx.fillText('NO TIRAR BASURA', 1560, 60);
    ctx.fillStyle = '#2a9d8f'; ctx.font = `40px ${FONTS.tag}`;
    ctx.fillText('azotea crew', 2300, 58);
    ctx.fillStyle = '#1d3557'; ctx.fillText('CDMX', 820, 58);
  },

  decor(A) {
    const W = A.w, H = A.h;
    // los dos edificios (se les ve la fachada del frente) y la ciudad allá abajo
    for (const sx of [-1, 1]) A.box(W / 2 + 0.6 - 0.95, 14, H + 1.2, 0x7a4a3c, sx * (0.95 + (W / 2 + 0.6 - 0.95) / 2), -14.02, 0);
    A.add(GEO.flat, A.own(new THREE.MeshBasicMaterial({ map: cityTexture() })), 0, -14, 0, 180, 1, 180);
    // edificios vecinos más bajos, con sus tinacos y cuartos de azotea
    const cols = [0x9a5a44, 0x8a6a5a, 0xa8684a, 0x77525a, 0xb07a52];
    const blocks = [
      // [x, z, ancho, fondo, altura del techo]
      [-21, -6, 11, 12, -3.5], [-22, 8, 12, 12, -6], [21, -4, 11, 13, -2.6], [22, 10, 12, 12, -5.5],
      [-9, -17, 13, 11, -3], [6, -18, 14, 12, -5], [22, -18, 12, 11, -4], [-24, -19, 12, 12, -6.5],
      [-12, 19, 16, 12, -7], [8, 20, 18, 12, -6],
    ];
    blocks.forEach(([x, z, bw, bd, topY], i) => {
      A.box(bw, 14 + topY, bd, cols[i % cols.length], x, -14, z);
      A.box(bw + 0.3, 0.3, bd + 0.3, 0xd9b48c, x, topY, z);
      A.cyl(0.7, 1.3, 0x17171b, x + bw * 0.25, topY + 0.3, z - bd * 0.2);
      A.box(2.6, 1.6, 2.2, 0xe3c9a6, x - bw * 0.22, topY + 0.3, z + bd * 0.18);
    });
    // ESPECTACULAR "GORRAZO by INKCAP" sobre el pretil del edificio izquierdo
    const bbW = 6.6, bbH = 2.25, bbX = -10.05;
    const bb = new THREE.Group();
    bb.position.set(bbX, 1.0, -H / 2 - 0.42);
    bb.rotation.x = -0.4; // recargado hacia atrás: se lee mejor desde la cámara
    const tex = A.signCanvas(bbW, bbH, 1024, 350, (ctx, cw, ch) => this.paintBillboard(ctx, cw, ch));
    const frame = A.mat(0x1c1b20);
    const panel = new THREE.Mesh(GEO.box, [frame, frame, frame, frame, A.glow(0xffffff, { map: tex }), frame]);
    panel.scale.set(bbW, bbH, 0.14); panel.position.y = bbH / 2 + 0.12;
    bb.add(panel);
    for (const sx of [-1, 1]) A.box(0.14, 0.2, 0.14, 0x1c1b20, sx * (bbW / 2 - 0.5), 0, 0, bb);
    for (const lx of [-2.2, 0, 2.2]) {
      A.box(0.05, 0.05, 0.5, 0x1c1b20, lx, bbH + 0.2, 0.3, bb);
      A.add(GEO.sphere, A.glow(0xfff4d6), lx, bbH + 0.16, 0.56, 0.08, 0.08, 0.08, bb);
    }
    A.group.add(bb);
    for (const sx of [-1, 1]) { // tirantes de la estructura
      const leg = A.box(0.1, 2.5, 0.1, 0x2a2930, bbX + sx * (bbW / 2 - 0.5), 0.9, -H / 2 - 1.45);
      leg.rotation.x = 0.35;
    }
    // TENDEDERO con playeras INKCAP reales, sobre el pretil del edificio derecho
    const x0 = 5.0, x1 = 12.9, yT = 2.72, zT = -H / 2 - 0.3;
    for (const x of [x0, x1]) A.cyl(0.05, yT - 0.95, 0x6b6b6b, x, 0.98, zT, A.group, GEO.cyl8);
    A.box(x1 - x0, 0.025, 0.025, 0x2b2b2b, (x0 + x1) / 2, yT - 0.08, zT);
    const ids = brandShirtIds();
    const n = 5;
    for (let i = 0; i < n; i++) {
      const x = x0 + 0.85 + i * (x1 - x0 - 1.7) / (n - 1);
      const p = hangShirt(A, ids[i % ids.length], A.group, x, yT - 0.06, zT + 0.02, 1.42, -0.42);
      A.anims.push((t) => { p.rotation.x = -0.42 + Math.sin(t * 1.6 + i * 1.3) * 0.07; p.rotation.z = Math.sin(t * 1.1 + i) * 0.03; });
    }
    // antena, parabólica y macetas en los pretiles
    A.cyl(0.035, 2.6, 0x8d8d8d, 13.3, 1.0, -H / 2 - 0.3, A.group, GEO.cyl8);
    for (const y of [2.6, 3.0, 3.3]) A.box(0.9 - (y - 2.6), 0.03, 0.03, 0x8d8d8d, 13.3, y, -H / 2 - 0.3);
    const dish = A.add(GEO.dome, 0xdfe3e6, -13.3, 1.5, 3.5, 0.55, 0.22, 0.55);
    dish.rotation.set(0.4, 0, 1.0);
    A.cyl(0.04, 0.5, 0x8d8d8d, -13.3, 1.0, 3.5, A.group, GEO.cyl8);
    for (const [x, z] of [[-13.3, -2.2], [13.3, 2.6], [-3.2, -H / 2 - 0.3], [13.3, -4.8]]) {
      A.cyl(0.2, 0.3, 0xa8563a, x, 1.0, z);
      A.add(GEO.sphere, 0x3f8f4a, x, 1.5, z, 0.2, 0.3, 0.1);
    }
    // foquitos de colores entre el espectacular y el tendedero
    const bulbs = [0xff2bd6, 0xffe600, 0x00e5ff, 0x39ff14, 0xff6a2b];
    for (let i = 0; i <= 10; i++) {
      const k = i / 10;
      A.add(GEO.sphere, A.glow(bulbs[i % bulbs.length]), -6.6 + k * 11.5, 2.5 - Math.sin(k * Math.PI) * 0.75, -H / 2 - 0.35, 0.08, 0.08, 0.08);
    }
  },

  paintBillboard(ctx, cw, ch) {
    ctx.fillStyle = INK; ctx.fillRect(0, 0, cw, ch);
    const strip = ch * 0.17;
    ctx.fillStyle = LIME; ctx.fillRect(0, ch - strip, cw, strip);
    ctx.strokeStyle = LIME; ctx.lineWidth = 7; ctx.strokeRect(3.5, 3.5, cw - 7, ch - 7);
    // "GORRAZO" con el desfase de serigrafía del título del juego
    const fs = ch * 0.5;
    ctx.font = `${fs}px ${FONTS.block}`; ctx.textAlign = 'left'; ctx.textBaseline = 'alphabetic';
    const title = 'GORRAZO';
    const tw = Math.min(cw - 50, ctx.measureText(title).width);
    const tx = (cw - tw) / 2, ty = ch * 0.5;
    ctx.fillStyle = RED; ctx.fillText(title, tx - 6, ty + 5, tw);
    ctx.fillStyle = LIME; ctx.fillText(title, tx + 6, ty - 4, tw);
    ctx.fillStyle = BONE; ctx.fillText(title, tx, ty, tw);
    const lw = cw * 0.36, ly = ch * 0.68;
    ctx.font = `${ch * 0.15}px ${FONTS.tag}`; ctx.fillStyle = BONE; ctx.textAlign = 'right';
    ctx.fillText('by', cw / 2 - lw / 2 + cw * 0.02, ly + ch * 0.05);
    drawLogo(ctx, cw / 2 + cw * 0.07, ly, lw, BONE);
    ctx.fillStyle = INK; ctx.font = brandFont(strip * 0.72); ctx.textAlign = 'center';
    ctx.fillText('LANZA TU GORRA, MANCHA A TODOS', cw / 2, ch - strip * 0.26, cw - 30);
  },
};

// ═════════════════════════════════════════════════════════════
// B4 · POP-UP STORE INKCAP
// ═════════════════════════════════════════════════════════════
const popup = {
  walls: { back: 2.6, side: 1.3, front: 0.35, capColor: 0x17161a, topColor: 0x2a2a2e, sideColor: 0x1d1c21, frontColor: 0x17161a, backUnlit: true },
  outer: 0x101014,
  pit: 'tienda', pitDepth: 3, rim: 'franjas',
  lights: { sky: 0xffffff, ground: 0x55545c, sun: 0xfff6ea },

  paintFloor(ctx, F) {
    const { W, H, r, X, Z, M } = F;
    // concreto pulido claro: las manchas de tinta de los KO se ven de lejos
    ctx.fillStyle = '#cbc7bf'; ctx.fillRect(0, 0, W, H);
    for (let i = 0; i < 12; i++) {
      ctx.fillStyle = `rgba(${r() < 0.5 ? '255,255,255' : '90,86,80'},${0.05 + r() * 0.06})`;
      ctx.beginPath(); ctx.ellipse(r() * W, r() * H, 60 + r() * 160, 30 + r() * 90, r() * 3, 0, Math.PI * 2); ctx.fill();
    }
    speckle(ctx, W, H, r, 2200, 0.08, false, 2);
    ctx.strokeStyle = 'rgba(0,0,0,0.12)'; ctx.lineWidth = 1.5;
    for (let x = M(2.4); x < W; x += M(2.4)) { ctx.beginPath(); ctx.moveTo(x, 0); ctx.lineTo(x, H); ctx.stroke(); }
    for (let y = M(2.5); y < H; y += M(2.5)) { ctx.beginPath(); ctx.moveTo(0, y); ctx.lineTo(W, y); ctx.stroke(); }
    // franja negra perimetral con filo neón
    const b = M(0.5);
    ctx.fillStyle = '#19181c';
    ctx.fillRect(0, 0, W, b); ctx.fillRect(0, H - b, W, b); ctx.fillRect(0, 0, b, H); ctx.fillRect(W - b, 0, b, H);
    ctx.strokeStyle = LIME; ctx.lineWidth = 3; ctx.strokeRect(b + 1.5, b + 1.5, W - b * 2 - 3, H - b * 2 - 3);
    // tapete redondo bajo la mesa de gorras y tapetes de los racks
    ctx.fillStyle = '#1b1a1f'; ctx.beginPath(); ctx.arc(X(0), Z(0), M(2.35), 0, Math.PI * 2); ctx.fill();
    ctx.strokeStyle = LIME; ctx.lineWidth = 4; ctx.beginPath(); ctx.arc(X(0), Z(0), M(2.2), 0, Math.PI * 2); ctx.stroke();
    ctx.strokeStyle = 'rgba(244,241,234,0.5)'; ctx.lineWidth = 1.5; ctx.beginPath(); ctx.arc(X(0), Z(0), M(2.05), 0, Math.PI * 2); ctx.stroke();
    for (const o of F.arena.obstacles) if (o.kind === 'rack') {
      ctx.fillStyle = '#2a292f';
      rr(ctx, X(o.x - o.w / 2 - 0.4), Z(o.z - o.d / 2 - 0.35), M(o.w + 0.8), M(o.d + 0.9), M(0.18)); ctx.fill();
    }
    // LOGO INKCAP grande en el piso (frente de la tienda)
    drawLogo(ctx, X(0), Z(4.5), M(7), '#17161a');
    ctx.fillStyle = '#17161a'; ctx.font = brandFont(M(0.42)); ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
    ctx.fillText('POP-UP STORE · CDMX', X(0), Z(6.05));
    // flechas de cinta neón hacia la pared del logo
    ctx.fillStyle = 'rgba(23,22,26,0.8)'; ctx.font = brandFont(M(0.5));
    ctx.fillText('ESTRENOS · SNAPBACKS · PLAYERAS', X(0), Z(-5.6));
    ctx.fillStyle = LIME;
    for (const sx of [-1, 1]) ctx.fillRect(X(sx * 6.4) - M(1.1), Z(-5.6) - 2.5, M(2.2), 5);
    // señal de "SÓTANO" junto a cada escalera
    for (const v of F.arena.voids) {
      const zz = Z(v.z + v.d / 2 + 0.72);
      ctx.fillStyle = '#17161a'; rr(ctx, X(v.x) - M(0.95), zz - M(0.24), M(1.9), M(0.48), 5); ctx.fill();
      ctx.fillStyle = '#ffd400'; ctx.font = brandFont(M(0.36)); ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
      ctx.fillText('▲ SÓTANO', X(v.x), zz + 1);
    }
    F.shadows(-0.08, -0.14, 0.4);
    // luces de techo de la tienda y resplandor del logo iluminado del fondo
    for (const lx of [-8, -2.7, 2.7, 8]) for (const lz of [-3.6, 3.6]) lightPool(ctx, X(lx), Z(lz), M(3.4), '255,255,255', 0.1);
    lightPool(ctx, X(0), Z(-7.2), M(5.5), '204,255,0', 0.16, 0.6);
    vignette(ctx, W, H, 0.3);
  },

  // Muro del fondo (cm): pared de gorras, LOGO grande iluminado y cartel "ESTRENOS"
  paintBackWall(ctx, cw, ch, A, X) {
    ctx.fillStyle = '#141317'; ctx.fillRect(0, 0, cw, ch);
    ctx.fillStyle = 'rgba(255,255,255,0.03)';
    for (let x = 0; x < cw; x += 120) ctx.fillRect(x, 0, 2, ch);
    // tablero perforado de la pared de gorras (las 6 snapbacks van en 3D encima)
    const px0 = X(-11.55), pw = 590;
    ctx.fillStyle = '#26252b'; rr(ctx, px0, 12, pw, 236, 10); ctx.fill();
    ctx.fillStyle = 'rgba(255,255,255,0.09)';
    for (let y = 24; y < 240; y += 13) for (let x = px0 + 12; x < px0 + pw - 6; x += 13) ctx.fillRect(x, y, 2.5, 2.5);
    // (las gorras en 3D tapan la parte de arriba: el rótulo va abajo)
    ctx.fillStyle = LIME; ctx.font = brandFont(44); ctx.textAlign = 'left'; ctx.textBaseline = 'alphabetic';
    ctx.fillText('SNAPBACKS', px0 + 20, 240);
    ctx.fillStyle = BONE; ctx.font = `600 21px ${FONTS.brand}`; ctx.textAlign = 'right';
    ctx.fillText('diseñado por artistas · hecho en México', px0 + pw - 20, 238);
    // LOGO INKCAP grande e iluminado
    const cx = X(0);
    lightPool(ctx, cx, 118, 520, '255,255,255', 0.2, 0.42);
    lightPool(ctx, cx, 226, 430, '204,255,0', 0.22, 0.2);
    ctx.save(); ctx.shadowColor = 'rgba(255,255,255,0.9)'; ctx.shadowBlur = 26;
    drawLogo(ctx, cx, 112, 640, '#ffffff');
    ctx.restore();
    drawLogo(ctx, cx, 112, 640, '#ffffff');
    ctx.save(); ctx.shadowColor = LIME; ctx.shadowBlur = 20;
    ctx.fillStyle = LIME; rr(ctx, cx - 320, 226, 640, 8, 4); ctx.fill();
    ctx.restore();
    // CARTEL "ESTRENOS": caja de luz con los diseños nuevos de los artistas
    const ex = X(5.7), ew = 585;
    ctx.save(); ctx.shadowColor = 'rgba(204,255,0,0.8)'; ctx.shadowBlur = 22;
    ctx.fillStyle = LIME; rr(ctx, ex, 12, ew, 236, 10); ctx.fill();
    ctx.restore();
    ctx.fillStyle = INK; ctx.font = brandFont(66); ctx.textAlign = 'left'; ctx.textBaseline = 'alphabetic';
    ctx.fillText('ESTRENOS', ex + 20, 72);
    ctx.font = `600 20px ${FONTS.brand}`; ctx.textAlign = 'right';
    ctx.fillText('NUEVOS DISEÑOS DE ARTISTAS', ex + ew - 20, 44);
    ctx.fillText('SOLO EN INKCAP', ex + ew - 20, 68);
    const list = artists();
    const ts = 124, gapx = (ew - ts * 4) / 5;
    for (let i = 0; i < 4; i++) {
      const a = list.length ? list[i % list.length] : null;
      const x = ex + gapx + i * (ts + gapx), y = 88;
      ctx.fillStyle = INK; ctx.fillRect(x - 4, y - 4, ts + 8, ts + 8);
      artPanel(ctx, a, x, y, ts, { caption: false, paper: INK });
      ctx.fillStyle = INK; ctx.font = brandFont(17); ctx.textAlign = 'center';
      ctx.fillText((a ? a.name : 'PRÓXIMAMENTE').toUpperCase(), x + ts / 2, y + ts + 22, ts + 14);
    }
  },

  // Escalera al sótano: escalones de verdad que bajan hacia el fondo y se pierden en lo oscuro
  buildVoid(A, v) {
    A.pit(v.x, v.z, v.w, v.d, 3, 'tienda');
    const n = 6, run = v.d / n;
    const shades = [0xa39f96, 0x858178, 0x66635c, 0x494742, 0x2e2d2a, 0x191819];
    for (let k = 0; k < n; k++) {
      const top = -0.24 - k * 0.44;
      const z = v.z + v.d / 2 - (k + 0.5) * run;
      A.add(GEO.box, A.glow(shades[k]), v.x, (top - 3) / 2, z, v.w - 0.03, 3 + top, run, A.group);
      if (k < 4) A.add(GEO.box, A.glow(0xffd400), v.x, top + 0.006, z + run / 2 - 0.035, v.w - 0.03, 0.012, 0.07); // filo antiderrapante
    }
    A.rim(v);
    return true;
  },

  decor(A) {
    const W = A.w, H = A.h, wl = this.walls;
    const lime = A.glow(LIME_HEX);
    // tiras de LED neón en el remate de los muros
    A.add(GEO.box, lime, 0, wl.back + 0.01, -H / 2 - 0.02, W, 0.06, 0.06);
    for (const sx of [-1, 1]) A.add(GEO.box, lime, sx * (W / 2 + 0.03), wl.side + 0.01, 0, 0.06, 0.06, H);
    A.add(GEO.box, lime, 0, wl.front + 0.01, H / 2 + 0.03, W, 0.05, 0.05);
    // resplandor del logo iluminado (late suavecito) y barra de neón debajo
    const gl = A.add(GEO.plane, A.own(new THREE.MeshBasicMaterial({ map: softDotTexture(), color: 0xffffff, transparent: true, opacity: 0.2, depthWrite: false, blending: THREE.AdditiveBlending, fog: false })), 0, 1.45, -H / 2 + 0.03, 10.5, 3.4, 1);
    gl.renderOrder = 2;
    A.anims.push((t) => { gl.material.opacity = 0.18 + Math.sin(t * 2.1) * 0.05 + (Math.sin(t * 17) > 0.96 ? -0.08 : 0); });
    // PARED DE GORRAS: las 6 snapbacks reales en una repisa alta (no estorba a nadie)
    const ids = brandCapIds();
    A.box(5.8, 0.05, 0.62, 0x26252b, -8.6, 1.78, -H / 2 + 0.31);
    A.add(GEO.box, lime, -8.6, 1.79, -H / 2 + 0.63, 5.8, 0.035, 0.03);
    for (let i = 0; i < 6; i++) displayCap(A, ids[i % ids.length], A.group, -11.1 + i * 1.0, 1.84, -H / 2 + 0.36, 1.0);
    // MOSTRADOR con la taza INKCAP (fuera del área de juego, a la derecha)
    const c = new THREE.Group(); c.position.set(W / 2 + 1.85, 0, -3.6); A.group.add(c);
    A.box(1.7, 1.0, 4.4, 0x1b1a1f, 0, 0, 0, c);
    A.box(1.9, 0.08, 4.6, 0xd9bd92, 0, 1.0, 0, c);
    A.add(GEO.box, lime, -0.86, 0.55, 0, 0.03, 0.05, 4.4, c);
    sticker(A, logoStickerTexture(BONE, null), c, -0.861, 0.62, 0, 2.6, 0.8, { rotY: -Math.PI / 2 });
    const mug = buildMug(4.2); mug.position.set(0, 1.6, 1.2); mug.rotation.y = -0.5; c.add(mug);
    A.add(GEO.disc, 0x26252b, 0, 1.085, 1.2, 0.6, 1, 0.6, c);
    A.box(0.7, 0.42, 0.6, 0x2a292f, 0.1, 1.08, -0.4, c);            // caja registradora
    A.add(GEO.box, lime, 0.1, 1.42, -0.4, 0.5, 0.26, 0.05, c).rotation.x = -0.5;
    const fab = [0x141414, 0xc78cd2, 0xeeede9];
    fab.forEach((col, i) => A.box(0.7, 0.1, 0.5, col, 0, 1.08 + i * 0.1, -1.6, c));  // playeras dobladas
    for (const [bx, bz, col] of [[-0.3, 2.75, LIME_HEX], [0.45, 2.9, 0x17161a]]) { // bolsas de compra
      A.box(0.6, 0.75, 0.3, col, bx, 0, bz, c);
      sticker(A, logoStickerTexture(col === LIME_HEX ? INK : BONE, null), c, bx, 0.4, bz + 0.155, 0.5, 0.16);
    }
    halo(A, c, 0, 1.7, 1.2, 3, 0xfff6e0, 0.28);
    // BODEGA con cajas DROP apiladas (a la izquierda)
    const s = new THREE.Group(); s.position.set(-W / 2 - 2.35, 0, -3.6); A.group.add(s);
    const dropMat = A.tmat(dropBoxTexture());
    A.box(2.9, 0.14, 3.2, 0x8a6a43, 0, 0, 0.1, s); // tarima
    for (const [x, y, z, ry] of [[-0.62, 0.14, -0.6, 0.1], [0.62, 0.14, -0.5, -0.12], [-0.6, 0.14, 0.8, -0.05], [0.65, 0.14, 0.85, 0.2], [0, 1.34, -0.4, 0.3], [0.1, 1.34, 0.9, -0.2]]) {
      const b = A.add(GEO.box, dropMat, x, y + 0.6, z, 1.2, 1.2, 1.2, s); b.rotation.y = ry;
    }
    // filo neón en el piso exterior, para que la tienda "flote" en lo oscuro
    for (const sx of [-1, 1]) A.add(GEO.flat, A.glow(0x1d1c22), sx * (W / 2 + 3.4), -0.008, 0, 5.6, 1, H + 6);
  },
};

export const ESCENAS = { callejon, museo, azotea, popup };

// "Estilo" de fondo del cielo de cada arena (degradado para el menú y el podio)
export function skyColors(id) {
  return {
    callejon: ['#0d0a1a', '#2a1f4a'],
    museo: ['#080c14', '#1a2334'],
    azotea: ['#5b3b7a', '#ff9a62'],
    popup: ['#08080b', '#17161c'],
  }[id] || ['#101018', '#202030'];
}
