using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoRestaurante.Models;

namespace ProyectoRestaurante.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IConfiguration _config;
        public ProductoController(IConfiguration config)
        {
            _config = config;
        }

        IEnumerable<Producto> Productos()
        {
            List<Producto> temporal = new List<Producto>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("usp_ListarProductos", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Producto()
                    {
                        Id = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        Descripcion = dr.GetString(2),
                        Precio = dr.GetDecimal(3),
                        Stock = dr.GetInt32(4),

                    });
                }
                dr.Close();
            }
            return temporal;
        }
        public async Task<IActionResult> Productos(int p = 0)
        {
            IEnumerable<Producto> temporal = Productos();

            int fila = 15;
            int c = temporal.Count();
            int pags = c % fila == 0 ? c / fila : c / fila + 1;

            ViewBag.p = p;
            ViewBag.pags = pags;

            return View(await Task.Run(() => temporal.Skip(fila * p).Take(fila)));
        }
    }
}
