DISEÑOS DE GORRAS · GORRAZO by INKCAP
======================================

Deja aquí los diseños de gorra de los artistas. El juego los carga SOLOS
(Resources.LoadAll<Texture2D>("Disenos/Gorras")) y aparecen en el vestidor.

Formato
- PNG cuadrado, de 512×512 a 1024×1024 px (512 o 1024 es lo ideal).
- El NOMBRE DEL ARCHIVO es el nombre del diseño en el vestidor.
  Evita el carácter | en el nombre (se usa internamente).
- Si un PNG se llama igual que un diseño de ejemplo (p. ej. "cuadros"),
  el PNG lo reemplaza.

Reglas de la gorra (spec v0.2 §E)
- El diseño va SOLO en la COPA. La silueta y el tamaño de la gorra nunca
  cambian (es el proyectil: todos deben leerla igual).
- El color del jugador vive en el aro del piso, el rastro y el botón de la
  copa; no hace falta pintarlo en el diseño.
- La visera de una gorra con diseño sale negro tinta.

Cómo se ve
- En el greybox la copa es media esfera: la imagen la envuelve, así que lo
  importante (rótulo, ícono) debe ir en la franja central del PNG.

Precio
- Cada diseño de artista cuesta 150 Gotas (DesignLibrary.ArtistCapPrice).

Ajustes de importación recomendados en Unity
- Texture Type: Default · Wrap Mode: Repeat · Max Size: 1024
