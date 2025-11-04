using DJualan.Core.Models;

namespace DJualan.Service.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task UpdateStockAsync(int productId, int newStock);
        Task<IEnumerable<string>> GetCategoriesAsync();
    }
}
