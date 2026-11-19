
using GestionFTTH.Web.Models;
using GestionFTTH.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionFTTH.Web.Controllers
{
    [Authorize(Roles = "AdministradorPrincipal,AdministradorGerencia")]
    public class UsuariosController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UsuariosController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var trabajadores = await _context.Trabajadores
                .ToListAsync();

            ViewBag.Trabajadores = trabajadores;

            var rolesUsuarios = new Dictionary<string, string>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);

                rolesUsuarios[usuario.Id] =
                    roles.FirstOrDefault() ?? "Sin rol";
            }

            ViewBag.RolesUsuarios = rolesUsuarios;


            return View(usuarios);
        }

        // GET: Usuarios/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Obtener trabajadores sin cuenta de usuario
            var trabajadoresConCuenta = await _userManager.Users
                .Where(u => u.TrabajadorId != null)
                .Select(u => u.TrabajadorId)
                .ToListAsync();

            var trabajadores = await _context.Trabajadores
                .Where(t => !trabajadoresConCuenta.Contains(t.Id))
                .OrderBy(t => t.Apellidos)
                .ToListAsync();

            ViewBag.Trabajadores = trabajadores;

            return View(new CrearUsuarioViewModel());
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CrearUsuarioViewModel modelo)
        {
            // Roles permitidos
            var rolesPermitidos = new[]
            {
                "AdministradorPrincipal",
                "AdministradorGerencia",
                "JefeArea",
                "Trabajador"
            };

            if (!rolesPermitidos.Contains(modelo.Rol))
            {
                ModelState.AddModelError(
                    "Rol", "Seleccione un rol válido.");
            }

            // Trabajadores y jefes de área necesitan vinculación
            bool requiereTrabajador =
                modelo.Rol == "Trabajador" ||
                modelo.Rol == "JefeArea";

            if (requiereTrabajador && modelo.TrabajadorId == null)
            {
                ModelState.AddModelError(
                    "TrabajadorId",
                    "Seleccione un trabajador.");
            }

            // Para otros roles no se vincula trabajador
            if (!requiereTrabajador)
            {
                modelo.TrabajadorId = null;
            }

            if (modelo.TrabajadorId.HasValue)
            {
                int trabajadorId = modelo.TrabajadorId.Value;

                bool existe = await _context.Trabajadores
                    .AnyAsync(t => t.Id == trabajadorId);

                bool tieneCuenta = await _userManager.Users
                    .AnyAsync(u => u.TrabajadorId == trabajadorId);

                if (!existe || tieneCuenta)
                {
                    ModelState.AddModelError(
                        "TrabajadorId",
                        "El trabajador no existe o ya tiene una cuenta.");
                }
            }

            if (!ModelState.IsValid)
            {
                await CargarTrabajadores();
                return View(modelo);
            }

            // Crear usuario
            var usuario = new ApplicationUser
            {
                UserName = modelo.Email,
                Email = modelo.Email,
                TrabajadorId = modelo.TrabajadorId
            };

            var resultado = await _userManager.CreateAsync(
                usuario, modelo.Password);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                await CargarTrabajadores();
                return View(modelo);
            }

            // Asignar rol
            var resultadoRol = await _userManager.AddToRoleAsync(
                usuario, modelo.Rol);

            if (!resultadoRol.Succeeded)
            {
                // Evitar dejar una cuenta sin rol
                await _userManager.DeleteAsync(usuario);

                foreach (var error in resultadoRol.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                await CargarTrabajadores();
                return View(modelo);
            }

            TempData["MensajeExito"] =
                "El usuario se registró correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Cargar trabajadores disponibles
        private async Task CargarTrabajadores()
        {
            var idsOcupados = await _userManager.Users
                .Where(u => u.TrabajadorId != null)
                .Select(u => u.TrabajadorId)
                .ToListAsync();

            ViewBag.Trabajadores = await _context.Trabajadores
                .Where(t => !idsOcupados.Contains(t.Id))
                .OrderBy(t => t.Apellidos)
                .ToListAsync();
        }


    }
}
