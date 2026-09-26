using System.Xml;
using System.Xml.Linq;
using IPC2_Proy02_202602_202112134.Estructuras;
using IPC2_Proy02_202602_202112134.Models;

namespace IPC2_Proy02_202602_202112134.Servicios;

public class LectorXml
{
    public void CargarArchivo(string ruta, ArbolCategoria categorias, ArbolLibro libros)
    {
        using var archivo = File.OpenRead(ruta);
        Console.WriteLine(Cargar(archivo, categorias, libros));
    }

    public string Cargar(Stream archivo, ArbolCategoria categorias, ArbolLibro libros)
    {
        // El lector XML es para interpretar el archivo. Los datos del catálogo
        // se almacenan exclusivamente en nuestros árboles.
        var ajustes = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };
        using var lector = XmlReader.Create(archivo, ajustes);
        var documento = XDocument.Load(lector);
        XElement? raiz = documento.Root;
        if (raiz?.Name != "config") throw new XmlException("El elemento principal debe ser <config>.");

        int categoriasAgregadas = 0, categoriasOmitidas = 0, librosAgregados = 0, librosOmitidos = 0;
        XElement? listaCategorias = raiz.Element("listaCategorias");
        if (listaCategorias != null)
        {
            // Resolvemos padres que aparezcan después de sus hijos sin cambiar el XML.
            int totalCategorias = 0;
            foreach (XElement elemento in listaCategorias.Elements("categoria")) totalCategorias++;
            bool huboCambios;
            do
            {
                huboCambios = false;
                foreach (XElement elemento in listaCategorias.Elements("categoria"))
                {
                    string nombre = elemento.Value.Trim();
                    if (categorias.Buscar(nombre) != null) continue;
                    if (categorias.AgregarCategoria(nombre, elemento.Attribute("padre")?.Value))
                    {
                        categoriasAgregadas++;
                        huboCambios = true;
                    }
                }
            } while (huboCambios);
            categoriasOmitidas = totalCategorias - categoriasAgregadas;
        }

        XElement? listaLibros = raiz.Element("listaLibros");
        if (listaLibros != null)
        {
            foreach (XElement elemento in listaLibros.Elements("libro"))
            {
                string? titulo = elemento.Element("titulo")?.Value.Trim();
                string? autor = elemento.Element("autor")?.Value.Trim();
                string? nombreCategoria = elemento.Element("categoria")?.Value.Trim();
                NodoCategoria? categoria = categorias.Buscar(nombreCategoria ?? "");
                if (!long.TryParse(elemento.Element("ISBN")?.Value, out long isbn) || isbn <= 0 ||
                    string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(autor) || categoria == null)
                {
                    librosOmitidos++;
                    continue;
                }
                if (libros.Insertar(new Libro(isbn, titulo, autor, categoria.Categoria.Nombre))) librosAgregados++;
                else librosOmitidos++;
            }
        }
        return $"Carga incremental: {categoriasAgregadas} categorías y {librosAgregados} libros agregados. " +
            $"Omitidos: {categoriasOmitidas} categorías (repetidas, vacías o con padres sin resolver) y " +
            $"{librosOmitidos} libros (duplicados o datos inválidos).";
    }
}

