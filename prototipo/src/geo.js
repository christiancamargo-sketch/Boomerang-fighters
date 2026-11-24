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
export const HIDDEN = new THREE.MeshBasicMaterial({ visible: false });
