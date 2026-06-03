using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{
    // 5b. CHI TIẾT PHIẾU NHẬP KHO (Khóa phức hợp)
    public class ChiTietPhieuNhap
    {
        [StringLength(20)]
        public string MaPhieuNhap { get; set; } = null!;

        [StringLength(20)]
        public string MaSanPham { get; set; } = null!;

        [Required]
        public int SoLuong { get; set; }

        [Required]
        [Column(TypeName = "decimal(15, 2)")]
        public decimal DonGia { get; set; }

        [ForeignKey("MaPhieuNhap")]
        public virtual PhieuNhap? PhieuNhap { get; set; }

        [ForeignKey("MaSanPham")]
        public virtual SanPham? SanPham { get; set; }
    }
}
