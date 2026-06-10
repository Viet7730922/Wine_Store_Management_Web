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
    public class BienNhanGiaoDichController : Controller
    {
        private readonly QLChilliquerContext _context;

        public BienNhanGiaoDichController(QLChilliquerContext context)
        {
            _context = context;
        }

        // GET: BienNhanGiaoDich/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.BienNhanGiaoDichs
                .Include(b => b.ThuNganXuatPhieuNavigation)
                .OrderByDescending(b => b.NgayIn).ThenByDescending(b => b.GioIn)
                .ToListAsync();
            return View(danhSach);
        }

        // GET: BienNhanGiaoDich/Details/5 (Trang kết xuất bản in BH_BM6)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var bienNhan = await _context.BienNhanGiaoDichs
                .Include(b => b.ThuNganXuatPhieuNavigation)
                .FirstOrDefaultAsync(m => m.MaBienNhan == id);

            if (bienNhan == null) return NotFound();

            return View(bienNhan);
        }

        // GET: BienNhanGiaoDich/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.NhanViens = _context.NhanViens
                .Where(nv => nv.VaiTro.Contains("Thu ngân") || nv.VaiTro.Contains("Admin"))
                .ToList();

            // Tự động gom dữ liệu từ Hóa Đơn và Phiếu Bảo Hành thành danh sách JSON chung
            var giaoDichList = new List<object>();

            // Lấy dữ liệu Hóa đơn (Đơn hàng chờ giao)
            var hoaDons = _context.HoaDons
                .Include(h => h.MaKhachHangNavigation)
                .Include(h => h.ChitietHoadons)
                    .ThenInclude(c => c.MaSanPhamNavigation)
                .ToList();

            foreach (var h in hoaDons)
            {
                giaoDichList.Add(new
                {
                    Ma = h.SoHoaDon,
                    Loai = "Đơn hàng chờ giao",
                    TenKH = h.HoTenNguoiMua,
                    SDT = h.MaKhachHangNavigation?.SoDienThoai ?? "",
                    ChiTiet = string.Join(", ", h.ChitietHoadons.Select(c => $"{c.MaSanPhamNavigation?.TenSanPham} (x{c.SoLuong})")),
                    TongTien = h.ChitietHoadons.Sum(c => c.SoLuong * c.DonGia)
                });
            }

            // Lấy dữ liệu Phiếu bảo hành (Biên nhận trả hàng)
            var baoHanhs = _context.PhieuBaoHanhKhos
                .Include(p => p.MaKhachHangNavigation)
                .Include(p => p.MaSanPhamNavigation)
                .ToList();

            foreach (var p in baoHanhs)
            {
                giaoDichList.Add(new
                {
                    Ma = p.MaPhieuBh,
                    Loai = "Biên nhận lấy hàng bảo hành",
                    TenKH = p.MaKhachHangNavigation?.HoTen,
                    SDT = p.MaKhachHangNavigation?.SoDienThoai,
                    ChiTiet = $"Bàn giao trả SP: {p.MaSanPhamNavigation?.TenSanPham} (Seri: {p.SoSeri})",
                    TongTien = 0
                });
            }

            ViewBag.GiaoDichJson = System.Text.Json.JsonSerializer.Serialize(giaoDichList);
            ViewBag.DanhSachMa = giaoDichList; // Để build dropdown select

            var model = new BiennhanGiaodich
            {
                NgayIn = DateOnly.FromDateTime(DateTime.Now),
                GioIn = TimeOnly.FromDateTime(DateTime.Now)
            };

            return View(model);
        }

        // POST: BienNhanGiaoDich/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NgayIn,GioIn,LoaiBienNhan,MaDoiChieuHeThong,TenKhachHang,SoDienThoai,ChiTietSanPham,TongGiaTriDoiChieu,ThuNganXuatPhieu")] BiennhanGiaodich bienNhan)
        {
            ModelState.Remove("ThuNganXuatPhieuNavigation");

            if (string.IsNullOrEmpty(bienNhan.ThuNganXuatPhieu))
            {
                ModelState.Remove("ThuNganXuatPhieu");
                bienNhan.ThuNganXuatPhieu = "NV02"; // Mặc định Thu ngân 1
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(bienNhan);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đã xuất lệnh in biên nhận giao dịch!";
                    // ĐIỀU HƯỚNG THẲNG SANG TRANG DETAILS ĐỂ IN
                    return RedirectToAction(nameof(Details), new { id = bienNhan.MaBienNhan });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi in biên nhận: " + ex.Message);
                }
            }

            // Nạp lại danh sách nếu có lỗi
            ViewBag.NhanViens = _context.NhanViens.Where(nv => nv.VaiTro.Contains("Thu ngân") || nv.VaiTro.Contains("Admin")).ToList();
            return View(bienNhan);
        }
    }
}