using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[Table("KHUYENMAI")]
public partial class Khuyenmai
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string MaChuongTrinh { get; set; } = null!;

    [StringLength(150)]
    public string TenChuongTrinh { get; set; } = null!;

    [StringLength(100)]
    public string HinhThucApDung { get; set; } = null!;

    [StringLength(255)]
    public string MucGiamGia { get; set; } = null!;

    public string? DieuKienApDung { get; set; }

    public DateOnly TuNgay { get; set; }

    public DateOnly DenNgay { get; set; }

    public DateOnly NgayDuyet { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string NguoiPheDuyet { get; set; } = null!;

    [InverseProperty("MaKhuyenMaiNavigation")]
    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    [ForeignKey("NguoiPheDuyet")]
    [InverseProperty("Khuyenmais")]
    public virtual Nhanvien NguoiPheDuyetNavigation { get; set; } = null!;
}
