# Plan de negocio y costos · GORRAZO by INKCAP

> **Proyecto:** party brawler para celular (iOS y Android). Nombre de desarrollo: *Boomerang Fighters*. Nombre propuesto para tiendas: **"GORRAZO by INKCAP"**, con el subtítulo "Lanza tu gorra, mancha a todos" (30 caracteres, el máximo de la App Store; todavía no se ha buscado en el IMPI ni en las tiendas). El diseño completo está en `docs/GDD.md`.
> **Fecha:** 23 de noviembre de 2026.
> **Tipo de cambio:** **1 USD = 19.1 MXN**, el mismo de toda la investigación (FIX de Banxico del 20-nov-2026 ≈ 19.02–19.03; mercado del 21 y 22-nov ≈ 19.16). Las cifras están redondeadas.
> **Fuentes:** reportes `docs/investigacion/00` a `08` (con prioridad a sus secciones "Verificación de datos" y a la síntesis 00) y `docs/diseno/`. **[Verificado]** = confirmado en fuente oficial el 23-nov-2026; **[No verificado]** = confirmar antes de pagar o firmar; **[Estimación]** = cálculo propio. No es asesoría fiscal ni legal.

---

## 1. Resumen ejecutivo

**Qué se construye.** Un juego de peleas para 2 a 6 jugadores en el que cada muñequito estilo *art toy* lanza **su propia gorra**: la gorra sale, regresa sola en curva y noquea de un golpe. Si tu gorra no está en tu cabeza, estás desarmado. Las arenas son una CDMX de arte (Callejón Grafiti, Museo de Arte y Azotea CDMX), cada KO deja una mancha de tinta y la partida termina convertida en un mural. **La ropa del muñequito es merch real de INKCAP**: playeras y gorras de los artistas. Se juega primero en **local por Wi-Fi**, después en **salas en línea con código** y, al final, en modo competitivo (*ranked*).

**Ya existe** un prototipo web jugable y un **núcleo en Unity 6** (21 pruebas automáticas, bots, red local sin internet y anfitrión sin gráficos que servirá de servidor). Eso cubre casi toda la etapa de prototipo. **Motor: Unity 6 LTS** (rama 6.3 hoy; migrar a 6.7 LTS antes del *soft launch*, porque 6.3 tiene soporte hasta dic-2027), que el equipo ya usa en Quest 3.

**Cuánto cuesta cada etapa** (escenario recomendado: equipo actual + IA + modelado 3D de la empresa hermana + *freelancers* puntuales):

| Etapa | Fechas | Qué sale | Rango | Plan de trabajo | Puerta para seguir |
|---|---|---|---|---|---|
| **0. Prueba de diversión** | dic-2026 → feb-2027 | Núcleo ajustado, prueba de *lag*, 30 pruebas presenciales, nombre y empresa | US$7–14 mil (MXN 135–265 mil) | US$12.4 mil (MXN 238 mil) | 7 de cada 10 piden "otra"; KO justos con 100 ms de *lag* |
| **1. "Edición Fiesta"** (beta local cerrada en México) | mar → jul-2027 | 3 arenas, 8 muñequitos (los 6 del prototipo + Lienzo y Axo, sobre la misma plataforma), 3 modos + Reto del Día; Wi-Fi, *hotspot*, QR y bots | US$11–31 mil (MXN 210–590 mil) | US$21.0 mil (MXN 401 mil) | 500 testers; 3 partidas o más por sesión |
| **2. Cuentas, tienda y online casual** | ago → nov-2027 | Cuentas en la nube, tienda, pase, portal de canje, salas por código y partida rápida con bots | US$22–56 mil (MXN 0.42–1.07 M) | US$39.4 mil (MXN 753 mil) | Sin fallos graves con *lag* real |
| **3. *Soft launch* con métricas** | dic-2027 → feb-2028 | Beta abierta en México + prueba pagada chica en Colombia | US$6–18 mil (MXN 115–345 mil) | US$12.5 mil (MXN 239 mil) | D1 ≥ 40 %, D7 ≥ 12 %, D30 ≥ 4 % |
| **4. Lanzamiento en México** (iOS + Android) | mar-2028 | Temporada 1 "Grafiti", primer *drop* phygital, creadores | US$5–10 mil de marketing + operación | US$7.5 mil + US$3.8 mil al mes | Conversión ≥ 1 %; ARPDAU ≥ US$0.015 |
| **Total hasta el lanzamiento** | 16 meses | | **US$51–129 mil (MXN 0.97–2.46 M)** | **≈ US$93 mil (MXN 1.77 M)** | |

*No incluye los sueldos del equipo interno ni las suscripciones de IA que ya pagan.*

**Cómo gana dinero.** El juego es gratis y **todo lo que se vende es cosmético**: paquete de inicio de $19, gorras y playeras de $29–79 (las de artista, de $59 a $79), *drops* de $129–149, pase de temporada de $89 y video con recompensa opcional. **No hay cajas botín de pago ni ventajas pagadas.** Apple y Google cobran 15 % de comisión y el artista recibe 15 % del neto. De un traje de $79 a INKCAP le quedan ≈ $49 antes de ISR. Además, la playera física trae un código que se canjea **en la web de INKCAP**, y el botón "Consíguela real" vende merch sin comisión de las tiendas.

**Expectativa honesta a 24 meses** (dic-2026 a nov-2028; detalle en §7):
- **Base:** ≈ 251 mil descargas y ≈ 6,900 jugadores diarios al cierre. Ingresos brutos de **MXN 456 mil**. Resultado acumulado de **−MXN 1.97 M**. Al cierre la operación casi se paga sola (déficit de ≈ MXN 13 mil al mes).
- **Optimista:** ≈ 37,600 jugadores diarios. La inversión se recupera en **sep-2028** y el acumulado llega a **+MXN 0.88 M**.
- **Conservador:** el juego no despega. Pero con las puertas, la pérdida se corta en MXN 0.24 M (si falla la prueba de diversión), en MXN 0.64 M (si falla la Edición Fiesta) o en MXN 1.63 M (si falla el *soft launch*).

En el año 1, el valor está en la **marca, la comunidad y la merch**.

**Qué decidir esta semana:**
1. **Qué empresa publica** (recomendado: S.A.P.I. nueva). Define si aplica Unity Pro (grupo con más de US$200 mil al año) y el límite de Apple. Tramitar el **D-U-N-S hoy** (tarda hasta 28 días).
2. **Buscar y registrar "GORRAZO by INKCAP"** (INKAZO de respaldo) ante el IMPI.
3. **Aprobar la Etapa 0** (≈ MXN 240 mil) y las **puertas** de este plan.
4. **Equipo (b)** y contratar a un revisor senior de Unity (5–10 h por semana).
5. **Lo local sale como beta cerrada, no como lanzamiento público**, para evitar la reseña "no tiene online" que persigue a *Boomerang Fu*.

---

## 2. Costos fijos para existir

| Concepto | Costo | Estado |
|---|---|---|
| **Apple Developer Program** (organización) | US$99 al año (≈ MXN 1,890) | [Verificado] |
| **Google Play** | US$25 una vez (≈ MXN 478) | [Verificado, indirecto] |
| **D-U-N-S** (lo piden ambas tiendas) | Gratis; tarda hasta 28 días | [Verificado] |
| **Unity Personal** | Gratis si la empresa y su grupo tienen menos de US$200 mil de ingresos + fondos en 12 meses | [No verificado] |
| **Unity Pro** (si se rebasa) | US$2,310 por puesto al año (≈ MXN 44 mil; 3 puestos ≈ MXN 132 mil) | [No verificado] |
| Unity AI en el Editor (opcional) | ≈ US$10 al mes | [No verificado] |
| Plugin de Unity para Claude Code, Blender, URP, Netcode, ProBuilder | Gratis | [No verificado el plugin] |
| **IA de código** (ya la pagan) | US$20–200 por persona al mes según el plan | [Estimación] |
| Paquetes Feel y DOTween Pro | ≈ US$50–100 | [Estimación] |
| **Builds de iOS en la nube** (Codemagic, 500 min gratis) | US$0–10 al mes | [No verificado] |
| **Mac mini** (Etapa 1) | Desde ≈ MXN 19,500 | [No verificado] |
| **Celulares de prueba** (Moto G06, Galaxy A07, Redmi, gama media, iPhone 11 usado, router) | MXN 20–28 mil | [Estimación] |
| Dominios, correo con dominio (Apple lo exige) y sitio web (puede ser el de INKCAP) | ≈ MXN 2–5 mil al año + US$6–8 por usuario al mes | [Estimación] |
| **IMPI:** nombre en 9, 41 y 28 + INKCAP en 9 y 41 | ≈ MXN 16,500 de derechos + MXN 12.5–30 mil de honorarios | [No verificado] |
| Aviso de privacidad, términos y contrato tipo con artistas | MXN 40–110 mil | [Estimación] |
| S.A.P.I. nueva + acuerdo de socios | MXN 30–80 mil | [Estimación] |
| Contabilidad de la empresa nueva | MXN 3–8 mil al mes | [Estimación] |
| Música comisionada con cesión / efectos de sonido | US$150–600 por pista / US$0–60 | [Estimación] |

**Por qué Unity y no otro motor.** El equipo ya publica en Unity para Quest 3 (que por dentro es Android), Unity tiene el mejor soporte de IA para programar en 2026 y el núcleo ya está escrito en C# para Unity. Cambiar a Godot o Unreal tiraría meses de trabajo sin ahorrar dinero. **El único riesgo de licencia es el límite de US$200 mil**, y por eso importa tanto qué empresa publica.

**Costo mínimo para mantener vivo el juego** si se pausa ("modo hibernación"): cuentas, dominio, correo, contabilidad y servidores mínimos ≈ **MXN 4–10 mil al mes**. El modo local por Wi-Fi sigue funcionando sin servidores; es el "seguro de vida" que le faltó a *Knockout City*.

---

## 3. Costos de desarrollo por etapa y escenario de equipo

### 3.1 Tres formas de armar el equipo

- **(a) Equipo actual + IA.** El equipo programa con IA. Un revisor senior de Unity (5 h por semana) revisa la red y las compras, la empresa hermana hace el 3D y el audio y la interfaz salen de catálogos. Es lo más barato, pero lo más lento y riesgoso.
- **(b) Equipo actual + IA + *freelancers* de México y LATAM** (**recomendado**). Igual que (a), pero se contratan por encargo rigs y animación, interfaz, música y efectos y QA. El revisor senior trabaja 8 h por semana en el MVP y 15–20 h en el online.
- **(c) Estudio externo** "llave en mano", de México o LATAM. Más rápido y con menos carga para el equipo, pero 2 a 3 veces más caro, y el equipo no aprende a operar el juego.

### 3.2 Costo por etapa (USD, MXN entre paréntesis)

| Etapa | (a) Actual + IA | **(b) + *freelancers*** | (c) Estudio externo |
|---|---|---|---|
| 0. Prueba de diversión | 5–10 mil | **7–14 mil** (135–265 mil) | 7–14 mil (el prototipo ya existe) |
| 1. Edición Fiesta (MVP local) | 5–14 mil | **11–31 mil** (210–590 mil) | 60–150 mil |
| 2. Cuentas, tienda y online | 12–30 mil | **22–56 mil** (420 mil–1.07 M) | 45–110 mil |
| 3. *Soft launch* | 5–15 mil | **6–18 mil** (115–345 mil) | 8–20 mil |
| 4. Marketing del lanzamiento | 5–10 mil | **5–10 mil** (95–190 mil) | 5–10 mil |
| **Total hasta el lanzamiento** | **32–79 mil (MXN 0.61–1.51 M)** | **51–129 mil (MXN 0.97–2.46 M)** | **125–304 mil (MXN 2.4–5.8 M)** |
| Lanzamiento en México | jun–oct 2028 | **mar–may 2028** | dic-2027–feb-2028 |
| Dedicación interna | 2–3 personas al 50–100 % | 1–2 personas al 50 %, más el cliente como productor | El cliente como productor |
| Riesgo técnico | Alto (la red y las compras con IA tienen errores sutiles) | Medio | Bajo, pero con dependencia del estudio |

Fuentes y supuestos:
- **(b)** sale del reporte 03 §9.2 y de la evaluación del productor §5. El MVP cuesta US$15–40 mil con arte externo, o **US$8–25 mil con el 3D hecho en casa**. El online cuesta US$20–50 mil y el portal de canje US$2–5 mil. El legal mínimo es de MXN 60–110 mil y el recomendado de MXN 120–220 mil.
- **(a) y (c)** son **[Estimación]** propia. Para (c) se supone un estudio de 3 a 6 personas con tarifa promedio de US$25–45 por hora durante 9 a 11 meses. **Hay que pedir 3 cotizaciones.**

**Plan de trabajo usado en la proyección (escenario b):** Etapa 0 = US$12.4 mil (revisor, celulares, IMPI, contrato con artistas, S.A.P.I.); Etapa 1 = US$21.0 mil (revisor US$7.5 mil, rigs y animación, interfaz, audio, piezas 3D, QA, Mac y aviso de privacidad); Etapa 2 = US$39.4 mil (online US$35 mil + portal de canje); Etapa 3 = US$12.5 mil (QA, prueba pagada en Colombia, eventos, contenido de la Temporada 1).

### 3.3 Tarifas de referencia en México y LATAM [Estimación del reporte 03; validar con cotizaciones]

| Rol | Tarifa | Por pieza |
|---|---|---|
| Programador Unity senior (revisor de red y compras) | US$35–60 por hora | — |
| Artista 3D de personajes | US$20–40 por hora | US$250–700 por muñequito |
| Artista 3D de escenarios | US$20–35 por hora | US$800–2,500 por arena |
| Rigger o animador | US$15–35 por hora | Rig de US$100–300; US$30–80 por animación |
| Interfaz (UI/UX) | US$15–35 por hora | US$1,000–3,000 por la interfaz del MVP |
| Audio | — | Efectos US$300–1,200; música US$150–600 por pista |
| QA en dispositivos | US$6–12 por hora | — |

**Ahorro clave:** los personajes y las arenas son la partida externa más grande del MVP (≈ US$6–12 mil). Si los hace la empresa de modelado y escaneo 3D del cliente, el MVP baja de US$15–40 mil a **US$8–25 mil**. Además, ese mismo pipeline sirve después para fabricar los *art toys* físicos.

### 3.4 Recomendación

Usar **(b) con el 3D en casa**. Para el online, si en jul-2027 el equipo no tiene a alguien que domine la compensación de *lag*, contratar **solo esa parte** a un estudio, con precio cerrado (US$20–50 mil). No vale la pena contratar todo el juego por fuera. **Regla:** no se gasta en arte final ni en online hasta que 7 de cada 10 personas pidan "otra" con muñequitos *greybox* (grises y sin detalle).

---

## 4. Costos de jugar en línea y backend

### 4.1 Respuesta corta

- **Local por Wi-Fi: US$0.** Un celular es el anfitrión; el núcleo ya lo hace.
- **En línea sí cuesta, pero poco.** Un "relay" (cartero en la nube) se paga por uso: con 1,000 jugadores diarios ≈ **MXN 400 al mes**; con 10,000 ≈ **MXN 5–11 mil**; con 100,000 ≈ **MXN 59–122 mil**.
- **Lo caro es el modo competitivo** con servidores dedicados: 2 a 3 veces más.

Supuestos (reporte 04): 25 min en línea por jugador al día, 0.3 GB por jugador al mes.

### 4.2 Red (USD al mes) [No verificado: precios de proveedores; las cuentas internas sí se verificaron]

| Opción | 1,000 diarios | 10,000 diarios | 100,000 diarios | Funciona sin internet | Trampas |
|---|---|---|---|---|---|
| **Unity Relay** (anfitrión en un celular), **recomendado** | ~20 (MXN 400) | 260–470 (MXN 5–9 mil) | 2,800–4,900 (MXN 53.5–93.6 mil) | Sí, con el mismo código | El anfitrión podría hacer trampa |
| Photon Fusion o Quantum | 8–125 | 250–500 | 5,000–7,500 | No | Media o baja |
| Servidores dedicados en Edgegap (*ranked*) | 75–120 | 750–1,200 | 7,500–12,000 | Sí (mismo código, sin gráficos) | Bajas |
| Epic Online Services (P2P + relay) | 0 | 0 | 0 | Sí | El anfitrión podría hacer trampa; soporte móvil por confirmar |

### 4.3 Backend: cuentas, guardado, economía y compras (USD al mes)

| Opción | 1,000 diarios | 10,000 diarios | 100,000 diarios | Comentario |
|---|---|---|---|---|
| **Unity Gaming Services** (Auth, Cloud Save, Economy, Cloud Code en C#, Remote Config), **recomendado** | ≈ 0 | 0–100 | 300–1,500 | Mismo panel que Relay; correr el estimador oficial antes de firmar [No verificado] |
| Firebase completo (plan B) | 0–5 | 10–30 | 150–400 | Hay que programar la economía desde cero |
| PlayFab | Gratis en desarrollo | Por cotización | Por cotización | [No verificado] |
| Nakama propio + Hiro/Satori | 25–50 | 80–250 | 500–1,500 + una persona de DevOps | Para escala grande |
| Crashlytics y notificaciones (Firebase) | Gratis | Gratis | Gratis | Desde la primera instalación de prueba |

### 4.4 Costo total recomendado por etapa

| Etapa | Tecnología | 1,000 diarios | 10,000 diarios | 100,000 diarios |
|---|---|---|---|---|
| Edición Fiesta (local) | Red UDP propia del núcleo + Bonjour o QR; progreso guardado en el celular | **US$0** | — | — |
| Online casual (salas por código + partida rápida con bots) | Unity Transport + **Relay** + **UGS** + Firebase + Unity IAP | **≈ US$20 (MXN 400)** | **US$260–570 (MXN 5–10.9 mil)** | **US$3,100–6,400 (MXN 59–122 mil)** |
| *Ranked* (año 2 en adelante) | El mismo anfitrión sin gráficos en Edgegap o GameLift Querétaro + UGS + emparejamiento | US$100–320 | **US$770–1,500 (MXN 14.7–28.7 mil)** | **US$8,000–13,700 (MXN 153–262 mil)** |

**Por jugador:** con 10,000 diarios, el online casual cuesta ≈ **US$0.002 por jugador al día**, contra un ingreso de US$0.01–0.05 por jugador al día en LATAM. Con servidores dedicados sube a ≈ US$0.005: **se comería ~30 % del ingreso** del escenario base. Por eso el *ranked* solo llega cuando el ingreso lo justifique.

### 4.5 Cuándo cambiar de proveedor

1. **Antes de feb-2027:** si la prueba de *lag* a 100 ms falla, cambiarse a Photon Quantum. Después sería reescribir.
2. **Relay → servidores dedicados** (Edgegap): cuando Relay pase de ~US$1,000 al mes o haya *ranked*, premios o trampas.
3. **Edgegap → flota propia o GameLift Querétaro:** con más de 30–50 mil jugadores diarios estables en México [No verificado].
4. **UGS → Nakama o PlayFab:** si UGS pasa de US$1,000–1,500 al mes.
5. **Si Unity sube precios o cierra un servicio** (como Hathora y Multiplay en 2026): relay de EOS (US$0) o Edgegap. La red ya está detrás de interfaces intercambiables.

---

## 5. Modelo de monetización por etapa

### 5.1 Qué se vende en cada etapa

| Etapa | Modelo | Qué se vende | Para qué |
|---|---|---|---|
| 0. Prueba de diversión | Nada | — | Medir diversión |
| 1. Edición Fiesta (beta local) | **Gratis; todo se desbloquea con XP** | Solo merch física. La invitación a la beta va en un QR en las bolsas de envío | Medir diversión en grupo sin contaminar con ventas |
| 2–3. Online casual y *soft launch* | **Gratis con cosméticos** | Kit de Barrio de $19, gorras, playeras, pase de $89, Tinta Dorada (TD), canje web phygital, pago con OXXO en la web de INKCAP (promocionado solo fuera de la app) y **video con recompensa** (máximo 5 al día, nunca en partidas locales) | Medir conversión y ARPDAU |
| 4. Lanzamiento en México | + phygital completo | *Drop* sincronizado con "Estrenos", códigos de creador (5 %), Offer Codes de Apple y *promo codes* de Google para eventos, inscripción al Small Business Program | Escalar |
| 5. Año 2 | + recurrentes | **Club INKCAP** de $59 al mes, muros patrocinados en las arenas, torneos gratis con premios de marca, mediación de anuncios (con más de 5 mil jugadores diarios), solicitud al Indie Games Fund y *art toys* numerados con chip NFC | Ingreso recurrente |
| 6. Año 3 | + otras plataformas | Versión **premium en Switch o Steam** con DLC de artistas (modelo *Boomerang Fu*) y **tienda web completa con bonos** (estilo Supercell Store) si se pasa de US$20–50 mil al mes o se lanza en EE.UU. La página simple para pagar con OXXO existe desde la Etapa 2 | Diversificar |

### 5.2 Lo local: ¿gratis o de paga?

- **Premium (pago único) en celular: no.** En LATAM casi nadie paga por descargar, y en local **todos los del grupo** tendrían que pagar, lo que mata la fiesta.
- **"Sala del anfitrión"** (inspirado en Jackbox, donde solo el anfitrión compra [No verificado: conocimiento previo]): **sí, desde el lanzamiento, pero solo cosmético.** Si el anfitrión tiene el pase premium, los invitados de su sala local **se prueban los trajes de temporada** durante esa sesión ("Pruébatela" en grupo). Las arenas, los modos y las latas **nunca se venden ni se encierran en el pase**: son gratis para todos (GDD §8), para no dividir a la comunidad ni romper la regla de "100 % cosmético".
- **"Pase de Fundador" de pago único** (como el Gold Pass de BombSquad): opcional, para fans que quieran apoyar desde la beta. Durante la beta se vende **solo como playera física** con código; el contenido digital se acredita cuando existan las cuentas en la nube y la tienda (Etapa 2), y desde ese día también debe venderse como compra dentro de la app (regla 3.1.3(b)). La beta sigue siendo "todo por XP, sin dinero" dentro del juego.

### 5.3 Catálogo de precios (MXN con IVA)

| Producto | Precio | ≈ USD |
|---|---|---|
| **Kit de Barrio** (gorra "Logo INKCAP De Barrio" + playera "De Barrio" + 150 TD; una sola vez) | **$19** | 1.0 |
| *Sticker* para gorra | $10–19 | 0.5–1.0 |
| Llavero colgante, emote o patrón de estela (el **color** del rastro nunca se vende: siempre es el del jugador) | $19–29 | 1.0–1.5 |
| Gorra o playera común | $29–39 | 1.5–2.0 |
| **Gorra de artista INKCAP** (el cosmético que más se ve) | $59–69 | 3.1–3.6 |
| Efecto de KO de técnica | $49 | 2.6 |
| **Playera de artista** | $59–79 | 3.1–4.1 |
| ***Drop* completo** (playera + gorra + estela + emote) | $129–149 | 6.8–7.8 |
| **Pase de temporada** (A/B de $79 contra $99) | **$89** ($149 con 10 niveles extra) | 4.7 |
| Paquetes de Tinta Dorada, TD (moneda de pago, no caduca y siempre muestra su equivalente en pesos): 100 / 550 / 1,200 / 3,250 TD | $19 / $99 / $199 / $499 | 1 / 5.2 / 10.4 / 26.1 |
| Club INKCAP (año 2) | $59 al mes | 3.1 |

Puntos de precio exactos por confirmar en App Store Connect. **Regla firme:** ninguna compra cambia la velocidad, el golpe, el *dash* ni las latas, y todas las gorras tienen la misma silueta y el mismo tamaño de golpe.

### 5.4 Cuánto le queda a INKCAP de cada venta [Estimación; confirmar con el contador]

Supuestos: el precio incluye IVA; la comisión de 15 % se calcula sobre el precio sin IVA; la regalía de 15 % se calcula sobre el neto.

| Producto | Precio | Sin IVA | − Comisión | Neto | − Regalía 15 % | **Para INKCAP antes de ISR** |
|---|---|---|---|---|---|---|
| Playera de artista en App Store o Google Play | $79 | $68.10 | $10.22 (15 %) | $57.89 | $8.68 | **$49.21** |
| La misma playera en la web de INKCAP con OXXO | $79 | $68.10 | ≈ $6.16 (~4 % + $3) | $61.94 | $9.29 | **$52.65** |
| *Drop* en tienda | $149 | $128.45 | $19.27 | $109.18 | $16.38 | **$92.80** |
| Pase (con contenido del artista invitado) | $89 | $76.72 | $11.51 | $65.22 | $9.78 | **$55.44** |
| Kit de Barrio (sin artista) | $19 | $16.38 | $2.46 | $13.92 | — | **$13.92** |

**Comisiones:**
- **Apple:** 15 % con el **Small Business Program** si los *proceeds* son de hasta US$1 M al año, sumando las cuentas asociadas. Hay que inscribirse [Verificado].
- **Google:** 15 % sobre el primer US$1 M de cada año [No verificado].
- **En México no se permite** poner dentro de la app un botón para "comprar más barato en la web" [Verificado].

**Regalía:** si hoy INKCAP paga 15 % sobre el *precio* de la playera física, el 15 % del *neto* en digital le paga al artista 15–27 % menos por cada peso vendido. **Hay que acordarlo por escrito y explicárselo a los artistas**, junto con una regla para repartir el pase y los paquetes.

### 5.5 Phygital INKCAP: la playera física y el traje digital

1. La etiqueta trae un **código único para raspar** (MXN 1–3 por prenda), ligado al lote y al artista. El QR abre la **web de INKCAP**, el jugador entra con su cuenta del juego y el traje aparece en su inventario (Apple 3.1.3(b)), **siempre que ese traje también se venda en la app**. **En iOS no puede haber casilla para códigos propios dentro de la app** (3.1.1) [Verificado]: hay que quitar la del prototipo.
2. El portal de canje con antifraude cuesta **US$2–5 mil** y necesita las cuentas en la nube (Etapa 2).
3. Campañas cortas: Offer Codes de Apple (hasta 1 M por trimestre; caducan en 6 meses) y *promo codes* de Google (**solo 500 por trimestre**: no alcanzan para un tiraje) [Verificado].
4. **Al revés:** "Consíguela real" lleva a una página **solo de productos físicos**, sin comisión de Apple (3.1.3(e)) [Verificado], con UTM por traje.
5. **Año 2:** *art toys* numerados con la empresa 3D y chip NFC: la regla 3.1.4 permite que un juguete físico aprobado desbloquee contenido si también existe la compra en la app [Verificado; consultar a App Review].

### 5.6 Anuncios, patrocinios y creadores

- **Anuncios:** solo video con recompensa opcional (duplicar Gotas, recuperar la racha, probar un traje 24 h), máximo 5 al día, **nunca en partidas locales**, desde la Etapa 2. AdMob primero; mediación con más de 5 mil jugadores diarios. Con eCPM de US$2–3.4, **1,000 jugadores diarios dejan ≈ MXN 2,600 al mes**. **Sin anuncios para menores de 13** y sin anuncios personalizados para menores de 18; sin pedir el permiso de rastreo (ATT) si los anuncios no son personalizados.
- **Patrocinios:** muros de las arenas para marcas de aerosol, tenis o bebidas y torneos con premios de marca, solo **con audiencia demostrable** (más de 10 mil jugadores diarios).
- **Creadores:** código de creador de 5 % del neto para cada artista e invitado de INKCAP TV. Solo se paga si vende.
- **Torneos:** siempre con **inscripción gratis** (con inscripción pagada podría necesitarse permiso de Gobernación) [No verificado].

### 5.7 Impuestos en México y cómo llega el dinero

| Tema | Qué pasa |
|---|---|
| **Quién publica** | Una **persona moral** con RFC. A las personas físicas Apple les retiene ISR (1 % o quizá 2.5 % en 2026 [No verificado]) |
| **IVA 16 %** | Los precios ya lo incluyen. **A las empresas mexicanas Apple les entrega el IVA y ellas lo enteran al SAT**; el IVA de la comisión es acreditable [Verificado]. Google: por confirmar |
| **ISR** | 30 % sobre la utilidad; las pérdidas de los primeros años se amortizan [No verificado] |
| **CFDI** | CFDI global a "público en general", conciliado cada mes con los reportes de las tiendas |
| **EE.UU.** | **W-8BEN-E** citando el tratado (0 % o 10 % en vez de 30 %); no se puede editar después [Verificado el formulario] |
| **IEPS 8 % a juegos violentos** | Con tinta y sin sangre no debería aplicar [No verificado] |
| **Cuándo llega el dinero** | Apple: **45 días después del cierre del mes** (lo de marzo llega a mediados de mayo) [Verificado]. Google: ~día 15 del mes siguiente. Web (Stripe, Mercado Pago o el procesador de INKCAP): en días |
| **Artistas** | Liquidación trimestral por artículo, con las retenciones de su régimen |

---

## 6. Ideas de monetización de los grandes y de los novatos exitosos

| # | Quién | Qué hizo | ¿Nos sirve? ¿Cuándo? |
|---|---|---|---|
| 1 | **Supercell, *Brawl Stars*** (gigante) | Quitó las cajas botín y mejoró el pase; ingresos ~9 veces mayores en 2024 [fuente secundaria] | **Sí:** pase barato y cero cajas botín. *Soft launch* |
| 2 | **Supercell, códigos de creador** | El creador gana un % de lo que gastan sus seguidores | **Sí:** código de artista e INKCAP TV (5 %). Lanzamiento |
| 3 | **Supercell Store** | Tienda web con bonos extra | Desde la Etapa 2, solo una página simple para pagar con OXXO (promocionada fuera de la app); la tienda completa con bonos, con más de US$20–50 mil al mes o en EE.UU. Año 3 |
| 4 | **Epic, *Fortnite*** | Colaboraciones con Nike y Ralph Lauren; suscripción Crew | **Sí:** cada artista es una colaboración; Club INKCAP. Año 2 |
| 5 | **Roblox** | Juguetes físicos con código canjeado en la web; ropa hecha por la comunidad | **Sí:** *art toys* con NFC y muñequito "Lienzo" para artistas. Año 2 |
| 6 | **Forever 21 en Roblox** | Vendió un gorro virtual 1.5 M de veces y **después** lo fabricó | **Sí:** el traje más votado se fabrica. Temporada 1 |
| 7 | **Ralph Lauren × *Fortnite*** | ~200 pares de botas físicas de US$250 | **Lección:** lo físico mueve poco volumen; no inflar la proyección |
| 8 | **Garena, *Free Fire*** | Colaboraciones con picos de ~US$2 M por semana en México; pase muy barato | **Sí** a colaboraciones y precio bajo; **no** a ruletas de azar |
| 9 | **Kitka, *Stumble Guys*** (8–9 personas) | Regaló gemas a *influencers* en vez de pagar anuncios; LATAM fue su #1 en descargas | **Sí:** Tinta para que los creadores la regalen. Beta |
| 10 | **Innersloth, *Among Us*** (3 personas) | ~908 M de descargas y solo ~US$60 M en celular: no tenía qué vender | **Lección:** tienda y pase listos **antes** del pico. Etapa 2 |
| 11 | ***BombSquad*** (1 persona) | Gold Pass de pago único; el celular como control | **Sí:** Pase de Fundador y Modo Pantallota para eventos. Año 1–2 |
| 12 | ***Boomerang Fu*** (estudio pequeño) | Premium de US$14.99 + DLC de US$2.99–4.99; vendió más en su año 2 | **Sí, después:** Switch o Steam con DLC de artistas. Año 3 |
| 13 | **Jackbox Games** [No verificado] | Solo el anfitrión compra; los invitados juegan gratis | **Sí, solo cosmético:** en la sala de quien tiene el pase, los invitados se prueban los trajes de temporada. Lanzamiento |
| 14 | **Habby, *Archero*** | Video con recompensa para duplicar premios | **Sí**, con tope de 5 al día y nunca en local. Etapa 2 |
| 15 | ***Pokémon GO*** y ***Call of Duty* × Doritos** | Canje de códigos en la web (iOS); lugares patrocinados; códigos en empaques [No verificado] | **Sí:** muros patrocinados y códigos en empaques de marcas aliadas. Año 2 |
| 16 | **Supercell, *mo.co*** | Lanzamiento con creadores y acceso por invitación con QR | **Sí:** beta con QR en bolsas de envío e INKCAP TV. Etapa 1 |
| 17 | **Hyperkani, *Bomber Friends*** | Más de 100 M de descargas en una década con contenido chico y constante | **Sí:** "temporada mínima" (1 arena o regla + 10 cosméticos) |
| 18 | **Indie Games Fund LATAM / Netflix Games** | US$150–200 mil sin ceder acciones (lo ganó un estudio mexicano en 2025) / financian juegos de fiesta en TV | **Sí** al fondo al publicar (mar-2028); Netflix, exploratorio. Apple Arcade choca con la tienda |

---

## 7. Proyección de ingresos y gastos a 24 meses

### 7.1 Supuestos explícitos

Ventana: **dic-2026 a nov-2028**. Hay ingresos desde el *soft launch* (dic-2027) y el lanzamiento en México es en mar-2028. **Montos en USD sin IVA, antes de ISR y sin sueldos internos.**

| Supuesto | Conservador | Base | Optimista | Fuente |
|---|---|---|---|---|
| Descargas en los 12 meses con tiendas | 58,500 | 251,000 | 940,000 | Inv. 06 §10 (60 mil / 250 mil / 1 M) |
| ↳ en el mes del lanzamiento / cada mes después | 12 mil / 5 mil | 50 mil / 22 mil | 180 mil / 85 mil | [Estimación] |
| Retención D1 / D7 / D30 | 30 / 8 / 2.5 % | **40 / 12 / 4 %** | 45 / 15 / 6 % | Inv. 02 y 05 (mediana 22 / 4 / 0.7; el 10 % mejor 40 / 12) |
| Jugadores diarios ÷ mensuales | 0.25 | 0.28 | 0.30 | Inv. 06 |
| % de jugadores mensuales que paga | 1.0 % | 1.5 % | 2.5 % | Inv. 06 |
| Gasto mensual por pagador (ARPPU) | US$5 | US$7 | US$9 | Inv. 06 (LATAM US$4–10) |
| Anuncios vistos por jugador al día | 0.8 | 1.0 | 1.2 | Inv. 06, ajustado (tope de 5, nunca en local) |
| Pago por cada mil anuncios (eCPM) | US$2.5 | US$3.0 | US$3.5 | Inv. 06 (Android LATAM US$2–3.4) |
| Comisión | 15 % en tiendas; 12.5 % de las ventas por la web a ~6 % | | | Inv. 06 |
| Regalías | 15 % del neto sobre el 40 % de las ventas (artículos de artista) + 5 % de código de creador sobre el 10 % | | | Inv. 06 y 07 |
| Servidores + backend | US$0.045 por jugador diario al mes | | | Inv. 04 y 05 |
| Playeras con código vendidas | 40 al mes | 200 al mes | 800 al mes | Inv. 06 (300 / 2,000 / 10,000 al año) |
| ↳ lo que se le atribuye al juego | 50 % incremental × 35 % de margen × MXN 388 sin IVA ≈ **US$3.55 por playera** | | | [Estimación; reemplazar con los datos reales de INKCAP] |
| Operación mensual desde mar-2028 (CM, contenido, mantenimiento y marketing) | US$2.9 mil | US$3.8 mil (MXN 73 mil) | US$4.85 mil + 10 % del ingreso | [Estimación] |
| Inversión hasta feb-2028 | US$85.4 mil (MXN 1.63 M) en los tres escenarios | | | Plan de trabajo §3 |

### 7.2 Resultados a 24 meses

| Resultado (dic-2026 → nov-2028) | Conservador | **Base** | Optimista |
|---|---|---|---|
| Jugadores diarios promedio (mar–nov 2028) | 900 | **5,600** | 29,000 |
| Jugadores diarios al cierre (nov-2028) | 1,000 | **6,900** | 37,600 |
| Ingreso por jugador diario (ARPDAU) | US$0.009 | **US$0.016** | US$0.029 |
| Ingresos brutos (compras + anuncios) | US$2.2 mil (MXN 42 mil) | **US$23.9 mil (MXN 456 mil)** | US$233 mil (MXN 4.46 M) |
| ↳ de ellos, anuncios | US$0.5 mil | US$4.6 mil | US$33.6 mil |
| − Comisiones de tiendas y web | US$0.2 mil | US$2.7 mil | US$27.7 mil |
| − Regalías y códigos de creador | US$0.1 mil | US$1.1 mil | US$11.2 mil |
| − Servidores y backend | US$0.4 mil | US$2.3 mil | US$12.0 mil |
| − Inversión y operación | US$117.1 mil | US$127.2 mil | US$162.0 mil |
| + Margen incremental de playeras con código | US$1.3 mil | US$6.4 mil | US$25.6 mil |
| **Resultado acumulado** | **−US$114.3 mil (−MXN 2.18 M)** | **−US$102.9 mil (−MXN 1.97 M)** | **+US$46.0 mil (+MXN 0.88 M)** |
| Ventas brutas de playeras con código (ingreso de INKCAP, no del juego) | MXN 162 mil | MXN 810 mil | MXN 3.24 M |
| ¿Cuándo se paga sola la operación? | No llega | Casi: −US$0.7 mil al mes en nov-2028 | Desde mar-2028 |
| ¿Cuándo se recupera la inversión? | No | Después de 2030 sin fondo; **antes con el Indie Games Fund** | **sep-2028** |

**Base por trimestre** (USD):

| Trimestre | Descargas | Jugadores diarios promedio | Compras | Anuncios | Gastos totales | Resultado | Acumulado |
|---|---|---|---|---|---|---|---|
| dic-26 – feb-27 | — | — | — | — | 12,444 | −12,444 | −12,444 |
| mar – may 2027 | — | — | — | — | 12,758 | −12,758 | −25,202 |
| jun – ago 2027 | — | — | — | — | 17,021 | −17,021 | −42,223 |
| sep – nov 2027 | — | — | — | — | 30,630 | −30,630 | −72,853 |
| dic-27 – feb-28 (*soft launch*) | 9,000 | 410 | 465 | 111 | 12,675 | −12,099 | −84,952 |
| mar – may 2028 (lanzamiento) | 102,000 | 5,220 | 5,873 | 1,409 | 20,778 | −11,364 | −96,316 |
| jun – ago 2028 | 66,000 | 5,310 | 5,975 | 1,435 | 13,311 | −3,769 | −100,084 |
| sep – nov 2028 | 74,000 | 6,180 | 6,955 | 1,670 | 13,619 | −2,861 | −102,946 |

*El "resultado" incluye el margen incremental de las playeras con código.*

### 7.3 Punto de equilibrio y qué lo mueve

- **Equilibrio de operación** (sin contar la inversión): con la economía del escenario base, cada jugador diario deja ≈ US$0.35 al mes después de comisiones, regalías y servidores. Para cubrir US$3.8 mil de operación al mes hacen falta **≈ 11,000 jugadores diarios**, o **≈ 9,000** si se suma el margen de las playeras. Con la economía optimista bastan ≈ 3,500–8,000.
- **Recuperar la inversión** (≈ US$93 mil hasta el lanzamiento) con la economía base exige **≈ 20 mil jugadores diarios durante unos 2 años** (deja ≈ US$3.9 mil al mes). Con la monetización optimista basta con menos de un año.

**Sensibilidad del escenario base (acumulado a nov-2028 y resultado del último mes):**

| Cambio | Jugadores diarios al cierre | Acumulado | Último mes |
|---|---|---|---|
| Base tal cual | 6,900 | −US$102.9 mil | −US$0.7 mil |
| Retención del 10 % mejor (45 / 15 / 6 %) | 10,000 | −US$96.0 mil | **+US$0.4 mil** |
| Monetización optimista (2.5 % paga, US$9) | 6,900 | −US$85.2 mil | **+US$1.7 mil** |
| El doble de descargas | 13,800 | −US$85.1 mil | **+US$1.7 mil** |
| Sin anuncios | 6,900 | −US$107.6 mil | −US$1.3 mil |

**¿Por qué sale menos que el reporte 06** (US$50.5 mil netos en su base)? Solo caben 9 meses después del lanzamiento, la retención es más estricta (5,600 jugadores diarios en vez de 10,000) y no hay anuncios en partidas locales.

**Sueldos internos:** si el equipo interno costara, por ejemplo, MXN 60 mil al mes, habría que sumar **MXN 1.44 M** en 24 meses [Estimación].

**Dinero que no se devuelve:** el **Google Play Indie Games Fund LATAM** (US$150–200 mil, solo con el juego ya publicado) cubriría toda la pérdida del escenario base. Hay que solicitarlo en mar-2028. **Ojo:** si el límite de Unity Personal suma "ingresos + fondos" de 12 meses, recibir ese fondo pasaría a la empresa de US$200 mil y obligaría a pagar Unity Pro (≈ US$2,310 por puesto al año; ≈ MXN 132 mil con 3 puestos) [No verificado: confirmar en los términos de Unity antes de solicitarlo]. Aun así conviene.

### 7.4 Lectura honesta

1. El juego **no paga un equipo en el año 1**, salvo en el optimista. Se justifica por la retención del 10 % mejor, el fondo no reembolsable o su valor como **medio de marca y canal de merch** (MXN 0.8 M en playeras con código en el base).
2. **Las puertas limitan la pérdida:** ≈ MXN 238 mil si falla la prueba de diversión, ≈ MXN 640 mil si falla la Edición Fiesta y ≈ MXN 1.63 M si falla el *soft launch*. En ese caso, el juego queda como modo de fiesta para eventos y tiendas.
3. **Lo que más mueve el resultado es la retención y el % de pagadores**, no los servidores.

---

## 8. Lanzamiento y marketing de bajo presupuesto

**Regla de oro:** no comprar instalaciones mientras un jugador de LATAM valga menos (US$0.10–0.50) de lo que cuesta conseguirlo (US$0.15–0.60). En México la mitad descarga juegos para jugar con amigos y **24 % descargó su último juego tras ver a un *influencer***: el crecimiento viene de la comunidad.

| Fase | Tácticas | Presupuesto |
|---|---|---|
| **Ahora a feb-2027** | Pruebas presenciales en posadas y en la tienda; apartar *handles*; lista de espera con QR en bolsas de envío y etiquetas | MXN 5–10 mil |
| **Edición Fiesta** (mar–jul 2027) | Beta por invitación; Discord y **canal de WhatsApp**; torneos de "La Reta" en bazares y eventos de arte urbano (gratis); artistas piloto jugando en vivo; "La Reta con…" en INKCAP TV | US$0.5–1.5 mil |
| ***Soft launch*** (dic-2027 – feb-2028) | Beta abierta en México; **prueba pagada chica en Colombia** (US$1.5–6 mil por 10 mil instalaciones) para comparar con la comunidad; **clip vertical automático de 6 s** y **postal del mural** con QR; "Gorrazo de la semana" con playera física (inscripción gratis, ganador por jurado y nunca al azar, con bases publicadas); enlace de sala por WhatsApp; Tinta para creadores | US$3–7.5 mil |
| **Lanzamiento en México** (mar-2028) | Nominación al *featuring* de Apple (de 3 meses a 2 semanas antes) y Google; creadores con código de 5 %; *drop* sincronizado con "Estrenos"; sala de *streamer*; prensa *gamer*; Indie Games Fund | US$5–10 mil |
| **LiveOps** | Temporada cada 8 semanas = técnica + artista + *drop* (Grafiti → Tatuaje → Serigrafía → Sticker → Bordado → Tinta derramada); torneo mensual; Día de Muertos, Buen Fin, 10 de mayo | US$300–500 al mes |

**Alternativas:** lanzar en el **Buen Fin de 2027** solo si el online se trabaja en paralelo a la beta. **No** hacer el *soft launch* solo en Colombia, Perú y Chile: sin amigos ni comunidad INKCAP se mediría a gente jugando contra bots, y el ciclo "prenda → juego → prenda" solo se mide en México.

---

## 9. KPIs y metas por etapa

| Etapa | Métrica | Meta |
|---|---|---|
| **0. Prueba de diversión** | Personas que piden "otra" en la prueba de sillón | ≥ 70 % (30 sesiones) |
| | KO que se sienten justos con 100 ms de *lag* simulado | 8 de cada 10 probadores |
| | Rendimiento en un Moto G06 | 30 fps estables |
| | Legibilidad a 64 px: ¿trae o no la gorra? | 9 de cada 10 aciertan |
| **1. Edición Fiesta** | Testers activos | 500 |
| | Partidas por sesión | ≥ 3 |
| | Tiempo de abrir la app a la primera partida | < 30 s, sin cuenta |
| | Sesiones sin fallos (Crashlytics) | ≥ 99.5 % |
| **2. Online casual** | *Ping* medio con relay | < 100 ms desde CDMX |
| | Partida rápida con bots de relleno | Arranca en ≤ 8 s |
| | Canje web phygital | Funciona de punta a punta; fraude < 1 % |
| **3. *Soft launch*** | D1 / D7 / D30 | **≥ 40 % / ≥ 12 % / ≥ 4 %** → escalar; 30–40 % / 7–12 % → iterar 8–12 semanas sin anuncios; < 30 % / < 7 % → replantear el núcleo o dejarlo como herramienta de marca |
| | Conversión a pago | ≥ 1 % |
| | ARPDAU | ≥ US$0.015 |
| | % de partidas con amigos (local o sala) | ≥ 30 % [Estimación] |
| **4. Lanzamiento en México** | Jugadores diarios | 10 mil en 6 meses (equilibrio de operación ≈ 9–11 mil) |
| | Calificación en tiendas | ≥ 4.3 |
| | Ventas atribuidas por UTM y códigos canjeados | Se reportan cada mes por artista |
| **Operación** | Costo de red por jugador al día | < US$0.002 en el modo casual |
| | Temporadas a tiempo | 1 cada 8 semanas |

---

## 10. Checklist legal y de tiendas antes de lanzar

Resumen del reporte 07. Costos [Estimación]: pedir 2–3 cotizaciones a despachos de la CDMX.

| Prioridad | Acción | Costo |
|---|---|---|
| **P0** (antes del arte final) | Empresa dueña del juego (S.A.P.I.) + acuerdo de socios INKCAP–estudio | MXN 30–80 mil |
| P0 | Contratos de cesión con todos los que ya crearon algo (código, bocetos, prototipo) | En la plantilla |
| P0 | Búsqueda de GORRAZO, INKAZO e INKCAPS (MARCia, WIPO, USPTO, tiendas) y **solicitud ante el IMPI**: nombre en 9, 41 y 28 + **INKCAP en 9 y 41 para ropa virtual** (5 clases; INKCAP en 35, recomendable, sería la sexta). Nunca usar Boomerang, Fu, Frisbee, Brawl, Clash, Saga ni Splat | MXN 25–45 mil |
| P0 | **Licencia digital con cada artista:** adaptación 3D, crédito, 15 % del neto, cláusula de **legado** para lo ya vendido y sin entrenar IA. Contrato aparte para el personaje "pillo" | MXN 15–40 mil |
| P0 | D-U-N-S, cuentas de organización, dominios y *handles* | ≈ MXN 5 mil |
| P0 | **Diseño limpio:** diario con fechas; nunca pasarle capturas de *Boomerang Fu* a la IA; gorra sin ojos ni captura (Cappy de *Mario Odyssey*); distancia de *Splatoon* | Gratis |
| **P1** (antes de la beta) | Aviso de privacidad (LFPDPPP 2025), etiquetas de privacidad de Apple y Data safety de Google | MXN 10–30 mil |
| P1 | Términos de servicio y de bienes virtuales (la moneda no caduca y se puede restaurar) | MXN 15–40 mil |
| P1 | Clasificación **Apple 13+ / IARC E10+** [Verificado]; tinta, nunca sangre. Sin chat libre. Sin cajas botín de pago. Pantalla de edad y modo protegido | Gratis |
| P1 | **Borrado de cuenta desde la app** (también invitados); Sign in with Apple si hay Google; **no premiar por activar notificaciones** [Verificado] | Desarrollo |
| P1 | Inventario de licencias, registro en INDAUTOR y junta con el contador (IVA, CFDI, W-8BEN-E, IEPS) | MXN 6–20 mil |
| P1 | **Consultar a App Review el flujo phygital** antes del primer *drop* | Gratis |
| **P2** (antes de salir de México) | Madrid dentro de 6 meses de la solicitud mexicana; edad en EE.UU. (Texas, Utah, Luisiana); RGPD y DSA; excluir Brasil hasta cumplir el ECA Digital; búsqueda de patentes antes de EE.UU. y Japón | US$4–7 mil Madrid; US$3–15 mil patentes |

**Requisitos técnicos** [Verificado salvo lo marcado]: compilar con Xcode 26; Apple permite desde iOS 13 (el GDD apunta a iOS 15+); Android API 36 hoy y, casi seguro, API 37 (Android 17) antes del lanzamiento de 2028 [No verificado: Google aún no publica la fecha]; Play Billing v8 y **páginas de 16 KB antes del 1-feb-2027**; reconocer compras de Google en 3 días; `NSLocalNetworkUsageDescription` y `NSBonjourServices` en iOS (el *broadcast* del núcleo requiere el permiso de multicast de Apple; mientras, Bonjour o QR); pantalla del permiso `ACCESS_LOCAL_NETWORK` de Android 17; prueba de 12 testers × 14 días si la cuenta de Google fuera personal [No verificado].

**Presupuesto legal:** mínimo MXN 60–110 mil; **recomendado MXN 120–220 mil**; expansión global + MXN 150–350 mil.

---

## 11. Riesgos de negocio y mitigación

| Riesgo | Impacto | Mitigación |
|---|---|---|
| **La gente no regresa** (mediana 22 / 4 / 0.7 %) | Crítico | Puertas por etapa; cero anuncios sin retención; cambiar el núcleo si hace falta; racha suave, calendarios de 7 y 28 días, Reto del Día |
| **El *lag* hace injustos los KO** | Alto | Prueba a 50–150 ms antes de feb-2027; el anfitrión decide con compensación de *lag*; doble KO = empate |
| **No alcanzan a producir temporadas** | Alto | Temporadas de 8 semanas, "temporada mínima", configuración remota, catálogo real de INKCAP |
| **La IA mete errores en red y compras** | Alto | Revisor senior; las pruebas automáticas (21 hoy) pasan antes de cada cambio; compras validadas en el servidor |
| **Rechazo por clon** (Apple 4.1) o parecido a *Boomerang Fu*, *Mario Odyssey* o *Splatoon* | Alto | 8 reglas propias (gorra = armado, ¡CLANK!, mural, "Te la regreso", regreso a los 3 s, recarga de *dash*, latas de técnica propias, modos propios); 4 de 6 latas con efecto propio (GDD §9) |
| **Apple rechaza el phygital** | Medio | Canje web + venta en la app; Offer Codes de plan B; consulta previa |
| **Nombre en conflicto o daño a la marca INKCAP** | Medio | Búsqueda antes del logo; INKAZO de respaldo; nombre propio "by INKCAP"; lo local sale como beta |
| **Unity Pro o cambio de precios / cierre de un proveedor** | Medio | Empresa nueva; red tras interfaces intercambiables (Unity, Edgegap, EOS); lo local no depende de nadie |
| **Contratos con artistas incompletos** | Alto | Licencia por escrito y temporal; base de regalía explicada; legado |
| **Menores y privacidad** | Medio | Sin chat libre, modo protegido, sin anuncios personalizados a menores |
| **Contar dos veces las playeras** | Medio | Solo el margen incremental medido con UTM y códigos |
| **Flujo de caja y tipo de cambio** (gastos en USD; Apple paga a 45 días) | Medio | Reserva de 3 meses; pagos por entregable; cotizar en MXN |
| **Permisos de red local** (iOS, Android 17) | Medio | Bonjour o QR + *hotspot*; pantallas de permiso desde el MVP |
| **Dependencia de 1–2 personas** | Medio | CLAUDE.md, pruebas y documentación; el revisor conoce el código |

---

## 12. Plan de acción de 90 días (23-nov-2026 → 21-feb-2027)

| Semana | Fechas | Acciones | Entregable |
|---|---|---|---|
| 1 | 23–29 nov | Junta con el contador (empresa y límite de Unity). **Tramitar el D-U-N-S.** Buscar los 3 nombres en MARCia y tiendas; apartar dominios y *handles*. Cotizar revisor senior. Comprar celulares de prueba | Empresa decidida |
| 2 | 30 nov–6 dic | **Solicitud IMPI** (5 clases). Contratar al revisor. **Prueba de *lag*** a 0–150 ms con `LoopbackTransport`, incluido el desvío. Probar con personas daltónicas la paleta Okabe-Ito (ya está en el prototipo y el núcleo). Quitar la casilla de códigos de iOS. Abrir el diario de diseño | Reporte de *lag* |
| 3 | 7–13 dic | Núcleo: doble KO, una lata a la vez y bots que usan el golpe; **medir** con `HeadlessHost balance` el regreso de la gorra a los 3 s, la recarga del *dash* y el Tiraje (ya programados en el último *commit*). Primer build en Moto G06 | Build Android |
| 4 | 14–20 dic | **Pruebas de sillón 1–10** (posadas, tienda). Legibilidad a 64 px. Contrato tipo con artistas | Primeras métricas |
| 5 | 21–27 dic | Pruebas en familia. **Decisión de red:** Unity Transport + Relay, o Photon Quantum si el *lag* falló | Decisión técnica |
| 6 | 28 dic–3 ene | Semana ligera: plan de arte con la empresa 3D (muñequito base, gorra, playera como textura) | Lista de piezas |
| 7 | 4–10 ene | Latas propias (**muro de tag**, aviso del Trazo, Charco que crece; el **Tiraje** ya existe) y muerte súbita con muros. Abrir cuentas de Apple y Google con el D-U-N-S | Reglas firma |
| 8 | 11–17 ene | **Pruebas 11–20.** La Reta y Lata Caliente. Inscripción al Small Business Program | Métricas por modo |
| 9 | 18–24 ene | 3 cotizaciones por rol (rigs y animación, interfaz, audio). Build de iPhone con Codemagic. Pantalla del permiso de red local | Etapa 1 cotizada |
| 10 | 25–31 ene | **Pruebas 21–30**, una en un bazar INKCAP. Lista de espera de la Edición Fiesta (QR en bolsas) | Lista abierta |
| 11 | 1–7 feb | **Firmar 2 artistas piloto.** Abrir Discord y canal de WhatsApp | Contratos |
| 12 | 8–14 feb | **Reporte de la Puerta 0** ("otra" ≥ 70 %, KO justos a 100 ms, 30 fps, legibilidad): seguir, iterar o parar | Decisión |
| 13 | 15–21 feb | Si se sigue: contratar *freelancers*, calendario del MVP, prueba cerrada de Google Play y TestFlight, Etapa 1 aprobada (≈ MXN 400 mil) | Arranque Etapa 1 |

**Gasto en estos 90 días:** ≈ **MXN 238 mil (US$12.4 mil)**; rango MXN 135–265 mil.

### Decisiones que debe tomar el cliente

1. **Empresa que publica** (S.A.P.I. nueva o INKCAP para el MVP), con el contador. **Esta semana.**
2. **Nombre:** autorizar la búsqueda y la solicitud de "GORRAZO by INKCAP", con INKAZO de respaldo. **Esta semana.**
3. **Escenario de equipo:** (b) con el 3D en casa, y quién del equipo dedica cuánto tiempo. **Semana 1.**
4. **Presupuesto y puertas:** aprobar la Etapa 0 y comprometerse a no gastar en arte final ni en online sin pasar la Puerta 0. **Semana 1.**
5. **Lo local como beta cerrada** (no como lanzamiento público en tiendas). **Semana 2.**
6. **Base de la regalía digital** (15 % del neto) y cómo se explica a los artistas. **Semana 4.**
7. **Artistas piloto** y si el personaje "pillo" entra como invitado de temporada con licencia (fuera del elenco base). **Semana 8.**
8. **Fecha objetivo del lanzamiento en México:** mar-2028 (realista) o Buen Fin de 2027 (agresiva, con el online en paralelo). **Semana 12.**
9. **Fondos:** preparar la solicitud al Indie Games Fund para cuando el juego esté publicado y decidir si se explora Netflix Games para el Modo Pantallota. **Etapa 3.**

---

### Fuentes

- Investigación: `docs/investigacion/00` (síntesis, costos y contradicciones), `02` (mercado y lanzamiento), `03` (motor, tarifas y presupuesto), `04` (red a 1k, 10k y 100k), `05` (backend), `06` (monetización, impuestos y proyección), `07` (legal) y `08` (arte y música).
- Diseño: `docs/diseno/propuestas/` y las evaluaciones del director creativo y del productor (calendario, costos ocultos y puertas). Proyecto: `unity/README.md` y `prototipo/src/`.
- Verificado el 23-nov-2026 en Apple y Android: cuotas de US$99 y US$25, Small Business Program, guías 3.1.1, 3.1.3(b), 3.1.3(e), 3.1.4, 4.1 y 5.1.1(v), Offer Codes, clasificación 13+, pagos a 45 días, IVA a empresas mexicanas, API 36, 16 KB, *promo codes* y permiso de red local de Android 17.
- **Sin verificar** (dominios bloqueados): precios de Unity, Photon, Edgegap, Firebase y Codemagic, comisión de Google, tarifas del IMPI 2026 y tasas fiscales de 2026. Confirmar antes de firmar.
