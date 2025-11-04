using DJualan.Core.Interfaces.Base;
using DJualan.Core.Models;

namespace DJualan.Core.Interfaces
{
    //public interface IProductRepository : IRepository<Product, int>
    //{
    //    Task<IEnumerable<Product>> GetAllAsync();
    //    Task<Product?> GetByIdAsync(int id);
    //    Task<Product> AddAsync(Product product);
    //    Task<Product> UpdateAsync(Product product);
    //    Task<bool> DeleteAsync(int id);
    //}

    public interface IProductRepository : IRepository<Product, int>
    {
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<bool> ProductExistsAsync(string productName);
        Task UpdateStockAsync(int productId, int newStock);
        Task<IEnumerable<string>> GetCategoriesAsync();
    }
}
