using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[Table("BIENNHAN_GIAODICH")]
public partial class BiennhanGiaodich
{
    [Key]
    public int MaBienNhan { get; set; }

    public DateOnly NgayIn { get; set; }

    public TimeOnly GioIn { get; set; }

    [StringLength(100)]
    public string LoaiBienNhan { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaDoiChieuHeThong { get; set; } = null!;

    [StringLength(100)]
    public string TenKhachHang { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string SoDienThoai { get; set; } = null!;

    public string ChiTietSanPham { get; set; } = null!;

    [Column(TypeName = "decimal(15, 2)")]
    public decimal? TongGiaTriDoiChieu { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string ThuNganXuatPhieu { get; set; } = null!;

    [ForeignKey("ThuNganXuatPhieu")]
    [InverseProperty("BiennhanGiaodiches")]
    public virtual Nhanvien ThuNganXuatPhieuNavigation { get; set; } = null!;
}
