using Proyecto_TallerII.ViewModels;
namespace Proyecto_TallerII.Models;

public class Tablero
{
    public int IdTablero { get; set; }
    public int IdUsuarioPropietario { get; set; }
    public string NombreTablero { get; set; }
    public string DescripcionTablero { get; set; }

    public Tablero() {}
    public Tablero(CrearTableroViewModel crearTableroViewModel) {
        NombreTablero = crearTableroViewModel.NombreDeTablero;
        DescripcionTablero = crearTableroViewModel.DescripcionDeTablero;
        IdUsuarioPropietario = crearTableroViewModel.IdUsuarioPropietario;
    }
    public Tablero(CrearTableroAjenoViewModel crearTableroAViewModel) {
        NombreTablero = crearTableroAViewModel.NombreDeTableroA;
        DescripcionTablero = crearTableroAViewModel.DescripcionDeTableroA;
        IdUsuarioPropietario = crearTableroAViewModel.IdUsuarioPropietarioA;
    }
    public Tablero(EditarTableroViewModel editarTableroViewModel) {
        IdTablero = editarTableroViewModel.IdTablero;
        NombreTablero = editarTableroViewModel.NombreDeTablero;
        DescripcionTablero = editarTableroViewModel.DescripcionDeTablero;
        IdUsuarioPropietario = editarTableroViewModel.IdUsuarioPropietario;
    }
}