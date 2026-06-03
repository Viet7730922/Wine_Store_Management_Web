using System.ComponentModel.DataAnnotations;

namespace Wine_Store_Management_Web.Models
{
    // 3. BẢNG KHÁCH HÀNG THÂN THIẾT
    public class KhachHang
    {
        [Key]
        [StringLength(20)]
        public string MaKhachHang { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = null!;

        [Required]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = null!;

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayDangKyThe { get; set; }

        [Required]
        [StringLength(50)]
        public string HangThanhVien { get; set; } = null!;

        public int DiemTichLuy { get; set; } = 0;
    }

}
