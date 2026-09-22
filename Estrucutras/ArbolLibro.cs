using IPC2_Proy02_202602_202112134.Models;

namespace IPC2_Proy02_202602_202112134.Estrucutras
{
    public class ArbolLibro
    {
        //raiz principa del arbol 
        private NodoLibro? raiz;

        //constructor
        public ArbolLibro()
        {
            raiz = null;

        }

        //metodo public para insertar un libro 
        public void Insertar(Libro libro)
        {

            //si el libro esta vacio el nuevo libro se convierte en la raiz 
            if (raiz == null)
            {
                raiz = new NodoLibro(libro);
                return;
            }

            //si ya existe una raiz, buscamos donde colocar el nuevo libro 
            InsertarRecursivo(raiz, libro);
        }

        //metodo que busca la posicion correcta 
        private void InsertarRecursivo(NodoLibro actual, Libro libro) {

            // si el ISBN nuevo es menor, debe ir hacia la izquierda 
            if (libro.ISBN < actual.libro.ISBN)
            {
                //si no exites nodo izquierdo, colocamos el nuevo libro 
                if (actual.Izaquierdo == null)
                {
                    actual.Izquierdo = new NodoLibro(libro);
                }
                else
                {
                    // si ya existe un nodo izquierdo, seguimos buscando ms abajo 
                    InsertarRecursivo(actual.Izquierdo, libro);
                }
            }
            //si el ISBN nuevo es mayot, debe ir hacia la derecha
            else if (libro.ISBN > actual.Libro.ISBN)
            {
                //SI NO existe nodo derecho colocamos el nuevo libro aqui 
                if (actual..Dercho == null)
                {
                    actual.Derecho = new NodoLibro(libro);
                }
                else
                {
                    //si ya existe un nodo derecho, seguimos buscando mas abajo 
                    InsertarRecursivo(actual.Derecho, libro);

                }

            }

        }

    }
}


       

    