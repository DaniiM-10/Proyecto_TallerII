using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.ViewModels;

public class TableroViewModel
{
    public int IdTableroVM { get; set; }
    public int? IdUsuarioPropietarioVM { get; set; }
    public string NombreTableroVM { get; set; }
    public string DescripcionTableroVM { get; set; }
    public string NombrePropietarioTableroVM { get; set; }
    public int CantidadTareasVM { get; set; }
    public int CantidadTareasRealizadasVM { get; set; }

    public TableroViewModel() {}
}