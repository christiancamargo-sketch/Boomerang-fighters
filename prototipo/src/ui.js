// ─────────────────────────────────────────────────────────────
// INTERFAZ: menú, preparación de partida, vestidor, premio diario,
// cómo se juega, HUD, pausa y resultados con XP.
// ─────────────────────────────────────────────────────────────
import { GAME_INFO, PLAYER_COLORS, POWERUPS, DIFFICULTY, PROGRESSION } from './config.js';
import { ARENAS } from './arena.js';
import { SPECIES } from './characters.js';
import { SHIRT_DESIGNS, shirtThumb } from './textures.js';
import {
  progress, xpNeeded, recordMatch, dailyStatus, claimDaily,
  buyShirt, equip, redeemCode, SPECIES_UNLOCK, playerId,
} from './progress.js';
import { sfx, initAudio, ensureMusic, setMusic, setSfx, audioState } from './audio.js';
import { KeyboardController, GamepadController, TouchController, connectedGamepads } from './input.js';

const $ = (sel, root = document) => root.querySelector(sel);
const isTouch = () => window.matchMedia('(pointer: coarse)').matches || 'ontouchstart' in window;

export class UI {
  constructor(game) {
    this.game = game;
    this.setup = { humans: 1, bots: 3, difficulty: 'normal', arena: 'callejon', points: 5 };
    this.touch = null;
    this.screens = ['menu', 'setup', 'wardrobe', 'daily', 'howto', 'results', 'pause'];
    this._bindGlobal();
    this.renderTitle();
    this.show('menu');
  }

  // ── Navegación ─────────────────────────────────────────────
  show(name) {
    for (const s of this.screens) { const el = $(`#scr-${s}`); if (el) el.hidden = s !== name; }
    const inMatch = name === null;
    $('#hud').hidden = !inMatch;
    $('#touch').hidden = !(inMatch && this.touch);
    this.current = name;
    if (name === 'menu') { this.renderMenu(); this.game.showMenu(this._outfit()); }
    if (name === 'setup') this.renderSetup();
    if (name === 'wardrobe') { this.renderWardrobe(); if (this.game.mode !== 'menu') this.game.showMenu(this._outfit()); }
    if (name === 'daily') this.renderDaily();
    if (name === 'howto') this.renderHowto();
  }

  _outfit() {
    const e = progress().equipped;
    return { shirt: e.shirt, species: e.species, arena: 'callejon' };
  }

  _bindGlobal() {
    document.addEventListener('click', (e) => {
      const b = e.target.closest('[data-go]');
      if (b) { initAudio(); ensureMusic(); sfx.ui(); this.show(b.dataset.go); }
    });
    $('#btn-pause').addEventListener('click', () => this.pause());
    $('#btn-music').addEventListener('click', () => { initAudio(); const on = !audioState().music; setMusic(on); this._audioBtns(); });
    $('#btn-sfx').addEventListener('click', () => { initAudio(); const on = !audioState().sfx; setSfx(on); this._audioBtns(); });
    window.addEventListener('keydown', (e) => {
      if (e.code === 'Escape' || e.code === 'KeyP') {
        if (this.current === null) this.pause();
        else if (this.current === 'pause') this.resume();
      }
    });
    document.addEventListener('visibilitychange', () => { if (document.hidden && this.current === null) this.pause(); });
    this._audioBtns();
  }

  _audioBtns() {
    const st = audioState();
    $('#btn-music').setAttribute('aria-pressed', String(st.music));
    $('#btn-music').classList.toggle('off', !st.music);
    $('#btn-sfx').setAttribute('aria-pressed', String(st.sfx));
    $('#btn-sfx').classList.toggle('off', !st.sfx);
  }

  renderTitle() {
    document.querySelectorAll('[data-title]').forEach((el) => { el.textContent = GAME_INFO.title; });
    document.querySelectorAll('[data-tagline]').forEach((el) => { el.textContent = GAME_INFO.tagline; });
    document.querySelectorAll('[data-version]').forEach((el) => { el.textContent = GAME_INFO.version; });
  }

  // ── Menú principal ─────────────────────────────────────────
  renderMenu() {
    const p = progress();
    $('#m-level').textContent = p.level;
    $('#m-xp').style.width = `${Math.round((p.xp / xpNeeded()) * 100)}%`;
    $('#m-xp-text').textContent = `${p.xp} / ${xpNeeded()} XP`;
    $('#m-coins').textContent = p.coins.toLocaleString('es-MX');
    const d = dailyStatus();
    $('#m-daily-dot').hidden = d.claimedToday;
    $('#m-stats').textContent = `${p.stats.matches} partidas · ${p.stats.wins} victorias · ${p.stats.kos} noqueos`;
  }

  // ── Preparar partida ───────────────────────────────────────
  renderSetup() {
    const s = this.setup;
    const pads = connectedGamepads().length;
    const seg = (id, options, value, onPick) => {
      const el = $(id);
      el.innerHTML = options.map((o) => `<button type="button" class="seg-btn${o.v === value ? ' on' : ''}" data-v="${o.v}" ${o.disabled ? 'disabled' : ''}>${o.l}</button>`).join('');
      el.onclick = (e) => { const b = e.target.closest('button'); if (!b || b.disabled) return; sfx.ui(); onPick(isNaN(+b.dataset.v) ? b.dataset.v : +b.dataset.v); this.renderSetup(); };
    };
    const maxHumans = 1 + Math.max(pads, isTouch() ? 0 : 1) + (isTouch() ? pads : 0);
    seg('#s-humans', [1, 2, 3, 4].map((v) => ({ v, l: String(v), disabled: v > Math.min(4, Math.max(1, maxHumans)) })), s.humans, (v) => { s.humans = v; if (s.humans + s.bots > 6) s.bots = 6 - s.humans; if (s.humans + s.bots < 2) s.bots = 2 - s.humans; });
    seg('#s-bots', [0, 1, 2, 3, 4, 5].map((v) => ({ v, l: String(v), disabled: s.humans + v > 6 || s.humans + v < 2 })), s.bots, (v) => { s.bots = v; });
    seg('#s-diff', Object.entries(DIFFICULTY).map(([v, d]) => ({ v, l: d.label })), s.difficulty, (v) => { s.difficulty = v; });
    seg('#s-points', [3, 5, 7].map((v) => ({ v, l: `${v} pts` })), s.points, (v) => { s.points = v; });
    const arenas = [...Object.values(ARENAS).map((a) => ({ v: a.id, l: a.name, d: a.desc })), { v: 'aleatorio', l: 'Aleatorio', d: 'Sorpréndeme.' }];
    const ar = $('#s-arena');
    ar.innerHTML = arenas.map((a) => `<button type="button" class="arena-card ${a.v}${a.v === s.arena ? ' on' : ''}" data-v="${a.v}"><strong>${a.l}</strong><span>${a.d}</span></button>`).join('');
    ar.onclick = (e) => { const b = e.target.closest('button'); if (!b) return; sfx.ui(); s.arena = b.dataset.v; this.renderSetup(); };

    const assign = this._assignControllers(s.humans, true);
    $('#s-controls').innerHTML = assign.map((a, i) => `<li><span class="dot" style="background:${PLAYER_COLORS[i].css}"></span><b>J${i + 1}</b> ${a ? a.label : '<em>sin control: conecta un control Bluetooth</em>'}</li>`).join('')
      + Array.from({ length: s.bots }, (_, i) => `<li><span class="dot" style="background:${PLAYER_COLORS[s.humans + i].css}"></span><b>Bot</b> ${DIFFICULTY[s.difficulty].label}</li>`).join('');
    $('#s-pads').textContent = pads ? `${pads} control(es) detectado(s)` : 'Sin controles. Conecta uno y presiona un botón.';
    $('#btn-start').disabled = assign.some((a) => !a);
    $('#btn-start').onclick = () => { initAudio(); ensureMusic(); this.startMatch(); };
  }

  _assignControllers(n, dryRun = false) {
    const out = [];
    const pads = connectedGamepads();
    let padIdx = 0;
    let kbUsed = 0;
    for (let i = 0; i < n; i++) {
      if (i === 0) {
        if (isTouch() && pads.length === 0) out.push(dryRun ? { label: 'Táctil' } : 'touch');
        else if (!isTouch()) { out.push(dryRun ? { label: 'Teclado WASD + mouse' } : new KeyboardController(0, { useMouse: true })); kbUsed = 1; }
        else { out.push(dryRun ? { label: `Control ${pads[padIdx].index + 1}` } : new GamepadController(pads[padIdx].index)); padIdx++; }
        continue;
      }
      if (padIdx < pads.length) { out.push(dryRun ? { label: `Control ${pads[padIdx].index + 1}` } : new GamepadController(pads[padIdx].index)); padIdx++; continue; }
      if (!isTouch() && kbUsed === 1) { out.push(dryRun ? { label: 'Teclado IJKL' } : new KeyboardController(1)); kbUsed = 2; continue; }
      out.push(null);
    }
    return out;
  }

  startMatch() {
    const s = this.setup;
    const ctrls = this._assignControllers(s.humans);
    this.touch = null;
    const slots = [];
    const eq = progress().equipped;
    const shirts = Object.keys(SHIRT_DESIGNS).filter((k) => !SHIRT_DESIGNS[k].codeOnly);
    ctrls.forEach((c, i) => {
      let controller = c;
      if (c === 'touch') { this.touch = new TouchController($('#touch')); controller = this.touch; }
      slots.push({
        type: 'human', name: i === 0 ? 'TÚ' : `J${i + 1}`, controller,
        shirt: i === 0 ? eq.shirt : shirts[(i * 3) % shirts.length],
        species: i === 0 ? eq.species : (eq.species + i) % SPECIES.length,
      });
    });
    for (let b = 0; b < s.bots; b++) {
      const i = s.humans + b;
      slots.push({
        type: 'bot', name: `BOT ${b + 1}`, difficulty: s.difficulty,
        shirt: shirts[(i * 5 + 2) % shirts.length],
        species: (eq.species + i) % SPECIES.length,
      });
    }
    this.lastSlotsSetup = { ...s };
    this.show(null);
    this.game.startMatch({ slots, arenaId: s.arena, pointsToWin: s.points });
  }

  // ── HUD ────────────────────────────────────────────────────
  hudPlayers(players) {
    $('#hud-players').innerHTML = players.map((p) => `
      <div class="chip${p.alive ? '' : ' out'}" style="--c:${p.color.css}">
        <span class="chip-name">${p.name}</span>
        <span class="pips">${Array.from({ length: this.game.pointsToWin }, (_, k) => `<i class="${k < p.score ? 'on' : ''}"></i>`).join('')}</span>
      </div>`).join('');
  }

  banner(text, sub = '', color = '#fff', ms = 900) {
    const b = $('#banner');
    b.innerHTML = `<div class="banner-main" style="color:${color}">${text}</div>${sub ? `<div class="banner-sub">${sub}</div>` : ''}`;
    b.hidden = false;
    b.classList.remove('pop'); void b.offsetWidth; b.classList.add('pop');
    clearTimeout(this._bannerT);
    this._bannerT = setTimeout(() => { b.hidden = true; }, ms);
  }

  pause() {
    if (this.current !== null) return;
    this.game.debugSpeedBeforePause = this.game.debugSpeed;
    this.game.debugSpeed = 0;
    this.show('pause');
  }

  resume() {
    this.game.debugSpeed = this.game.debugSpeedBeforePause || 1;
    this.show(null);
  }

  quitToMenu() {
    this.game.debugSpeed = this.game.debugSpeedBeforePause || 1;
    this.game.exitToMenu();
    this.show('menu');
  }

  // ── Resultados ─────────────────────────────────────────────
  showResults({ winner, players }) {
    const me = players.find((p) => !p.isBot && p.idx === 0);
    const won = me && winner === me;
    const res = me ? recordMatch({ won, kos: me.kos, roundWins: me.roundWins }) : null;
    const ranked = [...players].sort((a, b) => b.score - a.score || b.kos - a.kos);
    $('#r-title').textContent = won ? '¡GANASTE!' : `GANÓ ${winner.name}`;
    $('#r-title').style.color = winner.color.css;
    $('#r-table').innerHTML = ranked.map((p, i) => `
      <tr${p === winner ? ' class="win"' : ''}>
        <td>${i + 1}</td>
        <td><span class="dot" style="background:${p.color.css}"></span>${p.name}</td>
        <td class="num">${p.score}</td>
        <td class="num">${p.kos}</td>
      </tr>`).join('');
    $('#r-progress').hidden = !res;
    if (res) {
      $('#r-xp').innerHTML = res.breakdown.filter((b) => b.xp > 0).map((b) => `<li><span>${b.label}</span><b>+${b.xp} XP</b></li>`).join('')
        + `<li class="coins"><span>Monedas de tinta</span><b>+${res.coins}</b></li>`;
      const p = progress();
      $('#r-level').textContent = p.level;
      $('#r-levelup').hidden = res.levelsGained === 0;
      $('#r-levelup').textContent = res.levelsGained ? `¡Subiste a nivel ${p.level}!` : '';
      const bar = $('#r-bar');
      bar.style.transition = 'none';
      bar.style.width = res.levelsGained ? '0%' : `${Math.round((res.before.xp / xpNeeded()) * 100)}%`;
      void bar.offsetWidth;
      bar.style.transition = 'width 1.1s cubic-bezier(.2,.8,.2,1)';
      setTimeout(() => { bar.style.width = `${Math.round((p.xp / xpNeeded()) * 100)}%`; sfx.coins(); }, 250);
    }
    this.show('results');
  }

  // ── Vestidor ───────────────────────────────────────────────
  renderWardrobe() {
    const p = progress();
    $('#w-coins').textContent = p.coins.toLocaleString('es-MX');
    $('#w-player-id').textContent = playerId();
    const grid = $('#w-shirts');
    grid.innerHTML = Object.entries(SHIRT_DESIGNS).map(([id, d]) => {
      const owned = p.owned.includes(id);
      const equipped = p.equipped.shirt === id;
      let action;
      if (equipped) action = '<span class="tag on">Puesta</span>';
      else if (owned) action = `<button type="button" class="mini" data-equip="${id}">Ponérmela</button>`;
      else if (d.dailyOnly) action = '<span class="tag">Día 7 del premio diario</span>';
      else if (d.codeOnly) action = '<span class="tag">Canjea un código</span>';
      else action = `<button type="button" class="mini buy" data-buy="${id}" ${p.coins < d.price ? 'disabled' : ''}>${d.price} monedas</button>`;
      return `<div class="shirt${equipped ? ' equipped' : ''}${owned ? '' : ' locked'}">
        <img src="${shirtThumb(id)}" alt="" width="64" height="64">
        <div class="shirt-info"><strong>${d.name}</strong><small>${d.rarity}</small></div>
        ${action}
      </div>`;
    }).join('');
    grid.onclick = (e) => {
      const eqB = e.target.closest('[data-equip]');
      const buyB = e.target.closest('[data-buy]');
      if (eqB) { equip({ shirt: eqB.dataset.equip }); sfx.ui(); }
      else if (buyB) { if (buyShirt(buyB.dataset.buy)) { equip({ shirt: buyB.dataset.buy }); sfx.coins(); } }
      else return;
      this.game.updateShowcase(this._outfit());
      this.renderWardrobe();
    };
    const sp = $('#w-species');
    sp.innerHTML = SPECIES.map((s, i) => {
      const need = SPECIES_UNLOCK[i];
      const locked = p.level < need;
      return `<button type="button" class="seg-btn${p.equipped.species === i ? ' on' : ''}" data-sp="${i}" ${locked ? 'disabled' : ''}>${s.name}${locked ? ` · nv ${need}` : ''}</button>`;
    }).join('');
    sp.onclick = (e) => {
      const b = e.target.closest('[data-sp]'); if (!b || b.disabled) return;
      equip({ species: +b.dataset.sp }); sfx.ui();
      this.game.updateShowcase(this._outfit());
      this.renderWardrobe();
    };
    const form = $('#w-code-form');
    form.onsubmit = (e) => {
      e.preventDefault();
      const r = redeemCode($('#w-code').value);
      $('#w-code-msg').textContent = r.msg;
      $('#w-code-msg').className = r.ok ? 'ok' : 'err';
      if (r.ok) { sfx.win(); this.game.updateShowcase(this._outfit()); this.renderWardrobe(); $('#w-code-msg').textContent = r.msg; $('#w-code-msg').className = 'ok'; }
    };
  }

  // ── Premio diario ──────────────────────────────────────────
  renderDaily() {
    const st = dailyStatus();
    $('#d-streak').textContent = st.streak ? `Racha actual: ${st.streak} día${st.streak === 1 ? '' : 's'}` : 'Empieza tu racha hoy';
    $('#d-days').innerHTML = PROGRESSION.daily.map((r) => {
      const cls = r.day < st.nextDay || (st.claimedToday && r.day === st.nextDay) ? 'claimed' : r.day === st.nextDay ? 'today' : '';
      const parts = [];
      if (r.coins) parts.push(`${r.coins} monedas`);
      if (r.xp) parts.push(`${r.xp} XP`);
      if (r.shirt) parts.push(`Playera "${SHIRT_DESIGNS[r.shirt].name}"`);
      return `<li class="day ${cls}${r.shirt ? ' big' : ''}"><b>Día ${r.day}</b><span>${parts.join(' + ')}</span></li>`;
    }).join('');
    const btn = $('#btn-claim');
    btn.disabled = st.claimedToday;
    btn.textContent = st.claimedToday ? 'Vuelve mañana' : `Cobrar día ${st.nextDay}`;
    btn.onclick = () => {
      const r = claimDaily();
      if (!r) return;
      sfx.coins();
      $('#d-msg').textContent = `¡Listo! Racha de ${r.streak} día${r.streak === 1 ? '' : 's'}.${r.levelsGained ? ' ¡Subiste de nivel!' : ''}`;
      this.renderDaily();
    };
  }

  // ── Cómo se juega ──────────────────────────────────────────
  renderHowto() {
    $('#h-powers').innerHTML = Object.values(POWERUPS).map((p) => `<li><span class="pw" style="--c:${p.css}">${p.glyph}</span><div><strong>${p.name}</strong> · ${p.effect}<br><small>${p.desc}</small></div></li>`).join('');
  }
}
