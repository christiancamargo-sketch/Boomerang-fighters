// Prueba de humo (v0.2): abre el prototipo, revisa la marca, el vestidor con
// gorras, el premio diario (racha suave y gorra de la semana), la pantalla de
// reglas, una partida con teclado y una partida de CADA modo con bots
// acelerados hasta el final. Falla si hay errores de consola o algo no termina.
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
const fails = [];
const check = (ok, msg) => { console.log(`${ok ? 'OK  ' : 'FALLA'} ${msg}`); if (!ok) fails.push(msg); };
const page = await browser.newPage({ viewport: { width: 1280, height: 720 }, deviceScaleFactor: 1 });
page.on('console', (m) => { if (m.type() === 'error') errors.push(m.text()); });
page.on('pageerror', (e) => errors.push(String(e)));

await page.goto(url);
await page.evaluate(() => localStorage.clear());
await page.reload();
await page.waitForTimeout(400);
await page.screenshot({ path: `${OUT}/00-carga.png` });
await page.waitForTimeout(1300);
check(await page.evaluate(() => document.getElementById('loading').hidden), 'la pantalla de carga se va sola');
const seal = await page.locator('#scr-menu .seal').innerText();
check(/by\s*INKCAP/.test(seal), `menú con sello "${seal.trim()}"`);
await page.screenshot({ path: `${OUT}/01-menu.png` });

// ── Vestidor: canje de código, gorras ─────────────────────────
await page.click('text=VESTIDOR');
await page.waitForTimeout(300);
await page.fill('#w-code', 'TINTA-2026');
await page.click('#w-code-form button');
await page.waitForTimeout(300);
check((await page.locator('#w-code-msg').innerText()).includes('Desbloqueaste'), 'canje de código simulado');
await page.click('#w-tabs [data-tab="caps"]');
await page.waitForTimeout(200);
const capItems = await page.locator('#w-caps .shirt').count();
check(capItems >= 6, `vestidor con ${capItems} gorras`);
await page.click('#w-caps [data-equip-cap="inkcap-rotulo"]');
await page.waitForTimeout(300);
const eqCap = await page.evaluate(() => JSON.parse(localStorage.getItem('arena-inkcap.progress.v1')).equipped.cap);
check(eqCap === 'inkcap-rotulo', `gorra equipada: ${eqCap}`);
await page.screenshot({ path: `${OUT}/02-vestidor-gorras.png` });
const species = await page.locator('#w-species button').allInnerTexts();
check(species.length === 8 && species[0] === 'Lienzo' && species.some((s) => s.startsWith('Axo')), `muñequitos: ${species.join(', ')}`);
await page.click('#scr-wardrobe >> text=Listo');

// ── Premio diario: día 1 y luego racha suave hasta el día 7 ───
await page.click('text=PREMIO DIARIO');
const g0 = await page.evaluate(() => JSON.parse(localStorage.getItem('arena-inkcap.progress.v1')).coins);
await page.click('#btn-claim');
await page.waitForTimeout(200);
const g1 = await page.evaluate(() => JSON.parse(localStorage.getItem('arena-inkcap.progress.v1')).coins);
check(g1 - g0 === 150, `día 1 = +150 Gotas (${g0} → ${g1})`);
await page.screenshot({ path: `${OUT}/03-diario.png` });
// simula que faltó varios días con 6 días cobrados: la racha se pausa y toca el día 7
await page.evaluate(() => {
  const k = 'arena-inkcap.progress.v1';
  const d = JSON.parse(localStorage.getItem(k));
  d.daily = { last: '2000-01-01', claimed: 6 };
  d.calendar = { stamps: 27, months: 0 };
  localStorage.setItem(k, JSON.stringify(d));
});
await page.reload();
await page.waitForTimeout(1500);
await page.click('text=PREMIO DIARIO');
check((await page.locator('#btn-claim').innerText()).includes('7'), 'racha suave: tras faltar sigue en el día 7');
await page.click('#btn-claim');
await page.waitForTimeout(200);
const msg = await page.locator('#d-msg').innerText();
check(msg.includes('Gorra de la semana') && msg.includes('Playera del mes'), `día 7 + sello 28: "${msg}"`);
await page.screenshot({ path: `${OUT}/03b-diario-dia7.png` });
await page.click('#scr-daily >> text=Atrás');

// comprar una gorra con Gotas
await page.click('text=VESTIDOR');
await page.click('#w-tabs [data-tab="caps"]');
await page.click('#w-caps [data-buy-cap="cuadros"]');
await page.waitForTimeout(200);
const p1 = await page.evaluate(() => JSON.parse(localStorage.getItem('arena-inkcap.progress.v1')));
check(p1.ownedCaps.includes('cuadros') && p1.equipped.cap === 'cuadros', 'compra de gorra con Gotas');
await page.click('#scr-wardrobe >> text=Listo');

// ── Reglas ────────────────────────────────────────────────────
await page.click('text=REGLAS');
await page.waitForTimeout(200);
check(await page.locator('#rules-list .switch').count() === 4, 'pantalla de reglas con 4 interruptores');
await page.click('[data-rule="shove"]');
const r1 = await page.evaluate(() => JSON.parse(localStorage.getItem('gorrazo.rules.v1')));
check(r1.shove === false && r1.suddenDeath === true, 'interruptor guardado en localStorage');
await page.screenshot({ path: `${OUT}/04-reglas.png` });
await page.click('[data-rule="shove"]');
await page.click('#scr-rules >> text=Listo');

// ── Partida con teclado ───────────────────────────────────────
await page.click('text=JUGAR');
await page.waitForTimeout(300);
await page.screenshot({ path: `${OUT}/05-setup.png` });
await page.click('#btn-start');
await page.waitForTimeout(3200);
await page.keyboard.down('KeyD'); await page.waitForTimeout(400); await page.keyboard.up('KeyD');
await page.keyboard.down('KeyE'); await page.waitForTimeout(450); await page.keyboard.up('KeyE');
await page.waitForTimeout(250);
await page.screenshot({ path: `${OUT}/06-partida.png` });
await page.keyboard.press('KeyF');
await page.keyboard.press('KeyQ');
await page.waitForTimeout(800);
const st = await page.evaluate(() => ({ state: window.__game.state, cap: window.__game.players[0].capDesign, players: window.__game.players.map((p) => ({ n: p.name, alive: p.alive, x: +p.pos.x.toFixed(2), cap: p.cap.state })) }));
console.log('estado partida:', JSON.stringify(st));
check(st.cap === 'cuadros', 'tu gorra del vestidor entra a la partida');
check(st.players[0].x !== 0, 'el teclado mueve a J1');
await page.keyboard.press('Escape');
await page.waitForTimeout(200);
check(await page.locator('#scr-pause').isVisible(), 'pausa con Esc');
await page.click('#btn-quit');
await page.waitForTimeout(300);

// ── Una partida de cada modo con bots acelerados ─────────────
async function playMode(mode, { bots = 3, points = 3, shot }) {
  await page.click('text=JUGAR');
  await page.waitForTimeout(200);
  await page.evaluate(() => { window.__ui.autopilot = true; });
  await page.click(`#s-mode [data-v="${mode}"]`);
  if (mode !== 'retoDelDia') {
    await page.click(`#s-bots [data-v="${bots}"]`);
    if (mode !== 'laReta') await page.click(`#s-points [data-v="${points}"]`);
  }
  await page.click('#btn-start');
  await page.evaluate(() => { window.__game.debugSpeed = 8; });
  let done = false;
  const t0 = Date.now();
  for (let i = 0; i < 240 && !done; i++) {
    await page.waitForTimeout(1000);
    done = await page.evaluate(() => !document.getElementById('scr-results').hidden);
    if (i === 4) await page.screenshot({ path: `${OUT}/${shot}-juego.png` });
  }
  await page.screenshot({ path: `${OUT}/${shot}-resultados.png` });
  const info = await page.evaluate(() => {
    const g = window.__game;
    return { mode: g.modeId, rounds: g.round, events: g.eventCounts, durations: g.roundDurations, scores: g.players.map((p) => `${p.name}:${p.score}/${p.kos}ko`), title: document.getElementById('r-title').textContent };
  });
  console.log(`  ${mode}: ${((Date.now() - t0) / 1000).toFixed(0)} s reales ·`, JSON.stringify(info));
  check(done, `modo ${mode} terminó (${info.rounds} rondas, "${info.title}")`);
  await page.evaluate(() => { window.__game.debugSpeed = 1; window.__ui.autopilot = false; });
  await page.click('#btn-results-menu');
  await page.waitForTimeout(300);
  return info;
}

const gz = await playMode('gorrazo', { bots: 3, points: 3, shot: '07-gorrazo' });
check((gz.events.KO || 0) > 0, `¡Gorrazo!: hubo KOs (${gz.events.KO})`);
const lc = await playMode('lataCaliente', { bots: 3, points: 3, shot: '08-lata' });
check((lc.events.HotCanStart || 0) > 0 && (lc.events.HotCanExplode || 0) > 0, `Lata Caliente: se pegó (${lc.events.HotCanStart}), pasó (${lc.events.HotCanPass || 0}) y explotó (${lc.events.HotCanExplode})`);
const rt = await playMode('laReta', { bots: 3, shot: '09-reta' });
check(rt.rounds >= 3 && rt.rounds <= 10, `La Reta terminó en ${rt.rounds} rondas (3 seguidas o 10 rondas)`);
const rd = await playMode('retoDelDia', { shot: '10-reto' });
const reto = await page.evaluate(() => JSON.parse(localStorage.getItem('gorrazo.retoDelDia.v1') || '{}'));
const today = await page.evaluate(() => { const d = new Date(); return String(d.getFullYear() * 10000 + (d.getMonth() + 1) * 100 + d.getDate()); });
check(reto[today] != null, `Reto del Día: mejor puntaje de hoy guardado (${today}: ${reto[today]})`);
check(rd.events && rd.mode === 'retoDelDia', 'Reto del Día con semilla del día');

const p2 = await page.evaluate(() => JSON.parse(localStorage.getItem('arena-inkcap.progress.v1')));
check(p2.stats.matches >= 4, `progreso: ${p2.stats.matches} partidas, nivel ${p2.level}, ${p2.coins} Gotas`);

console.log('errores de consola:', errors.length ? errors : 'ninguno');
await browser.close();
if (errors.length || fails.length) { console.log('FALLAS:', fails); process.exit(1); }
console.log('TODO BIEN');
