using System.ComponentModel.DataAnnotations.Schema;

namespace Guardian.Domain.Common.Entities
{
    /// <summary>
    /// Entidade auditável abstrata que serve como base para criação de outras entidades.
    /// Herda de Entity.
    /// </summary>
    public class AuditableEntity : Entity
    {
        [Column("criado_por")]
        public int CreatedBy { get; set; }
        [Column("criado_em")]
        public DateTime CreatedAt { get; set; }

        [Column("atualizado_por")]
        public int LastUpdatedBy { get; set; }
        [Column("atualizado_em")]
        public DateTime LastUpdatedAt { get; set; }

        public AuditableEntity()
        {
            CreatedAt = DateTime.Now;
            LastUpdatedAt = DateTime.Now;
        }
    }
}
