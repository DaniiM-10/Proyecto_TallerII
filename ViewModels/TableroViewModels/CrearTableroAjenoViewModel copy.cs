using System.ComponentModel.DataAnnotations;
using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.ViewModels;

public class CrearTableroAjenoViewModel
{
    [Required(ErrorMessage = "El nombre del tablero es requerido.")]
    public string NombreDeTableroA { get; set; }

    [Required(ErrorMessage = "La descripcion del tablero es requerida.")]
    public string DescripcionDeTableroA { get; set; }

    public int? IdUsuarioPropietarioA { get; set; }

    public List<Usuario>? ListadoUsuarios { get; set; } = new List<Usuario>();

    public CrearTableroAjenoViewModel() {}
}