using System.ComponentModel.DataAnnotations;
using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.ViewModels;

public class EditarTableroViewModel
{
    public int IdTablero { get; set; }

    [Required(ErrorMessage = "El nombre del tablero es requerido.")]
    public string NombreDeTablero { get; set; }

    [Required(ErrorMessage = "La descripcion del tablero es requerida.")]
    public string DescripcionDeTablero { get; set; }
    public int? IdUsuarioPropietarioAnterior { get; set; }
    public int? IdUsuarioPropietario { get; set; }
    public List<Usuario>? ListadoUsuarios { get; set; } = new List<Usuario>();

    public EditarTableroViewModel() {}
}