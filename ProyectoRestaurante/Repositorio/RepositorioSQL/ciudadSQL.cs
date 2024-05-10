using Microsoft.Data.SqlClient;
using ProyectoRestaurante.Models;
using System.Data;

namespace ProyectoRestaurante.Repositorio.RepositorioSQL
{
    public class ciudadSQL : ICiudad
    {
        private readonly string cadena;

        public ciudadSQL()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").
                         Build().GetConnectionString("sql");

        }


        public IEnumerable<Ciudadcs> getCiudadcs()
        {
            List<Ciudadcs> temporal = new List<Ciudadcs>();
            using (SqlConnection cn = new SqlConnection(cadena))
            { 
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ListarCiudades", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Ciudadcs()
                    {
                        id_ciudad = dr.GetInt32(0),
                        ciudaad = dr.GetString(1),
                    });
                    
                }
                dr.Close();
            }
            return temporal;
        }
    }
}
