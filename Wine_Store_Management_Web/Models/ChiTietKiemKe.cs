using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{
    // 11b. CHI TIẾT KIỂM KÊ KHO (Khóa phức hợp)
    public class ChiTietKiemKe
    {
        [StringLength(20)]
        public string MaPhieuKiemKe { get; set; } = null!;

        [StringLength(20)]
        public string MaSanPham { get; set; } = null!;

        [Required]
        public int TonKhoThucTe { get; set; }

        [Required]
        public int TonKhoHeThong { get; set; }

        [Required]
        public int ChenhLech { get; set; }

        [ForeignKey("MaPhieuKiemKe")]
        public virtual KiemKeKho? KiemKeKho { get; set; }

        [ForeignKey("MaSanPham")]
        public virtual SanPham? SanPham { get; set; }
    }
}
