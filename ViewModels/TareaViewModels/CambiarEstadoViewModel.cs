using System.ComponentModel.DataAnnotations;
using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.ViewModels;
public class CambiarEstadoViewModel
{
    public int IdTablero { get; set; }
    public int IdTarea { get; set; }
    public int? IdPropietarioTablero { get; set; }
    public int? IdUsuarioAsignado { get; set; }
    public EstadoTarea EstadoTarea { get; set; }
    public string? ColorTarea { get; set; }

    public CambiarEstadoViewModel() {}
}