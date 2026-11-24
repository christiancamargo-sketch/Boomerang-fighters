// Revisa la versión táctil en un celular horizontal (emulado): menú (la tienda),
// vestidor, "Arma la reta" con los 4 escenarios y una partida táctil en la Pop-up Store.
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
const innerW = pw.devices['iPhone 13 landscape'].viewport.width;
const errors = [];
page.on('pageerror', (e) => errors.push(String(e)));
page.on('console', (m) => { if (m.type() === 'error') errors.push(m.text()); });
await page.goto(pathToFileURL(resolve('dist/index.html')).href);
await page.waitForTimeout(1500);
await page.screenshot({ path: `${OUT}/m1-menu.png` });
// el menú cabe sin scroll horizontal y trae el sello de la marca
const fit = await page.evaluate(() => ({ sw: document.documentElement.scrollWidth, w: innerWidth, seal: document.querySelector('#scr-menu .seal').innerText + (document.querySelector('#scr-menu .seal img.inkcap-logo') ? ' INKCAP' : '') }));
console.log('menú:', JSON.stringify(fit));
if (fit.sw > fit.w || !/INKCAP/.test(fit.seal)) errors.push('menú no cabe o falta el sello');
// vestidor con gorras en el celular
await page.tap('text=VESTIDOR');
await page.waitForTimeout(300);
await page.tap('#w-tabs [data-tab="caps"]');
await page.waitForTimeout(200);
await page.screenshot({ path: `${OUT}/m1b-vestidor-gorras.png` });
await page.tap('#scr-wardrobe >> text=Listo');
await page.waitForTimeout(300);
await page.tap('text=JUGAR');
await page.waitForTimeout(300);
await page.tap('#s-mode [data-v="retoDelDia"]');
await page.waitForTimeout(200);
await page.screenshot({ path: `${OUT}/m2b-setup-reto.png` });
await page.tap('#s-mode [data-v="gorrazo"]');
await page.waitForTimeout(200);
// los 4 escenarios caben en la lista y se puede elegir la tienda
const cards = await page.locator('#s-arena .arena-card').evaluateAll((els) => els.map((e) => e.dataset.v));
if (cards.join() !== 'callejon,museo,azotea,popup,aleatorio') errors.push(`lista de escenarios: ${cards.join()}`);
await page.tap('#s-arena [data-v="popup"]');
await page.waitForTimeout(200);
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
const st = await page.evaluate(() => ({ arena: window.__game.arena.def.id, cap: window.__game.players[0].cap.state, x: window.__game.players[0].pos.x }));
if (st.arena !== 'popup') errors.push(`no se jugó en la tienda: ${st.arena}`);
// las fichas del HUD no tapan el muro del fondo (van a las esquinas en celular horizontal)
const chips = await page.locator('#hud-players .chip').evaluateAll((els) => els.map((e) => { const r = e.getBoundingClientRect(); return [Math.round(r.left), Math.round(r.right), Math.round(r.top)]; }));
const centro = chips.filter(([l, r]) => r > innerW * 0.25 && l < innerW * 0.75).length;
console.log('fichas del HUD (izq, der, arriba):', JSON.stringify(chips));
if (centro) errors.push(`${centro} fichas del HUD tapan el centro del muro del fondo`);
console.log('táctil:', JSON.stringify(st), 'errores:', errors.length ? errors : 'ninguno');
await browser.close();
if (errors.length) process.exit(1);
