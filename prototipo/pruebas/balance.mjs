// Balance (spec v0.3 §D): partidas solo de bots en los 4 escenarios, avanzando la
// simulación a mano (sin esperar al reloj). Mide la duración de las rondas, las
// causas de KO y cuántas caídas al vacío hay (con y sin crédito).
// Metas de la spec: rondas de 10–25 s en promedio; caídas al vacío entre 5 % y 25 %
// de los KOs; y que los bots no se caigan solos: en 30 partidas, < 15 % de los KOs
// son caídas al vacío sin crédito.
// Uso: node pruebas/balance.mjs [partidasPorArena=8]   (8 × 4 arenas = 32 partidas)
import { createRequire } from 'node:module';
import { pathToFileURL } from 'node:url';
import { resolve } from 'node:path';
const require = createRequire(import.meta.url);
let pw; try { pw = require('playwright'); } catch { pw = require('/opt/node-tools/node_modules/playwright'); }
const PER_ARENA = +(process.argv[2] || 8);
const browser = await pw.chromium.launch({ args: ['--use-gl=angle', '--use-angle=swiftshader', '--enable-unsafe-swiftshader'] });
const page = await browser.newPage({ viewport: { width: 480, height: 270 } });
const errors = [];
page.on('pageerror', (e) => errors.push(String(e)));
page.on('console', (m) => { if (m.type() === 'error') errors.push(m.text()); });
await page.goto(pathToFileURL(resolve('dist/index.html')).href);
await page.waitForTimeout(1200);

const total = { kos: 0, vacio: 0, solo: 0, rounds: 0, secs: 0, matches: 0 };
const fails = [];
for (const arena of ['callejon', 'museo', 'azotea', 'popup']) {
  const r = await page.evaluate(({ arena, n }) => {
    const g = window.__game;
    window.__ui.show(null);
    g.debugSpeed = 0;
    const out = { durations: [], causes: {}, vacio: 0, solo: 0, kos: 0, matches: 0, draws: 0, unfinished: 0 };
    const hook = g.hooks.onMatchEnd;
    g.hooks.onMatchEnd = () => {};
    for (let m = 0; m < n; m++) {
      const slots = Array.from({ length: 4 }, (_, i) => ({ type: 'bot', name: `B${i + 1}`, difficulty: 'normal', species: i }));
      g.hooks.onEvent = (type, d) => {
        if (type === 'RoundDraw') out.draws++;
        if (type !== 'KO') return;
        out.kos++; out.causes[d.cause] = (out.causes[d.cause] || 0) + 1;
        if (d.cause === 'vacio') { out.vacio++; if (d.killer < 0) out.solo++; }
      };
      g.startMatch({ slots, arenaId: arena, pointsToWin: 5, seed: 1000 + m * 7919 + arena.length });
      g.fastForward(60 * 60 * 12); // tope: 12 minutos de juego
      if (g.state !== 'matchEnd') out.unfinished++;
      out.durations.push(...g.roundDurations);
      out.matches++;
    }
    g.hooks.onMatchEnd = hook; g.hooks.onEvent = null;
    return out;
  }, { arena, n: PER_ARENA });
  const avg = r.durations.reduce((a, b) => a + b, 0) / r.durations.length;
  const pv = (100 * r.vacio) / r.kos, ps = (100 * r.solo) / r.kos;
  console.log(`${arena.padEnd(9)} ${r.matches} partidas · ${r.durations.length} rondas (${r.draws} empates) · promedio ${avg.toFixed(1)} s (mín ${Math.min(...r.durations)}, máx ${Math.max(...r.durations)}) · KOs ${r.kos} · al vacío ${r.vacio} (${pv.toFixed(1)} %) · sin crédito ${r.solo} (${ps.toFixed(1)} %)`);
  console.log(`          causas: ${Object.entries(r.causes).sort((a, b) => b[1] - a[1]).map(([k, v]) => `${k} ${v}`).join(', ')}`);
  if (r.unfinished) fails.push(`${arena}: ${r.unfinished} partidas no terminaron`);
  if (avg < 10 || avg > 25) fails.push(`${arena}: promedio de ronda ${avg.toFixed(1)} s fuera de 10–25 s`);
  if (ps >= 15) fails.push(`${arena}: ${ps.toFixed(1)} % de KOs son caídas sin crédito (meta < 15 %)`);
  total.kos += r.kos; total.vacio += r.vacio; total.solo += r.solo; total.rounds += r.durations.length; total.secs += avg * r.durations.length; total.matches += r.matches;
}
const pv = (100 * total.vacio) / total.kos, ps = (100 * total.solo) / total.kos;
console.log(`TOTAL     ${total.matches} partidas · ${total.rounds} rondas · promedio ${(total.secs / total.rounds).toFixed(1)} s · KOs ${total.kos} · al vacío ${pv.toFixed(1)} % · sin crédito ${ps.toFixed(1)} %`);
if (pv < 5 || pv > 25) fails.push(`caídas al vacío ${pv.toFixed(1)} % de los KOs (meta 5–25 %)`);
if (ps >= 15) fails.push(`caídas sin crédito ${ps.toFixed(1)} % de los KOs (meta < 15 %)`);
console.log('errores de consola:', errors.length ? errors : 'ninguno');
console.log(fails.length ? `FUERA DE META: ${fails.join(' · ')}` : 'BALANCE DENTRO DE LAS METAS DE LA SPEC');
await browser.close();
if (errors.length || fails.length) process.exit(1);
