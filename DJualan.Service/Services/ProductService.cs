using AutoMapper;
using DJualan.Core.DTOs.Product;
using DJualan.Core.Interfaces;
using DJualan.Core.Models;

namespace DJualan.Service.Services
{
    public class ProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ProductResponse> CreateAsync(ProductCreateRequest request)
        {
            var product = _mapper.Map<Product>(request);
            var created = await _repository.AddAsync(product);
            return _mapper.Map<ProductResponse>(created);
        }

        public async Task<Product?> UpdateAsync(int id, Product product)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.ImageUrl = product.ImageUrl;
            existing.UpdatedAt = DateTime.UtcNow;

            return await _repository.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
