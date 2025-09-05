using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Guardian.Application.Accounts.Contracts;
using Guardian.Application.Accounts.Dtos.Inputs;
using Guardian.Domain.User.Entities;

namespace Guardian.Infrastructure.Identity.Seeds
{
    public class DefaultUsers
    {
        public static async Task SeedAsync(IAccountService accountService, UserManager<ApplicationUser> userManager)
        {
            RegisterRequest request = new()
            {
                FullName = "Coloque seu nome aqui",
                CPF = "Coloque seu CPF aqui",
                Email = "Coloque seu Email aqui",
                PhoneNumber = "Coloque seu Telefone aqui",
                Roles = ["SuperAdmin"],
                Password = "12345678",
                ConfirmPassword = "12345678"
            };

            bool userAlreadyRegistered = await userManager.Users.AnyAsync(x => x.CPF == request.CPF).ConfigureAwait(false);
            if (!userAlreadyRegistered)
            {
                await accountService.RegisterAsync(request);
            }
        }
    }
}
