using DJualan.Core.Interfaces;
using DJualan.Data.Repositories;
using DJualan.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DJualan.Service.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<AuthService>();
            services.AddScoped<ProductService>();

            return services;
        }
    }
}
