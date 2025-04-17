using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.Helpers;

public static class AuthHelper 
{
    public static bool EstaAutenticado(HttpContext context)
    {
        return context.Session.GetInt32("IdUsuario").HasValue;
    }
    public static bool EsAdmin(HttpContext context)
    {
        return Enum.Parse<Rol>(context.Session.GetString("Rol")!) == Rol.Administrador;
    }
    public static int ObtenerIdUsuario(HttpContext context)
    {
        return (int)context.Session.GetInt32("IdUsuario")!;
    }
    public static string ObtenerNombreDeUsuario(HttpContext context)
    {
        return context.Session.GetString("Usuario")!;
    }
    public static Rol ObtenerRolDelUsuario(HttpContext context)
    {
        return Enum.Parse<Rol>(context.Session.GetString("Rol")!);
    }
}