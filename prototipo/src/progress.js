// ─────────────────────────────────────────────────────────────
// PROGRESIÓN LOCAL (demo)
// XP y niveles, monedas, premio diario con racha, vestidor y canje
// de códigos. En el juego real esto vive en el servidor (para que
// nadie se regale monedas editando el teléfono); aquí usamos
// localStorage solo para demostrar el ciclo de recompensas.
// ─────────────────────────────────────────────────────────────
import { PROGRESSION } from './config.js';
import { SHIRT_DESIGNS } from './textures.js';

const KEY = 'arena-inkcap.progress.v1';

// Códigos de demostración (en producción se validan en el backend
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

export const SPECIES_UNLOCK = [1, 1, 1, 2, 4, 6]; // nivel requerido por muñequito

function today() {
  const d = new Date();
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}
function yesterday() {
  const d = new Date(Date.now() - 86400000);
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}

function fresh() {
  return {
    level: 1, xp: 0, coins: 100,
    owned: ['basica-blanca', 'basica-negra'],
    equipped: { shirt: 'basica-blanca', species: 0 },
    daily: { last: null, streak: 0 },
    stats: { matches: 0, wins: 0, kos: 0 },
    redeemed: [],
  };
}

let data = fresh();

export function loadProgress() {
  try {
    const raw = localStorage.getItem(KEY);
    if (raw) data = { ...fresh(), ...JSON.parse(raw) };
  } catch { data = fresh(); }
  return data;
}

function save() {
  try { localStorage.setItem(KEY, JSON.stringify(data)); } catch { /* sin almacenamiento: la demo sigue funcionando */ }
}

export function progress() { return data; }
export function xpNeeded() { return PROGRESSION.xpForLevel(data.level); }

function addXp(amount) {
  data.xp += amount;
  let levels = 0, coins = 0;
  while (data.xp >= PROGRESSION.xpForLevel(data.level)) {
    data.xp -= PROGRESSION.xpForLevel(data.level);
    data.level++;
    levels++;
    coins += PROGRESSION.coinsPerLevel;
  }
  data.coins += coins;
  return { levels, coins };
}

// Resultado de partida del jugador local → XP y monedas
export function recordMatch({ won, kos, roundWins }) {
  const R = PROGRESSION.rewards;
  const breakdown = [
    { label: 'Por jugar', xp: R.participate },
    { label: `Rondas ganadas ×${roundWins}`, xp: roundWins * R.roundWin },
    { label: `Noqueos ×${kos}`, xp: kos * R.ko },
  ];
  if (won) breakdown.push({ label: '¡Ganaste la partida!', xp: R.matchWin });
  const total = breakdown.reduce((a, b) => a + b.xp, 0);
  const matchCoins = 10 + (won ? 25 : 0) + kos * 2;
  const before = { level: data.level, xp: data.xp };
  data.coins += matchCoins;
  const lv = addXp(total);
  data.stats.matches++; if (won) data.stats.wins++; data.stats.kos += kos;
  save();
  return { breakdown, total, coins: matchCoins + lv.coins, levelsGained: lv.levels, before };
}

// ── Premio diario ────────────────────────────────────────────
export function dailyStatus() {
  const t = today();
  const claimedToday = data.daily.last === t;
  let streak = data.daily.streak;
  if (!claimedToday && data.daily.last !== yesterday()) streak = 0; // se rompió la racha
  const nextDay = claimedToday ? ((streak - 1) % 7) + 1 : (streak % 7) + 1;
  return { claimedToday, streak, nextDay };
}

export function claimDaily() {
  const st = dailyStatus();
  if (st.claimedToday) return null;
  const streak = st.streak + 1;
  const reward = PROGRESSION.daily[(streak - 1) % 7];
  data.daily = { last: today(), streak };
  let lv = { levels: 0, coins: 0 };
  if (reward.coins) data.coins += reward.coins;
  if (reward.xp) lv = addXp(reward.xp);
  if (reward.shirt && !data.owned.includes(reward.shirt)) data.owned.push(reward.shirt);
  save();
  return { reward, streak, levelsGained: lv.levels };
}

// ── Vestidor ─────────────────────────────────────────────────
export function buyShirt(id) {
  const def = SHIRT_DESIGNS[id];
  if (!def || data.owned.includes(id) || def.dailyOnly || def.codeOnly) return false;
  if (data.coins < def.price) return false;
  data.coins -= def.price;
  data.owned.push(id);
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
