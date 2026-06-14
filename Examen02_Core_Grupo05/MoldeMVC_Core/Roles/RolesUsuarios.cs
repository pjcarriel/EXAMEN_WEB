using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MoldeMVC_Core.Roles
{
    public class RolesUsuarios
    {
        public static async Task InitializeRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Crear Roles
            string[] roles = { "ADMIN", "ABOGADO", "ESTUDIANTE", "PROFESOR", "PLANIFICADOR", "DECANO" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Crear Usuarios predeterminados
            var users = new[]
            {
                new { Username = "Administrador", Email = "Adminitrador@hotmail.com", Password = "Admin.123",  Roles = new[] { "ADMIN" } },
                new { Username = "Secretario",    Email = "Secretario@hotmail.com",   Password = "Agogado.123", Roles = new[] { "ABOGADO" } }
            };

            foreach (var userInfo in users)
            {
                var user = await userManager.FindByNameAsync(userInfo.Username);
                if (user == null)
                {
                    user = new IdentityUser { UserName = userInfo.Username, Email = userInfo.Email, EmailConfirmed = true };
                    var result = await userManager.CreateAsync(user, userInfo.Password);
                    if (result.Succeeded)
                        foreach (var r in userInfo.Roles)
                            await userManager.AddToRoleAsync(user, r);
                }
                else
                {
                    foreach (var r in userInfo.Roles)
                        if (!await userManager.IsInRoleAsync(user, r))
                            await userManager.AddToRoleAsync(user, r);
                }
            }

            // Sembrar permisos en AspNetRoleClaims
            // Formato: ClaimType = "Modulo.Accion", ClaimValue = "true"
            var permisos = new[]
            {
                // DECANO - acceso completo
                ("DECANO",       "Estudiantes",      true,  true,  true,  true),
                ("DECANO",       "Profesors",        true,  true,  true,  true),
                ("DECANO",       "CatalogoMaterias", true,  true,  true,  true),
                // PLANIFICADOR - acceso completo a sus módulos
                ("PLANIFICADOR", "NrcMateriums",     true,  true,  true,  true),
                ("PLANIFICADOR", "Matriculas",       true,  true,  true,  true),
                // ESTUDIANTE - solo ver sus matrículas
                ("ESTUDIANTE",   "Matriculas",       true,  false, false, false),
                // PROFESOR - solo ver sus NRC
                ("PROFESOR",     "NrcMateriums",     true,  false, false, false),
                // ABOGADO - solo ver catálogo
                ("ABOGADO",      "CatalogoMaterias", true,  false, false, false),
            };

            foreach (var (rolNombre, modulo, ver, crear, editar, eliminar) in permisos)
            {
                var rol = await roleManager.FindByNameAsync(rolNombre);
                if (rol == null) continue;

                var claimsExistentes = await roleManager.GetClaimsAsync(rol);

                var nuevos = new[]
                {
                    (modulo + ".PuedeVer",      ver),
                    (modulo + ".PuedeCrear",    crear),
                    (modulo + ".PuedeEditar",   editar),
                    (modulo + ".PuedeEliminar", eliminar),
                };

                foreach (var (tipo, valor) in nuevos)
                {
                    // Solo agregar si no existe aún
                    if (!claimsExistentes.Any(c => c.Type == tipo))
                    {
                        if (valor)
                            await roleManager.AddClaimAsync(rol, new Claim(tipo, "true"));
                    }
                }
            }
        }
    }
}
