using DJualan.Core.Interfaces.Base;

namespace DJualan.Core.Entities.Base
{
    public abstract class BaseEntity : IEntity<int>
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
