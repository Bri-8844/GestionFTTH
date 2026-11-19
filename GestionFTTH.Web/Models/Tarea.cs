
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFTTH.Web.Models
{
    public class Tarea
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ingrese el código de la tarea")]
        [StringLength(30)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el título de la tarea")]
        [StringLength(150)]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        // Relación con Proyecto
        [Required(ErrorMessage = "Seleccione un proyecto")]
        [Display(Name = "Proyecto")]
        public int ProyectoId { get; set; }

        [ForeignKey(nameof(ProyectoId))]
        public Proyecto? Proyecto { get; set; }

        // Responsable de la tarea
        [StringLength(450)]
        public string? ResponsableId { get; set; }

        // Estado y prioridad
        [Display(Name = "Estado")]
        public EstadoTarea Estado { get; set; }
            = EstadoTarea.Pendiente;

        [Display(Name = "Prioridad")]
        public PrioridadTarea Prioridad { get; set; }
            = PrioridadTarea.Media;

        // Cronograma
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inicio")]
        public DateTime? FechaInicio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha límite")]
        public DateTime? FechaLimite { get; set; }

        // Seguimiento
        [Range(0, 100,
            ErrorMessage = "El avance debe estar entre 0 y 100")]
        [Display(Name = "Porcentaje de avance")]
        public int PorcentajeAvance { get; set; } = 0;

        public DateTime FechaRegistro { get; set; }
            = DateTime.UtcNow;
    }

    public enum EstadoTarea
    {
        Pendiente = 1,
        EnEjecucion = 2,
        Finalizada = 3
    }

    public enum PrioridadTarea
    {
        Baja = 1,
        Media = 2,
        Alta = 3
    }
}
