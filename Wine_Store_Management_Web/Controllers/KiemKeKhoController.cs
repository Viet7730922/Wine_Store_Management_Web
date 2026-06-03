using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wine_Store_Management_Web.Data;
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class KiemKeKhoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KiemKeKhoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KiemKeKho kiemKeKho, List<ChiTietKiemKe> ChiTietKiemKes)
        {
            if (ModelState.IsValid && ChiTietKiemKes != null && ChiTietKiemKes.Count > 0)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Lưu Phiếu Kiểm Kê (Master)
                    _context.KiemKeKhos.Add(kiemKeKho);
                    await _context.SaveChangesAsync();

                    // 2. Lưu Chi Tiết Kiểm Kê (Detail)
                    foreach (var chiTiet in ChiTietKiemKes)
                    {
                        chiTiet.MaPhieuKiemKe = kiemKeKho.MaPhieuKiemKe;

                        // Xác nhận lại toán học để đảm bảo tính toàn vẹn dữ liệu
                        // Công thức DFD: Chênh lệch = Tồn kho thực tế - Tồn kho hệ thống
                        chiTiet.ChenhLech = chiTiet.TonKhoThucTe - chiTiet.TonKhoHeThong;

                        _context.ChiTietKiemKes.Add(chiTiet);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Lưu phiếu kiểm kê kho thành công!";
                    return RedirectToAction("Index", "SanPham"); // Trở về danh sách tồn kho
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Lỗi hệ thống khi lưu phiếu kiểm kê: " + ex.Message);
                }
            }
            return View(kiemKeKho);
        }
    }
}