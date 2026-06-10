using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wine_Store_Management_Web.Data;
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class PhieuNhapController : Controller
    {
        private readonly QLChilliquerContext _context;

        public PhieuNhapController(QLChilliquerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var danhSachPhieuNhap = await _context.PhieuNhaps
                .Include(p => p.NguoiNhapKhoNavigation)
                .OrderByDescending(p => p.NgayNhap)
                .ToListAsync();
            return View(danhSachPhieuNhap);
        }

        // GET: PhieuNhap/Details/PN0001
        // Kết xuất dữ liệu hệ thống ra biểu mẫu in ấn mẫu KHO_BM2 giống thiết kế ảnh yêu cầu
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var phieuNhap = await _context.PhieuNhaps
                .Include(p => p.NguoiNhapKhoNavigation)
                .Include(p => p.ChitietPhieunhaps)
                    .ThenInclude(c => c.MaSanPhamNavigation)
                .FirstOrDefaultAsync(m => m.MaPhieuNhap == id);

            if (phieuNhap == null) return NotFound();

            return View(phieuNhap);
        }

        // GET: PhieuNhap/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.SanPhams = _context.SanPhams.ToList();

            var model = new Phieunhap
            {
                MaPhieuNhap = GenerateNextMaPhieuNhap(),           // sinh tạm để hiển thị
                NgayNhap = DateOnly.FromDateTime(DateTime.Now),
                NhapTaiKho = "Kho NVL 138 Mai Xuân Thưởng"
            };

            return View(model);
        }

        // POST: PhieuNhap/Create
        // POST: PhieuNhap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Phieunhap phieuNhap, List<ChitietPhieunhap> ChiTietPhieuNhaps)
        {
            // Sinh mã phiếu nếu cần
            if (string.IsNullOrEmpty(phieuNhap.MaPhieuNhap) || phieuNhap.MaPhieuNhap == "PN0000")
            {
                phieuNhap.MaPhieuNhap = GenerateNextMaPhieuNhap();
            }

            // Dọn ModelState
            ModelState.Remove("NguoiNhapKhoNavigation");
            ModelState.Remove("ChitietPhieunhaps");

            foreach (var key in ModelState.Keys.ToList())
            {
                if (key.Contains("Navigation") || key.Contains(".MaPhieuNhap"))
                    ModelState.Remove(key);
            }

            if (ChiTietPhieuNhaps != null)
            {
                for (int i = 0; i < ChiTietPhieuNhaps.Count; i++)
                {
                    ModelState.Remove($"ChiTietPhieuNhaps[{i}].MaPhieuNhap");
                    ModelState.Remove($"ChiTietPhieuNhaps[{i}].MaSanPhamNavigation");
                    ModelState.Remove($"ChiTietPhieuNhaps[{i}].MaPhieuNhapNavigation");
                }
            }

            if (string.IsNullOrEmpty(phieuNhap.NguoiNhapKho))
                phieuNhap.NguoiNhapKho = "NV04";

            // Kiểm tra trùng sản phẩm
            if (ChiTietPhieuNhaps?.GroupBy(x => x.MaSanPham).Any(g => g.Count() > 1) == true)
            {
                ModelState.AddModelError("", "Một sản phẩm không được nhập nhiều lần trong cùng một phiếu.");
            }

            if (ModelState.IsValid && ChiTietPhieuNhaps != null && ChiTietPhieuNhaps.Any())
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // === Cách sửa chính: Chỉ Add PhieuNhap và gán chi tiết vào collection ===
                    phieuNhap.ChitietPhieunhaps.Clear(); // Đảm bảo sạch

                    foreach (var chiTiet in ChiTietPhieuNhaps)
                    {
                        chiTiet.MaPhieuNhap = phieuNhap.MaPhieuNhap;
                        phieuNhap.ChitietPhieunhaps.Add(chiTiet);   // ← Quan trọng: gán vào navigation

                        // Cập nhật tồn kho
                        var sanPham = await _context.SanPhams.FindAsync(chiTiet.MaSanPham);
                        if (sanPham != null)
                        {
                            sanPham.SoLuongTon += chiTiet.SoLuong;
                            _context.Update(sanPham);   // Đánh dấu update
                        }
                        else
                        {
                            ModelState.AddModelError("", $"Mã sản phẩm {chiTiet.MaSanPham} không tồn tại!");
                            LoadLaiDuLieuViewBag();
                            return View(phieuNhap);
                        }
                    }

                    _context.PhieuNhaps.Add(phieuNhap);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = $"Nhập kho thành công! Mã phiếu: {phieuNhap.MaPhieuNhap}";
                    return RedirectToAction(nameof(Details), new { id = phieuNhap.MaPhieuNhap });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Lỗi khi lưu phiếu: " + ex.Message);
                }
            }
            else if (ChiTietPhieuNhaps == null || !ChiTietPhieuNhaps.Any())
            {
                ModelState.AddModelError("", "Vui lòng thêm ít nhất 1 sản phẩm vào phiếu nhập kho.");
            }

            LoadLaiDuLieuViewBag();
            return View(phieuNhap);
        }

        // ==================== Hàm sinh mã ====================
        private string GenerateNextMaPhieuNhap()
        {
            var danhSachMaPN = _context.PhieuNhaps
                .Where(p => p.MaPhieuNhap.StartsWith("PN"))
                .Select(p => p.MaPhieuNhap)
                .ToList();

            int soMoi = 1;

            if (danhSachMaPN.Any())
            {
                var cacSo = danhSachMaPN
                    .Select(m =>
                    {
                        int.TryParse(m.Substring(2), out int so);
                        return so;
                    })
                    .Where(s => s > 0)
                    .OrderBy(s => s)
                    .ToList();

                for (int i = 1; i <= cacSo.Count + 100; i++) // phòng trường hợp có khoảng trống lớn
                {
                    if (!cacSo.Contains(i))
                    {
                        soMoi = i;
                        break;
                    }
                }
            }

            return "PN" + soMoi.ToString("D4");
        }

        private void LoadLaiDuLieuViewBag()
        {
            ViewBag.SanPhams = _context.SanPhams.ToList();
        }
    }
}