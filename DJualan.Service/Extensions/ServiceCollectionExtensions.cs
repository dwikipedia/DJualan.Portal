using DJualan.Service.Interfaces;
using DJualan.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DJualan.Service.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// // BUSINESS LOGIC - What to do with data and Why
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IProductService, ProductService>();

            // Add more services as you create them
            // services.AddScoped<IUserService, UserService>();
            // services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}
