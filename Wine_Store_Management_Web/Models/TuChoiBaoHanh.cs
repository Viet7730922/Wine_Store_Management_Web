using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[Table("TUCHOI_BAOHANH")]
public partial class TuchoiBaohanh
{
    [Key]
    public int MaPhieuTuChoi { get; set; }

    public DateOnly NgayLapPhieu { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string MaKhachHang { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    [StringLength(16)]
    [Unicode(false)]
    public string SoSeri { get; set; } = null!;

    [StringLength(255)]
    public string LyDoTuChoi { get; set; } = null!;

    public string? BangChungDinhKem { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string ThuNganXacNhan { get; set; } = null!;

    [ForeignKey("MaKhachHang")]
    [InverseProperty("TuchoiBaohanhs")]
    public virtual Khachhang MaKhachHangNavigation { get; set; } = null!;

    [ForeignKey("MaSanPham")]
    [InverseProperty("TuchoiBaohanhs")]
    public virtual Sanpham MaSanPhamNavigation { get; set; } = null!;

    [ForeignKey("ThuNganXacNhan")]
    [InverseProperty("TuchoiBaohanhs")]
    public virtual Nhanvien ThuNganXacNhanNavigation { get; set; } = null!;
}
