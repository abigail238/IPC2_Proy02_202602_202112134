namespace IPC2_Proy02_202602_202112134.Models;

public class Libro
{
    // long permite ISBN de 13 dígitos, que no caben en un int.
    public long ISBN { get; }
    public string Titulo { get; }
    public string Autor { get; }
    public string Categoria { get; }
    public Libro(long isbn, string titulo, string autor, string categoria)
    {
        ISBN = isbn;
        Titulo = titulo;
        Autor = autor;
        Categoria = categoria;
    }
}

