using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.ViewModels;

public class TareaViewModel
{
    public int IdTareaVM { get; set; }
    public int IdTableroVM { get; set; }
    public int? IdUsuarioAsignadoVM { get; set; }
    public EstadoTarea EstadoTareaVM { get; set; }
    public string DescripcionTareaVM { get; set; }
    public string ColorTareaVM { get; set; }
    public string NombreTareaVM { get; set; }
    public string? NombreUsuarioAsignadoVM { get; set; }
    public string? NombrePropietarioTableroVM { get; set; }
    public string? NombreTableroVM { get; set; }

    public TareaViewModel() {}
}