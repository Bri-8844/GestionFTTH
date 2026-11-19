
using GestionFTTH.Web.Data;
using GestionFTTH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionFTTH.Web.Controllers
{
    [Authorize(Roles =
        "AdministradorPrincipal,AdministradorGerencia")]
    public class ProyectosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProyectosController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // Listar proyectos

        public async Task<IActionResult> Index(AreaProyecto? area)
        {
            var consulta = _context.Proyectos.AsNoTracking();

            if (area.HasValue)
            {
                if (!Enum.IsDefined(typeof(AreaProyecto), area.Value))
                {
                    return BadRequest("Área de proyecto no válida.");
                }

                consulta = consulta.Where(p => p.Area == area.Value);
            }

            var proyectos = await consulta
                .OrderByDescending(p => p.FechaRegistro)
                .ToListAsync();

            ViewBag.AreaSeleccionada = area;

            return View(proyectos);
        }


        // Mostrar detalles de un proyecto
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var proyecto = await _context.Proyectos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proyecto == null)
            {
                return NotFound();
            }

            return View(proyecto);
        }


        // Mostrar formulario de edición
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var proyecto = await _context.Proyectos
                .FindAsync(id);

            if (proyecto == null)
            {
                return NotFound();
            }

            return View(proyecto);
        }

        // Guardar cambios del proyecto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Codigo,Nombre,Descripcion,Area,Estado,FechaInicio,FechaEntrega")]
    Proyecto datos)
        {
            if (id != datos.Id)
            {
                return BadRequest();
            }

            var proyecto = await _context.Proyectos
                .FindAsync(id);

            if (proyecto == null)
            {
                return NotFound();
            }

            if (!Enum.IsDefined(typeof(AreaProyecto), datos.Area))
            {
                ModelState.AddModelError(
                    nameof(datos.Area), "Seleccione un área válida.");
            }

            if (!Enum.IsDefined(typeof(EstadoProyecto), datos.Estado))
            {
                ModelState.AddModelError(
                    nameof(datos.Estado), "Seleccione un estado válido.");
            }

            if (datos.FechaInicio.HasValue &&
                datos.FechaEntrega.HasValue &&
                datos.FechaEntrega.Value.Date < datos.FechaInicio.Value.Date)
            {
                ModelState.AddModelError(
                    nameof(datos.FechaEntrega),
                    "La fecha de entrega no puede ser anterior al inicio.");
            }

            bool codigoDuplicado = await _context.Proyectos
                .AnyAsync(p => p.Codigo == datos.Codigo && p.Id != id);

            if (codigoDuplicado)
            {
                ModelState.AddModelError(
                    nameof(datos.Codigo),
                    "Ya existe otro proyecto con ese código.");
            }

            if (!ModelState.IsValid)
            {
                return View(datos);
            }

            proyecto.Codigo = datos.Codigo;
            proyecto.Nombre = datos.Nombre;
            proyecto.Descripcion = datos.Descripcion;
            proyecto.Area = datos.Area;
            proyecto.Estado = datos.Estado;
            proyecto.FechaInicio = datos.FechaInicio;
            proyecto.FechaEntrega = datos.FechaEntrega;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Proyecto actualizado correctamente.";

            return RedirectToAction("Details", new { id = proyecto.Id });
        }


        // Mostrar formulario de registro
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Proyecto());
        }

        // Guardar proyecto en SQL Server
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Proyecto proyecto)
        {
            if (!Enum.IsDefined(typeof(AreaProyecto),
                proyecto.Area))
            {
                ModelState.AddModelError(
                    nameof(proyecto.Area),
                    "Seleccione un área válida.");
            }

            if (!Enum.IsDefined(typeof(EstadoProyecto),
                proyecto.Estado))
            {
                ModelState.AddModelError(
                    nameof(proyecto.Estado),
                    "Seleccione un estado válido.");
            }

            if (proyecto.FechaInicio.HasValue &&
                proyecto.FechaEntrega.HasValue &&
                proyecto.FechaEntrega < proyecto.FechaInicio)
            {
                ModelState.AddModelError(
                    nameof(proyecto.FechaEntrega),
                    "La fecha de entrega no puede ser anterior al inicio.");
            }

            if (await _context.Proyectos.AnyAsync(
                p => p.Codigo == proyecto.Codigo))
            {
                ModelState.AddModelError(
                    nameof(proyecto.Codigo),
                    "Ya existe un proyecto con ese código.");
            }

            if (!ModelState.IsValid)
            {
                return View(proyecto);
            }

            proyecto.Id = 0;
            proyecto.FechaRegistro = DateTime.UtcNow;

            _context.Proyectos.Add(proyecto);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Proyecto registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}
