using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wine_Store_Management_Web.Data;
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class KhuyenMaiController : Controller
    {
        private readonly QLChilliquerContext _context;

        public KhuyenMaiController(QLChilliquerContext context)
        {
            _context = context;
        }

        // GET: KhuyenMai/Create
        [HttpGet]
        public IActionResult Create()
        {
            // FIX BUG 1: Khởi tạo sẵn dữ liệu để thẻ asp-for không ghi đè ngày thành rỗng/mặc định
            var km = new Khuyenmai
            {
                NgayDuyet = DateOnly.FromDateTime(DateTime.Now),
                TuNgay = DateOnly.FromDateTime(DateTime.Now),
                DenNgay = DateOnly.FromDateTime(DateTime.Now).AddDays(7)
            };
            return View(km);
        }

        // POST: KhuyenMai/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Khuyenmai khuyenMai)
        {
            // 1. Quét và xóa toàn bộ các bắt lỗi ngầm của EF Core sinh ra
            var keysToRemove = ModelState.Keys.Where(k => k.Contains("Navigation") || k == "Hoadons").ToList();
            foreach (var key in keysToRemove)
            {
                ModelState.Remove(key);
            }

            // 2. Gán tự động Người duyệt nếu để trống
            if (string.IsNullOrEmpty(khuyenMai.NguoiPheDuyet))
            {
                ModelState.Remove("NguoiPheDuyet");
                khuyenMai.NguoiPheDuyet = "NV01";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra ngày hợp lệ
                    if (khuyenMai.TuNgay > khuyenMai.DenNgay)
                    {
                        ModelState.AddModelError("DenNgay", "Ngày kết thúc không được nhỏ hơn ngày bắt đầu!");
                        return View(khuyenMai);
                    }

                    // Kiểm tra trùng mã Khuyến mãi
                    var isExist = await _context.KhuyenMais.AnyAsync(k => k.MaChuongTrinh == khuyenMai.MaChuongTrinh);
                    if (isExist)
                    {
                        ModelState.AddModelError("MaChuongTrinh", "Mã chương trình ưu đãi này đã tồn tại!");
                        return View(khuyenMai);
                    }

                    // ĐÃ XÓA TÍNH NĂNG CHẶN CHỒNG CHÉO THỜI GIAN
                    // (Cho phép chạy nhiều chiến dịch khuyến mãi song song)

                    // Lưu thẳng vào Database
                    _context.Add(khuyenMai);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Thiết lập chương trình khuyến mãi thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }

            // Nếu có lỗi ngầm chưa bị bắt, hệ thống sẽ trả lại View
            return View(khuyenMai);
        }

        // GET: KhuyenMai/Index
        public IActionResult Index()
        {
            var danhSachKhuyenMai = _context.KhuyenMais.OrderByDescending(k => k.NgayDuyet).ToList();
            return View(danhSachKhuyenMai);
        }
    }
}