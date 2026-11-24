# Spec v0.3 · Escenarios más grandes, vacíos y mundo INKCAP

**Pedido del cliente (24-nov-2026):** mejorar los escenarios, agregar **vacíos** (huecos por donde te caes), hacerlos **un poco más grandes**, mejorar todo y **meter cosas de INKCAP** en el juego.

Igual que en v0.2, esta spec es la fuente única para el núcleo C# (`unity/.../Core`), el prototipo web (`prototipo/src`) y la capa Unity (`unity/.../Unity`). Coordenadas de la simulación: `x` a la derecha, `y` hacia la cámara (abajo en pantalla). Obstáculos y vacíos son rectángulos `(centroX, centroY, ancho, fondo)`.

---

## A. Vacíos (regla nueva)

| # | Regla | Detalle exacto |
|---|---|---|
| V1 | Caída | Si el **centro** de un jugador vivo queda dentro de un vacío (rectángulo reducido 0.15 m por lado) y **no está en dash** (`DashT <= 0`) ni empujado (`PushT <= 0`), cae: KO con causa nueva **`Vacio`**, letrero **¡AL VACÍO!**. Un dash o empujón puede cruzar un vacío; si termina encima, cae. |
| V2 | Crédito | Si en los últimos **2 s** alguien lo empujó (empujón A3), le pegó con un golpe, lo desvió o lo tocó con una gorra con escudo roto, ese jugador se lleva el KO. Si no, el KO no es de nadie (como el láser). |
| V3 | Gorras | Las gorras **en vuelo** pasan por encima de los vacíos. Una gorra **en el piso** que queda dentro de un vacío regresa sola de inmediato (como un *recall*). |
| V4 | Aparición | Ni jugadores, ni latas, ni la caja DROP aparecen dentro de un vacío (ni a menos de 0.8 m de uno). `RandomFreePoint` debe respetarlo. |
| V5 | Bots | Tratan los vacíos como obstáculos al moverse, no hacen dash hacia un vacío y se alejan si quedan a menos de 0.8 m de una orilla. |
| V6 | Muerte súbita | Los muros que avanzan pueden empujar a alguien a un vacío (es válido y divertido). |
| V7 | Vista | El vacío se ve como un hueco profundo y oscuro con orilla de franjas amarillo/negro (o el estilo del escenario); al caer, el muñequito se hace chiquito hacia abajo y suena un "fuuu". |

## B. Escenarios (más grandes)

Tamaño base **26 × 16 m** (antes 23 × 14). La cámara se ajusta sola. Obstáculos con altura ≥ 1 bloquean gorras.

### B1. Callejón Grafiti (26 × 16)
- **Obstáculos:** contenedores (−6.5, −3.2, 2.6, 1.3, h 1.4) y (6.5, 3.2, 2.6, 1.3, h 1.4); caja (0, 0, 1.2, 1.2, h 1.2); cajas bajas (−2.4, 4.6, 1, 1, h 0.7) y (2.4, −4.6, 1, 1, h 0.7); torres de cajas (−10.2, 3.8, 1, 2.4, h 1.3) y (10.2, −3.8, 1, 2.4, h 1.3).
- **Vacíos:** coladeras abiertas (−5.6, 4.9, 1.4, 1.4) y (5.6, −4.9, 1.4, 1.4); zanjas de obra (0, 3.0, 3.4, 0.9) y (0, −3.0, 3.4, 0.9).
- **Spawns:** (−11, −6.3), (11, 6.3), (11, −6.3), (−11, 6.3), (−3.5, 0), (3.5, 0).
- **INKCAP:** mural grande con el logo en el muro del fondo; *paste-ups* (pósters pegados) con los diseños de artistas; stickers INKCAP en contenedores; esténcil del logo en el piso.

### B2. Museo de Arte · "Exposición INKCAP" (26 × 16)
- **Obstáculos:** pedestales (±5.5, ±2.8, 1.4, 1.4, h 1.5) (4); banca (0, 0, 3.2, 0.7, h 0.55).
- **Vacíos:** fosos de instalación (−9.2, 0, 1.8, 3.2) y (9.2, 0, 1.8, 3.2), con orilla de neón.
- **Peligro:** láseres (sin cambio).
- **Spawns:** (−11, −6.3), (11, 6.3), (11, −6.3), (−11, 6.3), (0, −6.6), (0, 6.6).
- **INKCAP:** los cuadros de las paredes son los diseños de los artistas (Spooky, Mandala, Drop Cabra, La Cafeína…) con cédula "Artista · INKCAP"; manta "EXPOSICIÓN INKCAP" en el fondo; una vitrina con las 6 gorras reales sobre un pedestal (decorativa, fuera del área de juego).

### B3. Azotea CDMX (26 × 16) · dos azoteas con hueco entre edificios
- **Obstáculos:** tinacos (−6.5, 0, 1.6, 1.6, h 1.8) y (6.5, 0, 1.6, 1.6, h 1.8); macetas (−6.5, −4.4, 2.6, 0.5, h 0.6) y (6.5, 4.4, 2.6, 0.5, h 0.6); tanques de gas (−10.8, −5.6, 1.1, 1.1, h 1.0) y (10.8, 5.6, 1.1, 1.1, h 1.0).
- **Vacíos:** hueco entre edificios con un **puente de tablón** de 3 m en medio: (0, −4.75, 1.8, 6.5) y (0, 4.75, 1.8, 6.5).
- **Spawns:** (−10, 0), (10, 0), (−4, −6.2), (4, 6.2), (−4, 6.2), (4, −6.2).
- **INKCAP:** espectacular (anuncio grande) "GORRAZO by INKCAP" con el logo al fondo; tendedero con playeras INKCAP reales (estampados de artistas); logo pintado en un tinaco.

### B4. Pop-up Store INKCAP (24 × 15) · escenario nuevo
- **Obstáculos:** racks de playeras (−5, −3.2, 2.6, 0.6, h 1.4), (5, 3.2, 2.6, 0.6, h 1.4), (−5, 3.2, 2.6, 0.6, h 1.4), (5, −3.2, 2.6, 0.6, h 1.4); mesa de gorras (0, 0, 2.2, 1.2, h 0.9) (baja: las gorras pasan).
- **Vacíos:** escalera al sótano (−9.6, 4.6, 1.6, 2.2) y (9.6, −4.6, 1.6, 2.2).
- **Peligro nuevo: caja DROP.** Cada **20 s** (primera a los 8 s) cae una caja del techo en un punto libre al azar (Rng): **sombra de aviso 1.5 s** (evento `DropWarn` con posición), luego cae (`DropLand`): noquea a quien esté a **≤ 1.1 m** (causa `Drop`, letrero **¡DROP!**, el escudo bloquea), y deja una **lata de técnica** en ese lugar. La caja desaparece al caer.
- **Spawns:** (−9.5, −5.5), (9.5, 5.5), (6.5, −5.5), (−6.5, 5.5), (0, −5.8), (0, 5.8). *(Corregido: dos spawns caían dentro de los vacíos de la escalera.)*
- **INKCAP:** pared con el logo grande e iluminado; racks con las playeras reales (Basic y artistas); pared de gorras con las 6 snapbacks; mostrador con la taza INKCAP; cartel "ESTRENOS".

## C. Más INKCAP en todo el juego
- **Latas de técnica:** etiqueta con el logo INKCAP (negro tinta con banda blanca y el ícono de la técnica).
- **Resultados:** el ganador levanta la **taza INKCAP** (trofeo) en la pantalla de resultados / podio.
- **Menú:** el escenario de fondo del menú es la Pop-up Store.
- **Arena "Aleatorio"** incluye los 4.

## D. Balance y pruebas
- Correr el balance (`HeadlessHost balance`) en los 4 escenarios: rondas de 10–25 s promedio, y caídas al vacío entre 5 % y 25 % de los KOs.
- Pruebas nuevas: caer al vacío, cruzar con dash, crédito por empujón, gorra en el piso dentro de un vacío regresa, nada aparece en vacíos, caja DROP (aviso, KO, deja lata), bots que no se caen solos (en 30 partidas de bots, < 15 % de KOs por vacío sin crédito), determinismo con el escenario nuevo, snapshot con la caja DROP.

## E. Aclaraciones (24-nov-2026, acordadas durante la implementación)

Valen igual para el núcleo C#, el prototipo web y Unity:

1. **Caja DROP, tiempos:** se cuentan sobre el reloj de la ronda en juego (desde el "¡YA!"). La caja **cae** a los 8 s, 28 s, 48 s…; el aviso `DropWarn` sale 1.5 s antes (6.5 s, 26.5 s…). Noquea a distancia centro a centro ≤ 1.1 m. En Lata Caliente, o con las latas apagadas, cae y noquea pero no deja lata.
2. **V3, gorra en el piso:** regresa sola si su **centro** está dentro del rectángulo completo del vacío. La reducción de 0.15 m por lado es solo para jugadores (V1).
3. **Reto del Día:** la arena del día sale de las 4 arenas en este orden: callejon, museo, azotea, popup (`All[rng.NextInt(4)]`). Ejemplos: 20261123 → museo, 20261201 → azotea, 20270115 → popup, 20260228 → azotea. La regla del día no cambia.
