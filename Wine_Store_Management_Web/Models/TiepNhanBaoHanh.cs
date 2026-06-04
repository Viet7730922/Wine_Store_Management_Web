using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[Table("TIEPNHAN_BAOHANH")]
public partial class TiepnhanBaohanh
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string SoBienBan { get; set; } = null!;

    public DateOnly NgayTiepNhan { get; set; }

    [StringLength(100)]
    public string HoTenKhachHang { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string SoDienThoai { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    [StringLength(16)]
    [Unicode(false)]
    public string SoSeri { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string MaHoaDon { get; set; } = null!;

    public string TinhTrangHuHong { get; set; } = null!;

    public string? PhuKienKemTheo { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string ThuNganTiepNhan { get; set; } = null!;

    [ForeignKey("MaHoaDon")]
    [InverseProperty("TiepnhanBaohanhs")]
    public virtual Hoadon MaHoaDonNavigation { get; set; } = null!;

    [ForeignKey("MaSanPham")]
    [InverseProperty("TiepnhanBaohanhs")]
    public virtual Sanpham MaSanPhamNavigation { get; set; } = null!;

    [ForeignKey("ThuNganTiepNhan")]
    [InverseProperty("TiepnhanBaohanhs")]
    public virtual Nhanvien ThuNganTiepNhanNavigation { get; set; } = null!;
}
