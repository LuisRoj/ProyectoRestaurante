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

        private List<Usuario> ListarUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("usp_ListarUsuarios", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    usuarios.Add(new Usuario()
                    {
                        Nombre = dr.GetString(0),
                        Apellido = dr.GetString(1),
                        Correo = dr.GetString(2),
                        Telefono = dr.GetString(3),
                        Direccion = dr.GetString(4)
                    });
                }
            }
            return usuarios;
        }
        private void InsertarUsuarioEnBaseDeDatos(Usuario usuario)
        {
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("usp_InsertarUsuario", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", usuario.Id);
                cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                cmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                cmd.Parameters.AddWithValue("@Direccion", usuario.Direccion);
                cmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña); 

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
                return RedirectToAction("Index", "Producto"); // Redirige al usuario a la página de prodcutos
            }
            else
            {
                TempData["Mensaje"] = "Por favor, complete correctamente todos los campos.";
                return RedirectToAction("Registrar");
            }
        }
        [HttpPost]
        public IActionResult Registro(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (usuario.Contraseña != usuario.ConfirmarContraseña)
                {
                    ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden.");
                    return View(usuario);
                }

                InsertarUsuarioEnBaseDeDatos(usuario);
                TempData["Mensaje"] = "¡Usuario registrado exitosamente!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(usuario);
            }
        }

        public IActionResult Listar()
        {
            var usuarios = ListarUsuarios();
            return View(usuarios);
        }
    }

}
