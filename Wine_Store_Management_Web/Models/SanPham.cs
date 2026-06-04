using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[Table("SANPHAM")]
public partial class Sanpham
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    [StringLength(16)]
    [Unicode(false)]
    public string? SoSeri { get; set; }

    [StringLength(150)]
    public string TenSanPham { get; set; } = null!;

    [StringLength(100)]
    public string LoaiSanPham { get; set; } = null!;

    [StringLength(100)]
    public string HangSanXuat { get; set; } = null!;

    [StringLength(100)]
    public string XuatXu { get; set; } = null!;

    [StringLength(50)]
    public string DonViTinh { get; set; } = null!;

    [Column(TypeName = "decimal(15, 2)")]
    public decimal GiaNhap { get; set; }

    [Column(TypeName = "decimal(15, 2)")]
    public decimal GiaBanDeXuat { get; set; }

    public int ThoiHanBaoHanh { get; set; }

    public string? MoTa { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string NguoiLapPhieu { get; set; } = null!;

    public DateOnly NgayTao { get; set; }

    public int? SoLuongTon { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<ChitietHoadon> ChitietHoadons { get; set; } = new List<ChitietHoadon>();

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<ChitietKiemke> ChitietKiemkes { get; set; } = new List<ChitietKiemke>();

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<ChitietPhieunhap> ChitietPhieunhaps { get; set; } = new List<ChitietPhieunhap>();

    [ForeignKey("NguoiLapPhieu")]
    [InverseProperty("Sanphams")]
    public virtual Nhanvien NguoiLapPhieuNavigation { get; set; } = null!;

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<PhieuBaohanhKho> PhieuBaohanhKhos { get; set; } = new List<PhieuBaohanhKho>();

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<TiepnhanBaohanh> TiepnhanBaohanhs { get; set; } = new List<TiepnhanBaohanh>();

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<TuchoiBaohanh> TuchoiBaohanhs { get; set; } = new List<TuchoiBaohanh>();
}
