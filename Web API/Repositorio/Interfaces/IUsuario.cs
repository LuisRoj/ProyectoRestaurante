using Web_API.Models;

namespace Web_API.Repositorio.Interfaces
{
    public interface IUsuario
    {
        Task<IEnumerable<Usuarios>> ObtenerUsuarios();
        Task<Usuarios> ObtenerUsuario(int id);
        Task<string> InsertarUsuario(Usuarios usuario);
        Task<string> EditarUsuario(Usuarios usuario);
        Task EliminarUsuario(int id);
    }
}
