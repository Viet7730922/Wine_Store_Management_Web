using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{
    // 10. BẢNG BIÊN NHẬN GIAO DỊCH
    public class BienNhanGiaoDich
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaBienNhan { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayIn { get; set; }

        [Required]
        public TimeSpan GioIn { get; set; }

        [Required]
        [StringLength(100)]
        public string LoaiBienNhan { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string MaDoiChieuHeThong { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string TenKhachHang { get; set; } = null!;

        [Required]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = null!;

        [Required]
        public string ChiTietSanPham { get; set; } = null!;

        [Column(TypeName = "decimal(15, 2)")]
        public decimal? TongGiaTriDoiChieu { get; set; }

        [Required]
        [StringLength(20)]
        public string ThuNganXuatPhieu { get; set; } = null!;

        [ForeignKey("ThuNganXuatPhieu")]
        public virtual NhanVien? NhanVienThuNgan { get; set; }
    }

}
