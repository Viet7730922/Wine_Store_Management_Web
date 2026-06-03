using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{

    // 5. BẢNG PHIẾU NHẬP KHO
    public class PhieuNhap
    {
        [Key]
        [StringLength(20)]
        public string MaPhieuNhap { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayNhap { get; set; }

        [Required]
        [StringLength(150)]
        public string NhaCungCap { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string NhapTaiKho { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string NguoiNhapKho { get; set; } = null!;

        public string? SoChungTuGoc { get; set; }

        public string? TongTienVietBangChu { get; set; }

        [ForeignKey("NguoiNhapKho")]
        public virtual NhanVien? NhanVienNhapKho { get; set; }
    }

}
