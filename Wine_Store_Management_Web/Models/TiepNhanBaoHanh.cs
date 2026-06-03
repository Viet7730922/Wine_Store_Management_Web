using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{
    // 7. BẢNG BIÊN BẢN TIẾP NHẬN BẢO HÀNH
    public class TiepNhanBaoHanh
    {
        [Key]
        [StringLength(20)]
        public string SoBienBan { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayTiepNhan { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTenKhachHang { get; set; } = null!;

        [Required]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string MaSanPham { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string SoSeri { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string MaHoaDon { get; set; } = null!;

        [Required]
        public string TinhTrangHuHong { get; set; } = null!;

        public string? PhuKienKemTheo { get; set; }

        [Required]
        [StringLength(20)]
        public string ThuNganTiepNhan { get; set; } = null!;

        [ForeignKey("MaSanPham")]
        public virtual SanPham? SanPham { get; set; }

        [ForeignKey("MaHoaDon")]
        public virtual HoaDon? HoaDon { get; set; }

        [ForeignKey("ThuNganTiepNhan")]
        public virtual NhanVien? NhanVienThuNgan { get; set; }
    }
}
