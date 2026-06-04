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

        // GET: PhieuNhap/Index
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
            // 1. Nạp danh sách toàn bộ sản phẩm ra View phục vụ chọn mặt hàng nhập kho
            ViewBag.SanPhams = _context.SanPhams.ToList();

            // =====================================================================
            // 2. THUẬT TOÁN: TỰ ĐỘNG ĐIỀN MÃ PHIẾU NHẬP (Lấp đầy khoảng trống dạng PNXXXX)
            // =====================================================================
            var danhSachMaPN = _context.PhieuNhaps
                .Where(p => p.MaPhieuNhap.StartsWith("PN"))
                .Select(p => p.MaPhieuNhap)
                .ToList();

            int soMoi = 1;

            if (danhSachMaPN.Any())
            {
                var cacSoHienTai = danhSachMaPN
                    .Select(m => {
                        int.TryParse(m.Substring(2), out int so);
                        return so;
                    })
                    .Where(s => s > 0)
                    .OrderBy(s => s)
                    .ToList();

                for (int i = 1; i <= cacSoHienTai.Count + 1; i++)
                {
                    if (!cacSoHienTai.Contains(i))
                    {
                        soMoi = i;
                        break;
                    }
                }
            }

            // Định dạng chuỗi gồm 4 chữ số (VD: soMoi = 1 -> "PN0001")
            string maPhieuTuDong = "PN" + soMoi.ToString("D4");

            var model = new Phieunhap
            {
                MaPhieuNhap = maPhieuTuDong,
                NgayNhap = DateOnly.FromDateTime(DateTime.Now),
                NhapTaiKho = "Kho NVL 138 Mai Xuân Thưởng"
            };

            return View(model);
        }

        // POST: PhieuNhap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Phieunhap phieuNhap, List<ChitietPhieunhap> ChiTietPhieuNhaps)
        {
            // Dọn dẹp xác thực ngầm của EF Core
            ModelState.Remove("NguoiNhapKhoNavigation");
            ModelState.Remove("ChitietPhieunhaps");
            for (int i = 0; i < ChiTietPhieuNhaps.Count; i++)
            {
                ModelState.Remove($"ChiTietPhieuNhaps[{i}].MaPhieuNhapNavigation");
                ModelState.Remove($"ChiTietPhieuNhaps[{i}].MaSanPhamNavigation");
            }

            if (string.IsNullOrEmpty(phieuNhap.NguoiNhapKho))
            {
                ModelState.Remove("NguoiNhapKho");
                phieuNhap.NguoiNhapKho = "NV04"; // Mặc định gán tạm cho nhân viên quản lý kho
            }

            if (ModelState.IsValid && ChiTietPhieuNhaps != null && ChiTietPhieuNhaps.Count > 0)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Lưu thông tin Phiếu Nhập gốc
                    _context.PhieuNhaps.Add(phieuNhap);
                    await _context.SaveChangesAsync();

                    // 2. Lưu danh sách Chi tiết và cộng dồn số lượng tồn kho sản phẩm (Cơ chế D4)
                    foreach (var chiTiet in ChiTietPhieuNhaps)
                    {
                        chiTiet.MaPhieuNhap = phieuNhap.MaPhieuNhap;
                        _context.ChiTietPhieuNhaps.Add(chiTiet);

                        var sanPham = await _context.SanPhams.FindAsync(chiTiet.MaSanPham);
                        if (sanPham != null)
                        {
                            sanPham.SoLuongTon += chiTiet.SoLuong;
                            _context.Update(sanPham);
                        }
                        else
                        {
                            ModelState.AddModelError("", $"Mã sản phẩm {chiTiet.MaSanPham} chưa được khởi tạo!");
                            LoadLaiDuLieuViewBag();
                            return View(phieuNhap);
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Nhập kho thành công!";
                    // Chuyển hướng trực tiếp sang trang xem mẫu in chi tiết KHO_BM2 vừa tạo
                    return RedirectToAction(nameof(Details), new { id = phieuNhap.MaPhieuNhap });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Lỗi hệ thống khi tạo phiếu nhập: " + ex.Message);
                }
            }
            else if (ChiTietPhieuNhaps == null || ChiTietPhieuNhaps.Count == 0)
            {
                ModelState.AddModelError("", "Vui lòng thêm ít nhất 1 sản phẩm vào phiếu nhập kho.");
            }

            LoadLaiDuLieuViewBag();
            return View(phieuNhap);
        }

        private void LoadLaiDuLieuViewBag()
        {
            ViewBag.SanPhams = _context.SanPhams.ToList();
        }
    }
}