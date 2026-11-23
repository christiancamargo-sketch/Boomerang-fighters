# Spec v0.2 · versión local (Wi‑Fi) con marca INKCAP

**Decisión del cliente (23-nov-2026):** ir por la ruta más barata. El juego sale **solo con multijugador local por Wi‑Fi/hotspot** (sin servidores, sin backend, progreso guardado en el celular), con la marca INKCAP y todas las mejoras del GDD §4.3 que se puedan implementar. El online queda para después; la arquitectura de 3 capas no cambia.

Esta spec es la fuente única para **tres implementaciones que deben quedar iguales**:
- `unity/Assets/BoomerangFighters/Scripts/Core` (C#, la verdad del juego)
- `prototipo/src` (web, la demo jugable)
- `unity/Assets/BoomerangFighters/Scripts/Unity` (presentación, progreso local, marca)

Nombre mostrado: **GORRAZO** · sello **by INKCAP** · subtítulo **"Lanza tu gorra, mancha a todos"**.

---

## A. Reglas de combate (núcleo)

| # | Regla | Detalle exacto | Interruptor |
|---|---|---|---|
| A1 | Doble KO = empate | Si el último jugador vivo cae menos de **6 ticks (0.1 s)** después del penúltimo, la ronda es empate: nadie suma. Implementación: al quedar ≤1 vivo, no declarar ganador de inmediato; esperar 6 ticks; si en ese lapso cae el último, empate. | siempre |
| A2 | Muerte súbita | A los **20 s** de ronda en juego: aviso de **1 s** (evento `SuddenDeathWarn`), luego los muros de los **lados cortos** (izq. y der., eje X) avanzan **0.5 m/s** hacia el centro hasta dejar **8 m** de ancho jugable. **Empujan, no matan** (los jugadores y gorras se empujan hacia adentro). A los **45 s** la ronda termina en **empate**. Al empezar cada ronda los muros regresan. | `Rules.SuddenDeath` (true) |
| A3 | Empujón sin gorra | GOLPE **sin gorra**: no noquea; empuja al rival que esté en el arco del golpe **2.5 m en 0.2 s** (como un dash forzado), recarga **0.8 s**. Si toca una gorra **ajena caída** en el arco, la patea **4 m**. El empujón puede mandar a alguien a un peligro (láser). | `Rules.Shove` (true) [A PRUEBA] |
| A4 | Desvío "Te la regreso" | Si la ventana activa del golpe (0.13 s) toca una gorra rival **letal**, en vez de tirarla sale **hacia su dueño** a **1.2× su velocidad (máx. 22 m/s)** y durante **0.6 s** puede **noquear a su propio dueño** (y a otros, excepto a quien desvió). Después vuelve a ser una gorra normal en regreso. Visual: rastro rayado con el color de quien desvió. Si el interruptor está apagado: comportamiento actual (la gorra cae). | `Rules.DeflectReturn` (true) [A PRUEBA] |
| A5 | Sticker XL | La gorra gigante va **10 % más lenta** (velocidad × 0.9). | siempre |
| A6 | ¡CLANK! | Radio de choque entre dos gorras = **85 %** de la suma de radios. | siempre |
| A7 | Puntos | Local: **5** por defecto (opciones 3/5/7). | — |
| A8 | Letreros de KO | El evento KO lleva una causa detallada: `Gorrazo` (gorra de ida), `DeRegreso` (gorra regresando o llamada), `DeBanda` (gorra que ya rebotó en pared), `TeLaRegreso` (gorra desviada), `Zaz` (golpe), `Zap` (láser), `Splash` (Charco), más `Tiraje` (copia de Serigrafía). Textos: ¡GORRAZO!, ¡DE REGRESO!, ¡DE BANDA!, ¡TE LA REGRESO!, ¡ZAZ!, ¡ZAP!, ¡SPLASH!, ¡TIRAJE!. Empate: ¡EMPATE!. | — |

## B. Latas de técnica (power-ups)

Reglas: **1 lata a la vez** (la nueva reemplaza), se pierde con el KO, el Parche se gasta al bloquear. Latas **negro tinta / blanco hueso con ícono**, nunca de color de jugador.

| Lata | Efecto v0.2 |
|---|---|
| Serigrafía · Tiraje | Ya hecho: 2 copias en fila, 0.85 m, no regresan. |
| Tatuaje · Trazo | Ya hecho (persigue al más cercano, 4.5 rad/s, 11 m). **Nuevo:** cuando la gorra empieza a perseguir a alguien, evento `Marked(target)` para que la vista muestre un ícono sobre la víctima. |
| Tinta derramada · Charco | **Cambio:** al chocar (pared, obstáculo alto o jugador) deja un **charco** que crece de 0 a **2.0 m** de radio en **0.4 s** (visible) y al terminar de crecer **noquea a quien siga dentro** (excepto al dueño; el Parche bloquea). Evento `PuddleStart` al empezar y `Explode` al detonar. |
| Grafiti · Muro de Tag | **Cambio:** al terminar la ida (o al rebotar) pinta un **muro de esténcil de 2.5 × 0.4 m**, perpendicular a la dirección de vuelo, centrado en la gorra, que **bloquea gorras y jugadores 3 s**. Evento `TagWall`. Respaldo con interruptor `Rules.GraffitiWall=false`: rastro que alenta (comportamiento actual). |
| Parche bordado | Sin cambio. |
| Sticker XL | Gigante y −10 % de velocidad (A5). |

## C. Modos de juego (todos locales, gratis)

| Modo | Reglas |
|---|---|
| **¡Gorrazo!** | El actual: último en pie gana la ronda. |
| **Lata Caliente** | 3–6 jugadores. Al empezar la ronda (tras la cuenta), una "lata caliente" se pega a un jugador al azar (Rng). Mientras alguien la tiene, **los gorrazos y golpes no noquean**: si la gorra o el golpe de quien la tiene toca a otro, **le pasa la lata** (evento `HotCanPass`), con 0.5 s de inmunidad para que no regrese al instante. La lata explota entre **10 y 20 s** (aleatorio por Rng) y quien la tenga queda fuera (`HotCanExplode` + KO causa `Splash`). Luego se pega otra al azar entre los vivos. Último en pie gana. Láser y muerte súbita siguen activos (muerte súbita: solo empuja; sin empate a los 45 s en este modo). Sin latas de técnica en este modo. |
| **La Reta** | 2–8 jugadores en el lobby; solo **2 pelean** a la vez (1 contra 1). El ganador se queda; el perdedor va al final de la fila; entra el siguiente. Los que esperan no aparecen en la arena (o aparecen como espectadores fuera del área). Gana la partida quien primero encadene **3 victorias seguidas** o quien tenga más victorias al llegar a **10 rondas**. En empate de ronda, ambos se quedan. |
| **Reto del Día** | 1 jugador contra 3 bots en dificultad Difícil, con **semilla del día** (`AAAAMMDD`), arena y "regla del día" elegidas con esa semilla. Puntaje = KOs × 100 + rondas ganadas × 300 − segundos totales. Se guarda el mejor puntaje del día en el celular (tabla local de los últimos 7 días). Sin servidor. |

`MatchConfig` gana `Mode` (enum `GameMode { Gorrazo, LataCaliente, LaReta, RetoDelDia }`) y `Rules` (interruptores de A2–A4 y B). Los modos y reglas viajan en el `StartMatch` de la red LAN.

## D. Bots

- Usan **golpe** cuando el rival está a menos de 1.6 m (ya existe) y **desvían**: si una gorra rival letal va a pasar a menos de 1.8 m en los próximos 0.25 s y tienen su gorra, intentan golpe hacia ella (probabilidad según dificultad: 0.15 / 0.35 / 0.6).
- Sin gorra, usan **empujón** si un rival está a menos de 1.4 m (probabilidad 0.3 por segundo).
- En Lata Caliente: si tienen la lata, persiguen al más cercano; si no, huyen de quien la tiene.

## E. Progreso local (sin backend)

- XP por nivel: **150 + 50 × (n − 1)**, niveles 1–50.
- XP por partida: jugar 20, ronda ganada 10, KO 6, ganar partida 50. Primera victoria del día +50.
- **Gotas** (moneda blanda): 10–25 por partida; se gastan en el vestidor.
- Premio diario 7 días: 150 Gotas · sticker (cosmético simple) · 250 Gotas · 25 Gotas extra · emote (cosmético simple) · 100 Gotas · **gorra de la semana** (diseño de gorra INKCAP). Racha **suave**: si faltas, se pausa (no regresa al día 1).
- Calendario de 28 días → "Playera del mes".
- Vestidor: **playeras** (textura del diseño) y **gorras** (diseño en la copa; la silueta y el tamaño nunca cambian; el color del jugador vive en el aro y el rastro).
- Muñequitos desbloqueados por nivel: Lienzo, Osote, Michi (gratis) · Gotita nv 2 · Seri nv 4 · Zurci nv 6 · Pegote nv 10 · Axo nv 15. (El prototipo web usa sus 6 especies actuales con estos nombres + agrega Lienzo y Axo si es fácil.)
- Sin compras con dinero real en v0.2. El canje de códigos se muestra como **simulación del portal web**.

## F. Marca INKCAP

- Título "GORRAZO" con sello "by INKCAP" en menú, pantalla de carga y resultados.
- **Ranura de logo:** si existe el archivo del logo (`prototipo/src/marca/logo-inkcap.png` en web, `Assets/BoomerangFighters/Resources/Marca/logo_inkcap.png` en Unity), se muestra; si no, se usa el texto "INKCAP" en tipografía de rótulo. **No inventar un logo.**
- Ranura de diseños de artistas: PNG cuadrados en `Resources/Disenos/Playeras/*.png` y `Resources/Disenos/Gorras/*.png` (Unity) se cargan solos en el vestidor; el nombre del archivo es el nombre del diseño. En web, los diseños procedurales actuales siguen siendo los de ejemplo.

## G. Multijugador local (Unity)

- Android: adquirir `WifiManager.MulticastLock` al buscar/anunciar salas (vía `AndroidJavaObject`, sin plugin Java); permisos en el manifiesto: INTERNET, ACCESS_NETWORK_STATE, ACCESS_WIFI_STATE, CHANGE_WIFI_MULTICAST_STATE (y nota para `ACCESS_LOCAL_NETWORK` en API 37).
- iOS: script de editor post-build que agrega `NSLocalNetworkUsageDescription` y `NSBonjourServices` al Info.plist.
- Unirse por IP (ya existe) y por texto `bf://IP:PUERTO/Sala` (ya existe). Mostrar IP grande en la sala del anfitrión.
