using IPC2_Proy02_202602_202112134.Estructuras;
namespace IPC2_Proy02_202602_202112134.Models;

public class PaginaCatalogo
{
    public ArbolLibro Libros { get; } = new();
    public string Estructura { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string Titulo { get; set; } = "";
    public string Mensaje { get; set; } = "";
    public Libro? Resultado { get; set; }
    public int Total { get; set; }
    public int Coincidencias { get; set; }
    public int Pagina { get; set; }
    public int Paginas => Math.Max(1, (Coincidencias + 49) / 50);
}

