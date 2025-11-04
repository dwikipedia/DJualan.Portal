using DJualan.Core.Interfaces.Base;
using DJualan.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DJualan.Data.Repositories.Base
{
    public abstract class BaseRepository<TEntity, TId> : IRepository<TEntity, TId>
     where TEntity : class, IEntity<TId>
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ILogger _logger;

        public BaseRepository(AppDbContext context, ILogger logger)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _logger = logger;
        }

        public virtual async Task<TEntity?> GetByIdAsync(TId id)
        {
            try
            {
                _logger.LogDebug("Getting {EntityName} by ID: {Id}", typeof(TEntity).Name, id);
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting {EntityName} by ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            try
            {
                _logger.LogDebug("Creating new {EntityName}", typeof(TEntity).Name);

                // Set timestamps
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;

                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created {EntityName} with ID: {Id}", typeof(TEntity).Name, entity.Id);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating {EntityName}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<TEntity?> UpdateAsync(TEntity entity)
        {
            try
            {
                _logger.LogDebug("Updating {EntityName} with ID: {Id}", typeof(TEntity).Name, entity.Id);

                entity.UpdatedAt = DateTime.UtcNow;

                _dbSet.Update(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated {EntityName} with ID: {Id}", typeof(TEntity).Name, entity.Id);
                return entity;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict updating {EntityName} with ID: {Id}", typeof(TEntity).Name, entity.Id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating {EntityName} with ID: {Id}", typeof(TEntity).Name, entity.Id);
                throw;
            }
        }

        public virtual async Task<bool> DeleteAsync(TId id)
        {
            try
            {
                _logger.LogDebug("Deleting {EntityName} with ID: {Id}", typeof(TEntity).Name, id);

                var entity = await GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("{EntityName} with ID: {Id} not found for deletion", typeof(TEntity).Name, id);
                    return false;
                }

                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted {EntityName} with ID: {Id}", typeof(TEntity).Name, id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {EntityName} with ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            try
            {
                _logger.LogDebug("Getting all {EntityName} records", typeof(TEntity).Name);
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all {EntityName} records", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogDebug("Getting {EntityName} page {PageNumber} with size {PageSize}",
                    typeof(TEntity).Name, pageNumber, pageSize);

                return await _dbSet
                    .OrderBy(e => e.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged {EntityName}", typeof(TEntity).Name);
                throw;
            }
        }

        public virtual async Task<int> GetCountAsync()
        {
            try
            {
                _logger.LogDebug("Getting count of {EntityName}", typeof(TEntity).Name);
                return await _dbSet.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting count of {EntityName}", typeof(TEntity).Name);
                throw;
            }
        }

        protected IQueryable<TEntity> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}

