using System.Text;
using System.Xml;
using Microsoft.Extensions.Configuration;
using IPC2_Proy02_202602_202112134.Estructuras;
using IPC2_Proy02_202602_202112134.Models;
using IPC2_Proy02_202602_202112134.Servicios;

static void Exigir(bool condicion, string mensaje)
{
    if (!condicion) throw new Exception(mensaje);
}
static int Verificar(NodoLibro? nodo, long minimo = long.MinValue, long maximo = long.MaxValue)
{
    if (nodo == null) return 0;
    Exigir(nodo.Libro.ISBN > minimo && nodo.Libro.ISBN < maximo, "Orden BST incorrecto");
    int izq = Verificar(nodo.Izquierdo, minimo, nodo.Libro.ISBN);
    int der = Verificar(nodo.Derecho, nodo.Libro.ISBN, maximo);
    Exigir(Math.Abs(izq - der) <= 1, "Balance AVL incorrecto");
    Exigir(nodo.Altura == 1 + Math.Max(izq, der), "Altura incorrecta");
    return nodo.Altura;
}
var arbol = new ArbolLibro();
Exigir(arbol.ObtenerMenor() == null && arbol.ObtenerMayor() == null && !arbol.Eliminar(1), "Árbol vacío");
for (int i = 1; i <= 100_000; i++) Exigir(arbol.Insertar(new Libro(i, "Libro", "Autor", "Ciencia")), "Inserción");
Verificar(arbol.Raiz);
Exigir(arbol.Cantidad == 100_000 && arbol.Altura <= 24, "Crecimiento del árbol");
for (int i = 1; i <= 100_000; i++) Exigir(arbol.Buscar(i)?.ISBN == i, "Búsqueda");
Exigir(!arbol.Insertar(new Libro(1, "Duplicado", "Autor", "Ciencia")) && arbol.Buscar(1)?.Titulo == "Libro", "Duplicados");
for (int i = 2; i <= 100_000; i += 2) Exigir(arbol.Eliminar(i), "Eliminar pares");
Verificar(arbol.Raiz);
Exigir(arbol.Cantidad == 50_000 && arbol.ObtenerMenor()?.ISBN == 1 && arbol.ObtenerMayor()?.ISBN == 99_999, "Extremos");
long anterior = 0;
int cantidad = 0;
arbol.RecorrerEnOrden(libro => { Exigir(libro.ISBN > anterior && libro.ISBN % 2 == 1, "Recorrido"); anterior = libro.ISBN; cantidad++; });
Exigir(cantidad == 50_000, "Cantidad recorrido");
for (int i = 99_999; i >= 1; i -= 2) Exigir(arbol.Eliminar(i), "Vaciar");
Exigir(arbol.Raiz == null && arbol.Cantidad == 0, "Vacío final");

// Secuencia no ordenada para ejercitar rotaciones simples y dobles.
for (int i = 1; i < 997; i++) arbol.Insertar(new Libro(i * 37 % 997, "Prueba", "Autor", "Ciencia"));
Verificar(arbol.Raiz);
while (arbol.Raiz != null) { arbol.Eliminar(arbol.Raiz.Libro.ISBN); Verificar(arbol.Raiz); }
Console.WriteLine("OK: AVL con 100,000 ISBN ordenados, búsquedas, duplicados, recorridos y eliminaciones.");

var catalogo = new Catalogo();
Exigir(catalogo.Categorias.AgregarCategoria("Tecnologia"), "Categoría");
Exigir(catalogo.Categorias.AgregarCategoria("Arte"), "Categoría");
Exigir(!catalogo.Categorias.AgregarCategoria(" arte ") && !catalogo.Categorias.AgregarCategoria("X", "Inexistente"), "Validación categoría");
string xml = """
<config>
<listaCategorias><categoria padre="Ciencia">Fisica</categoria><categoria>Ciencia</categoria>
<categoria padre="Inexistente">SinPadre</categoria><categoria padre="B">A</categoria><categoria padre="A">B</categoria></listaCategorias>
<listaLibros>
<libro><ISBN>9781234567890</ISBN><titulo>Libro &amp; ciencia</titulo><autor>Autor</autor><categoria>Fisica</categoria></libro>
<libro><ISBN>2</ISBN><titulo>Sin categoria</titulo><autor>Autor</autor><categoria>No existe</categoria></libro>
<libro><ISBN>-1</ISBN><titulo>Invalido</titulo><autor>Autor</autor><categoria>Fisica</categoria></libro>
</listaLibros></config>
""";
string Cargar(string contenido)
{
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(contenido));
    return new LectorXml().Cargar(stream, catalogo.Categorias, catalogo.Libros);
}
Cargar(xml);
Exigir(catalogo.Libros.Cantidad == 1 && catalogo.Libros.Buscar(9781234567890) != null, "ISBN largo");
Exigir(catalogo.Categorias.Buscar("Fisica") != null && catalogo.Categorias.Buscar("A") == null, "Dependencias XML");
Cargar(xml);
Exigir(catalogo.Libros.Cantidad == 1, "Carga repetida");
Cargar("<config><listaLibros><libro><ISBN>4</ISBN><titulo>Otro</titulo><autor>Autor</autor><categoria>Fisica</categoria></libro></listaLibros></config>");
Exigir(catalogo.Libros.Cantidad == 2, "Carga incremental sin categorías");
Cargar("<config><listaCategorias><categoria>Literatura</categoria></listaCategorias></config>");
Cargar("<config/>");
try { Cargar("<config><listaCategorias>"); throw new Exception("Aceptó XML malformado"); } catch (XmlException) { }
try { Cargar("<otra/>"); throw new Exception("Aceptó raíz inválida"); } catch (XmlException) { }
try { Cargar("<!DOCTYPE config [<!ENTITY x SYSTEM 'file:///no-leer'>]><config>&x;</config>"); throw new Exception("Aceptó DTD"); } catch (XmlException) { }
Exigir(catalogo.Libros.Cantidad == 2, "XML inválido alteró datos");
string estructura = catalogo.Categorias.ObtenerEstructura();
Exigir(estructura.IndexOf("Arte") < estructura.IndexOf("Ciencia") && estructura.IndexOf("Ciencia") < estructura.IndexOf("Tecnologia"), "Orden categorías");
Exigir(!catalogo.Categorias.ObtenerEstructura("Ciencia").Contains("Tecnologia"), "Subárbol incluye hermanos");
Exigir(catalogo.ObtenerPagina("Fisica", "ciencia", 1).Coincidencias == 1, "Filtro");
for (int i = 100; i < 220; i++) catalogo.Registrar(i, "Libro", "Autor", "Fisica");
var pagina = catalogo.ObtenerPagina("Fisica", null, 2);
Exigir(pagina.Libros.Cantidad == 50 && pagina.Coincidencias == 122 && pagina.Paginas == 3, "Paginación");
Console.WriteLine("OK: categorías, cargas incrementales, XML inválido, ISBN de 13 dígitos y paginación.");

var graficos = new GeneradorGraphviz(new ConfigurationBuilder().Build());
string dot = graficos.Libros(catalogo.Libros, "Fisica");
Exigir(dot.IndexOf("l4 [") < dot.IndexOf("l9781234567890 ["), "Reporte no está ordenado");
Exigir(!graficos.Categorias(catalogo.Categorias, "Ciencia").Contains("Tecnologia"), "Reporte subárbol");
byte[] svg = await graficos.Renderizar(dot);
Exigir(Encoding.UTF8.GetString(svg).Contains("<svg"), "Render SVG");
catalogo.Registrar(999, "Comillas \" y barra \\ y salto\n<script>", "A & B", "Fisica");
svg = await graficos.Renderizar(graficos.Libros(catalogo.Libros, "Fisica"));
Exigir(Encoding.UTF8.GetString(svg).Contains("&lt;script&gt;"), "Escape Graphviz");
catalogo.Reiniciar();
Exigir(catalogo.Libros.Cantidad == 0 && catalogo.Categorias.Raiz == null, "Reiniciar");
Console.WriteLine("OK: Graphviz real, etiquetas especiales y reinicio.");

