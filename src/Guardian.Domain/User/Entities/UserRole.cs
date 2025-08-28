using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guardian.Domain.User.Entities
{
    public class UserRole : IdentityUserRole<int>
    {
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(RoleId))]
        public ApplicationRole Role { get; set; }
    }
}