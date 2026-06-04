using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[Table("KIEMKEKHO")]
public partial class Kiemkekho
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string MaPhieuKiemKe { get; set; } = null!;

    public DateOnly ThoiGianKiemKe { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string NguoiLapKiemKe { get; set; } = null!;

    [InverseProperty("MaPhieuKiemKeNavigation")]
    public virtual ICollection<ChitietKiemke> ChitietKiemkes { get; set; } = new List<ChitietKiemke>();

    [ForeignKey("NguoiLapKiemKe")]
    [InverseProperty("Kiemkekhos")]
    public virtual Nhanvien NguoiLapKiemKeNavigation { get; set; } = null!;
}
