DISEÑOS DE PLAYERAS · GORRAZO by INKCAP
========================================

Deja aquí los diseños de playera de los artistas. El juego los carga SOLOS
al abrir el vestidor (Resources.LoadAll<Texture2D>("Disenos/Playeras")):
no hay que tocar código ni escenas.

Formato
- PNG cuadrado, de 512×512 a 1024×1024 px (potencia de 2: 512 o 1024 es lo ideal).
- Fondo lleno de UN color liso: ese color es la tela de la playera. El
  estampado va al centro, con aire alrededor. (Si el PNG trae fondo
  transparente también sirve: la tela sale blanca o negra, la que más
  contraste con el estampado.)
- El NOMBRE DEL ARCHIVO es el nombre del diseño en el vestidor.
  Ejemplo: "Tinta Brava - Ana R.png" sale como «Tinta Brava - Ana R».
  Evita el carácter | en el nombre (se usa internamente).
- Si un PNG se llama igual que un diseño de ejemplo (p. ej. "basica-negra"),
  el PNG lo reemplaza.

Cómo se ve (v0.3)
- El cuerpo y las mangas del muñequito se pintan del color de la tela (el
  color dominante del PNG) y el estampado va como calcomanía al frente y
  en la espalda: el juego encuentra solo la zona del dibujo y la recorta,
  así que no se estira alrededor del cuerpo.
- El mismo PNG sale completo en los cuadros y pósters de los escenarios,
  y recortado en las playeras colgadas de la tienda y la azotea.
- Cuando llegue el modelo final, su UV de playera usará este mismo PNG.

Precio
- Cada diseño de artista cuesta 300 Gotas en el vestidor
  (DesignLibrary.ArtistShirtPrice). No hay compras con dinero real en v0.2.

Ajustes de importación en Unity
- Los pone solo Scripts/Editor/BrandTextureImport.cs la primera vez que
  se importa el PNG (Wrap Mode: Repeat · Non-Power of 2: None). NO hace
  falta activar Read/Write.
- Texture Type: Default · Max Size: 1024 · Compresión: Automatic (ASTC en
  celular).
