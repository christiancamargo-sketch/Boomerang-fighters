# Marca INKCAP en el prototipo

Todo lo de esta carpeta sale de la tienda **www.inkcap.com.mx** y se regenera con:

```bash
pip install pillow
python3 prototipo/herramientas/preparar_marca.py
cd prototipo && node build.mjs
```

| Archivo | Qué es | ¿Se sube a GitHub? |
|---|---|---|
| `logo-inkcap.png` | Logo oficial (blanco sobre transparente). Sale en menú, carga, resultados, gorras y playera Basic. | Sí |
| `artistas.json` | Lista de diseños de artistas (nombre, artista, precio en tienda, color de tela). | Sí |
| `artistas/*.jpg` | Recortes **provisionales** de las fotos de producto de cada artista. | **No** (repo público; falta la licencia digital) |

Para reemplazar un recorte por el arte original del artista, pon un JPG cuadrado con el mismo nombre en `artistas/`.
El juego también trae las 6 gorras snapback reales (negra, roja, azul rey, azul marino, gris, jaspe) y la playera INKCAP Basic negra, dibujadas con el logo.
