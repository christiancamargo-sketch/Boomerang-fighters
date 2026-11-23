// Revisa la versión táctil en un celular horizontal (emulado).
import { createRequire } from 'node:module';
import { pathToFileURL } from 'node:url';
import { resolve } from 'node:path';
import { mkdirSync } from 'node:fs';
const require = createRequire(import.meta.url);
let pw; try { pw = require('playwright'); } catch { pw = require('/opt/node-tools/node_modules/playwright'); }
const OUT = process.env.SHOTS || 'pruebas/capturas';
mkdirSync(OUT, { recursive: true });
const browser = await pw.chromium.launch({ args: ['--use-gl=angle', '--use-angle=swiftshader', '--enable-unsafe-swiftshader'] });
const ctx = await browser.newContext({ ...pw.devices['iPhone 13 landscape'] });
const page = await ctx.newPage();
const errors = [];
page.on('pageerror', (e) => errors.push(String(e)));
page.on('console', (m) => { if (m.type() === 'error') errors.push(m.text()); });
await page.goto(pathToFileURL(resolve('dist/index.html')).href);
await page.waitForTimeout(1500);
await page.screenshot({ path: `${OUT}/m1-menu.png` });
await page.tap('text=JUGAR');
await page.waitForTimeout(300);
await page.screenshot({ path: `${OUT}/m2-setup.png` });
await page.tap('#btn-start');
await page.waitForTimeout(3000);
// joystick: arrastre en la mitad izquierda con eventos de puntero
const box = await page.locator('#t-left').boundingBox();
const sx = box.x + 120, sy = box.y + box.height - 120;
await page.mouse.move(sx, sy); await page.mouse.down(); await page.mouse.move(sx + 50, sy - 20, { steps: 5 });
await page.waitForTimeout(500);
await page.mouse.up();
await page.tap('#t-throw');
await page.waitForTimeout(200);
await page.screenshot({ path: `${OUT}/m3-partida.png` });
const st = await page.evaluate(() => ({ cap: window.__game.players[0].cap.state, x: window.__game.players[0].pos.x }));
console.log('táctil:', JSON.stringify(st), 'errores:', errors.length ? errors : 'ninguno');
await browser.close();
if (errors.length) process.exit(1);
