// ─────────────────────────────────────────────────────────────
// PROGRESIÓN LOCAL (spec v0.2 §E): todo vive en el celular, sin backend.
// XP y niveles 1–50, Gotas (moneda blanda), primera victoria del día,
// premio diario de 7 días con racha suave, calendario de 28 días,
// vestidor (playeras + gorras), stickers/emotes, muñequitos por nivel,
// canje de códigos (simulación del portal web), reglas y Reto del Día.
// ─────────────────────────────────────────────────────────────
import { PROGRESSION, DEFAULT_RULES } from './config.js';
import { SHIRT_DESIGNS, CAP_DESIGNS, weeklyCapId } from './textures.js';
import { dayKey, daySeed } from './rng.js';

const KEY = 'arena-inkcap.progress.v1';
const RULES_KEY = 'gorrazo.rules.v1';
const RETO_KEY = 'gorrazo.retoDelDia.v1';

// Códigos de demostración (en producción se validan en el portal web
// y cada código impreso en una etiqueta es único y de un solo uso).
const DEMO_CODES = {
  'TINTA-2026': 'edicion-limitada',
};

// Diseño exclusivo que solo se obtiene con código (simula la playera física)
SHIRT_DESIGNS['edicion-limitada'] = {
  // Regla 3.1.3(b) de Apple: lo que se canjea con la prenda física también se vende en el juego.
  name: 'Edición Limitada 001', price: 600, rarity: 'Edición limitada · también con la playera física',
  paint(ctx, W, H) {
    ctx.fillStyle = '#0d0d0f'; ctx.fillRect(0, 0, W, H);
    const g = ctx.createLinearGradient(0, H * 0.3, W, H * 0.8);
    g.addColorStop(0, '#f6d365'); g.addColorStop(1, '#fda085');
    ctx.strokeStyle = g; ctx.lineWidth = 10;
    ctx.strokeRect(40, 70, W - 80, H - 120);
    ctx.fillStyle = g;
    ctx.font = 'bold 30px "Bungee", "Arial Black", sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText('ED.', W / 2, H * 0.5);
    ctx.fillText('LIMITADA', W / 2, H * 0.64);
    ctx.font = '18px "Bungee", "Arial Black", sans-serif';
    ctx.fillText('001/100', W / 2, H * 0.76);
  },
};

// Muñequitos desbloqueados por nivel (índices de SPECIES):
// Osote, Michi, Gotita nv2, Seri nv4, Zurci nv6, Pegote nv10, Lienzo, Axo nv15
export const SPECIES_UNLOCK = [1, 1, 2, 4, 6, 10, 1, 15];

// Cosméticos simples del premio diario
export const STICKERS = [
  { id: 'st-gotita', name: 'Gotita feliz', text: 'GOTITA' },
  { id: 'st-ko', name: 'KO de tinta', text: '¡KO!' },
  { id: 'st-crew', name: 'Crew', text: 'CREW' },
  { id: 'st-cdmx', name: 'Chilango', text: 'CDMX' },
];
export const EMOTES = [
  { id: 'em-orale', name: '¡Órale!', text: '¡ÓRALE!' },
  { id: 'em-quionda', name: '¿Qué onda?', text: '¿QUÉ ONDA?' },
  { id: 'em-ahi', name: 'Ahí nos vidrios', text: '¡AHÍ NOS VIDRIOS!' },
  { id: 'em-chido', name: '¡Qué chido!', text: '¡QUÉ CHIDO!' },
];

function today() { return dayKey(new Date()); }

function fresh() {
  return {
    level: 1, xp: 0, coins: 100, // coins = Gotas
    owned: ['inkcap-basic-negra', 'basica-blanca', 'basica-negra'],
    ownedCaps: ['inkcap-negra', 'clasica', 'inkcap-rotulo'],
    stickers: [], emotes: [],
    equipped: { shirt: 'inkcap-basic-negra', species: 0, cap: 'inkcap-negra', sticker: null, emote: null },
    daily: { last: null, claimed: 0 },
    calendar: { stamps: 0, months: 0 },
    firstWinDay: null,
    stats: { matches: 0, wins: 0, kos: 0 },
    redeemed: [],
  };
}

let data = fresh();

export function loadProgress() {
  try {
    const raw = localStorage.getItem(KEY);
    if (raw) {
      const saved = JSON.parse(raw);
      data = { ...fresh(), ...saved };
      data.equipped = { ...fresh().equipped, ...(saved.equipped || {}) };
      // migración desde v0.1 (racha estricta {last, streak})
      if (saved.daily && saved.daily.claimed == null) data.daily = { last: saved.daily.last || null, claimed: saved.daily.streak || 0 };
      if (!Array.isArray(data.ownedCaps)) data.ownedCaps = fresh().ownedCaps;
      if (!data.calendar) data.calendar = { stamps: 0, months: 0 };
      // v0.3: todos reciben la gorra y la playera reales de inicio
      if (!data.owned.includes('inkcap-basic-negra')) data.owned.unshift('inkcap-basic-negra');
      if (!data.ownedCaps.includes('inkcap-negra')) data.ownedCaps.unshift('inkcap-negra');
    }
  } catch { data = fresh(); }
  return data;
}

function save() {
  try { localStorage.setItem(KEY, JSON.stringify(data)); } catch { /* sin almacenamiento: la demo sigue funcionando */ }
}

export function progress() { return data; }
export function xpNeeded() { return PROGRESSION.xpForLevel(data.level); }

function addXp(amount) {
  let levels = 0;
  if (data.level >= PROGRESSION.maxLevel) return { levels };
  data.xp += amount;
  while (data.level < PROGRESSION.maxLevel && data.xp >= PROGRESSION.xpForLevel(data.level)) {
    data.xp -= PROGRESSION.xpForLevel(data.level);
    data.level++;
    levels++;
  }
  if (data.level >= PROGRESSION.maxLevel) data.xp = 0;
  return { levels };
}

// Gotas por partida: 10–25 según el desempeño
export function matchGotas({ won, kos, roundWins }) {
  const G = PROGRESSION.gotas;
  return Math.min(G.max, G.base + (won ? 8 : 0) + roundWins * 2 + kos);
}

// Resultado de partida del jugador local → XP y Gotas
export function recordMatch({ won, kos, roundWins }) {
  const R = PROGRESSION.rewards;
  const breakdown = [
    { label: 'Por jugar', xp: R.participate },
    { label: `Rondas ganadas ×${roundWins}`, xp: roundWins * R.roundWin },
    { label: `Noqueos ×${kos}`, xp: kos * R.ko },
  ];
  if (won) breakdown.push({ label: '¡Ganaste la partida!', xp: R.matchWin });
  let firstWin = false;
  if (won && data.firstWinDay !== today()) {
    firstWin = true;
    data.firstWinDay = today();
    breakdown.push({ label: 'Primera victoria del día', xp: R.firstWinOfDay });
  }
  const total = breakdown.reduce((a, b) => a + b.xp, 0);
  const gotas = matchGotas({ won, kos, roundWins });
  const before = { level: data.level, xp: data.xp };
  data.coins += gotas;
  const lv = addXp(total);
  data.stats.matches++; if (won) data.stats.wins++; data.stats.kos += kos;
  save();
  return { breakdown, total, coins: gotas, levelsGained: lv.levels, before, firstWin };
}

// ── Premio diario (racha suave) ──────────────────────────────
// Si faltas un día la racha se PAUSA: al volver sigues en el día que te tocaba.
export function dailyStatus() {
  const claimedToday = data.daily.last === today();
  const claimed = data.daily.claimed;
  const nextDay = claimedToday ? ((claimed - 1) % 7) + 1 : (claimed % 7) + 1;
  return { claimedToday, streak: claimed, nextDay, calendar: data.calendar.stamps };
}

export function claimDaily() {
  const st = dailyStatus();
  if (st.claimedToday) return null;
  const claimed = st.streak + 1;
  const reward = PROGRESSION.daily[(claimed - 1) % 7];
  const cycle = Math.floor((claimed - 1) / 7);
  data.daily = { last: today(), claimed };
  const got = [];
  if (reward.coins) { data.coins += reward.coins; got.push(`${reward.coins} Gotas`); }
  if (reward.sticker) {
    const s = STICKERS.find((x) => !data.stickers.includes(x.id)) || STICKERS[cycle % STICKERS.length];
    if (!data.stickers.includes(s.id)) data.stickers.push(s.id);
    if (!data.equipped.sticker) data.equipped.sticker = s.id;
    got.push(`Sticker "${s.name}"`);
  }
  if (reward.emote) {
    const e = EMOTES.find((x) => !data.emotes.includes(x.id)) || EMOTES[cycle % EMOTES.length];
    if (!data.emotes.includes(e.id)) data.emotes.push(e.id);
    if (!data.equipped.emote) data.equipped.emote = e.id;
    got.push(`Emote "${e.name}"`);
  }
  if (reward.weeklyCap) {
    const id = weeklyCapId();
    if (!data.ownedCaps.includes(id)) data.ownedCaps.push(id);
    else data.coins += 100; // si ya la tenías, te damos Gotas
    got.push(`Gorra de la semana "${CAP_DESIGNS[id].name}"`);
  }
  // calendario de 28 días → Playera del mes
  data.calendar.stamps++;
  let monthShirt = false;
  if (data.calendar.stamps >= PROGRESSION.calendarDays) {
    data.calendar.stamps = 0;
    data.calendar.months++;
    if (!data.owned.includes('playera-del-mes')) data.owned.push('playera-del-mes');
    else data.coins += 300;
    monthShirt = true;
    got.push('¡Playera del mes!');
  }
  save();
  return { reward, streak: claimed, got, monthShirt };
}

// ── Vestidor ─────────────────────────────────────────────────
export function buyShirt(id) {
  const def = SHIRT_DESIGNS[id];
  if (!def || data.owned.includes(id) || def.dailyOnly || def.codeOnly || def.calendarOnly) return false;
  if (data.coins < def.price) return false;
  data.coins -= def.price;
  data.owned.push(id);
  save();
  return true;
}

export function buyCap(id) {
  const def = CAP_DESIGNS[id];
  if (!def || data.ownedCaps.includes(id) || def.dailyOnly) return false;
  if (data.coins < def.price) return false;
  data.coins -= def.price;
  data.ownedCaps.push(id);
  save();
  return true;
}

export function equip(patch) {
  data.equipped = { ...data.equipped, ...patch };
  save();
}

export function redeemCode(raw) {
  const code = String(raw || '').trim().toUpperCase();
  const shirt = DEMO_CODES[code];
  if (!shirt) return { ok: false, msg: 'Ese código no existe. Revisa la etiqueta de tu playera.' };
  if (data.redeemed.includes(code)) return { ok: false, msg: 'Ese código ya se canjeó en este dispositivo.' };
  data.redeemed.push(code);
  if (!data.owned.includes(shirt)) data.owned.push(shirt);
  data.equipped.shirt = shirt;
  save();
  return { ok: true, shirt, msg: `Desbloqueaste "${SHIRT_DESIGNS[shirt].name}" y ya la traes puesta.` };
}

// ID de jugador que se escribe en el portal web de canje (GDD §11.5)
export function playerId() {
  if (!data.playerId) {
    const abc = 'ABCDEFGHJKLMNPQRSTUVWXYZ';
    const letter = () => abc[Math.floor(Math.random() * abc.length)];
    data.playerId = `GZ-${Math.floor(1000 + Math.random() * 9000)}-${letter()}${letter()}`;
    save();
  }
  return data.playerId;
}

export function resetProgress() {
  data = fresh();
  save();
}

// ── Reglas (interruptores) ───────────────────────────────────
export function loadRules() {
  try {
    const raw = localStorage.getItem(RULES_KEY);
    if (raw) return { ...DEFAULT_RULES, ...JSON.parse(raw) };
  } catch { /* sin almacenamiento */ }
  return { ...DEFAULT_RULES };
}

export function saveRules(rules) {
  try { localStorage.setItem(RULES_KEY, JSON.stringify(rules)); } catch { /* sin almacenamiento */ }
}

// ── Reto del Día: mejor puntaje del día y tabla local de 7 días ──
// Puntaje = KOs × 100 + rondas ganadas × 300 − segundos totales
export function retoScore({ kos, roundWins, seconds }) {
  return kos * 100 + roundWins * 300 - Math.round(seconds);
}

function loadReto() {
  try { return JSON.parse(localStorage.getItem(RETO_KEY) || '{}') || {}; } catch { return {}; }
}

export function retoTable() {
  const all = loadReto();
  const rows = [];
  for (let i = 0; i < 7; i++) {
    const d = new Date(); d.setDate(d.getDate() - i);
    const seed = String(daySeed(d));
    rows.push({ seed, label: i === 0 ? 'Hoy' : i === 1 ? 'Ayer' : `${d.getDate()}/${d.getMonth() + 1}`, best: all[seed] ?? null, today: i === 0 });
  }
  return rows;
}

export function recordReto(score) {
  const all = loadReto();
  const seed = String(daySeed());
  const prev = all[seed];
  const isBest = prev == null || score > prev;
  if (isBest) all[seed] = score;
  // solo se guardan los últimos 7 días
  const keep = new Set(retoTable().map((r) => r.seed));
  for (const k of Object.keys(all)) if (!keep.has(k)) delete all[k];
  try { localStorage.setItem(RETO_KEY, JSON.stringify(all)); } catch { /* sin almacenamiento */ }
  return { isBest, best: all[seed] ?? score, prev: prev ?? null };
}
