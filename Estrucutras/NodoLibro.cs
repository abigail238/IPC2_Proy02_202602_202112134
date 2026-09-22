using IPC2_Proy02_202602_202112134.Models;

namespace IPC2_Proy02_202602_202112134.Estrucutras
{
    public calss NodoLibro
    {
        // libro que se encuentra almacenado en este nodo 
        public Libro libro { get; set; }

    //nodo hijo izquierdo 
    public NodoLibro? Izquierdo { get; set; }

    //referencia al nodo hijo derecho 
    public NodoLibro? Derecho { get; set; }

    //constructor del nodo 
    public NodoLibro(Libro libro)
    {
        libro = libro;

        Izquierdo = null;
        Derecho = null;
    }
}
}