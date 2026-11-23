# 08 · Dirección de arte, mundo y referencias creativas

> **Proyecto:** Boomerang Fighters (nombre de trabajo). Es un *party brawler* móvil (iOS + Android, Unity) con muñequitos estilo *art toy*, un objeto arrojadizo que regresa y escenarios de arte urbano. Usa cosméticos basados en merch real de INKCAP.
> **Fecha de corte:** 23 de noviembre de 2026.
> **Tipo de cambio usado:** 1 USD ≈ **19.10 MXN**, el mismo de los reportes 01 a 06 (FIX del DOF 19.0270 al 20-nov-2026, según [dolardof.com](https://dolardof.com/)). Las cifras en pesos están redondeadas.

**Nota de método (léela antes de usar las cifras):** este reporte se escribió con dos limitaciones de la sesión:

1. **El cupo de búsquedas web de este turno ya se había agotado.** Lo comparten todos los agentes de la investigación y lo consumieron los reportes 01 a 07.
2. **La red de la sesión bloqueó varios dominios** que pensaba consultar directamente: Wikipedia, la documentación de Unity (docs.unity3d.com) y gameaccessibilityguidelines.com. Habría que permitirlos en la configuración de red de la sesión para verificarlos.

Por eso cada dato lleva una etiqueta:

- ✅ **Verificado hoy:** el dato tiene URL y fue verificado hoy por otro reporte de esta misma investigación (01 a 06). Se reutiliza tal cual.
- ⚠️ **Conocimiento previo:** dato que conozco de fuentes publicadas hasta 2025 o mediados de 2026, pero que **no pude abrir hoy**. Es confiable en lo general, pero conviene re-verificarlo antes de tomar decisiones de dinero o legales.
- 💡 **Propuesta o cálculo propio:** idea de diseño, regla práctica o número calculado por mí. No es un dato publicado.

En dirección de arte, la mayor parte del valor está en criterios de diseño y no en precios. Por eso estas limitaciones afectan menos aquí que en otros reportes. Aun así, lo digo con claridad.

---

## 1. Resumen ejecutivo (2 minutos)

1. **La gran idea de arte:** usar las **6 técnicas de tinta de INKCAP** (grafiti, tatuaje, serigrafía, bordado, sticker y tinta derramada) como **seis "lenguajes visuales"** del juego. Cada técnica define sus efectos, sus KO, su interfaz y su temporada. Así no nos parecemos a *Splatoon*, que se basa en tinta líquida brillante, ni a *Jet Set Radio*, que se basa en el *cel-shading* y en hacer *tags*. 💡
2. **Los muñequitos deben ser "plataforma de juguete":** un cuerpo base propio que cada artista "interviene", como se hace con los *platform toys* del mundo *designer toy* (Be@rbrick de Medicom o Munny y Dunny de Kidrobot) ⚠️. Esto calca el negocio real de INKCAP: artistas que colaboran y cobran 15 % de regalías.
3. **Proporción recomendada:** **2 a 2.5 cabezas de alto**. La cabeza ocupa el 45–50 % de la altura total, las manos son "mitones" sin dedos y no hay cuello. Las uniones visibles imitan las de una figura de vinil. 💡
4. **Los acabados reales de coleccionables se vuelven rarezas:** vinil mate → satinado → brillante → translúcido → cromado → flocado → brilla en la oscuridad. Así la sección "Edición Limitada" de la tienda tiene su equivalente directo en el juego. 💡
5. **Con cámara cenital, la gorra se ve mucho más que la playera.** Según mi cálculo, la parte de arriba de la gorra ocupa **unas 3 veces más píxeles** que el estampado del pecho (ver §3.3). Conclusión: **las playeras se lucen en el vestidor, en el podio y en la repetición del KO; las gorras se lucen durante la pelea.** 💡
6. **El piso ocupa ~70 % de la pantalla y debe estar "calmado".** El grafiti fuerte va en los muros y bordes de la arena, no en el piso. En el piso solo va la tinta que deja el juego. 💡
7. **El color de jugador no puede depender de la ropa.** Si la gente compra playeras, el color de equipo o de jugador va en un **aro bajo los pies**, en el **rastro del proyectil** y en un **ícono con forma**. Para eso usamos la paleta Okabe-Ito, que es segura para daltonismo ⚠️.
8. **Hay mucha cultura visual de CDMX que podemos usar** (rótulos hechos a mano, lonas rosas de tianguis vistas desde arriba, azoteas con tinacos, Metro, lucha libre, sonideros, tortillerías y talleres de serigrafía). **Ojo legal:** en México los derechos de autor duran **la vida del autor más 100 años** ⚠️. Los murales de Rivera, Siqueiros u Orozco y los pictogramas del Metro **no se pueden copiar**; hay que inspirarse sin reproducir.
9. **"Game feel" mínimo para que se sienta bien:**
   - pausa de impacto (*hit stop*) de 60–100 ms en cada golpe;
   - estirar y aplastar (*squash & stretch*) en saltos, lanzamientos y atrapadas;
   - sacudida de cámara suave;
   - vibración del teléfono;
   - un KO temático que deje **una mancha de tinta en el piso del color del jugador**.
   
   Todos los valores son puntos de partida 💡.
10. **Rendimiento:** se respeta el presupuesto del reporte 03 ✅: 2,500–5,000 triángulos por personaje, texturas de 512 px, 30 fps estables en gama baja y 60 fps en gama media. Aquí agrego reglas de arte para lograrlo: atlas de paleta, sombras falsas, partículas opacas en vez de transparentes y pintura de piso en **una sola textura de 512 × 512** en lugar de cientos de calcomanías.

---

## 2. Estética *art toy* / *designer toy* / *sofubi* / chibi

### 2.1 Qué es y de dónde viene ⚠️

- Los ***designer toys*** o ***art toys*** son figuras coleccionables de autor, en tirajes cortos. Nacieron a finales de los años 90 en **Hong Kong** (Michael Lau, Eric So) y **Japón**, y se extendieron en los 2000 con **Kidrobot** (EE.UU., 2002) y **Medicom Toy** (Japón; su Be@rbrick salió en 2001).
- En los años 2020 el fenómeno se volvió masivo con **Pop Mart** (China) y sus *blind boxes*. Labubu fue el gran éxito de 2024–2025.
- El ***sofubi*** (ソフビ, "vinil suave") es el vinil japonés hueco, fabricado con moldes rotacionales y pintado a mano o con aerógrafo. Viene de los juguetes de *kaiju* de los años 60 y 70. Su acabado es imperfecto: degradados de aerógrafo, ligeros desfases de pintura y brillos satinados.
- Los ***platform toys*** son una sola silueta base, siempre igual, que cada artista decora con su propio diseño. Los ejemplos clásicos son **Be@rbrick**, **Munny/Dunny** y **Qee**. **Este concepto es casi idéntico a lo que queremos:** un muñequito base al que se le cambian playera, gorra y "pintura" de artista.

### 2.2 Cómo se ven en 3D (lo que hay que imitar) 💡

| Rasgo de un *art toy* real | Cómo se traduce al modelo 3D del juego |
|---|---|
| Superficies lisas, sin detalle fino | Pocos polígonos bien distribuidos; detalles pintados en textura, no modelados |
| **Líneas de unión** (cuello, hombros, cadera) donde se separan las piezas del molde | Una ranura marcada en el modelo o en la textura. Es la "firma" de juguete y además sirve como borde para cambiar la ropa |
| Brillo satinado de vinil (ni mate total ni espejo) | Sombreado **matcap** o *toon* con un brillo suave y ancho (ver doc 03 ✅) |
| Pintura con aerógrafo: degradados suaves en mejillas y puntas | Degradado por color de vértice o en la textura de 512 px |
| Base o peana de exhibición | En menús y en el vestidor, el muñequito aparece **sobre una peana con su tarjeta de coleccionista** |
| Caja de colección, a veces *blind box* | Animación de "desempaque" al obtener un traje (**sin azar pagado**; ver reporte 05/06) |

### 2.3 Proporciones recomendadas 💡

- **Altura total:** 2 a 2.5 cabezas. La cabeza mide 45–50 % del alto.
- **Cuerpo:** forma de "frijol" o "gota" corta, sin cuello. Brazos cortos con manos esféricas o tipo mitón (sin dedos: ahorran polígonos y se ven más juguete). Pies como bloques redondeados.
- **Cara:** ojos grandes y separados. Pueden ser una **textura intercambiable**, lo que da expresiones sin necesidad de *blendshapes* caros. La boca es opcional (Be@rbrick y muchos *sofubi* no tienen).
- **Silueta propia, que no se parezca a nadie:**
  - **Evitar:** la forma de oso de Be@rbrick (Medicom la protege como marca/diseño ⚠️), el frijol de *Fall Guys*, el conejo-monstruo de Labubu y la comida con cara de *Boomerang Fu*.
  - **Propuesta:** una **cabeza en forma de gota de tinta** (redonda abajo, con un pico suave arriba) o una **cabeza que recuerde la tapa de una lata de aerosol**. Las dos son coherentes con la marca, se leen desde arriba y ninguna está tomada.
- **La mascota "pillo" de uno de los artistas** puede ser el primer personaje "intervenido": la plataforma base pintada y vestida como ese alter ego. Sería ideal como personaje de portada, siempre con contrato que cubra el uso en videojuego (ver §4.4).

### 2.4 Acabados de coleccionable = sistema de rarezas 💡

| Rareza | Acabado inspirado en *art toys* reales | Efecto en el *shader* (barato) |
|---|---|---|
| Común | Vinil mate | Matcap mate |
| Poco común | Satinado | Matcap con brillo suave |
| Raro | Brillante / "gloss" | Matcap brillante + borde de luz |
| Épico | Translúcido ("jelly", "clear") | Transparencia falsa con *fresnel*, sin transparencia real |
| Legendario | Cromado / metálico | Matcap de metal |
| Edición Limitada | Flocado (terciopelo) o **resina pintada a mano** | Textura de ruido fino + borde suave |
| Evento | Brilla en la oscuridad (GID) | Emisivo que se nota en arenas de noche |

**Ventaja extra:** el estudio de escaneo 3D del cliente puede **escanear los art toys físicos de edición limitada** y convertirlos en personajes del juego. También puede ir al revés: **imprimir en 3D o producir en vinil** a los muñequitos más populares del juego (ver doc 03 §5.1 ✅).

---

## 3. Juegos con personajes tipo juguete: qué los hace legibles

### 3.1 Comparativo

| Juego | Personajes | Cámara | Qué los hace legibles | Qué tomamos | Qué evitamos |
|---|---|---|---|---|---|
| **Fall Guys** (Mediatonic, 2020; hecho en Unity ✅ [DEV](https://dev.to/wlz/why-fall-guys-bet-on-unity-the-boring-phone-engine-behind-a-32-player-bean-battle-royale-28e9)) | Frijol con ojos | 3ª persona | Silueta única e idéntica para todos; la variedad está en colores, patrones y trajes. Los trajes se dividen en **parte de arriba y parte de abajo** ⚠️ | Cuerpo base fijo + cosméticos por piezas | Copiar el frijol o la estética de "gomita" |
| **Stumble Guys** (Kitka → Scopely) | Muñecos *low-poly* de colores | 3ª persona | Colores saturados y formas simples; corre en Android de gama baja ✅ (reporte 02) | Ligereza técnica | Lucir como clon barato |
| **Party Animals** (Recreate Games, 2023; móvil anunciado en jul-2026 ✅ [TechNode](https://technode.com/2026/07/03/hit-party-game-party-animals-heads-to-mobile-bringing-furry-chaos-to-smartphones/)) | Animalitos peludos con física de muñeco de trapo | 3ª persona | Ternura y animación física exagerada; los clips se vuelven virales | Humor físico y KO cómicos | Pelo con *shader* caro, que es inviable en gama baja |
| **Brawl Stars** (Supercell) | Personajes con siluetas muy distintas | **Cenital inclinada** | Aro de color bajo los pies, barras de vida sobre la cabeza, siluetas exageradas y arenas de colores limpios ⚠️ | **El estándar del género en móvil:** aro de equipo, controles y legibilidad | Su estilo caricaturesco "Supercell" |
| **Overcooked** (Ghost Town Games) | Chefs simples | Cenital | Personajes chicos pero con colores planos, sin ruido; peligros del escenario clarísimos ⚠️ | **Escenarios que cambian y peligros legibles** | — |
| **Astro Bot** (Team Asobi, Juego del Año en The Game Awards 2024 ⚠️) | Robot de plástico brillante | 3ª persona | **Materiales de juguete** muy claros (plástico, goma, metal) y animación con mucho *juice* | Que cada material "suene" y "se sienta" | Su presupuesto de consola |
| **Boomerang Fu** (Cranky Watermelon) | Comida con cara | **Cenital** | Personajes redondos de colores planos y sombras simples ✅ (reporte 01) | La cámara y la claridad | La comida con cara y el bumerán |

### 3.2 Reglas de legibilidad que repiten todos 💡

1. **Silueta reconocible a 64 px.** Si con el muñequito relleno de negro no se sabe qué es ni hacia dónde mira, falla.
2. **Contraste de valor (claro/oscuro) entre personaje y piso.** El piso va en valores medios y desaturados. El personaje va con colores saturados y un contorno oscuro.
3. **Prueba de "entrecerrar los ojos" y prueba en escala de grises.** Hay que poder distinguir a los jugadores incluso sin color.
4. **Un solo punto focal por personaje,** que normalmente es la cabeza o la gorra.
5. **Sombra bajo cada personaje y bajo el proyectil.** En vista cenital, la sombra es lo que dice dónde está algo en el piso y a qué altura va.
6. **Las zonas de los pulgares estorban.** Las esquinas inferiores quedan tapadas por los dedos. La cámara debe encuadrar la acción un poco hacia arriba del centro.

### 3.3 Cálculo: ¿qué tanto se ve una playera en el juego? 💡

Supuestos del ejemplo:
- Teléfono de 2400 × 1080 en horizontal.
- La cámara muestra ~18 m de ancho de arena, así que salen **~133 píxeles por metro**.
- La cámara está inclinada a 60°.
- El muñequito mide 0.9 m.

Con esos supuestos:

| Zona del muñequito | Tamaño real (aprox.) | Píxeles en pantalla | Por qué |
|---|---|---|---|
| Estampado del pecho | 0.25 × 0.25 m | **~33 × 17 px** | Es una superficie vertical; con la cámara a 60° se "aplasta" a la mitad (cos 60° = 0.5) |
| Parte de arriba de la gorra | 0.35 m de diámetro | **~47 × 40 px** | Es una superficie horizontal; casi no se aplasta (sen 60° ≈ 0.87) |
| Espalda de la playera | 0.25 × 0.30 m | ~33 × 20 px | Solo se ve cuando el personaje se aleja de la cámara |

**Conclusión práctica:**
- En la pelea, la playera se lee como **color de tela + una mancha gráfica grande**.
- El diseño completo del artista se aprecia en el **vestidor**, la **pantalla de victoria**, el **podio** y la **repetición del KO en primer plano**.
- La **gorra** es el cosmético más visible en partida, así que conviene **diseñar el gráfico de la gorra también en la corona y la visera**, no solo en el frente.
- Por eso las gorras pueden tener un precio mayor o una rareza más alta.

---

## 4. Arte urbano en videojuegos: qué tomar y cómo no copiar

### 4.1 Referencias ⚠️

| Juego | Qué hace con el arte urbano | Lección |
|---|---|---|
| **Jet Set Radio** (Sega/Smilebit, 2000) | Pionero del ***cel-shading***. Pandillas en patines pintan *tags* en una Tokio estilizada. La música fue clave (Hideki Naganuma) | La actitud y la música definen la identidad tanto como los gráficos |
| **Bomb Rush Cyberfunk** (Team Reptile, 2023) | Sucesor espiritual de JSR: *cel-shading*, grafiti como mecánica de territorio, música con aportes de Naganuma | Un estudio pequeño puede revivir un estilo, pero lo comparan sin piedad con el original |
| **Splatoon** (Nintendo, 2015 →) | La **tinta es la mecánica**: pintar territorio, moverse por tu color y equipos con colores que cambian en cada partida. Tiene una opción de "bloqueo de color" para daltonismo | El gran referente de "tinta + juego de equipo". **Justo por eso no debemos parecernos** |
| **Sunset Overdrive** (Insomniac, 2014) | Estética punk, colores chillones, grafiti y letreros como decoración y humor | El humor y la irreverencia también son dirección de arte |
| **Marvel's Spider-Man / Miles Morales** (Insomniac) | Murales y arte callejero como ambientación y coleccionables del mundo abierto | Los murales cuentan la identidad del barrio |
| **Tony Hawk's Pro Skater** | Modo multijugador **"Graffiti"**: cada truco "marca" con tu color las superficies donde lo haces, y gana quien marque más | **Gran referencia para un modo de territorio**: marcar el escenario al golpear o atrapar |

### 4.2 Cómo NO copiar (y en qué sí diferenciarnos) 💡

| No hacer | Por qué | Hacer en su lugar |
|---|---|---|
| *Cel-shading* con contorno grueso y ciudad futurista | Es la firma de JSR y Bomb Rush Cyberfunk | **Vinil de juguete** (matcap satinado) en un **CDMX real y estilizado** |
| Tinta líquida brillante como "baba" y mecánica de nadar en tu color | Es la firma de Splatoon | Tinta con **textura de técnica**: puntos de semitono de serigrafía, puntadas de bordado, borde blanco de *sticker* |
| Botón de *tag* con secuencias de dirección | Es mecánica de JSR | El grafiti es **consecuencia** del combate (KO y rastros), no un minijuego aparte |
| Copiar grafitis reales de muros fotografiados | Riesgo legal (ver §4.4) y falta de identidad | **Murales comisionados a artistas INKCAP** con contrato que cubra el videojuego |

### 4.3 Nuestra firma: las 6 tintas como lenguajes visuales 💡

| Técnica INKCAP | Lenguaje visual en el juego | Ejemplos de uso |
|---|---|---|
| **Grafiti** | Bordes difuminados de aerosol, escurridos, letras "wildstyle" | Rastro del proyectil, KO "spray", arena del callejón |
| **Tatuaje** | Línea negra gruesa + color plano (estilo tradicional), letras "Chicano" finas | Interfaz de rangos, KO "flash de tatuaje", arena del estudio |
| **Serigrafía** | Semitonos, **registro desfasado** (colores corridos), 2–4 tintas planas | Pantalla de victoria, cartel de temporada, filtro de repetición |
| **Bordado** | Puntadas, hilo, contornos con relieve | Insignias de nivel, parches de logro, KO "se descose" |
| **Sticker** | Borde blanco troquelado, brillo, esquinas que se levantan | Emotes, chat rápido, colección de logros, KO "se pega en la pared" |
| **Tinta derramada** | Manchas líquidas y salpicaduras | Charcos-trampa, transición entre pantallas, KO clásico |

**Bonus:** cada temporada del pase (6–8 semanas, ver reportes 02 y 05 ✅) puede dedicarse a una técnica y a un artista invitado. Con 6 técnicas hay material para el primer año.

### 4.4 Riesgos legales del arte en juegos ⚠️

- **Tatuajes:** en EE.UU. ha habido demandas de tatuadores contra juegos de deportes que recrearon los tatuajes de atletas reales. En *Solid Oak Sketches v. 2K* (NBA 2K) ganó la editorial en 2020. En *Alexander v. Take-Two* (WWE 2K), en 2022, un jurado dio la razón a la tatuadora con una indemnización modesta.
- **Grafiti:** varios grafiteros han demandado a marcas de moda y a anunciantes por usar sus murales sin permiso, y también es conocido el caso **5Pointz** (Nueva York, 2018). El grafiti tiene autor y derechos.
- **México:** la Ley Federal del Derecho de Autor protege las obras durante **la vida del autor más 100 años** ([LFDA](https://www.diputados.gob.mx/LeyesBiblio/pdf/LFDA.pdf)). Esto incluye a Diego Rivera (†1957), Frida Kahlo (†1954), Siqueiros (†1974) y Orozco (†1949). Sus derechos patrimoniales siguen vigentes; los de Rivera y Kahlo los administra un fideicomiso en el que el Banco de México es fiduciario ⚠️.
- **Qué hacer:** firmar con cada artista un **contrato de licencia o de obra por encargo** que diga de forma explícita:
  - videojuego y medios digitales;
  - territorio mundial;
  - derecho a modificar o adaptar (recolorear, reducir, animar);
  - duración;
  - uso en publicidad y tiendas de apps;
  - regalía del 15 % sobre la venta del cosmético.
  
  Ver el reporte 06 para el cálculo de regalías.

---

## 5. Museos y galerías en videojuegos

### 5.1 Referencias y mecánicas típicas ⚠️

- **Golpes (*heists*) en museos y galerías:** *Payday 2* tiene un golpe de galería de arte; la serie *Sly Cooper* gira en torno a robos con sigilo y museos; *Hitman* tiene niveles en eventos y subastas. Comparten mecánicas: **láseres de seguridad**, **guardias con conos de visión**, **vitrinas que se rompen** y **alarmas que cierran rejas**.
- **Party games:** las arenas de *Gang Beasts* (trenes, espectaculares, grúas) demuestran que **un escenario con un peligro claro e icónico** genera momentos memorables y clips.

### 5.2 Mecánicas de museo para nuestro brawler 💡

| Elemento | Mecánica | Por qué funciona en vista cenital |
|---|---|---|
| **Láseres de seguridad** | Rayos que barren o parpadean. Si los tocas suena la alarma: las rejas se cierran y la arena se encoge 10 s | Líneas rojas brillantes sobre piso oscuro: lo más legible que hay |
| **Esculturas en pedestal** | Cobertura que se destruye en 2 golpes; al caer suelta piezas que ruedan | Las piezas que ruedan dan física cómica |
| **Cuadros giratorios** | Paredes que giran como puertas secretas al darles con el proyectil | Atajos y sorpresas |
| **Piso pulido** | Inercia o deslizamiento en ciertas salas | Se marca con un brillo de matcap en el piso |
| **Vitrina de art toys** | Escaparate de los art toys reales de INKCAP, con botón "Ver en la tienda" fuera de la partida | Monetización sin estorbar (reporte 06 ✅) |
| **Guardia o robot de limpieza** | Un NPC con cono de visión: si te ve, te empuja | El cono se ve perfecto desde arriba |
| **Sala de luz negra** | Todo se oscurece y solo brillan los rastros y los acabados GID | Lucimiento de cosméticos raros |

---

## 6. Cultura visual de CDMX y México

| Elemento | Qué aporta visualmente | Uso en el juego 💡 | Cuidado |
|---|---|---|---|
| **Muralismo** (Rivera, Siqueiros, Orozco; Palacio Nacional, SEP, Bellas Artes, Polyforum; el campus de CU de la UNAM es Patrimonio Mundial desde 2007 ⚠️) | Figuras monumentales, color terroso y narrativa | Murales **nuevos** de artistas INKCAP con composición "muralista" | **No copiar obras** (vida + 100 años) |
| **Grafiti mexicano** (escena desde los años 80–90, con influencia de las placas cholas y chicanas) | *Wildstyle*, *throw-ups*, placas, letras chicanas | Muros del callejón y del Metro, *sprays* de rastro | Comisionar, no fotografiar muros ajenos |
| **Rótulos populares** (rotulación a mano de taquerías, tortillerías y misceláneas) | Letras con sombra y contorno, colores chillones, dibujos ingenuos | **Tipografía de la interfaz** y letreros de las arenas; homenaje a un oficio que se pierde | Contratar rotulistas reales para la tipografía: es una gran historia de marca |
| **Lucha libre** (CMLL desde 1933; patrimonio cultural intangible de CDMX desde 2018 ⚠️) | Máscaras, rings, luces, drama | **Arena de lucha libre**, máscaras como gorras alternativas, burlas de luchador | No usar máscaras de luchadores reales (son marcas) |
| **Tianguis** | **Lonas rosas, azules y naranjas** que **desde arriba son rectángulos de color perfectos** | Arena del tianguis con techos de lona que tapan la vista y se rasgan | — |
| **Metro** (inaugurado en 1969; iconografía de pictogramas de Lance Wyman ⚠️) | Pictogramas por estación, naranja y rosa, andenes | Arena del andén y vagón; **sistema de íconos propio** inspirado en ese lenguaje | Los pictogramas y el logo del Metro **no se copian** |
| **Azoteas** | Tinacos negros, tendederos, antenas, jaulas de tendido, perros | Arena de azotea con bordes sin barandal | No usar logos de marcas de tinacos |
| **Tortillerías** | Máquina, banda transportadora, comal, papel de estraza | Arena con banda y prensa como peligros | — |
| **Papel picado** (tradición poblana ⚠️) | Patrones calados de colores | Confeti de KO, banderines que se cortan con el proyectil, marcos de interfaz | — |
| **Estudios de tatuaje** | Hojas de *flash*, neón, sillón, máquinas | Arena del estudio, interfaz de rangos | — |
| **Talleres de serigrafía** | Pulpo giratorio, mallas, rasquetas, túnel de secado | Arena con plataforma giratoria y secador | Mostrar el proceso real de INKCAP: contenido para el podcast |
| **Sonideros y fiestas de barrio** | Bocinas gigantes, luces, cumbia | Arena con **bocinas que empujan al ritmo de la música** | — |

**Regla de oro:** CDMX **estilizado**, no fotográfico. Formas simples, paleta controlada y detalles "de juguete": el tianguis visto como un diorama de *art toy*.

---

## 7. Sistema de personalización modular

### 7.1 Anatomía del muñequito 💡

| Ranura | Técnica en Unity | Visibilidad en partida | Monetización |
|---|---|---|---|
| **Cuerpo base** (piel o "pintura" del vinil) | `SkinnedMeshRenderer` + textura de 512 px | Alta | Pintura de artista = personaje |
| **Playera** | Parte de la malla del cuerpo (evita que la piel atraviese la tela) con *shader* "PlayeraINKCAP" y segundo canal UV para el estampado (doc 03 §5.2 ✅) | Media (§3.3) | **Merch real INKCAP** |
| **Gorra / sombrero / máscara** | Malla rígida pegada al hueso "Cabeza" | **Muy alta** | Merch real + rarezas |
| **Accesorio de espalda** (mochila, lata, alas) | Malla rígida pegada al hueso "Pecho" | Alta al correr | Pase de temporada |
| **Cara / expresión** | Intercambio de textura | Media | Gratis o por XP |
| **Skin del proyectil** | Malla + rastro del color del jugador | Muy alta | Pase / tienda |
| **Efecto de KO** | Prefab de partículas por técnica | Alta (momento viral) | Tienda |
| **Burla / emote y pose de victoria** | Animación | Podio | Tienda |
| **Peana y tarjeta** | Objeto del vestidor | Menús | Logros |

### 7.2 Cómo poner diseños reales de playeras 💡

1. **Área de impresión con la proporción real.** El área UV del pecho debe tener la misma proporción que el área de estampado real que usa INKCAP en su producción (por ejemplo, el frente estándar de su serigrafía). Así el PNG del diseño entra sin deformarse.
2. **Dos opciones técnicas:**
   - **A. UV dedicada (recomendada):** la isla del pecho mapea a 0–1 en UV1. Es la opción más barata y estable (doc 03 ✅).
   - **B. Proyección de calcomanía en el *shader*:** se proyecta el PNG desde el espacio local del hueso "Pecho". Es flexible si hay varios cuerpos distintos, pero se deforma en los costados.
   - Las calcomanías de pantalla de URP (*Decal Projector*) **no conviene usarlas en personajes** en gama baja. Son para el escenario y aun ahí con moderación ⚠️.
3. **Versión "legible" de cada diseño:** el artista entrega el diseño completo para el vestidor y una **versión simplificada** (silueta y 2–3 colores) que se vea a 33 × 17 px. La herramienta del doc 03 genera las dos texturas (512 y 128–256 px).
4. **Combinar en una sola llamada de dibujo:** en partida, las texturas de cuerpo, playera y gorra se empaquetan en tiempo de ejecución en **un atlas por jugador** y las mallas se combinan en una sola. Así cada muñequito cuesta **1 llamada de dibujo**, no 4.

### 7.3 Colores de jugador y equipo (independientes de la ropa) 💡

- **Aro bajo los pies + flecha de dirección** del color del jugador, en **todos** los modos.
- **Rastro del proyectil** del mismo color. Ayuda a saber de quién es cada disparo.
- **Ícono con forma** sobre la cabeza (círculo, triángulo, cuadro, estrella, rombo, cruz, corazón, hexágono). **La forma identifica aunque no veas el color.**
- **Paleta de 8 jugadores basada en Okabe-Ito** ([jfly](https://jfly.uni-koeln.de/color/) ⚠️): `#E69F00` naranja, `#56B4E9` azul cielo, `#009E73` verde azulado, `#F0E442` amarillo, `#0072B2` azul, `#D55E00` bermellón, `#CC79A7` rosa púrpura y `#FFFFFF` blanco con contorno negro.
- **Modo por equipos:** **azul `#0072B2` contra naranja `#E69F00`**. Es la pareja más segura para daltonismo.
- **Opción "mis aliados siempre azules"** (como el bloqueo de color de Splatoon ⚠️).

### 7.4 Juegos que venden ropa de marcas reales ⚠️

| Juego | Ejemplos | Lección para INKCAP |
|---|---|---|
| **Fortnite** | Colaboraciones con Balenciaga (2021), Ralph Lauren y Jordan/Nike; Ralph Lauren vendió botas físicas después de lanzar los ítems virtuales ✅ ([Modern Retail](https://www.modernretail.co/technology/how-ralph-laurens-phygital-boot-sale-in-fortnite-shows-benefits-and-limits-of-virtual-commerce/)) | Lo digital puede empujar la venta física, aunque el volumen físico es modesto |
| **Roblox** | Gucci Garden (2021): un bolso virtual se revendió por cientos de miles de Robux. Mundos Nikeland y Vans World. Forever 21 vendió un gorro virtual **1.5 M veces** antes de fabricarlo ✅ (reporte 02) | **Lo virtual sirve para probar diseños**: el traje más vendido se vuelve playera real |
| **Free Fire** | Colaboraciones con moda (Balmain) y anime. En México, la colaboración con *Jujutsu Kaisen* dejó ~US$2 M en una semana (dic-2025) ✅ ([Sensor Tower](https://sensortower.com/blog/2025-q4-android-top-5-third-person-games-units-mx-60214330241bc16eb89ce15d)) | Las colaboraciones generan picos de ingreso en México |
| **Fall Guys** | Trajes por piezas (arriba y abajo) y muchas colaboraciones con franquicias | **Modularidad**: más combinaciones con menos contenido |

---

## 8. Animación y "game feel"

### 8.1 Principios ⚠️

- ***Squash & stretch*** (estirar y aplastar): es uno de los 12 principios de animación de Disney (Thomas y Johnston, *The Illusion of Life*, 1981). En un muñequito de vinil queda perfecto: rebota como juguete.
- ***Hit stop***: congelar el juego unos cuadros al golpear para que el impacto "pese". Masahiro Sakurai (*Smash Bros.*) lo explica en su canal de YouTube sobre diseño de juegos.
- ***Juice***: las charlas *"Juice it or lose it"* (Jonasson y Purho, 2012) y *"The Art of Screenshake"* (Jan Willem Nijman, Vlambeer, 2013) muestran cómo partículas, sacudidas y sonidos convierten un prototipo plano en algo adictivo. También está el libro *Game Feel* de Steve Swink.

### 8.2 Valores de partida (para ajustar en pruebas) 💡

| Efecto | Valor inicial | Nota |
|---|---|---|
| *Hit stop* en KO normal | **60–100 ms** (4–6 cuadros a 60 fps) | Congela a atacante y víctima, no la música |
| *Hit stop* en el último KO de la ronda | 150–200 ms + **cámara lenta al 30 % por 0.5 s** + acercamiento | El momento de "clip" |
| *Squash & stretch* | Escala 1.2 × 0.8 (conservando volumen) por 80–120 ms | En el lanzamiento, la atrapada, el aterrizaje y al recibir el golpe |
| Sacudida de cámara | 0.1–0.2 m durante 0.15 s, con caída rápida | **Opción para apagarla** (accesibilidad) |
| Vibración | Pulso corto al atrapar; fuerte en el KO | Core Haptics en iOS; vibración en Android |
| Anticipación al lanzar | 2–3 cuadros de "jalar hacia atrás" | Lo justo para leerse sin sentirse lento |
| Proyectil | **1.5–2×** su tamaño "realista", con sombra en el piso y rastro de color | Legibilidad cenital |
| Destello de impacto | 1 cuadro blanco sobre la víctima | Barato y muy efectivo |

### 8.3 Partículas de tinta sin matar el rendimiento 💡

- **Pintura del piso con una sola *RenderTexture*.** Cada mancha se "estampa" en una textura de **512 × 512** que cubre el piso de la arena (proyección desde arriba), y el *shader* del piso la lee. Es la misma idea general que usan los juegos de pintar territorio.
  - Costo de memoria: **1 MB** en RGBA8 o 256 KB en un canal; a 1024 px serían 4 MB.
  - Es mucho más barato que cientos de calcomanías y permite un **modo de territorio** al estilo del modo "Graffiti" de Tony Hawk.
- **Partículas opacas o con recorte alfa** en vez de transparentes. La transparencia superpuesta es lo más caro en GPUs Mali y Adreno de gama baja ⚠️.
- **Máximo 50–150 partículas vivas** en pantalla en gama baja, con texturas de partículas en **un solo atlas**.
- **Salpicaduras como mallas pequeñas** (gotas *low-poly*) que caen y luego se "estampan" en la *RenderTexture*.

---

## 9. Interfaz y controles táctiles

### 9.1 Esquema de control recomendado 💡

| Elemento | Propuesta | Referencia |
|---|---|---|
| **Movimiento** | Joystick virtual **flotante**: aparece donde pones el pulgar izquierdo | Brawl Stars y la mayoría de los brawlers móviles ⚠️ |
| **Lanzar** | Botón derecho: **toque = auto-apuntado** al rival más cercano dentro de un cono; **mantener y arrastrar = apuntado manual** con vista previa de la curva de ida y vuelta | Brawl Stars usa toque para auto-apuntar y arrastre para manual ⚠️ |
| **Atrapar** | Automático al pasar por el proyectil, o con un toque corto del mismo botón en una ventana de tiempo | Mantiene la regla de "un botón" |
| **Dash / esquivar** | Segundo botón grande | — |
| **Especial** (power-up) | Tercer botón que solo aparece cuando lo tienes | Evita saturar la pantalla |
| **Tamaño de botones** | Mínimo **44 × 44 pt** en iOS y **48 × 48 dp** en Android; los principales, de 72–96 pt | Guías de Apple y Google ⚠️ |
| **Personalización** | Mover y escalar botones, modo zurdo, opacidad | Estándar en juegos competitivos |

**Auto-apuntado asistido (regla de diseño 💡):**
- cono de 30–40°;
- prioridad al rival más cercano y visible;
- **nunca apuntar a través de paredes**;
- en modo competitivo, **menos asistencia que en casual**;
- opción de apagarlo.

### 9.2 Controles físicos ⚠️

- **iOS:** el framework *Game Controller* reconoce controles de Xbox, PlayStation (DualShock 4 y DualSense) y MFi. Apple también tiene controles virtuales en pantalla (`GCVirtualController`, desde iOS 15) ([Apple HIG – Game controls](https://developer.apple.com/design/human-interface-guidelines/game-controls)).
- **Android:** reconoce controles Bluetooth HID de forma nativa.
- **En Unity**, el *Input System* unifica táctil y control, y trae el componente *On-Screen Stick* (doc 03 ✅).
- **Idea de fiesta:** una **tableta o TV como pantalla compartida y teléfonos como controles** sería un modo futuro, como los *party games* de "sala". No lo recomiendo para el MVP.

### 9.3 Accesibilidad (lista mínima) 💡

- **Daltonismo:** alrededor del **8 % de los hombres y el 0.5 % de las mujeres** tienen alguna deficiencia de visión de color, sobre todo rojo-verde ⚠️. En un público mexicano mayormente masculino de 13–30 años, hay que considerarlo desde el diseño. Medidas:
  - formas además de colores (§7.3);
  - modos protanopia, deuteranopia y tritanopia que cambian la paleta (no solo un filtro);
  - simular con herramientas como [Color Oracle](https://colororacle.org/) ⚠️.
- **Interruptores para:** sacudida de cámara, destellos (fotosensibilidad), vibración y cámara lenta.
- **Escala de interfaz** de 80–130 %.
- **Contorno de alto contraste** para personajes.
- **Señales de audio** para los peligros (el tren que llega, el láser que se activa), además de las visuales.
- Guías de referencia: [Game Accessibility Guidelines](https://gameaccessibilityguidelines.com/) y [Xbox Accessibility Guidelines](https://learn.microsoft.com/en-us/gaming/accessibility/guidelines) ⚠️.

---

## 10. Audio: música, efectos y licencias

### 10.1 Identidad musical 💡

- **Géneros:**
  - hip hop mexicano para menús y vestidor;
  - **cumbia sonidera** y cumbia rebajada en arenas de barrio;
  - electrónica con samples locales (al estilo del sonido tijuanense "nortec") en museo y Metro;
  - guaracha para eventos.
  
  **Evitar** corridos con temática de narco: dañarían la clasificación de edad y la marca.
- **Música adaptativa:** la pista suma capas (percusión, bajo) conforme quedan menos jugadores en la ronda. El último duelo sube de intensidad.
- **Las bocinas del sonidero como peligro sincronizado al ritmo:** la música no solo acompaña, también es mecánica.

### 10.2 Firma sonora: efectos con identidad de tinta 💡

| Acción | Sonido |
|---|---|
| Confirmar en el menú / "listo" | El **clac-clac de la canica de una lata de aerosol** (sonido insignia) |
| Cambiar de pantalla | Pasada de la **rasqueta de serigrafía** |
| Cargar el lanzamiento | Zumbido de una **máquina de tatuar** que sube de tono |
| Atrapar | "Pop" de vinil o goma de juguete |
| KO | Salpicadura de tinta + "¡chas!" con un acento distinto por técnica |
| Subir de nivel | Ráfaga de una **máquina de bordar** |
| Ambiente de las arenas | Silbato del carrito de camotes, flauta del afilador, pregones de "fierro viejo" y de los tamales, todos **grabados de nuevo con actores**, nunca las grabaciones famosas que ya existen |

### 10.3 Caminos para conseguir música y su costo

| Camino | Qué implica | Costo aproximado | Comentario |
|---|---|---|---|
| **Comisionar a productores locales** (obra por encargo, art. 83 de la LFDA ⚠️) | Contrato que cede los derechos patrimoniales para el juego y la publicidad | 💡 Estimación: US$150–600 por pista (**≈ MXN 2,900–11,500**) con productores emergentes | **Recomendado.** Conecta con la escena musical y el podcast |
| **Licenciar canciones existentes** | Se necesitan **dos licencias**: la de la composición (editora o autor; SACM) y la del máster (disquera; SOMEXFON) ⚠️, con permiso de sincronización para videojuego | Muy variable; de cientos a miles de USD por canción | Complejo; además los *streamers* pueden recibir reclamos de derechos |
| **Bibliotecas por suscripción** | Hay que revisar que la licencia cubra **uso dentro de software o juegos** (muchas no lo cubren o piden una licencia especial) ⚠️ | Suscripciones de ~US$10–30 al mes ⚠️ | Útiles para tráileres y redes, no necesariamente para el juego |
| **Efectos gratuitos o baratos** | [Sonniss – GDC Game Audio Bundle](https://sonniss.com/gameaudiogdc) (gratis, libre de regalías) ⚠️, [Freesound](https://freesound.org/) (revisar si cada sonido es CC0 o CC-BY), paquetes de la Unity Asset Store | US$0–60 | Suficiente para el prototipo |

**Recomendación:** incluir un **"modo streamer"** que silencie cualquier música licenciada de terceros. Si toda la música es comisionada con cesión de derechos, el problema desaparece.

---

## 11. Presupuestos técnicos para Android de gama baja

Se toma como base la tabla del reporte 03 ✅ (2,500–5,000 triángulos por personaje, texturas de 512 px, ≤ 80–120 llamadas de dibujo, 30 fps fijos en gama baja, descarga inicial < 150 MB; teléfonos de referencia: Galaxy A07 y Moto G06, que están entre los más vendidos en México ✅ [Luz Noticias](https://www.luznoticias.mx/2026-03-17/mexico/celulares-android-que-mas-se-venden-en-mexico-este-2026-/282031)). Aquí agrego las **reglas de arte** que hacen posible ese presupuesto.

| Regla | Detalle | Ahorro |
|---|---|---|
| **Atlas de paleta** para el escenario 💡 | Casi todo el escenario usa **una textura diminuta de colores** (p. ej. 256 × 256 con muestras de color y degradados) y el modelado decide el color por UV | Cientos de objetos comparten material, así que se agrupan en pocas llamadas de dibujo |
| **Grafiti horneado en *trim sheets*** | Los murales se hornean en atlas de 1024–2048 px y se colocan en muros (doc 03 ✅) | Sin calcomanías en tiempo real |
| **Sombras falsas** (*blob*) | Un disco oscuro bajo cada personaje y proyectil; la luz va horneada en *lightmaps* | Sin sombras en tiempo real |
| **1 llamada de dibujo por muñequito** | Atlas y malla combinados en tiempo de ejecución (§7.2) | 8 jugadores ≈ 8 llamadas de dibujo |
| **LOD de público y fondo** | Público de la lucha o del sonidero como *sprites* planos o mallas de 100–300 triángulos con *GPU instancing* | Ambiente lleno a bajo costo |
| **Pintura en *RenderTexture*** | 512 px en gama baja y 1024 px en media (§8.3) | 1–4 MB de memoria |
| **Partículas** | ≤ 50–150 vivas, opacas o con recorte, en un solo atlas | Evita la sobrecarga de transparencias |
| **Ritmo de cuadros** | Activar *Optimized Frame Pacing* de Unity en Android (usa Swappy de Google) ⚠️ ([Android Frame Pacing](https://developer.android.com/games/sdk/frame-pacing)) | Se siente más fluido sin subir los fps |
| **Calentamiento del teléfono** | Menús a 30 fps; en gama media, partida a 60 fps solo si no hay estrangulamiento térmico. Se puede evaluar *Adaptive Performance* de Unity ⚠️ | Batería y estabilidad |

**¿30 o 60 fps? 💡**

| Criterio | 30 fps (33.3 ms por cuadro) | 60 fps (16.7 ms por cuadro) |
|---|---|---|
| Sensación en un juego de reflejos | Aceptable si es **estable** | Notablemente mejor |
| Batería y calor | Mucho mejor | Peor en gama baja |
| Recomendación | **Por defecto en gama baja**, con opción manual de 60 | **Por defecto en gama media y alta** |

Lo importante es que la **simulación y la red no dependan de los fps de dibujo**. La red corre a 30 Hz (reporte 04 ✅).

---

## 12. Guía de dirección de arte propuesta 💡

### 12.1 Pilares (en una frase cada uno)

1. **"Juguetes de vinil peleando en un CDMX de diorama."**
2. **"La tinta es la consecuencia del combate."** Cada KO deja huella en el escenario.
3. **"El piso calmado, los muros gritan."**
4. **"Seis tintas, seis lenguajes."**
5. **"Lo real de INKCAP se ve de cerca; lo legible manda de lejos."**

### 12.2 Paleta

| Rol | Colores (hex) | Uso |
|---|---|---|
| **Neutros del piso** (valores medios, poco saturados) | Asfalto `#4A4E57`, concreto `#8E9196`, loseta de museo `#B9B3A8` | 60 % de la pantalla |
| **Negro tinta y blanco hueso** | `#16161C`, `#F2EDE4` | Contornos, interfaz y tipografía |
| **Acentos de CDMX** (en muros, lonas y letreros) | Rosa mexicano `#E4007C`, naranja Metro `#F28C28`, amarillo rótulo `#FFD23F`, azul añil `#2B59C3`, turquesa `#1FB5AD`, verde nopal `#3E8E41` | 30 %: escenario "que grita" en los bordes |
| **Colores de jugador** (Okabe-Ito) | `#E69F00`, `#56B4E9`, `#009E73`, `#F0E442`, `#0072B2`, `#D55E00`, `#CC79A7`, `#FFFFFF` | 10 %: **reservados para aros, rastros, tinta de KO e íconos.** El escenario no debe usar estos tonos saturados en el piso |
| **Peligro** | Rojo láser `#FF2D2D` con parpadeo + un ícono de peligro que sea forma, no solo color | Láseres, zonas de tren, prensas |

### 12.3 Materiales

- **Personajes:** matcap de vinil satinado + contorno de casco invertido (delgado, de 1–2 px en pantalla). **Nada de PBR.**
- **Escenario:** colores planos desde el atlas de paleta, luz horneada y un degradado vertical suave (más oscuro abajo) para dar volumen.
- **Tinta en el piso:** colores del jugador con un ligero borde oscuro, para que se lea sobre cualquier neutro.
- **Rarezas:** los acabados de la tabla 2.4.

### 12.4 Proporciones y escala

- Muñequito de **0.9 m**, con 2–2.5 cabezas. Proyectil 1.5–2× más grande que su tamaño "realista".
- Arena de **14–20 m de ancho** visibles en teléfono para 2–4 jugadores, con un acercamiento dinámico hasta ~24 m para 8 jugadores (Target Group de Cinemachine, doc 03 ✅).
- Muros y obstáculos **de no más de 1.5 alturas del personaje** en los bordes cercanos a la cámara, para que no tapen la acción. Si algo tapa, se vuelve **semitransparente con un patrón de semitono** (firma de serigrafía).

### 12.5 Reglas de legibilidad (lista de verificación para cada asset)

- [ ] ¿Se reconoce en silueta a 64 px?
- [ ] ¿Se distingue del piso en escala de grises?
- [ ] ¿Usa los colores de jugador donde no debe? Si sí, cambiarlo.
- [ ] ¿Tiene sombra en el piso?
- [ ] ¿Se entiende si es **peligroso**, **cobertura** o **decorado** en menos de 1 segundo? (Los peligros tienen borde o ícono rojo y animación de aviso de al menos 1 s.)
- [ ] ¿Queda libre la zona de los pulgares?
- [ ] ¿Se ve bien con el modo de daltonismo activo?

---

## 13. Doce ideas de escenarios con peligros y trampas 💡

| # | Escenario | Peligros y trampas interactivas | Cobertura y rasgo especial | Técnica de tinta |
|---|---|---|---|---|
| 1 | **Callejón Grafiteado** | Camión de basura que cruza por un carril cada ~30 s con aviso de claxon (tocarlo = KO). Latas de aerosol que explotan en una nube de color que tapa la vista 3 s. Charcos de pintura resbaladizos | Cortinas metálicas de locales que suben y bajan; un mural que se "completa" con cada KO | Grafiti |
| 2 | **Azotea Chilanga** | Bordes sin barandal (caer = KO). Tinacos que al romperse sueltan un chorro de agua que empuja. Antena que cae en diagonal | **Tendederos con ropa** que tapan la vista; las prendas son diseños INKCAP (escaparate). Perro de azotea que persigue al jugador más cercano | Tinta derramada |
| 3 | **Museo de la Tinta** | **Láseres de seguridad** que barren; tocarlos activa la alarma y las rejas encogen la arena. Guardia o robot con cono de visión que empuja | Esculturas destructibles, cuadros giratorios como puertas, **vitrina de art toys reales**, sala de luz negra | Serigrafía (carteles) |
| 4 | **Andén del Metro** | Tren que llega con luz y silbato, y **barre la zona de vías**. Torniquetes que forman pasillos estrechos | Fase 2: la pelea sigue **dentro del vagón en movimiento**, con puertas que abren y cierran. Un vagonero con bocina que empuja con ondas de sonido | Sticker (íconos propios) |
| 5 | **Tianguis de Domingo** | Diablitos (carritos) que cruzan cargados. Pilas de fruta que ruedan al golpearlas | **Lonas rosas y azules como techo:** tapan la vista desde arriba y **se rasgan con el proyectil**. Un puesto de discos suelta un power-up musical | Sticker |
| 6 | **Tortillería La Tinta** | **Banda transportadora** que arrastra jugadores. **Prensa tortilladora** rítmica (te atrapa = KO, y sales "aplanado"). Comal caliente que cambia de zona | Costales de harina que sueltan una nube blanca y revelan jugadores invisibles | Tinta derramada |
| 7 | **Taller de Serigrafía** | **Pulpo giratorio** (plataforma que rota). Túnel de secado con banda y calor. Cubetas de plastisol derramadas que pegan y frenan. Rasqueta gigante que barre la mesa | Mallas como **paredes semitransparentes** que el proyectil atraviesa pero los jugadores no | Serigrafía |
| 8 | **Estudio de Tatuaje** | Tinteros que se derraman. Neón que parpadea y apaga zonas | Sillones giratorios como cobertura que rota. Hojas de *flash* en las paredes que cobran vida en el KO | Tatuaje |
| 9 | **Arena de Lucha Libre** | **Cuerdas del ring que hacen rebotar el proyectil** (rebote como mecánica). El público lanza vasos. Luces que bajan y dejan solo un reflector | Postes de esquina; una máscara power-up que da un *dash* de "plancha" | Bordado (máscaras) |
| 10 | **Sonidero de Barrio** | **Bocinas gigantes que empujan con una onda al ritmo de la cumbia** (peligro sincronizado a la música). Un "castillo" de cohetes que dispara en patrones | Papel picado colgante que se corta y cae como confeti | Tinta derramada + papel picado |
| 11 | **Galería en Inauguración** (cubo blanco) | Paredes móviles que reconfiguran la sala. Una instalación de **inflables gigantes** que se mueven y empujan | Invitados NPC con copas que estorban; esculturas de "arte conceptual" absurdo (humor) | Sticker / serigrafía |
| 12 | **Panteón del Arte** (evento de Día de Muertos, reporte 05 ✅) | Velas que, al caer, encienden un camino de fuego temporal. Calaveras de azúcar que ruedan | Ofrendas como cobertura, camino de cempasúchil, muñequitos catrina | Tatuaje + papel picado |
| 13 | **Skatepark con Mural** (bonus) | Rampas y *bowl* donde los jugadores y el proyectil toman curvas | Modo de territorio inspirado en el "Graffiti" de Tony Hawk: marcar rampas con tu color | Grafiti |
| 14 | **Trajineras** (bonus, temporada) | Balsas que avanzan y se separan; el agua = KO | Chinampas como islas; música de mariachi o marimba que cambia el ritmo | Tinta derramada |

**Reglas para todas las arenas:**
- cada peligro **avisa al menos 1 s antes** con sonido y luz;
- **un peligro "estrella" por arena**, para que se recuerde y se vuelva clip;
- greybox en ProBuilder antes del arte final (doc 03 ✅);
- una arena nueva por temporada.

---

## 14. Ideas de efectos y KO 💡

### 14.1 KO por técnica de tinta (cosméticos vendibles)

| Técnica | KO | Qué queda en el escenario |
|---|---|---|
| **Tinta derramada** (gratis, por defecto) | El muñequito **revienta en una mancha** del color del jugador que lo eliminó | Mancha en el piso; suma al modo territorio |
| **Grafiti** | Se "evapora" en una nube de aerosol y deja un ***tag*** con el nombre del ganador en el piso o la pared más cercana | *Tag* que se desvanece al final de la ronda |
| **Tatuaje** | Se convierte en un **flash de tatuaje** (dibujo plano de línea negra) que se pega en la pared como hoja de *flash* | Hoja de *flash* en el muro |
| **Serigrafía** | Queda **impreso en el piso** como una estampa plana de 2 tintas con registro desfasado: "aplanado" de forma cómica | Estampa en el piso |
| **Bordado** | **Se descose**: los hilos se deshacen en espiral y el muñequito queda como un parche que cae girando | Parche en el piso |
| **Sticker** | Se vuelve **sticker troquelado**, sale volando y **se pega en la pantalla** un instante (solo en la del ganador), luego en la pared | Sticker en el muro |

### 14.2 KO de "juguete" y de humor

- **Se desarma como art toy:** cabeza, brazos y piernas salen volando por las líneas de unión y rebotan con un "toc" de vinil. Luego se vuelven a ensamblar en el punto de reaparición.
- **De vuelta a la caja:** cae dentro de su **caja de coleccionista** (con ventana de plástico) y la caja se cierra con un "clic".
- **Estatua de museo:** se congela sobre un pedestal con una placa: *"Derrota, técnica mixta, 2026"*. Solo en la arena del museo.
- **Rótulo:** aparece un letrero hecho a mano estilo tortillería: *"SE FUE"*, *"HOY NO FÍO"*, *"CERRADO POR KO"*.
- **Lucha libre:** cuenta de tres palmadas en el ring con un réferi chiquito.
- **Papel picado:** explota en confeti calado del color del ganador.

### 14.3 Efectos que dan "juice" a todo

- Rastro del proyectil del color del dueño, más intenso cuando el proyectil va de regreso, para que se sepa que **regresa**.
- Al atrapar, un anillo de tinta que se expande y un *squash* en las manos.
- *Dash* con un trazo de aerosol corto.
- Pantalla de victoria en estilo **serigrafía** (semitonos + registro desfasado) con la **playera del ganador en primer plano**, el momento donde luce el merch.
- **Repetición del KO en primer plano** de 2 s al final de la ronda, con botón para **compartir el clip** en TikTok, Reels o WhatsApp.

---

## 15. Recomendación para nuestro juego

1. **Aprobar los 5 pilares de §12.1 y la paleta de §12.2** antes de modelar nada. Toda decisión de arte se mide contra ellos.
2. **Diseñar la "plataforma" base propia** (cabeza gota de tinta o tapa de aerosol, 2–2.5 cabezas, uniones visibles) y **registrar su diseño** como parte de la propiedad intelectual de INKCAP. Probar 3 siluetas en una **prueba de legibilidad a 64 px** y en un teléfono de gama baja real.
3. **Construir primero el pipeline de playera:** UV1 dedicada + *shader* "PlayeraINKCAP" + herramienta que genere la versión completa y la versión legible. Probarlo con **5 diseños reales** del catálogo.
4. **Dar a la gorra el protagonismo en partida** (diseño en corona y visera) y a la playera el protagonismo en vestidor, podio y repetición.
5. **Colores de jugador fuera de la ropa:** aro, rastro e ícono con forma, con la paleta Okabe-Ito desde el día 1.
6. **Hacer 2 arenas para el MVP local:** **Callejón Grafiteado** (fácil de entender, muy de marca) y **Taller de Serigrafía** o **Tortillería** (peligros mecánicos muy claros y muy mexicanos). El Museo, el Metro y la Lucha Libre quedan para la versión en línea y las temporadas.
7. **Hacer el *game feel* antes que el contenido:** *hit stop*, *squash & stretch*, KO de tinta derramada y la pintura en *RenderTexture* en las primeras 4–6 semanas del prototipo. Si no se siente bien con cubos, no se va a sentir bien con muñequitos.
8. **Comisionar la música y la tipografía a creadores locales** (productores emergentes, rotulistas) con contrato de obra por encargo. Cada colaboración es un episodio de INKCAP TV y contenido para redes.
9. **Contratos de artista "a prueba de videojuego"** (§4.4) antes de usar cualquier mural, diseño o mascota.
10. **Medir en el Galaxy A07 o el Moto G06 cada semana:** 30 fps estables, ≤ 120 llamadas de dibujo y ≤ 150 partículas.

---

## 16. Dudas abiertas y qué verificar

- **Re-verificar con fuentes abiertas** (cuando se libere el cupo de búsqueda y se permitan los dominios bloqueados): los datos marcados ⚠️, sobre todo los casos legales de tatuajes y grafiti, el plazo de derechos de autor y quién administra las obras de Rivera y Kahlo, los años de CMLL y la declaratoria de patrimonio de la lucha libre, y los detalles de controles de Brawl Stars y del bloqueo de color de Splatoon.
- **¿Qué tamaño real de estampado usa INKCAP** en su producción de playeras? Define la proporción del área UV.
- **¿Los contratos actuales con artistas cubren medios digitales y videojuegos?** Si no, hay que firmar un anexo.
- **¿La mascota "pillo" del artista** puede usarse como personaje del juego y bajo qué condiciones?
- **¿Qué cuesta comisionar música** a productores locales? Lo de §10.3 es una estimación propia; hay que pedir 3 cotizaciones.
- **¿Hay alguna marca registrada** que choque con la silueta base que elijamos? Conviene una búsqueda de diseño industrial ante el IMPI.
- **Presupuestos de rendimiento:** son reglas prácticas y hay que confirmarlos en el teléfono real con el *profiler* de Unity.

---

## 17. Fuentes

**A. Verificadas hoy por esta investigación (reportes 01–06):**
- Tipo de cambio: https://dolardof.com/
- Fall Guys hecho en Unity: https://dev.to/wlz/why-fall-guys-bet-on-unity-the-boring-phone-engine-behind-a-32-player-bean-battle-royale-28e9
- Party Animals llega a móvil (jul-2026): https://technode.com/2026/07/03/hit-party-game-party-animals-heads-to-mobile-bringing-furry-chaos-to-smartphones/
- Free Fire y colaboraciones en México: https://sensortower.com/blog/2025-q4-android-top-5-third-person-games-units-mx-60214330241bc16eb89ce15d
- Phygital (Ralph Lauren × Fortnite; Forever 21 × Roblox): https://www.modernretail.co/technology/how-ralph-laurens-phygital-boot-sale-in-fortnite-shows-benefits-and-limits-of-virtual-commerce/ · https://www.glossy.co/fashion/digital-fashion-brand-play-pop-go-launches-first-physical-collection
- Celulares más vendidos en México 2026: https://www.luznoticias.mx/2026-03-17/mexico/celulares-android-que-mas-se-venden-en-mexico-este-2026-/282031
- Participación de Android/iOS en México: https://gs.statcounter.com/os-market-share/mobile/mexico
- Configuración de URP para rendimiento: https://docs.unity3d.com/Manual/urp/configure-for-better-performance.html
- Optimización móvil en Unity 6 (foro): https://discussions.unity.com/t/unity-6-mobile-graphics-optimization-tips-and-tricks/1593293
- Presupuestos de rendimiento, pipeline de arte, red y progresión: reportes 03, 04 y 05 de esta carpeta.

**B. Referencias canónicas citadas de conocimiento previo (no se pudieron abrir hoy; re-verificar):**
- Apple HIG – Game controls: https://developer.apple.com/design/human-interface-guidelines/game-controls
- Apple – GCVirtualController: https://developer.apple.com/documentation/gamecontroller/gcvirtualcontroller
- Android – Game controllers: https://developer.android.com/develop/ui/views/touch-and-input/game-controllers
- Android Frame Pacing (Swappy): https://developer.android.com/games/sdk/frame-pacing
- Unity URP Decals: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.0/manual/renderer-feature-decal.html
- Unity Input System – On-Screen Controls: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/manual/OnScreen.html
- Arm GPU Best Practices (Mali): https://developer.arm.com/documentation/101897/latest/
- Paleta Okabe-Ito (Color Universal Design): https://jfly.uni-koeln.de/color/
- Color Oracle (simulador de daltonismo): https://colororacle.org/
- Game Accessibility Guidelines: https://gameaccessibilityguidelines.com/
- Xbox Accessibility Guidelines: https://learn.microsoft.com/en-us/gaming/accessibility/guidelines
- Ley Federal del Derecho de Autor (México): https://www.diputados.gob.mx/LeyesBiblio/pdf/LFDA.pdf
- Campus Central de CU, UNAM (UNESCO): https://whc.unesco.org/en/list/1250
- Metro CDMX: https://www.metro.cdmx.gob.mx/
- Lance Wyman (iconografía del Metro y México 68): https://www.lancewyman.com/
- CMLL: https://cmll.com/
- Splatoon: https://splatoon.nintendo.com/
- Bomb Rush Cyberfunk (Steam): https://store.steampowered.com/app/1353230/Bomb_Rush_Cyberfunk/
- Sonniss – GDC Game Audio Bundle: https://sonniss.com/gameaudiogdc
- Freesound: https://freesound.org/
- Masahiro Sakurai on Creating Games (YouTube, incluye *hit stop*): https://www.youtube.com/@sora_sakurai_en
- "Juice it or lose it" (Jonasson y Purho): https://www.youtube.com/watch?v=Fy0aCDmgnxg
- "The Art of Screenshake" (Jan Willem Nijman): https://www.youtube.com/watch?v=AJdEqssNZ-U
- Libros: Frank Thomas y Ollie Johnston, *The Illusion of Life* (1981); Steve Swink, *Game Feel* (2008).
- Casos legales mencionados en §4.4 (*Solid Oak Sketches v. 2K*, *Alexander v. Take-Two*, 5Pointz): conocimiento previo, sin URL verificada hoy.
