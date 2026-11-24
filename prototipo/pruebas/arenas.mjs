// Capturas de los 4 escenarios (spec v0.3) en escritorio (1280×720) y en celular
// horizontal, para revisar a ojo: ¿se leen los vacíos?, ¿se ve la marca INKCAP?,
// ¿algo tapa la acción? También mide cuántas llamadas de dibujo cuesta cada arena
// (presupuesto para 30 fps en un celular de gama media) y revisa que no haya
// errores de consola. Uso: node pruebas/arenas.mjs [arena…]
import { createRequire } from 'node:module';
import { pathToFileURL } from 'node:url';
import { resolve } from 'node:path';
import { mkdirSync } from 'node:fs';
const require = createRequire(import.meta.url);
let pw; try { pw = require('playwright'); } catch { pw = require('/opt/node-tools/node_modules/playwright'); }
const OUT = process.env.SHOTS || 'pruebas/capturas';
mkdirSync(OUT, { recursive: true });
const only = process.argv.slice(2);
const ARENAS = ['callejon', 'museo', 'azotea', 'popup'].filter((a) => !only.length || only.includes(a));
const browser = await pw.chromium.launch({ args: ['--use-gl=angle', '--use-angle=swiftshader', '--enable-unsafe-swiftshader'] });
const errors = [];
const url = pathToFileURL(resolve('dist/index.html')).href;
const MAX_CALLS = 330; // tope de llamadas de dibujo por cuadro con 4 jugadores (en reposo, sin partículas)

async function shots(tag, ctxOpts) {
  const ctx = await browser.newContext(ctxOpts);
  const page = await ctx.newPage();
  page.on('pageerror', (e) => errors.push(`${tag}: ${e}`));
  page.on('console', (m) => { if (m.type() === 'error') errors.push(`${tag}: ${m.text()}`); });
  await page.goto(url);
  await page.waitForTimeout(1500);
  for (const arena of ARENAS) {
    // partida de 4 (tú en piloto automático + 3 bots), semilla fija: siempre la misma foto
    await page.evaluate((arena) => {
      const g = window.__game, ui = window.__ui;
      ui.autopilot = true;
      ui.setup = { ...ui.setup, mode: 'gorrazo', humans: 1, bots: 3, arena, points: 3 };
      ui.startMatch();
      g.debugSpeed = 0;
      // misma semilla siempre
      g.startMatch({ slots: g.players.map((p) => p.slot), arenaId: arena, pointsToWin: 3, seed: 20261124 });
      g.fastForward(60 * 2 + 20);
    }, arena);
    await page.waitForTimeout(500);
    await page.screenshot({ path: `${OUT}/${tag}-${arena}-inicio.png` });
    // a media ronda: gorras volando, manchas de tinta y (en la tienda) la sombra de la caja DROP
    const info = await page.evaluate((arena) => {
      const g = window.__game;
      // se detiene en plena ronda: con 2 KOs ya pintados (o, en la tienda, con la sombra de la caja puesta)
      const listo = () => (arena === 'popup' ? g.drop && g.playT > 7.25 : (g.eventCounts.KO || 0) >= 2 && g.playT > 5);
      for (let i = 0; i < 3000 && g.state !== 'matchEnd'; i++) { g.fastForward(4); if (g.state === 'play' && listo()) break; }
      g._visuals(0.016);
      g.renderer.info.autoReset = true;
      return { state: g.state, playT: +g.playT.toFixed(2), ko: g.eventCounts.KO || 0, drop: !!g.drop };
    }, arena);
    await page.waitForTimeout(400);
    await page.screenshot({ path: `${OUT}/${tag}-${arena}-juego.png` });
    // costo "en reposo": se dejan caer las piezas y partículas del último KO y se mide un cuadro
    await page.evaluate(() => { window.__game._fx(4); });
    await page.waitForTimeout(250);
    const calls = await page.evaluate(() => { const i = window.__game.renderer.info; return { calls: i.render.calls, tris: i.render.triangles, geos: i.memory.geometries, texs: i.memory.textures }; });
    console.log(`${tag} ${arena}:`, JSON.stringify({ ...info, ...calls }));
    if (calls.calls > MAX_CALLS) errors.push(`${tag} ${arena}: ${calls.calls} llamadas de dibujo (tope ${MAX_CALLS})`);
    await page.evaluate(() => { window.__game.debugSpeed = 1; window.__ui.quitToMenu(); window.__ui.autopilot = false; });
    await page.waitForTimeout(250);
  }
  await ctx.close();
}

await shots('pc', { viewport: { width: 1280, height: 720 }, deviceScaleFactor: 1 });
await shots('cel', { ...pw.devices['iPhone 13 landscape'] });
console.log('errores de consola:', errors.length ? errors : 'ninguno');
await browser.close();
if (errors.length) process.exit(1);
