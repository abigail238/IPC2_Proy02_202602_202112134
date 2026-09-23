using IPC2_Proy02_202602_202112134.Models;
namespace IPC2_Proy02_202602_202112134.Estructuras
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
            if (libro.ISBN < actual.Libro.ISBN)
            {
                //si no exites nodo izquierdo, colocamos el nuevo libro 
                if (actual.Izquierdo == null)
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
                if (actual.Derecho == null)
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

        public Libro? Buscar(int isbn)
        {
            return BuscarRecursivo(raiz, isbn);
        }

        //metodo privado que recorre el arbol buscando el ISBN 
        private Libro?  BuscarRecursivo(NodoLibro? actual, int isbn)
        {
            // si llegamos a un esapcio vacio, significa que el libro no existe 
            if(actual == null)
            {
                return null;
            }

            //si encontramos el isbn buscado, devolvemos el libro
            if(isbn == actual.Libro.ISBN)
            {
                return actual.Libro;
            }

            //si el ISBN buscado es menor, buscamos en el lado izquierdo 
            if(isbn < actual.Libro.ISBN)
            {
                return BuscarRecursivo(actual.Izquierdo, isbn);

            }

            //si el ISBN buscado es mayor, buscamos en el lado derecho
            return BuscarRecursivo(actual.Derecho, isbn);
        }

        public Libro? ObtenerMenor()
        {
            //si no hay raiz, el arbol esta vacion 
            if(raiz == null)
            {
                return null;
            }

            //empezamos desde la raiz 
            NodoLibro actual = raiz;

            //mientras exista un nodo a la izquierda, seguimos avanzando hacia ese lado 
            while(actual.Izquierdo != null)
            {
                actual = actual.Izquierdo;

            }
             //el nodo mas a la izquierda contiene el isbn mas pequenio 
             return actual.Libro;

        }

        //devuelve el libro con isbn mas grande 
        public Libro? ObtenerMayor()
        {
            // si no hay raiz, el arbol esta vacio

            if(raiz == null)
            {
                return null;
            }

            //emepzamos desde la raiz 
            NodoLibro actual = raiz; 

            //Mientras exista un nodo a la derecha, seguimos avanzando hacia ese lado 
            while(actual.Derecho != null)
            {
                actual = actual.Derecho;
            }

            return actual.Libro;

        }

        public void MostrarEnOrden()
        {
            RecorrerInOrden(raiz);

        }

        private void RecorrerInOrden(NodoLibro? actual)
        {
            if (actual == null)
            {
                return;
            }

            // recorremos primero el lado izquierdo 
                RecorrerInOrden(actual.Izquierdo);

            //prcesamos el nodo actual
            Console.WriteLine($"ISBN: {actual.Libro.ISBN}, Titulo: {actual.Libro.Titulo}, Autor: {actual.Libro.Autor}, Categoria: {actual.Libro.Categoria}");
                RecorrerInOrden(actual.Derecho);
            
        }

        //Eliminar libro 
        public void Eliminar(int isbn)
        {
            raiz = EliminarRecursivo(raiz, isbn);
        }

        private NodoLibro? EliminarRecursivo(NodoLibro? actual, int isbn)
        {
            //si llegamos a null, significa que el isbn no existe 
            if(actual == null)
            {
                return null;
            }
            //si el isbn buscado es menor, buscamos en el lado izquierdo

            if (isbn < actual.Libro.ISBN)
            {
                actual.Izquierdo = EliminarRecursivo(actual.Izquierdo, isbn);
            }

            else if (isbn > actual.Libro.ISBN)
            {
                actual.Derecho = EliminarRecursivo(actual.Derecho, isbn);
            }

            else
            {
                //caso 1, cuando el nodo tiene hijo izquierdo 
                if(actual.Izquierdo == null)
                {
                    return actual.Derecho;
                }
                //caso 2: cuando el nodo tiene hijo derecho 
                if(actual.Derecho == null)
                {
                    return actual.Izquierdo;
                }

                //caso 3: cuando el nodo tiene dos hijos
                
                //buscamos el nodo con isbn mas pequenio del subarbol derecho 
                NodoLibro sucesor = ObtenerNodoMenor(actual.Derecho);

                //copiamos el libro del sucesor al nodo actual 

                actual.Libro = sucesor.Libro;

                //eliminamos el sucesor original 
                actual.Derecho = EliminarRecursivo(actual.Derecho, sucesor.Libro.ISBN);

            }

            return actual;
        }

        private NodoLibro ObtenerNodoMenor(NodoLibro actual)
        {
            while(actual.Izquierdo != null)
            {
                actual = actual.Izquierdo;

            }
            return actual;
        }
    }
}


       

    
