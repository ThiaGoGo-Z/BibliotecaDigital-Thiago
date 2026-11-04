
# Biblioteca Digital API

## Descripción del proyecto

API REST desarrollada con ASP.NET Core y .NET 10 para gestionar
libros y préstamos de una biblioteca.

Los datos se almacenan en memoria, por lo que no necesita MySQL.

## Pruebas automatizadas

El proyecto cuenta inicialmente con 14 pruebas:

- 5 pruebas unitarias con xUnit y Moq.
- 7 pruebas de integración con WebApplicationFactory.
- 2 pruebas E2E con peticiones HTTP reales.

## Integración continua

Utilizamos GitHub Actions para automatizar:

1. La restauración de dependencias NuGet.
2. La compilación de la solución.
3. La ejecución de las pruebas unitarias.
4. La ejecución de las pruebas de integración.
5. El arranque de la API y las pruebas E2E.
6. La generación de informes TRX.

## Objetivo

Detectar errores automáticamente y comprobar que los cambios
funcionan antes de integrarlos en la rama principal.
