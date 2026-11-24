// ─────────────────────────────────────────────────────────────
// TEXTURAS PROCEDURALES (canvas → textura)
// En el juego real, cada playera será el PNG del diseño del artista
// aplicado al UV de la playera. Aquí lo simulamos dibujando en canvas,
// para demostrar que "un diseño = una textura intercambiable".
// ─────────────────────────────────────────────────────────────
import * as THREE from 'three';
import { MARCA, tintedLogo } from './marca.js';

function makeCanvas(w, h) {
  const c = document.createElement('canvas');
  c.width = w; c.height = h;
  return c;
}

// Generador pseudoaleatorio con semilla: las texturas salen iguales siempre.
export function rng(seed) {
  let s = seed >>> 0 || 1;
  return () => {
    s ^= s << 13; s ^= s >>> 17; s ^= s << 5;
    return ((s >>> 0) % 100000) / 100000;
  };
}

export function toCanvasTexture(canvas, { repeat = null, srgb = true } = {}) {
  const t = new THREE.CanvasTexture(canvas);
  if (srgb) t.colorSpace = THREE.SRGBColorSpace;
  t.anisotropy = 4;
  if (repeat) {
    t.wrapS = t.wrapT = THREE.RepeatWrapping;
    t.repeat.set(repeat[0], repeat[1]);
  }
  return t;
}

// Rampa de 3 tonos para el look "juguete de vinil / caricatura".
let _toonRamp = null;
export function toonRamp() {
  if (_toonRamp) return _toonRamp;
  const data = new Uint8Array([90, 90, 90, 255, 175, 175, 175, 255, 255, 255, 255, 255]);
  _toonRamp = new THREE.DataTexture(data, 3, 1, THREE.RGBAFormat);
  _toonRamp.minFilter = THREE.NearestFilter;
  _toonRamp.magFilter = THREE.NearestFilter;
  _toonRamp.generateMipmaps = false;
  _toonRamp.needsUpdate = true;
  return _toonRamp;
}

// ── Utilidades de dibujo ─────────────────────────────────────
export function drawSplat(ctx, x, y, r, color, rand, alpha = 1) {
  ctx.save();
  ctx.globalAlpha = alpha;
  ctx.fillStyle = color;
  ctx.beginPath();
  const n = 14;
  for (let i = 0; i <= n; i++) {
    const a = (i / n) * Math.PI * 2;
    const rr = r * (0.7 + rand() * 0.5);
    const px = x + Math.cos(a) * rr;
    const py = y + Math.sin(a) * rr;
    if (i === 0) ctx.moveTo(px, py); else ctx.lineTo(px, py);
  }
  ctx.closePath();
  ctx.fill();
  // gotas satélite
  const drops = 5 + Math.floor(rand() * 6);
  for (let i = 0; i < drops; i++) {
    const a = rand() * Math.PI * 2;
    const d = r * (1.1 + rand() * 1.1);
    const dr = r * (0.08 + rand() * 0.16);
    ctx.beginPath();
    ctx.arc(x + Math.cos(a) * d, y + Math.sin(a) * d, dr, 0, Math.PI * 2);
    ctx.fill();
  }
  ctx.restore();
}

function drawDrips(ctx, x, y, w, color, rand, maxLen = 40) {
  ctx.fillStyle = color;
  const n = 3 + Math.floor(rand() * 5);
  for (let i = 0; i < n; i++) {
    const dx = x + rand() * w;
    const len = 8 + rand() * maxLen;
    const dw = 2 + rand() * 4;
    ctx.fillRect(dx, y, dw, len);
    ctx.beginPath();
    ctx.arc(dx + dw / 2, y + len, dw * 0.9, 0, Math.PI * 2);
    ctx.fill();
  }
}

function drawStar(ctx, x, y, r, points = 5) {
  ctx.beginPath();
  for (let i = 0; i < points * 2; i++) {
    const a = (i / (points * 2)) * Math.PI * 2 - Math.PI / 2;
    const rr = i % 2 === 0 ? r : r * 0.45;
    const px = x + Math.cos(a) * rr, py = y + Math.sin(a) * rr;
    if (i === 0) ctx.moveTo(px, py); else ctx.lineTo(px, py);
  }
  ctx.closePath();
}

function drawHeart(ctx, x, y, s) {
  ctx.beginPath();
  ctx.moveTo(x, y + s * 0.35);
  ctx.bezierCurveTo(x - s, y - s * 0.4, x - s * 0.4, y - s, x, y - s * 0.45);
  ctx.bezierCurveTo(x + s * 0.4, y - s, x + s, y - s * 0.4, x, y + s * 0.35);
  ctx.closePath();
}

const TAG_WORDS = ['TINTA', 'CAPS', 'KO!', 'CDMX', 'MANCHA', 'ESTILO', 'CREW', 'ARTE', 'ZAZ', 'POW', 'BRAVO', 'NEÓN'];
const GRAFFITI_FONT = '"Permanent Marker", "Marker Felt", "Comic Sans MS", system-ui, sans-serif';
const BLOCK_FONT = '"Bungee", "Impact", "Arial Black", system-ui, sans-serif';

// ── Playeras (diseños de ejemplo) ────────────────────────────
// Cada diseño es una función que pinta un canvas 256×256.
// El área "pecho" es el centro; el canvas se envuelve en el torso.
export const SHIRT_DESIGNS = {
  'basica-blanca': {
    name: 'Básica Blanca', price: 0, rarity: 'Común',
    paint(ctx, W, H) {
      ctx.fillStyle = '#f2f2ee'; ctx.fillRect(0, 0, W, H);
    },
  },
  'basica-negra': {
    name: 'Básica Negra', price: 0, rarity: 'Común',
    paint(ctx, W, H) {
      ctx.fillStyle = '#1c1c1f'; ctx.fillRect(0, 0, W, H);
    },
  },
  'tinta-derramada': {
    name: 'Tinta Derramada', price: 300, rarity: 'Rara',
    paint(ctx, W, H, r) {
      ctx.fillStyle = '#f4f1ea'; ctx.fillRect(0, 0, W, H);
      ctx.fillStyle = '#111';
      ctx.fillRect(0, 0, W, 34);
      drawDrips(ctx, 0, 30, W, '#111', r, 70);
      drawSplat(ctx, W * 0.5, H * 0.58, 38, '#111', r);
    },
  },
  'neon-grafiti': {
    name: 'Neón Grafiti', price: 300, rarity: 'Rara',
    paint(ctx, W, H, r) {
      ctx.fillStyle = '#16121f'; ctx.fillRect(0, 0, W, H);
      const cols = ['#39ff14', '#ff2bd6', '#00e5ff', '#ffe600'];
      for (let i = 0; i < 6; i++) {
        ctx.save();
        ctx.translate(r() * W, 40 + r() * (H - 60));
        ctx.rotate(-0.3 + r() * 0.6);
        ctx.font = `bold ${22 + r() * 18}px ${GRAFFITI_FONT}`;
        ctx.fillStyle = cols[i % cols.length];
        ctx.shadowColor = cols[i % cols.length]; ctx.shadowBlur = 10;
        ctx.fillText(TAG_WORDS[Math.floor(r() * TAG_WORDS.length)], -30, 0);
        ctx.restore();
      }
    },
  },
  'serigrafia-retro': {
    name: 'Serigrafía Retro', price: 250, rarity: 'Rara',
    paint(ctx, W, H) {
      ctx.fillStyle = '#ffcf4a'; ctx.fillRect(0, 0, W, H);
      // semitono (halftone)
      for (let y = 0; y < H; y += 10) {
        for (let x = 0; x < W; x += 10) {
          const d = Math.hypot(x - W / 2, y - H * 0.55) / (W * 0.55);
          const rr = Math.max(0, 4.6 * (1 - d));
          ctx.fillStyle = '#e8432e';
          ctx.beginPath(); ctx.arc(x + ((y / 10) % 2) * 5, y, rr, 0, Math.PI * 2); ctx.fill();
        }
      }
      ctx.fillStyle = '#1d3557';
      ctx.font = `48px ${BLOCK_FONT}`;
      ctx.textAlign = 'center';
      ctx.fillText('KO', W / 2, H * 0.62);
    },
  },
  'bordado-floral': {
    name: 'Bordado Floral', price: 400, rarity: 'Épica',
    paint(ctx, W, H, r) {
      ctx.fillStyle = '#20314f'; ctx.fillRect(0, 0, W, H);
      const cols = ['#ff5d8f', '#ffd166', '#06d6a0', '#f4f1ea'];
      ctx.setLineDash([4, 3]);
      ctx.lineWidth = 3;
      for (let i = 0; i < 7; i++) {
        const x = 30 + r() * (W - 60), y = 50 + r() * (H - 90), s = 12 + r() * 12;
        const c = cols[i % cols.length];
        for (let p = 0; p < 6; p++) {
          const a = (p / 6) * Math.PI * 2;
          ctx.strokeStyle = c;
          ctx.beginPath();
          ctx.ellipse(x + Math.cos(a) * s, y + Math.sin(a) * s, s * 0.6, s * 0.35, a, 0, Math.PI * 2);
          ctx.stroke();
        }
        ctx.fillStyle = '#ffd166';
        ctx.beginPath(); ctx.arc(x, y, s * 0.35, 0, Math.PI * 2); ctx.fill();
      }
      ctx.setLineDash([]);
    },
  },
  'sticker-bomb': {
    name: 'Sticker Bomb', price: 350, rarity: 'Rara',
    paint(ctx, W, H, r) {
      ctx.fillStyle = '#ececec'; ctx.fillRect(0, 0, W, H);
      const cols = ['#ff6a2b', '#22c7e8', '#ffd23f', '#e84393', '#4cd964', '#8e6cff', '#111'];
      for (let i = 0; i < 26; i++) {
        const x = r() * W, y = r() * H, s = 14 + r() * 20;
        ctx.save();
        ctx.translate(x, y); ctx.rotate(r() * Math.PI);
        ctx.fillStyle = '#fff'; ctx.strokeStyle = '#fff'; ctx.lineWidth = 5;
        const kind = Math.floor(r() * 3);
        ctx.fillStyle = cols[i % cols.length];
        if (kind === 0) { drawStar(ctx, 0, 0, s); ctx.stroke(); ctx.fill(); }
        else if (kind === 1) { ctx.beginPath(); ctx.arc(0, 0, s * 0.7, 0, Math.PI * 2); ctx.stroke(); ctx.fill(); }
        else { ctx.strokeRect(-s * 0.7, -s * 0.45, s * 1.4, s * 0.9); ctx.fillRect(-s * 0.7, -s * 0.45, s * 1.4, s * 0.9); }
        ctx.restore();
      }
    },
  },
  'tatuaje-old-school': {
    name: 'Tatuaje Old School', price: 450, rarity: 'Épica',
    paint(ctx, W, H) {
      ctx.fillStyle = '#efe6d2'; ctx.fillRect(0, 0, W, H);
      ctx.lineWidth = 5; ctx.strokeStyle = '#111';
      ctx.fillStyle = '#d62828';
      drawHeart(ctx, W / 2, H * 0.55, 46); ctx.fill(); ctx.stroke();
      // listón
      ctx.fillStyle = '#f6bd60';
      ctx.fillRect(W / 2 - 70, H * 0.52, 140, 26); ctx.strokeRect(W / 2 - 70, H * 0.52, 140, 26);
      ctx.fillStyle = '#111';
      ctx.font = `bold 18px ${BLOCK_FONT}`; ctx.textAlign = 'center';
      ctx.fillText('MAMÁ', W / 2, H * 0.52 + 20);
      ctx.fillStyle = '#2a9d8f';
      drawStar(ctx, W * 0.2, H * 0.3, 16); ctx.fill(); ctx.stroke();
      drawStar(ctx, W * 0.8, H * 0.3, 16); ctx.fill(); ctx.stroke();
    },
  },
  'rayas-lucha': {
    name: 'Rayas de Lucha', price: 500, rarity: 'Legendaria',
    paint(ctx, W, H) {
      ctx.fillStyle = '#c1121f'; ctx.fillRect(0, 0, W, H);
      ctx.fillStyle = '#fdf0d5';
      for (let i = -H; i < W; i += 34) {
        ctx.beginPath();
        ctx.moveTo(i, 0); ctx.lineTo(i + 14, 0); ctx.lineTo(i + 14 + H, H); ctx.lineTo(i + H, H);
        ctx.fill();
      }
      ctx.fillStyle = '#003049';
      ctx.beginPath(); ctx.ellipse(W / 2, H * 0.56, 40, 48, 0, 0, Math.PI * 2); ctx.fill();
      ctx.fillStyle = '#fdf0d5';
      ctx.beginPath(); ctx.ellipse(W / 2 - 15, H * 0.53, 11, 7, -0.3, 0, Math.PI * 2); ctx.fill();
      ctx.beginPath(); ctx.ellipse(W / 2 + 15, H * 0.53, 11, 7, 0.3, 0, Math.PI * 2); ctx.fill();
    },
  },
  'camuflaje-tinta': {
    name: 'Camuflaje Tinta', price: 200, rarity: 'Común',
    paint(ctx, W, H, r) {
      ctx.fillStyle = '#6b705c'; ctx.fillRect(0, 0, W, H);
      const cols = ['#a5a58d', '#3f4238', '#b7b7a4', '#283618'];
      for (let i = 0; i < 40; i++) drawSplat(ctx, r() * W, r() * H, 10 + r() * 16, cols[i % cols.length], r);
    },
  },
};

// Premio del calendario de 28 días (spec E)
SHIRT_DESIGNS['playera-del-mes'] = {
  name: 'Playera del Mes', price: 0, rarity: 'Calendario de 28 días', calendarOnly: true,
  paint(ctx, W, H, r) {
    ctx.fillStyle = '#121016'; ctx.fillRect(0, 0, W, H);
    const cols = ['#e69f00', '#56b4e9', '#f0e442', '#cc79a7', '#009e73'];
    for (let i = 0; i < 28; i++) {
      const x = 22 + (i % 7) * 32, y = 60 + Math.floor(i / 7) * 34;
      ctx.fillStyle = cols[i % cols.length];
      ctx.beginPath(); ctx.arc(x, y, 9 + r() * 4, 0, Math.PI * 2); ctx.fill();
    }
    ctx.fillStyle = '#f4f1ea';
    ctx.font = `26px ${BLOCK_FONT}`; ctx.textAlign = 'center';
    ctx.fillText('28 DÍAS', W / 2, 220);
  },
};

const _shirtCache = new Map();
export function shirtTexture(id) {
  if (_shirtCache.has(id)) return _shirtCache.get(id);
  const def = SHIRT_DESIGNS[id] || SHIRT_DESIGNS['basica-blanca'];
  const c = makeCanvas(256, 256);
  const ctx = c.getContext('2d');
  def.paint(ctx, 256, 256, rng(id.length * 7919 + id.charCodeAt(0)));
  const t = toCanvasTexture(c);
  _shirtCache.set(id, t);
  return t;
}

// Miniatura para la UI (data URL del mismo diseño)
export function shirtThumb(id) {
  const def = SHIRT_DESIGNS[id];
  const c = makeCanvas(128, 128);
  const ctx = c.getContext('2d');
  ctx.save();
  ctx.scale(0.5, 0.5);
  def.paint(ctx, 256, 256, rng(id.length * 7919 + id.charCodeAt(0)));
  ctx.restore();
  return c.toDataURL();
}

// ── Escenarios ───────────────────────────────────────────────
export function graffitiWallTexture(seed, base = '#5b4a44') {
  const W = 1024, H = 128;
  const c = makeCanvas(W, H);
  const ctx = c.getContext('2d');
  const r = rng(seed);
  // ladrillos
  ctx.fillStyle = base; ctx.fillRect(0, 0, W, H);
  for (let y = 0; y < H; y += 16) {
    for (let x = ((y / 16) % 2) * 16; x < W; x += 32) {
      const shade = 0.85 + r() * 0.3;
      ctx.fillStyle = `rgba(${Math.floor(120 * shade)},${Math.floor(80 * shade)},${Math.floor(68 * shade)},1)`;
      ctx.fillRect(x + 1, y + 1, 30, 14);
    }
  }
  // tags y piezas
  const cols = ['#ff2bd6', '#39ff14', '#00e5ff', '#ffe600', '#ff6a2b', '#ffffff', '#8e6cff'];
  for (let i = 0; i < 9; i++) {
    const x = (i / 9) * W + r() * 40;
    const col = cols[Math.floor(r() * cols.length)];
    ctx.save();
    ctx.translate(x, 70 + r() * 30);
    ctx.rotate(-0.15 + r() * 0.3);
    ctx.font = `${36 + r() * 30}px ${GRAFFITI_FONT}`;
    ctx.lineWidth = 6; ctx.strokeStyle = '#111';
    const word = TAG_WORDS[Math.floor(r() * TAG_WORDS.length)];
    ctx.strokeText(word, 0, 0);
    ctx.fillStyle = col;
    ctx.fillText(word, 0, 0);
    ctx.restore();
    if (r() < 0.6) drawDrips(ctx, x, 82, 60, col, r, 28);
  }
  for (let i = 0; i < 10; i++) drawSplat(ctx, r() * W, r() * H, 6 + r() * 10, cols[Math.floor(r() * cols.length)], r, 0.8);
  return toCanvasTexture(c);
}

export function museumWallTexture(seed) {
  const W = 1024, H = 128;
  const c = makeCanvas(W, H);
  const ctx = c.getContext('2d');
  const r = rng(seed);
  ctx.fillStyle = '#ece6dc'; ctx.fillRect(0, 0, W, H);
  ctx.fillStyle = '#d8cfc0'; ctx.fillRect(0, H - 18, W, 18);
  // cuadros abstractos (arte generativo)
  for (let i = 0; i < 6; i++) {
    const fw = 70 + r() * 50, fh = 50 + r() * 30;
    const x = 40 + i * (W / 6) + r() * 30, y = 20 + r() * 10;
    ctx.fillStyle = '#b08d57'; ctx.fillRect(x - 6, y - 6, fw + 12, fh + 12);
    ctx.fillStyle = ['#f4f1ea', '#1d3557', '#e63946', '#ffb703'][i % 4];
    ctx.fillRect(x, y, fw, fh);
    for (let k = 0; k < 5; k++) {
      ctx.fillStyle = ['#e63946', '#1d3557', '#ffb703', '#2a9d8f', '#111'][Math.floor(r() * 5)];
      if (r() < 0.5) ctx.fillRect(x + r() * fw * 0.7, y + r() * fh * 0.7, 6 + r() * fw * 0.4, 6 + r() * fh * 0.4);
      else { ctx.beginPath(); ctx.arc(x + r() * fw, y + r() * fh, 4 + r() * 14, 0, Math.PI * 2); ctx.fill(); }
    }
  }
  return toCanvasTexture(c);
}

export function rooftopWallTexture(seed) {
  const W = 1024, H = 128;
  const c = makeCanvas(W, H);
  const ctx = c.getContext('2d');
  const r = rng(seed);
  ctx.fillStyle = '#d9a46c'; ctx.fillRect(0, 0, W, H);
  for (let i = 0; i < 300; i++) {
    ctx.fillStyle = `rgba(0,0,0,${0.03 + r() * 0.05})`;
    ctx.fillRect(r() * W, r() * H, 2 + r() * 8, 2 + r() * 8);
  }
  // rótulo pintado a mano
  ctx.fillStyle = '#c1121f';
  ctx.font = `46px ${BLOCK_FONT}`;
  ctx.fillText('TORTILLERÍA', 60, 80);
  ctx.fillStyle = '#1d3557';
  ctx.fillText('SE RENTA', 560, 84);
  ctx.font = `30px ${GRAFFITI_FONT}`;
  ctx.fillStyle = '#2a9d8f';
  ctx.fillText('azotea crew', 860, 70);
  return toCanvasTexture(c);
}

// Etiqueta flotante de power-up (sprite)
export function labelTexture(text, color) {
  const c = makeCanvas(256, 64);
  const ctx = c.getContext('2d');
  ctx.font = `bold 30px ${BLOCK_FONT}`;
  ctx.textAlign = 'center';
  ctx.lineWidth = 8; ctx.strokeStyle = '#111';
  ctx.strokeText(text, 128, 44);
  ctx.fillStyle = color; ctx.fillText(text, 128, 44);
  return toCanvasTexture(c);
}

// ── Gorras (diseños en la copa) ──────────────────────────────
// Canvas 256×128 envuelto en la copa: x=64 es el frente (donde está la visera),
// x=192 la nuca; y=0 es la punta y y=128 el borde. La silueta nunca cambia.
const INK = '#121016', BONE = '#f4f1ea';
function capFront(ctx, fn) { for (const cx of [64, 192]) { ctx.save(); ctx.translate(cx, 0); fn(ctx); ctx.restore(); } }

export const CAP_DESIGNS = {
  clasica: { name: 'Clásica', rarity: 'Común · de tu color', price: 0, playerColor: true },
  'inkcap-rotulo': {
    name: 'INKCAP Rótulo', rarity: 'Común', price: 0, visor: 0x121016,
    paint(ctx, W, H) {
      ctx.fillStyle = BONE; ctx.fillRect(0, 0, W, H);
      capFront(ctx, (c) => {
        c.fillStyle = INK; c.font = `22px ${BLOCK_FONT}`; c.textAlign = 'center';
        c.fillText('INKCAP', 0, 96);
      });
    },
  },
  cuadros: {
    name: 'Cuadros de Carrera', rarity: 'Común', price: 80, visor: 0x121016,
    paint(ctx, W, H) {
      for (let y = 0; y < H; y += 16) for (let x = 0; x < W; x += 16) {
        ctx.fillStyle = ((x + y) / 16) % 2 === 0 ? INK : BONE; ctx.fillRect(x, y, 16, 16);
      }
    },
  },
  'gotas-tinta': {
    name: 'Gotas de Tinta', rarity: 'Rara', price: 150, visor: 0x0072b2,
    paint(ctx, W, H, r) {
      ctx.fillStyle = '#56b4e9'; ctx.fillRect(0, 0, W, H);
      drawDrips(ctx, 0, 0, W, INK, r, 50);
      for (let i = 0; i < 8; i++) drawSplat(ctx, r() * W, 60 + r() * 50, 6 + r() * 6, INK, r);
    },
  },
  flamas: {
    name: 'Flamas', rarity: 'Rara', price: 200, visor: 0x121016,
    paint(ctx, W, H) {
      ctx.fillStyle = INK; ctx.fillRect(0, 0, W, H);
      const cols = ['#d55e00', '#e69f00', '#f0e442'];
      cols.forEach((col, k) => {
        ctx.fillStyle = col;
        ctx.beginPath(); ctx.moveTo(0, H);
        for (let x = 0; x <= W; x += 16) ctx.lineTo(x, H - 30 - k * 14 - ((x / 16) % 2 ? 28 : 0) + k * 10);
        ctx.lineTo(W, H); ctx.closePath(); ctx.fill();
      });
    },
  },
  camuflaje: {
    name: 'Camuflaje', rarity: 'Común', price: 120, visor: 0x3f4238,
    paint(ctx, W, H, r) {
      ctx.fillStyle = '#6b705c'; ctx.fillRect(0, 0, W, H);
      const cols = ['#a5a58d', '#3f4238', '#b7b7a4', '#283618'];
      for (let i = 0; i < 26; i++) drawSplat(ctx, r() * W, r() * H, 8 + r() * 10, cols[i % cols.length], r);
    },
  },
  estrellas: {
    name: 'Estrellas', rarity: 'Épica', price: 250, visor: 0xcc79a7,
    paint(ctx, W, H, r) {
      ctx.fillStyle = '#2d1f3d'; ctx.fillRect(0, 0, W, H);
      for (let i = 0; i < 16; i++) {
        ctx.fillStyle = i % 3 ? '#f0e442' : BONE;
        drawStar(ctx, r() * W, 20 + r() * 100, 6 + r() * 7); ctx.fill();
      }
    },
  },
};

// Gorra de la semana (día 7 del premio diario): diseño de gorra INKCAP
// con texto en tipografía de rótulo (no se inventa logo). Rota cada semana.
const WEEKLY = [
  { id: 'semana-tinta', name: 'INKCAP Tinta', visor: 0xf4f1ea, bg: INK, fg: BONE, accent: '#56b4e9' },
  { id: 'semana-naranja', name: 'INKCAP Naranja', visor: 0x121016, bg: '#e69f00', fg: INK, accent: BONE },
  { id: 'semana-verde', name: 'INKCAP Verde', visor: 0x121016, bg: '#009e73', fg: BONE, accent: INK },
  { id: 'semana-rosa', name: 'INKCAP Rosa', visor: 0x121016, bg: '#cc79a7', fg: INK, accent: BONE },
];
for (const w of WEEKLY) {
  CAP_DESIGNS[w.id] = {
    name: w.name, rarity: 'Gorra de la semana · día 7', price: 0, dailyOnly: true, visor: w.visor,
    paint(ctx, W, H) {
      ctx.fillStyle = w.bg; ctx.fillRect(0, 0, W, H);
      ctx.fillStyle = w.accent; ctx.fillRect(0, 108, W, 8);
      capFront(ctx, (c) => {
        c.textAlign = 'center';
        c.fillStyle = w.fg; c.font = `24px ${BLOCK_FONT}`; c.fillText('INKCAP', 0, 94);
        c.font = `13px ${GRAFFITI_FONT}`; c.fillStyle = w.accent; c.fillText('gorra de la semana', 0, 60);
      });
    },
  };
}
export function weeklyCapId(date = new Date()) {
  // semana del año (lunes a domingo) → diseño
  const d = new Date(date.getFullYear(), date.getMonth(), date.getDate());
  const jan1 = new Date(d.getFullYear(), 0, 1);
  const week = Math.floor(((d - jan1) / 86400000 + ((jan1.getDay() + 6) % 7)) / 7);
  return WEEKLY[week % WEEKLY.length].id;
}

const _capCache = new Map();
function paintCap(id, w, h) {
  const def = CAP_DESIGNS[id];
  const c = makeCanvas(w, h);
  const ctx = c.getContext('2d');
  ctx.save(); ctx.scale(w / 256, h / 128);
  def.paint(ctx, 256, 128, rng(id.length * 104729 + id.charCodeAt(0)));
  ctx.restore();
  return c;
}
export function capTexture(id) {
  if (_capCache.has(id)) return _capCache.get(id);
  const t = toCanvasTexture(paintCap(id, 256, 128));
  _capCache.set(id, t);
  return t;
}

// Miniatura de la gorra (silueta fija; el diseño solo en la copa)
export function capThumb(id, playerCss = '#e69f00') {
  const def = CAP_DESIGNS[id];
  const c = makeCanvas(128, 128);
  const ctx = c.getContext('2d');
  ctx.fillStyle = '#2a2333'; ctx.fillRect(0, 0, 128, 128);
  // copa
  ctx.save();
  ctx.beginPath(); ctx.ellipse(64, 78, 44, 40, 0, Math.PI, 0); ctx.closePath(); ctx.clip();
  if (def.playerColor) { ctx.fillStyle = playerCss; ctx.fillRect(0, 0, 128, 128); }
  else ctx.drawImage(paintCap(id, 256, 128), 0, 0, 128, 128, 20, 38, 88, 40); // mitad del frente
  ctx.restore();
  ctx.lineWidth = 4; ctx.strokeStyle = '#111';
  ctx.beginPath(); ctx.ellipse(64, 78, 44, 40, 0, Math.PI, 0); ctx.closePath(); ctx.stroke();
  // visera
  const vis = def.playerColor ? playerCss : '#' + (def.visor ?? 0x222222).toString(16).padStart(6, '0');
  ctx.fillStyle = vis;
  ctx.beginPath(); ctx.ellipse(64, 82, 52, 12, 0, 0, Math.PI); ctx.closePath(); ctx.fill(); ctx.stroke();
  ctx.fillStyle = '#fff'; ctx.beginPath(); ctx.arc(64, 38, 5, 0, Math.PI * 2); ctx.fill(); ctx.stroke();
  return c.toDataURL();
}

// Esténcil del Muro de Tag (Grafiti): letras caladas sobre el color del jugador
const _stencilCache = new Map();
export function stencilTexture(css) {
  if (_stencilCache.has(css)) return _stencilCache.get(css);
  const c = makeCanvas(256, 128);
  const ctx = c.getContext('2d');
  ctx.fillStyle = css; ctx.fillRect(0, 0, 256, 128);
  ctx.fillStyle = 'rgba(18,16,22,0.85)';
  ctx.font = `64px ${BLOCK_FONT}`; ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
  ctx.fillText('TAG', 128, 66);
  ctx.fillStyle = 'rgba(244,241,234,0.9)';
  for (let x = 6; x < 256; x += 22) ctx.fillRect(x, 0, 8, 10);
  for (let x = 6; x < 256; x += 22) ctx.fillRect(x, 118, 8, 10);
  const t = toCanvasTexture(c);
  _stencilCache.set(css, t);
  return t;
}

// Franjas de precaución de los muros de muerte súbita
let _hazardTex = null;
export function hazardStripeTexture() {
  if (_hazardTex) return _hazardTex;
  const c = makeCanvas(128, 128);
  const ctx = c.getContext('2d');
  ctx.fillStyle = '#121016'; ctx.fillRect(0, 0, 128, 128);
  ctx.fillStyle = '#f0e442';
  for (let i = -128; i < 256; i += 32) {
    ctx.beginPath(); ctx.moveTo(i, 0); ctx.lineTo(i + 16, 0); ctx.lineTo(i + 16 + 128, 128); ctx.lineTo(i + 128, 128); ctx.fill();
  }
  _hazardTex = toCanvasTexture(c, { repeat: [1, 1] });
  return _hazardTex;
}


// ── Productos reales de INKCAP (www.inkcap.com.mx) ───────────
// Gorras snapback de la tienda ($499 MXN) con el logo bordado al frente
// y el parche redondo en la visera; playera Basic negra; y diseños de
// artistas recortados de las fotos de producto (provisionales hasta tener
// el archivo original y la licencia digital del artista).
export const TIENDA_URL = 'https://www.inkcap.com.mx/tienda';
const REAL_CAPS = [
  ['inkcap-negra', 'INKCAP Negra', '#161518', '#f2f2f2', 0],
  ['inkcap-roja', 'INKCAP Roja', '#c8102e', '#f7f7f7', 300],
  ['inkcap-azul-rey', 'INKCAP Azul Rey', '#1f3fa3', '#f2f2f2', 300],
  ['inkcap-azul-marino', 'INKCAP Azul Marino', '#1b1f3b', '#ededed', 300],
  ['inkcap-gris', 'INKCAP Gris', '#4b4a50', '#d9d9dc', 300],
  ['inkcap-jaspe', 'INKCAP Jaspe', '#9d9c9f', '#3a393d', 300],
];

// La u=0.5 del cuerpo es la ESPALDA y el frente queda en la costura (u=0/1):
// para el frente se dibuja partido en las dos orillas del canvas.
function drawFront(ctx, img, W, cy, w, h) {
  for (const cx of [0, W]) ctx.drawImage(img, cx - w / 2, cy - h / 2, w, h);
}

export function registerBrandDesigns() {
  for (const [id, name, crown, ink, price] of REAL_CAPS) {
    CAP_DESIGNS[id] = {
      name, price, rarity: price ? 'Gorra real INKCAP · $499 en tienda' : 'Gorra real INKCAP · de inicio',
      visor: parseInt(crown.slice(1), 16), button: parseInt(crown.slice(1), 16), patch: true, url: TIENDA_URL,
      crown: parseInt(crown.slice(1), 16), ink,
      paint(ctx, W, H) {
        ctx.fillStyle = crown; ctx.fillRect(0, 0, W, H);
        const mark = tintedLogo(ink, 92);
        const shadow = tintedLogo('rgba(0,0,0,0.35)', 92);
        capFront(ctx, (c) => {
          if (mark) {
            const y = 78 - mark.height / 2;
            c.drawImage(shadow, -mark.width / 2 + 2, y + 2); // relieve del bordado 3D
            c.drawImage(mark, -mark.width / 2, y);
          } else {
            c.fillStyle = ink; c.font = `22px ${BLOCK_FONT}`; c.textAlign = 'center'; c.fillText('INKCAP', 0, 90);
          }
        });
      },
    };
  }
  SHIRT_DESIGNS['inkcap-basic-negra'] = {
    name: 'INKCAP Basic Negra', price: 0, rarity: 'Playera real INKCAP · de inicio', url: TIENDA_URL,
    fabric: '#141414',
    print(ctx, W, H) {
      const mark = tintedLogo('#f2f2f2', Math.round(W * 0.86));
      if (mark) ctx.drawImage(mark, W / 2 - mark.width / 2, H * 0.42 - mark.height / 2);
    },
    paint(ctx, W, H) {
      ctx.fillStyle = '#141414'; ctx.fillRect(0, 0, W, H);
      const mark = tintedLogo('#f2f2f2', 120);
      if (mark) {
        ctx.drawImage(mark, W / 2 - mark.width / 2, H * 0.5 - mark.height / 2); // espalda: logo grande
        drawFront(ctx, mark, W, H * 0.42, mark.width * 0.45, mark.height * 0.45); // pecho: logo chico
      } else { ctx.fillStyle = '#f2f2f2'; ctx.font = `30px ${BLOCK_FONT}`; ctx.textAlign = 'center'; ctx.fillText('INKCAP', W / 2, H * 0.55); }
    },
  };
  for (const a of MARCA.artistas || []) {
    if (!a.imgEl) continue;
    SHIRT_DESIGNS[`artista-${a.id}`] = {
      name: a.name, price: 450, rarity: `Artista · ${a.artist} · $${a.price} en tienda`, url: a.tienda || TIENDA_URL, artist: a.artist,
      fabric: a.fabric,
      print(ctx, W, H) {
        // el recorte de la foto se funde con la tela con un borde suave (como tinta de serigrafía)
        ctx.drawImage(a.imgEl, 0, 0, W, H);
        ctx.globalCompositeOperation = 'destination-in';
        const g = ctx.createRadialGradient(W / 2, H / 2, W * 0.4, W / 2, H / 2, W * 0.52);
        g.addColorStop(0, 'rgba(0,0,0,1)'); g.addColorStop(1, 'rgba(0,0,0,0)');
        ctx.fillStyle = g; ctx.fillRect(0, 0, W, H);
        ctx.globalCompositeOperation = 'source-over';
      },
      paint(ctx, W, H) {
        ctx.fillStyle = a.fabric; ctx.fillRect(0, 0, W, H);
        const side = W * 0.62;
        ctx.drawImage(a.imgEl, W / 2 - side / 2, H * 0.5 - side / 2, side, side); // espalda: diseño completo
        drawFront(ctx, a.imgEl, W, H * 0.48, side * 0.7, side * 0.7);         // frente: el mismo diseño, más chico
      },
    };
  }
  // los productos reales de INKCAP van primero en el vestidor
  const brandFirst = (obj, isBrand) => {
    for (const k of Object.keys(obj)) if (!isBrand(obj[k])) { const v = obj[k]; delete obj[k]; obj[k] = v; }
  };
  brandFirst(CAP_DESIGNS, (d) => !!d.url);
  brandFirst(SHIRT_DESIGNS, (d) => !!d.url);
  _shirtCache.clear();
  _capCache.clear();
}


// ── Cómo se ve la ropa EN el muñequito ───────────────────────
// Playeras con estampado (productos reales): tela lisa + estampado como
// calcomanía al frente y atrás (no se estira). Las de patrón (cuadros,
// camuflaje…) siguen envolviendo el cuerpo.
const SOLID_FABRIC = { 'basica-blanca': '#f2f2ee', 'basica-negra': '#1c1c1f' };
const _printCache = new Map();
export function shirtLook(id) {
  const def = SHIRT_DESIGNS[id] || SHIRT_DESIGNS['basica-blanca'];
  const fabric = def.fabric || SOLID_FABRIC[id] || null;
  if (def.print) {
    if (!_printCache.has(id)) {
      const c = makeCanvas(256, 256);
      def.print(c.getContext('2d'), 256, 256);
      _printCache.set(id, toCanvasTexture(c));
    }
    return { fabric, wrap: null, print: _printCache.get(id) };
  }
  if (fabric) return { fabric, wrap: null, print: null };
  return { fabric: null, wrap: shirtTexture(id), print: null };
}

// Logo de gorra (calcomanía del panel de enfrente, fondo transparente)
const _capLogoCache = new Map();
export function capLogoTexture(id) {
  const def = CAP_DESIGNS[id];
  if (!def || !def.ink) return null;
  if (_capLogoCache.has(id)) return _capLogoCache.get(id);
  const c = makeCanvas(256, 128);
  const ctx = c.getContext('2d');
  const mark = tintedLogo(def.ink, 224);
  const shadow = tintedLogo('rgba(0,0,0,0.45)', 224);
  if (mark) {
    const y = 64 - mark.height / 2;
    ctx.drawImage(shadow, 16 + 4, y + 4); // relieve del bordado 3D
    ctx.drawImage(mark, 16, y);
  } else {
    ctx.fillStyle = def.ink; ctx.font = `54px ${BLOCK_FONT}`; ctx.textAlign = 'center'; ctx.fillText('INKCAP', 128, 84);
  }
  const t = toCanvasTexture(c);
  _capLogoCache.set(id, t);
  return t;
}


// ─────────────────────────────────────────────────────────────
// v0.3 · ESCENARIOS: texturas compartidas de vacíos, marca y utilería.
// Todas se crean una sola vez (caché de módulo) para gastar poca memoria.
// ─────────────────────────────────────────────────────────────
export const FONTS = {
  block: BLOCK_FONT,
  tag: GRAFFITI_FONT,
  // tipografía de rótulo de la tienda INKCAP (la misma de los botones del menú)
  brand: '"Barlow Condensed", "Arial Narrow", Impact, system-ui, sans-serif',
};
export { makeCanvas, drawDrips, drawStar, TAG_WORDS };

const LOGO_ASPECT = 364 / 1200;

// Dibuja el logo INKCAP centrado en (cx, cy) con el ancho dado. Si no hay
// archivo de logo, escribe "INKCAP" en tipografía de rótulo (no se inventa uno).
export function drawLogo(ctx, cx, cy, width, color, { rot = 0, alpha = 1 } = {}) {
  ctx.save();
  ctx.translate(cx, cy);
  if (rot) ctx.rotate(rot);
  ctx.globalAlpha *= alpha;
  const mark = tintedLogo(color, Math.max(8, Math.round(Math.min(width, 1200))));
  if (mark) {
    const h = width * LOGO_ASPECT;
    ctx.drawImage(mark, -width / 2, -h / 2, width, h);
  } else {
    ctx.fillStyle = color;
    ctx.font = `${Math.round(width * 0.25)}px ${BLOCK_FONT}`;
    ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
    ctx.fillText('INKCAP', 0, 0);
  }
  ctx.restore();
  return width * LOGO_ASPECT;
}
export const logoHeight = (width) => width * LOGO_ASPECT;

// Pared interior de un vacío: clara en la orilla y negra hacia el fondo.
// Solo tiene rasgos horizontales, así que se puede estirar a cualquier largo.
const _pitCache = new Map();
export function pitTexture(style = 'concreto') {
  if (_pitCache.has(style)) return _pitCache.get(style);
  const W = 16, H = 256;
  const c = makeCanvas(W, H);
  const ctx = c.getContext('2d');
  const top = { concreto: '#6f6a78', neon: '#2a2440', edificio: '#8a5a48', tienda: '#5a5850' }[style] || '#6f6a78';
  const g = ctx.createLinearGradient(0, 0, 0, H);
  // la orilla se ve apenas iluminada y enseguida se pierde en lo negro: se lee como hoyo profundo
  g.addColorStop(0, top);
  g.addColorStop(0.035, top);
  g.addColorStop(0.2, '#1d1924');
  g.addColorStop(0.42, '#08070b');
  g.addColorStop(1, '#000000');
  ctx.fillStyle = g; ctx.fillRect(0, 0, W, H);
  // labio de la orilla + hiladas (ladrillo/colado) que dan escala a la profundidad
  ctx.fillStyle = 'rgba(255,255,255,0.22)'; ctx.fillRect(0, 0, W, 4);
  ctx.fillStyle = 'rgba(0,0,0,0.35)';
  for (let y = 12; y < H * 0.5; y += style === 'edificio' ? 8 : 16) ctx.fillRect(0, y, W, 2);
  if (style === 'neon') {
    // líneas de luz de la instalación: se apagan hacia abajo
    const cols = ['#00e5ff', '#ff2bd6', '#00e5ff', '#ff2bd6', '#00e5ff'];
    cols.forEach((col, i) => {
      ctx.globalAlpha = 0.95 - i * 0.17;
      ctx.fillStyle = col; ctx.fillRect(0, 26 + i * 34, W, 5);
      ctx.globalAlpha = (0.95 - i * 0.17) * 0.3;
      ctx.fillRect(0, 20 + i * 34, W, 17);
    });
    ctx.globalAlpha = 1;
  }
  const t = toCanvasTexture(c);
  _pitCache.set(style, t);
  return t;
}

// Franjas de la orilla de los vacíos (amarillo/negro), repetibles a lo largo.
let _rimTex = null;
export function rimStripeTexture() {
  if (_rimTex) return _rimTex;
  const c = makeCanvas(64, 32);
  const ctx = c.getContext('2d');
  ctx.fillStyle = '#16141a'; ctx.fillRect(0, 0, 64, 32);
  ctx.fillStyle = '#ffd400';
  for (let i = -32; i < 96; i += 32) {
    ctx.beginPath(); ctx.moveTo(i, 32); ctx.lineTo(i + 16, 32); ctx.lineTo(i + 48, 0); ctx.lineTo(i + 32, 0); ctx.fill();
  }
  _rimTex = toCanvasTexture(c, { repeat: [1, 1] });
  return _rimTex;
}

// Degradado suave (blanco → transparente) para halos de neón, lámparas y avisos.
let _glowTex = null, _softTex = null;
export function glowStripTexture() {
  if (_glowTex) return _glowTex;
  const c = makeCanvas(8, 64);
  const ctx = c.getContext('2d');
  const g = ctx.createLinearGradient(0, 64, 0, 0); // v=0 (orilla interior) brillante → v=1 apagado
  g.addColorStop(0, 'rgba(255,255,255,0.85)');
  g.addColorStop(0.35, 'rgba(255,255,255,0.3)');
  g.addColorStop(1, 'rgba(255,255,255,0)');
  ctx.fillStyle = g; ctx.fillRect(0, 0, 8, 64);
  _glowTex = toCanvasTexture(c, { repeat: [1, 1] });
  return _glowTex;
}
export function softDotTexture() {
  if (_softTex) return _softTex;
  const c = makeCanvas(64, 64);
  const ctx = c.getContext('2d');
  const g = ctx.createRadialGradient(32, 32, 1, 32, 32, 31);
  g.addColorStop(0, 'rgba(255,255,255,1)');
  g.addColorStop(0.35, 'rgba(255,255,255,0.45)');
  g.addColorStop(1, 'rgba(255,255,255,0)');
  ctx.fillStyle = g; ctx.fillRect(0, 0, 64, 64);
  _softTex = toCanvasTexture(c);
  return _softTex;
}

// Diseño de un artista como textura directa (cuadros del museo, paste-ups, cartel).
export function artistTexture(a) {
  if (!a || !a.imgEl) return null;
  if (!a._tex) {
    a._tex = new THREE.Texture(a.imgEl);
    a._tex.colorSpace = THREE.SRGBColorSpace;
    a._tex.anisotropy = 4;
    a._tex.needsUpdate = true;
  }
  return a._tex;
}

// Playera colgada (tendedero de la azotea, racks de la tienda): silueta de
// playera con la tela y el estampado reales del vestidor. Una textura por diseño.
const _hangCache = new Map();
function shirtPath(ctx) {
  ctx.beginPath();
  ctx.moveTo(90, 24);
  ctx.quadraticCurveTo(128, 54, 166, 24);
  ctx.lineTo(212, 42); ctx.lineTo(250, 98); ctx.lineTo(214, 122); ctx.lineTo(198, 100);
  ctx.lineTo(198, 244); ctx.lineTo(58, 244); ctx.lineTo(58, 100);
  ctx.lineTo(42, 122); ctx.lineTo(6, 98); ctx.lineTo(44, 42);
  ctx.closePath();
}
export function hangingShirtTexture(id) {
  if (_hangCache.has(id)) return _hangCache.get(id);
  const def = SHIRT_DESIGNS[id] || SHIRT_DESIGNS['basica-blanca'];
  const c = makeCanvas(256, 256);
  const ctx = c.getContext('2d');
  const fabric = def.fabric || SOLID_FABRIC[id] || null;
  ctx.save();
  shirtPath(ctx); ctx.clip();
  if (fabric) { ctx.fillStyle = fabric; ctx.fillRect(0, 0, 256, 256); }
  else def.paint(ctx, 256, 256, rng(id.length * 7919 + id.charCodeAt(0)));
  if (def.print) {
    const p = makeCanvas(256, 256);
    def.print(p.getContext('2d'), 256, 256);
    ctx.drawImage(p, 64, 62, 128, 128);
  }
  // pliegues y sombra de la tela
  const sh = ctx.createLinearGradient(0, 0, 256, 0);
  sh.addColorStop(0, 'rgba(0,0,0,0.28)'); sh.addColorStop(0.3, 'rgba(0,0,0,0)');
  sh.addColorStop(0.7, 'rgba(255,255,255,0.06)'); sh.addColorStop(1, 'rgba(0,0,0,0.3)');
  ctx.fillStyle = sh; ctx.fillRect(0, 0, 256, 256);
  ctx.restore();
  // contorno (look de caricatura) y cuello
  ctx.lineWidth = 6; ctx.lineJoin = 'round'; ctx.strokeStyle = '#111';
  shirtPath(ctx); ctx.stroke();
  ctx.lineWidth = 4; ctx.strokeStyle = 'rgba(255,255,255,0.35)';
  ctx.beginPath(); ctx.moveTo(94, 30); ctx.quadraticCurveTo(128, 60, 162, 30); ctx.stroke();
  // gancho
  ctx.strokeStyle = '#d8d8d8'; ctx.lineWidth = 4;
  ctx.beginPath(); ctx.moveTo(128, 30); ctx.lineTo(128, 10); ctx.arc(134, 10, 6, Math.PI, 0); ctx.stroke();
  const t = toCanvasTexture(c);
  _hangCache.set(id, t);
  return t;
}
// Playeras reales de INKCAP para decorar (Basic + artistas); si no hay marca, las de ejemplo.
export function brandShirtIds() {
  const ids = Object.keys(SHIRT_DESIGNS).filter((k) => SHIRT_DESIGNS[k].url);
  return ids.length ? ids : ['basica-negra', 'tinta-derramada', 'serigrafia-retro', 'neon-grafiti'];
}
export function brandCapIds() {
  const ids = Object.keys(CAP_DESIGNS).filter((k) => CAP_DESIGNS[k].url);
  return ids.length ? ids : ['inkcap-rotulo', 'cuadros', 'flamas', 'gotas-tinta', 'camuflaje', 'estrellas'];
}

// Etiqueta de las latas de técnica (spec v0.3 §C): negro tinta, banda blanca
// con el ícono de la técnica y el logo INKCAP. Envuelve el cilindro de la lata.
const _canCache = new Map();
export function canLabelTexture(glyph) {
  if (_canCache.has(glyph)) return _canCache.get(glyph);
  const c = makeCanvas(256, 128);
  const ctx = c.getContext('2d');
  ctx.fillStyle = INK; ctx.fillRect(0, 0, 256, 128);
  ctx.fillStyle = BONE; ctx.fillRect(0, 50, 256, 46);
  for (const cx of [64, 192]) {
    drawLogo(ctx, cx, 26, 104, BONE);
    ctx.fillStyle = INK; ctx.font = `bold 40px ${BLOCK_FONT}`; ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
    ctx.fillText(glyph, cx, 75);
  }
  ctx.fillStyle = BONE;
  for (let x = 6; x < 256; x += 20) ctx.fillRect(x, 108, 10, 5);
  const t = toCanvasTexture(c);
  _canCache.set(glyph, t);
  return t;
}

// Taza INKCAP (trofeo del ganador y mostrador de la tienda): cerámica blanca con el logo.
let _mugTex = null;
export function mugTexture() {
  if (_mugTex) return _mugTex;
  const c = makeCanvas(256, 128);
  const ctx = c.getContext('2d');
  ctx.fillStyle = '#f6f4ee'; ctx.fillRect(0, 0, 256, 128);
  drawLogo(ctx, 128, 62, 132, INK);
  ctx.fillStyle = INK; ctx.fillRect(0, 118, 256, 4);
  _mugTex = toCanvasTexture(c);
  return _mugTex;
}

// Calcomanía / sticker con el logo (contenedores, tinaco, caja): tinta sobre papel
// o solo la tinta sobre fondo transparente (paper = null).
const _stickerCache = new Map();
export function logoStickerTexture(ink = INK, paper = BONE) {
  const key = `${ink}|${paper}`;
  if (_stickerCache.has(key)) return _stickerCache.get(key);
  const c = makeCanvas(256, 96);
  const ctx = c.getContext('2d');
  if (paper) {
    ctx.fillStyle = paper;
    ctx.beginPath(); ctx.roundRect ? ctx.roundRect(2, 2, 252, 92, 12) : ctx.rect(2, 2, 252, 92); ctx.fill();
    ctx.lineWidth = 4; ctx.strokeStyle = '#111'; ctx.stroke();
  }
  drawLogo(ctx, 128, 48, paper ? 212 : 248, ink);
  const t = toCanvasTexture(c);
  _stickerCache.set(key, t);
  return t;
}

// Caja de madera (tarima/huacal) para el callejón.
let _crateTex = null;
export function crateTexture() {
  if (_crateTex) return _crateTex;
  const c = makeCanvas(128, 128);
  const ctx = c.getContext('2d');
  const r = rng(77);
  ctx.fillStyle = '#b98556'; ctx.fillRect(0, 0, 128, 128);
  for (let i = 0; i < 5; i++) {
    ctx.fillStyle = `rgba(${90 + r() * 40},${55 + r() * 25},${30 + r() * 15},0.35)`;
    ctx.fillRect(0, i * 26, 128, 24);
    ctx.fillStyle = 'rgba(40,22,10,0.55)'; ctx.fillRect(0, i * 26 + 24, 128, 2);
  }
  ctx.strokeStyle = '#7a4f2c'; ctx.lineWidth = 14; ctx.strokeRect(7, 7, 114, 114);
  ctx.beginPath(); ctx.moveTo(10, 10); ctx.lineTo(118, 118); ctx.stroke();
  ctx.strokeStyle = 'rgba(30,16,8,0.6)'; ctx.lineWidth = 2; ctx.strokeRect(1, 1, 126, 126); ctx.strokeRect(14, 14, 100, 100);
  ctx.fillStyle = '#3b2616';
  for (const [x, y] of [[8, 8], [120, 8], [8, 120], [120, 120]]) { ctx.beginPath(); ctx.arc(x, y, 2.5, 0, Math.PI * 2); ctx.fill(); }
  _crateTex = toCanvasTexture(c);
  return _crateTex;
}

// Caja DROP de la Pop-up Store (peligro que cae del techo): cartón negro,
// cinta neón de la tienda, la palabra DROP y el logo.
let _dropTex = null;
export function dropBoxTexture() {
  if (_dropTex) return _dropTex;
  const c = makeCanvas(256, 256);
  const ctx = c.getContext('2d');
  ctx.fillStyle = '#17161a'; ctx.fillRect(0, 0, 256, 256);
  ctx.fillStyle = '#ccff00'; ctx.fillRect(0, 108, 256, 40);
  ctx.fillStyle = '#17161a'; ctx.font = `italic 900 40px ${FONTS.brand}`; ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
  ctx.fillText('DROP · DROP · DROP', 128, 129);
  ctx.fillStyle = '#ccff00'; ctx.font = `92px ${BLOCK_FONT}`;
  ctx.fillText('DROP', 128, 60);
  drawLogo(ctx, 128, 200, 190, BONE);
  ctx.strokeStyle = '#ccff00'; ctx.lineWidth = 8; ctx.strokeRect(4, 4, 248, 248);
  _dropTex = toCanvasTexture(c);
  return _dropTex;
}

// Limpia cachés que dependen de las fuentes o de la marca (se llama al recargar la marca).
export function clearSceneTextureCaches() {
  for (const m of [_hangCache, _canCache, _stickerCache]) { for (const t of m.values()) t.dispose(); m.clear(); }
  for (const t of [_mugTex, _dropTex]) if (t) t.dispose();
  _mugTex = null; _dropTex = null;
}
