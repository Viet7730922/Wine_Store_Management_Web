using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wine_Store_Management_Web.Data;
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HoaDon/Create
        // Trả về biểu mẫu Hóa đơn bán hàng trống (BH_BM1)
        [HttpGet]
        public IActionResult Create()
        {
            // Lấy danh sách Khách hàng
            ViewBag.KhachHangs = _context.KhachHangs.ToList();

            // Lấy danh sách Sản phẩm còn Tồn Kho > 0
            ViewBag.SanPhams = _context.SanPhams
                .Where(sp => sp.SoLuongTon > 0)
                .ToList();

            return View();
        }

        // POST: HoaDon/Create
        // Xử lý lưu hóa đơn và toàn bộ danh sách chi tiết hàng hóa đi kèm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoaDon hoaDon, List<ChiTietHoaDon> ChiTietHoaDons)
        {
            // =========================================================================
            // FIX BUG VALIDATION: Xóa triệt để các bắt lỗi ngầm của Khóa ngoại và Navigation
            // =========================================================================
            ModelState.Remove("ThuNgan");
            ModelState.Remove("KhachHang");
            ModelState.Remove("KhuyenMai");
            ModelState.Remove("NhanVienThuNgan");

            for (int i = 0; i < ChiTietHoaDons.Count; i++)
            {
                ModelState.Remove($"ChiTietHoaDons[{i}].SoHoaDon");
                ModelState.Remove($"ChiTietHoaDons[{i}].HoaDon");
                ModelState.Remove($"ChiTietHoaDons[{i}].SanPham");
            }

            if (string.IsNullOrEmpty(hoaDon.ThuNgan))
            {
                // Nếu chưa có chức năng đăng nhập, gán tạm cho NV01
                hoaDon.ThuNgan = "NV01";
            }
            // =========================================================================

            // Kiểm tra tính hợp lệ của thông tin
            if (ModelState.IsValid && ChiTietHoaDons != null && ChiTietHoaDons.Count > 0)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Gán cố định thông tin và TỰ ĐỘNG SINH Mẫu số, Ký hiệu
                    hoaDon.DonViBanHang = "Cửa Hàng Rượu Chilliquer";
                    hoaDon.DiaChiBanHang = "138 Mai Xuân Thưởng, Nha Trang, Khánh Hòa";
                    hoaDon.DienThoaiBanHang = "0905XXXXXX";

                    if (string.IsNullOrEmpty(hoaDon.MauSo)) hoaDon.MauSo = "01GTKT";
                    if (string.IsNullOrEmpty(hoaDon.KyHieu)) hoaDon.KyHieu = "CQ/26E";

                    decimal tongTienHang = 0; // Tiền gốc (Đơn giá * Số lượng)

                    // FIX BUG TRACKING: Xóa dữ liệu dư thừa do Model Binder tạo ra
                    if (hoaDon.ChiTietHoaDons != null && hoaDon.ChiTietHoaDons.Any())
                    {
                        hoaDon.ChiTietHoaDons.Clear();
                    }

                    // 2. Thêm Hóa đơn gốc vào trước
                    _context.HoaDons.Add(hoaDon);
                    await _context.SaveChangesAsync();

                    // 3. Xử lý Chi tiết hóa đơn
                    foreach (var chiTiet in ChiTietHoaDons)
                    {
                        chiTiet.SoHoaDon = hoaDon.SoHoaDon;
                        _context.ChiTietHoaDons.Add(chiTiet);

                        var sanPham = await _context.SanPhams.FindAsync(chiTiet.MaSanPham);
                        if (sanPham == null)
                        {
                            ModelState.AddModelError("", $"Mã sản phẩm {chiTiet.MaSanPham} không tồn tại!");
                            LoadLaiDuLieuViewBag();
                            return View(hoaDon);
                        }

                        if (sanPham.SoLuongTon < chiTiet.SoLuong)
                        {
                            ModelState.AddModelError("", $"Sản phẩm '{sanPham.TenSanPham}' không đủ số lượng tồn kho!");
                            LoadLaiDuLieuViewBag();
                            return View(hoaDon);
                        }

                        // Trừ tồn kho
                        sanPham.SoLuongTon -= chiTiet.SoLuong;
                        _context.Update(sanPham);

                        tongTienHang += (chiTiet.SoLuong * chiTiet.DonGia);
                    }

                    // TÍNH TOÁN GIẢM GIÁ (Dựa trên Mã Khuyến Mãi nếu có)
                    decimal giamGia = 0;
                    if (!string.IsNullOrEmpty(hoaDon.MaKhuyenMai))
                    {
                        var km = await _context.KhuyenMais.FindAsync(hoaDon.MaKhuyenMai);
                        if (km != null && !string.IsNullOrEmpty(km.MucGiamGia))
                        {
                            // Trích xuất con số từ chuỗi (VD: "Giảm 10%" -> 10)
                            string numberOnly = new string(km.MucGiamGia.Where(char.IsDigit).ToArray());
                            if (decimal.TryParse(numberOnly, out decimal mucGiam))
                            {
                                // Nếu chuỗi có dấu % thì giảm theo phần trăm, ngược lại giảm tiền mặt
                                if (km.MucGiamGia.Contains("%"))
                                    giamGia = tongTienHang * (mucGiam / 100);
                                else
                                    giamGia = mucGiam;
                            }
                        }
                    }

                    // Công thức tính giá: Tổng tiền = (Đơn giá * Số lượng) - Giảm giá
                    decimal tongThanhToan = tongTienHang - giamGia;
                    if (tongThanhToan < 0) tongThanhToan = 0;

                    // 4. Cộng điểm tích lũy (Tổng tiền thanh toán / 500, làm tròn xuống)
                    if (!string.IsNullOrEmpty(hoaDon.MaKhachHang))
                    {
                        var khachHang = await _context.KhachHangs.FindAsync(hoaDon.MaKhachHang);
                        if (khachHang != null)
                        {
                            int diemTichLuyMoi = (int)Math.Floor(tongThanhToan / 500);
                            khachHang.DiemTichLuy += diemTichLuyMoi;
                            _context.Update(khachHang);
                        }
                    }

                    // Lưu toàn bộ các thay đổi
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Tạo hóa đơn thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Lỗi hệ thống khi tạo hóa đơn: " + ex.Message);
                }
            }
            else if (ChiTietHoaDons == null || ChiTietHoaDons.Count == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất 1 sản phẩm vào hóa đơn.");
            }

            LoadLaiDuLieuViewBag();
            return View(hoaDon);
        }

        // Hàm tiện ích để nạp lại dữ liệu ViewBag
        private void LoadLaiDuLieuViewBag()
        {
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            ViewBag.SanPhams = _context.SanPhams
                .Where(sp => sp.SoLuongTon > 0)
                .ToList();
        }

        // GET: HoaDon/Index
        // Hiển thị lịch sử danh sách hóa đơn bán hàng
        public IActionResult Index()
        {
            var danhSachHoaDon = _context.HoaDons
                .Include(h => h.KhachHang) // Ép tải dữ liệu khách hàng liên kết
                .OrderByDescending(h => h.NgayHoaDon)
                .ToList();
            return View(danhSachHoaDon);
        }

        // GET: HoaDon/Details/5
        // Hiển thị chi tiết mẫu in hóa đơn
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var hoaDon = await _context.HoaDons
                .Include(h => h.KhachHang) // Load tên khách hàng
                .Include(h => h.NhanVienThuNgan) // Load tên nhân viên
                .Include(h => h.KhuyenMai) // Truy xuất thêm Khuyến mãi
                .Include(h => h.ChiTietHoaDons) // Load list sản phẩm
                    .ThenInclude(c => c.SanPham) // Load chi tiết từng sản phẩm
                .FirstOrDefaultAsync(m => m.SoHoaDon == id);

            if (hoaDon == null) return NotFound();

            return View(hoaDon);
        }
    }
}