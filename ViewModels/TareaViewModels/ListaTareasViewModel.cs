namespace Proyecto_TallerII.ViewModels;

public class ListaTareasViewModel
{
    public int IdTablero;
    public int? IdUsuarioPropietario;
    public string NombreTablero;
    public List<TareaViewModel>? ListaTareasVM { get; set; } = new List<TareaViewModel>();
    
    public ListaTareasViewModel() {}
}