using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Proyecto_TallerII.Models;
using Proyecto_TallerII.Repositories;
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
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método Index del controlador de Tarea."));
                return RedirectToAction("Index", "Login");
            }

            var esAdmin = Autorizaciones.EsAdmin(HttpContext);
            var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);

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
                return View(new ListaTareasViewModel()
                {
                    IdTablero = tablero.IdTablero,
                    IdUsuarioPropietario = tablero.IdUsuarioPropietario,
                    NombreTablero = tablero.NombreTablero,
                    ListaTareasVM = tareasVM
                });
            } else {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
                return RedirectToAction("Index", "Tablero");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método Index del controlador Tarea."));
            return BadRequest();
        }
    }

    [HttpPost]
    public IActionResult CambiarEstado(CambiarEstadoViewModel cambiarEstadoViewModel)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CambiarEstado del controlador de Tarea."));
                return RedirectToAction("Index", "Login");
            }
            
            if (ModelState.IsValid)
            {
                var esAdmin = Autorizaciones.EsAdmin(HttpContext);
                var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);

                if((esAdmin || (cambiarEstadoViewModel.IdPropietarioTablero == idUsuarioLogueado || cambiarEstadoViewModel.IdUsuarioAsignado == idUsuarioLogueado)) && cambiarEstadoViewModel.IdUsuarioAsignado != null)
                {
                    cambiarEstadoViewModel.ColorTarea = new Tarea().ObtenerColorPorEstado(cambiarEstadoViewModel.EstadoTarea);

                    _tareaRepository.CambiarEstadoTarea(cambiarEstadoViewModel.IdTarea, (int)cambiarEstadoViewModel.EstadoTarea, cambiarEstadoViewModel.ColorTarea);
                    _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se cambio el estado de la tarea correctamente."));
                    return RedirectToAction("Index", new { idTablero = cambiarEstadoViewModel.IdTablero });
                }
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
                return RedirectToAction("Index", new { idTablero = cambiarEstadoViewModel.IdTablero });
            }
            _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de cambiar el estado a una tarea con datos no válidos."));
            return RedirectToAction("Index", new { idTablero = cambiarEstadoViewModel.IdTablero });
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CambiarEstado del controlador Tarea."));
            return BadRequest();
        }
    }

    [HttpGet]
    public IActionResult CrearTarea(int idTablero)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearTarea del controlador de Tarea."));
                return RedirectToAction("Index", "Login");
            }

            var esAdmin = Autorizaciones.EsAdmin(HttpContext);
            var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);
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
            } else {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
                return RedirectToAction("Index", new { idTablero = idTablero });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearTarea del controlador Tarea."));
            return BadRequest();
        }
    }
    [HttpPost]
    public IActionResult CrearTarea(CrearTareaViewModel crearTareaViewModel)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearTarea del controlador de Tarea."));
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                var esAdmin = Autorizaciones.EsAdmin(HttpContext);
                var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);
                if(esAdmin || crearTareaViewModel.IdPropietarioTablero == idUsuarioLogueado)
                {
                    if (_tareaRepository.ExisteTarea(crearTareaViewModel.IdTablero, 0, crearTareaViewModel.NombreTarea!))
                    {
                        ModelState.AddModelError("NombreTarea", "El nombre de la tarea ya existe.");
                        _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear una tarea con un nombre existente."));
                        crearTareaViewModel.ListadoUsuariosDisponibles = _usuarioRepository.ObtenerTodosLosUsuarios();
                        return View(crearTareaViewModel);
                    }
                    _tareaRepository.CrearTarea(new Tarea()
                    {
                        IdTablero = crearTareaViewModel.IdTablero,
                        NombreTarea = crearTareaViewModel.NombreTarea,
                        IdUsuarioAsignado = crearTareaViewModel.IdUsuarioAsignado,
                        DescripcionTarea = crearTareaViewModel.DescripcionTarea,
                        EstadoTarea = crearTareaViewModel.EstadoTarea,
                        ColorTarea = crearTareaViewModel.ColorTarea,
                    });
                    _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se ha creado exitosamente una nueva tarea."));
                    return RedirectToAction("Index", new { idTablero = crearTareaViewModel.IdTablero });
                } else {
                    _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
                    return RedirectToAction("Index", new { idTablero = crearTareaViewModel.IdTablero });
                }
            }
            _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear una tarea con datos no válidos."));
            return View(new CrearTareaViewModel()
            {
                IdTablero = crearTareaViewModel.IdTablero,
                IdPropietarioTablero = crearTareaViewModel.IdPropietarioTablero,
                ListadoUsuariosDisponibles = _usuarioRepository.ObtenerTodosLosUsuarios()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearTarea del controlador Tarea."));
            return BadRequest();
        }
    }

    [HttpGet]
    public IActionResult AsignarCambiarUsuarioTarea(int idTablero, int idTarea)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método AsignarCambiarUsuarioTarea del controlador de Tablero."));
                return RedirectToAction("Index", "Login");
            }

            var tablero = _tableroRepository.ObtenerTableroPorId(idTablero);
            var tarea = _tareaRepository.ObtenerTareaPorId(idTarea);
            var usuarios = _usuarioRepository.ObtenerTodosLosUsuarios();
            var esAdmin = Autorizaciones.EsAdmin(HttpContext);
            var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);
            if(esAdmin || tablero.IdUsuarioPropietario == idUsuarioLogueado)
            {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Accediendo a la vista Asignar/Cambiar Usuario para la tarea con ID: {idTarea}."));
                return View(new AsignarUsuarioTareaViewModel()
                {
                    IdTarea = tarea.IdTarea,
                    IdTablero = tablero.IdTablero,
                    IdUsuarioAsignadoActual = tarea.IdUsuarioAsignado,
                    IdPropietarioTablero = tablero.IdUsuarioPropietario,
                    NombreTarea = tarea.NombreTarea,
                    Tipo = tarea.IdUsuarioAsignado == null ? "Asignar" : "Cambiar",
                    ListadoUsuariosDisponibles = usuarios
                });
            } else {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
                return RedirectToAction("Index", new { idTablero = idTablero });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método AsignarCambiarUsuarioTarea del controlador Tarea."));
            return BadRequest();
        }
    }
    [HttpPost]
    public IActionResult AsignarCambiarUsuarioTarea(AsignarUsuarioTareaViewModel asignarUsuarioTareaViewModel)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método AsignarCambiarUsuarioTarea del controlador de Tablero."));
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                var esAdmin = Autorizaciones.EsAdmin(HttpContext);
                var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);
                if(esAdmin || asignarUsuarioTareaViewModel.IdPropietarioTablero == idUsuarioLogueado)
                {
                    _tareaRepository.CambiarPropietarioTarea(asignarUsuarioTareaViewModel.IdUsuario, asignarUsuarioTareaViewModel.IdTarea);
                    _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se ha asignó/cambió exitosamente el usuario a una tarea."));
                    return RedirectToAction("Index", new { idTablero = asignarUsuarioTareaViewModel.IdTablero });
                } else {
                    _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
                    return RedirectToAction("Index", new { idTablero = asignarUsuarioTareaViewModel.IdTablero });
                }
            }
            _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de asignar/cambiar un usuario a una tarea con datos no válidos."));
            return View(new AsignarUsuarioTareaViewModel()
            {
                IdTarea = asignarUsuarioTareaViewModel.IdTarea,
                IdTablero = asignarUsuarioTareaViewModel.IdTablero,
                IdUsuarioAsignadoActual = asignarUsuarioTareaViewModel.IdUsuarioAsignadoActual,
                IdPropietarioTablero = asignarUsuarioTareaViewModel.IdPropietarioTablero,
                NombreTarea = asignarUsuarioTareaViewModel.NombreTarea,
                Tipo = asignarUsuarioTareaViewModel.IdUsuarioAsignadoActual == null ? "Asignar" : "Cambiar",
                ListadoUsuariosDisponibles = _usuarioRepository.ObtenerTodosLosUsuarios()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método AsignarCambiarUsuarioTarea del controlador Tarea."));
            return BadRequest();
        }
    }

    [HttpGet]
    public IActionResult EditarTarea(int idTarea)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EditarTarea del controlador de Tarea."));
                return RedirectToAction("Index", "Login");
            }

            var esAdmin = Autorizaciones.EsAdmin(HttpContext);
            var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);
            var tarea = _tareaRepository.ObtenerTareaPorId(idTarea);
            if(esAdmin || tarea.IdUsuarioAsignado == idUsuarioLogueado)
            {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Accediendo a la vista EditarTarea para el tablero con ID: {tarea.IdTablero}."));
                return View(new EditarTareaViewModel()
                {
                    IdTarea = tarea.IdTarea,
                    IdTablero = tarea.IdTablero,
                    IdUsuarioAsignado = tarea.IdUsuarioAsignado,
                    NombreTarea = tarea.NombreTarea,
                    DescripcionTarea = tarea.DescripcionTarea
                });
            } else {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
                return RedirectToAction("Index", new { idTablero = tarea.IdTablero });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EditarTarea del controlador Tarea."));
            return BadRequest();
        }
    }
    [HttpPost]
    public IActionResult EditarTarea(EditarTareaViewModel editarTareaViewModel)
    {
         try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EditarTarea del controlador de Tarea."));
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                var esAdmin = Autorizaciones.EsAdmin(HttpContext);
                var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);
                if(esAdmin || editarTareaViewModel.IdUsuarioAsignado == idUsuarioLogueado)
                {
                    if (_tareaRepository.ExisteTarea(editarTareaViewModel.IdTablero, editarTareaViewModel.IdTarea, editarTareaViewModel.NombreTarea!))
                    {
                        ModelState.AddModelError("NombreTarea", "El nombre de la tarea ya existe.");
                        _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de usar un nombre existente."));
                        var tarea = _tareaRepository.ObtenerTareaPorId(editarTareaViewModel.IdTarea);
                        return View(new EditarTareaViewModel()
                        {
                            IdTablero = tarea.IdTablero,
                            IdTarea = tarea.IdTarea,
                            IdUsuarioAsignado = tarea.IdUsuarioAsignado,
                            NombreTarea = tarea.NombreTarea,
                            DescripcionTarea = tarea.DescripcionTarea
                        });
                    }
                    _tareaRepository.EditarTarea(new Tarea()
                    {
                        IdTablero = editarTareaViewModel.IdTablero,
                        IdTarea = editarTareaViewModel.IdTarea,
                        IdUsuarioAsignado = editarTareaViewModel.IdUsuarioAsignado,
                        NombreTarea = editarTareaViewModel.NombreTarea,
                        DescripcionTarea = editarTareaViewModel.DescripcionTarea
                    });
                    _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se editó la tarea correctamente."));
                    return RedirectToAction("Index", new { idTablero = editarTareaViewModel.IdTablero });
                } else {
                    _logger.LogInformation(LoggerMsj.MensajeInfoWarn("No tiene los permisos para realizar esta acción."));
                    return RedirectToAction("Index", new { idTablero = editarTareaViewModel.IdTablero });
                }
            }
            _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de editar una tarea con datos no válidos."));
            return View(editarTareaViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EditarTarea del controlador Tarea."));
            return BadRequest();
        }
    }

    public IActionResult EliminarTarea(int idTarea, int idTablero)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EliminarTarea del controlador de Tarea."));
                return RedirectToAction("Index", "Login");
            }
            
            var esAdmin = Autorizaciones.EsAdmin(HttpContext);
            var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);
            var tablero = _tableroRepository.ObtenerTableroPorId(idTablero);
            if (esAdmin || idUsuarioLogueado == tablero.IdUsuarioPropietario)
            {
                _tareaRepository.EliminarTarea(idTarea);
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Se ha eliminado la tarea con ID {idTarea}."));
                return RedirectToAction("Index", new { idTablero =  tablero.IdTablero });
            }
            else
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos intentó acceder al método EliminarTarea del controlador de Tarea."));
                return RedirectToAction("Index", new { idTablero = tablero.IdTablero });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EliminarTarea del controlador Tarea."));
            return BadRequest();
        }
    }
}