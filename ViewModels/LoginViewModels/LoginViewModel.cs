using System.ComponentModel.DataAnnotations;
namespace Proyecto_TallerII.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "El Usuario es obligatorio.")]
    public string NombreUsuario { get; set; }

    [Required(ErrorMessage = "La Contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public LoginViewModel() {}
}