using System.ComponentModel.DataAnnotations;
using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.ViewModels;

public class EditarTareaViewModel
{
    public int IdTarea { get; set; }
    public int IdTablero { get; set; }
    public int? IdUsuarioAsignado { get; set; }

    [Required(ErrorMessage = "El nombre de la tarea es requerido.")]
    public string NombreTarea { get; set; }

    [Required(ErrorMessage = "La descripción de la tarea es requerida.")]
    public string DescripcionTarea { get; set; }

    public EditarTareaViewModel() {}
}
