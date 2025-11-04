using AutoMapper;
using DJualan.Core.DTOs.Product;
using DJualan.Core.Models;

namespace DJualan.Service.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // ProductCreateRequest → Product
            CreateMap<ProductCreateRequest, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive)); // Explicit mapping

            // ProductUpdateRequest → Product
            CreateMap<ProductUpdateRequest, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // ProductPatchRequest → Product (for PATCH operations)
            CreateMap<ProductPatchRequest, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null)); // Only map non-null source members

            // Product → ProductResponse
            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.PriceFormatted, opt =>
                    opt.MapFrom(src => FormatPrice(src.Price)));

            // Product → ProductPatchRequest (for reverse mapping in PATCH)
            CreateMap<Product, ProductPatchRequest>();

            // Product → ProductUpdateRequest (for reverse mapping if needed)
            CreateMap<Product, ProductUpdateRequest>();
        }

        private static string FormatPrice(decimal price)
        {
            return string.Format(new System.Globalization.CultureInfo("id-ID"), "{0:C}", price);
        }
    }
}
