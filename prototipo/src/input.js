// ─────────────────────────────────────────────────────────────
// CONTROLES: teclado (2 jugadores), mouse, controles (Gamepad API)
// y táctil (joystick virtual + botones, estilo juegos móviles).
// Cada control expone el mismo "estado de mando" para que el juego
// no sepa si quien juega es humano, táctil o bot.
// ─────────────────────────────────────────────────────────────

export function emptyPad() {
  return {
    mx: 0, mz: 0,          // movimiento (-1..1)
    ax: 0, az: 0, aim: false, // apuntado explícito (stick derecho, mouse, arrastre)
    throwHeld: false, throwPressed: false, throwReleased: false,
    meleePressed: false, dashPressed: false,
  };
}

const keys = new Set();
const pressedThisFrame = new Set();
const releasedThisFrame = new Set();
let listenersReady = false;

function setupKeyboard() {
  if (listenersReady) return;
  listenersReady = true;
  window.addEventListener('keydown', (e) => {
    if (e.repeat) return;
    if (['Space', 'ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight'].includes(e.code)) e.preventDefault();
    keys.add(e.code); pressedThisFrame.add(e.code);
  });
  window.addEventListener('keyup', (e) => { keys.delete(e.code); releasedThisFrame.add(e.code); });
  window.addEventListener('blur', () => keys.clear());
}

export function endInputFrame() {
  pressedThisFrame.clear();
  releasedThisFrame.clear();
  mouse.pressed = false; mouse.released = false; mouse.rightPressed = false;
}

// ── Teclado ──────────────────────────────────────────────────
// J1: WASD mover · E lanzar (mantén para cargar) · Q golpe · F/Espacio dash · mouse opcional
// J2: IJKL mover · O lanzar · U golpe · H dash
const LAYOUTS = [
  { up: 'KeyW', down: 'KeyS', left: 'KeyA', right: 'KeyD', throw: ['KeyE'], melee: ['KeyQ'], dash: ['KeyF', 'Space'] },
  { up: 'KeyI', down: 'KeyK', left: 'KeyJ', right: 'KeyL', throw: ['KeyO'], melee: ['KeyU'], dash: ['KeyH'] },
];

export const mouse = { x: 0, y: 0, down: false, pressed: false, released: false, rightPressed: false, active: false, lastMove: 0 };
let mouseReady = false;
export function setupMouse(canvas) {
  if (mouseReady) return;
  mouseReady = true;
  canvas.addEventListener('mousemove', (e) => { mouse.x = e.clientX; mouse.y = e.clientY; mouse.active = true; mouse.lastMove = performance.now(); });
  canvas.addEventListener('mousedown', (e) => {
    if (e.button === 0) { mouse.down = true; mouse.pressed = true; }
    if (e.button === 2) mouse.rightPressed = true;
    mouse.active = true;
  });
  window.addEventListener('mouseup', (e) => { if (e.button === 0 && mouse.down) { mouse.down = false; mouse.released = true; } });
  canvas.addEventListener('contextmenu', (e) => e.preventDefault());
}

export class KeyboardController {
  constructor(layoutIndex, { useMouse = false, aimFromMouse = null } = {}) {
    setupKeyboard();
    this.kind = 'teclado';
    this.layout = LAYOUTS[layoutIndex];
    this.useMouse = useMouse;
    this.aimFromMouse = aimFromMouse; // función (pad) → rellena ax/az desde el mouse
    this.label = layoutIndex === 0 ? 'Teclado WASD' : 'Teclado IJKL';
  }
  poll() {
    const L = this.layout;
    const p = emptyPad();
    p.mx = (keys.has(L.right) ? 1 : 0) - (keys.has(L.left) ? 1 : 0);
    p.mz = (keys.has(L.down) ? 1 : 0) - (keys.has(L.up) ? 1 : 0);
    const any = (arr, set) => arr.some((k) => set.has(k));
    p.throwHeld = any(L.throw, keys);
    p.throwPressed = any(L.throw, pressedThisFrame);
    p.throwReleased = any(L.throw, releasedThisFrame);
    p.meleePressed = any(L.melee, pressedThisFrame);
    p.dashPressed = any(L.dash, pressedThisFrame);
    if (this.useMouse && mouse.active) {
      if (mouse.down) p.throwHeld = true;
      if (mouse.pressed) p.throwPressed = true;
      if (mouse.released) p.throwReleased = true;
      if (mouse.rightPressed) p.meleePressed = true;
      if (this.aimFromMouse && performance.now() - mouse.lastMove < 4000) this.aimFromMouse(p);
    }
    return p;
  }
}

// ── Controles físicos ────────────────────────────────────────
export function connectedGamepads() {
  const list = navigator.getGamepads ? Array.from(navigator.getGamepads()).filter(Boolean) : [];
  return list.filter((g) => g.connected);
}

export class GamepadController {
  constructor(index) {
    this.kind = 'control';
    this.index = index;
    this.prev = { throw: false, melee: false, dash: false };
    this.label = `Control ${index + 1}`;
  }
  poll() {
    const p = emptyPad();
    const gp = (navigator.getGamepads ? navigator.getGamepads() : [])[this.index];
    if (!gp) return p;
    const dz = (v) => (Math.abs(v) < 0.18 ? 0 : v);
    p.mx = dz(gp.axes[0] || 0); p.mz = dz(gp.axes[1] || 0);
    const rx = dz(gp.axes[2] || 0), rz = dz(gp.axes[3] || 0);
    if (Math.hypot(rx, rz) > 0.35) { p.ax = rx; p.az = rz; p.aim = true; }
    const b = (i) => !!(gp.buttons[i] && gp.buttons[i].pressed);
    const thr = b(0) || b(7);   // A / RT
    const mel = b(2) || b(5);   // X / RB
    const dsh = b(1) || b(6) || b(4); // B / LT / LB
    p.throwHeld = thr;
    p.throwPressed = thr && !this.prev.throw;
    p.throwReleased = !thr && this.prev.throw;
    p.meleePressed = mel && !this.prev.melee;
    p.dashPressed = dsh && !this.prev.dash;
    this.prev = { throw: thr, melee: mel, dash: dsh };
    return p;
  }
}

// ── Táctil ───────────────────────────────────────────────────
// Mitad izquierda: joystick flotante (aparece donde pones el pulgar).
// Derecha: botón LANZAR (toca = tiro rápido con ayuda de apuntado,
// mantén = carga, arrastra = apunta a mano), GOLPE y DASH.
export class TouchController {
  constructor(root) {
    this.kind = 'tactil';
    this.label = 'Táctil';
    this.root = root;
    this.stick = { id: null, ox: 0, oy: 0, x: 0, y: 0 };
    this.throwBtn = { id: null, ox: 0, oy: 0, x: 0, y: 0, held: false };
    this.flags = { throwPressed: false, throwReleased: false, melee: false, dash: false };
    this._build();
  }

  _build() {
    const r = this.root;
    r.innerHTML = `
      <div class="t-zone t-left" id="t-left"></div>
      <div class="t-stick" id="t-stick" hidden><div class="t-knob" id="t-knob"></div></div>
      <button class="t-btn t-throw" id="t-throw" aria-label="Lanzar gorra"><span>LANZAR</span><small>mantén para cargar</small></button>
      <button class="t-btn t-melee" id="t-melee" aria-label="Golpe">GOLPE</button>
      <button class="t-btn t-dash" id="t-dash" aria-label="Dash">DASH</button>
    `;
    const left = r.querySelector('#t-left');
    const stickEl = r.querySelector('#t-stick');
    const knob = r.querySelector('#t-knob');
    const R = 56;

    left.addEventListener('pointerdown', (e) => {
      e.preventDefault();
      this.stick.id = e.pointerId;
      this.stick.ox = e.clientX; this.stick.oy = e.clientY;
      this.stick.x = 0; this.stick.y = 0;
      stickEl.hidden = false;
      stickEl.style.left = `${e.clientX}px`; stickEl.style.top = `${e.clientY}px`;
      knob.style.transform = 'translate(-50%,-50%)';
      left.setPointerCapture(e.pointerId);
    });
    left.addEventListener('pointermove', (e) => {
      if (e.pointerId !== this.stick.id) return;
      let dx = e.clientX - this.stick.ox, dy = e.clientY - this.stick.oy;
      const d = Math.hypot(dx, dy);
      if (d > R) { dx = (dx / d) * R; dy = (dy / d) * R; }
      this.stick.x = dx / R; this.stick.y = dy / R;
      knob.style.transform = `translate(calc(-50% + ${dx}px), calc(-50% + ${dy}px))`;
    });
    const endStick = (e) => {
      if (e.pointerId !== this.stick.id) return;
      this.stick.id = null; this.stick.x = 0; this.stick.y = 0;
      stickEl.hidden = true;
    };
    left.addEventListener('pointerup', endStick);
    left.addEventListener('pointercancel', endStick);

    const tb = r.querySelector('#t-throw');
    tb.addEventListener('pointerdown', (e) => {
      e.preventDefault();
      this.throwBtn.id = e.pointerId;
      this.throwBtn.ox = e.clientX; this.throwBtn.oy = e.clientY;
      this.throwBtn.x = 0; this.throwBtn.y = 0;
      this.throwBtn.held = true;
      this.flags.throwPressed = true;
      tb.classList.add('down');
      tb.setPointerCapture(e.pointerId);
    });
    tb.addEventListener('pointermove', (e) => {
      if (e.pointerId !== this.throwBtn.id) return;
      this.throwBtn.x = e.clientX - this.throwBtn.ox;
      this.throwBtn.y = e.clientY - this.throwBtn.oy;
    });
    const endThrow = (e) => {
      if (e.pointerId !== this.throwBtn.id) return;
      this.throwBtn.id = null; this.throwBtn.held = false;
      this.flags.throwReleased = true;
      tb.classList.remove('down');
    };
    tb.addEventListener('pointerup', endThrow);
    tb.addEventListener('pointercancel', endThrow);

    const tap = (sel, flag) => {
      const el = r.querySelector(sel);
      el.addEventListener('pointerdown', (e) => { e.preventDefault(); this.flags[flag] = true; el.classList.add('down'); });
      const up = () => el.classList.remove('down');
      el.addEventListener('pointerup', up); el.addEventListener('pointercancel', up); el.addEventListener('pointerleave', up);
    };
    tap('#t-melee', 'melee');
    tap('#t-dash', 'dash');
  }

  poll() {
    const p = emptyPad();
    p.mx = this.stick.x; p.mz = this.stick.y;
    p.throwHeld = this.throwBtn.held;
    p.throwPressed = this.flags.throwPressed;
    p.throwReleased = this.flags.throwReleased;
    p.meleePressed = this.flags.melee;
    p.dashPressed = this.flags.dash;
    // arrastrar el botón de lanzar = apuntar a mano (como en los brawlers móviles)
    const dx = this.throwBtn.x, dy = this.throwBtn.y;
    if (this.throwBtn.held && Math.hypot(dx, dy) > 22) { p.ax = dx; p.az = dy; p.aim = true; }
    p.wantsAssist = !p.aim; // si no apuntó a mano, usamos ayuda de apuntado
    this.flags = { throwPressed: false, throwReleased: false, melee: false, dash: false };
    return p;
  }
}
