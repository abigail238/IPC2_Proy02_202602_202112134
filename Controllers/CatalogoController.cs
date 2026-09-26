using Microsoft.AspNetCore.Mvc;
using IPC2_Proy02_202602_202112134.Estructuras;
using IPC2_Proy02_202602_202112134.Models;

namespace IPC2_Proy02_202602_202112134.Controllers
{
    public class CatalogoController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.EstructuraCategorias = CargaController.JerarquiaCategorias.MostrarEstructura();
            return View();
        }

        [HttpPost]
        public IActionResult AgregarCategoria(string nombre, string padre)
        {
            if (!string.IsNullOrEmpty(nombre))
            {
                CargaController.JerarquiaCategorias.InsertarOCategorizar(nombre, padre);
                TempData["Exito"] = $"Categoría '{nombre}' agregada exitosamente.";
            }
            else
            {
                TempData["Error"] = "El nombre de la categoría no puede estar vacío.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RegistrarLibro(long isbn, string titulo, string autor, string categoria)
        {
            if (isbn <= 0 || string.IsNullOrEmpty(titulo) || string.IsNullOrEmpty(categoria))
            {
                TempData["Error"] = "Todos los campos son obligatorios y el ISBN debe ser válido.";
                return RedirectToAction("Index");
            }

            Libro nuevo = new Libro(isbn, titulo, autor, categoria);

            CargaController.LibrosGlobales.Insertar(nuevo);

            NodoCategoria nodoCat = CargaController.JerarquiaCategorias.Buscar(categoria);
            if (nodoCat == null)
            {
                nodoCat = CargaController.JerarquiaCategorias.InsertarOCategorizar(categoria);
            }
            nodoCat.Libros.Insertar(nuevo);

            TempData["Exito"] = $"Libro '{titulo}' registrado con éxito.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EliminarLibro(long isbn)
        {
            Libro hallado = CargaController.LibrosGlobales.Buscar(isbn);
            if (hallado != null)
            {
                CargaController.LibrosGlobales.Eliminar(isbn);

                NodoCategoria nodoCat = CargaController.JerarquiaCategorias.Buscar(hallado.Categoria);
                if (nodoCat != null)
                {
                    nodoCat.Libros.Eliminar(isbn);
                }

                TempData["Exito"] = $"El libro con ISBN {isbn} fue eliminado correctamente.";
            }
            else
            {
                TempData["Error"] = $"No se encontró ningún libro con ISBN {isbn}.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ConsultarISBN(string tipo, long? isbn)
        {
            if (tipo == "menor")
            {
                Libro menor = CargaController.LibrosGlobales.ObtenerMenor(); // 3 pts
                ViewBag.Resultado = menor != null ? $"Menor ISBN: {menor.ISBN} - '{menor.Titulo}' por {menor.Autor}" : "El catálogo está vacío.";
            }
            else if (tipo == "mayor")
            {
                Libro mayor = CargaController.LibrosGlobales.ObtenerMayor(); // 3 pts
                ViewBag.Resultado = mayor != null ? $"Mayor ISBN: {mayor.ISBN} - '{mayor.Titulo}' por {mayor.Autor}" : "El catálogo está vacío.";
            }
            else if (tipo == "buscar" && isbn.HasValue)
            {
                Libro buscado = CargaController.LibrosGlobales.Buscar(isbn.Value); // 4 pts
                ViewBag.Resultado = buscado != null
                    ? $"Libro Encontrado: ISBN {buscado.ISBN} | Título: {buscado.Titulo} | Autor: {buscado.Autor} | Categoría: {buscado.Categoria}"
                    : $"No existe ningún libro registrado con ISBN {isbn.Value}.";
            }

            ViewBag.EstructuraCategorias = CargaController.JerarquiaCategorias.MostrarEstructura();
            return View("Index");
        }

        [HttpGet]
        public IActionResult ReporteCategorias(string subcategoria)
        {
            string dot = CargaController.JerarquiaCategorias.GenerarGraphviz(subcategoria);
            ViewBag.CodigoDot = dot;
            ViewBag.Subcategoria = subcategoria;
            return View();
        }

        [HttpGet]
        public IActionResult ReporteAVL()
        {
            string dot = CargaController.LibrosGlobales.GenerarGraphviz();
            ViewBag.CodigoDot = dot;
            return View();
        }
    }
}