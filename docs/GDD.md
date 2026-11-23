# GORRAZO · Documento de Diseño del Juego (GDD) v1.0

> **Nombre de desarrollo:** *Boomerang Fighters* (solo interno; nunca en tiendas). **Nombre recomendado:** **GORRAZO by INKCAP**.
> **Fecha:** 23-nov-2026 · **Autor:** diseño principal · **Tipo de cambio:** 1 USD ≈ 19.1 MXN.
> **Base:** el juego de la propuesta B (1.er lugar del juez creativo), el lanzamiento y los modos de C (1.er lugar del juez productor) y la capa de marca de A, simplificada. Construye sobre `prototipo/`, `unity/` y la investigación 00–08.
> **Etiquetas de estado:** **[HECHO]** ya funciona · **[EN CURSO]** ya está en el código esta semana, falta medirlo · **[NUEVO]** hay que construirlo · **[A PRUEBA]** se construye con un interruptor y solo se queda si pasa la prueba con gente · **[verificar]** dato que se confirma antes de gastar.

---

## 0. Ficha del juego

| Campo | Decisión |
|---|---|
| **Nombre** | **GORRAZO**. En tiendas: **"GORRAZO by INKCAP"** (17 de 30 caracteres). Subtítulo de App Store: **"Lanza tu gorra, mancha a todos"** (30 de 30). Grito de KO y nombre del modo principal: **¡GORRAZO!** |
| **Alternativa 1** | **INKAZO** (INK + "-azo"): palabra inventada, el registro de marca más fuerte. Riesgo: al oído suena a "Inca" y quien no sabe inglés no capta el "ink". |
| **Alternativa 2** | **INKCAPS**: mejor como **línea de personajes y art toys** ("colecciona los INKCAPS") que como título. Como título expone la marca de ropa a malas reseñas y en un juego de tinta se acerca a los *Inklings* de *Splatoon*. |
| **Descartados** | CACHUCHAZO (vulgar en el Cono Sur), cualquier "Brawl" o "Clash" (Supercell), SPLAT CAP (Nintendo), NO CAP (genérico; sirve como eslogan). Nunca *Boomerang*, *Fu* ni *Frisbee*. |
| **Género** | *Party brawler* de arena con vista desde arriba: **un golpe = KO**, el arma es tu propia gorra y regresa sola en curva. |
| **Plataformas** | iOS 15+ (Apple permite desde iOS 13; se compila con Xcode 26) y Android 8.0+ (API 26), con *target* API 36 hoy. Para el lanzamiento de 2028 casi seguro se exigirá **API 37 (Android 17)**, que bloquea la red local sin el permiso `ACCESS_LOCAL_NETWORK` [verificar fecha en Google Play]. Juego cruzado iPhone ↔ Android desde el día 1. PC/Steam como opción del año 2. |
| **Jugadores** | 1 contra bots; 2–6 en la arena (local y en línea); La Reta admite 8 en la fila. |
| **Duración** | Ronda de **~13 s** con bots (p90 ~20 s, medido en Unity; con humanos, 15–20 s). Partida **local a 5 puntos ≈ 4 min**; **en línea a 3 ≈ 2–2.5 min**. Sesión: 12–15 min. |
| **Clasificación objetivo** | **Apple 13+** (violencia caricaturesca *frecuente*), **IARC E10+ / PEGI 7**, público **13+** en Google Play. KO de tinta, **nunca sangre**; sin cajas botín ni chat libre. |
| **Motor** | **Unity 6.3 LTS + URP + C#**; como 6.3 tiene soporte hasta dic-2027, **migrar a 6.7 LTS antes del *soft launch*** [verificar fechas]. El núcleo ya existe en C# (simulación determinista, red LAN y **21 pruebas automáticas** que pasan en el último *commit*), el equipo ya publica en Unity para Quest y Unity anunció un plugin oficial para Claude Code [verificar]. Unity Personal es gratis si la empresa que publica factura menos de US$200 mil al año (si no, US$2,310 por puesto al año) [verificar]. Godot no tiene el núcleo hecho; Unreal es pesado para gama baja. |
| **Mercado inicial** | México (≈74 % Android; celulares de referencia: Moto G06 y Galaxy A07), luego LATAM hispana. |

### 0.1 Cómo validar el nombre (antes de pagar logo, dominio o tiendas)

Ninguna búsqueda de conflictos se pudo hacer en esta sesión, así que el riesgo de los tres nombres es una **estimación**. Son unos 30 minutos por nombre y deben hacerse **en este orden**:

| # | Paso | Dónde y qué buscar | Si sale mal |
|---|---|---|---|
| 1 | Marcas en México | **MARCia** (marcia.impi.gob.mx), búsqueda **fonética** de GORRAZO, GORRASO y GORRAZZO en clases **9, 41, 28, 35 y 25** | Marca igual en 9 o 41 → pasar a INKAZO |
| 2 | Marcas internacionales | WIPO Global Brand Database, USPTO y TMview, clases 9 y 41 | Consultar a un agente de marcas |
| 3 | Tiendas | App Store, Google Play, Steam e itch.io: apps con nombre igual o parecido | Apple rechaza nombres confusos |
| 4 | Dominios y redes | gorrazo.com / .mx / .gg / .app (≈ MXN 950–2,900 al año); @gorrazo en TikTok, Instagram, X, YouTube y Discord | Variantes: playgorrazo.com, @gorrazo.inkcap |
| 5 | Connotaciones | Preguntar a gente de Argentina, Colombia, Chile y España ("la gorra" es la policía en Argentina: detalle menor) | Si es grave, INKAZO |
| 6 | Patentes | Google Patents: "thrown hat returns", "throwable cap". Hay arte previo (Kung Lao, Oddjob) | **La gorra nunca tiene ojos ni captura** (distancia de Cappy de *Mario Odyssey*) |
| 7 | Solicitud | **IMPI, "Marca en Línea"**: GORRAZO en clases **9, 41 y 28** e **INKCAP en 9 y 41** (ropa virtual) = 5 clases; **INKCAP en 35** (venta de productos virtuales) es recomendable (+≈ MXN 3,300 + honorarios). ≈ **MXN 16,500** de derechos por las 5 + MXN 2,500–6,000 de honorarios por clase; 4–8 meses. Primero **nominativa**, luego **mixta** (con logo) | 6 meses de prioridad para otros países (Protocolo de Madrid) |

---

## 1. Pitch y gancho

**Gancho:** *"Lanza tu gorra, atrápala de regreso y noquea a tus amigos: cada KO pinta la arena hasta dejarla hecha un mural."*

**Pitch (1 párrafo):** GORRAZO es un *party brawler* para celular de 2 a 6 jugadores. Muñequitos de vinil estilo *art toy* pelean en callejones grafiteados, museos y azoteas de un CDMX de diorama, y el arma de cada uno es **su propia gorra**: la lanzas, vuela en curva y regresa sola. Mientras está en el aire estás desarmado, y todos lo ven porque tu cabeza queda descubierta con los pelos parados. Un golpe noquea: el muñequito se desarma como juguete y deja una mancha de tinta, y al final de la partida la arena queda convertida en un **mural** que compartes por WhatsApp o TikTok. Se juega primero con amigos en el mismo Wi-Fi, sin internet, y después en línea. Lo que te pones son **gorras y playeras reales de INKCAP**, diseñadas por artistas que cobran regalías, y nada de lo que compras te da ventaja.

---

## 2. Pilares de diseño

| Pilar | En una frase | Prueba que debe pasar |
|---|---|---|
| **1. Tu gorra es tu arma** | Quién está armado se lee en 1 segundo, sin interfaz | 9 de cada 10 novatos lo explican solos después de 2 rondas; un clip de 10 s sin sonido se entiende |
| **2. Risa en 15 segundos** | Rondas cortas, KO caricaturesco, siguiente ronda en 2 s | Rondas de 10–20 s; 7 de cada 10 piden "¡otra!" |
| **3. Se juega con la banda** | Wi-Fi local, QR, enlace de WhatsApp, emotes en vez de chat | Un grupo de 4 arma partida en menos de 60 s sin internet |
| **4. Cada KO es arte** | Las 6 técnicas de tinta son el lenguaje visual; la partida termina en mural | 10 % o más de las partidas se comparte |
| **5. Lo que te pones es real** | Merch real de INKCAP con regalías, **jamás** pagar para ganar | Ninguna compra cambia un solo número del combate |

**Fácil de agarrar, difícil de dominar:** 3 botones; la maestría está en la banda, el desvío, el atrape en *dash* y los peligros. Meta: un experto gana ~70 % contra un novato, no 100 %.

### 2.1 Las 8 reglas firma (distancia de *Boomerang Fu*)

Cada mecánica suelta es libre; el riesgo (legal y ante la regla 4.1 de Apple) es que **todo el paquete** se parezca. El juego conserva siempre estas 8 reglas que *Boomerang Fu* no tiene:

1. **Gorra en la cabeza = armado**, y esa gorra es el cosmético que compras.
2. **¡CLANK!:** dos gorras que chocan caen al piso.
3. **El mural:** las manchas se quedan y la partida termina en postal.
4. **Desvío "Te la regreso"** (la gorra desviada vuelve contra su dueño). [A PRUEBA]
5. **Regreso automático** de la gorra caída a los 3 s. [EN CURSO]
6. **Atrapar durante un *dash* recarga el *dash*.** [EN CURSO]
7. **Latas de técnica de tinta** que se pierden con el KO, con efectos propios (Tiraje, Muro de Tag, Sticker XL).
8. **Modos propios:** Lata Caliente, La Reta, Reto del Día, "Levántale la gorra" y Tagueo.

**Diseño limpio (obligatorio):** diario de diseño con fechas en el repositorio (`docs/diario/`); nunca pasar capturas, música ni código de *Boomerang Fu* a la IA; pedir mecánicas con palabras, nunca "que se vea como X".

---

## 3. Ciclos de juego

| Escala | Qué hace el jugador | Qué recibe | Números |
|---|---|---|---|
| **30 segundos** (una ronda) | Moverse → lanzar → esquivar con *dash* → desviar o empujar → atrapar → ¡KO! o ¡CLANK! | *Hit stop*, desarme del juguete, mancha, letrero de KO, cámara lenta final | Ronda ~13 s; primera lata a los 4 s; muerte súbita a los 20 s |
| **1 partida** | 5–8 rondas en línea (a 3 puntos) o ~13 en local (a 5) | **Revelación del mural**, podio con la playera del ganador, XP, Gotas, clip vertical de 6 s y postal | En línea ≈ 2–2.5 min; local ≈ 4 min; ≈ 45 XP |
| **1 día** (12–15 min) | Premio diario → 3 misiones → primera victoria del día → Reto del Día → tienda diaria | Racha, Gotas, Lata de *spray* gratis | ≈ 475 XP de cuenta, ≈ 250 Gotas, ≈ 850 XP de pase |
| **1 semana** | 7 misiones semanales, regla de fiesta de la semana, *drop* de la semana, concurso "Gorrazo de la semana", episodio "La Reta con…" en INKCAP TV | **Gorra o playera de la semana** (día 7), tabla semanal | ≈ 7,000 XP de pase por semanales |
| **1 temporada** (8 semanas) | Una técnica de tinta + un artista invitado: arena **o** modo nuevo, pase de 40 niveles, *drop* físico sincronizado con "Estrenos" | Muñequito de temporada (gratis), playera del artista (premium), traje legendario | Quien juega diario termina el pase en ~6 semanas |

---

## 4. Controles y reglas del combate

### 4.1 Controles

| Acción | Táctil | Control | Teclado (jugador 1 / jugador 2) |
|---|---|---|---|
| Moverse | **Joystick flotante**: aparece donde pones el pulgar en el 45 % izquierdo de la pantalla | Stick izquierdo | WASD / IJKL |
| Apuntar | Arrastrar desde LANZAR (dibuja la curva de ida y vuelta) | Stick derecho | Mouse / dirección de movimiento |
| **Lanzar** | Botón **LANZAR** (96 pt): **toque** = tiro rápido con auto-apuntado; **mantener y arrastrar** = tiro cargado y apuntado a mano | A o RT (mantener carga) | E o clic izquierdo / O |
| Regreso anticipado | Toque a LANZAR con la gorra en el aire | A o RT | E / O |
| Llamar la gorra caída | Mantener LANZAR sin gorra | A o RT | E / O |
| **Golpe** (con gorra) / **Empujón** (sin gorra) | Botón **GOLPE** (72 pt) | X o RB | Q o clic derecho / U |
| **Dash** | Botón **DASH** (72 pt) | B, LB o LT | F o Espacio / H |
| Emote | Botón chico arriba a la derecha → rueda de 4 emotes | Y | 1–4 |

Botones de mínimo 44 pt (iOS) / 48 dp (Android), editables y con modo zurdo; vibración al atrapar y en el KO; cámara un poco arriba del centro para que los pulgares no tapen.

**Estado:** joystick flotante, LANZAR con toque y arrastre, auto-apuntado, GOLPE, DASH, teclado para 2 jugadores y controles **[HECHO]** en el prototipo web (`input.js`) y en el núcleo Unity (`LocalInputReader.cs`). Emotes, botones editables, modo zurdo y vibración **[NUEVO]**.

### 4.2 Reglas exactas y *tuning* actual

Valores de `prototipo/src/config.js` y `unity/.../Core/Tuning.cs` (coinciden en los dos). La arena mide 23 × 14 m (Azotea 22 × 13.5 m) y el muñequito 0.9 m de alto.

| Sistema | Valor actual | Qué significa |
|---|---|---|
| Muñequito | Radio 0.55 m (**igual para todos**); 6.4 m/s; aceleración 70; ×0.55 al cargar | Respuesta inmediata; cargar te expone |
| *Dash* | 18 m/s × 0.15 s (≈ 2.7 m); recarga 0.9 s | Esquiva principal |
| Golpe | Alcance 1.75 m, arco 112°, activo 0.13 s de 0.22 s, recarga 0.45 s | Noquea de cerca y desvía gorras |
| Invulnerable al aparecer | 0.6 s | Sin KOs injustos al iniciar |
| Gorra | Radio **0.38 m (igual para todas las skins)**; vuela a 1.05 m de altura | Choca con lo alto, pasa sobre lo bajo |
| Lanzamiento | 13–22 m/s; carga completa en 0.55 s; soltar antes de 0.12 s = tiro rápido (30 %) | — |
| Ida | 0.34–0.62 s; frena hasta 30 % al final | **Alcance: ≈ 4.3 m (rápido) a ≈ 8.9 m (cargado)** |
| Regreso / llamada | Acelera 45 m/s² hasta 17 m/s, gira 8.5 rad/s / 15 m/s sobre obstáculos | **También noquean** |
| Atrape | Radio 0.95 m; desde 0.12 s después de lanzar | — |
| Auto-apuntado | Cono de 34°, hasta 17 m, **nunca a través de paredes**, con predicción | Se puede apagar |
| Gorra caída | Fricción 6; **regresa sola a los 3 s** | [EN CURSO] |
| Rondas | Cuenta 2.2 s; fin 1.9 s; pausa 0.9 s; cámara lenta 0.9 s al 30 %; *hit stop* ≈ 80 ms | — |
| Latas / láser | Primera a los 4 s, luego cada 6–10 s, máx. 2 en la arena / ciclo 9 s (aviso 1.4 s, activo 1.6 s) | El láser causa ≈ 13 % de los KOs (tope: 15 %) |
| Simulación / red | 60 ticks por segundo; *snapshots* a 30 Hz; entradas de 5 bytes | Determinista |

**Reglas de combate (las que el jugador debe entender):**

1. **Un golpe = KO.** Matan: la gorra en ida, regreso o llamada; el golpe cuerpo a cuerpo; los peligros de la arena.
2. **Tu propia gorra nunca te noquea**, salvo cuando un rival te la desvía ("Te la regreso").
3. Sin gorra en la cabeza **estás desarmado**: no puedes lanzar ni golpear (solo empujar, si pasa la prueba).
4. **¡CLANK!:** dos gorras rivales que chocan caen al piso. Si tu gorra regresa y choca con algo alto, también cae; la recoges caminando sobre ella, la llamas o regresa sola a los 3 s.
5. El golpe cuerpo a cuerpo **desvía** gorras rivales.
6. Gana la ronda el último en pie; gana la partida el primero en llegar a 5 puntos (local) o 3 (en línea).

### 4.3 Cambios que propongo al núcleo (uno por uno, midiendo cada uno con `HeadlessHost balance` y pruebas de sillón)

| # | Cambio | Regla exacta | Estado | Por qué |
|---|---|---|---|---|
| 1 | **Regreso automático** | La gorra en el piso hace *recall* a los 3 s | [EN CURSO] | Corrige la queja principal contra *Boomerang Fu*: quedarse sin arma |
| 2 | **Atrape en *dash*** | Atrapar la gorra durante un *dash* pone su recarga en 0 (¡RECARGA!) | [EN CURSO] | Premia la habilidad sin botón extra. **Reemplaza** la "Encasquetada" de B (chocaba con el regreso anticipado y era frágil con lag) |
| 3 | **Tiraje** (Serigrafía) | 2 copias en fila sobre la misma línea, separadas 0.85 m; no regresan | [EN CURSO] | Se aleja del *Multi Boomerang* |
| 4 | **Doble KO = empate** | Si los últimos caen con menos de 6 ticks (0.1 s) de diferencia, nadie suma | [NUEVO] | Más justo; esconde el lag |
| 5 | **Muerte súbita** | A los 20 s, aviso de 1 s; los muros avanzan 0.5 m/s desde los lados cortos hasta dejar 8 m. **Empujan, no matan**. A los 45 s, empate | [NUEVO] | Corta el ~10 % de rondas largas. **Sin tinta líquida** (ni *Splatoon* ni confusión con el mural) |
| 6 | **Empujón sin gorra** | GOLPE sin gorra: no noquea, empuja 2.5 m en 0.2 s (recarga 0.8 s); patea 4 m una gorra caída ajena | [A PRUEBA], A/B contra "sin golpe al estar desarmado" | El desarmado tiene algo que hacer y los peligros se vuelven jugadas. **No es idea propia** (así funciona *Boomerang Fu*): se conserva por divertido, no como diferenciador |
| 7 | **Desvío "Te la regreso"** | Si la ventana activa del golpe (0.13 s; +0.05 s en línea) toca una gorra rival, sale hacia su dueño a 1.2× (máx. 22 m/s) y **durante 0.6 s puede noquearlo**. El rastro conserva el color del dueño pero se vuelve **rayado** con el de quien desvió. Hoy el desvío solo tira la gorra | [A PRUEBA] | El mejor clip del juego. Entra al MVP **solo si** los bots aprenden a desviar y 7 de 10 probadores lo entienden; si no, en la Temporada 1 |
| 8 | **Sticker XL** / **¡CLANK!** | −10 % de velocidad / radio de choque entre gorras al 85 % | [NUEVO] | Hoy casi no se esquiva / entre bots sale ~3 veces por ronda |
| 9 | **Bots que golpean** | Usan golpe, desvío y empujón | [NUEVO] | Hoy hacen 0 desvíos: sin esto no se balancean los puntos 6 y 7 |
| 10 | **Puntos por modo** | En línea a 3, local a 5 | [NUEVO] | Partidas de bolsillo |
| 11 | Chanfle (curvar la ida al soltar) | — | Después del MVP | Dos pulgares a la vez es difícil en táctil |

**Letreros de KO** (cada clip se explica solo): ¡GORRAZO! (KO normal), ¡DE REGRESO! (con la curva de regreso), ¡DE BANDA! (después de rebotar en pared), ¡TE LA REGRESO!, ¡ZAZ! (golpe), ¡ZAP! (láser), ¡SPLASH! (Charco), ¡EMPATE!

---

## 5. El objeto arrojadizo: la gorra

**Decisión final: la gorra *snapback* del propio muñequito.** Las tres propuestas y los dos jueces la eligieron por unanimidad, con 23 de 25 puntos.

| Alternativa (puntos /25) | Por qué pierde y qué se hace con ella |
|---|---|
| Disco de vinil (17–18) | Delgado y oscuro desde arriba → regla de evento "Sonidero" |
| Rasqueta (15–18) / rodillo (13–17) | Desde arriba es una línea / arma icónica de *Splatoon* → peligros del Taller |
| Disco volador (14–16) | Genérico (*Windjammers*); "Frisbee" es marca de Wham-O → descartado |
| Lata de aerosol (16) | No regresa → **contenedor de los power-ups** |
| Chancla / yo-yo (12–16) | Rompe la silueta / el hilo cambia todo → "Modo Chancla" del 10 de mayo |
| Pelota de tinta (11–13) | No regresa; la usaba *Knockout City* → la lata del modo Lata Caliente |

**Por qué gana:**
1. **Se lee al instante:** gorra puesta = armado; cabeza descubierta con pelos parados = desarmado.
2. **El arma es el producto:** desde arriba, la corona de la gorra ocupa ~47 × 40 px contra ~33 × 17 px del estampado de la playera (≈ 3 veces más). La gorra INKCAP que compraste vuela por la pantalla en cada tiro.
3. **Es literalmente el "CAP" de INKCAP** (INK + CAP = tinta + gorra).
4. **Física creíble:** lanzada de lado gira plana como disco y hace curva.
5. **Distancia total** del bumerán, del "Fu" y de la comida con cara.

**Reglas que no se rompen:**
1. Todas miden lo mismo (radio 0.38) y tienen **la misma silueta en vuelo**; solo cambia la gráfica de **corona y visera** (nada de *bucket* ni visera sola).
2. **Rastro y sombra** siempre del color del dueño; las "estelas" que se venden cambian el **patrón**, nunca el color.
3. El poder de una lata se ve por **efecto y forma**, **nunca por color**.
4. La gorra **no tiene ojos** ni captura a nadie (distancia de Cappy).
5. **Ninguna cabeza de personaje se parece a una gorra.** Prueba: a 64 px, 9 de cada 10 dicen bien si trae gorra.
6. El llavero mide máximo 25 % de la gorra, no cambia el golpe y se puede apagar.
7. Chancla, vinil y otros objetos solo como **reglas de evento iguales para todos**; nunca se venden.

---

## 6. Personajes y personalización

### 6.1 Los 8 muñequitos iniciales

Todos salen de **una misma plataforma de vinil** (2–2.5 cabezas de alto, uniones visibles, manos de mitón), con el mismo radio (0.55) y las mismas estadísticas. Elegir es expresión, no estrategia: la personalidad está en la silueta, las animaciones, el emote y el KO. Los 6 primeros ya existen como "especies" en el prototipo (oso, gato, gota, conejo, alien, diablito).

| # | Muñequito | Técnica (KO por defecto) | Silueta a 64 px y personalidad | Se obtiene |
|---|---|---|---|---|
| 1 | **Lienzo** | Todas (vinil blanco) | Cabeza de gota lisa y uniones marcadas. El novato curioso: la **plataforma** que intervienen los artistas | Gratis |
| 2 | **Osote** (oso) | Grafiti: se evapora y deja su *tag* | Ancho, orejas redondas, respirador de pintor (cuerpo distinto al Be@rbrick). Líder de *crew*; al ganar firma su *tag* | Gratis |
| 3 | **Michi Flash** (gata) | Tatuaje: queda como hoja de *flash* en la pared | Delgada, orejas en punta, cola en "S". Ruda pero tierna | Gratis |
| 4 | **Gotita** (gota) | Tinta derramada: revienta en charco | Lágrima sin orejas, la más distinta. Torpe y alegre. **Mascota del juego** | Nivel 2 |
| 5 | **Seri** (coneja) | Serigrafía: queda estampada | Orejas largas, mandil, registro desfasado. Perfeccionista | Nivel 4 |
| 6 | **Zurci** (alien) | Bordado: se descose en espiral | Antenas-aguja, costuras. Abuelito que remienda todo | Nivel 6 |
| 7 | **Pegote** (diablito) | Sticker: sale volando como calcomanía | Cuernitos, cola con stickers. Travieso | Nivel 10 |
| 8 | **Axo** (ajolote) | Rótulo: letrero "CERRADO POR KO" | Branquias rosas como brochas. Rotulista relajado de Xochimilco | Nivel 15 |

**Después:** **Xolo Rótulo** (xoloitzcuintle) como muñequito gratis del pase de la Temporada 1. **El "pillo" del artista aliado** llega como **invitado de temporada** (T2), con contrato de licencia, regalías, reserva de derechos ante INDAUTOR y su *art toy* físico. **Nunca** en el elenco base, para no depender de un personaje ajeno. Nombre colectivo: no se usa "los Inkcaps" para las criaturas; INKCAPS queda como nombre de la **línea física de art toys**.

### 6.2 Ranuras de personalización

| Ranura | Qué es | Visibilidad en partida | Fuente |
|---|---|---|---|
| **Gorra (= arma)** | Diseño real de INKCAP en corona y visera; misma malla para todas | **Muy alta** (cada tiro) | Tienda, pase, premio diario, canje físico |
| **Playera** | PNG real del catálogo en pecho (y espalda opcional) | Media; **luce en vestidor, podio y repetición** | Tienda, calendario de 28 días, canje físico |
| **Llavero TPU** | *Charm* que cuelga de la gorra y se balancea al volar | Alta | Tienda, niveles |
| **Espalda** | Mochila, lata o llavero gigante | Alta al correr | Pase |
| **Acabado del vinil** | Mate, satinado, brillante, *jelly*, cromado, flocado, brilla en la oscuridad | Alta | Niveles, pase, *drops* de art toy |
| **KO de técnica / estela / emotes** | Se descose o queda estampado / patrón del rastro (nunca su color) / bailes, "firma", "Cafecito" con la taza INKCAP | Alta | Tienda, pase, niveles |
| **Peana y tarjeta de coleccionista** | Base del muñequito en el vestidor | Menús | Logros |

**Kilometraje de gorra** [NUEVO, etapa online]: cada gorra cuenta sus KOs. A los **25 KO** gana una costura dorada en la visera; a los **100**, un contador bordado; a los **500**, un acabado gastado de leyenda. Solo cosmético; hace que la gorra comprada se sienta tuya.

### 6.3 Flujo de arte: "diseño del artista en PNG → textura en el juego"

| Paso | Quién | Qué pasa |
|---|---|---|
| 1. Entrega | Artista / INKCAP | PNG de 4096 px con fondo transparente (el mismo arte de producción física): frente de playera, espalda opcional y gráfica de gorra (corona, frente y visera) |
| 2. Versión legible | Herramienta + artista | Versión de silueta y 2–3 colores que se lea a 33 × 17 px; el artista la aprueba |
| 3. Estampado | Script de Unity "Estampadora" (automático) | Recorta al **área de estampado real** de INKCAP (misma proporción, sin deformar); genera texturas de **512 px** (vestidor, podio) y **128–256 px** (partida) comprimidas en ASTC; crea la variante del *shader* "PlayeraINKCAP" (isla del pecho en UV1) |
| 4. Ficha | Estampadora | `ItemDef`: artista, técnica, rareza, precio en MXN y en TD, SKU, % de regalía y enlace "Consíguela real" con UTM |
| 5. Revisión y aprobación | Diseño + artista | Prueba a 64 px, en grises y con modo daltónico; render firmado por el artista (la ley exige autorizar la adaptación a 3D) |
| 6. Publicación | Producción | Paquete *Addressables* en el CDN, activado por configuración remota el mismo día que el "Estreno" físico, sin actualizar la app |

En partida, cuerpo, playera y gorra van en **un atlas por jugador**: **1 llamada de dibujo por muñequito**. Unos 30–60 min técnicos por diseño [estimación]; llaveros y *art toys* los escanea la empresa 3D del cliente.

---

## 7. Escenarios

Reglas para todas: **un peligro estrella** que avisa al menos 1 s antes (luz + sonido + ícono con forma), **piso calmado** (el grafiti va en los muros), muros cercanos a la cámara de máximo 1.5 alturas del muñequito, y una **muerte súbita propia** sin líquido.

| # | Arena | Peligro estrella | Mecánica secundaria y muerte súbita | Cuándo |
|---|---|---|---|---|
| 1 | **Callejón Grafiti** (Grafiti) | **Camión de basura** que cruza un carril cada ~25 s (claxon 1.5 s antes); tocarlo = KO | Charcos de pintura fresca que alentan al 50 % (el efecto que antes tenía la lata Grafiti). Bajan las **cortinas metálicas** | **Existe** (sin camión) · MVP |
| 2 | **Museo de Arte** (Serigrafía) | **Láseres de seguridad** (ciclo 9 s) | Vitrina con *art toys* reales de INKCAP. Suena la **alarma** y bajan las **rejas** | **Existe** (con láseres) · MVP |
| 3 | **Azotea CDMX** (Tinta derramada) | **Bordes sin barandal**: un empujón te tira | **Tinacos** que, con 2 golpes, sueltan un chorro que empuja; tendederos con playeras INKCAP que atoran la gorra 1 s. Avanzan **vallas de obra** | **Existe** (tinacos como obstáculos) · MVP |
| 4 | **Pop-up Store INKCAP** (Sticker) | **Caja "DROP"**: cada 20 s cae del techo (su sombra avisa 1.5 s) y suelta una lata; si te cae encima = KO | Racks giratorios; el aparador muestra el *drop* real de la semana. Baja la cortina **"CERRADO"** | Lanzamiento público |
| 5 | **Skatepark con Mural** (Grafiti) | ***Bowl***: corres más rápido y las gorras siguen la curva | Rampas que hacen saltar gorras; muros para Tagueo | Temporada 1 |
| 6 | **Estudio de Tatuaje** | **Apagón de neón**: cada 15 s, 3 s de oscuridad; solo brillan aros, gorras y rastros | Sillones giratorios como cobertura | Temporada 2 |
| 7 | **Taller de Serigrafía** | **Pulpo giratorio** de 6 brazos (una vuelta cada 12 s) que bloquea gorras y empuja | Mallas que deja pasar la gorra pero no a los jugadores; rasqueta gigante | Temporada 3 |
| 8 | **Andén "Línea Tinta"** (Metro ficticio, sin logos ni pictogramas reales) | **Tren** cada 20 s (aviso 2 s) | El tren **se lleva las gorras** y las suelta al otro extremo | Temporada 4 |
| 9 | **Set de INKCAP TV** | **Reflector "EN VIVO"** que sigue al líder; noquearlo bajo la luz da +1 punto | Cámaras de utilería como cobertura | Evento |
| 10 | **Tianguis de Domingo** | **Lonas rosas** que tapan la vista; la gorra las rasga o rebota con +30 % de velocidad | Diablitos de carga | Temporada 5 |
| 11 | **Galería en Inauguración** | **Paredes móviles** que reacomodan la sala cada ronda | Inflables que empujan | Temporada 6 |

**Eventos:** Panteón del Arte (Día de Muertos), Sonidero de Barrio (bocinas que empujan al ritmo; requiere música comisionada) y Lucha Libre (las cuerdas rebotan la gorra).

**El MVP usa las 3 arenas que ya existen**, no una cuarta: cada arena nueva cuesta US$1.5–2.5 mil más integración, y en el MVP se prueba la diversión. Solo les faltan sus peligros estrella (camión, chorro del tinaco) y el arte final.

---

## 8. Modos de juego

| Modo | Jugadores | Cómo se gana | Por qué es divertido | Cuándo |
|---|---|---|---|---|
| **¡Gorrazo!** (Último en Pie) | 2–6 | Último vivo gana la ronda; primero a 5 (local) o 3 (en línea) | El núcleo puro | **MVP** [HECHO en el núcleo] |
| **Lata Caliente** | 3–6 | Una lata a punto de explotar va pegada a alguien; tu gorrazo **no noquea: le pasa la lata**. Explota entre los 10 y 20 s (pitido que acelera) y ese jugador queda fuera. Último en pie gana | Gritos garantizados; ideal para niños, fiestas y eventos | **MVP** [NUEVO] |
| **La Reta** | 2–8 | 1 contra 1: **el que gana se queda**; los demás hacen fila y mandan emotes. Gana quien encadene más victorias | Cultura de barrio; perfecto para *streamers* contra su público ("Sala de streamer") | **MVP** [NUEVO] |
| **Reto del Día** | 1 contra bots | Misma arena, semilla y regla para todo el país; tabla nacional por puntaje. El servidor **re-simula** la partida con las entradas del jugador para detectar trampas (posible gracias al núcleo determinista) | Hábito diario estilo Wordle; se juega sin internet | **MVP** (la tabla nacional llega con el online) |
| **Crews** | 2v2 / 3v3 | Último equipo en pie. **"Levántale la gorra":** llevas la gorra de tu compa caído a su mancha en menos de 4 s y revive (1 vez por ronda). Respaldo si confunde: **"Retoque"** (pararse 2 s sobre la mancha) | Jugadas de héroe que solo existen porque el arma es una gorra | Lanzamiento público |
| **Tagueo** | 2v2 / 3v3 | 5 muros en blanco; 3 gorrazos a un muro lo vuelven de tu equipo; cada segundo con muros suma puntos. Con reaparición | Control de zona **en muros, no en el piso** (lejos de *Splatoon*) | Temporada 1 |
| **Coleccionista** | 3v3 | Cada KO suelta un pin; llevarlo a tu vitrina lo asegura; gana el primero con 15 | Reaparición rápida | Etapa online |
| **Duelo** | 1v1 | Al mejor de 7 en servidor dedicado; divisiones Boceto → Línea → Color → Sombra → Obra Maestra | Base del *ranked* | Año 2 |

**Para eventos y después del MVP:** El Velador (1 velador con linterna contra 4 grafiteros), Atraco al Museo y **Pantallota / Gorrazo Fest** (la partida en una pantalla grande y los celulares como controles, para tiendas y eventos de arte urbano).

**Reglas de fiesta** (salas privadas y "regla de la semana" en línea; baratas de producir): **Una Pa' Todos** (una sola gorra que acelera 10 % con cada atrape), **Doble lata** (combos), **Latas persistentes**, **Puro Sticker XL**, **Gorras pesadas** (×0.7), **Turbo** (×1.3), **Sin dash**, **Fuego amigo**, **Modo Chancla** (10 de mayo, misma física para todos).

**Regla:** todos los modos, arenas y reglas de fiesta son **gratis para todos**, en local y en línea. Nunca se venden ni se encierran en el pase: dividirían a la comunidad y romperían el pilar 5.

**Descartados:** "Edición Limitada" de A (igual al *Golden Boomerang* de *Boomerang Fu*), la "Inundación de tinta" de B y la "Caza del Drop" con premio físico (posible permiso de sorteos).

---

## 9. Power-ups: las latas de técnica

**Reglas:** **1 lata a la vez** (la nueva reemplaza a la anterior), **se pierde con el KO** y en línea se reinician al empezar cada ronda; el Parche se gasta al bloquear. Elegí 1 lata (C y juez productor) y no 2 (A, B y juez creativo) porque en 6" cada estado extra cuesta lectura y balance; los combos viven como regla de fiesta ("Doble lata", "Latas persistentes").

**Lectura:** las latas son **negro tinta y blanco hueso con un ícono grande con forma**, nunca de los colores de jugador (hoy Serigrafía rosa, Grafiti verde y Parche ámbar chocan). El poder activo se ve por **efecto** en la gorra y como ícono en el aro.

| Lata | Efecto final | Cómo se ve / contra | Estado |
|---|---|---|---|
| **Serigrafía · Tiraje** | 2 copias siguen tu trayecto en fila (0.85 m); no regresan | Copias semitransparentes con registro desfasado / desviar la real borra las copias | [EN CURSO] |
| **Tatuaje · Trazo** | Curva hacia el rival más cercano (4.5 rad/s, hasta 11 m); **marca** al objetivo con un ícono de *flash* 0.5 s antes | Línea punteada / *dash* lateral tardío | [HECHO] + aviso [NUEVO] |
| **Tinta derramada · Charco** | Al chocar deja un charco que **crece 0.4 s visibles** hasta 2.0 m y noquea a quien siga dentro (hoy explota al instante, 2.3 m) | La gorra gotea / salir del círculo | [HECHO] + ajuste [NUEVO] |
| **Grafiti · Muro de Tag** | Al final de la ida pinta un muro de esténcil de 2.5 × 0.4 m que **bloquea gorras y jugadores 3 s** (reemplaza el rastro que alentaba, parecido al *Ice Boomerang*) | Estela de aerosol gruesa / rodearlo | [A PRUEBA]; respaldo: rastro actual |
| **Parche bordado · Escudo** | Aguanta 1 golpe y se descose. Variante a probar: **"Te la coso"** (la gorra que te pega queda cosida 2 s y su dueño sigue desarmado) | Parche en el pecho / dos golpes seguidos | [HECHO] |
| **Sticker XL** | Gorra al doble de tamaño, −10 % de velocidad | Borde blanco troquelado / esquivar temprano | [HECHO] + ajuste [NUEVO] |

Así, 4 de 6 latas tienen efecto propio. **Nunca** usar "no se puede desviar" (*Unstoppable*) ni "copia inmóvil" (*Decoy*). **Latas de temporada:** una por artista invitado, gratis para todos (ej. **Neón**: rebota 3 veces antes de regresar).

---

## 10. Progresión

### 10.1 XP por acción (XP de cuenta)

| Acción | XP |
|---|---|
| Terminar una partida | 20 |
| Cada KO (máximo 5 por partida) | 5 |
| 1.º / 2.º / 3.er lugar (4 o más jugadores) | +25 / +15 / +10 |
| Jugar con tu *crew* (Wi-Fi local o amigos) | +5 |
| **Primera victoria del día** | **+100** (y 50 Gotas) |
| Misión diaria (3 al día) | 50 cada una |
| Terminar el tutorial (una vez) | **200** |
| Bienvenida: tus 3 primeras partidas de la vida | ×2 |
| Bono de regreso: si no jugaste ayer, tus 3 primeras partidas | ×2 |

Partida típica ≈ 45 XP. **Día típico (5 partidas, 15 min) ≈ 475 XP.** Con el tutorial, la bienvenida y la primera victoria, la **primera sesión llega al nivel 4** (meta de C para el D1).

### 10.2 Tabla de niveles 1–50

Fórmula: **XP para pasar del nivel *n* al *n*+1 = 150 + 50 × (*n* − 1)** (el prototipo usa hoy 100 + 60 × (*n* − 1); hay que cambiarlo). "Días" = días jugados a 475 XP diarios, sin contar el impulso de la primera sesión. Cada 5 niveles se dan 25 Tinta Dorada (TD); en los niveles 30, 40 y 50, más.

| Nivel | XP acumulada | Días | Recompensa | | Nivel | XP acumulada | Días | Recompensa |
|---|---|---|---|---|---|---|---|---|
| 1 | 0 | 0 | Lienzo, Osote, Michi Flash, gorra "Logo INKCAP" blanca, playera básica | | 26 | 18,750 | 39 | 500 Gotas |
| 2 | 150 | 1.ª sesión | **Gotita** | | 27 | 20,150 | 42 | Emote |
| 3 | 350 | 1.ª sesión | Sticker "Tag" para la gorra + aviso "Protege tu progreso" | | 28 | 21,600 | 45 | Lata de *spray* |
| 4 | 600 | 1.ª sesión | **Seri** + 200 Gotas | | 29 | 23,100 | 49 | 500 Gotas |
| 5 | 900 | 2 | Emote "Saludo" + 25 TD | | 30 | 24,650 | 52 | **Traje "Veterano de Tinta"** (gorra + playera) + 50 TD |
| 6 | 1,250 | 3 | **Zurci** | | 31 | 26,250 | 55 | 600 Gotas |
| 7 | 1,650 | 3 | Gorra "Básica Negra" | | 32 | 27,900 | 59 | Sticker dorado |
| 8 | 2,100 | 4 | Estela "Aerosol" (patrón) | | 33 | 29,600 | 62 | Lata de *spray* |
| 9 | 2,600 | 5 | 300 Gotas | | 34 | 31,350 | 66 | 600 Gotas |
| 10 | 3,150 | 7 | **Pegote** + misiones semanales + llavero "Gotita" + 25 TD | | 35 | 33,150 | 70 | Acabado **Satinado** + 25 TD |
| 11 | 3,750 | 8 | 300 Gotas | | 36 | 35,000 | 74 | 700 Gotas |
| 12 | 4,400 | 9 | Marco de perfil | | 37 | 36,900 | 78 | Emote |
| 13 | 5,100 | 11 | Emote "Cafecito" (taza INKCAP) | | 38 | 38,850 | 82 | Lata de *spray* |
| 14 | 5,850 | 12 | 400 Gotas | | 39 | 40,850 | 86 | 700 Gotas |
| 15 | 6,650 | 14 | **Axo** + 25 TD | | 40 | 42,900 | 90 | KO de técnica "Estampado" + 50 TD |
| 16 | 7,500 | 16 | Sticker | | 41 | 45,000 | 95 | 800 Gotas |
| 17 | 8,400 | 18 | 400 Gotas | | 42 | 47,150 | 99 | Peana "Galería" |
| 18 | 9,350 | 20 | Emote | | 43 | 49,350 | 104 | Lata de *spray* |
| 19 | 10,350 | 22 | Lata de *spray* | | 44 | 51,600 | 109 | 800 Gotas |
| 20 | 11,400 | 24 | **Playera "Logo INKCAP"** (diseño real) + 25 TD | | 45 | 53,900 | 114 | Llavero "Gorrazo" dorado + 25 TD |
| 21 | 12,500 | 26 | 500 Gotas | | 46 | 56,250 | 118 | 900 Gotas |
| 22 | 13,650 | 29 | KO de técnica "Se descose" | | 47 | 58,650 | 124 | Emote |
| 23 | 14,850 | 31 | Estela "Semitono" | | 48 | 61,100 | 129 | Lata de *spray* |
| 24 | 16,100 | 34 | Lata de *spray* | | 49 | 63,600 | 134 | 900 Gotas |
| 25 | 17,400 | 37 | Gorra "Tinta derramada" + peana + 25 TD | | 50 | 66,150 | 139 | Acabado **Cromado** (legendario) + título "Leyenda de Tinta" + 100 TD |

Después del 50: **Maestría**, cada 2,500 XP da 100 Gotas y una estrella en el perfil. Ritmo: un premio por nivel la primera semana, nivel 10 en ~1 semana, 20 en ~1 mes, 30 en ~2 meses y 50 en ~5–6 meses.

### 10.3 Premio diario y racha

| Día | 1 | 2 | 3 | 4 | 5 | 6 | **7** |
|---|---|---|---|---|---|---|---|
| Premio | 150 Gotas | Sticker para la gorra | 250 Gotas | 25 TD | Emote | Lata de *spray* | **Gorra de la semana** (semana 1: gorra INKCAP "Tinta derramada"). Desde la semana 2 se alterna gorra y playera |

- **Cambio frente al prototipo:** hoy el día 7 da la playera "Tinta derramada". Pasa a dar **gorra**, porque en partida se ve ~3 veces más. La playera pasa al calendario de 28 días.
- **Calendario de 28 días:** al completarlo, **"Playera del mes"**, un diseño real de INKCAP, gratis.
- **Racha suave:** si faltas, el calendario **se pausa**, no regresa al día 1. Aparte, cada día seguido con al menos 1 partida da +10 % de Gotas (máximo +50 %), con **1 protector gratis por semana** e insignias a los 7, 30 y 100 días.

### 10.4 Misiones

- **Diarias:** 3 al azar (1 cambio gratis); cada una da 50 XP, 200 XP de pase y 30 Gotas. Ej.: "Haz 3 KO ¡DE REGRESO!", "Atrapa tu gorra 10 veces", "Provoca un ¡CLANK!", "Recarga tu *dash* atrapando 3 veces", "Gana 1 Lata Caliente".
- **Semanales:** 7, desde el nivel 10; cada una da 1,000 XP de pase y 150 Gotas y **no caduca hasta el fin de temporada**. Ej.: "Gana 7 partidas", "Juega en las 3 arenas", "Juega 5 partidas con tu *crew*".
- **Reto del artista:** 5 por temporada, presentados en INKCAP TV (premio: sticker y marco "Firma del artista").

### 10.5 Logros: el Álbum de Stickers

Como un álbum de estampas del Mundial: **6 páginas (una por técnica) × 12 stickers = 72**. Cada sticker se gana con una hazaña y **no se vende ni se intercambia**. Página completa = gorra de logro; álbum completo = traje "Obra Completa". Ejemplos: "¡CLANK! ×100", "Muralista" (1,000 manchas), "Rey de la Reta" (5 seguidas), "De Banda ×25", "Te la regreso ×10", "Coleccionista" (KO con las 6 latas), "Pasarela" (usó *Pruébatela* con 10 trajes distintos). **Ningún sticker del álbum exige comprar nada**, ni en la app ni fuera: se quitó el sticker "Phygital" (canjear una prenda física), porque un logro exclusivo de una compra hecha fuera de la app choca con la regla 3.1.3(b) de Apple (mismo motivo por el que se descartó la "estampa dorada"). **"Crew Fundadora"** (jugó la beta local) es un sticker **de bonificación**, fuera de los 72, para que quien llegue después también pueda completar el álbum.

### 10.6 Pase de temporada (8 semanas, 40 niveles)

- **Precio:** premium **$89 MXN** como compra directa (o 500 TD); "+10 niveles" $149. Prueba A/B de $79 contra $99 cuando haya ≥ 4,000 jugadores nuevos por variante.
- **Candado de cajas botín:** los niveles que se **compran** ("+10 niveles") nunca entregan Latas de *spray*; si un nivel comprado trae una, se cambia por 150 Gotas. Las Latas solo se ganan jugando (investigación 07 §5.2: no se pueden comprar ni directa ni indirectamente).
- **Costo:** 2,000 XP de pase por nivel (80,000 en total). **Fuentes:** 20 por partida, 150 por la primera victoria del día, 200 por misión diaria, 1,000 por misión semanal.
- **Ritmo:** quien juega diario gana ≈ 12,950 a la semana y termina en **~6 semanas**; quien juega 4 días y hace 5 semanales llega al **nivel ~33**; quien juega 2 días, al ~18. Después del 40, cada 2,500 XP dan 100 Gotas.

| Nivel | Vía gratis | Vía premium |
|---|---|---|
| 1 | 100 Gotas | **Traje de temporada** (versión base) |
| 5 | Sticker | Emote de temporada |
| 10 | 10 TD | Estela de temporada (patrón) |
| 12 | 200 Gotas | **Gorra edición temporada** |
| 15 | Lata de *spray* | 100 TD |
| 20 | **Muñequito de temporada** (T1: Xolo Rótulo) | Llavero de temporada |
| 25 | 200 Gotas | **Playera del artista invitado** (diseño real) |
| 30 | 15 TD | KO de técnica de temporada + 100 TD |
| 35 | Lata de *spray* | Emote "Firma" |
| 40 | Insignia de temporada + 25 TD | **Traje legendario** + acabado de temporada + 100 TD |
| Niveles intermedios | 100 Gotas o sticker | 150 Gotas o sticker animado |

La premium devuelve **≈ 300 TD** (≈ 60 % del precio).

---

## 11. Economía

### 11.1 Monedas

| Moneda | Cómo se gana | En qué se gasta |
|---|---|---|
| **Gotas** (blanda) | Jugando (10–25 por partida, 50 por primera victoria, misiones, racha, niveles): **≈ 250 al día** | Tienda diaria: stickers 300, emotes 800, gorras comunes 1,500, playeras genéricas 3,000, peanas 500, muñequito atrasado 6,000 |
| **Tinta Dorada (TD)** (dura) | Comprándola; goteo gratis ≈ 150 al mes (niveles, calendario, pase gratis) | Pase, Destacados, gorras y playeras de artista, emotes, estelas |
| **XP de pase** | Partidas y misiones | Niveles del pase; no se compra suelta |

**Reglas:** la TD **no caduca** y muestra su **equivalente en pesos**; los duplicados se vuelven Gotas; el pase y los artículos de artista también se venden **en pesos directos**, para que no sobren monedas.

**Pago con OXXO** (desde la etapa online): los mismos artículos y paquetes se pueden pagar también en la web de INKCAP, con OXXO o tarjeta, y se acreditan al ID de jugador; todos existen además como compra dentro de la app (regla 3.1.3(b)). Esa web **solo se promociona fuera del juego** (redes, INKCAP TV, bolsas de envío), nunca con un botón o texto dentro de la app: en México está prohibido dirigir a otro método de pago. La proyección del plan supone que 12.5 % de las ventas digitales pasan por ahí. La tienda web completa con bonos (estilo Supercell Store) queda para el año 3.

**Paquetes de TD:** $19 → 100 · $99 → 550 · $199 → 1,200 · $499 → 3,250 (confirmar puntos de precio en App Store Connect). Valor neto promedio ≈ **$0.13 MXN por TD**.

### 11.2 Precios en MXN (con IVA)

| Producto | Precio directo | En TD |
|---|---|---|
| **Kit de Barrio** (una sola vez): gorra "Logo INKCAP De Barrio" + playera "De Barrio" (distintas de la gorra y la playera básicas que se regalan en el nivel 1) + 150 TD | **$19** | — |
| Sticker para la gorra | $10–19 | 60–100 |
| Llavero, emote o estela | $19–29 | 100–160 |
| Gorra o playera común | $29–39 | 160–220 |
| **Gorra de artista INKCAP** | **$59–69** | 330–380 |
| **Playera de artista INKCAP** | **$59–79** | 330–440 |
| KO de técnica | $49 | 270 |
| Acabado de coleccionista (regresa en 6–12 meses) | $99 | 550 |
| **Drop completo** (gorra + playera + estela + emote) | **$129–149** | 720–830 |
| Set de Edición Limitada | hasta $199 | — |
| **Pase de temporada** | **$89** ($149 con +10 niveles) | 500 |
| Club INKCAP (año 2; pase incluido + 1 gorra al mes) | $59 al mes | — |

### 11.3 Fuentes y sumideros (jugador típico, 1 mes)

**Entra:** ≈ 8,000 Gotas (partidas 2,250 + primeras victorias 1,500 + misiones 3,500 + racha, calendario y niveles ≈ 800) y ≈ 150 TD. **Sale:** 2 gorras comunes (3,000) + 1 playera genérica (3,000) + stickers y emotes (1,500). Un jugador gratuito compra 2–3 cosméticos comunes al mes y junta para un pase cada ~3 meses. Toda la economía va en **configuración remota** para calibrarla con datos.

### 11.4 Tienda rotativa y *drops*

| Sección | Rotación | Moneda | Contenido |
|---|---|---|---|
| **Diaria** | 4 espacios cada 24 h | Gotas | Comunes, stickers, emotes |
| **Destacados** | 2 espacios cada 48 h | TD | Estelas, llaveros, KO |
| **Drop INKCAP** | Semanal, el mismo día que "Estrenos" | Pesos o TD | Gorra y playera de artista con cuenta regresiva y botón "Consíguela real" |
| **Edición Limitada** | Por evento, 7 días | Pesos | Set del artista invitado; **puede regresar en 6–12 meses con otro acabado** (sin escasez falsa). Lo único escaso de verdad es la **pieza física numerada** |

**Votación de drop:** cada temporada se vota entre 3 diseños digitales y el ganador se fabrica en físico en preventa (Forever 21 vendió un gorro virtual 1.5 M de veces antes de fabricarlo).

### 11.5 Phygital (de ida y vuelta)

**De la etiqueta al traje:**
1. La prenda física trae un **código único para raspar**, de un solo uso y ligado a lote y artista (≈ MXN 1–3 por etiqueta; con OXXO se envía al confirmarse el pago).
2. El QR abre **la web de INKCAP** (nunca una casilla dentro de la app de iPhone, regla 3.1.1): se escribe el código y el **ID de jugador** del Perfil (ej. `GZ-4821-KQ`) y el artículo aparece en el inventario. Antifraude: límite de intentos y códigos invalidados si se devuelve la prenda. Portal: US$2–5 mil [estimación].
3. Ese artículo **también se vende dentro del juego** (regla 3.1.3(b)). Una variante "Edición Física" exclusiva solo después de consultarlo con App Review. Para campañas: Offer Codes de Apple y *promo codes* de Google.
4. **Cambio obligatorio en el prototipo:** quitar la casilla de códigos del vestidor (`ui.js`, `progress.js`) y poner a la venta la playera que hoy es "solo con código".

**Del traje a la tienda:** botón **"Consíguela real"** hacia una página **solo de productos físicos** (sin comisión, regla 3.1.3(e)) con UTM por traje; **tener la gorra digital da un cupón de 10 %** para la gorra física (30 días) [verificar con el contador]; **"Pruébatela"** (1 partida gratis con el traje; con anuncio opcional de 24 h en la etapa online); y **postal del mural** con QR de descarga.

### 11.6 Regalías a artistas

**15 % del neto** (después de IVA y de la comisión de la tienda, 15 % con el programa para pequeños desarrolladores), igual que en la merch, con reporte trimestral por producto.

| Venta | Precio | Sin IVA | − Comisión 15 % | Neto | **Artista (15 %)** | **INKCAP** (antes de ISR) |
|---|---|---|---|---|---|---|
| Gorra de artista | $59 | $50.86 | $7.63 | $43.23 | **$6.48** | **$36.75** |
| Playera de artista | $79 | $68.10 | $10.22 | $57.89 | **$8.68** | **$49.20** |
| Pase (si incluye al artista invitado) | $89 | $76.72 | $11.51 | $65.22 | **$9.78** | **$55.43** |
| Drop completo | $149 | $128.45 | $19.27 | $109.18 | **$16.38** | **$92.80** |

- **Compras con TD:** regalía = 15 % × TD gastada × valor neto promedio de la TD pagada del trimestre (≈ $0.13). Ej.: playera de 440 TD → ≈ $57 de neto → ≈ $8.60 al artista (se paga igual aunque la TD sea regalada: simple y generoso).
- **Pase y paquetes:** se reparte según los artículos de cada artista dentro del paquete.
- **Código de creador:** 5 % extra del neto de lo que gasten quienes usen el código del artista o de INKCAP TV.
- **Ojo:** si hoy INKCAP paga 15 % sobre el **precio de venta** físico, el 15 % del **neto** digital le paga al artista **entre 15 % y 27 % menos por peso** (15 % si hoy se calcula sobre el precio sin IVA; 27 % si es sobre el precio con IVA). Acordarlo por escrito, o igualarlo pagando ≈ 17.7 % del neto (base sin IVA) o ≈ 20.5 % (base con IVA).
- **Contrato digital:** licencia escrita, onerosa y temporal; autorización de adaptación a 3D; crédito; qué pasa con lo ya vendido si termina; prohibido entrenar IA con su arte.

### 11.7 Lo que NO vamos a hacer

Pagar para ganar · cajas botín pagadas o ruletas (las Latas de *spray* son gratis, muestran probabilidades —60 % Gotas, 30 % sticker, 8 % emote, 2 % traje— y garantizan traje a las 25) · energía o esperas · escasez falsa · más de 1 oferta por sesión (ninguna en la primera) · premio por aceptar notificaciones o el rastreo (ATT) · anuncios intersticiales o en partidas locales · anuncios para menores de 13 · anuncios personalizados para menores de 18 · premios aleatorios con valor fuera del juego (merch física al azar = posible sorteo) · NFT.

**Anuncios recompensados:** solo desde la etapa online, opcionales, máximo 5 al día (duplicar Gotas, misión extra, recuperar racha 1 vez por semana, probador de 24 h). Con 1,000 jugadores diarios dejan ≈ MXN 2,600 al mes: ayudan, no sostienen.

---

## 12. Flujos de pantallas y textos clave

### 12.1 Flujos

| Flujo | Pasos |
|---|---|
| **Primera vez** | Abrir app → edad neutral (año de nacimiento) → **tutorial contra 2 bots fáciles en menos de 30 s, sin cuenta**, con 3 avisos jugados → primera victoria → revelación del mural → elegir 1 de 3 muñequitos → menú. Permiso de notificaciones **después de la primera victoria y sin premio**; "Protege tu progreso" (ligar Apple/Google) en el nivel 3 |
| **Menú principal** | JUGAR (Partida rápida · **Fiesta Wi-Fi** · En línea) · RETO DEL DÍA · VESTIDOR · TIENDA · PASE · punto rojo de premio diario · perfil (ID de jugador) · ajustes |
| **Sala local Wi-Fi** | **Crear:** explicación del permiso de "Red local" → el anfitrión ve **QR + código de 4 dígitos + nombre de sala** → elige modo, arena, puntos y "Rellenar con bots". **Unirse:** salas cercanas automáticas (Bonjour) · "Escanear QR" · "Escribir IP". Hasta 6 con color y forma → **¡A JUGAR!** Si no aparece nada: guía de *hotspot* |
| **Sala en línea** | Partida rápida (a los 8 s se rellena con bots) · sala con código → **"Invitar por WhatsApp"** |
| **Partida** | Marcador con aro, forma y puntos de cada jugador · joystick y botones · ícono de la lata sobre LANZAR · emotes · pausa (solo anfitrión local) |
| **Resultados** | Revelación del mural (mate → a todo color, 2 s) con la firma del ganador → podio con su playera y la taza trofeo → XP, Gotas, misiones → REVANCHA · COMPARTIR MURAL · COMPARTIR CLIP (vertical, 6 s) · SALIR |
| **Vestidor** | Muñequito en su peana · pestañas Gorra / Playera / Llavero / Espalda / Acabado / KO / Estela / Emotes · ficha con artista, Kilometraje, "Pruébatela" y "Consíguela real" |
| **Tienda / Pase** | Drop de la semana con cuenta regresiva · Diaria · Destacados · Paquetes de TD · Kit de Barrio / carril de 40 niveles gratis y premium. Precio en pesos siempre visible |

### 12.2 Textos clave de la interfaz

| Lugar | Texto |
|---|---|
| Tutorial | "Toca LANZAR." · "¡Tu gorra regresa! Atrápala." · "Sin gorra estás desarmado. ¡Corre por ella!" |
| KO | ¡GORRAZO! · ¡DE REGRESO! · ¡DE BANDA! · ¡TE LA REGRESO! · ¡ZAZ! · ¡ZAP! · ¡SPLASH! |
| Eventos de ronda | ¡CLANK! · ¡RECARGA! · ¡MUERTE SÚBITA! · ¡EMPATE! · ¡BLOQUEADO! · "SIN GORRA" |
| Sala | "Crea tu sala y que se unan con el QR." · "¿No aparece? Prendan el hotspot de un celular y conéctense todos." |
| Permiso de red local | "Para encontrar a tus amigos en el mismo Wi-Fi, GORRAZO necesita ver tu red local. No usamos tus datos para nada más." |
| Invitación | "Únete a mi reta en GORRAZO: 4821" |
| Notificaciones (máx. 1 al día, nunca de 22:00 a 9:00) | "Tu premio diario está listo." · "Tu racha está por pausarse." · "Nuevo drop: ya salió la gorra de la semana." (este último es **promocional**: solo para quien aceptó recibir promociones con un texto claro dentro de la app, con opción de darse de baja en Ajustes, regla 4.5.4 de Apple; nunca a menores en modo protegido) |
| Vestidor | "Consíguela real" · "Pruébatela" · "Diseño de [artista] · INKCAP" |
| Canje | "¿Tienes una prenda INKCAP? Canjea tu código en inkcap.[dominio]/gorrazo con tu ID de jugador." Este texto va en la **etiqueta física, el QR y los canales de INKCAP**. Dentro de la app de iPhone no hay casilla ni enlace: solo el ID de jugador en el Perfil; cualquier texto informativo se consulta antes con App Review |

---

## 13. Dirección de arte y audio

**Frase guía:** *"Juguetes de vinil peleando en un CDMX de diorama. El piso calmado, los muros gritan."*

**Proporciones:** muñequito de 0.9 m con 2–2.5 cabezas (cabeza = 45–50 % de la altura), cuerpo de frijol corto sin cuello, manos de mitón, ojos grandes como textura intercambiable. Gorra 1.5–2 veces su tamaño "realista". Cámara inclinada ~60° que muestra 14–20 m de arena.

**Paleta:**

| Rol | Colores | % de pantalla |
|---|---|---|
| Neutros del piso | Asfalto `#4A4E57`, concreto `#8E9196`, loseta `#B9B3A8` | 60 % |
| Negro tinta / blanco hueso | `#16161C` / `#F2EDE4` | Contornos, interfaz, latas |
| Acentos CDMX (muros, lonas, letreros) | Rosa mexicano `#E4007C`, naranja `#F28C28`, amarillo rótulo `#FFD23F`, añil `#2B59C3`, turquesa `#1FB5AD`, nopal `#3E8E41` | 30 % |
| **Jugadores (Okabe-Ito, reservados)** | `#E69F00`, `#56B4E9`, `#F0E442`, `#CC79A7`, `#009E73`, `#0072B2` (+ `#D55E00` y blanco para salas de 8). Formas: círculo, triángulo, cuadro, estrella, rombo, hexágono. Equipos: azul `#0072B2` contra naranja `#E69F00` | 10 % (aros, rastros, tinta de KO, íconos) [EN CURSO en el prototipo] |
| Peligro | Rojo láser `#FF2D2D` con parpadeo + ícono de forma | — |

**Materiales:** personajes con *matcap* de vinil satinado y contorno delgado (nada de PBR); escenario con atlas de paleta y luz horneada; rarezas = acabados de art toy. Tinta del piso **mate y desaturada al 40 % durante la partida**, con textura de su técnica, y **a todo color en la revelación del mural**; lo que hace daño va animado, con borde brillante y aviso.

**Presupuesto para gama baja** (Moto G06 / Galaxy A07):

| Elemento | Presupuesto |
|---|---|
| Muñequito completo (cuerpo + playera) | 2,500–4,000 triángulos (meta 3,000); ~20–25 huesos [estimación] |
| Gorra / llavero / espalda | 300–500 / ≤ 150 / ≤ 400 triángulos |
| Texturas del personaje | Atlas de 512 px por jugador (128–256 px para la playera en partida) |
| Arena visible | 30,000–60,000 triángulos [estimación]; atlas de paleta de 256 px + murales en *trim sheets* de 1024–2048 px |
| Pintura del piso | Una sola *RenderTexture* de 512 px (gama baja) o 1024 px (media) |
| Llamadas de dibujo / partículas | ≤ 80–120 / ≤ 150 vivas, opacas o con recorte |
| Rendimiento | **30 fps estables** en gama baja (60 opcional en gama media); descarga inicial **< 150 MB** |
| Datos por partida en línea | ≈ 1.2 MB por partida de 3 min (anfitrión ≈ 6 MB) |

**Animaciones por muñequito:** reposo, correr, cargar, lanzar (2–3 cuadros de anticipación), atrapar (*squash* 1.2 × 0.8), golpe, empujón, *dash*, KO (desarme por las uniones), victoria y 4 emotes. *Hit stop* de 60–100 ms en el KO; 150–200 ms + cámara lenta en el último.

**Audio:** hip hop mexicano en menús, cumbia sonidera en arenas de barrio, electrónica con samples locales en el museo; la música suma capas al quedar 2 jugadores. **Sonidos insignia:** clac-clac de la canica de la lata (confirmar), rasqueta (cambiar de pantalla), máquina de tatuar (cargar), "pop" de vinil (atrapar), salpicadura por técnica (KO), máquina de bordar (subir de nivel). **Música comisionada** a productores locales con cesión de derechos (≈ MXN 2,900–11,500 por pista), para que los *streamers* no reciban reclamos.

---

## 14. Arquitectura técnica

### 14.1 Lo que ya existe: 3 capas

| Capa | Carpeta | Qué hace | Estado |
|---|---|---|---|
| **Presentación** | `Scripts/Unity` (MonoBehaviour) | Dibuja, suena y vibra; **nunca decide golpes**. Hoy con primitivas grises (*greybox*), entrada táctil, teclado y control | [HECHO] |
| **Simulación** | `Scripts/Core` (C# puro, sin UnityEngine) | Reglas a 60 ticks por segundo, gorras, KOs, latas, láser, rondas, bots de 3 dificultades. **Determinista** | [HECHO] |
| **Red** | `Scripts/Net` (C# puro) | Anfitrión autoritativo por UDP en LAN, descubrimiento por *broadcast*, unión por QR (`bf://IP:PUERTO/Sala`), entradas redundantes (aguantan 30 % de pérdida), *snapshots* de 0.6–1 KB a 30 Hz, máximo 6 por sala por defecto (la simulación ya acepta 8; La Reta con fila de 8 solo requiere subir `MaxPlayers` de la sala y probar el hotspot) | [HECHO] |

Además: anfitrión sin gráficos (`HeadlessHost`), métricas de balance con miles de rondas de bots y pruebas automáticas (`dotnet test`) que detectan si un cambio rompe la sincronización.

### 14.2 Ruta de red: LAN → Relay → servidor dedicado

| Etapa | Tecnología | Costo de red al mes |
|---|---|---|
| **Local (MVP)** | UDP actual + **Bonjour/mDNS** (no requiere el permiso especial de *multicast* de Apple) + QR de respaldo + *hotspot* | **US$0** |
| **Online casual** | Implementar `ITransport` sobre **Unity Transport + Unity Relay** y conservar `Sessions.cs`; salas por código + Lobby | ~US$20 con 1k jugadores diarios; **US$260–470 con 10k**; US$2,800–4,900 con 100k |
| **Online público / *ranked*** | El mismo `HeadlessHost` como servidor dedicado en **Edgegap** o **AWS GameLift en Querétaro** (8–20 ms desde CDMX) | **US$750–1,200 con 10k jugadores diarios** |

Plan B solo si la prueba de lag falla **antes del mes 3**: Photon Quantum (no funciona en LAN sin internet).

### 14.3 Backend de progresión

| Etapa | Qué se usa |
|---|---|
| Prototipo y beta local | Sin cuentas ni servidores de juego: progreso cifrado en el celular. **Sí** llevan desde la primera instalación de prueba **Crashlytics** (fallos) y **Unity Analytics** (eventos de §16.2), porque las puertas de la beta se miden con ellos (ambos gratis) |
| Desde la etapa online | **Unity Gaming Services**: cuenta de invitado automática + Apple/Google, Cloud Save, Economy, Cloud Code en C# (Reto del Día re-simulado, canje de códigos, compras), Remote Config. **Firebase** para Crashlytics y notificaciones push. **Unity IAP** con validación en servidor. Portal web de canje. ≈ US$0 con 1k jugadores diarios; US$0–100 con 10k |
| Año 2 | Tablas de *ranked* calculadas en servidor; Club INKCAP |

Regla de oro: **las partidas locales nunca cuentan para tablas ni premios en línea**.

**Propuesta [A PRUEBA] para dos huecos:** (1) en la etapa online, las partidas locales sí dan XP de cuenta, XP de pase y avance de misiones, pero con un **tope de 10 partidas locales con premio al día**, para que un anfitrión modificado no pueda "farmear" el pase; (2) el progreso guardado en el celular durante la beta se sube **una sola vez** a la cuenta en la nube al crearla (nivel, muñequitos, trajes y el sticker "Crew Fundadora"), sin convertirse en Gotas ni TD.

### 14.4 Qué falta (en orden)

1. **Prueba de lag** con `LoopbackTransport` a 0, 50, 100 y 150 ms, con desvío y atrape.
2. Cambios de §4.3 y **bots que golpean**.
3. **Bonjour** y permisos de red local (`NSLocalNetworkUsageDescription` y `NSBonjourServices` en iOS; `MulticastLock` y `ACCESS_LOCAL_NETWORK` para Android 17).
4. Vistas finales (muñequito, *shader* "PlayeraINKCAP", Estampadora, animaciones, pintura en *RenderTexture*, revelación del mural), interfaz, audio y peligros de las 3 arenas.
5. Modos Lata Caliente, La Reta y Reto del Día.
6. **Clip desde entradas:** como la simulación es determinista, el clip de 6 s se reconstruye con las entradas y se graba **solo cuando el jugador lo pide**, sin costo durante la partida.
7. Etapa online: Relay, **predicción del movimiento propio y compensación de lag** (hoy no existen), cuentas, tienda, pase, IAP, analítica y *Addressables*.

**Reglas para programar con IA:** nada de `UnityEngine` en `Core/` ni `Net/`; nunca `Time.deltaTime` ni `Random` en las reglas; todo jugador solo produce `InputCommand`; `dotnet test` antes de cada cambio. Contratar un **programador senior de Unity de medio tiempo** (5–10 h por semana) que revise red y compras.

---

## 15. Accesibilidad, clasificación y seguridad de menores

**Accesibilidad:** forma además de color (aro, ícono y rastro); modos para protanopia, deuteranopia y tritanopia que cambian la paleta; interruptores de sacudida, destellos, vibración y cámara lenta; interfaz de 80–130 %; botones movibles y modo zurdo; auto-apuntado sí/no; contorno de alto contraste; **avisos sonoros** de cada peligro; llavero desactivable.

**Clasificación:** Apple 13+ e IARC E10+/PEGI 7, respondiendo el cuestionario con honestidad. **No** habrá muro de clips con comentarios (contaría como "red social"), ni ruletas o tragamonedas ("apuesta simulada" frecuente = 18+). Los concursos frecuentes dan 13+ en Apple, igual que nuestra clasificación. **"Gorrazo de la semana"** y los torneos con premio físico: **inscripción gratis, sin compra, ganador por jurado o por habilidad (nunca al azar) y bases publicadas**; confirmar con el abogado si alguno requiere permiso de Gobernación.

**Menores (el arte de muñequitos atrae niños, aunque el público objetivo sea 13+):**
- **Sin chat libre de texto ni voz:** solo emotes, frases rápidas ("¡GG!", "¡Revancha!", "Ups") y **stickers de artistas** (que se venden con regalía). Nombres generados ("Gorra Veloz 23") o con filtro.
- Pantalla de edad neutral; menores de 13 en **modo protegido** (sin anuncios, ofertas ni tablas públicas con nombre). Declared Age Range API en iOS y consentimiento parental donde la ley lo pida (Texas, Utah, Luisiana, Brasil).
- Sin cajas botín; confirmación de compra; nada de cuentas regresivas agresivas.
- **Google Play "Familias":** aunque el público declarado sea 13+, Google revisa si el arte atrae a niños; si lo considera así, aplican sus reglas (solo SDK de anuncios certificados para familias, sin identificador de anuncios de menores y pantalla de edad neutral). Diseñar desde el MVP como si aplicaran.
- Aviso de privacidad (LFPDPPP 2025), etiquetas de privacidad, borrado de cuenta en la app y correo de seguridad atendido.

---

## 16. Métricas, KPIs y analítica

### 16.1 Puertas de avance

| Etapa | Métrica | Meta |
|---|---|---|
| Prueba de diversión | Piden "¡otra!" | ≥ 7 de 10 |
| | Explican armado/desarmado tras 2 rondas | ≥ 9 de 10 |
| | Sin sensación de injusticia con 100 ms de lag | ≥ 8 de 10 |
| | Rendimiento en Moto G06 | 30 fps estables |
| Beta local | Testers / partidas por sesión / sesión | 500 / ≥ 3 / ≥ 12 min |
| *Soft launch* | **D1 / D7 / D30** | **≥ 40 % / ≥ 12 % / ≥ 4 %** (mediana de la industria: 22 % / 4 % / 0.7 %; el 10 % mejor llega a ~40 % / ~12 %) |
| | Conversión a pagador / ARPDAU | ≥ 1 % / ≥ US$0.015 |
| | Partidas compartidas | ≥ 10 % |
| | Canje de códigos físicos / clic en "Consíguela real" | ≥ 60 % / ≥ 4 % (3 % de esos compra) |

**Qué se hace con el resultado del *soft launch*:** D1 ≥ 40 % y D7 ≥ 12 % → **escalar** (México en iOS y Android, gasto en creadores). D1 30–40 % o D7 7–12 % → **iterar** 8–12 semanas sin anuncios sobre el núcleo, el primer minuto y el ciclo diario. D1 < 30 % o D7 < 7 % → **replantear el núcleo** (como hizo *Brawl Stars*) o dejar el juego como **herramienta de marca** (modo fiesta para tiendas y eventos, sin online caro).

**KPIs semanales:** DAU/MAU, D1/D7/D30, partidas por jugador, duración de sesión, % que reclama el premio diario, conversión, ARPDAU, ARPPU (LATAM: US$4–10), flujo de Gotas y TD, % local contra online y % de partidas compartidas.

### 16.2 Eventos de analítica (Unity Analytics; una sola herramienta)

| Grupo | Eventos (parámetros clave) |
|---|---|
| Primer minuto | `instalacion`, `tutorial_paso`, `tutorial_fin` (segundos desde abrir) |
| Partida | `partida_inicio` / `partida_fin` (modo, arena, local u online, jugadores, bots, puesto, KOs, duración); `ronda_fin` (duración, empate, muerte súbita); `ko` (causa: gorra, regreso, banda, golpe, láser, camión, Charco, te la regreso; lata activa); `clank`, `desvio`, `empujon`, `atrape_dash`, `lata_recogida` |
| Social y red | `sala_creada` / `sala_unida` (Bonjour, QR, IP, código, WhatsApp); `permiso_red_local` (resultado); `mural_compartido`, `clip_compartido` |
| Progresión | `nivel_sube`, `recompensa_diaria` (día, racha), `mision_completada`, `pase_nivel` |
| Negocio | `tienda_vista`, `compra_virtual`, `compra_real` (artículo, artista, moneda, MXN); `pruebatela`, `consiguela_real_click` (UTM); `codigo_canjeado` (servidor: lote, artista) |
| Técnico | `cuenta_ligada`, `push_abierta`, `error_red`, fps promedio por modelo de celular |

---

## 17. Hoja de ruta

Las fechas son realistas según la investigación 03 (los calendarios de las 3 propuestas iban 3–6 meses adelantados). **Regla del inversionista:** no se gasta en arte final ni en online hasta que 7 de cada 10 personas pidan "otra" con muñequitos *greybox*.

La numeración, las fechas y los montos de esta tabla son **los mismos del plan de negocio** (`docs/PLAN-DE-NEGOCIO-Y-COSTOS.md` §1 y §3, escenario b: equipo actual + IA + 3D en la empresa del cliente + *freelancers*).

| Etapa | Fechas | Entregables | Gasto externo (rango del plan) [estimación] | Puerta |
|---|---|---|---|---|
| **Ya hecho: prototipo web + núcleo Unity** | nov-2026 | Prototipo jugable en el navegador (3 arenas, 6 latas, bots, táctil, XP, premio diario, vestidor); núcleo C# con red LAN, *host* sin gráficos y 21 pruebas | — | — |
| **0. Prueba de diversión** (*vertical slice* en Unity) | dic-2026 → feb-2027 | Cambios de §4.3 uno por uno · prueba de lag · **prueba de estilo** de Osote y del pipeline de playera con 5 diseños reales, hecha en casa por la empresa 3D (sin gasto externo; las pruebas de sillón se hacen en *greybox*, según la regla del inversionista) · Callejón *greybox* con camión · *game feel* y mural · Lata Caliente y La Reta · Bonjour + QR entre 2 iPhone y 4 Android · 30 pruebas de sillón · legibilidad a 64 px | **US$7–14 mil (MXN 135–265 mil)**: revisor senior, celulares, cuentas, IMPI, contrato con artistas y empresa | §16.1 "Prueba de diversión" |
| **1. MVP local Wi-Fi "Edición Fiesta"** (beta cerrada en México) | mar → jul-2027 | 8 muñequitos (los 6 del prototipo + Lienzo y Axo, sobre la misma plataforma), 3 arenas con peligro estrella, 4 modos, 6 latas, niveles 1–50, premio diario, misiones, álbum, vestidor con 20–30 diseños reales, **todo por XP, sin dinero**. Prueba cerrada de Google Play + TestFlight; QR en bolsas de envío e INKCAP TV; Discord | **US$11–31 mil (MXN 210–590 mil)** con el 3D en la empresa del cliente (incluye revisor senior, rigs y animación, interfaz, audio, QA, Mac y aviso de privacidad) | 500 testers, ≥ 3 partidas por sesión |
| **2. Online casual, cuentas y tienda** | ago → nov-2027 | Relay con salas por código y WhatsApp · partida rápida con bots a los 8 s · compensación de lag · cuentas · tienda, pase, IAP · portal de canje · pago con OXXO en la web · tabla del Reto del Día · Crews · Pop-up Store | **US$22–56 mil (MXN 0.42–1.07 M)**, incluido el portal de canje (US$2–5 mil) + red US$0–300 al mes | Sin fallos críticos con lag real |
| **3. *Soft launch* con métricas** | dic-2027 → feb-2028 | Beta abierta en México con la comunidad + prueba pagada pequeña en Colombia; "Temporada 0" de 4 semanas; cohortes separadas | **US$6–18 mil (MXN 115–345 mil)**: QA, anuncios de prueba, eventos y contenido de la T1 | Tabla de §16.1 |
| **4. Lanzamiento México** (iOS + Android) | mar → may-2028 | **Temporada 1 "Grafiti"** (Skatepark, Tagueo, Xolo Rótulo) · primer *drop* phygital · código de creador · votación de *drop* · anuncios recompensados · solicitud al Indie Games Fund LATAM de Google Play (US$150–200 mil) | **US$5–10 mil** en creadores y anuncios + operación ≈ US$3.8 mil al mes | Conversión ≥ 1 %, ARPDAU ≥ US$0.015 |
| **5. Temporadas y online completo** | 2028 → 2029 | Temporadas de 8 semanas (T2 Tatuaje con el "pillo" invitado…) · emparejamiento público · *ranked* en servidor dedicado · Kilometraje · Club INKCAP · Pantallota · art toys con chip NFC de la empresa 3D | Red US$750–1,200 al mes con 10k jugadores diarios | D30 ≥ 4–5 % y > 10k jugadores diarios |

**Por qué lo local sale como beta y no como lanzamiento público:** se respeta la idea de empezar por Wi-Fi, pero un juego público solo local recibe reseñas de "no tiene online" (lo que persigue a *Boomerang Fu* desde 2018) y su D1 mide gente sola contra bots. **Meta agresiva:** Buen Fin 2027, solo si el online se trabaja en paralelo desde mediados de 2027.

**Temporada mínima** (para aguantar el ritmo): 1 arena **o** 1 modo + 8–10 cosméticos del catálogo real + 1 lata. Temporadas de 8 semanas; bajar a 6 solo con datos.

**Totales** (plan de negocio §1 y §7): hasta el lanzamiento en México **US$51–129 mil (MXN 0.97–2.46 M)**, con un plan de trabajo de **≈ US$93 mil (MXN 1.77 M)**, sin sueldos internos. Proyección a 24 meses (dic-2026 → nov-2028): ingresos brutos de MXN 42 mil / **456 mil** / 4.46 M y resultado acumulado de −MXN 2.18 M / **−1.97 M** / +0.88 M (conservador / **base** / optimista). La cifra de "≈ MXN 0.96 M netos en el primer año" que citan la investigación 06 y la evaluación del productor suponía 12 meses en tiendas y más jugadores; **la que vale es la del plan**. En el escenario base el juego no paga un equipo en el año 1: su valor está en **marca, comunidad y venta de merch**; por eso las puertas son innegociables.

### 17.1 Qué tomamos de otros estudios (grandes y novatos)

| Juego (equipo) | Lección que aplicamos |
|---|---|
| *Brawl Stars* (Supercell) | Aro de color y controles del género; *soft launch* paciente; creció al **quitar las cajas botín** |
| *Stumble Guys* (equipo pequeño) / *Fall Guys* (Unity) | Ligero para gama baja y enorme en LATAM / cuerpo base fijo con cosméticos por piezas |
| *Among Us* (3 personas) / *BombSquad* (1 persona) | Salir local sí funciona, pero con **tienda y pase listos antes del pico** / celular como control → Pantallota |
| *Knockout City* (solo online, cerró) | El modo local y el anfitrión propio son un seguro de vida |
| *Boomerang Fu* (estudio pequeño, +1 M de copias) | El esqueleto funciona; su hueco es **móvil + online + progresión** |

---

## 18. Riesgos de diseño y mitigación

| Riesgo | Prob. | Mitigación |
|---|---|---|
| **Retención baja** (lo que de verdad quiebra un juego) | Alta | Primera partida en < 30 s, nivel 4 en la primera sesión, Reto del Día, racha suave; puertas de §16.1; cero anuncios pagados antes de D1 ≥ 40 % |
| **Parecido con *Boomerang Fu*** (regla 4.1 de Apple) | Media | Las 8 reglas firma, latas propias, diario de diseño, abogado de PI antes de lanzar |
| Parecido con *Splatoon* o *Mario Odyssey* | Media | No pintar el piso para ganar; sin "Ink-"/"Splat-" para criaturas; gorra sin ojos ni captura |
| **Lag injusto** en un juego de un golpe | Alta | Anfitrión con compensación, doble KO = empate, ventanas generosas; prueba a 100 ms **antes del mes 3** |
| **Controles táctiles torpes** | Alta | Auto-apuntado de 34°, curva visible, botones grandes y movibles; prueba semanal en Moto G06 |
| **Cosméticos que estorban la lectura** | Media | Misma silueta y radio para todas las gorras, color solo en aro y rastro, latas monocromas |
| Demasiadas mecánicas nuevas a la vez | Media | Una por una, con interruptor y medición; chanfle después del MVP |
| Red local: permisos y Wi-Fi que aísla celulares | Alta | Bonjour + QR + IP + guía de *hotspot*; pantalla de permiso desde el MVP |
| Apple rechaza el phygital | Media | Canje en la web y el mismo artículo a la venta en la app; consultar a App Review antes del primer *drop* |
| Nombre ya registrado | Media | Validación de §0.1 antes del logo; INKAZO de respaldo |
| Contratos de artistas incompletos o mascota sin licencia | Media | Anexo de licencia digital antes de usar cualquier diseño; el "pillo" solo como invitado con contrato |
| El equipo no alcanza a producir temporadas | Alta | Temporada mínima, 8 semanas, configuración remota, arte 3D en la empresa del cliente |
| Advergame aburrido (demasiada marca) | Media | Ninguna capa de marca nueva hasta que 8 de cada 10 probadores pidan otra |
| Costos ocultos (Unity Pro, Mac, legal, comunidad) | Media | Confirmar con el contador qué empresa publica (límite de US$200 mil); presupuestar Mac mini (≈ MXN 19.5 mil) y 0.5–1 persona de comunidad desde el *soft launch* |

---

**Próximas 4 semanas:** (1) validar GORRAZO, INKAZO e INKCAPS (§0.1) y solicitar en el IMPI; (2) prueba de lag; (3) cambios del núcleo uno por uno con `HeadlessHost balance`; (4) prueba de legibilidad a 64 px en un Moto G06; (5) quitar la casilla de códigos en iOS; (6) confirmar con el contador qué empresa publica; (7) contrato tipo con 2 artistas y acuerdo sobre la base de la regalía; (8) abrir el diario de diseño.
