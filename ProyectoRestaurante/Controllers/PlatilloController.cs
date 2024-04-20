using Microsoft.AspNetCore.Mvc;
using ProyectoRestaurante.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoRestaurante.Controllers
{
    public class PlatilloController : Controller
    {
        private readonly IConfiguration _config;

        public PlatilloController(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            return View(await Task.Run(() => ListarPlatillos()));
        }

        public List<Platillo> ListarPlatillos()
        {
            List<Platillo> lista = new List<Platillo>();
            using (SqlConnection cnn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("usp_Platillo_Listar", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Platillo
                    {
                        id = dr.GetInt32(0),
                        nombre = dr.GetString(1),
                        descripcion = dr.GetString(2),
                        precio = dr.GetDecimal(3),
                        stock = dr.GetInt32(4),
                        imagen = dr.GetString(5) // Asegúrate de que la ruta de la imagen se almacena en la base de datos
                    });
                }
                dr.Close();
            }
            return lista;
        }
       
    }
}
