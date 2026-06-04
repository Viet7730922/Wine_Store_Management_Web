using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wine_Store_Management_Web.Data;

namespace Wine_Store_Management_Web.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly QLChilliquerContext _context;

        public BaoCaoController(QLChilliquerContext context)
        {
            _context = context;
        }

        // =================================================================================
        // 1. KHO_BM3: BÁO CÁO THỐNG KÊ KHO (Nhập - Xuất - Tồn)
        // DFD Chức năng 1.3.7
        // =================================================================================
        [HttpGet]
        public async Task<IActionResult> ThongKeKho(DateTime? tuNgay, DateTime? denNgay)
        {
            // Thiết lập giá trị mặc định là từ đầu tháng đến ngày hiện tại nếu chưa chọn
            if (!tuNgay.HasValue) tuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!denNgay.HasValue) denNgay = DateTime.Now;

            // Truyền giá trị ra View để hiển thị trên giao diện và giữ nguyên giá trị bộ lọc
            ViewBag.TuNgay = tuNgay.Value.ToString("dd/MM/yyyy");
            ViewBag.DenNgay = denNgay.Value.ToString("dd/MM/yyyy");
            ViewBag.TuNgay_Input = tuNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.DenNgay_Input = denNgay.Value.ToString("yyyy-MM-dd");

            // 1. Lấy danh sách toàn bộ sản phẩm
            var sanPhams = await _context.SanPhams.ToListAsync();

            var startDate = DateOnly.FromDateTime(tuNgay.Value.Date);
            var endExclusive = DateOnly.FromDateTime(denNgay.Value.Date).AddDays(1);

            var danhSachNhap = await _context.ChiTietPhieuNhaps
                .Include(c => c.MaPhieuNhapNavigation)
                .Where(c => c.MaPhieuNhapNavigation!.NgayNhap >= startDate
                         && c.MaPhieuNhapNavigation.NgayNhap < endExclusive)
                .ToListAsync();

            var danhSachXuat = await _context.ChiTietHoaDons
                .Include(c => c.SoHoaDonNavigation)
                .Where(c => c.SoHoaDonNavigation!.NgayHoaDon >= startDate
                         && c.SoHoaDonNavigation.NgayHoaDon < endExclusive)
                .ToListAsync();

            // 4. Kết hợp dữ liệu (LINQ) tạo ra các Object ẩn danh (dynamic) đẩy ra View
            var ketQuaThongKe = sanPhams.Select(sp => new
            {
                MaSanPham = sp.MaSanPham,
                TenSanPham = sp.TenSanPham,
                TongNhap = danhSachNhap.Where(n => n.MaSanPham == sp.MaSanPham).Sum(n => n.SoLuong),
                TongXuat = danhSachXuat.Where(x => x.MaSanPham == sp.MaSanPham).Sum(x => x.SoLuong),
                TonKho = sp.SoLuongTon // Lấy tồn kho hiện tại
            }).OrderBy(x => x.MaSanPham).ToList();

            return View(ketQuaThongKe);
        }

        // =================================================================================
        // 2. QLCH_BM5: BÁO CÁO TỔNG HỢP DOANH THU VÀ LỢI NHUẬN
        // DFD Chức năng 1.3.18
        // =================================================================================
        [HttpGet]
        public async Task<IActionResult> DoanhThu(DateTime? tuNgay, DateTime? denNgay)
        {
            if (!tuNgay.HasValue) tuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!denNgay.HasValue) denNgay = DateTime.Now;

            ViewBag.TuNgay = tuNgay.Value.ToString("dd/MM/yyyy");
            ViewBag.DenNgay = denNgay.Value.ToString("dd/MM/yyyy");
            ViewBag.TuNgay_Input = tuNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.DenNgay_Input = denNgay.Value.ToString("yyyy-MM-dd");

            // Prepare half-open date range [startDate, endExclusive)
            var startDate = DateOnly.FromDateTime(tuNgay.Value.Date);
            var endExclusive = DateOnly.FromDateTime(denNgay.Value.Date).AddDays(1);

            // Truy vấn hóa đơn cùng với chi tiết hóa đơn và thông tin giá vốn sản phẩm
            var hoaDons = await _context.HoaDons
                .Include(h => h.ChitietHoadons)
                    .ThenInclude(c => c.MaSanPhamNavigation)
                .Where(h => h.NgayHoaDon >= startDate && h.NgayHoaDon < endExclusive)
                .ToListAsync();

            // Nhóm dữ liệu theo ngày (Group By) để tính tổng
            var thongKeTheoNgay = hoaDons.GroupBy(h => h.NgayHoaDon)
                .Select(g => new
                {
                    NgayThang = g.Key.ToString("dd/MM/yyyy"),
                    SoDonHang = g.Count(),
                    TongDoanhThu = g.Sum(h => h.ChitietHoadons.Sum(c => c.SoLuong * c.DonGia)),
                    TongChiPhiVon = g.Sum(h => h.ChitietHoadons.Sum(c => c.SoLuong * c.MaSanPhamNavigation!.GiaNhap)),
                    LoiNhuan = g.Sum(h => h.ChitietHoadons.Sum(c => c.SoLuong * c.DonGia))
                             - g.Sum(h => h.ChitietHoadons.Sum(c => c.SoLuong * c.MaSanPhamNavigation!.GiaNhap))
                }).OrderBy(x => x.NgayThang).ToList();

            // Tính toán số liệu tổng kết cuối kỳ
            ViewBag.TongDonHang = thongKeTheoNgay.Sum(x => x.SoDonHang);
            ViewBag.TongDoanhThu = thongKeTheoNgay.Sum(x => x.TongDoanhThu).ToString("N2");
            ViewBag.TongLoiNhuan = thongKeTheoNgay.Sum(x => x.LoiNhuan).ToString("N2");

            return View(thongKeTheoNgay);
        }

        // =================================================================================
        // 3. QLCH_BM6: BÁO CÁO THỐNG KÊ HIỆU SUẤT BÁN HÀNG CỦA NHÂN VIÊN
        // DFD Chức năng 1.3.19
        // =================================================================================
        [HttpGet]
        public async Task<IActionResult> HieuSuatNhanVien(string kyThongKe)
        {
            DateTime dateFilter;
            // Xử lý chuỗi input month (YYYY-MM) từ View
            if (string.IsNullOrEmpty(kyThongKe) || !DateTime.TryParse(kyThongKe + "-01", out dateFilter))
            {
                dateFilter = DateTime.Now;
            }

            ViewBag.KyThongKe_Input = dateFilter.ToString("yyyy-MM");
            ViewBag.Thang = dateFilter.Month.ToString("D2");
            ViewBag.Nam = dateFilter.Year;

            // Truy vấn hóa đơn trong tháng/năm được chọn - use half-open range to include full days
            var startOfMonth = new DateOnly(dateFilter.Year, dateFilter.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            var hoaDons = await _context.HoaDons
                .Include(h => h.ThuNganNavigation) // Lấy thông tin họ tên nhân viên
                .Include(h => h.ChitietHoadons)
                .Where(h => h.NgayHoaDon >= startOfMonth && h.NgayHoaDon < startOfNextMonth)
                .ToListAsync();

            // Nhóm theo Thu ngân và tính toán hiệu suất
            var bangXepHang = hoaDons.GroupBy(h => h.ThuNgan)
                .Select(g => new
                {
                    MaNhanVien = g.Key,
                    HoTen = g.First().ThuNganNavigation?.HoTen ?? "Chưa xác định",
                    // Số ca trực (Mock logic): Đếm số ngày làm việc độc lập có phát sinh hóa đơn
                    SoCaTruc = g.Select(h => h.NgayHoaDon.Day).Distinct().Count(),
                    SoHoaDonLap = g.Count(),
                    DoanhSo = g.Sum(h => h.ChitietHoadons.Sum(c => c.SoLuong * c.DonGia))
                })
                // Sắp xếp thứ hạng từ doanh số cao nhất đến thấp nhất
                .OrderByDescending(x => x.DoanhSo)
                .ToList();

            return View(bangXepHang);
        }
    }
}