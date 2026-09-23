namespace IPC2_Proy02_202602_202112134.Models
{
    public class Categoria
    {
        // Nombre único de la categoría
        public string Nombre { get; set; }

        // Constructor
        public Categoria(string nombre)
        {
            Nombre = nombre;
        }
    }
}