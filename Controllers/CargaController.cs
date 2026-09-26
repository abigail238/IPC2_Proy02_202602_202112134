using Microsoft.AspNetCore.Mvc;
using IPC2_Proy02_202602_202112134.Servicios;

namespace IPC2_Proy02_202602_202112134.Controllers;

[AutoValidateAntiforgeryToken]
public class CargaController(Catalogo catalogo, GeneradorGraphviz graficos) : Controller
{
    [HttpGet]
    public IActionResult Index() => RedirectToAction("Index", "Home");

    [HttpPost]
    [RequestSizeLimit(27 * 1024 * 1024)]
    public IActionResult SubirArchivo(IFormFile? archivo)
    {
        var controlador = new HomeController(catalogo, graficos) { ControllerContext = ControllerContext, TempData = TempData };
        controlador.Cargar(archivo);
        return RedirectToAction("Index", "Home");
    }
}
