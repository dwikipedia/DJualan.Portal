using AutoMapper;
using DJualan.Service.Mappings;
using FluentAssertions;
using Xunit;
using DJualan.Core.DTOs.Product;
using DJualan.Core.Models;

namespace DJualan.Tests.AutoMapper
{
    public class AutoMapperProfileTests
    {
        private readonly IConfigurationProvider _config;
        private readonly IMapper _mapper;

        public AutoMapperProfileTests()
        {
            _config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });

            _mapper = _config.CreateMapper();
        }

        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            _config.AssertConfigurationIsValid();
        }

        [Fact]
        public void AutoMapper_Should_Map_Properties_Correctly()
        {
            var source = new Product
            {
                Id = 1,
                Name = "Laptop",
                Description = "Gaming Laptop",
                Price = 15000000,
                Stock = 5,
                ImageUrl = "https://example.com/laptop.jpg",
                Category = "Electronics",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = _mapper.Map<ProductResponse>(source);

            result.Name.Should().Be("Laptop");
            result.PriceFormatted.Should().Contain("Rp");
        }
    }
}
