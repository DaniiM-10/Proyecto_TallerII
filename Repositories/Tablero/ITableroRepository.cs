using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.Repositories;

public interface ITableroRepository
{
    void CrearTablero(Tablero tablero);
    void EditarTablero(Tablero tablero);
    void EliminarTablero(int idTablero);
    Tablero ObtenerTableroPorId(int idTablero);
    List<Tablero> ListaTableros(int idUsuarioMio);
    List<Tablero> ListaTablerosAjenos(int idUsuarioMio);
    bool ExisteTablero(string nombreDeTablero, int idTablero);
}