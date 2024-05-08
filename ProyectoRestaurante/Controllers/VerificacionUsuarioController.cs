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

        private bool VerificarCredenciales(string correo, string contraseña)
        {
            bool credencialesValidas = false;
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("usp_VerificarCredenciales", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Contraseña", contraseña);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        credencialesValidas = true;
                    }
                }
            }
            return credencialesValidas;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerificarCredenciales(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (VerificarCredenciales(usuario.Correo, usuario.Contraseña))
                {
                    TempData["Mensaje"] = "Inicio de sesión exitoso.";
                    return Redirect("/Platillo/Index");
                }
                else
                {
                    TempData["Mensaje"] = "Correo electrónico o contraseña incorrectos.";
                }
            }
            return View("Login", usuario);
        }
    }
}