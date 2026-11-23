# GORRAZO by INKCAP

**Nombre de desarrollo:** Boomerang Fighters · **Nombre propuesto:** GORRAZO by INKCAP (falta la búsqueda de marca en el IMPI)

Un *party brawler* para celular de 2 a 6 jugadores. Muñequitos estilo art toy pelean en escenarios de arte urbano, y el arma de cada uno es **su propia gorra**: la lanzas, vuela en curva y regresa sola. Un golpe noquea y deja una mancha de tinta. Los cosméticos son gorras y playeras reales de INKCAP diseñadas por artistas. Se juega primero en el mismo Wi‑Fi y después en línea.

## Qué hay en este repositorio

| Carpeta | Contenido | Estado |
|---|---|---|
| [`docs/GDD.md`](docs/GDD.md) | Documento de diseño completo: reglas, controles, 8 muñequitos, 11 arenas, modos, latas de técnica, progresión, economía, phygital, pantallas, arte, arquitectura y hoja de ruta | Listo para revisar |
| [`docs/PLAN-DE-NEGOCIO-Y-COSTOS.md`](docs/PLAN-DE-NEGOCIO-Y-COSTOS.md) | Costos por etapa, costo de jugar en línea a 1k/10k/100k jugadores diarios, monetización, impuestos, proyección a 24 meses, lanzamiento, KPIs y plan de 90 días | Listo para revisar |
| [`docs/investigacion/`](docs/investigacion/) | 8 reportes con fuentes (Boomerang Fu, competidores y casos de éxito, motor, multijugador, backend, monetización, legal, arte) + síntesis. Los precios y las políticas se re-verificaron | Completo |
| [`docs/diseno/`](docs/diseno/) | 3 propuestas de concepto independientes y la evaluación de 2 jueces | Completo |
| [`docs/concepto/`](docs/concepto/) | Página visual del concepto para compartir con el equipo | Publicada |
| [`prototipo/`](prototipo/) | Prototipo jugable en el navegador (Three.js): 3 arenas, bots, 6 latas, táctil, XP, premio diario, vestidor y canje simulado | Jugable |
| [`unity/`](unity/) | Núcleo del juego para Unity 6: simulación C# determinista, bots, red local por Wi‑Fi, vistas greybox y 21 pruebas automáticas | Base para el desarrollo |

## Probarlo

- **Prototipo:** abre `prototipo/dist/index.html` en cualquier navegador (funciona sin internet y en el celular). Para reconstruirlo: `cd prototipo && npm install && npm run build`. Pruebas en navegador: `npm test`.
- **Unity:** ver [`unity/README.md`](unity/README.md). Pruebas sin Unity: `cd unity/Tests~/SimTests && dotnet test`.

## Decisiones pendientes del cliente

1. Autorizar la búsqueda y el registro del nombre ante el IMPI (GORRAZO by INKCAP, con INKAZO de respaldo).
2. Definir qué empresa publica y tramitar el D‑U‑N‑S (tarda hasta 28 días).
3. Aprobar el presupuesto de la Etapa 0, "prueba de diversión" (≈ MXN 238 mil), con la regla de no pagar arte final hasta pasarla.
4. Aprobar que lo local salga como beta cerrada y que el lanzamiento público ya traiga juego en línea.
5. Definir la regalía digital para los artistas (15 % del neto, o igualar la de la merch física).

Detalle en el GDD §17 y en el plan de negocio §12.
