using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProyectoRestaurante.Models;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;


namespace RegistroUsuarioRestaurante
{
    public class UsuarioController : Controller
    {
        private readonly IConfiguration _config;

        public UsuarioController(IConfiguration config)
        {
            _config = config;
        }
        private void InsertarUsuarioEnBaseDeDatos(Usuario usuario)
        {
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("usp_ListarUsuarios", cn);

                cmd.Parameters.AddWithValue("@Id", usuario.Id);
                cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                cmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                cmd.Parameters.AddWithValue("@Direccion", usuario.Direccion);
                cmd.ExecuteNonQuery();
            }
        }
      

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                InsertarUsuarioEnBaseDeDatos(usuario);
                TempData["Mensaje"] = "Usuario registrado correctamente.";
                return RedirectToAction("Index", "Platillo"); // Redirige al usuario a la página de prodcutos
            }
            else
            {
                TempData["Mensaje"] = "Por favor, complete correctamente todos los campos.";
                return RedirectToAction("Registrar");
            }
        }
    }
}
