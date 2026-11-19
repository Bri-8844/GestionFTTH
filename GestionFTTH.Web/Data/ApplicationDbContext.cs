
using GestionFTTH.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestionFTTH.Web.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tabla de proyectos
        public DbSet<Proyecto> Proyectos { get; set; }
            = null!;

        // Tabla de tareas
        public DbSet<Tarea> Tareas { get; set; }
            = null!;

        // Tabla de trabajadores
        public DbSet<Trabajador> Trabajadores { get; set; }
            = null!;

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Evitar códigos de proyecto duplicados
            builder.Entity<Proyecto>()
                .HasIndex(p => p.Codigo)
                .IsUnique();

            // Evitar códigos de tarea duplicados
            builder.Entity<Tarea>()
                .HasIndex(t => t.Codigo)
                .IsUnique();

            // Relación Proyecto (1) - Tareas (N)
            builder.Entity<Tarea>()
                .HasOne(t => t.Proyecto)
                .WithMany()
                .HasForeignKey(t => t.ProyectoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Evitar códigos de trabajador duplicados
            builder.Entity<Trabajador>()
                .HasIndex(t => t.Codigo)
                .IsUnique();
        }
    }
}
