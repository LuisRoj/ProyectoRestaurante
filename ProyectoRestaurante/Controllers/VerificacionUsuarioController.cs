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

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Usuario oUsuario, string cadena)
        {

            using(SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Contraseña);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                oUsuario.Id = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }

            if (oUsuario.Id != 0)
            {
                TempData["usuario"] = oUsuario;
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