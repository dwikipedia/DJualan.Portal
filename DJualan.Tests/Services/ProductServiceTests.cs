using AutoMapper;
using DJualan.Core.DTOs.Product;
using DJualan.Core.Interfaces;
using DJualan.Core.Models;
using DJualan.Service.Mappings;
using DJualan.Service.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DJualan.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _mockLogger = new Mock<ILogger<ProductService>>();

            // Configure AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductProfile>();
            });
            _mapper = config.CreateMapper();

            _productService = new ProductService(_mockRepo.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task PatchAsync_ValidPatch_ReturnsUpdatedProduct()
        {
            // Arrange
            var productId = 1;
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Old Name",
                Description = "Old Description",
                Price = 100.0m,
                Stock = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var patchDoc = new JsonPatchDocument<ProductPatchRequest>();
            patchDoc.Replace(p => p.Name, "New Name");
            patchDoc.Replace(p => p.Price, 150.0m);

            _mockRepo.Setup(r => r.GetByIdAsync(productId))
                    .ReturnsAsync(existingProduct);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
                    .ReturnsAsync((Product p) => p);

            // Act
            var result = await _productService.PatchAsync(productId, patchDoc);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Name", result.Name);
            Assert.Equal(150.0m, result.Price);
            Assert.Equal("Old Description", result.Description); // Should remain unchanged
            Assert.True(result.UpdatedAt > existingProduct.CreatedAt);

            _mockRepo.Verify(r => r.GetByIdAsync(productId), Times.Once);
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task PatchAsync_ProductNotFound_ReturnsNull()
        {
            // Arrange
            var productId = 999;
            var patchDoc = new JsonPatchDocument<ProductPatchRequest>();
            patchDoc.Replace(p => p.Name, "New Name");

            _mockRepo.Setup(r => r.GetByIdAsync(productId))
                    .ReturnsAsync((Product)null);

            // Act
            var result = await _productService.PatchAsync(productId, patchDoc);

            // Assert
            Assert.Null(result);
            _mockRepo.Verify(r => r.GetByIdAsync(productId), Times.Once);
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task PatchAsync_NullPatchDocument_ThrowsArgumentNullException()
        {
            // Arrange
            var productId = 1;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _productService.PatchAsync(productId, null));
        }

        [Fact]
        public async Task PatchAsync_PartialUpdate_OnlyUpdatesProvidedFields()
        {
            // Arrange
            var productId = 1;
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Original Name",
                Description = "Original Description",
                Price = 100.0m,
                Stock = 10,
                IsActive = true
            };

            var patchDoc = new JsonPatchDocument<ProductPatchRequest>();
            patchDoc.Replace(p => p.Description, "Updated Description");
            // Only update description, other fields should remain unchanged

            _mockRepo.Setup(r => r.GetByIdAsync(productId))
                    .ReturnsAsync(existingProduct);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
                    .ReturnsAsync((Product p) => p);

            // Act
            var result = await _productService.PatchAsync(productId, patchDoc);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Description", result.Description);
            Assert.Equal("Original Name", result.Name); // Unchanged
            Assert.Equal(100.0m, result.Price); // Unchanged
            Assert.Equal(10, result.Stock); // Unchanged
        }

        [Fact]
        public async Task PatchAsync_UpdateStock_SuccessfullyUpdates()
        {
            // Arrange
            var productId = 1;
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Test Product",
                Stock = 5,
                IsActive = true
            };

            var patchDoc = new JsonPatchDocument<ProductPatchRequest>();
            patchDoc.Replace(p => p.Stock, 25);

            _mockRepo.Setup(r => r.GetByIdAsync(productId))
                    .ReturnsAsync(existingProduct);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
                    .ReturnsAsync((Product p) => p);

            // Act
            var result = await _productService.PatchAsync(productId, patchDoc);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(25, result.Stock);
        }

        [Fact]
        public async Task PatchAsync_DeactivateProduct_SuccessfullyUpdates()
        {
            // Arrange
            var productId = 1;
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Test Product",
                IsActive = true
            };

            var patchDoc = new JsonPatchDocument<ProductPatchRequest>();
            patchDoc.Replace(p => p.IsActive, false);

            _mockRepo.Setup(r => r.GetByIdAsync(productId))
                    .ReturnsAsync(existingProduct);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
                    .ReturnsAsync((Product p) => p);

            // Act
            var result = await _productService.PatchAsync(productId, patchDoc);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsActive);
        }

        [Fact]
        public async Task PatchAsync_MultipleOperations_AppliesAllChanges()
        {
            // Arrange
            var productId = 1;
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Old Name",
                Description = "Old Description",
                Price = 100.0m,
                Stock = 10
            };

            var patchDoc = new JsonPatchDocument<ProductPatchRequest>();
            patchDoc.Replace(p => p.Name, "New Name");
            patchDoc.Replace(p => p.Description, "New Description");
            patchDoc.Replace(p => p.Price, 200.0m);
            patchDoc.Replace(p => p.Stock, 20);

            _mockRepo.Setup(r => r.GetByIdAsync(productId))
                    .ReturnsAsync(existingProduct);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
                    .ReturnsAsync((Product p) => p);

            // Act
            var result = await _productService.PatchAsync(productId, patchDoc);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Name", result.Name);
            Assert.Equal("New Description", result.Description);
            Assert.Equal(200.0m, result.Price);
            Assert.Equal(20, result.Stock);
        }
    }
}
