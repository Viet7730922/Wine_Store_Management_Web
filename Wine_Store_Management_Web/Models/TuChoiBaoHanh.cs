using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{
    // 8. BẢNG PHIẾU TỪ CHỐI BẢO HÀNH
    public class TuChoiBaoHanh
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaPhieuTuChoi { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayLapPhieu { get; set; }

        [Required]
        [StringLength(20)]
        public string MaKhachHang { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string MaSanPham { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string SoSeri { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string LyDoTuChoi { get; set; } = null!;

        public string? BangChungDinhKem { get; set; }

        [Required]
        [StringLength(20)]
        public string ThuNganXacNhan { get; set; } = null!;

        [ForeignKey("MaKhachHang")]
        public virtual KhachHang? KhachHang { get; set; }

        [ForeignKey("MaSanPham")]
        public virtual SanPham? SanPham { get; set; }

        [ForeignKey("ThuNganXacNhan")]
        public virtual NhanVien? NhanVienThuNgan { get; set; }
    }
}
