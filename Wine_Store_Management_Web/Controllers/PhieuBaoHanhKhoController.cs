using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wine_Store_Management_Web.Data;
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class PhieuBaoHanhKhoController : Controller
    {
        private readonly QLChilliquerContext _context;

        public PhieuBaoHanhKhoController(QLChilliquerContext context)
        {
            _context = context;
        }

        // GET: PhieuBaoHanhKho/Index
        // Hiển thị lịch sử nhật ký kết xuất phiếu bảo hành kho
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.PhieuBaoHanhKhos
                .Include(p => p.MaKhachHangNavigation)
                .Include(p => p.MaSanPhamNavigation)
                .Include(p => p.NhanVienThucHienNavigation)
                .OrderByDescending(p => p.NgayTiepNhanHeThong)
                .ToListAsync();
            return View(danhSach);
        }

        // GET: PhieuBaoHanhKho/Details/5
        // Kết xuất dữ liệu hệ thống ra biểu mẫu in ấn mẫu BH_BM05 giống thiết kế ảnh yêu cầu
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var phieu = await _context.PhieuBaoHanhKhos
                .Include(p => p.MaKhachHangNavigation)
                .Include(p => p.MaSanPhamNavigation)
                .Include(p => p.NhanVienThucHienNavigation)
                .FirstOrDefaultAsync(m => m.MaPhieuBh == id);

            if (phieu == null) return NotFound();

            return View(phieu);
        }

        // GET: PhieuBaoHanhKho/Create
        [HttpGet]
        public IActionResult Create()
        {
            // 1. Nạp danh sách hỗ trợ tìm kiếm/chọn Khách hàng và Sản phẩm ở Frontend
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            ViewBag.SanPhams = _context.SanPhams.ToList();

            // 2. Thuật toán tự động sinh Mã Phiếu lấp khoảng trống dãy số (Find Missing Sequence)
            var existingCodes = _context.PhieuBaoHanhKhos.Select(p => p.MaPhieuBh).ToList();
            int nextNumber = 1;

            var numList = existingCodes
                .Select(c => {
                    int.TryParse(c.Replace("PBH-2026-", ""), out int n);
                    return n;
                })
                .Where(n => n > 0)
                .OrderBy(n => n)
                .ToList();

            for (int i = 1; i <= numList.Count + 1; i++)
            {
                if (!numList.Contains(i))
                {
                    nextNumber = i;
                    break;
                }
            }

            // 3. Khởi tạo thực thể ném ra giao diện với các giá trị thời gian tính toán sẵn
            var model = new PhieuBaohanhKho
            {
                MaPhieuBh = "PBH-2026-" + nextNumber.ToString("D4"), // Kết quả định dạng chuẩn: PBH-2026-0001
                NgayTiepNhanHeThong = DateOnly.FromDateTime(DateTime.Now),
                ThoiGianXuLyDuKien = 7, // Mặc định thời gian phòng kho xử lý kỹ thuật là 7 ngày
                NgayHenTra = DateOnly.FromDateTime(DateTime.Now.AddDays(7)) // Tự động cộng ngày hẹn trả
            };

            return View(model);
        }

        // POST: PhieuBaoHanhKho/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuBaohanhKho phieuBH)
        {
            // Loại bỏ kiểm tra xác thực dữ liệu của các thực thể liên kết ảo EF Core Scaffold
            ModelState.Remove("MaKhachHangNavigation");
            ModelState.Remove("MaSanPhamNavigation");
            ModelState.Remove("NhanVienThucHienNavigation");

            // Tự động kiểm tra và gán tài khoản nhân viên đang thực hiện
            if (string.IsNullOrEmpty(phieuBH.NhanVienThucHien))
            {
                ModelState.Remove("NhanVienThucHien");
                // Nếu ứng dụng tích hợp Identity thì dùng: User.Identity?.Name hoặc gán tài khoản phòng bảo hành mẫu
                phieuBH.NhanVienThucHien = "NV04";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra phòng ngừa trùng lặp mã khóa chính hệ thống
                    var isDuplicate = await _context.PhieuBaoHanhKhos.AnyAsync(p => p.MaPhieuBh == phieuBH.MaPhieuBh);
                    if (isDuplicate)
                    {
                        ModelState.AddModelError("MaPhieuBh", "Mã phiếu bảo hành kho này đã tồn tại trong hệ thống!");
                        LoadLaiDuLieuViewBag();
                        return View(phieuBH);
                    }

                    // Lưu dữ liệu biểu mẫu vào bảng PHIEU_BAOHANH_KHO
                    _context.Add(phieuBH);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Kết xuất dữ liệu hệ thống phiếu bảo hành kho thành công!";

                    // Sau khi lưu thành công, chuyển thẳng đến trang Details để xem và tiến hành In ấn mẫu BH_BM05
                    return RedirectToAction(nameof(Details), new { id = phieuBH.MaPhieuBh });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi hệ thống khi lưu phiếu bảo hành: " + ex.Message);
                }
            }

            LoadLaiDuLieuViewBag();
            return View(phieuBH);
        }

        // Hàm tiện ích nạp lại danh sách phòng khi dữ liệu bị từ chối/lỗi validation quay lại View
        private void LoadLaiDuLieuViewBag()
        {
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            ViewBag.SanPhams = _context.SanPhams.ToList();
        }
    }
}