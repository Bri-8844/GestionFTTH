
using GestionFTTH.Web.Data;
using Microsoft.AspNetCore.Identity;

namespace GestionFTTH.Web.Services
{
    public static class IdentitySeeder
    {
        public static async Task CrearRolesAsync(
            IServiceProvider services)
        {
            var roleManager = services
                .GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles =
            {
                "AdministradorPrincipal",
                "AdministradorGerencia",
                "JefeArea",
                "Trabajador"
            };

            foreach (var rol in roles)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    var resultado =
                        await roleManager.CreateAsync(
                            new IdentityRole(rol));

                    if (!resultado.Succeeded)
                    {
                        throw new Exception(
                            $"No se pudo crear el rol {rol}: " +
                            string.Join(", ",
                                resultado.Errors.Select(e =>
                                    e.Description)));
                    }
                }
            }
        }


        public static async Task CrearAdminInicialAsync(
            IServiceProvider services,
            IConfiguration configuration)
        {
            var userManager = services
                .GetRequiredService<UserManager<ApplicationUser>>();

            var email = configuration["AdminInicial:Email"];
            var password = configuration["AdminInicial:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Faltan las credenciales del administrador inicial.");
            }

            var usuario = await userManager.FindByEmailAsync(email);

            if (usuario == null)
            {
                usuario = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var resultado = await userManager.CreateAsync(
                    usuario, password);

                if (!resultado.Succeeded)
                {
                    throw new InvalidOperationException(
                        "No se pudo crear el administrador: " +
                        string.Join("; ",
                            resultado.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(
                usuario, "AdministradorPrincipal"))
            {
                var resultadoRol = await userManager.AddToRoleAsync(
                    usuario, "AdministradorPrincipal");

                if (!resultadoRol.Succeeded)
                {
                    throw new InvalidOperationException(
                        "No se pudo asignar el rol: " +
                        string.Join("; ",
                            resultadoRol.Errors.Select(e => e.Description)));
                }
            }
        }


    }
}
