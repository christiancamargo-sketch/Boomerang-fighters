// Sin archivos de marca (ni logo ni diseños de artistas) todo debe funcionar igual:
// los escenarios se arman con el texto "INKCAP" en tipografía de rótulo y colores
// lisos (no se inventa un logo). Esta prueba carga el prototipo quitándole la marca
// incrustada, arma los 4 escenarios, juega una partida de bots en la tienda hasta
// el podio y falla si hay errores de consola.
import { createRequire } from 'node:module';
import { pathToFileURL } from 'node:url';
import { resolve } from 'node:path';
import { mkdirSync } from 'node:fs';
const require = createRequire(import.meta.url);
let pw; try { pw = require('playwright'); } catch { pw = require('/opt/node-tools/node_modules/playwright'); }
const OUT = process.env.SHOTS || 'pruebas/capturas';
mkdirSync(OUT, { recursive: true });
const browser = await pw.chromium.launch({ args: ['--use-gl=angle', '--use-angle=swiftshader', '--enable-unsafe-swiftshader'] });
const page = await browser.newPage({ viewport: { width: 1280, height: 720 } });
const errors = [];
page.on('pageerror', (e) => errors.push(String(e)));
page.on('console', (m) => { if (m.type() === 'error') errors.push(m.text()); });
// la página intenta poner window.__MARCA con el logo y los artistas: aquí se ignora
await page.addInitScript(() => {
  Object.defineProperty(window, '__MARCA', { get: () => window.__sinMarca || (window.__sinMarca = { logo: null, artistas: [] }), set() {} });
});
await page.goto(pathToFileURL(resolve('dist/index.html')).href);
await page.waitForTimeout(1600);
const marca = await page.evaluate(() => ({ logo: !!window.__MARCA.logoImg, artistas: (window.__MARCA.artistas || []).length, menu: window.__game.arena.def.id }));
console.log('marca cargada:', JSON.stringify(marca));
if (marca.logo || marca.artistas) errors.push('la prueba no logró quitar la marca');
await page.screenshot({ path: `${OUT}/s0-sin-marca-menu.png` });
for (const arena of ['callejon', 'museo', 'azotea', 'popup']) {
  await page.evaluate((arena) => {
    const g = window.__game, ui = window.__ui;
    ui.autopilot = true;
    ui.setup = { ...ui.setup, mode: 'gorrazo', humans: 1, bots: 3, arena, points: 3 };
    ui.startMatch();
    g.debugSpeed = 0;
    g.fastForward(60 * 6);
  }, arena);
  await page.waitForTimeout(400);
  await page.screenshot({ path: `${OUT}/s-sin-marca-${arena}.png` });
  if (arena !== 'popup') await page.evaluate(() => { window.__game.debugSpeed = 1; window.__ui.quitToMenu(); });
  await page.waitForTimeout(200);
}
// la partida de la tienda sigue hasta el podio (taza con el texto INKCAP)
await page.evaluate(() => { const g = window.__game; g.fastForward(60 * 60 * 8); g.debugSpeed = 1; });
await page.waitForTimeout(2600);
const fin = await page.evaluate(() => ({ results: !document.getElementById('scr-results').hidden, podium: window.__game.mode === 'podium', drops: window.__game.eventCounts.DropLand || 0 }));
await page.screenshot({ path: `${OUT}/s1-sin-marca-podio.png` });
console.log('partida en la tienda sin marca:', JSON.stringify(fin));
if (!fin.results || !fin.podium) errors.push('la partida sin marca no llegó al podio');
console.log('errores de consola:', errors.length ? errors : 'ninguno');
await browser.close();
if (errors.length) process.exit(1);
console.log('SIN MARCA: TODO BIEN');
