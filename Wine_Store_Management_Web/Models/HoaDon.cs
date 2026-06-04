using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[Table("HOADON")]
public partial class Hoadon
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string SoHoaDon { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MauSo { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? KyHieu { get; set; }

    public DateOnly NgayHoaDon { get; set; }

    [StringLength(150)]
    public string? DonViBanHang { get; set; }

    [StringLength(255)]
    public string? DiaChiBanHang { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? DienThoaiBanHang { get; set; }

    [StringLength(100)]
    public string HoTenNguoiMua { get; set; } = null!;

    [StringLength(255)]
    public string? DiaChiKhachHang { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? MaKhachHang { get; set; }

    [StringLength(50)]
    public string HinhThucThanhToan { get; set; } = null!;

    public string? GhiChu { get; set; }

    public string? TongTienVietBangChu { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string ThuNgan { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? MaKhuyenMai { get; set; }

    [InverseProperty("SoHoaDonNavigation")]
    public virtual ICollection<ChitietHoadon> ChitietHoadons { get; set; } = new List<ChitietHoadon>();

    [ForeignKey("MaKhachHang")]
    [InverseProperty("Hoadons")]
    public virtual Khachhang? MaKhachHangNavigation { get; set; }

    [ForeignKey("MaKhuyenMai")]
    [InverseProperty("Hoadons")]
    public virtual Khuyenmai? MaKhuyenMaiNavigation { get; set; }

    [ForeignKey("ThuNgan")]
    [InverseProperty("Hoadons")]
    public virtual Nhanvien ThuNganNavigation { get; set; } = null!;

    [InverseProperty("MaHoaDonNavigation")]
    public virtual ICollection<TiepnhanBaohanh> TiepnhanBaohanhs { get; set; } = new List<TiepnhanBaohanh>();
}
