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
- La silueta y el tamaño de la gorra-proyectil nunca cambian (todos deben
  leerla igual). La gorra puesta en la cabeza se dibuja un poco más grande.
- El color del jugador vive en el aro del piso, el rastro y el botón de la
  copa; no hace falta pintarlo en el diseño.

Cómo se ve (v0.3)
- Fondo de UN color liso: ese color es la COPA (y, un poco más oscuro, la
  visera). El logo o dibujo va SOLO en el panel de enfrente: el juego
  encuentra la zona del dibujo y la recorta para ese panel.
- Las gorras INKCAP de esta carpeta son plantillas de 512 × 512 con el logo
  repetido en x ≈ 128 y x ≈ 384: se usa el de la izquierda. Un PNG con un
  solo logo o dibujo al centro también funciona.

Precio
- Cada diseño de artista cuesta 150 Gotas (DesignLibrary.ArtistCapPrice).

Ajustes de importación en Unity
- Los pone solo Scripts/Editor/BrandTextureImport.cs la primera vez que
  se importa el PNG (Wrap Mode: Repeat · Non-Power of 2: None). NO hace
  falta activar Read/Write.
- Texture Type: Default · Max Size: 1024
