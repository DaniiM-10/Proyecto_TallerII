using Microsoft.AspNetCore.Mvc;
using Proyecto_TallerII.Models;
using Proyecto_TallerII.Repositories;
using Proyecto_TallerII.Helpers;
using Proyecto_TallerII.ViewModels;
namespace Proyecto_TallerII.Controllers;

public class TareaController : Controller
{
    private readonly ILogger<TareaController> _logger;
    private readonly ITareaRepository _tareaRepository;
    private readonly ITableroRepository _tableroRepository; 
    private readonly IUsuarioRepository _usuarioRepository; 
    public TareaController(ILogger<TareaController> logger, ITareaRepository tareaRepository, ITableroRepository tableroRepository, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _tareaRepository = tareaRepository;
        _tableroRepository = tableroRepository;
        _usuarioRepository = usuarioRepository;
    }

    public IActionResult Index(int idTablero)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método Index del controlador de Tarea."));
                return StatusCode(404);
            }

            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);

            var tablero = _tableroRepository.ObtenerTableroPorId(idTablero);
            List<Tarea> tareas = _tareaRepository.ListarTodasLasTareas(idTablero);
            Tarea siPoseeTareas = tareas.FirstOrDefault(t => t.IdUsuarioAsignado == idUsuarioLogueado)!;

            if(esAdmin || tablero.IdUsuarioPropietario == idUsuarioLogueado || siPoseeTareas != null)
            {    
                var tareasVM = tareas.Select(t => new TareaViewModel
                {
                    IdTareaVM = t.IdTarea,
                    IdTableroVM = t.IdTablero,
                    IdUsuarioAsignadoVM = t.IdUsuarioAsignado,
                    NombreTareaVM = t.NombreTarea,
                    DescripcionTareaVM = t.DescripcionTarea,
                    EstadoTareaVM = t.EstadoTarea,
                    ColorTareaVM = t.ColorTarea,
                    NombreUsuarioAsignadoVM = _usuarioRepository.ObtenerNombreUsuario(t.IdUsuarioAsignado ?? 0)
                }).ToList();

                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se mostraron todas las tareas correctamente."));
                return View(ConvertirAListaTareasViewModel(tablero, tareasVM));
            }

            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método Index del controlador Tarea."));
            return StatusCode(500);
        }
    }

    [HttpPost]
    public IActionResult CambiarEstado(CambiarEstadoViewModel cambiarEstadoViewModel)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CambiarEstado del controlador de Tarea."));
                return StatusCode(404);
            }

            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);

            if((esAdmin || (cambiarEstadoViewModel.IdPropietarioTablero == idUsuarioLogueado || cambiarEstadoViewModel.IdUsuarioAsignado == idUsuarioLogueado)) && cambiarEstadoViewModel.IdUsuarioAsignado != null)
            {
                if (!ModelState.IsValid)
                {
                    TempData["Mensaje"] = "Por favor, Ingrese los datos correctamente.";
                    _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de cambiar el estado a una tarea con datos no válidos."));
                    return RedirectToAction("Index", new { idTablero = cambiarEstadoViewModel.IdTablero });
                }

                cambiarEstadoViewModel.ColorTarea = Tarea.ObtenerColorPorEstado(cambiarEstadoViewModel.EstadoTarea);

                _tareaRepository.CambiarEstadoTarea(cambiarEstadoViewModel.IdTarea, (int)cambiarEstadoViewModel.EstadoTarea, cambiarEstadoViewModel.ColorTarea);
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se cambio el estado de la tarea correctamente."));
                return RedirectToAction("Index", new { idTablero = cambiarEstadoViewModel.IdTablero });
            }

            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
            return RedirectToAction("Index", new { idTablero = cambiarEstadoViewModel.IdTablero });
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CambiarEstado del controlador Tarea."));
            return StatusCode(500);
        }
    }

    [HttpGet]
    public IActionResult CrearTarea(int idTablero)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearTarea del controlador de Tarea."));
                return StatusCode(404);
            }

            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            var tablero = _tableroRepository.ObtenerTableroPorId(idTablero);
            if(esAdmin || tablero.IdUsuarioPropietario == idUsuarioLogueado)
            {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Accediendo a la vista CrearTarea para el tablero con ID: {idTablero}."));
                return View(new CrearTareaViewModel()
                {
                    IdTablero = idTablero,
                    IdPropietarioTablero = tablero.IdUsuarioPropietario,
                    ListadoUsuariosDisponibles = _usuarioRepository.ObtenerTodosLosUsuarios()
                });
            }

            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearTarea del controlador Tarea."));
            return StatusCode(500);
        }
    }
    [HttpPost]
    public IActionResult CrearTarea(CrearTareaViewModel crearTareaViewModel)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearTarea del controlador de Tarea."));
                return StatusCode(404);
            }

            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            if(esAdmin || crearTareaViewModel.IdPropietarioTablero == idUsuarioLogueado)
            {
                if (!ModelState.IsValid)
                {
                    TempData["Mensaje"] = "Por favor, Ingrese los datos correctamente.";
                    _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear una tarea con datos no válidos."));
                    return View(crearTareaViewModel);
                }

                if (_tareaRepository.ExisteTarea(crearTareaViewModel.IdTablero, 0, crearTareaViewModel.NombreTarea!))
                {
                    ModelState.AddModelError("NombreTarea", "El nombre de la tarea ya existe.");
                    _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear una tarea con un nombre existente."));
                    crearTareaViewModel.ListadoUsuariosDisponibles = _usuarioRepository.ObtenerTodosLosUsuarios();
                    return View(crearTareaViewModel);
                }

                _tareaRepository.CrearTarea(new Tarea(crearTareaViewModel));
                TempData["Mensaje"] = "Se creó una nueva tarea.";
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se ha creado exitosamente una nueva tarea."));
                return RedirectToAction("Index", new { idTablero = crearTareaViewModel.IdTablero });
            } 

            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearTarea del controlador Tarea."));
            return StatusCode(500);
        }
    }

    [HttpGet]
    public IActionResult AsignarCambiarUsuarioTarea(int idTablero, int idTarea)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método AsignarCambiarUsuarioTarea del controlador de Tablero."));
                return StatusCode(404);
            }

            var tablero = _tableroRepository.ObtenerTableroPorId(idTablero);
            var tarea = _tareaRepository.ObtenerTareaPorId(idTarea);
            var usuarios = _usuarioRepository.ObtenerTodosLosUsuarios();
            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            if(esAdmin || tablero.IdUsuarioPropietario == idUsuarioLogueado)
            {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Accediendo a la vista Asignar/Cambiar Usuario para la tarea con ID: {idTarea}."));
                return View(ConvertirAAsignarUsuarioTareaViewModel(tarea, tablero, usuarios));
            }

            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método AsignarCambiarUsuarioTarea del controlador Tarea."));
            return StatusCode(500);
        }
    }
    [HttpPost]
    public IActionResult AsignarCambiarUsuarioTarea(AsignarUsuarioTareaViewModel asignarUsuarioTareaViewModel)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método AsignarCambiarUsuarioTarea del controlador de Tablero."));
                return StatusCode(404);
            }

            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            if(esAdmin || asignarUsuarioTareaViewModel.IdPropietarioTablero == idUsuarioLogueado)
            {
                if(asignarUsuarioTareaViewModel.Tipo == "Asignar" && asignarUsuarioTareaViewModel.IdUsuario == null)
                {
                    return AsignarCambiarUsuarioTarea(asignarUsuarioTareaViewModel.IdTablero, asignarUsuarioTareaViewModel.IdTarea);
                }
                if (!ModelState.IsValid)
                {
                    TempData["Mensaje"] = "Por favor, Ingrese los datos correctamente.";
                    _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de asignar/cambiar un usuario a una tarea con datos no válidos."));
                    return AsignarCambiarUsuarioTarea(asignarUsuarioTareaViewModel.IdTablero, asignarUsuarioTareaViewModel.IdTarea);
                }

                _tareaRepository.CambiarPropietarioTarea(asignarUsuarioTareaViewModel.IdUsuario, asignarUsuarioTareaViewModel.IdTarea);
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se ha asignó/cambió exitosamente el usuario a una tarea."));
                return RedirectToAction("Index", new { idTablero = asignarUsuarioTareaViewModel.IdTablero });
            }

            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método AsignarCambiarUsuarioTarea del controlador Tarea."));
            return StatusCode(500);
        }
    }

    [HttpGet]
    public IActionResult EditarTarea(int idTarea)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EditarTarea del controlador de Tarea."));
                return StatusCode(404);
            }

            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            var tarea = _tareaRepository.ObtenerTareaPorId(idTarea);
            var tablero = _tableroRepository.ObtenerTableroPorId(tarea.IdTablero);
            if(esAdmin || tablero.IdUsuarioPropietario == idUsuarioLogueado)
            {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Accediendo a la vista EditarTarea para el tablero con ID: {tarea.IdTablero}."));
                return View(new EditarTareaViewModel()
                {
                    IdTarea = tarea.IdTarea,
                    IdTablero = tarea.IdTablero,
                    IdUsuarioPropietario = tablero.IdUsuarioPropietario,
                    IdUsuarioAsignado = tarea.IdUsuarioAsignado,
                    NombreTarea = tarea.NombreTarea,
                    DescripcionTarea = tarea.DescripcionTarea
                });
            }

            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EditarTarea del controlador Tarea."));
            return StatusCode(500);
        }
    }
    [HttpPost]
    public IActionResult EditarTarea(EditarTareaViewModel editarTareaViewModel)
    {
         try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EditarTarea del controlador de Tarea."));
                return StatusCode(404);
            }

            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            if(esAdmin || editarTareaViewModel.IdUsuarioPropietario == idUsuarioLogueado)
            {
                if (!ModelState.IsValid)
                {
                    TempData["Mensaje"] = "Por favor, Ingrese los datos correctamente.";
                    _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de editar una tarea con datos no válidos."));
                    return EditarTarea(editarTareaViewModel.IdTarea);
                }

                if (_tareaRepository.ExisteTarea(editarTareaViewModel.IdTablero, editarTareaViewModel.IdTarea, editarTareaViewModel.NombreTarea!))
                {
                    ModelState.AddModelError("NombreTarea", "El nombre de la tarea ya existe.");
                    _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de usar un nombre existente."));
                    return EditarTarea(editarTareaViewModel.IdTarea);
                }
                
                _tareaRepository.EditarTarea(new Tarea(editarTareaViewModel));
                TempData["Mensaje"] = $"Se ha modificado la tarea con ID: {editarTareaViewModel.IdTarea}";
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Se ha modificado la tarea con ID: {editarTareaViewModel.IdTarea}"));
                return RedirectToAction("Index", new { idTablero = editarTareaViewModel.IdTablero });
            }
            
            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EditarTarea del controlador Tarea."));
            return StatusCode(500);
        }
    }

    public IActionResult EliminarTarea(int idTarea, int idTablero)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EliminarTarea del controlador de Tarea."));
                return StatusCode(404);
            }
            
            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            var tablero = _tableroRepository.ObtenerTableroPorId(idTablero);
            if (esAdmin || idUsuarioLogueado == tablero.IdUsuarioPropietario)
            {
                _tareaRepository.EliminarTarea(idTarea);
                TempData["Mensaje"] = $"Se eliminó la tarea con ID {idTarea} correctamente.";
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Se ha eliminado la tarea con ID {idTarea}."));
                return RedirectToAction("Index", new { idTablero =  tablero.IdTablero });
            }

            _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos intentó acceder al método EliminarTarea del controlador de Tarea."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EliminarTarea del controlador Tarea."));
            return StatusCode(500);
        }
    }

    
    private ListaTareasViewModel ConvertirAListaTareasViewModel(Tablero tablero, List<TareaViewModel> tareasVM)
    {
        return new ListaTareasViewModel()
        {
            IdTablero = tablero.IdTablero,
            IdUsuarioPropietario = tablero.IdUsuarioPropietario,
            NombreTablero = tablero.NombreTablero,
            NombreUsuarioPropietario = _usuarioRepository.ObtenerNombreUsuario(tablero.IdUsuarioPropietario),
            ListaTareasVM = tareasVM
        };
    }
    private AsignarUsuarioTareaViewModel ConvertirAAsignarUsuarioTareaViewModel(Tarea tarea, Tablero tablero, List<Usuario> usuarios)
    {
        return new AsignarUsuarioTareaViewModel()
        {
            IdTarea = tarea.IdTarea,
            IdTablero = tarea.IdTablero,
            IdUsuarioAsignadoActual = tarea.IdUsuarioAsignado,
            IdPropietarioTablero = tablero.IdUsuarioPropietario,
            NombreTarea = tarea.NombreTarea,
            Tipo = tarea.IdUsuarioAsignado == null ? "Asignar" : "Cambiar",
            ListadoUsuariosDisponibles = usuarios
        };
    }
}