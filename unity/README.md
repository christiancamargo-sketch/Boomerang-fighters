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
3. Copia la carpeta `unity/Assets/BoomerangFighters` dentro de `Assets/` de tu proyecto.
4. Crea una escena vacía, agrega un GameObject vacío y ponle el componente **MatchRunner**.
5. Dale **Play**. Aparece un menú de prueba:
   - **Jugar vs bots:** WASD para moverte, `E` o clic para lanzar (mantén para cargar), `Q` o clic derecho para el golpe, `F` o `Espacio` para el dash. El mouse apunta.
   - **2 jugadores aquí + bots:** el segundo usa IJKL, `O` lanzar, `U` golpe y `H` dash, o un control.
   - **Crear sala / Buscar salas / Unirse por IP:** multijugador por Wi‑Fi entre dos computadoras o celulares en la misma red.

> Si Unity marca error en `Unity.InputSystem`, ve a *Window ▸ Package Manager* e instala **Input System**. Luego, en *Project Settings ▸ Player ▸ Active Input Handling*, elige **Input System Package (New)** o **Both**.

Todo se dibuja con primitivas grises (*greybox*) para que se juegue desde el día 1. Cuando lleguen los modelos de los muñequitos, se reemplaza `ToyView` por un prefab animado y la simulación queda igual.

## Qué hay en cada carpeta

| Archivo | Qué hace |
|---|---|
| `Core/MatchSimulation.cs` | La partida completa a 60 ticks/seg: movimiento, gorra que va y regresa en curva, rebote en paredes, golpe que desvía, ¡CLANK! entre gorras, power-ups, láser del museo, rondas y marcador. |
| `Core/BotBrain.cs` | Bots que "presionan botones" igual que un humano (apuntan con predicción, esquivan con dash, recogen latas). 3 dificultades. |
| `Core/Arena.cs` | Los 3 escenarios (Callejón Grafiti, Museo de Arte, Azotea CDMX) y sus colisiones. |
| `Core/Tuning.cs` | Todos los números del juego, ya balanceados en el prototipo web. Se ajustan desde un `TuningAsset` en el Inspector. |
| `Core/Entities.cs` | `InputCommand` (5 bytes por tick para la red), jugadores, gorras, eventos. |
| `Core/Snapshot.cs` | Foto del estado que el anfitrión manda a los clientes (≈0.6–1 KB, 30 por segundo). |
| `Net/Sessions.cs` | `HostSession` (anfitrión autoritativo) y `ClientSession`. Entradas con redundancia: un toque de "lanzar" no se pierde aunque se pierda el 30% de los paquetes. |
| `Net/Transport.cs` | `ITransport` + `UdpTransport` (LAN) + `LoopbackTransport` (para probar con lag simulado de 50–150 ms sin red). |
| `Net/LanDiscovery.cs` | Anuncio de salas por broadcast y unión por QR (`bf://IP:PUERTO/Sala`). |
| `Unity/MatchRunner.cs` | El único componente que va en la escena. Modos: vs bots, anfitrión LAN, cliente LAN. |
| `Unity/LocalInputReader.cs` | Teclado (2 jugadores), mouse, controles y táctil (joystick flotante, LANZAR con arrastre para apuntar, GOLPE, DASH). |
| `Unity/GreyboxViews.cs`, `FxDirector.cs`, `DebugHud.cs` | Vistas con primitivas, manchas de tinta en el piso, partículas, temblor de cámara, HUD y menú de prueba. |

## Pruebas sin abrir Unity

Necesitas el [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
cd unity/Tests~/SimTests && dotnet test            # 21 pruebas: mecánicas, determinismo, snapshots, red con lag/pérdida, UDP real, anuncio LAN
cd unity/Tests~/HeadlessHost && dotnet run -c Release -- balance 25 normal   # métricas de balance con miles de rondas de bots
cd unity/Tests~/HeadlessHost && dotnet run -- lan "Sala de prueba"           # abre una sala LAN sin gráficos (servidor)
cd unity/Tests~/CoreCompat && dotnet build         # confirma compatibilidad con el perfil de Unity (.NET Standard 2.1, C# 9)
cd unity/Tests~/UnityCompileCheck && dotnet build  # opcional: compila la capa de Unity contra referencias de UnityEngine
```

La carpeta termina en `~` para que Unity la ignore.

**Métricas del último balance** (25 partidas por arena, 4 bots "Normal", a 5 puntos):

| Arena | Rondas | Duración promedio | Mediana | p90 | KOs por láser |
|---|---|---|---|---|---|
| Callejón Grafiti | 331 | 13.8 s | 12.9 s | 20.6 s | — |
| Museo de Arte | 315 | 12.9 s | 11.8 s | 19.3 s | 131 (≈13%) |
| Azotea CDMX | 313 | 12.6 s | 11.4 s | 18.5 s | — |

Las victorias por posición de salida salieron parejas y las 6 latas se recogen con frecuencia similar. Pendientes de balance: los bots casi no usan el golpe cuerpo a cuerpo (0 desvíos), y entre bots el ¡CLANK! sale unas 3 veces por ronda, más de lo que se espera con humanos.

## Siguientes pasos técnicos (en orden)

1. **Prueba de lag** con `LoopbackTransport` a 0, 50, 100 y 150 ms. Si a 100 ms los KO se sienten injustos, decidir antes del mes 3 si cambiar a Photon Quantum (ver investigación 04, §8.1).
2. **iOS:** agregar `NSLocalNetworkUsageDescription` y `NSBonjourServices` al Info.plist, y pedir a Apple el entitlement de multicast. Mientras llega: unión por QR o IP.
3. **Android:** adquirir `WifiManager.MulticastLock` al buscar salas (plugin de 20 líneas en Java/Kotlin). Las apps que apunten a **Android 17 (API 37)** tienen la red local bloqueada por defecto y deben pedir el permiso de tiempo de ejecución `ACCESS_LOCAL_NETWORK` (ver la verificación en la investigación 04).
4. **Online (Etapa 2):** la investigación recomienda **Netcode for GameObjects + Unity Transport + Unity Relay**. Este núcleo ya sigue ese modelo (anfitrión autoritativo que manda snapshots y recibe entradas), así que hay dos caminos sin tocar `Core/`: (a) implementar `ITransport` sobre Unity Transport, que trae Relay, y conservar `Sessions.cs`; o (b) mover el envío de entradas y snapshots a mensajes de NGO. El camino (a) es más corto. El `UdpTransport` actual sirve para LAN desde hoy.
5. **Arte:** prefab del muñequito con playera (material con la textura del diseño), gorra intercambiable y animaciones (idle, correr, lanzar, golpe, dash, KO).
6. **Backend de progresión** (XP, premio diario, vestidor, códigos de playera): separado de la red, ver investigación 05.

## Reglas para el equipo (programar con IA sin romper la arquitectura)

- Nada de `UnityEngine` dentro de `Core/` ni `Net/`. El asmdef lo impide (`noEngineReferences: true`).
- Las reglas nunca usan `Time.deltaTime`, `UnityEngine.Random` ni `System.Random`: solo `MatchSimulation.Dt` y `Rng`.
- Un jugador (humano, bot o remoto) solo produce `InputCommand`. La simulación decide todo lo demás.
- Antes de subir cambios a la simulación, corre `dotnet test`. La prueba de determinismo detecta si algo rompe la sincronización de red.
