using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wine_Store_Management_Web.Data;

namespace Wine_Store_Management_Web.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly QLChilliquerContext _context;

        public KhachHangController(QLChilliquerContext context)
        {
            _context = context;
        }

        // GET: KhachHang/TraCuuNhanh (Dành cho Thu ngân - Biểu mẫu BH_BM2)
        // Sơ đồ DFD 1.3.9
        [HttpGet]
        public async Task<IActionResult> TraCuuNhanh(string keyword)
        {
            ViewBag.Keyword = keyword;

            if (string.IsNullOrEmpty(keyword))
            {
                return View();
            }

            // Tìm kiếm khách hàng theo Mã KH hoặc Số điện thoại
            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.MaKhachHang == keyword || k.SoDienThoai == keyword);

            if (khachHang == null)
            {
                ViewBag.Message = "Không tìm thấy khách hàng nào phù hợp với thông tin đã nhập!";
                return View();
            }

            // Lấy 3 giao dịch gần nhất của khách hàng kèm chi tiết để tính tổng tiền
            var recentTransactions = await _context.HoaDons
                .Include(h => h.ChitietHoadons)
                .Where(h => h.MaKhachHang == khachHang.MaKhachHang)
                .OrderByDescending(h => h.NgayHoaDon)
                .Take(3)
                .ToListAsync();

            // Tính tổng tiền cho từng hóa đơn và lưu tạm vào GhiChu (hoặc có thể dùng ViewModel)
            // Để tiện hiển thị ra View, ta tạm dùng Viewbag truyền danh sách gốc qua
            ViewBag.RecentTransactions = recentTransactions;

            return View(khachHang);
        }

        // GET: KhachHang/TraCuuChiTiet (Dành cho Quản lý - Biểu mẫu QLCH_BM3)
        // Sơ đồ DFD 1.3.16
        [HttpGet]
        public async Task<IActionResult> TraCuuChiTiet(string keyword)
        {
            ViewBag.Keyword = keyword;

            if (string.IsNullOrEmpty(keyword))
            {
                return View();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.MaKhachHang == keyword || k.SoDienThoai == keyword);

            if (khachHang == null)
            {
                ViewBag.Message = "Hệ thống không ghi nhận hồ sơ khách hàng này.";
                return View();
            }

            // Quản lý có thể xem được nhiều giao dịch hơn (ví dụ: lấy top 10)
            var transactions = await _context.HoaDons
                .Where(h => h.MaKhachHang == khachHang.MaKhachHang)
                .OrderByDescending(h => h.NgayHoaDon)
                .Take(10)
                .ToListAsync();

            ViewBag.RecentTransactions = transactions;

            return View(khachHang);
        }

        // Action hiển thị danh sách toàn bộ khách hàng (Dành cho Admin quản lý tệp KH)
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.KhachHangs.OrderByDescending(k => k.NgayDangKyThe).ToListAsync();
            return View(danhSach);
        }
    }
}