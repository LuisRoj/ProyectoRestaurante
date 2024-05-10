using Web_API.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Web_API.Repositorio.DAO
{
	public class UsuarioDAO
	{
		private readonly string _connectionString;

		public UsuarioDAO(IConfiguration config)
		{
			_connectionString = config.GetConnectionString("sql");
		}

		public IEnumerable<Usuarios> ObtenerUsuarios()
		{
			List<Usuarios> usuarios = new List<Usuarios>();

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				SqlCommand command = new SqlCommand("usp_ListarUsuarios", connection);
				command.CommandType = CommandType.StoredProcedure;

				using (SqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Usuarios usuario = new Usuarios
						{
							Id = Convert.ToInt32(reader["id"]),
							Nombre = reader["nombre"].ToString(),
							Apellido = reader["apellido"].ToString(),
							Correo = reader["correo"].ToString(),
							Telefono = reader["telefono"].ToString(),
							Direccion = reader["direccion"].ToString()
						};
						usuarios.Add(usuario);
					}
				}
			}

			return usuarios;
		}

		public Usuarios ObtenerUsuarioPorId(int id)
		{
			Usuarios usuario = null;

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				SqlCommand command = new SqlCommand("usp_BuscarUsuarioPorId", connection);
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@Id", id);

				using (SqlDataReader reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						usuario = new Usuarios
						{
							Id = Convert.ToInt32(reader["id"]),
							Nombre = reader["nombre"].ToString(),
							Apellido = reader["apellido"].ToString(),
							Correo = reader["correo"].ToString(),
							Telefono = reader["telefono"].ToString(),
							Direccion = reader["direccion"].ToString()
						};
					}
				}
			}

			return usuario;
		}

		public void InsertarUsuario(Usuarios usuario)
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				SqlCommand command = new SqlCommand("usp_InsertarUsuario", connection);
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
				command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
				command.Parameters.AddWithValue("@Correo", usuario.Correo);
				command.Parameters.AddWithValue("@Telefono", usuario.Telefono);
				command.Parameters.AddWithValue("@Direccion", usuario.Direccion);

				command.ExecuteNonQuery();
			}
		}

		public void EditarUsuario(Usuarios usuario)
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				SqlCommand command = new SqlCommand("usp_EditarUsuario", connection);
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@Id", usuario.Id);
				command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
				command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
				command.Parameters.AddWithValue("@Correo", usuario.Correo);
				command.Parameters.AddWithValue("@Telefono", usuario.Telefono);
				command.Parameters.AddWithValue("@Direccion", usuario.Direccion);

				command.ExecuteNonQuery();
			}
		}

		public void EliminarUsuario(int id)
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				SqlCommand command = new SqlCommand("usp_EliminarUsuario", connection);
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@Id", id);

				command.ExecuteNonQuery();
			}
		}
	}
}
