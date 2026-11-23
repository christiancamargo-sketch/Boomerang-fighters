DISEÑOS DE PLAYERAS · GORRAZO by INKCAP
========================================

Deja aquí los diseños de playera de los artistas. El juego los carga SOLOS
al abrir el vestidor (Resources.LoadAll<Texture2D>("Disenos/Playeras")):
no hay que tocar código ni escenas.

Formato
- PNG cuadrado, de 512×512 a 1024×1024 px (potencia de 2: 512 o 1024 es lo ideal).
- Fondo lleno (sin transparencia): es la tela de la playera.
- El NOMBRE DEL ARCHIVO es el nombre del diseño en el vestidor.
  Ejemplo: "Tinta Brava - Ana R.png" sale como «Tinta Brava - Ana R».
  Evita el carácter | en el nombre (se usa internamente).
- Si un PNG se llama igual que un diseño de ejemplo (p. ej. "basica-negra"),
  el PNG lo reemplaza.

Cómo se ve
- La imagen se pone como textura del cuerpo del muñequito (material
  mainTexture). En el greybox el cuerpo es una cápsula: la imagen da la
  vuelta completa, así que lo importante debe ir al centro.
- Cuando llegue el modelo final, su UV de playera usará este mismo PNG.

Precio
- Cada diseño de artista cuesta 300 Gotas en el vestidor
  (DesignLibrary.ArtistShirtPrice). No hay compras con dinero real en v0.2.

Ajustes de importación recomendados en Unity
- Texture Type: Default · Wrap Mode: Repeat · Max Size: 1024
- Compresión: Automatic (ASTC en celular).
