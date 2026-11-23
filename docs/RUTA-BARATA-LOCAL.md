# Ruta barata: GORRAZO solo local por Wi‑Fi

**Decisión (23-nov-2026):** primera versión **sin juego en línea**. Se juega con amigos en el mismo Wi‑Fi o con el hotspot de un celular, contra bots, o en un mismo dispositivo con controles. Sin servidores ni backend; el progreso se guarda en el celular. La marca INKCAP va desde el día 1.

## Por qué es la más barata

| Rubro | Con online (plan original) | **Solo local (esta ruta)** |
|---|---|---|
| Servidores de juego (Relay / dedicados) | US$20–6,400 al mes según jugadores | **US$0** |
| Backend (cuentas, guardado, economía) | US$0–1,500 al mes | **US$0** (se guarda en el celular) |
| Programar red en línea, cuentas, tienda, portal de canje | US$22–56 mil (etapa 2 del plan) | **US$0** (no se hace todavía) |
| Pruebas con lag, antitrampas, moderación | Sí | **No hacen falta** |

El código ya está separado en 3 capas: cuando se quiera online, se agrega un transporte nuevo (Unity Relay) sin rehacer el juego.

## Lo que sí cuesta

| Concepto | Costo | ¿Obligatorio? |
|---|---|---|
| **Google Play** (cuenta de desarrollador) | **US$25 una vez** (~MXN 480) | Sí, para Android |
| Unity Personal | **$0** (empresa con menos de US$200 mil al año) | Sí |
| Servidores | **$0** | — |
| Celulares de prueba | $0 si usan los del equipo; MXN 20–28 mil si se compran 4–6 de gama baja/media | Recomendable 2–3 Android baratos (Moto G / Galaxy A) |
| **Apple App Store** | **US$99 al año** (~MXN 1,890) + compilar en Mac: Mac mini usada o nube (Codemagic tiene minutos gratis al mes) | Solo si sale en iPhone; **se puede dejar para después** |
| Registro de marca en el IMPI | ~MXN 3,000–3,500 por clase (mínimo clases 9 y 41; verificar tarifa 2026) | Muy recomendable antes de publicar |
| Aviso de privacidad / términos | MXN 0–5 mil (plantilla revisada) | Sí (las tiendas lo piden aunque no se recolecten datos) |
| Arte 3D (muñequitos, arenas) | **$0 externo** si lo hace la empresa 3D del cliente | — |
| Revisor senior de Unity (5–10 h/semana) | Opcional; ~MXN 6–15 mil al mes | Recomendable al empezar |
| Música y efectos | $0–1,150 (bibliotecas con licencia) | Sí, con licencia |

**Mínimo para publicar en Android:** **~US$25 + IMPI + aviso de privacidad ≈ MXN 7–12 mil**, si el equipo hace el arte y el código con IA.
**Con iPhone también:** + US$99 al año y una forma de compilar en Mac.

## Cómo gana dinero una versión solo local (opciones baratas)

Sin tienda dentro del juego todavía (eso requiere cuentas y validación de compras). Opciones que no necesitan servidor:

1. **Gratis como herramienta de marca:** el juego vende playeras y gorras. Botón "Consíguela real" que abre la tienda INKCAP (sin comisión de Apple/Google por ser producto físico).
2. **Precio único pequeño** en tienda (ej. $29–49 MXN), estilo *Boomerang Fu*: sin anuncios ni compras.
3. **Paquete de diseños** como compra única dentro de la app (Google Play Billing sin servidor es posible, con validación local; más simple que una economía completa).

Recomendación: **gratis + "Consíguela real"** en la primera versión, para medir si la gente juega y vuelve antes de cobrar.

## Qué limita jugar solo local

- **Reseñas de "no tiene online":** se mitiga presentándolo como "juego de fiesta para jugar juntos" y con bots buenos.
- **Wi‑Fi que aísla clientes** (cafés, escuelas, plazas): plan B con hotspot de un celular; la app lo explica.
- **iPhone:** pide permiso de "Red local"; para buscar salas automáticamente hace falta un permiso especial de Apple (multicast) o Bonjour. Mientras, se une por IP.
- **Android 17 (API 37):** pedirá un permiso nuevo de red local; ya está documentado.

## Pasos

1. Abrir el proyecto Unity (`unity/README.md`), probar en 2 celulares Android en el mismo Wi‑Fi.
2. Reemplazar los muñequitos greybox por los modelos de la empresa 3D y cargar logo y diseños INKCAP en las carpetas `Resources/`.
3. Cuenta de Google Play (US$25), prueba cerrada con 12+ testers por 14 días (requisito de cuentas personales nuevas).
4. Publicar en Android. iPhone después.
