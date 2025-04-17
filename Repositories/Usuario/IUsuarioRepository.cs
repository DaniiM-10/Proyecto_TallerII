using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.Repositories;

public interface IUsuarioRepository
{
    void CrearUsuario(Usuario usuario);
    void EditarUsuario(Usuario usuario);
    void EliminarUsuario(int idUsuario);
    List<Usuario> ObtenerTodosLosUsuarios();
    Usuario ObtenerUsuarioPorId(int idUsuario);
    Usuario ObtenerUsuarioPorCredenciales(string nombreUsuario, string passwordUsuario);
    string ObtenerNombreUsuario(int idUsuario);
    bool ExisteUsuario(string nombreUsuario, int idUsuario);
}