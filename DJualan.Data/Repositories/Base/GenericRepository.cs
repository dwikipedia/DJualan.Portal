using DJualan.Core.Interfaces.Base;
using Microsoft.Extensions.Logging;

namespace DJualan.Data.Repositories.Base
{
    public class GenericRepository<TEntity, TId> : BaseRepository<TEntity, TId>, IRepository<TEntity, TId>
      where TEntity : class, IEntity<TId>
    {
        public GenericRepository(AppDbContext context, ILogger<GenericRepository<TEntity, TId>> logger)
            : base(context, logger)
        {
        }
    }
}
