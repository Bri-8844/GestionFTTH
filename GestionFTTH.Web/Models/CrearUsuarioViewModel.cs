
using System.ComponentModel.DataAnnotations;

namespace GestionFTTH.Web.Models
{
    public class CrearUsuarioViewModel
    {
        [Required(ErrorMessage = "Ingrese el correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese una contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme la contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password",
            ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un rol")]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = string.Empty;

        [Display(Name = "Trabajador")]
        public int? TrabajadorId { get; set; }
    }
}
