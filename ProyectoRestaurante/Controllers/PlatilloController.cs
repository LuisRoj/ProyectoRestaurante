using Microsoft.AspNetCore.Mvc;
using ProyectoRestaurante.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ProyectoRestaurante.Controllers
{
    public class PlatilloController : Controller
    {
        private readonly IConfiguration _config;

        public PlatilloController(IConfiguration config)
        {
            _config = config;
        }

        public ActionResult Carrito()
        {
            List<Registro> carrito = obtenerCarrito();
            return View(carrito);
        }
        List<Registro> obtenerCarrito()
        {
            if (HttpContext.Session.GetString("cadena") != null)
            {
                string jsonCarrito = HttpContext.Session.GetString("cadena");
                return JsonConvert.DeserializeObject<List<Registro>>(jsonCarrito);
            }
            return new List<Registro>();
        }

        public ActionResult IrAlCarrito() 
        {
            return RedirectToAction("carrito");
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

        Platillo buscar(int id)
        {
            return ListarPlatillos().FirstOrDefault(c => c.id == id);
        }
        public ActionResult Select(int id)
        {
            Platillo platillo = buscar(id);
            return View(platillo);
        }

        [HttpPost]
        public ActionResult Select(int id, int cantidad)
        {
            
            Platillo platillo = buscar(id);

            List<Registro> auxiliar = new List<Registro>();
            if (HttpContext.Session.GetString("cadena") != null)
            {
                string jsonCarrito = HttpContext.Session.GetString("cadena");
                auxiliar = JsonConvert.DeserializeObject<List<Registro>>(jsonCarrito);
            }

            Registro item = auxiliar.FirstOrDefault(p => p.id == id);
            if (item != null)
            {
                item.cantidad += cantidad;
            }
            else
            {
                item = new Registro()
                {
                    id = platillo.id,
                    nombre = platillo.nombre,
                    precio = platillo.precio,
                    cantidad = cantidad,
                    Imagen = platillo.imagen 
                };
                auxiliar.Add(item);
            }

            HttpContext.Session.SetString("cadena", JsonConvert.SerializeObject(auxiliar));
            ViewBag.mensaje = $"Tiene un pedido de {item.cantidad} unidades del producto {item.nombre}.";
            return View("Select", platillo);
        }

        public ActionResult Eliminar(int id)
        {
            List<Registro> auxiliar = obtenerCarrito();

            Registro productoEliminar = auxiliar.FirstOrDefault(p => p.id == id);

            if (productoEliminar != null)
            {
                auxiliar.Remove(productoEliminar);
                HttpContext.Session.SetString("cadena", JsonConvert.SerializeObject(auxiliar));
                ViewBag.elimi = $"Se ha eliminado el producto {productoEliminar.nombre} del carrito.";
            }
            else
            {
                ViewBag.notelimi = $"No se encontró el producto en el carrito.";
            }

            return RedirectToAction("Carrito");
        }


        public ActionResult Pedido()
        {
            // Recupera los registros de la sesión carrito
            List<Registro> auxiliar = null;
            string jsonCarrito = HttpContext.Session.GetString("cadena");
            if (!string.IsNullOrEmpty(jsonCarrito))
            {
                auxiliar = JsonConvert.DeserializeObject<List<Registro>>(jsonCarrito);
            }

            // Si no hay registros, redirigir al Index
            if (auxiliar == null || auxiliar.Count == 0)
            {
                return RedirectToAction("Index");
            }

            // Si hay registros, mostrar el carrito
            ViewBag.carrito = auxiliar;
            return View(new Pedido());
        }

        [HttpPost]
        public ActionResult Pedido(Pedido reg)
        {
            string mensaje = "";
            string npedido = ""; // Declarar npedido fuera del bloque try

            using (SqlConnection cn = new SqlConnection(
                _config.GetConnectionString("sql")))
            {
                cn.Open();
                SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);

                try
                {
                    IEnumerable<Registro> auxiliar = null;
                    string jsonCarrito = HttpContext.Session.GetString("cadena");
                    if (!string.IsNullOrEmpty(jsonCarrito))
                    {
                        auxiliar = JsonConvert.DeserializeObject<List<Registro>>(jsonCarrito);
                    }

                    if (auxiliar != null && auxiliar.Any())
                    {
                        // Llamar al procedimiento almacenado usp_npedido_add para generar el número de pedido
                        SqlCommand cmdAddPedido = new SqlCommand("usp_npedido_add", cn, tr);
                        cmdAddPedido.CommandType = CommandType.StoredProcedure;
                        cmdAddPedido.Parameters.AddWithValue("@dnicliente", reg.dnicliente);
                        cmdAddPedido.Parameters.AddWithValue("@nomcliente", reg.nombrecliente);
                        cmdAddPedido.Parameters.AddWithValue("@emailcliente", reg.emailcliente);
                        cmdAddPedido.Parameters.AddWithValue("@fonocliente", reg.fonocliente);
                        npedido = cmdAddPedido.ExecuteScalar().ToString();

                        // Verificar que el número de pedido se haya generado correctamente
                        SqlCommand cmdVerificarPedido = new SqlCommand("SELECT COUNT(*) FROM tb_pedido WHERE npedido = @npedido", cn, tr);
                        cmdVerificarPedido.Parameters.AddWithValue("@npedido", npedido);
                        int pedidoCount = (int)cmdVerificarPedido.ExecuteScalar();

                        if (pedidoCount > 0)
                        {
                            // Llamar al procedimiento almacenado usp_npedido_detalle_adds para insertar los detalles del pedido
                            foreach (var item in auxiliar)
                            {
                                SqlCommand cmdAddDetalle = new SqlCommand("usp_npedido_detalle_adds", cn, tr);
                                cmdAddDetalle.CommandType = CommandType.StoredProcedure;
                                cmdAddDetalle.Parameters.AddWithValue("@npedido", npedido);
                                cmdAddDetalle.Parameters.AddWithValue("@IdProducto", item.id);
                                cmdAddDetalle.Parameters.AddWithValue("@PrecioUnidad", item.precio);
                                cmdAddDetalle.Parameters.AddWithValue("@Cantidad", item.cantidad);
                                cmdAddDetalle.ExecuteNonQuery();
                            }

                            tr.Commit(); // Commit si todo está bien
                            mensaje = $"Se ha registrado el pedido código {npedido} satisfactoriamente. Por favor copie su código si desea cancelar el pedido";
                        }
                        else
                        {
                            mensaje = "No se pudo generar el número de pedido.";
                        }
                    }
                    else
                    {
                        mensaje = "No hay productos en el carrito.";
                    }
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    tr.Rollback(); // Rollback en caso de error
                }
                finally
                {
                    cn.Close();
                }
            }

            // Redireccionar a la acción "Mensaje" con los datos del pedido
            return RedirectToAction("Mensaje", new { idPedido = npedido, msj = mensaje });
        }



        public ActionResult Mensaje(string npedido, string msj)
        {
            ViewBag.mensaje = msj;
            ViewBag.npedido = npedido;

            // Limpiar la sesión
            HttpContext.Session.Clear();

            return View();
        }


        public async Task<IActionResult> Detalle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Platillo platillo = await ObtenerPlatillo(id.Value);

            if (platillo == null)
            {
                return NotFound();
            }

            return View(platillo);
        }

        private async Task<Platillo> ObtenerPlatillo(int id)
        {
            Platillo platillo = null;

            using (SqlConnection cnn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                await cnn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("usp_Platillo_ObtenerPorId", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader dr = await cmd.ExecuteReaderAsync();

                    if (await dr.ReadAsync())
                    {
                        platillo = new Platillo
                        {
                            id = dr.GetInt32(0),
                            nombre = dr.GetString(1),
                            descripcion = dr.GetString(2),
                            precio = dr.GetDecimal(3),
                            stock = dr.GetInt32(4),
                            imagen = dr.GetString(5)
                        };
                    }
                }
            }

            return platillo;
        }





    }
}
