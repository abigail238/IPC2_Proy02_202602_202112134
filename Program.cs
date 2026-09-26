using IPC2_Proy02_202602_202112134.Servicios;
// Obtenemos la carpeta raíz real del proyecto.
// AppContext.BaseDirectory actualmente apunta a:
// bin\Debug\net10.0
string rutaProyecto = Directory.GetParent(AppContext.BaseDirectory)!
    .Parent!
    .Parent!
    .Parent!
    .FullName;


// Le indicamos a ASP.NET dónde está realmente el proyecto
var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions
    {
        Args = args,

        // Carpeta raíz del proyecto
        ContentRootPath = rutaProyecto,

        // Carpeta que contiene CSS, JS e imágenes
        WebRootPath = Path.Combine(
            rutaProyecto,
            "wwwroot"
        )
    }
);
builder.Services.AddControllersWithViews();
// Una misma instancia conserva los árboles entre las peticiones de la web.
// El catálogo inicia vacío; el usuario decide cuándo cargar los XML.
builder.Services.AddSingleton<Catalogo>();
builder.Services.AddSingleton<GeneradorGraphviz>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.Run();

