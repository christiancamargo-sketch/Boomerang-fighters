RANURA DEL LOGO DE INKCAP
=========================

Si existe un archivo llamado  logo_inkcap.png  en esta carpeta, el juego lo
dibuja en el sello "by INKCAP" del menú, la pantalla de carga y los
resultados (Resources.Load<Texture2D>("Marca/logo_inkcap")).

Si NO existe, el juego escribe el texto "GORRAZO · by INKCAP".
No se debe inventar ni dibujar un logo provisional: solo va el archivo
oficial que entregue INKCAP.

Formato recomendado
- PNG con transparencia (fondo transparente), versión clara para fondo oscuro.
- Alto mínimo 256 px; se escala conservando la proporción.
- Ajustes de importación: Texture Type "Default" o "Sprite (2D and UI)",
  Alpha Is Transparency activado, sin compresión con pérdida si se ve borroso.
