# 04 · Multijugador local por Wi-Fi y online: arquitectura, herramientas, precios 2026 y plan para no reescribir el juego

> **Proyecto:** "Boomerang Fighters" (nombre de trabajo). Es un party brawler móvil para 2 a 8 jugadores, con partidas de 2 a 4 minutos y regla de "un golpe = KO". Los personajes son muñequitos estilo art toy y el arma es un objeto que se lanza y regresa. Se hace en Unity para iOS y Android, con juego cruzado entre ambos.
> **Fecha de la investigación:** 23 de noviembre de 2026.
> **Tipo de cambio usado:** **1 USD ≈ 19.1 MXN**, el mismo de los documentos 01 y 02 de esta investigación (FIX de Banxico del 20-nov-2026 ≈ 19.02; mercado del 21 y 22-nov-2026 ≈ 19.16). Fuentes: [dolar.info – FIX Banxico](https://dolar.info/mexico/hoy/precio/banxico-fix.php) y [Milenio, 21-nov-2026](https://www.milenio.com/negocios/precio-del-dolar-hoy-21-noviembre-2026-a-cuanto-cotiza-en-vivo-atm). Las conversiones están redondeadas.

---

## 0. Cómo leer este documento (nota de transparencia)

- **Leyenda de confianza:**
  - ✅ **Confirmado:** dato encontrado en una búsqueda de noviembre de 2026, con fuente enlazada.
  - ⚠️ **No confirmado en esta sesión:** sale de documentación que conozco de antes (hasta mediados de 2026) o de una sola fuente secundaria. **Hay que revisarlo en la página oficial antes de firmar o presupuestar.**
- **Limitaciones reales de esta investigación:**
  1. La herramienta para leer páginas completas no pudo entrar a `photonengine.com`, `doc.photonengine.com` ni `unity.com`: la red de esta sesión no los permite. Por eso los precios oficiales de Photon y Unity vienen de los **fragmentos que devolvió el buscador**, que en varios casos citan la página oficial, y de sitios secundarios.
  2. Hice unas 25 búsquedas y después **se agotó el presupuesto de búsquedas**, que comparten todos los agentes de esta tarea. **No pude verificar en 2026** Nakama/Heroic Cloud, Coherence, Normcore, PurrNet, Netick, FishNet, Mirror ni los permisos de red local de iOS y Android. Esas secciones salen de documentación técnica que conozco y van marcadas con ⚠️. La lista final de fuentes separa lo verificado de lo que hay que revisar.
- **No es asesoría legal ni contable.** Los precios cambian seguido. En 2026 cerraron o cambiaron de dueño varios servicios (Hathora, Multiplay de Unity), así que conviene revisar cada proveedor justo antes de elegir.

---

## Resumen ejecutivo (lo más importante en 12 puntos)

1. **Hay una sola forma de que "local" y "online" usen el mismo código: la arquitectura "un celular es el anfitrión (host) y los demás se conectan a él".** En local, los celulares se conectan directo por la IP del Wi-Fi. En online, el mismo tráfico pasa por un **relay** (un "cartero" en la nube). Más adelante, el anfitrión puede ser un **servidor dedicado** que corre exactamente el mismo código, sin gráficos. Esto lo permiten de forma nativa **Unity Netcode for GameObjects + Unity Transport**, **Mirror**, **FishNet**, **PurrNet** y **Netick**.
2. **Las soluciones "todo en la nube" (Photon Fusion, Photon Quantum, Photon PUN, Epic Online Services, Nakama, Normcore, Coherence) necesitan internet** para crear la partida. No sirven para el modo "local sin internet" tal como viene de fábrica (⚠️ en Fusion hay que probarlo, ver §3.2).
3. **Photon Quantum 3 es técnicamente lo mejor para un juego de peleas** (determinista con *rollback*, el estándar de los juegos de pelea), pero **no funciona sin internet** y obliga a programar con su propio paradigma. Elegirlo después significa **reescribir**. Si se va a elegir, tiene que ser al principio.
4. **En un juego de "un golpe = KO", lo que más importa es quién decide si el golpe pegó.** La recomendación es que lo decida **el anfitrión o servidor**, "rebobinando" las posiciones al momento en que el atacante vio el golpe (*lag compensation*). Además, el diseño debe esconder el lag: el objeto viaja visible, el lanzamiento tiene una anticipación corta y el doble KO cuenta como empate.
5. **Local sin internet es posible y barato, pero iOS pone trabas.** Desde iOS 14 la app debe pedir permiso de **"Red local"** (`NSLocalNetworkUsageDescription`). Para buscar partidas con broadcast/multicast UDP hace falta un **permiso especial de Apple (entitlement `com.apple.developer.networking.multicast`) que se solicita por formulario**. La alternativa sin ese permiso es **Bonjour/mDNS**, que funciona entre iOS y Android. En Android hay que tomar un **`MulticastLock`**. ⚠️
6. **Muchos Wi-Fi reales bloquean que los celulares se vean entre sí** (redes de invitados, cafés, escuelas, plazas: "aislamiento de clientes"). Hay que incluir desde el día 1 un **plan B por hotspot del celular** y la **unión por código QR o IP**.
7. **Hathora cerró su hosting de juegos el 5 de mayo de 2026** (Fireworks AI la compró en marzo de 2026). **Unity dejó de operar Multiplay el 31 de marzo de 2026**; ahora lo opera Rocket Science Group. Relay, Lobby, Matchmaker y Netcode de Unity siguen. ✅
8. **En México ya hay región de nube cercana:** AWS **mx-central-1 (Querétaro)**, y desde **septiembre de 2026** Amazon GameLift Servers puede alojar ahí servidores de juego ✅. Photon **no tiene región en México**; las más cercanas son `us` (este de EE. UU.), `usw` (oeste de EE. UU.) y `sa` (São Paulo) ✅.
9. **Precios clave 2026:**
   - **Photon** Fusion/Quantum: gratis hasta 100 CCU en una app comercial; luego **US$125/mes por 500 CCU**, **US$250 por 1,000** y **US$500 por 2,000**. Arriba de eso, Premium Cloud a **US$0.50 por CCU** (mínimo US$1,000/mes). ✅
   - **Unity Relay:** **US$0.16 por CCU promedio adicional** y **US$0.09 por GiB** extra en América/Europa. ✅
   - **Edgegap** (servidores dedicados): **US$0.00115 por vCPU-minuto** + **US$0.10 por GB** de salida. ✅
   - **Epic Online Services:** P2P, relay y lobbies **gratis** (⚠️ hay que confirmar el soporte móvil actual).
10. **Proyección de costo mensual de red** (tráfico conservador; ver §6):

    | Opción | 1,000 jugadores diarios | 10,000 diarios | 100,000 diarios |
    |---|---|---|---|
    | Photon (Fusion/Quantum) | US$8–125 (MXN 150–2,400) | US$250–500 (MXN 4,800–9,550) | US$5,000–7,500 (MXN 95,500–143,000) |
    | Unity Relay (anfitrión en celular) | ~US$20 (MXN 400) | ~US$260–470 (MXN 5,000–9,000) | ~US$2,800–4,900 (MXN 53,500–93,600) |
    | Servidores dedicados (Edgegap) | ~US$75–120 (MXN 1,400–2,300) | ~US$750–1,200 (MXN 14,300–22,900) | ~US$7,500–12,000 (MXN 143,000–229,000) |
11. **Recomendación:** usar **Netcode for GameObjects (NGO) + Unity Transport** con anfitrión en un celular para **local** (sin internet) y para el **primer online** (relay de Unity o de Edgegap, con códigos de sala). Si después hace falta ranked y antitrampas, ese mismo código del anfitrión corre como **servidor dedicado headless** en Edgegap o en GameLift Querétaro. Antes de escribir mucho gameplay, hay que hacer una **prueba de sensación con lag simulado** (80–150 ms). Si se siente mal, ese es el único momento razonable para cambiarse a **Photon Quantum 3**.
12. **Regla de oro de programación desde el día 1:** separar la **simulación** (reglas, golpes, física simple, a ritmo fijo) de la **presentación** (gráficos, sonido, efectos) y de la **red** (una interfaz con dos implementaciones: "LAN" y "Online"). Los bots deben ser "jugadores sin red" que mandan las mismas entradas que un humano.

---

## 1. Conceptos en lenguaje simple

### 1.1 Mini glosario

| Término | Qué significa en cristiano | Por qué te importa |
|---|---|---|
| **DAU** | Jugadores distintos que abren el juego en un día | Base de las proyecciones de costo |
| **CCU** | Jugadores conectados **al mismo tiempo**. "CCU pico" es el máximo del mes | La mayoría de los servicios cobran por CCU, ya sea pico o promedio |
| **Ping / RTT** | Lo que tarda un mensaje en ir y volver (milisegundos) | En LAN: 1–10 ms. Online en México: 30–150 ms. Arriba de ~120 ms un juego de reflejos ya se siente raro |
| **Tick** | Cada "fotograma" de la simulación de red (p. ej. 30 o 60 por segundo) | Más ticks = más precisión, pero más tráfico y batería |
| **NAT** | El router de tu casa "esconde" tu celular detrás de una sola IP pública | Por eso dos celulares en casas distintas casi nunca se pueden conectar directo |
| **Relay** | Servidor en la nube que solo reenvía paquetes entre jugadores | Resuelve el problema del NAT. Agrega algo de lag y cobra por tráfico |
| **Host (anfitrión)** | Uno de los celulares hace de "servidor" y juega al mismo tiempo | Gratis en cómputo, pero si se sale, se acaba la partida |
| **Servidor dedicado** | Una computadora en un centro de datos corre la partida sin gráficos | Justo y difícil de hackear, pero cuesta dinero por minuto |
| **Autoridad** | Quién tiene la última palabra sobre lo que pasó (¿te pegó o no?) | Es LA decisión de diseño de red en un juego de un golpe |
| **Predicción** | Tu celular muestra tu movimiento **antes** de que el servidor lo confirme | Sin ella, todo se siente "chicloso" |
| **Lag compensation** | El servidor "rebobina el tiempo" para revisar el golpe como lo vio el atacante | Evita el "¡pero si le di!" |
| **Determinista + rollback** | Todos simulan exactamente lo mismo y solo se mandan los botones presionados; si llega tarde un botón, se "regresa" y se recalcula | Es lo que usan los juegos de pelea modernos (estilo GGPO) |

### 1.2 Las 5 arquitecturas posibles

| # | Arquitectura | Cómo funciona | Ventajas | Desventajas | Costo de nube | ¿Sirve local sin internet? |
|---|---|---|---|---|---|---|
| 1 | **Host P2P directo** (*listen server*) | Un celular es servidor + jugador; los demás se conectan a su IP | Gratis, latencia mínima en el mismo Wi-Fi | No cruza NAT por internet. El host tiene ventaja (ping 0) y puede hacer trampa. Si se va, se acaba la partida | **US$0** | ✅ **Sí, ideal** |
| 2 | **Host + relay** | Igual que la 1, pero los paquetes pasan por un relay en la nube | Funciona entre casas y redes celulares. **Es el mismo código que la 1** | +10–60 ms por el desvío. Se paga tráfico. Sigue la ventaja y la trampa del host | Bajo (pago por tráfico/CCU) | ✅ (la parte de host sí) |
| 3 | **Servidor dedicado** | Un servidor en la nube corre la partida y todos son clientes | Justo, antitrampas, nadie "tumba" la partida, permite ranked | Lo más caro: pagas cómputo por minuto + tráfico. Hay que operarlo (DevOps) | Medio-alto | ✅ si el código es "host = servidor + cliente" |
| 4 | **Autoridad compartida o distribuida** (Photon Fusion *Shared Mode*, Unity *Distributed Authority*) | Cada celular manda sobre sus propios objetos y la nube sincroniza | Sin servidor que pagar, fácil de programar | **Muy fácil hacer trampa**. Hay conflictos de "¿quién mató a quién primero?", malo para un juego de un golpe | Bajo-medio | ❌ (necesita la nube) |
| 5 | **Determinista con rollback** (Photon Quantum) | Solo viajan los botones; todos simulan igual con matemáticas de punto fijo; si llega tarde una entrada, se corrige | La mejor sensación en peleas, tráfico bajísimo, sin discusiones sobre el golpe | Otra forma de programar (ECS + punto fijo). Más caro de migrar. Con mucho ping los rivales "brincan" | Medio (pago por CCU) | ❌ en Quantum estándar |

### 1.3 Por qué esto es crítico en un juego de "un golpe = KO"

En un shooter con barra de vida, un error de 50 ms quita 10% de vida y nadie lo nota. **En nuestro juego, ese error decide si estás vivo o muerto**, así que la pregunta "¿quién decide el golpe?" importa muchísimo.

**Ejemplo con números.** Ana (ping 70 ms) lanza la gorra/disco hacia Beto, que en su pantalla está quieto. Durante esos 70 ms, Beto ya hizo dash en *su* celular.

| Quién decide | Qué pasa | Sensación |
|---|---|---|
| **El celular de Ana (autoridad del cliente)** | Ana ve que le pegó y Beto muere, aunque en su pantalla ya lo había esquivado | Beto: "¡yo ya me había quitado!" Además, Ana podría hacer trampa |
| **El servidor/host, sin compensación** | El servidor ve a Beto donde está *ahora*, ya movido, y dice "fallaste" | Ana: "¡le di clarito!" (frustrante para quien ataca) |
| **El servidor/host CON lag compensation** (recomendado) | El servidor guarda ~1 segundo de historial de posiciones y revisa el golpe "como lo vio Ana" (70 ms atrás) | Justo para quien ataca. Quien esquiva de vez en cuando muere "detrás del muro". Es el estándar de la industria ("favorecer al que dispara") |
| **Determinista con rollback** (Quantum) | Todos simulan las mismas entradas. Si llega tarde el dash de Beto, se recalcula y puede "revivir" o morir en la corrección | Lo más justo y consistente. Con mucho ping se ven correcciones bruscas |

**Trucos de diseño que esconden el lag** (lo que hacen los buenos juegos de este tipo):

1. **Objeto lento y visible.** Un objeto que vuela 0.3–0.8 s esconde 50–100 ms de lag de forma natural, porque nadie espera que pegue al instante.
2. **Anticipación corta al lanzar** (60–100 ms de animación de "cargar"). Ese tiempo sirve para que el mensaje llegue al servidor.
3. **El golpe cuerpo a cuerpo y el dash se predicen en tu propio celular.** El KO lo confirma el servidor, pero tu animación sale al instante.
4. **Doble KO = empate.** Si dos golpes se confirman en el mismo tick, mueren ambos o nadie. Así se evitan las discusiones.
5. **Hitbox un poquito más generosa para pegarle a otros y más pequeña para recibir.** Se siente justo con lag.
6. **Interpolación de 100 ms para los rivales.** Se ven suaves aunque lleguen paquetes desordenados.
7. **Tick de 30 Hz para la red y 60 fps para dibujar.** Es un buen equilibrio entre batería y datos en celulares de gama media.

---

## 2. Multijugador LOCAL (mismo Wi-Fi, idealmente sin internet)

### 2.1 Cómo funciona

1. Un celular toca "Crear sala". Se vuelve **host**: abre un puerto UDP (p. ej. 7777) y empieza a **anunciarse** en la red.
2. Los demás tocan "Buscar sala" y ven la lista: "Sala de Ana · 3/8 · Calle Grafiti".
3. Al tocar la sala se conectan **directo a la IP local** del host (p. ej. `192.168.1.34:7777`).
4. Si el anuncio no llega porque el router lo bloquea, hay un plan B: **escanear un QR** que muestra el host (contiene IP, puerto y versión del juego) o escribir un código corto.

### 2.2 Cuatro formas de descubrir partidas en la red local

| Método | Cómo funciona | iOS | Android | iOS ↔ Android | Comentario |
|---|---|---|---|---|---|
| **Broadcast UDP** (a `255.255.255.255` o `x.x.x.255`) | El host grita "¡aquí hay sala!" cada segundo a toda la red | ⚠️ **Requiere el entitlement de multicast** de Apple (formulario) | Requiere `MulticastLock` para recibir de forma confiable | Sí | Lo más simple. **Mirror** ya trae un componente `NetworkDiscovery` que hace esto ⚠️ |
| **Multicast UDP** (grupo `239.x.x.x`) | Igual, pero solo lo escuchan los suscritos al grupo | ⚠️ Mismo entitlement | `MulticastLock` | Sí | Algunos routers mesh o empresariales lo filtran |
| **Bonjour / mDNS (DNS-SD)** | Estándar abierto de "servicios en la red" (así se encuentran impresoras y Chromecast) | ✅ **No requiere el entitlement de multicast**, solo declarar el servicio en `NSBonjourServices` ⚠️ | API nativa `NsdManager` ⚠️ | **Sí** (es el mismo estándar) | **Es lo que Apple recomienda.** Requiere un pequeño plugin nativo (Swift/Kotlin) que la IA puede escribir |
| **QR / código / IP manual** | El host muestra un QR con su IP y puerto | Sí (solo el permiso de red local) | Sí | Sí | Plan B obligatorio. Funciona aunque el broadcast esté bloqueado, **pero no** si el router aísla clientes |

**Recomendación:** **Bonjour/mDNS como método principal + QR como respaldo.** Si Apple aprueba el entitlement de multicast, se puede agregar broadcast UDP como tercera vía.

### 2.3 iOS: permisos y trampas (⚠️ revisar con Apple antes de publicar)

| Elemento | Qué es | Qué poner |
|---|---|---|
| `NSLocalNetworkUsageDescription` (Info.plist) | Texto del aviso "¿Permitir que *[juego]* busque dispositivos en tu red local?" (obligatorio desde iOS 14) | "Para jugar con tus amigos conectados al mismo Wi-Fi." |
| `NSBonjourServices` (Info.plist) | Lista de los tipos de servicio Bonjour que usará la app | p. ej. `_tintafight._udp` (nombre ilustrativo) |
| **Entitlement `com.apple.developer.networking.multicast`** | Permiso especial para mandar o recibir broadcast/multicast "a mano". **Se solicita con un formulario a Apple** y Apple decide si lo da | Solo si usamos broadcast UDP o multicast. Con Bonjour no hace falta |
| Comportamiento del aviso | iOS lo muestra **la primera vez que la app intenta tocar la red local**. No hay API oficial para preguntar si el usuario aceptó. Si dice "No", la conexión LAN **falla en silencio** | Mostrar antes una pantalla propia que explique el permiso. Si no se encuentra a nadie, mostrar "Revisa Ajustes > Privacidad > Red local" |
| **IPv6** | Apple revisa las apps en redes **solo IPv6** (NAT64). Si el juego conecta por IPv4 "a mano" en modo online, puede fallar la revisión | En online, usar nombres de host o servicios (relay) que soporten IPv6. En LAN no afecta |
| Unity | Unity no agrega estas claves por sí solo | Agregarlas con un script de *post-build* (`PostProcessBuild`) que edite el Info.plist |

### 2.4 Android: permisos y trampas (⚠️ revisar con la documentación de Android)

| Elemento | Para qué |
|---|---|
| `INTERNET`, `ACCESS_NETWORK_STATE`, `ACCESS_WIFI_STATE` | Básicos para sockets y para saber la IP del Wi-Fi |
| `CHANGE_WIFI_MULTICAST_STATE` + **`WifiManager.MulticastLock`** | Muchos teléfonos **tiran los paquetes broadcast/multicast** para ahorrar batería. El lock se adquiere al buscar sala y se suelta al terminar. Desde Unity se llama con `AndroidJavaObject` |
| `NsdManager` (Network Service Discovery) | El "Bonjour de Android"; interopera con iOS |
| `NEARBY_WIFI_DEVICES` (Android 13+) | Solo si se usa Wi-Fi Direct, Wi-Fi Aware o *LocalOnlyHotspot*. No hace falta para LAN normal |
| `LocalOnlyHotspot` (Android 8+) | La app crea un hotspot **sin compartir internet** para que otros se conecten. Sirve para el "modo fiesta sin router" |

### 2.5 Problemas del mundo real y cómo resolverlos

| Problema | Dónde pasa | Solución |
|---|---|---|
| **Aislamiento de clientes (AP isolation)**: los celulares tienen internet pero no se ven entre sí | Wi-Fi de invitados, cafeterías, escuelas, plazas, hoteles, algunos módems de operador | Detectarlo ("no encontramos a nadie") y proponer **"Crear hotspot con tu celular"** |
| Router mesh que filtra multicast | Casas con mesh, oficinas | Bonjour suele pasar. Si no, QR con IP |
| Bandas 2.4 GHz y 5 GHz separadas con distinto nombre | Módems mal configurados | Pedir a todos que usen "la misma red" (mostrar el nombre de la red en la sala) |
| El usuario negó el permiso de red local en iOS | iPhone | Pantalla de ayuda con el atajo a Ajustes |
| Versiones distintas del juego | Siempre | Enviar `versionProtocolo` en el anuncio y ocultar salas incompatibles |
| El host bloquea el celular o contesta una llamada | Siempre | Pausa automática de 10 s y fin de partida con "host desconectado". Las partidas cortas (2–4 min) hacen que esto sea tolerable |

### 2.6 Alternativas sin router

| Opción | Plataformas | Jugadores | Latencia | Comentario |
|---|---|---|---|---|
| **Hotspot de un celular** | iOS (Compartir Internet) ↔ Android, en ambos sentidos | 5–10 según el teléfono | Muy baja (2–10 ms) | **Plan B principal.** En iPhone, "Compartir Internet" normalmente requiere plan de datos activo ⚠️. En Android, `LocalOnlyHotspot` no consume datos. La IP del host cambia (iPhone suele usar `172.20.10.1`) ⚠️, por eso conviene descubrir por Bonjour o QR |
| **Bluetooth clásico / BLE** | Difícil entre iOS y Android | 2–4 en la práctica | Alta e inestable (30–100+ ms) | **No recomendado** para un juego de reflejos |
| **Google Nearby Connections** | Android (y una versión para iOS, experimental) ⚠️ | Topología estrella con varios equipos | Variable (usa BT + Wi-Fi) | Bueno para "juntarse sin Wi-Fi" en Android, pero el soporte iOS ↔ Android no es confiable ⚠️ |
| **Apple MultipeerConnectivity** | **Solo iOS** | Hasta 8 por sesión ⚠️ | Baja | Sirve entre iPhones; **rompe el juego cruzado** |
| **Wi-Fi Aware** | Android 8+ con hardware compatible; iOS 26 agregó un framework de Wi-Fi Aware ⚠️ | Varios | Baja | Promete conexión directa iPhone ↔ Android sin router. **Muy nuevo**: evaluarlo en 2027, no depender de él |

### 2.7 Checklist del modo local

- [ ] Host en celular con Unity Transport (UDP) en un puerto fijo, con alternativa si está ocupado.
- [ ] Descubrimiento Bonjour/NSD (plugin nativo) + QR + IP manual.
- [ ] Permisos de iOS en Info.plist (script post-build) y pantalla previa que explique el permiso.
- [ ] `MulticastLock` en Android si se usa broadcast.
- [ ] Botón "Crear hotspot" con instrucciones por plataforma.
- [ ] Bots para llenar la sala (corren en el host).
- [ ] Probar en: módem Telmex/Izzi/Totalplay, Wi-Fi de un café, hotspot iPhone→Android y Android→iPhone, y 8 celulares de gama media.

---

## 3. Multijugador ONLINE: opciones y precios 2026

### 3.1 Tabla maestra comparativa

| Solución | Tipo | Arquitecturas | Plan gratis | Precio de pago (2026) | ¿LAN sin internet + online con el mismo código? | Ideal para | Confianza |
|---|---|---|---|---|---|---|---|
| **Photon Fusion 2** | Netcode + nube (relay incluido) | Shared, Host, Server dedicado | 20 CCU (desarrollo) y **100 CCU comerciales** (1 app) | US$95 pago único por 100 CCU (12 meses); **US$125/mes por 500 CCU**, **US$250 por 1,000**, **US$500 por 2,000**; Premium **US$0.50/CCU** (mín. US$1,000) | ⚠️ No confirmado (el flujo estándar usa Photon Cloud) | Online rápido con predicción y lag compensation ya hechos | ✅ precios / ⚠️ LAN |
| **Photon Quantum 3** | Motor determinista + nube | Rollback con predicción; el servidor solo ordena entradas | Igual que Fusion (20/100 CCU) | Igual que Fusion | ❌ (necesita Photon Cloud; solo trae modo local de un solo dispositivo) ⚠️ | **Juegos de pelea y brawlers competitivos** | ✅ precios / ⚠️ modos |
| **Photon PUN 2** | Netcode "legado" | Cliente maestro + relay | 20 CCU (el gratis de 100 es **solo** para Fusion/Quantum) | Mismos planes por CCU | ❌ | Proyectos viejos; **no empezar nada nuevo con PUN** | ✅/⚠️ |
| **Unity NGO + Unity Transport** | Netcode oficial de Unity (gratis, código abierto) | Host, Server, Distributed Authority | Gratis | Gratis (solo pagas los servicios de abajo) | ✅ **Sí**: conexión directa (LAN) ↔ Relay (online), solo cambia la configuración del transporte | Equipos que ya usan Unity y quieren lo oficial | ✅/⚠️ |
| **Unity Relay** | Relay | Host + relay | ~50 CCU promedio/mes (fuentes secundarias) + 3 GiB por CCU, máximo 150 GiB/mes | **US$0.16 por CCU promedio adicional**; **US$0.09/GiB** (América/Europa), **US$0.16/GiB** (Asia/Australia) | ✅ (parte online) | El online del modo host | ✅ (página oficial vía buscador) |
| **Unity Lobby / Matchmaker / Distributed Authority** | Salas, emparejamiento, autoridad distribuida | — | Lobby: ~10 GiB por región al mes (una sola fuente); DA: "capa gratis generosa" | Pago por uso; no confirmé tarifas exactas | — | Salas con código, búsqueda de partida | ⚠️ |
| **Unity Multiplay** | Hosting de servidores | Dedicado | — | **Unity dejó de operarlo el 31-mar-2026**; lo sigue **Rocket Science Group** | — | Ya no recomendado como primera opción | ✅ |
| **Mirror** | Netcode gratis (MIT), comunidad enorme | Host, Server | Gratis | Gratis | ✅ (KCP directo + `NetworkDiscovery` incluido; online con relay de Edgegap o EOS) ⚠️ | Lo más documentado; la IA lo conoce muy bien | ⚠️ |
| **FishNet** | Netcode gratis | Host, Server; **predicción integrada** | Gratis (algunas funciones avanzadas en "Pro") ⚠️ | Pro de pago ⚠️ | ✅ (Tugboat/LiteNetLib en LAN; transportes para Edgegap, EOS y Unity Transport) | Quien quiere predicción sin pagar por CCU | ⚠️ |
| **PurrNet** | Netcode gratis (nuevo, 2024–2025) | Host, Server, con predicción | Gratis | Gratis ⚠️ | ✅ ⚠️ | Alternativa moderna; comunidad más chica | ⚠️ |
| **Netick 2** | Netcode gratis, muy eficiente | Host, Server; predicción y lag compensation | Gratis ⚠️ | Gratis ⚠️ | ✅ ⚠️ | Acción rápida con poco ancho de banda | ⚠️ |
| **Edgegap** | Hosting de servidores dedicados "en el borde" + relays | Dedicado / relay | Prueba gratis (1.5 vCPU); relay gratis con **325 GB** | **US$0.00115 por vCPU-minuto** (solo mientras corre) + **US$0.10/GB** de salida; flota privada desde **US$280/mes** (16 vCPU, 6 TB incluidos, contrato de 12 meses); relay: **US$0.10/GB** extra y **US$0.14 por CCU pico** extra | ✅ (es solo hosting/relay; el código es el de tu netcode) | Servidores por partida que se apagan solos | ✅ (página del proveedor) |
| **Hathora** | Hosting | — | — | **Cerró el hosting de juegos el 5-may-2026** (Fireworks AI la compró el 4-mar-2026); recomienda migrar a GameFabric (Nitrado) | — | **No usar** | ✅ |
| **Amazon GameLift Servers** | Hosting de servidores (AWS) | Dedicado | El free tier de AWS cambió en 2025 (créditos) ⚠️ | c5.large (2 vCPU) **US$0.109/h** en Ohio; transferencia de salida aparte (~US$0.09/GB ⚠️). **Disponible en mx-central-1 (Querétaro) desde sep-2026** | ✅ (hosting) | Escala grande y región en México | ✅ |
| **PlayFab Multiplayer Servers** (Microsoft) | Hosting + backend | Dedicado | **750 horas-núcleo Dasv4 al mes** (East US / North Europe) + 10 GB de salida | Por consumo, según la hoja de precios (no la pude abrir) | ✅ (hosting) | Si se usa todo el ecosistema PlayFab | ✅ gratis / ⚠️ pago |
| **Rivet** | Antes orquestador de servidores de juego | — | — | En 2026 se presenta como orquestador de "workloads agénticos" (actors); tarifas por uso (~US$0.15/GB de salida) | — | **Ya no está enfocado en juegos**: no recomendado | ✅ (directorio) |
| **Epic Online Services (EOS)** | Backend + P2P con relay automático | P2P/host + relay | **Gratis** (títulos y cuentas ilimitados, sin regalías) | Gratis | ❌ offline (necesita internet); ✅ online | Online gratis a cualquier escala | ✅ gratis / ⚠️ móvil |
| **Steam (Datagram Relay)** | Relay de Valve | — | Gratis | — | No aplica a celulares | Solo si sacan versión de PC en Steam | ✅ |
| **Nakama (Heroic Labs)** | Servidor de juego + backend de código abierto | Partidas autoritativas (Go/TS/Lua) o reenviadas; matchmaker, cuentas, billetera, tablas | **Gratis si lo alojas tú** (VPS de ~US$10–40/mes para empezar) ⚠️ | Heroic Cloud (administrado): por cotización ⚠️ | ❌ en celulares (corre en un servidor) | Backend completo (XP, recompensas diarias, inventario de trajes) + matchmaking | ⚠️ |
| **Coherence** | Netcode + nube | Servidor de replicación; *client hosting* vía relays de EOS o Steam | Plan gratis de desarrollo ⚠️ | Por uso ⚠️ | ⚠️ (en LAN solo con su servidor de replicación local) | Mundos persistentes y mucha gente | ⚠️ |
| **Normcore** | Netcode + nube enfocado en VR social | Salas en la nube | Gratis limitado ⚠️ | Por horas de sala ⚠️ | ❌ | VR social (el equipo ya hace Quest 3); **no** para competitivo | ⚠️ |

### 3.2 Fichas rápidas

**Photon Fusion 2** ✅/⚠️
- **Qué es:** el netcode moderno de Photon. Tiene tres modos: *Shared* (cada quien manda sobre lo suyo), *Host* (un celular es servidor y el relay de Photon conecta a todos) y *Server* (dedicado).
- **Lo que trae hecho:** predicción, tick fijo, interpolación, **lag compensation con historial de hitboxes** y migración de host ⚠️. Es lo más "llave en mano" para el online.
- **Precio (página oficial, vía buscador):** gratis hasta 100 CCU para una app comercial. Luego US$125, 250 o 500 al mes por 500, 1,000 o 2,000 CCU; los planes altos permiten "burst" (pasarte del límite y pagar después). Premium Cloud cuesta US$0.50/CCU con mínimo de US$1,000/mes (2,000 CCU y 6 TB incluidos), hasta 50,000 CCU. Cada CCU pagado incluye ~3 GB de tráfico al mes. **Ojo:** Photon **suma los picos de cada región**, así que tener jugadores en `us` y en `sa` puede subirte de plan ([Photon Fusion Pricing](https://www.photonengine.com/fusion/pricing), [Supercraft, 2026](https://crux.supercraft.host/blog/photon-fusion-pricing-2026/)).
- **El plan gratis de 100 CCU:** no tiene burst (al llegar a 100, ya no deja entrar a más), incluye 0.3 TB de tráfico y es **solo para una app**. Si pagas un plan, el espacio gratis queda libre para otra app ([Photon Pricing docs](https://doc.photonengine.com/photon/current/pricing), [Blog Photon](https://blog.photonengine.com/new-free-100-ccu-for-photon-fusion-and-quantum-games/)).
- **LAN sin internet:** **no lo pude confirmar.** El flujo estándar crea las sesiones en Photon Cloud. Antes de comprometerse, hay que probar con el internet del módem desconectado.

**Photon Quantum 3** ✅/⚠️
- **Qué es:** motor determinista con *predict/rollback*. Solo viajan las entradas; el servidor de Photon ordena y reparte, y no corre lógica de juego. Trae física determinista en punto fijo y, según su documentación, ejemplos de tipo "arena brawler" ⚠️.
- **Por qué es atractivo:** el golpe siempre es consistente para todos, no hay "el host tiene ventaja" y los bots y las repeticiones salen casi gratis.
- **Por qué no es la primera opción aquí:** (1) **no sirve offline LAN**; (2) se programa distinto (ECS con estructuras de datos especiales, sin `Rigidbody` de Unity), lo que es más difícil para un equipo que programa con IA; (3) migrar después = reescribir la lógica.
- **Precio:** igual que Fusion ([Photon Quantum Pricing](https://www.photonengine.com/quantum/pricing)).

**Photon PUN 2** ⚠️: es la generación anterior. Photon la mantiene, pero recomienda Fusion o Quantum para proyectos nuevos, y el gratis de 100 CCU **no aplica** a PUN ([Photon Pricing docs](https://doc.photonengine.com/photon/current/pricing)). **No empezar con PUN.**

**Unity: NGO + Unity Transport + Multiplayer Services** ✅/⚠️
- **NGO (Netcode for GameObjects)** es gratis y oficial. Funciona con el modelo "host = servidor + cliente", así que **el mismo código sirve en LAN, en online con relay y en servidor dedicado**. NGO 2.x agregó **Distributed Authority**, que es más barata pero **no conviene** para un juego de un golpe por las trampas. NGO **no trae lag compensation hecha**: hay que programar un historial simple de posiciones, unas 100–200 líneas que la IA puede escribir. Para la predicción tiene "anticipación" básica ⚠️.
- **Unity Transport (UTP):** capa UDP. Con `SetConnectionData(ip, puerto)` conecta directo (LAN) y con `SetRelayServerData(...)` usa Relay. **Es el "interruptor" entre local y online.**
- **Relay (precio oficial, vía buscador):** 3 GiB por CCU gratis, máximo 150 GiB al mes; **US$0.09/GiB** extra en EE. UU./Europa y **US$0.16/GiB** en Asia/Australia; **US$0.16 por CCU promedio adicional** ([UGS Pricing](https://unity.com/products/gaming-services/pricing)). Fuentes secundarias dicen que los primeros **50 CCU promedio** son gratis ([tech-insider, 2026](https://tech-insider.org/unity-multiplay-relay-lobby-setup-2026/)). **Importante:** Unity cobra el **CCU promedio** del mes, no el pico. Según foros, toma "fotos" diarias y las promedia ([Unity Discussions](https://discussions.unity.com/t/how-can-i-correctly-calculate-the-excess-of-the-free-limit-when-using-the-relay-service/941519)). Para un juego de sesiones cortas, eso es mucho más barato que cobrar por pico.
- **Lobby/Matchmaker/DA:** con capa gratis y pago por uso. No pude confirmar las tarifas exactas; Unity tiene una calculadora: [UGS Pricing Estimator](https://unity-player-services-pricing-estimator.ds.unity3d.com/).
- **Multiplay:** Unity **dejó de darle soporte directo el 31-mar-2026**. El servicio lo opera ahora **Rocket Science Group** ("Multiplay by Rocket Science"). Relay, Lobby, Matchmaker, DA y NGO **no se ven afectados** ([AccelByte](https://accelbyte.io/blog/unity-multiplay-is-transitioning-practical-paths-forward-for-multiplayer-studios-using-or-evaluating-multiplay), [Gameye 2026](https://gameye.com/blog/unity-multiplay-shutdown-migration-options/), [Unity Discussions](https://discussions.unity.com/t/end-of-services/1698428)).
- **Netcode for Entities:** es la otra opción de Unity, con predicción y lag compensation integradas, pero requiere DOTS/ECS. **Demasiado complejo** para este equipo.

**Mirror** ⚠️: gratis (MIT), el netcode comunitario más antiguo de Unity. Hay muchísimos tutoriales, así que la IA lo conoce muy bien. Trae `NetworkDiscovery` (broadcast UDP) listo para LAN, transporte KCP y una alianza con Edgegap para relay y servidores. En los últimos años agregó componentes de predicción de física y de compensación de lag, pero no tan completos como los de Fusion.

**FishNet** ⚠️: gratis, con **predicción del lado del cliente integrada** y muchos transportes: Tugboat para LAN y [FishyEOS para Epic Online Services](https://fish-networking.gitbook.io/docs/manual/components/transports/fishyeos-epic-online-services), además de Unity Transport y Edgegap. Algunas funciones (como lag compensation) han sido de la versión "Pro" de pago ⚠️.

**PurrNet** ⚠️: gratis y moderno (2024–2025), con predicción, varios transportes y buena ergonomía. Comunidad más chica que la de Mirror o FishNet.

**Netick 2** ⚠️: gratis, muy eficiente en ancho de banda, con predicción y lag compensation integradas y modo host. Comunidad pequeña, así que la IA lo conoce menos.

**Edgegap** ✅: servidores dedicados que se encienden por partida en la ubicación más cercana a los jugadores y se apagan al terminar (cuestan US$0 cuando no corren). **US$0.00115 por vCPU-minuto** (subió desde US$0.001 en 2025) + **US$0.10/GB** de salida. Hay prueba gratis con 1.5 vCPU, y para jugadores estables hay flotas privadas a US$280/mes por host. También ofrece relays P2P con una prueba gratis de 325 GB y planes de 1,625, 3,250 y 6,500 GB; el excedente cuesta US$0.10/GB y US$0.14 por CCU pico ([Edgegap Pricing](https://edgegap.com/resources/pricing), [Anuncio 2025](https://edgegap.com/blog/announcement-offering-changes-for-2025)). Integra con Mirror, FishNet, NGO y Photon Fusion (Server mode).

**Hathora** ✅: **ya no existe como hosting de juegos.** Fireworks AI (inteligencia artificial) la compró el 4-mar-2026 y apagó el servicio el 5-may-2026. Stormgate perdió su online por eso. Es la lección de 2026: **no casarse con un hosting pequeño sin plan B** ([Supercraft](https://crux.supercraft.host/blog/hathora-shut-down-where-to-go-after-may-2026/), [PCWorld](https://www.pcworld.com/article/3105695/ai-is-coming-for-your-online-gaming-servers-next.html), [Gameye](https://gameye.com/blog/game-server-shake-up-2026/)).

**Amazon GameLift Servers** ✅: es el más sólido a largo plazo. **Desde septiembre de 2026 está disponible en mx-central-1 (Querétaro)** ([AWS What's New, 2026](https://aws.amazon.com/about-aws/whats-new/2026/09/amazon-gamelift-servers-region-expansion-2026/), [Regiones GameLift](https://docs.aws.amazon.com/gameliftservers/latest/developerguide/gamelift-regions.html)). Una instancia c5.large (2 vCPU, 4 GiB) cuesta US$0.109/h en EE. UU. Este (Ohio); el precio en Querétaro puede ser distinto ([GameLift pricing](https://aws.amazon.com/gamelift/servers/pricing/)). Es más trabajo de DevOps que Edgegap.

**PlayFab Multiplayer Servers** ✅/⚠️: Microsoft da **750 horas-núcleo Dasv4 al mes + 10 GB de salida gratis** para evaluar (East US / North Europe, máximo 24 núcleos simultáneos). El precio de pago está en una hoja aparte que no pude abrir ([Microsoft Learn](https://learn.microsoft.com/en-ca/xbox/playfab/multiplayer/servers/billing-for-thunderhead), [PlayFab pricing](https://developer.microsoft.com/en-us/games/products/playfab/pricing/)).

**Rivet** ✅: hoy se describe como orquestador de "workloads agénticos" (actors), con tarifas como ~US$0.15/GB de salida ([rivet.dev/cloud](https://www.rivet.dev/cloud/), [directorio, sep-2026](https://www.positioniseverything.net/tools/rivet/)). Ya no es una opción natural para servidores de juego.

**Epic Online Services (EOS)** ✅/⚠️: **gratis**: lobbies, matchmaking, P2P con NAT traversal y **relay automático cuando el NAT no deja pasar**, cuentas, estadísticas y tablas. Sin regalías ni costo de hosting ([EOS servicios](https://onlineservices.epicgames.com/en-US/services-games), [PC Gamer](https://www.pcgamer.com/epics-multiplayer-systems-are-now-available-for-other-devs-to-use-for-free), [foro Unreal citando la doc de P2P](https://forums.unrealengine.com/t/question-about-networking-ports-and-eos/598259)). El relay de EOS **no es un servidor autoritativo** ([Edgegap](https://edgegap.com/blog/can-epic-online-services-eos-relays-allow-for-dedicated-server-or-authoritative-server)). Soporte móvil: wrappers como FishyEOS listan iOS y Android, pero **no confirmé en el portal de Epic** el estado actual del SDK móvil ⚠️. Es la opción **"online gratis"** más fuerte si el presupuesto es cero.

**Steam** ✅: Steam Datagram Relay es gratis, pero **solo para juegos en Steam (PC)**. No aplica a celulares ([Steamworks](https://partner.steamgames.com/doc/features/multiplayer/steamdatagramrelay)).

**Nakama (Heroic Labs)** ⚠️: servidor de código abierto que hace dos cosas. (1) **Backend del juego**: cuentas, XP, recompensas diarias, inventario de trajes, monedas, tablas, amigos, chat y validación de compras. (2) **Partidas en tiempo real**, reenviadas o autoritativas, escritas en Go, TypeScript o Lua. Gratis si lo alojas tú; el plan administrado (Heroic Cloud) es por cotización. **No pude confirmar precios 2026.** Para este proyecto es más útil como **backend de progresión** que como netcode del combate.

**Coherence** ⚠️ y **Normcore** ⚠️: Coherence es muy bueno para mundos persistentes y permite *client hosting* con relays de EOS o Steam ([docs coherence](https://docs.coherence.io/hosting/client-hosting/implementing-client-hosting/epic-online-services-eos-relay)). Normcore está pensado para VR social y salas. Ninguno es la mejor opción para un brawler competitivo móvil que además necesita LAN offline.

### 3.3 ¿Cuáles sirven para "LAN sin internet" Y "online" con el mismo código?

| Netcode | LAN offline | Online por relay | Servidor dedicado después | Predicción | Lag compensation | ¿Qué tan bien lo conoce la IA? | Veredicto |
|---|---|---|---|---|---|---|---|
| **NGO + UTP** | ✅ | ✅ Unity Relay (o Edgegap/EOS con transporte) | ✅ (build headless) | Básica ⚠️ | Hacerla nosotros | Alta (oficial, mucha doc) | **Recomendado** |
| **Mirror** | ✅ (discovery incluido) | ✅ Edgegap Relay / EOS | ✅ | Parcial ⚠️ | Básica ⚠️ | **Muy alta** | Alternativa fuerte |
| **FishNet** | ✅ | ✅ | ✅ | ✅ | Pro ⚠️ | Media-alta | Alternativa si la predicción es crítica |
| **PurrNet** | ✅ | ✅ | ✅ | ✅ ⚠️ | ⚠️ | Media | Vigilar |
| **Netick 2** | ✅ | ✅ (relay externo) | ✅ | ✅ | ✅ | Media-baja | Técnicamente muy bueno |
| **Photon Fusion 2** | ⚠️ por probar | ✅ (incluido) | ✅ | ✅ | ✅ | Alta | Si se acepta "local = online con código de sala" |
| **Photon Quantum 3** | ❌ | ✅ | No aplica | ✅ rollback | No aplica (determinista) | Media | Solo si se decide al inicio |
| **EOS / Nakama / Normcore / Coherence** | ❌ | ✅ | Varía | — | — | — | Complementos, no la base del modo local |

---

## 4. Latencia en México y LATAM

### 4.1 ¿Dónde están los servidores más cercanos?

| Proveedor | Región más cercana a CDMX | ¿Hay región en México? | Fuente |
|---|---|---|---|
| **AWS (EC2 / GameLift)** | **mx-central-1, Querétaro** (3 zonas de disponibilidad) | ✅ **Sí.** GameLift Servers disponible ahí desde sep-2026 | [AWS blog – región México](https://aws.amazon.com/blogs/aws/now-open-aws-mexico-central-region/), [AWS What's New 2026](https://aws.amazon.com/about-aws/whats-new/2026/09/amazon-gamelift-servers-region-expansion-2026/) |
| **Photon Cloud** | `us` (este de EE. UU.), `usw` (San José, CA), `sa` (São Paulo) | ❌ No hay código `mx` | [Fusion 2 – Regions](https://doc.photonengine.com/fusion/current/manual/connection-and-matchmaking/regions) |
| **Edgegap** | Red de cientos de ubicaciones; dice desplegar "lo más cerca de los jugadores" | ⚠️ Probablemente sí (no confirmado) | [Edgegap](https://edgegap.com/gaming/indies) |
| **Unity Relay** | Regiones de EE. UU. y Sudamérica ⚠️ | ⚠️ No confirmado | [UGS Pricing](https://unity.com/products/gaming-services/pricing) |
| **PlayFab** | Regiones de Azure (Azure tiene región en México Central ⚠️); el gratis aplica solo en East US y North Europe | ⚠️ | [Microsoft Learn](https://learn.microsoft.com/en-ca/xbox/playfab/multiplayer/servers/billing-for-thunderhead) |

### 4.2 Ping típico desde CDMX (estimaciones para planear; ⚠️ hay que medirlo)

| Destino | Ping aproximado por fibra o buen Wi-Fi | Con 4G/5G (agrega jitter) | Sensación en nuestro juego |
|---|---|---|---|
| Mismo Wi-Fi (LAN) | 1–10 ms | — | Perfecta |
| Querétaro (AWS mx-central-1) | 8–20 ms | 30–60 ms | Excelente |
| Texas (Dallas/Houston) | 30–50 ms | 50–90 ms | Muy buena |
| Este de EE. UU. (Photon `us`) | 50–75 ms | 70–120 ms | Buena con predicción |
| Oeste de EE. UU. (Photon `usw`) | 55–85 ms | 75–130 ms | Buena con predicción |
| São Paulo (Photon `sa`) | 120–170 ms | 150–220 ms | Mala para mexicanos; buena para Brasil |

> La herramienta [awsspeedtest.com – mx-central-1](https://awsspeedtest.com/regions/mx-central-1) sirve para medir desde el celular. Recomiendo una semana de pruebas reales con 10 personas en CDMX, Monterrey, Guadalajara y Mérida.

**Conclusión de latencia:** para jugadores mexicanos, un relay o servidor en **Querétaro o Texas** da 2 a 4 veces menos ping que Photon (Virginia/California). Si después hay jugadores en Brasil y Argentina, conviene un servidor o relay en São Paulo y emparejar por región para no mezclar mexicanos con brasileños.

---

## 5. ¿Cuánto tráfico gasta una partida? (estimación de ingeniería)

**Supuestos:** 8 jugadores, red a 30 ticks/s, posiciones comprimidas, ~6 objetos en vuelo y estados de animación.

| Concepto | Cálculo | Resultado |
|---|---|---|
| Snapshot de estado por tick (comprimido, con deltas) | ~130 bytes | — |
| Encabezados UDP/IP | 28 bytes × 30 paquetes/s | ~0.8 KB/s |
| **Bajada por jugador** (servidor/relay → celular) | 130 B × 30 + encabezados | **≈ 5 KB/s** |
| **Subida por jugador** (entradas) | ~20–30 B × 30 + encabezados | **≈ 1.5 KB/s** |
| Por minuto, por jugador (lo que cobra la nube como "salida") | ≈ 6.7 KB/s × 60 | **≈ 0.4 MB/min** |
| Partida de 3 min, por jugador | 0.4 × 3 | **≈ 1.2 MB** |
| **Subida del celular anfitrión** (8 jugadores) | 7 clientes × 5 KB/s | ≈ 35 KB/s ≈ 280 kbps → **≈ 2 MB/min** |
| Quantum (determinista, solo entradas) | Entradas de 8 jugadores a 60 Hz | Del mismo orden: 2–5 KB/s por jugador ⚠️ |

**Implicaciones:**
- En México mucha gente juega con **datos prepagados**. 10 partidas ≈ 12 MB como cliente (aceptable). El **anfitrión** gasta ~6 MB por partida de 3 min; vale la pena avisarlo o elegir como anfitrión a quien esté en Wi-Fi.
- Para costos usamos **0.4 MB de salida por jugador-minuto**, que es conservador.

---

## 6. Proyecciones de costo mensual (3 opciones recomendadas)

### 6.1 Supuestos comunes

| Supuesto | Valor | Comentario |
|---|---|---|
| CCU pico | **10–15% de DAU** (según el encargo) | Es pesimista para sesiones cortas; con 25 min/día, el pico real suele ser menor. Sirve como "techo" |
| Tiempo jugando online por DAU | **25 min/día** (≈ 7–8 partidas + menús) | — |
| CCU **promedio** | DAU × 25 / 1,440 = **1.74% de DAU** | Es lo que cobra Unity Relay |
| Salida de datos | 0.4 MB por jugador-minuto → **0.3 GB por DAU al mes** | 1k DAU = 300 GB; 10k = 3 TB; 100k = 30 TB |
| Jugadores por partida (servidor dedicado) | 6 en promedio | Salas de 2–8 |
| CPU por partida dedicada | 0.5 vCPU (conservador; optimizado puede ser 0.25) | Unity headless a 30 Hz |
| Tipo de cambio | 19.1 MXN/USD | — |

| DAU | CCU pico (10–15%) | CCU promedio | Salida mensual |
|---|---|---|---|
| 1,000 | 100–150 | ~17 | ~300 GB |
| 10,000 | 1,000–1,500 | ~174 | ~3 TB |
| 100,000 | 10,000–15,000 | ~1,736 | ~30 TB |

### 6.2 Opción A: Photon (Fusion 2 en modo Host/Shared, o Quantum 3). Sin servidores propios.

| DAU | Plan | USD/mes | MXN/mes |
|---|---|---|---|
| 1,000 | Gratis 100 CCU + paquete de 100 CCU por US$95/año (= 200 CCU, ~US$8/mes). Opción segura: 500 CCU | **US$8–125** | **~$150–2,390** |
| 10,000 | 1,000 CCU (US$250, 3 TB justos) o 2,000 CCU (US$500, 6 TB) | **US$250–500** | **~$4,780–9,550** |
| 100,000 | Premium Cloud: 10,000–15,000 CCU × US$0.50 (incluye ~3 GB/CCU, suficiente para ~30 TB) | **US$5,000–7,500** | **~$95,500–143,250** |

*Nota:* si se usara Fusion en *Server mode*, hay que sumar el hosting (Opción C). Photon suma los picos por región.

### 6.3 Opción B: NGO + Unity Relay + Lobby (anfitrión en un celular)

| DAU | CCU promedio cobrable | Excedente de ancho de banda (US$0.09/GiB) | Lobby y otros ⚠️ | **Total USD/mes** | **MXN/mes** |
|---|---|---|---|---|---|
| 1,000 | 17 → dentro de los 50 gratis: US$0 | 279 GiB − ~51 GiB gratis ≈ 228 GiB → US$21 | ~US$0 | **~US$20** | **~$400** |
| 10,000 | (174 − 50) × 0.16 = US$20 | 2,794 − 150 ≈ 2,644 GiB → US$238 | US$0–20 | **~US$260** (US$470 si cobraran por pico) | **~$5,000–9,000** |
| 100,000 | (1,736 − 50) × 0.16 = US$270 | 27,940 − 150 ≈ 27,790 GiB → US$2,501 | US$20–50 | **~US$2,800** (US$4,900 si cobraran por pico) | **~$53,500–93,600** |

*Notas:* (1) GB a GiB: 1 GB = 0.931 GiB. (2) El tráfico del jugador anfitrión no pasa por el relay, lo que ahorra ~1/8; no lo descontamos. (3) No confirmé si Sudamérica se cobra a US$0.09 o a otra tarifa. (4) La Opción B **no incluye servidor**: el anfitrión puede hacer trampa y, si se va, termina la partida.

### 6.4 Opción C: servidores dedicados en Edgegap (con NGO, FishNet, Mirror o Netick en modo servidor)

Cálculo: vCPU-minutos/mes = DAU × 25 min × 30 días ÷ 6 jugadores × 1.25 (tiempo de espera y arranque) × 0.5 vCPU ≈ **DAU × 78 vCPU-min**.

| DAU | Cómputo (US$0.00115/vCPU-min) | Salida (US$0.10/GB) | **Total USD/mes** (0.25–0.5 vCPU por partida) | **MXN/mes** |
|---|---|---|---|---|
| 1,000 | 78,000 vCPU-min → US$45–90 | 300 GB → US$30 | **~US$75–120** | **~$1,430–2,290** |
| 10,000 | 781,000 → US$450–900 | 3 TB → US$300 | **~US$750–1,200** | **~$14,300–22,900** |
| 100,000 | 7.8 M → US$4,500–9,000 | 30 TB → US$3,000 | **~US$7,500–12,000** | **~$143,000–229,000** |

*Notas:* hay que sumar el backend de matchmaking (Nakama en un VPS de US$20–200, Unity Lobby/Matchmaker o el matchmaker de Edgegap). Con jugadores estables, una **flota privada** (US$280/mes por host con 6 TB incluidos) o **GameLift en Querétaro** pueden bajar el costo por minuto, a cambio de pagar capacidad aunque esté ociosa.

### 6.5 Comparativa final y una opción "D" de costo cero

| | 1k DAU | 10k DAU | 100k DAU | Trampas | ¿Partida se cae si se va alguien? | LAN offline con mismo código |
|---|---|---|---|---|---|---|
| **A. Photon** | US$8–125 | US$250–500 | US$5,000–7,500 | Media (Host/Shared) / Baja (Quantum) | Fusion tiene migración de host ⚠️; Quantum no depende de un host | ❌/⚠️ |
| **B. Unity Relay (host)** | ~US$20 | ~US$260–470 | ~US$2,800–4,900 | Alta (el host manda) | Sí, si se va el host | ✅ |
| **C. Edgegap dedicado** | ~US$75–120 | ~US$750–1,200 | ~US$7,500–12,000 | **Baja** | No | ✅ |
| **D. EOS P2P + relay** | US$0 | US$0 | US$0 | Alta (el host manda) | Sí, si se va el host | ✅ (la parte de host) ⚠️ |

**Contexto de negocio ⚠️:** a 100k DAU, aun la opción más cara (~US$12,000/mes) son unos **US$0.004 por jugador al día**. En juegos casuales móviles, el ingreso diario por jugador suele estar varias veces arriba de eso, así que **la red no es lo que quiebra el proyecto; lo que lo quiebra es la retención** (ver documento 02).

**Costos fijos que siempre aplican ⚠️:** Apple Developer Program, US$99/año (~MXN 1,890); Google Play, US$25 en un solo pago (~MXN 480); Unity Personal, gratis mientras los ingresos y fondos de los últimos 12 meses sean menores a US$200,000 ([A Plus Games Studio, 2026](https://aplusgamesstudio.com/blog/unity-pricing-2026)).

---

## 7. Qué han usado otros (grandes y pequeños)

| Juego | Equipo | Red | Lección para nosotros | Confianza |
|---|---|---|---|---|
| **Among Us** | 3 personas (Innersloth) | Empezó **solo local**; el online llegó después con servidores propios y la librería UDP "Hazel" ⚠️ | Local primero sí funciona, y se puede crecer después | ✅ (inicio local, doc. 02) / ⚠️ (Hazel) |
| **Duck Game** | 1 persona | **P2P**, porque el servidor dedicado era muy difícil; el publisher exigió online | El online puede ser requisito comercial; P2P es barato | ✅ ([Wikipedia](https://en.wikipedia.org/wiki/Duck_Game)) |
| **Boomerang Fu** | Estudio pequeño | **Sin online**; la crítica más repetida es la falta de online | Nuestro hueco: móvil + online | ✅ (doc. 01) |
| **Stick Fight** | Landfall (pequeño) | Online por Steam + local; versión móvil | Online + física graciosa = viral | ✅ (doc. 01) |
| **Brawl Stars** | Supercell | Servidores propios autoritativos | El estándar de calidad móvil; con un presupuesto que no tenemos | ⚠️ |
| **Juegos de pelea modernos** | Varios | **Rollback** (estilo GGPO) | Si el objetivo es competitivo serio, rollback es el estándar | ⚠️ ([ggpo.net](https://www.ggpo.net)) |
| **Stormgate** | Frost Giant | Hosting en Hathora → **perdió el online** cuando Hathora cerró | Depender de un solo proveedor pequeño es un riesgo real | ✅ ([finalboss.io](https://finalboss.io/stormgate-is-losing-its-multiplayer-to-an-ai)) |
| **Knockout City** | Velan/EA | Solo online, servidores dedicados; cerró en 2023 y liberó servidores privados | Diseñar para que el juego **pueda vivir sin nuestros servidores** (local/host) es un seguro de vida | ✅ (doc. 01) |

---

## 8. Recomendación para nuestro juego

### 8.1 Decisión principal

> **Netcode for GameObjects (NGO) + Unity Transport, con arquitectura "host = servidor + jugador", autoridad del host o servidor y lag compensation hecha por nosotros.**
> - **Etapa 1 (local):** conexión directa por IP en el mismo Wi-Fi o hotspot, descubrimiento por Bonjour/NSD + QR. **US$0 en nube.**
> - **Etapa 2 (online casual: amigos, salas con código):** el **mismo código**, pero el transporte usa **Unity Relay** (o EOS si queremos costo cero, o Edgegap Relay). Salas y códigos con Unity Lobby o el servicio de "Sessions". **~US$20–260/mes** hasta 10k DAU.
> - **Etapa 3 (matchmaking público y ranked):** el mismo código del host compilado como **servidor dedicado headless** en **Edgegap** (pago por minuto, para empezar) o **Amazon GameLift en Querétaro** (para escalar con ping bajo en México). **~US$750–1,200/mes** a 10k DAU.

**Por qué NGO y no Photon de entrada:**
1. Es **el único camino oficial de Unity que cumple el requisito crítico** de funcionar sin internet en LAN y pasar a online sin reescribir.
2. Es gratis, el equipo ya domina Unity y **la IA tiene muchísima documentación y ejemplos** para generar código correcto.
3. Evita quedar atados a un solo proveedor (lección Hathora/Multiplay 2026): el relay se puede cambiar (Unity → Edgegap → EOS) y el hosting también (Edgegap ↔ GameLift ↔ Rocket Science).

**Plan B evaluado en el prototipo:** si después de la prueba de lag simulado (§8.3) el combate online se siente mal y no se arregla con los trucos de §1.3, **cambiar a Photon Quantum 3 antes del mes 3**. Eso implica aceptar que el "modo local" será "online con código de sala", porque Quantum no funciona sin internet. Después de ese momento, cambiar de paradigma ya sería una reescritura.

**Alternativa equivalente:** **Mirror** o **FishNet**, si el equipo encuentra más ejemplos o necesita la predicción integrada de FishNet. Cumplen el mismo patrón host/servidor y LAN/online.

### 8.2 Cómo diseñar el código desde el día 1 para no reescribir

**Regla 1: tres capas separadas.**

```
┌─────────────────────────────────────────────┐
│  PRESENTACIÓN (Unity: modelos, animación,   │  ← solo "dibuja" el estado; nunca decide golpes
│  VFX de tinta, sonido, UI, vibración)       │
├─────────────────────────────────────────────┤
│  SIMULACIÓN (C# puro, tick fijo 30/60 Hz):  │  ← reglas: mover, lanzar, regresar objeto,
│  GameSimulation.Tick(entradas[])            │    colisiones simples, KO, power-ups, rondas
├─────────────────────────────────────────────┤
│  RED (interfaces intercambiables)           │  ← LAN directo │ Relay │ Servidor dedicado
│  IMatchTransport · IMatchFinder             │    (y "Offline" para bots y tutorial)
└─────────────────────────────────────────────┘
```

**Regla 2: todo lo que hace un jugador es una "entrada" (input).** Mover (vector), lanzar (dirección + fuerza), golpe, dash. Los celulares **mandan entradas, no resultados**. El host o servidor aplica las entradas en la simulación y reparte el estado. Así un **bot** es simplemente "algo que genera entradas", y sirve igual offline, en LAN y online.

**Regla 3: el host siempre se comporta como servidor.** Nada de "if soyHost hago trampa diferente". El código del host debe poder correr **sin cámara ni gráficos** (modo headless) desde el primer prototipo. Esa es la llave del servidor dedicado en la Etapa 3.

**Regla 4: interfaces de red pequeñas y aburridas** (ejemplo ilustrativo):

```csharp
public interface IMatchTransport {        // LAN: UTP directo · Online: Relay · Dedicado: servidor
    void StartHost(MatchOptions options);
    void Join(JoinTicket ticket);         // IP:puerto (LAN/QR), código de Relay o dirección del servidor
    void Shutdown();
}

public interface IMatchFinder {           // LAN: Bonjour/NSD + QR · Online: Lobby/Matchmaker
    void BeginBrowse(Action<MatchInfo> onFound);
    Task<JoinTicket> CreateRoom(MatchOptions options);
}
```

**Regla 5: el KO lo decide la simulación del host o servidor, con historial.** Guardar las posiciones de los últimos ~1 s (30–60 ticks) de cada jugador. Cuando llega "golpe de Ana en el tick 1520", el host revisa contra las posiciones de ese tick, con un tope de ~150 ms de rebobinado para que nadie con ping altísimo abuse.

**Regla 6: determinismo "barato" aunque no usemos Quantum.** Números aleatorios con semilla compartida (power-ups, spawns), tick fijo y nada de `Time.deltaTime` en reglas. Si algún día se quiere rollback, la migración duele mucho menos.

**Regla 7: versión de protocolo y "feature flags".** Cada anuncio LAN y cada sala online lleva `protocolVersion`. Las funciones nuevas se encienden desde configuración remota (Unity Remote Config o Nakama) sin publicar otra versión.

**Regla 8: la progresión (XP, recompensas diarias, trajes INKCAP) va en un backend separado, nunca dentro del netcode.** En modo local sin internet, la XP se guarda en el celular y se sincroniza al volver la conexión, con validación en el servidor para evitar trampas en las compras. Opciones: Unity Cloud Save/Economy, PlayFab o Nakama (otro documento cubre el backend).

### 8.3 Pruebas que deben hacerse en el prototipo (meses 0–3)

| Prueba | Cómo | Criterio de éxito |
|---|---|---|
| **Sensación con lag** | Simulador de red de Unity Transport / Multiplayer Tools: 0, 50, 100 y 150 ms, con 1–3% de pérdida | Con 100 ms, 8 de cada 10 probadores no sienten "injusticia" en los KO |
| LAN real | 8 celulares (mitad iOS, mitad Android) en módems comunes de México | Encuentran la sala en menos de 5 s; 0 desconexiones en 20 partidas |
| Hotspot | iPhone → Android y Android → iPhone | Conexión en menos de 30 s con instrucciones en pantalla |
| Permisos iOS | Instalación limpia + negar el permiso | La app explica cómo arreglarlo |
| Relay | 4 personas en distintas ciudades por 4G | Ping medio menor a 100 ms con relay en EE. UU. o Querétaro |
| Headless | Correr el host como servidor sin gráficos en una PC | Una partida completa con bots |
| Batería y datos | 30 min de juego como host | Menos de 15% de batería; menos de 70 MB de datos |

### 8.4 Presupuesto de red sugerido por etapa

| Etapa | Tecnología | Costo mensual de red | MXN aprox. |
|---|---|---|---|
| 0–1: Prototipo + lanzamiento local | NGO + UTP directo + Bonjour/QR | **US$0** | $0 |
| 2: Online casual (hasta ~10k DAU) | + Unity Relay/Lobby (o EOS gratis) | **US$0–300** | $0–5,700 |
| 3: Online público/ranked | + Edgegap (luego GameLift mx-central-1) | **US$750–1,200** a 10k DAU; **US$7,500–12,000** a 100k DAU | $14,300–229,000 |
| Fijos anuales | Apple US$99 + Google US$25 (una vez) | — | ~$2,370 el primer año |

### 8.5 Riesgos y cómo mitigarlos

| Riesgo | Probabilidad | Mitigación |
|---|---|---|
| Apple no aprueba el entitlement de multicast | Media | Usar Bonjour (no lo necesita) + QR |
| Wi-Fi con aislamiento de clientes | Alta en lugares públicos | Hotspot + mensaje claro |
| Cierre o cambio de proveedor (como Hathora/Multiplay en 2026) | Media | Interfaces de red intercambiables; no usar APIs exclusivas de un proveedor dentro del gameplay |
| El host hace trampa en online público | Alta si hay ranked | Ranked solo en servidores dedicados (Etapa 3) |
| Lag en 4G hace "injustos" los KO | Media | Trucos de §1.3, emparejamiento por región, relay o servidor en Querétaro/Texas |
| El equipo (que programa con IA) mezcla lógica en la presentación | Alta | Revisiones con la regla de tres capas; pruebas automáticas de la simulación en C# puro |

---

## 9. Dudas abiertas (verificar antes de decidir)

1. Si **Photon Fusion 2** puede crear partidas por IP directa **sin internet**. Probarlo con el SDK y el módem sin internet.
2. **Tarifas exactas 2026** de Unity Lobby, Matchmaker y Distributed Authority. Si los **50 CCU gratis** de Relay siguen vigentes y si Sudamérica cuenta como "Americas". Usar el [estimador de UGS](https://unity-player-services-pricing-estimator.ds.unity3d.com/).
3. Excedente de tráfico de Photon por GB y reglas exactas de "burst" en los planes de 500–2,000 CCU.
4. **Estado actual del SDK de EOS para iOS/Android** y si el relay P2P tiene límites de uso justo.
5. Precio de GameLift en **mx-central-1** (puede ser distinto al de Ohio) y costo de transferencia de datos ahí.
6. Si **Edgegap** tiene ubicaciones en México (CDMX/Querétaro/Monterrey) y precios de sus planes de relay.
7. Precios 2026 de **Heroic Cloud (Nakama)**, **Coherence** y **Normcore**.
8. Requisitos vigentes de Apple para el **entitlement de multicast** y la redacción del aviso de red local.
9. Madurez de **Wi-Fi Aware** en iOS 26 ↔ Android para un futuro "modo sin router".
10. Qué tan bien funciona la **lag compensation casera** en NGO contra Fusion o Quantum: se resuelve con la prueba de §8.3.

---

## 10. Fuentes

### 10.1 Verificadas en búsquedas de noviembre de 2026

**Photon**
- Photon Fusion Pricing (oficial, vía buscador): https://www.photonengine.com/fusion/pricing
- Photon Quantum Pricing (oficial, vía buscador): https://www.photonengine.com/quantum/pricing
- Photon – Pricing & CCU Plans (documentación oficial): https://doc.photonengine.com/photon/current/pricing
- Blog Photon – Free 100 CCU para Fusion y Quantum: https://blog.photonengine.com/new-free-100-ccu-for-photon-fusion-and-quantum-games/
- Blog Photon – Paquetes Plus 200 CCU: https://blog.photonengine.com/new-200-ccu-plus-package-100-paid-100-free/
- Photon Fusion 2 – Regions: https://doc.photonengine.com/fusion/current/manual/connection-and-matchmaking/regions
- Supercraft – Photon Fusion Pricing 2026: https://crux.supercraft.host/blog/photon-fusion-pricing-2026/
- iTechGuides – Photon Fusion Review 2026: https://www.itechguides.com/products/photon-fusion/

**Unity**
- Unity – UGS Pricing (oficial, vía buscador): https://unity.com/products/gaming-services/pricing
- UGS Pricing Estimator: https://unity-player-services-pricing-estimator.ds.unity3d.com/
- Unity Discussions – cálculo del excedente de Relay: https://discussions.unity.com/t/how-can-i-correctly-calculate-the-excess-of-the-free-limit-when-using-the-relay-service/941519
- Unity Discussions – Distributed Authority GA: https://discussions.unity.com/t/unity-distributed-authority-is-now-generally-available/1643825
- Unity Discussions – End of services (Multiplay): https://discussions.unity.com/t/end-of-services/1698428
- AccelByte – Unity Multiplay is transitioning: https://accelbyte.io/blog/unity-multiplay-is-transitioning-practical-paths-forward-for-multiplayer-studios-using-or-evaluating-multiplay
- Gameye – Unity Multiplay Shutdown (2026): https://gameye.com/blog/unity-multiplay-shutdown-migration-options/
- Gameye – The Game Server Shake-up of 2026: https://gameye.com/blog/game-server-shake-up-2026/
- Tech Insider – Unity Relay y Lobby 2026: https://tech-insider.org/unity-multiplay-relay-lobby-setup-2026/
- A Plus Games Studio – Unity Pricing 2026: https://aplusgamesstudio.com/blog/unity-pricing-2026

**Hosting**
- Edgegap – Pricing: https://edgegap.com/resources/pricing
- Edgegap – Cambios de oferta 2025: https://edgegap.com/blog/announcement-offering-changes-for-2025
- Edgegap – Indies: https://edgegap.com/gaming/indies
- Edgegap – ¿Los relays de EOS sirven como servidor autoritativo?: https://edgegap.com/blog/can-epic-online-services-eos-relays-allow-for-dedicated-server-or-authoritative-server
- Supercraft – Hathora Shutdown 2026: https://crux.supercraft.host/blog/hathora-shut-down-where-to-go-after-may-2026/
- PCWorld – "AI is coming for your online gaming servers": https://www.pcworld.com/article/3105695/ai-is-coming-for-your-online-gaming-servers-next.html
- Final Boss – Stormgate pierde su multijugador: https://finalboss.io/stormgate-is-losing-its-multiplayer-to-an-ai
- AWS – GameLift Servers, 5 nuevas regiones (sep-2026): https://aws.amazon.com/about-aws/whats-new/2026/09/amazon-gamelift-servers-region-expansion-2026/
- AWS – GameLift service locations: https://docs.aws.amazon.com/gameliftservers/latest/developerguide/gamelift-regions.html
- AWS – GameLift Servers pricing: https://aws.amazon.com/gamelift/servers/pricing/
- AWS – Región México (Central): https://aws.amazon.com/blogs/aws/now-open-aws-mexico-central-region/
- AWS Speed Test – mx-central-1: https://awsspeedtest.com/regions/mx-central-1
- Microsoft Learn – PlayFab MPS billing: https://learn.microsoft.com/en-ca/xbox/playfab/multiplayer/servers/billing-for-thunderhead
- PlayFab pricing: https://developer.microsoft.com/en-us/games/products/playfab/pricing/
- Rivet Cloud: https://www.rivet.dev/cloud/
- Directorio Rivet (sep-2026): https://www.positioniseverything.net/tools/rivet/

**Epic / Steam / otros**
- EOS – Servicios de juego: https://onlineservices.epicgames.com/en-US/services-games
- PC Gamer – Sistemas multijugador de Epic gratis: https://www.pcgamer.com/epics-multiplayer-systems-are-now-available-for-other-devs-to-use-for-free
- Foro Unreal – puertos y relay de EOS: https://forums.unrealengine.com/t/question-about-networking-ports-and-eos/598259
- FishNet – FishyEOS: https://fish-networking.gitbook.io/docs/manual/components/transports/fishyeos-epic-online-services
- coherence – Client hosting con relay de EOS: https://docs.coherence.io/hosting/client-hosting/implementing-client-hosting/epic-online-services-eos-relay
- Steamworks – Steam Datagram Relay: https://partner.steamgames.com/doc/features/multiplayer/steamdatagramrelay
- Duck Game – Wikipedia: https://en.wikipedia.org/wiki/Duck_Game

**Tipo de cambio**
- dolar.info – FIX Banxico: https://dolar.info/mexico/hoy/precio/banxico-fix.php
- Milenio, 21-nov-2026: https://www.milenio.com/negocios/precio-del-dolar-hoy-21-noviembre-2026-a-cuanto-cotiza-en-vivo-atm

### 10.2 Documentación de referencia NO verificada en esta sesión (⚠️ revisar)

Se agotó el presupuesto de búsquedas y la red bloqueó la lectura directa de páginas.

- Apple – TN3179 "Understanding local network privacy": https://developer.apple.com/documentation/technotes/tn3179-understanding-local-network-privacy
- Apple – Solicitud del entitlement de multicast: https://developer.apple.com/contact/request/networking-multicast
- Apple – NSBonjourServices / NSLocalNetworkUsageDescription: https://developer.apple.com/documentation/bundleresources/information_property_list/nslocalnetworkusagedescription
- Apple – MultipeerConnectivity: https://developer.apple.com/documentation/multipeerconnectivity
- Apple – Soporte de redes IPv6 (App Review): https://developer.apple.com/support/ipv6/
- Android – WifiManager.MulticastLock: https://developer.android.com/reference/android/net/wifi/WifiManager.MulticastLock
- Android – Network Service Discovery (NSD): https://developer.android.com/develop/connectivity/wifi/use-nsd
- Android – Permisos Wi-Fi (NEARBY_WIFI_DEVICES): https://developer.android.com/develop/connectivity/wifi/wifi-permissions
- Android – LocalOnlyHotspot: https://developer.android.com/develop/connectivity/wifi/localonlyhotspot
- Google – Nearby Connections: https://developers.google.com/nearby/connections/overview
- Unity – Netcode for GameObjects: https://docs-multiplayer.unity3d.com/netcode/current/about/
- Unity – Unity Transport: https://docs.unity3d.com/Packages/com.unity.transport@latest
- Mirror – Network Discovery: https://mirror-networking.gitbook.io/docs/manual/components/network-discovery
- FishNet – Documentación: https://fish-networking.gitbook.io/docs
- PurrNet: https://purrnet.gitbook.io/docs
- Netick: https://netick.net
- Photon Quantum 3 – Documentación: https://doc.photonengine.com/quantum/current/
- Heroic Labs – Nakama: https://heroiclabs.com/docs/nakama/ · https://github.com/heroiclabs/nakama
- Normcore – Pricing: https://normcore.io/pricing
- GGPO (rollback): https://www.ggpo.net
