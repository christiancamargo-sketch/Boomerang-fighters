// ─────────────────────────────────────────────────────────────
// CONFIGURACIÓN Y "TUNING" DEL PROTOTIPO
// Todos los números que definen cómo se siente el juego viven aquí.
// Cuando pasemos a Unity, estos valores se copian a ScriptableObjects.
// ─────────────────────────────────────────────────────────────

export const GAME_INFO = {
  // Nombre de trabajo. Se cambia en un solo lugar cuando se elija el definitivo.
  title: 'GORRAZO',
  brand: 'INKCAP',       // sello "by INKCAP" (el logo real va en src/marca/logo-inkcap.png)
  tagline: 'Lanza tu gorra, mancha a todos',
  version: 'prototipo 0.3 · local',
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
    groundAutoReturn: 3, // la gorra tirada regresa sola a los 3 s (sin quedarte desarmado para siempre)
    tirajeGap: 0.85,     // Serigrafía "Tiraje": separación entre copias en la misma línea
    aimAssistAngle: 0.6, // ~34°: ayuda de apuntado táctil
  },
  round: {
    countdown: 2.2,
    endDelay: 1.9,
    slowMoTime: 0.9,
    slowMoScale: 0.3,
    doubleKoTicks: 6,    // A1: si el último cae < 6 ticks (0.1 s) después del penúltimo → empate
  },
  // A2: muerte súbita (los muros de los lados cortos avanzan y empujan)
  suddenDeath: {
    start: 20,           // s de ronda en juego
    warn: 1,             // s de aviso antes de que se muevan
    speed: 0.5,          // m/s cada muro
    minWidth: 8,         // m de ancho jugable al final
    drawAt: 45,          // s: la ronda termina en empate (no aplica en Lata Caliente)
  },
  // A3: empujón sin gorra
  shove: {
    dist: 2.5, time: 0.2, cooldown: 0.8,
    capKick: 4,          // m que patea una gorra ajena caída
  },
  // A4: "Te la regreso"
  deflect: {
    speedMult: 1.2, maxSpeed: 22,
    lethalTime: 0.6,     // s en los que puede noquear a su propio dueño
  },
  clankFactor: 0.85,     // A6: radio de choque gorra-gorra = 85 % de la suma de radios
  stickerSpeedMult: 0.9, // A5: la gorra gigante va 10 % más lenta
  tracking: { turn: 4.5, range: 11 }, // Tatuaje · Trazo
  puddle: { radius: 2.0, grow: 0.4 },  // Tinta derramada · Charco
  tagWall: { len: 2.5, thick: 0.4, life: 3, height: 1.25 }, // Grafiti · Muro de Tag
  // Spec v0.3 §A: vacíos (huecos por donde te caes)
  voids: {
    shrink: 0.15,        // V1: el centro debe quedar dentro del rectángulo reducido 0.15 m por lado
    creditTime: 2,       // V2: s en los que el KO se le acredita a quien te empujó/golpeó/desvió
    spawnMargin: 0.8,    // V4: nada aparece dentro ni a menos de 0.8 m de un vacío
    botEdge: 0.8,        // V5: los bots se alejan si quedan a menos de 0.8 m de una orilla
  },
  // Spec v0.3 §B4: caja DROP de la Pop-up Store
  drop: { first: 8, every: 20, warn: 1.5, radius: 1.1 },
  hotCan: { fuseMin: 10, fuseMax: 20, passImmunity: 0.5 },  // modo Lata Caliente
  reta: { streakToWin: 3, maxRounds: 10 },                   // modo La Reta
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
// Paleta Okabe-Ito: distinguible con los tipos comunes de daltonismo.
// El color del jugador vive en el aro bajo sus pies y en su gorra, nunca en la ropa.
export const PLAYER_COLORS = [
  { name: 'Naranja', hex: 0xe69f00, css: '#e69f00' },
  { name: 'Celeste', hex: 0x56b4e9, css: '#56b4e9' },
  { name: 'Amarillo', hex: 0xf0e442, css: '#f0e442' },
  { name: 'Rosa', hex: 0xcc79a7, css: '#cc79a7' },
  { name: 'Verde', hex: 0x009e73, css: '#009e73' },
  { name: 'Azul', hex: 0x0072b2, css: '#0072b2' },
  // 7.º y 8.º solo hacen falta en La Reta (hasta 8 en la fila)
  { name: 'Bermellón', hex: 0xd55e00, css: '#d55e00' },
  { name: 'Gris', hex: 0xbbbbbb, css: '#bbbbbb' },
];

// Power-ups = las 6 técnicas de tinta. Duran hasta que te noquean
// (excepto el parche bordado, que se gasta al bloquear un golpe).
export const POWERUPS = {
  serigrafia: {
    name: 'Serigrafía',
    effect: 'Tiraje',
    desc: 'Imprime 2 copias de tu gorra que viajan en fila detrás de la original.',
    color: 0xff4fa3, css: '#ff4fa3', glyph: '≡',
  },
  tatuaje: {
    name: 'Tatuaje',
    effect: 'Trazo',
    desc: 'Tu gorra "marca" al rival más cercano y lo persigue.',
    color: 0x3d5afe, css: '#6d83ff', glyph: '✦',
  },
  tinta: {
    name: 'Tinta derramada',
    effect: 'Charco',
    desc: 'Al chocar deja un charco que crece: quien siga dentro al final, queda manchado.',
    color: 0x1a1a1a, css: '#9aa0a6', glyph: '●',
  },
  grafiti: {
    name: 'Grafiti',
    effect: 'Muro de Tag',
    desc: 'Al terminar la ida (o al rebotar) pinta un muro de esténcil que bloquea gorras y jugadores 3 s.',
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
    desc: 'Tu gorra crece al doble (y va 10 % más lenta). Imposible esquivarla fácil.',
    color: 0xffffff, css: '#ffffff', glyph: '★',
  },
};

export const DIFFICULTY = {
  // deflectChance: probabilidad de intentar "Te la regreso" (spec D)
  // pitHunt / pitShoveChance (v0.3): sin gorra, alcance (m) al que buscan al rival que un empujón
  // dejaría dentro de un vacío, y probabilidad de soltar el empujón al estar en posición
  facil: { label: 'Fácil', reaction: 0.55, aimError: 0.32, dodgeChance: 0.18, throwRate: 0.9, lead: 0.3, deflectChance: 0.15, pitHunt: 6, pitShoveChance: 0.3 },
  normal: { label: 'Normal', reaction: 0.32, aimError: 0.16, dodgeChance: 0.45, throwRate: 1.4, lead: 0.75, deflectChance: 0.35, pitHunt: 9, pitShoveChance: 1 },
  dificil: { label: 'Difícil', reaction: 0.17, aimError: 0.07, dodgeChance: 0.75, throwRate: 2.1, lead: 1.0, deflectChance: 0.6, pitHunt: 9, pitShoveChance: 1 },
};

// ── Modos de juego (spec C) ───────────────────────────────────
export const MODES = {
  gorrazo: { id: 'gorrazo', name: '¡Gorrazo!', desc: 'Último en pie gana la ronda.', min: 2, max: 6 },
  lataCaliente: { id: 'lataCaliente', name: 'Lata Caliente', desc: 'Una lata se pega a alguien y explota entre 10 y 20 s. Gorrazos y golpes no noquean: pasan la lata.', min: 3, max: 6 },
  laReta: { id: 'laReta', name: 'La Reta', desc: '1 contra 1: el ganador se queda y entra el siguiente de la fila. Gana quien encadene 3 o tenga más victorias a las 10 rondas.', min: 2, max: 8 },
  retoDelDia: { id: 'retoDelDia', name: 'Reto del Día', desc: 'Tú contra 3 bots difíciles. Misma arena y regla para todos hoy.', min: 4, max: 4 },
};

// Interruptores de reglas (spec A2–A4 y B). Se guardan en el celular.
export const DEFAULT_RULES = { suddenDeath: true, shove: true, deflectReturn: true, graffitiWall: true, powerUps: true };
export const RULE_INFO = {
  suddenDeath: { name: 'Muerte súbita', desc: 'A los 20 s los muros de los lados se cierran y empujan; a los 45 s es empate.' },
  shove: { name: 'Empujón', desc: 'Sin gorra, el golpe no noquea: empuja 2.5 m (y patea gorras caídas). [A PRUEBA]' },
  deflectReturn: { name: 'Te la regreso', desc: 'Golpear una gorra rival la regresa a su dueño y puede noquearlo. [A PRUEBA]' },
  graffitiWall: { name: 'Muro de Tag', desc: 'La lata de Grafiti pinta un muro que bloquea 3 s. Apagado: rastro que alenta.' },
  powerUps: { name: 'Latas de técnica', desc: 'Aparecen latas con poderes en la arena. Apagado: puro gorrazo.' },
};

// "Regla del día" del Reto del Día (se elige con la semilla AAAAMMDD)
export const DAILY_RULES = [
  { id: 'todo', name: 'Todo vale', rules: {} },
  { id: 'sin-empujon', name: 'Sin empujón', rules: { shove: false } },
  { id: 'desvio-clasico', name: 'Desvío clásico', rules: { deflectReturn: false } },
  { id: 'aerosol', name: 'Rastro de aerosol', rules: { graffitiWall: false } },
  { id: 'sin-muerte-subita', name: 'Sin muerte súbita', rules: { suddenDeath: false } },
  { id: 'lluvia-latas', name: 'Lluvia de latas', rules: {}, pickupsEvery: [3, 5], pickupsMax: 3 },
];

// Letreros de KO (A8). La causa viaja en el evento KO igual que en el núcleo C#.
export const KO_LABELS = {
  gorrazo: '¡GORRAZO!', deRegreso: '¡DE REGRESO!', deBanda: '¡DE BANDA!', teLaRegreso: '¡TE LA REGRESO!',
  zaz: '¡ZAZ!', zap: '¡ZAP!', splash: '¡SPLASH!', tiraje: '¡TIRAJE!',
  vacio: '¡AL VACÍO!', drop: '¡DROP!', // spec v0.3: caída a un vacío y caja DROP
};

// Progresión local (spec E): todo en el celular, sin servidor.
export const PROGRESSION = {
  xpForLevel: (lvl) => 150 + (lvl - 1) * 50, // curva del GDD §10.2
  maxLevel: 50,
  rewards: {
    participate: 20,
    roundWin: 10,
    ko: 6,
    matchWin: 50,
    firstWinOfDay: 50,
  },
  // Gotas por partida: 10–25
  gotas: { base: 10, max: 25 },
  // Premio diario de 7 días con racha suave (si faltas se pausa, no regresa al día 1)
  daily: [
    { day: 1, coins: 150 },
    { day: 2, sticker: true },
    { day: 3, coins: 250 },
    { day: 4, coins: 25, extra: true },
    { day: 5, emote: true },
    { day: 6, coins: 100 },
    { day: 7, weeklyCap: true },
  ],
  calendarDays: 28, // 28 sellos → "Playera del mes"
};
