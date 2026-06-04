using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[Table("KHACHHANG")]
[Index("SoDienThoai", Name = "UQ__KHACHHAN__0389B7BD5B1357E1", IsUnique = true)]
public partial class Khachhang
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string MaKhachHang { get; set; } = null!;

    [StringLength(100)]
    public string HoTen { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string SoDienThoai { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    public DateOnly NgayDangKyThe { get; set; }

    [StringLength(50)]
    public string HangThanhVien { get; set; } = null!;

    public int? DiemTichLuy { get; set; }

    [InverseProperty("MaKhachHangNavigation")]
    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    [InverseProperty("MaKhachHangNavigation")]
    public virtual ICollection<PhieuBaohanhKho> PhieuBaohanhKhos { get; set; } = new List<PhieuBaohanhKho>();

    [InverseProperty("MaKhachHangNavigation")]
    public virtual ICollection<TuchoiBaohanh> TuchoiBaohanhs { get; set; } = new List<TuchoiBaohanh>();
}
