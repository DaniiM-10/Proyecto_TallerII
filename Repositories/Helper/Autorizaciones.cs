namespace Proyecto_TallerII.Repositories;

public static class Autorizaciones 
{
    public static bool EstaAutenticado(HttpContext context)
    {
        return context.Session.GetInt32("IdUsuario").HasValue;
    }
    public static bool EsAdmin(HttpContext context)
    {
        return context.Session.GetString("Rol") == "Administrador";
    }
    public static int ObtenerIdUsuario(HttpContext context)
    {
        return (int)context.Session.GetInt32("IdUsuario")!;
    }
    public static string ObtenerNombreDeUsuario(HttpContext context)
    {
        return context.Session.GetString("Usuario")!;
    }
    public static string ObtenerRolDelUsuario(HttpContext context)
    {
        return context.Session.GetString("Rol")!;
    }
}