# 03 · Motor de juego, herramientas y requisitos técnicos

> **Proyecto:** Boomerang Fighters (nombre de trabajo) · **Fecha de corte:** 23 de noviembre de 2026
> **Tipo de cambio usado:** 1 USD ≈ **19.10 MXN**, el mismo de los reportes 01 y 02 (FIX publicado en el DOF: 19.0270 al 20-nov-2026, según [dolardof.com](https://dolardof.com/)). Las conversiones a pesos son aproximadas y redondeadas.

**Nota de método (léela antes de usar este reporte):** Hice unas 45 búsquedas web antes de agotar el límite de búsquedas de esta sesión, que comparten todos los agentes del flujo. La lectura directa de páginas estuvo bloqueada para casi todos los dominios (unity.com, photonengine.com, support.google.com, dev.to, gamedev.net, ziva.sh). Solo pude abrir **developer.apple.com**. Por eso muchos datos vienen de los fragmentos que devolvió el buscador.

Uso tres etiquetas:
- **[No confirmado]:** viene de una sola fuente secundaria, o no pude abrir la página oficial.
- **[Conflicto entre fuentes]:** dos fuentes dicen cosas distintas.
- **[Estimación / conocimiento previo]:** no salió de una búsqueda de esta sesión. Es criterio de la industria o conocimiento general y hay que validarlo antes de comprometer dinero. Aplica sobre todo a **tarifas de freelancers, presupuestos de polígonos y tiempos**.

---

## 1. Resumen ejecutivo (para leer en 3 minutos)

1. **Recomendación: Unity 6 (rama 6.3 LTS), con URP y C#.** Su equipo ya publica en Unity para Meta Quest 3, que por dentro es Android. Eso significa que ya conocen la mitad del camino móvil: firmar el APK/AAB, ADB, IL2CPP y perfilar en Android. Cambiar de motor tiraría ese conocimiento.
2. **Unity Personal es gratis** si la empresa que usa Unity tuvo menos de **USD 200,000 (≈ MXN 3.82 M)** de ingresos más financiamiento en los últimos 12 meses. Desde Unity 6 la pantalla "Made with Unity" es **opcional**. Si la empresa pasa ese límite necesita **Unity Pro: USD 2,310 por puesto al año (≈ MXN 44,100)** desde el 12-ene-2026. **Ojo:** el límite se mide con los ingresos de toda la empresa, no solo del juego. Si INKCAP o el estudio VR ya superan esa cifra, necesitan Pro.
3. **El Runtime Fee (cobro por instalación) se canceló** el 12-sep-2024 y no existe en 2026. Unity regresó al cobro por puesto ([blog oficial](https://unity.com/blog/unity-is-canceling-the-runtime-fee)).
4. **Unity es el motor mejor integrado con las IAs de código en 2026.** Unity lanzó el **9-sep-2026** un **plugin oficial para Claude Code** con 29 *skills* del motor (URP, UI Toolkit, multijugador, compras dentro de la app), su CLI y control del Editor por MCP. Requiere Unity 6 o superior ([Unity Discussions](https://discussions.unity.com/t/unity-s-plugin-is-now-available-in-claude-code/1736143), [PocketGamer.biz](https://www.pocketgamer.biz/unity-launches-official-claude-code-plugin-with-29-built-in-engine-skills/)). Godot y Unreal solo tienen servidores MCP de la comunidad.
5. **El estilo debe ser 3D low-poly con cámara cenital inclinada**, como *Boomerang Fu*, con sombreado *toon*/*matcap* que imite vinil de *art toy*. Presupuesto para gama baja: **2,500–5,000 triángulos por personaje**, texturas de **512 px** y **30 fps estables** en teléfonos tipo Galaxy A07 o Moto G06, que están entre los más vendidos en México.
6. **Multijugador: Netcode for GameObjects + Unity Transport.** En la fase 1 los teléfonos se conectan por IP directa en la misma red Wi-Fi, sin costo de servidores. En la fase 2 se cambia el transporte a **Unity Relay**: los primeros **50 usuarios simultáneos (CCU) promedio son gratis** y después cuesta USD 0.16 por CCU **[No confirmado en la página oficial]**. Si en línea hace falta mejor predicción de movimiento, **Photon Fusion 2** queda como plan B (100 CCU gratis en una app).
7. **Para publicar:** Apple cobra **USD 99/año** y pide D-U-N-S para cuenta de empresa. Google Play cobra **USD 25 una sola vez**. Una **cuenta personal nueva** de Google debe hacer una **prueba cerrada con al menos 12 testers durante 14 días seguidos** antes de producción. **Recomiendo abrir ambas cuentas a nombre de la empresa**: el nombre del vendedor en las tiendas será el de la marca y la cuenta de Google aparentemente evita la regla de 12 testers **[No confirmado]**.
8. **iOS se puede compilar sin Mac propia.** Codemagic da 500 minutos macOS gratis al mes y Unity Build Automation da 100. Aun así, conviene **comprar una Mac mini** (desde ≈ MXN 19,499) cuando empiecen las pruebas serias en iPhone, para depurar y perfilar.
9. **Presupuesto externo [Estimación]:** prototipo **USD 1.5k–5k**, MVP local Wi-Fi **USD 15k–40k** y versión en línea **USD 20k–50k adicionales**. Tiempos: **6–8 semanas** para el prototipo, **4–6 meses más** para el MVP local y **3–6 meses más** para el online.

---

## 2. Comparativa de motores

### 2.1 Tabla general

| Criterio | **Unity 6** | **Godot 4** | **Unreal Engine 5** |
|---|---|---|---|
| Licencia | Personal gratis hasta USD 200k de ingresos+fondos en 12 meses. Pro: USD 2,310/puesto/año (≈ MXN 44,100) | **MIT**: gratis, sin regalías ni pantalla de inicio | Gratis. **5 % de regalías** sobre ingresos brutos de por vida **por encima de USD 1 M** por juego |
| Pantalla de inicio | Opcional desde Unity 6 (también en Personal) | No tiene | No obligatoria |
| Lenguaje | C# | GDScript (principal) o C# (en móvil es menos maduro) **[Conocimiento previo — verificar]** | C++ y Blueprints (visual) |
| Exportación móvil | Muy madura: Android (AAB) e iOS (proyecto Xcode) | Funcional. 4.6 mejoró el renderizador móvil y corrigió fallos Vulkan en GPU Mali y Adreno | Funcional, pero pensada para gama media-alta |
| Tamaño mínimo de APK | ~10 MB en un proyecto vacío según un hilo viejo; un juego real de este tipo debería quedar en 40–120 MB **[Estimación]** | ~13–19 MB en blanco con una sola arquitectura; 114 MB si incluyes ARMv7+ARMv8 o exportas en *debug* | 25–140 MB en blanco, según hilos (mayoría de UE4) |
| Gama baja Android | Buena con URP bien configurado | Buena en 2D; en 3D móvil sigue madurando | La más débil de las tres |
| Multijugador | NGO, Photon, Mirror, FishNet, Relay/Lobby, EOS | API de alto nivel integrada (ENet, WebRTC, WebSocket); sin servicio administrado propio | Replicación integrada excelente, servidores dedicados |
| Anuncios / IAP | SDK oficiales de AdMob, LevelPlay, MAX, Unity IAP y RevenueCat | AdMob comunitario (Poing Studios v5.0.0, jul-2026); facturación de Google Play oficial | Soporte integrado en Online Subsystem y plugins |
| IA de código | **Plugin oficial para Claude Code** (sep-2026), Unity AI Assistant, CLI con MCP | MCP comunitarios (Godot-MCP, Wick, Beckett, Godot AI) | MCP comunitarios (UnrealMCP, Claude Unreal, Jutsu); los Blueprints son difíciles para la IA |
| Tutoriales y comunidad móvil | Los más grandes | Crecen rápido; menos material sobre móvil y monetización | Grandes, pero enfocados en PC y consola |
| Encaje con su equipo | **Total**: ya lo usan para Quest 3 | Tendrían que aprender otro motor | Tendrían que aprender otro motor y C++ |

Fuentes de la tabla: [Unity pricing updates](https://unity.com/products/pricing-updates), [80.lv sobre precios 2026](https://80.lv/articles/unity-announces-its-upcoming-2026-price-changes), [Godot 4.6](https://godotengine.org/releases/4.6/), [Godot Mobile update abr-2026](https://godotengine.org/article/godot-mobile-update-apr-2026/), [licencia Unreal](https://www.unrealengine.com/en-US/license), [PC Gamer sobre el umbral de USD 1 M](https://pcgamer.com/unreal-engine-games-no-longer-owe-royalties-on-their-first-dollar1m-in-revenue), [foro Unity sobre APK mínimo](https://discussions.unity.com/t/android-apk-minimum-file-size/633101), [foro Godot sobre tamaño](https://forum.godotengine.org/t/game-size-too-big-compared-to-apk/39416), [foro Godot 4.5.1, 114 MB](https://forum.godotengine.org/t/absurd-file-size-increase-when-my-project-is-exported-to-apk/130000), [foro Unreal](https://forums.unrealengine.com/t/what-is-the-minimum-size-one-can-achieve-for-android-apk/296898).

### 2.2 Unity 6 a fondo

**Licencias y precios vigentes (nov-2026)**

| Plan | Precio | Condición | Fuente |
|---|---|---|---|
| **Personal** | USD 0 | Ingresos + fondos < **USD 200,000** en los últimos 12 meses. El límite se duplicó de USD 100k a 200k con Unity 6 (oct-2024). Puestos ilimitados | [Unity pricing updates](https://unity.com/products/pricing-updates), [Soporte Unity](https://support.unity.com/hc/en-us/articles/30322080156692) |
| **Pro** | **USD 2,310/puesto/año** (≈ MXN 44,100) o **USD 210/mes** (≈ MXN 4,010) | Obligatorio arriba de USD 200k. Subió 5 % el 12-ene-2026 (antes USD 2,200) | [80.lv](https://80.lv/articles/unity-announces-its-upcoming-2026-price-changes), [Unity pricing updates](https://unity.com/products/pricing-updates) |
| **Enterprise** | Cotización | Para empresas grandes; también subió 5 % en 2026 | Ídem |
| Pantalla de inicio | **Opcional desde Unity 6**, también en Personal | — | [Unity pricing updates (10-nov-2025)](https://unity.com/products/pricing-updates) |

- **El Runtime Fee ya no existe.** Unity lo anunció en sep-2023 como cobro por instalación y lo canceló el **12-sep-2024** "con efecto inmediato" para juegos. A cambio subió el precio de Pro y Enterprise desde el 1-ene-2025 ([blog Unity](https://unity.com/blog/unity-is-canceling-the-runtime-fee), [GamesRadar](https://www.gamesradar.com/games/a-year-later-unity-cancels-its-controversial-runtime-fee-effective-immediately)). El riesgo que queda es de confianza: Unity ya intentó cambiar las reglas una vez. Lo compensa que es el estándar de facto en móvil y que el costo actual es predecible.
- **Qué versión usar.** **Unity 6.3 LTS** salió el 4-dic-2025 y tiene soporte hasta **diciembre de 2027**. Unity 6.0 LTS terminó soporte en octubre de 2026. También existen las versiones "Supported" 6.4, 6.5 (jun-2026) y 6.6 (sep-2026), y **6.7 LTS** viene en camino **[Conflicto entre fuentes sobre fechas exactas]** ([Unity 6.3 LTS](https://unity.com/blog/unity-6-3-lts-is-now-available), [Unity 6 support](https://unity.com/releases/unity-6/support), [Unity 6.5](https://discussions.unity.com/t/unity-6-5-is-now-available/1723176), [Unity Industry 2026](https://unity.com/blog/unity-industry-product-releases-2026), [brewmonster](https://brewmonster.vn/en/blog/unity-6-whats-new/)).
  **Recomendación:** empezar en **6.3 LTS** y pasar a **6.7 LTS** cuando esté estable, antes de lanzar. Unity Hub permite tener varias versiones instaladas, así que no choca con sus proyectos de Quest.
- **Unity AI (asistente dentro del Editor).** En Personal hay una prueba de 14 días con 1,000 créditos y luego cuesta **USD 10/mes por 1,000 créditos** (≈ MXN 191). Pro incluye 2,000 créditos al mes y hasta 3 conexiones MCP. Según la fuente, el uso de agentes externos como Claude Code a través del "AI Gateway" no consume créditos de Unity **[No confirmado: fuentes secundarias]** ([Unity Discussions](https://discussions.unity.com/t/unity-ai-s-open-beta-now-live-for-unity-6/1718560), [vindler.solutions](https://vindler.solutions/blog/unity-ai-open-beta), [aigamer.gg](https://www.aigamer.gg/guides/unity-ai-mcp-claude-code)).

### 2.3 Godot 4 a fondo

- **Costo:** USD 0, licencia MIT, sin regalías ni pantalla de inicio. Es su mayor ventaja.
- **Versión:** la **4.6** trajo un empuje al renderizador *Mobile* en 3D, correcciones de cierres en GPU Mali y Adreno, espejo del dispositivo Android en el Editor y pruebas instrumentadas ([Godot 4.6](https://godotengine.org/releases/4.6/), [Godot Mobile abr-2026](https://godotengine.org/article/godot-mobile-update-apr-2026/)). Wikipedia lista **4.7.2 (ago-2026)** como versión estable **[No confirmado]** ([Wikipedia](https://en.wikipedia.org/wiki/Godot_(game_engine))). La app **GABE**, que permite compilar AAB y APK desde el propio Android, quedó estable el 2-jun-2026 ([Godot](https://godotengine.org/article/gabe-stable-release/)).
- **iOS:** sigue necesitando **macOS + Xcode**. Godot exporta un proyecto Xcode, no un instalable ([strayspark](https://www.strayspark.studio/blog/godot-46-export-web-android-ios-guide)).
- **Monetización:**
  - Hay un **plugin oficial de Google Play Billing** para Godot 4 ([docs Godot](https://docs.godotengine.org/en/stable/tutorials/platform/android/android_in_app_purchases.html)).
  - **AdMob** depende de un plugin comunitario: Poing Studios v5.0.0, MIT, publicado el 22-jul-2026, para Godot 4.2+ en Android e iOS ([Asset Library](https://godotengine.org/asset-library/asset/2063), [Asset Store](https://store.godotengine.org/asset/poingstudios/admob/)).
  - **No encontré SDK oficiales de LevelPlay ni AppLovin MAX para Godot.** En la práctica eso te deja con menos opciones de mediación y con menos ingresos por anuncio.
- **Redes:** la API de alto nivel trae ENet (UDP), WebRTC y WebSocket ([docs Godot](https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html)). No incluye un *relay* administrado como Unity Relay o Photon; tendrías que montarlo o contratarlo aparte. Un análisis de 2026 resume a Godot móvil como **"listo para juegos premium, no para *live service*"** ([ziva.sh](https://ziva.sh/blogs/godot-mobile)). Eso es justo lo contrario de lo que queremos: un juego en línea con progresión diaria y tienda.
- **Veredicto:** es una gran opción para un estudio que empieza de cero y quiere costo cero. Para ustedes implicaría mantener dos motores y perder el plugin oficial de IA.

### 2.4 Unreal Engine 5 a fondo

- **Regalías:** 5 % sobre ingresos brutos de por vida **por encima de USD 1 M** por juego (≈ MXN 19.1 M). Si el juego sale en Epic Games Store el mismo día o antes que en otras tiendas, la tasa baja a **3.5 %** desde el 1-ene-2025 (programa *Launch Everywhere with Epic*). Para iOS Epic quitó el requisito de lanzamiento simultáneo ([Unreal Fest 2024](https://www.unrealengine.com/blog/catch-up-on-the-big-news-from-unreal-fest-seattle-2024), [PocketGamer.biz](https://www.pocketgamer.biz/unreal-engine-royalty-fee-reducing-to-35-for-games-landing-on-epic-games-store-on-launch-day)). Las empresas **no** de videojuegos con más de USD 1 M de ingresos pagan por puesto ([blog Epic](https://www.unrealengine.com/blog/we-are-updating-unreal-engine-twinmotion-and-realitycapture-pricing-in-late-april)).
- **Peso y rendimiento en gama baja:** hay reportes de proyectos vacíos de **60–140 MB** en Android. Un usuario logró 25 MB de APK más 19 MB de OBB con un mapa en blanco. Epic documenta trucos para reducir el binario ([Epic docs](https://dev.epicgames.com/documentation/unreal-engine/reducing-android-binary-size-in-unreal-engine-projects), [foro](https://answers.unrealengine.com/t/why-is-a-compiled-mobile-project-so-huge/425988)). Las funciones estrella de UE5 (Nanite, Lumen) no sirven en teléfonos de gama baja. Para un juego que debe correr en un Galaxy A07 es pagar complejidad sin beneficio.
- **IA:** los Blueprints son archivos binarios y visuales, y una IA de texto no los lee bien. Existen puentes MCP comunitarios como UnrealMCP (requiere UE 5.7), Claude Unreal en Fab y Jutsu (UE 5.5–5.8), pero son de terceros y no están verificados ([PyPI UnrealMCP](https://pypi.org/project/unrealmcp/), [foro Unreal](https://forums.unrealengine.com/t/carlos-li-claude-unreal/2720506)).
- **Veredicto:** no conviene para este juego.

### 2.5 Otros motores, en breve **[Conocimiento previo — verificar precios]**

| Motor | Qué es | Por qué sí | Por qué no para nosotros |
|---|---|---|---|
| **Defold** | Gratis, Lua, de la Defold Foundation (nació en King) | APKs diminutos y rendimiento excelente en gama baja | Hecho para 2D; las herramientas 3D son limitadas y la comunidad es chica |
| **Cocos Creator** | Gratis, TypeScript, 2D y 3D | Builds ligeros; muy usado en China y en minijuegos | Poca comunidad y documentación en español o inglés; plugins de anuncios menos estándar |
| **GameMaker** | Free para uso no comercial; Professional ≈ USD 99.99 pago único para comercial | Excelente para 2D | Prácticamente solo 2D; no sirve para un brawler 3D cenital |
| **Flax** | C# y C++, 3D moderno; gratis con regalía (≈ 4 % arriba de cierto umbral trimestral) | Parecido a Unity y ligero | Comunidad muy pequeña; soporte móvil inmaduro |

### 2.6 Matriz de decisión (calificación propia, 1–5)

| Criterio (peso) | Unity | Godot | Unreal | Defold | Cocos |
|---|---|---|---|---|---|
| Encaje con el equipo y con la IA de código (25 %) | 5 | 3 | 2 | 2 | 2 |
| Rendimiento en gama baja y tamaño del APK (20 %) | 4 | 4 | 2 | 5 | 5 |
| Multijugador local → online (15 %) | 5 | 3 | 4 | 2 | 2 |
| Anuncios e IAP (15 %) | 5 | 3 | 3 | 3 | 3 |
| Costo de licencia (10 %) | 4 | 5 | 4 | 5 | 5 |
| Comunidad, tutoriales y *assets* (10 %) | 5 | 3 | 4 | 2 | 2 |
| Riesgo legal o de plataforma (5 %) | 3 | 5 | 4 | 5 | 4 |
| **Total ponderado** | **4.60** | 3.50 | 2.95 | 3.20 | 3.15 |

---

## 3. Qué tan bien trabajan las IAs de código con cada motor

Para un equipo que **no programa a mano**, este criterio pesa tanto como el rendimiento.

| Herramienta | Unity | Godot | Unreal |
|---|---|---|---|
| **Claude Code** | **Plugin oficial de Unity** (9-sep-2026): 29 *skills*, Unity CLI y control del Editor por MCP. Se instala con `/plugin marketplace add Unity-Technologies/unity-agent-plugin` y luego `/plugin install unity@unity-agent-plugin`. Requiere Unity 6+ ([PocketGamer.biz](https://www.pocketgamer.biz/unity-launches-official-claude-code-plugin-with-29-built-in-engine-skills/), [AICoder](https://aicoder.com/news/news-20260913-unity-claude-code-plugin)) | MCP comunitarios: Godot-MCP de ivanmurzak (GDScript y C#, capturas del *viewport*), Wick, Beckett (valida que el GDScript compile antes de escribirlo) y Godot AI ([mcpservers.org](https://mcpservers.org/servers/ivanmurzak/godot-mcp), [foro Godot](https://forum.godotengine.org/t/beckett-zero-sidecar-mcp-server-for-godot-4-2-the-ai-sees-and-optionally-playtests-your-game/141177)) | MCP comunitarios (UnrealMCP, Claude Unreal, Jutsu, PinWright) |
| **Servidor MCP** | El servidor MCP del paquete AI Assistant **quedó obsoleto** y lo reemplaza `unity mcp` de la Unity CLI, gratis y sin límite de conexiones. Para versiones anteriores a Unity 6 existe el paquete comunitario "MCP for Unity" **[Conflicto entre fuentes sobre la fecha]** ([dev.to](https://dev.to/indiebots/unitys-mcp-server-is-deprecated-what-to-use-instead-2026-3khl), [docs Unity MCP](https://docs.unity3d.com/Packages/com.unity.ai.assistant@2.7/manual/integration/unity-mcp-get-started.html)) | Ídem a la fila anterior | Ídem |
| **Asistente dentro del Editor** | Unity AI Assistant; su AI Gateway permite correr Claude Code, Codex, Gemini o Cursor dentro de la ventana del asistente | No tiene oficial | No confirmado en esta investigación |
| **Cursor / Copilot** | Funcionan muy bien con C# en VS Code o Rider | Funcionan, con menos precisión en GDScript | Funcionan con C++; no ayudan con Blueprints |
| **Calidad del código generado** | Alta: C# es muy común en los datos de entrenamiento | Media: las IAs suelen mezclar sintaxis de Godot 3 y 4 **[Conocimiento previo]** | Media-baja |

**Buenas prácticas para un equipo que programa con IA** (estas son recomendaciones mías, no salen de una fuente):
1. Tengan un **CLAUDE.md** con las reglas del proyecto: arquitectura, nombres, "no usar `FindObjectOfType` en `Update`", y cosas así.
2. Pidan tareas pequeñas y comprobables: "el disco regresa al dueño en 1.2 s con esta curva", no "haz el juego".
3. Usen **Git** y hagan commit tras cada paso que funcione.
4. Escriban **pruebas automáticas** con Unity Test Framework para la lógica de puntajes, XP y recompensas diarias.
5. **Perfilen en el teléfono real**, no en el Editor. Unity mismo advierte que el Editor no refleja el rendimiento en Android ([foro Unity](https://discussions.unity.com/t/unity-6-mobile-graphics-optimization-tips-and-tricks/1593293)).
6. **Contraten a un programador senior de Unity a tiempo parcial**, 5–10 h por semana, para revisar el código de red y el de compras. Ahí los errores de la IA son sutiles y caros.

---

## 4. ¿2D o 3D? Estilo visual y presupuesto técnico

### 4.1 Decisión: 3D low-poly con cámara cenital inclinada

*Boomerang Fu* es 3D con cámara desde arriba. Para nuestro concepto el 3D es casi obligatorio por tres razones:
- Los personajes son **muñequitos tipo art toy**, figuras de vinil con volumen.
- Los trajes son **playeras y gorras reales** de INKCAP que deben verse puestas sobre el cuerpo.
- Su otra empresa ya domina el modelado y escaneo 3D.

**Cámara:** perspectiva con FOV bajo (30–35°) e inclinación de 55–65°, o cámara **ortográfica** si queremos un look más "diorama". Con el **Target Group de Cinemachine** la cámara se acerca o se aleja sola para que siempre se vea a todos los jugadores, algo clave en una pantalla de 6 pulgadas.

**Sombreado recomendado:** *toon* o **matcap**, hecho con Shader Graph. El matcap es baratísimo de calcular y da el brillo de un juguete de vinil. Le sumamos un contorno de casco invertido para el look de ilustración o tatuaje. Nada de PBR caro.

### 4.2 Presupuesto de rendimiento para Android gama baja **[Estimación: reglas prácticas de la industria]**

Android tiene ~78.7 % del mercado móvil en México e iOS ~21.3 % (jul-2026, [StatCounter](https://gs.statcounter.com/os-market-share/mobile/mexico)). Entre los Android más vendidos de 2026 están el **Samsung Galaxy A07** y el **Moto G06** ([Luz Noticias](https://www.luznoticias.mx/2026-03-17/mexico/celulares-android-que-mas-se-venden-en-mexico-este-2026-/282031), [Merca2.0](https://www.merca20.com/cuales-son-los-celulares-android-mas-populares-en-mexico-en-2026-y-por-que-samsung-domina-el-mercado/)). Hay que diseñar para ese piso.

| Elemento | Gama baja (objetivo) | Gama media/alta |
|---|---|---|
| Personaje (muñequito) | 2,500–5,000 triángulos, 1 material | hasta 8,000 |
| Huesos del rig | ≤ 30, con 2 influencias por vértice en calidad baja | ≤ 45, con 4 influencias |
| Textura base del personaje | 512 × 512 (ASTC) | 1024 × 1024 |
| Textura del estampado de la playera | 256–512 px | 512–1024 px |
| Gorra o accesorio | 300–1,000 triángulos, textura de 256 px | — |
| Objeto arrojadizo | 300–800 triángulos | — |
| Arena completa | 30k–60k triángulos, 1–2 atlas de 1024–2048 px | ~100k |
| Total en pantalla con 6 jugadores | ≤ 100k–150k triángulos | ≤ 300k |
| Draw calls / batches | ≤ 80–120 (SRP Batcher activo) | ≤ 200 |
| Iluminación | 1 luz direccional **sin sombras en tiempo real**, sombras falsas (*blob*) y lightmaps horneados | 1 cascada de sombras suaves |
| Post-procesado | Ninguno, o solo un LUT de color | Bloom ligero |
| Escala de render | 0.7–0.85 | 1.0 |
| Cuadros por segundo | **30 fijos**, con opción de 60 | 60 |
| Descarga inicial | **< 150 MB**; los trajes se bajan aparte | — |

Recomendaciones que sí vienen de la documentación oficial y de foros:
- Activa el SRP Batcher y apaga sombras donde se pueda, porque reducen las draw calls.
- Cuidado con las sondas de reflejo, que salen muy caras.
- Cada cámara extra puede costar hasta 1 ms de CPU.
- En gama baja usa *Store Actions* en Auto o Discard, Baked Lit para lo estático y Simple Lit para lo dinámico ([docs URP](https://docs.unity3d.com/Manual/urp/configure-for-better-performance.html)).
- Unity 6.1 permite elegir la API gráfica por dispositivo: OpenGL ES en los viejos y Vulkan en los nuevos ([blog Unity](https://unity.com/blog/mobile-recap-new-tools-for-performance-in-unity-6-1)).
- Un hilo que apunta al Galaxy A14 (GPU Mali-G52) sugiere como punto de partida escala de render 0.5, sin bloom y sin antialiasing ([foro Unity](https://discussions.unity.com/t/unity-6-mobile-graphics-optimization-tips-and-tricks/1593293)).
- Hay reportes de Unity 6 rindiendo peor que 2022.3 en ciertos Android ([foro](https://discussions.unity.com/t/unity-6-android-urp-baseline-frametime-50ms-20-fps/1566399)), así que hay que medir desde la semana 1.

---

## 5. Pipeline de arte y animación

### 5.1 Modelado (Blender)
1. **Concepto 2D** del muñequito: proporción chibi con cabeza grande, en pose T o A.
2. **Modelado en Blender**, que es gratis. Si INKCAP tiene *art toys* físicos, **su empresa de escaneo puede escanearlos, hacer retopología a 3–5k triángulos y hornear el color a textura**. Es una ventaja única: los muñequitos del juego serían versiones digitales fieles de las figuras edición limitada. Eso se presta a una mecánica de "compra el art toy y desbloquea el personaje", sujeta a las reglas de las tiendas (ver la sección 9, dudas).
3. **Un mapa UV con una isla dedicada al "área de estampado"** en el pecho y otra en la espalda de la playera. Este paso es la base del sistema de trajes.
4. Exportar en FBX o glTF a Unity con escala y ejes consistentes.

### 5.2 Cómo convertir diseños de playeras (PNG) en texturas intercambiables

La idea es que el **cuerpo del muñequito no cambie** y solo cambie el estampado, la gorra o el color.

1. **Malla base de la playera** como parte del personaje, o como malla separada pegada al mismo esqueleto.
2. **Shader "PlayeraINKCAP"** hecho en Shader Graph, con cinco propiedades:
   - `ColorTela`: tinte de la tela, para tener negra, blanca, arena, etc.
   - `EstampadoFrente` y `EstampadoEspalda`: los PNG con transparencia.
   - `MáscaraEstampado`: delimita dónde se permite imprimir.
   - Usa un **segundo canal UV (UV1)** que mapea solo el área del pecho a 0–1. Así cualquier PNG cuadrado cae centrado sin re-texturizar el modelo.
3. **Herramienta de editor** (Unity o script de Blender/Python; la IA la puede escribir en un día). Recibe el PNG original del diseño, por ejemplo de 4,000 px. Lo recorta, lo centra, lo reduce a 512 y a 256 px, lo comprime en ASTC y genera un **ScriptableObject "Cosmético"** con estos campos:
   - ID y nombre
   - artista (para calcular el 15 % de regalías)
   - rareza
   - textura y color base
   - SKU de la tienda y precio
   - fecha de "Estreno" o bandera de "Edición Limitada"
4. **Gorras y accesorios:** mallas separadas pegadas a un hueso "Cabeza", con la misma lógica de textura intercambiable.
5. **Distribución con Addressables:** los trajes nuevos se descargan desde un CDN sin actualizar la app. Puede ser Unity Cloud Content Delivery o uno propio. Así se puede lanzar un "drop" el mismo día que sale la playera física en la tienda en línea.

### 5.3 Animación **[Conocimiento previo — verificar]**
- **Mixamo** (gratis con cuenta Adobe) sirve para rigs humanoides y animaciones base. **Pero** las proporciones chibi, con cabeza enorme y extremidades cortas, se adaptan mal a sus animaciones. Úsalo para prototipar.
- Para la versión final: un **rig propio y simple** en Blender (Rigify o hecho a mano, ≤ 30 huesos) con **6–10 animaciones hechas a mano**: idle, correr, lanzar, atrapar, dash, golpeado, victoria, derrota y un baile o burla. En personajes chibi las animaciones exageradas y cortas se ven mejor que el *motion capture*.
- Alternativas: AccuRIG (rigging automático) y Cascadeur, que tiene versión gratuita para animación con física.
- En Unity usa el tipo **Humanoid** si quieres compartir animaciones entre personajes, o **Generic** si cada muñequito tiene proporciones muy distintas.

### 5.4 Escenarios de arte
- Usa ***trim sheets* y atlas** para las calles grafiteadas: los grafitis van horneados en la textura, no como calcomanías en tiempo real.
- El **museo** se arma con pocos materiales, pisos brillantes falsos (matcap) y esculturas low-poly que hacen de obstáculos.
- **ProBuilder**, gratis dentro de Unity, sirve para hacer las arenas en *greybox* antes del arte final.

---

## 6. Paquetes y servicios de Unity recomendados

| Paquete o servicio | Para qué | Costo |
|---|---|---|
| **URP** (Universal Render Pipeline) | Render móvil | Incluido |
| **Input System** + *On-Screen Stick* | Joystick táctil y control Bluetooth | Incluido |
| **Cinemachine 3** | Cámara que encuadra a todos (Target Group) y temblores de cámara | Incluido |
| **Shader Graph** | Shader toon/matcap y el de la playera | Incluido |
| **Addressables** | Trajes y escenarios descargables | Incluido; el CDN aparte |
| **TextMeshPro / UGUI** | Texto e interfaz | Incluido |
| **Localization** | Español de México e inglés | Incluido |
| **Mobile Notifications** | "Tu recompensa diaria te espera" | Incluido |
| **Netcode for GameObjects 2.x + Unity Transport** | Multijugador local y luego en línea. Desde la 2.0 usa la Unity Companion License | Incluido ([Unity docs](https://docs.unity3d.com/Manual/multiplayer-new.html)) |
| **Multiplayer Services** (Relay, Lobby, Sessions) | Fase online | Nivel gratuito más pago por uso |
| **Authentication + Cloud Save** (o Firebase) | Guardar XP, nivel, racha diaria e inventario | Nivel gratuito **[verificar límites]** |
| **Analytics + Crashlytics** (Unity o Firebase) | Métricas y reporte de errores | Nivel gratuito |
| **Unity IAP 5** o **RevenueCat** | Compras de trajes y pase de temporada | Unity IAP gratis. RevenueCat gratis hasta USD 2,500 de ingresos mensuales rastreados; después 1 % del total ([RevenueCat](https://revenuecat.com/pricing), [usagepricing](https://www.usagepricing.com/blueprint/revenuecat)) |
| **DOTween** | Animaciones de interfaz y "jugo" por código | Gratis; versión Pro de pago **[verificar precio]** |
| **Feel** (More Mountains) | Paquete de efectos de impacto: *hit-stop*, temblor, partículas, vibración | De pago en Asset Store **[verificar precio]**. Opcional pero muy recomendable para un brawler |
| **ProBuilder** | Arenas en *greybox* | Incluido |
| **Unity Test Framework** | Pruebas automáticas | Incluido |
| **Multiplayer Play Mode** | Probar varios jugadores en el Editor | Incluido en Unity 6 **[Conocimiento previo]** |

**Anuncios: cuál red usar**

| Plataforma | Ventaja | Desventaja | Recomendación |
|---|---|---|---|
| **AdMob** (Google) | Lo más simple; es Google | Según fuentes de la industria paga menos en video recompensado de juegos | Para empezar |
| **Unity LevelPlay** (antes ironSource) | Integrada al flujo de Unity y enfocada en juegos | Términos de reparto menos transparentes. La entrega de anuncios de ironSource Exchange y del SDK iAds termina el **31-dic-2026**; LevelPlay sigue **[No confirmado]** | Buena opción con Unity |
| **AppLovin MAX** | La mediación independiente dominante, con subastas entre más de 25 redes. La mediación es gratis para el publicador | Más compleja | Cuando haya volumen |

Fuentes: [undrads](https://www.undrads.com/blogs/ironsource-vs-applovin-max-vs-admob), [appagent](https://appagent.com/blog/best-ad-networks-for-mobile-games/). **No encontré datos de eCPM para Latinoamérica.** Hay que hacer pruebas A/B con tráfico real. Para un *party game*, recomiendo **solo video recompensado** ("ve un anuncio y duplica tu XP"), sin anuncios forzados entre rondas.

---

## 7. Multijugador: de Wi-Fi local a en línea

Aquí solo trato la parte técnica. Los costos de servidores con más detalle van en el reporte de juego en línea.

| Opción | Wi-Fi local sin internet | En línea | Costo | Encaje |
|---|---|---|---|---|
| **Netcode for GameObjects (NGO) + Unity Transport** | **Sí**, por IP directa. La búsqueda automática de partidas en la red es un componente comunitario por *broadcast* UDP, sin garantía en móvil | Con **Unity Relay** o con *Distributed Authority* | Gratis. Relay: 50 CCU promedio gratis, luego ~USD 0.16 por CCU **[No confirmado]** | **Recomendado** |
| **Photon Fusion 2** | **No es su caso de uso**: requiere la nube de Photon. No confirmé un modo LAN sin internet | Excelente, con predicción y modos *Shared* o *Host* | 100 CCU gratis en una app; 100 CCU pagados = USD 95 por 12 meses (≈ MXN 1,815); 500 CCU ≈ USD 125/mes **[No confirmado en la página oficial]** | Plan B para la fase online |
| **Mirror** | Sí, con búsqueda en red integrada | Necesita *relay* u hosting propio | Gratis (open source) | Alternativa |
| **FishNet** | Sí | Ídem; buena predicción del lado del cliente | Gratis | Alternativa técnica fuerte |

Fuentes: [hilo LAN con NGO](https://discussions.unity.com/t/creating-an-offline-lan-multiplayer-game-using-netcode-for-gameobjects-relay/1524568), [NetworkDiscovery comunitario](https://github.com/Optifun/multiplayer-community-contributions/blob/main/com.community.netcode.extensions/Runtime/NetworkDiscovery), [precios de Relay en el espejo de unity.cn](https://activation.unity.cn/products/relay), [foro sobre precios de Relay](https://forum.unity.com/threads/relay-lag-issues-and-pricing.1530151/), [Photon: 100 CCU gratis](https://blog.photonengine.com/new-free-100-ccu-for-photon-fusion-and-quantum-games/), [Photon: paquete Plus](https://blog.photonengine.com/new-200-ccu-plus-package-100-paid-100-free/), [precios Fusion 2026](https://crux.supercraft.host/blog/photon-fusion-pricing-2026/), [topologías de Fusion](https://doc.photonengine.com/fusion/v2/manual/network-topologies), [foro Photon sobre LAN](https://forum.photonengine.com/discussion/15150/photon-self-hosted-server-without-internet-lan), [comparativa de Ocean View](https://oceanviewgames.co.uk/blog/posts/cross-platform-multiplayer-networking-unity).

**Arquitectura recomendada:** el anfitrión (*host*) es el teléfono de un jugador y los demás se conectan a él.

- **Fase 1, local:** el anfitrión muestra un **código QR con su IP**. Los demás lo escanean y se conectan, sin escribir números. También funciona si todos se conectan al **hotspot** del teléfono anfitrión.
- **Fase 2, online:** el mismo código de juego pero con transporte Relay. Se cambia "QR con IP" por "código de sala de 6 letras". La lógica del juego **no se reescribe**.

**Trampas de la red local** **[Conocimiento previo — verificar con Apple]**:
- **iOS 14 en adelante** pide el permiso de "Red local" (`NSLocalNetworkUsageDescription`).
- Para enviar ***broadcast* o *multicast*** (la búsqueda automática de partidas) Apple exige un **permiso especial que hay que solicitarle** (`com.apple.developer.networking.multicast`). El QR con IP evita ese trámite.
- Muchas redes públicas o de invitados (cafés, escuelas) tienen **aislamiento de clientes**, que bloquea la conexión entre teléfonos. El hotspot resuelve ese caso.

**Alternativa a evaluar:** **Epic Online Services (EOS)** ofrece *lobbies*, *relay* P2P y *matchmaking* sin costo, y tiene plugin para Unity **[Conocimiento previo — verificar términos vigentes]**.

---

## 8. Requisitos para publicar

### 8.1 Apple App Store

| Concepto | Dato | Fuente |
|---|---|---|
| Apple Developer Program | **USD 99/año** (≈ MXN 1,890; Apple puede mostrarlo en moneda local) | [developer.apple.com, página consultada directamente](https://developer.apple.com/support/enrollment/) |
| Persona física vs empresa | Como **persona** el vendedor en la App Store aparece con tu nombre legal. Como **organización** aparece el nombre de la entidad legal. Los negocios unipersonales deben inscribirse como persona | Ídem |
| Requisitos de organización | Entidad legal (no se aceptan nombres comerciales ni DBA); **número D-U-N-S** (9 dígitos, gratis); autoridad para firmar contratos; correo con el dominio de la empresa; sitio web público y funcional. Apple puede pedir documentos notariados | Ídem |
| SDK mínimo | Desde el **28-abr-2026** toda app nueva se compila con **Xcode 26** y el SDK de iOS 26. La versión mínima de iOS que soporta tu app puede seguir siendo vieja | [Apple: próximos requisitos](https://developer.apple.com/news/upcoming-requirements/), [9to5Mac](https://9to5mac.com/2026/02/03/apple-to-update-minimum-sdk-requirements-for-all-app-store-submissions/) |
| Comisión | 15 % para desarrolladores con menos de USD 1 M al año (Small Business Program); 30 % por encima | **[Conocimiento previo — verificar]** |

**Recomendación:** inscribirse como **organización**, a nombre de la sociedad que será dueña del juego. Así la tienda muestra la marca. Tramiten el D-U-N-S **ya**: puede tardar hasta ~30 días (Google habla de hasta 30 días en su ayuda y de 28 en una FAQ de jul-2026, [Android FAQ](https://developer.android.com/developer-verification/guides/faq)).

### 8.2 Google Play

| Concepto | Dato | Fuente |
|---|---|---|
| Registro | **USD 25 una sola vez** (≈ MXN 478) | [Ayuda de Play Console](https://support.google.com/googleplay/android-developer/answer/13628312) |
| Prueba cerrada obligatoria | Las **cuentas personales creadas después del 13-nov-2023** deben hacer una prueba cerrada con **al menos 12 testers inscritos de forma continua durante 14 días** antes de pedir acceso a producción. Algunos blogs dicen que antes eran 20 testers **[Conflicto entre fuentes sobre la fecha del cambio]** | [Ayuda Google](https://support.google.com/googleplay/android-developer/answer/14151465), [ontest.app](https://ontest.app/blog/google-play-12-testers-14-days-requirement-explained) |
| Cuentas de organización | Necesitan D-U-N-S y según blogs **están exentas** de la prueba de 12 testers **[No confirmado por Google]** | [dev.to](https://dev.to/samfadigitalstack/google-play-closed-testing-a-practical-guide-to-the-12-testers-and-14-days-requirement-1p03), [Ayuda Google, D-U-N-S](https://support.google.com/googleplay/android-developer/answer/13634885) |
| API objetivo | Desde el **31-ago-2026** las apps nuevas deben apuntar a **Android 16 (API 36)** | [Android Developers](https://developer.android.com/google/play/requirements/target-sdk) |
| Páginas de 16 KB | Las apps para Android 15+ deben soportar páginas de memoria de 16 KB. Unity lo soporta desde 6000.0.38f1 (Unity 6.0) y 2022.3.56f1. Los plugins nativos de terceros pueden romperlo | [Unity Discussions](https://discussions.unity.com/t/info-unity-engine-support-for-16-kb-memory-page-sizes-android-15/1589588), [Android Developers](https://developer.android.com/games/develop/build-in-unity) |
| Verificación de identidad | Desde sep-2026 Google exige identidad, y D-U-N-S para organizaciones, incluso a quien distribuye fuera de Play | [Biometric Update](https://www.biometricupdate.com/202508/google-unveils-identity-verification-rules-for-android-app-developers) |
| Formato | AAB obligatorio para apps nuevas y solo 64 bits | **[Conocimiento previo]** |

### 8.3 ¿Necesitamos una Mac?

Para **compilar iOS hace falta macOS con Xcode**, sea propio o en la nube. El flujo es: Unity genera un proyecto Xcode y la Mac lo compila y lo firma.

| Opción | Costo | Comentario |
|---|---|---|
| **Mac mini propia** | M4 (16 GB / 512 GB) desde **MXN 19,499** en la Apple Store México. Hay un modelo nuevo con chip M6 desde **MXN 19,999** (preventa sep-2026) | La mejor para depurar y perfilar en iPhone ([Xataka, M4](https://www.xataka.com.mx/ordenadores/apple-acaba-eliminar-uno-sus-productos-atractivos-mexico-mac-mini-15-mil-pesos), [Xataka, M6](https://www.xataka.com.mx/ordenadores/nueva-apple-mac-mini-m6-m5-pro-lanzamiento-precio-mexico-caracteristicas-especificaciones-ficha-tecnica)) **[Verificar disponibilidad]** |
| **Unity Build Automation** | 100 minutos Mac gratis cada mes; después ~USD 0.07 por minuto (≈ MXN 1.34) **[No confirmado para 2026]** | Integrado con Unity ([Unity Discussions](https://discussions.unity.com/t/is-mac-build-100-free-minutes-for-unity-build-after-q1-of-2026-every-month-or-only-once/1696004)) |
| **Codemagic** | **500 minutos macOS M2 gratis al mes** (cuenta personal); después USD 0.095 por minuto (≈ MXN 1.81) | El nivel gratuito más generoso ([docs Codemagic](https://docs.codemagic.io/billing/pricing/)) |
| **GitHub Actions (macOS)** | ~USD 0.062 por minuto. El plan Free incluye un crédito mensual equivalente a ~190 minutos macOS **[No confirmado; fuentes secundarias]** | Requiere GameCI o similar. Activar una licencia Personal en CI tiene complicaciones ([lizard.build](https://lizard.build/blog/github-actions-pricing), [GameCI](https://www.github.com/game-ci/unity-builder), [foro Unity](https://discussions.unity.com/t/unity-no-longer-supports-manual-activation-of-personal-licenses/926760)) |

**Ejemplo:** 30 builds de iOS al mes × ~20 min = 600 min. En Codemagic serían 100 min por encima del nivel gratuito, unos **USD 9.50 (≈ MXN 180) al mes** **[Estimación]**. **Recomendación:** Codemagic o Build Automation durante el prototipo y una Mac mini al iniciar el MVP.

### 8.4 Dispositivos de prueba recomendados **[Estimación de precios]**

| Dispositivo | Por qué | Precio aprox. |
|---|---|---|
| Samsung Galaxy A07 (o A06) | Gama baja y de los más vendidos en México | MXN 2,500–3,500 |
| Motorola Moto G06 | Gama baja y de los más vendidos | MXN 2,500–3,000 |
| Xiaomi Redmi de entrada (serie A o 14C) | Otro fabricante y otra GPU | MXN 2,000–3,000 |
| Samsung Galaxy A26/A36 o Redmi Note | Gama media de referencia | MXN 5,000–8,000 |
| iPhone 11 o SE (2a/3a gen.), usado | El iPhone más viejo que soportaremos | MXN 4,000–6,000 |
| iPhone reciente (el de alguien del equipo) | Referencia de gama alta | — |
| Router Wi-Fi propio | Probar la red local de forma controlada | MXN 600–1,200 |
| **Total aproximado** | | **≈ MXN 20,000–28,000** |

---

## 9. Equipo, tarifas y tiempos

### 9.1 Roles necesarios y tarifas aproximadas en México y LATAM **[Estimación — no verificada con fuentes en esta sesión; validar con cotizaciones reales en Workana, Upwork, LinkedIn o ArtStation]**

| Rol | Dedicación sugerida | Tarifa por hora USD (MXN) | Por proyecto (referencia) |
|---|---|---|---|
| **Programador Unity senior** (revisor del código de la IA, red y compras) | 5–10 h/semana en el MVP; 15–20 h en online | 35–60 (670–1,150) | — |
| Programador Unity *mid* (si no basta la IA) | Tiempo parcial | 20–35 (380–670) | — |
| **Artista 3D de personajes** estilizados | Por pieza | 20–40 (380–760) | USD 250–700 por muñequito low-poly texturizado (MXN 4,800–13,400) |
| Artista 3D de escenarios | Por arena | 20–35 | USD 800–2,500 por arena (MXN 15,300–47,800) |
| Rigger / animador | Por pieza | 15–35 (290–670) | Rig de USD 100–300; USD 30–80 por animación simple |
| Diseñador de UI/UX para juegos | Por paquete | 15–35 | USD 1,000–3,000 por la interfaz del MVP |
| Sonido (efectos y música) | Por paquete | — | Efectos: USD 300–1,200. Música: USD 150–600 por pista en loop, o bibliotecas con licencia |
| QA (pruebas en dispositivos) | Por jornada | 6–12 (115–230) | — |
| Productor / diseñador del juego | Interno (el cliente) | — | — |

Como referencia, los sueldos de desarrolladores Unity en México suelen rondar **MXN 25,000–60,000 al mes** según experiencia **[Estimación]**.

### 9.2 Tiempos y presupuesto por etapa **[Estimación]**

| Etapa | Duración | Entregable | Gasto externo aproximado |
|---|---|---|---|
| **0. Preparación** | 2 semanas | Cuentas (D-U-N-S, Apple, Google), repositorio, plantilla Unity 6.3 URP móvil, CLAUDE.md, compra de dispositivos | USD 125 en cuentas + ≈ MXN 20–28k en dispositivos |
| **1. Prototipo jugable** | **6–8 semanas** | 1 arena en *greybox*, 1 muñequito placeholder, lanzar/atrapar/dash, un golpe = KO, rondas, bots básicos, 2–4 teléfonos por Wi-Fi local con QR | **USD 1.5k–5k** (revisión senior más un personaje de prueba) |
| **2. Vertical slice** | 6–8 semanas | Estilo final con 2 muñequitos y 1 arena, "jugo" (Feel), sonido, interfaz base | Incluido en la etapa 3 |
| **3. MVP local Wi-Fi** | **4–6 meses** después del prototipo | 6–8 muñequitos, 3 arenas (calle grafiteada, museo, taller de serigrafía o tatuaje), 3–4 modos, XP y niveles, recompensa diaria, inventario de trajes, analítica, crash reports, prueba cerrada y publicación | **USD 15k–40k** |
| **4. Lanzamiento suave en México** | 1–2 meses | Métricas de retención (D1, D7) y ajustes | USD 1k–5k (QA y marketing de prueba) |
| **5. Versión en línea** | **3–6 meses** | Relay y Lobby (o Fusion), autenticación, economía validada en servidor, predicción y compensación de lag, regiones | **USD 20k–50k** más servicios por uso |
| **6. Live ops** | Continuo | Pase de temporada, drops ligados a "Estrenos" y "Edición Limitada", eventos con artistas | Según el ritmo de contenido |

Cómo se arma el MVP de **USD 15k–40k**:
- 6–8 personajes × USD 400–700
- rigs y animaciones por USD 3k–6k
- 3 arenas × USD 1.5k–2.5k
- interfaz por USD 1.5k–3k
- audio por USD 1k–2.5k
- programador senior: 20 semanas × 8 h × USD 45, unos USD 7k
- QA

---

## 10. Recomendación de motor y stack

**Motor: Unity 6, rama 6.3 LTS**, con migración planeada a 6.7 LTS antes del lanzamiento. Las razones, en orden de peso:

1. **El equipo ya lo usa.** Su experiencia en Quest 3 (Android, IL2CPP, URP, perfilado) sirve directamente. Cambiar de motor costaría meses.
2. **Es el motor con mejor soporte de IA en 2026**: plugin oficial para Claude Code, CLI con MCP, Unity AI Assistant y C# como lenguaje bien conocido por las IAs.
3. **Tiene el ecosistema móvil más completo**: AdMob, LevelPlay y MAX; Unity IAP y RevenueCat; Relay, Lobby y Photon.
4. **El costo es predecible.** Es gratis mientras la empresa factura menos de USD 200k al año. Si la superan, son USD 2,310 por puesto al año, sin regalías y sin cobro por instalación.
5. **Rinde en gama baja** con URP bien configurado y tiene los tutoriales y *assets* más abundantes.

**Stack:**

| Capa | Elección | Alternativa |
|---|---|---|
| Motor y render | Unity 6.3 LTS + URP + Shader Graph (toon/matcap) | — |
| Lenguaje e IA | C# + **Claude Code con el plugin oficial de Unity** + Git/LFS | Cursor o Copilot; Unity AI Assistant |
| Arte | Blender + escaneo 3D propio + atlas y trim sheets | — |
| Animación | Rig propio con ≤ 30 huesos y animación a mano | Mixamo para prototipar; AccuRIG |
| Controles y cámara | Input System (joystick en pantalla) + Cinemachine 3 Target Group | — |
| "Jugo" | DOTween + Feel | — |
| Red, fase 1 | **NGO + Unity Transport**, IP directa con QR | Mirror o FishNet |
| Red, fase 2 | NGO + **Unity Relay/Lobby** | **Photon Fusion 2** (100 CCU gratis) o EOS |
| Progresión | Unity Authentication + Cloud Save (o Firebase) | — |
| Contenido descargable | Addressables + CDN | — |
| Compras | **Unity IAP** para los trajes | RevenueCat si hay suscripción ("Club INKCAP") |
| Anuncios | Video recompensado con **LevelPlay o MAX** (AdMob como red dentro de la mediación) | Solo AdMob al inicio |
| Analítica y errores | Unity Analytics o Firebase + Crashlytics | — |
| Builds de iOS | Codemagic o Unity Build Automation, luego una Mac mini propia | GitHub Actions + GameCI |

---

## 11. Plan técnico por etapas

**Etapa 0, semanas 1–2: cimientos**
- [ ] Decidir qué empresa será dueña del juego y confirmar si sus ingresos permiten Unity Personal.
- [ ] Tramitar el **D-U-N-S** y abrir las cuentas de **Apple (organización)** y **Google Play (organización)**.
- [ ] Crear el proyecto Unity 6.3 LTS con plantilla URP móvil: IL2CPP, ARM64, API objetivo 36, 16 KB.
- [ ] Repositorio Git con LFS, CLAUDE.md y el plugin de Unity para Claude Code instalado.
- [ ] Comprar los dispositivos de prueba y hacer el primer build "Hola mundo" en un Galaxy A07 y en un iPhone (este vía Codemagic).

**Etapa 1, semanas 3–10: prototipo jugable**
- [ ] Movimiento con joystick, lanzamiento del objeto que regresa (curva de ida y vuelta), atrapar, dash, un golpe = KO y rondas.
- [ ] Arena en *greybox* con ProBuilder y 1 muñequito placeholder (Mixamo).
- [ ] Bots simples para probar solo.
- [ ] Red local NGO: el anfitrión muestra un QR, 2–4 clientes se conectan y se prueba en router propio y en hotspot.
- [ ] Medir fps y memoria en el teléfono más barato desde el primer día.
- [ ] **Criterio de salida:** 4 personas juegan 20 minutos seguidos y quieren "otra ronda".

**Etapa 2, semanas 11–18: vertical slice**
- [ ] Estilo final: shader toon/matcap, 2 muñequitos, arena "Calle grafiteada" e interfaz base.
- [ ] Sistema de **trajes intercambiables** con el pipeline PNG → ScriptableObject → Addressables de la sección 5.2.
- [ ] Jugo: *hit-stop*, temblor de cámara, partículas de "tinta" y vibración.

**Etapa 3, meses 5–9: MVP local Wi-Fi**
- [ ] 6–8 personajes, 3 arenas y 3–4 modos.
- [ ] XP, niveles, racha y recompensa diaria con notificaciones; Cloud Save.
- [ ] Inventario de trajes y una tienda con Unity IAP (o desbloqueo por XP en la primera versión).
- [ ] Analítica, Crashlytics, localización es-MX/en y política de privacidad.
- [ ] Prueba cerrada en Google Play (12+ testers si la cuenta fuera personal), TestFlight en iOS y publicación.

**Etapa 4, meses 9–10: lanzamiento suave en México**
- [ ] Medir retención D1/D7, sesiones por día y conversión a compra; ajustar la economía.

**Etapa 5, meses 10–16: en línea**
- [ ] Cambiar el transporte a Relay, agregar Lobby y códigos de sala, y *matchmaking* rápido.
- [ ] Validar la economía en servidor (que nadie se regale trajes) y aplicar anti-trampas básicas.
- [ ] Prueba técnica comparativa NGO vs **Photon Fusion 2** en latencia real desde México. Decidir según la sensación de juego.
- [ ] Video recompensado y pase de temporada.

**Etapa 6, continuo: live ops**
- [ ] Drops de trajes sincronizados con "Estrenos" y "Edición Limitada" de la tienda en línea, eventos con artistas y temporadas temáticas por técnica de tinta (grafiti, tatuaje, serigrafía, bordado, sticker, tinta derramada).

---

## 12. Dudas abiertas (por verificar)

1. **¿Qué empresa publica?** Si los ingresos más fondos de esa entidad superan USD 200k en 12 meses, se requiere Unity Pro para cada puesto. Hay que revisarlo con su contador.
2. **Precios oficiales vigentes de Unity Relay, Lobby, Cloud Save y Cloud Content Delivery.** La página oficial de precios estuvo bloqueada en esta sesión.
3. **Si las cuentas de organización de Google Play están exentas de la prueba de 12 testers.** Solo lo afirman blogs.
4. **Reglas de Apple y Google para desbloquear contenido digital con compras físicas** (por ejemplo, "compra la playera o el art toy y recibe el traje"). La guía 3.1.1 de Apple restringe desbloquear con códigos o QR. Requiere revisión legal antes de diseñar esa mecánica **[Conocimiento previo]**.
5. **Si Photon Fusion 2 funciona en LAN sin internet.** No lo confirmé. Si la fase 1 debe funcionar sin internet, NGO es la opción segura.
6. **Permiso de multicast en iOS** para la búsqueda automática de partidas en la red local. Hay que confirmar el trámite con Apple o diseñar la conexión con QR.
7. **Tarifas reales de freelancers.** Las de este reporte son estimaciones; conviene pedir 3 cotizaciones por rol.
8. **Precios de Feel y DOTween Pro** en la Asset Store, y si la licencia por puesto aplica a todo el equipo.
9. **Rendimiento de Unity 6.3 en GPU Mali de entrada.** Hay reportes de regresiones frente a 2022.3; medirlo en la semana 1.
10. **Contratos con artistas:** confirmar que el 15 % de regalías y los derechos de uso cubren también las **versiones digitales** de sus diseños en el juego.

---

## 13. Fuentes

**Unity: licencias, versiones e IA**
- https://unity.com/products/pricing-updates
- https://unity.com/blog/unity-is-canceling-the-runtime-fee (12-sep-2024)
- https://support.unity.com/hc/en-us/articles/30322080156692
- https://80.lv/articles/unity-announces-its-upcoming-2026-price-changes
- https://www.gamesradar.com/games/a-year-later-unity-cancels-its-controversial-runtime-fee-effective-immediately
- https://unity.com/blog/unity-6-3-lts-is-now-available (dic-2025)
- https://unity.com/releases/unity-6/support
- https://discussions.unity.com/t/unity-6-5-is-now-available/1723176 (jun-2026)
- https://discussions.unity.com/t/unity-6-6-beta-is-now-available/1724014
- https://unity.com/blog/unity-industry-product-releases-2026
- https://brewmonster.vn/en/blog/unity-6-whats-new/
- https://discussions.unity.com/t/unity-ai-s-open-beta-now-live-for-unity-6/1718560
- https://vindler.solutions/blog/unity-ai-open-beta
- https://www.aigamer.gg/guides/unity-ai-mcp-claude-code
- https://discussions.unity.com/t/unity-s-plugin-is-now-available-in-claude-code/1736143 (sep-2026)
- https://www.pocketgamer.biz/unity-launches-official-claude-code-plugin-with-29-built-in-engine-skills/
- https://aicoder.com/news/news-20260913-unity-claude-code-plugin
- https://dev.to/indiebots/unitys-mcp-server-is-deprecated-what-to-use-instead-2026-3khl
- https://docs.unity3d.com/Packages/com.unity.ai.assistant@2.7/manual/integration/unity-mcp-get-started.html
- https://voyager.so/blog/unity-mcp

**Unity: rendimiento y Android**
- https://docs.unity3d.com/Manual/urp/configure-for-better-performance.html
- https://discussions.unity.com/t/unity-6-mobile-graphics-optimization-tips-and-tricks/1593293
- https://unity.com/blog/mobile-recap-new-tools-for-performance-in-unity-6-1
- https://discussions.unity.com/t/unity-6-android-urp-baseline-frametime-50ms-20-fps/1566399
- https://discussions.unity.com/t/android-apk-minimum-file-size/633101
- https://discussions.unity.com/t/info-unity-engine-support-for-16-kb-memory-page-sizes-android-15/1589588
- https://developer.android.com/games/develop/build-in-unity

**Multijugador**
- https://docs.unity3d.com/Manual/multiplayer-new.html
- https://unity.com/blog/games/how-to-choose-the-right-netcode-for-your-game
- https://docs.unity.com/en-us/relay/ngo-vs-mirror-for-relay
- https://discussions.unity.com/t/creating-an-offline-lan-multiplayer-game-using-netcode-for-gameobjects-relay/1524568
- https://github.com/Optifun/multiplayer-community-contributions/blob/main/com.community.netcode.extensions/Runtime/NetworkDiscovery
- https://unity.com/products/distributed-authority
- https://activation.unity.cn/products/relay
- https://forum.unity.com/threads/relay-lag-issues-and-pricing.1530151/
- https://tech-insider.org/unity-multiplay-relay-lobby-setup-2026/
- https://discussions.unity.com/t/so-what-is-the-difference-between-netcode-for-gameobjects-mirror-and-fishnet-which-one-is-better-for-fast-co-op/1678432
- https://oceanviewgames.co.uk/blog/posts/cross-platform-multiplayer-networking-unity
- https://blog.photonengine.com/new-free-100-ccu-for-photon-fusion-and-quantum-games/
- https://blog.photonengine.com/new-200-ccu-plus-package-100-paid-100-free/
- https://crux.supercraft.host/blog/photon-fusion-pricing-2026/
- https://doc.photonengine.com/fusion/v2/manual/network-topologies
- https://www.issoh.co.jp/md/tech/6888
- https://forum.photonengine.com/discussion/15150/photon-self-hosted-server-without-internet-lan

**Monetización (SDK)**
- https://www.undrads.com/blogs/ironsource-vs-applovin-max-vs-admob
- https://appagent.com/blog/best-ad-networks-for-mobile-games/
- https://revenuecat.com/pricing
- https://www.usagepricing.com/blueprint/revenuecat
- https://www.revenuecat.com/blog/company/navigating-revenuecats-new-pricing-for-existing-users/

**Godot**
- https://godotengine.org/releases/4.6/
- https://godotengine.org/article/godot-mobile-update-apr-2026/
- https://godotengine.org/article/gabe-stable-release/
- https://ziva.sh/blogs/godot-mobile
- https://en.wikipedia.org/wiki/Godot_(game_engine)
- https://www.strayspark.studio/blog/godot-46-export-web-android-ios-guide
- https://docs.godotengine.org/en/stable/tutorials/platform/android/android_in_app_purchases.html
- https://godotengine.org/asset-library/asset/2063
- https://store.godotengine.org/asset/poingstudios/admob/
- https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html
- https://forum.godotengine.org/t/game-size-too-big-compared-to-apk/39416
- https://forum.godotengine.org/t/absurd-file-size-increase-when-my-project-is-exported-to-apk/130000
- https://docs.godotengine.org/en/4.2/tutorials/export/exporting_for_android.html
- https://mcpservers.org/servers/ivanmurzak/godot-mcp
- https://godotengine.org/asset-library/asset/5040
- https://forum.godotengine.org/t/beckett-zero-sidecar-mcp-server-for-godot-4-2-the-ai-sees-and-optionally-playtests-your-game/141177

**Unreal Engine**
- https://www.unrealengine.com/en-US/license
- https://pcgamer.com/unreal-engine-games-no-longer-owe-royalties-on-their-first-dollar1m-in-revenue
- https://www.unrealengine.com/blog/catch-up-on-the-big-news-from-unreal-fest-seattle-2024
- https://www.pocketgamer.biz/unreal-engine-royalty-fee-reducing-to-35-for-games-landing-on-epic-games-store-on-launch-day
- https://www.unrealengine.com/blog/we-are-updating-unreal-engine-twinmotion-and-realitycapture-pricing-in-late-april
- https://dev.epicgames.com/documentation/unreal-engine/reducing-android-binary-size-in-unreal-engine-projects
- https://forums.unrealengine.com/t/what-is-the-minimum-size-one-can-achieve-for-android-apk/296898
- https://answers.unrealengine.com/t/why-is-a-compiled-mobile-project-so-huge/425988
- https://pypi.org/project/unrealmcp/
- https://forums.unrealengine.com/t/carlos-li-claude-unreal/2720506

**Publicación**
- https://developer.apple.com/support/enrollment/ (consultada directamente)
- https://developer.apple.com/news/upcoming-requirements/
- https://9to5mac.com/2026/02/03/apple-to-update-minimum-sdk-requirements-for-all-app-store-submissions/
- https://support.google.com/googleplay/android-developer/answer/14151465
- https://ontest.app/blog/google-play-12-testers-14-days-requirement-explained
- https://dev.to/samfadigitalstack/google-play-closed-testing-a-practical-guide-to-the-12-testers-and-14-days-requirement-1p03
- https://support.google.com/googleplay/android-developer/answer/13628312
- https://support.google.com/googleplay/android-developer/answer/13634885
- https://developer.android.com/developer-verification/guides/faq
- https://www.biometricupdate.com/202508/google-unveils-identity-verification-rules-for-android-app-developers
- https://developer.android.com/google/play/requirements/target-sdk

**Builds, CI y hardware**
- https://docs.codemagic.io/billing/pricing/
- https://codemagic.io/pricing
- https://discussions.unity.com/t/is-mac-build-100-free-minutes-for-unity-build-after-q1-of-2026-every-month-or-only-once/1696004
- https://discussions.unity.com/t/unity-devops-updates-in-2026/1711090
- https://lizard.build/blog/github-actions-pricing
- https://macswarm.com/blog/github-actions-macos-runner-cost-2026/
- https://www.github.com/game-ci/unity-builder
- https://discussions.unity.com/t/unity-no-longer-supports-manual-activation-of-personal-licenses/926760
- https://www.xataka.com.mx/ordenadores/apple-acaba-eliminar-uno-sus-productos-atractivos-mexico-mac-mini-15-mil-pesos
- https://www.xataka.com.mx/ordenadores/nueva-apple-mac-mini-m6-m5-pro-lanzamiento-precio-mexico-caracteristicas-especificaciones-ficha-tecnica
- https://everymac.com/global-mac-prices/all-msrp-mac-mini-prices-in-mexico.html

**Mercado mexicano**
- https://gs.statcounter.com/os-market-share/mobile/mexico
- https://www.luznoticias.mx/2026-03-17/mexico/celulares-android-que-mas-se-venden-en-mexico-este-2026-/282031
- https://www.merca20.com/cuales-son-los-celulares-android-mas-populares-en-mexico-en-2026-y-por-que-samsung-domina-el-mercado/
- https://news.samsung.com/mx/samsung-lidera-el-mercado-de-smartphones-en-mexico-de-cara-a-el-buen-fin

**Tipo de cambio**
- https://dolardof.com/
