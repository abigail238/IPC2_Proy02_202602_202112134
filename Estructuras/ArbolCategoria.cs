using IPC2_Proy02_202602_202112134.Models;

namespace IPC2_Proy02_202602_202112134.Estructuras
{
    public class ArbolCategoria
    {
        private NodoCategoria? raiz;

        public bool AgregarCategoria(string nombre, string? nombrePadre = null)
        {
            if (Buscar(nombre) != null)
                return false;

            NodoCategoria nuevo = new NodoCategoria(new Categoria(nombre));
            if (nombrePadre == null)
            {
                raiz = InsertarOrdenado(raiz, nuevo);
                return true;
            }

            NodoCategoria? padre = Buscar(nombrePadre);
            if (padre == null)
                return false;

            padre.PrimerHijo = InsertarOrdenado(padre.PrimerHijo, nuevo);
            return true;
        }

        public NodoCategoria? Buscar(string nombre)
        {
            return BuscarRecursivo(raiz, nombre);
        }

        private NodoCategoria? BuscarRecursivo(NodoCategoria? actual, string nombre)
        {
            if (actual == null)
                return null;

            if (string.Equals(actual.Categoria.Nombre, nombre, StringComparison.OrdinalIgnoreCase))
                return actual;

            NodoCategoria? encontrado = BuscarRecursivo(actual.PrimerHijo, nombre);
            return encontrado ?? BuscarRecursivo(actual.SiguienteHermano, nombre);
        }

        // Inserta en orden alfabetico y devuelve el primer nodo del nivel.
        private NodoCategoria InsertarOrdenado(NodoCategoria? primero, NodoCategoria nuevo)
        {
            if (primero == null || string.Compare(nuevo.Categoria.Nombre,
                primero.Categoria.Nombre, StringComparison.OrdinalIgnoreCase) < 0)
            {
                nuevo.SiguienteHermano = primero;
                return nuevo;
            }

            NodoCategoria actual = primero;
            while (actual.SiguienteHermano != null &&
                string.Compare(actual.SiguienteHermano.Categoria.Nombre,
                    nuevo.Categoria.Nombre, StringComparison.OrdinalIgnoreCase) < 0)
            {
                actual = actual.SiguienteHermano;
            }

            nuevo.SiguienteHermano = actual.SiguienteHermano;
            actual.SiguienteHermano = nuevo;
            return primero;
        }

        public void MostrarEstructura()
        {
            MostrarNivel(raiz, 0);
        }

        public void MostrarDesde(string nombre)
        {
            NodoCategoria? categoria = Buscar(nombre);
            if (categoria == null)
            {
                Console.WriteLine("Categoria no encontrada: " + nombre);
                return;
            }

            Console.WriteLine(categoria.Categoria.Nombre);
            MostrarNivel(categoria.PrimerHijo, 1);
        }

        private void MostrarNivel(NodoCategoria? actual, int nivel)
        {
            while (actual != null)
            {
                Console.WriteLine(new string(' ', nivel * 2) + actual.Categoria.Nombre);
                MostrarNivel(actual.PrimerHijo, nivel + 1);
                actual = actual.SiguienteHermano;
            }
        }
    }
}
