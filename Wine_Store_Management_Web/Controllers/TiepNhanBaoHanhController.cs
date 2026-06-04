using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wine_Store_Management_Web.Data;
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
            // 1. Tải danh sách Khách hàng, Sản phẩm, Nhân viên thu ngân
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            ViewBag.SanPhams = _context.SanPhams.ToList();
            ViewBag.NhanViens = _context.NhanViens
                .Where(nv => nv.VaiTro.Contains("Thu ngân") || nv.VaiTro.Contains("Admin"))
                .ToList();

            // 2. Tải danh sách Hóa đơn kèm chi tiết, chuyển sang JSON để JS dò tìm tự động
            var listHoaDon = _context.HoaDons
                .Include(h => h.ChitietHoadons)
                .Where(h => h.MaKhachHang != null) // Chỉ lấy khách có mã thành viên
                .Select(h => new {
                    MaHoaDon = h.SoHoaDon,
                    MaKhachHang = h.MaKhachHang,
                    SanPhams = h.ChitietHoadons.Select(ct => ct.MaSanPham).ToList()
                }).ToList();

            ViewBag.HoaDonsJson = System.Text.Json.JsonSerializer.Serialize(listHoaDon);

            // 3. Tự động sinh Số Biên Bản (Dò tìm khoảng trống khuyết)
            var existingTN = _context.TiepNhanBaoHanhs.Select(t => t.SoBienBan).ToList();
            int nextId = 1;
            var existingNums = existingTN
                .Select(m => { int.TryParse(m.Replace("TNBH_", ""), out int num); return num; })
                .Where(n => n > 0).OrderBy(n => n).ToList();

            for (int i = 1; i <= existingNums.Count + 1; i++)
            {
                if (!existingNums.Contains(i)) { nextId = i; break; }
            }

            var model = new TiepnhanBaohanh
            {
                SoBienBan = "TNBH_" + nextId.ToString("D3"),
                NgayTiepNhan = DateOnly.FromDateTime(DateTime.Now)
            };

            return View(model);
        }

        // POST: TiepNhanBaoHanh/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TiepnhanBaohanh tiepNhan)
        {
            // FIX BUG: Xóa validation ngầm của EF Core do Navigation
            ModelState.Remove("MaHoaDonNavigation");
            ModelState.Remove("MaSanPhamNavigation");
            ModelState.Remove("ThuNganTiepNhanNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Kiểm tra trùng lặp số biên bản
                    var isExist = await _context.TiepNhanBaoHanhs.AnyAsync(t => t.SoBienBan == tiepNhan.SoBienBan);
                    if (isExist)
                    {
                        ModelState.AddModelError("SoBienBan", "Số biên bản này đã tồn tại trong hệ thống!");
                        LoadLaiDuLieuViewBag();
                        return View(tiepNhan);
                    }

                    // 2. Đọc dữ liệu đơn hàng cũ D3 dựa trên mã hóa đơn để đối chiếu
                    var hoaDonCu = await _context.HoaDons
                        .Include(h => h.ChitietHoadons)
                        .FirstOrDefaultAsync(h => h.SoHoaDon == tiepNhan.MaHoaDon);

                    if (hoaDonCu == null)
                    {
                        ModelState.AddModelError("MaHoaDon", "Không tìm thấy Hóa đơn. Vui lòng kiểm tra lại mã hóa đơn!");
                        LoadLaiDuLieuViewBag();
                        return View(tiepNhan);
                    }

                    // 3. Kiểm tra sản phẩm có trong hóa đơn không
                    var isProductInOrder = hoaDonCu.ChitietHoadons!.Any(ct => ct.MaSanPham == tiepNhan.MaSanPham);
                    if (!isProductInOrder)
                    {
                        ModelState.AddModelError("MaSanPham", "Sản phẩm này không nằm trong hóa đơn mua hàng đã cung cấp!");
                        LoadLaiDuLieuViewBag();
                        return View(tiepNhan);
                    }

                    // 4. Lưu biên bản
                    _context.Add(tiepNhan);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Lập biên bản tiếp nhận bảo hành thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                }
            }
            LoadLaiDuLieuViewBag();
            return View(tiepNhan);
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

        // GET: TiepNhanBaoHanh/Index
        public IActionResult Index()
        {
            var danhSachBaoHanh = _context.TiepNhanBaoHanhs.OrderByDescending(t => t.NgayTiepNhan).ToList();
            return View(danhSachBaoHanh);
        }
    }
}