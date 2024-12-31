using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Proyecto_TallerII.Models;
using Proyecto_TallerII.Repositories;
namespace Proyecto_TallerII.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        try
        {
            if (!Autorizaciones.EstaAutenticado(HttpContext))
            {
                _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Intento de acceso sin autenticación al método Index del controlador Home. Redirigiendo al login."));
                return RedirectToAction("Index", "Login");
            }
            _logger.LogInformation(LoggerMsj.MensajeInfoWarn("Accediendo al Inicio de la aplicación."));
            return RedirectToAction("Index", "Tablero");
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al acceder al método Index del controlador Home."));
            return BadRequest();
        }
    }
}
