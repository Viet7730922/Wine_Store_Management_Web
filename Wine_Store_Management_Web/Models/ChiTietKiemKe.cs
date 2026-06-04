using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[PrimaryKey("MaPhieuKiemKe", "MaSanPham")]
[Table("CHITIET_KIEMKE")]
public partial class ChitietKiemke
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string MaPhieuKiemKe { get; set; } = null!;

    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    public int TonKhoThucTe { get; set; }

    public int TonKhoHeThong { get; set; }

    public int ChenhLech { get; set; }

    [ForeignKey("MaPhieuKiemKe")]
    [InverseProperty("ChitietKiemkes")]
    public virtual Kiemkekho MaPhieuKiemKeNavigation { get; set; } = null!;

    [ForeignKey("MaSanPham")]
    [InverseProperty("ChitietKiemkes")]
    public virtual Sanpham MaSanPhamNavigation { get; set; } = null!;
}
