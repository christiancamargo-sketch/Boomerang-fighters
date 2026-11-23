# 02 · Panorama competitivo y casos de éxito (y fracaso) de juegos party / brawler / arena multijugador

> **Proyecto:** "Boomerang Fighters" (nombre de trabajo) — party brawler móvil con muñequitos estilo art toy, objeto arrojadizo que regresa y escenarios de arte urbano, con cosméticos de merch real de INKCAP.
> **Fecha de la investigación:** 23 de noviembre de 2026.
> **Tipo de cambio usado:** **1 USD ≈ 19.1 MXN** (FIX de Banxico del 20-nov-2026: 19.0228; mercado del 22-nov-2026: ~19.16). Fuente: [dolar.info – FIX Banxico](https://dolar.info/mexico/hoy/precio/banxico-fix.php), [Expreso, 22-nov-2026](https://www.expreso.com.mx/noticias/negocios/precio-del-dolar-hoy-22-de-noviembre-de-2026-en-mexico/257000).

---

## 0. Cómo leer este documento (nota metodológica)

- **Casi ninguna empresa publica sus ventas reales.** La mayoría de las cifras de descargas e ingresos vienen de **estimadores externos** (AppMagic, Sensor Tower, Appfigures, SteamSpy, Raijin). Son buenas para órdenes de magnitud, **no** para contabilidad. Cuando dos fuentes no coinciden, lo digo.
- "Ingreso bruto" (lo que paga el jugador) ≠ "ingreso neto" (lo que le queda al estudio después de la comisión de Apple/Google, normalmente 15–30%).
- Las cifras de Steam (PC) incluyen copias regaladas o gratuitas en el caso de SteamSpy.
- Algunas páginas oficiales (p. ej. The CIU y mobilegamer.biz) no se pudieron abrir directamente desde este entorno; en esos casos uso los resúmenes de búsqueda y cito la URL original para que se pueda verificar.
- Todo lo que **no pude confirmar** está marcado como *(no confirmado)*.

---

## 1. Resumen ejecutivo (lo más importante en 10 líneas)

1. **El género "party brawler" sí puede generar cientos de millones de dólares en móvil**, pero los grandes ganadores (Brawl Stars, Stumble Guys, Among Us) tienen tres cosas en común: **partidas de 2–3 minutos, se juega con amigos y se monetiza casi solo con cosméticos/pase de temporada**.
2. **Un equipo pequeño puede ganar:** Stumble Guys lo hacían **8–9 personas** en Finlandia cuando Scopely lo compró, con **más de 225 millones de descargas** ([PocketGamer.biz](https://www.pocketgamer.biz/scopely-acquires-stumble-guys-from-kitka-games)). Among Us: **3 desarrolladores** ([Kotaku](https://kotaku.com/among-us-improbable-rise-to-the-top-of-twitch-1844986025)). BombSquad y Pico Park: **una sola persona**.
3. **Los gigantes también fracasan:** Supercell cierra **Squad Busters** el 14-dic-2026 ([PocketGamer.biz](https://www.pocketgamer.biz/supercell-will-shut-down-squad-busters-next-year/)); Sega declaró que **Sonic Rumble** quedó por debajo de expectativas y recortó su división free-to-play ([GamesRadar](https://www.gamesradar.com/games/sonic-the-hedgehog/sega-downsizes-free-to-play-division-after-weak-performance-from-sonic-rumble-party-moving-over-100-devs-onto-full-game-releases-in-mainstay-ips/)). Tener marca o dinero **no** sustituye un núcleo de juego divertido y diferente.
4. **Los fracasos (Knockout City, Rumbleverse, Squad Busters, Smash Legends) caen por retención**: la gente lo prueba, pero no regresa. La causa casi siempre es un núcleo de juego "correcto pero sin nada único" y/o una economía mal diseñada.
5. **México es Android:** ~**74% Android vs ~26% iOS** en celulares ([StatCounter, oct-2026](https://gs.statcounter.com/os-market-share/mobile/mexico)). Hay que optimizar para Android de gama media.
6. **México tiene ~73.8 millones de jugadores, 85.9% en smartphone**, pero **las consolas se llevan ~79% del dinero** según The CIU. El jugador móvil promedio gasta poco: hay que diseñar para *muchísimos jugadores que no pagan* + una minoría que compra cosméticos.
7. **El efectivo importa:** en México se puede pagar Google Play **en efectivo en OXXO y 7-Eleven** desde la propia app ([Ayuda de Google Play](https://support.google.com/googleplay/answer/9462077?hl=es-419)). Esto es una ventaja directa para una marca que ya cobra con OXXO.
8. **El descubrimiento es social:** amigos/familia son el canal #1 y **24% de los jugadores en México descargó su último juego después de ver a un influencer jugarlo** ([Google Play / Medium](https://medium.com/googleplaydev/engaging-players-and-unlocking-spend-in-spanish-speaking-latin-america-1cfef4ac08b)). TikTok tiene ~85 millones de cuentas en México.
9. **Las colaboraciones de marca mueven dinero:** los picos de ingresos semanales de Free Fire en México coinciden con colaboraciones (p. ej. ~US$2 M/semana con la colaboración Jujutsu Kaisen en dic-2025, según [Sensor Tower](https://sensortower.com/blog/2025-q4-android-top-5-third-person-games-units-mx-60214330241bc16eb89ce15d)). Los cosméticos de merch real de INKCAP encajan en este patrón.
10. **La estrategia recomendada:** prototipo jugable en 3–4 meses → pruebas con la comunidad INKCAP en CDMX → beta cerrada → *soft launch* en países hispanos baratos → lanzamiento LATAM con programa de creadores (códigos de creador) y "drops" físicos+digitales.

---

## 2. Mapa del género: ¿contra quién competimos realmente?

"Party game de arena" no es una sola categoría. Hay cinco subgéneros y cada uno enseña algo distinto:

| Subgénero | Ejemplos | Qué enseña para nuestro juego |
|---|---|---|
| **Party brawler de arena (2–8 jugadores, un toque mata)** | Boomerang Fu, BombSquad, Bomber Friends, Stick Fight, Duck Game, Gang Beasts | Es **exactamente** nuestro núcleo. Casi todos nacieron en PC/consola o como juegos locales; muy pocos han conquistado el móvil online → **hay un hueco**. |
| **Party royale masivo (32–60 jugadores, eliminatorias)** | Fall Guys, Stumble Guys, Eggy Party, Sonic Rumble | Muestra el tamaño del mercado casual y la importancia de cosméticos, temporadas y creadores. |
| **Deducción social** | Among Us, Super Sus | El poder del streaming y de "jugar con tus amigos". Among Us empezó **solo local**, como nosotros. |
| **Brawler competitivo / MOBA ligero** | Brawl Stars, Smash Legends, Brawlhalla, Zooba, Squad Busters | El techo comercial del género (Brawl Stars > US$2,000 M). También el cementerio de imitadores. |
| **Hybrid-casual / plataformas sociales** | Archero, Bowmasters, Hago | Monetización mixta (anuncios + compras) y sesiones ultra cortas. |

---

## 3. Tabla maestra de casos

> Cifras aproximadas; ver detalle y fuentes en cada sección. MXN al tipo de cambio 19.1.

| Juego | Estudio / tamaño | Año | Plataforma principal | Escala (descargas / ventas) | Ingresos (aprox.) | Modelo | Resultado |
|---|---|---|---|---|---|---|---|
| **Brawl Stars** | Supercell; equipo de ~20 (2020) → ~64 (no fechado) | 2018 global | Móvil | ~545 M descargas (agregadores) | > US$2,000 M de por vida (~38,200 M MXN) | F2P, pase, cosméticos | Éxito masivo |
| **Squad Busters** | Supercell | 2024 | Móvil | 40 M descargas en 30 días | > US$100 M de por vida | F2P | **Fracaso** (cierra 14-dic-2026) |
| **mo.co** | Supercell | 2025 | Móvil | ~2.5 M primera semana; ~9 M después | ~US$5–7 M | F2P, lanzamiento por invitación | Bajo rendimiento, relanzado |
| **Stumble Guys** | Kitka Games: **8–9 personas** al venderlo | 2020 | Móvil | > 270 M (ene-2023) | > US$80 M (ene-2023) | F2P cosméticos | Éxito → vendido a Scopely |
| **Fall Guys** | Mediatonic → Epic | 2020 | PC/consola, luego móvil (Epic Store) | 50 M jugadores en 2 semanas tras F2P | No público | Pago → F2P | Éxito, luego declive y despidos |
| **Among Us** | Innersloth: **3 devs** | 2018 | Móvil + PC | ~909 M descargas móviles | ~US$60 M móvil de por vida | Gratis + cosméticos + PC US$4.99 | Fenómeno 2020 |
| **Eggy Party** | NetEase | 2022 | Móvil (China) | 500 M jugadores en < 2 años | > US$750 M (98% China) | F2P + UGC | Éxito (en China) |
| **Sonic Rumble** | Sega (+Rovio) | 2025 | Móvil | 15 M descargas | US$228 K en 10 días | F2P | **Fracaso** comercial |
| **Archero** | Habby (equipo "lean") | 2019 | Móvil | ~96 M | ~US$263 M neto / 376 M bruto | Hybrid-casual | Éxito |
| **Bowmasters** | Playgendary | 2016 | Móvil | (sin dato confiable) | > US$4 M solo en Q3-2018 | Anuncios + IAP | Éxito sostenido |
| **Bomber Friends** | Hyperkani (Finlandia, pequeño) | ~2015 | Móvil | 100 M+ (Google Play) | Estimaciones contradictorias | F2P | Éxito longevo |
| **BombSquad** | Eric Froemling (**1 persona**) | 2011/2014 | PC/Mac/Android | ~74 M instalaciones Android | No público | Gratis + moneda/pase | Éxito longevo |
| **Pico Park** | TECOPARK (**1 persona**) | 2016/2019 | PC / Switch | > 1 M copias (2021) | No público | Pago | Éxito |
| **Stick Fight** | Landfall (pequeño, ~2 meses de desarrollo) | 2017 | PC | 2–5 M dueños (SteamSpy) | No público | Pago | Éxito |
| **Duck Game** | Landon Podbielski (**1 persona**) | 2015 | PC | 2–5 M dueños (SteamSpy) | No público | Pago | Éxito |
| **Gang Beasts** | Boneloaf (3 hermanos) | 2014 EA / 2017 | PC/consola | ~1.3 M en Steam (est.) | ~US$15 M bruto (est.) | Pago | Éxito |
| **Party Animals** | Recreate Games (China) | 2023 | PC/Xbox (móvil anunciado 2026) | ~2.6 M en Steam (est.) | ~US$31 M bruto (est.) | Pago | Gran lanzamiento, fuerte caída |
| **Ultimate Chicken Horse** | Clever Endeavour (pequeño, Montreal) | 2016 | PC/consola | 0.8–1.7 M (est.) | US$12–25 M bruto (est.) | Pago | Éxito |
| **Brawlhalla** | Blue Mammoth → Ubisoft | 2017 | PC/consola/móvil | 100 M jugadores de por vida | No público | F2P cosméticos | Éxito |
| **Smash Legends** | 5minlab / LINE Games | 2021 | Móvil + PC | (sin dato) | (sin dato) | F2P | Modo "mantenimiento" (sin contenido nuevo) |
| **Zooba** | Wildlife Studios (Brasil) | 2019 | Móvil | (sin dato oficial) | (sin dato) | F2P | Estable |
| **Knockout City** | Velan Studios / EA | 2021 | PC/consola | 12 M lo probaron | — | Pago → F2P | **Cerrado** jun-2023 |
| **Rumbleverse** | Iron Galaxy / Epic | 2022 | PC/consola | — | — | F2P | **Cerrado** a los 6 meses |
| **Boomerang Fu** (referencia) | Cranky Watermelon (pequeño) | 2020 | PC/consola | 500 K unidades en ~2 años | No público | Pago | Éxito indie; **sin versión móvil** encontrada |

---

## 4. Casos de estudios gigantes

### 4.1 Brawl Stars (Supercell) — el techo del género

- **Cómo empezó:** soft launch en **Canadá (solo iOS) el 15-jun-2017**; estuvo **~18 meses** en prueba y fue rediseñado tres veces, incluido el cambio de **vertical a horizontal**. El community manager lo resumió así: *"no fue vertical u horizontal; fue horizontal o matar el juego"*. El director general admitió que el juego estuvo "en la orilla" entre lanzarse o cancelarse ([PocketGamer.biz](https://www.pocketgamer.biz/brawl-stars-was-very-much-on-the-edge/), [Deconstructor of Fun 2018](https://www.deconstructoroffun.com/blog/2018/5/11/bs), [TCRF](https://tcrf.net/Brawl_Stars/Version_Differences/Beta_Softlaunch)). Lanzamiento global: diciembre 2018.
- **Tamaño de equipo:** "poco más de 20 personas" en verano 2020; 26 hacia 2022; "menos de 40, de 18 nacionalidades" después; un reporte posterior (sin fecha confirmada) habla de 64 ([mobilegamer.biz](https://mobilegamer.biz/why-supercell-doubled-the-brawl-stars-team-and-continues-to-evolve-its-culture/), [PocketGamer.biz](https://www.pocketgamer.biz/frank-keienburg-brawl-stars-game-lead-on-supercellian-culture-and-what-other-platforms-can-learn-from-mobile)). **Lección:** el juego que factura cientos de millones al año lo hace un equipo del tamaño de un salón de clases.
- **Escala:** más de **US$2,000 M** de ingresos de por vida ([PocketGamer.biz](https://www.pocketgamer.biz/brawl-stars-was-very-much-on-the-edge/)); agregadores citan ~US$2,300 M y ~545 M descargas ([Udonis](https://www.blog.udonis.co/mobile-marketing/mobile-games/brawl-stars-player-count)).
- **El "renacimiento" de 2024:** en marzo 2024 facturó **US$67 M** y en abril **US$84.3 M**; para fines de abril ya había superado todo 2023 ([PocketGamer.biz](https://www.pocketgamer.biz/april-2024-mobile-game-charts-call-of-duty-warzone-and-brawl-stars/), [PocketGamer.biz](https://www.pocketgamer.biz/brawl-stars-has-already-earned-more-in-2024-than-the-entirety-of-2023/)). Los ingresos se multiplicaron ~9x y los usuarios mensuales ~2.4x frente a 2023 ([GamingOnPhone](https://gamingonphone.com/news/brawl-stars-revenue-jumped-to-almost-9x-and-mau-to-2-4x-in-2024-compared-to-last-year/)). Las razones citadas: **mejor pase de batalla, economía mejor administrada, eliminación de cajas de botín y eventos centrados en la comunidad** ([Deconstructor of Fun](https://www.deconstructoroffun.com/blog/2024/2/11/brawl-stars-to-the-moon)).
- **2025–2026:** 2025 bajó (Statista estimó ~US$205 M en el año hasta la fecha) y en marzo 2026 volvió a subir a US$48.6 M en el mes (+46.8%) ([PocketGamer.biz](https://www.pocketgamer.biz/march-2026-mobile-game-charts-brawl-stars-revenue-rises-nearly-50/)). **Lección:** incluso el mejor juego vive de ciclos de contenido.
- **En México:** Sensor Tower registra picos de ~US$294 K **por semana** en México (última semana de dic-2025) ([Sensor Tower](https://sensortower.com/blog/2025-q4-unified-top-5-gacha-games-revenue-mx-62052cd7afb48b4f71f52675)).
- **Creadores:** Supercell tiene "códigos de creador": un porcentaje del valor de las gemas que gasta el jugador va al creador elegido, sin costo extra para el jugador ([Supercell – Creator Boost FAQ](https://supercell.com/en/games/brawlstars/blog/community/creator-boost-faq/)). El porcentaje (~5%) que se repite en internet **no está confirmado oficialmente**.

### 4.2 Squad Busters (Supercell) — cuando el gigante falla

- **Lanzamiento:** global el 29-may-2024, **sin soft launch tradicional**, con 40 M de pre-registros. Primeros 30 días: **US$24 M netos (US$34 M brutos) y 40 M de descargas** ([PocketGamer.biz](https://www.pocketgamer.biz/supercells-squad-busters-hits-24m-net-revenue-in-first-30-days)).
- **La caída:** el ingreso diario bajó de más de US$1.2 M a menos de US$600 K y las descargas diarias de 3.2 M a menos de 200 K **en el primer mes** ([Gamelight](https://www.gamelight.io/news/squad-busters-by-supercell-earns-$24m-in-first-30-days,-sees-revenue-decline-post-launch), [Mobidictum](https://mobidictum.com/squad-busters-why-a-50-revenue-drop/)).
- **Por qué falló:** Deconstructor of Fun lo resume en un **"núcleo de juego débil encima de una economía pobre"** ([DoF](https://www.deconstructoroffun.com/blog/2024/9/30/4-x-reasons-why-squad-busters-suffers-while-brawl-stars-soars)). Tenía casi el doble de ingreso por usuario diario que Brawl Stars, pero 59% menos usuarios diarios: **monetizaba bien a pocos y no retenía a nadie** ([Liquid & Grit](https://www.liquidandgrit.com/?p=14281)). PocketGamer.biz reportó **presión interna por lanzar un juego nuevo** ([PocketGamer.biz](https://pocketgamer.biz/pressure-to-launch-a-new-game-led-to-squad-busters-global-release)). Un análisis lo describió como un juego que "no hacía nada mal, pero nada único" ([NextBigGames](https://nextbiggames.com/2026/02/16/squad-busters-failure-lessons-supercell/)).
- **Final:** desarrollo terminado en oct-2025; sale de tiendas el 1-dic-2026 y cierra servidores el **14-dic-2026**, con más de US$100 M de por vida ([PocketGamer.biz](https://www.pocketgamer.biz/supercell-will-shut-down-squad-busters-next-year/), [Respawn](https://respawn.outlookindia.com/gaming/gaming-news/supercell-confirms-squad-busters-shutdown-date)).

### 4.3 mo.co (Supercell, 2025) — el experimento de "lanzar con creadores"

- Lanzado el 18-mar-2025 **solo por invitación**: los códigos QR aparecían en los *streams* de creadores. ~2.5 M descargas la primera semana y US$2.5 M en menos de un mes; comparado con Clash Royale (US$162 M en 30 días) fue pequeño ([PocketGamer.biz](https://www.pocketgamer.biz/supercells-moco-hits-2-5m-downloads-and-570k-in-first-week), [PocketGamer.biz](https://www.pocketgamer.biz/supercells-moco-surpasses-25m-in-under-a-month)). Luego quitó la invitación, "regresó a beta" y se relanzó con temporadas mensuales ([mobilegamer.biz](https://mobilegamer.biz/supercells-monster-hunting-game-mo-co-is-going-back-into-beta/)).
- **Lección:** la escasez artificial genera ruido inicial, pero **no reemplaza la retención**. Sí es útil como táctica de *beta cerrada* con la comunidad propia (justo lo que INKCAP puede hacer con su podcast y tienda).

### 4.4 Fall Guys (Mediatonic → Epic) — de fenómeno a lección de live service

- Lanzado de pago en agosto 2020 (pico en Steam ~172 K jugadores simultáneos). Al volverse **gratis (21-jun-2022)** logró **20 M de jugadores en 48 horas y 50 M en ~2 semanas** ([Game Developer](https://gamedeveloper.com/business/fall-guys-hit-50-million-players-two-weeks-after-freemium-switch), [Icon Era](https://icon-era.com/statistics/fall-guys-player-count/)).
- Epic despidió ~830 personas en sep-2023 y Mediatonic fue fuertemente afectado ([Game Developer](https://www.gamedeveloper.com/business/report-i-fall-guys-i-developer-mediatonic-decimated-epic-games-layoffs)). Llegó a móvil vía **Epic Games Store el 16-ago-2024** (Android global, iOS solo en la UE).
- Dato técnico relevante: Fall Guys está hecho en **Unity** ([DEV Community](https://dev.to/wlz/why-fall-guys-bet-on-unity-the-boring-phone-engine-behind-a-32-player-bean-battle-royale-28e9)).
- **Lección:** Fall Guys **no llegó a tiempo a móvil** y un estudio finlandés de 8 personas (Stumble Guys) se quedó con ese mercado.

### 4.5 Eggy Party (NetEase) — party game + creación de niveles

- Más de **US$750 M** de gasto de jugadores en móvil (ene-2026), **98% en China**; 2023: US$395.7 M → 2024: US$183 M → 2025: US$158.7 M ([PocketGamer.biz](https://www.pocketgamer.biz/eggy-party-cracks-750m-in-mobile-player-spending)). 500 M de jugadores en menos de dos años y más de 100 M de mapas creados por usuarios ([PocketGamer.biz](https://www.pocketgamer.biz/neteases-eggy-party-reaches-500-million-players-in-under-two-years), [PocketGamer.biz](https://www.pocketgamer.biz/eggy-party-sails-past-100-million-user-generated-maps)).
- **Lección:** el contenido creado por usuarios (UGC) alarga la vida del juego. Para nosotros, una versión ligera sería **"diseña tu playera/sticker"** dentro del juego, ligada a concursos reales de INKCAP.

### 4.6 Sonic Rumble (Sega, nov-2025) — la marca no basta

- Lanzado el 5-nov-2025: **15 M de descargas** pero solo **US$228 K en 10 días** y US$772 K en dos meses ([PocketGamer.biz](https://www.pocketgamer.biz/sonic-rumble-earns-just-228000-in-first-10-days-with-a-future-major-update-in-the-works), [PocketGamer.biz](https://www.pocketgamer.biz/sonic-rumble-jogs-to-772000-in-two-months)). Sega dijo que quedó por debajo en todos los indicadores, **especialmente adquisición de usuarios** ([VGC](https://www.videogameschronicle.com/news/sega-says-its-new-releases-especially-sonic-rumble-have-performed-below-expectations)); se renombró *Sonic Rumble Party* y Sega reasignó a más de 100 desarrolladores ([GamesRadar](https://www.gamesradar.com/games/sonic-the-hedgehog/sega-downsizes-free-to-play-division-after-weak-performance-from-sonic-rumble-party-moving-over-100-devs-onto-full-game-releases-in-mainstay-ips/)).
- **Lección:** el "party royale" ya está saturado. **Copiar el formato de Fall Guys/Stumble Guys en 2026 es mala idea**; el brawler de arena con objeto que regresa es un nicho menos peleado.

---

## 5. Casos de equipos pequeños que se volvieron enormes

### 5.1 Stumble Guys (Kitka Games → Scopely) — EL caso a estudiar

- **Origen:** en octubre 2020 un equipo diminuto en **Kajaani, Finlandia** decidió hacer "Fall Guys para celular"; unas tres semanas después ya estaba en mercados de prueba ([mobilegamer.biz](https://mobilegamer.biz/how-stumble-guys-beat-fall-guys-at-its-own-game/); otra base de datos fecha el primer lanzamiento el 24-sep-2020 — [PlayTracker](https://playtracker.net/quest/game/130973)). iOS: 12-feb-2021.
- **Crecimiento:** a agosto 2022, **225 M descargas y US$40 M** ([mobilegamer.biz](https://mobilegamer.biz/surprise-smash-stumble-guys-hits-40m-revenue-and-225m-downloads/)); luego >265 M y >US$60 M, y en enero 2023 >US$80 M con 270 M descargas ([mobilegamer.biz](https://mobilegamer.biz/fall-guys-clone-stumble-guys-has-now-earned-over-60m-from-265m-downloads/)). En su pico llegó a facturar **casi medio millón de dólares al día** ([PocketGamer.biz](https://www.pocketgamer.biz/battle-royale-game-stumble-guys-is-making-almost-half-a-million-dollars-every-day/)). Fue el juego de **mayor crecimiento en descargas del Q4-2022** según Sensor Tower ([Game World Observer](https://gameworldobserver.com/2023/01/26/sensor-tower-subway-surfers-was-most-downloaded-game-in-q4-2022-but-the-stumble-guys-showed-fastest-growth)).
- **Venta:** Scopely lo compró en sep-2022; Kitka tenía **8 empleados** (GamesBeat dice 9) ([PocketGamer.biz](https://www.pocketgamer.biz/scopely-acquires-stumble-guys-from-kitka-games), [GamesBeat](https://gamesbeat.com/scopely-acquires-stumble-guys-battle-royale-game-from-kitka-games/)).
- **LATAM fue clave:** en 2022 **Latinoamérica fue la región con más descargas (52.5 M)**, Brasil ~25 M. En ingresos, **Brasil fue el #2 mundial (US$7.4 M) después de EE.UU. (US$17.8 M)**; Indonesia fue #1 en descargas (59 M) ([PocketGamer.biz](https://www.pocketgamer.biz/battle-royale-game-stumble-guys-is-making-almost-half-a-million-dollars-every-day/), [Sensor Tower vía Naavik](https://naavik.co/digest/stumbleguys-garenafreefire/)).
- **Marketing:** en vez de subir el gasto en anuncios, Kitka apostó por **influencers (pagados y orgánicos) en YouTube y TikTok**, y les daba **moneda del juego (gemas) para regalar** a sus audiencias, lo que también hacía crecer los canales de los creadores ([GameAnalytics](https://gameanalytics.com/?p=17893), [mobilegamer.biz](https://mobilegamer.biz/how-stumble-guys-beat-fall-guys-at-its-own-game/)).
- **Hoy:** las estimaciones independientes indican ingresos menores que en 2022 y datos de usuarios contradictorios; Scopely no publica cifras auditadas ([Udonis](https://www.blog.udonis.co/mobile-marketing/mobile-games/stumble-guys)).
- **Lecciones directas:** (1) **velocidad**: lanzaron un producto jugable en semanas; (2) **móvil primero**, donde el original no estaba; (3) **ligero para Android de gama baja**, lo que le abrió Indonesia, India y LATAM; (4) creadores + regalos de moneda; (5) un "clon" puede ganar si llega primero a una plataforma desatendida, **pero** hoy eso ya no aplica al party royale.

### 5.2 Among Us (Innersloth) — empezó local, como nosotros

- **3 desarrolladores** principales en Redmond, EE.UU. ([Kotaku](https://kotaku.com/among-us-improbable-rise-to-the-top-of-twitch-1844986025)). Salió en **junio 2018** como juego **móvil, solo multijugador local y con un solo mapa**; un mes después tenían multijugador en línea funcionando ([Global Games Forum](https://globalgamesforum.com/features/the-long-and-winding-road-to-success-for-among-us)). Steam: noviembre 2018 a **US$4.99** ([SteamDB](https://steamdb.info/app/945360)).
- Pasó **dos años casi desconocido**. En julio 2020 el streamer Sodapoppin empezó a jugarlo (también ayudaron streamers coreanos): de un par de cientos de espectadores promedio a inicios de julio, a **más de 100,000 a fines de agosto** ([Kotaku](https://kotaku.com/among-us-improbable-rise-to-the-top-of-twitch-1844986025)). En Q4-2020 se vieron **221 M de horas** en Twitch ([Sportskeeda](https://opera.sportskeeda.com/among-us/among-us-221-millions-hours-watched-twitch-just-three-months)).
- **Escala:** ~908.6 M descargas móviles de por vida y ~US$60.4 M de ingresos móviles (AppMagic vía [Udonis](https://www.blog.udonis.co/mobile-marketing/mobile-games/among-us-player-count)); 1,000 M de descargas solo en Google Play ([GamesRadar](https://www.gamesradar.com/games/co-op/among-us-hits-1-billion-downloads-on-google-play-after-almost-8-years-1-8-of-the-world-are-crewmates-or-impostors-hard-to-say/)). Pico de ingresos: US$9.7 M en oct-2020 ([Appfigures](https://appfigures.com/resources/insights/20240119/5-the-game-that-could-have-been-the-next-angry-birds-but-wasnt)).
- **Monetización:** cosméticos, quitar anuncios en móvil y el precio en PC. **Lección:** generó un fenómeno cultural pero **ingresos modestos** comparados con su audiencia, porque casi no tenía qué vender ni temporadas. El equipo además terminó agotado ([PC Gamer](https://www.pcgamer.com/among-us-sudden-twitch-success-left-its-developer-feeling-burnt-out)). Hay que tener **listo el sistema de cosméticos y temporadas** antes de que llegue el éxito.

### 5.3 BombSquad (Eric Froemling, 1 persona) — el party brawler local más longevo

- Un solo desarrollador (ex-Pixar), sin departamento de marketing ([GitHub](https://fr.github.com/efroemling), [froemling.net](https://froemling.net/page/3)). Salió en Mac App Store ~2011 y en **Android en sep-2014**; ~**74 M de instalaciones en Android** y ~80 M sumando sus apps ([AppGoblin](https://appgoblin.info/apps/net.froemling.bombsquad), [AppBrain](https://www.appbrain.com/dev/Eric+Froemling/)).
- Su idea genial: la app gratuita **"BombSquad Remote" convierte celulares y tablets en controles** (~2.8 M instalaciones) ([AppGoblin](https://appgoblin.info/apps/net.froemling.bsremote), [APKMirror](https://www.apkmirror.com/?p=8843974)). Monetiza con una moneda ("tokens") y un "Gold Pass" como alternativa a microtransacciones ([ballistica.net](https://ballistica.net/whataretokens)).
- **Lección:** el multijugador **local vía Wi-Fi** es viable y duradero, y "tu celular es el control" es una gran puerta de entrada para fiestas y reuniones.

### 5.4 Bomber Friends (Hyperkani, Finlandia)

- Estudio finlandés pequeño (Hyperkani Oy). **100 M+ descargas** en Google Play (~112.9 M según AppGoblin); el estudio suma ~300 M en 6 apps ([AppGoblin](https://appgoblin.info/apps/com.hyperkani.bomberfriends), [AppBrain](https://www.appbrain.com/dev/Hyperkani/)). Las estimaciones de ingresos se contradicen (de <US$10 K a >US$100 K mensuales) *(no confirmado)*.
- **Lección:** un brawler de arena sencillo (estilo Bomberman) **con multijugador online y amigos** puede vivir una década en móvil.

### 5.5 Bowmasters (Playgendary)

- En Q3-2018 facturó **más de US$4 M**, ~25% de los US$16.5 M de Playgendary en ese trimestre; el publisher sumaba 318 M de descargas ([Sensor Tower](https://sensortower.com/blog/playgendary-revenue-q3-2018), [PocketGamer.biz](https://www.pocketgamer.biz/playgendary-revenue-went-up-57-per-cent-during-q3-2018)). Una supuesta compra por Miniclip **no se pudo confirmar**.
- **Lección:** física de "un tiro" + muchos personajes caricaturescos coleccionables + anuncios = éxito sostenido. Personajes "graciosos" son marketing por sí mismos.

### 5.6 Archero (Habby) — el padre del "hybrid-casual"

- Fundadores salidos de Cheetah Mobile; Habby describe a su equipo como "lean" sin dar números ([Deconstructor of Fun](https://www.deconstructoroffun.com/blog/2019/8/9/why-archero-banked-25m-but-leaves-25m-hanging-hlx9n), [Gamigion](https://www.gamigion.com/?p=20716)).
- ~US$263 M netos (US$376 M brutos) y ~96 M descargas de por vida; Archero 2 (ene-2025) hizo **US$32.8 M en sus primeros 30 días**; Habby supera US$2,000 M de por vida ([Game World Observer](https://gameworldobserver.com/2025/01/20/archero-2-revenue-8-million-in-11-days-vs-original-game), [PocketGamer.biz](https://www.pocketgamer.biz/archero-2-makes-328m-in-first-30-days-from-player-spending), [Deconstructor of Fun](https://www.deconstructoroffun.com/blog/2025/7/31/habbys-hybridcasual-empire-the-template-that-built-a-powerhouse)).
- **Lección:** controles de un dedo + progresión permanente (subir de nivel, desbloquear) multiplica el valor de cada jugador. Nuestro sistema de XP debe dar **sensación de progreso** sin dar ventaja injusta en combate.

### 5.7 Otros móviles relevantes

- **Super Sus** (Giant Network): "Among Us-like" que nació en móvil y salió en Steam gratis el 31-jul-2024 con ~2,900 reseñas (~70% positivas) ([Steam](https://store.steampowered.com/app/2920270/Super_Sus/), [Sensor Tower](https://app.sensortower.com/vgi/game/super-sus)). La cifra de "100 M+ descargas" que circula en wikis **no está confirmada**.
- **Zooba** (Wildlife Studios, Brasil): battle royale de animales caricaturescos, 2019; fue el 5º juego más descargado de EE.UU. días después de su lanzamiento; ~495 K calificaciones en iOS con 4.7 ([GamesBeat](https://gamesbeat.com/?p=182748), [App Store](https://apps.apple.com/app/id1459402952)). **Lección:** un estudio latinoamericano puede competir globalmente con personajes carismáticos.
- **Sausage Man** (XD): battle royale cómico de salchichas; los datos de descargas disponibles (~32 M en Android según AppGoblin) son estimaciones de baja calidad ([AppGoblin](https://appgoblin.info/apps/com.GlobalSoFunny.Sausage)). **Lección:** el humor y lo "tierno" diferencian en géneros saturados.
- **Hago** (JOYY): app social con salas de voz y más de 100 minijuegos; superó 100 M de descargas en su primer año (2019) ([YourStory](https://yourstory.com/2019/09/app-fridays-hago-social-gaming-google-play)). **Lección:** el chat/voz y la presencia de amigos son el "pegamento" de los juegos sociales.

---

## 6. Indies de PC/consola: el ADN de nuestro género

| Juego | Quién | Dato clave | Lección |
|---|---|---|---|
| **Pico Park** | 1 persona (TECOPARK) | > 1 M copias en 2021; casi todo fuera de Japón; filas largas en sus stands de eventos ([AUTOMATON](https://automaton-media.com/en/?p=3784)) | **Demos en eventos físicos** generan boca a boca. |
| **Stick Fight** | Landfall (pequeño) | Hecho en **~2 meses**; 2–5 M dueños ([SteamSpy](https://steamspy.com/dev/Landfall+West), [Wikiwand](https://wikwiand-revamp.pages.dev/en/Landfall_Games)) | Proyectos cortos y divertidos > proyectos enormes. |
| **Duck Game** | 1 persona | 2–5 M dueños; el publisher le pidió agregar online; usó **peer-to-peer** porque el servidor dedicado era muy difícil ([Wikipedia](https://en.wikipedia.org/wiki/Duck_Game)) | Online simple primero; servidores dedicados después. |
| **Gang Beasts** | 3 hermanos (Boneloaf) | Early access 2014 → 1.0 en 2017; ~1.3 M copias en Steam (est.) ([Boneloaf press](https://gangbeasts.game/press-pack), [Raijin](https://raijin.gg/app/285900/Gang_Beasts)) | Personajes torpes y físicos = momentos virales. |
| **Party Animals** | Recreate Games | Demo con 135 K simultáneos (2020); pico de 104 K al lanzar (2023); hoy ~99% menos; versión móvil anunciada en jul-2026 ([SteamDB](https://steamdb.info/app/1260320/charts), [TechNode](https://technode.com/2026/07/03/hit-party-game-party-animals-heads-to-mobile-bringing-furry-chaos-to-smartphones/)) | Lo tierno vende, pero sin contenido continuo la gente se va. **Competencia futura directa en móvil.** |
| **Ultimate Chicken Horse** | Clever Endeavour | Nació en una game jam, Kickstarter, 0.8–1.7 M copias (est.) ([Clever Endeavour](https://www.cleverendeavourgames.com/about-us), [Steam Page Analyzer](https://www.steampageanalyzer.com/games/386940)) | Validar con jam/prototipo antes de invertir. |
| **Brawlhalla** | Blue Mammoth → Ubisoft (2018) | **100 M de jugadores** (may-2023); F2P con monedas para cosméticos; todos los personajes por US$39.99 ([Ubisoft News](https://news.ubisoft.com/en-us/article/MvVxfuQT6SZ5XnlJY8Pgp/brawlhalla-celebrates-100-million-lifetime-players-with-in-game-event), [Wikipedia](https://en.wikipedia.org/wiki/Brawlhalla)) | Un brawler 2D gratis y multiplataforma puede durar años. |
| **Boomerang Fu** | Cranky Watermelon | ~500 K unidades en ~2 años; el 2º año vendió más que el 1º ([Sifter](https://sifter.com.au/post/boomerang-fu-sold-more-copies-in-its-second-year-clocking-up-500-thousand-sales)); **no encontramos versión móvil** | **El formato no tiene un dueño en móvil online.** |

---

## 7. Fracasos: por qué murieron y qué evitar

| Juego | Qué pasó | Causa principal | Qué hacemos distinto |
|---|---|---|---|
| **Knockout City** (2021–2023) | 12 M lo probaron; pasó de pago a F2P; cerró el 6-jun-2023 ([PC Gamer](https://www.pcgamer.com/two-years-after-launch-live-service-dodgeball-game-knockout-city-is-shutting-down), [GameSpot](https://www.Gamespot.Com/articles/knockout-city-dev-explains-what-went-wrong/1100-6512063/)) | Retención cayendo + un estudio indie no pudo sostener un live service caro | Mantener el costo operativo bajo; no prometer temporadas que no podamos producir. |
| **Rumbleverse** (ago-2022 – feb-2023) | Cerró a los ~6 meses ([PC Gamer](https://pcgamer.com/rumbleverse-the-wrestling-battle-royale-is-shutting-down-only-six-months-after-launch)) | Servidores con problemas al lanzar, poca gente se enteró, pocos jugadores simultáneos ([Game Informer](https://gameinformer.com/2023/01/31/rumbleverse-is-shutting-down-in-february)) | No lanzar online masivo sin pruebas de carga; construir comunidad *antes*. |
| **Squad Busters** (2024–2026) | Ver §4.2 | Núcleo débil + economía mala + sin soft launch real | Probar diversión y retención antes de escalar. |
| **Smash Legends** (2021–) | Pasó a "mantenimiento": sin personajes ni sistemas nuevos ([Smash Legends Dev Notes #18](https://smashlegends.com/en/developer-team-notes-18_en/)) | Brawler competitivo en un nicho dominado por Brawl Stars | No competir de frente con Brawl Stars en "competitivo"; ser **party/casual**. |
| **Sonic Rumble** (2025) | Ver §4.6 | Formato saturado; mala adquisición de usuarios | Diferenciación clara (objeto que regresa + arte urbano + merch real). |
| **mo.co** (2025) | Ver §4.3 | Lanzamiento exclusivo sin retención suficiente | Usar la escasez solo en beta, no como estrategia principal. |

**Patrón común de los fracasos:** (1) la gente lo descarga pero no regresa a los 7 y 30 días; (2) el juego "no hace nada mal, pero nada único"; (3) live service caro con pocos jugadores; (4) lanzamientos sin construir comunidad antes.

---

## 8. Mercado de México y LATAM

### 8.1 Datos duros

| Indicador | Dato | Fuente |
|---|---|---|
| Sistema operativo móvil en México | **Android 74.1% / iOS 25.9%** (oct-2026) | [StatCounter](https://gs.statcounter.com/os-market-share/mobile/mexico) |
| Jugadores en México (cierre 2025) | **73.8 M** (61.4% de la población de 6+ años) | The CIU vía [Diario de Tabasco](https://www.diariodetabasco.mx/negocios/2026/08/27/ingresos-de-la-industria-de-videojuegos-superaran-los-43-mil-500-mdp-en-mexico-the-ciu/) y [The CIU](https://www.theciu.com/publicaciones-2/2026/9/7/mercado-de-videojuegos-en-2026-evolucin-retos-estructurales-y-riesgos-fiscales) |
| Jugadores en smartphone | **63.4 M (85.9%)** | The CIU (misma fuente) |
| Ingresos de videojuegos en México | **42,362 M MXN en 2025** (~US$2,218 M); 43,590 M MXN est. 2026 (+2.9%) | The CIU |
| Dónde está el dinero | Consolas: 22.5% de jugadores pero **79.4% de los ingresos** | The CIU vía [Julio Astillero](https://julioastillero.com/el-crecimiento-de-la-industria-del-gaming-en-mexico/) |
| Gasto por jugador | Varía mucho según la fuente: ~4,900–4,965 MXN/año "jugador de smartphone" (The CIU citada en prensa); 5,500 MXN/año gamer promedio 2024 (Endeavor/Santander) | [Informador](https://www.informador.mx/mexico/Esto-es-lo-que-gasta-un-gamer-en-Mexico-al-ano-20250402-0123.html), [Gamers México](https://gamersmexico.org/el-jugador-mexicano-gasta-38-mas-al-ano-que-el-brasileno/) |
| Gama de celulares (base instalada, Q3-2025) | **Gama media 67.9%**, alta 17.1%, baja 15.0% (≤3,000 MXN) | The CIU vía [Xataka México](https://www.xataka.com.mx/celulares-y-smartphones/mexico-fin-se-vendieron-telefonos-gama-alta-que-gama-baja-asi-cambio-mercado) |
| Configuración más vendida 2025 | 8 GB RAM / 256 GB (37% del mercado) | Counterpoint vía [Expansión](https://expansion.mx/tecnologia/2026/01/19/mexicanos-prefieren-celulares-premium-motorola-xiaomi) |
| Celulares vendidos por año | ~32 M | The CIU (misma nota) |
| Mercado LATAM 2025 | 372.3 M jugadores, 170.9 M pagadores, **US$8.3 B**; **móvil US$4.4 B** (+7.8%/año); Brasil US$2.71 B | Newzoo vía [GamesBeat](https://gamesbeat.com/latin-america-defies-game-stagnation-with-372-3m-players-and-8-3b-in-2025-revenues/) |
| Usuarios de TikTok en México | ~85 M cuentas (Digital 2025); ~99 M de alcance publicitario 18+ a fines de 2025 | DataReportal vía [ADN40](https://www.adn40.mx/es-tendencia/2025-09-01/rango-edad-las-personas-que-usan-tiktok-en-mexico-y-que-hacen-en-la-plataforma/), [juancmejia.com](https://juancmejia.com/?p=69620) |

### 8.2 Interpretación (lo que significan estos números para nosotros)

- **Cálculo propio (cota superior):** si las consolas generan 79.4% de 42,362 M MXN, todo lo demás (móvil + PC) es como máximo ~8,727 M MXN. Dividido entre 63.4 M de jugadores móviles da **≤ ~138 MXN por jugador móvil al año (~US$7)**. Esto **contradice** la cifra de "4,900 MXN por jugador de smartphone" que circula en prensa; lo más probable es que esa cifra mida a quienes sí pagan o mezcle plataformas. *(No pude abrir el reporte original de The CIU para resolverlo.)* **Conclusión práctica: la gran mayoría de jugadores móviles en México paga poco o nada.** El modelo debe ser F2P con cosméticos accesibles (p. ej. 19–49 MXN) y un pase de temporada barato.
- **Android de gama media es la plataforma objetivo.** Aunque la gama baja ya no domina, el 15% sigue siendo millones de equipos. Meta técnica: correr fluido en un Android de ~3–4 GB de RAM, con descarga inicial pequeña.
- **LATAM puede ser el motor de descargas, no de ingresos:** Stumble Guys tuvo 52.5 M descargas en LATAM, pero el país #1 en ingresos fue EE.UU. Brasil sí fue #2 en ingresos, lo que demuestra que la región **paga si el producto le gusta**.

### 8.3 ¿Qué se juega en México y LATAM?

- **Free Fire** sigue dominando: más de 100 M de descargas en LATAM en 2025 ([Diario de Morelos](https://www.diariodemorelos.com/noticias/los-juegos-m-s-populares-para-los-mexicanos-en-2025)). En México, Sensor Tower registró picos semanales de **~US$1.75 M** (evento Light Fest, sep-2025) y **~US$2 M** (colaboración con *Jujutsu Kaisen*, dic-2025) ([Sensor Tower Q3](https://sensortower.com/blog/2025-q3-android-top-5-free%20to%20play%20games-revenue-mx-620e5136afb48b4f71fc7c60), [Sensor Tower Q4](https://sensortower.com/blog/2025-q4-android-top-5-third-person-games-units-mx-60214330241bc16eb89ce15d)). Su éxito histórico se atribuye a que corre en celulares baratos. Garena creció 40.6% en Q1-2026 porque **más jugadores pagaron**, no porque hubiera más jugadores ([Free Fire Mania](https://www.freefiremania.com.br/news/free-fire-grows-40-and-brings-garena-to-its-best-results-since-2021.html)).
- **Roblox**: en Brasil sus usuarios diarios crecieron **181%** entre Q4-2020 y Q4-2024 ([Roblox](https://corp.roblox.com/newsroom/2025/09/roblox-annual-economic-impact-report)). No encontré cifras específicas de México.
- **Brawl Stars** es fuerte en México (ver §4.1) y **Stumble Guys** fue enorme en Brasil (ver §5.1).
- No hay un ranking único y confiable 2025 de "juegos más jugados en México" en fuentes abiertas; las listas mezclan Clash Royale, Candy Crush, Subway Surfers y Block Blast como los más descargados *(datos de fechas poco claras)*.

### 8.4 ¿Cómo pagan los mexicanos? (clave para INKCAP)

- **Efectivo en tienda:** Google Play permite elegir "pago en efectivo" en la app, genera una referencia y se paga en caja de **OXXO o 7-Eleven** en México (la tienda puede cobrar comisión) ([Ayuda de Google Play](https://support.google.com/googleplay/answer/9462077?hl=es-419), [La Crónica, oct-2026](https://www.cronica.com.mx/destino-c/2026/10/09/tarjetas-de-google-play-en-mexico-en-tienda-en-linea-o-en-efectivo-desde-la-app/)).
- **Tarjetas de regalo Google Play** en OXXO, Neto, Six, Soriana, 7-Eleven, Chedraui, Kiosko y Walmart (montos de 100 a 5,000 MXN) ([Google Play gift cards](https://support.google.com/googleplay/answer/3422734?hl=es)).
- **Población no bancarizada:** las estimaciones varían de **42% a 52.8%**; un dato antiguo da solo **7.7%** con tarjeta de crédito *(dato viejo, usar con cuidado)* ([Unlimit](https://www.unlimit.com/blog/payments/blog-payment-methods-in-mexico/), [Rebill](https://www.rebill.com/en/blog/oxxo-pay-cash-is-king-in-the-mexican-market)). En comercio electrónico general: tarjeta de crédito 32%, billeteras digitales 26%, débito 23%, efectivo/OXXO ~9% ([Antom](https://www.antom.com/guides/markets/mexico)).
- **OXXO Pay** ya se usa para recargas de juegos (incluso V-Bucks de Fortnite) ([Rebill](https://www.rebill.com/en/blog/oxxo-pay-cash-is-king-in-the-mexican-market)).
- **Implicación:** INKCAP ya cobra con OXXO en su tienda. Un modelo **"compra la playera física en la tienda INKCAP → recibe un código para la skin en el juego"** evita la comisión de las tiendas de apps para el componente físico y aprovecha un método de pago que su cliente ya usa. *(Ojo: las reglas de Apple/Google sobre canjear bienes digitales comprados fuera de la app deben revisarse en el reporte legal/monetización.)*

### 8.5 TikTok, streamers y creadores

- **Amigos y familia** son el canal #1 de descubrimiento en LATAM hispanohablante; cerca de la mitad descarga juegos *para jugar con amigos*, y **24% de los jugadores en México descargó su último juego tras ver a un influencer jugarlo** ([Google Play – Medium](https://medium.com/googleplaydev/engaging-players-and-unlocking-spend-in-spanish-speaking-latin-america-1cfef4ac08b)) *(fecha exacta de la encuesta no confirmada)*.
- **75%** de los jugadores mexicanos encuestados juega en línea con otros al menos una vez al mes ([Power of Play México](https://www.gamesmarket.global/power-of-play-mexico/)).
- Newzoo/Google notaron que muchos espectadores hispanohablantes ven streamers de España: **hay espacio para creadores con cultura latinoamericana** ([Google Play – Medium](https://medium.com/googleplaydev/insights-into-latin-americas-3-5-efda3b6d7917)). INKCAP TV (podcast con tatuadores y grafiteros) es exactamente ese tipo de contenido.
- **Advertencia:** algunas agencias reportan que las vistas de TikTok de juegos indie están bajando y que **vistas ≠ descargas** ([CloutBoost](https://www.cloutboost.com/blog/tiktoks-changing-landscape-for-game-marketing-in-2026-what-developers-need-to-know), [Growth Therapist](https://growththerapist.substack.com/p/the-1-tiktok-virality-hack)). TikTok es necesario pero no suficiente.

### 8.6 Costos de adquirir jugadores y de anuncios en LATAM

| Concepto | Rango | Fuente |
|---|---|---|
| CPI (costo por instalación) LATAM, juegos (2025) | ~US$0.14 promedio regional; subió 40% en el año; Brasil +50%, México +40% | Adjust vía [GameDev Reports](https://gamedevreports.substack.com/p/adjust-gaming-app-insights-report) |
| CPI casual Android LATAM | **US$0.15–0.60** (~3–11 MXN) | [Game Growth Advisor](https://gamegrowthadvisor.com/blog/2026-05-26-latam-mobile-game-market-entry-guide-2026/) |
| CPI mid-core Brasil/México | **US$0.80–2.00** (~15–38 MXN) | Misma fuente |
| Países más baratos para soft launch casual | Colombia, Perú, Argentina (< US$1 en Meta/TikTok) | Misma fuente |
| Anuncio recompensado (eCPM) | LATAM vale **4–5 veces menos** que Norteamérica; ~US$2–3.4 por mil en Android | [Playio](https://blog.playio.co/mobile-game-ecpm-benchmarks-2026), [Appodeal](https://appodeal.com/the-mobile-ecpm-report-updated-q4-2024-view) |

**Ojo:** el CPI medido en LATAM **no predice** lo que costará EE.UU./Europa; úsalo para medir retención y monetización, no para proyectar costos globales ([Game Growth Advisor](https://gamegrowthadvisor.com/blog/2026-03-17-user-acquisition-cpi-benchmarks-2026/)).

---

## 9. Patrones de éxito (lo que hicieron bien)

| Patrón | Quién lo hizo | Cómo aplicarlo |
|---|---|---|
| **Sesiones cortas (1–3 min)** | Brawl Stars, Stumble Guys, Archero, Bowmasters | Rondas de 60–90 s, partidas "al mejor de 5" en < 5 min. |
| **Jugar con amigos** | Among Us, BombSquad, Stumble Guys, Hago | Sala con código/QR, Wi-Fi local, invitación por WhatsApp. |
| **Controles de un toque** | Archero, Bowmasters, Stumble Guys | Joystick + 1 botón (lanzar/recoger), 1 botón opcional (esquivar). |
| **Cosméticos, no ventaja** | Brawlhalla, Fall Guys, Among Us, Brawl Stars (2024) | Vender look (playeras, gorras, stickers, efectos de "tinta"), nunca poder. |
| **Temporadas + pase** | Brawl Stars, Fall Guys, Free Fire | Temporadas de 6–8 semanas con temática de una "técnica de tinta" y un artista invitado. |
| **Colaboraciones** | Free Fire (picos con colaboraciones), Brawl Stars, Stumble Guys | Cada colaboración con un artista INKCAP = evento + skin + merch física. |
| **Creadores de contenido** | Stumble Guys (gemas para regalar), Supercell (códigos de creador), mo.co (QR en streams) | Programa de creadores desde el día 1; códigos que dan % al creador. |
| **Soft launch largo y valiente** | Brawl Stars (18 meses, rediseños radicales) | Estar dispuestos a cambiar todo si la retención no da. |
| **Móvil primero y ligero** | Stumble Guys, Free Fire | Apuntar a Android de gama media; tamaño de descarga pequeño. |
| **Velocidad** | Stumble Guys (semanas), Stick Fight (2 meses) | Prototipo jugable en semanas; iterar con jugadores reales. |
| **Personajes con personalidad** | Bowmasters, Gang Beasts, Party Animals, Zooba | Muñequitos art toy con animaciones exageradas = clips virales. |
| **Eventos físicos / demos** | Pico Park (filas en stands) | Torneos en la tienda INKCAP, convenciones, eventos de arte urbano. |

---

## 10. Errores a evitar

1. **Ser "uno más" del party royale** (Sonic Rumble, Squad Busters): el formato de 32–60 jugadores está saturado.
2. **Lanzar sin medir retención** (Squad Busters saltó el soft launch).
3. **Economía agresiva / pay-to-win:** Squad Busters monetizaba alto pero perdía jugadores; Brawl Stars mejoró al **quitar cajas de botín**.
4. **No tener qué vender cuando llega el éxito** (Among Us generó relativamente poco para su tamaño).
5. **Live service más caro de lo que se puede sostener** (Knockout City).
6. **Servidores frágiles el día 1** (Rumbleverse).
7. **Depender de un solo pico de hype** sin contenido continuo (Party Animals, Fall Guys).
8. **Gastar en anuncios antes de tener retención:** cada instalación cuesta y en LATAM vale poco.
9. **Ignorar Android de gama media** en un país 74% Android.
10. **Prometer online global desde el inicio** con un equipo que no programa; mejor local → online asíncrono/ligero → online completo.

---

## 11. Estrategias de lanzamiento de bajo presupuesto que funcionaron

| Táctica | Evidencia | Costo aprox. |
|---|---|---|
| **Creadores + regalos de moneda del juego** | Stumble Guys cambió anuncios por influencers ([GameAnalytics](https://gameanalytics.com/?p=17893)) | Moneda virtual (costo casi cero) + pagos a micro-creadores |
| **Códigos de creador** | Supercell lo usa en todos sus juegos ([Supercell](https://supercell.com/en/games/brawlstars/blog/community/creator-boost-faq/)) | % de las ventas (solo pagas si vendes) |
| **Lanzamiento por invitación / QR en streams** | mo.co ([PocketGamer.biz](https://www.pocketgamer.biz/supercells-moco-launches-today-as-studios-seventh-global-game-but-theres-a-catch)) | Gratis; útil para beta cerrada |
| **TikTok orgánico** | Casos indie con saltos de *wishlists* tras un video viral; pero las vistas indie están bajando ([CloutBoost](https://www.cloutboost.com/blog/tiktoks-changing-landscape-for-game-marketing-in-2026-what-developers-need-to-know)) | Tiempo del equipo |
| **Discord propio** | Estándar de la industria para comunidad y testers | Gratis |
| **Demos en eventos físicos** | Pico Park ([AUTOMATON](https://automaton-media.com/en/?p=3784)) | Stand / tienda propia |
| **Steam Next Fest (si sacamos versión PC)** | Llegar con 2,000+ *wishlists*; ~1 de cada 5 que juega la demo la agrega; no estrenar la demo en el festival ([How To Market A Game](https://howtomarketagame.com/2025/03/26/archive-january-2026-benchmarks-how-many-wishlists-can-i-get-from-steam-next-fest/), [Unity](https://unity.com/blog/steam-next-fest-developer-tips-unity)) | Gratis |
| **Featuring de Apple** | Nominaciones desde App Store Connect (desde nov-2024); enviar hasta 3 meses antes, mínimo 2 semanas ([Apple](https://developer.apple.com/app-store/getting-featured)) | Gratis |
| **Google Play Indie Games Fund LATAM** | US$2 M repartidos; US$150–200 K por estudio; ≤ 50 empleados; juego ya publicado; abierto a interés para 2026; en 2025 ganó un estudio mexicano ([Google Play](https://play.google.com/intl/id_ALL/console/about/programs/indiegames/), [Google Blog](https://blog.google/products-and-platforms/platforms/google-play/indie-games-fund-latin-america-2025/)) | Gratis (¡dinero sin ceder acciones!) |
| **Soft launch en países baratos** | CPI casual < US$1 en Colombia/Perú/Argentina ([Game Growth Advisor](https://gamegrowthadvisor.com/blog/2026-05-26-latam-mobile-game-market-entry-guide-2026/)) | US$1,500–6,000 por 10,000 instalaciones |
| **"Phygital": merch física + skin digital** | Forever 21 vendió un gorro virtual 1.5 M veces en Roblox antes de hacerlo físico; Ralph Lauren vendió 200 botas físicas de US$250 tras ítems en Fortnite ([Modern Retail](https://www.modernretail.co/technology/how-ralph-laurens-phygital-boot-sale-in-fortnite-shows-benefits-and-limits-of-virtual-commerce/), [Glossy](https://www.glossy.co/fashion/digital-fashion-brand-play-pop-go-launches-first-physical-collection)); Balenciaga × Need for Speed Mobile con chips NFC en ropa *(no confirmado en fuente primaria)* ([Trend Hunter](https://trendhunter.com/trends/need-for-speed)) | Lo que ya produce INKCAP |

**Referencia de retención (para saber si vamos bien):** según GameAnalytics (datos 2025), el juego **mediano** retiene **~22% al día 1, ~4% al día 7 y ~0.7% al día 30**; el **10% mejor** logra **~40% D1 y ~12% D7**; el 1% mejor, 64–68% D1 y 13–15% D30 ([GameDev Reports](https://gamedevreports.substack.com/p/gameanalytics-mobile-and-pc-game), [InvestGame](https://investgame.net/news/pdf/2026-01-20-mobile_retention_benchmarks_2026/)). **Meta mínima para escalar: D1 ≥ 40%, D7 ≥ 15%, D30 ≥ 5%.**

---

## 12. 10 lecciones accionables para nuestro juego

1. **No hagamos "otro Stumble Guys".** Nuestro hueco es el **party brawler de arena 2–8 jugadores con objeto que regresa**, formato que en móvil online no tiene un dueño claro (Boomerang Fu no tiene versión móvil; Party Animals apenas anunció la suya).
2. **Rondas de 60–90 segundos, partida completa < 5 minutos**, controles de joystick + 1 botón. Si no se entiende en 10 segundos de video de TikTok, hay que simplificar.
3. **Local primero, pero con bots.** Among Us y BombSquad prueban que lo local funciona; agreguemos bots para que un jugador solo pueda practicar y los cuartos siempre se llenen. Diseñar la red desde el día 1 para que luego pase a online sin reescribir todo.
4. **Cosméticos 100% estéticos y baratos.** Playeras, gorras, stickers, "salpicaduras de tinta" y emotes. Precios de entrada de ~19–49 MXN; pase de temporada ~79–99 MXN *(precios sugeridos, a validar)*. Nunca vender ventaja.
5. **Cada temporada = una técnica de tinta + un artista INKCAP.** Grafiti, tatuaje, serigrafía, bordado, sticker y tinta derramada dan **6 temporadas temáticas listas**. Cada una con escenario, skin y "drop" físico de edición limitada.
6. **Phygital desde el lanzamiento:** la playera física de la tienda trae un **código para la skin**; la skin más usada en el juego se convierte en playera real (modelo Forever 21). Esto convierte al juego en canal de ventas de INKCAP y viceversa.
7. **Optimizar para Android de gama media** (74% del mercado): meta de 60 fps en un equipo de ~4 GB de RAM, descarga inicial pequeña y paquetes de contenido opcionales.
8. **Programa de creadores desde la beta:** códigos de creador con % de ventas, gemas para regalar (táctica Stumble Guys) y aprovechar INKCAP TV y los artistas de la marca como primeros creadores.
9. **Medir antes de gastar:** no invertir en anuncios hasta tener D1 ≥ 40% y D7 ≥ 15% en el soft launch. Si no llegamos, cambiar el núcleo (como Brawl Stars con el giro a horizontal).
10. **Cobrar como paga México:** Google Play en efectivo (OXXO/7-Eleven), tarjetas de regalo y la tienda INKCAP con OXXO para el combo físico+digital. Revisar con cuidado las reglas de Apple/Google sobre canje de códigos.

---

## 13. Estrategia de lanzamiento sugerida

| Fase | Tiempo | Objetivo | Acciones | Presupuesto aprox. |
|---|---|---|---|---|
| **0. Prototipo de diversión** | Meses 0–3 | Probar que el núcleo es divertido | Prototipo en Unity con 1 arena (calle grafiteada), 4 muñequitos, objeto que regresa; pruebas de "sillón" con amigos y clientes INKCAP | Tiempo del equipo + licencias (ver reporte técnico) |
| **1. Comunidad y beta cerrada local** | Meses 3–6 | Validar con fans | Discord, beta cerrada por invitación (QR en INKCAP TV y en la tienda), torneos en tienda/eventos de arte urbano; versión **local Wi-Fi** con bots | ~US$500–1,500 (~9,550–28,650 MXN) en eventos/premios |
| **2. Soft launch Android** | Meses 6–9 | Medir retención y monetización | Lanzar en Colombia, Perú, Chile (hispanos, CPI bajo); 10,000 instalaciones pagadas para tener datos | ~US$1,500–6,000 (~28,650–114,600 MXN) |
| **3. Lanzamiento México + LATAM** | Meses 9–12 | Escalar | Android + iOS; online básico (salas con código + emparejamiento simple); programa de creadores; nominación a featuring de Apple; solicitud al Google Play Indie Games Fund; primer "drop" phygital con un artista | Depende de métricas; empezar con US$5,000–10,000 (~95,500–191,000 MXN) en creadores y anuncios |
| **4. Live service** | Año 2 | Retener y monetizar | Temporadas de 6–8 semanas (una por técnica de tinta), colaboraciones, ranking online, posible versión PC/Steam con demo en Next Fest | Financiado por ingresos + merch |

**Regla de oro:** no pasar de una fase a la siguiente sin cumplir la meta de la fase anterior (diversión comprobada → comunidad activa → D1 ≥ 40% / D7 ≥ 15% → escalar).

---

## 14. Recomendación para nuestro juego

1. **Posicionamiento:** "El party brawler de arte urbano". Un **brawler de arena para 2–8 jugadores** (no un battle royale de 60), con muñequitos estilo art toy y un objeto que regresa con identidad propia de la marca (p. ej. **una gorra o una lata de aerosol "boomerang"** que deja rastro de tinta). Esto nos separa de Stumble Guys/Fall Guys/Sonic Rumble y nos acerca a un hueco que Boomerang Fu dejó libre en móvil.
2. **Monetización:** F2P + cosméticos + pase de temporada barato + **combo físico/digital con la tienda INKCAP**. Los anuncios recompensados pueden existir (opcionales), pero en LATAM valen 4–5 veces menos que en EE.UU.; no deben ser el pilar.
3. **Plataforma:** Android primero (74% de México), iOS en paralelo cuando el soft launch esté validado. Unity encaja (Fall Guys y el equipo de Quest 3 de INKCAP ya lo usan).
4. **Red:** local Wi-Fi + bots al inicio (como Among Us 2018 y BombSquad); online con salas por código después; servidores dedicados solo si la retención lo justifica (Duck Game empezó con peer-to-peer).
5. **Marketing:** comunidad propia (INKCAP TV, artistas, tienda) → beta por invitación → creadores con códigos y gemas para regalar → TikTok con clips de "jugadas épicas" de los muñequitos → eventos físicos de arte urbano.
6. **Ventaja injusta de INKCAP:** ya tiene artistas, una base de clientes, una tienda que cobra con OXXO, producción de merch y un podcast. **Ningún competidor del género tiene un puente real entre la skin del juego y la playera que te pones.** Ese es el diferenciador.
7. **Expectativas realistas:** la mayoría de los juegos no supera la mediana de retención (22% D1 / 4% D7). El objetivo del primer año no es "ser Stumble Guys", sino **validar diversión y retención con un presupuesto controlado**, y usar el juego como motor de marca y ventas de merch aunque los ingresos dentro de la app tarden en llegar.

---

## 15. Dudas abiertas / datos no confirmados

- Gasto real por jugador móvil en México: la cifra de ~4,900 MXN/año contradice el cálculo de ≤138 MXN/año que sale de la proporción de ingresos de consolas (reporte original de The CIU no consultado directamente).
- Porcentaje exacto que Supercell paga en sus códigos de creador (se dice ~5%, sin confirmación oficial).
- Ingresos 2025 de Stumble Guys y de Brawl Stars (las estimaciones varían mucho entre trackers).
- Cifras propias de Zooba, Sausage Man, Super Sus y Bomber Friends (solo hay estimaciones de baja calidad).
- Si Bowmasters cambió de dueño (Miniclip) — no confirmado.
- Ranking 2025 confiable de los juegos más jugados en México (requiere Sensor Tower/AppMagic de pago).
- Reglas actualizadas 2026 de Apple/Google sobre canje en la app de códigos obtenidos con compras físicas (tema para el reporte de monetización/legal).
- Fecha exacta de la encuesta de Google Play sobre influencers en México (24%).

---

## 16. Fuentes

**Tipo de cambio**
- https://dolar.info/mexico/hoy/precio/banxico-fix.php
- https://www.expreso.com.mx/noticias/negocios/precio-del-dolar-hoy-22-de-noviembre-de-2026-en-mexico/257000

**Brawl Stars / Supercell**
- https://www.pocketgamer.biz/brawl-stars-was-very-much-on-the-edge/
- https://www.deconstructoroffun.com/blog/2018/5/11/bs
- https://tcrf.net/Brawl_Stars/Version_Differences/Beta_Softlaunch
- https://mobilegamer.biz/why-supercell-doubled-the-brawl-stars-team-and-continues-to-evolve-its-culture/
- https://www.pocketgamer.biz/frank-keienburg-brawl-stars-game-lead-on-supercellian-culture-and-what-other-platforms-can-learn-from-mobile
- https://www.blog.udonis.co/mobile-marketing/mobile-games/brawl-stars-player-count
- https://www.pocketgamer.biz/april-2024-mobile-game-charts-call-of-duty-warzone-and-brawl-stars/
- https://www.pocketgamer.biz/brawl-stars-has-already-earned-more-in-2024-than-the-entirety-of-2023/
- https://gamingonphone.com/news/brawl-stars-revenue-jumped-to-almost-9x-and-mau-to-2-4x-in-2024-compared-to-last-year/
- https://www.deconstructoroffun.com/blog/2024/2/11/brawl-stars-to-the-moon
- https://www.pocketgamer.biz/march-2026-mobile-game-charts-brawl-stars-revenue-rises-nearly-50/
- https://sensortower.com/blog/2025-q4-unified-top-5-gacha-games-revenue-mx-62052cd7afb48b4f71f52675
- https://supercell.com/en/games/brawlstars/blog/community/creator-boost-faq/

**Squad Busters / mo.co**
- https://www.pocketgamer.biz/supercells-squad-busters-hits-24m-net-revenue-in-first-30-days
- https://www.gamelight.io/news/squad-busters-by-supercell-earns-$24m-in-first-30-days,-sees-revenue-decline-post-launch
- https://mobidictum.com/squad-busters-why-a-50-revenue-drop/
- https://www.deconstructoroffun.com/blog/2024/9/30/4-x-reasons-why-squad-busters-suffers-while-brawl-stars-soars
- https://www.liquidandgrit.com/?p=14281
- https://pocketgamer.biz/pressure-to-launch-a-new-game-led-to-squad-busters-global-release
- https://nextbiggames.com/2026/02/16/squad-busters-failure-lessons-supercell/
- https://www.pocketgamer.biz/supercell-will-shut-down-squad-busters-next-year/
- https://respawn.outlookindia.com/gaming/gaming-news/supercell-confirms-squad-busters-shutdown-date
- https://www.pocketgamer.biz/supercells-moco-hits-2-5m-downloads-and-570k-in-first-week
- https://www.pocketgamer.biz/supercells-moco-surpasses-25m-in-under-a-month
- https://www.pocketgamer.biz/supercells-moco-launches-today-as-studios-seventh-global-game-but-theres-a-catch
- https://mobilegamer.biz/supercells-monster-hunting-game-mo-co-is-going-back-into-beta/

**Fall Guys / Eggy Party / Sonic Rumble**
- https://gamedeveloper.com/business/fall-guys-hit-50-million-players-two-weeks-after-freemium-switch
- https://icon-era.com/statistics/fall-guys-player-count/
- https://www.gamedeveloper.com/business/report-i-fall-guys-i-developer-mediatonic-decimated-epic-games-layoffs
- https://dev.to/wlz/why-fall-guys-bet-on-unity-the-boring-phone-engine-behind-a-32-player-bean-battle-royale-28e9
- https://www.pocketgamer.biz/eggy-party-cracks-750m-in-mobile-player-spending
- https://www.pocketgamer.biz/neteases-eggy-party-reaches-500-million-players-in-under-two-years
- https://www.pocketgamer.biz/eggy-party-sails-past-100-million-user-generated-maps
- https://www.pocketgamer.biz/sonic-rumble-earns-just-228000-in-first-10-days-with-a-future-major-update-in-the-works
- https://www.pocketgamer.biz/sonic-rumble-jogs-to-772000-in-two-months
- https://www.videogameschronicle.com/news/sega-says-its-new-releases-especially-sonic-rumble-have-performed-below-expectations
- https://www.gamesradar.com/games/sonic-the-hedgehog/sega-downsizes-free-to-play-division-after-weak-performance-from-sonic-rumble-party-moving-over-100-devs-onto-full-game-releases-in-mainstay-ips/

**Stumble Guys**
- https://www.pocketgamer.biz/scopely-acquires-stumble-guys-from-kitka-games
- https://gamesbeat.com/scopely-acquires-stumble-guys-battle-royale-game-from-kitka-games/
- https://mobilegamer.biz/how-stumble-guys-beat-fall-guys-at-its-own-game/
- https://mobilegamer.biz/surprise-smash-stumble-guys-hits-40m-revenue-and-225m-downloads/
- https://mobilegamer.biz/fall-guys-clone-stumble-guys-has-now-earned-over-60m-from-265m-downloads/
- https://www.pocketgamer.biz/battle-royale-game-stumble-guys-is-making-almost-half-a-million-dollars-every-day/
- https://gameworldobserver.com/2023/01/26/sensor-tower-subway-surfers-was-most-downloaded-game-in-q4-2022-but-the-stumble-guys-showed-fastest-growth
- https://naavik.co/digest/stumbleguys-garenafreefire/
- https://gameanalytics.com/?p=17893
- https://playtracker.net/quest/game/130973
- https://www.blog.udonis.co/mobile-marketing/mobile-games/stumble-guys

**Among Us**
- https://kotaku.com/among-us-improbable-rise-to-the-top-of-twitch-1844986025
- https://globalgamesforum.com/features/the-long-and-winding-road-to-success-for-among-us
- https://steamdb.info/app/945360
- https://opera.sportskeeda.com/among-us/among-us-221-millions-hours-watched-twitch-just-three-months
- https://www.blog.udonis.co/mobile-marketing/mobile-games/among-us-player-count
- https://www.gamesradar.com/games/co-op/among-us-hits-1-billion-downloads-on-google-play-after-almost-8-years-1-8-of-the-world-are-crewmates-or-impostors-hard-to-say/
- https://appfigures.com/resources/insights/20240119/5-the-game-that-could-have-been-the-next-angry-birds-but-wasnt
- https://www.pcgamer.com/among-us-sudden-twitch-success-left-its-developer-feeling-burnt-out

**Otros móviles (BombSquad, Bomber Friends, Bowmasters, Archero, Super Sus, Zooba, Sausage Man, Hago)**
- https://fr.github.com/efroemling
- https://froemling.net/page/3
- https://appgoblin.info/apps/net.froemling.bombsquad
- https://appgoblin.info/apps/net.froemling.bsremote
- https://www.appbrain.com/dev/Eric+Froemling/
- https://ballistica.net/whataretokens
- https://www.apkmirror.com/?p=8843974
- https://appgoblin.info/apps/com.hyperkani.bomberfriends
- https://www.appbrain.com/dev/Hyperkani/
- https://sensortower.com/blog/playgendary-revenue-q3-2018
- https://www.pocketgamer.biz/playgendary-revenue-went-up-57-per-cent-during-q3-2018
- https://www.deconstructoroffun.com/blog/2019/8/9/why-archero-banked-25m-but-leaves-25m-hanging-hlx9n
- https://www.gamigion.com/?p=20716
- https://gameworldobserver.com/2025/01/20/archero-2-revenue-8-million-in-11-days-vs-original-game
- https://www.pocketgamer.biz/archero-2-makes-328m-in-first-30-days-from-player-spending
- https://www.deconstructoroffun.com/blog/2025/7/31/habbys-hybridcasual-empire-the-template-that-built-a-powerhouse
- https://store.steampowered.com/app/2920270/Super_Sus/
- https://app.sensortower.com/vgi/game/super-sus
- https://gamesbeat.com/?p=182748
- https://apps.apple.com/app/id1459402952
- https://appgoblin.info/apps/com.GlobalSoFunny.Sausage
- https://yourstory.com/2019/09/app-fridays-hago-social-gaming-google-play

**Indies PC/consola**
- https://automaton-media.com/en/?p=3784
- https://steamspy.com/dev/Landfall+West
- https://wikwiand-revamp.pages.dev/en/Landfall_Games
- https://en.wikipedia.org/wiki/Duck_Game
- https://gangbeasts.game/press-pack
- https://raijin.gg/app/285900/Gang_Beasts
- https://steamdb.info/app/1260320/charts
- https://raijin.gg/app/1260320/Party_Animals
- https://technode.com/2026/07/03/hit-party-game-party-animals-heads-to-mobile-bringing-furry-chaos-to-smartphones/
- https://www.cleverendeavourgames.com/about-us
- https://www.steampageanalyzer.com/games/386940
- https://news.ubisoft.com/en-us/article/MvVxfuQT6SZ5XnlJY8Pgp/brawlhalla-celebrates-100-million-lifetime-players-with-in-game-event
- https://en.wikipedia.org/wiki/Brawlhalla
- https://sifter.com.au/post/boomerang-fu-sold-more-copies-in-its-second-year-clocking-up-500-thousand-sales

**Fracasos**
- https://www.pcgamer.com/two-years-after-launch-live-service-dodgeball-game-knockout-city-is-shutting-down
- https://www.Gamespot.Com/articles/knockout-city-dev-explains-what-went-wrong/1100-6512063/
- https://pcgamer.com/rumbleverse-the-wrestling-battle-royale-is-shutting-down-only-six-months-after-launch
- https://gameinformer.com/2023/01/31/rumbleverse-is-shutting-down-in-february
- https://smashlegends.com/en/developer-team-notes-18_en/

**México y LATAM**
- https://gs.statcounter.com/os-market-share/mobile/mexico
- https://www.theciu.com/publicaciones-2/2026/9/7/mercado-de-videojuegos-en-2026-evolucin-retos-estructurales-y-riesgos-fiscales
- https://www.theciu.com/publicaciones-2/2025/9/1/dadelgamer2025-mercado-de-videojuegos-en-mxico
- https://www.diariodetabasco.mx/negocios/2026/08/27/ingresos-de-la-industria-de-videojuegos-superaran-los-43-mil-500-mdp-en-mexico-the-ciu/
- https://julioastillero.com/el-crecimiento-de-la-industria-del-gaming-en-mexico/
- https://www.informador.mx/mexico/Esto-es-lo-que-gasta-un-gamer-en-Mexico-al-ano-20250402-0123.html
- https://gamersmexico.org/el-jugador-mexicano-gasta-38-mas-al-ano-que-el-brasileno/
- https://www.xataka.com.mx/celulares-y-smartphones/mexico-fin-se-vendieron-telefonos-gama-alta-que-gama-baja-asi-cambio-mercado
- https://expansion.mx/tecnologia/2026/01/19/mexicanos-prefieren-celulares-premium-motorola-xiaomi
- https://gamesbeat.com/latin-america-defies-game-stagnation-with-372-3m-players-and-8-3b-in-2025-revenues/
- https://www.diariodemorelos.com/noticias/los-juegos-m-s-populares-para-los-mexicanos-en-2025
- https://sensortower.com/blog/2025-q3-android-top-5-free%20to%20play%20games-revenue-mx-620e5136afb48b4f71fc7c60
- https://sensortower.com/blog/2025-q4-android-top-5-third-person-games-units-mx-60214330241bc16eb89ce15d
- https://www.freefiremania.com.br/news/free-fire-grows-40-and-brings-garena-to-its-best-results-since-2021.html
- https://corp.roblox.com/newsroom/2025/09/roblox-annual-economic-impact-report
- https://support.google.com/googleplay/answer/9462077?hl=es-419
- https://www.cronica.com.mx/destino-c/2026/10/09/tarjetas-de-google-play-en-mexico-en-tienda-en-linea-o-en-efectivo-desde-la-app/
- https://support.google.com/googleplay/answer/3422734?hl=es
- https://www.unlimit.com/blog/payments/blog-payment-methods-in-mexico/
- https://www.rebill.com/en/blog/oxxo-pay-cash-is-king-in-the-mexican-market
- https://www.antom.com/guides/markets/mexico
- https://www.adn40.mx/es-tendencia/2025-09-01/rango-edad-las-personas-que-usan-tiktok-en-mexico-y-que-hacen-en-la-plataforma/
- https://juancmejia.com/?p=69620
- https://medium.com/googleplaydev/engaging-players-and-unlocking-spend-in-spanish-speaking-latin-america-1cfef4ac08b
- https://medium.com/googleplaydev/insights-into-latin-americas-3-5-efda3b6d7917
- https://www.gamesmarket.global/power-of-play-mexico/

**Adquisición, anuncios, retención y lanzamiento**
- https://gamedevreports.substack.com/p/adjust-gaming-app-insights-report
- https://gamegrowthadvisor.com/blog/2026-05-26-latam-mobile-game-market-entry-guide-2026/
- https://gamegrowthadvisor.com/blog/2026-03-17-user-acquisition-cpi-benchmarks-2026/
- https://blog.playio.co/mobile-game-ecpm-benchmarks-2026
- https://appodeal.com/the-mobile-ecpm-report-updated-q4-2024-view
- https://gamedevreports.substack.com/p/gameanalytics-mobile-and-pc-game
- https://investgame.net/news/pdf/2026-01-20-mobile_retention_benchmarks_2026/
- https://developer.apple.com/app-store/getting-featured
- https://play.google.com/intl/id_ALL/console/about/programs/indiegames/
- https://blog.google/products-and-platforms/platforms/google-play/indie-games-fund-latin-america-2025/
- https://howtomarketagame.com/2025/03/26/archive-january-2026-benchmarks-how-many-wishlists-can-i-get-from-steam-next-fest/
- https://unity.com/blog/steam-next-fest-developer-tips-unity
- https://www.cloutboost.com/blog/tiktoks-changing-landscape-for-game-marketing-in-2026-what-developers-need-to-know
- https://growththerapist.substack.com/p/the-1-tiktok-virality-hack

**Phygital / colaboraciones de moda**
- https://www.modernretail.co/technology/how-ralph-laurens-phygital-boot-sale-in-fortnite-shows-benefits-and-limits-of-virtual-commerce/
- https://www.glossy.co/fashion/digital-fashion-brand-play-pop-go-launches-first-physical-collection
- https://trendhunter.com/trends/need-for-speed
- https://esports.gg/news/pubg-mobile-3/balenciaga-pubg-collaboration-2025
