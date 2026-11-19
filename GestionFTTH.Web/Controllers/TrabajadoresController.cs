
using GestionFTTH.Web.Data;
using GestionFTTH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionFTTH.Web.Controllers
{
    [Authorize(Roles =
        "AdministradorPrincipal,AdministradorGerencia")]
    public class TrabajadoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrabajadoresController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Trabajadores
        public async Task<IActionResult> Index()
        {
            var trabajadores = await _context.Trabajadores
                .OrderBy(t => t.Apellidos)
                .ThenBy(t => t.Nombres)
                .ToListAsync();

            return View(trabajadores);
        }

        // GET: Trabajadores/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Trabajador());
        }

        // POST: Trabajadores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Nombres,Apellidos,Area,Cargo")]
            Trabajador trabajador)
        {
            // Normalizar los datos ingresados
            trabajador.Nombres =
                trabajador.Nombres?.Trim() ?? "";

            trabajador.Apellidos =
                trabajador.Apellidos?.Trim() ?? "";

            trabajador.Cargo =
                trabajador.Cargo?.Trim() ?? "";

            // El código se genera en el servidor
            trabajador.Codigo = "TEMP";

            ModelState.Remove(nameof(Trabajador.Codigo));

            // Validar el área
            if (!Enum.IsDefined(
                typeof(AreaProyecto), trabajador.Area))
            {
                ModelState.AddModelError(
                    nameof(Trabajador.Area),
                    "Seleccione un área válida.");
            }

            if (!ModelState.IsValid)
            {
                return View(trabajador);
            }

            // Transacción para guardar el trabajador
            // y generar su código definitivo
            await using var transaccion =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Código temporal único para evitar
                // conflictos con el índice único
                trabajador.Codigo =
                    $"TEMP-{Guid.NewGuid():N}";

                // El campo Codigo admite 20 caracteres
                trabajador.Codigo =
                    trabajador.Codigo.Substring(0, 20);

                _context.Trabajadores.Add(trabajador);

                await _context.SaveChangesAsync();

                // Generar código a partir del Id
                trabajador.Codigo =
                    $"TRB{trabajador.Id:D4}";

                await _context.SaveChangesAsync();

                await transaccion.CommitAsync();

                TempData["MensajeExito"] =
                    "El trabajador se registró correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                await transaccion.RollbackAsync();

                ModelState.AddModelError(
                    "",
                    "No se pudo registrar el trabajador. Intente nuevamente.");

                return View(trabajador);
            }

        }

        // GET: Trabajadores/Edit/1
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var trabajador = await _context.Trabajadores
                .FindAsync(id);

            if (trabajador == null)
            {
                return NotFound();
            }

            return View(trabajador);
        }

        // POST: Trabajadores/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Nombres,Apellidos,Area,Cargo")]
            Trabajador datos)
        {
            if (id != datos.Id)
            {
                return NotFound();
            }

            var trabajador = await _context.Trabajadores
                .FindAsync(id);

            if (trabajador == null)
            {
                return NotFound();
            }

            // El código original no se modifica
            ModelState.Remove(nameof(Trabajador.Codigo));

            if (!Enum.IsDefined(typeof(AreaProyecto), datos.Area))
            {
                ModelState.AddModelError(
                    nameof(Trabajador.Area),
                    "Seleccione un área válida.");
            }

            if (!ModelState.IsValid)
            {
                datos.Codigo = trabajador.Codigo;
                return View(datos);
            }

            trabajador.Nombres = datos.Nombres.Trim();
            trabajador.Apellidos = datos.Apellidos.Trim();
            trabajador.Area = datos.Area;
            trabajador.Cargo = datos.Cargo.Trim();

            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "Los datos del trabajador se actualizaron correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // POST: Trabajadores/Delete/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var trabajador = await _context.Trabajadores
                .FindAsync(id);

            if (trabajador == null)
            {
                return NotFound();
            }

            _context.Trabajadores.Remove(trabajador);

            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "El trabajador se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }




    }
}
