namespace Proyecto_TallerII.Models;

public class Tablero
{
    public int IdTablero { get; set; }
    public int? IdUsuarioPropietario { get; set; }
    public string NombreTablero { get; set; }
    public string DescripcionTablero { get; set; }

    public Tablero() {}
}