using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoRestaurante.Models;
using ProyectoRestaurante.Repositorio;
using ProyectoRestaurante.Repositorio.RepositorioSQL;
namespace ProyectoRestaurante.Controllers
{
    public class ComentarioController : Controller
    {
        ICiudad _ciudad;
        IComentario _comentario;

        public ComentarioController()
        {
            _ciudad = new ciudadSQL();
            _comentario = new comentarioSQL();
        }

        public async Task<IActionResult> Create()
        { 
            ViewBag.ciudades=new SelectList(_ciudad.getCiudadcs(),"id_ciudad", "ciudaad");
            return View(await Task.Run(() => new Comentario()));
        }
        [HttpPost]
        public async Task<IActionResult> Create(Comentario reg)
        {
            if (ModelState.IsValid)
            {
                string mensaje = _comentario.insertarComentario(reg);
                if (mensaje.StartsWith("Se a registrado su comentario"))
                {
                    ViewBag.mensaje = mensaje;
                    return RedirectToAction("Create");
                }
                else
                {
                    ViewBag.mensaje = mensaje;
                }
            }

            ViewBag.ciudades = new SelectList(_ciudad.getCiudadcs(), "id_ciudad", "ciudaad", reg.id_ciudad);
            return View(reg);
        }

    }
}
