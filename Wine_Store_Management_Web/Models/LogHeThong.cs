using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wine_Store_Management_Web.Models;

[Table("LOG_HE_THONG")]
public partial class LogHeThong
{
    [Key]
    public int MaLog { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ThoiGian { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string TenTaiKhoan { get; set; } = null!;

    [StringLength(255)]
    public string ThaoTacThucHien { get; set; } = null!;

    [StringLength(100)]
    public string TrangThai { get; set; } = null!;
}
