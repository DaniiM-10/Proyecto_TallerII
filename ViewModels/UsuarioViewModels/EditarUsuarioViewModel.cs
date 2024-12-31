using System.ComponentModel.DataAnnotations;
using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.ViewModels;

public class EditarUsuarioViewModel
{
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es requerido.")]
    public string NombreUsuario { get; set; }
    public string? Password { get; set; }
    public Rol RolUsuario { get; set; }

    public EditarUsuarioViewModel() {}
}