using System.ComponentModel.DataAnnotations;
using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.ViewModels;

public class CrearTareaViewModel
{
    public int IdTablero { get; set; }
    public int? IdPropietarioTablero { get; set; }

    [Required(ErrorMessage = "El nombre de la tarea es requerido.")]
    public string NombreTarea { get; set; }

    [Required(ErrorMessage = "La descripción de la tarea es requerida.")]
    public string DescripcionTarea { get; set; }

    public EstadoTarea EstadoTarea { get; set; }

    public string ColorTarea { get; set; }

    public int? IdUsuarioAsignado { get; set; }
    public List<Usuario>? ListadoUsuariosDisponibles { get; set; } = new List<Usuario>();

    public CrearTareaViewModel() {}
}