using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DJualan.Core.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        // 🏷️ Nama produk
        [Required(ErrorMessage = "Nama produk wajib diisi")]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // 📝 Deskripsi singkat produk
        [MaxLength(1000)]
        public string? Description { get; set; }

        // 💲 Harga produk
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Harga tidak boleh negatif")]
        public decimal Price { get; set; }

        // 🔢 Jumlah stok yang tersedia
        [Range(0, int.MaxValue, ErrorMessage = "Stok tidak boleh negatif")]
        public int Stock { get; set; }

        // 🖼️ URL gambar produk
        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        // 🏷️ Kategori produk (opsional)
        [MaxLength(100)]
        public string? Category { get; set; }

        // 🌟 Apakah produk aktif / tampil di etalase
        public bool IsActive { get; set; } = true;

        // 📅 Waktu pembuatan dan update
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
