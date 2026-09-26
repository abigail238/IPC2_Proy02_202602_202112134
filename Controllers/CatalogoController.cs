using Microsoft.AspNetCore.Mvc;
using IPC2_Proy02_202602_202112134.Servicios;

namespace IPC2_Proy02_202602_202112134.Controllers;

// Conserva las rutas anteriores y utiliza las operaciones vigentes del sistema.
[AutoValidateAntiforgeryToken]
public class CatalogoController(Catalogo catalogo) : Controller
{
    [HttpGet]
    public IActionResult Index() => RedirectToAction("Index", "Home");

    [HttpPost]
    public IActionResult AgregarCategoria(string? nombre, string? padre)
    {
        lock (catalogo.Candado)
            TempData["Mensaje"] = catalogo.Categorias.AgregarCategoria(nombre ?? "", padre)
                ? "Categoría agregada." : "Revisa el nombre: debe ser único y su padre debe existir.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult RegistrarLibro(long isbn, string? titulo, string? autor, string? categoria)
    {
        lock (catalogo.Candado)
            TempData["Mensaje"] = catalogo.Registrar(isbn, titulo, autor, categoria);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult EliminarLibro(long isbn)
    {
        lock (catalogo.Candado)
            TempData["Mensaje"] = catalogo.Libros.Eliminar(isbn)
                ? "Libro eliminado." : "No existe un libro con ese ISBN.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult ConsultarISBN(string? tipo, long? isbn) =>
        RedirectToAction("Index", "Home", new
        {
            isbn,
            extremo = tipo == "menor" || tipo == "mayor" ? tipo : null
        });

    [HttpGet]
    public IActionResult ReporteCategorias(string? subcategoria) =>
        RedirectToAction("Reporte", "Home", new { tipo = "categorias", categoria = subcategoria });

    [HttpGet]
    public IActionResult ReporteAVL() =>
        Redirect(Url.Action("Index", "Home") + "#graphviz");
}
