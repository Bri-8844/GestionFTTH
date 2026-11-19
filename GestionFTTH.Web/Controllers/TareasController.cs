
using GestionFTTH.Web.Data;
using GestionFTTH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionFTTH.Web.Controllers
{
    [Authorize(Roles =
        "AdministradorPrincipal,AdministradorGerencia")]
    public class TareasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TareasController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tareas
        public async Task<IActionResult> Index()
        {
            var tareas = await _context.Tareas
                .Include(t => t.Proyecto)
                .OrderByDescending(t => t.FechaRegistro)
                .ToListAsync();

            // Nombres de los responsables
            var usuarios = await _userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName
                })
                .ToListAsync();

            ViewBag.NombresResponsables = usuarios
                .ToDictionary(
                    u => u.Id,
                    u => u.UserName ?? "Sin nombre");

            return View(tareas);
        }

        // GET: Tareas/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();

            return View(new Tarea());
        }

        // POST: Tareas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Codigo,Titulo,Descripcion,ProyectoId,ResponsableId,Estado,Prioridad,FechaInicio,FechaLimite,PorcentajeAvance")]
            Tarea tarea)
        {
            // Validar proyecto
            bool proyectoExiste = await _context.Proyectos
                .AnyAsync(p => p.Id == tarea.ProyectoId);

            if (!proyectoExiste)
            {
                ModelState.AddModelError(
                    nameof(Tarea.ProyectoId),
                    "Seleccione un proyecto válido.");
            }

            // Validar responsable
            if (string.IsNullOrWhiteSpace(tarea.ResponsableId))
            {
                ModelState.AddModelError(
                    nameof(Tarea.ResponsableId),
                    "Seleccione un responsable.");
            }
            else
            {
                var responsable = await _userManager
                    .FindByIdAsync(tarea.ResponsableId);

                if (responsable == null ||
                    !await _userManager.IsInRoleAsync(
                        responsable, "Trabajador"))
                {
                    ModelState.AddModelError(
                        nameof(Tarea.ResponsableId),
                        "Seleccione un trabajador válido.");
                }
            }

            // Validar código duplicado
            if (!string.IsNullOrWhiteSpace(tarea.Codigo))
            {
                tarea.Codigo = tarea.Codigo.Trim();

                bool codigoExiste = await _context.Tareas
                    .AnyAsync(t => t.Codigo == tarea.Codigo);

                if (codigoExiste)
                {
                    ModelState.AddModelError(
                        nameof(Tarea.Codigo),
                        "Ya existe una tarea con este código.");
                }
            }

            // Validar fechas
            if (tarea.FechaInicio.HasValue &&
                tarea.FechaLimite.HasValue &&
                tarea.FechaLimite.Value.Date <
                tarea.FechaInicio.Value.Date)
            {
                ModelState.AddModelError(
                    nameof(Tarea.FechaLimite),
                    "La fecha límite no puede ser anterior a la fecha de inicio.");
            }

            // Validar estado
            if (!Enum.IsDefined(
                typeof(EstadoTarea), tarea.Estado))
            {
                ModelState.AddModelError(
                    nameof(Tarea.Estado),
                    "Seleccione un estado válido.");
            }

            // Validar prioridad
            if (!Enum.IsDefined(
                typeof(PrioridadTarea), tarea.Prioridad))
            {
                ModelState.AddModelError(
                    nameof(Tarea.Prioridad),
                    "Seleccione una prioridad válida.");
            }

            if (ModelState.IsValid)
            {
                tarea.FechaRegistro = DateTime.UtcNow;

                _context.Tareas.Add(tarea);
                await _context.SaveChangesAsync();

                TempData["MensajeExito"] =
                    "La tarea se registró correctamente.";

                return RedirectToAction(nameof(Index));
            }

            // Recuperar listas si hay errores
            await CargarListasAsync();

            return View(tarea);
        }

        // Cargar proyectos y trabajadores
        private async Task CargarListasAsync()
        {
            ViewBag.Proyectos = await _context.Proyectos
                .OrderBy(p => p.Codigo)
                .ToListAsync();

            var trabajadores = await _userManager
                .GetUsersInRoleAsync("Trabajador");

            ViewBag.Trabajadores = trabajadores
                .OrderBy(u => u.UserName)
                .ToList();
        }
    }
}
