using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[Table("PHIEU_BAOHANH_KHO")]
public partial class PhieuBaohanhKho
{
    [Key]
    [Column("MaPhieuBH")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaPhieuBh { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string MaKhachHang { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    [StringLength(16)]
    [Unicode(false)]
    public string SoSeri { get; set; } = null!;

    public string NoiDungLoiGhiNhan { get; set; } = null!;

    public DateOnly NgayTiepNhanHeThong { get; set; }

    public int ThoiGianXuLyDuKien { get; set; }

    public DateOnly NgayHenTra { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string NhanVienThucHien { get; set; } = null!;

    [ForeignKey("MaKhachHang")]
    [InverseProperty("PhieuBaohanhKhos")]
    public virtual Khachhang MaKhachHangNavigation { get; set; } = null!;

    [ForeignKey("MaSanPham")]
    [InverseProperty("PhieuBaohanhKhos")]
    public virtual Sanpham MaSanPhamNavigation { get; set; } = null!;

    [ForeignKey("NhanVienThucHien")]
    [InverseProperty("PhieuBaohanhKhos")]
    public virtual Nhanvien NhanVienThucHienNavigation { get; set; } = null!;
}
