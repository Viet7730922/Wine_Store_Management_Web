using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wine_Store_Management_Web.Data;
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class PhieuNhapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhieuNhapController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PhieuNhap/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: PhieuNhap/Create
        // Sơ đồ luồng dữ liệu 1.3.2: Nhận thông tin phiếu nhập và lưu.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuNhap phieuNhap, List<ChiTietPhieuNhap> ChiTietPhieuNhaps)
        {
            if (ModelState.IsValid && ChiTietPhieuNhaps != null && ChiTietPhieuNhaps.Count > 0)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Lưu thông tin Phiếu Nhập (Master)
                    _context.PhieuNhaps.Add(phieuNhap);
                    await _context.SaveChangesAsync(); // Cần Save trước để có mã phiếu tham chiếu

                    // 2. Lưu Chi tiết Phiếu Nhập (Detail) và cộng dồn Tồn Kho
                    foreach (var chiTiet in ChiTietPhieuNhaps)
                    {
                        chiTiet.MaPhieuNhap = phieuNhap.MaPhieuNhap;
                        _context.ChiTietPhieuNhaps.Add(chiTiet);

                        // Tìm sản phẩm trong kho để cập nhật số lượng
                        var sanPham = await _context.SanPhams.FindAsync(chiTiet.MaSanPham);
                        if (sanPham != null)
                        {
                            // Hàng về kho -> Tăng số lượng tồn
                            sanPham.SoLuongTon += chiTiet.SoLuong;
                            _context.Update(sanPham);
                        }
                        else
                        {
                            ModelState.AddModelError("", $"Mã sản phẩm {chiTiet.MaSanPham} chưa được khởi tạo trong hệ thống!");
                            return View(phieuNhap);
                        }
                    }

                    // 3. Xác nhận lưu dữ liệu đồng loạt
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Hiển thị thông báo nhập sản phẩm thành công 
                    TempData["SuccessMessage"] = "Nhập kho thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Lỗi hệ thống khi tạo phiếu nhập: " + ex.Message);
                }
            }

            return View(phieuNhap);
        }

        // GET: PhieuNhap/Index
        public IActionResult Index()
        {
            var danhSachPhieuNhap = _context.PhieuNhaps
                .OrderByDescending(p => p.NgayNhap)
                .ToList();
            return View(danhSachPhieuNhap);
        }
    }
}