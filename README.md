# Librería — IPC2 Proyecto 2

Aplicación ASP.NET Core MVC (.NET 10) con TDA propios: AVL por ISBN y árbol
de categorías mediante primer hijo y siguiente hermano.

## Ejecutar

Requisitos: SDK .NET 10 y Graphviz (comando dot).
En Windows también se detecta C:\Program Files\Graphviz\bin\dot.exe.
Si está en otra carpeta, configura Graphviz:Ruta en appsettings.json.

Desde la carpeta del proyecto:

```powershell
dotnet build
dotnet run
```

Abre en el navegador la dirección indicada en la consola.
El catálogo inicia vacío. Carga Archivos/Entrada.xml y luego Archivos/Entrada2.xml
para probar seis libros y nueve categorías en total.

## Verificar

```powershell
dotnet run --project Pruebas/Pruebas.csproj
```

Incluye 100,000 libros, invariantes AVL, eliminación, carga XML incremental,
validaciones, paginación y generación SVG real. Requiere Graphviz.

## Alcance

Los datos viven en memoria y se comparten entre todas las visitas a la aplicación.
Reiniciar el proceso los elimina. El filtro de categoría muestra libros asignados
directamente a ella; el reporte jerárquico incluye sus descendientes.

La consulta directa por ISBN es O(log n). Los filtros de título y categoría son O(n).
La búsqueda de categorías es O(c). No se usan colecciones integradas para almacenar
libros ni categorías. Los tipos de ASP.NET y XML se utilizan para interfaz y lectura.

Las releases y colaboradores de GitHub deben revisarse en el repositorio remoto.
No se crean releases retroactivas ni se modifica el historial existente.

