// ─────────────────────────────────────────────────────────────
// ARRANQUE: conecta juego + interfaz + progresión
// ─────────────────────────────────────────────────────────────
import { Game } from './game.js';
import { UI } from './ui.js';
import { loadProgress } from './progress.js';
import { endInputFrame, mouse, setupMouse } from './input.js';
import { sfx } from './audio.js';

loadProgress();

const canvas = document.getElementById('scene');
setupMouse(canvas);

let ui = null;
let roundNo = 1;

const game = new Game(canvas, {
  floatLayer: document.getElementById('floaters'),
  endInputFrame,
  mouse,
  onMatchStart: (players) => { ui.hudPlayers(players); document.body.classList.add('in-match'); },
  onRoundStart: (n, players) => { roundNo = n; ui.hudPlayers(players); },
  onCountdown: (t) => ui.banner(t, t === '¡YA!' ? '' : `RONDA ${roundNo}`, t === '¡YA!' ? '#ffd23f' : '#f4f1ea', t === '¡YA!' ? 650 : 760),
  onKO: () => ui.hudPlayers(game.players),
  onRoundEnd: (w, players) => {
    ui.hudPlayers(players);
    ui.banner(w ? `PUNTO PARA ${w.name}` : 'EMPATE', w ? `${w.score} de ${game.pointsToWin}` : 'nadie quedó en pie', w ? w.color.css : '#f4f1ea', 1100);
  },
  onMatchEnd: (res) => {
    document.body.classList.remove('in-match');
    setTimeout(() => ui.showResults(res), 1500 / Math.max(1, game.debugSpeed));
  },
});

ui = new UI(game);

// botones fijos
document.getElementById('btn-resume').addEventListener('click', () => { sfx.ui(); ui.resume(); });
document.getElementById('btn-quit').addEventListener('click', () => { sfx.ui(); document.body.classList.remove('in-match'); ui.quitToMenu(); });
document.getElementById('btn-rematch').addEventListener('click', () => { sfx.ui(); ui.startMatch(); });
document.getElementById('btn-results-menu').addEventListener('click', () => { sfx.ui(); ui.show('menu'); });
document.getElementById('btn-rotate-ok').addEventListener('click', () => { document.body.classList.add('rotate-dismissed'); });

// Modo espectador / pruebas: #demo o #demo-3 (velocidad ×3) → partida solo de bots
const m = location.hash.match(/^#demo(?:-(\d+))?$/);
if (m) {
  game.debugSpeed = m[1] ? Math.min(8, +m[1]) : 1;
  const slots = Array.from({ length: 4 }, (_, i) => ({ type: 'bot', name: `BOT ${i + 1}`, difficulty: 'normal', species: i, shirt: ['neon-grafiti', 'serigrafia-retro', 'sticker-bomb', 'rayas-lucha'][i] }));
  ui.show(null);
  game.startMatch({ slots, arenaId: 'aleatorio', pointsToWin: 3 });
}

// Para pruebas automáticas y depuración
window.__game = game;
window.__ui = ui;
