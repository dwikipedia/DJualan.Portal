using DJualan.Core.DTOs.Product;
using DJualan.Core.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace DJualan.Service.Interfaces
{
    public interface IProductService
    {
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(ProductCreateRequest request);
        Task<Product?> UpdateAsync(int id, ProductUpdateRequest request);
        Task<Product?> PatchAsync(int id, JsonPatchDocument<ProductPatchRequest> patchDoc);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        //Task<IEnumerable<Product>> GetPagedAsync(int pageNumber, int pageSize);
        //Task<int> GetCountAsync();

        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task UpdateStockAsync(int productId, int newStock);
        Task<IEnumerable<string>> GetCategoriesAsync();
    }
}
