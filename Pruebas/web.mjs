// Prueba HTTP contra una instancia local de prueba; vacía su catálogo.
// Iniciar la aplicación en http://127.0.0.1:5187 antes de ejecutar.
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
const base = process.env.URL_PRUEBA ?? 'http://127.0.0.1:5187';
const cookies = new Map();
async function pedir(ruta, opciones = {}) {
    const respuesta = await fetch(base + ruta, {
        ...opciones, redirect: 'manual',
        headers: { ...opciones.headers, Cookie: [...cookies].map(([k, v]) => k + '=' + v).join('; ') }
    });
    for (const valor of respuesta.headers.getSetCookie()) {
        const [clave, ...contenido] = valor.split(';')[0].split('=');
        cookies.set(clave, contenido.join('='));
    }
    if (respuesta.status === 302) return pedir(respuesta.headers.get('location'));
    return { status: respuesta.status, tipo: respuesta.headers.get('content-type'), texto: await respuesta.text() };
}
let pagina = await pedir('/');
assert.equal(pagina.status, 200);
function token() {
    const valor = pagina.texto.match(/name="__RequestVerificationToken"[^>]*value="([^"]+)"/);
    assert.ok(valor, 'Falta token antifalsificación');
    return valor[1];
}
async function formulario(accion, datos) {
    pagina = await pedir('/Home/' + accion, {
        method: 'POST', body: new URLSearchParams({ ...datos, __RequestVerificationToken: token() })
    });
    assert.equal(pagina.status, 200);
    return pagina.texto;
}
await formulario('Reiniciar', { confirmar: 'true' });
assert.match(pagina.texto, /<strong>0<\/strong>/);
assert.match(await formulario('AgregarCategoria', { nombre: 'Prueba Web' }), /Categor&#xED;a agregada|Categoría agregada/);
assert.match(await formulario('Registrar', { isbn: '9780000000001', titulo: '<script>alert(1)</script>', autor: 'Autor', categoria: 'Prueba Web' }), /Libro registrado/);
assert.ok(pagina.texto.includes('&lt;script&gt;') && !pagina.texto.includes('<script>alert(1)</script>'));
assert.match(await formulario('Registrar', { isbn: '9780000000001', titulo: 'Duplicado', autor: 'Autor', categoria: 'Prueba Web' }), /Ya existe un libro/);
pagina = await pedir('/?isbn=9780000000001');
assert.match(pagina.texto, /Libro encontrado/);
pagina = await pedir('/?isbn=no-es-numero');
assert.match(pagina.texto, /enteros v/);
await formulario('Eliminar', { isbn: '9780000000001' });
assert.match(pagina.texto, /Libro eliminado/);
await formulario('Reiniciar', { confirmar: 'true' });
for (const archivo of ['Entrada.xml', 'Entrada2.xml']) {
    const body = new FormData();
    body.append('__RequestVerificationToken', token());
    body.append('archivo', new Blob([await readFile(new URL('../Archivos/' + archivo, import.meta.url))], { type: 'text/xml' }), archivo);
    pagina = await pedir('/Home/Cargar', { method: 'POST', body });
    assert.equal(pagina.status, 200);
    assert.match(pagina.texto, /Carga incremental/);
}
assert.match(pagina.texto, /<strong>6<\/strong>/);
pagina = await pedir('/?categoria=Python');
assert.match(pagina.texto, /250/);
assert.ok(!pagina.texto.includes('<td>100</td>'));
const grafico = await pedir('/Home/Reporte?tipo=libros&categoria=Fisica');
assert.equal(grafico.status, 200);
assert.match(grafico.tipo, /image\/svg/);
assert.match(grafico.texto, /<svg/);
const jerarquia = await pedir('/Home/Reporte?tipo=categorias&categoria=Tecnologia&descargar=true');
assert.match(jerarquia.texto, /Programacion/);
assert.ok(!jerarquia.texto.includes('Literatura'));
const ayuda = await pedir('/Home/Ayuda');
assert.equal(ayuda.status, 200);
const sinToken = await pedir('/Home/Eliminar', { method: 'POST', body: new URLSearchParams({ isbn: '100' }) });
assert.equal(sinToken.status, 400);
console.log('OK: HTTP, formularios, validación, carga de ambos XML, escape HTML, consultas, reportes SVG/DOT, ayuda y antifalsificación.');

