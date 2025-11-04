using DJualan.Core.Interfaces;
using DJualan.Core.Models;
using DJualan.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DJualan.Data.Repositories
{
    public class ProductRepository : BaseRepository<Product, int>, IProductRepository
    {
        public ProductRepository(AppDbContext context, ILogger<ProductRepository> logger)
            : base(context, logger)
        {
        }
        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            try
            {
                _logger.LogDebug("Getting products by category: {Category}", category);
                return await _dbSet
                    .Where(p => p.Category == category && p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by category: {Category}", category);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            try
            {
                _logger.LogDebug("Getting all active products");
                return await _dbSet
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active products");
                throw;
            }
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                _logger.LogDebug("Searching products with term: {SearchTerm}", searchTerm);

                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetActiveProductsAsync();

                return await _dbSet
                    .Where(p => p.IsActive &&
                               (p.Name.Contains(searchTerm) ||
                                p.Description.Contains(searchTerm) ||
                                p.Category.Contains(searchTerm)))
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            try
            {
                _logger.LogDebug("Getting products by price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
                return await _dbSet
                    .Where(p => p.IsActive && p.Price >= minPrice && p.Price <= maxPrice)
                    .OrderBy(p => p.Price)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
                throw;
            }
        }

        public async Task<bool> ProductExistsAsync(string productName)
        {
            try
            {
                _logger.LogDebug("Checking if product exists: {ProductName}", productName);
                return await _dbSet
                    .AnyAsync(p => p.Name.ToLower() == productName.ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if product exists: {ProductName}", productName);
                throw;
            }
        }

        public async Task UpdateStockAsync(int productId, int newStock)
        {
            try
            {
                _logger.LogDebug("Updating stock for product ID: {ProductId} to {NewStock}", productId, newStock);

                var product = await GetByIdAsync(productId);
                if (product == null)
                {
                    throw new ArgumentException($"Product with ID {productId} not found.");
                }

                product.Stock = newStock;
                product.UpdatedAt = DateTime.UtcNow;

                await UpdateAsync(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product ID: {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            try
            {
                _logger.LogDebug("Getting all product categories");
                return await _dbSet
                    .Where(p => p.IsActive).Select(p => p.Category).Distinct().OrderBy(c => c).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product categories");
                throw;
            }
        }

        // Override GetAllAsync to include custom ordering/filtering
        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                _logger.LogDebug("Getting all products with custom ordering");
                return await _dbSet
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                throw;
            }
        }
    }
}
