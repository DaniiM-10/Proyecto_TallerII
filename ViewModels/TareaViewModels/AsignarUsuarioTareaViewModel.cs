using System.ComponentModel.DataAnnotations;
using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.ViewModels;

public class AsignarUsuarioTareaViewModel
{
    public int IdTarea { get; set; }
    public int IdTablero { get; set; }
    public int? IdUsuarioAsignadoActual { get; set; }
    public int? IdPropietarioTablero { get; set; }
    public string? NombreTarea { get; set; }
    public string? Tipo { get; set; }

    [Required(ErrorMessage = "Debe elegir un usuario.")]
    public int IdUsuario { get; set; }
    public List<Usuario>? ListadoUsuariosDisponibles { get; set; } = new List<Usuario>();
    public AsignarUsuarioTareaViewModel() {}
}