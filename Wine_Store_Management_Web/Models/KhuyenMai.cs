using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{
    // 4. BẢNG CHƯƠNG TRÌNH KHUYẾN MÃI
    public class KhuyenMai
    {
        [Key]
        [StringLength(20)]
        public string MaChuongTrinh { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string TenChuongTrinh { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string HinhThucApDung { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string MucGiamGia { get; set; } = null!;

        public string? DieuKienApDung { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime TuNgay { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DenNgay { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayDuyet { get; set; }

        [Required]
        [StringLength(20)]
        public string NguoiPheDuyet { get; set; } = null!;

        [ForeignKey("NguoiPheDuyet")]
        public virtual NhanVien? NhanVienPheDuyet { get; set; }
    }
}
