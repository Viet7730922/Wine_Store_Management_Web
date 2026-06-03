using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{
    // 11. BẢNG KIỂM KÊ KHO
    public class KiemKeKho
    {
        [Key]
        [StringLength(20)]
        public string MaPhieuKiemKe { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime ThoiGianKiemKe { get; set; }

        [Required]
        [StringLength(20)]
        public string NguoiLapKiemKe { get; set; } = null!;

        [ForeignKey("NguoiLapKiemKe")]
        public virtual NhanVien? NhanVienKiemKe { get; set; }
    }
}
