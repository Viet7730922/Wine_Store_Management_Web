using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{
    // 1. BẢNG NHÂN VIÊN
    public class NhanVien
    {
        [Key]
        [StringLength(20)]
        public string MaNhanVien { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [Required]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string MatKhau { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string VaiTro { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayLap { get; set; }

        [StringLength(20)]
        public string? NguoiThucHien { get; set; }

        [ForeignKey("NguoiThucHien")]
        public virtual NhanVien? NhanVienThucHien { get; set; }
    }
}
