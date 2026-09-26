using IPC2_Proy02_202602_202112134.Models;

namespace IPC2_Proy02_202602_202112134.Estructuras;

public class NodoLibro
{
    public Libro Libro { get; set; }
    public NodoLibro? Izquierdo { get; set; }
    public NodoLibro? Derecho { get; set; }
    // Una hoja tiene altura 1. Usamos la altura para mantener el equilibrio AVL.
    public int Altura { get; set; } = 1;
    public NodoLibro(Libro libro) { Libro = libro; }
}

