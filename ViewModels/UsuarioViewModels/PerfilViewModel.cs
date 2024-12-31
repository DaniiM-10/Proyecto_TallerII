using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.ViewModels;

public class PerfilViewModel
{
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; }
    public string NombreTablero { get; set; }
    public Rol RolUsuario { get; set; }
    public List<TareaViewModel> ListaMisTareas { get; set; } = new List<TareaViewModel>();
    public List<TareaViewModel> ListaTareasSinAsignar { get; set; } = new List<TareaViewModel>();

    public PerfilViewModel() {}
}