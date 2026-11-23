# Evaluación independiente · Director creativo y jugador

> **Fecha:** 23-nov-2026 · **Proyecto:** "Boomerang Fighters" (nombre de desarrollo)
> **Qué evalué:** las propuestas A (marca primero), B (juego primero) y C (mercado móvil), contra el prototipo web (`prototipo/`), el núcleo Unity (`unity/`) y la investigación 01–08.
> **Mi enfoque:** que sea divertido en un celular, que se entienda de un vistazo, que no parezca copia de *Boomerang Fu* (y que no tenga riesgo legal), que se sienta INKCAP y que la gente quiera compartir clips.
> **Pesos:** diversión 30 %, originalidad y distancia legal 25 %, marca 25 %, viralidad 20 %.
> **Aviso:** no pude buscar en internet si los nombres están libres, porque el cupo de búsquedas de esta sesión ya estaba agotado. Ninguna de las 3 propuestas lo pudo hacer tampoco. El riesgo de cada nombre es una estimación, y hay que revisarlo en MARCia (IMPI), en las tiendas de apps y en WIPO antes de pagar un logo.

---

## 1. Veredicto en 30 segundos

1. **Las tres propuestas coinciden en lo importante**, y estoy de acuerdo: **el arma es la gorra del propio muñequito**, un golpe = KO, la arena termina como mural, los cosméticos son ropa real de INKCAP que nunca da ventaja, y se lanza primero en Wi-Fi local y después en línea.
2. **Ninguna gana por mucho.** B tiene el mejor juego, C la mejor estrategia para México y celulares, y A el mejor puente con la marca. El diseño final debe usar **B como base del juego, C para lanzamiento y modos, y A para la parte de marca**.
3. **Ranking:** **B (81.6) > C (80.8) > A (72.0).**
4. **Nombre favorito: GORRAZO** (en la tienda, *"GORRAZO by INKCAP"*). Le siguen **INKAZO** e **INKCAPS**.
5. **Objeto: la gorra.** No se cambia. Más abajo van 6 reglas para que siga siendo justa y fácil de leer.
6. **Los 5 problemas reales más graves que encontré:**
   - **C usa el color de la gorra para mostrar el poder.** Eso choca con el color del jugador (el rastro) y con la gorra de pago (el diseño). Tres señales distintas en un solo objeto de 47 × 40 píxeles.
   - **A y B subestiman el costo de jugar en línea.** Dicen "US$0–300 al mes hasta 10,000 jugadores diarios". La investigación 04 dice **~US$260–470 al mes** con Unity Relay.
   - **B presenta el empujón sin gorra como idea propia, pero es la regla exacta de *Boomerang Fu*.** Ahí, sin bumerán, el golpe solo empuja. No es ilegal, porque las mecánicas son libres, pero hace que el paquete de reglas se parezca todavía más.
   - **A tiene un modo que es casi el "Golden Boomerang" de *Boomerang Fu*.** También usa personajes que son "objetos con cara", el mismo concepto que la comida con cara de *Boomerang Fu*. Y llama "los Inkcaps" a criaturas de tinta, lo que recuerda a los *Inklings* de *Splatoon*.
   - **Ninguna propuesta calcula si el equipo puede producir una temporada cada 6 semanas.** Cada una pide 1 arena, 1 modo y unos 10 cosméticos por temporada, y el equipo no son programadores.

---

## 2. Calificaciones

| Criterio (peso) | A · Marca primero | B · Juego primero | C · Mercado móvil |
|---|---|---|---|
| Diversión en celular (30 %) | 64 | **87** | 78 |
| Originalidad y distancia legal (25 %) | 60 | 76 | **80** |
| Encaje con INKCAP (25 %) | **92** | 78 | 80 |
| Viralidad (20 %) | 74 | 85 | **87** |
| **Total ponderado** | **72.0** | **81.6** | **80.8** |

Cómo se calcula, con B de ejemplo: 0.30 × 87 + 0.25 × 76 + 0.25 × 78 + 0.20 × 85 = **81.6**.

---

## 3. Propuesta por propuesta

### 3.1 B · "Juego primero" · 81.6 · 1.er lugar

**Por qué gana:** es la única que ataca los puntos débiles del núcleo que ya existe.

- **Le da algo que hacer al desarmado.** Hoy, en el prototipo, sin gorra no puedes golpear (`game.js`: el golpe exige `hasCap`). B propone un empujón que sirve para tirar rivales al láser, al tren o fuera de la azotea. Eso convierte los peligros de la arena en jugadas.
- **Mejora lo que criticaron de *Boomerang Fu*.** La gorra que se cae **regresa sola a los 3 s**, y así ya no hay que esperar quieto a que vuelva.
- **Crea momentos para clip.** Con el **desvío "Te la regreso"**, la gorra desviada vuelve hacia quien la lanzó y su rastro cambia de color, así que **puede noquear a su propio dueño**. En 10 segundos y sin sonido se entiende "¡se mató con su propia gorra!".
- **Tiene reglas visuales firmes:** aro de color bajo los pies, ícono con forma, rastro del color del dueño, poder que se ve por su efecto (brilla, gotea o crece) y no por su color, y piso tranquilo.
- **Lata Caliente** es el mejor modo para fiestas y niños de las 3 propuestas.
- **El muñequito "Lienzo"** es un juguete base de vinil blanco que cada artista "interviene". Repite en el juego el negocio real de los *art toys* y de INKCAP con sus artistas.
- **El "Kilometraje de gorra"** cuenta los KO de cada gorra: con 25 gana una costura dorada, con 100 un contador bordado y con 500 un acabado gastado. Hace que la gorra comprada se sienta tuya y no cuesta casi nada producirlo.

**Problemas reales:**

| Problema | Por qué importa | Arreglo |
|---|---|---|
| Dice que el empujón sin gorra es "solo nuestro" | En *Boomerang Fu*, sin bumerán, el golpe solo empuja (doc 01 §4.1). Cada regla igual suma parecido. | Conservarlo porque es divertido, pero sacarlo de la lista de ideas propias. La distancia la dan las reglas firma (§7). |
| La "Encasquetada" se activa con un toque a LANZAR justo antes de atrapar | En el prototipo, tocar LANZAR con la gorra en el aire **ya la hace regresar antes**: los dos usos chocan. Además, una ventana de 0.15 s es muy sensible al lag en línea. | Usar la regla de C: **atrapar la gorra durante un dash recarga el dash**. No necesita botón extra. |
| "Inundación de tinta": la arena se encoge con tinta líquida que entra por los bordes | Tinta líquida cubriendo el piso es la imagen de *Splatoon*, y encoger la arena es la muerte súbita de *Boomerang Fu*. | Que cada arena se cierre a su manera y sin líquido: rejas en el museo, cortinas metálicas en el callejón. |
| Propone skins de disco de vinil y de chancla | Rompe su propia regla de que todas las gorras tienen la misma silueta en vuelo. | Usarlos solo como reglas de evento donde **todos** juegan igual, por ejemplo un "Modo Chancla" el 10 de mayo. Nunca venderlos. |
| Pone CACHUCHAZO como 2.ª alternativa | "Cachucha" es vulgar en Argentina y Uruguay (C lo detecta). | Descartarlo. |
| 6 mecánicas nuevas a la vez, 9 modos y 10 arenas | Los novatos tendrían que aprender demasiado de golpe. | Agregarlas una por una y medir cada cambio con `HeadlessHost balance` y pruebas de sillón. |
| Costo de red de US$0–300 al mes con 10k jugadores diarios | La investigación 04 dice ~US$260–470. | Usar las cifras de C. |

### 3.2 C · "Mercado móvil" · 80.8 · 2.º lugar (casi empatada)

**Lo mejor:**

- **Tiraje (Serigrafía).** Las 2 copias siguen tu mismo trayecto 0.12 s y 0.24 s después, en lugar de salir en abanico. Se parece más a una impresión de serigrafía, ensucia menos la pantalla y **se aleja del multibumerán de *Boomerang Fu***, que se divide en 5.
- **Atrapar durante un dash recarga el dash.** Premia la habilidad sin agregar botones.
- **Modos que encajan con la cultura de México:**
  - **La Reta:** el que gana se queda y los demás hacen fila mandando emotes. Es ideal para *streamers* que juegan contra su público.
  - **Reto del Día:** la misma arena y las mismas condiciones para todo el país, con tabla nacional, al estilo Wordle. Aprovecha que la simulación de Unity siempre da el mismo resultado: el servidor puede repetir la partida para detectar trampas.
- **"Levántale la gorra":** en equipos, llevas la gorra de tu compañero caído hasta su mancha y revive. Es la forma de revivir más original de las 3, porque solo existe en un juego donde el arma es la gorra.
- **Disciplina de producto:**
  - la primera partida empieza en **menos de 30 s** desde que abres la app, sin cuenta;
  - se llega al nivel 4 en la primera sesión;
  - las partidas en línea son a 3 puntos (≈ 2.3 min);
  - cada partida gasta ≈ 1.2 MB de datos;
  - el permiso de notificaciones se pide después de la primera victoria y sin premio (regla 5.1.2(i) de Apple);
  - no se pasa de etapa sin cumplir las metas de cuántos jugadores regresan al día 1, al día 7 y al día 30 (D1/D7/D30).
- **Viralidad pensada para México:** enlace de sala por WhatsApp ("únete a mi reta: 4821"), clip vertical automático y concurso "Gorrazo de la semana" con playera física de premio.
- **Es la más cuidadosa con lo legal.** Descarta CACHUCHAZO, menciona a Cappy de *Mario Odyssey* ("la gorra no tiene ojos") y descarta el rodillo, que es arma de *Splatoon*. Pone el control de zona en muros, no en el piso, y pide un diario de diseño fechado como prueba de creación propia.

**Problemas reales:**

| Problema | Por qué importa | Arreglo |
|---|---|---|
| **"El color de tu gorra dice qué poder traes"** | La gorra ya lleva dos cosas: el color del jugador en el rastro y el diseño que compraste. Además, en el prototipo los colores de las latas se repiten con los de los jugadores: Serigrafía rosa y jugador magenta, Grafiti verde y jugador verde, Parche ámbar y jugadores naranja y amarillo. **Es la confusión visual más grave de las 3 propuestas.** | El poder se muestra por **efecto y forma** (brillo, goteo, tamaño) más un ícono chico en el aro. El color queda **solo** para identificar al jugador. |
| El desarmado sigue sin poder hacer nada | Correr hacia tu gorra no es una decisión interesante. | Agregar el empujón de B. |
| Solo se puede cargar 1 lata | Es más claro, pero se pierden las combinaciones que generan clips. | Permitir 2 latas, como A y B, con íconos en el aro. |
| El "Modo sencillo" choca con el botón de llamar la gorra | Sin gorra, LANZAR ya sirve para llamarla. | Probarlo con dos versiones (A/B) o dejarlo fuera. |
| Anuncios con premio desde el inicio | Para una marca de ropa urbana abaratan la imagen. Además, el kit de anuncios obliga a pedir permiso de rastreo en iOS y a actualizar las etiquetas de privacidad, y pesa en celulares de gama baja. Con 1,000 jugadores diarios dejan ≈ MXN 2,600 al mes. | Posponerlos a la etapa en línea. "Pruébatela" puede funcionar con 1 partida gratis en lugar de un anuncio. |
| Dice "lectura medida en pantalla de 6 pulgadas" | Es el cálculo del doc 08, no una medición real. | Medirlo en un Moto G06. |

### 3.3 A · "Marca primero" · 72.0 · 3.er lugar

**Lo mejor:**

- **La marca está en todo, y bien pensada:**
  - llavero TPU colgando de la gorra;
  - taza como trofeo del podio;
  - acabados de *art toy* como rarezas;
  - arena **Pop-up Store INKCAP**, donde cada 20 s cae una caja "DROP" cuya sombra avisa dónde va a caer;
  - set de **INKCAP TV** con un **reflector "EN VIVO" que sigue al líder**, para que el que va perdiendo pueda alcanzarlo;
  - firma del ganador en el mural;
  - postal del mural con un QR para descargar el juego;
  - votación de qué diseño se fabrica en físico;
  - código de creador para cada artista.
- **Argumento correcto sobre el día 7.** Ese día debe dar una **gorra**, porque en partida la gorra ocupa unas 3 veces más píxeles que la playera (doc 08). La playera pasa al calendario de 28 días como "Playera del mes".
- **Es la única que dice qué hacer si el juego fracasa:** usar "GORRAZO by INKCAP" para que la marca no quede expuesta.

**Problemas reales:**

| Problema | Por qué importa | Arreglo |
|---|---|---|
| **Modo "Edición Limitada"**: sumas puntos mientras llevas la Gorra #001 | Funciona igual que el modo *Golden Boomerang* de *Boomerang Fu*. Quitar la lentitud no cambia la idea. | Descartarlo. Usar Lata Caliente o Coleccionista. |
| **Personajes que son objetos con cara** (vasito, rasqueta, sticker, hongo) | Repite el concepto de *Boomerang Fu* (comida con cara). "Rasqueta", plana y alta, vista desde arriba es una línea. "Coprino" usa su sombrero de hongo como gorra, así que **no puede ponerse gorras INKCAP**: rompe el modelo de venta. | Elenco de animales *art toy* (B y C) con base común. Los objetos, como mucho, para skins de evento. |
| **Llamar "los Inkcaps" a los muñequitos** | Criaturas de tinta con nombre "Ink-…" recuerdan a los *Inklings* de *Splatoon*. El doc 07 pide evitarlo y Nintendo defiende fuerte sus marcas. | Usar INKCAP para la marca y el universo, no para las criaturas. |
| Subtítulo "Gorrazo **Brawl**" | "Brawl" es la palabra de *Brawl Stars*. Aporta poco y suma riesgo. | Usar *"Lanza tu gorra. Mancha a todos."* |
| Partidas de 2 a 8 jugadores | En una pantalla de 6" con un golpe = KO, 8 muñequitos son ilegibles, y en Wi-Fi local son más celulares que sincronizar. | Máximo 6, como B y C. |
| "Estampa dorada": solo se consigue canjeando una prenda física y es la única forma de completar el álbum | Presiona a comprar para completar, incluso a menores, y queda en zona gris con la regla 3.1.1 de Apple. | Que la prenda física dé solo una variante cosmética. El álbum se completa jugando. |
| Intercambio de estampas entre amigos desde el lanzamiento | Necesita servidor y protección contra estampas duplicadas o fraude. | Dejarlo para el año 2. |
| El "Pillo" en el elenco base | El juego dependería del personaje de otra persona. | Que llegue como invitado de temporada con contrato, como proponen B y C. |
| "Atrape con estilo" | No queda claro: atrapar siempre ocurre al final del regreso. | Reemplazarlo por la recarga del dash de C. |
| Clip de 2 s | Muy corto para TikTok. | De 6 s y vertical (B y C). |
| Costo de red de US$0–300 al mes | Subestimado (doc 04: ~US$260–470 con 10k jugadores diarios). | Usar las cifras de C. |
| Demasiadas capas de marca | Se corre el riesgo de un "advergame" aburrido. A misma lo reconoce. | Cumplir su propia regla: no agregar capas de marca hasta que 8 de cada 10 probadores pidan otra partida. |

---

## 4. Problemas que comparten las tres (y que nadie resolvió)

### 4.1 Confusión visual

- **Los colores del prototipo no son los de la paleta para daltónicos.** Todas las propuestas piden Okabe-Ito, pero `config.js` usa naranja, cian, amarillo, magenta, verde y violeta, y esos colores se repiten con los de las latas. **Hay que cambiar ambos antes de la primera prueba con gente.**
- **Mantener el piso calmado.** Los murales van en los muros. En el piso solo va la tinta de la partida, y con **textura de cada técnica** (semitono, puntadas, borde blanco de sticker), nunca brillante como baba. Así se separa de *Splatoon*.
- **El llavero que cuelga de la gorra** no debe cambiar el área de golpe ni tapar el rastro: máximo 25 % del tamaño de la gorra.

### 4.2 Costos ocultos o subestimados

| Concepto | Monto aproximado | ¿Quién lo menciona? |
|---|---|---|
| Red en línea con Unity Relay | ~US$20 al mes con 1k jugadores diarios; **~US$260–470 con 10k** | Solo C lo da bien |
| Servidor dedicado para ranked | US$750–1,200 al mes con 10k jugadores diarios | Las 3 |
| Mac para compilar iOS | Mac mini ≈ MXN 19,500, o Codemagic ≈ MXN 180 al mes | Ninguna |
| Registro en el IMPI | Nombre del juego en clases 9, 41 y 28 + INKCAP en 9 y 41 ≈ **MXN 16,500** de derechos, más honorarios de MXN 2,500–6,000 por clase | B y C, en parte |
| Lanzamiento suave | US$1,500–6,000 por 10,000 instalaciones | B y C |
| Etiquetas con código para raspar, página de canje web, soporte y fraude | No calculado ⚠️ | Ninguna |
| Murales y modelos 3D encargados a artistas, con contrato que cubra el videojuego | No calculado ⚠️ | A, sin monto |
| Música con licencia | No calculado (doc 08 §10) | Ninguna |
| Unity Pro si el **grupo** de empresas supera US$200 mil al año | US$2,310 por puesto al año | A y B |

### 4.3 Riesgos que todos tocan poco

- **Lag en el desvío y en el atrape.** En Wi-Fi local no hay problema. En línea, con 80–150 ms, las ventanas cortas se sienten injustas. Las ventanas de tiempo deben ser generosas, y la prueba de lag del núcleo Unity (README, paso 1) debe incluir el desvío.
- **Permisos de red local.** Apple debe aprobar un permiso especial para buscar partidas en la red local; la otra opción es Bonjour. Android 17 pedirá el permiso de red local. Sin eso, la Edición Fiesta no funciona.
- **El ritmo de temporadas.** Recomiendo **temporadas de 8 semanas** y una "temporada mínima" de 1 arena **o** 1 modo, más 8–10 cosméticos sacados del catálogo real. Una temporada cada 6 semanas es demasiado para un equipo pequeño.
- **Soft launch solo en Colombia, Perú y Chile.** El ciclo de la marca (prenda → juego → prenda) solo se puede medir en México. Hay que hacer al mismo tiempo una beta cerrada en México con el QR en las bolsas de envío, como propone A.

---

## 5. Ideas que deben injertarse en el diseño final

| # | Idea | Viene de | Por qué |
|---|---|---|---|
| 1 | **Desvío "Te la regreso":** la gorra vuelve a quien la lanzó, su rastro toma 0.6 s el color de quien la desvió y puede noquear a su dueño, con letrero "¡TE LA REGRESO!" | B | El momento más compartible de todos; se entiende sin sonido |
| 2 | **La gorra caída regresa sola a los 3 s** | B | Corrige la queja principal contra *Boomerang Fu* |
| 3 | **Atrapar durante un dash recarga el dash** (en lugar de la "Encasquetada") | C | Premia la habilidad sin botón nuevo y sin chocar con el botón de llamar la gorra |
| 4 | **Empujón sin gorra** (no noquea; empuja 2.5 m hacia los peligros) | B | El desarmado tiene algo que hacer y los peligros se vuelven jugadas. No presentarlo como idea propia. |
| 5 | **Doble KO = empate** | B y C | Es más justo y esconde el lag |
| 6 | **Latas: máximo 2, se pierden con el KO** y se reinician en cada ronda en línea; en local se pueden dejar persistentes | A y B | Evita que el que va ganando se escape y se aleja de *Boomerang Fu*, donde se acumulan 3 entre rondas |
| 7 | **El poder se ve por efecto y forma, nunca por color** (brillo, goteo, tamaño, más un ícono en el aro) | B (contra C) | El color queda solo para identificar al jugador |
| 8 | **Serigrafía "Tiraje"** (copias en el mismo trayecto) | C | Más original, más fiel a la técnica y más limpio en pantalla |
| 9 | **Sticker XL −10 % de velocidad** y **radio del ¡CLANK! −15 %** | C y B | Balance basado en las métricas de los bots |
| 10 | **Muerte súbita a los 20 s, distinta en cada arena y sin tinta líquida** (rejas en el museo, cortinas metálicas en el callejón) | C + B (alarma del museo) | Corta el 10 % de rondas largas sin parecerse a *Splatoon* |
| 11 | **Lata Caliente** como modo de fiesta en el MVP | B | Gritos garantizados y nadie queda fuera mucho tiempo |
| 12 | **La Reta** (el que gana se queda) con sala de *streamer* y "La Reta con…" en INKCAP TV | C y A | Formato barato, de barrio y hecho para transmisiones |
| 13 | **Reto del Día** nacional con la misma semilla, repetido en el servidor para detectar trampas, en formato Galería de Retos | C y B | Hábito diario sin servidor de partidas; aprovecha el núcleo determinista que ya existe |
| 14 | **"Levántale la gorra"** para revivir en Crews; si confunde en pruebas, usar el "Retoque" de A (pararse 2 s sobre la mancha) | C y A | Una forma de revivir que solo existe en este juego |
| 15 | **Una Pa' Todos** (una sola gorra que acelera con cada atrape) como regla semanal | C | Mucha tensión con un costo de producción mínimo |
| 16 | **Tagueo en muros** como modo de territorio de la Temporada 1 (une Estampa, Tagueo y Bombardeo) | B, C y A | Control de zona sin pintar el piso |
| 17 | **Arenas:** Pop-up Store con caja "DROP" (4.ª arena del MVP), apagón de neón en el Estudio de Tatuaje, tren que se lleva las gorras al otro extremo, reflector "EN VIVO" que sigue al líder en el set de INKCAP TV | A y B | Cada peligro estrella genera clips y lleva marca |
| 18 | **Cierre de partida para compartir:** mural con la firma del ganador y los nombres de todos, clip vertical de 6 s y postal con QR | A, B y C | El mural es lo único que nadie más tiene; hay que convertirlo en el objeto que se comparte |
| 19 | **Kilometraje de gorra** | B | La gorra comprada se vuelve tuya; casi no cuesta producirlo |
| 20 | **Muñequito "Lienzo"** de vinil blanco para que intervengan los artistas, más un elenco de animales *art toy* (oso, gato, gota, coneja, alien, diablito, **Axo** y **Xolo Rótulo**) | B y C | Se aleja de los objetos con cara y copia el negocio real de los *platform toys* |
| 21 | **El "Pillo" como invitado de temporada**, con contrato y reserva de derechos ante INDAUTOR | B y C | El elenco base no depende de un personaje ajeno |
| 22 | **Llavero TPU colgando de la gorra**, **acabados de art toy como rarezas** y **versión "legible" de 2–3 colores** de cada playera | A y C | Merch real que se ve y se lee en pantalla |
| 23 | **Día 7 = gorra** en la primera semana, alternando gorra y playera después; **Playera del mes** a los 28 días; racha suave que se pausa en lugar de reiniciarse | A y C | Lo gratis que más se ve en partida es lo que más antoja |
| 24 | **Primera partida en menos de 30 s, nivel 4 en la primera sesión**, notificaciones después de la primera victoria y sin premio | C | Para llegar a la meta de D1 ≥ 40 % |
| 25 | **Enlace de sala por WhatsApp** y concurso **"Gorrazo de la semana"** con playera física | C | Así se comparte en México |
| 26 | **Votación:** el traje digital más votado se fabrica en físico | A y B | Menos riesgo de inventario; modelo de Forever 21 en Roblox (1.5 M de gorros virtuales) |
| 27 | **En línea a 3 puntos, en local a 5** | C | Partidas de 2–3 min en el celular |
| 28 | **Letreros de KO:** "¡DE REGRESO!", "¡DE BANDA!", "¡TE LA REGRESO!", "¡GORRAZO!" | B | Cada clip se explica solo |
| 29 | **Modo Pantallota / Gorrazo Fest** (pantalla grande con los celulares como control) para eventos y tiendas, no para el MVP | C y A | Activación de marca en eventos de arte urbano |

---

## 6. Ideas que deben descartarse (o posponerse)

| Idea | Viene de | Razón |
|---|---|---|
| Modo "Edición Limitada" (puntos con la Gorra #001) | A | Funciona igual que el *Golden Boomerang* de *Boomerang Fu*: suma parecido sin aportar nada |
| Elenco base de objetos con cara (vasito, rasqueta, hongo) | A | Repite el concepto de comida con cara; la rasqueta no se ve desde arriba; el hongo no puede usar gorras de pago |
| "Los Inkcaps" como nombre de los muñequitos | A | Cercano a los *Inklings* de *Splatoon* en un juego de tinta |
| CACHUCHAZO | A y B | Vulgar en Argentina y Uruguay |
| "Brawl" en el nombre o subtítulo (INKCAP BRAWL, "Gorrazo Brawl") | A y B | Palabra ligada a *Brawl Stars*; aporta poco |
| Color de la gorra = poder | C | La señal se confunde con el color del jugador y con la gorra de pago |
| Inundación de tinta líquida como muerte súbita | B | Imagen de *Splatoon* más la mecánica de *Boomerang Fu* |
| Skins que cambian la silueta (disco de vinil, chancla) | B | Rompen la justicia y la lectura. Solo como reglas de evento iguales para todos |
| Encasquetada con toque a tiempo | B | Choca con el botón de regreso anticipado y es frágil con lag |
| Atrape con estilo | A | La regla no está clara |
| Partidas de 8 jugadores | A | No se leen en 6" |
| Estampa dorada solo con prenda física para completar el álbum | A | Presión de compra a menores y zona gris con Apple 3.1.1 |
| Intercambio de estampas al lanzar | A | Complejidad de servidor y fraude; año 2 |
| El Pillo en el elenco base | A | Dependencia de un personaje ajeno |
| Anuncios con premio antes de la etapa en línea | C | Abaratan la marca, traen trámites de privacidad y dejan poco (≈ MXN 2,600 al mes con 1k jugadores diarios) |
| **Posponer:** chanfle (curvar con el joystick al soltar) | B | Dos pulgares a la vez es difícil en pantalla táctil; probarlo después del MVP |
| **Posponer:** El Velador, Atraco al Museo, Coleccionista y Sonidero | B y C | Buenos para eventos y para la etapa en línea, no para el MVP. El Sonidero también requiere licencia de música |

---

## 7. Distancia frente a *Boomerang Fu*: las reglas firma

Cada mecánica por sí sola es libre. El riesgo, ante la regla 4.1 de Apple contra clones y la idea de "combinación de reglas" del caso coreano *King v. Avocado* (doc 07), es que **todo el paquete** se parezca. El diseño final debe tener al menos estas **8 reglas que *Boomerang Fu* no tiene**:

1. **La gorra en la cabeza indica que estás armado**, y esa misma gorra es el cosmético que compras.
2. **¡CLANK!:** si dos gorras chocan, las dos caen al piso.
3. **El mural:** las manchas se quedan en el piso y la partida termina en una postal.
4. **Desvío "Te la regreso"**, con el rastro cambiando de color.
5. **Regreso automático** de la gorra caída a los 3 s.
6. **Recarga del dash** al atrapar la gorra mientras haces dash.
7. **Latas de técnica** que se pierden con el KO (máximo 2), entre ellas el **Tiraje**.
8. **Modos propios:** Lata Caliente, La Reta, Reto del Día, "Levántale la gorra" y Tagueo en muros.

Además, como pide el doc 07, hay que **llevar un diario de diseño con fechas**, nunca pasarle capturas de *Boomerang Fu* a la IA y nunca usar "Fu", "Frisbee" ni "Boomerang" en el nombre ni en la tienda.

---

## 8. Nombre

| Lugar | Nombre | Por qué | Riesgo (estimado, sin búsqueda web ⚠️) |
|---|---|---|---|
| **1 (favorito)** | **GORRAZO** · tienda: *"GORRAZO by INKCAP"* (17 de 30 caracteres) · subtítulo: *"Lanza tu gorra. Mancha a todos."* | Es el grito del KO: se vuelve verbo ("échate unos gorrazos"), funciona en cualquier país de habla hispana (gorra + "-azo") y ya está en el prototipo. "by INKCAP" en el ícono y el título lleva la marca sin arriesgarla si el juego no funciona. | Bajo-medio. Es una palabra evocativa y registrable, pero puede haber marcas de gorras con ese nombre en la clase 25. Revisar las clases 9, 41 y 28. Pierde fuerza en inglés. |
| 2 | **INKAZO** | Palabra inventada que une INK con "-azo" ("golpe de tinta"): el registro más fuerte y el más ligado a la marca | Bajo. Se puede confundir al oído con "Inca", y quien no sabe inglés no capta el "ink" |
| 3 | **INKCAPS** | El máximo vínculo con la marca: cada descarga dice INKCAP | Medio. Si el juego falla, la marca queda expuesta, y recuerda a "Inklings". Funciona mejor como universo ("INKCAP presenta…") que como título |

**Descartados:** CACHUCHAZO (vulgar en el Río de la Plata), CAP CLASH (Supercell), SPLAT CAP (*Splatoon*), NO CAP (jerga común; sirve como eslogan), INKCAP BRAWL ("Brawl"). **MANCHADOS** queda como nombre de un evento o de un modo, porque su chiste solo se entiende en México.

---

## 9. Objeto arrojadizo

**Recomendación: la gorra del propio muñequito** (*snapback*). Las 3 propuestas le dan 23 de 25 puntos y estoy de acuerdo. Es la única opción que se lee en un instante (gorra puesta = armado), se ve unas 3 veces más que la playera, es literalmente la mitad del nombre de la marca y queda lejos del bumerán, del "Fu" y del disco volador de *Windjammers*.

**6 reglas para que funcione:**

1. **Todas las gorras miden lo mismo** (radio 0.38) y tienen **la misma silueta en vuelo**. Solo cambia la gráfica de la corona y la visera.
2. **El rastro siempre es del color del jugador** (Okabe-Ito) y la gorra en el aire siempre lleva **sombra**.
3. **Los poderes se ven por efecto o forma, nunca por el color de la gorra.**
4. **La gorra no tiene ojos ni vida propia**, para no parecerse a Cappy.
5. **Nunca decir "Frisbee".** Los objetos especiales (chancla, vinil) solo aparecen en reglas de evento donde todos juegan igual.
6. **Se lanza de lado y gira plana, como disco.** La física ya existe en el prototipo y en Unity.

---

## 10. Pendiente de verificar antes de gastar

1. Buscar conflictos de **GORRAZO, INKAZO e INKCAPS** en MARCia (IMPI), WIPO, USPTO, App Store y Google Play. No se pudo hacer en esta sesión porque se agotó el cupo de búsquedas.
2. Confirmar que la marca INKCAP cubra las **clases 9, 35 y 41** (ropa virtual) antes de anunciar trajes digitales.
3. Consultar con App Review la variante exclusiva "Edición Física" del canje en la web.
4. Hacer la prueba de lag con desvío y atrape a 50, 100 y 150 ms en el núcleo Unity.
5. Cambiar la paleta de jugadores a Okabe-Ito y separar los colores de las latas antes de las pruebas de sillón.
