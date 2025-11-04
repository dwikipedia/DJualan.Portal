using AutoMapper;
using DJualan.Core.Interfaces.Base;
using Microsoft.Extensions.Logging;

namespace DJualan.Service.Services.Base
{
    public abstract class BaseService<TEntity, TId, TRepository>
        where TEntity : class, IEntity<TId>
        where TRepository : IRepository<TEntity, TId>
    {
        protected readonly TRepository _repository;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        protected BaseService(TRepository repository, IMapper mapper, ILogger logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        protected void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation($"[{typeof(TEntity).Name}] {message}", args);
        }

        protected void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug($"[{typeof(TEntity).Name}] {message}", args);
        }

        protected void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning($"[{typeof(TEntity).Name}] {message}", args);
        }

        protected void LogError(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, $"[{typeof(TEntity).Name}] {message}", args);
        }
    }
}
