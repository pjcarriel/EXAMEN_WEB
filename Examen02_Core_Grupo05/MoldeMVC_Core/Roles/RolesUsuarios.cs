using Microsoft.AspNetCore.Identity;

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
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Crear Usuarios
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
                    user = new IdentityUser
                    {
                        UserName = userInfo.Username,
                        Email = userInfo.Email,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, userInfo.Password);
                    if (result.Succeeded)
                    {
                        foreach (var role in userInfo.Roles)
                            await userManager.AddToRoleAsync(user, role);
                    }
                }
                else
                {
                    foreach (var role in userInfo.Roles)
                    {
                        if (!await userManager.IsInRoleAsync(user, role))
                            await userManager.AddToRoleAsync(user, role);
                    }
                }
            }
        }
    }
}
