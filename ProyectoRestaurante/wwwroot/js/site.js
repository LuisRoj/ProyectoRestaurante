// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Función para verificar si hay registros en la página
function hayRegistrosEnPagina() {
    var contenedorRegistros = document.querySelector('.contenedor-registros');
    return contenedorRegistros.innerHTML.trim() !== ''; 
}




function aplicarVibracionSiHayRegistros() {
    var botonCarrito = document.querySelector('.carrito-btn');

   
    if (hayRegistrosEnPagina()) {
   
        botonCarrito.classList.add('vibrar');
    } else {
    
        botonCarrito.classList.remove('vibrar');
    }
}


window.onload = function () {
    aplicarVibracionSiHayRegistros();
};
