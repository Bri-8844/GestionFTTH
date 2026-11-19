
using GestionFTTH.Web.Data;
using GestionFTTH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionFTTH.Web.Controllers
{
    [Authorize(Roles =
        "AdministradorPrincipal,AdministradorGerencia")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var proyectos = _context.Proyectos.AsNoTracking();

            ViewBag.TotalProyectos = await proyectos.CountAsync();

            ViewBag.EnEjecucion = await proyectos.CountAsync(
                p => p.Estado == EstadoProyecto.EnEjecucion);

            ViewBag.Finalizados = await proyectos.CountAsync(
                p => p.Estado == EstadoProyecto.Finalizado);

            ViewBag.Pendientes = await proyectos.CountAsync(
                p => p.Estado == EstadoProyecto.Pendiente);

            ViewBag.TotalPrediseno = await proyectos.CountAsync(
                p => p.Area == AreaProyecto.Prediseño);

            ViewBag.TotalDiseno = await proyectos.CountAsync(
                p => p.Area == AreaProyecto.Diseño);

            ViewBag.TotalConstruccion = await proyectos.CountAsync(
                p => p.Area == AreaProyecto.Construccion);


            return View();
        }
    }
}
