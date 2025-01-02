using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Proyecto_TallerII.Models;
using Proyecto_TallerII.Repositories;
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
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método Index del controlador de Tablero."));
                return RedirectToAction("Index", "Login");
            }
            
            var esAdmin = Autorizaciones.EsAdmin(HttpContext);
            var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);

            var id = esAdmin ? 0 : idUsuarioLogueado;
            var listaTableros = _tableroRepository.ListaTableros(id);

            var tableros = esAdmin
            ? listaTableros
            : listaTableros.Concat(_tableroRepository.ListaTablerosAjenos(idUsuarioLogueado));

            var tablerosVM = tableros.Select(ConvertirATableroViewModel).ToList();

            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Inicio de la aplicación."));
            return View("Index", new ListaTablerosViewModel()
            {
                ListaTablerosVM = tablerosVM
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método Index del controlador Tablero."));
            return BadRequest();
        }
    }

    [HttpGet]
    public IActionResult CrearTablero()
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearTablero del controlador de Tablero."));
                return RedirectToAction("Index", "Login");
            }
            
            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Acceso exitoso al método CrearTablero del controlador de Tableros."));
            return View(new CrearTableroViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearTablero del controlador Tablero."));
            return BadRequest();
        }
    }
    [HttpPost]
    public IActionResult CrearTablero(CrearTableroViewModel crearTableroViewModel)
    {
         try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearTablero del controlador de Tablero."));
                return RedirectToAction("Index", "Login");
            }
            if (_tableroRepository.ExisteTablero(crearTableroViewModel.NombreDeTablero!, 0))
            {
                ModelState.AddModelError("NombreDeTablero", "El nombre del tablero ya existe.");
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear un tablero con un nombre existente."));
                return View("CrearTablero", crearTableroViewModel);
            }
            if (ModelState.IsValid)
            {
                _tableroRepository.CrearTablero(new Tablero
                {
                    NombreTablero = crearTableroViewModel.NombreDeTablero,
                    DescripcionTablero = crearTableroViewModel.DescripcionDeTablero,
                    IdUsuarioPropietario = crearTableroViewModel.IdUsuarioPropietario
                });
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se ha creado exitosamente un nuevo tablero."));
                return RedirectToAction("Index");
            }
            _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear un tablero con datos no válidos."));
            return RedirectToAction("CrearTablero");
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearTablero del controlador Tablero."));
            return BadRequest();
        }
    }

    [HttpGet]
    public IActionResult CrearTableroAjeno()
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearTableroAjeno del controlador de Tablero."));
                return RedirectToAction("Index", "Login");
            }
            if (Autorizaciones.EsAdmin(HttpContext))
            {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Acceso exitoso al método CrearTableroAjeno del controlador de Tableros."));
                return View(new CrearTableroAjenoViewModel()
                {
                    ListadoUsuarios = _usuarioRepository.ObtenerTodosLosUsuarios()
                });
            } else {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo CrearTableroAjeno del controlador de Tablero."));
                return RedirectToAction("Index");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearTableroAjeno del controlador Tablero."));
            return BadRequest();
        }
    }
    [HttpPost]
    public IActionResult CrearTableroAjeno(CrearTableroAjenoViewModel crearTableroAViewModel)
    {
         try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método CrearTableroAjeno del controlador de Tablero."));
                return RedirectToAction("Index", "Login");
            }
            if (Autorizaciones.EsAdmin(HttpContext))
            {
                if (_tableroRepository.ExisteTablero(crearTableroAViewModel.NombreDeTableroA!, 0))
                {
                    ModelState.AddModelError("NombreDeTableroA", "El nombre del tablero ya existe.");
                    _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear un tablero con un nombre de tablero existente."));
                    crearTableroAViewModel.ListadoUsuarios = _usuarioRepository.ObtenerTodosLosUsuarios();
                    return View(crearTableroAViewModel);
                }
                if (ModelState.IsValid)
                {
                    _tableroRepository.CrearTablero(new Tablero
                    {
                        NombreTablero = crearTableroAViewModel.NombreDeTableroA,
                        DescripcionTablero = crearTableroAViewModel.DescripcionDeTableroA,
                        IdUsuarioPropietario = crearTableroAViewModel.IdUsuarioPropietarioA
                    });
                    _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Se ha creado exitosamente un nuevo tablero."));
                    return RedirectToAction("Index");
                } 
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear un tablero con datos no válidos."));
                return RedirectToAction("CrearTableroAjeno");
            } else {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos de administrador intentó acceder al metodo CrearTableroAjeno del controlador de Tablero."));
                return RedirectToAction("Index");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método CrearTableroAjeno del controlador Tablero."));
            return BadRequest();
        }
    }

    [HttpGet]
    public IActionResult EditarTablero(int idTablero, int idUsuarioProp)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EditarTablero del controlador de Tablero."));
                return RedirectToAction("Index", "Login");
            }

            var esAdmin = Autorizaciones.EsAdmin(HttpContext);
            var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);
            if (esAdmin || idUsuarioLogueado == idUsuarioProp)
            {
                var tablero = _tableroRepository.ObtenerTableroPorId(idTablero);

                if (tablero == null)
                {
                    _logger.LogWarning(LoggerMsj.MensajeInfoWarn($"No se encontró ningún tablero con el ID: {idTablero}."));
                    return NotFound();
                }

                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Accediendo a la vista EditarTablero para el tablero con ID: {idTablero}."));
                return View(new EditarTableroViewModel
                {
                    IdTablero = idTablero,
                    NombreDeTablero = tablero.NombreTablero,
                    DescripcionDeTablero = tablero.DescripcionTablero!,
                    IdUsuarioPropietarioAnterior = tablero.IdUsuarioPropietario,
                    IdUsuarioPropietario = tablero.IdUsuarioPropietario,
                    ListadoUsuarios = _usuarioRepository.ObtenerTodosLosUsuarios()
                });
            } else {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos intentó acceder al metodo EditarTablero del controlador de Tablero."));
                return RedirectToAction("Index");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EditarTablero del controlador Tablero."));
            return BadRequest();
        }
    }
    [HttpPost]
    public IActionResult EditarTablero(EditarTableroViewModel editarTableroViewModel)
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EditarTablero del controlador de Tablero."));
                return RedirectToAction("Index");
            }

            var esAdmin = Autorizaciones.EsAdmin(HttpContext);
            var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);
            if (esAdmin || idUsuarioLogueado == editarTableroViewModel.IdUsuarioPropietarioAnterior)
            {
                if (_tableroRepository.ExisteTablero(editarTableroViewModel.NombreDeTablero!, editarTableroViewModel.IdTablero))
                {
                    ModelState.AddModelError("NombreDeTablero", "El nombre del tablero ya existe.");
                    _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de crear un tablero con un nombre de tablero existente."));
                    var tablero = _tableroRepository.ObtenerTableroPorId(editarTableroViewModel.IdTablero);
                    return View(new EditarTableroViewModel()
                    {
                        IdTablero = tablero.IdTablero,
                        DescripcionDeTablero = tablero.DescripcionTablero,
                        NombreDeTablero = tablero.NombreTablero,
                        IdUsuarioPropietario = tablero.IdUsuarioPropietario,
                        IdUsuarioPropietarioAnterior = tablero.IdUsuarioPropietario,
                        ListadoUsuarios = _usuarioRepository.ObtenerTodosLosUsuarios()
                    });
                }
                if (ModelState.IsValid)
                {
                    _tableroRepository.EditarTablero(new Tablero
                    {
                        IdTablero = editarTableroViewModel.IdTablero,
                        NombreTablero = editarTableroViewModel.NombreDeTablero,
                        DescripcionTablero = editarTableroViewModel.DescripcionDeTablero,
                        IdUsuarioPropietario = editarTableroViewModel.IdUsuarioPropietario ?? editarTableroViewModel.IdUsuarioPropietarioAnterior
                    });
                    _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Se ha modificado el tablero con ID: {editarTableroViewModel.IdTablero}"));
                    return RedirectToAction("Index");
                }
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("ModelState no es válido en EditarTablero"));
                return RedirectToAction("Index");
            } else {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos intentó acceder al metodo EditarTablero del controlador de Tablero."));
                return RedirectToAction("Index");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EditarTablero del controlador Tablero."));
            return BadRequest();
        }
    }

    public IActionResult EliminarTablero(int idTablero, int idUsuarioProp) 
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso sin loguearse: Alguien intentó acceder sin estar logueado al método EliminarTablero del controlador de Tablero."));
                return RedirectToAction("Index", "Login");
            }
            
            var esAdmin = Autorizaciones.EsAdmin(HttpContext);
            var idUsuarioLogueado = Autorizaciones.ObtenerIdUsuario(HttpContext);
            if (esAdmin || idUsuarioLogueado == idUsuarioProp)
            {
                _tableroRepository.EliminarTablero(idTablero);
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"Se ha eliminado el tablero con ID {idTablero}."));
                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de acceso denegado: Usuario sin permisos intentó acceder al método EliminarTablero del controlador de Tablero."));
                return RedirectToAction("Index");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al ejecutar el método EliminarTablero del controlador Tablero."));
            return BadRequest();
        }
    }

    // Auxiliares
    private TableroViewModel ConvertirATableroViewModel(Tablero tablero)
    {
        return new TableroViewModel
        {
            IdTableroVM = tablero.IdTablero,
            IdUsuarioPropietarioVM = tablero.IdUsuarioPropietario,
            NombreTableroVM = tablero.NombreTablero,
            DescripcionTableroVM = tablero.DescripcionTablero!,
            NombrePropietarioTableroVM = _usuarioRepository.ObtenerNombreUsuario(tablero.IdUsuarioPropietario ?? 0),
            CantidadTareasVM = _tareaRepository.CantidadDeTareas(tablero.IdTablero),
            CantidadTareasRealizadasVM = _tareaRepository.CantidadDeTareasRealizadas(tablero.IdTablero)
        };
    }
}