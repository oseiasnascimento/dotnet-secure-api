using Microsoft.AspNetCore.Identity;

namespace Guardian.Domain.User.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; }
        public string CPF { get; set; }
        public bool IsActive { get; set; }
        public string RefreshToken { get; set; }
        public bool IsDefaultPassword { get; set; } = true;
        public DateTime? LastLogin { get; set; }
        public DateTime RefreshTokenValidity { get; set; }

        public List<UserRole> UserRoles { get; set; } = [];

        public ApplicationUser()
        {
            IsActive = true;
        }
    }
}