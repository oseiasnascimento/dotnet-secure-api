using System.ComponentModel.DataAnnotations.Schema;

namespace Guardian.Domain.Common.Entities
{
    /// <summary>
    /// Entidade abstrata que serve como base para criação de outras entidades.
    /// </summary>
    public abstract class Entity
    {
        [Column("id")]
        public int Id { get; set; }
    }
}
