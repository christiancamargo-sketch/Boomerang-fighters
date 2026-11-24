#!/usr/bin/env python3
"""
Descarga los recursos de la tienda INKCAP (www.inkcap.com.mx) y prepara
los archivos que usan el prototipo web y el proyecto Unity.

    pip install pillow
    python3 prototipo/herramientas/preparar_marca.py

Genera:
  • prototipo/src/marca/logo-inkcap.png                (se sube a git)
  • prototipo/src/marca/artistas/*.jpg                 (NO se sube: licencia de artistas)
  • unity/.../Resources/Marca/logo_inkcap.png          (se sube a git)
  • unity/.../Resources/Disenos/Gorras/INKCAP *.png    (se sube a git)
  • unity/.../Resources/Disenos/Playeras/INKCAP *.png  (se sube a git)
  • unity/.../Resources/Disenos/Playeras/Artista - *.png (NO se sube)

Los diseños de artistas se recortan de las fotos de producto de la tienda.
Son provisionales: lo ideal es reemplazarlos por el archivo original del
artista (PNG de alta resolución) cuando firme la licencia digital.
"""
import io
import json
import os
import ssl
import sys
import urllib.request

from PIL import Image, ImageDraw, ImageOps

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..'))
WEB = os.path.join(ROOT, 'prototipo', 'src', 'marca')
UNITY = os.path.join(ROOT, 'unity', 'Assets', 'BoomerangFighters', 'Resources')
SITE = 'https://www.inkcap.com.mx'

LOGO_URL = SITE + '/images/inkcap.png'
BASIC_SHIRT_URL = SITE + '/productos/1780453040_playera-inkcap-basic-negra1.webp'

# Gorras snapback reales de la tienda ($499 MXN). Colores tomados de las fotos.
CAPS = [
    # id, nombre, copa, logo
    ('inkcap-negra', 'INKCAP Negra', '#161518', '#f2f2f2'),
    ('inkcap-roja', 'INKCAP Roja', '#c8102e', '#f7f7f7'),
    ('inkcap-azul-rey', 'INKCAP Azul Rey', '#1f3fa3', '#f2f2f2'),
    ('inkcap-azul-marino', 'INKCAP Azul Marino', '#1b1f3b', '#ededed'),
    ('inkcap-gris', 'INKCAP Gris', '#4b4a50', '#d9d9dc'),
    ('inkcap-jaspe', 'INKCAP Jaspe', '#9d9c9f', '#3a393d'),
]

# Diseños de artistas: recorte (x0, y0, x1, y1) en píxeles de la foto original.
ARTISTS = [
    {
        'id': 'cabra-chava-chavs', 'name': 'Drop Cabra', 'artist': 'Chava Chavs', 'price': 349,
        'url': SITE + '/productos/1775092983_1774480030-69c46a9e3d011-whatsapp-image-2026-02-21-at-142205.webp',
        'crop': (310, 330, 920, 940), 'fabric': '#121212',
    },
    {
        'id': 'mandala-alien-inkspiral', 'name': 'Mandala', 'artist': 'Alien Inkspiral', 'price': 649,
        'url': SITE + '/productos/1785194259_alien2-inkcap.webp',
        'crop': (192, 491, 523, 875), 'fabric': None,
    },
    {
        'id': 'spooky-joann-spooky', 'name': 'Spooky', 'artist': 'Joann Spooky', 'price': 649,
        'url': SITE + '/productos/1790092262_spooky5.webp',
        'crop': (245, 480, 672, 1131), 'fabric': None,
    },
    {
        'id': 'cafeina-jotch', 'name': 'La Cafeína me Domina', 'artist': 'Jotch', 'price': 649,
        'url': SITE + '/productos/1793240191_inkcap-jotch-6.webp',
        'crop': (192, 480, 587, 1067), 'fabric': '#eeede9',
    },
]


def fetch(url):
    ctx = ssl.create_default_context(cafile=os.environ.get('SSL_CERT_FILE'))
    req = urllib.request.Request(url, headers={'User-Agent': 'gorrazo-marca/1.0'})
    with urllib.request.urlopen(req, context=ctx, timeout=60) as r:
        return Image.open(io.BytesIO(r.read()))


def hexrgb(h):
    h = h.lstrip('#')
    return tuple(int(h[i:i + 2], 16) for i in (0, 2, 4))


def tinted_logo(logo, color, width):
    """Logo (alfa del original) pintado de un color sólido."""
    alpha = logo.split()[3]
    solid = Image.new('RGBA', logo.size, hexrgb(color) + (255,))
    solid.putalpha(alpha)
    h = round(logo.height * width / logo.width)
    return solid.resize((width, h), Image.LANCZOS)


def save(img, *parts, quality=None):
    path = os.path.join(*parts)
    os.makedirs(os.path.dirname(path), exist_ok=True)
    if quality:
        img.convert('RGB').save(path, quality=quality, optimize=True)
    else:
        img.save(path, optimize=True)
    print('  ✓', os.path.relpath(path, ROOT))


def main():
    print('Descargando logo…')
    logo = fetch(LOGO_URL).convert('RGBA')
    logo = logo.crop(logo.getbbox())                     # quitar bordes vacíos
    logo_w = logo.resize((1200, round(logo.height * 1200 / logo.width)), Image.LANCZOS)
    save(logo_w, WEB, 'logo-inkcap.png')
    save(logo_w, UNITY, 'Marca', 'logo_inkcap.png')

    print('Gorras INKCAP para Unity (textura de copa 512×512, logo al frente y atrás)…')
    for cid, name, crown, ink in CAPS:
        tex = Image.new('RGBA', (512, 512), hexrgb(crown) + (255,))
        mark = tinted_logo(logo, ink, 150)
        for cx in (128, 384):
            tex.alpha_composite(mark, (cx - mark.width // 2, 150))
        save(tex, UNITY, 'Disenos', 'Gorras', f'{name}.png')

    print('Playera INKCAP Basic negra…')
    shirt = Image.new('RGBA', (1024, 1024), hexrgb('#141414') + (255,))
    mark = tinted_logo(logo, '#f2f2f2', 520)
    shirt.alpha_composite(mark, (512 - mark.width // 2, 470))
    save(shirt, UNITY, 'Disenos', 'Playeras', 'INKCAP Basic Negra.png')

    print('Diseños de artistas (recortes provisionales, no se suben a git)…')
    manifest = []
    for a in ARTISTS:
        src = fetch(a['url']).convert('RGB')
        art = src.crop(a['crop'])
        fabric = a['fabric']
        if not fabric:  # color de la tela: mediana del borde del recorte
            border = ImageOps.expand(art, border=0)
            px = [border.getpixel((x, 0)) for x in range(0, art.width, 7)] + \
                 [border.getpixel((0, y)) for y in range(0, art.height, 7)]
            fabric = '#%02x%02x%02x' % tuple(sorted(c[i] for c in px)[len(px) // 2] for i in range(3))
        side = max(art.size)
        sq = Image.new('RGB', (side, side), hexrgb(fabric))
        sq.paste(art, ((side - art.width) // 2, (side - art.height) // 2))
        sq = sq.resize((512, 512), Image.LANCZOS)
        save(sq, WEB, 'artistas', a['id'] + '.jpg', quality=86)
        big = Image.new('RGB', (1024, 1024), hexrgb(fabric))
        big.paste(sq.resize((620, 620), Image.LANCZOS), (202, 230))
        save(big, UNITY, 'Disenos', 'Playeras', f"Artista - {a['name']} ({a['artist']}).png")
        manifest.append({k: a[k] for k in ('id', 'name', 'artist', 'price')} | {'fabric': fabric, 'tienda': SITE + '/tienda'})

    with open(os.path.join(WEB, 'artistas.json'), 'w', encoding='utf-8') as f:
        json.dump(manifest, f, ensure_ascii=False, indent=2)
    print('  ✓ prototipo/src/marca/artistas.json')
    print('Listo. Recompila el prototipo: cd prototipo && node build.mjs')


if __name__ == '__main__':
    try:
        main()
    except Exception as e:  # mensaje claro si no hay internet o cambió la tienda
        print('No se pudo preparar la marca:', e, file=sys.stderr)
        sys.exit(1)
