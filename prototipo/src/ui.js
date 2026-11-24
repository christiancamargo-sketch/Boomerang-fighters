// ─────────────────────────────────────────────────────────────
// INTERFAZ: carga, menú, "Arma la reta" (4 modos), reglas, vestidor
// (playeras, gorras y colección), premio diario + calendario,
// cómo se juega, HUD, pausa y resultados con XP, Gotas y Reto del Día.
// ─────────────────────────────────────────────────────────────
import { GAME_INFO, PLAYER_COLORS, POWERUPS, DIFFICULTY, PROGRESSION, MODES, RULE_INFO, DAILY_RULES } from './config.js';
import { ARENAS } from './arena.js';
import { SPECIES, SPECIES_ORDER } from './characters.js';
import { SHIRT_DESIGNS, CAP_DESIGNS, shirtThumb, capThumb, weeklyCapId } from './textures.js';
import {
  progress, xpNeeded, recordMatch, dailyStatus, claimDaily,
  buyShirt, buyCap, equip, redeemCode, SPECIES_UNLOCK, playerId,
  loadRules, saveRules, retoScore, recordReto, retoTable, STICKERS, EMOTES,
} from './progress.js';
import { sfx, initAudio, ensureMusic, setMusic, setSfx, audioState } from './audio.js';
import { KeyboardController, GamepadController, TouchController, connectedGamepads } from './input.js';
import { Rng, daySeed } from './rng.js';

const $ = (sel, root = document) => root.querySelector(sel);
const isTouch = () => window.matchMedia('(pointer: coarse)').matches || 'ontouchstart' in window;
const esc = (t) => String(t).replace(/[&<>"]/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[c]));

// Reto del Día: arena y "regla del día" salen de la semilla AAAAMMDD
// (primero la arena, luego la regla, con el mismo Rng que la simulación).
export function retoDelDia(date = new Date()) {
  const seed = daySeed(date);
  const r = new Rng(seed);
  const ids = Object.keys(ARENAS);
  const arena = ids[r.int(ids.length)];
  const rule = DAILY_RULES[r.int(DAILY_RULES.length)];
  return { seed, arena, rule };
}

export class UI {
  constructor(game) {
    this.game = game;
    this.setup = { mode: 'gorrazo', humans: 1, bots: 3, difficulty: 'normal', arena: 'callejon', points: 5 };
    this.touch = null;
    this.autopilot = false; // pruebas: los humanos los maneja un bot
    this.wardrobeTab = 'shirts';
    this.screens = ['menu', 'setup', 'rules', 'wardrobe', 'daily', 'howto', 'results', 'pause'];
    this._bindGlobal();
    this.renderTitle();
    this.show('menu');
  }

  // ── Navegación ─────────────────────────────────────────────
  show(name) {
    if (name !== this.current && this.current && this.current !== 'pause') this.prev = this.current;
    for (const s of this.screens) { const el = $(`#scr-${s}`); if (el) el.hidden = s !== name; }
    const inMatch = name === null;
    $('#hud').hidden = !inMatch;
    $('#touch').hidden = !(inMatch && this.touch);
    this.current = name;
    if (name === 'menu') { this.renderMenu(); this.game.showMenu(this._outfit()); }
    if (name === 'setup') this.renderSetup();
    if (name === 'rules') this.renderRules();
    if (name === 'wardrobe') { this.renderWardrobe(); if (this.game.mode !== 'menu') this.game.showMenu(this._outfit()); }
    if (name === 'daily') this.renderDaily();
    if (name === 'howto') this.renderHowto();
  }

  _outfit() {
    const e = progress().equipped;
    return { shirt: e.shirt, species: e.species, cap: e.cap, arena: 'callejon' };
  }

  _bindGlobal() {
    document.addEventListener('click', (e) => {
      const b = e.target.closest('[data-go]');
      if (b) {
        initAudio(); ensureMusic(); sfx.ui();
        this.show(b.dataset.go === 'back' ? (this.prev || 'menu') : b.dataset.go);
      }
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

  // ── Arma la reta ───────────────────────────────────────────
  renderSetup() {
    const s = this.setup;
    const M = MODES[s.mode];
    const pads = connectedGamepads().length;
    const seg = (id, options, value, onPick) => {
      const el = $(id);
      el.innerHTML = options.map((o) => `<button type="button" class="seg-btn${o.v === value ? ' on' : ''}" data-v="${o.v}" ${o.disabled ? 'disabled' : ''}>${o.l}</button>`).join('');
      el.onclick = (e) => { const b = e.target.closest('button'); if (!b || b.disabled) return; sfx.ui(); onPick(isNaN(+b.dataset.v) ? b.dataset.v : +b.dataset.v); this.renderSetup(); };
    };
    const clampBots = () => {
      const m = MODES[s.mode];
      if (s.mode === 'retoDelDia') { s.humans = 1; return; }
      if (s.humans + s.bots > m.max) s.bots = Math.max(0, m.max - s.humans);
      if (s.humans + s.bots < m.min) s.bots = m.min - s.humans;
    };
    seg('#s-mode', Object.values(MODES).map((m) => ({ v: m.id, l: m.name })), s.mode, (v) => { s.mode = v; clampBots(); });
    $('#s-mode-desc').textContent = M.desc;
    const reto = s.mode === 'retoDelDia';
    for (const id of ['#f-humans', '#f-bots', '#f-diff', '#f-arena', '#f-points']) $(id).hidden = reto;
    $('#f-reto').hidden = !reto;
    $('#f-points').hidden = reto || s.mode === 'laReta';

    const maxHumans = 1 + Math.max(pads, isTouch() ? 0 : 1) + (isTouch() ? pads : 0);
    seg('#s-humans', [1, 2, 3, 4].map((v) => ({ v, l: String(v), disabled: v > Math.min(4, Math.max(1, maxHumans)) })), s.humans, (v) => { s.humans = v; clampBots(); });
    seg('#s-bots', [0, 1, 2, 3, 4, 5, 6, 7].filter((v) => v <= M.max).map((v) => ({ v, l: String(v), disabled: s.humans + v > M.max || s.humans + v < M.min })), s.bots, (v) => { s.bots = v; });
    seg('#s-diff', Object.entries(DIFFICULTY).map(([v, d]) => ({ v, l: d.label })), s.difficulty, (v) => { s.difficulty = v; });
    seg('#s-points', [3, 5, 7].map((v) => ({ v, l: `${v} pts` })), s.points, (v) => { s.points = v; });
    const arenas = [...Object.values(ARENAS).map((a) => ({ v: a.id, l: a.name, d: a.desc })), { v: 'aleatorio', l: 'Aleatorio', d: 'Sorpréndeme.' }];
    const ar = $('#s-arena');
    ar.innerHTML = arenas.map((a) => `<button type="button" class="arena-card ${a.v}${a.v === s.arena ? ' on' : ''}" data-v="${a.v}"><strong>${a.l}</strong><span>${a.d}</span></button>`).join('');
    ar.onclick = (e) => { const b = e.target.closest('button'); if (!b) return; sfx.ui(); s.arena = b.dataset.v; this.renderSetup(); };

    if (reto) this._renderRetoInfo();

    const humans = reto ? 1 : s.humans;
    const bots = reto ? 3 : s.bots;
    const diff = reto ? 'dificil' : s.difficulty;
    const assign = this._assignControllers(humans, true);
    $('#s-controls').innerHTML = assign.map((a, i) => `<li><span class="dot" style="background:${PLAYER_COLORS[i].css}"></span><b>J${i + 1}</b> ${a ? a.label : '<em>sin control: conecta un control Bluetooth</em>'}</li>`).join('')
      + Array.from({ length: bots }, (_, i) => `<li><span class="dot" style="background:${PLAYER_COLORS[(humans + i) % PLAYER_COLORS.length].css}"></span><b>Bot</b> ${DIFFICULTY[diff].label}</li>`).join('');
    $('#s-pads').textContent = pads ? `${pads} control(es) detectado(s)` : 'Sin controles. Conecta uno y presiona un botón.';
    const r = loadRules();
    $('#s-rules').textContent = reto ? 'El Reto del Día usa su propia regla.' : Object.entries(RULE_INFO).map(([k, v]) => `${v.name}: ${r[k] ? 'sí' : 'no'}`).join(' · ');
    $('#btn-start').disabled = !this.autopilot && assign.some((a) => !a);
    $('#btn-start').onclick = () => { initAudio(); ensureMusic(); this.startMatch(); };
  }

  _renderRetoInfo() {
    const rd = retoDelDia();
    const rows = retoTable();
    $('#s-reto').innerHTML = `
      <div class="reto-card">
        <div><span class="lbl">Semilla</span><b>${rd.seed}</b></div>
        <div><span class="lbl">Arena</span><b>${ARENAS[rd.arena].name}</b></div>
        <div><span class="lbl">Regla del día</span><b>${rd.rule.name}</b></div>
        <div><span class="lbl">Tu mejor de hoy</span><b>${rows[0].best ?? '—'}</b></div>
      </div>
      <p class="hint">Puntaje = KOs × 100 + rondas ganadas × 300 − segundos. Tú contra 3 bots en Difícil, a 3 puntos.</p>
      ${this._retoTableHtml(rows)}`;
  }

  _retoTableHtml(rows) {
    return `<table class="reto-table"><thead><tr><th>Día</th><th class="num">Mejor puntaje</th></tr></thead><tbody>${rows.map((r) => `<tr${r.today ? ' class="win"' : ''}><td>${r.label}</td><td class="num">${r.best ?? '—'}</td></tr>`).join('')}</tbody></table>`;
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
    const reto = s.mode === 'retoDelDia';
    const rd = reto ? retoDelDia() : null;
    const humans = reto ? 1 : s.humans;
    const botsN = reto ? 3 : s.bots;
    const diff = reto ? 'dificil' : s.difficulty;
    const ctrls = this._assignControllers(humans);
    this.touch = null;
    const slots = [];
    const eq = progress().equipped;
    const shirts = Object.keys(SHIRT_DESIGNS).filter((k) => !SHIRT_DESIGNS[k].codeOnly && !SHIRT_DESIGNS[k].calendarOnly);
    ctrls.forEach((c, i) => {
      let controller = c;
      if (c === 'touch' && !this.autopilot) { this.touch = new TouchController($('#touch')); controller = this.touch; }
      slots.push({
        type: 'human', name: i === 0 ? 'TÚ' : `J${i + 1}`, controller,
        autopilot: this.autopilot, difficulty: 'normal',
        shirt: i === 0 ? eq.shirt : shirts[(i * 3) % shirts.length],
        species: i === 0 ? eq.species : (eq.species + i) % SPECIES.length,
        capDesign: i === 0 ? eq.cap : 'clasica',
      });
    });
    for (let b = 0; b < botsN; b++) {
      const i = humans + b;
      slots.push({
        type: 'bot', name: `BOT ${b + 1}`, difficulty: diff,
        shirt: shirts[(i * 5 + 2) % shirts.length],
        species: (eq.species + i) % SPECIES.length,
      });
    }
    this.lastSlotsSetup = { ...s };
    this.show(null);
    const rules = reto ? { ...rd.rule.rules } : loadRules();
    this.game.startMatch({
      slots, mode: s.mode, rules,
      arenaId: reto ? rd.arena : s.arena,
      pointsToWin: reto ? 3 : s.points,
      seed: reto ? rd.seed : null,
      pickupsEvery: reto ? rd.rule.pickupsEvery : null,
      pickupsMax: reto ? rd.rule.pickupsMax : null,
    });
    this.banner(MODES[s.mode].name, reto ? `Regla del día: ${rd.rule.name}` : '', '#f4f1ea', 1000);
  }

  // ── HUD ────────────────────────────────────────────────────
  hudPlayers(players) {
    const g = this.game;
    const reta = g.modeId === 'laReta';
    const hot = g.hot && g.hot.holder;
    const eq = progress().equipped;
    const sticker = STICKERS.find((x) => x.id === eq.sticker);
    const order = reta ? g.queue : players;
    $('#hud-players').innerHTML = order.map((p, qi) => {
      const pipsN = reta ? 3 : g.pointsToWin;
      const filled = reta ? p.streak : p.score;
      const tag = reta ? (qi < 2 ? `<b class="chip-tag">${p.score}V</b>` : `<b class="chip-tag">fila ${qi - 1}</b>`) : '';
      const st = sticker && p.idx === 0 && !p.isBot ? `<b class="chip-sticker">${esc(sticker.text)}</b>` : '';
      return `
      <div class="chip${p.alive || (reta && qi < 2 && g.state === 'countdown') ? '' : ' out'}${p.benched ? ' benched' : ''}${hot === p ? ' hot' : ''}" style="--c:${p.color.css}">
        ${hot === p ? '<span class="chip-can" aria-label="trae la lata">LATA</span>' : ''}
        <span class="chip-name">${esc(p.name)}</span>${st}
        <span class="pips">${Array.from({ length: pipsN }, (_, k) => `<i class="${k < filled ? 'on' : ''}"></i>`).join('')}</span>${tag}
      </div>`;
    }).join('');
  }

  banner(text, sub = '', color = '#fff', ms = 900) {
    const b = $('#banner');
    b.innerHTML = `<div class="banner-main" style="color:${color}">${text}</div>${sub ? `<div class="banner-sub">${sub}</div>` : ''}`;
    b.hidden = false;
    b.classList.remove('pop'); void b.offsetWidth; b.classList.add('pop');
    clearTimeout(this._bannerT);
    this._bannerT = setTimeout(() => { b.hidden = true; }, ms / Math.max(1, this.game.debugSpeed || 1));
  }

  // Emote equipado cuando ganas una ronda
  emote(w) {
    if (!w || w.isBot || w.idx !== 0) return;
    const e = EMOTES.find((x) => x.id === progress().equipped.emote);
    if (e) this.game.float(w.pos.x, 3, w.pos.z, e.text, w.color.css, true);
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
  showResults({ winner, players, mode, seconds }) {
    const me = players.find((p) => !p.isBot && p.idx === 0);
    const won = me && winner === me;
    const res = me ? recordMatch({ won, kos: me.kos, roundWins: me.roundWins }) : null;
    const reta = mode === 'laReta';
    const ranked = [...players].sort((a, b) => (b === winner) - (a === winner) || b.score - a.score || b.kos - a.kos);
    $('#r-mode').textContent = MODES[mode]?.name || '';
    $('#r-title').textContent = won ? '¡GANASTE!' : `GANÓ ${winner.name}`;
    $('#r-title').style.color = winner.color.css;
    $('#r-col-pts').textContent = reta ? 'Vict.' : 'Pts';
    $('#r-table').innerHTML = ranked.map((p, i) => `
      <tr${p === winner ? ' class="win"' : ''}>
        <td>${i + 1}</td>
        <td><span class="dot" style="background:${p.color.css}"></span>${esc(p.name)}</td>
        <td class="num">${p.score}</td>
        <td class="num">${p.kos}</td>
      </tr>`).join('');
    // Reto del Día
    const retoEl = $('#r-reto');
    retoEl.hidden = !(mode === 'retoDelDia' && me);
    if (mode === 'retoDelDia' && me) {
      const score = retoScore({ kos: me.kos, roundWins: me.roundWins, seconds });
      const rr = recordReto(score);
      retoEl.innerHTML = `
        <div class="reto-score"><span class="lbl">Puntaje del Reto</span><b id="r-reto-score">${score}</b>
        <small>${me.kos} KOs × 100 + ${me.roundWins} rondas × 300 − ${Math.round(seconds)} s</small>
        ${rr.isBest ? '<em class="levelup">¡Tu mejor de hoy!</em>' : `<small>Tu mejor de hoy: ${rr.best}</small>`}</div>
        ${this._retoTableHtml(retoTable())}`;
    }
    $('#r-progress').hidden = !res;
    if (res) {
      $('#r-xp').innerHTML = res.breakdown.filter((b) => b.xp > 0).map((b) => `<li${b.label.startsWith('Primera') ? ' class="first"' : ''}><span>${b.label}</span><b>+${b.xp} XP</b></li>`).join('')
        + `<li class="coins"><span>Gotas</span><b>+${res.coins}</b></li>`;
      const p = progress();
      $('#r-level').textContent = p.level;
      $('#r-levelup').hidden = res.levelsGained === 0;
      $('#r-levelup').textContent = res.levelsGained ? `¡Subiste a nivel ${p.level}!${this._unlockText(res.before.level, p.level)}` : '';
      const bar = $('#r-bar');
      bar.style.transition = 'none';
      bar.style.width = res.levelsGained ? '0%' : `${Math.round((res.before.xp / xpNeeded()) * 100)}%`;
      void bar.offsetWidth;
      bar.style.transition = 'width 1.1s cubic-bezier(.2,.8,.2,1)';
      setTimeout(() => { bar.style.width = `${Math.round((p.xp / xpNeeded()) * 100)}%`; sfx.coins(); }, 250);
    }
    this.show('results');
  }

  _unlockText(from, to) {
    const names = SPECIES.map((s, i) => ({ s, need: SPECIES_UNLOCK[i] })).filter((x) => x.need > from && x.need <= to).map((x) => x.s.name);
    return names.length ? ` Desbloqueaste a ${names.join(', ')}.` : '';
  }

  // ── Reglas ─────────────────────────────────────────────────
  renderRules() {
    const r = loadRules();
    const list = $('#rules-list');
    list.innerHTML = Object.entries(RULE_INFO).map(([k, v]) => `
      <li class="rule">
        <div><strong>${v.name}</strong><small>${v.desc}</small></div>
        <button type="button" class="switch" role="switch" aria-checked="${r[k]}" data-rule="${k}" aria-label="${v.name}"><i></i></button>
      </li>`).join('');
    list.onclick = (e) => {
      const b = e.target.closest('[data-rule]'); if (!b) return;
      const cur = loadRules();
      cur[b.dataset.rule] = !cur[b.dataset.rule];
      saveRules(cur);
      sfx.ui();
      this.renderRules();
    };
  }

  // ── Vestidor ───────────────────────────────────────────────
  renderWardrobe() {
    const p = progress();
    $('#w-coins').textContent = p.coins.toLocaleString('es-MX');
    $('#w-player-id').textContent = playerId();
    const tabs = $('#w-tabs');
    tabs.querySelectorAll('[data-tab]').forEach((b) => { b.classList.toggle('on', b.dataset.tab === this.wardrobeTab); b.setAttribute('aria-selected', String(b.dataset.tab === this.wardrobeTab)); });
    tabs.onclick = (e) => { const b = e.target.closest('[data-tab]'); if (!b) return; sfx.ui(); this.wardrobeTab = b.dataset.tab; this.renderWardrobe(); };
    $('#w-shirts').hidden = this.wardrobeTab !== 'shirts';
    $('#w-caps').hidden = this.wardrobeTab !== 'caps';
    $('#w-collection').hidden = this.wardrobeTab !== 'collection';

    const item = ({ id, img, name, rarity, owned, equipped, action, url }) => `<div class="shirt${equipped ? ' equipped' : ''}${owned ? '' : ' locked'}" data-id="${id}">
        <img src="${img}" alt="" width="64" height="64">
        <div class="shirt-info"><strong>${name}</strong><small>${rarity}</small>${url ? `<a class="real" href="${url}" target="_blank" rel="noopener">Consíguela real ↗</a>` : ''}</div>
        ${action}
      </div>`;
    const actionFor = (kind, id, d, owned, equipped) => {
      if (equipped) return `<span class="tag on">${kind === 'cap' ? 'Puesta' : 'Puesta'}</span>`;
      if (owned) return `<button type="button" class="mini" data-equip-${kind}="${id}">Ponérmela</button>`;
      if (d.dailyOnly) return '<span class="tag">Día 7 del premio diario</span>';
      if (d.codeOnly) return '<span class="tag">Canjea un código</span>';
      if (d.calendarOnly) return '<span class="tag">Calendario de 28 días</span>';
      return `<button type="button" class="mini buy" data-buy-${kind}="${id}" ${p.coins < d.price ? 'disabled' : ''}>${d.price} Gotas</button>`;
    };

    const grid = $('#w-shirts');
    grid.innerHTML = Object.entries(SHIRT_DESIGNS).map(([id, d]) => item({
      id, img: shirtThumb(id), name: d.name, rarity: d.rarity, url: d.url,
      owned: p.owned.includes(id), equipped: p.equipped.shirt === id,
      action: actionFor('shirt', id, d, p.owned.includes(id), p.equipped.shirt === id),
    })).join('');

    const week = weeklyCapId();
    const caps = Object.entries(CAP_DESIGNS).filter(([id, d]) => !d.dailyOnly || id === week || p.ownedCaps.includes(id));
    $('#w-caps').innerHTML = `<p class="hint">El diseño va en la copa: la silueta y el tamaño nunca cambian, y tu color de jugador sigue en el aro y el rastro.</p>`
      + caps.map(([id, d]) => item({
        id, img: capThumb(id, PLAYER_COLORS[0].css), name: d.name, rarity: d.rarity, url: d.url,
        owned: p.ownedCaps.includes(id), equipped: p.equipped.cap === id,
        action: actionFor('cap', id, d, p.ownedCaps.includes(id), p.equipped.cap === id),
      })).join('');

    const st = STICKERS.filter((s) => p.stickers.includes(s.id));
    const em = EMOTES.filter((s) => p.emotes.includes(s.id));
    $('#w-collection').innerHTML = `
      <span class="lbl">Stickers (salen en tu ficha del HUD)</span>
      <div class="seg">${st.length ? st.map((s) => `<button type="button" class="seg-btn${p.equipped.sticker === s.id ? ' on' : ''}" data-sticker="${s.id}">${esc(s.text)}</button>`).join('') : '<span class="hint">Día 2 del premio diario.</span>'}</div>
      <span class="lbl">Emotes (al ganar una ronda)</span>
      <div class="seg">${em.length ? em.map((s) => `<button type="button" class="seg-btn${p.equipped.emote === s.id ? ' on' : ''}" data-emote="${s.id}">${esc(s.text)}</button>`).join('') : '<span class="hint">Día 5 del premio diario.</span>'}</div>`;

    const onPick = (e) => {
      const t = (attr) => e.target.closest(`[data-${attr}]`);
      let b;
      if ((b = t('equip-shirt'))) { equip({ shirt: b.dataset.equipShirt }); sfx.ui(); }
      else if ((b = t('buy-shirt'))) { if (buyShirt(b.dataset.buyShirt)) { equip({ shirt: b.dataset.buyShirt }); sfx.coins(); } }
      else if ((b = t('equip-cap'))) { equip({ cap: b.dataset.equipCap }); sfx.ui(); }
      else if ((b = t('buy-cap'))) { if (buyCap(b.dataset.buyCap)) { equip({ cap: b.dataset.buyCap }); sfx.coins(); } }
      else if ((b = t('sticker'))) { equip({ sticker: b.dataset.sticker }); sfx.ui(); }
      else if ((b = t('emote'))) { equip({ emote: b.dataset.emote }); sfx.ui(); }
      else return;
      this.game.updateShowcase(this._outfit());
      this.renderWardrobe();
    };
    grid.onclick = onPick; $('#w-caps').onclick = onPick; $('#w-collection').onclick = onPick;

    const sp = $('#w-species');
    sp.innerHTML = SPECIES_ORDER.map((i) => {
      const s = SPECIES[i];
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
      if (r.ok) { sfx.win(); this.game.updateShowcase(this._outfit()); this.wardrobeTab = 'shirts'; this.renderWardrobe(); }
      $('#w-code-msg').textContent = r.msg;
      $('#w-code-msg').className = r.ok ? 'ok' : 'err';
    };
  }

  // ── Premio diario + calendario de 28 días ──────────────────
  renderDaily() {
    const st = dailyStatus();
    $('#d-streak').textContent = st.streak ? `Días cobrados: ${st.streak} · si faltas, tu racha se pausa (no regresa al día 1)` : 'Empieza tu racha hoy';
    const week = CAP_DESIGNS[weeklyCapId()];
    $('#d-days').innerHTML = PROGRESSION.daily.map((r) => {
      const cls = r.day < st.nextDay || (st.claimedToday && r.day === st.nextDay) ? 'claimed' : r.day === st.nextDay ? 'today' : '';
      let txt = '';
      if (r.coins) txt = r.extra ? `+${r.coins} Gotas extra` : `${r.coins} Gotas`;
      if (r.sticker) txt = 'Sticker';
      if (r.emote) txt = 'Emote';
      if (r.weeklyCap) txt = `Gorra de la semana: "${week.name}"`;
      return `<li class="day ${cls}${r.weeklyCap ? ' big' : ''}"><b>Día ${r.day}</b><span>${txt}</span></li>`;
    }).join('');
    const N = PROGRESSION.calendarDays;
    $('#d-cal').innerHTML = Array.from({ length: N }, (_, i) => `<i class="${i < st.calendar ? 'on' : ''}${i === N - 1 ? ' goal' : ''}">${i === N - 1 ? 'P' : i + 1}</i>`).join('');
    $('#d-cal-text').textContent = `Calendario: ${st.calendar} de ${N} sellos → Playera del mes`;
    const btn = $('#btn-claim');
    btn.disabled = st.claimedToday;
    btn.textContent = st.claimedToday ? 'Vuelve mañana' : `Cobrar día ${st.nextDay}`;
    btn.onclick = () => {
      const r = claimDaily();
      if (!r) return;
      sfx.coins();
      $('#d-msg').textContent = `¡Listo! ${r.got.join(' + ')}.`;
      this.renderDaily();
    };
  }

  // ── Cómo se juega ──────────────────────────────────────────
  renderHowto() {
    $('#h-powers').innerHTML = Object.values(POWERUPS).map((p) => `<li><span class="pw" style="--c:${p.css}">${p.glyph}</span><div><strong>${p.name}</strong> · ${p.effect}<br><small>${p.desc}</small></div></li>`).join('');
  }
}
