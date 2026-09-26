using IPC2_Proy02_202602_202112134.Models;

namespace IPC2_Proy02_202602_202112134.Estructuras
{
    public class ListaCategorias
    {
        // Primer nodo de la lista
        public NodoCategoria? Cabeza { get; private set; }

        // Constructor
        public ListaCategorias()
        {
            Cabeza = null;
        }

        // Inserta un nodo manteniendo orden alfabético
        public void InsertarOrdenado(NodoCategoria nuevo)
        {
            // Si la lista está vacía
            if (Cabeza == null)
            {
                Cabeza = nuevo;
                return;
            }

            // Si el nuevo nodo debe ir antes que la cabeza
            if (string.Compare(
                nuevo.Categoria.Nombre,
                Cabeza.Categoria.Nombre,
                StringComparison.OrdinalIgnoreCase) < 0)
            {
                nuevo.SiguienteHermano = Cabeza;
                Cabeza = nuevo;
                return;
            }

            NodoCategoria actual = Cabeza;

            // Buscamos la posición correcta
            while (
                actual.SiguienteHermano != null &&
                string.Compare(
                    actual.SiguienteHermano.Categoria.Nombre,
                    nuevo.Categoria.Nombre,
                    StringComparison.OrdinalIgnoreCase
                ) < 0)
            {
                actual = actual.SiguienteHermano;
            }

            // Insertamos el nodo
            nuevo.SiguienteHermano = actual.SiguienteHermano;
            actual.SiguienteHermano = nuevo;
        }

        // Agrega un nodo ya creado
        public void AgregarNodo(NodoCategoria nuevo)
        {
            InsertarOrdenado(nuevo);
        }

        // Agrega una categoría creando su nodo
        public void Agregar(Categoria categoria)
        {
            NodoCategoria nuevoNodo =
                new NodoCategoria(categoria);

            InsertarOrdenado(nuevoNodo);
        }
    }
}