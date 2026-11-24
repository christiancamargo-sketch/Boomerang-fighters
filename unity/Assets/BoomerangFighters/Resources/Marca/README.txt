RANURA DEL LOGO DE INKCAP
=========================

Si existe un archivo llamado  logo_inkcap.png  en esta carpeta, el juego lo
dibuja en el sello "by INKCAP" del menú, la pantalla de carga y los
resultados (Resources.Load<Texture2D>("Marca/logo_inkcap")).

Desde v0.3 también sale en los escenarios: mural del callejón, manta del
museo, espectacular y tinacos de la azotea, pared y piso de la Pop-up
Store. Ahí el juego lo usa de plantilla (su transparencia) y lo pinta del
color que toque sobre un panel liso.

Si NO existe, el juego escribe el texto "GORRAZO · by INKCAP" en los menús
y pone paneles de color liso en los escenarios.
No se debe inventar ni dibujar un logo provisional: solo va el archivo
oficial que entregue INKCAP.

Formato recomendado
- PNG con transparencia (fondo transparente), versión clara para fondo oscuro.
- Alto mínimo 256 px; se escala conservando la proporción.
- Ajustes de importación: los pone solo Scripts/Editor/BrandTextureImport.cs
  la primera vez que se importa (Non-Power of 2 = None, Alpha Is
  Transparency). IMPORTANTE: con "Non-Power of 2" en su valor por defecto
  Unity estira el logo (1200 × 364 → 1024 × 512) y sale deformado; si ya
  estaba importado de antes, cámbialo a "None" en el Inspector.
- Texture Type "Default" o "Sprite (2D and UI)"; sin compresión con pérdida
  si se ve borroso. No hace falta activar Read/Write.
