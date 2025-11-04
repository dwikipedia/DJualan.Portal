using DJualan.Core.Models;

namespace DJualan.Core.Interfaces.Base
{

    public interface IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        Task<TEntity?> GetByIdAsync(TId id);
        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity?> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(TId id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        //Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize);
        //Task<int> GetCountAsync();
    }

    public interface IEntity<TId>
    {
        TId Id { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }

    public interface ITimestamped
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}
