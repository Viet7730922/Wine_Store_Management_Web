using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[Table("PHIEUNHAP")]
public partial class Phieunhap
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string MaPhieuNhap { get; set; } = null!;

    public DateOnly NgayNhap { get; set; }

    [StringLength(150)]
    public string NhaCungCap { get; set; } = null!;

    [StringLength(150)]
    public string NhapTaiKho { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string NguoiNhapKho { get; set; } = null!;

    public string? SoChungTuGoc { get; set; }

    public string? TongTienVietBangChu { get; set; }

    [InverseProperty("MaPhieuNhapNavigation")]
    public virtual ICollection<ChitietPhieunhap> ChitietPhieunhaps { get; set; } = new List<ChitietPhieunhap>();

    [ForeignKey("NguoiNhapKho")]
    [InverseProperty("Phieunhaps")]
    public virtual Nhanvien NguoiNhapKhoNavigation { get; set; } = null!;
}
