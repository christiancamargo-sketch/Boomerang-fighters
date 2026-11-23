# 07 · Legal, propiedad intelectual y cumplimiento para lanzar el juego desde México

> **Proyecto:** "Boomerang Fighters" (nombre de trabajo). Es un *party brawler* móvil para iOS y Android hecho en Unity, con muñequitos estilo *art toy*, un objeto arrojadizo que regresa y escenarios de arte urbano. Primero tendrá multijugador local por Wi-Fi y después online, con progresión diaria y cosméticos de merch real de la marca.
> **Fecha de corte:** 23-nov-2026.
> **Tipo de cambio usado:** **1 USD ≈ 19.1 MXN**, el mismo de los reportes 01 a 06 (FIX de Banxico del 20-nov-2026 ≈ 19.02; mercado del 22-nov-2026 ≈ 19.16. Fuente: [dolar.info – FIX Banxico](https://dolar.info/mexico/hoy/precio/banxico-fix.php)). Para euros y francos suizos uso **1 EUR ≈ 1.15 USD** y **1 CHF ≈ 1.25 USD**. Esas dos tasas **son aproximadas y no las verifiqué hoy** ⚠️.

> ### ⚖️ Esto NO es asesoría legal
> Este documento explica cómo funcionan las reglas para que las juntas con el abogado de propiedad intelectual, el abogado de privacidad y el contador sean cortas y productivas. Antes de firmar contratos, registrar marcas o publicar, **un abogado mexicano tiene que revisarlo**. Para Estados Unidos y Europa se necesita además asesoría local.

---

## Cómo leer este reporte (importante)

| Etiqueta | Qué significa |
|---|---|
| ✅ **Verificado hoy** | Lo leí el 23-nov-2026 en la página oficial. Casi todo viene de developer.apple.com: App Review Guidelines, clasificaciones de edad, App Privacy, ATT, Developer News, requisitos de inscripción, DSA y EULA. |
| 📄 **De otro reporte** | Viene de los reportes 01–06 de esta investigación, que consultaron la fuente original. Doy esa URL. |
| ⚠️ **Conocimiento previo** | No lo pude confirmar hoy. Es información que conozco hasta mediados de 2026. **Hay que verificarla antes de decidir o gastar.** |

**Limitación honesta.** Cuando empecé este reporte, el presupuesto de búsquedas web de la sesión ya estaba agotado por los reportes anteriores. Además, el entorno **no permite abrir www.ftc.gov ni www.diputados.gob.mx**. Para verificar el texto de las leyes mexicanas (LFPDPPP, LFDA, LFPPI) y los comunicados de la FTC, **esos dominios tienen que permitirse en la configuración de red de la sesión**. También se puede pedir una segunda ronda de investigación. Por eso, **lo de Apple está verificado; lo de leyes, casos judiciales, IMPI, Google Play y otros países es conocimiento previo** y lleva ⚠️. Cito los casos con su referencia judicial oficial (por ejemplo, "862 F.2d 204") para que un abogado los localice en minutos.

> **Nota de la verificación independiente (23-nov-2026):** una segunda pasada volvió a leer developer.apple.com y developer.android.com y corrigió o completó varios datos en el texto (Utah, Luisiana, Brasil, ATT en la UE, cuestionario de edad, Play Age Signals, D-U-N-S de Google y la fecha de los principios de la CPC). Los dominios www.wipo.int y www.uspto.gov **tampoco se pudieron abrir** desde esta sesión, y el presupuesto de búsquedas web seguía agotado. El detalle está en la sección final **"Verificación de datos (2026-11-23)"**.

---

## Resumen ejecutivo

1. **Las mecánicas no se pueden "adueñar" con derechos de autor, pero la expresión sí.** Vista cenital, un golpe = KO y un arma que regresa son ideas libres. Lo que perdió juicios fue copiar **el aspecto**: personajes, interfaz, piezas, textos y orden de pantallas (*Tetris v. Xio*, 2012, y *Spry Fox v. 6waves*, 2012) 📄⚠️. En México, el artículo 14 de la Ley Federal del Derecho de Autor excluye "las reglas para realizar juegos" ⚠️.
2. **Las patentes son el riesgo real en EE. UU. y Japón.** Nintendo demandó a *Palworld* en 2024 con **patentes**, no con derechos de autor, y obligó a cambiar mecánicas ⚠️. El riesgo para un arma que regresa es bajo, porque hay arte previo desde *Zelda* (1986), pero conviene una búsqueda rápida antes de lanzar en esos países.
3. **Apple prohíbe los clones obvios.** La guía **4.1 Copycats** dice "no copies la última app popular". Tampoco se puede usar la marca o el nombre de otro desarrollador en el nombre o ícono ✅. **"Boomerang Fighters" no sirve como nombre final.**
4. **El nombre final debe registrarse ante el IMPI en clases 9 y 41 como mínimo**, más 28 y 35 si habrá juguetes o tienda. Cuesta **≈ MXN 3,000–3,600 por clase** con IVA (≈ USD 160–190) y tarda **≈ 4–8 meses** si nadie se opone ⚠️. Con la solicitud mexicana se tienen **6 meses de prioridad** para ir a otros países por el **Protocolo de Madrid** ⚠️.
5. **Hay un bloque de marca muy valioso para INKCAP: la ropa virtual.** Las playeras y gorras **digitales** se protegen en **clase 9** (productos virtuales descargables) y **35/41**, no en la clase 25 de la ropa física ⚠️. Hay que confirmar que la marca INKCAP cubra esas clases.
6. **Privacidad:** México tiene una **nueva LFPDPPP** (DOF 20-mar-2025). El INAI desapareció y la autoridad ahora es la **Secretaría Anticorrupción y Buen Gobierno** ⚠️. Se necesita un **aviso de privacidad** desde el primer lanzamiento si hay cuentas, analítica o compras. Si entran usuarios de la UE se suma el **RGPD**, y si hay niños de EE. UU., la **COPPA** (reformada en 2025, con cumplimiento obligatorio desde el 22-abr-2026) ⚠️.
7. **La verificación de edad en tiendas ya es real.** Apple confirma que la ley **SB 2420 de Texas** aplica a cuentas nuevas desde el **4-jun-2026**, después de un fallo judicial. Hay herramientas obligatorias: **Declared Age Range API**, **Significant Change API**, `ageRatingCode` y notificaciones de retiro de consentimiento ✅. Utah aplica reglas parecidas a cuentas nuevas desde el **6-may-2026** y Luisiana desde el **1-jul-2026** ✅ ([Apple, 24-feb-2026](https://developer.apple.com/news/?id=f5zj08ey)).
8. **Clasificación esperada en Apple: 13+**, porque la "violencia caricaturesca **frecuente**" da 13+ ✅. Si fuera "poco frecuente" sería 9+. Las cajas botín suben a **9+** en el mundo, a **16+ en Australia** y a **18+ en la tienda de Brasil** ✅ ([Apple, 24-feb-2026](https://developer.apple.com/news/?id=f5zj08ey)). *Boomerang Fu* tiene **ESRB E10+** 📄.
9. **No usar cajas botín pagadas.** **Brasil las prohibió** en juegos accesibles para menores (ECA Digital, Ley 15.211/2025) y **Bélgica** las trata como apuesta ⚠️. La FTC multó a **HoYoverse (Genshin Impact) con USD 20 M** en ene-2025 ⚠️. Apple exige publicar probabilidades ✅.
10. **Sin chat libre al inicio.** Con texto o voz libre, Apple exige filtro, reportes, bloqueo y contacto publicado ✅. A Epic le costó **USD 520 M** con la FTC, en parte por tener el chat de voz activado por defecto para niños ⚠️. La alternativa son **stickers y emotes predefinidos de los artistas**, que además se pueden vender.
11. **Contratos con artistas bajo ley mexicana:** toda cesión o licencia debe ser **por escrito, onerosa y temporal**. Sin plazo pactado dura **5 años**, y más de **15 años** solo se justifica por la inversión. Los **derechos morales** (crédito e integridad) **no se pueden renunciar** ⚠️. Por eso la **adaptación a 3D** de cada diseño requiere **autorización expresa**. Hay que prever qué pasa con los trajes **ya comprados** si termina el contrato.
12. **Empresa:** publicar como **persona moral** dueña de toda la propiedad intelectual: código, arte, marca del juego y licencias. Apple pide **entidad legal, D-U-N-S, sitio web con dominio propio y USD 99/año** ✅. Para vender en la UE hay que declarar el **estatus de comerciante (DSA)**, y Apple publicará la dirección, el teléfono y el correo ✅.

---

## 1. ¿Se pueden copiar las mecánicas de juego?

### 1.1 El principio: idea contra expresión

- **EE. UU.:** la ley de derechos de autor no protege "ideas, procedimientos, procesos, sistemas, métodos de operación" (17 U.S.C. §102(b)) ⚠️. Las reglas de un juego son eso.
- **México:** la **Ley Federal del Derecho de Autor (LFDA), art. 14, fr. III**, dice que no son objeto de protección "los esquemas, planes o reglas para realizar actos mentales, juegos o negocios" ⚠️. La **Ley Federal de Protección a la Propiedad Industrial (LFPPI)** tampoco considera invenciones los métodos para juegos ni los programas de cómputo "en sí mismos" ⚠️.
- **Sí se protege la expresión:** personajes, arte, animaciones, interfaz, textos, música, sonidos, código y el **diseño concreto de niveles**. También la **combinación específica** de muchos elementos expresivos.
- **Otras vías de protección:** **marcas** (nombre, logo), **imagen comercial o *trade dress*** (la apariencia general que identifica un producto; desde 2018 se puede registrar como marca en México ⚠️), **competencia desleal** y **patentes** (en EE. UU. y Japón sí se patentan implementaciones técnicas de mecánicas).

### 1.2 Casos clave

| Caso (año, tribunal) | Qué pasó | Resultado | Lección para nosotros |
|---|---|---|---|
| **Atari v. Philips** (*K.C. Munchkin* vs. *Pac-Man*), 672 F.2d 607 (7.º Circuito, 1982) ⚠️ | El clon cambió detalles, pero el "comelón" y los fantasmas se veían y sentían igual | **Infracción** | Cambiar el color o el nombre no basta si el personaje "se siente" igual |
| **Data East v. Epyx** (*Karate Champ* vs. *World Karate Championship*), 862 F.2d 204 (9.º Circuito, 1988) ⚠️ | Dos juegos de karate con movimientos, árbitro y puntuación parecidos | **Sin infracción** en apelación: las similitudes venían de la idea misma del karate (*scènes à faire*, los elementos obligados de un tema) | Lo que es inevitable en el género (vista cenital, arena, KO) es libre |
| **Tetris Holding v. Xio Interactive** (*Mino*), 863 F. Supp. 2d 394 (D.N.J., 2012) 📄⚠️ | Xio copió el tablero de 20×10, la forma y el color de las piezas, la pieza "fantasma" y la basura que sube | **Infracción** de derechos de autor y de *trade dress* | Se puede hacer un juego de bloques que caen, pero **no con la misma apariencia** ([PC Gamer](https://www.pcgamer.com/court-declares-tetris-clone-a-breach-of-copyright-big-implications-for-derivative-games/), [Loeb & Loeb](https://www.loeb.com/zh-hans/insights/publications/2012/06/tetris-holding-llc-v-xio-interactive-inc)) |
| **Spry Fox v. LOLApps/6waves** (*Triple Town* vs. *Yeti Town*), W.D. Wash., 2012 ⚠️ | 6waves vio *Triple Town* en versión beta y bajo NDA mientras negociaba publicarlo. Después lanzó un juego casi idéntico: copió reglas, progresión, textos y hasta la tienda | El juez **rechazó desechar la demanda**. Hubo arreglo y *Yeti Town* **pasó a ser propiedad de Spry Fox** ⚠️ | Cambiar osos por yetis es "cosmético". **Nunca ver juegos ajenos bajo NDA si vamos a hacer algo parecido** ([Medium – Clones Wars](https://medium.com/patents-technology-law/clones-wars-video-game-litigation-illustrated-36682abb4d68)) 📄 |
| **DaVinci Editrice v. ZiKo Games** (*Bang!* vs. *Legends of the Three Kingdoms*), 183 F. Supp. 3d 820 (C.D. Cal., 2016) ⚠️ | Juego de cartas con la misma mecánica de roles, con otra temática y otro arte | **Sin infracción**: la mecánica no se protege | Misma mecánica con tema y arte distintos = permitido en EE. UU. |
| **King v. Avocado** (*Farm Heroes Saga* vs. *Fruit Farm*), Suprema Corte de Corea, 2017Da212095 (27-jun-2019) ⚠️ | El clon combinó las mismas reglas de *match-3* en la misma selección y orden | **Infracción**: la Corte protegió la **combinación creativa** de reglas | En Corea (y quizá en otros países), copiar **todo el paquete** de reglas en el mismo orden sí puede ser infracción |
| **PUBG Corp. v. Epic Games** (*Fortnite*), Corte de Distrito Central de Seúl, ene-2018 ⚠️ | PUBG acusó a *Fortnite Battle Royale* de copiar | **Demanda retirada** en jun-2018 sin explicación pública | Nadie es dueño de un **género** (*battle royale*, *party brawler*) |
| **NCSoft v. Webzen** (*Lineage M* vs. *R2M*), Corte de Seúl, ago-2023 ⚠️ | Se alegó copia de sistemas y diseño | Primera instancia: **competencia desleal** (no derechos de autor). Se ordenó indemnizar ≈ 1,000 millones de wones y suspender el servicio ⚠️ (estado de la apelación sin confirmar) | Aunque no haya infracción de derechos de autor, la **competencia desleal** puede alcanzar a un clon "de inversión ajena" |
| **Nintendo y The Pokémon Company v. Pocketpair** (*Palworld*), Tribunal de Distrito de Tokio, demanda del 19-sep-2024 ⚠️ | Nintendo **no alegó derechos de autor**. Usó **3 patentes japonesas** sobre lanzar un objeto para capturar o invocar criaturas y sobre montarlas. Pide ≈ ¥5 millones por patente más intereses, y suspensión | Pocketpair **cambió mecánicas** en actualizaciones de nov-2024 y may-2025 (invocar sin lanzar la esfera, planeo). En EE. UU., Nintendo obtuvo en 2025 una patente sobre invocar un personaje para pelear, y la USPTO **ordenó reexaminarla** en nov-2025 por arte previo, según la prensa ⚠️. **Estado a nov-2026: no lo pude confirmar** | Las **patentes** pueden bloquear una mecánica aunque el arte sea 100% distinto. Importa en **EE. UU. y Japón**, no en México |

### 1.3 Imagen comercial (*trade dress*) y la regla de Apple contra clones

- **Imagen comercial:** si un juego es muy reconocible por su "look" (paleta, forma de personajes, logo, interfaz), copiar ese conjunto puede confundir al público. Eso se sanciona por *trade dress* en EE. UU. y por **competencia desleal o infracción de imagen comercial** en México (LFPPI, infracciones administrativas ante el IMPI) ⚠️.
- **Apple, guía 4.1 Copycats** ✅:
  - (a) "No copies simplemente la última app popular del App Store ni hagas cambios menores al nombre o la interfaz de otra app para hacerla pasar como tuya."
  - (b) Hacerse pasar por otras apps puede causar la **expulsión del Apple Developer Program**.
  - (c) "No puedes usar el ícono, la marca o el nombre de producto de otro desarrollador en el ícono o el nombre de tu app sin su aprobación."
- **Apple, guía 5.2.1** ✅: no usar material protegido de terceros (marcas, obras con derechos de autor o **ideas patentadas**) sin permiso. La app la debe enviar **quien es dueño o licenciatario** de la propiedad intelectual.
- **Metadatos:** no meter "Boomerang Fu" ni otros nombres de juegos populares en el título, subtítulo o palabras clave de la tienda. La guía 2.3.7 de Apple pide "elegir un nombre de app único" y no llenar los metadatos con "términos registrados como marca, nombres de apps populares" u otras frases irrelevantes. Los subtítulos tampoco pueden "hacer referencia a otras apps", y el nombre tiene un **máximo de 30 caracteres** ✅ (releída en la verificación del 23-nov-2026). Google tiene una regla equivalente ⚠️.

### 1.4 Semáforo frente a *Boomerang Fu*

El reporte 01 (§9) ya detalla qué no copiar 📄. Este es el resumen legal:

| 🟢 Libre (idea o género) | 🟡 Cuidado (puede sumar "parecido") | 🔴 No (expresión o marca ajena) |
|---|---|---|
| Vista cenital, un golpe = KO, proyectil que regresa, dash, *parry*, power-ups apilables, rondas cortas, bots, cámara lenta final, sombreros o gorros cosméticos | Mismo número exacto de power-ups con efectos equivalentes en el mismo orden de desbloqueo; mismo flujo de menús; cámara y encuadre idénticos; misma lógica de "libro" que suelta power-ups | Personajes de comida con cara, el bumerán y el sufijo "Fu", sus íconos y nombres de power-ups (*Caffeinated*, *Bamboozled*…), sus mapas, música, sonidos, tipografía y logo; el nombre "Boomerang" en el título; la palabra **"Frisbee"** (marca de Wham-O 📄) |

**Una precaución extra que nadie ha mencionado: Splatoon (Nintendo).** Su tema central **también es la tinta**. Si nuestro juego pinta el piso con tinta, hay que evitar su presentación (pintar el suelo para ganar territorio con su misma estética, nombres como "Inkling" o el prefijo "Splat-"). Nintendo defiende con fuerza sus marcas y patentes ⚠️. La identidad de INKCAP (grafiti, tatuaje, serigrafía, bordado, *sticker*, tinta derramada) ya es distinta. Hay que mantenerla así.

### 1.5 Patentes: qué tan preocupados debemos estar

- **México:** no se patentan reglas de juegos ni software "en sí mismo" ⚠️. El riesgo local es casi nulo.
- **EE. UU. y Japón:** sí existen patentes de mecánicas implementadas en software. Ejemplos conocidos: el **minijuego en pantallas de carga** de Namco (vencida en 2015) y el **"Sistema Némesis"** de Warner Bros. (otorgada en 2021) ⚠️.
- **Nuestra mecánica central** (lanzar un objeto que regresa) tiene **décadas de arte previo**: el bumerán de *The Legend of Zelda* (1986), *Boomerang Fu* (2020) y muchos más. El riesgo es **bajo**. Si inventamos algo muy original (por ejemplo, "la gorra rebota en grafitis y cambia de color según la técnica de tinta"), conviene una **búsqueda de libertad de operación (FTO)** antes de lanzar en EE. UU. y Japón.
  - Una búsqueda ligera en Google Patents la puede hacer el equipo, gratis.
  - Una opinión formal de un despacho cuesta ≈ **USD 3,000–15,000** (≈ MXN 57,000–286,000) ⚠️, así que solo vale la pena si el juego despega.

### 1.6 Reglas internas de "diseño limpio" para el equipo (que programa con IA)

1. **No meter assets, capturas de la interfaz ni código de *Boomerang Fu*** (ni de ningún juego) en las herramientas de IA, el repositorio o los tableros de referencia.
2. **No descompilar** juegos ajenos ni usar sprites o sonidos "extraídos".
3. Pedir a la IA **mecánicas descritas con palabras**, nunca "haz que se vea como X".
4. Llevar una **bitácora de diseño** con fechas, bocetos y decisiones (por ejemplo, en el repo o en Notion). Prueba la **creación independiente** si algún día hay un reclamo.
5. Revisar las **licencias del código** que sugiera la IA. Evitar código GPL dentro de la app móvil y pasar un escáner de licencias (por ejemplo, el *dependency review* de GitHub).

---

## 2. Nombre y marca

### 2.1 Por qué el nombre de trabajo no sirve como nombre final

| Problema | Detalle |
|---|---|
| "Boomerang" + "Fighters" | Se confunde con *Boomerang Fu* y *Boomerang X* 📄. Hay marcas BOOMERANG registradas en EE. UU. 📄 ([Justia](https://trademarks.justia.com/760/20/boomerang-76020095.html)). "Boomerang" también es una marca de entretenimiento infantil de Warner Bros. Discovery (el canal Boomerang) ⚠️ |
| "Frisbee" | Marca de Wham-O, que la defiende activamente 📄 ([Wham-O](https://wham-o.com/pages/wham-o-brand-protection)) |
| Palabras que los grandes defienden | En 2014, **King** registró "Candy" en la UE y se opuso a juegos con "Saga" (*The Banner Saga*) ⚠️. Es mejor evitar palabras muy ligadas a gigantes: *Clash*, *Royale*, *Saga*, *Crush*, *Brawl Stars*, *Splat-* |
| Apple 4.1(c) | No se puede usar el nombre de producto de otro desarrollador en el nombre de la app ✅ |

### 2.2 Estrategia de marca recomendada

**Usar dos marcas a la vez:** **INKCAP** como marca "casa" y un **nombre propio del juego**, por ejemplo "INKCAP presenta: ___". Así el juego refuerza la marca de ropa, y el nombre del juego se puede licenciar o vender por separado.

**Qué tan fuerte es un nombre** (de más débil a más fuerte):

| Tipo | Ejemplo | ¿Se puede registrar? |
|---|---|---|
| Genérico | "Juego de Gorras" | ❌ No |
| Descriptivo | "Gorra Fighters", "Tira Gorras" | ⚠️ Difícil y débil |
| Evocativo | "Rebotinta", "Gorrazo" | ✅ Sí, buena opción |
| Arbitrario o de fantasía | Palabra inventada | ✅✅ El más fuerte y fácil de defender |

**Ideas para filtrar con búsqueda de marca** (ninguna verificada; son solo punto de partida): **"Gorrazo"** (la gorra que regresa), **"Rebotinta"**, **"Tinta Toss"**, **"Cap Back!"**, **"INKCAP Arena"** (más fácil de registrar porque INKCAP ya es una marca propia, aunque "Arena" es descriptivo). El nombre creativo definitivo se decide en la fase de diseño. **Este reporte solo marca las reglas.**

### 2.3 Registro ante el IMPI

**Clases recomendadas** (Clasificación de Niza) ⚠️:

| Clase | Qué cubre | ¿Para quién? | Prioridad |
|---|---|---|---|
| **9** | Software de videojuegos descargable; apps; **productos virtuales descargables (ropa, gorras y accesorios virtuales para uso en videojuegos)** | Nombre del juego **e INKCAP** | 🔴 Imprescindible |
| **41** | Servicios de entretenimiento; **suministro de juegos en línea no descargables**; organización de torneos | Nombre del juego **e INKCAP** | 🔴 Imprescindible |
| **28** | Juguetes, figuras coleccionables, *art toys* tipo juguete; consolas portátiles | Nombre del juego (si habrá muñequitos físicos) e INKCAP | 🟡 Si habrá figuras |
| **35** | Venta minorista en línea (también de **productos virtuales**); publicidad | INKCAP (tienda) | 🟡 Recomendable |
| **25** | Ropa y gorras **físicas** | INKCAP (seguramente ya la tiene). Para el nombre del juego, solo si habrá merch con ese nombre | 🟡 |
| **16** | *Stickers*, impresos | Opcional | 🟢 |

> 💡 **Ojo con la ropa virtual:** la clase 25 protege las playeras de algodón, **no las del juego**. Desde 2021, marcas como Nike registraron la ropa virtual en las clases 9, 35 y 41 ⚠️. INKCAP debería hacer lo mismo **antes** de anunciar que venderá sus diseños como trajes digitales.

**Pasos y tiempos** (LFPPI vigente desde el 5-nov-2020) ⚠️:

1. **Búsqueda previa** en **MARCia** (marcia.impi.gob.mx), el buscador gratuito del IMPI. Tiene búsqueda **fonética** (nombres que *suenan* igual) y **figurativa** (logos parecidos).
2. **Solicitud en línea** ("Marca en Línea") y pago de derechos.
3. **Publicación en la Gaceta**: cualquier tercero tiene **1 mes** para oponerse.
4. **Examen de forma y de fondo**: el IMPI puede emitir requerimientos o negativas que se contestan en plazo.
5. **Título**: vigencia de **10 años desde el otorgamiento**, renovable.
6. **Declaración de uso** obligatoria dentro de los **3 meses** siguientes al **tercer aniversario** del registro. Si no se presenta, el registro caduca.

| Concepto | Costo aproximado | Tiempo |
|---|---|---|
| Derechos IMPI por clase | ≈ **MXN 3,000–3,600** con IVA (≈ USD 160–190). La tarifa 2025 rondaba MXN 2,800 + IVA; **la de 2026 no la pude confirmar** ⚠️ | — |
| Honorarios de un agente o abogado de marcas | ≈ **MXN 2,500–6,000 por clase** ⚠️ | — |
| Trámite sin oposiciones | — | ≈ **4–6 meses** ⚠️ |
| Con requerimientos u oposición | — | ≈ **8–14 meses** ⚠️ |

**Ejemplo:** el nombre del juego en 9, 41 y 28 + INKCAP en 9 y 41 = 5 clases × MXN 3,300 ≈ **MXN 16,500** de derechos (≈ USD 865) + honorarios ⚠️.

**Conviene presentar dos solicitudes:** una **nominativa** (solo la palabra, que es la más amplia) y una **mixta** (palabra + logo) cuando el logo esté listo.

### 2.4 Cómo buscar conflictos antes de enamorarse de un nombre

| Herramienta | Qué revisa | Costo |
|---|---|---|
| **MARCia** (IMPI) | Marcas en México: fonética, figurativa, por titular | Gratis ⚠️ |
| **WIPO Global Brand Database** (branddb.wipo.int) | Marcas de decenas de oficinas, incluidas las del sistema de Madrid | Gratis ⚠️ |
| **USPTO Trademark Search** | EE. UU. | Gratis ⚠️ |
| **TMview** (EUIPO) | UE y oficinas asociadas | Gratis ⚠️ |
| App Store, Google Play, Steam, itch.io, Google | Juegos con nombre igual o parecido. **No puede haber dos apps con el mismo nombre en el App Store**: la guía 2.3.7 pide un nombre único ✅ | Gratis |
| Revisión profesional | Reporte de similitud con opinión | ≈ MXN 3,000–8,000 por nombre ⚠️ |

### 2.5 Registro en otros países (Protocolo de Madrid)

- **México está en el Protocolo de Madrid desde 2013** ⚠️. Con la solicitud o el registro mexicano se presenta **una sola solicitud internacional a través del IMPI** y se designan países.
- **Derecho de prioridad (Convenio de París):** quien presenta en otro país dentro de los **6 meses** siguientes a la solicitud mexicana conserva **la fecha mexicana** ⚠️. **Recomendación:** presentar en México en cuanto se elija el nombre, y decidir países antes de que pasen 6 meses.
- **Riesgo de "ataque central":** durante 5 años, si cae la marca mexicana, caen las designaciones ⚠️.

| Vía | Costo oficial aproximado ⚠️ | Notas |
|---|---|---|
| **Madrid: tasa base de la OMPI** | **CHF 653** en blanco y negro (≈ USD 816 ≈ MXN 15,600); **CHF 903** con logo a color (≈ USD 1,130 ≈ MXN 21,600) | Se paga una vez por solicitud |
| + designar **EE. UU.** | Tasa individual por clase, aprox. **USD 350–600 por clase** | Usar la calculadora de tasas de la OMPI |
| + designar **UE** (27 países) | Equivalente a la tasa de la EUIPO: **€850 la 1.ª clase**, €50 la 2.ª y €150 cada clase adicional (≈ USD 980 ≈ MXN 18,700 por la 1.ª) | Una sola marca para toda la UE |
| + Brasil, Colombia, Chile, Reino Unido, Japón, Corea | Desde ≈ CHF 100 hasta varios cientos por país o clase | **Argentina no es miembro**: hay que ir directo |
| **Directo a la USPTO** | **USD 350 por clase** (tarifas desde el 18-ene-2025), más cargos si la descripción no usa el catálogo | **Quien tiene domicilio fuera de EE. UU. debe usar un abogado con licencia en EE. UU.** (regla desde 2019): ≈ USD 500–1,500 de honorarios |
| **Paquete razonable** (EE. UU. + UE + Brasil + Colombia + Chile, 2 clases) | ≈ **USD 4,000–7,000** (≈ MXN 76,000–134,000) entre tasas y honorarios | Estimación gruesa. Hacerla **cuando el juego tenga tracción**, dentro de la ventana de 6 meses si se puede |

### 2.6 Proteger a los personajes (INDAUTOR)

- Cada personaje, diseño y el código del juego **están protegidos desde que se crean**. El registro ante INDAUTOR es **declarativo**: sirve como **prueba** ⚠️.
- **Reserva de derechos al uso exclusivo** de **personajes ficticios o simbólicos** (LFDA, art. 173 y siguientes) ⚠️. Protege el **nombre y las características** de un personaje. Es ideal para la **mascota "pillo"** de un artista y para los muñequitos estrella del juego. Hay que renovarla periódicamente. **Definir en el contrato quién es titular de cada reserva**: el artista o la empresa.

### 2.7 Dominios y redes sociales

| Activo | Costo aproximado ⚠️ | Recomendación |
|---|---|---|
| `.com` | USD 10–20/año (MXN 190–380) | Imprescindible. Apple exige un **sitio web con dominio propio** para cuentas de organización ✅ |
| `.mx` / `.com.mx` | ≈ MXN 400–700/año | Recomendado (identidad mexicana) |
| `.gg` / `.games` | USD 20–90/año | Opcional, popular en gaming |
| Handles: TikTok, Instagram, YouTube, X, Twitch, Discord, Reddit | Gratis | Apartarlos **el mismo día** en que se elija el nombre. Usar herramientas como Namechk |
| Correo con el dominio | Incluido en Google Workspace o similar | Apple pide un correo del **dominio de la empresa** ✅ |

---

## 3. Privacidad de datos

### 3.1 Qué datos tocaría el juego

| Dato | Ejemplo | ¿Es dato personal? |
|---|---|---|
| ID de cuenta, ID de dispositivo, IP | Invitado UGS o Firebase, Sign in with Apple | ✅ Sí (identificador) |
| Correo | Sign in with Apple / Google | ✅ Sí |
| Compras | Historial de compras | ✅ Sí |
| Telemetría de juego ligada a la cuenta | Partidas, nivel, tiempo de juego | ✅ Sí |
| Rango de edad | Declared Age Range API | ✅ Sí, y sensible si es un menor |
| Mensajes, voz o nombre de usuario libre | Si hubiera chat o nombres escritos | ✅ Sí, y además es contenido de usuario |
| Datos de anuncios (IDFA o AAID) | Si se usan anuncios personalizados | ✅ Sí, y requiere ATT en iOS |

> 💡 **Ventaja del lanzamiento por Wi-Fi local:** si la primera versión **no crea cuentas, no usa analítica con identificadores y no tiene anuncios**, el cumplimiento de privacidad es mínimo. Eso solo cambia cuando entren cuentas, guardado en la nube, compras y modo online.

### 3.2 México: nueva LFPDPPP (2025) ⚠️

- **Publicada en el DOF el 20-mar-2025**, vigente desde el día siguiente. Abrogó la ley de 2010.
- **El INAI se extinguió.** La autoridad ahora es la **Secretaría Anticorrupción y Buen Gobierno (SABG)**.
- **Aviso de privacidad obligatorio** antes de recabar datos. Debe decir quién es el responsable y su domicilio, qué datos se recaban, **para qué** (separando las finalidades que requieren consentimiento de las necesarias), a quién se transfieren y cómo ejercer los **derechos ARCO** (Acceso, Rectificación, Cancelación y Oposición) y revocar el consentimiento.
- **Consentimiento:** por regla general es **tácito** si se puso el aviso a disposición y la persona no se opone. Es **expreso**, y por escrito para datos sensibles, en los casos que marca la ley.
- **Menores:** su consentimiento lo dan **los padres o tutores** (reglas de capacidad del derecho civil).
- **Transferencias a proveedores:** usar Unity, Google o AWS se considera **remisión a encargados** y debe quedar en contrato. Las transferencias a terceros que usan los datos para sus propios fines requieren informarse y, en su caso, consentimiento.
- **Multas:** según la gravedad, hasta **320,000 UMA** (≈ **MXN 36 millones** con la UMA 2025 de $113.14; con la UMA 2026, que según el verificador es de ≈ $117.31, serían ≈ **MXN 37.5 millones** ⚠️, confirmar en inegi.org.mx), y pueden aumentar con datos sensibles. Los rangos exactos de la ley de 2025 **hay que confirmarlos** ⚠️.
- **Reglamento:** la ley previó emitir un nuevo reglamento. **No pude confirmar si ya se publicó** ⚠️.

### 3.3 Unión Europea y Reino Unido (RGPD / UK GDPR) ⚠️

- **Sí aplica a una empresa mexicana** si **ofrece el juego a personas en la UE** (art. 3.2 RGPD). Publicarlo en las tiendas europeas basta.
- **Bases legales típicas:** *contrato* (cuenta, partidas, compras), *interés legítimo* (antitrampas, seguridad, cierta analítica) y *consentimiento* (anuncios personalizados y analítica no esencial).
- **Edad de consentimiento digital (art. 8):** 16 años por defecto. Cada país puede bajarla hasta 13: España 14, Italia 14, Francia 15, Alemania 16, Bélgica 13 y Reino Unido 13.
- **Representante en la UE (art. 27)** si se tratan datos de europeos de forma no ocasional: ≈ €300–1,500/año con proveedores especializados.
- **Anuncios en la UE/Reino Unido:** Google AdMob exige una **plataforma de consentimiento (CMP) certificada** con IAB TCF desde ene-2024. La **UMP de Google es gratuita**.
- **Reino Unido:** el **Children's Code** (código de diseño apropiado para la edad) pide privacidad alta por defecto, perfilado y geolocalización apagados y nada de "empujones" (*nudges*) para menores, en servicios **probablemente usados por niños**.
- **Multas RGPD:** hasta **€20 M o 4%** de la facturación mundial.
- **Apple ATT en la UE** ✅: desde iOS/iPadOS 27.2 existe un **aviso ATT alternativo** con botón de "Información adicional" (anunciado el 16-sep-2026, por acuerdos con autoridades de competencia europeas). Es **el único disponible en Alemania, Francia, Italia, Polonia y Rumania**, y en la UE se puede volver a preguntar **un año después** de la última respuesta del usuario, salvo que haya desactivado las solicitudes de rastreo en Ajustes. Las reglas sobre cuándo hay que pedir permiso no cambian ([Apple – User Privacy and Data Use](https://developer.apple.com/app-store/user-privacy-and-data-use/), [Apple News](https://developer.apple.com/news/?id=idsft9ai)).

### 3.4 Estados Unidos: COPPA y casos de la FTC ⚠️

- **COPPA** aplica si el servicio está **dirigido a menores de 13** o si **se sabe** que hay menores de 13. Para decidir si un servicio está "dirigido a niños", la FTC mira, entre otras cosas, **personajes animados y estética infantil**. Nuestros muñequitos *art toy* **pueden atraer niños**: hay que diseñar pensando en una **audiencia mixta**.
- **Reforma de la Regla COPPA:** se aprobó en ene-2025, se publicó el **22-abr-2025**, entró en vigor el **23-jun-2025** y es de **cumplimiento obligatorio desde el 22-abr-2026**. Exige:
  - **consentimiento parental separado** para compartir datos con terceros, incluida la publicidad dirigida;
  - **política escrita de retención** de datos;
  - **programa escrito de seguridad** de la información;
  - una definición de "información personal" más amplia.
- **Multa civil:** hasta ≈ **USD 53,088 por infracción** (cifra ajustada de 2025), es decir ≈ MXN 1 millón **por cada una**. La FTC la ajusta por inflación cada enero; **la cifra de 2026 no se verificó** ⚠️.
- **Casos que hay que conocer:**

| Caso | Fecha | Monto | Qué ordenó |
|---|---|---|---|
| **FTC v. Epic Games (Fortnite)** | dic-2022 | **USD 275 M** de multa COPPA + **USD 245 M** en reembolsos por *dark patterns* = **USD 520 M** (≈ MXN 9,930 M) 📄⚠️ | **Chat de voz y texto apagado por defecto** para niños y adolescentes; dejar de cobrar con botones confusos |
| **FTC v. Cognosphere/HoYoverse (Genshin Impact)** | ene-2025 | **USD 20 M** (≈ MXN 382 M) ⚠️ | **Prohibido vender cajas botín a menores de 16 sin consentimiento parental**; publicar probabilidades y **tipos de cambio de la moneda virtual** en dinero real; cumplir COPPA |

### 3.5 Leyes de verificación de edad en tiendas (EE. UU.) ✅/⚠️

| Estado | Ley | Estado actual | Qué implica para el desarrollador |
|---|---|---|---|
| **Texas** | **SB 2420** (App Store Accountability Act) | Debía entrar el 1-ene-2026. Un juez la suspendió el 23-dic-2025 ✅. **Después de un nuevo fallo, aplica a cuentas nuevas de Texas desde el 4-jun-2026** ✅ | Verificación de edad y **consentimiento de padres** para menores de 18 en descargas, **compras dentro de la app y "cambios significativos"**. Los padres pueden **revocar** el consentimiento ✅ |
| **Utah** | SB 142 | Aplica a cuentas nuevas de Apple en Utah desde el **6-may-2026** ✅ | Similares; Apple comparte la categoría de edad por la Declared Age Range API e indica si hace falta permiso parental para "actualizaciones significativas" ✅ |
| **Luisiana** | HB 570 | Aplica a cuentas nuevas de Apple en Luisiana desde el **1-jul-2026** ✅ | Similares ✅ |
| **California** | AB 1043 (Digital Age Assurance Act) | Firmada en oct-2025, **vigente desde el 1-ene-2027** ⚠️ | Señal de edad desde el sistema operativo; las apps deben consultarla |

**Herramientas de Apple** (Developer News, 3-jun-2026) ✅: **Declared Age Range API** (recibe solo el **rango de edad**, no la fecha de nacimiento), **Significant Change API** (PermissionKit; el desarrollador decide qué cambio es "significativo"), la propiedad **`ageRatingCode` de StoreKit** y una **App Store Server Notification** cuando el padre retira el consentimiento. Todo se prueba en *sandbox*. Apple también menciona requisitos de edad en **Brasil, Australia y Singapur** (24-feb-2026) ✅. Desde el **24-feb-2026**, Apple **bloquea la descarga de apps 18+** en Australia, Brasil y Singapur si no se confirmó que el usuario es adulto. En Brasil, la categoría de edad se comparte con la app solo si el usuario o su padre o tutor lo aceptan ✅ ([Apple, 24-feb-2026](https://developer.apple.com/news/?id=f5zj08ey)).

**Google Play** tiene su equivalente, la **Play Age Signals API**, todavía **en beta** ✅. Entrega señales de edad, permite avisar a Google Play de cambios significativos que requieren aprobación parental y notifica cuando se revoca una aprobación. Sus términos (oct-2025) **prohíben usar esa información para publicidad, marketing, perfilado o analítica** ([developer.android.com – Play Age Signals](https://developer.android.com/google/play/age-signals/overview), leído el 23-nov-2026).

### 3.6 Etiquetas de privacidad en las tiendas

**Apple – App Privacy ("etiquetas nutricionales")** ✅:
- Se llenan en App Store Connect y deben mantenerse **exactas y actualizadas**.
- Hay que declarar **cada tipo de dato** que recolecta la app o **cualquier SDK de terceros**: identificadores, compras, datos de uso, diagnósticos, contenido de juego, etc. También para qué se usa (anuncios de terceros, analítica, funcionalidad, personalización) y si está **ligado a la persona**.
- **Se responde por los SDK de terceros** (Unity, Firebase, redes de anuncios). Solo los servicios propios de Apple están exentos.
- **URL de política de privacidad: obligatoria.**

**Apple – App Tracking Transparency (ATT)** ✅:
- Hay que pedir permiso antes de **rastrear**, es decir, de cruzar datos con los de otras empresas para anuncios o medición, o de **leer el IDFA**. Sin permiso, el IDFA llega en ceros.
- **Prohibido** condicionar funciones o **dar premios** por aceptar (guía 5.1.2(i)), manipular el consentimiento o usar **huella digital del dispositivo** (*fingerprinting*).
- El **IDFV** sí se puede usar para analítica propia sin ATT.
- **Recomendación:** si solo habrá video recompensado sin personalización, **no pedir ATT** y declarar "no tracking".

**Google Play – Seguridad de los datos (Data safety)** ⚠️: es un formulario obligatorio para todas las apps que declara qué se recolecta y comparte, incluidos los SDK. Si hay cuentas, también pide un **enlace web para solicitar el borrado** 📄 (reporte 05).

### 3.7 Si el juego atrae niños: Apple Kids, Google "Familias" y Brasil

- **Apple Kids Category** ✅ (guías 1.3 y 5.1.4):
  - Sin enlaces externos, **sin compras** ni distracciones, salvo detrás de una **"puerta parental"**.
  - **Sin analítica ni publicidad de terceros**, con excepciones limitadas.
  - **Política de privacidad obligatoria** y cumplimiento de las leyes de privacidad infantil. Ojo: la puerta parental **no equivale** al consentimiento parental de la COPPA.
- **Google Play – Política de Familias** ⚠️: si el **público objetivo** incluye **menores de 13**:
  - solo **SDK de anuncios autocertificados para familias**;
  - nada de identificadores de anuncios ni ubicación precisa de niños;
  - anuncios apropiados;
  - una **pantalla de edad neutral** si el público es mixto.
  - Elegir un público de 13+ **no protege** si el contenido evidentemente apela a niños: Google revisa el arte y la ficha.
- **Brasil – ECA Digital (Ley 15.211/2025)** ⚠️:
  - se sancionó en sep-2025 y aplica desde **mar-2026**;
  - exige verificación de edad confiable (no basta la autodeclaración), configuraciones protectoras **por defecto**, herramientas de supervisión parental y nada de perfilado publicitario de menores;
  - **prohíbe las cajas botín** en juegos para menores o con **acceso probable** de menores;
  - multas de hasta **R$ 50 millones o 10% de la facturación en Brasil**.
  - Apple ya aplica reglas de edad específicas para Brasil y muestra clasificaciones oficiales del Ministerio de Justicia (MJSP) ✅.

---

## 4. Clasificación por edades

### 4.1 Apple: nuevas categorías ✅

Desde el **24-jul-2025** las clasificaciones son **4+, 9+, 13+, 16+ y 18+** (antes había 12+ y 17+). El cuestionario nuevo pregunta por **controles parentales, verificación de edad, temas violentos y capacidades** de la app, y venció el **31-ene-2026**. En 2026 se agregaron preguntas de **redes sociales** (9-jul-2026; **obligatorias desde sep-2026** para apps nuevas y actualizaciones, porque definen si la app cae en la categoría "Social Media" de **Time Allowances**, el control parental de tiempo de iOS 27 anunciado el 8-jun-2026 que también cubre la categoría Juegos) y las declaraciones de **ubicación compartida** y **citas y relaciones** (desde el 16-nov-2026; todas las apps deben completarlas para el **12-ene-2027**) ✅ ([Apple News](https://developer.apple.com/news/)). La guía 2.3.6 pide responder **con honestidad**: una mala clasificación "puede provocar la intervención de reguladores".

**Cómo afecta cada función de nuestro juego** (tabla oficial de Apple) ✅:

| Función | Mínimo en Apple (global) | Comentario |
|---|---|---|
| Violencia caricaturesca o de fantasía **poco frecuente** | **9+** | |
| Violencia caricaturesca o de fantasía **frecuente** | **13+** | Un *brawler* donde golpear es el centro casi seguro cuenta como "frecuente" → **13+** |
| Violencia realista (poco frecuente / frecuente) | 13+ / 18+ | Evitar sangre y realismo |
| Cajas botín | **9+** (Australia **16+**; Vietnam 12+; **Brasil 18+**) | Otra razón para no tenerlas |
| Concursos (poco frecuentes / frecuentes) | 4+ / **13+** | Torneos con premio constantes suben la edad |
| Apuesta simulada (poco frecuente / frecuente) | 13+ / 18+ | Ruletas, tragamonedas de monedas virtuales: ❌ |
| Contenido de usuarios, mensajería o chat, publicidad | 4+ | No suben la edad por sí mismos, pero activan las obligaciones de la guía 1.2 |
| **Redes sociales** (feed para amplificar contenido de usuarios) | **13+** | Un "muro" de clips con comentarios contaría |
| Acceso libre a la web | 16+ | No poner un navegador dentro del juego |

**Corea (GRAC)** ✅: la violencia caricaturesca poco frecuente y las cajas botín son "All"; la frecuente, 12+. Las redes sociales dan 15+.

### 4.2 Google Play: IARC ⚠️

- Google usa el cuestionario gratuito de la **IARC** (*International Age Rating Coalition*), **el mismo sistema que usa la tienda de Meta Quest**, que el equipo seguramente ya conoce.
- Una sola encuesta genera la clasificación de cada región: **ESRB** (Norteamérica), **PEGI** (Europa), **USK** (Alemania), **ClassInd** (Brasil), **ACB** (Australia), **GRAC** (Corea) y una **genérica IARC** (3+, 7+, 12+, 16+, 18+) en el resto de los países.
- **Referencia:** *Boomerang Fu* tiene **ESRB E10+ por "violencia de fantasía"** 📄 ([Nintendo UK](https://www.nintendo.com/en-gb/DLC/Boomerang-Fu-Street-Food-DLC-3235070.html)). Algo similar (E10+, PEGI 7) es esperable si no hay sangre.
- **Avisos que no suben la edad pero aparecen en la ficha:** "Compras dentro del juego", "Incluye artículos aleatorios" (si hay cajas botín) y "Los usuarios interactúan" (si hay chat).

### 4.3 Decisiones de arte que cuidan la clasificación (y los impuestos)

- **KO con "salpicadura de tinta"** o *confetti* de *stickers*, **nunca sangre**. Encaja con la identidad de las 6 técnicas de tinta.
- Golpes "de caricatura": aplastar, rebotar, convertirse en *sticker* pegado a la pared.
- Nada de armas realistas. La gorra, el disco o la bola de tinta son objetos de juego.
- **México – IEPS de 8% a videojuegos violentos (Paquete Económico 2026)** 📄⚠️: va dirigido a juegos con clasificación "C" o superior. Un juego caricaturesco sin sangre **no debería pagarlo**, pero hay que confirmarlo con el contador (ver reporte 06, §4).

---

## 5. Cajas botín y gacha

### 5.1 Panorama por país ⚠️ (salvo lo marcado ✅)

| Jurisdicción | Regla | Riesgo para nosotros |
|---|---|---|
| **Bélgica** | Desde 2018, su Comisión de Juego considera las cajas botín pagadas **juegos de azar ilegales** sin licencia. Hay sanciones penales (multas de hasta ≈ €800,000 y prisión), que se agravan con menores | 🔴 Alto si vendemos cajas |
| **Países Bajos** | En 2022, el Consejo de Estado **anuló la multa de €10 M a EA** (FIFA Ultimate Team). El Parlamento y el gobierno impulsan una prohibición a nivel UE | 🟡 |
| **Brasil** | **ECA Digital: prohíbe las cajas botín** en juegos para menores o con acceso probable de menores (desde mar-2026) ⚠️. Además, si el cuestionario de Apple declara cajas botín, **la clasificación en la tienda de Brasil sube a 18+** ✅ ([Apple, 24-feb-2026](https://developer.apple.com/news/?id=f5zj08ey)) | 🔴 Alto: LATAM es nuestro mercado |
| **China** | Desde 2017 es obligatorio publicar probabilidades. Los menores tienen horarios de juego muy limitados. Publicar juegos con compras exige **licencia (ISBN)** | ⚪ No lanzaremos en China al inicio |
| **Corea del Sur** | Desde el **22-mar-2024** es obligatorio publicar las probabilidades en el juego, la web y la publicidad. Una reforma de 2025 permite **daños punitivos de hasta 3x** por probabilidades falsas | 🟡 |
| **Japón** | La "kompu gacha" (completar sets con tiradas pagadas) está prohibida desde 2012 | 🟡 |
| **Australia** | Desde sep-2024, las cajas botín pagadas obligan a clasificación **M** y la apuesta simulada a **R18+**. En Apple, las cajas botín dan **16+** en Australia desde el 18-jun-2026, cuando Apple retiró la clasificación 15+ en ese país ✅ ([Apple News, 21-may-2026](https://developer.apple.com/news/)) | 🟡 |
| **Reino Unido** | No hay prohibición; existen principios voluntarios de la industria (2023): nada de compras de menores sin consentimiento y probabilidades visibles | 🟢 |
| **España** | Ha discutido proyectos para restringir las cajas botín a menores. **Estado actual no confirmado** | 🟡 |
| **México** | **No hay regulación específica.** La Ley Federal de Juegos y Sorteos (1947) exige permiso de Gobernación para juegos con apuesta y sorteos. No hay criterio público sobre cajas botín | 🟡 Incierto |
| **EE. UU.** | Sin ley federal, pero la **FTC actúa** (Genshin: menores de 16, probabilidades, tipos de cambio) | 🟡 |
| **Apple** ✅ | Guía 3.1.1: quien ofrezca "cajas botín" u otros mecanismos de artículos aleatorios **a la venta** debe **publicar las probabilidades de cada tipo de artículo antes de la compra** | Obligatorio |
| **Google Play** ⚠️ | Regla equivalente: probabilidades claras antes de la compra y cerca de ella | Obligatorio |
| **UE (consumo)** ⚠️ | **Corrección:** fue en **mar-2025** (no en 2024) cuando la red de autoridades de consumo de la UE (CPC) publicó sus **principios clave sobre monedas virtuales en juegos**: mostrar el precio en dinero real, no ocultar costos y no presionar a menores (dato del verificador, no reconsultado en línea). La Comisión anunció una propuesta de **Ley de Equidad Digital (*Digital Fairness Act*)** para 2026; **su estado a nov-2026 no se pudo verificar** | 🟡 |

### 5.2 Recomendación

1. **Cero cajas botín pagadas y cero gacha.** Todo lo que se vende se ve antes de comprar: tienda directa, pase de temporada y paquetes. Es lo que hizo *Brawl Stars* al **quitar las cajas en dic-2022**, y sus ingresos crecieron 📄 (reporte 05).
2. **Las "Latas de spray" gratis del reporte 05 están bien, con tres candados:**
   - **no se pueden comprar ni directa ni indirectamente** (ni con moneda dura, ni con "abre ya por X gemas", ni con un pase que las regale en masa si eso equivale a venderlas);
   - **probabilidades publicadas** aunque sean gratis (por transparencia y por Corea y China);
   - **nada de premios con valor fuera del juego** (dinero, merch física aleatoria). Una tirada aleatoria por un premio físico puede convertirse en **sorteo** y requerir permiso de Gobernación ⚠️.
3. **Moneda virtual honesta:** mostrar siempre el **equivalente en pesos**, sin paquetes que dejen "sobrantes" diseñados para forzar otra compra. La moneda comprada **no caduca** (Apple 3.1.1) 📄.
4. **Menores:** sin ofertas con cuenta regresiva agresiva para cuentas de menores. Respetar el consentimiento parental de Texas, Utah, Luisiana y Brasil ✅/⚠️.

---

## 6. Chat y moderación

### 6.1 Por qué el chat libre es el mayor riesgo

- **Riesgo para los niños:** acoso, contacto de adultos con menores (*grooming*) y contenido sexual. Es el riesgo reputacional n.º 1 para una marca de arte urbano que quiere colaborar con artistas.
- **Riesgo legal:**
  - Apple, guía 1.2 ✅: con contenido de usuarios se necesita **filtro**, **reportes con respuesta oportuna**, **bloqueo** y **contacto publicado**.
  - COPPA: un chat donde un niño puede escribir su nombre o teléfono equivale a "recolectar información personal" ⚠️.
  - FTC v. Epic: chat **apagado por defecto** ⚠️.
  - **Online Safety Act** del Reino Unido (servicios usuario-a-usuario) ⚠️ y obligaciones de "aviso y retiro" de la **DSA** de la UE ⚠️.
- **Costo:** moderar en serio requiere herramientas y gente. Hay proveedores de filtrado y moderación de texto y voz (Community Sift, GGWP, Hive, ToxMod para voz) ⚠️. **Precios: cotizar.**
- **Casos recientes:** en 2025 varios estados de EE. UU. demandaron a **Roblox** por fallas de seguridad infantil ⚠️.

### 6.2 Alternativas seguras (y vendibles)

| Opción | Cómo funciona | Riesgo legal | ¿Monetiza? |
|---|---|---|---|
| **Chat rápido predefinido** | Frases fijas: "¡GG!", "¡Revancha!", "¡Buena!", "Ups" | 🟢 Muy bajo | No |
| ***Stickers* de artistas** | *Stickers* animados con el arte de los artistas de INKCAP, que se lanzan durante la partida | 🟢 Muy bajo | ✅ Sí (paquetes de *stickers* con regalía) |
| **Emotes del muñequito** | Bailes, poses, "firma de grafiti" | 🟢 | ✅ Sí |
| **Ping / señalización** | Marcar a un rival o un objeto | 🟢 | No |
| **Nombres generados** | El juego propone nombres ("Gorra Veloz 23") o nombres escritos con filtro y revisión | 🟡 Bajo | No |
| Chat de texto libre | — | 🔴 Alto | No |
| Chat de voz | — | 🔴 Muy alto | No |

**En el modo local por Wi-Fi no hace falta chat:** los jugadores están en el mismo cuarto. **En el modo online,** usar solo las opciones verdes, que además son una **línea de producto** para los artistas.

**Si algún día hay chat libre:** que solo exista entre **amigos confirmados**, esté **apagado por defecto** y bloqueado para menores, con filtro, reporte, bloqueo, registros y un plazo de respuesta publicado. Hay que agregarlo al aviso de privacidad y a las etiquetas de las tiendas. También se necesita un **correo de seguridad** que alguien atienda.

---

## 7. Contratos con artistas, registro de obras y música

### 7.1 La Ley Federal del Derecho de Autor en lo que nos importa ⚠️

| Artículo LFDA | Qué dice (resumen) | Consecuencia práctica |
|---|---|---|
| **13** | Protege dibujo, pintura, caricatura e historieta, escultura, obra audiovisual y **programas de cómputo** | Cada diseño de playera, la mascota, los muñequitos y el código están protegidos |
| **14, fr. III** | No protege las reglas de juegos | Las mecánicas son libres (§1) |
| **19 y 21** | **Derechos morales**: perpetuos, **inalienables, irrenunciables**, imprescriptibles e inembargables. Incluyen el derecho a que se reconozca la autoría (**crédito**) y la **integridad** (oponerse a deformaciones o mutilaciones) | **No se pueden "comprar" ni renunciar.** Sí se puede obtener la **autorización expresa** para adaptaciones concretas: modelar en 3D, recolorear, animar, simplificar para móvil |
| **30** | El titular puede **ceder** derechos patrimoniales o dar **licencias exclusivas o no exclusivas**. Debe ser **onerosa y temporal**, y el contrato debe constar **por escrito** o es nulo | Nada de acuerdos de palabra ni por WhatsApp. Siempre contrato firmado |
| **31** | El autor tiene derecho **irrenunciable** a una **participación proporcional** en los ingresos o a una **remuneración fija** | El 15% de regalías de INKCAP encaja |
| **32** | Los actos de transmisión deben **inscribirse en el Registro Público del Derecho de Autor (INDAUTOR)** para valer **frente a terceros** | Registrar los contratos importantes |
| **33** | Sin plazo pactado, la transmisión dura **5 años**. **Más de 15 años** solo cuando la naturaleza de la obra o **la magnitud de la inversión** lo justifiquen | Pactar plazos expresos y **justificar** en el contrato si se pide más de 15 años |
| **83** | **Obra por encargo:** quien comisiona y paga tiene los derechos patrimoniales, salvo pacto en contrario. El autor conserva el derecho a ser mencionado | Para arte hecho **para el juego** (muñequitos, escenarios, UI) por *freelancers*: contrato de obra por encargo **con cesión explícita** |
| **84 y 103** | Las obras y los programas creados por **empleados** en sus funciones pertenecen al patrón (con matices) | El código hecho por el estudio VR debe quedar a nombre de la empresa del juego (§9) |
| **87** | El **retrato** de una persona solo se usa con su consentimiento | Si un artista o tatuador real aparece como personaje o *cameo*, se necesita **licencia de imagen** |
| **173 y siguientes** | **Reserva de derechos** sobre personajes ficticios o simbólicos | La mascota "pillo" y los muñequitos estrella (§2.6) |

### 7.2 Cláusulas clave de la licencia digital con cada artista

1. **Objeto:** qué obras (lista con imágenes anexas) y qué **usos**: trajes, playeras, gorras y *stickers* **dentro del juego**, más **tráilers, capturas de tienda, redes sociales, prensa y transmisiones**.
2. **Adaptación (derechos morales):** autorización **expresa** para modelar en 3D, animar, recolorear, simplificar y combinar con otros elementos. Incluir un **proceso de aprobación** del artista (por ejemplo, 5 días hábiles; si no responde, se entiende aprobado) y una lista de cosas prohibidas (por ejemplo, que su diseño aparezca en un contexto que lo denigre).
3. **Crédito:** el nombre del artista en la ficha del objeto ("Diseño de ___") y en los créditos.
4. **Territorio:** **mundial** (las tiendas venden en 175 países).
5. **Plazo:** por ejemplo, **10 años con renovación automática**, dentro del tope de 15 años y justificado por la inversión en desarrollo ⚠️.
6. **Exclusividad:** normalmente **no exclusiva**. Si INKCAP la quiere, que sea **exclusiva solo en videojuegos o mundos virtuales**, y por escrito (art. 30) ⚠️.
7. **Regalía:** **15% del ingreso neto** del objeto, ya **sin IVA ni comisión de la tienda**. Ejemplo del reporte 06: en un objeto de **MXN 99** el artista recibe ≈ **MXN 10.9** 📄. Definir en el contrato:
   - cómo se reparten los **paquetes** (bundles) y el **pase de temporada**;
   - qué pasa con las **devoluciones** que hacen Apple y Google;
   - el **tipo de cambio**;
   - **pagos trimestrales** con reporte por SKU y **derecho de auditoría** una vez al año.
8. **Objetos gratis:** si un traje se **regala** en eventos o en el pase gratis, ¿hay pago fijo? Pactar una **tarifa mínima** o un **adelanto** (*advance*) recuperable.
9. **Terminación y "legado" (crítico):** si el contrato termina, la empresa **deja de vender** el diseño, pero **los jugadores que ya lo compraron lo conservan para siempre**, y el diseño puede seguir en repeticiones, videos viejos y respaldos. Sin esta cláusula **habría que borrar objetos ya pagados**, y eso genera reclamos de consumidores.
10. **Garantías del artista:** la obra es original, no copia nada ajeno y no usa elementos de marcas de terceros. Debe responder si resulta que sí (**indemnidad**).
11. **Inteligencia artificial:** la empresa **no entrenará modelos de IA** con el arte del artista ni generará "variaciones" sin su aprobación. Es clave para la confianza de la comunidad artística.
12. **Físico + digital:** si una misma obra sale en playera física **y** como traje digital (*phygital*), aclarar si la regalía se cobra por separado en cada canal.
13. **Personaje-mascota del artista:** hacer un **contrato aparte** (licencia de personaje) con regalía propia, reglas de "voz" y comportamiento del personaje, y la titularidad de la reserva de derechos.
14. **Impuestos:** el contador debe definir qué **retenciones de ISR e IVA** aplican a cada pago (persona física o moral) y qué **CFDI** debe emitir el artista ⚠️.
15. **Jurisdicción:** tribunales de la CDMX y ley mexicana. Prever mediación antes de demandar.

### 7.3 Contratos con quienes hacen el juego

- **Empleados del estudio:** contrato laboral con una cláusula de titularidad de obras y software (arts. 84 y 103) y **confidencialidad**.
- ***Freelancers*** (modeladores, músicos, ilustradores de UI): **obra por encargo + cesión de derechos patrimoniales** con plazo y precio, y autorización de modificaciones.
- **Entre empresas** (el estudio VR desarrolla para la empresa del juego): **contrato de desarrollo** con cesión del código y los assets a la empresa dueña. Así se evita que la propiedad intelectual quede "en el aire" entre socios.

### 7.4 Registro ante INDAUTOR ⚠️

| Qué registrar | Para qué | Costo aproximado |
|---|---|---|
| Muñequitos principales, escenarios y logo del juego (como dibujo u obra artística) | Prueba de autoría y fecha | Unos cientos de pesos por obra (≈ MXN 300–400). **Tarifa 2026 no confirmada** |
| El juego como **programa de cómputo** (y, si conviene, como obra audiovisual) | Prueba de titularidad del código | Similar |
| **Contratos** de cesión y licencia con artistas (art. 32) | Que valgan frente a terceros | Similar |
| **Reserva de derechos** de personajes | Exclusividad del nombre y las características del personaje | Unos miles de pesos. **No confirmado** |

### 7.5 Código y arte hechos con IA ⚠️

- **México:** en 2025 la **Suprema Corte (SCJN)** resolvió que las obras **generadas exclusivamente por IA** no tienen protección autoral, porque el autor debe ser una persona física (conocimiento previo; confirmar la tesis exacta). Pista del verificador, no confirmada en línea: probablemente es el **Amparo Directo 6/2025** (ago-2025), sobre una obra hecha con IA que INDAUTOR se negó a registrar ⚠️.
- **EE. UU.:** la Oficina de Derechos de Autor (informe de ene-2025) protege la **contribución humana** (selección, arreglo, modificaciones), pero no lo que sale solo de un *prompt*.
- **Qué significa para nosotros:**
  - Lo más valioso, el **arte de los artistas y los diseños de personajes**, debe ser **creación humana** documentada.
  - Para el código escrito con IA: conservar historial (commits, revisiones humanas, decisiones de arquitectura) que demuestre **aporte humano**.
  - Revisar los **términos de cada herramienta de IA**: la mayoría de los planes comerciales asignan la propiedad del resultado al usuario.
  - **No usar IA para imitar el estilo** de artistas reales sin permiso.

### 7.6 Música, sonidos, fuentes y assets

| Opción | Pros | Contras / qué cuidar |
|---|---|---|
| **Música original por encargo** (productores locales de hip-hop o *beats*) | Identidad propia; se puede vender el *soundtrack* | Contrato de **obra por encargo con cesión**. Si el productor es socio de una sociedad de gestión (SACM en México), revisar que pueda ceder o licenciar el uso en el juego ⚠️ |
| **Bibliotecas libres de regalías** con licencia para juegos | Barato y rápido | Leer que la licencia **incluya videojuegos o apps** (muchas licencias para "video" no lo cubren) ⚠️. Evitar pistas registradas en **Content ID de YouTube**: los *streamers* recibirían reclamos y eso mata el marketing orgánico |
| **Canción con licencia** de un artista conocido | Golpe de marketing | Requiere **dos licencias**: la obra (editora o compositor) y la grabación (disquera o fonograma), además de los derechos de intérpretes. Caro y con plazos |
| **Efectos de sonido** | — | Licencia comercial clara o hechos en casa |
| **Fuentes tipográficas** | — | Muchas requieren una **licencia de "app/embedding"** aparte de la licencia de escritorio. Las de Google Fonts (licencia OFL) son seguras ⚠️ |
| **Unity Asset Store** | Licencia estándar que permite usar los assets en juegos | No se pueden redistribuir sueltos. Guardar los comprobantes ⚠️ |
| **Código abierto** | — | MIT y Apache requieren **atribución** (pantalla de "licencias de terceros"). **Evitar GPL** en la app ⚠️ |

---

## 8. Términos de servicio, privacidad, EULA y bienes virtuales

### 8.1 Documentos necesarios

| Documento | Dónde vive | ¿Obligatorio? | Notas |
|---|---|---|---|
| **Aviso / política de privacidad** | Web (URL en las tiendas) y dentro del juego | ✅ Apple lo exige ✅ y Google también ⚠️. En México es obligatorio por la LFPDPPP ⚠️ | Versión en **español** (México) e **inglés**. Si hay usuarios en la UE, la sección del RGPD |
| **EULA (licencia de uso)** | App Store Connect o web | Opcional en Apple: **si no pones una propia, aplica la EULA estándar de Apple** en todos los países ✅ | La EULA propia se pega como **texto plano** en App Store Connect; los idiomas van en el mismo campo ✅ |
| **Términos de servicio** | Web y aceptación al crear la cuenta | Muy recomendable cuando hay cuentas y modo online | Reglas de conducta, trampas, suspensión |
| **Términos de bienes virtuales** | Parte de los Términos o documento aparte | Muy recomendable cuando hay tienda | Ver §8.2 |
| **Reglas de torneos o concursos** | Dentro de la app | ✅ Si hay concursos, Apple exige reglas oficiales en la app que digan que **Apple no es patrocinador**, y el desarrollador debe ser el patrocinador ✅ | En México, sorteos y concursos con premio pueden requerir permiso de Gobernación ⚠️ |
| **Página de borrado de cuenta** | Dentro de la app + web | ✅ Apple (5.1.1(v)) ✅ y Google ⚠️ | Ver reporte 05 📄 |
| **Licencias de terceros** | Pantalla "Créditos / Licencias" | ✅ Si se usa código abierto | |
| **Contacto de seguridad y soporte** | Web y ficha | ✅ Apple 1.2 si hay contenido de usuarios ✅ | |

### 8.2 Cláusulas de bienes virtuales que no deben faltar

- Lo que se compra es una **licencia limitada, personal, no transferible y revocable** para usar el objeto **dentro del juego**. **No es propiedad** y **no tiene valor monetario**.
- **No hay reventa ni intercambio por dinero real** (evita mercados grises y problemas de juego de azar).
- **La moneda comprada no caduca** (Apple 3.1.1) 📄.
- **Reembolsos:** los procesan **Apple o Google** según sus políticas. Si hay devolución, la empresa puede retirar el objeto o la moneda.
- **Cambios al juego:** el balance y los modos pueden cambiar, pero **los cosméticos comprados no se quitan** salvo por fraude o por orden legal.
- **Cierre del juego:** aviso con anticipación (por ejemplo, 60–90 días) y sin obligación de reembolso, salvo lo que exija la ley de consumo.
- **Suspensión de cuentas** por trampas, fraude o acoso, con un proceso de apelación.
- **Menores:** las compras requieren la autorización de padres o tutores.

### 8.3 Derecho del consumidor que no se puede "firmar en contra" ⚠️

- **México (Profeco, LFPC):** precios **totales con IVA** en pesos, información **clara en español** y nada de cláusulas abusivas. Los términos no pueden quitar derechos irrenunciables al consumidor.
- **UE:** existe un derecho de desistimiento de 14 días para contenido digital, salvo que el usuario acepte la ejecución inmediata y reconozca que pierde ese derecho (en compras de la tienda lo gestionan Apple y Google). Las cláusulas de arbitraje obligatorio no valen contra consumidores.
- **Elegir la ley mexicana y los tribunales de la CDMX** está bien como base, **pero la ley de consumo del país del jugador puede aplicar igual.**
- **Apple, guía 5.6** ✅: no engañar con compras no deseadas, no subir precios de forma tramposa y no cobrar por contenido que no se entrega. Muchas quejas o reembolsos pueden afectar la cuenta de desarrollador.

---

## 9. Estructura empresarial y facturación

### 9.1 ¿Con qué empresa publicamos?

| Opción | Pros | Contras | Comentario |
|---|---|---|---|
| **La empresa actual de INKCAP** | Ya existe; el vendedor en la tienda sería la marca | Mezcla los riesgos del juego con la ropa; complica la entrada de inversionistas solo al juego | Viable para un MVP |
| **La empresa del estudio VR** | Ya desarrolla y publica en Meta Quest | La marca y los artistas son de INKCAP; habría que licenciarlos | Viable si INKCAP licencia la marca |
| **Nueva S.A.P.I. de C.V. (*joint venture*)** ✅ recomendado | **Una caja limpia** dueña del juego, el código, la marca del juego y las licencias de INKCAP y los artistas. Admite acuerdos de socios y entrada de inversionistas (ideal para fondos como el Google Play Indie Games Fund 📄) | Costo de notario (≈ MXN 15,000–40,000 ⚠️) y contabilidad aparte | La mejor opción si el juego crece |
| **S.A.S.** (sociedad por acciones simplificada) | Se constituye **gratis en línea** con la Secretaría de Economía; puede tener un solo accionista ⚠️ | **Tope de ingresos anuales** (≈ MXN 5 millones originalmente, actualizable ⚠️); al rebasarlo hay que transformarla | Solo para empezar muy pequeño |
| **Persona física** | Rápido | Apple **retiene ISR** a personas físicas en México 📄; el nombre personal aparece como vendedor ✅; responsabilidad ilimitada | ❌ No recomendado |

### 9.2 La cadena de propiedad intelectual (que no se rompa)

```
Artistas ──(licencia digital + regalías)──────────────┐
INKCAP ──(licencia de marca INKCAP para el juego)─────┤
Estudio VR ──(contrato de desarrollo + cesión de código)┤──► EMPRESA DEL JUEGO (S.A.P.I.)
Freelancers ──(obra por encargo + cesión)─────────────┤      · dueña del código y los assets
Músicos ──(cesión o licencia de música)───────────────┘      · titular de la marca del juego (IMPI)
                                                              · cuenta Apple / Google
                                                              · aviso de privacidad (responsable)
```

**Por qué importa:** Apple pide que la app la envíe **quien es dueño o licenciatario** de la propiedad intelectual (5.2.1) ✅. Un inversionista, un comprador del juego o un *publisher* revisará esta cadena antes de firmar (*due diligence*).

### 9.3 Cuentas de tienda y requisitos ✅

- **Apple (organización)** ✅:
  - entidad legal (**no** se aceptan nombres comerciales ni sucursales);
  - **número D-U-N-S** de 9 dígitos;
  - una persona con **facultades para obligar** a la empresa;
  - **correo con el dominio de la empresa** y **sitio web público y funcional** con dominio propio (no basta una red social);
  - **USD 99/año** (≈ MXN 1,890). El vendedor que aparece en la tienda es el **nombre legal de la empresa**.
- **Google Play:** **USD 25 una sola vez** (≈ MXN 478) 📄. La cuenta de organización también pide **D-U-N-S** y un sitio web verificado en Search Console ✅📄 (verificado por el reporte 03 en [developer.android.com](https://developer.android.com/developer-verification/guides)). El D-U-N-S **puede tardar hasta 28 días** según la FAQ oficial de Android (la ayuda de Play Console menciona hasta 30): tramitarlo ya 📄 (reporte 03, [Android FAQ](https://developer.android.com/developer-verification/guides/faq)).
- **UE – Ley de Servicios Digitales (DSA)** ✅: para estar en el App Store de la UE hay que declarar si se es **comerciante (*trader*)**. Una empresa que vende compras dentro de la app lo es. Apple **muestra en la ficha** la dirección del D-U-N-S, más el **teléfono y correo** que se agreguen (verificados con 2FA), y pide **documentos de la empresa**. Desde el **17/18-feb-2025**, las apps sin estatus de comerciante **se retiraron** del App Store de la UE ✅. **Consejo:** usar un teléfono y un correo **corporativos de soporte**, no personales.

### 9.4 Impuestos y facturación (resumen; detalle en el reporte 06)

- **IVA en México:** Apple **entrega el IVA** de las ventas en México a las **empresas con sede en México**, que lo enteran al SAT. Apple descuenta el IVA de su comisión (acreditable) y emite cada mes una factura y una constancia ✅📄 ([Apple – Invoices](https://developer.apple.com/help/app-store-connect/manage-tax-information/manage-invoices-and-other-tax-documents)).
- **ISR:** Apple **no retiene ISR a personas morales**; a personas físicas sí ✅📄. La empresa paga ISR (≈ 30% sobre la utilidad) ⚠️.
- **Ventas al extranjero:** podrían calificar como **exportación de servicios con IVA de 0%** ⚠️.
- **Formularios fiscales de EE. UU.:** las tiendas piden el **W-8BEN-E** para aplicar el tratado México–EE. UU. y evitar retenciones de más ⚠️.
- **CFDI:** cómo documentar los ingresos que pagan Apple y Google (entidades extranjeras), por ejemplo con el RFC genérico de extranjeros. **Confirmar con el contador** ⚠️.
- **IEPS 2026 a videojuegos violentos:** confirmar que no aplica (§4.3) ⚠️.
- **Pagos a artistas:** retenciones según su régimen (§7.2, punto 14) ⚠️.

---

## Checklist legal antes del lanzamiento (priorizado)

**Prioridad:** 🔴 P0 = antes de invertir en arte final y marketing · 🟠 P1 = antes del lanzamiento local por Wi-Fi en tiendas · 🟡 P2 = antes del modo online y la expansión global.

| # | Prioridad | Acción | Costo aproximado | Etiqueta |
|---|---|---|---|---|
| 1 | 🔴 P0 | Decidir **qué empresa será dueña del juego** (recomendado: S.A.P.I. nueva) y firmar el **acuerdo de socios** INKCAP–estudio | MXN 15,000–40,000 notario + MXN 15,000–40,000 abogado (USD 1,600–4,200) | ⚠️ |
| 2 | 🔴 P0 | **Contratos de cesión** con todos los que ya crearon algo (código, bocetos, prototipo) | Incluido en la plantilla (≈ MXN 10,000–25,000) | ⚠️ |
| 3 | 🔴 P0 | **Búsqueda de marca** del nombre final (MARCia, WIPO, USPTO, TMview, tiendas) | Gratis–MXN 8,000 | ⚠️ |
| 4 | 🔴 P0 | **Solicitud IMPI** del nombre en clases **9 y 41** (+28/35), e **INKCAP en 9, 35 y 41** (ropa virtual) | ≈ MXN 3,300/clase + honorarios → ≈ MXN 25,000–45,000 por 5 clases (USD 1,300–2,400) | ⚠️ |
| 5 | 🔴 P0 | **Dominios** (.com, .mx) y **handles** en todas las redes | USD 50–150/año (MXN 950–2,900) | ⚠️ |
| 6 | 🔴 P0 | **Plantilla de licencia digital con artistas**: adaptación 3D, crédito, 15% neto, auditoría, **legado**, IA | MXN 15,000–40,000 (USD 800–2,100) | ⚠️ |
| 7 | 🔴 P0 | **D-U-N-S** y cuentas **Apple (USD 99/año) y Google (USD 25)** a nombre de la empresa | ≈ MXN 2,370 el primer año | ✅/📄 |
| 8 | 🔴 P0 | **Reglas de diseño limpio**: sin assets ajenos, bitácora de diseño, escáner de licencias | Gratis | — |
| 9 | 🟠 P1 | **Aviso de privacidad** (LFPDPPP) + política de privacidad web (ES/EN) | MXN 10,000–30,000 | ⚠️ |
| 10 | 🟠 P1 | **Etiquetas de privacidad de Apple** y **Data safety de Google**, revisando cada SDK | Gratis (tiempo interno) | ✅/⚠️ |
| 11 | 🟠 P1 | **Términos de servicio + términos de bienes virtuales** (+ EULA propia o la estándar de Apple) | MXN 15,000–40,000 | ⚠️ |
| 12 | 🟠 P1 | **Cuestionarios de edad** honestos (Apple + IARC). Arte "tinta, no sangre" | Gratis | ✅ |
| 13 | 🟠 P1 | **Inventario de licencias**: música, sonidos, fuentes, Asset Store, código abierto | Gratis–variable | ⚠️ |
| 14 | 🟠 P1 | **Registro INDAUTOR** de personajes clave, código y contratos con artistas; **reserva** de la mascota | ≈ MXN 3,000–10,000 en conjunto | ⚠️ |
| 15 | 🟠 P1 | **Sin chat libre**: solo chat rápido, *stickers* y emotes. Contacto de soporte publicado | Gratis | ✅ |
| 16 | 🟠 P1 | **Sin cajas botín pagadas**; probabilidades visibles en las recompensas gratis aleatorias | Gratis | ✅ |
| 17 | 🟠 P1 | **Junta con el contador**: IVA de Apple, CFDI, W-8BEN-E, IEPS, retenciones a artistas | MXN 3,000–10,000 | 📄⚠️ |
| 18 | 🟠 P1 | **Pantalla de edad neutral** + "modo protegido" para menores (sin anuncios personalizados ni ofertas agresivas) | Desarrollo interno | ⚠️ |
| 19 | 🟡 P2 | **Madrid** (EE. UU., UE, Brasil, Colombia, Chile…) dentro de los **6 meses de prioridad** | ≈ USD 4,000–7,000 (MXN 76,000–134,000) | ⚠️ |
| 20 | 🟡 P2 | **Herramientas de edad de EE. UU.**: Declared Age Range, Significant Change API, notificaciones; Play Age Signals | Desarrollo interno | ✅/⚠️ |
| 21 | 🟡 P2 | **RGPD**: representante en la UE, CMP (si hay anuncios), contratos con proveedores, evaluación de transferencias | €300–1,500/año + MXN 20,000–60,000 de abogado | ⚠️ |
| 22 | 🟡 P2 | **Estatus de comerciante DSA** en App Store Connect (UE) | Gratis | ✅ |
| 23 | 🟡 P2 | **Brasil (ECA Digital)**: cumplir o **excluir Brasil** al principio | Variable | ⚠️ |
| 24 | 🟡 P2 | **Búsqueda de patentes** si hay una mecánica original, antes de EE. UU. y Japón | Gratis (ligera) o USD 3,000–15,000 (formal) | ⚠️ |
| 25 | 🟡 P2 | **Plan de moderación** (reportes, bloqueo, tiempos de respuesta) si se agrega contenido de usuarios | Cotizar proveedor | ✅/⚠️ |
| 26 | 🟡 P2 | **Excluir China** (licencia ISBN) y **revisar Corea** (probabilidades, GRAC) | — | ⚠️ |

### Presupuesto legal aproximado ⚠️

| Escenario | Qué incluye | Total aproximado |
|---|---|---|
| **Mínimo viable (solo México)** | IMPI 5 clases, plantillas básicas, aviso de privacidad, cuentas de tienda, dominios | **MXN 60,000–110,000** (USD 3,100–5,800) |
| **Recomendado (México bien hecho)** | Lo anterior + S.A.P.I., acuerdo de socios, términos completos, INDAUTOR, contador | **MXN 120,000–220,000** (USD 6,300–11,500) |
| **Expansión global** | Madrid en 5 territorios, abogado en EE. UU., RGPD, representante en la UE, revisión de patentes ligera | **+ MXN 150,000–350,000** (USD 7,900–18,300) |

*Son rangos de mercado que conozco, **no cotizaciones**. Pedir 2–3 propuestas a despachos de PI y privacidad en la CDMX.*

---

## Recomendación para nuestro juego

1. **Copiar el "esqueleto" del género, nunca la piel.** Vista cenital, un golpe = KO y un arma que regresa son libres. Personajes, power-ups, mapas, interfaz, sonidos y nombres deben ser **100% INKCAP**: muñequitos *art toy*, gorra o disco propio, calles grafiteadas y museo. Además, mantener **distancia de Splatoon** en todo lo relacionado con la tinta.
2. **Elegir ya el nombre final** con la regla "INKCAP + nombre de fantasía". Sin "Boomerang", "Frisbee", "Fu", "Clash", "Saga" ni "Splat". Buscarlo en MARCia y en la WIPO y **presentarlo ante el IMPI en clases 9 y 41** (+28/35). **Registrar INKCAP para ropa virtual** (9/35/41) antes de anunciar los trajes digitales.
3. **Crear una S.A.P.I. dueña de todo**, con contratos de cesión y licencias que formen una cadena sin huecos. Publicar en Apple y Google **como organización**.
4. **Firmar una licencia digital estándar con cada artista:** adaptación 3D autorizada, crédito visible, 15% del neto con reporte trimestral, cláusula de **legado** para los trajes ya comprados y **prohibición de entrenar IA** con su arte. Registrar los contratos en INDAUTOR.
5. **Lanzar primero en México por Wi-Fi local, con "privacidad mínima":** sin cuentas obligatorias, sin anuncios personalizados y sin chat. Aun así, publicar aviso de privacidad, etiquetas de tienda y términos.
6. **Clasificación objetivo: 13+ en Apple y E10+/PEGI 7 en IARC**, con KO de tinta y sin sangre. **Pantalla de edad neutral** y **modo protegido** para menores, porque los muñequitos atraerán niños.
7. **Monetización limpia:** cero cajas botín pagadas, todo a la vista, moneda con equivalente en pesos que no caduca. Las recompensas aleatorias **solo gratis** y con probabilidades publicadas.
8. **Comunicación sin chat libre:** chat rápido, emotes y **paquetes de *stickers* de artistas** como producto de pago con regalías.
9. **Expansión ordenada:** antes del modo online global, implementar las APIs de edad de Apple y Google (Texas, Utah, Luisiana). Declarar el **estatus DSA** para la UE, resolver el **RGPD** y decidir si se **excluye Brasil** hasta cumplir el ECA Digital. Presentar Madrid **dentro de los 6 meses** de la solicitud mexicana si el juego tiene tracción.
10. **Presupuesto legal:** reservar **≈ MXN 120,000–220,000** para hacerlo bien en México y otros **≈ MXN 150,000–350,000** para la expansión global, y **validar todo con un abogado** antes de firmar o publicar.

---

## Dudas abiertas (verificar en una segunda ronda o con abogado)

- Texto exacto de la **nueva LFPDPPP 2025**: rangos de multas, requisitos del aviso y si ya hay **reglamento**. Requiere acceso a diputados.gob.mx o al DOF.
- **Tarifas 2026** del IMPI (marca por clase) y de INDAUTOR (registro de obra, contratos y reserva de derechos).
- Estado **a nov-2026** de *Nintendo v. Pocketpair* y de la reexaminación de la patente de Nintendo en EE. UU.
- Situación vigente de la ley de Texas después del fallo que mencionó Apple (¿apelación pendiente?). *(Las fechas de Utah, 6-may-2026, y Luisiana, 1-jul-2026, ya se confirmaron en Apple el 23-nov-2026.)*
- Texto vigente de la **Política de Familias**, **Data safety** y la regla de cajas botín de **Google Play** (support.google.com no estuvo disponible). *(Que Google exige D-U-N-S a organizaciones ya está confirmado en developer.android.com; ver reporte 03.)*
- Alcance exacto del **IEPS 2026** a videojuegos y qué sistema de clasificación usa.
- Si **PEGI o la UE** cambiaron en 2025–2026 los criterios para juegos con cajas botín o monedas virtuales, y el estado de la ley de "equidad digital" de la UE.
- Criterio de la **SCJN (2025)** sobre obras generadas por IA: número de asunto y alcance.
- Si la marca **INKCAP** ya registrada cubre las clases 9, 28, 35 y 41.

---

## Fuentes

### Verificadas hoy (23-nov-2026) ✅
- Apple – App Review Guidelines (1.2, 1.2.1(a), 1.3, 2.3.6, 3.1.1, 4.1, 5.1.1(v), 5.1.4, 5.2.1, 5.2.2, 5.3, 5.6): https://developer.apple.com/app-store/review/guidelines/
- Apple – Age ratings values and definitions (4+/9+/13+/16+/18+, tabla de descriptores, Australia, Corea, Brasil, Vietnam): https://developer.apple.com/help/app-store-connect/reference/app-information/age-ratings-values-and-definitions/
- Apple Developer News (age ratings 24-jul-2025; Texas 8-oct-2025, 4-nov-2025, 23-dic-2025 y 3-jun-2026; Brasil/Australia/Singapur/Utah/Luisiana 24-feb-2026; Australia y Vietnam 21-may-2026; Corea 12-ago-2026; redes sociales 9-jul-2026; Time Allowances; nuevos descriptores 2-nov-2026; trader status 16-ene y 18-feb-2025): https://developer.apple.com/news/
- Apple – App Privacy Details: https://developer.apple.com/app-store/app-privacy-details/
- Apple – User Privacy and Data Use (ATT): https://developer.apple.com/app-store/user-privacy-and-data-use/
- Apple – EU Digital Services Act trader requirements: https://developer.apple.com/help/app-store-connect/manage-compliance-information/manage-european-union-digital-services-act-trader-requirements
- Apple – Enrollment (organización, D-U-N-S, USD 99): https://developer.apple.com/programs/enroll/
- Apple – Custom license agreement (EULA): https://developer.apple.com/help/app-store-connect/manage-app-information/provide-a-custom-license-agreement
- Apple – Invoices and tax documents (México): https://developer.apple.com/help/app-store-connect/manage-tax-information/manage-invoices-and-other-tax-documents
- *(Verificación del 23-nov-2026)* Apple News – Utah, Luisiana, Brasil, Australia y Singapur (24-feb-2026): https://developer.apple.com/news/?id=f5zj08ey
- *(Verificación del 23-nov-2026)* Apple News – ATT en la UE (16-sep-2026): https://developer.apple.com/news/?id=idsft9ai
- *(Verificación del 23-nov-2026)* Google – Play Age Signals API (beta): https://developer.android.com/google/play/age-signals/overview

### De otros reportes de esta investigación 📄
- Reporte 01 (*Boomerang Fu*, qué no copiar, ESRB E10+): ./01-boomerang-fu.md
- Reporte 03 (cuentas de tienda, D-U-N-S): ./03-motor-de-juego-y-herramientas.md
- Reporte 05 (políticas de tiendas, cajas botín, Brasil, FTC v. Epic): ./05-backend-progresion-y-liveops.md
- Reporte 06 (impuestos, IVA de Apple, regalías, IEPS): ./06-monetizacion-tiendas-e-impuestos.md
- PC Gamer – Tetris vs. Xio: https://www.pcgamer.com/court-declares-tetris-clone-a-breach-of-copyright-big-implications-for-derivative-games/
- Loeb & Loeb – Tetris Holding v. Xio: https://www.loeb.com/zh-hans/insights/publications/2012/06/tetris-holding-llc-v-xio-interactive-inc
- Medium – Clones Wars (Spry Fox v. 6waves y otros): https://medium.com/patents-technology-law/clones-wars-video-game-litigation-illustrated-36682abb4d68
- Wham-O – protección de marca (Frisbee): https://wham-o.com/pages/wham-o-brand-protection
- Justia – marcas BOOMERANG: https://trademarks.justia.com/760/20/boomerang-76020095.html
- Nintendo UK – Boomerang Fu (clasificación): https://www.nintendo.com/en-gb/DLC/Boomerang-Fu-Street-Food-DLC-3235070.html
- Tipo de cambio: https://dolar.info/mexico/hoy/precio/banxico-fix.php

### Referencias oficiales para verificar lo marcado ⚠️ (NO consultadas hoy)
- Ley Federal de Protección de Datos Personales en Posesión de los Particulares (2025): https://www.diputados.gob.mx/LeyesBiblio/pdf/LFPDPPP.pdf
- Ley Federal del Derecho de Autor: https://www.diputados.gob.mx/LeyesBiblio/pdf/LFDA.pdf
- Ley Federal de Protección a la Propiedad Industrial: https://www.diputados.gob.mx/LeyesBiblio/pdf/LFPPI.pdf
- Ley Federal de Juegos y Sorteos: https://www.diputados.gob.mx/LeyesBiblio/pdf/LFJS.pdf
- IMPI – MARCia: https://marcia.impi.gob.mx/
- INDAUTOR: https://www.indautor.gob.mx/
- OMPI – Tasas del Sistema de Madrid y calculadora: https://www.wipo.int/madrid/en/fees/
- OMPI – Global Brand Database: https://branddb.wipo.int/
- USPTO – Fee schedule: https://www.uspto.gov/learning-and-resources/fees-and-payment/uspto-fee-schedule
- EUIPO – Tasas: https://www.euipo.europa.eu/en/trade-marks/before-applying/fees-payable-direct-filing
- FTC – Reforma de la Regla COPPA (ene-2025): https://www.ftc.gov/news-events/news/press-releases/2025/01/ftc-finalizes-changes-childrens-privacy-rule-limiting-companies-ability-monetize-kids-data
- FTC – Genshin Impact / HoYoverse (ene-2025): https://www.ftc.gov/news-events/news/press-releases/2025/01/genshin-impact-game-developer-will-be-banned-selling-lootboxes-teens-under-16-without-parental-consent-pay-20-million-fine
- FTC – Epic Games (dic-2022): https://www.ftc.gov/news-events/news/press-releases/2022/12/fortnite-video-game-maker-epic-games-pay-more-half-billion-dollars-over-ftc-allegations
- Brasil – Ley 15.211/2025 (ECA Digital): https://www.planalto.gov.br/ccivil_03/_ato2023-2026/2025/lei/L15211.htm
- RGPD (Reglamento UE 2016/679): https://eur-lex.europa.eu/eli/reg/2016/679/oj
- IARC – Cómo funciona: https://www.globalratings.com/how-iarc-works.aspx
- Google Play – Política de Familias: https://support.google.com/googleplay/android-developer/answer/9893335
- Google Play – Seguridad de los datos: https://support.google.com/googleplay/android-developer/answer/10787469
- Google Play – Política de pagos (cajas botín): https://support.google.com/googleplay/android-developer/answer/9858738
- Casos de EE. UU. (referencias judiciales): *Atari v. Philips*, 672 F.2d 607 (7th Cir. 1982); *Data East v. Epyx*, 862 F.2d 204 (9th Cir. 1988); *Tetris Holding v. Xio*, 863 F. Supp. 2d 394 (D.N.J. 2012); *DaVinci Editrice v. ZiKo Games*, 183 F. Supp. 3d 820 (C.D. Cal. 2016); *Spry Fox v. LOLApps*, No. 2:12-cv-00147 (W.D. Wash. 2012).
- Corea: *King.com v. Avocado Entertainment*, Suprema Corte, 2017Da212095 (27-jun-2019).

---

## Verificación de datos (2026-11-23)

**Cómo se hizo.** Un verificador independiente revisó las afirmaciones de alto riesgo: precios, tasas, fechas de leyes y políticas, requisitos de tiendas y estado de servicios. **Límites de esta pasada:** el presupuesto de búsquedas web de la sesión seguía agotado, y **www.wipo.int y www.uspto.gov no se pudieron abrir** (el entorno no los permite, igual que www.ftc.gov y www.diputados.gob.mx). Solo se pudieron leer de nuevo **developer.apple.com** (Developer News, App Review Guidelines, tabla de clasificaciones de edad, User Privacy and Data Use) y **developer.android.com** (Play Age Signals). Para ver esas tasas y leyes hay que permitir esos dominios en la configuración de red de la sesión o pedir una segunda ronda con búsquedas disponibles.

### Qué se corrigió o completó

| Sección | Decía | Ahora dice | Fuente |
|---|---|---|---|
| Resumen 7 y §3.5 (Utah y Luisiana) | "Reglas parecidas en 2026" / "Obligaciones en 2026 ✅/⚠️" | Utah: cuentas nuevas desde el **6-may-2026**. Luisiana: desde el **1-jul-2026** | [Apple, 24-feb-2026](https://developer.apple.com/news/?id=f5zj08ey) |
| Resumen 8, §4.1 y §5.1 (cajas botín en Brasil) | No mencionaba la clasificación de Apple en Brasil | Si se declaran cajas botín, Apple sube la clasificación en **Brasil a 18+** | [Apple, 24-feb-2026](https://developer.apple.com/news/?id=f5zj08ey) |
| §3.5 (Australia, Brasil, Singapur) | Sin detalle | Desde el 24-feb-2026 Apple bloquea las apps 18+ si no se confirmó que el usuario es adulto | Ídem |
| §3.5 (Google Play Age Signals) | "⚠️ no pude abrir la documentación" | Existe **en beta**: señales de edad, aviso de cambios significativos, notificación de revocación. Sus términos prohíben usarla para anuncios, marketing, perfilado o analítica | [developer.android.com](https://developer.android.com/google/play/age-signals/overview) |
| §3.3 (ATT en la UE) | "Obligatorio en 5 países; se puede volver a preguntar después de un año" (sin fuente puntual) | Confirmado y precisado: anunciado el 16-sep-2026 por acuerdos con autoridades de competencia; es **el único aviso disponible** en Alemania, Francia, Italia, Polonia y Rumania; se puede volver a preguntar **un año después** de la última respuesta, salvo que el usuario haya desactivado las solicitudes | [Apple – User Privacy](https://developer.apple.com/app-store/user-privacy-and-data-use/), [News](https://developer.apple.com/news/?id=idsft9ai) |
| §4.1 (preguntas nuevas de 2026) | "Time Allowances para juegos (septiembre)" | Las preguntas de **redes sociales** se agregaron el 9-jul-2026 y son **obligatorias desde sep-2026**. **Time Allowances** es el control parental de iOS 27 (8-jun-2026), que también cubre Juegos, y no una pregunta "de septiembre". Ubicación compartida y citas: desde el 16-nov-2026, plazo 12-ene-2027 | [Apple News](https://developer.apple.com/news/) |
| §5.1 (Australia) | "Cajas botín 16+ en Australia" | Igual, y se precisa que es desde el **18-jun-2026**, cuando Apple retiró el 15+ en Australia | [Apple News, 21-may-2026](https://developer.apple.com/news/) |
| §5.1 (UE, principios de la CPC) | "En 2024 la CPC publicó principios sobre monedas virtuales" | Fue en **mar-2025** (dato del verificador, ⚠️ no reconsultado en línea). Estado de la *Digital Fairness Act* a nov-2026: no verificado | Conocimiento del verificador |
| §1.3 (guía 2.3.7) | "⚠️ no la releí hoy" | Releída: prohíbe meter "términos registrados como marca" y "nombres de apps populares" en metadatos; nombre único de **máx. 30 caracteres** | [App Review Guidelines](https://developer.apple.com/app-store/review/guidelines/) |
| §9.3 (D-U-N-S de Google) | "La cuenta de organización también pide D-U-N-S ⚠️; hasta ~30 días" | D-U-N-S y sitio web verificado **confirmados** (reporte 03). La FAQ oficial dice **hasta 28 días** | [Android – verificación de desarrolladores](https://developer.android.com/developer-verification/guides), [FAQ](https://developer.android.com/developer-verification/guides/faq) |
| §3.2 (multas LFPDPPP) | Solo la UMA 2025 | Se agregó la UMA 2026 (≈ $117.31 → ≈ MXN 37.5 M por 320,000 UMA). ⚠️ Dato del verificador, confirmar en INEGI | Conocimiento del verificador |
| §3.4 (multa COPPA) | USD 53,088 sin aclaración | Se aclaró que es la cifra de 2025 y que la FTC la ajusta cada enero | Conocimiento del verificador |
| §7.5 (SCJN e IA) | Sin número de asunto | Pista: probablemente el **Amparo Directo 6/2025** (ago-2025) ⚠️ | Conocimiento del verificador |

### Qué se confirmó (✅ leído hoy en la fuente oficial)

- **Texas SB 2420:** iba a entrar el 1-ene-2026. Apple anunció el 23-dic-2025 que una suspensión judicial la frenaba y el 3-jun-2026 que un fallo levantó esa suspensión. Desde el **4-jun-2026** aplica a cuentas nuevas de Texas, con Declared Age Range API, Significant Change API (PermissionKit), notificaciones de retiro de consentimiento y pruebas en *sandbox* ([Apple News](https://developer.apple.com/news/)).
- **Clasificaciones de Apple** 4+/9+/13+/16+/18+ desde el 24-jul-2025, con plazo del cuestionario el **31-ene-2026**.
- **Tabla de Apple:** violencia caricaturesca 9+ poco frecuente / 13+ frecuente; violencia realista 13+/18+; concursos 4+/13+; apuesta simulada 13+/18+; cajas botín 9+ (Australia 16+, Vietnam 12+); contenido de usuarios, chat y publicidad 4+; redes sociales 13+; web libre 16+. **Corea (GRAC):** violencia poco frecuente y cajas botín "All", frecuente 12+, redes sociales 15+.
- **Guías de Apple:** 4.1(a)(b)(c) Copycats; 3.1.1 (probabilidades de cajas botín antes de comprar; la moneda comprada no caduca); 5.1.2(i) (no condicionar funciones ni compensaciones al rastreo); 5.3.1 y 5.3.2 (concursos patrocinados por el desarrollador, con reglas en la app que digan que Apple no es patrocinador); 5.6 (prácticas manipuladoras; reembolsos excesivos como señal de mala calidad).
- **ATT:** prohibido el *fingerprinting*. El IDFV se puede usar para analítica propia sin ATT.
- **DSA:** desde el 17/18-feb-2025 se retiraron del App Store de la UE las apps sin estatus de comerciante.
- **Brasil en Apple:** clasificaciones oficiales del Ministerio de Justicia; ubicación compartida = A16 y citas = A18 (2-nov-2026).

### Qué NO se pudo verificar (sigue ⚠️)

- **Tasas y tiempos del IMPI 2026** (≈ MXN 3,000–3,600 por clase con IVA; 4–6 meses) y **tarifas de INDAUTOR 2026**.
- **Tasas de Madrid** (CHF 653 / 903), **USPTO** (USD 350 por clase desde el 18-ene-2025; abogado de EE. UU. obligatorio desde 2019) y **EUIPO** (€850/€50/€150). Coinciden con lo que conoce el verificador, pero **wipo.int y uspto.gov no se pudieron abrir**.
- **Leyes mexicanas** (LFPDPPP 2025 y su reglamento, LFDA, LFPPI, Ley Federal de Juegos y Sorteos) e **IEPS 2026 a videojuegos violentos** (8%, clasificación "C" o superior).
- **COPPA** (fechas de la reforma: 22-abr-2025, 23-jun-2025 y 22-abr-2026) y casos de la FTC (**Epic USD 520 M**, **HoYoverse USD 20 M**). Coinciden con lo que conoce el verificador, pero **ftc.gov no está permitido**.
- **ECA Digital de Brasil** (Ley 15.211/2025: vigencia desde mar-2026, multas de hasta R$ 50 M o 10%), Bélgica, Países Bajos, Corea (daños de hasta 3x), Japón, Australia (clasificación M / R18+), Reino Unido, España y China.
- **Estado a nov-2026** de *Nintendo v. Pocketpair*, de la reexaminación de la patente de Nintendo en EE. UU. y de la apelación en *NCSoft v. Webzen*.
- **California AB 1043** (vigente desde el 1-ene-2027), demandas estatales contra Roblox y RGPD (edades del art. 8, CMP de AdMob desde ene-2024).
- **Políticas de Google Play** (Familias, Data safety, cajas botín): support.google.com no estuvo disponible.
- **Tipos de cambio** EUR y CHF a USD y **precios de mercado** (honorarios, dominios, notario, representante en la UE).

*Fin del reporte 07. No es asesoría legal.*
