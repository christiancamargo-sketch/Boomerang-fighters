// ─────────────────────────────────────────────────────────────
// GENERADOR PSEUDOALEATORIO DE LA SIMULACIÓN
// Mismo algoritmo que el núcleo C# (SimMath.Rng, xorshift32):
// con la misma semilla salen los mismos números en web y en Unity.
// Toda decisión de juego (spawns, latas, lata caliente, láser,
// bots, Reto del Día) usa esto; Math.random queda solo para
// efectos visuales que no cambian el resultado de la partida.
// ─────────────────────────────────────────────────────────────
export class Rng {
  constructor(seed) {
    this.state = (seed >>> 0) || 0x9e3779b9;
  }

  nextUInt() {
    let s = this.state;
    s ^= s << 13; s >>>= 0;
    s ^= s >>> 17;
    s ^= s << 5; s >>>= 0;
    this.state = s;
    return s;
  }

  // número en [0, 1) con 24 bits, igual que NextFloat() en C#
  next() { return (this.nextUInt() >>> 8) * (1 / 16777216); }
  range(min, max) { return min + (max - min) * this.next(); }
  int(maxExclusive) { return maxExclusive <= 0 ? 0 : this.nextUInt() % maxExclusive; }
}

// Semilla del día con formato AAAAMMDD (hora local del celular)
export function daySeed(date = new Date()) {
  return date.getFullYear() * 10000 + (date.getMonth() + 1) * 100 + date.getDate();
}

export function dayKey(date = new Date()) {
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
}
