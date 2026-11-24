# Proyecto Unity · núcleo del juego (nombre de trabajo: Boomerang Fighters)

Este folder tiene **el código base del juego para Unity 6**, organizado como recomienda la investigación de multijugador ([docs/investigacion/04](../docs/investigacion/04-multijugador-local-y-online.md)):

```
┌──────────────────────────────────────────────┐
│ PRESENTACIÓN  Scripts/Unity   (MonoBehaviour) │  dibuja, suena, vibra. Nunca decide golpes.
├──────────────────────────────────────────────┤
│ SIMULACIÓN    Scripts/Core    (C# puro)       │  reglas, gorras, KOs, power-ups, rondas, bots
├──────────────────────────────────────────────┤
│ RED           Scripts/Net     (C# puro)       │  LAN por UDP hoy · Relay / servidor dedicado después
└──────────────────────────────────────────────┘
```

**Core** y **Net** no usan `UnityEngine`: se compilan y se prueban fuera de Unity (`Tests~`). Por eso la misma lógica corre en el celular, en el anfitrión de una sala LAN y, más adelante, en un servidor dedicado sin gráficos.

## Abrirlo en Unity (15 minutos)

1. Instala **Unity 6 LTS** (6000.x) desde Unity Hub con los módulos **Android Build Support** e **iOS Build Support**.
2. Crea un proyecto nuevo con la plantilla **Universal 3D (URP)**. Ya trae el paquete **Input System** activado.
3. Copia la carpeta `unity/Assets/BoomerangFighters` dentro de `Assets/` de tu proyecto (incluye `Scripts/` y `Resources/`).
4. Crea una escena vacía, agrega un GameObject vacío y ponle el componente **MatchRunner**. Él solo agrega `DebugHud` (HUD) y `GameMenu` (menús).
5. Dale **Play**. Sale la pantalla de carga **GORRAZO · by INKCAP** y luego el menú:
   - **Jugar aquí:** elige modo (¡Gorrazo!, Lata Caliente, La Reta), escenario, 1–2 jugadores en el mismo equipo, bots y dificultad, puntos (3/5/7) y los interruptores de reglas. WASD para moverte, `E` o clic para lanzar (mantén para cargar), `Q` o clic derecho para el golpe (sin gorra: empujón), `F` o `Espacio` para el dash. El mouse apunta. El segundo jugador usa IJKL, `O` lanzar, `U` golpe y `H` dash, o un control.
   - **Wi‑Fi local:** crear sala (muestra tu IP en grande y el texto `bf://IP:PUERTO/Sala`), buscar salas o unirse por IP. Entre computadoras o celulares en la misma red o en el hotspot de un celular.
   - **Reto del Día**, **Vestidor** y **Premio diario** (ver "Ruta local sin servidores").
   - En el celular el menú se desplaza arrastrando con el dedo.

> Si Unity marca error en `Unity.InputSystem`, ve a *Window ▸ Package Manager* e instala **Input System**. Luego, en *Project Settings ▸ Player ▸ Active Input Handling*, elige **Input System Package (New)** o **Both**.

Todo se dibuja con primitivas grises (*greybox*) para que se juegue desde el día 1. Cuando lleguen los modelos de los muñequitos, se reemplaza `ToyView` por un prefab animado y la simulación queda igual.

### Para compilar en el celular

- **Android** (*Player ▸ Other Settings*): *Internet Access* = **Require**. Los permisos de red local (`INTERNET`, `ACCESS_NETWORK_STATE`, `ACCESS_WIFI_STATE`, `CHANGE_WIFI_MULTICAST_STATE`) los agrega solo `Scripts/Editor/AndroidLanPermissions.cs` al manifiesto que genera Gradle, así que **no** hace falta un `Assets/Plugins/Android/AndroidManifest.xml` propio (choca con los de otros plugins al fusionar). Si prefieres agregarlos a mano, son esas cuatro líneas `<uses-permission android:name="..."/>`. El `WifiManager.MulticastLock` se toma al crear o buscar sala y se suelta al salir (`LanPlatform.cs`, vía `AndroidJavaObject`, sin plugin Java). **Android 17 (API 37):** las apps que apunten a esa API tienen la red local bloqueada por defecto y deben pedir el permiso de tiempo de ejecución `ACCESS_LOCAL_NETWORK`; mientras el *target* sea menor no hace falta.
- **iOS:** `Scripts/Editor/IosLocalNetworkPostBuild.cs` agrega al `Info.plist` `NSLocalNetworkUsageDescription` ("GORRAZO usa tu red Wi‑Fi para jugar con amigos cerca") y `NSBonjourServices` (`_gorrazo._udp`). El anuncio de salas por *broadcast* en iOS además necesita el entitlement `com.apple.developer.networking.multicast` que Apple da por solicitud; mientras llega, en iOS se une por IP o por el texto `bf://`.
- `Scripts/Editor` tiene su propio asmdef **solo de editor** (`includePlatforms: ["Editor"]`): no entra en el juego.

## Qué hay en cada carpeta

| Archivo | Qué hace |
|---|---|
| `Core/MatchSimulation.cs` | La partida completa a 60 ticks/seg: movimiento, gorra que va y regresa en curva, rebote en paredes, golpe que desvía, ¡CLANK! entre gorras, power-ups, láser del museo, rondas y marcador. |
| `Core/BotBrain.cs` | Bots que "presionan botones" igual que un humano (apuntan con predicción, esquivan con dash, recogen latas). 3 dificultades. Desde v0.3 rodean los vacíos, salen de la sombra de la caja DROP y, sin gorra, empujan al rival hacia un vacío. |
| `Core/Arena.cs` | Los 4 escenarios (Callejón Grafiti, Museo de Arte, Azotea CDMX y Pop-up Store INKCAP) con sus vacíos, la caja DROP y las colisiones. |
| `Core/Tuning.cs` | Todos los números del juego, ya balanceados en el prototipo web. Se ajustan desde un `TuningAsset` en el Inspector. |
| `Core/Entities.cs` | `InputCommand` (5 bytes por tick para la red), jugadores, gorras, eventos. |
| `Core/Snapshot.cs` | Foto del estado que el anfitrión manda a los clientes (≈0.6–1 KB, 30 por segundo). |
| `Net/Sessions.cs` | `HostSession` (anfitrión autoritativo) y `ClientSession`. Entradas con redundancia: un toque de "lanzar" no se pierde aunque se pierda el 30% de los paquetes. |
| `Net/Transport.cs` | `ITransport` + `UdpTransport` (LAN) + `LoopbackTransport` (para probar con lag simulado de 50–150 ms sin red). |
| `Net/LanDiscovery.cs` | Anuncio de salas por broadcast y unión por QR (`bf://IP:PUERTO/Sala`). |
| `Unity/MatchRunner.cs` | El único componente que va en la escena. Modos: vs bots, anfitrión LAN, cliente LAN. |
| `Unity/LocalInputReader.cs` | Teclado (2 jugadores), mouse, controles y táctil (joystick flotante, LANZAR con arrastre para apuntar, GOLPE, DASH). |
| `Unity/GreyboxViews.cs`, `FxDirector.cs` | Vistas con primitivas: muros de muerte súbita, charco que crece, muro de tag, lata caliente sobre la cabeza, marca de Tatuaje, rastro rayado de "te la regreso", grada de La Reta; manchas de tinta, partículas y temblor de cámara. |
| `Unity/DebugHud.cs` | HUD en juego: marcador, reloj y muerte súbita, lata caliente, fila de La Reta, letreros de KO (¡GORRAZO!, ¡DE REGRESO!, ¡DE BANDA!, ¡TE LA REGRESO!, ¡ZAZ!, ¡ZAP!, ¡SPLASH!, ¡TIRAJE!) y ¡EMPATE!, emote, botones táctiles. |
| `Unity/GameMenu.cs`, `UiSkin.cs` | Menús IMGUI legibles en celular: inicio, ajustes de partida, Wi‑Fi local, Reto del Día, vestidor, premio diario y resultados con XP. |
| `Unity/LocalProgress.cs`, `ProgressStore.cs` | Progreso local (XP, nivel, Gotas, premio diario, calendario, inventario, equipado, tabla del Reto) en JSON dentro de `Application.persistentDataPath`. |
| `Unity/DailyChallenge.cs` | Reto del Día: semilla `AAAAMMDD`, arena y regla del día (las mismas que la demo web). |
| `Unity/DesignLibrary.cs` | Diseños del vestidor: carga sola los PNG de `Resources/Disenos/*` y el logo de `Resources/Marca/logo_inkcap` si existe. |
| `Unity/LanPlatform.cs` | `MulticastLock` de Android para anunciar y buscar salas. |
| `Editor/*` | Post-build de iOS (Info.plist) y de Android (permisos en el manifiesto de Gradle). Solo editor. |
| `Resources/Disenos/Playeras`, `Resources/Disenos/Gorras`, `Resources/Marca` | Ranuras para los PNG de artistas y el logo oficial. Cada una trae un `README.txt` con el formato. |

## Lo nuevo en v0.2 (spec [`docs/diseno/spec-v0.2-local.md`](../docs/diseno/spec-v0.2-local.md))

- **Reglas (núcleo):** doble KO = empate, muerte súbita (muros que empujan a los 20 s, empate a los 45 s), empujón sin gorra, "te la regreso", Sticker más lento, ¡CLANK! al 85 %, causas de KO con letrero. Los interruptores `Rules` (muerte súbita, empujón, te la regreso, muro de tag, latas) se eligen en el menú y viajan en el `StartMatch` de la red.
- **Latas:** Charco que crece 0.4 s y luego noquea, Muro de Tag de 2.5 × 0.4 m que bloquea 3 s, marca de Tatuaje sobre la víctima. Las latas son negro tinta con banda del color de la técnica (nunca de color de jugador).
- **Modos:** ¡Gorrazo!, Lata Caliente (3–6), La Reta (2–8, fila en el HUD y grada detrás del muro) y Reto del Día (1 contra 3 bots difíciles).
- **Marca:** "GORRAZO · by INKCAP" y "Lanza tu gorra, mancha a todos" en la carga, el menú y los resultados. No se inventa logo: si existe `Resources/Marca/logo_inkcap.png` se dibuja; si no, va el texto.
- **Vestidor con diseños de artistas:** los PNG cuadrados (512–1024 px) de `Resources/Disenos/Playeras` y `Resources/Disenos/Gorras` aparecen solos; el nombre del archivo es el nombre del diseño. La playera es la textura del cuerpo; el diseño de gorra va en la copa (la silueta nunca cambia y el color del jugador queda en el aro, el rastro y el botón). Hay diseños de ejemplo hechos con código para que no salga vacío.

## Ruta local sin servidores

La v0.2 sale **solo con multijugador local por Wi‑Fi/hotspot**: sin backend, sin cuentas, sin costos de servidor. El online queda para después y la arquitectura de 3 capas no cambia.

- **Jugar juntos:** uno toca *Wi‑Fi local ▸ Crear sala* y su celular hace de anfitrión (corre la simulación). Los demás, en el mismo Wi‑Fi o en el hotspot del anfitrión, tocan la sala que aparece o escriben la IP que se ve en grande (o pegan el texto `bf://IP:PUERTO/Sala`). Hasta 6 jugadores (8 en La Reta), con bots para completar. El anfitrión elige modo, arena, puntos y reglas; los diseños de playera y gorra del anfitrión se ven en todos los celulares.
- **Progreso en el celular** (`gorrazo_progreso.json` en `Application.persistentDataPath`, escritura atómica):
  - XP por nivel 150 + 50 × (n − 1), niveles 1–50. Por partida: jugar 20, ronda ganada 10, KO 6, ganar 50, primera victoria del día +50.
  - **Gotas** (moneda blanda): 10–25 por partida; se gastan en el vestidor. Sin compras con dinero real.
  - **Premio diario de 7 días** (150 Gotas · sticker · 250 Gotas · 25 Gotas extra · emote · 100 Gotas · gorra de la semana INKCAP) con **racha suave**: si faltas, se pausa. **Calendario de 28 días** → Playera del mes.
  - Muñequitos por nivel: Lienzo, Osote, Michi gratis · Gotita nv 2 · Seri nv 4 · Zurci nv 6 · Pegote nv 10 · Axo nv 15.
  - Pantalla de resultados con la XP desglosada, Gotas, barra de nivel y desbloqueos.
  - El canje de códigos de playera es una **simulación del portal web** (código de demostración `TINTA-2026`).
- **Reto del Día:** semilla `AAAAMMDD` con la fecha del celular; la arena y la "regla del día" salen de esa semilla con el mismo `Rng` que la demo web, así que todos juegan lo mismo el mismo día. Puntaje = KOs × 100 + rondas ganadas × 300 − segundos. Se guarda el mejor del día y una tabla de los últimos 7 días, solo en el celular.
- **Qué falta para online (Etapa 2):** progreso en la nube y anti-trampa (hoy el JSON se puede editar), tabla mundial del Reto, códigos de playera de un solo uso validados en servidor, y Relay para jugar fuera de la misma red.

## Pruebas sin abrir Unity

Necesitas el [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
cd unity/Tests~/SimTests && dotnet test            # 112 pruebas: mecánicas, reglas v0.2 y v0.3 (vacíos, 4 arenas, caja DROP), modos, bots, determinismo, snapshots, red con lag/pérdida, UDP real, anuncio LAN, progreso local y Reto del Día
cd unity/Tests~/HeadlessHost && dotnet run -c Release -- balance 25 normal   # métricas de balance con miles de rondas de bots
cd unity/Tests~/HeadlessHost && dotnet run -- lan "Sala de prueba"           # abre una sala LAN sin gráficos (servidor)
cd unity/Tests~/CoreCompat && dotnet build         # confirma compatibilidad con el perfil de Unity (.NET Standard 2.1, C# 9)
cd unity/Tests~/UnityCompileCheck && dotnet build  # opcional: compila la capa de Unity contra referencias de UnityEngine (Scripts/Editor no entra: usa UnityEditor)
```

La carpeta termina en `~` para que Unity la ignore.

**Métricas del último balance v0.2** (`balance 15 normal`: 15 partidas por arena, 4 bots "Normal", modo ¡Gorrazo!, a 5 puntos, todas las reglas encendidas):

| Arena | Rondas | Promedio | Mediana | p90 | Llegan a muerte súbita | Empates (doble KO) | KOs por láser |
|---|---|---|---|---|---|---|---|
| Callejón Grafiti | 184 | 13.7 s | 12.6 s | 21.6 s | 7.6 % | 2 (1.1 %) | — |
| Museo de Arte | 188 | 12.9 s | 11.9 s | 19.1 s | 3.2 % | 1 (0.5 %) | 63 (≈11 %) |
| Azotea CDMX | 187 | 11.7 s | 11.0 s | 17.1 s | 1.1 % | 4 (2.1 %) | — |

KOs por causa: ¡GORRAZO! ≈ 50 %, ¡DE REGRESO! 27–37 %, ¡TE LA REGRESO! 10–11 %, ¡DE BANDA! 2 %. Ninguna ronda llegó al empate por tiempo (45 s). Los bots ya desvían (83–101 "te la regreso" por arena) pero casi no empujan (2–6 por arena); entre bots el ¡CLANK! sigue saliendo unas 3 veces por ronda. En Lata Caliente con 4 bots la ronda dura ≈ 38–49 s según la arena (3 explosiones de 10–20 s): si se quiere más corta, bajar `ModeTuning.HotCanMin/Max`.

**Métricas del balance v0.3** (mismo comando; escenarios de 26 × 16 con vacíos y la Pop-up Store de 24 × 15; spec `docs/diseno/spec-v0.3-escenarios.md`):

| Arena | Rondas | Promedio | Mediana | p90 | KOs por vacío (con crédito + sin crédito) | Caja DROP | Láser |
|---|---|---|---|---|---|---|---|
| Callejón Grafiti | 170 | 12.4 s | 11.4 s | 18.6 s | 7.8 % (7.8 + 0.0) | — | — |
| Museo de Arte | 190 | 12.1 s | 11.3 s | 16.8 s | 4.2 % (4.2 + 0.0) | — | 10.2 % |
| Azotea CDMX | 185 | 10.8 s | 10.1 s | 15.8 s | 10.1 % (9.7 + 0.4) | — | — |
| Pop-up Store INKCAP | 172 | 12.6 s | 11.5 s | 18.8 s | 2.3 % (2.3 + 0.0) | 0 KOs en 79 cajas | — |
| **Total** | 717 | 11.9 s | | | **6.1 %** (6.0 + 0.1) | 0.0 % | 2.7 % |

Metas de la spec: rondas de 10–25 s en cada arena, caídas al vacío entre 5 % y 25 % de los KOs y menos de 15 % sin crédito, estas dos sobre el total de las 4 arenas. Las tres se cumplen. Los bots casi nunca se caen solos; las caídas salen de su táctica de empujar al rival hacia un vacío cuando se quedan sin gorra. En la Pop-up Store el vacío pesa poco (las escaleras están en dos esquinas) y los bots siempre salen de la sombra de la caja DROP; con jugadores humanos habrá más caídas y más ¡DROP!. Con bots en Fácil el total de vacío baja a ≈ 2.4 % y en Difícil sube a ≈ 7.9 %.

## Siguientes pasos técnicos (en orden)

1. **Prueba de lag** con `LoopbackTransport` a 0, 50, 100 y 150 ms. Si a 100 ms los KO se sienten injustos, decidir antes del mes 3 si cambiar a Photon Quantum (ver investigación 04, §8.1).
2. **iOS:** el Info.plist ya se arma solo (`Editor/IosLocalNetworkPostBuild.cs`). Falta pedir a Apple el entitlement de multicast. Mientras llega: unión por QR o IP.
3. **Android:** el `MulticastLock` y los permisos ya están (`LanPlatform.cs`, `Editor/AndroidLanPermissions.cs`). Falta probarlo en 3–4 celulares reales y, al apuntar a **Android 17 (API 37)**, pedir el permiso de tiempo de ejecución `ACCESS_LOCAL_NETWORK` (ver la verificación en la investigación 04).
4. **Online (Etapa 2):** la investigación recomienda **Netcode for GameObjects + Unity Transport + Unity Relay**. Este núcleo ya sigue ese modelo (anfitrión autoritativo que manda snapshots y recibe entradas), así que hay dos caminos sin tocar `Core/`: (a) implementar `ITransport` sobre Unity Transport, que trae Relay, y conservar `Sessions.cs`; o (b) mover el envío de entradas y snapshots a mensajes de NGO. El camino (a) es más corto. El `UdpTransport` actual sirve para LAN desde hoy.
5. **Arte:** prefab del muñequito con playera (material con la textura del diseño), gorra intercambiable y animaciones (idle, correr, lanzar, golpe, dash, KO).
6. **Backend de progresión** (Etapa 2): hoy XP, premio diario, vestidor y Reto del Día viven en el celular (`LocalProgress`). Pasarlos a la nube y validar los códigos de playera en servidor queda para el online, ver investigación 05.

## Reglas para el equipo (programar con IA sin romper la arquitectura)

- Nada de `UnityEngine` dentro de `Core/` ni `Net/`. El asmdef lo impide (`noEngineReferences: true`).
- Las reglas nunca usan `Time.deltaTime`, `UnityEngine.Random` ni `System.Random`: solo `MatchSimulation.Dt` y `Rng`.
- Un jugador (humano, bot o remoto) solo produce `InputCommand`. La simulación decide todo lo demás.
- Antes de subir cambios a la simulación, corre `dotnet test`. La prueba de determinismo detecta si algo rompe la sincronización de red.
