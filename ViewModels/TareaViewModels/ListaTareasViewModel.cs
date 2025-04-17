namespace Proyecto_TallerII.ViewModels;

public class ListaTareasViewModel
{
    public int IdTablero { get; set; }
    public int IdUsuarioPropietario { get; set; }
    public string NombreTablero { get; set; }
    public string NombreUsuarioPropietario { get; set; }
    public List<TareaViewModel>? ListaTareasVM { get; set; } = new List<TareaViewModel>();
    
    public ListaTareasViewModel() {}
}