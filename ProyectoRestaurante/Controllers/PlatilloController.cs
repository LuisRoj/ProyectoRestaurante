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

        // 08/05/2024

        // filtro Jhon

        public async Task<IActionResult> Index(string nombreBuscar = null, int pagina = 0)
        {
            List<Platillo> platillos = await Task.Run(() => ListarPlatillos(nombreBuscar));

            int fila = 6;
            int totalPlatillos = platillos.Count();
            int totalPaginas = totalPlatillos % fila == 0 ? totalPlatillos / fila : totalPlatillos / fila + 1;

            ViewBag.TotalPaginas = totalPaginas;

            pagina = Math.Max(Math.Min(pagina, totalPaginas - 1), 0);

            ViewBag.PaginaActual = pagina;

            platillos = platillos.Skip(fila * pagina).Take(fila).ToList();

            return View(platillos);
        }



        private List<Platillo> FiltrarPlatillosPorNombre(List<Platillo> platillos, string nombreBuscar)
        {
            return platillos.Where(p => p.nombre.Contains(nombreBuscar)).ToList();
        }


        public List<Platillo> ListarPlatillos(string nombreBuscar = "")
        {
            List<Platillo> lista = new List<Platillo>();
            using (SqlConnection cnn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                cnn.Open();
                SqlCommand cmd;
                if (string.IsNullOrWhiteSpace(nombreBuscar))
                {
                    // Si no se proporciona un nombre de búsqueda, simplemente listar todos los platillos
                    cmd = new SqlCommand("usp_Platillo_Listar", cnn);
                }
                else
                {
                    // Proporcione un filtro JHONNNNNN
                    cmd = new SqlCommand("sp_FiltrarPlatillosPorNombre", cnn);
                    cmd.Parameters.AddWithValue("@NombreBuscar", nombreBuscar);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Platillo platillo = new Platillo
                    {
                        id = dr.GetInt32(0),
                        nombre = dr.GetString(1),
                        descripcion = dr.GetString(2),
                        precio = dr.GetDecimal(3),
                        stock = dr.GetInt32(4)
                    };

                    // Verificar si el valor de la columna de la imagen es nulo
                    if (!dr.IsDBNull(5))
                    {
                        platillo.imagen = dr.GetString(5);
                    }
                    else
                    {
                        // Si es nulo, asignar un valor predeterminado o dejarlo como null, según sea necesario
                        platillo.imagen = null; // O puedes asignar un valor predeterminado como una ruta de imagen genérica
                    }

                    lista.Add(platillo);
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

        public void agregarPlatillo(Platillo nuevoPlatillo)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_AgregarPlatillo", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nombre", nuevoPlatillo.nombre);
                    cmd.Parameters.AddWithValue("@descripcion", nuevoPlatillo.descripcion);
                    cmd.Parameters.AddWithValue("@precio", nuevoPlatillo.precio);
                    cmd.Parameters.AddWithValue("@stock", nuevoPlatillo.stock);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar el platillo: " + ex.Message);
            }
        }

        public void editarPlatillo(Platillo platillo)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_EditarPlatillo", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", platillo.id);
                    cmd.Parameters.AddWithValue("@nombre", platillo.nombre);
                    cmd.Parameters.AddWithValue("@descripcion", platillo.descripcion);
                    cmd.Parameters.AddWithValue("@precio", platillo.precio);
                    cmd.Parameters.AddWithValue("@stock", platillo.stock);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al editar el producto: " + ex.Message);
            }
        }

        public List<Platillo> seleccionarPlatillos(int? idPlatillo = null)
        {
            List<Platillo> temporal = new List<Platillo>();
            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_SeleccionarPlatilloPorId", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", idPlatillo);
                    SqlDataReader dr = cmd.ExecuteReader();

                    // Verificar si hay filas antes de intentar leer datos
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            temporal.Add(new Platillo()
                            {
                                id = dr.GetInt32(0),
                                nombre = dr.GetString(1),
                                descripcion = dr.GetString(2),
                                precio = dr.GetDecimal(3),
                                stock = dr.GetInt32(4),
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

        public void eliminarPlatillo(Platillo platillo)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_EliminarPlatillo", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", platillo.id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar el producto: " + ex.Message);
            }
        }


        // Método para buscar un pedido por su número de pedido (npedido)
        public Pedido BuscarPedidoPorNpedido(string npedido)
        {
            Pedido pedido = null;

            using (SqlConnection cnn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("usp_BuscarPedidoPorNpedido", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@npedido", npedido);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    pedido = new Pedido
                    {
                        id = dr.GetInt32(0),
                        npedido = dr.GetString(1),
                        fpedido = dr.GetDateTime(2),
                        dnicliente = dr.GetString(3),
                        nombrecliente = dr.GetString(4),
                        emailcliente = dr.GetString(5),
                        fonocliente = dr.GetString(6)
                    };
                }

                dr.Close();
            }

            return pedido;
        }


        private DetallePedido BuscarDetallePedidoPorNpedido(string npedido)
        {
            DetallePedido detallePedido = null;

            using (SqlConnection cnn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("usp_BuscarDetallePedidoPorNpedido", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@npedido", npedido);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    detallePedido = new DetallePedido
                    {
                        Id = dr.GetInt32(0),
                        Npedido = dr.GetString(1),
                        FechaPedido = dr.GetDateTime(2),
                        NombrePlatillo = dr.GetString(3),
                        Precio = dr.GetDecimal(4),
                        Cantidad = dr.GetInt32(5),
                        Subtotal = dr.GetDecimal(6)
                    };
                }

                dr.Close();
            }

            return detallePedido;
        }


        public IActionResult BuscarPedido()
        {
            return View();
        }

        /*public IActionResult CancelarPedido(string npedido)
        {
            var detallePedido = BuscarDetallePedidoPorNpedido(npedido);
            return View(detallePedido);
        }*/

        public async Task<IActionResult> CancelarPedido(string npedido)
        {
            DetallePedido detallePedido = BuscarDetallePedidoPorNpedido(npedido);
            return View(await Task.Run(() => detallePedido));
        }


        public async Task<IActionResult> Platillos(int p = 0)
        {
            IEnumerable<Platillo> temporal = ListarPlatillos();

            int fila = 15;
            int c = temporal.Count();
            int pags = c % fila == 0 ? c / fila : c / fila + 1;

            ViewBag.p = p;
            ViewBag.pags = pags;

            return View(await Task.Run(() => temporal.Skip(fila * p).Take(fila)));
        }

        public async Task<IActionResult> EditPlatillos(int? id = null)
        {
            if (id == null)
                return RedirectToAction("Platillos");
            Platillo platillo = seleccionarPlatillos(id).FirstOrDefault();
            return View(await Task.Run(() => platillo));
        }

        public async Task<IActionResult> CreatePlatillo()
        {
            return View(await Task.Run(() => new Platillo()));
        }

        public async Task<IActionResult> DeletePlatillos(int id)
        {
            if (id == null)
                return RedirectToAction("Platillos");
            Platillo platillo = seleccionarPlatillos(id).FirstOrDefault();
            return View(await Task.Run(() => platillo));
        }
  
        [HttpPost]
        public IActionResult BuscarPedido(string npedido)
        {
            // Busca el pedido por su número de pedido
            Pedido pedidoEncontrado = BuscarPedidoPorNpedido(npedido);

            if (pedidoEncontrado == null)
            {
                ViewBag.ErrorMessage = "No se encontró ningún pedido con el número de pedido especificado.";
                return View();
            }

            // Redirige a la acción DetallePedido y pasa el número de pedido
            return RedirectToAction("CancelarPedido", new { npedido = pedidoEncontrado.npedido });
        }


        [HttpPost]
        public IActionResult CancelarPedido(DetallePedido detallePedido)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_cancelar_pedido", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@npedido", detallePedido.Npedido);

                    string mensaje = cmd.ExecuteScalar()?.ToString();

                    ViewBag.SuccessMessage = mensaje ?? "El pedido ha sido cancelado correctamente.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al cancelar el pedido: " + ex.Message;
            }

            // Retorna la misma vista con los mensajes
            return View();
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlatillo(Platillo reg)
        {
            if (!ModelState.IsValid)
            {
                return View(await Task.Run(() => reg));
            }

            agregarPlatillo(reg);
            return RedirectToAction(nameof(Platillos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlatillos(Platillo reg)
        {
            if (!ModelState.IsValid)
            {
                return View(await Task.Run(() => reg));
            }

            editarPlatillo(reg);
            return RedirectToAction(nameof(Platillos));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePlatillos(Platillo platillo)
        {
            if (platillo.id == null)
                return RedirectToAction("Platillos");
            eliminarPlatillo(platillo);
            return RedirectToAction("Platillos");
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

        //filtros Jhon







    }
}
