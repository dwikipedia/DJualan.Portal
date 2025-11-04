using DJualan.Core.DTOs.Product;
using DJualan.Core.Models;
using DJualan.Service.Interfaces;
using DJualan.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace DJualan.APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _service.GetByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateRequest product)
        {
            var updated = await _service.UpdateAsync(id, product);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }

        /// <summary>
        /// Partially update a product using JSON Patch format
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="patchDoc">JSON Patch operations array</param>
        /// <remarks>
        /// Sample request:
        /// 
        /// PATCH /api/Product/1
        /// [
        ///   {
        ///     "op": "replace",
        ///     "path": "/name",
        ///     "value": "New Product Name"
        ///   },
        ///   {
        ///     "op": "replace", 
        ///     "path": "/price",
        ///     "value": 99.99
        ///   }
        /// ]
        /// </remarks>
        /// <response code="200">Returns the updated product</response>
        /// <response code="400">If the patch document is invalid</response>
        /// <response code="404">If the product is not found</response>
        [HttpPatch("{id}")]
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<ProductPatchRequest> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest(new { message = "Patch document is required" });

            if (patchDoc.Operations.Count == 0)
                return BadRequest(new { message = "No patch operations provided" });

            try
            {
                var patched = await _service.PatchAsync(id, patchDoc);
                return patched == null ? NotFound() : Ok(patched);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
