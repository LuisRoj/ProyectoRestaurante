using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoRestaurante.Models;
using System.Data;

namespace ProyectoRestaurante.Controllers
{
    public class VerificacionUsuarioController : Controller
    {
        private readonly IConfiguration _config;

        public VerificacionUsuarioController(IConfiguration config)
        {
            _config = config;
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Usuario oUsuario)
        {
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                SqlCommand cmd = new SqlCommand("sp_ValidadUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Contraseña", oUsuario.Contraseña);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                oUsuario.Id = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }

            if (oUsuario.Id != 0)
            {
                TempData["usuarioId"] = oUsuario.Id;
                return RedirectToAction("Index", "Platillo");
            }
            else
            {
                ViewData["Mensaje"] = "Usuario no encontrado";
                return View("Login");
            }
        }

    }
}