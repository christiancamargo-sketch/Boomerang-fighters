// ─────────────────────────────────────────────────────────────
// GEOMETRÍAS UNITARIAS COMPARTIDAS por todos los escenarios y la utilería.
// Se escalan por malla y nunca se liberan: son poquitas y así no se
// recrean al cambiar de arena (menos memoria y menos tirones en celular).
// ─────────────────────────────────────────────────────────────
import * as THREE from 'three';

export const GEO = {
  box: new THREE.BoxGeometry(1, 1, 1),
  cyl: new THREE.CylinderGeometry(1, 1, 1, 16),
  cyl8: new THREE.CylinderGeometry(1, 1, 1, 8),
  cone: new THREE.ConeGeometry(1, 1, 12),
  sphere: new THREE.SphereGeometry(1, 14, 10),
  dome: new THREE.SphereGeometry(1, 16, 8, 0, Math.PI * 2, 0, Math.PI / 2),
  plane: new THREE.PlaneGeometry(1, 1),
  // plano acostado sobre el piso (mirando hacia arriba)
  flat: new THREE.PlaneGeometry(1, 1).rotateX(-Math.PI / 2),
  disc: new THREE.CircleGeometry(1, 32).rotateX(-Math.PI / 2),
  ring: new THREE.RingGeometry(0.86, 1, 40).rotateX(-Math.PI / 2),
};

// Caja con solo 3 grupos de material: [cara principal, tapa de arriba, lo demás].
// Un muro cuesta 3 llamadas de dibujo en vez de 6. face: 0 = +x, 1 = −x, 4 = +z.
const _faceBoxes = new Map();
export function faceBox(face) {
  if (_faceBoxes.has(face)) return _faceBoxes.get(face);
  const g = new THREE.BoxGeometry(1, 1, 1);
  const src = Array.from(g.index.array);
  const tri = (f) => src.slice(f * 6, f * 6 + 6); // BoxGeometry: 6 índices por cara (+x, −x, +y, −y, +z, −z)
  const rest = [0, 1, 3, 4, 5].filter((f) => f !== face);
  g.setIndex([...tri(face), ...tri(2), ...rest.flatMap(tri)]);
  g.clearGroups();
  g.addGroup(0, 6, 0); g.addGroup(6, 6, 1); g.addGroup(12, rest.length * 6, 2);
  _faceBoxes.set(face, g);
  return g;
}

// Caja hueca de un vacío vista por dentro: [4 paredes, fondo] (sin tapa): 2 llamadas de dibujo.
let _pitBox = null;
export function pitBox() {
  if (_pitBox) return _pitBox;
  const g = new THREE.BoxGeometry(1, 1, 1);
  const src = Array.from(g.index.array);
  const tri = (f) => src.slice(f * 6, f * 6 + 6);
  g.setIndex([...tri(0), ...tri(1), ...tri(4), ...tri(5), ...tri(3)]);
  g.clearGroups();
  g.addGroup(0, 24, 0); g.addGroup(24, 6, 1);
  _pitBox = g;
  return g;
}
