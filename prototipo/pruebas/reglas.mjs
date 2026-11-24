// Pruebas de reglas (spec v0.2 §A, §B, §C y spec v0.3 §A, §B, §E): escenarios
// armados a mano que avanzan la simulación tick por tick (sin depender de los
// bots) para revisar que el prototipo web se comporte igual que lo que pide la
// spec / el núcleo C#. v0.3: vacíos (caída, dash, crédito, gorras, apariciones,
// bots, muerte súbita), caja DROP, las 4 arenas y el Reto del Día.
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
    events: [],
    start(n = 3, opts = {}) {
      g.debugSpeed = 0; // el bucle de render no avanza la simulación: la avanzamos a mano
      const slots = Array.from({ length: n }, (_, i) => ({ type: opts.bots ? 'bot' : 'human', name: 'ABCDEFGH'[i] }));
      this.events = [];
      g.hooks.onEvent = (type, data) => this.events.push({ type, ...data });
      g.startMatch({ slots, arenaId: 'callejon', seed: 7, ...opts });
      // cuenta regresiva (2.2 s): se avanza justo hasta el "¡YA!" (playT ≈ 0)
      for (let i = 0; i < 200 && g.state === 'countdown'; i++) this.ticks(1);
      for (const p of g.players) p.spawnInvuln = 0;
      g.pickupTimer = 999;
      return g.players;
    },
    ev(type) { return this.events.filter((e) => e.type === type); },
    floaters() { return document.getElementById('floaters').textContent; },
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
  T.place(A, -3, 1.5); T.place(B, 6, 5.5); T.place(C, 9, -6.5);
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
  T.place(A, 4, 0); T.place(B, 5.5, 0); T.place(C, 4, 3.5);
  A.hasCap = false; A.cap.launch(4, 0, 1e-4, 0, 9, 'tinta');
  g.startPuddle(A.cap, 4, 0);
  T.ticks(12);
  const mid = g.puddles[0] && g._puddleRadius(g.puddles[0]);
  const aliveMid = B.alive;
  T.ticks(13);
  return { ok: Math.abs(mid - 1.0) < 0.06 && aliveMid && !B.alive && A.alive && C.alive && g.eventCounts.PuddleStart === 1 && g.eventCounts.Explode === 1, info: `radio a 0.2 s = ${mid && mid.toFixed(2)} m` };
});

await test('B Charco: al chocar con la pared deja el charco', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, 9.5, 0); T.place(B, -8, 2); T.place(C, -8, -2);
  A.power = 'tinta'; A.hasCap = false;
  A.cap.launch(11, 0, 15, 0, 2, 'tinta');
  T.ticks(10);
  return { ok: (g.eventCounts.PuddleStart || 0) === 1 && A.cap.power === null };
});

await test('B Muro de Tag: 2.5 × 0.4 m perpendicular, bloquea 3 s', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, -8, -5.5); T.place(B, 6, 5.5); T.place(C, 9, -6.5);
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
  T.place(A, -6, 1.5); T.place(B, 3, 1.5);
  A.hasCap = false; A.cap.launch(-5, 1.5, 14, 0, 0.6, 'tatuaje');
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


// ═════════════════════════════════════════════════════════════
// Spec v0.3 · escenarios, vacíos y caja DROP
// ═════════════════════════════════════════════════════════════
await test('B las 4 arenas con las medidas de la spec (26×16 ×3 y 24×15) y sus vacíos', () => {
  const g = T.g; const out = [];
  const want = { callejon: [26, 16, 7, 4], museo: [26, 16, 5, 2], azotea: [26, 16, 6, 2], popup: [24, 15, 5, 2] };
  let ok = true;
  for (const [id, [w, h, no, nv]] of Object.entries(want)) {
    T.start(2, { arenaId: id });
    const a = g.arena;
    const good = a.def.id === id && a.w === w && a.h === h && a.obstacles.length === no && a.voids.length === nv;
    ok = ok && good; out.push(`${id} ${a.w}×${a.h} obst ${a.obstacles.length} vacíos ${a.voids.length}`);
  }
  const v = g.arena.voids.map((x) => [x.x, x.z, x.w, x.d].join(' ')).join(' | ');
  const sp = g.arena.def.spawns.map((x) => x.join(' ')).join(' | ');
  ok = ok && v === '-9.6 4.6 1.6 2.2 | 9.6 -4.6 1.6 2.2' && sp === '-9.5 -5.5 | 9.5 5.5 | 6.5 -5.5 | -6.5 5.5 | 0 -5.8 | 0 5.8';
  return { ok, info: out.join(' · ') };
});

await test('V1 caer al vacío: el centro dentro (sin dash ni empujón) = KO "Vacio", letrero ¡AL VACÍO!, sin crédito', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, -5.6, 4.9); T.place(B, 8, 0); T.place(C, 8, 5.5);
  T.ticks(1);
  const ko = T.ev('KO')[0];
  return { ok: !A.alive && B.alive && ko && ko.cause === 'vacio' && ko.killer === -1 && ko.victim === 0 && B.kos === 0 && T.floaters().includes('¡AL VACÍO!'), info: JSON.stringify(ko) };
});

await test('V1 el vacío cuenta reducido 0.15 m por lado: en la orilla no caes, 0.2 m adentro sí', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  // coladera (-5.6, 4.9, 1.4, 1.4): orilla izquierda en x = -6.3
  T.place(A, -6.3 + 0.10, 4.9); T.place(B, -6.3 + 0.20, 6.9); T.place(C, 8, 0);
  T.ticks(3);
  const edgeAlive = A.alive;
  T.place(B, -6.3 + 0.20, 4.9); T.ticks(1);
  return { ok: edgeAlive && !B.alive && A.alive, info: `a 0.10 m: vivo ${edgeAlive}; a 0.20 m: vivo ${B.alive}` };
});

await test('V1 un dash cruza un vacío; si el dash termina encima, cae', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  // A cruza la zanja (0, 3.0, 3.4 × 0.9) de lado a lado; B termina su dash sobre la coladera
  T.place(A, 0.9, 1.4, 0, 1); T.place(B, -8.3, 4.9, 1, 0); T.place(C, 9, 0);
  T.pad(A, { dashPressed: true, mx: 0, mz: 1 }); T.pad(B, { dashPressed: true, mx: 1, mz: 0 });
  g.step(1 / 60, true);
  let overA = false, overB = false;
  for (let i = 0; i < 12; i++) {
    T.ticks(1);
    if (g.arena.voidAt(A.pos.x, A.pos.z) && A.alive && A.dashT > 0) overA = true;
    if (g.arena.voidAt(B.pos.x, B.pos.z) && B.alive && B.dashT > 0) overB = true;
  }
  const ko = T.ev('KO');
  return { ok: overA && A.alive && A.pos.z > 3.45 && overB && !B.alive && ko.length === 1 && ko[0].cause === 'vacio', info: `A pasó por encima ${overA} y quedó en z=${A.pos.z.toFixed(2)} vivo ${A.alive}; B sobre la coladera en dash ${overB}, vivo ${B.alive}` };
});

await test('V2 crédito por empujón: quien te empuja al vacío se lleva el KO', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, -8.9, 4.9, 1, 0); T.place(B, -7.9, 4.9); T.place(C, 9, 0);
  A.hasCap = false; A.cap.state = 'ground'; A.cap.pos = { x: 9, z: -6.5 }; A.cap.t = 0;
  T.pad(A, { meleePressed: true, aim: true, ax: 1, az: 0 }); g.step(1 / 60, true);
  const pushed = B.pushT > 0;
  let aliveWhilePushed = true;
  for (let i = 0; i < 20; i++) { T.ticks(1); if (B.pushT > 1e-6 && !B.alive) aliveWhilePushed = false; }
  const ko = T.ev('KO')[0];
  return { ok: pushed && aliveWhilePushed && !B.alive && ko && ko.cause === 'vacio' && ko.killer === 0 && A.kos === 1, info: `KO ${JSON.stringify(ko)} · KOs de A: ${A.kos}` };
});

await test('V2 el crédito dura 2 s: después, la caída no es de nadie', () => {
  const run = (wait) => {
    const [A, B, C] = T.start(3); const g = T.g;
    T.place(A, 3, 1.5, 1, 0); T.place(B, 4, 1.5); T.place(C, 9, 6);
    A.hasCap = false; A.cap.state = 'ground'; A.cap.pos = { x: 9, z: -6.5 }; A.cap.t = 0;
    T.pad(A, { meleePressed: true, aim: true, ax: 1, az: 0 }); g.step(1 / 60, true);
    T.ticks(wait - 1);
    A.cap.t = 0; // (que no regrese sola a media prueba)
    T.place(B, 5.6, -4.9); T.ticks(1);
    return T.ev('KO')[0];
  };
  const dentro = run(115), fuera = run(125); // 1.92 s y 2.08 s después del empujón
  return { ok: dentro && dentro.killer === 0 && fuera && fuera.killer === -1 && fuera.cause === 'vacio', info: `a 1.92 s: killer ${dentro && dentro.killer} · a 2.08 s: killer ${fuera && fuera.killer}` };
});

await test('V2 también dan crédito: desviar su gorra y romperle el escudo con una gorra', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, -2, 1.5, 1, 0); T.place(B, 4, 1.5, -1, 0); T.place(C, 9, 6);
  B.hasCap = false; B.cap.launch(-0.5, 1.5, -15, 0, 2, null);
  T.pad(A, { meleePressed: true, aim: true, ax: 1, az: 0 }); g.step(1 / 60, true);
  const desvio = B.lastHitBy === A;
  // C con escudo recibe la gorra de A
  const [A2, B2, C2] = T.start(3);
  T.place(A2, -2, 1.5, 1, 0); T.place(B2, 9, 6); T.place(C2, 3, 1.5);
  C2.shield = true; A2.hasCap = false; A2.cap.launch(2, 1.5, 15, 0, 2, null);
  T.ticks(3);
  return { ok: desvio && C2.alive && !C2.shield && C2.lastHitBy === A2, info: `desvío ${desvio}, escudo roto con crédito ${C2.lastHitBy === A2}` };
});

await test('V3 las gorras en vuelo pasan por encima de los vacíos', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, -8, 1.2); T.place(B, 9, 6); T.place(C, 9, -6.5);
  // cruza la zanja (0, 3.0) a lo largo, por el centro
  A.hasCap = false; A.cap.launch(-3, 3.0, 16, 0, 2, null);
  let over = false;
  for (let i = 0; i < 20; i++) { T.ticks(1); if (g.arena.voidAt(A.cap.pos.x, A.cap.pos.z, 0)) over = over || A.cap.state === 'out'; }
  return { ok: over && A.cap.state === 'out' && A.cap.pos.x > 1.7 && (T.ev('CapVoidReturn').length === 0), info: `estado ${A.cap.state}, x=${A.cap.pos.x.toFixed(2)}` };
});

await test('V3 una gorra en el piso con el CENTRO dentro del rectángulo completo regresa sola (sin la reducción de 0.15)', () => {
  const [A, B, C] = T.start(3); const g = T.g;
  T.place(A, -8, 1.2); T.place(B, -8, -1.2); T.place(C, 9, 6);
  // A: 0.05 m dentro de la orilla (zona que NO tira jugadores). B: 0.05 m afuera.
  A.hasCap = false; A.cap.state = 'ground'; A.cap.pos = { x: -6.3 + 0.05, z: 4.9 }; A.cap.vel = { x: 0, z: 0 }; A.cap.t = 0; A.cap.mesh.visible = true;
  B.hasCap = false; B.cap.state = 'ground'; B.cap.pos = { x: -6.3 - 0.05, z: 4.9 }; B.cap.vel = { x: 0, z: 0 }; B.cap.t = 0; B.cap.mesh.visible = true;
  T.ticks(1);
  const stA = A.cap.state, stB = B.cap.state;
  T.ticks(60);
  // si el dueño ya no está, la gorra se va al fondo
  g.kill(C, null, 'zap'); C.hasCap = false; C.cap.state = 'ground'; C.cap.pos = { x: 5.6, z: -4.9 }; C.cap.dead = false; C.cap.t = 0;
  T.ticks(1);
  return { ok: stA === 'recall' && stB === 'ground' && A.hasCap && C.cap.dead && T.ev('CapVoidReturn').length === 2, info: `dentro: ${stA}, afuera: ${stB}, A la recuperó ${A.hasCap}` };
});

await test('V4 nada aparece en un vacío ni a menos de 0.8 m: spawns y 500 puntos al azar por arena', () => {
  const g = T.g; const out = []; let ok = true;
  for (const id of ['callejon', 'museo', 'azotea', 'popup']) {
    T.start(2, { arenaId: id });
    const a = g.arena;
    let bad = 0, minD = 99;
    for (let i = 0; i < 500; i++) {
      const p = a.randomFreePoint(1.0);
      const n = a.voidNear(p.x, p.z, 50);
      if (n) minD = Math.min(minD, n.d);
      if (a.voidAt(p.x, p.z, 0) || a.voidNear(p.x, p.z, 0.8) || !a.isFree(p.x, p.z, 1.0)) bad++;
    }
    let badSpawns = 0;
    for (const s of a.def.spawns) if (a.voidNear(s[0], s[1], 0.8) || !a.isFree(s[0], s[1], 0.55)) badSpawns++;
    ok = ok && bad === 0 && badSpawns === 0;
    out.push(`${id}: ${bad} malos, spawns malos ${badSpawns}, más cerca ${minD.toFixed(2)} m`);
  }
  return { ok, info: out.join(' · ') };
});

await test('V5 bots: no hacen dash hacia un vacío, no caminan hacia él y se alejan de la orilla', () => {
  const [A, B] = T.start(2, { bots: true }); const g = T.g;
  // A a 0.5 m de la orilla izquierda de la coladera (-6.3), con el vacío a su derecha
  T.place(A, -6.8, 4.9); T.place(B, 9, -6.5);
  const bot = A.controller;
  const dashHacia = bot._dashSafe(1, 0), dashLejos = bot._dashSafe(-0.6, -0.8);
  const pad = { mx: 0, mz: 0, dashPressed: true };
  bot._steer(pad, 1, 0); // "quiere" caminar y hacer dash hacia el vacío
  const quieto = { mx: 0, mz: 0, dashPressed: false };
  bot._steer(quieto, 0, 0); // sin intención: debe alejarse de la orilla
  // si conserva el dash, es porque ya lo redirigió a un lado seguro
  const l = Math.hypot(pad.mx, pad.mz) || 1;
  const dashOk = !pad.dashPressed || (pad.mx <= 0.05 && bot._dashSafe(pad.mx / l, pad.mz / l));
  return { ok: !dashHacia && dashLejos && pad.mx <= 0.05 && dashOk && quieto.mx < -0.3, info: `dash hacia el vacío permitido ${dashHacia}; paso mx=${pad.mx.toFixed(2)} (dash ${pad.dashPressed}); se aleja mx=${quieto.mx.toFixed(2)}` };
});

await test('V5 bots: en 20 s parados junto a los vacíos (sin rivales cerca) ninguno se cae solo', () => {
  const ps = T.start(4, { bots: true, rules: { suddenDeath: false } }); const g = T.g;
  T.place(ps[0], -6.9, 4.9); T.place(ps[1], 6.9, -4.9); T.place(ps[2], 0, 2.0); T.place(ps[3], 0, -2.0);
  g.fastForward(60 * 20);
  const solos = T.ev('KO').filter((k) => k.cause === 'vacio' && k.killer === -1).length;
  return { ok: solos === 0, info: `KOs: ${T.ev('KO').map((k) => k.cause).join(', ') || 'ninguno'} · caídas sin crédito: ${solos}` };
});

await test('V6 muerte súbita: el muro que avanza puede empujarte a un vacío', () => {
  const [A, B, C] = T.start(3, { arenaId: 'museo' }); const g = T.g;
  g.arena.hazards = []; // sin láser en esta prueba
  // foso (-9.2, 0, 1.8 × 3.2): A entre el muro izquierdo y el foso
  T.place(A, -10.9, 0); T.place(B, 0, 4); T.place(C, 0, -4);
  g.playT = 24; T.ticks(1);
  const antes = A.alive, x1 = A.pos.x;
  g.playT = 26.4; T.ticks(2);
  const ko = T.ev('KO')[0];
  return { ok: antes && !A.alive && ko.cause === 'vacio' && ko.killer === -1, info: `muro en ${(-13 + g.arena.inset).toFixed(2)}, A estaba en ${x1.toFixed(2)}, KO ${JSON.stringify(ko)}` };
});

await test('V1 ya declarada la ronda nadie cae: el vacío se vuelve sólido para el ganador', () => {
  const [A, B] = T.start(2); const g = T.g;
  // A hace un dash que termina sobre la coladera, pero la ronda se declara (6 ticks) antes de que acabe
  T.place(A, -8.3, 4.9, 1, 0); T.place(B, 9, 0);
  g.kill(B, null, 'zap');
  T.pad(A, { dashPressed: true, mx: 1, mz: 0 }); g.step(1 / 60, true);
  T.ticks(5);
  const declarada = g.state === 'roundEnd' && g.roundWinner === A && A.dashT > 0;
  T.ticks(30);
  return { ok: declarada && A.alive && !g.arena.voidAt(A.pos.x, A.pos.z, 0) && T.ev('KO').length === 1, info: `declarada en pleno dash ${declarada}; A vivo ${A.alive} en x=${A.pos.x.toFixed(2)} (vacío de -6.30 a -4.90)` };
});

await test('B4 caja DROP: aviso a los 6.5 s, CAE a los 8 s, noquea a ≤ 1.1 m (¡DROP!), el escudo bloquea y deja una lata', () => {
  const [A, B, C, D] = T.start(4, { arenaId: 'popup', rules: { suddenDeath: false } }); const g = T.g;
  T.ticks(60 * 6.4);
  const sinAviso = T.ev('DropWarn').length === 0 && !g.drop;
  T.ticks(8);
  const w = T.ev('DropWarn')[0];
  if (!w) return { ok: false, info: 'no hubo aviso' };
  const tAviso = +(g.drop.warnAt).toFixed(2);
  const libre = g.arena.isFree(w.x, w.z, 1.0) && !g.arena.voidNear(w.x, w.z, 0.8);
  // A a 1.0 m (cae), B a 1.2 m (se salva), C con escudo a 0.6 m (bloquea), D lejos
  T.place(A, w.x + 1.0, w.z); T.place(B, w.x - 1.2, w.z); T.place(C, w.x, w.z + 0.6); T.place(D, -w.x, -w.z);
  C.shield = true;
  let tCae = null;
  for (let i = 0; i < 120 && tCae === null; i++) {
    T.place(A, w.x + 1.0, w.z); T.place(B, w.x - 1.2, w.z); T.place(C, w.x, w.z + 0.6);
    T.ticks(1);
    if (T.ev('DropLand').length) tCae = +g.playT.toFixed(2);
  }
  const ko = T.ev('KO');
  const lata = g.pickups.find((p) => Math.abs(p.pos.x - w.x) < 1e-9 && Math.abs(p.pos.z - w.z) < 1e-9);
  const ok = sinAviso && libre && Math.abs(tAviso - 6.5) < 0.02 && Math.abs(tCae - 8.0) < 0.02
    && !A.alive && B.alive && C.alive && !C.shield && D.alive && ko.length === 1 && ko[0].cause === 'drop' && ko[0].killer === -1
    && !!lata && T.floaters().includes('¡DROP!') && !g.drop;
  return { ok, info: `aviso ${tAviso} s en (${w.x.toFixed(2)}, ${w.z.toFixed(2)}), cae ${tCae} s, KO ${JSON.stringify(ko)}, lata ${lata && lata.type}` };
});

await test('B4 caja DROP: la siguiente cae a los 28 s (aviso a los 26.5 s); en Lata Caliente cae pero no deja lata', () => {
  const ps = T.start(3, { arenaId: 'popup', rules: { suddenDeath: false } }); const g = T.g;
  for (const p of ps) T.place(p, p.pos.x, p.pos.z);
  T.ticks(60 * 9); g.pickups = []; // ya cayó la primera (nadie cerca)
  const una = T.ev('DropLand').length === 1;
  // los jugadores se quitan de donde va a caer la segunda
  let t2 = null, w2 = null;
  for (let i = 0; i < 60 * 21 && t2 === null; i++) {
    T.ticks(1);
    const w = T.ev('DropWarn')[1];
    if (w && !w2) { w2 = +g.playT.toFixed(2); ps.forEach((p, k) => T.place(p, -w.x || 5, k * 2 - 2 - (w.z > 0 ? 3 : -3))); }
    if (T.ev('DropLand').length === 2) t2 = +g.playT.toFixed(2);
  }
  const hot = T.start(4, { arenaId: 'popup', mode: 'lataCaliente', rules: { suddenDeath: false } });
  T.ticks(60 * 6.6);
  const w = T.ev('DropWarn')[0];
  hot.forEach((p, k) => T.place(p, w.x > 0 ? -6 - k : 6 + k, w.z > 0 ? -5.5 : 5.5));
  for (let i = 0; i < 100 && !T.ev('DropLand').length; i++) T.ticks(1);
  const sinLata = T.ev('DropLand').length === 1 && g.pickups.length === 0;
  return { ok: una && Math.abs(w2 - 26.5) < 0.02 && Math.abs(t2 - 28) < 0.02 && sinLata, info: `2.º aviso ${w2} s, 2.ª caída ${t2} s · Lata Caliente: cayó sin lata ${sinLata}` };
});

await test('B los otros escenarios no tienen caja DROP', () => {
  const g = T.g; let ok = true;
  for (const id of ['callejon', 'museo', 'azotea']) { T.start(2, { arenaId: id, rules: { suddenDeath: false } }); g.arena.hazards = []; T.ticks(60 * 9); ok = ok && !g.arena.dropDef && T.ev('DropWarn').length === 0; }
  return { ok };
});

await test('E3 Reto del Día: arena por semilla entre las 4 (20261123 → museo, 20261201 → azotea, 20270115 → popup, 20260228 → azotea)', () => {
  const got = [20261123, 20261201, 20270115, 20260228].map((s) => `${s}:${window.__retoDeSemilla(s).arena}`);
  return { ok: got.join() === '20261123:museo,20261201:azotea,20270115:popup,20260228:azotea', info: got.join(' · ') };
});

await test('C "Aleatorio" incluye los 4 escenarios (y con la misma semilla sale el mismo)', () => {
  const g = T.g; const seen = new Set(); let same = true;
  for (let seed = 1; seed <= 40; seed++) {
    T.start(2, { arenaId: 'aleatorio', seed });
    const id = g.arena.def.id; seen.add(id);
    if (seed <= 3) { T.start(2, { arenaId: 'aleatorio', seed }); same = same && g.arena.def.id === id; }
  }
  return { ok: seen.size === 4 && same, info: [...seen].join(', ') };
});

await test('D determinismo en la Pop-up Store: misma semilla → mismos eventos (bots, caja DROP incluida)', () => {
  const run = () => {
    T.start(4, { arenaId: 'popup', bots: true, seed: 4242 });
    T.g.fastForward(60 * 30);
    return T.events.map((e) => `${e.type}:${e.victim ?? ''}:${e.cause ?? ''}:${e.x != null ? e.x.toFixed(3) : ''}`).join('|');
  };
  const a = run(), b = run();
  return { ok: a === b && a.includes('DropWarn') && a.includes('DropLand'), info: `${a.split('|').length} eventos` };
});

const fails = results.filter((r) => !r.ok);
console.log(`\n${results.length - fails.length}/${results.length} pruebas de reglas OK · errores de consola: ${errors.length ? errors : 'ninguno'}`);
await browser.close();
if (fails.length || errors.length) process.exit(1);
