# 📚 Biblioteca Digital API

Proyecto desarrollado con **ASP.NET Core Web API (.NET 10)** para gestionar el catálogo de libros y sus préstamos.

Incluye una API REST, pruebas automatizadas, análisis de calidad de código y un portal web estático publicado mediante GitHub Pages.

## 🌐 Portal web publicado

**Visitar Biblioteca Digital:**

https://thiagogo-z.github.io/BibliotecaDigital-Thiago/

El portal está desarrollado con HTML, CSS y JavaScript y permite:

- Consultar un catálogo de libros de ejemplo.
- Buscar libros por texto.
- Filtrar libros según su disponibilidad.
- Consultar información sobre los endpoints de la API REST.
- Conocer los procesos de automatización del proyecto.

**Importante:** El portal es una demostración estática. GitHub Pages no ejecuta la API ASP.NET Core ni proporciona acceso a una base de datos.

## 🛠️ Tecnologías utilizadas

- C# y .NET 10.
- ASP.NET Core Web API.
- HTML, CSS y JavaScript.
- xUnit y Moq.
- WebApplicationFactory para pruebas de integración.
- Playwright para pruebas E2E.
- Git y GitHub.
- GitHub Actions.
- SonarQube Cloud.
- GitHub Pages.

## 🧪 Pruebas automatizadas

El proyecto dispone de pruebas para comprobar el correcto funcionamiento de la aplicación.

Se incluyen pruebas unitarias, pruebas de integración y pruebas E2E.

Las pruebas se ejecutan automáticamente mediante GitHub Actions para detectar errores antes de incorporar cambios a la rama principal.

## 🔍 Calidad del código

El proyecto utiliza **SonarQube Cloud** para analizar la calidad del código.

Se comprueban aspectos como:

- Seguridad.
- Fiabilidad.
- Mantenibilidad.
- Cobertura de pruebas.
- Duplicación de código.

El Quality Gate permite comprobar si el proyecto cumple los criterios de calidad establecidos.

## ⚙️ Automatización con GitHub Actions

El repositorio dispone de tres workflows en `.github/workflows/`.

### 1. CI - Pruebas automatizadas

**Archivo:** `tests.yml`

Compila el proyecto y ejecuta las pruebas automatizadas para detectar posibles errores.

### 2. SonarQube Cloud - Análisis de calidad

**Archivo:** `sonar-quality.yml`

Ejecuta el análisis de calidad de código y verifica el resultado del Quality Gate.

### 3. Deploy - GitHub Pages

**Archivo:** `deploy-pages.yml`

Automatiza la publicación del portal web.

El workflow realiza dos trabajos:

1. **Preparar sitio web:** comprueba los archivos HTML, CSS y JavaScript y genera el artefacto de publicación.
2. **Publicar en GitHub Pages:** despliega el artefacto cuando los cambios llegan a la rama `main`.

En los Pull Requests se valida el portal, pero no se publica.

También permite iniciar una publicación manual mediante `workflow_dispatch`.

## ✅ Pruebas de despliegue realizadas

Durante la actividad se han comprobado los siguientes escenarios:

- Publicación inicial del portal web.
- Actualización automática después de integrar cambios en `main`.
- Validación del sitio durante los Pull Requests.
- Detección de errores al faltar `site/index.html`.
- Detección de errores al utilizar una ruta incorrecta para el artefacto.
- Recuperación del workflow después de corregir los errores.
- Ejecución manual del despliegue desde GitHub Actions.

## 📁 Estructura principal del proyecto

```text
BibliotecaDigitalE2E/
├── .github/
│   └── workflows/
│       ├── tests.yml
│       ├── sonar-quality.yml
│       └── deploy-pages.yml
├── site/
│   ├── index.html
│   ├── styles.css
│   └── app.js
├── src/
├── tests/
├── DOCUMENTACION.md
└── README.md
```

## 📖 Documentación adicional

El archivo `DOCUMENTACION.md` contiene información sobre la API, las pruebas y la integración continua.

## 🚀 Resultado

El proyecto combina desarrollo de una API REST, pruebas automatizadas, análisis de calidad y despliegue continuo de un portal web.

GitHub Actions permite validar los cambios y publicar automáticamente nuevas versiones del portal en GitHub Pages.