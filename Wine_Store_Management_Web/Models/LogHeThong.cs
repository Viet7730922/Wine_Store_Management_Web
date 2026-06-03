using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{
    // 12. BẢNG NHẬT KÝ HOẠT ĐỘNG
    public class LogHeThong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLog { get; set; }

        [Required]
        public DateTime ThoiGian { get; set; }

        [Required]
        [StringLength(50)]
        public string TenTaiKhoan { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string ThaoTacThucHien { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string TrangThai { get; set; } = null!;
    }
}
