using DJualan.Core.Interfaces;
using DJualan.Core.Interfaces.Base;
using DJualan.Data.Repositories;
using DJualan.Data.Repositories.Base;
using Microsoft.Extensions.DependencyInjection;

namespace DJualan.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// // DATA ACCESS - How to get/store data
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDataLayer(this IServiceCollection services)
        {
            // Register generic repository
            services.AddScoped(typeof(IRepository<,>), typeof(GenericRepository<,>));

            // Register specific repositories
            services.AddScoped<IProductRepository, ProductRepository>();

            // Add more repositories as you create them
            // services.AddScoped<IUserRepository, UserRepository>();
            // services.AddScoped<IOrderRepository, OrderRepository>();
            // services.AddScoped<ICategoryRepository, CategoryRepository>();

            return services;
        }
    }
}
