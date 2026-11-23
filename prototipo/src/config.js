// ─────────────────────────────────────────────────────────────
// CONFIGURACIÓN Y "TUNING" DEL PROTOTIPO
// Todos los números que definen cómo se siente el juego viven aquí.
// Cuando pasemos a Unity, estos valores se copian a ScriptableObjects.
// ─────────────────────────────────────────────────────────────

export const GAME_INFO = {
  // Nombre de trabajo. Se cambia en un solo lugar cuando se elija el definitivo.
  title: 'GORRAZO',
  tagline: 'Lanza tu gorra. Mancha a todos.',
  version: 'prototipo 0.1',
};

export const TUNING = {
  dtFixed: 1 / 60,
  player: {
    radius: 0.55,
    speed: 6.4,          // unidades/seg
    accel: 70,           // qué tan rápido alcanza la velocidad (respuesta del stick)
    chargeMoveMult: 0.55, // te mueves más lento mientras cargas un tiro
    dashSpeed: 18,
    dashTime: 0.15,
    dashCooldown: 0.9,
    meleeRange: 1.75,
    meleeArc: Math.PI * 0.62,
    meleeActive: 0.13,   // ventana en la que el golpe "pega"
    meleeTime: 0.22,
    meleeCooldown: 0.45,
    slowMult: 0.5,       // al pisar pintura de grafiti
  },
  cap: {
    radius: 0.38,
    flyHeight: 1.05,
    speedMin: 13,
    speedMax: 22,
    chargeTime: 0.55,    // segundos para carga completa
    tapThreshold: 0.12,  // si sueltas antes de esto = tiro rápido
    outTimeMin: 0.34,
    outTimeMax: 0.62,
    returnSpeed: 17,
    returnAccel: 45,
    turnRate: 8.5,       // rad/seg, qué tanto "curva" al regresar
    recallSpeed: 15,
    catchRadius: 0.95,
    earlyCatchDelay: 0.12,
    spin: 24,
    groundFriction: 6,
    aimAssistAngle: 0.6, // ~34°: ayuda de apuntado táctil
  },
  round: {
    countdown: 2.2,
    endDelay: 1.9,
    slowMoTime: 0.9,
    slowMoScale: 0.3,
  },
  pickups: {
    firstSpawn: 4,
    every: [6, 10],
    max: 2,
  },
  feel: {
    hitStop: 0.075,
    shakeKO: 0.45,
    shakeClank: 0.18,
  },
};

// Paleta de jugadores: colores muy distintos entre sí (legibles en pantalla chica
// y diferenciables con daltonismo común gracias a la luminosidad distinta).
export const PLAYER_COLORS = [
  { name: 'Naranja', hex: 0xff6a2b, css: '#ff6a2b' },
  { name: 'Cian', hex: 0x22c7e8, css: '#22c7e8' },
  { name: 'Amarillo', hex: 0xffd23f, css: '#ffd23f' },
  { name: 'Magenta', hex: 0xe84393, css: '#e84393' },
  { name: 'Verde', hex: 0x4cd964, css: '#4cd964' },
  { name: 'Violeta', hex: 0x8e6cff, css: '#8e6cff' },
];

// Power-ups = las 6 técnicas de tinta. Duran hasta que te noquean
// (excepto el parche bordado, que se gasta al bloquear un golpe).
export const POWERUPS = {
  serigrafia: {
    name: 'Serigrafía',
    effect: 'Triple tiro',
    desc: 'Imprime 2 copias extra de tu gorra en abanico.',
    color: 0xff4fa3, css: '#ff4fa3', glyph: '≡',
  },
  tatuaje: {
    name: 'Tatuaje',
    effect: 'Teledirigida',
    desc: 'Tu gorra "marca" al rival más cercano y lo persigue.',
    color: 0x3d5afe, css: '#6d83ff', glyph: '✦',
  },
  tinta: {
    name: 'Tinta derramada',
    effect: 'Explosiva',
    desc: 'Al chocar revienta en un charco de tinta que noquea.',
    color: 0x1a1a1a, css: '#9aa0a6', glyph: '●',
  },
  grafiti: {
    name: 'Grafiti',
    effect: 'Rastro de aerosol',
    desc: 'Tu gorra deja pintura fresca: quien la pisa se alenta.',
    color: 0x00e676, css: '#00e676', glyph: '≈',
  },
  bordado: {
    name: 'Parche bordado',
    effect: 'Escudo',
    desc: 'Aguanta un golpe. Se descose al bloquear.',
    color: 0xffb300, css: '#ffb300', glyph: '✚',
  },
  sticker: {
    name: 'Sticker XL',
    effect: 'Gorra gigante',
    desc: 'Tu gorra crece al doble. Imposible esquivarla fácil.',
    color: 0xffffff, css: '#ffffff', glyph: '★',
  },
};

export const DIFFICULTY = {
  facil: { label: 'Fácil', reaction: 0.55, aimError: 0.32, dodgeChance: 0.18, throwRate: 0.9, lead: 0.3 },
  normal: { label: 'Normal', reaction: 0.32, aimError: 0.16, dodgeChance: 0.45, throwRate: 1.4, lead: 0.75 },
  dificil: { label: 'Difícil', reaction: 0.17, aimError: 0.07, dodgeChance: 0.75, throwRate: 2.1, lead: 1.0 },
};

// Progresión de ejemplo (en el juego real esto vive en el backend).
export const PROGRESSION = {
  xpForLevel: (lvl) => 100 + (lvl - 1) * 60,
  rewards: {
    participate: 20,
    roundWin: 10,
    ko: 6,
    matchWin: 50,
  },
  coinsPerLevel: 60,
  daily: [
    { day: 1, coins: 50 },
    { day: 2, coins: 75 },
    { day: 3, xp: 80 },
    { day: 4, coins: 100 },
    { day: 5, xp: 150 },
    { day: 6, coins: 150 },
    { day: 7, shirt: 'tinta-derramada', coins: 200 },
  ],
};
