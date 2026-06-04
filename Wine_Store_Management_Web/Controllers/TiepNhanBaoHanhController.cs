using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class TiepNhanBaoHanhController : Controller
    {
        private readonly QLChilliquerContext _context;

        public TiepNhanBaoHanhController(QLChilliquerContext context)
        {
            _context = context;
        }

        // GET: TiepNhanBaoHanh/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TiepNhanBaoHanh/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SoBienBan,NgayTiepNhan,HoTenKhachHang,SoDienThoai,MaSanPham,SoSeri,MaHoaDon,TinhTrangHuHong,PhuKienKemTheo,ThuNganTiepNhan")] TiepnhanBaohanh tiepNhan)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Kiểm tra trùng lặp số biên bản
                    var isExist = await _context.TiepNhanBaoHanhs.AnyAsync(t => t.SoBienBan == tiepNhan.SoBienBan);
                    if (isExist)
                    {
                        ModelState.AddModelError("SoBienBan", "Số biên bản này đã tồn tại trong hệ thống!");
                        return View(tiepNhan);
                    }

                    // 2. Đọc dữ liệu đơn hàng cũ D3 dựa trên mã hóa đơn để đối chiếu (DFD 1.3.10 - Bước 3)
                    var hoaDonCu = await _context.HoaDons
                        .Include(h => h.ChitietHoadons)
                        .FirstOrDefaultAsync(h => h.SoHoaDon == tiepNhan.MaHoaDon);

                    if (hoaDonCu == null)
                    {
                        ModelState.AddModelError("MaHoaDon", "Không tìm thấy dữ liệu Hóa đơn này trong hệ thống. Vui lòng kiểm tra lại mã hóa đơn mua hàng!");
                        return View(tiepNhan);
                    }

                    // 3. Kiểm tra xem Sản phẩm khách mang tới bảo hành có thực sự nằm trong Hóa đơn cũ không
                    var isProductInOrder = hoaDonCu.ChitietHoadons!.Any(ct => ct.MaSanPham == tiepNhan.MaSanPham);
                    if (!isProductInOrder)
                    {
                        ModelState.AddModelError("MaSanPham", "Sản phẩm này không nằm trong hóa đơn mua hàng đã cung cấp! Chuyển sang luồng Từ chối bảo hành.");
                        // Thực tế ở đây bạn có thể redirect sang Action của TuChoiBaoHanhController theo bước 4 của DFD.
                        return View(tiepNhan);
                    }

                    // 4. Lưu biên bản tiếp nhận vào bảng TIEPNHAN_BAOHANH
                    _context.Add(tiepNhan);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Lập biên bản tiếp nhận bảo hành thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lập biên bản: " + ex.Message);
                }
            }
            return View(tiepNhan);
        }

        // GET: TiepNhanBaoHanh/Index
        public IActionResult Index()
        {
            var danhSachBaoHanh = _context.TiepNhanBaoHanhs
                .OrderByDescending(t => t.NgayTiepNhan)
                .ToList();
            return View(danhSachBaoHanh);
        }
    }
}