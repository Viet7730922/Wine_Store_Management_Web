using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{
    // 9. BẢNG PHIẾU BẢO HÀNH HỆ THỐNG KẾT XUẤT
    public class PhieuBaoHanhKho
    {
        [Key]
        [StringLength(50)]
        public string MaPhieuBH { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string MaKhachHang { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string MaSanPham { get; set; } = null!;

        [Required]
        [StringLength(16)]
        public string SoSeri { get; set; } = null!;

        [Required]
        public string NoiDungLoiGhiNhan { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayTiepNhanHeThong { get; set; }

        [Required]
        public int ThoiGianXuLyDuKien { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayHenTra { get; set; }

        [Required]
        [StringLength(20)]
        public string NhanVienThucHien { get; set; } = null!;

        [ForeignKey("MaKhachHang")]
        public virtual KhachHang? KhachHang { get; set; }

        [ForeignKey("MaSanPham")]
        public virtual SanPham? SanPham { get; set; }

        [ForeignKey("NhanVienThucHien")]
        public virtual NhanVien? NhanVienThuNgan { get; set; }
    }
}
