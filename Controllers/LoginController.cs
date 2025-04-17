using Microsoft.AspNetCore.Mvc;
using Proyecto_TallerII.Models;
using Proyecto_TallerII.Repositories;
using Proyecto_TallerII.Helpers;
using Proyecto_TallerII.ViewModels;
namespace Proyecto_TallerII.Controllers;

public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    private readonly IUsuarioRepository _usuarioRepository;
    public LoginController(ILogger<LoginController> logger, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _usuarioRepository = usuarioRepository;
    }

    public IActionResult Index()
    {
        try
        {
            if(!AuthHelper.EstaAutenticado(HttpContext)) 
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Acceso no autenticado al método Index del controlador Login."));
                return View(new LoginViewModel());
            }

            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Accediendo al método Index del controlador Login."));
            return RedirectToAction("Index", "Tablero");
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al acceder al método Index del controlador Login."));
            return StatusCode(500);
        }
    }
    [HttpPost]
    public IActionResult Index(LoginViewModel loginViewModel)
    {
        try
        {
            if (!ModelState.IsValid) 
            {
                TempData["Mensaje"] = "Por favor, complete todos los campos.";
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("ModelState no válido en el método Index del controlador Login."));
                return View(loginViewModel);
            }

            var usuarioLogin = _usuarioRepository.ObtenerUsuarioPorCredenciales(loginViewModel.NombreUsuario!, loginViewModel.Password!);
            if (usuarioLogin == null)
            {
                TempData["Mensaje"] = "Credenciales inválidas. Intente nuevamente.";
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn($"Intento de acceso inválido - Usuario: {loginViewModel.NombreUsuario}"));
                return View(loginViewModel);
            }
            
            LogearUsuario(usuarioLogin);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Ocurrió un error al procesar la solicitud. Por favor, inténtalo nuevamente más tarde."));
            return StatusCode(500);
        }   
    }
    
    [HttpGet]
    public IActionResult Logout()
    {
        try
        {
            if(!AuthHelper.EstaAutenticado(HttpContext)) 
            {
                _logger.LogWarning(LoggerMsj.MensajeInfoWarn("Intento de cerrar sesión sin estar autenticado."));
                return RedirectToAction("Index");
            }

            HttpContext.Session.Clear();
            TempData["Mensaje"] = "¡La sesión se cerró exitosamente!";
            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("La sesión se cerró exitosamente para el usuario."));
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Ocurrió un error al cerrar sesión. Por favor, inténtalo nuevamente más tarde."));
            return StatusCode(500);
        }
    }

    
    private void LogearUsuario(Usuario usuario)
    {
        try
        {
            HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
            HttpContext.Session.SetString("Usuario", usuario.NombreUsuario!);
            HttpContext.Session.SetString("Rol", usuario.RolUsuario.ToString());
            _logger.LogInformation(LoggerMsj.MensajeInfoWarn($"El usuario {usuario.NombreUsuario} se ha registrado en la sesión con ID: {usuario.IdUsuario} y rol: {usuario.RolUsuario}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al iniciar sesión del usuario en la sesión"));
            throw new Exception("Error al iniciar sesión del usuario en la sesión", ex);
        }
    }
}