using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Proyecto_TallerII.Models;
using Proyecto_TallerII.Repositories;
using Proyecto_TallerII.Helpers;
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
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método Index del controlador de Usuario."));
                return StatusCode(404);
            }

            if (!AuthHelper.EsAdmin(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al método Index del controlador de Usuario."));
                return StatusCode(404);
            }

            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Obteniendo todos los usuarios."));
            return View(new ListaUsuariosViewModel()
            {
                ListaUsuariosVM = _usuarioRepository.ObtenerTodosLosUsuarios()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método Index del controlador Usuario."));
            return StatusCode(500);
        }
    }

    [HttpGet]
    public IActionResult CrearUsuario()
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearUsuario del controlador de Usuario."));
                return StatusCode(404);
            }

            if (!AuthHelper.EsAdmin(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo CrearUsuario del controlador de Usuario."));
                return StatusCode(404);
            }

            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("El usuario con permisos de administrador accedió a crear un usuario."));
            return View(new CrearUsuarioViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearUsuario del controlador Usuario."));
            return StatusCode(500);
        }
    }
    [HttpPost]
    public IActionResult CrearUsuario(CrearUsuarioViewModel crearUsuarioViewModel)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearUsuario del controlador de Usuario."));
                return StatusCode(404);
            }

            if (!AuthHelper.EsAdmin(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo CrearUsuario del controlador de Usuario."));
                return StatusCode(404);
            }

            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = "Por favor, Ingrese los datos correctamente.";
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear un usuario con datos no válidos."));
                return View(crearUsuarioViewModel);
            }

            if (_usuarioRepository.ExisteUsuario(crearUsuarioViewModel.NombreUsuario!, 0))
            {
                ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya existe.");
                return View(crearUsuarioViewModel);
            }

            _usuarioRepository.CrearUsuario(new Usuario(crearUsuarioViewModel));
            TempData["Mensaje"] = "Se creó un nuevo usuario.";
            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se ha creado un nuevo usuario por el administrador."));
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearUsuario del controlador Usuario."));
            return StatusCode(500);
        }
    }

    public IActionResult Perfil(int idUsuario)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método Perfil del controlador de Usuario."));
                return StatusCode(404);
            }

            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            if (esAdmin || idUsuarioLogueado == idUsuario)
            {
                var datosUsuario = _usuarioRepository.ObtenerUsuarioPorId(idUsuario);

                var listaMisTareas = _tareaRepository.ListarTareasDeUsuario(idUsuario, false);

                var listaTareasSinAsignar = datosUsuario.RolUsuario == Rol.Administrador
                    ? _tareaRepository.ListarTareasSinAsignarAdmin()
                    : _tareaRepository.ListarTareasDeUsuario(idUsuario, true);
                
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Obteniendo datos del usuario {idUsuario}."));
                return View(ConvertirAPerfilViewModel(datosUsuario, listaMisTareas, listaTareasSinAsignar));
            }

            _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo Perfil del controlador de Usuario."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método Perfil del controlador Usuario."));
            return StatusCode(500);
        }
    }

    [HttpGet]
    public IActionResult EditarUsuario(int idUsuario)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EditarUsuario del controlador de Usuario."));
                return StatusCode(404);
            }
            
            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            if (esAdmin || idUsuarioLogueado == idUsuario)
            {
                var usuario = _usuarioRepository.ObtenerUsuarioPorId(idUsuario);
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Accediendo a la vista EditarUsuario para el usuario con ID: {idUsuario}."));
                return View(ConvertirAEditarUsuarioViewModel(usuario));
            }

            _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo EditarUsuario del controlador de Usuario."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EditarUsuario del controlador Usuario."));
            return StatusCode(500);
        }
    }
    [HttpPost]
    public IActionResult EditarUsuario(EditarUsuarioViewModel editarUsuarioViewModel)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EditarUsuario del controlador de Usuario."));
                return StatusCode(404);
            }
            
            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            if (esAdmin || idUsuarioLogueado == editarUsuarioViewModel.IdUsuario)
            {
                if (!ModelState.IsValid)
                {
                    TempData["Mensaje"] = "Por favor, Ingrese los datos correctamente.";
                    _logger.LogWarning(LoggerMsj.MensajeInfoWarn("ModelState no es válido en EditarUsuario"));
                    return EditarUsuario(editarUsuarioViewModel.IdUsuario);
                }

                if (_usuarioRepository.ExisteUsuario(editarUsuarioViewModel.NombreUsuario!, editarUsuarioViewModel.IdUsuario))
                {
                    var usuario = _usuarioRepository.ObtenerUsuarioPorId(editarUsuarioViewModel.IdUsuario);
                    ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya existe.");
                    return EditarUsuario(editarUsuarioViewModel.IdUsuario);
                }

                _usuarioRepository.EditarUsuario(new Usuario(editarUsuarioViewModel));
                TempData["Mensaje"] = $"Se ha modificado el usuario con ID: {editarUsuarioViewModel.IdUsuario}";
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Se ha modificado el usuario con ID: {editarUsuarioViewModel.IdUsuario}"));

                return esAdmin ? RedirectToAction("Index") : RedirectToAction("Perfil", new { idUsuario = editarUsuarioViewModel.IdUsuario });
            }

            _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo EditarUsuario del controlador de Usuario."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EditarUsuario del controlador Usuario."));
            return StatusCode(500);
        }
    }

    public IActionResult EliminarUsuario(int idUsuario)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EliminarUsuario del controlador de Usuario.");
                return StatusCode(404);
            }

            if (!AuthHelper.EsAdmin(HttpContext))
            {
                _logger.LogWarning("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al método EliminarUsuario del controlador de Usuario.");
                return StatusCode(404);
            }
            
            if(_tareaRepository.ListarTareasDeUsuario(idUsuario, false).Count() != 0 || _tableroRepository.ListaTableros(idUsuario).Count() != 0)
            {
                TempData["Mensaje"] = $"No se podrá eliminar este usuario, ya que posee tareas o tableros.";
                _logger.LogWarning($"No se podrá eliminar este usuario, ya que posee tareas o tableros.");
                return RedirectToAction("Index");
            }

            int idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            _usuarioRepository.EliminarUsuario(idUsuario);
            TempData["Mensaje"] = $"Se eliminó el usuario con ID {idUsuario} correctamente.";
            _logger.LogInformation($"Se eliminó el usuario con ID {idUsuario}");

            if(idUsuario == idUsuarioLogueado)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Login");
            }
            
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EliminarUsuario del controlador Usuario."));
            return StatusCode(500);
        }
    }

    
    private TareaViewModel ConvertirATareaViewModel(Tarea tarea)
    {
        var tablero = _tableroRepository.ObtenerTableroPorId(tarea.IdTablero);
        return new TareaViewModel
        {
            IdTareaVM = tarea.IdTarea,
            IdTableroVM = tarea.IdTablero,
            IdUsuarioAsignadoVM = tarea.IdUsuarioAsignado,
            NombreTareaVM = tarea.NombreTarea,
            NombreTableroVM = tablero.NombreTablero,
            NombrePropietarioTableroVM = _usuarioRepository.ObtenerNombreUsuario(tablero.IdUsuarioPropietario),
            EstadoTareaVM = tarea.EstadoTarea,
            ColorTareaVM = tarea.ColorTarea,
            DescripcionTareaVM = tarea.DescripcionTarea
        };
    }
    private PerfilViewModel ConvertirAPerfilViewModel(Usuario usuario, List<Tarea> listaMisTareas, List<Tarea> listaTareasSinAsignar)
    {
        return new PerfilViewModel()
        {
            IdUsuario = usuario.IdUsuario,
            NombreUsuario = usuario.NombreUsuario,
            RolUsuario = usuario.RolUsuario,
            ListaMisTareas = listaMisTareas.Select(tarea => ConvertirATareaViewModel(tarea)).ToList(),
            ListaTareasSinAsignar = listaTareasSinAsignar.Select(tarea => ConvertirATareaViewModel(tarea)).ToList()
        };
    }
    private EditarUsuarioViewModel ConvertirAEditarUsuarioViewModel(Usuario usuario)
    {
        return new EditarUsuarioViewModel()
        {
            IdUsuario = usuario.IdUsuario,
            NombreUsuario = usuario.NombreUsuario,
            Password = usuario.Password,
            RolUsuario = usuario.RolUsuario
        };
    }
}
