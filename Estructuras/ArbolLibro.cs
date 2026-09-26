using IPC2_Proy02_202602_202112134.Models;

namespace IPC2_Proy02_202602_202112134.Estructuras;

public delegate void VisitarLibro(Libro libro);

// AVL: conserva el orden del árbol original y evita que se convierta en una cadena.
public class ArbolLibro
{
    private NodoLibro? raiz;
    public NodoLibro? Raiz => raiz;
    public int Cantidad { get; private set; }
    public int Altura => Alto(raiz);

    private static int Alto(NodoLibro? nodo) => nodo?.Altura ?? 0;
    private static void Actualizar(NodoLibro nodo) =>
        nodo.Altura = 1 + Math.Max(Alto(nodo.Izquierdo), Alto(nodo.Derecho));

    private static NodoLibro RotarDerecha(NodoLibro nodo)
    {
        NodoLibro nuevaRaiz = nodo.Izquierdo!;
        nodo.Izquierdo = nuevaRaiz.Derecho;
        nuevaRaiz.Derecho = nodo;
        Actualizar(nodo);
        Actualizar(nuevaRaiz);
        return nuevaRaiz;
    }

    private static NodoLibro RotarIzquierda(NodoLibro nodo)
    {
        NodoLibro nuevaRaiz = nodo.Derecho!;
        nodo.Derecho = nuevaRaiz.Izquierdo;
        nuevaRaiz.Izquierdo = nodo;
        Actualizar(nodo);
        Actualizar(nuevaRaiz);
        return nuevaRaiz;
    }

    private static NodoLibro Equilibrar(NodoLibro nodo)
    {
        Actualizar(nodo);
        int balance = Alto(nodo.Izquierdo) - Alto(nodo.Derecho);
        if (balance > 1)
        {
            if (Alto(nodo.Izquierdo!.Izquierdo) < Alto(nodo.Izquierdo.Derecho))
                nodo.Izquierdo = RotarIzquierda(nodo.Izquierdo);
            return RotarDerecha(nodo);
        }
        if (balance < -1)
        {
            if (Alto(nodo.Derecho!.Derecho) < Alto(nodo.Derecho.Izquierdo))
                nodo.Derecho = RotarDerecha(nodo.Derecho);
            return RotarIzquierda(nodo);
        }
        return nodo;
    }

    public bool Insertar(Libro libro)
    {
        bool agregado = false;
        raiz = InsertarRecursivo(raiz, libro, ref agregado);
        if (agregado) Cantidad++;
        return agregado;
    }

    private static NodoLibro InsertarRecursivo(NodoLibro? nodo, Libro libro, ref bool agregado)
    {
        if (nodo == null) { agregado = true; return new NodoLibro(libro); }
        if (libro.ISBN < nodo.Libro.ISBN)
            nodo.Izquierdo = InsertarRecursivo(nodo.Izquierdo, libro, ref agregado);
        else if (libro.ISBN > nodo.Libro.ISBN)
            nodo.Derecho = InsertarRecursivo(nodo.Derecho, libro, ref agregado);
        else return nodo; // El ISBN es único: nunca reemplazamos otro libro.
        return Equilibrar(nodo);
    }

    public Libro? Buscar(long isbn)
    {
        NodoLibro? actual = raiz;
        while (actual != null)
        {
            if (isbn == actual.Libro.ISBN) return actual.Libro;
            actual = isbn < actual.Libro.ISBN ? actual.Izquierdo : actual.Derecho;
        }
        return null;
    }

    public Libro? ObtenerMenor()
    {
        NodoLibro? actual = raiz;
        if (actual == null) return null;
        while (actual.Izquierdo != null) actual = actual.Izquierdo;
        return actual.Libro;
    }

    public Libro? ObtenerMayor()
    {
        NodoLibro? actual = raiz;
        if (actual == null) return null;
        while (actual.Derecho != null) actual = actual.Derecho;
        return actual.Libro;
    }

    public bool Eliminar(long isbn)
    {
        bool eliminado = false;
        raiz = EliminarRecursivo(raiz, isbn, ref eliminado);
        if (eliminado) Cantidad--;
        return eliminado;
    }

    private static NodoLibro? EliminarRecursivo(NodoLibro? nodo, long isbn, ref bool eliminado)
    {
        if (nodo == null) return null;
        if (isbn < nodo.Libro.ISBN)
            nodo.Izquierdo = EliminarRecursivo(nodo.Izquierdo, isbn, ref eliminado);
        else if (isbn > nodo.Libro.ISBN)
            nodo.Derecho = EliminarRecursivo(nodo.Derecho, isbn, ref eliminado);
        else
        {
            eliminado = true;
            if (nodo.Izquierdo == null) return nodo.Derecho;
            if (nodo.Derecho == null) return nodo.Izquierdo;
            NodoLibro sucesor = nodo.Derecho;
            while (sucesor.Izquierdo != null) sucesor = sucesor.Izquierdo;
            nodo.Libro = sucesor.Libro;
            nodo.Derecho = EliminarRecursivo(nodo.Derecho, sucesor.Libro.ISBN, ref eliminado);
        }
        return Equilibrar(nodo);
    }

    // La acción recibe cada libro en orden, sin crear List ni arreglos de libros.
    public void RecorrerEnOrden(VisitarLibro visitar) => Recorrer(raiz, visitar);
    private static void Recorrer(NodoLibro? nodo, VisitarLibro visitar)
    {
        if (nodo == null) return;
        Recorrer(nodo.Izquierdo, visitar);
        visitar(nodo.Libro);
        Recorrer(nodo.Derecho, visitar);
    }

    public void MostrarEnOrden() => RecorrerEnOrden(libro =>
        Console.WriteLine($"ISBN: {libro.ISBN}, Titulo: {libro.Titulo}, Autor: {libro.Autor}, Categoria: {libro.Categoria}"));
}
