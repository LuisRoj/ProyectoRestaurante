using Web_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web_API.Repositorio.Interfaces
{
	public class iUsuario
	{
		IEnumerable<Usuario> ObtenerUsuarios();
		Usuario ObtenerUsuarioPorId(int id);
		string InsertarUsuario(Usuario usuario);
		string EditarUsuario(Usuario usuario);
		void EliminarUsuario(int id);
	}
}
