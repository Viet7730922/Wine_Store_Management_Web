using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{
    // 6b. CHI TIẾT HÓA ĐƠN BÁN HÀNG (Khóa phức hợp)
    public class ChiTietHoaDon
    {
        [StringLength(20)]
        public string SoHoaDon { get; set; } = null!;

        [StringLength(20)]
        public string MaSanPham { get; set; } = null!;

        [Required]
        public int SoLuong { get; set; }

        [Required]
        [Column(TypeName = "decimal(15, 2)")]
        public decimal DonGia { get; set; }

        [ForeignKey("SoHoaDon")]
        public virtual HoaDon? HoaDon { get; set; }

        [ForeignKey("MaSanPham")]
        public virtual SanPham? SanPham { get; set; }
    }
}
