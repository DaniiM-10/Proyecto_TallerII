namespace Proyecto_TallerII.Models;

public enum EstadoTarea
{
    ToDo = 1,  // Hacer
    Doing = 2, // Haciendo
    Review = 3, // Revisar
    Done = 4 // Realizada
}

public class Tarea
{
    public int IdTarea { get; set; }
    public int IdTablero { get; set; }
    public string NombreTarea { get; set; }
    public EstadoTarea EstadoTarea { get; set; }
    public int? IdUsuarioAsignado { get; set; }
    public string DescripcionTarea { get; set; }
    public string ColorTarea { get; set; }

    public Tarea() {}
    
    public string ObtenerColorPorEstado(EstadoTarea estado)
    {
        switch (estado)
        {
            case EstadoTarea.ToDo:
                return "737373";
            case EstadoTarea.Doing:
                return "8c52ff";
            case EstadoTarea.Review:
                return "ffbd59";
            case EstadoTarea.Done:
                return "6cbd4a";
            default:
                return "41b8d5";
        }
    }
}
