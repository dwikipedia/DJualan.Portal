using DJualan.Core.Entities.Base;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DJualan.Core.Models
{
    public class Product : BaseEntity
    {
        [Required(ErrorMessage = "Nama produk wajib diisi")]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Harga tidak boleh negatif")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stok tidak boleh negatif")]
        public int Stock { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [SwaggerSchema(ReadOnly = true, Description = "Formatted price in Rupiah, e.g. Rp10.000,00")]
        public string PriceInRp => string.Format(new System.Globalization.CultureInfo("id-ID"), "{0:C}", Price);
    }
}
