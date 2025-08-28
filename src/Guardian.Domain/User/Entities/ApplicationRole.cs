using Microsoft.AspNetCore.Identity;

namespace Guardian.Domain.User.Entities
{
    /// <summary>
    /// Cargos do sistema.
    /// </summary>
    public class ApplicationRole : IdentityRole<int>
    {
        public string Description { get; set; }
        public List<UserRole> UserRoles { get; set; }

        public static ApplicationRole Create(string name, string description)
        {
            return new()
            {
                Name = name,
                NormalizedName = name.ToUpper().Trim(),
                Description = description,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
        }
    }
}
