// ─────────────────────────────────────────────────────────────
// ARRANQUE: conecta juego + interfaz + progresión
// ─────────────────────────────────────────────────────────────
import { Game } from './game.js';
import { UI, retoDeSemilla } from './ui.js';
import { loadProgress } from './progress.js';
import { endInputFrame, mouse, setupMouse } from './input.js';
import { sfx } from './audio.js';
import { loadMarca } from './marca.js';
import { registerBrandDesigns, clearSceneTextureCaches } from './textures.js';

// Primero se cargan el logo y los diseños reales de INKCAP (incrustados en la página)
function start() {
  loadProgress();
  const fontsAtStart = !document.fonts || document.fonts.check('20px Bungee');

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
    onSuddenDeath: () => ui.banner('¡MUERTE SÚBITA!', 'los muros se cierran', '#f0e442', 1000),
    onRoundEnd: (w, players, reason) => {
      ui.hudPlayers(players);
      if (w) ui.emote(w);
      const reta = game.modeId === 'laReta';
      const sub = w
        ? (reta ? `${w.streak} seguidas · ${w.score} victorias` : `${w.score} de ${game.pointsToWin}`)
        : reason === 'doble' ? 'doble KO: nadie suma' : reason === 'tiempo' ? 'se acabó el tiempo' : 'nadie quedó en pie';
      ui.banner(w ? (reta ? `¡${w.name} SE QUEDA!` : `PUNTO PARA ${w.name}`) : '¡EMPATE!', sub, w ? w.color.css : '#f4f1ea', 1100);
    },
    onMatchEnd: (res) => {
      document.body.classList.remove('in-match');
      setTimeout(() => ui.showResults(res), 1500 / Math.max(1, game.debugSpeed));
    },
  });

  ui = new UI(game);

  // pantalla de carga con la marca: se va cuando están las fuentes (o a los 1.2 s)
  const loading = document.getElementById('loading');
  if (loading) {
    const hide = () => { loading.classList.add('gone'); setTimeout(() => { loading.hidden = true; }, 350); };
    const t0 = performance.now();
    const fontsReady = document.fonts && document.fonts.ready ? document.fonts.ready : Promise.resolve();
    Promise.race([fontsReady, new Promise((r) => setTimeout(r, 1200))]).then(() => setTimeout(hide, Math.max(0, 600 - (performance.now() - t0))));
  }

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
    game.startMatch({ slots, arenaId: 'aleatorio', pointsToWin: 3, seed: 20261123 });
  }

  // Los rótulos de los escenarios se pintan en canvas: si las fuentes web llegaron
  // después de armar el menú, se repinta la tienda con la tipografía buena.
  if (document.fonts && document.fonts.ready && !fontsAtStart) {
    document.fonts.ready.then(() => {
      if (!document.fonts.check('20px Bungee') || game.mode !== 'menu') return;
      clearSceneTextureCaches();
      game._canMats.clear();
      game._arenaUsed = true;
      game.showMenu(ui._outfit());
    });
  }

  // Para pruebas automáticas y depuración
  window.__game = game;
  window.__ui = ui;
  window.__retoDeSemilla = retoDeSemilla;
}

loadMarca().then(() => { registerBrandDesigns(); start(); });
