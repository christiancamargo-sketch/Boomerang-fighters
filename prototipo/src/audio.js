// ─────────────────────────────────────────────────────────────
// AUDIO SINTETIZADO (WebAudio). Sin archivos: todo se genera en vivo.
// En el juego real se reemplaza por SFX y música con licencia.
// ─────────────────────────────────────────────────────────────

let ctx = null;
let master = null;
let sfxBus = null;
let musicBus = null;
let noiseBuf = null;
let musicTimer = null;
let musicStep = 0;
let nextNoteTime = 0;
const state = { sfx: true, music: true };

export function initAudio() {
  if (ctx) { if (ctx.state === 'suspended') ctx.resume(); return; }
  try {
    ctx = new (window.AudioContext || window.webkitAudioContext)();
  } catch { ctx = null; return; }
  master = ctx.createGain(); master.gain.value = 0.8; master.connect(ctx.destination);
  sfxBus = ctx.createGain(); sfxBus.gain.value = 0.9; sfxBus.connect(master);
  musicBus = ctx.createGain(); musicBus.gain.value = 0.32; musicBus.connect(master);
  noiseBuf = ctx.createBuffer(1, ctx.sampleRate * 1, ctx.sampleRate);
  const d = noiseBuf.getChannelData(0);
  for (let i = 0; i < d.length; i++) d[i] = Math.random() * 2 - 1;
}

export function setSfx(on) { state.sfx = on; if (sfxBus) sfxBus.gain.value = on ? 0.9 : 0; }
export function setMusic(on) {
  state.music = on;
  if (on) startMusic(); else stopMusic();
}
export function audioState() { return { ...state }; }

function env(g, t, a, peak, d) {
  g.gain.setValueAtTime(0.0001, t);
  g.gain.exponentialRampToValueAtTime(peak, t + a);
  g.gain.exponentialRampToValueAtTime(0.0001, t + a + d);
}

function tone({ type = 'sine', f0 = 440, f1 = null, a = 0.005, d = 0.15, vol = 0.3, bus = sfxBus, when = 0 }) {
  if (!ctx || !bus) return;
  const t = ctx.currentTime + when;
  const o = ctx.createOscillator();
  const g = ctx.createGain();
  o.type = type;
  o.frequency.setValueAtTime(f0, t);
  if (f1) o.frequency.exponentialRampToValueAtTime(Math.max(20, f1), t + a + d);
  env(g, t, a, vol, d);
  o.connect(g).connect(bus);
  o.start(t); o.stop(t + a + d + 0.05);
}

function noise({ a = 0.005, d = 0.2, vol = 0.3, filter = 'bandpass', f0 = 1200, f1 = null, q = 1, bus = sfxBus, when = 0 }) {
  if (!ctx || !bus) return;
  const t = ctx.currentTime + when;
  const src = ctx.createBufferSource();
  src.buffer = noiseBuf;
  const fl = ctx.createBiquadFilter();
  fl.type = filter; fl.Q.value = q;
  fl.frequency.setValueAtTime(f0, t);
  if (f1) fl.frequency.exponentialRampToValueAtTime(f1, t + a + d);
  const g = ctx.createGain();
  env(g, t, a, vol, d);
  src.connect(fl).connect(g).connect(bus);
  src.start(t, Math.random() * 0.5); src.stop(t + a + d + 0.05);
}

// ── Efectos del juego ────────────────────────────────────────
export const sfx = {
  throw(power = 0.5) {
    noise({ f0: 600, f1: 2600 + power * 1500, d: 0.18 + power * 0.1, vol: 0.22, q: 2 });
  },
  catch() {
    tone({ type: 'triangle', f0: 520, f1: 880, d: 0.08, vol: 0.25 });
  },
  recall() {
    noise({ f0: 2400, f1: 700, d: 0.22, vol: 0.15, q: 3 });
  },
  dash() {
    noise({ f0: 900, f1: 300, d: 0.12, vol: 0.18, filter: 'lowpass' });
  },
  melee() {
    noise({ f0: 1800, f1: 600, d: 0.09, vol: 0.2, q: 4 });
  },
  clank() {
    tone({ type: 'square', f0: 1800, f1: 1200, d: 0.07, vol: 0.12 });
    tone({ type: 'square', f0: 2700, f1: 2100, d: 0.05, vol: 0.08, when: 0.01 });
    noise({ f0: 4000, d: 0.06, vol: 0.12, filter: 'highpass' });
  },
  ko() {
    // "splat" de tinta + golpe grave
    noise({ f0: 700, f1: 120, d: 0.35, vol: 0.5, filter: 'lowpass', q: 0.7 });
    tone({ type: 'sine', f0: 160, f1: 45, d: 0.3, vol: 0.55 });
    tone({ type: 'triangle', f0: 900, f1: 300, d: 0.12, vol: 0.12, when: 0.02 });
  },
  shieldBreak() {
    tone({ type: 'sawtooth', f0: 600, f1: 200, d: 0.18, vol: 0.12 });
    noise({ f0: 3000, d: 0.12, vol: 0.12, filter: 'highpass' });
  },
  explode() {
    noise({ f0: 500, f1: 80, d: 0.5, vol: 0.55, filter: 'lowpass' });
    tone({ type: 'sine', f0: 120, f1: 35, d: 0.45, vol: 0.5 });
  },
  pickup() {
    [0, 0.06, 0.12].forEach((w, i) => tone({ type: 'triangle', f0: [660, 880, 1320][i], d: 0.09, vol: 0.2, when: w }));
  },
  beep(high = false) {
    tone({ type: 'square', f0: high ? 1046 : 523, d: high ? 0.25 : 0.12, vol: 0.12 });
  },
  win() {
    [523, 659, 784, 1046, 784, 1046].forEach((f, i) => tone({ type: 'triangle', f0: f, d: 0.14, vol: 0.2, when: i * 0.11 }));
  },
  ui() {
    tone({ type: 'triangle', f0: 740, d: 0.05, vol: 0.12 });
  },
  coins() {
    [988, 1318, 1568].forEach((f, i) => tone({ type: 'square', f0: f, d: 0.06, vol: 0.08, when: i * 0.07 }));
  },
};

// ── Música: loop boom-bap procedural (90 BPM) ───────────────
const BPM = 92;
const STEP = 60 / BPM / 4; // semicorchea
const KICK = [1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0];
const SNARE = [0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1];
const HAT = [1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1];
const BASS = [41, 0, 0, 41, 0, 0, 44, 0, 46, 0, 0, 44, 0, 0, 39, 0]; // MIDI
const CHORDS = [[65, 68, 72], [63, 67, 70], [61, 65, 68], [63, 67, 70]];
const midi = (n) => 440 * Math.pow(2, (n - 69) / 12);

function scheduleStep(step, t) {
  const when = t - ctx.currentTime;
  const s = step % 16;
  const bar = Math.floor(step / 16) % 4;
  if (KICK[s]) tone({ type: 'sine', f0: 140, f1: 40, d: 0.22, vol: 0.7, bus: musicBus, when });
  if (SNARE[s]) noise({ f0: 1800, d: 0.14, vol: 0.32, q: 0.8, bus: musicBus, when });
  if (HAT[s]) noise({ f0: 8000, d: 0.03, vol: 0.09, filter: 'highpass', bus: musicBus, when });
  if (BASS[s]) tone({ type: 'triangle', f0: midi(BASS[s] - (bar === 2 ? 2 : 0)), d: 0.25, vol: 0.35, bus: musicBus, when });
  if (s === 0 || s === 8) {
    CHORDS[bar].forEach((n) => tone({ type: 'sine', f0: midi(n), a: 0.04, d: 0.9, vol: 0.06, bus: musicBus, when }));
  }
}

function startMusic() {
  if (!ctx || musicTimer || !state.music) return;
  nextNoteTime = ctx.currentTime + 0.1;
  musicTimer = setInterval(() => {
    while (nextNoteTime < ctx.currentTime + 0.25) {
      scheduleStep(musicStep++, nextNoteTime);
      nextNoteTime += STEP;
    }
  }, 50);
}

function stopMusic() {
  if (musicTimer) { clearInterval(musicTimer); musicTimer = null; }
}

export function ensureMusic() { if (state.music) startMusic(); }
