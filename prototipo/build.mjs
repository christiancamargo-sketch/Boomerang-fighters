// Empaqueta el prototipo en UN solo archivo HTML (sin dependencias externas
// salvo las fuentes de Google, que tienen respaldo). Funciona sin internet,
// desde una memoria USB o abierto directo en el celular.
//   dist/index.html    → documento completo (para abrir local o subir a cualquier hosting)
//   dist/artifact.html → mismo contenido sin <html>/<head>/<body> (para publicar como página)
import { build } from 'esbuild';
import { readFileSync, writeFileSync, mkdirSync, existsSync } from 'node:fs';

// Ranura del logo INKCAP (spec F): si existe src/marca/logo-inkcap.png se incrusta
// como data URI; si no, se usa el texto "INKCAP" en tipografía de rótulo.
// No se inventa un logo: el archivo lo pone la marca.
const LOGO = 'src/marca/logo-inkcap.png';
const inkcap = existsSync(LOGO)
  ? `<img class="inkcap-logo" src="data:image/png;base64,${readFileSync(LOGO).toString('base64')}" alt="INKCAP">`
  : '<b class="inkcap-word">INKCAP</b>';

// Marca para el juego (texturas): logo + diseños de artistas presentes en src/marca/
const mime = (f) => (f.endsWith('.png') ? 'image/png' : 'image/jpeg');
const dataUri = (f) => `data:${mime(f)};base64,${readFileSync(f).toString('base64')}`;
let artistas = [];
try { artistas = JSON.parse(readFileSync('src/marca/artistas.json', 'utf8')); } catch { /* sin manifiesto */ }
artistas = artistas.filter((a) => existsSync(`src/marca/artistas/${a.id}.jpg`)).map((a) => ({ ...a, img: dataUri(`src/marca/artistas/${a.id}.jpg`) }));
const marca = { logo: existsSync(LOGO) ? dataUri(LOGO) : null, artistas };
const marcaScript = `<script>window.__MARCA=${JSON.stringify(marca)};</script>`;

const result = await build({
  entryPoints: ['src/main.js'],
  bundle: true,
  minify: true,
  format: 'iife',
  target: ['es2020', 'safari15'],
  write: false,
  legalComments: 'none',
});
let js = result.outputFiles[0].text.replace(/<\/script/gi, '<\\/script');

const tpl = readFileSync('src/index.template.html', 'utf8').replaceAll('<!--@inkcap-->', inkcap);
const [, afterHead] = tpl.split('<!--@head-->');
const [headPart, rest] = afterHead.split('<!--@body-->');
const [bodyPart] = rest.split('<!--@script-->');
const script = `${marcaScript}\n<script>${js}</script>`;

mkdirSync('dist', { recursive: true });
const fragment = `${headPart.trim()}\n${bodyPart.trim()}\n${script}\n`;
writeFileSync('dist/artifact.html', fragment);
const full = `<!doctype html>\n<html lang="es">\n<head>\n<meta charset="utf-8">\n<meta name="viewport" content="width=device-width,initial-scale=1,viewport-fit=cover">\n${headPart.trim()}\n</head>\n<body>\n${bodyPart.trim()}\n${script}\n</body>\n</html>\n`;
writeFileSync('dist/index.html', full);
console.log(`OK: dist/index.html ${(full.length / 1024).toFixed(0)} KB · logo INKCAP: ${existsSync(LOGO) ? 'imagen incrustada' : 'texto (no hay src/marca/logo-inkcap.png)'} · diseños de artistas: ${artistas.length}`);
