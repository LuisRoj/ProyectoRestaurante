using Microsoft.Data.SqlClient;
using ProyectoRestaurante.Models;
using System.Data;

namespace ProyectoRestaurante.Repositorio.RepositorioSQL
{
    public class comentarioSQL : IComentario

    {
        private readonly string cadena;
        public comentarioSQL() {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").
                         Build().GetConnectionString("sql");

        }


        public string insertarComentario(Comentario reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("InsertarComentario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@comentario", reg.comentario);
                    cmd.Parameters.AddWithValue("@id_ciudad", reg.id_ciudad);
                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se a registrado su comentario  {i}";
                }
                catch (Exception ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }
    }
}
