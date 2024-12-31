using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.Repositories;

public interface ITareaRepository
{
    void CrearTarea(Tarea nuevaTarea);
    void EditarTarea(Tarea tareaModificada);
    void EliminarTarea(int idTarea);
    List<Tarea> ListarTodasLasTareas(int idTablero);
    List<Tarea> ListarTareasDeUsuario(int idUsuario, bool sa);
    List<Tarea> ListarTareasSinAsignarAdmin();
    Tarea ObtenerTareaPorId(int idTarea);
    bool ExisteTarea(int idTablero, int idTarea, string nombreTarea);
    void CambiarPropietarioTarea(int idUsuario, int idTarea);
    void CambiarEstadoTarea(int idTarea, int codigoEstado, string colorEstado);
    int CantidadDeTareas(int idTablero);
    int CantidadDeTareasRealizadas(int idTablero);
}