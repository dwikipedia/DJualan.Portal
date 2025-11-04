using System.ComponentModel.DataAnnotations;

namespace DJualan.Core.DTOs.Product
{
    public class ProductPatchRequest
    {
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string? Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int? Stock { get; set; }

        [Url(ErrorMessage = "ImageUrl must be a valid URL")]
        [StringLength(500, ErrorMessage = "ImageUrl cannot exceed 500 characters")]
        public string? ImageUrl { get; set; }

        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string? Category { get; set; }

        public bool? IsActive { get; set; }
    }
}
