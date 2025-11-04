using DJualan.APIServer.Controllers;
using DJualan.Core.DTOs.Product;
using DJualan.Core.Models;
using DJualan.Service.Interfaces;
using DJualan.Service.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DJualan.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductController _controller;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductController(_mockService.Object);
        }

        [Fact]
        public async Task Patch_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var productId = 1;
            var patchDoc = new JsonPatchDocument<ProductPatchRequest>();
            patchDoc.Replace(p => p.Name, "Updated Name");

            var updatedProduct = new Product
            {
                Id = productId,
                Name = "Updated Name",
                Description = "Test Description",
                Price = 100.0m,
                Stock = 10
            };

            _mockService.Setup(s => s.PatchAsync(productId, patchDoc))
                       .ReturnsAsync(updatedProduct);

            // Act
            var result = await _controller.Patch(productId, patchDoc);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal("Updated Name", returnedProduct.Name);

            _mockService.Verify(s => s.PatchAsync(productId, patchDoc), Times.Once);
        }

        [Fact]
        public async Task Patch_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            var productId = 999;
            var patchDoc = new JsonPatchDocument<ProductPatchRequest>();
            patchDoc.Replace(p => p.Name, "Updated Name");

            _mockService.Setup(s => s.PatchAsync(productId, patchDoc))
                       .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.Patch(productId, patchDoc);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockService.Verify(s => s.PatchAsync(productId, patchDoc), Times.Once);
        }

        [Fact]
        public async Task Patch_NullPatchDocument_ReturnsBadRequest()
        {
            // Arrange
            var productId = 1;

            // Act
            var result = await _controller.Patch(productId, null);

            // Assert
            // Updated to expect BadRequestObjectResult instead of BadRequestResult
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
            _mockService.Verify(s => s.PatchAsync(It.IsAny<int>(), It.IsAny<JsonPatchDocument<ProductPatchRequest>>()), Times.Never);
        }

        [Fact]
        public async Task Patch_EmptyPatchDocument_ReturnsBadRequest()
        {
            // Arrange
            var productId = 1;
            var patchDoc = new JsonPatchDocument<ProductPatchRequest>(); // Empty patch

            // Act
            var result = await _controller.Patch(productId, patchDoc);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
            _mockService.Verify(s => s.PatchAsync(It.IsAny<int>(), It.IsAny<JsonPatchDocument<ProductPatchRequest>>()), Times.Never);
        }
    }
}
