# Evaluación de las 3 propuestas · Mirada de productor e inversionista

> **Juez:** productor e inversionista independiente · **Fecha:** 23-nov-2026
> **Qué evalué:** propuesta A ("Marca primero", nombre INKCAPS), propuesta B ("Juego primero", nombre GORRAZO) y propuesta C ("Mercado móvil", nombre GORRAZO).
> **Contra qué las validé:** investigación 01 a 08, el prototipo web (`prototipo/src/config.js`, `progress.js`, `ui.js`) y el núcleo Unity (`unity/README.md`, `Core/Tuning.cs`, `Net/Sessions.cs`).
> **Pesos:** factibilidad 30 %, negocio 30 %, riesgo 20 %, escalabilidad 20 %. En "riesgo", más puntos = **menos** riesgo.
> **Limitación:** intenté revisar en la web si GORRAZO e INKAZO ya existen como juegos, pero el cupo de búsquedas de la sesión estaba agotado. Ninguna de las tres propuestas pudo hacer esa revisión. **Sigue pendiente y es obligatoria antes de pagar un logo.**
> **Etiquetas:** **[estimación]** = cálculo mío, no un dato verificado.

---

## 0. Veredicto en 10 líneas

1. **Ganadora: C (74/100).** Es la única que piensa como negocio móvil en México: metas de rendimiento en un Moto G06, retención por etapas, pago en OXXO, partida rápida con bots de relleno, costo por etapa y un plan con fechas.
2. **Segunda: B (69/100).** Tiene las mejores mecánicas y la mayor disciplina de alcance ("MVP cerrado"). Le falta el modelo de negocio con números, y algunas de sus ideas nuevas se parecen demasiado a *Boomerang Fu*.
3. **Tercera: A (66/100).** Tiene la mejor idea de negocio, el puente entre la merch y el juego, pero le carga al equipo demasiados sistemas: álbum con intercambios, estampa dorada, pasaporte, votaciones, 12 arenas y 8 modos. Además pone la marca en riesgo con el nombre INKCAPS.
4. **Las tres coinciden en lo importante, y eso ya no se discute:** el arma es **la gorra**, el modelo es gratis con cosméticos, sin pagar para ganar ni cajas botín pagadas, el canje de códigos va **en la web** y no en la app de iOS, el color del jugador va en un aro bajo los pies, y el orden es local, luego online casual y al final ranked.
5. **Diseño final recomendado:** usar C como esqueleto, sumar 4 o 5 mecánicas de B y una versión **simplificada** de la capa de marca de A.
6. **Nombre favorito: GORRAZO** (con el sello "by INKCAP" en el ícono). **Top 3: GORRAZO, INKAZO, INKCAPS.** INKCAPS funciona mejor como nombre de la línea de personajes y *art toys* que como título del juego.
7. **Objeto: la gorra**, con la regla más estricta de las tres (la de A): **una skin cambia el diseño, nunca la silueta ni el tamaño de golpe**.
8. **Tres problemas que ninguna propuesta resolvió:**
   - El *soft launch* "solo local" en Colombia, Perú y Chile mediría a gente jugando contra bots, no el producto real.
   - La meta D7 ≥ 15 % está **por encima del 10 % mejor de la industria** (~12 %).
   - Cuatro de las seis latas del prototipo tienen un equivalente directo en *Boomerang Fu*.
9. **Dinero en riesgo:**
   - Hasta saber si el juego divierte: **≈ MXN 75–135 mil**.
   - Hasta tener un MVP local en manos de la comunidad: **≈ MXN 0.35–0.9 M adicionales**.
   - El online cuesta **otros ≈ MXN 0.4–1.0 M** y solo se paga si la retención lo justifica.
10. **Expectativa honesta:** en el escenario base de la investigación 06, el juego deja **≈ MXN 0.96 M netos** en su primer año en tiendas, **antes de sueldos**. Recupera la inversión en el año 2 con suerte. Su valor del año 1 está en **marca, comunidad y venta de merch**, no en las compras dentro de la app.

---

## 1. Calificaciones

| Propuesta | Factibilidad (30 %) | Negocio (30 %) | Riesgo (20 %, más = menos riesgo) | Escalabilidad (20 %) | **Total /100** |
|---|---|---|---|---|---|
| **C · Mercado móvil** | 74 | 76 | 70 | 74 | **74** |
| **B · Juego primero** | 72 | 66 | 66 | 70 | **69** |
| **A · Marca primero** | 58 | 78 | 60 | 65 | **66** |

Cálculo de C: 0.3 × 74 + 0.3 × 76 + 0.2 × 70 + 0.2 × 74 = 22.2 + 22.8 + 14.0 + 14.8 = **73.8**. Los demás se calculan igual.

---

## 2. Lo que las tres hacen bien (base común del diseño final)

| Decisión | Por qué la apruebo como inversionista | Evidencia |
|---|---|---|
| **La gorra como arma** (23/25 en las tres tablas) | Lectura instantánea, sin interfaz. El cosmético más visible es el producto que INKCAP más vende. Está lejos del bumerán. | Inv. 08 §3.3: la corona de la gorra ocupa ~47×40 px contra ~33×17 px del estampado de la playera |
| **Gratis para jugar, con cosméticos y pase de MXN 89** | Es el modelo del género. En LATAM el gasto es bajo, así que el pase barato y las compras de MXN 19–79 son lo correcto. | Inv. 06: ARPDAU LATAM de US$0.01–0.05 y pagadores de 0.5–2 % |
| **Sin pagar para ganar, sin cajas botín pagadas, sin chat libre** | Cajas botín: Brasil las prohíbe para menores y Apple sube la clasificación. Chat libre: exige moderación y es el mayor riesgo legal con niños. | Inv. 07 §5 y §6 |
| **Canje de códigos de playera en la web**, mismo artículo en venta dentro de la app | Cumple las reglas 3.1.1 y 3.1.3(b) de Apple. Las tres piden quitar la casilla de códigos del prototipo en iOS (`ui.js`, `progress.js`). | Inv. 06 §7.1 |
| **Botón "Cómprala de verdad"** hacia una página de solo productos físicos | Sin comisión de Apple ni de Google (3.1.3(e)). Convierte el juego en escaparate. | Inv. 06 §7.4 |
| **Color del jugador en un aro bajo los pies** (paleta Okabe-Ito), nunca en la ropa | Sin esto, vender playeras rompe la lectura del juego. | Inv. 08 §7.3 |
| **Seguir con Unity 6** | El núcleo ya existe (simulación determinista, red LAN, 19 pruebas). El equipo ya publica en Unity para Quest, y Unity tiene plugin oficial para Claude Code. | Inv. 03 §10 |
| **Local, luego Relay, luego servidor dedicado**, con puertas de métricas | Es la única arquitectura que reutiliza el mismo código. La red cuesta US$0–300 al mes hasta ~10 mil jugadores diarios. | Inv. 04 §8 |
| **Regalía de 15 % del neto** para el artista | Cabe en los números. Revisé las cuentas de las tres y son correctas: gorra de $79 → $8.68 al artista y $49.21 a INKCAP. | Inv. 06 §7.5 |

---

## 3. Evaluación de cada propuesta

### 3.1 Propuesta C · Mercado móvil · **74 / 100**

**Lo mejor:**
- **Factibilidad (74).**
  - Metas técnicas medibles: 30 fps en un Moto G06, descarga de menos de 150 MB, como máximo 120 llamadas de dibujo y 150 partículas.
  - Define una **"temporada mínima"**: 1 arena, 1 regla y 10 cosméticos del catálogo. Es la única que se pregunta si el equipo alcanza a producir temporadas.
  - Pocos cambios al núcleo, todos baratos: partida online a 3 puntos, "Tiraje", Sticker XL un 10 % más lento, atrapar durante el *dash* lo recarga, muerte súbita a los 20 s y doble KO = empate.
- **Negocio (76).**
  - Es la más realista para México:
    - pago en efectivo en OXXO y con tarjetas de regalo;
    - anuncios recompensados, como máximo 5 al día y nunca en partidas locales;
    - un orden de magnitud de ingresos coherente con la investigación (10 mil jugadores diarios ≈ US$3,000–15,000 al mes);
    - regalías calculadas para 3 productos.
  - Tiene palancas de retención concretas: primera partida en menos de 30 s, nivel 4 en la primera sesión, y permiso de notificaciones **después** de la primera victoria y **sin premio** (regla 5.1.2(i) de Apple).
  - Su idea **"Pruébatela 24 h con anuncio"** monetiza a quien no paga y antoja la compra.
- **Escalabilidad (74).**
  - **Partida rápida con bots de relleno a los 8 s**: es la idea que salva el online cuando hay pocos jugadores conectados.
  - **Reto del Día con la misma semilla para todo el país**, re-simulado en el servidor para detectar trampas. Aprovecha el determinismo que **ya está construido** y cuesta casi nada.
  - Calcula el gasto de datos: ≈ 1.2 MB por partida y ≈ 6 MB para el anfitrión. Eso importa con datos prepagados.

**Problemas reales:**
1. **Confusión visual con los estilos de gorra.** Permite *snapback*, *trucker*, *bucket* y visera con el mismo radio de golpe. Pero la visera deja la cabeza descubierta y el *bucket* cambia la silueta, así que **se rompe la lectura de armado/desarmado**, que es el pilar 1. Debe aplicarse la regla de A: misma silueta siempre.
2. **"El color de tu gorra dice qué poder traes."** Choca con las gorras de pago, que tienen colores de diseño, y con el color del dueño en el rastro. El poder debe verse como **efecto**: brillo, goteo o tamaño, como propone B.
3. **El *soft launch* (etapa 2) no tiene online.** Lanzar en Colombia, Perú y Chile un juego de fiesta local, donde nadie conoce a INKCAP, mide a gente sola contra bots. El D1 y el D7 que salgan no dirán nada del producto real (ver §4.1).
4. **Calendario optimista.** La Edición Fiesta (mar–may 2027) promete 4 arenas, 5 modos y 8 muñequitos en 3 meses. La investigación 03 estima **4–6 meses** para un MVP local después del prototipo. La cuarta arena (Taller de Serigrafía) no debe estar en el MVP.
5. **El pase está mal calibrado.** Con 700 XP de pase al día más 4,900 a la semana por misiones semanales, quien juega 4 días a la semana **sí termina** los 40 niveles (≈ 46 mil XP de 40 mil), no "llega al nivel 37". Es un detalle menor, pero confirma que la economía debe ir en configuración remota y calibrarse con datos.
6. **Riesgo con menores.** Los anuncios recompensados más un arte de muñequitos que atrae niños obligan a configurar las redes de anuncios para menores (Google Familias, COPPA) [estimación de esfuerzo: media]. C lo menciona en riesgos, pero no lo presupuesta.
7. **"Andén del Metro"**: debe ser explícitamente ficticio, sin pictogramas reales, como lo dicen A y B ("Línea Tinta").

**Riesgo (70):** elige bien el nombre y descarta **CACHUCHAZO** por la razón correcta (en Argentina y Uruguay "cachucha" es vulgar). Le restan puntos los estilos de gorra y el color de la lata.

---

### 3.2 Propuesta B · Juego primero · **69 / 100**

**Lo mejor:**
- **Factibilidad (72).**
  - El **"MVP cerrado"** es la mejor defensa contra el riesgo número uno de un equipo pequeño: querer hacer todo a la vez. Son 3 arenas, 4 modos, 6 muñequitos y 6 latas.
  - Los siguientes pasos son concretos y de 2 semanas.
  - Todas sus mecánicas se ajustan en `Tuning` sin tocar la red, y se miden con `HeadlessHost balance`.
- **Mecánicas.** Es la propuesta con más "juego":
  - **Empujón sin gorra:** el jugador desarmado puede hacer algo, y eso conecta con los peligros (láser, tren, borde de la azotea).
  - **Regreso solo de la gorra caída a los 3 s:** quita la frustración de quedarse atorado, que es la queja contra *Boomerang Fu*.
  - **Kilometraje de gorra:** la gorra que compraste gana costuras doradas con los KO. Hace que una compra se sienta tuya, y cuesta muy poco.
  - **Doble KO = empate.**
- **Escalabilidad (70):**
  - pone **salas online por código en el *soft launch***, la única que mide algo parecido al producto real;
  - incluye Galería de Retos para jugar solo sin servidor;
  - reconoce los problemas de permisos de red local en iOS y de Wi-Fi que bloquea a los celulares entre sí.

**Problemas reales:**
1. **Parecido con *Boomerang Fu* en ideas "nuevas".**
   - Su lata de evento **Barniz** ("no se puede desviar") es exactamente el power-up *Unstoppable* de *Boomerang Fu*.
   - **Esténcil** ("deja una copia tuya inmóvil") es casi su *Decoy*.
   - Además mantiene la Serigrafía en abanico, que recuerda a *Multi Boomerang*.
   - La investigación 07 §1.4 marca en amarillo tener "power-ups con efectos equivalentes" (ver §4.4).
2. **Demasiadas mecánicas de maestría a la vez:** chanfle, Encasquetada, desvío que cambia de dueño, empujón y regreso automático. Cada una cambia el balance, y **los bots todavía no usan ni el golpe** (0 desvíos en el balance del README). Hay que enseñárselas una por una. Mi recomendación: dejar chanfle y desvío que cambia de dueño para la temporada 1.
3. **Confusión visual:**
   - **"Inundación de tinta"** (la tinta entra desde los bordes y pisarla es KO) pone **tinta que mata en el mismo piso donde hay manchas de tinta decorativas** del mural. El jugador no sabrá cuál mata.
   - El desvío que cambia el color del rastro durante 0.6 s rompe la regla de que el rastro es del color del dueño.
   - Su muñequito **"Lienzo"**, con cabeza en forma de tapa de aerosol, puede confundirse con una cabeza con gorra.
4. **Negocio (66).** El catálogo y el phygital están bien, pero no trae proyección de ingresos, costo de adquisición ni plan de pagos en efectivo. Recomienda **CACHUCHAZO** como alternativa 2, que en el Cono Sur es vulgar.
5. **Calendario muy agresivo:** beta en el mes 2–5 y online por código en el mes 5–9. El núcleo **todavía no tiene predicción ni compensación de lag** (lo dice `Net/Sessions.cs`), y la investigación 03 estima 3–6 meses y US$20–50 mil para el online.

---

### 3.3 Propuesta A · Marca primero · **66 / 100**

**Lo mejor:**
- **Negocio (78), la mejor.** Es la única que cuantifica el **círculo merch ↔ juego**:
  - 2,000 prendas con código → ~1,200 jugadores sin pagar anuncios;
  - 4 % que toca "Consíguela real" × 3 % que compra → ~42 pedidos al mes;
  - enlaces UTM por traje para medirlo.
- **Visión de producto de marca:**
  - cada producto del catálogo tiene un lugar en el juego (el llavero cuelga de la gorra, la taza es el trofeo, el *art toy* es el acabado);
  - **escanear art toys reales con la empresa 3D del cliente**, que es sinergia real de costos;
  - **votación de drop** para reducir el inventario sin vender;
  - **código de creador** del 5 % para artistas.
- **Buenas reglas de diseño:**
  - **"Una skin cambia el diseño, nunca la silueta"** (la mejor regla de las tres);
  - mismo radio de golpe para todos los personajes;
  - el día 7 da una gorra, porque se ve 3 veces más que la playera;
  - el reflector "EN VIVO" sobre el líder, que frena a quien va escapándose en el marcador.

**Problemas reales:**
1. **Factibilidad (58): demasiados sistemas.** Para un equipo pequeño que programa con IA, cada uno de estos es backend, tienda, pruebas y soporte:
   - 12 arenas y 8 modos;
   - álbum de 60 estampas **con intercambio de repetidas entre amigos**, que exige una economía de intercambios, control de fraude y moderación;
   - estampa dorada, pasaporte de artista, votación de drop y Club;
   - *art toys* numerados;
   - una cuarta arena (Pop-up Store) en el MVP.
2. **Riesgo de tiendas:** la **estampa dorada** se obtiene *solo* canjeando una prenda física y es *la única forma* de completar el álbum. Es contenido digital exclusivo de una compra fuera de la app, justo lo que la regla 3.1.3(b) de Apple pide evitar (el artículo debe venderse también dentro de la app). Además, poner un logro detrás de un pago genera mala reputación.
3. **Riesgo de marca con INKCAPS:**
   - si el juego tiene reseñas de 1 estrella, aparecen al buscar la marca de ropa;
   - en un juego **de tinta**, llamar a los personajes "los Inkcaps" los acerca a los "Inklings" de *Splatoon* (Nintendo). El riesgo es bajo-moderado porque INKCAP es una marca previa del cliente, pero suma parecido;
   - A ofrece "GORRAZO by INKCAP" como alternativa, lo cual es sensato.
4. **Confusión visual:**
   - **Fat Cap** (cabeza de boquilla de aerosol) y **Coprino** (cuya "gorra es su sombrero de hongo") hacen ambiguo el estado armado/desarmado;
   - **8 jugadores** en la pantalla de un celular se lee peor. El núcleo hoy limita la sala LAN a 6 (`Sessions.cs`, `MaxPlayers = 6`).
5. **El Pillo en el elenco base:** hace depender al juego de la propiedad intelectual de un tercero. B y C lo ponen como invitado de temporada, que es lo correcto.
6. **Doble conteo:** suma "≈ $900 mil al año de prendas con código" al círculo del juego, pero esas playeras se venderían de todos modos. **Lo que cuenta es lo incremental**: la subida de ventas por el código y las compras que vienen del juego.
7. **"Caza del Drop" con cupón físico de premio:** un concurso con premio puede requerir permiso en México (Ley Federal de Juegos y Sorteos, por verificar) y sube la clasificación por edad si es frecuente (inv. 07: concursos frecuentes = 13+). Mejor dar recompensas garantizadas.
8. **Escalabilidad (65):** el *soft launch* local en el extranjero tiene el mismo problema que el de C.

---

## 4. Problemas que ninguna propuesta resolvió (y cómo resolverlos)

### 4.1 El *soft launch* "solo local" mide lo que no es

**Qué pasa:** A y C lanzan en Colombia, Perú y Chile **sin online**, con US$1,500–6,000 en anuncios. Esos jugadores no conocen a INKCAP y casi nunca tendrán 3 amigos con la app en el mismo Wi-Fi, así que terminarán jugando solos contra bots. El D1 y el D7 medirán "un juego contra bots", no el juego de fiesta ni el online. Y el canal real de crecimiento (comunidad INKCAP, artistas, INKCAP TV) no está en esos países.

**Solución:**
- **Beta "Edición Fiesta" en México**, por invitación y con la comunidad INKCAP: QR en bolsas de envío, INKCAP TV y eventos. Mide la diversión y la sesión en grupo.
- **El *soft launch* con métricas se hace hasta tener online casual**: salas por código más partida rápida con relleno de bots, como propone C.
- Mantener una pequeña prueba pagada en Colombia **solo** para comparar contra tráfico frío.
- Medir por separado la cohorte que llega por la comunidad y la que llega por anuncios.

**Por qué importa además:** si el juego sale **público** en tiendas solo con modo local, las reseñas dirán "no tiene online", la misma crítica que persigue a *Boomerang Fu* desde 2018 (inv. 01). Lo local puede salir como **acceso anticipado o beta**; el lanzamiento público debe traer online casual. Así se respeta la idea del cliente de "empezar local por Wi-Fi" sin quemar la primera impresión en la tienda.

### 4.2 La meta D7 ≥ 15 % está por arriba del 10 % mejor de la industria

En la investigación 05 §6.5, el 10 % mejor de los juegos logra ~40 % D1 y **~12 % D7**. Exigir D7 ≥ 15 % para avanzar significa que, con alta probabilidad, la puerta no se abre. No es malo ser exigente, pero hay que decidir de antemano qué se hace en cada caso:

| Resultado del *soft launch* | Decisión |
|---|---|
| D1 ≥ 40 %, D7 ≥ 12 %, D30 ≥ 4 % | **Escalar:** México con iOS y Android, y gasto en creadores |
| D1 de 30–40 %, D7 de 7–12 % | **Iterar** 8–12 semanas sobre el núcleo, el primer minuto y el ciclo diario. Sin anuncios. |
| D1 < 30 % o D7 < 7 % | **Replantear el núcleo**, como hizo *Brawl Stars*, o dejar el juego como **herramienta de marca**: modo fiesta para eventos y tienda, sin online caro |

### 4.3 Costos ocultos que las propuestas no presupuestaron

| Costo | Monto | Comentario |
|---|---|---|
| **Unity Pro** si la empresa que publica (sumando el grupo) factura más de US$200 mil al año | **US$2,310 por puesto al año (≈ MXN 44 mil)** | Una marca de ropa con e-commerce más un estudio VR pueden superar ese límite. Conviene publicar desde una empresa nueva o confirmarlo con el contador (inv. 03). |
| **Predicción y compensación de lag** para el online | Parte de los **US$20–50 mil** del online (inv. 03) | El núcleo hoy no la tiene: en LAN no hace falta, pero en 4G sí. |
| **Portal web de canje** con inicio de sesión de la cuenta del juego, códigos de un solo uso por lote y antifraude | **US$2–5 mil** [estimación] más soporte | Es lo que hace legal el phygital en iOS. Sin cuentas en la nube no existe. |
| **Etiquetas con código para raspar** | **MXN 1–3 por prenda** [estimación] | Con 2,000 prendas, MXN 2–6 mil al año |
| **IMPI**: nombre del juego en las clases 9, 41 y 28, más INKCAP en las clases 9 y 41 | **≈ MXN 16.5 mil** de derechos + **MXN 12.5–30 mil** de honorarios | Inv. 07 §2.3. La ropa virtual va en la clase 9, no en la 25. |
| **Contrato tipo con artistas**, aviso de privacidad (ley de 2025) y términos de servicio | **MXN 25–60 mil** [estimación] | El contrato debe cubrir la adaptación a 3D y qué pasa con los trajes ya vendidos si termina. |
| **Mac mini** para pruebas serias en iPhone | **≈ MXN 19.5 mil** | Se puede compilar sin Mac (Codemagic), pero para depurar conviene tenerla. |
| **Celulares de prueba** (Moto G06 o Galaxy A07, un iPhone viejo) | **MXN 20–28 mil** | Inv. 03 |
| **Murales comisionados por arena** (A) | Pago fijo a cotizar + 15 % | No viene en el presupuesto de arenas de la inv. 03 |
| **Configurar los anuncios para menores** (C) | Tiempo de desarrollo y revisión | Por el arte de muñequitos, Google puede considerar que el juego "atrae niños" (inv. 07 §3.7) |
| **Gestión de comunidad y eventos** | **US$500–1,500** en la beta (inv. 02) más tiempo del equipo | Es el canal principal de crecimiento, así que hay que presupuestarlo |

**Ahorro que nadie aprovechó del todo:** la otra empresa del cliente hace **modelado y escaneo 3D**. Los personajes y las arenas son la partida externa más grande del MVP (≈ US$6–12 mil de los US$15–40 mil, inv. 03 §9.2). Hacerlos en casa puede **bajar el MVP a US$8–25 mil** [estimación]. A lo menciona solo para *art toys*.

### 4.4 Las latas se parecen demasiado a *Boomerang Fu*

Las mecánicas son libres, pero la investigación 07 §1.4 marca en **amarillo** un conjunto de power-ups "con efectos equivalentes". Así quedan las 6 latas del prototipo frente a *Boomerang Fu* (inv. 01 §4.4):

| Lata del prototipo | Equivalente en *Boomerang Fu* | Riesgo |
|---|---|---|
| Tinta derramada (explosiva) | *Explosive Boomerang* | Idéntico |
| Parche bordado (escudo de 1 golpe) | *Shield* | Idéntico |
| Grafiti (rastro que hace lentos a los demás) | *Ice Boomerang* (rastro que ralentiza) | Casi idéntico |
| Tatuaje (teledirigida) | *Telekinesis* (guías el bumerán) | Parecido |
| Serigrafía (triple en abanico) | *Multi Boomerang* (se divide en 5) | Parecido; el "Tiraje" de C lo aleja |
| Sticker XL (gorra gigante) | No tiene | Propio |

**Recomendación:** antes de lanzar, que **al menos 3 de las 6** tengan un efecto propio ligado a su técnica de tinta:
- **Serigrafía:** usar el "Tiraje" de C (copias que siguen el mismo trayecto).
- **Grafiti:** en lugar de un rastro que alenta, un **muro de tag** que bloquea gorras por 3 s, que es el "Esténcil" de A.
- **Bordado:** en vez de un escudo, un **parche que "cose" tu gorra a tu mano**: la siguiente gorra que te desvíen regresa sola [idea a probar].

Nunca usar Barniz (= *Unstoppable*) ni una copia inmóvil (= *Decoy*). Sobre todo, **llevar la bitácora de diseño limpio** de la inv. 07 §1.6.

### 4.5 Confusión visual: tres reglas que hay que agregar

1. **El piso no puede tener dos tintas que se lean igual.** Durante la partida, las manchas del mural se ven **mate, planas y desaturadas al 40 %**. Al final, en la "revelación del mural", recuperan todo su color. Cualquier tinta que haga daño (charco explosivo, rastro) va **animada, con borde brillante y aviso de 0.4–1 s**. Así se conserva el momento más compartible (el mural) sin ensuciar la lectura.
2. **Ninguna cabeza puede parecerse a una gorra.** Se descartan las cabezas de boquilla de aerosol, tapa de aerosol y sombrero de hongo. Prueba obligatoria: a 64 px, 9 de cada 10 personas dicen correctamente si el personaje trae o no la gorra.
3. **El power-up se ve como efecto, nunca como color.** El color pertenece al dueño (aro y rastro). El diseño de la gorra pertenece a lo que compraste.

### 4.6 El paso a online es más trabajo del que parece

- **Hoy:** la red es LAN por UDP con descubrimiento por *broadcast*. Ese descubrimiento necesita en iOS un permiso especial de Apple (*entitlement* de multicast). La salida es Bonjour o QR (inv. 04 §2.3).
- **Ninguna propuesta menciona** que desde **Android 17** las apps que apunten a esa versión deberán pedir el permiso de red local **para todo el modo local** (inv. 04, punto 5). Hay que diseñar desde ya la pantalla que explica ese permiso.
- **Para online:** falta implementar el transporte con Unity Relay y, sobre todo, la **predicción de movimiento propio y la compensación de lag** en el anfitrión. La prueba de lag a 50, 100 y 150 ms con `LoopbackTransport` debe hacerse **antes del mes 3**: es el último momento razonable para cambiarse a Photon Quantum (inv. 04 §8.1).
- **Primer online casual:** el anfitrión es un celular, así que puede hacer trampa y, si se va, se cae la partida. Por eso: nada de ranked ni premios en ese modo, y tablas de clasificación solo con el Reto del Día re-simulado (idea de C).

### 4.7 La base de la regalía debe ser la misma en físico y en digital

Las tres usan "15 % del **neto**" (después de IVA y comisión de la tienda). Si hoy INKCAP paga 15 % sobre el **precio de venta** de la playera, el artista recibirá en digital entre 15 % y 27 % menos por cada peso vendido (según si hoy se calcula con o sin IVA), y puede sentirlo como un recorte. Hay que **definirlo por escrito y explicarlo**, junto con una regla para los paquetes y el pase. Por ejemplo: "la regalía del pase se reparte según los artículos de cada artista dentro del pase". Si no, C le paga al artista invitado regalía sobre el pase completo.

---

## 5. Números del inversionista: presupuesto y calendario realistas

Fuentes: inv. 03 §9.2, inv. 04 §8.4 e inv. 06 §10. **[estimación]** salvo donde se indica.

| Etapa | Fechas realistas | Qué sale | Gasto externo | Puerta para avanzar |
|---|---|---|---|---|
| **0. Prueba de diversión** | dic-2026 → feb-2027 | Cambios mínimos al núcleo (§6), Lata Caliente o La Reta, prueba de lag, 30 pruebas presenciales | Revisor Unity senior ≈ US$2.7–5.4 mil + celulares MXN 20–28 mil + cuentas ≈ MXN 2.4 mil → **≈ MXN 75–135 mil** | 70 % o más pide "otra"; KO justos a 100 ms |
| **1. Beta local "Edición Fiesta"** | mar → jul-2027 | Android en prueba cerrada en México, más TestFlight. 3 arenas, 6 muñequitos, 3 modos + Reto del Día, progreso en el celular | **US$15–40 mil** (US$8–25 mil con arte 3D en casa) + IMPI ≈ MXN 30–47 mil + legal ≈ MXN 25–60 mil | 500 testers, 3 partidas o más por sesión |
| **2. Cuentas, tienda y online casual** | ago → nov-2027 | Cuentas en la nube, tienda, pase, portal de canje, salas por código y partida rápida con bots (Unity Relay) | **US$20–50 mil** + portal US$2–5 mil + red US$0–300 al mes | Sin fallos críticos con lag real |
| **3. *Soft launch* con métricas** | nov-2027 → feb-2028 | Beta abierta en México con la comunidad + prueba pagada pequeña en Colombia | Anuncios **US$1.5–6 mil** + eventos US$0.5–1.5 mil | Tabla de §4.2 |
| **4. Lanzamiento México** (iOS y Android) | mar → may-2028. Meta agresiva: Buen Fin 2027, solo si el online empieza en paralelo a mediados de 2027 | Temporada 1 "Grafiti", primer drop phygital, programa de creadores, solicitud al Indie Games Fund | Creadores y anuncios US$5–10 mil (inv. 02) | Conversión de 1 % o más; ARPDAU de US$0.015 o más |
| **5. Ranked y servicio continuo** | 2029 | Servidor dedicado (Edgegap o GameLift en Querétaro), Club INKCAP | **US$750–1,200 al mes** con 10 mil jugadores diarios | D30 de 4–5 %, más de 10 mil jugadores diarios |

**Totales:**
- **Hasta el lanzamiento en México:** ≈ **US$50–125 mil (≈ MXN 0.95–2.4 M)**, o ≈ US$43–110 mil con arte 3D en casa.
- **Ingreso neto del primer año en tiendas** (inv. 06 §10, antes de sueldos): conservador ≈ MXN 0.1 M, **base ≈ MXN 0.96 M**, optimista ≈ MXN 8.4 M. A eso se suma la venta de playeras con código, que es **ingreso bruto y en buena parte no incremental**.
- **Conclusión:** con el escenario base, el juego **no paga un equipo en el año 1**. Se justifica por tres caminos: (1) que la retención llegue al 10 % superior, (2) el **fondo no reembolsable** (Indie Games Fund LATAM, US$150–200 mil, solo con el juego ya publicado) o (3) su valor como **medio de marca y canal de merch**. Por eso las puertas son innegociables. **El online no se construye hasta que la Edición Fiesta pruebe que divierte.**

Comparación con los calendarios propuestos: A pone el lanzamiento en México en los meses 9–12, B pone el online por código en los meses 5–9 y C lanza México online en sep–dic 2027. Los tres están **3–6 meses por delante** de lo que la investigación 03 considera realista (prototipo de 6–8 semanas, MVP en 4–6 meses y online en 3–6 meses más). La meta agresiva de C (Día de Muertos o Buen Fin 2027) es posible **solo** si el online se trabaja en paralelo a la beta local.

---

## 6. Ideas a injertar en el diseño final

Prioridad: **MVP** = entra en la Edición Fiesta; **On** = con el online casual; **T1+** = temporadas.

| # | Idea | De | Por qué | Prioridad |
|---|---|---|---|---|
| 1 | **Una skin cambia el diseño o el material de la gorra, nunca la silueta ni el tamaño de golpe**, y el rastro siempre es del color del dueño | A + B | Protege el pilar "gorra en la cabeza = armado" y evita el pagar para ganar | MVP |
| 2 | **Doble KO = empate** | B, C | Es justo y esconde el lag en online | MVP |
| 3 | **Partida online a 3 puntos, local a 5**, y **muerte súbita a los 20 s** con muros que se cierran (no con tinta en el piso) | C | Partidas de 2–3 min y corta el 10 % de rondas largas | MVP |
| 4 | **Una lata a la vez**, que se pierde con el KO; las combinaciones quedan como regla de evento | C (mezcla con A y B) | Menos estados que leer en pantalla chica y menos balance | MVP |
| 5 | **Serigrafía "Tiraje"** (copias que siguen el mismo trayecto) y **Esténcil-muro** como lata de temporada | C, A | Nos aleja de *Multi Boomerang* y *Decoy* (§4.4) | MVP |
| 6 | **Empujón sin gorra**, probado con prueba A/B contra el **"Modo sencillo"** de C | B, C | El desarmado puede hacer algo, y se conecta con los peligros | MVP (prueba) |
| 7 | **Un solo atrape con premio:** atrapar durante el *dash* lo recarga, sin botón extra | C (en lugar de la Encasquetada de B y el atrape con estilo de A) | Una mecánica, no tres; en táctil y con lag es la más fácil | MVP |
| 8 | **Regreso solo de la gorra caída a los 3 s** | B | Corrige la frustración de quedarse sin arma | MVP |
| 9 | **Reto del Día** con la misma semilla para todo el país, re-simulado en el servidor | C | Hábito diario barato que aprovecha el núcleo determinista | MVP (tabla en On) |
| 10 | **La Reta** (el que gana se queda) y **Lata Caliente** (papa caliente) | C, B | Baratos, muy de fiesta y de streamer; ideales para eventos de INKCAP | MVP |
| 11 | **Crews con "Retoque":** párate 2 s sobre la mancha de tu compañero para revivirlo, una vez por ronda | A (igual al "Re-ensamblar" de B) | Jugadas de héroe ligadas a la tinta, sin menús | MVP |
| 12 | **Metas técnicas de C:** 30 fps en Moto G06, menos de 150 MB, primera partida en menos de 30 s, nivel 4 en la primera sesión, notificaciones sin premio | C | Retención D1 y cumplimiento de Apple | MVP |
| 13 | **Revelación del mural:** manchas mate durante la partida y a todo color al final, más **postal con QR de descarga** y **clip vertical de 6 s** | Mío + A + B/C | Lo más compartible, sin ensuciar la lectura del piso | MVP |
| 14 | **Día 7 = gorra de la semana; calendario de 28 días = playera del mes** (diseño real); racha suave que se pausa | A, B, C | La gorra se ve 3 veces más; la playera premia la lealtad | MVP |
| 15 | **Partida rápida con bots de relleno a los 8 s** | C | Sin esto, el online con pocos jugadores conectados se siente vacío | On |
| 16 | **Kilometraje de gorra** (costura dorada a los 25 KO, contador a los 100) | B | Apego a lo comprado, solo cosmético, casi gratis | On |
| 17 | **"Consíguela real" con UTM por traje + drop sincronizado con Estrenos + "Pruébatela 24 h con anuncio"** | A, C | Mide qué diseños venden playeras y antoja la compra | On |
| 18 | **Código de creador** del 5 % para artistas e INKCAP TV | A, B, C | Convierte a cada artista en promotor | On |
| 19 | **Pago en OXXO y con tarjetas de regalo** de Google Play; anuncios recompensados como máximo 5 al día, nunca en partidas locales | C | Así paga México | On |
| 20 | **Temporada mínima** (1 arena + 1 regla + 10 cosméticos) y temporadas de **8 semanas** al inicio, no de 6 | C + mío | El equipo debe poder sostener el ritmo; con datos se baja a 6 | T1+ |
| 21 | **Personaje "pillo" como invitado de temporada**, con licencia y *art toy*, fuera del elenco base | B, C | No depender de la propiedad intelectual de un tercero | T1+ |
| 22 | **Votación de drop** (la gorra digital más votada se fabrica) | A, B | Menos inventario sin vender (caso Forever 21) | T1+ |
| 23 | **Reflector "EN VIVO" sobre el líder** en la arena Set de INKCAP TV | A | Frena al que se escapa en el marcador | T2+ |
| 24 | **Art toys numerados** hechos por la empresa 3D del cliente, de preferencia con **chip NFC** (regla 3.1.4 de Apple) | A + inv. 06 | Ruta phygital limpia en iOS y sinergia de grupo | Año 2 |
| 25 | **Metro ficticio "Línea Tinta"**, sin logos ni pictogramas reales | A, B | Evita problemas de derechos de autor | Cuando toque |

---

## 7. Ideas a descartar

| Idea | De | Por qué se descarta |
|---|---|---|
| **Estampa dorada solo con prenda física** para completar el álbum | A | Contenido digital exclusivo de una compra fuera de la app: choca con la regla 3.1.3(b) de Apple y pone un logro detrás de un pago |
| **Álbum con intercambio de repetidas** | A | Exige economía de intercambios, antifraude y moderación. Queda el **álbum de logros por hazañas** de B, que no se vende ni se intercambia. |
| **El Pillo en el elenco base** | A | Hace depender al juego de la licencia de un tercero; va como invitado (B, C) |
| **Cabezas con forma de gorra** (Fat Cap, Coprino, Lienzo) | A, B | Hacen ambiguo el estado armado/desarmado |
| **Estilos de gorra con distinta silueta** (*bucket*, visera) | C | La visera deja la cabeza descubierta y rompe la lectura |
| **"El color de tu gorra dice qué poder traes"** | C | Choca con los diseños de pago y con el color del dueño; el poder se muestra con efectos |
| **Latas Barniz (no se puede desviar) y Esténcil-copia inmóvil** | B | Son *Unstoppable* y *Decoy* de *Boomerang Fu* |
| **Inundación de tinta que mata desde el piso** | B | Se confunde con la tinta decorativa del mural; la muerte súbita va con muros |
| **Desvío que cambia el color del rastro** y **chanfle** en el MVP | B | Demasiada maestría escondida a la vez; los bots aún no usan el golpe. Se revisan en la T1. |
| **8 jugadores en celular al lanzamiento** | A | Peor lectura en pantalla chica y más carga para el anfitrión; el núcleo limita a 6 en LAN |
| **Cuarta arena en el MVP** (Pop-up Store, Taller de Serigrafía) | A, C | Cada arena cuesta US$1.5–2.5 mil más integración; el MVP usa las 3 que ya existen |
| ***Soft launch* "solo local" en Colombia, Perú y Chile** | A, C | Mide a gente jugando contra bots, no el producto (§4.1) |
| **Lanzamiento público en tiendas sin online** | Implícito en A y C | Reseñas de "no tiene online" desde el día 1. Lo local sale como beta o acceso anticipado. |
| **"Caza del Drop" con cupón físico de premio** | A | Posible permiso de sorteos y sube la clasificación por edad; mejor recompensas garantizadas |
| **Exclusividad "Edición Física"** sin aprobación de Apple | A, B, C | Primero se lanza el canje del **mismo** artículo que se vende en la app; la variante exclusiva solo después de consultarlo con App Review |
| **Sumar las playeras con código como ingreso del juego** | A | No es incremental; se mide solo la subida de ventas |
| **Nombres CACHUCHAZO, CAP CLASH, INKCAP BRAWL, SPLAT CAP, NO CAP** | B, A, C | Vulgar en el Cono Sur; "Clash" y "Brawl" de Supercell; "Splat-" de Nintendo; NO CAP es jerga genérica (sirve como eslogan) |
| **"Tintoys" como nombre colectivo** | B | Suena a *tin toys* ("juguetes de hojalata") y al corto de Pixar *Tin Toy*: es débil y descriptivo en inglés |

---

## 8. Nombre

| Lugar | Nombre | Por qué | Riesgo |
|---|---|---|---|
| **1 (favorito)** | **GORRAZO** (ícono con el sello "by INKCAP"; en la tienda, "GORRAZO: Duelo de Gorras") | Es la acción central y el grito del KO. Ya está en el prototipo, tiene 7 letras y se vuelve verbo ("échate unos gorrazos"). **Protege la marca INKCAP** si al juego le va mal, sin perder la liga visual. | **Bajo-medio:** es una palabra común en español, así que la marca es evocativa (registrable, inv. 07 §2.2), pero más débil que una inventada. Conviene registrarla como **nominativa y mixta** (con el logo). La "rr" cuesta en inglés, pero la primera ola es México y LATAM. |
| **2** | **INKAZO** | Palabra inventada: es el registro **más fuerte** (inv. 07) y el más ligado a INKCAP (INK + "-azo") | **Bajo** en marcas. Al oído puede confundirse con "Inca", y requiere más marketing para que se entienda. |
| **3** | **INKCAPS** | Máxima exposición de la marca; ideal como nombre de la **línea de personajes y art toys** ("colecciona los Inkcaps") | Expone la marca de ropa a las reseñas del juego, y en un juego de tinta se acerca a los "Inklings" de *Splatoon*. Mejor como **submarca de coleccionables**. |

**Pendiente obligatorio** (30 minutos por nombre): MARCia del IMPI (búsqueda fonética en las clases 9, 41, 28 y 25), WIPO Brand Database, App Store, Google Play, Steam, itch.io, dominios `.com` y `.mx`, y cuentas en TikTok e Instagram. Hay que hacerlo **antes** de pagar el logo y presentar la solicitud ante el IMPI para el juego **y** para INKCAP en las clases 9 y 41 (ropa virtual).

---

## 9. Objeto arrojadizo

**Recomendación: LA GORRA.** Las tres propuestas la eligen con 23/25. Le gana al disco de vinil (17–18) y al disco volador (14–16, que además usa *Windjammers* y no se puede llamar "Frisbee"). La pelota (11–13) no regresa y la usaba *Knockout City*, que cerró.

**Reglas que hacen que funcione (todas obligatorias):**
1. **Misma silueta y mismo tamaño de golpe** para todas las gorras (radio 0.38 en `Tuning.cs`). Solo la lata Sticker XL cambia el tamaño.
2. **El rastro y la sombra siempre son del color del dueño.** El diseño comprado va en la corona y la visera, que es la parte que más se ve desde arriba.
3. **Sin gorra = cabeza descubierta con pelos parados** (B). Ninguna cabeza puede parecerse a una gorra.
4. **El poder se ve como efecto** (brillo, goteo, tamaño), nunca como color.
5. **Llavero colgante** (charm TPU) como cosmético barato: pequeño, sin cambiar la silueta ni el tamaño de golpe, y con la opción de apagarlo en ajustes para quien quiera una pantalla más limpia.

---

## 10. Recomendación final para el diseño definitivo

**Esqueleto: propuesta C.** Le suma de B el MVP cerrado, el empujón, el regreso de la gorra a los 3 s, el kilometraje y Lata Caliente. Le suma de A, simplificado, el "Consíguela real" con UTM, la votación de drop, el código de creador, la regla de silueta, el día 7 con gorra y el plan de *art toys* con la empresa 3D.

**Las próximas 4 semanas, en este orden:**
1. **Revisión de nombres** (GORRAZO, INKAZO e INKCAPS) en el IMPI y las tiendas. Si GORRAZO está libre, solicitarlo en las clases 9, 41 y 28 junto con INKCAP en las clases 9 y 41.
2. **Prueba de lag** a 50, 100 y 150 ms con `LoopbackTransport`. Es la decisión técnica más cara si se toma tarde.
3. **Cambios mínimos al núcleo**, uno por uno y midiendo con `HeadlessHost balance`:
   - doble KO;
   - una lata a la vez;
   - Tiraje;
   - regreso de la gorra a los 3 s;
   - recarga del *dash* al atrapar;
   - muerte súbita con muros;
   - enseñar a los bots a usar el golpe.
4. **Prueba de legibilidad a 64 px** con 3 siluetas de muñequito y la gorra puesta y quitada, en un Moto G06 real.
5. **Quitar la casilla de códigos** en iOS (`ui.js` y `progress.js`) y convertir la playera "solo con código" del prototipo en un artículo que **también se vende** en la tienda del juego.
6. **Confirmar con el contador** desde qué empresa se publica: define si aplica Unity Pro y el Small Business Program de Apple (cuentas asociadas).
7. **Firmar el contrato tipo** con 2 artistas, con licencia digital, adaptación a 3D y qué pasa si el juego cierra, y acordar la base de la regalía (§4.7).

**Regla del inversionista:** no se gasta en arte final ni en online hasta que **7 de cada 10 personas pidan "otra"** en la prueba de sillón con cubos y muñequitos *greybox*. Si no se siente bien con cubos, no se va a sentir bien con muñequitos (inv. 08 §15).
