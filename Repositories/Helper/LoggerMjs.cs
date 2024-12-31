namespace Proyecto_TallerII.Repositories;

public static class LoggerMsj 
{
    public static string MensajeEx(Exception ex, string msj)
    {
        var mensaje = "Error mensaje: " + msj;
        mensaje = mensaje + "Error message: " + ex.Message;
        // Información sobre la excepción interna
        if (ex.InnerException != null)
        {
            mensaje = mensaje + " Inner exception: " + ex.InnerException.Message;
        }
        // Dónde ha sucedido
        mensaje = mensaje + " Stack trace: " + ex.StackTrace;
        mensaje = mensaje + " Date: "+ DateTime.Now.ToString("HH:mm_dd/MM/yyyy");
        
        return mensaje;
    }

    public static string MensajeInfoWarn(string msj)
    {
        string fechaHora = DateTime.Now.ToString("HH:mm_dd/MM/yyyy");
        return $"{fechaHora} -> \"{msj}\"";
    }
}