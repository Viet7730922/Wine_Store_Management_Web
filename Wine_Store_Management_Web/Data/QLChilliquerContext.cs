using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Data;

public partial class QLChilliquerContext : DbContext
{
    public QLChilliquerContext()
    {
    }

    public QLChilliquerContext(DbContextOptions<QLChilliquerContext> options)
        : base(options)
    {
    }

    // Đổi tên DbSet trong QLChilliquerContext cho khớp với controller
    public virtual DbSet<Nhanvien> NhanViens { get; set; }
    public virtual DbSet<Sanpham> SanPhams { get; set; }
    public virtual DbSet<Khachhang> KhachHangs { get; set; }
    public virtual DbSet<Khuyenmai> KhuyenMais { get; set; }
    public virtual DbSet<Phieunhap> PhieuNhaps { get; set; }
    public virtual DbSet<ChitietPhieunhap> ChiTietPhieuNhaps { get; set; }
    public virtual DbSet<Hoadon> HoaDons { get; set; }
    public virtual DbSet<ChitietHoadon> ChiTietHoaDons { get; set; }
    public virtual DbSet<TiepnhanBaohanh> TiepNhanBaoHanhs { get; set; }
    public virtual DbSet<TuchoiBaohanh> TuChoiBaoHanhs { get; set; }
    public virtual DbSet<PhieuBaohanhKho> PhieuBaoHanhKhos { get; set; }
    public virtual DbSet<BiennhanGiaodich> BienNhanGiaoDichs { get; set; }
    public virtual DbSet<Kiemkekho> KiemKeKhos { get; set; }
    public virtual DbSet<ChitietKiemke> ChiTietKiemKes { get; set; }
    public virtual DbSet<LogHeThong> LogHeThongs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BiennhanGiaodich>(entity =>
        {
            entity.HasKey(e => e.MaBienNhan).HasName("PK__BIENNHAN__95CF1198303B7BFB");

            entity.HasOne(d => d.ThuNganXuatPhieuNavigation).WithMany(p => p.BiennhanGiaodiches)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BNGD_ThuNgan");
        });

        modelBuilder.Entity<ChitietHoadon>(entity =>
        {
            entity.HasKey(e => new { e.SoHoaDon, e.MaSanPham }).HasName("PK__CHITIET___CE82EA10E4FC4656");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChitietHoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTHD_SanPham");

            entity.HasOne(d => d.SoHoaDonNavigation).WithMany(p => p.ChitietHoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTHD_HoaDon");
        });

        modelBuilder.Entity<ChitietKiemke>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieuKiemKe, e.MaSanPham }).HasName("PK__CHITIET___8F1CFAEA1D5E3AC6");

            entity.HasOne(d => d.MaPhieuKiemKeNavigation).WithMany(p => p.ChitietKiemkes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTKK_PhieuKiemKe");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChitietKiemkes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTKK_SanPham");
        });

        modelBuilder.Entity<ChitietPhieunhap>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieuNhap, e.MaSanPham }).HasName("PK__CHITIET___DBDC9B7971381393");

            entity.HasOne(d => d.MaPhieuNhapNavigation).WithMany(p => p.ChitietPhieunhaps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPN_PhieuNhap");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChitietPhieunhaps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPN_SanPham");
        });

        modelBuilder.Entity<Hoadon>(entity =>
        {
            entity.HasKey(e => e.SoHoaDon).HasName("PK__HOADON__012E9E52FC0B042B");

            entity.Property(e => e.DiaChiBanHang).HasDefaultValue("138 Mai Xuân Thưởng, Nha Trang, Khánh Hòa");
            entity.Property(e => e.DienThoaiBanHang).HasDefaultValue("0905XXXXXX");
            entity.Property(e => e.DonViBanHang).HasDefaultValue("Cửa Hàng Rượu Chilliquer");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.Hoadons).HasConstraintName("FK_HOADON_KhachHang");

            entity.HasOne(d => d.MaKhuyenMaiNavigation).WithMany(p => p.Hoadons).HasConstraintName("FK_HOADON_KhuyenMai");

            entity.HasOne(d => d.ThuNganNavigation).WithMany(p => p.Hoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HOADON_ThuNgan");
        });

        modelBuilder.Entity<Khachhang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__KHACHHAN__88D2F0E5DE1A2F30");

            entity.Property(e => e.DiemTichLuy).HasDefaultValue(0);
        });

        modelBuilder.Entity<Khuyenmai>(entity =>
        {
            entity.HasKey(e => e.MaChuongTrinh).HasName("PK__KHUYENMA__5323D588EAA74C17");

            entity.HasOne(d => d.NguoiPheDuyetNavigation).WithMany(p => p.Khuyenmais)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KHUYENMAI_NguoiPheDuyet");
        });

        modelBuilder.Entity<Kiemkekho>(entity =>
        {
            entity.HasKey(e => e.MaPhieuKiemKe).HasName("PK__KIEMKEKH__40B08EA8C7BC3234");

            entity.HasOne(d => d.NguoiLapKiemKeNavigation).WithMany(p => p.Kiemkekhos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KIEMKE_NguoiLap");
        });

        modelBuilder.Entity<LogHeThong>(entity =>
        {
            entity.HasKey(e => e.MaLog).HasName("PK__LOG_HE_T__3B98D24A4FF9E5FC");
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__NHANVIEN__77B2CA47C4EF5A8A");

            entity.HasOne(d => d.NguoiThucHienNavigation).WithMany(p => p.InverseNguoiThucHienNavigation).HasConstraintName("FK_NHANVIEN_NguoiThucHien");
        });

        modelBuilder.Entity<PhieuBaohanhKho>(entity =>
        {
            entity.HasKey(e => e.MaPhieuBh).HasName("PK__PHIEU_BA__880CB92CAE656FA4");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.PhieuBaohanhKhos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PBHK_KhachHang");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.PhieuBaohanhKhos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PBHK_SanPham");

            entity.HasOne(d => d.NhanVienThucHienNavigation).WithMany(p => p.PhieuBaohanhKhos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PBHK_NhanVien");
        });

        modelBuilder.Entity<Phieunhap>(entity =>
        {
            entity.HasKey(e => e.MaPhieuNhap).HasName("PK__PHIEUNHA__1470EF3B09908B8F");

            entity.HasOne(d => d.NguoiNhapKhoNavigation).WithMany(p => p.Phieunhaps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PHIEUNHAP_NguoiNhapKho");
        });

        modelBuilder.Entity<Sanpham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SANPHAM__FAC7442DDF642CB4");

            entity.Property(e => e.SoLuongTon).HasDefaultValue(0);
            entity.Property(e => e.TrangThai).HasDefaultValue("Kích hoạt");

            entity.HasOne(d => d.NguoiLapPhieuNavigation).WithMany(p => p.Sanphams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SANPHAM_NguoiLapPhieu");
        });

        modelBuilder.Entity<TiepnhanBaohanh>(entity =>
        {
            entity.HasKey(e => e.SoBienBan).HasName("PK__TIEPNHAN__E0A3A8B7DD72516F");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.TiepnhanBaohanhs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TNBH_HoaDon");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.TiepnhanBaohanhs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TNBH_SanPham");

            entity.HasOne(d => d.ThuNganTiepNhanNavigation).WithMany(p => p.TiepnhanBaohanhs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TNBH_ThuNgan");
        });

        modelBuilder.Entity<TuchoiBaohanh>(entity =>
        {
            entity.HasKey(e => e.MaPhieuTuChoi).HasName("PK__TUCHOI_B__CE6E0D6606504165");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.TuchoiBaohanhs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TCBH_KhachHang");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.TuchoiBaohanhs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TCBH_SanPham");

            entity.HasOne(d => d.ThuNganXacNhanNavigation).WithMany(p => p.TuchoiBaohanhs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TCBH_ThuNgan");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
