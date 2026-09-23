using IPC2_Proy02_202602_202112134.Models;

namespace IPC2_Proy02_202602_202112134.Estructuras
{
    public class NodoCategoria
    {
        public Categoria Categoria { get; set; }

        // Primera subcategoria y siguiente categoria del mismo nivel.
        public NodoCategoria? PrimerHijo { get; set; }
        public NodoCategoria? SiguienteHermano { get; set; }

        public NodoCategoria(Categoria categoria)
        {
            Categoria = categoria;
        }
    }
}
