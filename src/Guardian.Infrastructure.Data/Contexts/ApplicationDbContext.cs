using Guardian.Application.Accounts.Contracts;
using Guardian.Domain.Common.Entities;
using Guardian.Domain.User.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace Guardian.Infrastructure.Data.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,
                                                          ApplicationRole,
                                                          int,
                                                          IdentityUserClaim<int>,
                                                          UserRole,
                                                          IdentityUserLogin<int>,
                                                          IdentityRoleClaim<int>,
                                                          IdentityUserToken<int>>
    {
        private readonly IAuthenticatedUserService _authenticatedUser;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
                                    IAuthenticatedUserService authenticatedUser)
            : base(options)
        {
            _authenticatedUser = authenticatedUser;
        }

        #region Database Sets

        #endregion

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = _authenticatedUser.Id;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastUpdatedAt = DateTime.UtcNow;
                        entry.Entity.LastUpdatedBy = _authenticatedUser.Id;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // todos os decimais possuem um range de 18,6.
            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,6)");
            }

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "USUARIOS");

                entity.Property(u => u.FullName).HasColumnName("nome_completo");
                entity.Property(u => u.CPF).HasColumnName("cpf");
                entity.Property(u => u.IsActive).HasColumnName("ativo");
                entity.Property(u => u.RefreshToken).HasColumnName("refresh_token");
                entity.Property(u => u.IsDefaultPassword).HasColumnName("acesso_padrao");
                entity.Property(u => u.LastLogin).HasColumnName("ultimo_acesso");
                entity.Property(u => u.RefreshTokenValidity).HasColumnName("refresh_token_expiracao");
                entity.Property(u => u.UserName).HasColumnName("nome_usuario");
                entity.Property(u => u.NormalizedUserName).HasColumnName("nome_usuario_normalizado");
                entity.Property(u => u.Email).HasColumnName("email");
                entity.Property(u => u.NormalizedEmail).HasColumnName("email_normalizado");
                entity.Property(u => u.EmailConfirmed).HasColumnName("email_verificado");
                entity.Property(u => u.PasswordHash).HasColumnName("senha");
                entity.Property(u => u.SecurityStamp).HasColumnName("security_stamp");
                entity.Property(u => u.ConcurrencyStamp).HasColumnName("concurrency_stamp");
                entity.Property(u => u.PhoneNumber).HasColumnName("telefone");
                entity.Property(u => u.PhoneNumberConfirmed).HasColumnName("telefone_verificado");
                entity.Property(u => u.TwoFactorEnabled).HasColumnName("auth_dois_fatores_ativo");
                entity.Property(u => u.LockoutEnabled).HasColumnName("lockout_ativo");
                entity.Property(u => u.LockoutEnd).HasColumnName("lockout_expiracao");
                entity.Property(u => u.AccessFailedCount).HasColumnName("qtd_tentativas_acesso");
            });

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable(name: "CARGOS");
                entity.Property(c => c.Name).HasColumnName("nome");
                entity.Property(c => c.NormalizedName).HasColumnName("nome_normalizado");
                entity.Property(c => c.Description).HasColumnName("descricao");
                entity.Property(c => c.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            });

            builder.Entity<UserRole>(entity =>
            {
                entity.ToTable(name: "USUARIOS_CARGOS");

                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<IdentityUserClaim<int>>(entity =>
            {
                entity.ToTable(name: "USUARIOS_CLAIMS");
            });

            builder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.ToTable(name: "USUARIOS_LOGINS");
            });

            builder.Entity<IdentityRoleClaim<int>>(entity =>
            {
                entity.ToTable(name: "CARGOS_CLAIMS");
            });

            builder.Entity<IdentityUserToken<int>>(entity =>
            {
                entity.ToTable(name: "USUARIOS_TOKENS");
            });
        }
    }
}