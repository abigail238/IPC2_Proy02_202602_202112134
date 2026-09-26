using System.Diagnostics;
using System.Text;
using IPC2_Proy02_202602_202112134.Estructuras;

namespace IPC2_Proy02_202602_202112134.Servicios;

public class GeneradorGraphviz
{
    private readonly IConfiguration configuracion;
    public GeneradorGraphviz(IConfiguration configuracion) { this.configuracion = configuracion; }

    // Las etiquetas se escapan: los datos del usuario no son instrucciones DOT.
    private static string Escapar(string texto) => texto.Replace("\\", "\\\\")
        .Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n");

    public string Categorias(ArbolCategoria arbol, string? categoria)
    {
        var dot = new StringBuilder("digraph Catalogo { node [shape=box]; ordering=out;\n");
        int identificador = 0;
        void Visitar(NodoCategoria nodo, int? padre)
        {
            int actual = identificador++;
            dot.AppendLine($"n{actual} [label=\"{Escapar(nodo.Categoria.Nombre)}\"];");
            if (padre != null) dot.AppendLine($"n{padre} -> n{actual};");
            NodoCategoria? hijo = nodo.PrimerHijo;
            while (hijo != null) { Visitar(hijo, actual); hijo = hijo.SiguienteHermano; }
        }
        if (!string.IsNullOrWhiteSpace(categoria))
        {
            NodoCategoria? inicio = arbol.Buscar(categoria);
            if (inicio == null) throw new ArgumentException("La categoría no existe.");
            Visitar(inicio, null);
        }
        else
        {
            NodoCategoria? actual = arbol.Raiz;
            while (actual != null) { Visitar(actual, null); actual = actual.SiguienteHermano; }
        }
        return dot.AppendLine("}").ToString();
    }

    public string Libros(ArbolLibro arbol, string categoria)
    {
        var dot = new StringBuilder("digraph Libros { rankdir=LR; node [shape=box];\n");
        long? anterior = null;
        arbol.RecorrerEnOrden(libro =>
        {
            if (!string.Equals(libro.Categoria, categoria.Trim(), StringComparison.OrdinalIgnoreCase)) return;
            string etiqueta = $"{libro.ISBN}\n{libro.Titulo}\n{libro.Autor}";
            dot.AppendLine($"l{libro.ISBN} [label=\"{Escapar(etiqueta)}\"];");
            if (anterior != null) dot.AppendLine($"l{anterior} -> l{libro.ISBN};");
            anterior = libro.ISBN;
        });
        if (anterior == null) dot.AppendLine("vacio [label=\"Esta categoría no tiene libros\"];");
        return dot.AppendLine("}").ToString();
    }

    public async Task<byte[]> Renderizar(string dot)
    {
        string? ejecutable = configuracion["Graphviz:Ruta"];
        if (string.IsNullOrWhiteSpace(ejecutable))
            ejecutable = OperatingSystem.IsWindows() && File.Exists(@"C:\Program Files\Graphviz\bin\dot.exe")
                ? @"C:\Program Files\Graphviz\bin\dot.exe" : "dot";
        var inicio = new ProcessStartInfo(ejecutable)
        {
            RedirectStandardInput = true, RedirectStandardOutput = true,
            RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true,
            StandardInputEncoding = new UTF8Encoding(false)
        };
        inicio.ArgumentList.Add("-Tsvg");
        using var proceso = new Process { StartInfo = inicio };
        proceso.Start();
        using var limite = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        using var salida = new MemoryStream();
        var copiar = proceso.StandardOutput.BaseStream.CopyToAsync(salida, limite.Token);
        var errores = proceso.StandardError.ReadToEndAsync(limite.Token);
        try
        {
            await proceso.StandardInput.WriteAsync(dot.AsMemory(), limite.Token);
            proceso.StandardInput.Close();
            await proceso.WaitForExitAsync(limite.Token);
            await copiar;
            string detalle = await errores;
            if (proceso.ExitCode != 0) throw new InvalidOperationException("Graphviz no pudo generar el reporte. " + detalle);
            return salida.ToArray();
        }
        catch
        {
            if (!proceso.HasExited) proceso.Kill(entireProcessTree: true);
            try { await copiar; await errores; } catch { }
            throw;
        }
    }
}

