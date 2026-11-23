// Pruebas de reglas (spec v0.2 §A, §B, §C): escenarios armados a mano que
// avanzan la simulación tick por tick (sin depender de los bots) para revisar
// que el prototipo web se comporte igual que lo que pide la spec / el núcleo C#.
import { createRequire } from 'node:module';
import { pathToFileURL } from 'node:url';
import { resolve } from 'node:path';
const require = createRequire(import.meta.url);
let pw; try { pw = require('playwright'); } catch { pw = require('/opt/node-tools/node_modules/playwright'); }

const browser = await pw.chromium.launch({ args: ['--use-gl=angle', '--use-angle=swiftshader', '--enable-unsafe-swiftshader'] });
const page = await browser.newPage({ viewport: { width: 800, height: 450 } });
const errors = [];
page.on('pageerror', (e) => errors.push(String(e)));
page.on('console', (m) => { if (m.type() === 'error') errors.push(m.text()); });
await page.goto(pathToFileURL(resolve('dist/index.html')).href);
await page.waitForTimeout(1500);

// utilidades dentro de la página
await page.evaluate(() => {
  const g = window.__game;
  window.__ui.show(null);
  window.T = {
    g,
    start(n = 3, opts = {}) {
      g.debugSpeed = 0; // el bucle de render no avanza la simulación: la avanzamos a mano
      const slots = Array.from({ length: n }, (_, i) => ({ type: 'human', name: 'ABCDEFGH'[i] }));
      g.startMatch({ slots, arenaId: 'callejon', seed: 7, ...opts });
      this.ticks(140); // cuenta regresiva
      for (const p of g.players) p.spawnInvuln = 0;
      g.pickupTimer = 999;
      return g.players;
    },
    pad(p, patch) { p.pad = { mx: 0, mz: 0, ax: 0, az: 0, aim: false, throwHeld: false, throwPressed: false, throwReleased: false, meleePressed: false, dashPressed: false, ...patch }; },
    ticks(n) {
      for (let i = 0; i < n; i++) {
        g.step(1 / 60, true);
        for (const p of g.players) this.pad(p, {});
      }
    },
    place(p, x, z, fx = 1, fz = 0) { p.pos.x = x; p.pos.z = z; p.vel = { x: 0, z: 0 }; p.facing = { x: fx, z: fz }; p.aimDir = { x: fx, z: fz }; },
  };
});

const results = [];
async function test(name, fn) {
  try {
    const r = await page.evaluate(fn);
    const ok = r === true || (r && r.ok);
    results.push({ name, ok, info: r && r.info });
    console.log(`${ok ? 'OK  ' : 'FALLA'} ${name}${r && r.info ? ` · ${r.info}` : ''}`);
  } catch (e) {
    results.push({ name, ok: false });
    console.log(`FALLA ${name} · ${e.message.split('\n')[0]}`);
  }
}

await test('Rng igual al de C# (xorshift32, semilla 20261123)', () => {
  const r = new T.g.rng.constructor(20261123);
  const v = [r.nextUInt(), r.nextUInt(), r.nextUInt()];
  return { ok: v.join() === '649493033,3616994512,680721046', info: v.join(', ') };
});

await test('A1 doble KO (< 6 ticks) = empate', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, -6, 0); T.place(B, 0, 4); T.place(C, 6, 0);
  g.kill(B, A, 'zaz'); g.kill(A, C, 'zaz'); T.ticks(3); g.kill(C, null, 'zap'); T.ticks(6);
  return { ok: g.state === 'roundEnd' && g.roundWinner === null && g.roundEndReason === 'doble', info: `estado ${g.state}, ganador ${g.roundWinner && g.roundWinner.name}` };
});

await test('A1 KO separado (≥ 6 ticks) = gana el último', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, -6, 0); T.place(B, 0, 4); T.place(C, 6, 0);
  g.kill(B, A, 'zaz'); g.kill(A, C, 'zaz'); T.ticks(5);
  const before = g.state;
  T.ticks(1);
  return { ok: before === 'play' && g.state === 'roundEnd' && g.roundWinner === C, info: `antes ${before}, después ${g.state}, ganador ${g.roundWinner && g.roundWinner.name}` };
});

await test('A1 ya declarada la ronda nadie más cae: el ganador conserva su punto (igual que C#)', () => {
  const [A, B] = T.start(2); const g = T.g;
  T.place(A, -6, 0); T.place(B, 6, 0);
  g.kill(B, A, 'zaz'); T.ticks(6);
  const declared = g.state === 'roundEnd' && g.roundWinner === A;
  g.kill(A, null, 'zap');
  return { ok: declared && A.alive, info: `declarada ${declared}, A vivo ${A.alive}` };
});

await test('A2 muerte súbita: aviso a 20 s, muros a 0.5 m/s, empujan, empate a 45 s', () => {
  const [A, B] = T.start(2); const g = T.g;
  T.place(A, -10.5, 0); T.place(B, 0, 4);
  g.playT = 19.99; T.ticks(2);
  const warned = (g.eventCounts.SuddenDeathWarn || 0) === 1;
  g.playT = 25; T.ticks(1);
  const inset = g.arena.inset; // (25 - 21) × 0.5 ≈ 2 m
  const pushed = A.pos.x >= -(g.arena.w / 2 - inset) + 0.5 && A.alive;
  g.playT = 60; T.ticks(1);
  const maxInset = (g.arena.w - 8) / 2;
  const capped = Math.abs(g.arena.inset - maxInset) < 1e-6;
  return { ok: warned && Math.abs(inset - 2.0083) < 0.02 && pushed && capped && g.state === 'roundEnd' && g.roundWinner === null && g.roundEndReason === 'tiempo', info: `inset ${inset.toFixed(3)} m, A.x ${A.pos.x.toFixed(2)}, tope ${g.arena.inset.toFixed(2)}, ${g.state}/${g.roundEndReason}` };
});

await test('A2 apagada: sin muros ni empate', () => {
  T.start(2, { rules: { suddenDeath: false } }); const g = T.g;
  g.playT = 50; T.ticks(2);
  return { ok: g.arena.inset === 0 && g.state === 'play' };
});

await test('A3 empujón sin gorra: 2.5 m en 0.2 s, no noquea, recarga 0.8 s', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, -3, 1.5); T.place(B, -2, 1.5); T.place(C, 6, 5);
  A.hasCap = false; A.cap.state = 'ground'; A.cap.pos = { x: -8, z: -5 }; A.cap.t = 0;
  T.pad(A, { meleePressed: true }); g.step(1 / 60, true);
  T.ticks(20);
  const moved = B.pos.x - (-2);
  return { ok: B.alive && Math.abs(moved - 2.5) < 0.35 && Math.abs(A.shoveCd - (0.8 - 21 / 60)) < 0.02, info: `B se movió ${moved.toFixed(2)} m` };
});

await test('A3 empujón patea 4 m una gorra ajena caída', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, -3, 1.5); T.place(B, 6, 5); T.place(C, 6, -5);
  A.hasCap = false; A.cap.state = 'ground'; A.cap.pos = { x: -9, z: -5 }; A.cap.t = 0;
  B.hasCap = false; B.cap.state = 'ground'; B.cap.pos = { x: -2, z: 1.5 }; B.cap.vel = { x: 0, z: 0 }; B.cap.t = 0; B.cap.mesh.visible = true;
  T.pad(A, { meleePressed: true }); g.step(1 / 60, true);
  T.ticks(90);
  const d = B.cap.pos.x - (-2);
  return { ok: Math.abs(d - 4) < 0.4, info: `la gorra recorrió ${d.toFixed(2)} m` };
});

await test('A4 "Te la regreso": la gorra desviada noquea a su dueño (crédito a quien desvió)', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, -2, 1.5, 1, 0); T.place(B, 4, 1.5, -1, 0); T.place(C, 6, 5);
  B.hasCap = false; B.cap.launch(-0.5, 1.5, -15, 0, 2, null);
  T.pad(A, { meleePressed: true, aim: true, ax: 1, az: 0 }); g.step(1 / 60, true);
  const st = B.cap.state;
  const sp = Math.hypot(B.cap.vel.x, B.cap.vel.z); // ≈ 15 × 1.2 (la ida ya frenó un tick)
  T.ticks(30);
  const ko = g.eventCounts.KO;
  return { ok: st === 'deflect' && sp > 17.8 && sp <= 18 && !B.alive && A.alive && A.kos === 1, info: `estado ${st}, vel ${sp.toFixed(1)} m/s, B vivo ${B.alive}, KOs ${ko}` };
});

await test('A4 apagada: la gorra desviada cae', () => {
  const [A, B, C] = T.start(3, { rules: { deflectReturn: false } }); const g = T.g;
  T.place(A, -2, 1.5, 1, 0); T.place(B, 4, 1.5, -1, 0); T.place(C, 6, 5);
  B.hasCap = false; B.cap.launch(-0.5, 1.5, -15, 0, 2, null);
  T.pad(A, { meleePressed: true, aim: true, ax: 1, az: 0 }); g.step(1 / 60, true);
  return { ok: B.cap.state === 'ground' };
});

await test('A5 Sticker XL: radio ×2 y velocidad ×0.9', () => {
  const [A] = T.start(2);
  A.cap.launch(0, 0, 20, 0, 0.5, 'sticker');
  return { ok: Math.abs(A.cap.v0 - 18) < 1e-6 && Math.abs(A.cap.radius - 0.76) < 1e-6 };
});

await test('A6 ¡CLANK! a 85 % de la suma de radios', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, -9, -5); T.place(B, 9, 5); T.place(C, 9, -5);
  A.hasCap = false; B.hasCap = false;
  A.cap.launch(0, 1.5, 1e-4, 0, 9, null); B.cap.launch(0.7, 1.5, -1e-4, 0, 9, null);
  g.step(1 / 60, true);
  const noClank = A.cap.state === 'out' && B.cap.state === 'out'; // 0.70 > 0.646
  B.cap.pos.x = 0.6; g.step(1 / 60, true);
  return { ok: noClank && A.cap.state === 'ground' && B.cap.state === 'ground' };
});

await test('A8 causas de KO por gorra', () => {
  const [A, B] = T.start(2); const g = T.g;
  const c = A.cap; const out = [];
  c.state = 'out'; c.bounced = false; c.ghost = false; out.push(g._capCause(c));
  c.state = 'return'; out.push(g._capCause(c));
  c.bounced = true; out.push(g._capCause(c));
  c.state = 'deflect'; out.push(g._capCause(c));
  c.ghost = true; out.push(g._capCause(c));
  c.ghost = false; c.state = 'held'; c.bounced = false;
  return { ok: out.join() === 'gorrazo,deRegreso,deBanda,teLaRegreso,tiraje', info: out.join(', ') };
});

await test('B Charco: crece a 2 m en 0.4 s y noquea a quien siga dentro (no al dueño)', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, 0, 0); T.place(B, 1.5, 0); T.place(C, 0, 3.5);
  A.hasCap = false; A.cap.launch(0, 0, 1e-4, 0, 9, 'tinta');
  g.startPuddle(A.cap, 0, 0);
  T.ticks(12);
  const mid = g.puddles[0] && g._puddleRadius(g.puddles[0]);
  const aliveMid = B.alive;
  T.ticks(13);
  return { ok: Math.abs(mid - 1.0) < 0.06 && aliveMid && !B.alive && A.alive && C.alive && g.eventCounts.PuddleStart === 1 && g.eventCounts.Explode === 1, info: `radio a 0.2 s = ${mid && mid.toFixed(2)} m` };
});

await test('B Charco: al chocar con la pared deja el charco', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, 8, 0); T.place(B, -8, 4); T.place(C, -8, -4);
  A.power = 'tinta'; A.hasCap = false;
  A.cap.launch(9.5, 0, 15, 0, 2, 'tinta');
  T.ticks(10);
  return { ok: (g.eventCounts.PuddleStart || 0) === 1 && A.cap.power === null };
});

await test('B Muro de Tag: 2.5 × 0.4 m perpendicular, bloquea 3 s', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, -8, -4); T.place(B, 6, 5); T.place(C, 6, -5);
  A.hasCap = false; A.cap.launch(-7, 2, 14, 0, 0.3, 'grafiti');
  T.ticks(19);
  const tw = g.arena.tagWalls[0];
  if (!tw) return { ok: false, info: 'no hay muro' };
  const len = Math.hypot(tw.bx - tw.ax, tw.bz - tw.az);
  const perp = Math.abs(tw.bx - tw.ax) < 1e-6; // vuela en X → muro a lo largo de Z
  // un jugador que camina contra el muro no lo atraviesa
  const wx = (tw.ax + tw.bx) / 2, wz = (tw.az + tw.bz) / 2;
  T.place(B, wx - 1.5, wz);
  for (let i = 0; i < 60; i++) { T.pad(B, { mx: 1 }); g.step(1 / 60, true); }
  const blocked = B.pos.x < wx - 0.2;
  T.ticks(180);
  return { ok: Math.abs(len - 2.5) < 1e-6 && perp && tw.half === 0.2 && blocked && g.arena.tagWalls.length === 0 && g.eventCounts.TagWall === 1, info: `largo ${len}, B.x ${B.pos.x.toFixed(2)} vs muro ${wx.toFixed(2)}` };
});

await test('B Muro de Tag apagado: rastro que alenta', () => {
  const [A] = T.start(2, { rules: { graffitiWall: false } }); const g = T.g;
  T.place(A, -8, -4);
  A.hasCap = false; A.cap.launch(-7, 2, 14, 0, 0.3, 'grafiti');
  T.ticks(15);
  return { ok: g.arena.tagWalls.length === 0 && g.paintPatches.length > 0 };
});

await test('B Tatuaje: evento Marked al empezar a perseguir', () => {
  const [A, B] = T.start(2); const g = T.g;
  T.place(A, -6, 0); T.place(B, 2, 3);
  A.hasCap = false; A.cap.launch(-5, 0, 14, 0, 0.6, 'tatuaje');
  T.ticks(3);
  return { ok: g.eventCounts.Marked === 1 && B.markedT > 0 };
});

await test('C Lata Caliente: se pega al azar, el golpe la pasa (no noquea), explota entre 10 y 20 s', () => {
  const ps = T.start(4, { mode: 'lataCaliente' }); const g = T.g;
  const h = g.hot && g.hot.holder;
  const fuseOk = g.hot && g.hot.max >= 10 && g.hot.max <= 20;
  const other = ps.find((p) => p !== h);
  T.place(h, -2, 0, 1, 0); T.place(other, -1, 0);
  for (const p of ps) if (p !== h && p !== other) T.place(p, 8, p.idx * 2 - 4);
  T.pad(h, { meleePressed: true, aim: true, ax: 1, az: 0 }); g.step(1 / 60, true);
  const passed = g.hot.holder === other && other.alive;
  const immune = h.hotImmune > 0;
  g.hot.fuse = 0.01; T.ticks(2);
  return { ok: !!h && fuseOk && passed && immune && !other.alive && g.eventCounts.HotCanExplode === 1 && g.hot && g.hot.holder !== other, info: `mecha ${g.hot && g.hot.max.toFixed(1)} s` };
});

await test('C La Reta: solo 2 pelean; ganador se queda, perdedor al final', () => {
  const ps = T.start(4, { mode: 'laReta' }); const g = T.g;
  const [A, B, C, D] = ps;
  const benchedOk = C.benched && D.benched && !C.alive && A.alive && B.alive;
  g.kill(B, A, 'zaz'); T.ticks(6);
  g.roundEndT = 0; g.step(1 / 60, true);
  const order = g.queue.map((p) => p.name).join('');
  return { ok: benchedOk && order === 'ACDB' && A.streak === 1, info: `fila ${order}` };
});

await test('C La Reta: 3 seguidas gana la partida', () => {
  const ps = T.start(4, { mode: 'laReta' }); const g = T.g;
  let ended = null;
  const hook = g.hooks.onMatchEnd; g.hooks.onMatchEnd = (r) => { ended = r; };
  for (let k = 0; k < 3; k++) {
    if (k > 0) { g.nextRound(); T.ticks(140); }
    const [a, b] = g.queue;
    g.kill(b, a, 'zaz'); T.ticks(6); g.roundEndT = 0; g.step(1 / 60, true);
  }
  g.hooks.onMatchEnd = hook;
  return { ok: ended && ended.winner === ps[0] && g.round === 3, info: ended ? `ganó ${ended.winner.name} en ${g.round}` : 'no terminó' };
});

const fails = results.filter((r) => !r.ok);
console.log(`\n${results.length - fails.length}/${results.length} pruebas de reglas OK · errores de consola: ${errors.length ? errors : 'ninguno'}`);
await browser.close();
if (fails.length || errors.length) process.exit(1);
