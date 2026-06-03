using Microsoft.EntityFrameworkCore;
using Wine_Store_Management_Web.Models; 

namespace Wine_Store_Management_Web.Data 
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<NhanVien> NhanViens { get; set; } = null!;
        public DbSet<SanPham> SanPhams { get; set; } = null!;
        public DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public DbSet<KhuyenMai> KhuyenMais { get; set; } = null!;
        public DbSet<PhieuNhap> PhieuNhaps { get; set; } = null!;
        public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = null!;
        public DbSet<HoaDon> HoaDons { get; set; } = null!;
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; } = null!;
        public DbSet<TiepNhanBaoHanh> TiepNhanBaoHanhs { get; set; } = null!;
        public DbSet<TuChoiBaoHanh> TuChoiBaoHanhs { get; set; } = null!;
        public DbSet<PhieuBaoHanhKho> PhieuBaoHanhKhos { get; set; } = null!;
        public DbSet<BienNhanGiaoDich> BienNhanGiaoDichs { get; set; } = null!;
        public DbSet<KiemKeKho> KiemKeKhos { get; set; } = null!;
        public DbSet<ChiTietKiemKe> ChiTietKiemKes { get; set; } = null!;
        public DbSet<LogHeThong> LogHeThongs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) // Sửa từ ModelCreatingBuilder thành ModelBuilder
        {
            base.OnModelCreating(modelBuilder);

            // Khớp chính xác tên bảng dạng viết hoa trong SQL Server
            modelBuilder.Entity<NhanVien>().ToTable("NHANVIEN");
            modelBuilder.Entity<SanPham>().ToTable("SANPHAM");
            modelBuilder.Entity<KhachHang>().ToTable("KHACHHANG");
            modelBuilder.Entity<KhuyenMai>().ToTable("KHUYENMAI");
            modelBuilder.Entity<PhieuNhap>().ToTable("PHIEUNHAP");
            modelBuilder.Entity<ChiTietPhieuNhap>().ToTable("CHITIET_PHIEUNHAP");
            modelBuilder.Entity<HoaDon>().ToTable("HOADON");
            modelBuilder.Entity<ChiTietHoaDon>().ToTable("CHITIET_HOADON");
            modelBuilder.Entity<TiepNhanBaoHanh>().ToTable("TIEPNHAN_BAOHANH");
            modelBuilder.Entity<TuChoiBaoHanh>().ToTable("TUCHOI_BAOHANH");
            modelBuilder.Entity<PhieuBaoHanhKho>().ToTable("PHIEU_BAOHANH_KHO");
            modelBuilder.Entity<BienNhanGiaoDich>().ToTable("BIENNHAN_GIAODICH");
            modelBuilder.Entity<KiemKeKho>().ToTable("KIEMKEKHO");
            modelBuilder.Entity<ChiTietKiemKe>().ToTable("CHITIET_KIEMKE");
            modelBuilder.Entity<LogHeThong>().ToTable("LOG_HE_THONG");

            // Cấu hình Khóa phức hợp (Composite Primary Keys)
            modelBuilder.Entity<ChiTietPhieuNhap>()
                .HasKey(c => new { c.MaPhieuNhap, c.MaSanPham });

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasKey(c => new { c.SoHoaDon, c.MaSanPham });

            modelBuilder.Entity<ChiTietKiemKe>()
                .HasKey(c => new { c.MaPhieuKiemKe, c.MaSanPham });

            // Ràng buộc tự tham chiếu (Self-referencing FK) của bảng NHANVIEN
            modelBuilder.Entity<NhanVien>()
                .HasOne(n => n.NhanVienThucHien)
                .WithMany()
                .HasForeignKey(n => n.NguoiThucHien)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}