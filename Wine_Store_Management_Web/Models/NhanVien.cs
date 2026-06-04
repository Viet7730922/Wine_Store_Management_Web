using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[Table("NHANVIEN")]
[Index("TenDangNhap", Name = "UQ__NHANVIEN__55F68FC0395849AA", IsUnique = true)]
public partial class Nhanvien
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [StringLength(100)]
    public string HoTen { get; set; } = null!;

    public DateOnly NgaySinh { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string SoDienThoai { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string TenDangNhap { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string MatKhau { get; set; } = null!;

    [StringLength(50)]
    public string VaiTro { get; set; } = null!;

    [StringLength(50)]
    public string TrangThai { get; set; } = null!;

    public DateOnly NgayLap { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? NguoiThucHien { get; set; }

    [InverseProperty("ThuNganXuatPhieuNavigation")]
    public virtual ICollection<BiennhanGiaodich> BiennhanGiaodiches { get; set; } = new List<BiennhanGiaodich>();

    [InverseProperty("ThuNganNavigation")]
    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    [InverseProperty("NguoiThucHienNavigation")]
    public virtual ICollection<Nhanvien> InverseNguoiThucHienNavigation { get; set; } = new List<Nhanvien>();

    [InverseProperty("NguoiPheDuyetNavigation")]
    public virtual ICollection<Khuyenmai> Khuyenmais { get; set; } = new List<Khuyenmai>();

    [InverseProperty("NguoiLapKiemKeNavigation")]
    public virtual ICollection<Kiemkekho> Kiemkekhos { get; set; } = new List<Kiemkekho>();

    [ForeignKey("NguoiThucHien")]
    [InverseProperty("InverseNguoiThucHienNavigation")]
    public virtual Nhanvien? NguoiThucHienNavigation { get; set; }

    [InverseProperty("NhanVienThucHienNavigation")]
    public virtual ICollection<PhieuBaohanhKho> PhieuBaohanhKhos { get; set; } = new List<PhieuBaohanhKho>();

    [InverseProperty("NguoiNhapKhoNavigation")]
    public virtual ICollection<Phieunhap> Phieunhaps { get; set; } = new List<Phieunhap>();

    [InverseProperty("NguoiLapPhieuNavigation")]
    public virtual ICollection<Sanpham> Sanphams { get; set; } = new List<Sanpham>();

    [InverseProperty("ThuNganTiepNhanNavigation")]
    public virtual ICollection<TiepnhanBaohanh> TiepnhanBaohanhs { get; set; } = new List<TiepnhanBaohanh>();

    [InverseProperty("ThuNganXacNhanNavigation")]
    public virtual ICollection<TuchoiBaohanh> TuchoiBaohanhs { get; set; } = new List<TuchoiBaohanh>();
}
