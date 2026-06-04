using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[PrimaryKey("MaPhieuNhap", "MaSanPham")]
[Table("CHITIET_PHIEUNHAP")]
public partial class ChitietPhieunhap
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string MaPhieuNhap { get; set; } = null!;

    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(15, 2)")]
    public decimal DonGia { get; set; }

    [ForeignKey("MaPhieuNhap")]
    [InverseProperty("ChitietPhieunhaps")]
    public virtual Phieunhap MaPhieuNhapNavigation { get; set; } = null!;

    [ForeignKey("MaSanPham")]
    [InverseProperty("ChitietPhieunhaps")]
    public virtual Sanpham MaSanPhamNavigation { get; set; } = null!;
}
