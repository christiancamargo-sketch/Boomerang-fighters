# 05 · Backend, progresión y LiveOps: cuentas, economía, pase de temporada y operación diaria

> **Proyecto:** Boomerang Fighters (nombre de trabajo). Party brawler móvil (iOS + Android) en Unity, de muñequitos estilo *art toy*, con un objeto arrojadizo que regresa. Primero multijugador local por Wi-Fi y después online.
> **Fecha de corte:** 23 de noviembre de 2026.
> **Tipo de cambio:** 1 USD ≈ **19.10 MXN**, el mismo de los reportes 01 a 04 (FIX del DOF 19.0270 al 20-nov-2026, según [dolardof.com](https://dolardof.com/)). Las cifras en pesos están redondeadas.

---

## 0. Nota de transparencia (léela primero)

- **No pude hacer ninguna búsqueda web.** El presupuesto de búsquedas es compartido (200 por turno entre todos los agentes de esta investigación) y ya estaba agotado cuando empezó este reporte.
- **Solo pude leer páginas de `developer.apple.com`.** Los dominios `unity.com`, `firebase.google.com` y `playfab.com` no resuelven desde la red de esta sesión. Para verificarlos con una herramienta hay que permitirlos en la configuración de red del entorno. Desde un navegador normal se abren sin problema.
- Cada dato lleva una de estas etiquetas:
  - ✅ **Confirmado:** lo leí hoy en developer.apple.com, o ya lo verificaron los reportes 02, 03 o 04 con búsquedas de noviembre de 2026. En ese caso cito la fuente original.
  - ⚠️ **Conocimiento previo:** viene de documentación que conozco hasta 2025 o mediados de 2026. Aquí caen **casi todos los precios y límites gratuitos** de Unity Gaming Services, Firebase, PlayFab, Nakama, LootLocker, Beamable, Supabase, Metaplay, AccelByte, GameAnalytics, Sentry y Backtrace, además de varios detalles de diseño de Brawl Stars, Fortnite y otros. **Hay que confirmarlos en la página oficial antes de presupuestar.**
  - 🧮 **Estimación propia:** un cálculo con los supuestos que explico en cada caso.
- **Lo que sí es sólido:** los requisitos de Apple sobre cuentas, borrado de cuenta, compras y edad (texto oficial leído hoy), los datos de retención y de Brawl Stars que verificó el reporte 02, y todo el **diseño de progresión** de las secciones 6 y 9, que no depende de precios.

---

## 1. Resumen ejecutivo (para leer en 3 minutos)

1. **El backend es el "notario" del juego.** El servidor guarda el registro oficial de la XP, las monedas y los trajes de cada jugador; el celular solo lo muestra. Sin backend, cualquiera puede editar su archivo de guardado, darse todos los trajes gratis y acabar con la tienda.
2. **El prototipo local por Wi-Fi no necesita backend.** El progreso se guarda en el celular. El backend se vuelve **obligatorio** cuando se vende algo con dinero real, cuando hay tablas de clasificación o cuando el jugador quiere conservar su progreso al cambiar de celular.
3. **Recomendación de stack:** usar **Unity Gaming Services (UGS)** como núcleo: Authentication, Cloud Save, Economy, Cloud Code en C#, Remote Config y Leaderboards. Está en el mismo panel de Unity y usa el mismo lenguaje que el juego, y Unity tiene **plugin oficial para Claude Code** desde sep-2026 ✅ ([reporte 03](./03-motor-de-juego-y-herramientas.md)). **Firebase** se usaría solo para **Crashlytics y notificaciones push (FCM)**, que son gratis ⚠️. Si los precios 2026 de UGS no convencen, el **plan B es todo en Firebase**. A gran escala se pueden evaluar **Nakama + Hiro/Satori** o **PlayFab**.
4. **Costo del backend de progresión 🧮:** **≈ USD 0 al mes con 1k jugadores diarios (DAU)**, **≈ USD 10–100 con 10k DAU** y **≈ USD 150–1,500 con 100k DAU**, según el proveedor. **La red del juego online (reporte 04) es un costo aparte y mayor.**
5. **Apple, inicio de sesión ✅:** si ofreces Google o Facebook como inicio de sesión, la guía **4.8** te obliga a ofrecer también un inicio de sesión "equivalente" que proteja la privacidad. **Sign in with Apple** es la forma más simple de cumplir. Si la app no tiene funciones importantes que dependan de una cuenta, debe dejar jugar **sin iniciar sesión**, así que conviene tener **cuenta de invitado**.
6. **Apple, borrar la cuenta ✅ (guía 5.1.1(v)):** el borrado debe poder **iniciarse dentro de la app**, aplica **también a las cuentas de invitado** y obliga a **revocar los tokens de Sign in with Apple**.
7. **Apple, compras ✅ (guía 3.1.1):** la moneda comprada **no puede caducar**. Las cajas aleatorias pagadas deben **publicar sus probabilidades**. **No se permiten códigos QR ni claves para desbloquear contenido dentro de la app.** Por eso el código que venga con una playera física debe canjearse **en el sitio web de INKCAP**, ligado a la cuenta del juego (guía 3.1.3(b)), o hacerse con los **Offer Codes de Apple**, que desde oct-2025 sirven para cualquier tipo de compra ✅.
8. **Apple, notificaciones ✅:** **no se puede dar una recompensa por activar las notificaciones** (5.1.2(i)). Las notificaciones de marketing requieren consentimiento explícito dentro de la app y una forma de desactivarlas (4.5.4).
9. **Progresión propuesta:** nivel de cuenta por XP, **bono de primera victoria del día**, **calendario de 7 días con "racha suave"** (faltar un día no te regresa al inicio), **3 misiones diarias y 7 semanales**, y un **pase de temporada de 40 niveles** cada **6 semanas** con vía gratis y vía premium de **≈ 89 MXN**. Cada temporada se dedica a una de las **6 técnicas de tinta**. Dos monedas (blanda y dura), tienda rotativa y **cero cajas botín pagadas**. Todo lo que se vende es **100 % cosmético**.
10. **Retención de referencia ✅ (reporte 02):** el juego mediano retiene **22 % al día 1, 4 % al día 7 y 0.7 % al día 30**; el 10 % mejor llega a **~40 % D1 y ~12 % D7**. **Meta mínima antes de invertir en anuncios: D1 ≥ 40 %, D7 ≥ 15 %, D30 ≥ 5 %.**
11. **Edad de los jugadores ✅:** en 2025–2026 Apple agregó las clasificaciones 13+, 16+ y 18+, la **Declared Age Range API** y herramientas para las leyes de Texas (SB 2420, vigente desde el 4-jun-2026), Utah, Luisiana, Brasil, Australia y Singapur. Con muñequitos caricaturescos van a llegar niños, así que **hay que diseñar el manejo de edad desde el MVP**.

---

## 2. Las piezas del backend, en lenguaje simple

| Pieza | Qué hace en nuestro juego | ¿Cuándo hace falta? |
|---|---|---|
| **Cuentas (autenticación)** | Identifica al jugador: invitado, Apple, Google Play Games | MVP |
| **Guardado en la nube** | XP, nivel, racha y ajustes; sobrevive a un cambio de celular | MVP |
| **Economía** | Monedas blanda y dura, precios, compras con moneda virtual | MVP (antes de vender) |
| **Inventario** | Qué trajes, gorras, *stickers* y emotes tiene cada quien | MVP |
| **Validación de compras** | Comprueba con Apple o Google que el pago es real antes de entregar el traje | **Obligatoria** al vender |
| **Código en servidor** (Cloud Code / Functions) | Reglas que el jugador no puede manipular: entregar recompensas, calcular XP, abrir *drops* | MVP |
| **Configuración remota** | Cambiar valores sin publicar una actualización: XP por partida, precios, eventos | MVP |
| **Pruebas A/B** | Mostrar dos versiones a grupos distintos y medir cuál retiene o vende más | Lanzamiento suave |
| **Tablas de clasificación** | *Rankings* semanales, de amigos y por región | Online |
| **Analítica** | Saber qué hacen los jugadores: retención, embudos, economía | MVP |
| **Reporte de fallos** | Saber en qué celular y en qué línea de código truena el juego | Desde el prototipo |
| **Notificaciones push** | Avisos como "tu recompensa diaria está lista" o "empezó la temporada" | MVP (locales) / Online (remotas) |
| **Eventos y temporadas** | Contenido con fecha de inicio y fin, controlado desde el servidor | Post-lanzamiento |

---

## 3. Comparativa de plataformas de backend

### 3.1 Tabla general

> **Ojo:** salvo RevenueCat, los precios de esta tabla son ⚠️ **conocimiento previo, no verificado en nov-2026**.

| Plataforma | Qué es | Plan gratuito ⚠️ | Cómo cobra ⚠️ | Ideal para | Riesgo principal |
|---|---|---|---|---|---|
| **Unity Gaming Services** | Suite de Unity: Authentication, Cloud Save, Economy, Cloud Code, Remote Config + Game Overrides, Leaderboards, Analytics, Triggers/Scheduler, Friends, Relay y Lobby | Cada servicio tiene su capa gratuita. En mi experiencia, un juego chico suele quedar dentro de ella. **No pude ver los límites 2026.** Estimador oficial: [UGS Pricing Estimator](https://unity-player-services-pricing-estimator.ds.unity3d.com/) (enlace del reporte 04) | Pago por uso, por servicio (llamadas, almacenamiento, MAU de analítica) | Equipos 100 % Unity que programan en C# con IA | Unity ya cambió reglas antes (Runtime Fee) y cerró Multiplay ✅ ([reporte 04](./04-multijugador-local-y-online.md)) |
| **Firebase** (Google) | Auth, Firestore, Cloud Functions, Remote Config, A/B Testing, Analytics, Crashlytics, FCM, App Check | **Spark:** gratis. Firestore con 50k lecturas, 20k escrituras y 20k borrados al día y 1 GiB de almacenamiento. Analytics, Crashlytics, FCM y Remote Config sin costo | **Blaze:** pago por uso; incluye la misma capa gratis. Firestore ≈ USD 0.03–0.06 por 100k lecturas y ≈ USD 0.09–0.18 por 100k escrituras, según región. Functions: 2 M invocaciones gratis al mes y luego ≈ USD 0.40 por millón | Lo más barato y probado para reportes de fallos, push y analítica | **Functions exige Blaze** (tarjeta de crédito). La economía del juego (inventario, pase) hay que programarla desde cero |
| **Microsoft PlayFab** | Backend completo para juegos: Economy v2, Leaderboards v2, CloudScript/Azure Functions, Experiments (A/B), Insights | **Development Mode** gratis hasta **100,000 cuentas** de jugador por título. **No pude confirmar si cambió en 2025–2026** | Al pasar a producción: plan de pago con cuota base mensual (antes era ≈ USD 99/mes en el nivel Standard) más medidores de uso. La página de precios ahora está en [developer.microsoft.com](https://developer.microsoft.com/en-us/games/products/playfab/pricing/) ✅ | Juegos con LiveOps intensas, *crossplay* y ecosistema Xbox | Curva de aprendizaje alta y precios difíciles de predecir |
| **Nakama** (Heroic Labs) | Servidor de juego de **código abierto (Apache 2.0)**: cuentas, billetera, inventario, tablas, amigos, chat, validación de compras y partidas | Gratis si lo alojas tú. El reporte 04 calcula un VPS de USD 10–40 al mes para empezar ✅/⚠️ | **Heroic Cloud** (administrado): por cotización. **Hiro** (metajuego: economía, rachas, pase, eventos) y **Satori** (LiveOps: A/B, banderas, audiencias) son de pago | Escala grande con control total y sin atarse a un proveedor | Requiere a alguien que opere servidores (DevOps) |
| **LootLocker** | Backend "listo para juegos": jugadores, progresiones, monedas, catálogos, tablas, validación de compras | Tiene plan gratuito para empezar. **Cifras 2026 no confirmadas** | Por jugadores activos al mes (MAU) | Equipos chicos que no quieren programar servidor | Proveedor pequeño: existe riesgo de cierre o venta (ver el caso Hathora en el reporte 04) |
| **Beamable** | LiveOps nativo de Unity: contenido, inventario, tienda, eventos, microservicios en C# | Capa de desarrollo gratuita. **Cifras no confirmadas** | Por uso | Estudios Unity que quieren todo dentro del editor | Proveedor pequeño y comunidad reducida |
| **Supabase** | Base de datos Postgres con Auth, almacenamiento y funciones; no está hecho para juegos | **Free:** 50k MAU, 500 MB de base de datos. Los proyectos se pausan tras 7 días sin uso, así que no sirve para producción | **Pro:** USD 25/mes con 100k MAU incluidos; luego ≈ USD 0.00325 por MAU extra | Si ya dominan SQL y quieren un backend genérico | Toda la lógica de juego (economía, pase, inventario) es programación propia |
| **Metaplay** | *Full-stack* en C# que comparte código entre cliente y servidor, hecho para F2P serios | No sé si tiene plan gratuito en 2026 | Licencia/planes, por cotización | Estudios medianos con ingenieros de backend | Excesivo para el MVP |
| **AccelByte** (AGS) | Suite completa: cuentas, economía, temporadas, *matchmaking*, *hosting* | "Shared Cloud" con pago por uso. **Cifras no confirmadas** | Por uso o contrato | Estudios medianos y grandes, *crossplay* consola/PC | Pensado para equipos más grandes |
| **RevenueCat** (solo compras) | Valida compras, administra suscripciones y unifica iOS/Android | ✅ Gratis hasta **USD 2,500 (≈ MXN 47,750)** de ingreso mensual rastreado | ✅ **1 %** del ingreso rastreado arriba de eso ([RevenueCat](https://revenuecat.com/pricing), vía reporte 03) | Una futura suscripción "Club INKCAP" | Para cosméticos sueltos no es indispensable |

### 3.2 Qué cubre cada uno (✔ incluido · ◐ parcial o hay que programarlo · ✖ no)

| Función | UGS | Firebase | PlayFab | Nakama (+Hiro/Satori) | LootLocker | Supabase |
|---|---|---|---|---|---|---|
| Invitado + Apple + Google Play Games | ✔ ⚠️ | ✔ ⚠️ | ✔ | ✔ | ✔ | ◐ |
| Guardado en la nube | ✔ Cloud Save | ✔ Firestore | ✔ | ✔ | ✔ | ✔ |
| Monedas e inventario | ✔ Economy | ◐ propio | ✔ Economy v2 | ✔ | ✔ | ◐ propio |
| Validación de compras Apple/Google | ✔ Economy ⚠️ | ◐ con Functions | ✔ | ✔ | ✔ | ◐ propio |
| Pase de temporada y eventos | ◐ Cloud Code + Remote Config | ◐ propio | ◐ | ✔ con Hiro | ◐ | ◐ propio |
| Tablas de clasificación con reinicio | ✔ | ◐ propio | ✔ | ✔ | ✔ | ◐ propio |
| Configuración remota + A/B | ✔ Game Overrides | ✔ Remote Config + A/B | ✔ Experiments | ✔ con Satori | ◐ | ✖ |
| Analítica | ✔ | ✔ (GA4) | ✔ Insights | ✔ con Satori | ◐ | ✖ |
| Reporte de fallos | ◐ ⚠️ | ✔ Crashlytics | ✖ | ✖ | ✖ | ✖ |
| Push | ◐ ⚠️ (ver nota) | ✔ FCM | ◐ | ◐ | ✖ | ✖ |
| Lenguaje del código en servidor | **C#** o JS | JS/TS (Python) | C#/JS (Azure Functions) | Go/TS/Lua | — | TS/SQL |

**Notas importantes ⚠️:**
- **Push de UGS:** el servicio estuvo en beta durante 2023–2024 y **no pude confirmar si sigue activo en 2026**. Para no depender de él: **FCM** para push remoto (gratis; llega a iPhone a través de APNs) y el paquete **Unity Mobile Notifications** para avisos locales, como "tu recompensa diaria está lista" ✅ (incluido en Unity según el [reporte 03](./03-motor-de-juego-y-herramientas.md)).
- **Firebase Storage:** desde finales de 2024 Google exige el plan **Blaze** para los *buckets* de Storage. Los proyectos Spark perdían acceso a principios de 2026. **Firebase Dynamic Links se apagó en agosto de 2025**, así que no se debe usar para invitaciones. Para eso existen los App Links de Android y los Universal Links de iOS.
- **Firebase Auth:** sin el *upgrade* a Identity Platform, los inicios de sesión de invitado, Apple, Google y Play Games no tienen cobro por MAU; el teléfono (SMS) sí cobra. **Con el *upgrade***, solo los primeros 50k MAU son gratis y luego se paga cada MAU. Con 100k DAU (≈ 350k MAU) eso podría costar **más de USD 1,500 al mes** 🧮. **No hagan ese *upgrade* sin necesidad.**
- **Unity Multiplay** dejó de ser operado por Unity el **31-mar-2026** y hoy lo opera Rocket Science Group. Relay, Lobby y Matchmaker **siguen** ✅ ([reporte 04](./04-multijugador-local-y-online.md)).

### 3.3 Compras dentro de la app: Unity IAP o RevenueCat

| | **Unity IAP 5** | **RevenueCat** |
|---|---|---|
| Costo | Gratis ✅ ([reporte 03](./03-motor-de-juego-y-herramientas.md)) | Gratis hasta USD 2,500 de ingreso mensual rastreado; luego 1 % ✅ |
| Validación | Local en el celular (falsificable). **Hay que validar además en el servidor**, con UGS Economy o una Function propia ⚠️ | Valida en su servidor y avisa por *webhook* ⚠️ |
| Mejor para | Monedas, trajes sueltos y pase de temporada | Suscripciones y análisis de ingresos |
| **Decisión** | **Usarlo desde el MVP** | Solo si se lanza un "Club INKCAP" mensual |

### 3.4 Analítica

| Herramienta | Costo ⚠️ | Fortalezas | Debilidades |
|---|---|---|---|
| **Unity Analytics** | Capa gratis, luego por uso | Mismo panel que UGS. Mide los resultados de los A/B de Game Overrides. Explorador SQL | Precio 2026 no confirmado |
| **Firebase Analytics (GA4)** | Gratis, sin límite de eventos (máx. ~500 tipos distintos) | Audiencias para Remote Config, A/B y FCM. Exportación a BigQuery | Pensado para apps en general; hay que armar los reportes de economía |
| **GameAnalytics** | Núcleo gratis; planes de pago para datos crudos | Hecho para juegos: retención, ARPDAU, **flujo de recursos** (fuentes y sumideros de monedas) y *benchmarks* del sector | Otro SDK más |

**Decisión:** usar **una sola** herramienta principal para no duplicar eventos. Con UGS como núcleo, **Unity Analytics** (si su capa gratis alcanza); si no, **GameAnalytics** como alternativa gratuita.

### 3.5 Reporte de fallos

| Herramienta | Costo ⚠️ | Comentario |
|---|---|---|
| **Firebase Crashlytics** | **Gratis** | Estándar en móvil. Soporta Unity e IL2CPP (subir símbolos) |
| **Sentry** | Gratis hasta ~5k errores al mes y 1 usuario; plan Team ≈ USD 26/mes | Buen SDK de Unity, *breadcrumbs* y rendimiento |
| **Backtrace** (Sauce Labs) | Capa gratis pequeña; planes de pago | Muy usado por estudios grandes; buen manejo de volcados nativos |

**Decisión:** **Crashlytics desde el primer prototipo** que se instale en celulares de prueba.

---

## 4. Requisitos de las tiendas: cuentas, borrado, compras y edad

### 4.1 Apple: texto oficial leído hoy ✅ ([App Review Guidelines](https://developer.apple.com/app-store/review/guidelines/))

| Guía | Qué dice (resumido fielmente) | Qué hacemos |
|---|---|---|
| **4.8 Login Services** | Si usas un inicio de sesión de terceros o social (Google, Facebook, X, etc.) para la cuenta principal, debes ofrecer **otro equivalente** que: limite los datos a **nombre y correo**, permita **ocultar el correo** y **no rastree con fines publicitarios** sin consentimiento. Hay excepciones, por ejemplo si la app solo usa cuentas propias | En iOS: **Invitado + Sign in with Apple** (+ Google opcional). Sign in with Apple cumple los tres requisitos |
| **5.1.1(v) Borrado de cuenta** | "Si tu app permite crear cuentas, debes ofrecer el borrado de la cuenta **dentro de la app**." En la misma guía: si la app no tiene funciones importantes basadas en una cuenta, **deja usarla sin iniciar sesión** | Botón "Eliminar mi cuenta" en Ajustes. **Cuenta de invitado** para jugar de inmediato |
| **Detalle del borrado** ([página de Apple](https://developer.apple.com/support/offering-account-deletion-in-your-app/)) | Debe borrar el registro **completo** y los datos personales (salvo lo que la ley obligue a conservar). **Desactivar la cuenta no basta.** Incluye **cuentas de invitado creadas automáticamente**. Puede terminar en un sitio web solo si la app enlaza directo a esa página. No se puede exigir llamada ni correo. Se puede confirmar con un código. Hay que **revocar los tokens** con la REST API de Sign in with Apple. Si hay suscripciones, hay que avisar que el cobro sigue con Apple. El borrado no tiene que ser inmediato si se informa el plazo | El servidor borra Cloud Save, Economy, tablas y el usuario de Auth, y revoca el token de Apple. Mostrar "se completará en hasta 7 días" |
| **3.1.1 Compras** | Desbloquear funciones o contenido (monedas, niveles, contenido premium) **requiere compra dentro de la app**. **No se permiten mecanismos propios** como "claves de licencia, marcadores de RA, **códigos QR**". La moneda comprada **no puede caducar**. Hay que ofrecer **restaurar** las compras restaurables. Las cajas aleatorias pagadas deben **publicar las probabilidades antes de comprar** | Monedas sin caducidad. Inventario ligado a la cuenta. Sin cajas pagadas |
| **3.1.3(b) Multiplataforma** | Se permite usar en la app contenido **adquirido en otras plataformas o en tu sitio web**, incluso consumibles, **siempre que también se venda dentro de la app** | El traje "gemelo digital" de una playera física se canjea en la web de INKCAP **y** también se vende en la tienda del juego |
| **3.1.3(e) Bienes físicos** | Los bienes físicos se cobran **fuera** del sistema de compras de Apple | Vender playeras reales dentro del juego es posible con pasarela propia (incluido OXXO), sin la comisión de Apple |
| **3.2.2(x)** | No se puede obligar a calificar la app. **Sí se puede premiar acciones** dentro de la app, como completar un nivel o ver un anuncio | Los anuncios con recompensa opcionales están permitidos |
| **5.1.2(i)** | No se puede exigir activar **notificaciones**, ubicación o rastreo para **recibir compensación** | **No dar premio por activar push** |
| **4.5.4 Push** | Las notificaciones de **promoción** requieren **consentimiento explícito** en la interfaz y una forma de **darse de baja** | Pantalla de permiso propia + interruptor "Ofertas y eventos" |

**Otros datos de Apple confirmados hoy ✅** ([Apple Developer News](https://developer.apple.com/news/), [Small Business Program](https://developer.apple.com/app-store/small-business-program/)):
- **Comisión del 15 %** con el Small Business Program si los ingresos netos son menores a **USD 1 M** al año. Hay que inscribirse; no es automático.
- **Offer Codes para todo tipo de compra** (consumibles y no consumibles) desde el **29-oct-2025**. Desde el **26-mar-2026** ya no se pueden crear *promo codes* de compras dentro de la app.
- **Nuevas clasificaciones de edad 13+, 16+ y 18+** (24-jul-2025). El nuevo cuestionario, con preguntas sobre controles dentro de la app y violencia, venció el **31-ene-2026**. En sep-2026 se agregaron preguntas de redes sociales.
- **Declared Age Range API**, disponible en todo el mundo desde iOS 26 (23-dic-2025). Apple la relaciona con la ley de **Texas SB 2420, vigente desde el 4-jun-2026**, y con requisitos de edad de **Utah, Luisiana, Brasil, Australia y Singapur** (24-feb-2026).
- **Guía 2.5.19** (16-oct-2026): mostrar después de la aprobación **funciones ocultas** que Apple no revisó puede provocar la baja de la app. Descargar trajes nuevos es contenido y está bien; activar **mecánicas nuevas** escondidas, no.
- Sign in with Apple usa el nuevo dominio de correo privado `private.icloud.com` desde ago-2026. Las direcciones viejas siguen funcionando.

### 4.2 Google Play ⚠️ (no pude abrir support.google.com ni developer.android.com)

- **Borrado de cuenta:** obligatorio desde 2024. Debe existir **dentro de la app** y además un **enlace web** donde se pueda pedir el borrado sin reinstalar. Ese enlace se declara en el formulario de **Seguridad de los datos (Data safety)**. La página puede vivir en el sitio de INKCAP.
- **Google Play Games Services v2:** inicia sesión **automáticamente** al abrir el juego, sin botón. La v1 está obsoleta. UGS y Firebase pueden usar la identidad de Play Games como cuenta. También ofrece logros y tablas gratis.
- **Play Billing Library:** Google obliga a actualizar a una versión nueva cada año. Unity IAP se encarga de eso, pero hay que actualizar el paquete.
- **Compras:** hay que **reconocer (acknowledge)** cada compra en **3 días** o Google la reembolsa automáticamente. Las cajas aleatorias también deben mostrar sus probabilidades.
- **Comisión** del 15 % sobre el primer USD 1 M anual (programa de Google).
- **Política de Familias:** si en la ficha se elige un público que incluye **menores de 13 años**, hay restricciones fuertes de anuncios y datos. Recomiendo declarar **público de 13+** y poner una pantalla neutral de edad.

### 4.3 México y Latinoamérica ⚠️

- **Datos personales:** en marzo de 2025 se publicó una **nueva Ley Federal de Protección de Datos Personales en Posesión de los Particulares** y el INAI desapareció. Hace falta un **aviso de privacidad** que cubra las cuentas, la analítica y las compras. Conviene revisarlo con abogado.
- **Brasil:** el **ECA Digital** (Ley 15.211/2025) **prohíbe las cajas botín en juegos dirigidos a menores** desde 2026. Apple también aplica reglas específicas de edad para Brasil ✅ ([Apple News, 2-nov-2026 y 24-feb-2026](https://developer.apple.com/news/)). Es otra razón para **no tener cajas pagadas**, sobre todo si se lanza en LATAM.
- **IEPS a videojuegos violentos (2026):** México aprobó un impuesto especial para videojuegos "violentos". Un juego caricaturesco sin sangre probablemente no aplica, pero hay que confirmarlo con el contador.

### 4.4 Flujo de cuentas recomendado

1. **Primera vez:** se crea una **cuenta de invitado** sin pedir nada (anónima en UGS o Firebase). Se juega en 10 segundos.
2. **Al llegar al nivel 3 o antes de la primera compra:** aparece "Protege tu progreso". En iOS se ofrece **Apple** (+ Google opcional); en Android, **Google Play Games**. Ligar la cuenta es opcional, pero se recomienda.
3. **Si la cuenta tiene Apple y Google ligados**, el inventario pasa entre iPhone y Android ✅ (3.1.3(b)).
4. **Ajustes → Eliminar cuenta:** confirmación, borrado en el servidor, revocación del token de Apple y aviso sobre compras.
5. **Edad:** pedir el rango de edad (en iOS con la Declared Age Range API) **antes de mostrar la tienda**. A menores se les muestra la tienda con límites y sin ofertas agresivas.

---

## 5. Arquitectura y antitrampas

### 5.1 Cómo se conectan las piezas

```
 Celular (Unity)                         Nube
 ─────────────────                       ──────────────────────────────────────────
 Juego / UI ──── inicio de sesión ─────► Authentication (invitado/Apple/Play Games)
     │                                         │
     │── "terminé partida" (resumen) ─────────►│ Cloud Code (reglas)  ──► Cloud Save (XP, racha)
     │── "reclamar recompensa diaria" ────────►│   valida hora del     ──► Economy (monedas, inventario)
     │── "comprar traje con monedas" ─────────►│   servidor y topes    ──► Leaderboards
     │                                         │
     │── compra real (Unity IAP) ──► Apple/Google ──recibo──► Cloud Code valida con Apple/Google
     │                                                         y entrega el traje una sola vez
     │◄── configuración remota (precios, eventos, XP) ──────── Remote Config + Game Overrides (A/B)
     │── eventos de analítica ────────────────────────────────► Analytics
     │── fallos ──────────────────────────────────────────────► Crashlytics
     │◄── push ───────────────────────────────────────────────── FCM (→ APNs en iPhone)
```

### 5.2 Siete reglas de oro (para pedírselas a la IA que programa)

1. **El celular solo pide; el servidor decide.** El cliente nunca dice "dame 500 monedas". Dice "terminé una partida con este resultado" y el servidor calcula cuánto le toca.
2. **La hora la pone el servidor.** La recompensa diaria y la racha usan la hora del servidor, para que cambiar el reloj del celular no sirva para hacer trampa. Hay que definir un **reinicio diario fijo**, por ejemplo a las 00:00 hora del centro de México.
3. **Topes diarios.** Máximo de XP y monedas por día, y máximo de partidas que cuentan para recompensas. Eso limita el daño si alguien hace trampa.
4. **Idempotencia.** Cada compra y cada recompensa tiene un ID único. Si el celular reintenta, no se entrega dos veces.
5. **Reembolsos.** Hay que escuchar las notificaciones de Apple (App Store Server Notifications) y de Google (Real-time Developer Notifications) para **quitar el traje** si alguien pide su reembolso ⚠️.
6. **Integridad del dispositivo.** Usar App Attest en iOS y Play Integrity en Android (Firebase App Check lo empaqueta) para bloquear clientes modificados ⚠️.
7. **Capa de abstracción.** Todo el acceso al backend pasa por interfaces propias (`IPerfilService`, `IEconomiaService`, `ITiendaService`). Así se puede cambiar de UGS a Firebase o Nakama sin reescribir el juego. Es la lección de Hathora y Multiplay en 2026 ✅ ([reporte 04](./04-multijugador-local-y-online.md)).

### 5.3 Validación de compras, paso a paso ⚠️

1. Unity IAP completa la compra y obtiene el **recibo**: en iOS, la transacción firmada (JWS) de StoreKit 2; en Android, el *purchase token*.
2. El celular manda el recibo a **Cloud Code** (o a UGS Economy, que trae validación de compras reales de Apple y Google).
3. El servidor verifica la firma o consulta la API de Apple o Google, revisa que el ID de transacción **no se haya usado** y entrega el contenido.
4. En Android, el servidor **reconoce** la compra (máximo 3 días).
5. Solo entonces el celular "consume" la compra y muestra el traje.

### 5.4 Modo local por Wi-Fi sin internet

- El progreso se guarda en un archivo local **cifrado**, con una cola de "pendientes por sincronizar".
- En partidas locales, cada celular gana **XP y monedas blandas** con un **tope diario**, por ejemplo 300 Gotas al día sin conexión. Al volver la conexión, el servidor **acepta lo que cae dentro de los topes** y descarta el resto.
- **La moneda dura, las compras y las tablas nunca funcionan sin conexión.** Las partidas locales **no cuentan para tablas**, porque el anfitrión podría mentir.

### 5.5 Modelo de datos de ejemplo (perfil en Cloud Save)

```json
{
  "perfil": { "nivel": 12, "xp": 4630, "personajeFavorito": "pillo_01" },
  "racha": { "dias": 5, "ultimoDia": "2026-11-22", "protectores": 1 },
  "calendario": { "diaActual": 4, "ultimoReclamo": "2026-11-22" },
  "misiones": { "diarias": ["ko_regreso_5", "jugar_3", "atrapar_10"], "rerollUsado": false },
  "pase": { "temporada": "T1_grafiti", "xpPase": 18250, "premium": true, "reclamados": [1,2,3,4,5] },
  "ajustes": { "pushOfertas": false, "rangoEdad": "13-15" }
}
```
Las monedas y el inventario van en **Economy**, no en este JSON, para que solo el servidor pueda modificarlos.

---

## 6. Cómo diseñan la progresión los grandes

### 6.1 Comparativa

> ✅ = verificado por el reporte 02 · el resto es ⚠️ conocimiento previo. Estos juegos cambian sus sistemas cada pocos meses; revisen la versión actual.

| Juego | Progresión de cuenta | Pase | Diarios y misiones | Monedas | Aleatoriedad | Lección para nosotros |
|---|---|---|---|---|---|---|
| **Brawl Stars** (Supercell) | **Trophy Road**: premios por hitos de trofeos. La **Starr Road** (desbloquear brawlers con créditos) se rediseñó en 2024–2025 | Brawl Pass gratis + premium (+ Plus desde 2024) | Misiones diarias y de temporada; las primeras victorias del día dan **Starr Drops** | Monedas, gemas (dura), créditos, *bling* (cosméticos) | **Quitó las cajas botín pagadas (dic-2022)** ✅. Los Starr Drops son gratis y se ganan jugando | En 2024 sus ingresos se multiplicaron ~9x con **mejor pase, economía mejor administrada y sin cajas** ✅ ([DoF](https://www.deconstructoroffun.com/blog/2024/2/11/brawl-stars-to-the-moon), [GamingOnPhone](https://gamingonphone.com/news/brawl-stars-revenue-jumped-to-almost-9x-and-mau-to-2-4x-in-2024-compared-to-last-year/)) |
| **Stumble Guys** (Kitka → Scopely) | Nivel de cuenta y "coronas" | Stumble Pass gratis + premium | Misiones y eventos | Gemas (dura) y tokens (blanda) | Ruletas o giros con gemas: ojo regulatorio | Creció con **influencers a los que se les daba moneda del juego para regalar** ✅ ([GameAnalytics](https://gameanalytics.com/?p=17893)) |
| **Fall Guys** (Mediatonic/Epic) | *Crown Rank* por coronas ganadas | Pase de temporada gratis + premium (950 Show-Bucks) | Desafíos diarios y semanales | Kudos (blanda), Show-Bucks (dura) | Tienda directa, sin cajas | Pasó a F2P en jun-2022. Hecho en Unity ✅ (reporte 02) |
| **Fortnite** (Epic) | Nivel de temporada; XP de todos los modos | Pase de 950 V-Bucks que **devuelve hasta 1,500 V-Bucks** | Misiones diarias y semanales que no caducan hasta fin de temporada | V-Bucks (dura) | Quitó las llamas pagadas de pago a ciegas (2019) | **Multa de la FTC de USD 520 M (dic-2022):** 275 M por privacidad infantil (COPPA) y 245 M en reembolsos por *dark patterns* que provocaban compras accidentales |
| **Clash Royale** (Supercell) | Trophy Road + niveles de rey | Pass Royale (gratis + niveles de pago) | Misiones y eventos | Oro, gemas y comodines | Cofres con temporizador (modelo clásico) | En jun-2023 Supercell **tuvo que disculparse** por una actualización que encareció las mejoras de cartas: **el *pay-to-win* enoja a la comunidad** |
| **Pokémon Unite** (TiMi/TPC) | Nivel de entrenador, licencias | Pase premium | Login diario y misiones | Monedas Aeos (blanda, **con tope semanal**), gemas Aeos (dura) | Sorteos de "Energía" | En 2021 lo criticaron por vender **potenciadores de objetos** con gemas, una ventaja pagada. Evitar los topes semanales de moneda que frustran |

### 6.2 Patrones comunes que sí funcionan

1. **Pase de temporada barato con vía gratis generosa.** Es el producto que más venden todos estos juegos.
2. **Bono de primeras victorias del día.** Da una razón para jugar a diario sin castigar a quien juega poco.
3. **Misiones semanales que no caducan** hasta fin de temporada. Así quien falta unos días puede ponerse al corriente y no se siente castigado.
4. **Tienda directa con precio visible** (sabes exactamente qué compras) en lugar de cajas.
5. **Recompensas aleatorias, si las hay, gratis y ganadas jugando**, con probabilidades publicadas (los Starr Drops de Brawl Stars).
6. **Cosméticos con identidad** (colaboraciones). Los picos de Free Fire en México coinciden con colaboraciones ✅ ([Sensor Tower](https://sensortower.com/blog/2025-q4-android-top-5-third-person-games-units-mx-60214330241bc16eb89ce15d), vía reporte 02). Aquí entran los diseños reales de INKCAP.

### 6.3 Por qué evitar las cajas botín pagadas

- **Regulación:** Apple y Google obligan a publicar probabilidades ✅/⚠️. Bélgica las trata como apuesta desde 2018 ⚠️ y Brasil las prohíbe para menores desde 2026 ⚠️. Con muñequitos caricaturescos, el público incluirá niños.
- **Reputación:** Brawl Stars mejoró al quitarlas ✅. Una marca de ropa que colabora con artistas no quiere asociarse a "apuestas para niños".
- **Negocio:** un traje que **representa una playera real** vale más si se compra **directo**, porque el jugador sabe qué está comprando y puede querer la playera física.
- **Alternativa:** "Latas de spray" **gratis** (ver 9.8) con probabilidades publicadas, sin duplicados inútiles y **sin venderlas nunca**.

### 6.4 Economía sin *pay-to-win*: reglas

1. **Todo lo que se vende es estético.** Todos los muñequitos tienen las mismas estadísticas, como en *Boomerang Fu*. El objeto arrojadizo cambia de **aspecto**, nunca de alcance ni de daño.
2. **La moneda dura no compra poder ni tiempo de juego competitivo.** Sirve para el pase, trajes y emotes.
3. **El jugador gratuito recibe un goteo de moneda dura** (≈ 150 al mes, ver 9.1) para que pueda comprar un pase cada pocos meses sin pagar.
4. **Precios redondos y claros en MXN**, confirmación antes de cada compra, sin botones engañosos (lección de la FTC con Epic) y nada de temporizadores falsos.
5. **Monitorear fuentes y sumideros** de cada moneda (cuánto entra y cuánto sale al día) para evitar la inflación.

### 6.5 Retención: los números para saber si vamos bien ✅

| Percentil (GameAnalytics, datos 2025) | D1 | D7 | D30 |
|---|---|---|---|
| Mediana | ~22 % | ~4 % | ~0.7 % |
| 10 % mejor | ~40 % | ~12 % | — |
| 1 % mejor | 64–68 % | — | 13–15 % |
| **Nuestra meta para escalar** | **≥ 40 %** | **≥ 15 %** | **≥ 5 %** |

Fuentes: [GameDev Reports](https://gamedevreports.substack.com/p/gameanalytics-mobile-and-pc-game) e [InvestGame](https://investgame.net/news/pdf/2026-01-20-mobile_retention_benchmarks_2026/) (vía reporte 02). **La progresión no salva un juego aburrido:** primero hay que lograr que la partida sea divertida y solo después pulir la progresión.

---

## 7. LiveOps: operar el juego cada semana

### 7.1 Qué controlar con configuración remota

- XP y monedas por partida, multiplicadores de fin de semana ("doble XP").
- Rotación de la tienda, precios y ofertas.
- Fechas de inicio y fin de eventos y temporadas, en UTC.
- Balance del juego: velocidad del objeto, tiempo de regreso, duración de los *power-ups*.
- **Interruptores de emergencia** para apagar un modo o la tienda si hay un error.
- **Versión mínima** de la app, para obligar a actualizar.

### 7.2 Pruebas A/B que valen la pena

| Prueba | Variante A | Variante B | Métrica |
|---|---|---|---|
| Recompensa del día 1 | 150 Gotas | Un *sticker* exclusivo | D1 |
| Tutorial | 3 pasos | Directo a una partida contra bots | D1 y partidas en la primera sesión |
| Precio del pase | 450 Tinta Dorada (~89 MXN) | 400 (~79 MXN) | Conversión e ingreso por usuario |
| Momento de pedir ligar la cuenta | Nivel 3 | Primera victoria | % de cuentas ligadas |

🧮 **Tamaño de muestra:** para detectar que el D1 sube de 35 % a 38 % con confianza estadística (95 % de confianza, 80 % de potencia) hacen falta **≈ 4,000 jugadores nuevos por variante**. No tiene caso hacer A/B con 300 jugadores.

### 7.3 Eventos de analítica mínimos

`instalacion`, `tutorial_paso`, `tutorial_fin`, `partida_inicio` (modo, arena, local u online, número de jugadores), `partida_fin` (puesto, KO, duración), `nivel_sube`, `recompensa_diaria_reclamada` (día de calendario, racha), `mision_completada`, `pase_nivel`, `tienda_vista`, `compra_virtual` (artículo, moneda, precio), `compra_real` (producto, MXN), `cuenta_ligada` (proveedor), `push_abierta`, `error_red`.

**KPIs semanales:** DAU, D1/D7/D30, partidas por día, duración de sesión, % que reclama la recompensa diaria, % que completa las misiones, conversión a pagador, ARPDAU (ingreso diario por jugador activo), ARPPU (ingreso por pagador), y el flujo de Gotas y Tinta Dorada.

### 7.4 Notificaciones push

- **Locales** (sin servidor): "Tu recompensa diaria está lista" y "Tu racha está por romperse". Usar **Unity Mobile Notifications**.
- **Remotas** (FCM): inicio de temporada, *drop* nuevo de INKCAP, evento de fin de semana.
- **Reglas:** máximo **1 al día**, nada de 22:00 a 9:00, pedir permiso **después de la primera victoria** y no al abrir el juego. En Android 13+ el permiso es explícito ⚠️. **Nunca dar premio por aceptarlas** ✅ (5.1.2(i)).

### 7.5 Calendario de eventos México + *drops* de INKCAP

| Mes | Evento | Idea en el juego |
|---|---|---|
| Ene | Reyes (6-ene) | Evento "Rosca": el muñequito que saque el "niño" en una ronda gana doble XP |
| Feb | San Valentín | Trajes de pareja, modo 2 vs 2 |
| Abr | Día del Niño (30-abr) | Recompensas gratis para todos, sin ofertas de pago |
| May–Jun | Hot Sale ⚠️ (las fechas cambian cada año) | Ofertas en el juego + cupón cruzado con la tienda en línea |
| Jul–Ago | Vacaciones | Doble XP los fines de semana, torneos de amigos por Wi-Fi |
| Sep | Fiestas Patrias | Estelas de tinta tricolor y emote de "Grito" |
| Oct–Nov | **Día de Muertos** | **Arena "Panteón del Arte"**, muñequitos-catrina *art toy*. Es el evento con más identidad mexicana |
| Nov | Buen Fin ⚠️ (las fechas cambian cada año) | Paquetes de Tinta Dorada con bono |
| Dic | Guadalupe–Reyes | Calendario de adviento de 12 días |
| Siempre | **"Estrenos" y "Edición Limitada"** de INKCAP | Cada lanzamiento físico sale el mismo día como traje digital (ver 9.9) |

### 7.6 Contenido nuevo sin actualizar la app

Los trajes nuevos pueden bajarse como **Addressables** desde un CDN (Unity Cloud Content Delivery u otro) ⚠️. Las mecánicas o modos nuevos sí deben pasar por revisión de la tienda (guía 2.5.19 ✅).

---

## 8. Stack de backend recomendado por etapa

### 8.1 Tabla de etapas y costos mensuales

> 🧮 Estimaciones con los supuestos de 8.2. ⚠️ Los precios de UGS, PlayFab y Nakama no pudieron confirmarse para 2026. **No incluye la red del juego online** (relay o servidores), que está en el reporte 04: ≈ USD 20, 260–470 y 2,800–4,900 al mes con Unity Relay para 1k, 10k y 100k DAU ✅.

| Etapa | Qué incluye | Servicios | 1k DAU | 10k DAU | 100k DAU |
|---|---|---|---|---|---|
| **0. Prototipo local (sin servidor)** | Partidas por Wi-Fi, XP y racha guardadas en el celular, compras en *sandbox* | Archivo local cifrado, Unity Mobile Notifications, **Crashlytics** (gratis) | **USD 0** | n/a (solo probadores) | n/a |
| **1. MVP con cuentas (recomendado)** | Invitado + Apple + Play Games, guardado en la nube, monedas, inventario, tienda, compras validadas, recompensas diarias, misiones, configuración remota, analítica | **UGS** (Auth, Cloud Save, Economy, Cloud Code C#, Remote Config, Leaderboards, Analytics) + **Firebase** (Crashlytics, FCM) + **Unity IAP 5** | **≈ USD 0** | **≈ USD 0–100** (MXN 0–1,900) ⚠️ | **≈ USD 300–1,500** (MXN 5,700–28,700) ⚠️ |
| **1-B. MVP alternativo: todo en Firebase** | Lo mismo, programando economía y pase en Cloud Functions | Auth (sin Identity Platform), Firestore, Functions, Remote Config, A/B, Analytics, Crashlytics, FCM, App Check | **≈ USD 0–5** (requiere Blaze) | **≈ USD 10–30** (MXN 190–570) | **≈ USD 150–400** (MXN 2,900–7,600) |
| **2. Online a escala** | Tablas por región, *ranked*, eventos complejos, segmentación, *crossplay* | Seguir con UGS si el costo cuadra, **o** Nakama propio + Hiro/Satori, **o** PlayFab | UGS igual que arriba · Nakama propio **USD 25–50** | Nakama **USD 80–250** | Nakama **USD 500–1,500** + persona de DevOps · PlayFab: por cotización ⚠️ |
| **Complementos** | — | RevenueCat (si hay suscripción), Sentry (opcional) | USD 0 | USD 0–26 | RevenueCat 1 % arriba de USD 2.5k al mes de ingreso; Sentry USD 26–80 |

**Costos fijos que no dependen de los jugadores ✅** ([reporte 03](./03-motor-de-juego-y-herramientas.md)): Apple Developer **USD 99 al año** y Google Play **USD 25 una sola vez**. La página web de privacidad y borrado de cuenta puede vivir en el sitio actual de INKCAP.

### 8.2 Supuestos del cálculo (Firebase, que es el que conozco con más detalle) 🧮

- Por jugador activo al día: ~5 partidas, ~20 lecturas, ~10 escrituras y ~8 llamadas a Functions. Perfil de ~20 KB. MAU ≈ 3.5 × DAU.
- **1k DAU:** 20k lecturas y 10k escrituras al día (dentro de la capa gratis), 240k invocaciones al mes (gratis) → **≈ USD 0**.
- **10k DAU:** ~4.5 M lecturas y ~2.4 M escrituras cobrables al mes → ≈ USD 3 + 4. Functions y almacenamiento ≈ USD 1. Se deja margen para registros y transferencia → **USD 10–30**.
- **100k DAU:** ~58 M lecturas (≈ USD 35), ~29 M escrituras (≈ USD 53), ~22 M invocaciones extra + cómputo (≈ USD 40), almacenamiento y transferencia (≈ USD 15–30). Con margen → **USD 150–400**.
- **UGS:** no pude ver las tarifas 2026. El rango sale de que cada servicio tiene capa gratuita y cobra por uso. **Hay que correr el [estimador oficial](https://unity-player-services-pricing-estimator.ds.unity3d.com/) con estos mismos supuestos antes de decidir.**
- **Contexto:** con 100k DAU, aun USD 1,500 al mes son **≈ USD 0.0005 por jugador al día**. Igual que con la red (reporte 04), **el backend no quiebra el proyecto; lo quiebra la retención.**

### 8.3 Por qué UGS como núcleo y no solo Firebase

| A favor de UGS | A favor de Firebase |
|---|---|
| Economía, inventario y validación de compras **ya hechas** | Capa gratis más conocida y estable |
| Cloud Code en **C#**, el mismo lenguaje del juego: la IA comparte modelos y reglas entre cliente y servidor | Crashlytics, FCM y Analytics, los mejores en su categoría, gratis |
| Mismo panel que Relay y Lobby (la etapa online del reporte 04) | Muchísima documentación: las IAs lo conocen muy bien |
| Plugin oficial de Unity para Claude Code ✅ | No depende de que Unity mantenga sus precios |

**Decisión:** UGS para la lógica del juego y Firebase para fallos y push. Si al revisar los precios 2026 UGS sale caro, se pasa al plan 1-B sin dolor, gracias a la capa de abstracción (regla 7).

---

## 9. Diseño de progresión propuesto

### 9.1 Monedas (solo dos, más la XP del pase)

| Moneda | Nombre propuesto | Cómo se gana | En qué se gasta |
|---|---|---|---|
| **Blanda** | **Gotas** (de tinta) | Jugando: 10–25 por partida, 50 por la primera victoria del día, misiones. ≈ **200 al día** | *Stickers*, colores, *sprays* (300–500), gorras básicas (1,500), playeras genéricas (3,000) |
| **Dura** | **Tinta Dorada** (TD) | Comprándola, más un goteo gratis de ≈ 150 al mes (calendario, niveles cada 5, pase gratis) | Pase premium, trajes de artista, gorras INKCAP, emotes, paquetes de colaboración |
| **Progreso de temporada** | **XP de pase** | Partidas, misiones, primera victoria del día | Sube niveles del pase; no se compra suelta |

**Paquetes de Tinta Dorada** (precios sugeridos en MXN; se usa el punto de precio de la tienda más cercano ⚠️):

| Paquete | TD | Precio | ≈ USD |
|---|---|---|---|
| Probadita | 80 | $19 MXN | 1.0 |
| Bolsa | 450 (+50 de regalo) | $89 MXN | 4.7 |
| Lata | 1,000 (+150) | $179 MXN | 9.4 |
| Cubeta | 2,200 (+400) | $349 MXN | 18.3 |

🧮 **Cuánto llega a INKCAP de un traje de artista de $179 MXN:** en México el precio de tienda ya incluye IVA ⚠️ → $154 sin IVA. Menos 15 % de comisión de la tienda (programa para pequeños desarrolladores) → **≈ $131**. Menos 15 % de regalía al artista sobre ese neto → **≈ $111 MXN para INKCAP**. Hay que confirmar con el contador cómo se calculan la comisión y las regalías.

### 9.2 XP por partida

| Concepto | XP |
|---|---|
| Terminar la partida | 20 |
| Cada KO (máximo 5 por partida) | 5 |
| 1.er / 2.º / 3.er lugar (partidas de 4 o más jugadores) | +25 / +15 / +10 |
| Jugar con tu *crew* (amigos o Wi-Fi local) | +5 |
| **Primera victoria del día** | **+100** (+50 Gotas) |
| **Bono de regreso**: si no jugaste ayer, tus primeras 3 partidas dan el doble | ×2 |
| 3 misiones diarias | 50 c/u |

Un **jugador típico** (15 minutos, ~5 partidas al día) gana **≈ 470 XP diarios**.

### 9.3 Tabla de XP por nivel de cuenta

Fórmula: **XP para pasar del nivel *n* al *n*+1 = 150 + 50 × (*n* − 1)**. Después del nivel 30, cada nivel de "Maestría" cuesta 2,000 XP fijos.

| Nivel | XP para el siguiente | XP acumulada para llegar | Días aprox. (470 XP/día) | Recompensa al llegar |
|---|---|---|---|---|
| 2 | 200 | 150 | 1.ª sesión | Emote "Saludo" |
| 3 | 250 | 350 | 1 | Color de tinta para el objeto · aviso "Protege tu progreso" |
| 4 | 300 | 600 | 1–2 | 200 Gotas |
| 5 | 350 | 900 | 2 | **Muñequito nuevo** + 25 TD |
| 6 | 400 | 1,250 | 3 | *Sticker* para el frisbee |
| 7 | 450 | 1,650 | 4 | 300 Gotas |
| 8 | 500 | 2,100 | 4–5 | Estela de tinta "Salpicón" |
| 9 | 550 | 2,600 | 5–6 | 300 Gotas |
| 10 | 600 | 3,150 | ~7 | **Gorra básica** + 25 TD + desbloquea misiones semanales |
| 12 | 700 | 4,400 | ~9 | Marco de perfil |
| 15 | 850 | 6,650 | ~14 | **Muñequito nuevo** + 25 TD |
| 18 | 1,000 | 9,350 | ~20 | Emote |
| 20 | 1,100 | 11,400 | ~24 | **Playera genérica** + 25 TD |
| 25 | 1,350 | 17,400 | ~37 | **Muñequito nuevo** + 25 TD |
| 30 | 1,550 | 24,650 | ~52 | **Traje "Veterano de Tinta"** + 50 TD → empieza la Maestría |

El ritmo da **un premio en cada nivel durante la primera semana**, nivel 10 en ~7 días, nivel 20 en ~1 mes y nivel 30 en ~2 meses.

### 9.4 Calendario de 7 días + racha suave

| Día | Recompensa |
|---|---|
| 1 | 150 Gotas |
| 2 | *Sticker* "Tag" para el objeto |
| 3 | 250 Gotas |
| 4 | 25 Tinta Dorada |
| 5 | Emote "Salpicada" |
| 6 | 1 Lata de *spray* (*drop* gratis) |
| 7 | **Gorra de la semana** (cambia cada semana; cosmética) |

- **Racha suave:** si faltas un día, el calendario **se pausa**; no regresa al día 1.
- **Contador de racha** aparte, por días seguidos con al menos 1 partida: +10 % de Gotas por día, hasta **+50 %**. **1 protector de racha gratis a la semana.** Insignias de perfil a los 7, 30 y 100 días.
- **Calendario mensual de 28 días** encima del semanal: al completarlo se gana la **"Playera del mes"**, un diseño real de INKCAP, gratis.

### 9.5 Misiones de ejemplo

**Diarias** (3 al azar de una lista, con 1 cambio gratis; 200 XP de pase + 30 Gotas cada una):
- Juega 3 partidas.
- Consigue 5 KO **con el regreso** del objeto, es decir, golpear de espaldas al rival cuando el frisbee vuelve.
- Atrapa tu objeto 10 veces.
- Gana 1 partida en la **Calle Grafiteada**.
- Recoge 3 *power-ups* de **Tinta Derramada**.
- Haz un KO doble.
- Juega 1 partida con un amigo, por Wi-Fi local u online.

**Semanales** (7, desbloqueadas al nivel 10; 1,000 XP de pase + 150 Gotas; **no caducan hasta el fin de temporada**):
- Gana 7 partidas.
- Juega en 3 arenas distintas (calle, museo, taller de serigrafía).
- Consigue 40 KO.
- Juega 5 partidas con tu *crew*.
- Completa 10 misiones diarias.
- Queda entre los 2 primeros en 10 partidas de 4 o más jugadores.
- Haz 3 KO con el *power-up* de **Bordado** (el que enlaza a los rivales).

**Reto del artista invitado** (5 retos por temporada diseñados con el artista; premio: su *sticker* y el marco "Firma del artista").

### 9.6 Pase de temporada (40 niveles, 6 semanas)

- **Duración:** 6 semanas. Cada temporada se dedica a una técnica de tinta: T1 Grafiti → T2 Tatuaje → T3 Serigrafía → T4 *Sticker* → T5 Bordado → T6 Tinta derramada. Así sale **un año de contenido con un hilo de marca**.
- **Precio:** premium = **450 TD (≈ $89 MXN)**, también vendible como compra directa.
- **Costo por nivel:** 1,250 XP de pase fijos (50,000 en total).
- **Fuentes de XP de pase:** 20 por partida, 150 por la primera victoria del día, 200 por misión diaria y 1,000 por misión semanal.
- **Ritmo 🧮:** quien juega a diario (≈ 850 al día + 5,000 por semana) **termina en ~4.6 semanas**. Quien juega 4 días a la semana llega al **nivel ~27–30**. Después del nivel 40 hay **niveles extra**: cada 2,500 XP dan 100 Gotas.

| Nivel | Vía gratis | Vía premium |
|---|---|---|
| 1 | 100 Gotas | **Traje de temporada (versión base)** |
| 5 | *Sticker* | Emote de temporada |
| 10 | 10 TD | *Spray* animado |
| 12 | 200 Gotas | **Gorra edición temporada** |
| 15 | Lata de *spray* | 100 TD |
| 20 | **Muñequito de temporada** | Estela de tinta de temporada |
| 25 | 200 Gotas | **Playera del artista invitado** (diseño real de INKCAP) |
| 30 | 15 TD | 100 TD + marco |
| 35 | Lata de *spray* | Emote "Firma" |
| 40 | Insignia de temporada | **Traje legendario de temporada** + 100 TD |
| Resto | Se alternan 100 Gotas y *sticker* | Se alternan 150 Gotas y *spray* |

- **Totales aproximados:** la vía gratis da ≈ 3,000 Gotas, 1 muñequito y 25 TD. La vía premium da 2 trajes completos, 1 gorra, 1 estela, 2 emotes, ≈ 5,000 Gotas y **≈ 300 TD de vuelta**. Eso devuelve dos tercios del precio y empuja a comprar el siguiente pase con un poco más.
- **Futuro (no en el MVP):** "Premium+" con 10 niveles de salto y un Pase de *Crew* para regalar a amigos.

### 9.7 Tienda rotativa

| Sección | Rotación | Moneda | Precios sugeridos |
|---|---|---|---|
| **Diaria** | 4 espacios cada 24 h | Gotas | 300–1,500 |
| **Destacados** | 2 espacios cada 48 h | TD | *Sticker* o *spray* 80–150 · emote 150–250 |
| **Drop INKCAP** | Semanal, sincronizado con "Estrenos" | TD | Gorra 300–450 (~$59–89) · playera de artista 600–900 (~$119–179) |
| **Colaboración / Edición Limitada** | Por evento | TD | Paquete traje + gorra + estela + emote: 1,200–1,500 (~$239–299) |

**Regla anti-FOMO:** los artículos "limitados" pueden **regresar en 6 a 12 meses**, salvo las recompensas de logro, como la insignia del pase. Así no se presiona a nadie con escasez falsa.

### 9.8 *Drops* gratuitos: "Latas de *spray*"

- Se ganan **solo jugando**: calendario, pase y niveles. **No se venden.**
- Contenido: Gotas (60 %), *sticker* o color (30 %), emote (8 %), traje (2 %). Las probabilidades se **muestran dentro del juego**.
- Si sale un **duplicado**, se convierte en Gotas. Hay "lástima garantizada": a las 25 latas sin traje, la siguiente lo trae seguro.

### 9.9 Merch físico ↔ traje digital: la ventaja única de INKCAP

1. Quien compra una playera o gorra en la tienda en línea de INKCAP (incluido pago en OXXO) recibe un **código**.
2. El código se canjea **en el sitio web de INKCAP**, con sesión de la cuenta del juego, y el traje aparece en el inventario. **No se canjea con QR dentro de la app**, porque Apple lo prohíbe (3.1.1) ✅.
3. Ese mismo traje **también se vende en la tienda del juego**, como exige la guía 3.1.3(b) ✅.
4. Alternativa 100 % oficial en iOS: **Offer Codes de Apple** para un artículo gratis ✅ (no pude verificar los límites de cantidad de códigos ⚠️).
5. Al revés también funciona: dentro del juego, un botón "Consigue la playera real" lleva a la tienda web. Vender bienes físicos **no paga comisión a Apple** ✅ (3.1.3(e)).

### 9.10 Tablas de clasificación

- **MVP:** tabla **semanal de México** por victorias en línea, tabla de **amigos** y *Muro de la Fama* de temporada (el top 100 aparece pintado en la arena del Museo).
- **Online maduro:** modo *ranked* con divisiones con nombres de técnica: Boceto → Línea → Color → Sombra → Obra Maestra. **Recompensas solo cosméticas.**
- Los puntajes los **calcula el servidor**. Las partidas locales por Wi-Fi **no cuentan**.
- Game Center (iOS) y Play Games (Android) pueden mostrar logros y tablas gratis como complemento ✅/⚠️ ([Game Center](https://developer.apple.com/game-center/)).

---

## 10. Riesgos principales

| Riesgo | Probabilidad | Mitigación |
|---|---|---|
| Trampas en XP y monedas (archivos editados, reloj adelantado) | Alta | Servidor autoritativo, hora del servidor, topes, App Check |
| Rechazo en App Store por cuenta o borrado | Media | Seguir las guías 4.8 y 5.1.1(v) al pie de la letra (sección 4) |
| Reembolsos fraudulentos | Media | Notificaciones de reembolso de Apple y Google → quitar el artículo |
| Cambio de precios o cierre del proveedor (como Multiplay y Hathora en 2026) | Media | Capa de abstracción y exportar datos con regularidad |
| Menores comprando sin permiso | Media | Rango de edad, confirmación de compra, sin ofertas agresivas a menores |
| Economía inflada o tacaña | Alta al inicio | Configuración remota + analítica de fuentes y sumideros + A/B |
| Factura sorpresa en Blaze o UGS | Baja | Alertas de presupuesto y límites de uso desde el primer día |

---

## 11. Recomendación para nuestro juego

1. **Etapa 0 (ahora):** prototipo local **sin backend**. Progreso en un archivo cifrado. **Crashlytics** desde la primera instalación de prueba. Todo el acceso a datos pasa por interfaces (`IPerfilService`, etc.) para cambiar de proveedor sin reescribir.
2. **Etapa 1 (MVP para tiendas):** **UGS** (Authentication con invitado, Apple y Play Games; Cloud Save; Economy; Cloud Code en C#; Remote Config con Game Overrides; Leaderboards; Analytics) + **Firebase** (Crashlytics, FCM) + **Unity IAP 5** con validación en servidor. **Antes de firmar, correr el estimador de UGS** con los supuestos de 8.2. Si sale caro, usar el plan 1-B (todo en Firebase).
3. **Cumplimiento desde el día 1:** cuenta de invitado, Sign in with Apple, borrado dentro de la app (también para invitados) con página web para Google, revocación de tokens de Apple, aviso de privacidad bajo la ley mexicana de 2025, rango de edad e inscripción al **Small Business Program** de Apple (15 %).
4. **Progresión del MVP:** nivel de cuenta (tabla 9.3), primera victoria del día, calendario de 7 y 28 días con racha suave, 3 misiones diarias y 7 semanales, tienda rotativa y Latas gratuitas. **El pase de temporada sale con el lanzamiento suave**, cuando ya haya datos para calibrar.
5. **Monetización 100 % cosmética:** dos monedas, paquetes de $19 a $349 MXN, pase de ≈ $89 MXN y trajes de artista de $119 a $179 MXN. **Nunca cajas botín pagadas.**
6. **Diferenciador INKCAP:** cada "Estreno" físico tiene un gemelo digital canjeable en la web, temporadas por técnica de tinta y un artista invitado por temporada con sus retos, en lugar de cajas.
7. **LiveOps realistas para un equipo pequeño:** temporadas de 6 semanas, 1 evento al mes del calendario mexicano y 1 *drop* por semana. Todo se controla con configuración remota para no depender de actualizaciones.
8. **Medir antes de escalar:** no invertir en anuncios hasta tener **D1 ≥ 40 % y D7 ≥ 15 %** en el lanzamiento suave. Hacer A/B solo con ~4,000 jugadores nuevos por variante o más.
9. **Etapa 2 (online a escala, >50–100k DAU):** volver a comparar UGS con Nakama + Hiro/Satori (alojado por nosotros o en Heroic Cloud) y con PlayFab, según los costos reales y la necesidad de eventos complejos.

---

## 12. Dudas abiertas: qué verificar antes de comprometer dinero

1. **Precios y límites gratuitos 2026 de UGS** (Cloud Save, Economy, Cloud Code, Leaderboards, Analytics) y si el servicio de **Push de UGS** sigue activo.
2. **PlayFab:** si el Development Mode sigue siendo gratis hasta 100k jugadores y cuál es la cuota base 2026 para producción.
3. **Firebase:** tarifas vigentes de Firestore y Functions en la región elegida (¿hay región en México?), y la política de Auth con Identity Platform.
4. **Heroic Cloud, LootLocker, Beamable, Metaplay y AccelByte:** planes y precios 2026.
5. **Google Play:** texto vigente del requisito de borrado de cuenta y la versión obligatoria de Play Billing Library para 2026–2027.
6. **Límites de los Offer Codes de Apple** (cuántos códigos por trimestre) para la promoción merch → traje.
7. **Fiscal:** cómo se calculan la comisión de las tiendas, el IVA y la regalía del 15 % en compras digitales, y si aplica el IEPS a videojuegos.
8. **Brasil y otras leyes de edad** si el lanzamiento suave incluye LATAM fuera de México.
9. **Sistemas actuales** de Brawl Stars, Clash Royale y Stumble Guys: cambian cada pocos meses.

---

## 13. Fuentes

**Leídas directamente hoy (23-nov-2026) ✅**
- Apple – App Review Guidelines (4.8, 5.1.1(v), 3.1.1, 3.1.3(b), 3.1.3(e), 3.2.2(x), 5.1.2(i), 4.5.4): https://developer.apple.com/app-store/review/guidelines/
- Apple – Offering account deletion in your app: https://developer.apple.com/support/offering-account-deletion-in-your-app/
- Apple – Developer News (clasificaciones de edad, Declared Age Range API, Texas SB 2420, Offer Codes, guía 2.5.19, dominio de Sign in with Apple): https://developer.apple.com/news/
- Apple – App Store Small Business Program: https://developer.apple.com/app-store/small-business-program/
- Apple – Game Center: https://developer.apple.com/game-center/

**Verificadas por los reportes 02, 03 y 04 de esta investigación ✅**
- Retención (GameAnalytics 2025): https://gamedevreports.substack.com/p/gameanalytics-mobile-and-pc-game · https://investgame.net/news/pdf/2026-01-20-mobile_retention_benchmarks_2026/
- Brawl Stars 2024 (pase, economía, sin cajas): https://www.deconstructoroffun.com/blog/2024/2/11/brawl-stars-to-the-moon · https://gamingonphone.com/news/brawl-stars-revenue-jumped-to-almost-9x-and-mau-to-2-4x-in-2024-compared-to-last-year/
- Stumble Guys e influencers: https://gameanalytics.com/?p=17893
- Free Fire y colaboraciones en México: https://sensortower.com/blog/2025-q4-android-top-5-third-person-games-units-mx-60214330241bc16eb89ce15d
- RevenueCat, precios: https://revenuecat.com/pricing
- UGS – precios (Relay) y estimador: https://unity.com/products/gaming-services/pricing · https://unity-player-services-pricing-estimator.ds.unity3d.com/
- Fin de Unity Multiplay: https://discussions.unity.com/t/end-of-services/1698428
- PlayFab, página de precios: https://developer.microsoft.com/en-us/games/products/playfab/pricing/
- Tipo de cambio: https://dolardof.com/

**Oficiales para verificar ⚠️ (no se pudieron abrir en esta sesión)**
- Firebase pricing: https://firebase.google.com/pricing
- PlayFab pricing (anterior): https://playfab.com/pricing/
- Unity Gaming Services, documentación: https://docs.unity.com/ugs/
- Heroic Labs (Nakama, Hiro, Satori): https://heroiclabs.com/pricing/
- LootLocker: https://lootlocker.com/pricing
- Beamable: https://beamable.com/pricing
- Supabase: https://supabase.com/pricing
- Metaplay: https://metaplay.io/pricing
- AccelByte: https://accelbyte.io/pricing
- GameAnalytics: https://gameanalytics.com/pricing
- Sentry: https://sentry.io/pricing/
- Backtrace: https://backtrace.io/pricing
- Google Play – borrado de cuenta: https://support.google.com/googleplay/android-developer/answer/13327111
- Google Play Games Services v2: https://developer.android.com/games/pgs/overview
- FTC – acuerdo con Epic Games (2022): https://www.ftc.gov/news-events/news/press-releases/2022/12/fortnite-video-game-maker-epic-games-pay-more-half-billion-dollars-over-ftc-allegations
