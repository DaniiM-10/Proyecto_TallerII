using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Proyecto_TallerII.Models;
using Proyecto_TallerII.Repositories;
using Proyecto_TallerII.ViewModels;
namespace Proyecto_TallerII.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITareaRepository _tareaRepository;
    private readonly ITableroRepository _tableroRepository;
    public UsuarioController(ILogger<UsuarioController> logger, IUsuarioRepository usuarioRepository, ITareaRepository tareaRepository, ITableroRepository tableroRepository)
    {
        _logger = logger;
        _usuarioRepository = usuarioRepository;
        _tareaRepository = tareaRepository;
        _tableroRepository = tableroRepository;
    }

    public IActionResult Index()
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método Index del controlador de Usuario."));
                return RedirectToAction("Index", "Login");
            }
            if (Autorizaciones.EsAdmin(HttpContext))
            {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Obteniendo todos los usuarios."));
                return View("index", new ListaUsuariosViewModel()
                {
                    ListaUsuariosVM = _usuarioRepository.ObtenerTodosLosUsuarios()
                });
            }
            else
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al método Index del controlador de Usuario."));
                return RedirectToAction("Index", "Tablero");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método Index del controlador Usuario."));
            return BadRequest();
        }
    }

    [HttpGet]
    public IActionResult CrearUsuario()
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearUsuario del controlador de Usuario."));
                return RedirectToAction("Index", "Login");
            }
            if (Autorizaciones.EsAdmin(HttpContext))
            {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("El usuario con permisos de administrador accedió a crear un usuario."));
                return View(new CrearUsuarioViewModel());
            }
            else
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo CrearUsuario del controlador de Usuario."));
                return RedirectToAction("Index", "Tablero");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearUsuario del controlador Usuario."));
            return BadRequest();
        }
    }
    [HttpPost]
    public IActionResult CrearUsuario(CrearUsuarioViewModel crearUsuarioViewModel)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearUsuario del controlador de Usuario."));
                return RedirectToAction("Index", "Login");
            }
            if (Autorizaciones.EsAdmin(HttpContext))
            {
                if (ModelState.IsValid)
                {
                    if (_usuarioRepository.ExisteUsuario(crearUsuarioViewModel.NombreUsuario!, 0))
                    {
                        ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya existe.");
                        return View(crearUsuarioViewModel);
                    }
                    _usuarioRepository.CrearUsuario(new Usuario
                    {
                        NombreUsuario = crearUsuarioViewModel.NombreUsuario!,
                        Password = crearUsuarioViewModel.Password!,
                        RolUsuario = crearUsuarioViewModel.RolUsuario
                    });
                    _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se ha creado un nuevo usuario por el administrador."));
                    return RedirectToAction("Index");
                }
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear un usuario con datos no válidos."));
                return View(crearUsuarioViewModel);
            }
            else
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo CrearUsuario del controlador de Usuario."));
                return RedirectToAction("Index", "Tablero");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearUsuario del controlador Usuario."));
            return BadRequest();
        }
    }

    public IActionResult Perfil(int idUsuario)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método Perfil del controlador de Usuario."));
                return RedirectToAction("Index", "Login");
            }
            var esAdmin = Autorizaciones.EsAdmin(HttpContext);
            var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);
            if (esAdmin || idUsuarioLogueado == idUsuario)
            {
                var datosUsuario = _usuarioRepository.ObtenerUsuarioPorId(idUsuario);

                var listaMisTareas = _tareaRepository.ListarTareasDeUsuario(idUsuario, false)
                    .Select(tarea => ConstruirListasTareasViewModel(tarea))
                    .ToList();

                var listaTareasSinAsignar = datosUsuario.RolUsuario == Rol.Administrador
                    ? _tareaRepository.ListarTareasSinAsignarAdmin()
                        .Select(tarea => ConstruirListasTareasViewModel(tarea))
                        .ToList()
                    : _tareaRepository.ListarTareasDeUsuario(idUsuario, true)
                        .Select(tarea => ConstruirListasTareasViewModel(tarea))
                        .ToList();

                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Obteniendo datos del usuario {idUsuario}."));
                return View(new PerfilViewModel()
                {
                    IdUsuario = datosUsuario.IdUsuario,
                    NombreUsuario = datosUsuario.NombreUsuario,
                    RolUsuario = datosUsuario.RolUsuario,
                    ListaMisTareas = listaMisTareas,
                    ListaTareasSinAsignar = listaTareasSinAsignar
                });
            } else {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo Perfil del controlador de Usuario."));
                return RedirectToAction("Index", "Tablero");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método Perfil del controlador Usuario."));
            return BadRequest();
        }
    }

    [HttpGet]
    public IActionResult EditarUsuario(int idUsuario)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EditarUsuario del controlador de Usuario."));
                return RedirectToAction("Index", "Login");
            }
            
            var esAdmin = Autorizaciones.EsAdmin(HttpContext);
            var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);
            if (esAdmin || idUsuarioLogueado == idUsuario)
            {
                var usuario = _usuarioRepository.ObtenerUsuarioPorId(idUsuario);
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Accediendo a la vista EditarUsuario para el usuario con ID: {idUsuario}."));
                return View(new EditarUsuarioViewModel
                {
                    IdUsuario = usuario.IdUsuario,
                    NombreUsuario = usuario!.NombreUsuario,
                    Password = usuario.Password,
                    RolUsuario = usuario.RolUsuario
                });
            }
            else
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo EditarUsuario del controlador de Usuario."));
                return RedirectToAction("Index", "Tablero");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EditarUsuario del controlador Usuario."));
            return BadRequest();
        }
    }
    [HttpPost]
    public IActionResult EditarUsuario(EditarUsuarioViewModel editarUsuarioViewModel)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EditarUsuario del controlador de Usuario."));
                return RedirectToAction("Index", "Login");
            }
            
            var esAdmin = Autorizaciones.EsAdmin(HttpContext);
            var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);
            if (esAdmin || idUsuarioLogueado == editarUsuarioViewModel.IdUsuario)
            {
                if (ModelState.IsValid)
                {
                    if (_usuarioRepository.ExisteUsuario(editarUsuarioViewModel.NombreUsuario!, editarUsuarioViewModel.IdUsuario))
                    {
                        var usuario = _usuarioRepository.ObtenerUsuarioPorId(editarUsuarioViewModel.IdUsuario);
                        ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya existe.");
                        return View(new EditarUsuarioViewModel()
                        {
                            IdUsuario = usuario.IdUsuario,
                            NombreUsuario = usuario.NombreUsuario,
                            Password = usuario.Password,
                            RolUsuario = usuario.RolUsuario
                        });
                    }
                    _usuarioRepository.EditarUsuario(editarUsuarioViewModel.IdUsuario, new Usuario
                    {
                        NombreUsuario = editarUsuarioViewModel.NombreUsuario!,
                        Password = editarUsuarioViewModel.Password!,
                        RolUsuario = editarUsuarioViewModel.RolUsuario
                    });
                    _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Se ha modificado el usuario con ID: {editarUsuarioViewModel.IdUsuario}"));
                    return RedirectToAction("Index");
                }
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("ModelState no es válido en EditarUsuario"));
                return RedirectToAction("Index");
            } else {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo EditarUsuario del controlador de Usuario."));
                return RedirectToAction("Index", "Tablero");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EditarUsuario del controlador Usuario."));
            return BadRequest();
        }
    }

    public IActionResult EliminarUsuario(int idUsuario)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EliminarUsuario del controlador de Usuario.");
                return RedirectToAction("Index", "Login");
            }
            if (Autorizaciones.EsAdmin(HttpContext))
            {
                _usuarioRepository.EliminarUsuario(idUsuario);
                _logger.LogInformation($"Se eliminó el usuario con ID {idUsuario}");
                if(idUsuario == Autorizaciones.ObtenerIdUsuario(HttpContext))
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogWarning("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al método EliminarUsuario del controlador de Usuario.");
                return RedirectToAction("Index");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EliminarUsuario del controlador Usuario."));
            return BadRequest();
        }
    }

    // Auxiliares
    private TareaViewModel ConstruirListasTareasViewModel(Tarea tarea)
    {
        var tablero = _tableroRepository.ObtenerTableroPorId(tarea.IdTablero);
        return new TareaViewModel
        {
            IdTareaVM = tarea.IdTarea,
            IdTableroVM = tarea.IdTablero,
            IdUsuarioAsignadoVM = tarea.IdUsuarioAsignado,
            NombreTareaVM = tarea.NombreTarea,
            NombreTableroVM = tablero.NombreTablero,
            NombrePropietarioTableroVM = _usuarioRepository.ObtenerNombreUsuario(tablero.IdUsuarioPropietario ?? 0),
            EstadoTareaVM = tarea.EstadoTarea,
            ColorTareaVM = tarea.ColorTarea,
            DescripcionTareaVM = tarea.DescripcionTarea
        };
    }
}
