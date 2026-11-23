// Mide duración de rondas y KOs con partidas solo de bots (para balancear).
import { createRequire } from 'node:module';
import { pathToFileURL } from 'node:url';
import { resolve } from 'node:path';
const require = createRequire(import.meta.url);
let pw; try { pw = require('playwright'); } catch { pw = require('/opt/node-tools/node_modules/playwright'); }
const browser = await pw.chromium.launch({ args: ['--use-gl=angle', '--use-angle=swiftshader', '--enable-unsafe-swiftshader'] });
const page = await browser.newPage({ viewport: { width: 640, height: 360 } });
const all = [];
for (const arena of ['callejon', 'museo', 'azotea']) {
  await page.goto(pathToFileURL(resolve('dist/index.html')).href);
  await page.waitForTimeout(800);
  await page.evaluate((arena) => {
    const g = window.__game; g.debugSpeed = 8;
    const slots = Array.from({ length: 4 }, (_, i) => ({ type: 'bot', name: `B${i + 1}`, difficulty: 'normal', species: i }));
    window.__ui.show(null); g.startMatch({ slots, arenaId: arena, pointsToWin: 5 });
  }, arena);
  for (let i = 0; i < 150; i++) {
    await page.waitForTimeout(1000);
    if (await page.evaluate(() => window.__game.state === 'matchEnd')) break;
  }
  const r = await page.evaluate(() => ({ d: window.__game.roundDurations || [], kos: window.__game.players.map((p) => p.kos), state: window.__game.state }));
  console.log(arena, JSON.stringify(r));
  all.push(...r.d);
}
const avg = all.reduce((a, b) => a + b, 0) / all.length;
console.log(`rondas: ${all.length}, promedio ${avg.toFixed(1)} s, min ${Math.min(...all)}, max ${Math.max(...all)}`);
await browser.close();
