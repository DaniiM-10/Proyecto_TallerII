using Proyecto_TallerII.ViewModels;

namespace Proyecto_TallerII.Models;

public enum Rol
{
    Administrador = 1,
    Operador = 2
}

public class Usuario
{
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; }
    public string? Password { get; set; } 
    public Rol RolUsuario { get; set; }

    public Usuario() {}
    public Usuario(CrearUsuarioViewModel crearUsuarioViewModel){
        NombreUsuario = crearUsuarioViewModel.NombreUsuario;
        Password = crearUsuarioViewModel.Password;
        RolUsuario = crearUsuarioViewModel.RolUsuario;
    }
    public Usuario(EditarUsuarioViewModel editarUsuarioViewModel){
        IdUsuario = editarUsuarioViewModel.IdUsuario;
        NombreUsuario = editarUsuarioViewModel.NombreUsuario;
        Password = editarUsuarioViewModel.Password;
        RolUsuario = editarUsuarioViewModel.RolUsuario;
    }
}