using IPC2_Proy02_202602_202112134.Estructuras;
using IPC2_Proy02_202602_202112134.Models;
namespace IPC2_Proy02_202602_202112134.Servicios;

public class Catalogo
{
    // El controlador toma este candado para que dos solicitudes no modifiquen
    // los mismos nodos al mismo tiempo.
    public object Candado { get; } = new();
    public ArbolLibro Libros { get; private set; } = new();
    public ArbolCategoria Categorias { get; private set; } = new();

    public void Reiniciar()
    {
        Libros = new ArbolLibro();
        Categorias = new ArbolCategoria();
    }

    public string Registrar(long isbn, string? titulo, string? autor, string? categoria)
    {
        if (isbn <= 0 || string.IsNullOrWhiteSpace(titulo) ||
            string.IsNullOrWhiteSpace(autor) || string.IsNullOrWhiteSpace(categoria))
            return "Completa todos los datos y usa un ISBN entero positivo.";
        NodoCategoria? nodo = Categorias.Buscar(categoria);
        if (nodo == null) return "La categoría no existe. Agrégala primero.";
        if (!Libros.Insertar(new Libro(isbn, titulo.Trim(), autor.Trim(), nodo.Categoria.Nombre)))
            return "Ya existe un libro con ese ISBN.";
        return "Libro registrado.";
    }

    public PaginaCatalogo ObtenerPagina(string? categoria, string? titulo, int pagina)
    {
        var modelo = new PaginaCatalogo
        {
            Total = Libros.Cantidad,
            Categoria = categoria?.Trim() ?? "",
            Titulo = titulo?.Trim() ?? "",
            Estructura = Categorias.ObtenerEstructura(categoria)
        };
        bool Coincide(Libro libro) =>
            (modelo.Categoria.Length == 0 || string.Equals(libro.Categoria, modelo.Categoria, StringComparison.OrdinalIgnoreCase)) &&
            (modelo.Titulo.Length == 0 || libro.Titulo.Contains(modelo.Titulo, StringComparison.OrdinalIgnoreCase));
        Libros.RecorrerEnOrden(libro => { if (Coincide(libro)) modelo.Coincidencias++; });
        modelo.Pagina = Math.Clamp(pagina, 1, modelo.Paginas);
        int posicion = 0;
        int desde = (modelo.Pagina - 1) * 50;
        Libros.RecorrerEnOrden(libro =>
        {
            if (!Coincide(libro)) return;
            if (posicion >= desde && posicion < desde + 50) modelo.Libros.Insertar(libro);
            posicion++;
        });
        return modelo;
    }
}

