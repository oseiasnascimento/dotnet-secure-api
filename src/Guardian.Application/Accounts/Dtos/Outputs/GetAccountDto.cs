using Guardian.Domain.User.Entities;

namespace Guardian.Application.Accounts.Dtos.Outputs
{
    public class GetAccountDto
    {
        public string FullName { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<string> Roles { get; set; }

        public static GetAccountDto Map(ApplicationUser user, IEnumerable<string> roles)
        {
            return new()
            {
                FullName = user.FullName,
                CPF = user.CPF,
                Email = user.Email,
                IsActive = user.IsActive,
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            };
        }

        public static IEnumerable<GetAccountDto> Map(IEnumerable<ApplicationUser> users)
        {
            return users.Select(user => new GetAccountDto
            {
                FullName = user.FullName,
                CPF = user.CPF,
                Email = user.Email,
                IsActive = user.IsActive,
                PhoneNumber = user.PhoneNumber,
                Roles = user.UserRoles.Select(x => x.Role.Name).ToList(),

            });
        }
    }
}