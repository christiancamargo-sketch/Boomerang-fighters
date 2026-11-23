// Prueba de humo: abre el prototipo, navega menús, juega una partida con
// teclado y deja que los bots terminen una partida acelerada. Falla si hay
// errores de consola o si la partida no termina.
import { createRequire } from 'node:module';
// Playwright viene preinstalado globalmente; si no, instálalo con: npm i -D playwright
const require = createRequire(import.meta.url);
let chromium;
try { ({ chromium } = require('playwright')); } catch { ({ chromium } = require('/opt/node-tools/node_modules/playwright')); }
import { pathToFileURL } from 'node:url';
import { resolve } from 'node:path';
import { mkdirSync } from 'node:fs';

const OUT = process.env.SHOTS || 'pruebas/capturas';
mkdirSync(OUT, { recursive: true });
const url = pathToFileURL(resolve('dist/index.html')).href;
const browser = await chromium.launch({ args: ['--use-gl=angle', '--use-angle=swiftshader', '--enable-unsafe-swiftshader'] });
const errors = [];
const page = await browser.newPage({ viewport: { width: 1280, height: 720 }, deviceScaleFactor: 1 });
page.on('console', (m) => { if (m.type() === 'error') errors.push(m.text()); });
page.on('pageerror', (e) => errors.push(String(e)));

await page.goto(url);
await page.waitForTimeout(1500);
await page.screenshot({ path: `${OUT}/01-menu.png` });

// Vestidor + canje de código
await page.click('text=VESTIDOR');
await page.waitForTimeout(400);
await page.fill('#w-code', 'TINTA-2026');
await page.click('#w-code-form button');
await page.waitForTimeout(500);
await page.screenshot({ path: `${OUT}/02-vestidor.png` });
await page.click('#scr-wardrobe >> text=Listo');

// Premio diario
await page.click('text=PREMIO DIARIO');
await page.click('#btn-claim');
await page.waitForTimeout(300);
await page.screenshot({ path: `${OUT}/03-diario.png` });
await page.click('#scr-daily >> text=Atrás');

// Preparar y jugar
await page.click('text=JUGAR');
await page.waitForTimeout(300);
await page.screenshot({ path: `${OUT}/04-setup.png` });
await page.click('#btn-start');
await page.waitForTimeout(3200);
// moverse y lanzar con teclado
await page.keyboard.down('KeyD'); await page.waitForTimeout(400); await page.keyboard.up('KeyD');
await page.keyboard.down('KeyE'); await page.waitForTimeout(450); await page.keyboard.up('KeyE');
await page.waitForTimeout(250);
await page.screenshot({ path: `${OUT}/05-partida.png` });
await page.keyboard.press('KeyF');
await page.keyboard.press('KeyQ');
await page.waitForTimeout(1500);
await page.screenshot({ path: `${OUT}/06-partida-2.png` });
const st = await page.evaluate(() => ({ state: window.__game.state, players: window.__game.players.map((p) => ({ n: p.name, alive: p.alive, x: +p.pos.x.toFixed(2), cap: p.cap.state })) }));
console.log('estado partida:', JSON.stringify(st));

// Partida de bots acelerada hasta el final
await page.evaluate(() => { location.hash = '#demo-6'; location.reload(); });
await page.waitForTimeout(1000);
let done = false;
for (let i = 0; i < 90 && !done; i++) {
  await page.waitForTimeout(1000);
  done = await page.evaluate(() => !document.getElementById('scr-results').hidden);
  if (i === 6) await page.screenshot({ path: `${OUT}/07-bots.png` });
}
await page.screenshot({ path: `${OUT}/08-resultados.png` });
const summary = await page.evaluate(() => ({ rounds: window.__game.round, scores: window.__game.players.map((p) => `${p.name}:${p.score}/${p.kos}ko`) }));
console.log('demo terminó:', done, JSON.stringify(summary));
console.log('errores de consola:', errors.length ? errors : 'ninguno');
await browser.close();
if (errors.length || !done) process.exit(1);
