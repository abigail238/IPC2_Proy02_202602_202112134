using System.Diagnostics;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using IPC2_Proy02_202602_202112134.Models;
using IPC2_Proy02_202602_202112134.Servicios;

namespace IPC2_Proy02_202602_202112134.Controllers;

[AutoValidateAntiforgeryToken]
public class HomeController(Catalogo catalogo, GeneradorGraphviz graficos) : Controller
{
    public IActionResult Index(string? categoria, string? titulo, int pagina = 1, string? isbn = null, string? extremo = null)
    {
        lock (catalogo.Candado)
        {
            var modelo = catalogo.ObtenerPagina(categoria, titulo, pagina);
            modelo.Mensaje = TempData["Mensaje"] as string ?? "";
            if (isbn != null || extremo != null)
            {
                modelo.Resultado = extremo switch
                {
                    "menor" => catalogo.Libros.ObtenerMenor(),
                    "mayor" => catalogo.Libros.ObtenerMayor(),
                    _ => long.TryParse(isbn, out long numero) && numero > 0 ? catalogo.Libros.Buscar(numero) : null
                };
                if (modelo.Resultado == null) modelo.Mensaje = "No se encontró el libro solicitado.";
            }
            return View(modelo);
        }
    }

    private IActionResult Volver(string mensaje)
    {
        TempData["Mensaje"] = mensaje;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [RequestSizeLimit(27 * 1024 * 1024)]
    public IActionResult Cargar(IFormFile? archivo)
    {
        if (archivo == null || archivo.Length == 0 || archivo.Length > 25 * 1024 * 1024)
            return Volver("Selecciona un archivo XML de hasta 25 MB.");
        try
        {
            using var stream = archivo.OpenReadStream();
            lock (catalogo.Candado)
                return Volver(new LectorXml().Cargar(stream, catalogo.Categorias, catalogo.Libros));
        }
        catch (XmlException) { return Volver("El archivo XML no es válido. Revisa su estructura."); }
        catch (IOException) { return Volver("No se pudo leer el archivo XML."); }
    }

    [HttpPost]
    public IActionResult AgregarCategoria(string? nombre, string? padre)
    {
        lock (catalogo.Candado)
            return Volver(catalogo.Categorias.AgregarCategoria(nombre ?? "", padre)
                ? "Categoría agregada." : "Revisa el nombre: debe ser único y su padre debe existir.");
    }

    [HttpPost]
    public IActionResult Registrar(long isbn, string? titulo, string? autor, string? categoria)
    {
        lock (catalogo.Candado) return Volver(catalogo.Registrar(isbn, titulo, autor, categoria));
    }

    [HttpPost]
    public IActionResult Eliminar(long isbn)
    {
        lock (catalogo.Candado)
            return Volver(catalogo.Libros.Eliminar(isbn) ? "Libro eliminado." : "No existe un libro con ese ISBN.");
    }

    [HttpPost]
    public IActionResult Reiniciar(bool confirmar)
    {
        if (!confirmar) return Volver("Confirma que deseas vaciar el catálogo.");
        lock (catalogo.Candado) catalogo.Reiniciar();
        return Volver("El sistema ha sido reiniciado a cero correctamente.");
    }

    [HttpPost]
    public IActionResult InicializarSistema(bool confirmar) => Reiniciar(confirmar);

    [HttpGet]
    public async Task<IActionResult> Reporte(string tipo = "categorias", string? categoria = null, bool descargar = false)
    {
        string dot;
        lock (catalogo.Candado)
        {
            if (tipo != "categorias" && tipo != "libros") return Volver("Tipo de reporte inválido.");
            if (tipo == "libros" && string.IsNullOrWhiteSpace(categoria)) return Volver("Indica una categoría para el reporte de libros.");
            if (!string.IsNullOrWhiteSpace(categoria) && catalogo.Categorias.Buscar(categoria) == null)
                return Volver("La categoría no existe.");
            dot = tipo == "libros" ? graficos.Libros(catalogo.Libros, categoria!) : graficos.Categorias(catalogo.Categorias, categoria);
        }
        if (descargar) return File(Encoding.UTF8.GetBytes(dot), "text/plain; charset=utf-8", "reporte.dot");
        try { return File(await graficos.Renderizar(dot), "image/svg+xml"); }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException or OperationCanceledException or IOException)
        {
            return Volver("No se pudo generar el gráfico. Verifica la instalación y la ruta de Graphviz o descarga el archivo DOT.");
        }
    }

    public IActionResult Privacy() => View();
    public IActionResult Ayuda() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
