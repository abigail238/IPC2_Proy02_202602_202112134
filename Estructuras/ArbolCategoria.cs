using IPC2_Proy02_202602_202112134.Models;

namespace IPC2_Proy02_202602_202112134.Estructuras
{
    public class ArbolCategoria
    {
        //primera categoria principal del arbol
        private NodoCategoria? raiz;
        public NodoCategoria? Raiz => raiz;

        public string ObtenerEstructura(string? nombre = null)
        {
            var texto = new System.Text.StringBuilder();
            if (string.IsNullOrWhiteSpace(nombre)) Escribir(raiz, 0, texto);
            else
            {
                NodoCategoria? inicio = Buscar(nombre);
                if (inicio == null) return "La categoria no existe.";
                texto.AppendLine(inicio.Categoria.Nombre);
                Escribir(inicio.PrimerHijo, 1, texto);
            }
            return texto.Length == 0 ? "No hay categorias registradas." : texto.ToString();
        }

        private static void Escribir(NodoCategoria? nodo, int nivel, System.Text.StringBuilder texto)
        {
            while (nodo != null)
            {
                texto.Append(' ', nivel * 2).Append("- ").AppendLine(nodo.Categoria.Nombre);
                Escribir(nodo.PrimerHijo, nivel + 1, texto);
                nodo = nodo.SiguienteHermano;
            }
        }

        //costructor
        public ArbolCategoria()
        {
            raiz = null;
        }

        //metodo publico para agregar una categoria
        public bool AgregarCategoria(string nombre, string? nombrePadre = null)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return false;
            }

            nombre = nombre.Trim();
            nombrePadre = string.IsNullOrWhiteSpace(nombrePadre) ? null : nombrePadre.Trim();

            //validamos que no exita otra categoria con el mismo nombre en todo el arbol
            if (Buscar(nombre) != null)
            {
                return false;
            }

            //creamos la nueva categoria
            Categoria nuevaCategoria = new Categoria(nombre);

            //creamos el nodo que contendra la categoria
            NodoCategoria nuevoNodo = new NodoCategoria(nuevaCategoria);


            //si no tiene padre, significa que es un categoria prinicpal
            if (nombrePadre == null)
            {
                InsertarOrdenadoComoRaiz(nuevoNodo);
                return true;
            }

            // buscamos la categoria padre
            NodoCategoria? padre = Buscar(nombrePadre);

            //si el padre no existe, no podemos agregar la subactegoria
            if (padre == null)
            {
                return false;
            }

            InsertarHijoOrdenado(padre, nuevoNodo);

            return true;
        }

        // busca una categoria por su nombre
        public NodoCategoria? Buscar(string nombre)
        {
            return string.IsNullOrWhiteSpace(nombre) ? null : BuscarRecursivo(raiz, nombre.Trim());
        }

        //Metodo recursivo para buscar una categoria
        private NodoCategoria? BuscarRecursivo(NodoCategoria? actual, string nombre)
        {

            //si llegamos a null, no encontramos la categoria

            if (actual == null)
            {
                return null;
            }


            //comparamos el nombre de la categoria actual
            if (string.Equals(actual.Categoria.Nombre, nombre, StringComparison.OrdinalIgnoreCase))
            {
                return actual;
            }

            //Busacmos dentro de sus hijos
            NodoCategoria? encontrado = BuscarRecursivo(actual.PrimerHijo, nombre);

            if (encontrado != null)
            {
                return encontrado;
            }

            // si no estaba en los hijos, buscamos en los hermnaos
            return BuscarRecursivo(actual.SiguienteHermano, nombre);


        }
        private void InsertarOrdenadoComoRaiz(NodoCategoria nuevo)
        {

            //si no existe ninguna categoria prinicpal, el nuevo nodo se convierte en la raiz
            if (raiz == null)
            {
                raiz = nuevo;
                return;
            }

            // si el nuevo nombre debe ir antes que la raiz actual
            if (string.Compare(nuevo.Categoria.Nombre, raiz.Categoria.Nombre, StringComparison.OrdinalIgnoreCase) < 0)
            {
                nuevo.SiguienteHermano = raiz;
                raiz = nuevo;
                return;

            }

            NodoCategoria actual = raiz;

            //buscamos la posicion correcta

            while (actual.SiguienteHermano != null && string.Compare(actual.SiguienteHermano.Categoria.Nombre, nuevo.Categoria.Nombre, StringComparison.OrdinalIgnoreCase) < 0)
            {
                actual = actual.SiguienteHermano;
            }

            //Insertamos entre los nodos
            nuevo.SiguienteHermano = actual.SiguienteHermano;
            actual.SiguienteHermano = nuevo;

        }

        private void InsertarHijoOrdenado(NodoCategoria padre, NodoCategoria nuevo)
        {

            //si el padre todavia no tiene hijo el nuevo nodo sera el primero
            if (padre.PrimerHijo == null)
            {
                padre.PrimerHijo = nuevo;
                return;
            }

            //si el nuevo nodo debe ir antes del primer hijo actual
            if (string.Compare(nuevo.Categoria.Nombre, padre.PrimerHijo.Categoria.Nombre, StringComparison.OrdinalIgnoreCase) < 0)
            {
                nuevo.SiguienteHermano = padre.PrimerHijo;
                padre.PrimerHijo = nuevo;
                return;
            }

            NodoCategoria actual = padre.PrimerHijo;

            //buscamos la posicion correcta
            while (actual.SiguienteHermano != null && string.Compare(actual.SiguienteHermano.Categoria.Nombre, nuevo.Categoria.Nombre, StringComparison.OrdinalIgnoreCase) < 0)
            {
                actual = actual.SiguienteHermano;
            }

            //insertar el nuevo nodo
            nuevo.SiguienteHermano = actual.SiguienteHermano;
            actual.SiguienteHermano = nuevo;

        }

        public void MostrarEstructura()
        {
            MostrarRecursivo(raiz, 0);
        }

        //Muestra la estructura comenzando una categoria especifica
        public void MostrarDesde(string nombreCategoria)
        {
            NodoCategoria? inicio = Buscar(nombreCategoria);

            if (inicio == null)
            {
                Console.WriteLine("La categoria no existe");
                return;
            }

            Console.WriteLine(" - " + inicio.Categoria.Nombre);
            MostrarRecursivo(inicio.PrimerHijo, 1);
        }

        //Metodo privado recursivo para mostrar la jerarquia
        private void MostrarRecursivo(NodoCategoria? actual, int nivel)
        {
            if (actual == null)
            {
                return;
            }


            //dejamos espacios dependiendo del nivel para que se note la jerarquia
            string sangria = "";

            for (int i = 0; i < nivel; i++)
            {
                sangria += " ";

            }

            Console.WriteLine(sangria + " - " + actual.Categoria.Nombre);

            // primero mostramos los hijos
            MostrarRecursivo(actual.PrimerHijo, nivel + 1);

            // Mostrar los hermanos
            MostrarRecursivo(actual.SiguienteHermano, nivel);

        }
    }
}
