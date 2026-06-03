using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wine_Store_Management_Web.Models
{
    // 6. BẢNG HÓA ĐƠN BÁN HÀNG
    public class HoaDon
    {
        [Key]
        [StringLength(20)]
        public string SoHoaDon { get; set; } = null!;

        [StringLength(50)]
        public string? MauSo { get; set; }

        [StringLength(50)]
        public string? KyHieu { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayHoaDon { get; set; }

        [StringLength(150)]
        public string DonViBanHang { get; set; } = "Cửa Hàng Rượu Chilliquer";

        [StringLength(255)]
        public string DiaChiBanHang { get; set; } = "138 Mai Xuân Thưởng, Nha Trang, Khánh Hòa";

        [StringLength(20)]
        public string DienThoaiBanHang { get; set; } = "0905XXXXXX";

        [Required]
        [StringLength(100)]
        public string HoTenNguoiMua { get; set; } = null!;

        [StringLength(255)]
        public string? DiaChiKhachHang { get; set; }

        [StringLength(20)]
        public string? MaKhachHang { get; set; }

        [Required]
        [StringLength(50)]
        public string HinhThucThanhToan { get; set; } = null!;

        public string? GhiChu { get; set; }

        public string? TongTienVietBangChu { get; set; }

        [Required]
        [StringLength(20)]
        public string ThuNgan { get; set; } = null!;

        [StringLength(20)]
        public string? MaKhuyenMai { get; set; }

        [ForeignKey("MaKhachHang")]
        public virtual KhachHang? KhachHang { get; set; }

        [ForeignKey("ThuNgan")]
        public virtual NhanVien? NhanVienThuNgan { get; set; }

        [ForeignKey("MaKhuyenMai")]
        public virtual KhuyenMai? KhuyenMai { get; set; }

        // Thuộc tính điều hướng trỏ tới danh sách chi tiết của hóa đơn
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
    }
}
