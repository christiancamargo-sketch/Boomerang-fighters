# 00 · Síntesis de la investigación, contradicciones resueltas y huecos pendientes

> **Proyecto:** "Boomerang Fighters" (nombre de trabajo). Es un *party brawler* móvil para iOS y Android con muñequitos estilo *art toy*, un objeto arrojadizo que regresa, escenarios de arte urbano y trajes basados en merch real de INKCAP. Primero se juega en local por Wi-Fi y después en línea.
> **Fecha de corte:** 23 de noviembre de 2026.
> **Tipo de cambio:** **1 USD ≈ 19.1 MXN**, el mismo de los ocho reportes (FIX de Banxico del 20-nov-2026 ≈ 19.02–19.03; mercado del 21 y 22-nov-2026 ≈ 19.16). Las cifras en pesos están redondeadas.
> **Qué es este documento:** una revisión crítica de los reportes **01 a 08**. Hace cuatro cosas: (1) resume toda la investigación en una página; (2) junta en una sola tabla todos los costos; (3) propone las decisiones finales; y (4) señala dónde se contradicen los reportes y qué falta investigar.

**Nota de método (léela antes de usar las cifras).** Leí completos los ocho reportes. Cuando quise volver a verificar datos en internet, el cupo de búsquedas web de la sesión ya estaba agotado; lo comparten todos los agentes de esta investigación. Además, la red de la sesión no permite abrir `unity.com` ni `revenuecat.com`. Sí pude abrir una página oficial de Apple y verifiqué hoy un dato clave: la tabla de clasificación por edad ([Apple – Age ratings values and definitions](https://developer.apple.com/help/app-store-connect/reference/age-ratings-values-and-definitions/), consultada el 23-nov-2026). Todo lo demás viene de los reportes, con las fuentes que ellos citan. Uso tres etiquetas:

- **[Verificado]:** confirmado en una fuente oficial, por este documento o por el reporte que se indica.
- **[No verificado]:** viene de conocimiento previo o de fuentes secundarias. **Hay que confirmarlo antes de gastar o firmar.**
- **[Estimación]:** cálculo o criterio propio de los reportes. No es un precio publicado.

---

## 1. Resumen ejecutivo (para leer en una página)

1. **El hueco de mercado existe.** *Boomerang Fu* vendió **más de 1 millón de copias** (junio de 2024), tiene ~97 % de reseñas positivas en Steam y sigue sacando DLC en 2026. Pero **nunca tuvo versión móvil ni juego en línea**, que es la crítica más repetida desde 2018 (reporte 01). En móvil, el formato "brawler de arena de un golpe con objeto que regresa" **no tiene dueño claro**. *Party Animals* apenas anunció su versión móvil en jul-2026 (reporte 02).
2. **Lo que se copia es el esqueleto, nunca la piel.** Vista desde arriba, un golpe = KO, arma que regresa, dash y power-ups son ideas libres. Personajes de comida, el bumerán, el sufijo "Fu", sus mapas, su interfaz y la palabra "Frisbee" (marca de Wham-O) **no se usan** (reportes 01 y 07).
3. **Nuestra propuesta:** muñequitos de vinil tipo *platform toy* que "intervienen" los artistas de INKCAP. **La gorra como arma que regresa.** Seis "lenguajes visuales" basados en las seis técnicas de tinta. Arenas de un CDMX en versión diorama: callejón grafiteado, taller de serigrafía y museo (reportes 01, 07 y 08).
4. **Motor: Unity 6.3 LTS.** El equipo ya lo usa en Quest 3, que por dentro es Android. Unity tiene un plugin oficial para Claude Code desde sep-2026, y Unity Personal es gratis si la empresa que lo usa factura menos de USD 200,000 al año (reporte 03).
5. **Multijugador sin reescribir el juego:** Netcode for GameObjects con un celular como anfitrión. En local se juega por la misma red Wi-Fi o por el hotspot de un teléfono, y las partidas se encuentran con Bonjour o con un código QR: **US$0 en servidores**. En línea, el mismo código pasa por Unity Relay: **≈ US$20 al mes con 1,000 jugadores diarios** y ≈ US$260–470 con 10,000 [No verificado]. Los servidores dedicados quedan para el modo competitivo del año 2 (reporte 04).
6. **Jugar en línea sí cuesta, pero no es lo que quiebra un juego.** Con 100,000 jugadores diarios, aun la opción más cara (≈ US$12,000 al mes) equivale a ~US$0.004 por jugador al día. **Lo que quiebra un juego es que la gente no regrese** (reportes 02, 04 y 05).
7. **Negocio:** gratis para jugar y **100 % cosmético**. Pase de temporada de ~$89 MXN, trajes de artista de $59–79 MXN, paquete de inicio de $19 MXN, video con recompensa opcional y **cero cajas botín pagadas**. La comisión de Apple y Google será de **15 %**, no de 30 % (reportes 05 y 06).
8. **La ventaja única de INKCAP es el puente entre la playera física y el traje digital.** La regla de Apple obliga a hacerlo así: el código de la etiqueta se canjea **en la web de INKCAP**, nunca con una casilla dentro de la app de iPhone, y el mismo traje también se vende dentro del juego. Además, vender merch física desde el juego **no paga comisión** a Apple ni a Google (reportes 05, 06 y 07).
9. **México es Android** (~74 % de los celulares) y de gama media. La meta técnica es correr a 30 fps estables en un Galaxy A07 o un Moto G06 (reportes 02, 03 y 08).
10. **Costo del año 1 [Estimación]:** de **≈ US$29,000 a US$91,000 (MXN 0.55–1.73 millones)**, sin contar los sueldos del equipo interno (§4.5). Esto incluye desarrollo externo, legal, marketing, dispositivos, cuentas y servidores. La proyección base de ingresos del reporte 06 deja **≈ US$50,500 netos** el primer año, más **≈ MXN 900 mil** en playeras con código. En el escenario conservador **el juego no paga un equipo**: su valor sería como motor de marca y de venta de merch.
11. **Regla de oro de toda la investigación:** no gastar en anuncios hasta que, en el lanzamiento suave, regresen **al menos 40 % de los jugadores al día siguiente y 15 % a la semana** (D1 ≥ 40 %, D7 ≥ 15 %). La mediana de la industria es 22 % y 4 % (reportes 02 y 05).
12. **Lo más urgente que falta:** elegir el **nombre final** y buscarlo en el IMPI. Revisar el riesgo de propiedad intelectual de **una gorra que se lanza y regresa** frente a *Super Mario Odyssey*, que ningún reporte cubrió. Confirmar los precios oficiales de Unity, Photon, Edgegap y Google Play, que no se pudieron abrir. Decidir **qué empresa publica** (§7).

---

## 2. Mapa del paquete

| # | Reporte | Qué responde | Palabras aprox. |
|---|---|---|---|
| 01 | Boomerang Fu | Mecánicas, ventas, críticas, qué copiar y qué no, oportunidades | 6,500 |
| 02 | Competidores y casos de éxito | Grandes (Supercell, Epic, NetEase, Sega) y pequeños (Stumble Guys, Among Us, BombSquad); mercado México/LATAM; lanzamiento | 7,500 |
| 03 | Motor y herramientas | Unity vs Godot vs Unreal, IA de código, pipeline de arte, requisitos de tiendas, Mac, equipo y presupuesto | 9,500 |
| 04 | Multijugador local y online | Arquitectura, permisos iOS/Android, precios de Photon, Unity, Edgegap, EOS y GameLift, latencia desde CDMX | 12,000 |
| 05 | Backend, progresión y LiveOps | UGS vs Firebase vs PlayFab vs Nakama, reglas de Apple, XP, racha, pase, tienda, eventos | 11,600 |
| 06 | Monetización e impuestos | Modelos, comisiones, IVA/ISR/CFDI, anuncios, precios, phygital, proyección a 12 meses | 11,700 |
| 07 | Legal y propiedad intelectual | Clones, nombre y marca (IMPI, Madrid), privacidad, edad, cajas botín, chat, contratos con artistas, empresa | 13,600 |
| 08 | Dirección de arte | Art toy, legibilidad, 6 tintas, CDMX, trajes modulares, game feel, controles, audio, 14 arenas | 9,000 |

### 2.1 ¿Se cubrió todo lo que pidió el cliente?

| Lo que pidió el cliente | ¿Cubierto? | Dónde | Comentario |
|---|---|---|---|
| Investigar todo sobre *Boomerang Fu* | Sí | 01 | Muy completo |
| Muñequitos | Sí | 08 §2–3, 03 §5 | Proporciones, legibilidad y rarezas |
| XP, recompensas diarias, jugar diario | Sí | 05 §9 | Tablas de XP, calendario de 7 y 28 días, misiones y pase |
| Trajes de pago con playeras y gorras de INKCAP | Sí | 03 §5.2, 06 §6–7, 08 §7 | Pipeline de PNG a traje, precios, regalías y phygital |
| Nombre relacionado con INKCAP | **Parcial** | 07 §2 | Solo da reglas y 5 ideas sin verificar. **No hay nombre elegido ni búsqueda de marca** |
| Otro objeto en lugar del bumerán | **Parcial** | 01 §11, 02 §14, 07 §2 | Gorra, disco, lata o bola de tinta. **Nadie lo decidió** y nadie revisó el riesgo de PI de la gorra (ver H2) |
| Escenarios de arte (calles grafiteadas, museo) | Sí | 08 §5, §13 | 14 ideas de arenas con peligros |
| Unity u otro motor | Sí | 03 | Unity 6.3 LTS |
| iOS y Android | Sí | 03 §8, 04 §2.3–2.4 | Requisitos 2026 verificados en Apple y Android |
| Costo de jugar en línea | Sí | 04 §6 | Tres opciones a 1k, 10k y 100k jugadores diarios |
| Cómo monetizar | Sí | 06 | 22 ideas ordenadas por facilidad e impacto |
| Qué usaron desarrolladores grandes y novatos | Sí | 02, 04 §7, 06 §1.2–1.3 | Grandes, pequeños y fracasos |
| Local por Wi-Fi primero, online después | Sí | 04 §8 | Mismo código en las dos etapas |
| "Todo lo que necesitaríamos" (equipo, presupuesto, legal) | Sí, pero disperso | 03 §9, 07, 02 §13 | Se consolida aquí en §4 |

---

## 3. Cronograma consolidado

Los reportes 02, 03 y 06 proponen calendarios distintos (ver contradicción C7). Este es el calendario unificado que uso en el resto del documento:

| Etapa | Meses | Qué se entrega | Monetización | Meta para pasar a la siguiente |
|---|---|---|---|---|
| **0. Preparación** | Semanas 1–2 | Empresa, D-U-N-S, cuentas Apple y Google, proyecto Unity, CLAUDE.md, dispositivos de prueba | — | Cuentas abiertas a nombre de la empresa |
| **1. Prototipo de diversión** | 0–3 | 1 arena en *greybox*, la gorra que regresa, dash, un golpe = KO, bots, 2–6 celulares por Wi-Fi o hotspot, **prueba con lag simulado** | Ninguna | 4 personas juegan 20 minutos seguidos y piden otra ronda; con 100 ms de lag, 8 de cada 10 no sienten injusticia en los KO |
| **2. Vertical slice + MVP + beta cerrada en México** | 3–9 | Estilo final, 6–8 muñequitos, 3 arenas, 3–4 modos, XP, racha, cuenta de invitado automática, trajes; beta cerrada con la comunidad INKCAP (prueba cerrada de Google Play y TestFlight) | Todo se desbloquea con XP | Comunidad activa y juego estable |
| **3. Lanzamiento suave en Android** | 9–11 | Colombia, Perú y Chile. Tienda, paquete de inicio, pase, video con recompensa y **salas online privadas con código** (Relay) | Sí | D1 ≥ 40 %, D7 ≥ 15 %, conversión ≥ 1 % |
| **4. Lanzamiento en México (iOS + Android)** | ~12 | Primer *drop* phygital, programa de creadores, Small Business Program | Completa | D30 ≥ 5 %; más de 10 mil jugadores diarios |
| **5. Online público y LiveOps** | Año 2 | Emparejamiento público, *ranked* con servidores dedicados, temporadas, Club INKCAP | + Club y patrocinios | Ingresos que cubran la operación |

---

## 4. Tabla de costos consolidada

### 4.1 Costos fijos y de arranque

| Concepto | USD | MXN | Cuándo | Fuente y estado |
|---|---|---|---|---|
| Apple Developer Program | 99 al año | ≈ 1,890 | Etapa 0 | 03, 04 [Verificado en developer.apple.com] |
| Google Play (registro) | 25, una sola vez | ≈ 478 | Etapa 0 | 03, 04 [Verificado de forma indirecta en la FAQ de Android] |
| Número D-U-N-S | 0 | 0 | Etapa 0; puede tardar hasta 28–30 días | 03 [Verificado] |
| Unity Personal | 0 | 0 | Siempre, si la empresa factura menos de USD 200k al año (ingresos + fondos) | 03 [No verificado: unity.com bloqueado] |
| Unity Pro (solo si la empresa pasa de USD 200k) | 2,310 por puesto al año (o 210 al mes) | ≈ 44,100 por puesto | Desde que se rebase el límite | 03 [No verificado] |
| Unity AI dentro del Editor (opcional) | 10 al mes por 1,000 créditos | ≈ 191 | Opcional | 03 [No verificado] |
| Plugin de Unity para Claude Code, Blender, URP, NGO, ProBuilder, Cinemachine | 0 | 0 | Siempre | 03 [No verificado el plugin; lo demás es gratis] |
| Suscripciones de IA de código del equipo | Ya las pagan | — | — | **No cotizado en ningún reporte** |
| Dispositivos de prueba (4–6 Android de gama baja y media, iPhone usado, router) | ≈ 1,050–1,470 | 20,000–28,000 | Etapa 0 | 03 [Estimación] |
| Builds de iOS en la nube (Codemagic: 500 min gratis al mes) | 0–10 al mes | 0–190 | Etapas 0–2 | 03 [No verificado] |
| Mac mini (al empezar las pruebas serias en iPhone) | ≈ 1,020 | desde ≈ 19,499 | Etapa 2 | 03 [No verificado] |
| Dominios (.com, .mx) | 50–150 al año | 950–2,900 | Etapa 0 | 07 [Estimación] |
| Efectos de sonido (bibliotecas y paquetes) | 0–60 | 0–1,150 | Etapa 1 | 08 [Estimación] |
| Paquetes "Feel" y DOTween Pro (opcionales) | **Precio no verificado** | — | Etapa 2 | 03 |

### 4.2 Costos por etapa

> Todo en USD, con MXN entre paréntesis. **No incluye los sueldos del equipo interno.** El desarrollo externo es el **[Estimación]** del reporte 03: un programador senior de Unity como revisor y arte, audio e interfaz por encargo.

| Concepto | 0–1. Preparación + prototipo (meses 0–3) | 2. MVP + beta (meses 3–9) | 3. Lanzamiento suave (meses 9–11) | 4. Lanzamiento MX (mes ~12) | 5. Año 2 (online público) |
|---|---|---|---|---|---|
| Cuentas y dominios | 174–274 (3,300–5,200) | — | — | — | 99 + dominios (renovación) |
| Motor (Unity) | 0 (Personal) | 0 | 0 | 0 | 0, o 2,310 por puesto si se pasa el límite |
| Dispositivos, Mac y builds en la nube | 1,050–1,500 (20,000–28,650) | 0–1,100 (0–21,000) | — | — | — |
| **Desarrollo externo** | 1,500–5,000 (28,650–95,500) | 15,000–40,000 (286,500–764,000); incluye 6–8 muñequitos, rigs, 3 arenas, interfaz, audio y revisión senior | 1,000–5,000 (19,100–95,500) en QA y ajustes | Incluido en la etapa 3 | 20,000–50,000 (382,000–955,000) en la versión online completa |
| **Red multijugador** | 0 | 0 (local) | 0–20 al mes (≈ 1k DAU en Relay) | 20–470 al mes (hasta 10k DAU en Relay) | 750–1,200 al mes con servidores dedicados a 10k DAU |
| **Backend** (UGS + Firebase) | 0 (sin backend) | 0 | ≈ 0 | 0–100 al mes | 300–1,500 al mes a 100k DAU |
| Legal y contador | P0: 3,100–5,800 (60,000–110,000), mínimo viable | P1: completar hasta el recomendado de 6,300–11,500 en total (120,000–220,000) | — | — | Expansión global: + 7,900–18,300 (150,000–350,000) |
| Marketing | — | 500–1,500 (9,550–28,650) en eventos y premios | 1,500–6,000 (28,650–114,600) por 10,000 instalaciones | 5,000–10,000 (95,500–191,000) en creadores y anuncios | Financiado con ingresos y merch |
| Comisiones de tienda | — | — | 15 % de cada compra | 15 % | 15 % (30 % arriba de USD 1 M al año) |

Fuentes de la tabla: 02 §13 (marketing), 03 §8–9 (cuentas, dispositivos, desarrollo), 04 §6 y §8.4 (red), 05 §8 (backend), 06 §2 (comisiones), 07 (presupuesto legal).

### 4.3 Costos mensuales que crecen con los jugadores [No verificado: precios de proveedores no re-verificados]

| Servicio | 1,000 jugadores diarios | 10,000 | 100,000 | Comentario |
|---|---|---|---|---|
| **Unity Relay** (anfitrión en un celular) — *recomendado para salas online* | ~US$20 (MXN 400) | US$260–470 (MXN 5,000–9,000) | US$2,800–4,900 (MXN 53,500–93,600) | Cobra por jugadores conectados **en promedio**, no en el pico; 50 gratis |
| Photon Fusion o Quantum (alternativa) | US$8–125 | US$250–500 | US$5,000–7,500 | No funciona en LAN sin internet |
| Servidores dedicados en Edgegap (*ranked*, año 2) | US$75–120 | US$750–1,200 | US$7,500–12,000 | Sin trampas del anfitrión |
| Epic Online Services, P2P con relay | US$0 | US$0 | US$0 | Soporte móvil por confirmar |
| **Backend UGS** (cuentas, guardado, economía) | ≈ US$0 | US$0–100 | US$300–1,500 | Correr el estimador oficial de UGS antes de firmar |
| Backend 100 % Firebase (plan B) | US$0–5 | US$10–30 | US$150–400 | Hay que programar la economía desde cero |
| Crashlytics y notificaciones push (FCM) | Gratis | Gratis | Gratis | — |

### 4.4 Cuánto le queda a INKCAP de cada venta [Estimación]

Supuestos: los precios de tienda en México ya incluyen IVA; la comisión de 15 % se calcula sobre el precio sin IVA; la regalía de 15 % al artista se calcula sobre el neto (criterio de los reportes 06 y 07). **Hay que confirmarlo con el contador.**

| Producto | Precio al público | Sin IVA | − Comisión (15 %, o ~4 % + $3 en la web con OXXO) | Neto | − Regalía 15 % | **Para INKCAP** (antes de ISR) |
|---|---|---|---|---|---|---|
| Traje de artista en la App Store | $79 | $68.10 | $10.22 | $57.89 | $8.68 | **≈ $49.20** |
| Mismo traje en la web de INKCAP con OXXO | $79 | $68.10 | $6.16 | $61.94 | $9.29 | **≈ $52.65** |
| Pase de temporada (si incluye contenido del artista invitado) | $89 | $76.72 | $11.51 | $65.22 | $9.78 | **≈ $55.40** |
| Paquete de inicio (sin artista) | $19 | $16.38 | $2.46 | $13.92 | — | **≈ $13.90** |

### 4.5 Presupuesto total aproximado del año 1 [Estimación]

| | Bajo | Alto |
|---|---|---|
| Cuentas, dominios, dispositivos, Mac y builds | US$1,224 | US$2,884 |
| Desarrollo externo (prototipo + MVP + lanzamiento suave) | US$17,500 | US$50,000 |
| Legal y contador | US$3,100 (mínimo) | US$11,500 (recomendado) |
| Marketing (beta + lanzamiento suave + lanzamiento MX) | US$7,000 | US$17,500 |
| Red y backend (≈ 4 meses en línea) | US$0 | US$2,000 |
| Unity Pro (solo si aplica; 3 puestos) | US$0 | US$6,930 |
| **Total** | **≈ US$29,000 (≈ MXN 550 mil)** | **≈ US$91,000 (≈ MXN 1.73 millones)** |

**No incluye:** sueldos del equipo interno, suscripciones de IA de código, producción de playeras, ISR ni adquisición de jugadores después del lanzamiento. Para comparar, el reporte 06 proyecta un neto de **US$5,300 / US$50,500 / US$442,000** en los escenarios conservador, base y optimista del primer año en tiendas, más ventas de playeras con código de **MXN 135 mil / 900 mil / 4.5 millones**.

---

## 5. Decisiones recomendadas consolidadas

### 5.1 Las 10 decisiones

| # | Decisión | Detalle | Reportes |
|---|---|---|---|
| **1** | **Motor: Unity 6.3 LTS + URP + C#** | Usar el plugin oficial de Unity para Claude Code. Migrar a 6.7 LTS antes de lanzar. Contratar a un **programador senior de Unity de medio tiempo** (5–10 h por semana) que revise el código de red y de compras. Estilo 3D *low-poly* con cámara cenital inclinada y *shader* matcap de vinil | 03, 08 |
| **2** | **Red: NGO + Unity Transport, con un celular como anfitrión** | En local: IP directa por Wi-Fi o hotspot, con descubrimiento por **Bonjour/mDNS + QR** (no requiere el permiso especial de multicast de Apple). En línea: el **mismo código** por **Unity Relay**, con salas por código. *Ranked*: el mismo código como servidor dedicado en Edgegap o GameLift Querétaro en el año 2. **Prueba con lag simulado antes del mes 3**; si falla, ese es el único momento razonable para cambiarse a Photon Quantum 3 | 03, 04 |
| **3** | **Lanzamiento por fases, local primero** | Prototipo y beta cerrada **solo en local + bots** (cumple el plan del cliente). Lanzamiento suave en Android en **Colombia, Perú y Chile**, ya con **salas online privadas** por Relay (≈ US$0–20 al mes). Lanzamiento en México en iOS y Android a la vez. Emparejamiento público y *ranked* en el año 2 | 02, 03, 04, 06 |
| **4** | **El objeto arrojadizo es la gorra** | Es merch real de INKCAP. Cuando la lanzas, tu cabeza queda descubierta y todos ven que estás desarmado: la mecánica central se vuelve visible. Y el *skin* de la gorra sirve a la vez como cosmético y como proyectil, así que vale el doble. **Nunca** decir "Frisbee", "Boomerang" ni "Fu". **No** darle ojos ni poderes de "captura" a la gorra (ver H2) | 01, 02, 07, 08 |
| **5** | **Nombre: "INKCAP + nombre de fantasía"** | Hacer una lista corta (§7, H1), buscarla en MARCia, WIPO, USPTO y en las tiendas, y **presentar la solicitud ante el IMPI en clases 9 y 41** (+28 y 35). **Registrar INKCAP para ropa virtual** (9, 35 y 41) antes de anunciar los trajes. Presentar la solicitud por el Protocolo de Madrid dentro de los 6 meses siguientes si el juego tiene tracción | 07 |
| **6** | **Diseño del juego y arte del MVP** | **2 a 6 jugadores** en el MVP; las salas de 8 quedan para el modo online. Rondas de 30–60 s y partidas de 3–4 min. Joystick flotante + botón de lanzar (un toque apunta solo; mantener y arrastrar apunta a mano) + dash + botón especial que solo aparece cuando lo tienes. **Bots de 3 dificultades** desde el día 1. **3 arenas:** Callejón Grafiteado, Taller de Serigrafía y Museo de la Tinta. El color de cada jugador va en un aro bajo los pies, con la paleta Okabe-Ito (segura para daltonismo), nunca en la ropa | 01, 03, 08 |
| **7** | **Monetización 100 % cosmética, sin cajas botín** | Dos monedas: Gotas, que se ganan jugando, y Tinta Dorada, que se compra. **Pase de $89 MXN** (probar $79 contra $99). **Trajes de artista de $59–79** y sets de $99–149. Paquete de inicio de $19. Video con recompensa opcional con **AdMob** y sin anuncios entre rondas. **Regalía de 15 % del neto** para cada artista. Solo latas de recompensa gratis, con probabilidades publicadas | 05, 06, 07 |
| **8** | **Phygital con canje en la web** | Código único en la etiqueta → **página web de INKCAP** con la cuenta del juego → el traje aparece en el inventario. El mismo traje (o su versión base) **también se vende dentro del juego**, como pide la regla 3.1.3(b) de Apple. En iOS no habrá casilla para códigos propios (regla 3.1.1); para campañas se usan Offer Codes de Apple y *promo codes* de Google. Botón "Tienda INKCAP" solo con productos físicos, **sin comisión**. Consultar a App Review antes del primer *drop* | 05, 06, 07 |
| **9** | **Backend y progresión por etapas** | Prototipo **sin backend**: el progreso se guarda cifrado en el celular. Desde la beta: **UGS** (cuenta de invitado automática + Apple + Play Games, Cloud Save, Economy, Cloud Code en C#, Remote Config) + **Firebase** (Crashlytics y notificaciones push) + **Unity IAP** con validación en el servidor. Bono de primera victoria del día, calendario de 7 y 28 días con racha suave, 3 misiones diarias y 7 semanales. **Temporadas de 8 semanas**, una por técnica de tinta | 04, 05 |
| **10** | **Empresa y cumplimiento** | **Nueva S.A.P.I. dueña de toda la propiedad intelectual** (o la empresa de INKCAP solo para el MVP). Cuentas de Apple y Google **como organización**, con el D-U-N-S tramitado ya. Inscripción al Small Business Program (15 %). **Apple 13+ / IARC E10+**, con KO de tinta y nunca sangre. Sin chat libre: chat rápido, emotes y *stickers* de artistas. Aviso de privacidad bajo la LFPDPPP de 2025. Licencia digital con cada artista que incluya adaptación 3D, crédito, cláusula de **legado** y prohibición de entrenar IA con su arte | 06, 07 |

### 5.2 Decisiones secundarias ya resueltas

| Tema | Decisión |
|---|---|
| Analítica | Unity Analytics; GameAnalytics como alternativa. Solo **una** herramienta principal (05) |
| Reporte de fallos | Crashlytics desde la primera instalación de prueba (05) |
| Notificaciones | Locales con Unity Mobile Notifications y remotas con FCM. Máximo 1 al día; **nunca dar premio por aceptarlas** (05) |
| Contenido nuevo sin actualizar la app | Addressables + CDN para trajes; las mecánicas nuevas sí pasan por revisión de la tienda (03, 05) |
| Builds de iOS | Codemagic o Unity Build Automation al principio; Mac mini al empezar el MVP (03) |
| Edad | Pantalla de edad neutral + "modo protegido" para menores; Declared Age Range API en iOS (05, 07) |
| Música | Comisionada a productores locales con cesión de derechos, para que los *streamers* no reciban reclamos (07, 08) |
| Compartir clips | Repetición del KO de 2 s con botón para compartir en TikTok, Reels y WhatsApp. Su implementación técnica está pendiente (H4) |

---

## 6. Contradicciones encontradas y cómo se resuelven

| # | Tema | Qué dice cada reporte | Resolución | Confianza |
|---|---|---|---|---|
| **C1** | Ventas de *Boomerang Fu* | 02 (tabla §3 y §6): "500 K unidades en ~2 años". 01: **más de 1 millón** para junio de 2024 | **Vale la de 01: más de 1 millón.** El 02 usó la cifra de 2022 | Alta. 01 cita SteamDB y Wikipedia del parche 1.3.1 |
| **C2** | Cuota de Android en México | 02: **74.1 % / 25.9 %** (StatCounter, oct-2026). 03: **78.7 % / 21.3 %** (StatCounter, jul-2026) | **Usar 74/26 (oct-2026)**, por ser el dato más reciente. La conclusión no cambia: ~3 de cada 4 celulares son Android | Media. No se pudo re-verificar |
| **C3** | Jugadores por partida | 01: *Boomerang Fu* admite hasta 6. 03 calcula el rendimiento para 6. 02, 04 y 08 proponen 2–8 | **MVP de 2 a 6 jugadores.** El presupuesto de rendimiento está hecho para 6, un hotspot aguanta 5–10 equipos y la pantalla es de 6". La red (30 Hz) y la paleta ya están pensadas para 8, así que las salas de 8 pueden llegar con el modo online | Alta (decisión de diseño) |
| **C4** | Duración de rondas y partidas | 01: rondas de 30–60 s. 02: rondas de 60–90 s, partidas de menos de 5 min. 04: partidas de 2–4 min | **Rondas de 30–60 s; partida a 3–5 puntos ≈ 3–4 min.** Ajustar en las pruebas | Alta |
| **C5** | Controles y golpe cuerpo a cuerpo | 01: tres verbos (lanzar, tajo cuerpo a cuerpo y dash). 02: joystick + 1 botón (+ esquivar opcional). 08: lanzar (toque o arrastre) + atrapar automático + dash + especial, **sin golpe cuerpo a cuerpo** | **Esquema del 08 como base.** El golpe cuerpo a cuerpo se resuelve en el prototipo con una prueba A/B: (A) sin golpe; (B) el botón de lanzar da un golpe si el rival está a menos de ~1.5 m y tienes la gorra. Conservar el dilema "armado o desarmado" es lo que no se negocia | Media. **Pendiente de prueba** |
| **C6** | Objeto arrojadizo y cómo se llama | 01 prefiere la gorra. 02 propone la gorra o una lata de aerosol. 07 habla de "gorra o disco". 08 no decide. **05 lo llama "frisbee"** en recompensas y misiones (§9.3 y §9.5), pese a que 01 y 07 lo prohíben por ser marca de Wham-O | **La gorra** (decisión 4). **Hay que corregir el texto del 05:** "Sticker para el frisbee" → "Sticker para la gorra"; "cuando el frisbee vuelve" → "cuando la gorra vuelve" | Alta |
| **C7** | Cuándo llega el modo online | 02: online básico en el lanzamiento en México (meses 9–12). 03: versión online en los meses 10–16. 06: online en el año 2. 07: primero en México solo por Wi-Fi local | **Salas online privadas con código (Relay) en el lanzamiento suave; online público y *ranked* en el año 2.** Motivo: un juego solo local depende de que haya amigos juntos y difícilmente llega a D1 de 40 % (ver H3). Las salas privadas usan el mismo código (solo cambia el "transporte") y cuestan ≈ US$0–20 al mes. **Si el presupuesto o la prueba de lag no dan, salir solo con local + bots y agregar las salas en una actualización 2–3 meses después** | Media (decisión de producto) |
| **C8** | Dónde se hace el lanzamiento suave | 02: Colombia, Perú y Chile. 03: "lanzamiento suave en México". 06: Android en Latinoamérica hispana | **Beta cerrada (no pública) en México con la comunidad INKCAP + lanzamiento suave público de Android en Colombia, Perú y Chile.** Ahí la instalación cuesta menos de US$1 y se conserva el "estreno" en México para el marketing | Alta |
| **C9** | Orden de plataformas | 02 y 06: Android primero e iOS después. 03: publicar en las dos tiendas en el MVP. 04: juego cruzado iOS ↔ Android | **Desarrollar y probar las dos desde el día 1**, porque el juego cruzado en local exige probar iPhone ↔ Android. El lanzamiento suave es solo Android; el de México, en las dos tiendas a la vez | Alta |
| **C10** | Precio del pase | 02: 79–99 MXN. 05: 450 Tinta Dorada ≈ **$89**. 06: **79–99** (sus ejemplos usan $99) | **$89 MXN** como precio inicial, con A/B de $79 contra $99. También se vende como compra directa, para que no sobren monedas | Alta |
| **C11** | Precio de los trajes de artista | 05: playera de artista de 600–900 TD (**$119–179 MXN**) y paquete de colaboración de **$239–299**. 06: traje de artista de **$59–79** y set completo de **$99–149** | **Usar los precios del 06 en México:** comunes $29–39, de artista $59–79, set $99–149, Edición Limitada hasta $199. Motivo: en LATAM el gasto mensual por pagador ronda US$4–10 (06) y el gasto medio por jugador móvil es muy bajo (≤ ~$138 MXN al año según el cálculo del 02). Los precios altos del 05 quedan como prueba A/B o para EE. UU. **Hay que recalcular el ejemplo de regalías del 05 (§9.1)** | Media |
| **C12** | Estructura de monedas y paquetes | 05: dos monedas, Gotas y Tinta Dorada, con paquetes de $19 / $89 / $179 / $349. 06: una sola "tinta" con paquetes de $19 / $99 / $199 / $499 | **Dos monedas (diseño del 05) con los precios redondos del 06** ($19 / $99 / $199 / $499). Antes hay que confirmar en App Store Connect qué puntos de precio en MXN existen, y mostrar siempre el equivalente en pesos (07) | Media |
| **C13** | Clasificación por edad | 06: "clasificación **9+/12+**". 07: **13+** en Apple. 05: declarar público de 13+ en Google Play | **Apple 13+, IARC E10+/PEGI 7 y público objetivo de 13+ en Google Play.** **Verificado hoy:** en la tabla oficial de Apple, la violencia caricaturesca **frecuente** da **13+** y la **poco frecuente**, 9+. **El 12+ ya no existe** desde jul-2025. El dato del 06 es obsoleto | **Alta [Verificado]** |
| **C14** | Red de anuncios al inicio | 03: LevelPlay o MAX con AdMob dentro de la mediación. 06: AdMob solo, y mediación a partir de ~5,000 jugadores diarios | **Empezar solo con AdMob y solo video con recompensa.** Pasar a MAX o LevelPlay con más de 5k jugadores diarios. El propio 03 da "solo AdMob al inicio" como alternativa | Alta |
| **C15** | RevenueCat: ¿1 % del total o del excedente? | 03: 1 % de todo el ingreso rastreado arriba de US$2,500. 05: 1 % solo del excedente | **No se pudo resolver** (revenuecat.com bloqueado y sin búsquedas). **No afecta el MVP**, porque se usará Unity IAP. Solo importa si se lanza un "Club INKCAP" por suscripción | Baja. Pendiente |
| **C16** | Arenas del MVP | 03: 3 arenas (calle, museo, serigrafía o tatuaje). 08: 2 (callejón + serigrafía o tortillería; el museo después). 05: las misiones semanales suponen 3, con museo | **3 arenas: Callejón Grafiteado, Taller de Serigrafía y Museo de la Tinta**, este último simplificado (solo láseres y esculturas destructibles). El cliente pidió el museo de forma explícita y el presupuesto del 03 ya cubre 3. El prototipo usa 1 (el callejón) | Alta |
| **C17** | Cómo se encuentran las partidas en local | 03: QR con la IP (+ un componente comunitario de *broadcast*). 04: **Bonjour/mDNS como principal + QR de respaldo** | **Vale la del 04.** Bonjour funciona entre iOS y Android sin el permiso especial de multicast de Apple (verificado en la nota técnica TN3179). El QR queda de respaldo. Las dos opciones disparan el aviso de "Red local" | Alta [Verificado por 04] |
| **C18** | Cuándo se monetiza | 03: tienda con Unity IAP en el MVP local ("o desbloqueo por XP"). 05: el pase sale en el lanzamiento suave. 06: sin monetización en el prototipo, tienda en la beta y pase en el lanzamiento en México. 07: lanzar con "privacidad mínima" y sin cuentas obligatorias | **Prototipo y beta cerrada: todo por XP, sin dinero. Lanzamiento suave: cuenta de invitado automática, tienda, pase y video con recompensa,** porque esa etapa existe para medir la monetización. La cuenta de invitado **no** es un inicio de sesión obligatorio, así que también cumple con el 07 | Alta |
| **C19** | Duración de las temporadas | 02, 06 y 08: 6–8 semanas. 05: 6 semanas, y dice que así "sale un año de contenido" (6 × 6 = 36 semanas ≈ 8 meses: **error de cuenta interno**) | **8 semanas.** Seis técnicas × 8 semanas ≈ 1 año, con menos presión de producción para un equipo pequeño (lección de Knockout City). **Hay que recalibrar el pase del 05** (40 niveles y XP por nivel) a 8 semanas | Alta |
| **C20** | Phygital | 02: "la playera trae un código para la skin" (genérico). 03: "código impreso en la playera → traje gratis" **choca** con la regla 3.1.1. 05 y 06: **canje en la web** + el traje también se vende dentro de la app (3.1.3(b)) + Offer Codes | **Canje en la web de INKCAP (05 y 06).** No es una contradicción de fondo: el 03 analizaba el canje *dentro* de la app, que sí está prohibido en iOS. La exclusividad de una "Edición Física" sin versión en la app **hay que consultarla con App Review** | Alta [Verificado por 03, 05 y 06 en las App Review Guidelines] |
| **C21** | Sobre qué se calcula la regalía | 06 y 07: **15 % del neto** (sin IVA ni comisión). 08 §4.4: "15 % sobre la venta del cosmético" (ambiguo) | **15 % del neto**, definido por escrito en el contrato, junto con cómo se reparten los paquetes y el pase | Alta |
| **C22** | Costo de servidores en la proyección optimista | 06: US$6,000 en los meses 7–12 con 45k jugadores diarios. Con las fórmulas del 04 (Relay) más el backend del 05, salen **≈ US$8,500–12,000** en 6 meses | **El escenario optimista del 06 subestima los servidores en ~40–100 %.** El impacto en el neto es pequeño (< 2 %) | Media [Estimación] |
| **C23** | Gasto por jugador en México | Dentro del 02: ~4,900 MXN al año "por jugador de smartphone" (prensa) contra ≤ ~138 MXN calculados con los datos de The CIU | **Planear con el número bajo** (la mayoría paga poco o nada). El propio 02 ya marca este conflicto | Media |

---

## 7. Huecos pendientes (lo que el cliente pidió o necesita y no quedó resuelto)

Prioridad: **P0** = antes de invertir en arte final y marca · **P1** = antes del lanzamiento suave · **P2** = antes del online público.

| # | Prioridad | Hueco | Por qué importa | Cómo se cierra |
|---|---|---|---|---|
| **H1** | P0 | **No hay nombre final ni búsqueda de marca.** El 07 solo da reglas y 5 ideas sin verificar: *Gorrazo*, *Rebotinta*, *Tinta Toss*, *Cap Back!* e *INKCAP Arena* ("Arena" es descriptivo y débil) | Sin nombre no hay logo, dominios, cuentas de redes ni solicitud ante el IMPI. Tampoco empieza a correr la prioridad de 6 meses del Protocolo de Madrid | Taller de nombres → lista corta de 5–8 → búsqueda en MARCia, WIPO, USPTO, TMview, App Store y Google Play → solicitud ante el IMPI en clases 9 y 41. Regla: "INKCAP presenta: [nombre de fantasía]"; evitar *Boomerang*, *Frisbee*, *Fu*, *Brawl*, *Clash*, *Saga* y *Splat-* |
| **H2** | P0 | **Riesgo de PI de una gorra que se lanza y regresa.** Ningún reporte menciona a ***Super Mario Odyssey*** (Nintendo, 2017), donde la gorra "Cappy" se lanza, regresa y **captura** enemigos. El 07 recuerda que Nintendo demandó a *Palworld* con **patentes** sobre lanzar objetos para capturar. También hay arte previo de gorras que se lanzan: Kung Lao en *Mortal Kombat* (1993) u Oddjob en *Goldfinger* (1964) [No verificado: conocimiento previo; no se pudo buscar] | La gorra es el objeto recomendado y Nintendo defiende su PI con fuerza | **No** dar a la gorra ojos, personalidad ni poder de captura o posesión. Usarla solo como proyectil de golpe. Hacer una búsqueda ligera en Google Patents ahora, y una búsqueda formal de libertad de operación (USD 3,000–15,000) antes de lanzar en EE. UU. o Japón |
| **H3** | P1 | **Retención de un juego solo local.** Todas las metas (D1 ≥ 40 %, D7 ≥ 15 %) vienen de juegos F2P en línea. Nadie estimó cómo retiene un juego que necesita amigos en el mismo cuarto | Es el motivo para adelantar las salas online privadas (C7) | Medir en la beta cerrada: partidas contra bots en solitario y sesiones por grupo. Diseñar un modo solo contra bots con misiones |
| **H4** | P1 | **Cómo compartir clips en celulares de gama baja.** El 08 propone el botón, pero nadie evaluó si grabar video en un Galaxy A07 afecta el rendimiento | Los clips son el motor de marketing (TikTok) | Opción barata: como la simulación es determinista, reconstruir la repetición a partir de las entradas de los jugadores y grabarla **solo al pedirla**, en el vestidor y no durante la partida. Probarlo en el MVP |
| **H5** | P1 | **No se revisaron competidores directos en móvil.** Nadie buscó en App Store y Google Play clones móviles de *Boomerang Fu* o brawlers con arma que regresa. El 01 menciona "muchos clones" sin listarlos | Valida el "hueco de mercado" | Revisión manual de las tiendas + AppMagic o Sensor Tower en su versión gratuita |
| **H6** | P0 | **¿La empresa que publica pasa de USD 200k al año?** Nadie lo sabe. Además, no está confirmado si Unity suma los ingresos de empresas afiliadas [No verificado] | Define si se paga Unity Pro (USD 2,310 por puesto al año) | El contador revisa las cifras; leer los términos de Unity (unity.com bloqueado en esta sesión) |
| **H7** | P0 | **Precios oficiales sin verificar:** Unity (Pro, Relay, Lobby, Cloud Save, Economy), Photon (y si existe la región `ussc` en Dallas), Edgegap, RevenueCat, Codemagic y GameLift en Querétaro | Toda la tabla de §4.3 depende de ellos | Permitir esos dominios en la red de la sesión o hacer una segunda ronda de búsqueda; correr el estimador de UGS |
| **H8** | P0 | **Reglas de Google Play sin verificar:** si las cuentas de organización están exentas de la prueba de 12 testers durante 14 días, la comisión de 15 %, la política sobre canjear códigos propios dentro de la app, la política de Familias y el texto de borrado de cuenta (support.google.com bloqueado) | Afecta el calendario y el diseño phygital en Android | Leer la ayuda de Play Console; abrir la cuenta como **organización** |
| **H9** | P1 | **Temas fiscales abiertos:** alcance del IEPS de 8 % a videojuegos "violentos" (2026), si la retención de ISR a personas físicas en plataformas subió a 2.5 %, cómo trata Google el IVA de desarrolladores mexicanos y los puntos de precio exactos en MXN | Afecta el precio final y el flujo de caja | Junta con el contador usando la lista del 06 §4.7 |
| **H10** | P0 | **¿Los contratos actuales con artistas cubren el videojuego?** ¿Y la mascota "pillo"? | Sin licencia digital no se puede usar ningún diseño | Anexo de licencia digital (07 §7.2) y contrato aparte para la mascota |
| **H11** | P1 | **Costo de operar el juego (LiveOps):** *community manager*, soporte, moderación y Discord. Ningún reporte lo cuantifica | Es un costo fijo mensual real | Estimar 0.5–1 persona dedicada desde el lanzamiento suave |
| **H12** | P1 | **Tarifas reales de freelancers** de Unity, 3D, animación y música. Las del 03 y el 08 son estimaciones sin verificar | Son el 60–70 % del presupuesto del año 1 | Pedir 3 cotizaciones por rol (Workana, Upwork, ArtStation, LinkedIn) |
| **H13** | P2 | **Fecha en que Google Play exigirá API 37 (Android 17).** Desde entonces el modo local necesitará pedir el permiso `ACCESS_LOCAL_NETWORK` | Si llega antes del lanzamiento, el modo local necesita una pantalla de permiso nueva | Seguir el calendario de Play; dejar la pantalla de permiso diseñada desde el MVP (04) |
| **H14** | P2 | **¿Fusion 2 funciona en LAN sin internet?** No se confirmó | Solo importa si la prueba de lag obliga a dejar NGO | Probarlo con el SDK y el módem desconectado |
| **H15** | P1 | **El texto del 05 todavía dice "frisbee"** y su pase está calibrado para 6 semanas | Son inconsistencias internas | Corregirlo al pasar del 05 al documento de diseño (C6 y C19) |

---

## 8. Datos que cuestan dinero y no se pudieron verificar

| Dato | Valor que usan los reportes | Dónde confirmarlo |
|---|---|---|
| Unity Pro | USD 2,310 por puesto al año (desde el 12-ene-2026) | [Unity – pricing updates](https://unity.com/products/pricing-updates) |
| Límite de Unity Personal | USD 200,000 de ingresos + fondos en 12 meses | Ídem |
| Unity Relay | 50 CCU promedio gratis; US$0.16 por CCU; US$0.09 por GiB en América | [UGS Pricing](https://unity.com/products/gaming-services/pricing) |
| Photon Fusion o Quantum | 100 CCU gratis; US$125 al mes por 500 CCU | [Photon Fusion pricing](https://www.photonengine.com/fusion/pricing) |
| Edgegap | US$0.00115 por vCPU-minuto + US$0.10 por GB | [Edgegap pricing](https://edgegap.com/resources/pricing) |
| RevenueCat | Gratis hasta US$2,500 de ingreso mensual rastreado; luego 1 % (¿del total o del excedente?) | [RevenueCat pricing](https://revenuecat.com/pricing) |
| Codemagic | 500 min macOS gratis al mes; luego US$0.095 por minuto | [Codemagic docs](https://docs.codemagic.io/billing/pricing/) |
| Google Play | 15 % sobre el primer USD 1 M; prueba de 12 testers durante 14 días para cuentas personales | [Ayuda de Play Console](https://support.google.com/googleplay/android-developer/answer/14151465) |
| IMPI | ≈ MXN 3,000–3,600 por clase (2026 sin confirmar) | marcia.impi.gob.mx / tarifas del IMPI |
| IEPS a videojuegos | 8 % a clasificación "C" o superior | DOF / contador |

**Ya verificado en fuente oficial** (por este documento o por los reportes): Apple USD 99 al año; Google USD 25; Small Business Program de Apple (15 % hasta USD 1 M de *proceeds*, con inscripción); reglas 3.1.1, 3.1.3(b), 3.1.3(e) y 3.1.4; Offer Codes; Xcode 26 desde el 28-abr-2026; API 36 de Android desde el 31-ago-2026; páginas de memoria de 16 KB desde el 1-feb-2027; permisos de red local en iOS (TN3179) y Android 17; clasificación **13+** por violencia caricaturesca frecuente.

---

## 9. Próximos pasos (los primeros 30 días)

1. **Decidir qué empresa publica** y revisar con el contador si pasa de USD 200k (H6). **Tramitar el D-U-N-S hoy mismo:** tarda hasta 28–30 días.
2. **Taller de nombres** → lista corta → búsqueda en MARCia y WIPO → solicitud ante el IMPI (H1). Apartar dominios y cuentas de redes el mismo día.
3. **Búsqueda ligera de patentes** sobre "gorra que se lanza y regresa" en Google Patents (H2).
4. **Anexo de licencia digital** con 1–2 artistas piloto, y pedir permiso para usar la mascota "pillo" (H10).
5. **Arrancar el prototipo** (Unity 6.3 LTS, NGO, 1 arena, bots), con la **prueba de lag simulado** y la **prueba A/B del golpe cuerpo a cuerpo** (C5).
6. **Permitir los dominios bloqueados** (unity.com, photonengine.com, edgegap.com, revenuecat.com, support.google.com) o pedir una segunda ronda de búsqueda para cerrar H7 y H8.
7. **Corregir el reporte 05** al pasar al documento de diseño: quitar "frisbee", poner temporadas de 8 semanas y usar los precios de C11 y C12.

---

## 10. Fuentes

**Reportes de esta investigación (fuente de casi todo lo consolidado aquí)**
- 01 · Boomerang Fu: `./01-boomerang-fu.md`
- 02 · Competidores y casos de éxito: `./02-competidores-y-casos-de-exito.md`
- 03 · Motor de juego y herramientas: `./03-motor-de-juego-y-herramientas.md`
- 04 · Multijugador local y online: `./04-multijugador-local-y-online.md`
- 05 · Backend, progresión y LiveOps: `./05-backend-progresion-y-liveops.md`
- 06 · Monetización, tiendas e impuestos: `./06-monetizacion-tiendas-e-impuestos.md`
- 07 · Legal, propiedad intelectual y políticas: `./07-legal-propiedad-intelectual-y-politicas.md`
- 08 · Dirección de arte, mundo y referencias: `./08-direccion-de-arte-mundo-y-referencias.md`

**Verificado directamente por este documento (23-nov-2026)**
- Apple – Age ratings values and definitions (violencia caricaturesca frecuente = 13+; poco frecuente = 9+): https://developer.apple.com/help/app-store-connect/reference/age-ratings-values-and-definitions/

**Fuentes clave que citan los reportes (para re-verificar)**
- Apple – Inscripción al Developer Program: https://developer.apple.com/support/enrollment/
- Apple – Small Business Program: https://developer.apple.com/app-store/small-business-program/
- Apple – App Review Guidelines (3.1.1, 3.1.3, 3.1.4, 4.1, 4.8, 5.1.1): https://developer.apple.com/app-store/review/guidelines/
- Apple – Próximos requisitos (Xcode 26, iOS 13 mínimo): https://developer.apple.com/news/upcoming-requirements/
- Apple – TN3179, privacidad de red local: https://developer.apple.com/documentation/technotes/tn3179-understanding-local-network-privacy
- Apple – Offer Codes para compras dentro de la app: https://developer.apple.com/help/app-store-connect/manage-in-app-purchases/create-offer-codes-for-in-app-purchases
- Android – FAQ de verificación de desarrolladores (cuota de US$25, D-U-N-S): https://developer.android.com/developer-verification/guides/faq
- Android – Requisito de API objetivo: https://developer.android.com/google/play/requirements/target-sdk
- Android – Permiso de red local (Android 17): https://developer.android.com/privacy-and-security/local-network-permission
- Android – Promo codes de Play: https://developer.android.com/google/play/billing/promo
- Unity – Pricing updates: https://unity.com/products/pricing-updates
- Unity – UGS Pricing: https://unity.com/products/gaming-services/pricing
- Unity – Estimador de precios de UGS: https://unity-player-services-pricing-estimator.ds.unity3d.com/
- Unity – Cancelación del Runtime Fee: https://unity.com/blog/unity-is-canceling-the-runtime-fee
- Unity – Plugin para Claude Code (PocketGamer.biz): https://www.pocketgamer.biz/unity-launches-official-claude-code-plugin-with-29-built-in-engine-skills/
- Photon – Precios de Fusion: https://www.photonengine.com/fusion/pricing
- Edgegap – Precios: https://edgegap.com/resources/pricing
- AWS – GameLift en mx-central-1: https://aws.amazon.com/about-aws/whats-new/2026/09/amazon-gamelift-servers-region-expansion-2026/
- Codemagic – Precios: https://docs.codemagic.io/billing/pricing/
- RevenueCat – Precios: https://revenuecat.com/pricing
- StatCounter – Sistemas operativos móviles en México: https://gs.statcounter.com/os-market-share/mobile/mexico
- GameAnalytics vía GameDev Reports – Retención: https://gamedevreports.substack.com/p/gameanalytics-mobile-and-pc-game
- Playio – eCPM 2026: https://blog.playio.co/mobile-game-ecpm-benchmarks-2026
- Game Growth Advisor – CPI en LATAM 2026: https://gamegrowthadvisor.com/blog/2026-05-26-latam-mobile-game-market-entry-guide-2026/
- Google Play – Indie Games Fund LATAM: https://play.google.com/intl/id_ALL/console/about/programs/indiegames/
- Wham-O – Protección de la marca Frisbee: https://wham-o.com/pages/wham-o-brand-protection
- Steam – Respuesta del desarrollador de Boomerang Fu sobre el online: https://steamcommunity.com/app/965680/discussions/0/1748980761815284909
- SteamDB – Parche 1.3.1 de Boomerang Fu (1 M de copias): https://steamdb.info/patchnotes/14321551
- Tipo de cambio – FIX de Banxico: https://dolar.info/mexico/hoy/precio/banxico-fix.php
