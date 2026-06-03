using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{
    // 2. BẢNG SẢN PHẨM
    public class SanPham
    {
        [Key]
        [StringLength(20)]
        public string MaSanPham { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string TenSanPham { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LoaiSanPham { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string HangSanXuat { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string XuatXu { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string DonViTinh { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(15, 2)")]
        public decimal GiaNhap { get; set; }

        [Required]
        [Column(TypeName = "decimal(15, 2)")]
        public decimal GiaBanDeXuat { get; set; }

        [Required]
        public int ThoiHanBaoHanh { get; set; }

        public string? MoTa { get; set; }

        [Required]
        [StringLength(20)]
        public string NguoiLapPhieu { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayTao { get; set; }

        public int SoLuongTon { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; } = "Kích hoạt";

        [ForeignKey("NguoiLapPhieu")]
        public virtual NhanVien? NhanVienLapPhieu { get; set; }
    }

}
