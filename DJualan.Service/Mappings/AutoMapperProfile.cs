using AutoMapper;
using DJualan.Core.DTOs.Product;
using DJualan.Core.Models;

namespace DJualan.Service.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Product mappings
            CreateMap<ProductCreateRequest, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.PriceFormatted, opt =>
                    opt.MapFrom(src =>
                        string.Format(new System.Globalization.CultureInfo("id-ID"), "{0:C}", src.Price)));
        }
    }
}
