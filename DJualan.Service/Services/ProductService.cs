using AutoMapper;
using DJualan.Core.DTOs.Product;
using DJualan.Core.Interfaces;
using DJualan.Core.Models;
using DJualan.Service.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace DJualan.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        
        public ProductService(
            IProductRepository productRepository,
            IMapper mapper,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // CRUD Methods
        public async Task<Product?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting product by ID: {ProductId}", id);
                return await _productRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<Product> CreateAsync(ProductCreateRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new product: {ProductName}", request.Name);

                // Validate business rules
                if (await _productRepository.ProductExistsAsync(request.Name))
                {
                    throw new InvalidOperationException($"Product with name '{request.Name}' already exists.");
                }

                if (request.Price <= 0)
                {
                    throw new ArgumentException("Product price must be greater than zero.");
                }

                // Map to entity
                var product = _mapper.Map<Product>(request);

                // Create in repository
                var createdProduct = await _productRepository.CreateAsync(product);

                _logger.LogInformation("Successfully created product: {ProductName} with ID: {ProductId}",
                    createdProduct.Name, createdProduct.Id);

                return createdProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {ProductName}", request.Name);
                throw;
            }
        }

        public async Task<Product?> UpdateAsync(int id, ProductUpdateRequest request)
        {
            try
            {
                _logger.LogInformation("Updating product ID: {ProductId}", id);

                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for update", id);
                    return null;
                }

                // Check if name is being changed and if new name already exists
                if (existingProduct.Name != request.Name &&
                    await _productRepository.ProductExistsAsync(request.Name))
                {
                    throw new InvalidOperationException($"Product with name '{request.Name}' already exists.");
                }

                // Map updates to existing entity
                _mapper.Map(request, existingProduct);
                existingProduct.UpdatedAt = DateTime.UtcNow;

                var updatedProduct = await _productRepository.UpdateAsync(existingProduct);

                _logger.LogInformation("Successfully updated product ID: {ProductId}", id);
                return updatedProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<Product?> PatchAsync(int id, JsonPatchDocument<ProductPatchRequest> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogWarning("Patch document is null for product ID: {ProductId}", id);
                throw new ArgumentNullException(nameof(patchDoc));
            }

            try
            {
                _logger.LogInformation("Patching product ID: {ProductId}", id);

                // Simple inline logging - good enough for most cases
                _logger.LogDebug("Patch operations count: {OperationCount}", patchDoc.Operations.Count);

                // Only serialize for debug level to avoid performance hit in production
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    var operations = patchDoc.Operations.Select(op => $"{op.op} {op.path} = {op.value}");
                    _logger.LogDebug("Patch operations: {Operations}", string.Join("; ", operations));
                }

                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for patch", id);
                    return null;
                }

                // Map entity to patch DTO
                var patchDto = _mapper.Map<ProductPatchRequest>(existingProduct);

                // Apply patch operations to DTO
                patchDoc.ApplyTo(patchDto);

                // Map patched DTO back to entity (only non-null values will be mapped)
                _mapper.Map(patchDto, existingProduct);

                // Update timestamp
                existingProduct.UpdatedAt = DateTime.UtcNow;

                // Validate business rules
                if (patchDto.Name != null && existingProduct.Name != patchDto.Name)
                {
                    if (await _productRepository.ProductExistsAsync(patchDto.Name))
                    {
                        throw new InvalidOperationException($"Product with name '{patchDto.Name}' already exists.");
                    }
                }

                var patchedProduct = await _productRepository.UpdateAsync(existingProduct);

                _logger.LogInformation("Successfully patched product ID: {ProductId}", id);
                return patchedProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error patching product ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting product ID: {ProductId}", id);

                var result = await _productRepository.DeleteAsync(id);

                if (result)
                {
                    _logger.LogInformation("Successfully deleted product ID: {ProductId}", id);
                }
                else
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                _logger.LogDebug("Getting all products");
                return await _productRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                throw;
            }
        }

        //public async Task<IEnumerable<Product>> GetPagedAsync(int pageNumber, int pageSize)
        //{
        //    try
        //    {
        //        _logger.LogDebug("Getting products page {PageNumber} with size {PageSize}", pageNumber, pageSize);

        //        if (pageNumber < 1) pageNumber = 1;
        //        if (pageSize < 1) pageSize = 10;
        //        if (pageSize > 100) pageSize = 100; // Prevent excessive data retrieval

        //        return await _productRepository.GetPagedAsync(pageNumber, pageSize);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting paged products");
        //        throw;
        //    }
        //}

        //public async Task<int> GetCountAsync()
        //{
        //    try
        //    {
        //        _logger.LogDebug("Getting product count");
        //        return await _productRepository.GetCountAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting product count");
        //        throw;
        //    }
        //}

        // Product-specific service methods
        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            try
            {
                _logger.LogDebug("Getting products by category: {Category}", category);

                if (string.IsNullOrWhiteSpace(category))
                    throw new ArgumentException("Category cannot be empty");

                return await _productRepository.GetByCategoryAsync(category);
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
                _logger.LogDebug("Getting active products");
                return await _productRepository.GetActiveProductsAsync();
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
                return await _productRepository.SearchProductsAsync(searchTerm);
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

                if (minPrice < 0 || maxPrice < 0)
                    throw new ArgumentException("Price cannot be negative");

                if (minPrice > maxPrice)
                    throw new ArgumentException("Min price cannot be greater than max price");

                return await _productRepository.GetProductsByPriceRangeAsync(minPrice, maxPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
                throw;
            }
        }

        public async Task UpdateStockAsync(int productId, int newStock)
        {
            try
            {
                _logger.LogInformation("Updating stock for product ID: {ProductId} to {NewStock}", productId, newStock);

                if (newStock < 0)
                    throw new ArgumentException("Stock cannot be negative");

                await _productRepository.UpdateStockAsync(productId, newStock);
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
                _logger.LogDebug("Getting all categories");
                return await _productRepository.GetCategoriesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                throw;
            }
        }

        public async Task<bool> ProductExistsAsync(string productName)
        {
            try
            {
                _logger.LogDebug("Checking if product exists: {ProductName}", productName);
                return await _productRepository.ProductExistsAsync(productName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if product exists: {ProductName}", productName);
                throw;
            }
        }
    }
}
