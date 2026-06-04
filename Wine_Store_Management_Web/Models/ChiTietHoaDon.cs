using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[PrimaryKey("SoHoaDon", "MaSanPham")]
[Table("CHITIET_HOADON")]
public partial class ChitietHoadon
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string SoHoaDon { get; set; } = null!;

    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(15, 2)")]
    public decimal DonGia { get; set; }

    [ForeignKey("MaSanPham")]
    [InverseProperty("ChitietHoadons")]
    public virtual Sanpham MaSanPhamNavigation { get; set; } = null!;

    [ForeignKey("SoHoaDon")]
    [InverseProperty("ChitietHoadons")]
    public virtual Hoadon SoHoaDonNavigation { get; set; } = null!;
}
