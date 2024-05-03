using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoRestaurante.Models;
using System.Data;

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

        public List<Producto> seleccionarProductos(int? idProducto = null)
        {
            List<Producto> temporal = new List<Producto>();
            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_SeleccionarProductoPorId", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", idProducto);
                    SqlDataReader dr = cmd.ExecuteReader();

                    // Verificar si hay filas antes de intentar leer datos
                    if (dr.HasRows)
                    {
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
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones: puedes registrar el error o lanzar una excepción personalizada
                Console.WriteLine("Error al buscar el producto: " + ex.Message);
            }
            return temporal;
        }

        public void agregarProducto(Producto nuevoProducto)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_GuardarProducto", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nombre", nuevoProducto.Nombre);
                    cmd.Parameters.AddWithValue("@descripcion", nuevoProducto.Descripcion);
                    cmd.Parameters.AddWithValue("@precio", nuevoProducto.Precio);
                    cmd.Parameters.AddWithValue("@stock", nuevoProducto.Stock);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar el producto: " + ex.Message);
            }
        }

        public void editarProducto(Producto producto)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_ActualizarProducto", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", producto.Id);
                    cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                    cmd.Parameters.AddWithValue("@precio", producto.Precio);
                    cmd.Parameters.AddWithValue("@stock", producto.Stock);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al editar el producto: " + ex.Message);
            }
        }

        public void eliminarProducto(Producto producto)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_EliminarProducto", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", producto.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar el producto: " + ex.Message);
            }
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

        public async Task<IActionResult> Create()
        {
            return View(await Task.Run(() => new Producto()));
        }

        public async Task<IActionResult> Edit(int? id = null)
        {
            if (id == null)
                return RedirectToAction("Productos");
            Producto producto = seleccionarProductos(id).FirstOrDefault();
            return View(await Task.Run(() => producto));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
                return RedirectToAction("Productos");
            Producto producto = seleccionarProductos(id).FirstOrDefault();
            return View(await Task.Run(() => producto));
        }

        //-----------------------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto reg)
        {
            if (!ModelState.IsValid)
            {
                return View(await Task.Run(() => reg));
            }

            agregarProducto(reg);
            return RedirectToAction(nameof(Productos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Producto reg)
        {
            if (!ModelState.IsValid)
            {
                return View(await Task.Run(() => reg));
            }

            editarProducto(reg);
            return RedirectToAction(nameof(Productos));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Producto producto)
        {
            if (producto.Id == null)
                return RedirectToAction("Productos");
            eliminarProducto(producto);
            return RedirectToAction("Productos");
        }
    }
}
