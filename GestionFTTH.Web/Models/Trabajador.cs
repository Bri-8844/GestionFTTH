
using System.ComponentModel.DataAnnotations;

namespace GestionFTTH.Web.Models
{
    public class Trabajador
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese los nombres")]
        [StringLength(100)]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese los apellidos")]
        [StringLength(100)]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un área")]
        [Display(Name = "Área")]
        public AreaProyecto Area { get; set; }

        [Required(ErrorMessage = "Ingrese el cargo")]
        [StringLength(100)]
        [Display(Name = "Cargo")]
        public string Cargo { get; set; } = string.Empty;
    }
}
