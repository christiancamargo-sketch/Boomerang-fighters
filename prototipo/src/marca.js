// ─────────────────────────────────────────────────────────────
// MARCA INKCAP: logo y diseños reales que build.mjs incrusta en la
// página (window.__MARCA). Se cargan como imágenes antes de arrancar.
// Fuente: www.inkcap.com.mx (ver herramientas/preparar_marca.py).
// ─────────────────────────────────────────────────────────────
export const MARCA = (typeof window !== 'undefined' && window.__MARCA) || { logo: null, artistas: [] };

const loadImg = (src) => new Promise((resolve) => {
  if (!src) { resolve(null); return; }
  const im = new Image();
  im.onload = () => resolve(im);
  im.onerror = () => resolve(null);
  im.src = src;
});

export async function loadMarca() {
  MARCA.logoImg = await loadImg(MARCA.logo);
  for (const a of MARCA.artistas || []) a.imgEl = await loadImg(a.img);
  return MARCA;
}

// Logo pintado de un color (el original es blanco sobre transparente)
export function tintedLogo(color, width) {
  const img = MARCA.logoImg;
  if (!img) return null;
  const c = document.createElement('canvas');
  c.width = width;
  c.height = Math.round((img.height * width) / img.width);
  const ctx = c.getContext('2d');
  ctx.drawImage(img, 0, 0, c.width, c.height);
  ctx.globalCompositeOperation = 'source-in';
  ctx.fillStyle = color;
  ctx.fillRect(0, 0, c.width, c.height);
  return c;
}
