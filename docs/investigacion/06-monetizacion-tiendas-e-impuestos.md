# 06 · Monetización, comisiones de tiendas, impuestos en México y modelos de negocio

> **Proyecto:** "Boomerang Fighters" (nombre de trabajo), un *party brawler* móvil con muñequitos estilo *art toy*, objeto arrojadizo que regresa y escenarios de arte urbano.
> **Fecha de corte:** 23-nov-2026.
> **Tipo de cambio usado:** **1 USD ≈ 19.1 MXN** (FIX de Banxico del 20-nov-2026: 19.0228; mercado del 22-nov-2026: ~19.16). Es el mismo que usa el reporte 02 de esta investigación ([dolar.info – FIX Banxico](https://dolar.info/mexico/hoy/precio/banxico-fix.php)).

---

## Cómo leer este reporte (importante)

Cada dato lleva una etiqueta según qué tan seguro es:

| Etiqueta | Qué significa |
|---|---|
| ✅ **Verificado hoy** | Lo leí el 23-nov-2026 directamente en la página oficial (casi todo es de developer.apple.com). |
| 📄 **De otro reporte** | Viene de los reportes 01–04 de esta misma investigación, que sí consultaron la fuente original. Doy esa URL. |
| ⚠️ **Conocimiento previo** | No lo pude confirmar hoy. Es información que yo conocía hasta mediados de 2026. **Hay que verificarlo antes de decidir.** |

**Limitación honesta:** durante esta investigación se agotó el presupuesto de búsquedas web de la sesión y el entorno no permite abrir `support.google.com` ni Wikipedia. Por eso **todo lo de Apple está verificado en fuente oficial, pero casi todo lo de Google Play viene de conocimiento previo** y está marcado con ⚠️. Para confirmarlo hay que permitir `support.google.com` en la configuración de red de la sesión, o pedirme una segunda ronda de búsqueda.

**Esto no es asesoría fiscal ni legal.** La parte de impuestos describe cómo funciona el sistema para que la conversación con el contador sea más productiva.

---

## Resumen ejecutivo

1. **El modelo ganador del género es gratis para jugar (F2P), con cosméticos y pase de temporada.** Brawl Stars mejoró sus ingresos al **quitar las cajas de botín** y mejorar el pase ([Deconstructor of Fun](https://www.deconstructoroffun.com/blog/2024/2/11/brawl-stars-to-the-moon)) 📄. Among Us se volvió fenómeno y aun así **ganó poco porque casi no tenía qué vender** 📄. Nuestro juego necesita la tienda y el pase listos antes de que llegue el éxito.
2. **La comisión que pagaremos será de 15%, no de 30%.** Apple cobra 15% a quien gana menos de USD 1 M al año en el **App Store Small Business Program** (hay que inscribirse) ✅. Google Play cobra 15% sobre el primer USD 1 M de cada año ⚠️.
3. **En México no hay ninguna regla especial que baje comisiones o permita pagos externos dentro de la app.** Las excepciones existen en EE.UU., la Unión Europea, Japón, Brasil, Corea, China, Taiwán y Países Bajos ✅/⚠️. Dentro de la app en México **no se puede poner un botón que diga "cómpralo más barato en nuestra web"** ✅.
4. **Sí se puede vender fuera de la app y usarlo dentro.** La regla 3.1.3(b) de Apple permite que el jugador use dentro de la app lo que compró en tu web, **siempre que ese artículo también se venda como compra dentro de la app** ✅. Esto abre la puerta a OXXO y Mercado Pago en una tienda web propia y al **phygital** (playera física + traje digital).
5. **Phygital sí, pero con el diseño correcto.** Apple prohíbe que la app use **sus propios** códigos QR o claves para desbloquear contenido ✅. La solución que usan Roblox, Epic y Pokémon GO es **canjear el código en una página web**, fuera de la app, ligada a la cuenta del jugador ⚠️. La otra opción es usar los **Offer Codes oficiales de Apple** ✅.
6. **Vender playeras y gorras físicas desde el juego está permitido y no paga comisión.** Apple **obliga** a cobrar los bienes físicos fuera de su sistema de compras (regla 3.1.3(e)) ✅. Esto convierte al juego en un escaparate para el e-commerce de INKCAP.
7. **IVA: la empresa mexicana lo recibe y lo paga ella misma.** Según la documentación oficial de Apple, a las **empresas con sede en México** Apple les entrega el IVA de sus ventas en México para que ellas lo enteren al SAT. Además les descuenta el IVA de la comisión de Apple, que es acreditable. A las empresas **no** les retiene ISR; a las personas físicas sí ✅. Conclusión: **publicar como persona moral**, con RFC y Constancia de Situación Fiscal.
8. **Los anuncios ayudan, pero no sostienen el negocio en México.** Un anuncio en Latinoamérica vale **4–5 veces menos** que en Norteamérica, unos **US$2–3.4 por cada mil vistas** en video recompensado de Android ([Playio](https://blog.playio.co/mobile-game-ecpm-benchmarks-2026)) 📄. Recomiendo **solo video recompensado y opcional**.
9. **Las regalías a artistas (15%) caben en el modelo.** En un pase de MXN 99, después de IVA y comisión quedan ~MXN 72.5. El 15% para el artista son ~MXN 10.9 y a INKCAP le quedan ~MXN 61.7 antes de ISR (cálculo en §7.5).
10. **Proyección a 12 meses del lanzamiento**, ingreso neto antes de sueldos, marketing e ISR: **conservador ~MXN 100 mil, base ~MXN 960 mil, optimista ~MXN 8.4 millones**. Aparte, hay ventas de merch atribuibles al juego de MXN 135 mil a 4.5 millones (§10).
11. **Hay dinero que no se devuelve.** El **Google Play Indie Games Fund LATAM** da US$150–200 mil por estudio y en 2025 lo ganó un estudio mexicano 📄. **Apple Arcade** recibe propuestas de juegos sin lanzar, aunque exige juego sin anuncios ni compras ✅.

---

## 1. Modelos de negocio: qué existe y quién lo usa

### 1.1 Tabla general

| Modelo | Cómo funciona | Quién lo usa (ejemplos) | Pros | Contras | ¿Para nosotros? |
|---|---|---|---|---|---|
| **Premium** (pago único) | Se cobra la descarga | Boomerang Fu: USD 14.99 en consola/PC, más DLC de USD 2.99–4.99 📄 ([Gematsu](https://www.gematsu.com/2020/07/frantic-boomerang-brawler-boomerang-fu-for-xbox-one-switch-and-pc-launches-august-13), [Steam](https://store.steampowered.com/app/4629160/Boomerang_Fu_Street_Food_DLC/)) | Simple, sin presión de monetizar | En móvil casi nadie paga por descargar, y menos en LATAM; mata el multijugador (todos deben pagar) | ❌ En móvil. ✅ Si algún día sale en Switch o Steam |
| **F2P + cosméticos** | Gratis; se venden trajes, emotes y efectos | Brawl Stars, Fortnite, Stumble Guys 📄 | Máximo alcance; no rompe la competencia | Necesita contenido constante | ✅ **Pilar principal** |
| **Pase de temporada** | Gratis + premium (~US$3–10) con niveles por XP | Brawl Stars (Brawl Pass), Fortnite, Free Fire (Booyah Pass) ⚠️ | El ingreso más predecible; premia jugar diario | Hay que producir 1 temporada cada 6–8 semanas | ✅ **Pilar principal** (1 técnica de tinta por temporada) |
| **Video recompensado** | "Ve un anuncio y duplica tu XP" | Juegos híbrido-casuales (Archero, etc.) 📄 | Monetiza al 97–99% que nunca paga | eCPM bajo en México | ✅ Complemento |
| **Anuncios forzados** (intersticiales, banners) | Anuncio entre rondas | Juegos hiper-casuales | Ingresos inmediatos | Destruyen la experiencia social | ❌ |
| **Moneda virtual (IAP)** | Gemas o "tinta" para comprar cosméticos | Todos los F2P | Flexibilidad, paquetes | Hay que diseñar bien la economía | ✅ Con moneda "premium" sencilla |
| **Suscripción tipo "Club"** | Pago mensual con beneficios | Fortnite Crew (~US$11.99/mes) ⚠️, Roblox Premium ⚠️, Gold Pass de Clash of Clans ⚠️ | Ingreso recurrente | Debe dar valor continuo (Apple pide un periodo mínimo de 7 días ✅) | ✅ Etapa 3 ("Club INKCAP") |
| **Bundles / starter packs** | Paquete inicial muy barato | Casi todos los F2P | Convierte al primer pagador | Si abusas, se siente agresivo | ✅ Desde el lanzamiento |
| **Programa de creadores** | El creador recibe un % de lo que gastan sus seguidores | Supercell (Creator Boost) 📄 ([Supercell](https://supercell.com/en/games/brawlstars/blog/community/creator-boost-faq/)); Epic "Support-A-Creator" (~5%) ⚠️ | Marketing que solo pagas si vende | Requiere administración y pagos | ✅ Etapa 2–3 (artistas e INKCAP TV) |
| **Suscripciones de plataforma** | Apple Arcade, Netflix Games, Google Play Pass pagan por el juego o por su uso | Apple Arcade: juegos "sin anuncios ni compras dentro de la app" ✅ ([Apple](https://developer.apple.com/apple-arcade/)) | Financiamiento antes de lanzar | Pierdes la tienda de cosméticos de INKCAP (Arcade) o el control | 🤔 Opción B (§1.4) |
| **Patrocinio de marca** | Marcas pagan por aparecer: espectaculares en el escenario, skins patrocinadas | Free Fire: picos de ingreso con colaboraciones 📄 ([Sensor Tower](https://sensortower.com/blog/2025-q4-android-top-5-third-person-games-units-mx-60214330241bc16eb89ce15d)) | Encaja con calles grafiteadas (muros = espacio publicitario) | Requiere audiencia demostrable | ✅ Etapa 3+ |
| **Torneos / eSports** | Torneos con premio, patrocinios | Brawl Stars Championship ⚠️ | Comunidad, prensa | Caro; con inscripción pagada puede requerir permiso de Gobernación (Ley Federal de Juegos y Sorteos) ⚠️ | ✅ Torneos locales gratis en eventos INKCAP |
| **Phygital** (físico + digital) | La mercancía física trae el traje digital y viceversa | Forever 21 vendió un gorro virtual 1.5 M veces antes de hacerlo físico; Ralph Lauren vendió botas físicas tras ítems en Fortnite 📄 ([Modern Retail](https://www.modernretail.co/technology/how-ralph-laurens-phygital-boot-sale-in-fortnite-shows-benefits-and-limits-of-virtual-commerce/), [Glossy](https://www.glossy.co/fashion/digital-fashion-brand-play-pop-go-launches-first-physical-collection)) | **Ventaja única de INKCAP** | Reglas de las tiendas (§7) | ✅✅ **Diferenciador** |

### 1.2 Lo que hacen los grandes

- **Supercell (Brawl Stars, Clash Royale):** F2P con pase de temporada (versión normal y "plus"), tienda diaria rotativa, ofertas personalizadas, códigos de creador y una **tienda web propia** (Supercell Store) que da bonos extra por comprar ahí ⚠️. En 2024 Brawl Stars multiplicó sus ingresos ~9 veces frente a 2023, y los análisis lo atribuyen a un mejor pase, una economía más ordenada y la eliminación de las cajas de botín 📄 ([GamingOnPhone](https://gamingonphone.com/news/brawl-stars-revenue-jumped-to-almost-9x-and-mau-to-2-4x-in-2024-compared-to-last-year/)).
- **Epic (Fortnite):** pase de batalla de ~950 V-Bucks, tienda diaria de skins de 800–2,000 V-Bucks, suscripción Fortnite Crew, colaboraciones con marcas (Nike, Ralph Lauren) y "Support-A-Creator" ⚠️. Después del fallo contra Apple de abril 2025, Fortnite regresó al App Store de EE.UU. con enlaces a su propio sistema de pago ⚠️.
- **Roblox:** moneda única (Robux), Premium mensual, artículos de avatar creados por la comunidad con reparto de ingresos y **códigos en juguetes físicos** ⚠️.
- **Garena (Free Fire), el rey de LATAM:** pase muy barato, eventos de "ruleta" y colaboraciones con anime. En México registró semanas de ~US$2 M con la colaboración *Jujutsu Kaisen* (dic-2025) 📄 ([Sensor Tower](https://sensortower.com/blog/2025-q4-android-top-5-third-person-games-units-mx-60214330241bc16eb89ce15d)). **Ojo:** su modelo de ruletas (azar pagado) **no** lo recomiendo (§6.4).

### 1.3 Lo que hacen los indies exitosos

- **Stumble Guys (8–9 personas):** F2P con skins y gemas. **LATAM fue la región con más descargas** y Brasil fue el #2 mundial en ingresos. En vez de comprar anuncios, **regaló gemas a influencers** para que las sortearan entre su audiencia 📄 ([PocketGamer.biz](https://www.pocketgamer.biz/battle-royale-game-stumble-guys-is-making-almost-half-a-million-dollars-every-day/), [GameAnalytics](https://gameanalytics.com/?p=17893)).
- **Among Us (3 personas):** cosméticos baratos, pago por quitar anuncios en móvil y precio en PC. Tuvo ~908 M de descargas móviles y solo ~US$60 M de ingresos móviles 📄 ([Udonis](https://www.blog.udonis.co/mobile-marketing/mobile-games/among-us-player-count)). **Lección:** sin temporadas ni catálogo, el éxito viral no se convierte en dinero.
- **BombSquad (1 persona):** fichas ("tokens") y un "Gold Pass" que se compra una vez como alternativa a las microtransacciones 📄 ([ballistica.net](https://ballistica.net/whataretokens)). **Lección:** un pase de pago único y honesto genera lealtad.
- **Boomerang Fu (premium):** vendió más en su segundo año que en el primero y sigue sacando DLC en 2026 📄. **Lección:** el contenido después del lanzamiento es lo que hace crecer los ingresos.

### 1.4 Alternativas de financiamiento: Apple Arcade, Netflix Games, Google Play Pass y fondos

| Opción | Cómo paga | Requisitos | Encaje |
|---|---|---|---|
| **Apple Arcade** | Apple financia y paga una parte según participación; los términos no son públicos ✅/⚠️ | Juego **sin anuncios ni compras dentro de la app** ✅; propuesta de juego **no lanzado** vía formulario ✅ ([Apple Arcade](https://developer.apple.com/apple-arcade/)) | Choca con los trajes de INKCAP. Solo valdría como versión "edición Arcade" separada |
| **Netflix Games** | Licencias o financiamiento por proyecto ⚠️ | Desde 2025 se enfoca en juegos *party* en TV con el celular como control, juegos para niños y juegos de sus series ⚠️ | Un brawler local es justo el tipo de *party game*. Exploratorio |
| **Google Play Pass** | Google reparte un fondo según el uso ⚠️ | Versión sin anuncios ni compras ⚠️ | Complemento menor |
| **Google Play Indie Games Fund LATAM** | **US$2 M repartidos, US$150–200 mil por estudio**, sin ceder acciones 📄 | ≤ 50 empleados, juego ya publicado; en 2025 ganó un estudio mexicano 📄 ([Google Play](https://play.google.com/intl/id_ALL/console/about/programs/indiegames/), [Google Blog](https://blog.google/products-and-platforms/platforms/google-play/indie-games-fund-latin-america-2025/)) | ✅ **Aplicar en cuanto el juego esté publicado** |

---

## 2. Comisiones de las tiendas en 2026

### 2.1 Apple App Store (verificado hoy)

**Small Business Program** ([Apple](https://developer.apple.com/app-store/small-business-program/)) ✅:
- Comisión de **15%** en apps de pago y compras dentro de la app, si tú y tus **cuentas asociadas** ganaron **≤ USD 1 M de proceeds** en el año calendario anterior. Las cuentas nuevas también califican.
- "Proceeds" significa lo que te queda **después** de la comisión y de impuestos. O sea, el límite equivale a ~USD 1.18 M de ventas netas de impuestos.
- Si en el año pasas de USD 1 M, las ventas siguientes pagan la tarifa estándar. Si después bajas del límite, recalificas al año siguiente.
- **Hay que inscribirse:** ser Account Holder, aceptar el Paid Apps Agreement y declarar las **cuentas asociadas**. Una cuenta es asociada si alguien tiene más del 50% de la propiedad o el control de las decisiones. Si INKCAP y el estudio VR comparten dueños y ambos publican en Apple, sus ventas se suman.
- La tarifa reducida empieza 15 días después del cierre del mes fiscal en que te aprueban.

**Comisiones por región (Apple):**

| Región | Compra con Apple (IAP) | Pago alterno dentro de la app | Enlace a web | Fuente |
|---|---|---|---|---|
| **México y resto del mundo** | 30% estándar / **15% Small Business** | ❌ No permitido | ❌ Prohibido dentro de la app (regla 3.1.1(a)) | ✅ [Guías](https://developer.apple.com/app-store/review/guidelines/) |
| **EE.UU.** | 30% / 15% | — | ✅ **Se permiten botones y enlaces externos** sin pedir permiso | ✅ [Guías 3.1.1(a)](https://developer.apple.com/app-store/review/guidelines/), [News 1-may-2025](https://developer.apple.com/news/). Comisión sobre compras enlazadas: ver nota abajo ⚠️ |
| **Unión Europea** (desde 1-oct-2026) | 26% / **15%** | 20% / 10% | 15% / 10% (ventas dentro de 7 días del clic) | ✅ [Apple EU](https://developer.apple.com/support/dma-and-apps-in-the-eu/) |
| **Japón** (iOS 26.2+, ley MSCA) | 21% + 5% de procesamiento = 26%; Small Business 10% + 5% = **15%** | 21% / 10% | 15% / 10% (7 días) | ✅ [Apple Japón](https://developer.apple.com/support/app-distribution-in-japan/) |
| **Brasil** (iOS 26.5+, acuerdo con CADE) | 21% + 5%; Small Business 10% + 5% = **15%** | 21% / 10% | 15% / 10% (7 días) | ✅ [Apple Brasil](https://developer.apple.com/support/app-distribution-in-brazil/) |
| **China** (desde 15-mar-2026) | 25% / **12%** | — | — | ✅ [News](https://developer.apple.com/news/) |
| **Taiwán** (desde 31-oct-2026) | 26% estándar | — | — | ✅ [News](https://developer.apple.com/news/) |
| **Corea del Sur** | 30% / 15% | ~26% / ~12% (4 puntos menos) | — | ⚠️ |
| **Países Bajos** | — | Solo apps de citas | — | ✅ [Apple NL](https://developer.apple.com/support/storekit-external-entitlement/) |

**Nota sobre EE.UU. (Epic v. Apple)** ⚠️: el 30-abr-2025 una jueza federal declaró a Apple en desacato y le prohibió cobrar comisión por las compras hechas mediante enlaces externos en apps de EE.UU. Apple cambió sus guías el 1-may-2025 ✅. Hasta donde sé, a fines de 2025 la corte de apelaciones confirmó el desacato, pero abrió la puerta a que Apple cobre en el futuro alguna comisión "razonable" sobre esas compras, y regresó el tema al juzgado. **Hoy las guías dicen que en EE.UU. no se necesita permiso para poner enlaces** ✅. Si debe pagarse algún porcentaje hay que confirmarlo justo antes de lanzar allá. **Para México no cambia nada.**

**Otros cambios útiles de Apple** ✅ ([News](https://developer.apple.com/news/)):
- **29-oct-2025:** los **Offer Codes** ya cubren **todos** los tipos de compra dentro de la app, incluidos consumibles y no consumibles, es decir, trajes. Los antiguos *promo codes* de compras dentro de la app **ya no se pueden crear desde el 26-mar-2026**.
- **13-nov-2025:** nace el **Mini Apps Partner Program** (15%). No nos aplica.
- **Ninguna noticia de 2025–2026 menciona México.** Tampoco existe una página de Apple sobre "distribución en México" (da error 404). No encontré ningún régimen especial para nuestro país.

### 2.2 Google Play (⚠️ conocimiento previo; verificar en Play Console)

| Concepto | Lo que sé | Confianza |
|---|---|---|
| Comisión base | **15% sobre el primer USD 1 M** de ingresos de cada año (hay que crear un "grupo de cuentas"); 30% arriba de eso | ⚠️ Alta |
| Suscripciones | 15% desde el primer día | ⚠️ Alta |
| *User Choice Billing* | Facturación alterna con ~4 puntos menos, en países como el EEE, Corea, India, Japón e Indonesia. **No confirmé que aplique a México** | ⚠️ |
| EE.UU. (Epic v. Google) | Desde oct-2025 la orden judicial obliga a Google a permitir enlaces y facturación alterna en EE.UU. En nov-2025 Epic y Google anunciaron un acuerdo propuesto con topes de comisión de ~9% / 20% y "tiendas registradas". **No sé si el juez ya lo aprobó** | ⚠️ Media |
| Pago en efectivo | Google Play acepta **efectivo en OXXO y 7-Eleven** desde la app, y vende tarjetas de regalo de MXN 100 a 5,000 en OXXO, Walmart, Soriana y otras | 📄 ([Ayuda Google Play](https://support.google.com/googleplay/answer/9462077?hl=es-419), [tarjetas](https://support.google.com/googleplay/answer/3422734?hl=es)) |

**Ventaja para México:** Google Play ya cobra en efectivo en OXXO dentro de la app 📄, así que en Android **no hace falta** una web propia para alcanzar al jugador sin tarjeta.

### 2.3 ¿Y México?

- **No hay una ley tipo DMA** (Unión Europea) ni tipo MSCA (Japón) que obligue a Apple o Google a permitir pagos alternos en México ⚠️. La autoridad de competencia se reestructuró en 2025 (la COFECE pasó a ser la Comisión Nacional Antimonopolio) y **no encontré** ninguna resolución sobre tiendas de apps ⚠️.
- Ni las guías de Apple ni sus noticias 2025–2026 mencionan a México ✅.
- **Conclusión práctica:** en México hay que planear con **15% de comisión (Small Business)** y sin enlaces de pago dentro de la app. Lo que sí se puede hacer es **vender fuera de la app**, como explico en §3.

---

## 3. Tiendas web propias (web shops)

### 3.1 Qué son y por qué las usan los grandes

Una *web shop* es una página donde el jugador compra moneda o paquetes del juego con tarjeta, OXXO o Mercado Pago, y lo recibe en su cuenta dentro del juego. La comisión típica es de **3–8%** frente al 15–30% de las tiendas. Supercell, Scopely, Playtika y Moon Active las usan, y suelen dar **bonos extra** por comprar en la web ⚠️.

### 3.2 Proveedores

| Proveedor | Qué hace | Costo aproximado | México (OXXO, Mercado Pago, SPEI) | Confianza |
|---|---|---|---|---|
| **Xsolla** | Tienda web, pagos, *merchant of record* (se encarga de impuestos), cientos de métodos de pago | ~5% + método de pago | Sí (OXXO y otros) | ⚠️ |
| **Stash** | Tienda web para juegos móviles, enfocada en la oportunidad que abrió el fallo de EE.UU. | Por negociación | Por confirmar | ⚠️ |
| **Appcharge** | Tienda web D2C para juegos móviles, *merchant of record* | Por transacción, negociable | Por confirmar | ⚠️ |
| **Coda (Codashop)** | Recargas de juegos; muy fuerte en Asia y presente en LATAM | Reparto de ingreso | Por confirmar | ⚠️ |
| **Stripe** | Procesador. En México acepta tarjetas y **OXXO** | ~3.6% + MXN 3 + IVA por cobro | Sí (OXXO) | ⚠️ |
| **Mercado Pago / Conekta / Openpay** | Procesadores mexicanos | ~3.5–4% + IVA | Sí, nativo | ⚠️ |
| **El e-commerce actual de INKCAP** | Ya cobra con OXXO | Lo que ya pagan | Sí ✅ (dato del cliente) | — |

### 3.3 Las reglas (anti-steering) para México

1. **Dentro de la app (fuera de EE.UU.) no puede haber botones, enlaces ni mensajes** que lleven a comprar contenido digital fuera de la compra dentro de la app (regla 3.1.1(a)) ✅.
2. **Fuera de la app sí se puede comunicar:** "Developers can send communications outside of the app to their user base about purchasing methods other than in-app purchase" (3.1.3) ✅. Esto incluye redes sociales, el podcast INKCAP TV, correo, la tienda en línea y las etiquetas de las playeras.
3. **Lo comprado afuera se puede usar adentro** (3.1.3(b) Multiplatform Services): la app puede dar acceso a contenido "acquired... on other platforms or your web site, including consumable items in multi-platform games, **provided those items are also available as in-app purchases within the app**" ✅. Es decir: **todo lo que vendas en la web debe existir también como compra dentro de la app.**
4. **Google Play:** fuera de los programas especiales, tampoco permite dirigir al jugador a pagos externos desde la app. Sí permite usar en la app lo comprado fuera ⚠️.

### 3.4 ¿Se puede aceptar OXXO o Mercado Pago para monedas del juego?

**Sí, en la web propia** (la de INKCAP o una de Xsolla), siempre que se cumplan cuatro condiciones: (a) el jugador llega a la web por canales externos, (b) la compra se acredita a su cuenta del juego, (c) esos paquetes también existen como compra dentro de la app y (d) la web muestra precios en MXN con IVA incluido, como exige la ley de protección al consumidor ⚠️.

**Ejemplo con un paquete de MXN 99:**

| Concepto | App Store (15%) | Web propia con OXXO (~4% + MXN 3) |
|---|---|---|
| Precio al público (con IVA) | 99.00 | 99.00 |
| IVA 16% (se entera al SAT) | 13.66 | 13.66 |
| Comisión | 12.80 | 6.96 |
| **Le queda al estudio** | **72.54** | **78.38 (+8%)** |

**Mi opinión:** en el año 1 una web shop **no vale la pena solo por el ahorro**. Si 20% de las ventas se mueven a la web, el ingreso total sube ~1.5–2%, y cada paso extra hace que se pierdan compradores. **Sí vale la pena** como **tienda INKCAP de paquetes físicos + digitales** (§7), porque ese canal ya existe y cobra con OXXO. Una web shop dedicada tiene sentido cuando los ingresos superen ~US$20–50 mil al mes, o al lanzar en EE.UU., donde los enlaces están permitidos.

---

## 4. Impuestos en México (para platicar con el contador)

### 4.1 IVA 16%: quién lo cobra

- **Ley:** desde el 1-jun-2020 los servicios digitales que prestan empresas extranjeras a consumidores en México pagan IVA de 16% (Ley del IVA, Capítulo III Bis) ⚠️. Por eso los precios del App Store y de Google Play en México **ya incluyen IVA**.
- **Lo que dice Apple (verificado)** ([Apple – Provide tax information](https://developer.apple.com/help/app-store-connect/manage-tax-information/provide-tax-information)) ✅:
  - "All sales on the App Store in Mexico are subject to a Value-Added Tax (VAT)."
  - Apple **entera ella misma** el IVA cuando el desarrollador es una persona física sin registro fiscal o una empresa extranjera sin registro en México.
  - Apple **le entrega el IVA al desarrollador** cuando se trata de personas físicas con registro fiscal, empresas extranjeras registradas en México o **"All companies based in Mexico"**. **La empresa mexicana es quien entera ese IVA al SAT.**
  - **Empresas en México:** necesitan **Clave en el RFC** y **Cédula de Identificación Fiscal**, es decir, la Constancia de Situación Fiscal. "Your earnings will be reduced based on the VAT on Apple's commission, for which you may be able to receive an input tax credit". O sea, Apple descuenta el IVA de su comisión y ese IVA **se puede acreditar**.
  - **Personas físicas:** además se les retiene **ISR "on your total monthly worldwide sales before VAT"**.
  - Apple emite cada mes una **factura de IVA sobre su comisión** y una **constancia de retenciones**. Se descargan en App Store Connect → Reports → Tax Statements → Mexico ✅ ([Apple – Invoices](https://developer.apple.com/help/app-store-connect/manage-tax-information/manage-invoices-and-other-tax-documents)).
- **Google Play:** Google cobra y entera el IVA de las ventas a consumidores mexicanos. Cómo trata a los desarrolladores mexicanos (si les traslada el IVA como Apple o no) **no lo pude verificar** ⚠️.

### 4.2 ISR

| Si publican como… | Retención de la plataforma | Qué pagan |
|---|---|---|
| **Persona moral** (empresa, S.A.P.I., S.A. de C.V.) ✅ recomendado | Apple **no retiene ISR** ✅ | ISR de 30% sobre la utilidad (régimen general) ⚠️, más IVA mensual |
| **Persona física** con actividad en plataformas | Apple **sí retiene ISR** sobre las ventas mensuales ✅. La Ley del ISR (art. 113-A) fija **1%** para venta de bienes y servicios por plataformas; Apple no publica la tasa ⚠️ | Además, la retención de IVA aplicable ⚠️ |

**Recomendación:** publicar con la **persona moral** que vaya a ser dueña del juego y de la propiedad intelectual: INKCAP, el estudio VR o una nueva empresa conjunta. Hay que revisar dos cosas: (1) si sus ingresos más financiamiento superan USD 200 mil, Unity exige licencia Pro de USD 2,310 por puesto al año 📄 ([reporte 03](https://unity.com/products/pricing-updates)); (2) las **cuentas asociadas** del Small Business Program de Apple.

### 4.3 Facturación (CFDI)

- Las ventas a jugadores anónimos se documentan normalmente con un **CFDI global a "público en general"**, diario, mensual o bimestral según el régimen ⚠️. El contador debe conciliar estos CFDI con los reportes financieros de Apple y Google.
- Las facturas de comisión de Apple vienen de una entidad extranjera (para LATAM es Apple Services LATAM LLC, Florida ✅ según [Apple – Understanding taxes](https://developer.apple.com/help/app-store-connect/making-payments-to-apple/understanding-taxes)). No son CFDI, así que hay que confirmar con el contador cómo **deducirlas y acreditarlas** ⚠️.
- Las ventas a jugadores **de otros países** pueden calificar como exportación de servicios con IVA de 0% ⚠️.

### 4.4 Cómo llega el dinero

| | Apple ✅ | Google ⚠️ |
|---|---|---|
| Plazo | Dentro de **45 días** después del cierre del mes fiscal. Los reportes salen el primer viernes del mes siguiente | Mensual, alrededor del día 15 del mes siguiente |
| Requisitos | Paid Apps Agreement + datos fiscales + cuenta bancaria + **superar el mínimo de pago de cada país** | Perfil de pagos + datos fiscales |
| Moneda | El banco de Apple convierte a la moneda de tu cuenta, con tipo de cambio *spot* | Similar |
| Cuentas | Una sola cuenta principal, sin dividir pagos | — |

Fuente Apple: [Overview of receiving payments](https://developer.apple.com/help/app-store-connect/getting-paid/overview-of-receiving-payments) ✅. **Para la caja:** una venta del 1 de marzo puede llegar a mediados de mayo.

### 4.5 Retención de impuestos de EE.UU. (formulario W-8BEN-E)

- Apple pide a **todo** desarrollador un formulario fiscal de EE.UU. Fuera de EE.UU. puede ser el **W-8BEN** (persona física), el **W-8BEN-E** (empresa) o el W-8ECI. App Store Connect hace preguntas para elegir el correcto ✅. **Una vez enviado no se puede editar desde App Store Connect**; hay que contactar a Apple ✅.
- **Tratado México–EE.UU.** ⚠️: si no hay formulario, EE.UU. puede retener hasta **30%** de los ingresos de fuente estadounidense. Con un W-8BEN-E bien llenado que invoque el tratado, la retención normalmente baja a **0%** (beneficios empresariales, art. 7, si no hay establecimiento permanente en EE.UU.) o a **10%** (regalías, art. 12), según cómo se clasifique el ingreso. **Que el contador decida el artículo antes de enviarlo.**
- Lo mismo aplica para **AdMob/Google** (ingresos por anuncios vistos en EE.UU.) ⚠️.

### 4.6 Otros temas fiscales de 2026 a vigilar ⚠️

- **IEPS de 8% a videojuegos "violentos"** (Paquete Económico 2026), dirigido a juegos de clasificación "C" o superior. Un brawler caricaturesco **sin sangre** debería quedar en "A" o "B" y no pagarlo, pero **conviene cuidar el arte** (KO con "salpicadura de tinta", no de sangre) y confirmarlo con el contador.
- **Reforma 2026 al Código Fiscal:** obligaciones nuevas de las plataformas digitales para dar acceso en línea al SAT. Afecta a Apple y Google, no directamente a nosotros.

### 4.7 Lista para el contador

1. ¿Qué persona moral publica y es dueña de la propiedad intelectual?
2. ¿Esa empresa es "cuenta asociada" de otra que publica en Apple?
3. Cómo declarar el IVA que Apple entrega y acreditar el IVA de su comisión.
4. CFDI global y conciliación mensual con los reportes de Apple y Google.
5. Qué artículo del tratado se usa en el W-8BEN-E para Apple y para AdMob.
6. Qué tratamiento tienen las ventas al extranjero (¿IVA 0%?).
7. Contratos de regalías con artistas: retenciones si el artista es persona física y CFDI de pago.
8. Si hay torneos con premio: implicaciones fiscales y permisos.

---

## 5. Anuncios: cuánto pagan y cómo no arruinar el juego

### 5.1 Redes y mediación

| Plataforma | Qué es | Pros | Contras | Fuente |
|---|---|---|---|---|
| **AdMob** (Google) | Red + mediación gratuita | La más sencilla de integrar | Menor competencia entre redes si se usa sola | 📄 ([reporte 03](https://www.undrads.com/blogs/ironsource-vs-applovin-max-vs-admob)) |
| **Unity LevelPlay** (antes ironSource) | Mediación integrada a Unity | Natural para Unity; enfocada en juegos | La entrega de anuncios de ironSource Exchange y del SDK iAds termina el 31-dic-2026; LevelPlay sigue ⚠️ | 📄 |
| **AppLovin MAX** | Mediación independiente líder, con subasta entre más de 25 redes | Suele dar el mayor eCPM cuando hay volumen; mediación gratis | Más compleja | 📄 |

**Recomendación:** empezar con **AdMob** (o LevelPlay) solo con video recompensado. Cuando haya más de ~5,000 usuarios diarios, probar **MAX o LevelPlay con AdMob dentro de la mediación**.

### 5.2 eCPM (lo que pagan por cada 1,000 anuncios vistos)

| Formato | México / LATAM | EE.UU. | Fuente |
|---|---|---|---|
| Video recompensado, Android | **~US$2–3.4** (~MXN 38–65) | 4–5 veces más (~US$8–18) | 📄 [Playio](https://blog.playio.co/mobile-game-ecpm-benchmarks-2026), [Appodeal](https://appodeal.com/the-mobile-ecpm-report-updated-q4-2024-view); cifra de EE.UU. ⚠️ |
| Video recompensado, iOS | ~US$3–6 ⚠️ | ~US$10–25 ⚠️ | ⚠️ |
| Intersticial | Menor que el recompensado ⚠️ | — | ⚠️ |
| Banner | Centavos (US$0.05–0.5) ⚠️ | ~US$0.3–1 ⚠️ | ⚠️ |

**Traducción:** 1,000 jugadores diarios que ven 1.3 anuncios cada uno generan ~US$4.5 al día en México, unos **MXN 2,600 al mes**. Ayuda, pero no paga sueldos.

### 5.3 Buenas prácticas para un juego social

1. **Solo video recompensado y opcional.** Apple permite expresamente incentivar acciones "such as by completing a level or watching an ad" (3.2.2(x)) ✅.
2. **Nunca anuncios entre rondas** ni durante una partida local con amigos, porque rompen el ritmo del grupo.
3. **Ofertas recompensadas útiles:** duplicar la XP de la partida, una vuelta extra en la recompensa diaria, desbloquear un traje "de prueba" por 24 h o recuperar una racha diaria perdida.
4. **Límite diario** de 5–8 anuncios por jugador; más que eso devalúa la recompensa.
5. **Pago para quitar anuncios** (o incluirlo en el pase o el Club).
6. **Menores de edad** ⚠️: si el juego atrae niños menores de 13, hay que usar redes certificadas para familias, desactivar la personalización y cumplir las políticas de "Familias" de Google, la categoría Kids de Apple y COPPA si hay jugadores de EE.UU. Recomiendo una **clasificación 9+/12+** sin dirigirlo a niños, más una **verificación de edad** al crear cuenta.

---

## 6. Precios: escalones, comparables y métricas

### 6.1 Escalones de precio

- **Apple** ✅: hasta **800 puntos de precio** por defecto (más 100 superiores, hasta US$10,000, si se piden), desde ~US$0.29. Se elige un país base y Apple genera el precio para los **otros 174 países (43 monedas)** según tipo de cambio, impuestos y la convención local de precios. También **se puede fijar a mano el precio en México**, pero entonces Apple deja de ajustarlo ([Apple](https://developer.apple.com/help/app-store-connect/manage-in-app-purchases/set-a-price-for-an-in-app-purchase)).
- **Google Play** ⚠️: el precio de cada país se fija libremente dentro de un rango mínimo y máximo; hay plantillas de precio y conversión automática.
- **Recomendación:** usar **México como país base** para los paquetes clave y fijar precios "redondos mexicanos" (19, 29, 49, 79, 99, 149, 199, 499). **Hay que comprobar en App Store Connect cuáles puntos MXN existen exactamente**: no pude ver la tabla de equivalencias.

### 6.2 Precios en juegos similares (⚠️ aproximados, verificar en la tienda)

| Juego | Pase premium | Skins / trajes | Suscripción |
|---|---|---|---|
| Fortnite | ~950 V-Bucks (≈US$8–9) | 800–2,000 V-Bucks (≈US$8–20) | Fortnite Crew ≈US$11.99/mes |
| Brawl Stars | ≈US$7–10 (versión "Plus" más cara) | 29–299 gemas (≈US$2–20) | — |
| Stumble Guys | ≈US$5–10 | Con gemas | — |
| Free Fire | Muy barato (≈US$3–5) | Ruletas y paquetes | — |
| Boomerang Fu (premium) | — | DLC de US$2.99–4.99 con personajes y trajes 📄 | — |

### 6.3 Benchmarks de negocio (⚠️ rangos de la industria; usar como referencia, no como promesa)

| Métrica | Qué es | Global casual / *party* | México / LATAM |
|---|---|---|---|
| **ARPDAU** | Ingreso diario ÷ jugadores diarios | US$0.05–0.20 | **US$0.01–0.05** |
| **% de pagadores** | Jugadores del mes que compran algo | 1–3% (los top, 3–5%) | **0.5–2%** |
| **ARPPU** | Gasto mensual por pagador | US$10–25 | **US$4–10** |
| **LTV** | Lo que deja un jugador en toda su vida | US$0.5–3 por instalación | **US$0.10–0.50** por instalación |
| **CPI** | Costo por instalación con anuncios | — | **US$0.15–0.60** casual Android 📄 ([Game Growth Advisor](https://gamegrowthadvisor.com/blog/2026-05-26-latam-mobile-game-market-entry-guide-2026/)) |
| Retención D1 / D7 / D30 | Jugadores que regresan al día 1, 7 y 30 | Mediana 22% / 4% / 0.7%; 10% mejor: 40% / 12% 📄 ([GameDev Reports](https://gamedevreports.substack.com/p/gameanalytics-mobile-and-pc-game)) | — |

**Lectura:** si el LTV en LATAM ronda US$0.10–0.50 y el CPI cuesta US$0.15–0.60, **comprar instalaciones con anuncios casi no se paga solo**. Por eso el crecimiento debe venir de **comunidad, creadores, artistas y la tienda INKCAP**: es la ventaja injusta del cliente.

### 6.4 Catálogo de precios propuesto (MXN con IVA)

| Producto | Precio | ≈ USD | Comentario |
|---|---|---|---|
| **Starter pack** (playera básica + gorra + 200 de tinta) | **19** | 0.99 | Solo una vez, para convertir al primer pagador |
| Traje común (playera o gorra) | 29–39 | 1.5–2.0 | Rotación en la tienda diaria |
| **Traje de artista** (diseño real INKCAP) | 59–79 | 3.1–4.1 | Con regalía de 15% |
| Set completo (traje + gorra + estela de tinta + emote) | 99–149 | 5.2–7.8 | "Drop" de temporada |
| **Pase de temporada** | **79–99** | 4.1–5.2 | Similar a Free Fire y debajo de Fortnite |
| Pase premium + 10 niveles | 149 | 7.8 | |
| **Club INKCAP** (mensual) | 59 | 3.1 | Etapa 3 |
| Tinta (moneda): 100 / 550 / 1,200 / 3,250 | 19 / 99 / 199 / 499 | 1 / 5.2 / 10.4 / 26.1 | Bonos crecientes |

**Reglas de diseño:**
- **Nada de pagar para ganar.** El juego es de un golpe y fuera; cualquier ventaja pagada lo arruina.
- **Sin cajas de botín de pago.** Apple exige publicar las probabilidades ✅. Brasil las prohibió en juegos para menores (ECA Digital) ⚠️, y Brawl Stars mejoró al quitarlas 📄.
- **Se puede regalar a amigos:** Apple permite regalar artículos comprables, con reembolso solo al comprador original ✅.

---

## 7. PHYGITAL: la playera física desbloquea el traje digital (y viceversa)

### 7.1 Qué dicen las reglas

**Apple** ([App Review Guidelines](https://developer.apple.com/app-store/review/guidelines/)) ✅:

| Regla | Texto clave | Qué significa para nosotros |
|---|---|---|
| **3.1.1** | "Apps may not use their own mechanisms to unlock content or functionality, such as license keys, augmented reality markers, **QR codes**…" | ❌ La app de iOS **no debe tener una casilla para teclear nuestros propios códigos** ni un lector de QR que desbloquee trajes |
| **3.1.1** | "Digital gift cards, certificates, vouchers… redeemed for digital goods… can only be sold **in your app** using in-app purchase" | Vender códigos **dentro de la app** solo se puede con compra dentro de la app; vender códigos **fuera** de la app no está prohibido |
| **3.1.3(b)** | Acceso en la app a lo adquirido "on other platforms or your web site… **provided those items are also available as in-app purchases**" | ✅ El traje canjeado en la web aparece en la cuenta, **siempre que ese traje también se venda dentro de la app** |
| **3.1.3(e)** | "If your app enables people to purchase **physical goods**… you **must** use purchase methods other than in-app purchase" | ✅ Vender playeras desde la app está permitido, sin comisión de Apple |
| **3.1.3** | Comunicar fuera de la app otros métodos de compra está permitido | ✅ Etiquetas, redes y podcast pueden promover el canje |
| **Offer Codes** | Códigos oficiales de Apple, gratis o con descuento, para cualquier compra dentro de la app | ✅ Alternativa 100% dentro de las reglas (detalle abajo) |
| **NFT** | Se pueden mostrar, pero "NFT ownership does not unlock features or functionality" | ❌ No usar NFT para desbloquear trajes |

**Offer Codes de Apple** ✅ ([Apple](https://developer.apple.com/help/app-store-connect/manage-in-app-purchases/create-offer-codes-for-in-app-purchases)):
- **Hasta 1 millón de códigos por app cada trimestre** y hasta 10 ofertas activas a la vez.
- Códigos de un solo uso en lotes de **500 a 25,000**, o códigos personalizados (tipo "INKCAPTINTA") con un límite de canjes que tú defines.
- Se canjean con enlace, en el App Store o **dentro de la app con el sistema de Apple** (requiere iOS 16.3 o superior).
- **Límite importante:** los de un solo uso **caducan en máximo 6 meses**. Una playera puede pasar más tiempo en inventario. Los personalizados pueden no tener fecha de caducidad.
- El traje debe existir como compra dentro de la app **aprobada** y la app debe estar publicada.
- **Solo funcionan en iOS.** Para Android se necesita el equivalente de Google: *promo codes* de Play Console, con límites por trimestre que no pude verificar ⚠️.

**Google Play** ⚠️: los bienes físicos se cobran fuera de Google Play Billing (obligatorio). Para canjear códigos propios dentro de la app, la práctica común en Android (Pokémon GO lo hace) sugiere que está permitido, pero **hay que confirmarlo en la política de pagos de Google**.

### 7.2 Ejemplos reales

| Caso | Mecánica | Lección | Confianza |
|---|---|---|---|
| **Roblox – juguetes con código** | Cada figura física trae un código de un artículo virtual **exclusivo**, que se canjea en la web de Roblox | Canjear **en la web** evita el problema de Apple | ⚠️ |
| **Fortnite – Minty Legends Pack (2019)** y paquetes físicos en tiendas | Caja física con V-Bucks + skin exclusiva; código canjeado en la web de Epic | Mismo patrón: web y cuenta | ⚠️ |
| **Fortnite – skins por comprar celulares** (Samsung, Honor) | El dispositivo físico desbloquea una skin | Una marca puede regalar la skin con su producto | ⚠️ |
| **Pokémon GO – códigos promocionales** | En Android se canjean dentro de la app; **en iOS se canjean en una página web** | **Es exactamente la regla 3.1.1 aplicada** | ⚠️ |
| **Pokémon TCG Live** | Las cartas físicas traen códigos digitales | La mercancía física impulsa el juego digital | ⚠️ |
| **Skylanders (iPad, 2014)** | Figuras físicas con chip desbloquean personajes (*toys-to-life*) | Precedente de juguete + juego (hoy sería zona gris) | ⚠️ |
| **Call of Duty / Halo × Mountain Dew y Doritos** | Códigos en el empaque dan doble XP y artículos | Botanas y bebidas como canal; nosotros, playeras | ⚠️ |
| **Nike (Nikeland) y Vans (Vans World) en Roblox** | Mundos de marca con ropa virtual | Las marcas de ropa sí invierten en esto | ⚠️ |
| **Forever 21 en Roblox** | Gorro virtual vendido 1.5 M veces y **después** fabricado | **De lo digital a lo físico:** el traje más popular se vuelve playera 📄 | 📄 |
| **Ralph Lauren × Fortnite** | Ítems en Fortnite + venta de botas físicas (~200 pares de US$250) | Funciona, pero el volumen físico es modesto 📄 | 📄 |

### 7.3 Diseño recomendado: "De la etiqueta al traje"

1. **Etiqueta o hang tag** con QR y **código único** (p. ej. `INK-7F3K-92QX`) bajo una franja para raspar, para que no lo copien en la tienda. Generado por **nuestro backend**: cada código es de un solo uso y va ligado al traje, al lote y al artista (para calcular regalías).
2. **El QR abre una página web de INKCAP** (`…/canje`), **no la app**. El jugador inicia sesión con su cuenta del juego (Sign in with Apple, Google o su ID de jugador más un PIN que muestra el juego). El traje se acredita a su cuenta.
3. **El traje aparece en el juego** la próxima vez que abre la app, gracias a la sincronización de cuenta (3.1.3(b)).
4. **El mismo traje también se vende dentro de la app** (por ejemplo, la versión digital a MXN 79). La versión **"Edición Física"**, con un detalle extra como el bordado brillante o una estela de tinta, puede ser exclusiva de la playera. **Hay que validar esta exclusividad con App Review antes del primer drop.**
5. **En Android** se puede agregar además una casilla "Canjear código" dentro de la app, si Google lo confirma ⚠️. **En iOS, no**, salvo con Offer Codes de Apple.
6. **Antifraude:** límite de intentos, invalidar el código si la playera se devuelve, y en compras en línea enviar el código por correo **después** del pago (OXXO confirma el pago en horas).
7. **Para campañas cortas** (eventos, lanzamientos): usar **Offer Codes de Apple** y *promo codes* de Google. El QR lleva a una web que detecta el sistema operativo y entrega el código correcto.

### 7.4 Al revés: vender merch desde el juego

- **Permitido en ambas tiendas y sin comisión:** un botón "Tienda INKCAP" que abre el e-commerce para comprar **playeras, gorras, tazas, llaveros y art toys físicos** (Apple 3.1.3(e) ✅; Google ⚠️).
- **Cuidado (zona gris):** fuera de EE.UU., el enlace dentro de la app debe llevar a una página **solo de productos físicos**. Si esa página también vende **moneda o códigos digitales**, Apple podría considerarlo "dirigir a pagos externos" ✅ (3.1.1(a)). **No anunciar dentro de la app** que la playera "trae el traje": eso se comunica en la etiqueta, en redes y en la tienda.
- **En EE.UU.** se puede enlazar a todo ✅.
- **Ideas:** "Pruébatelo en el juego": cada traje digital tiene botón "Cómprala de verdad". "Drop sincronizado": el traje y la playera salen el mismo día, con cuenta regresiva en el juego. "Vitrina del museo": el escenario del museo exhibe los art toys reales de edición limitada, con enlace a su ficha.
- **Medición:** enlaces con UTM por traje para saber qué diseños venden playeras.

### 7.5 Regalías a artistas por trajes (modelo 15%)

**Propuesta:** **15% del ingreso neto** de cada traje del artista, calculado después de IVA y de la comisión de la tienda, para mantener el modelo que INKCAP ya usa en merch. Liquidación trimestral con reporte por SKU.

**Ejemplo con un traje o pase de MXN 99 en el App Store:**

| Concepto | MXN |
|---|---|
| Precio al público | 99.00 |
| − IVA 16% (al SAT) | 13.66 |
| = Base | 85.34 |
| − Comisión Apple 15% | 12.80 |
| = Neto | **72.54** |
| − Regalía artista 15% | **10.88** |
| = Para INKCAP (antes de ISR y costos) | **61.66** |

(Supone que la comisión se calcula sobre el precio sin IVA ⚠️. El IVA que Apple cobra por su comisión, MXN 2.05, es acreditable.)

**Cláusulas que no deben faltar en el contrato:**
- Licencia de uso **digital** del diseño (juego, tráileres, redes): territorio, duración y si es exclusiva.
- Cómo se calcula el "neto" y cómo se reparten los paquetes que traen varios artículos.
- Derecho del artista a aprobar el modelo 3D. Los derechos morales del autor en México no se pueden renunciar ⚠️.
- Qué pasa si el juego cierra o el traje se regala en eventos.
- **Combo con código de creador:** el artista recibe además un % (p. ej. 5%) de lo que gastan quienes usan su código. Así cada artista se vuelve promotor.

---

## 8. Modelo de monetización recomendado por etapa

| Etapa | Duración aprox. | Qué se lanza | Monetización | Meta para avanzar |
|---|---|---|---|---|
| **0. Prototipo local** | Meses 0–4 | Wi-Fi local + bots | Ninguna. Todo se desbloquea con XP | Que sea divertido en pruebas con la comunidad INKCAP |
| **1. Beta local / soft launch** | Meses 4–8 | Android primero (Latinoamérica hispana), local + cuentas en la nube | XP, **recompensa diaria y racha**, tienda con 6–10 trajes, **starter pack de MXN 19**, video recompensado opcional | D1 ≥ 40%, D7 ≥ 15% 📄 |
| **2. Lanzamiento México (iOS + Android)** | Meses 8–12 | Primer **pase de temporada** ("Grafiti"), **primer drop phygital** con un artista, inscripción al Small Business Program | Pase de MXN 79–99, trajes de artista, códigos en playeras, programa de creadores (INKCAP TV) | Conversión ≥ 1%, ARPDAU ≥ US$0.015 |
| **3. Online + live service** | Año 2 | Online con salas y matchmaking, temporadas de 6–8 semanas (las 6 técnicas de tinta) | **Club INKCAP** (MXN 59/mes), patrocinios en muros del escenario, torneos gratuitos con premios de marca, **solicitud al Indie Games Fund** | D30 ≥ 5%, más de 10 mil jugadores diarios |
| **4. Escala** | Año 2–3 | EE.UU. y resto de LATAM; posible Switch o Steam | **Web shop** (EE.UU. con enlaces), colaboraciones con marcas externas, edición premium en consola | Ingresos mensuales > US$20–50 mil |

---

## 9. 20 ideas de monetización ordenadas por facilidad e impacto

Facilidad: 5 = muy fácil. Impacto: 5 = muy alto.

| # | Idea | Facilidad | Impacto | Nota |
|---|---|---|---|---|
| 1 | **Pase de temporada** temático (1 técnica de tinta por temporada) | 4 | 5 | El motor principal de ingresos |
| 2 | **Trajes de artistas INKCAP** con regalía de 15% | 4 | 5 | Diferenciador; los artistas los promueven |
| 3 | **Playera física con código** del traje (canje web) | 3 | 5 | Une la tienda y el juego |
| 4 | **Starter pack** de MXN 19 | 5 | 4 | Convierte al primer pagador |
| 5 | **Video recompensado** (x2 XP, vuelta extra en la recompensa diaria) | 5 | 3 | Monetiza a quienes no pagan |
| 6 | **Recompensa diaria + racha** (con traje al día 7) | 5 | 4 (retención) | Hace volver al jugador, que después compra |
| 7 | **Códigos de creador** para tatuadores, grafiteros e INKCAP TV | 3 | 4 | Marketing pagado por resultados |
| 8 | **Tienda diaria rotativa** con trajes por tiempo limitado | 4 | 4 | Urgencia sin azar pagado |
| 9 | **Drops de "Edición Limitada" sincronizados** (digital + físico) | 3 | 4 | Encaja con la sección "Edición Limitada" del e-commerce |
| 10 | **Emotes, estelas de tinta y efectos de KO** baratos (MXN 19–29) | 4 | 3 | Compras impulsivas |
| 11 | **Botón "Tienda INKCAP"** para merch física | 5 | 3 | Sin comisión de las tiendas |
| 12 | **Regalar trajes a amigos** | 4 | 3 | Viral y permitido por Apple ✅ |
| 13 | **Club INKCAP** mensual (pase + traje mensual + cupón para la tienda física) | 3 | 4 | Etapa 3 |
| 14 | **Pago único "Sin anuncios + Pase dorado"** al estilo BombSquad | 4 | 2 | Lealtad |
| 15 | **Escenarios patrocinados** (muros para marcas de aerosol, tenis, bebidas) | 2 | 4 | Requiere audiencia |
| 16 | **Colaboraciones con marcas o IP** (al estilo Free Fire) | 2 | 5 | Etapa 3–4 |
| 17 | **Torneos en eventos de arte urbano** con premios de patrocinadores | 3 | 2 (marca) | Inscripción gratis para evitar permisos ⚠️ |
| 18 | **"El traje más votado se hace playera"** (digital → físico, modelo Forever 21) | 3 | 4 | Reduce el riesgo de inventario |
| 19 | **Art toy físico con código** del muñequito | 2 | 4 | Para coleccionistas; edición numerada |
| 20 | **Web shop con OXXO y Mercado Pago** para paquetes de tinta | 2 | 2 (año 1) / 4 (año 3) | Cuando haya volumen o al lanzar en EE.UU. |
| 21 | **Fondos no reembolsables** (Indie Games Fund) y propuestas a Netflix Games | 3 | 4 (caja) | US$150–200 mil 📄 |
| 22 | **Versión premium en Switch o Steam** con DLC de artistas | 2 | 3 | El modelo de Boomerang Fu |

---

## 10. Proyección de ingresos: 12 meses desde el lanzamiento en tiendas

**Supuestos comunes:** comisión de tienda de 15% en las compras dentro de la app; 10–15% de las compras pasa por la web o el canje de INKCAP con ~6% de comisión; regalía de 15% sobre el 40% del ingreso que corresponde a trajes de artista; Apple US$99 + Google US$25. Los montos **no incluyen IVA**. El primer tercio del año es casi solo local, por eso los promedios son moderados.

| Supuesto | Conservador | Base | Optimista |
|---|---|---|---|
| Descargas en 12 meses | 60,000 | 250,000 | 1,000,000 |
| Jugadores diarios (DAU) promedio | 2,000 | 10,000 | 45,000 |
| Jugadores mensuales (MAU) promedio | 8,000 | 35,000 | 150,000 |
| % de MAU que paga cada mes | 1.0% | 1.5% | 2.5% |
| Pagadores al mes | 80 | 525 | 3,750 |
| ARPPU (gasto mensual por pagador) | US$5 | US$7 | US$9 |
| Anuncios recompensados por DAU al día | 1.0 | 1.3 | 1.5 |
| eCPM promedio | US$2.5 | US$3.5 | US$5 (más tráfico de EE.UU.) |
| Servidores online (meses 7–12) | US$300 | US$1,500 | US$6,000 |
| Playeras con código vendidas | 300 | 2,000 | 10,000 |

| Resultado (12 meses) | Conservador | Base | Optimista |
|---|---|---|---|
| Compras dentro de la app (bruto sin IVA) | US$4,800 (MXN 91.7 mil) | US$44,100 (MXN 842 mil) | US$405,000 (MXN 7.74 M) |
| Anuncios | US$1,825 (MXN 34.9 mil) | US$16,600 (MXN 317 mil) | US$123,200 (MXN 2.35 M) |
| **Total bruto** | **US$6,600 (MXN 127 mil)** | **US$60,700 (MXN 1.16 M)** | **US$528,200 (MXN 10.1 M)** |
| − Comisiones de tiendas y web | US$680 | US$6,220 | US$55,280 |
| − Regalías a artistas | US$250 | US$2,270 | US$20,980 |
| − Servidores, cuentas y herramientas | US$420 | US$1,770 | US$9,870 |
| **Neto antes de sueldos, marketing e ISR** | **≈ US$5,300 (MXN 101 mil)** | **≈ US$50,500 (MXN 964 mil)** | **≈ US$442,000 (MXN 8.44 M)** |
| ARPDAU resultante | US$0.009 | US$0.017 | US$0.032 |
| **Aparte: venta de playeras con código** (a MXN 450 supuesto) | MXN 135 mil | MXN 900 mil | MXN 4.5 M |

**Cómo leerlo:**
- El escenario **base** (ARPDAU de US$0.017) está dentro del rango típico de LATAM. El **optimista** requiere crecer fuera de México y una retención del 10% superior de la industria.
- En el conservador el juego **no paga un equipo**. Su valor estaría en la **marca y en las ventas de merch**, y por eso el phygital es tan importante.
- **Lo que más mueve el resultado:** la retención (que eleva el DAU) y el % de pagadores. Una mejora de 1% a 2% en conversión duplica el ingreso por compras.
- **Costos no incluidos:** sueldos, programador Unity de apoyo, arte 3D, marketing y adquisición de usuarios, contador e ISR de 30% sobre la utilidad.

---

## 11. Recomendación para nuestro juego

1. **Modelo:** **F2P + cosméticos + pase de temporada barato (MXN 79–99) + video recompensado opcional.** Nada de pagar para ganar ni cajas de botín de pago.
2. **Empresa y cuentas:** publicar como **persona moral** con RFC y Constancia de Situación Fiscal. Abrir Apple (US$99/año ≈ MXN 1,890) y Google (US$25 ≈ MXN 478) a nombre de la empresa 📄. **Inscribirse al Small Business Program el día uno** y revisar las cuentas asociadas.
3. **Phygital desde el primer drop:** código único en la etiqueta, **canje en la web de INKCAP**, traje acreditado a la cuenta y el mismo traje (o su versión base) **también a la venta dentro de la app**. En iOS no habrá casilla de códigos propios; para campañas se usan Offer Codes de Apple.
4. **Tienda INKCAP dentro del juego** solo con productos **físicos**. El juego es escaparate y la tienda es caja registradora.
5. **Regalías de 15% del neto** para el artista en sus trajes, más un **código de creador** con su nombre.
6. **México primero y Android primero:** Google ya acepta OXXO en efectivo 📄. Después iOS. EE.UU. y la web shop hasta la etapa 4.
7. **No gastar en anuncios** mientras el LTV de LATAM (~US$0.10–0.50) no supere el CPI (~US$0.15–0.60). Crecer con comunidad, artistas, INKCAP TV y creadores.
8. **Pedir dinero no reembolsable:** Indie Games Fund LATAM en cuanto el juego esté publicado; explorar Netflix Games como *party game* de TV.
9. **Antes de lanzar,** validar con el contador la lista de §4.7 y consultar a App Review el flujo phygital.

---

## 12. Dudas abiertas (qué falta confirmar)

- **Google Play:** comisión vigente, programas de pago alterno, trato fiscal a desarrolladores mexicanos (IVA e ISR), límites de *promo codes* y si permite canjear códigos propios dentro de la app. *No pude abrir support.google.com.*
- **EE.UU.:** si Apple ya cobra alguna comisión sobre compras enlazadas tras la apelación de Epic v. Apple, y si se aprobó el acuerdo Epic–Google.
- **Exclusividad phygital en iOS:** si un traje que **solo** se obtiene con la playera, sin versión en la app, cumple la 3.1.3(b). Consultar a App Review.
- **Tasas exactas de ISR e IVA** que Apple retiene a personas físicas en México (Apple no las publica).
- **Artículo del tratado** para el W-8BEN-E (beneficios empresariales 0% o regalías 10%).
- **Puntos de precio exactos en MXN** del App Store.
- **IEPS de 8% a videojuegos** de 2026: vigencia y alcance.
- **eCPM reales en México** para nuestro tipo de juego: solo se sabrán con tráfico real.

---

## Fuentes

**Verificadas hoy (23-nov-2026) en sitios oficiales de Apple**
- App Store Small Business Program: https://developer.apple.com/app-store/small-business-program/
- App Review Guidelines (3.1.1, 3.1.1(a), 3.1.2, 3.1.3(a), 3.1.3(b), 3.1.3(e), 3.2.2(x), 5.2): https://developer.apple.com/app-store/review/guidelines/
- Apple Developer News (2025–2026: guías para EE.UU. del 1-may-2025, Offer Codes del 29-oct-2025, Mini Apps del 13-nov-2025, Japón del 17-dic-2025, China del 12-mar-2026, Brasil del 18-jun-2026, UE del 18-ago-2026, Taiwán del 29-oct-2026, impuestos del 17-nov-2026): https://developer.apple.com/news/
- Offer Codes para compras dentro de la app: https://developer.apple.com/help/app-store-connect/manage-in-app-purchases/create-offer-codes-for-in-app-purchases
- Provide tax information (sección México, formularios de EE.UU.): https://developer.apple.com/help/app-store-connect/manage-tax-information/provide-tax-information
- Manage invoices and other tax documents (retenciones Brasil/México): https://developer.apple.com/help/app-store-connect/manage-tax-information/manage-invoices-and-other-tax-documents
- Understanding taxes: https://developer.apple.com/help/app-store-connect/making-payments-to-apple/understanding-taxes
- Overview of receiving payments: https://developer.apple.com/help/app-store-connect/getting-paid/overview-of-receiving-payments
- Set a price for an In-App Purchase: https://developer.apple.com/help/app-store-connect/manage-in-app-purchases/set-a-price-for-an-in-app-purchase
- In-App Purchase and subscriptions pricing: https://developer.apple.com/help/app-store-connect/reference/pricing-and-availability/in-app-purchase-and-subscriptions-pricing-and-availability
- Distribución en Japón: https://developer.apple.com/support/app-distribution-in-japan/
- Distribución en Brasil: https://developer.apple.com/support/app-distribution-in-brazil/
- DMA y apps en la UE: https://developer.apple.com/support/dma-and-apps-in-the-eu/
- StoreKit External Purchase (Países Bajos): https://developer.apple.com/support/storekit-external-entitlement/
- Apple Arcade: https://developer.apple.com/apple-arcade/

**Tomadas de los reportes 01–04 de esta investigación (fuente original)**
- Tipo de cambio: https://dolar.info/mexico/hoy/precio/banxico-fix.php
- Google Play efectivo en OXXO: https://support.google.com/googleplay/answer/9462077?hl=es-419 · Tarjetas de regalo: https://support.google.com/googleplay/answer/3422734?hl=es
- Supercell Creator Boost: https://supercell.com/en/games/brawlstars/blog/community/creator-boost-faq/
- Brawl Stars (pase y fin de cajas de botín): https://www.deconstructoroffun.com/blog/2024/2/11/brawl-stars-to-the-moon · https://gamingonphone.com/news/brawl-stars-revenue-jumped-to-almost-9x-and-mau-to-2-4x-in-2024-compared-to-last-year/
- Stumble Guys: https://www.pocketgamer.biz/battle-royale-game-stumble-guys-is-making-almost-half-a-million-dollars-every-day/ · https://gameanalytics.com/?p=17893 · https://mobilegamer.biz/how-stumble-guys-beat-fall-guys-at-its-own-game/
- Among Us: https://www.blog.udonis.co/mobile-marketing/mobile-games/among-us-player-count
- BombSquad: https://ballistica.net/whataretokens
- Boomerang Fu (precio y DLC): https://www.gematsu.com/2020/07/frantic-boomerang-brawler-boomerang-fu-for-xbox-one-switch-and-pc-launches-august-13 · https://store.steampowered.com/app/4629160/Boomerang_Fu_Street_Food_DLC/
- Free Fire en México: https://sensortower.com/blog/2025-q4-android-top-5-third-person-games-units-mx-60214330241bc16eb89ce15d
- eCPM: https://blog.playio.co/mobile-game-ecpm-benchmarks-2026 · https://appodeal.com/the-mobile-ecpm-report-updated-q4-2024-view
- CPI LATAM: https://gamegrowthadvisor.com/blog/2026-05-26-latam-mobile-game-market-entry-guide-2026/
- Retención: https://gamedevreports.substack.com/p/gameanalytics-mobile-and-pc-game
- Phygital (Ralph Lauren, Forever 21): https://www.modernretail.co/technology/how-ralph-laurens-phygital-boot-sale-in-fortnite-shows-benefits-and-limits-of-virtual-commerce/ · https://www.glossy.co/fashion/digital-fashion-brand-play-pop-go-launches-first-physical-collection
- Indie Games Fund LATAM: https://play.google.com/intl/id_ALL/console/about/programs/indiegames/ · https://blog.google/products-and-platforms/platforms/google-play/indie-games-fund-latin-america-2025/
- Redes de anuncios: https://www.undrads.com/blogs/ironsource-vs-applovin-max-vs-admob
- Unity (precios y fin del Runtime Fee): https://unity.com/products/pricing-updates · https://unity.com/blog/unity-is-canceling-the-runtime-fee
- RevenueCat: https://revenuecat.com/pricing
- Photon (costos de servidores): https://www.photonengine.com/fusion/pricing · https://doc.photonengine.com/photon/current/pricing
- Pagos en México (OXXO Pay, métodos de pago): https://www.rebill.com/en/blog/oxxo-pay-cash-is-king-in-the-mexican-market · https://www.antom.com/guides/markets/mexico
- Mercado de videojuegos en México y LATAM: https://www.diariodetabasco.mx/negocios/2026/08/27/ingresos-de-la-industria-de-videojuegos-superaran-los-43-mil-500-mdp-en-mexico-the-ciu/ · https://gamesbeat.com/latin-america-defies-game-stagnation-with-372-3m-players-and-8-3b-in-2025-revenues/

**Marcado como ⚠️ conocimiento previo (sin URL verificada hoy):** comisiones y programas de Google Play, Epic v. Apple (apelación) y Epic v. Google (acuerdo), Corea, tarifas de Xsolla, Stripe, Mercado Pago, Stash, Appcharge y Coda, Ley del IVA (Capítulo III Bis), artículo 113-A de la Ley del ISR, tratado México–EE.UU., IEPS a videojuegos 2026, eCPM de EE.UU., benchmarks de ARPDAU, conversión, ARPPU y LTV, precios de pases y skins en otros juegos, y los ejemplos phygital de Roblox, Fortnite, Pokémon, Skylanders, Call of Duty, Nike y Vans.
