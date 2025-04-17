using Proyecto_TallerII.ViewModels;

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
    public Tarea(CrearTareaViewModel crearTareaViewModel) {
        IdTablero = crearTareaViewModel.IdTablero;
        NombreTarea = crearTareaViewModel.NombreTarea;
        EstadoTarea = crearTareaViewModel.EstadoTarea;
        IdUsuarioAsignado = crearTareaViewModel.IdUsuarioAsignado;
        DescripcionTarea = crearTareaViewModel.DescripcionTarea;
        ColorTarea = crearTareaViewModel.ColorTarea;
    }
    public Tarea(EditarTareaViewModel editarTareaViewModel) {
        IdTablero = editarTareaViewModel.IdTablero;
        IdTarea = editarTareaViewModel.IdTarea;
        IdUsuarioAsignado = editarTareaViewModel.IdUsuarioAsignado;
        NombreTarea = editarTareaViewModel.NombreTarea;
        DescripcionTarea = editarTareaViewModel.DescripcionTarea;
    }
    
    public static string ObtenerColorPorEstado(EstadoTarea estado)
    {
        return estado switch
        {
            EstadoTarea.ToDo => "737373",
            EstadoTarea.Doing => "8c52ff",
            EstadoTarea.Review => "ffbd59",
            EstadoTarea.Done => "6cbd4a",
            _ => "41b8d5" //color predeterminado
        };
    }
}
