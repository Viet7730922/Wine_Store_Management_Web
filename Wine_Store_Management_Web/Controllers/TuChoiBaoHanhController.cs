using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wine_Store_Management_Web.Data;
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class TuChoiBaoHanhController : Controller
    {
        private readonly QLChilliquerContext _context;

        public TuChoiBaoHanhController(QLChilliquerContext context)
        {
            _context = context;
        }

        // GET: TuChoiBaoHanh/Index (Trang danh sách)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.TuChoiBaoHanhs
                .Include(t => t.MaKhachHangNavigation)
                .Include(t => t.MaSanPhamNavigation)
                .Include(t => t.ThuNganXacNhanNavigation)
                .OrderByDescending(t => t.NgayLapPhieu)
                .ToListAsync();
            return View(danhSach);
        }

        // GET: TuChoiBaoHanh/Details/5 (Xem và in phiếu BH_BM4)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var phieu = await _context.TuChoiBaoHanhs
                .Include(t => t.MaKhachHangNavigation)
                .Include(t => t.MaSanPhamNavigation)
                .Include(t => t.ThuNganXacNhanNavigation)
                .FirstOrDefaultAsync(m => m.MaPhieuTuChoi == id);

            if (phieu == null) return NotFound();

            return View(phieu);
        }

        // GET: TuChoiBaoHanh/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            ViewBag.SanPhams = _context.SanPhams.ToList();
            ViewBag.NhanViens = _context.NhanViens
                .Where(nv => nv.VaiTro.Contains("Thu ngân") || nv.VaiTro.Contains("Admin"))
                .ToList();

            var listHoaDon = _context.HoaDons
                .Include(h => h.ChitietHoadons)
                .Where(h => h.MaKhachHang != null)
                .Select(h => new {
                    MaHoaDon = h.SoHoaDon,
                    MaKhachHang = h.MaKhachHang,
                    SanPhams = h.ChitietHoadons.Select(ct => ct.MaSanPham).ToList()
                }).ToList();

            ViewBag.HoaDonsJson = System.Text.Json.JsonSerializer.Serialize(listHoaDon);

            var model = new TuchoiBaohanh
            {
                NgayLapPhieu = DateOnly.FromDateTime(DateTime.Now)
            };

            return View(model);
        }

        // POST: TuChoiBaoHanh/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TuchoiBaohanh tuChoi, string? LyDoChiTietKhac)
        {
            ModelState.Remove("MaKhachHangNavigation");
            ModelState.Remove("MaSanPhamNavigation");
            ModelState.Remove("ThuNganXacNhanNavigation");

            if (tuChoi.LyDoTuChoi == "Khác" && !string.IsNullOrEmpty(LyDoChiTietKhac))
            {
                tuChoi.LyDoTuChoi = "Lý do khác: " + LyDoChiTietKhac;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(tuChoi);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đã lập phiếu từ chối bảo hành thành công!";
                    // ĐIỀU HƯỚNG THẲNG SANG TRANG CHI TIẾT ĐỂ IN ẤN PHIẾU VỪA TẠO
                    return RedirectToAction(nameof(Details), new { id = tuChoi.MaPhieuTuChoi });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lập phiếu từ chối: " + ex.Message);
                }
            }

            LoadLaiDuLieuViewBag();
            return View(tuChoi);
        }

        private void LoadLaiDuLieuViewBag()
        {
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            ViewBag.SanPhams = _context.SanPhams.ToList();
            ViewBag.NhanViens = _context.NhanViens.Where(nv => nv.VaiTro.Contains("Thu ngân") || nv.VaiTro.Contains("Admin")).ToList();

            var listHoaDon = _context.HoaDons.Include(h => h.ChitietHoadons).Where(h => h.MaKhachHang != null).Select(h => new {
                MaHoaDon = h.SoHoaDon,
                MaKhachHang = h.MaKhachHang,
                SanPhams = h.ChitietHoadons.Select(ct => ct.MaSanPham).ToList()
            }).ToList();
            ViewBag.HoaDonsJson = System.Text.Json.JsonSerializer.Serialize(listHoaDon);
        }
    }
}