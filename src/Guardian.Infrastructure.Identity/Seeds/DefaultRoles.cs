using Microsoft.AspNetCore.Identity;
using Guardian.Domain.User.Entities;
using Guardian.Domain.User.Enums;

namespace Guardian.Infrastructure.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager)
        {
            await roleManager.CreateAsync(ApplicationRole.Create(ERoles.SuperAdmin.ToString(), "Super Administrador"));
            await roleManager.CreateAsync(ApplicationRole.Create(ERoles.Admin.ToString(), "Administrador"));
            await roleManager.CreateAsync(ApplicationRole.Create(ERoles.User.ToString(), "Usuario"));
            await roleManager.CreateAsync(ApplicationRole.Create(nameof(ERoles.ReadOnly), "Leitor"));
        }
    }
}