using Microsoft.AspNetCore.Identity;
using Guardian.Application.Accounts.Contracts;
using Guardian.Domain.User.Entities;

namespace Guardian.WebApi.Helpers
{
    public static class SeedDatabaseHelper
    {
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            {
                try
                {
                    UserManager<ApplicationUser> userManager = (UserManager<ApplicationUser>)scope
                        .ServiceProvider
                        .GetService(typeof(UserManager<ApplicationUser>));

                    IAccountService accountService = (IAccountService)scope
                        .ServiceProvider
                        .GetService(typeof(IAccountService));

                    RoleManager<ApplicationRole> roleManager = (RoleManager<ApplicationRole>)scope
                        .ServiceProvider
                        .GetService(typeof(RoleManager<ApplicationRole>));

                    await Infrastructure.Identity.Seeds.DefaultRoles.SeedAsync(roleManager);
                    await Infrastructure.Identity.Seeds.DefaultUsers.SeedAsync(accountService, userManager);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ops! An Error ocurred while seeding database: {ex}");
                }
            }
        }
    }
}