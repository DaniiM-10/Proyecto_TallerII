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
    public string Password { get; set; }
    public Rol RolUsuario { get; set; }

    public Usuario() {}
}