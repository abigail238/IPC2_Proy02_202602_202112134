using IPC2_Proy02_202602_202112134.Estructuras;
using IPC2_Proy02_202602_202112134.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// ============================================
// PRUEBA TEMPORAL DEL ARBOL DE LIBROS
// ============================================

ArbolLibro arbolPrueba = new ArbolLibro();

Libro libro1 = new Libro(
    500,
    "Clean Code",
    "Robert C. Martin",
    "Programacion"
);

Libro libro2 = new Libro(
    200,
    "El Principito",
    "Antoine de Saint-Exupery",
    "Literatura"
);

Libro libro3 = new Libro(
    800,
    "Redes de Computadoras",
    "Andrew Tanenbaum",
    "Redes"
);

Libro libro4 = new Libro(
    100,
    "Fisica Universitaria",
    "Sears y Zemansky",
    "Fisica"
);

Libro libro5 = new Libro(
    300,
    "Cien Anios de Soledad",
    "Gabriel Garcia Marquez",
    "Literatura"
);


// Insertamos los libros
arbolPrueba.Insertar(libro1);
arbolPrueba.Insertar(libro2);
arbolPrueba.Insertar(libro3);
arbolPrueba.Insertar(libro4);
arbolPrueba.Insertar(libro5);


// Mostramos en orden
Console.WriteLine("===== LIBROS EN ORDEN =====");
arbolPrueba.MostrarEnOrden();


// Buscar libro
Console.WriteLine();
Console.WriteLine("===== BUSCAR ISBN 300 =====");

Libro? encontrado = arbolPrueba.Buscar(300);

if (encontrado != null)
{
    Console.WriteLine("Libro encontrado:");
    Console.WriteLine("ISBN: " + encontrado.ISBN);
    Console.WriteLine("Titulo: " + encontrado.Titulo);
}
else
{
    Console.WriteLine("Libro no encontrado");
}


// Mostrar menor
Console.WriteLine();
Console.WriteLine("===== ISBN MENOR =====");

Libro? menor = arbolPrueba.ObtenerMenor();

if (menor != null)
{
    Console.WriteLine(
        "Menor ISBN: " + menor.ISBN +
        " - " + menor.Titulo
    );
}


// Mostrar mayor
Console.WriteLine();
Console.WriteLine("===== ISBN MAYOR =====");

Libro? mayor = arbolPrueba.ObtenerMayor();

if (mayor != null)
{
    Console.WriteLine(
        "Mayor ISBN: " + mayor.ISBN +
        " - " + mayor.Titulo
    );
}


// Eliminar libro
Console.WriteLine();
Console.WriteLine("===== ELIMINAMOS ISBN 200 =====");

arbolPrueba.Eliminar(200);


// Mostrar de nuevo
Console.WriteLine();
Console.WriteLine("===== ARBOL DESPUES DE ELIMINAR =====");

arbolPrueba.MostrarEnOrden();



// ============================================
// PRUEBA TEMPORAL DEL ARBOL DE CATEGORIAS
// ============================================

Console.WriteLine();
Console.WriteLine("===== PRUEBA DE CATEGORIAS =====");

ArbolCategoria arbolCategorias = new ArbolCategoria();


// Categorías principales
arbolCategorias.AgregarCategoria("Tecnologia");
arbolCategorias.AgregarCategoria("Literatura");
arbolCategorias.AgregarCategoria("Ciencia");


// Subcategorías de Tecnología
arbolCategorias.AgregarCategoria(
    "Redes",
    "Tecnologia"
);

arbolCategorias.AgregarCategoria(
    "Programacion",
    "Tecnologia"
);

arbolCategorias.AgregarCategoria(
    "Bases de Datos",
    "Tecnologia"
);


// Subcategorías de Programación
arbolCategorias.AgregarCategoria(
    "Java",
    "Programacion"
);

arbolCategorias.AgregarCategoria(
    "CSharp",
    "Programacion"
);


// Subcategorías de Ciencia
arbolCategorias.AgregarCategoria(
    "Fisica",
    "Ciencia"
);

arbolCategorias.AgregarCategoria(
    "Quimica",
    "Ciencia"
);


// Mostramos toda la estructura
Console.WriteLine();
Console.WriteLine("===== ESTRUCTURA COMPLETA =====");

arbolCategorias.MostrarEstructura();


// Mostramos desde una categoría determinada
Console.WriteLine();
Console.WriteLine("===== DESDE TECNOLOGIA =====");

arbolCategorias.MostrarDesde("Tecnologia");



// Construimos la aplicación web
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
