using Microsoft.AspNetCore.Mvc;
using Proyecto_TallerII.Models;
using Proyecto_TallerII.Repositories;
using Proyecto_TallerII.Helpers;
using Proyecto_TallerII.ViewModels;
namespace Proyecto_TallerII.Controllers;

public class TableroController : Controller
{
    private readonly ILogger<TableroController> _logger;
    private readonly ITableroRepository _tableroRepository;
    private readonly IUsuarioRepository _usuarioRepository; 
    private readonly ITareaRepository _tareaRepository; 
    public TableroController(ILogger<TableroController> logger, ITableroRepository tableroRepository, IUsuarioRepository usuarioRepository, ITareaRepository tareaRepository)
    {
        _logger = logger;
        _tableroRepository = tableroRepository;
        _usuarioRepository = usuarioRepository;
        _tareaRepository = tareaRepository;
    }

    public IActionResult Index()
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método Index del controlador de Tablero."));
                return StatusCode(404);
            }
            
            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);

            var id = esAdmin ? 0 : idUsuarioLogueado;
            var listaTableros = _tableroRepository.ListaTableros(id);

            var tableros = esAdmin
            ? listaTableros
            : listaTableros.Concat(_tableroRepository.ListaTablerosAjenos(idUsuarioLogueado));

            var tablerosVM = tableros.Select(ConvertirATableroViewModel).ToList();

            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Inicio de la aplicación."));
            return View(new ListaTablerosViewModel()
            {
                ListaTablerosVM = tablerosVM
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método Index del controlador Tablero."));
            return StatusCode(500);
        }
    }

    [HttpGet]
    public IActionResult CrearTablero()
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearTablero del controlador de Tablero."));
                return StatusCode(404);
            }
            
            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Acceso exitoso al método CrearTablero del controlador de Tableros."));
            return View(new CrearTableroViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearTablero del controlador Tablero."));
            return StatusCode(500);
        }
    }
    [HttpPost]
    public IActionResult CrearTablero(CrearTableroViewModel crearTableroViewModel)
    {
         try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearTablero del controlador de Tablero."));
                return StatusCode(404);
            }

            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = "Por favor, Ingrese los datos correctamente.";
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear un tablero con datos no válidos."));
                return View(crearTableroViewModel);
            }

            if (_tableroRepository.ExisteTablero(crearTableroViewModel.NombreDeTablero!, 0))
            {
                ModelState.AddModelError("NombreDeTablero", "El nombre del tablero ya existe.");
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear un tablero con un nombre existente."));
                return View(crearTableroViewModel);
            }

            _tableroRepository.CrearTablero(new Tablero(crearTableroViewModel));
            TempData["Mensaje"] = "Se creó un nuevo tablero.";
            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se ha creado exitosamente un nuevo tablero."));
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearTablero del controlador Tablero."));
            return StatusCode(500);
        }
    }

    [HttpGet]
    public IActionResult CrearTableroAjeno()
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearTableroAjeno del controlador de Tablero."));
                return StatusCode(404);
            }

            if (!AuthHelper.EsAdmin(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo CrearTableroAjeno del controlador de Tablero."));
                return StatusCode(404);
            }

            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Acceso exitoso al método CrearTableroAjeno del controlador de Tableros."));
            return View(new CrearTableroAjenoViewModel()
            {
                ListadoUsuarios = _usuarioRepository.ObtenerTodosLosUsuarios()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearTableroAjeno del controlador Tablero."));
            return StatusCode(500);
        }
    }
    [HttpPost]
    public IActionResult CrearTableroAjeno(CrearTableroAjenoViewModel crearTableroAViewModel)
    {
         try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearTableroAjeno del controlador de Tablero."));
                return StatusCode(404);
            }

            if (!AuthHelper.EsAdmin(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo CrearTableroAjeno del controlador de Tablero."));
                return StatusCode(404);
            }

            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = "Por favor, Ingrese los datos correctamente.";
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear un tablero con datos no válidos."));
                return View(crearTableroAViewModel);
            }

            if (_tableroRepository.ExisteTablero(crearTableroAViewModel.NombreDeTableroA!, 0))
            {
                ModelState.AddModelError("NombreDeTableroA", "El nombre del tablero ya existe.");
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear un tablero con un nombre de tablero existente."));
                crearTableroAViewModel.ListadoUsuarios = _usuarioRepository.ObtenerTodosLosUsuarios();
                return View(crearTableroAViewModel);
            }

            _tableroRepository.CrearTablero(new Tablero(crearTableroAViewModel));
            TempData["Mensaje"] = "Se creó un nuevo tablero ajeno.";
            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se ha creado exitosamente un nuevo tablero."));
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearTableroAjeno del controlador Tablero."));
            return StatusCode(500);
        }
    }

    [HttpGet]
    public IActionResult EditarTablero(int idTablero)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EditarTablero del controlador de Tablero."));
                return StatusCode(404);
            }

            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            var tablero = _tableroRepository.ObtenerTableroPorId(idTablero);
            if (tablero == null)
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn($"No se encontró ningún tablero con el ID: {idTablero}."));
                return StatusCode(404);
            }
            if (esAdmin || idUsuarioLogueado == tablero.IdUsuarioPropietario)
            {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Accediendo a la vista EditarTablero para el tablero con ID: {idTablero}."));
                return View(ConvertirAEditarTableroViewModel(tablero));
            }

            _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos intentó acceder al metodo EditarTablero del controlador de Tablero."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EditarTablero del controlador Tablero."));
            return StatusCode(500);
        }
    }
    [HttpPost]
    public IActionResult EditarTablero(EditarTableroViewModel editarTableroViewModel)
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EditarTablero del controlador de Tablero."));
                return StatusCode(404);
            }

            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            if (esAdmin || idUsuarioLogueado == editarTableroViewModel.IdUsuarioPropietario)
            {
                if (!ModelState.IsValid)
                {
                    TempData["Mensaje"] = "Por favor, Ingrese los datos correctamente.";
                    _logger.LogWarning(LoggerMsj.MensajeInfoWarn("ModelState no es válido en EditarTablero"));
                    return EditarTablero(editarTableroViewModel.IdTablero);
                }
                
                if (_tableroRepository.ExisteTablero(editarTableroViewModel.NombreDeTablero!, editarTableroViewModel.IdTablero))
                {
                    ModelState.AddModelError("NombreDeTablero", "El nombre del tablero ya existe.");
                    _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear un tablero con un nombre de tablero existente."));
                    return EditarTablero(editarTableroViewModel.IdTablero);
                }
                
                _tableroRepository.EditarTablero(new Tablero(editarTableroViewModel));
                TempData["Mensaje"] = $"Se ha modificado el tablero con ID: {editarTableroViewModel.IdTablero}";
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Se ha modificado el tablero con ID: {editarTableroViewModel.IdTablero}"));
                return RedirectToAction("Index", "Tarea", new { idTablero = editarTableroViewModel.IdTablero });
            }

            _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos intentó acceder al metodo EditarTablero del controlador de Tablero."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EditarTablero del controlador Tablero."));
            return StatusCode(500);
        }
    }

    public IActionResult EliminarTablero(int idTablero, int idUsuarioProp) 
    {
        try
        {
            if (!AuthHelper.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EliminarTablero del controlador de Tablero."));
                return StatusCode(404);
            }
            
            var esAdmin = AuthHelper.EsAdmin(HttpContext);
            var idUsuarioLogueado = AuthHelper.ObtenerIdUsuario(HttpContext);
            if (esAdmin || idUsuarioLogueado == idUsuarioProp)
            {
                var tareas = _tareaRepository.ListarTodasLasTareas(idTablero);  
                if(tareas.Count() > 0)
                {
                    TempData["Mensaje"] = "No se pudo eliminar el tablero, tiene tareas pendientes.";
                    _logger.LogWarning(LoggerMsj.MensajeInfoWarn("No se puede eliminar un tablero con tareas pendientes."));
                    return RedirectToAction("Index", "Tarea", new { idTablero = idTablero });
                }

                _tableroRepository.EliminarTablero(idTablero);
                TempData["Mensaje"] = $"Se eliminó el tablero con ID {idTablero} correctamente.";
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Se ha eliminado el tablero con ID {idTablero}."));
                return RedirectToAction("Index");
            }
            
            _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos intentó acceder al método EliminarTablero del controlador de Tablero."));
            return StatusCode(404);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EliminarTablero del controlador Tablero."));
            return StatusCode(500);
        }
    }

    
    private TableroViewModel ConvertirATableroViewModel(Tablero tablero)
    {
        return new TableroViewModel()
        {
            IdTableroVM = tablero.IdTablero,
            IdUsuarioPropietarioVM = tablero.IdUsuarioPropietario,
            NombreTableroVM = tablero.NombreTablero,
            DescripcionTableroVM = tablero.DescripcionTablero!,
            NombrePropietarioTableroVM = _usuarioRepository.ObtenerNombreUsuario(tablero.IdUsuarioPropietario),
            CantidadTareasVM = _tareaRepository.CantidadDeTareas(tablero.IdTablero),
            CantidadTareasRealizadasVM = _tareaRepository.CantidadDeTareasRealizadas(tablero.IdTablero)
        };
    }
    private EditarTableroViewModel ConvertirAEditarTableroViewModel(Tablero tablero)
    {
        return new EditarTableroViewModel()
        {
            IdTablero = tablero.IdTablero,
            NombreDeTablero = tablero.NombreTablero,
            DescripcionDeTablero = tablero.DescripcionTablero!,
            IdUsuarioPropietario = tablero.IdUsuarioPropietario,
            ListadoUsuarios = _usuarioRepository.ObtenerTodosLosUsuarios()
        };
    }
}