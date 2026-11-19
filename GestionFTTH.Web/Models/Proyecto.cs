
using System;
using System.ComponentModel.DataAnnotations;

namespace GestionFTTH.Web.Models
{
    public class Proyecto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ingrese el código del proyecto")]
        [StringLength(30)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el nombre del proyecto")]
        [StringLength(150)]
        [Display(Name = "Nombre del proyecto")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [Required]
        [Display(Name = "Área")]
        public AreaProyecto Area { get; set; }

        [Required]
        public EstadoProyecto Estado { get; set; }
            = EstadoProyecto.Pendiente;

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inicio")]
        public DateTime? FechaInicio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de entrega")]
        public DateTime? FechaEntrega { get; set; }

        public DateTime FechaRegistro { get; set; }
            = DateTime.UtcNow;
    }

    public enum AreaProyecto
    {
        Prediseño = 1,
        Diseño = 2,
        Construccion = 3
    }

    public enum EstadoProyecto
    {
        Pendiente = 1,
        EnEjecucion = 2,
        Finalizado = 3
    }
}
