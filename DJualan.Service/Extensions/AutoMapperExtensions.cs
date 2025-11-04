using DJualan.Service.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace DJualan.Service.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ProductProfile));
            return services;
        }
    }
}
