# Intuit Clientes API - Challenge .NET

## 🚀 Arquitectura y Decisiones de Diseño
Se aplicó una arquitectura por capas siguiendo principios **SOLID** para garantizar la escalabilidad y mantenibilidad:

* **Repository Pattern:** Para desacoplar la lógica de acceso a datos.
* **Service Layer:** Donde reside la lógica de negocio, aislada de los controladores.
* **Pattern Matching & Functional Error Handling:** Uso de `OneOf` para evitar el flujo basado en excepciones y mejorar la legibilidad.
* **Performance:** Implementación de `AsNoTracking()` en consultas de solo lectura y uso de **Stored Procedures** para búsquedas optimizadas.
* **Observabilidad:** Logging estructurado con **Serilog** y métricas de tiempo de ejecución con `Stopwatch`.

## 🛠️ Tecnologías utilizadas
* .NET 6 / C#
* Entity Framework Core (PostgreSQL)
* AutoMapper (Mapeo de DTOs)
* FluentValidation (Validaciones de negocio)
* NUnit & Moq (Unit Testing)
* SonarCloud (Calidad de código)
* GitHub Actions

## 🧪 Tests
Para ejecutar las pruebas unitarias:
`dotnet test`