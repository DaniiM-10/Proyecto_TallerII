using System.ComponentModel.DataAnnotations;
using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.ViewModels;

public class CrearTableroViewModel
{
    [Required(ErrorMessage = "El nombre del tablero es requerido.")]
    public string NombreDeTablero { get; set; }

    [Required(ErrorMessage = "La descripcion del tablero es requerida.")]
    public string DescripcionDeTablero { get; set; }
    
    public int? IdUsuarioPropietario { get; set; }

    public CrearTableroViewModel() {}
}