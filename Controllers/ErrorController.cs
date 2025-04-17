using Microsoft.AspNetCore.Mvc;
using Proyecto_TallerII.ViewModels;
namespace Proyecto_TallerII.Controllers;

[Route("Error")]
public class ErrorController : Controller
{
    [Route("{statusCode}")]
    public IActionResult HandleError(int statusCode)
    {
        var errorViewModel = statusCode switch
        {
            500 => new ErrorViewModel
            {
                StatusCode = "500",
                Title = "Error interno del servidor.",
                Message = "Ocurrió un error en el servidor."
            },
            404 => new ErrorViewModel
            {
                StatusCode = "404",
                Title = "Página no encontrada.",
                Message = "La página solicitada no existe."
            },
            _ => new ErrorViewModel
            {
                StatusCode = statusCode.ToString(),
                Title = "Error inesperado.",
                Message = "Algo salió mal."
            }
        };

        return View("Error", errorViewModel);
    }
}
