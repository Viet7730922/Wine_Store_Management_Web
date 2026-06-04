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
    public class HoaDonController : Controller
    {
        private readonly QLChilliquerContext _context;

        public HoaDonController(QLChilliquerContext context)
        {
            _context = context;
        }

        // GET: HoaDon/Create
        // Trả về biểu mẫu Hóa đơn bán hàng trống (BH_BM1)
        [HttpGet]
        public IActionResult Create()
        {
            // 1. Nạp dữ liệu các danh sách ra View
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            ViewBag.SanPhams = _context.SanPhams.Where(sp => sp.SoLuongTon > 0).ToList();
            ViewBag.KhuyenMais = _context.KhuyenMais.ToList();

            // =====================================================================
            // 2. THUẬT TOÁN: TỰ ĐỘNG ĐIỀN SỐ HÓA ĐƠN (Lấp đầy khoảng trống)
            // =====================================================================

            // Lấy toàn bộ mã hóa đơn bắt đầu bằng chữ "HD" trong DB
            var danhSachMaHD = _context.HoaDons
                .Where(h => h.SoHoaDon.StartsWith("HD"))
                .Select(h => h.SoHoaDon)
                .ToList();

            int soMoi = 1; // Mặc định bắt đầu từ 1 nếu chưa có Hóa đơn nào

            if (danhSachMaHD.Any())
            {
                // Bóc tách chữ "HD" (Lấy từ ký tự thứ 2 trở đi), chuyển thành số và sắp xếp tăng dần
                var cacSoHienTai = danhSachMaHD
                    .Select(m => {
                        int.TryParse(m.Substring(2), out int so);
                        return so;
                    })
                    .Where(s => s > 0)
                    .OrderBy(s => s)
                    .ToList();

                // Dò tìm số nhỏ nhất bị khuyết trong dãy (Ví dụ: có 1, 3, 4 -> sẽ lấy số 2)
                for (int i = 1; i <= cacSoHienTai.Count + 1; i++)
                {
                    if (!cacSoHienTai.Contains(i))
                    {
                        soMoi = i;
                        break;
                    }
                }
            }

            // Định dạng lại thành chuỗi (VD: soMoi = 2 -> ToString("D2") sẽ biến thành "02" -> "HD02")
            string maHoaDonTuDong = "HD" + soMoi.ToString("D2");

            // 3. Khởi tạo sẵn Model ném ra View để nó tự điền vào thẻ <input>
            var hoaDon = new Hoadon
            {
                SoHoaDon = maHoaDonTuDong,
                NgayHoaDon = DateOnly.FromDateTime(DateTime.Now) // Tiện thể tự điền luôn ngày hôm nay
            };

            return View(hoaDon);
        }

        // POST: HoaDon/Create
        // Xử lý lưu hóa đơn và toàn bộ danh sách chi tiết hàng hóa đi kèm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hoadon hoaDon, List<ChitietHoadon> ChiTietHoaDons)
        {
            ModelState.Remove("ThuNgan"); // Xóa validation khóa ngoại (được gán bằng NV01 ở dưới)
            ModelState.Remove("MaKhachHangNavigation");
            ModelState.Remove("MaKhuyenMaiNavigation");
            ModelState.Remove("ThuNganNavigation");
            ModelState.Remove("ChitietHoadons"); // Xóa thuộc tính list tự động sinh

            for (int i = 0; i < ChiTietHoaDons.Count; i++)
            {
                ModelState.Remove($"ChiTietHoaDons[{i}].SoHoaDon"); // Được gán sau khi lưu hóa đơn
                ModelState.Remove($"ChiTietHoaDons[{i}].SoHoaDonNavigation");
                ModelState.Remove($"ChiTietHoaDons[{i}].MaSanPhamNavigation");
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

                    // Xóa dữ liệu dư thừa do Model Binder tạo ra
                    if (hoaDon.ChitietHoadons != null && hoaDon.ChitietHoadons.Any())
                    {
                        hoaDon.ChitietHoadons.Clear();
                    }

                    // 2. Thêm Hóa đơn gốc vào trước để lấy khóa chính
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

                        // Trừ tồn kho (Cơ chế D4)
                        sanPham.SoLuongTon -= chiTiet.SoLuong;
                        _context.Update(sanPham);

                        tongTienHang += (chiTiet.SoLuong * chiTiet.DonGia);
                    }

                    decimal giamGiaKhuyenMai = 0;
                    if (!string.IsNullOrEmpty(hoaDon.MaKhuyenMai))
                    {
                        var km = await _context.KhuyenMais.FindAsync(hoaDon.MaKhuyenMai);
                        if (km != null && !string.IsNullOrEmpty(km.MucGiamGia))
                        {
                            string numberOnly = new string(km.MucGiamGia.Where(char.IsDigit).ToArray());
                            if (decimal.TryParse(numberOnly, out decimal mucGiam))
                            {
                                giamGiaKhuyenMai = km.MucGiamGia.Contains("%") ? tongTienHang * (mucGiam / 100) : mucGiam;
                            }
                        }
                    }


                    // Tổng tiền = (Đơn giá * Số lượng) - Giảm giá KM (Không dùng điểm tích lũy nữa)
                    decimal tongThanhToan = tongTienHang - giamGiaKhuyenMai;
                    if (tongThanhToan < 0) tongThanhToan = 0;

                    // XỬ LÝ CỘNG ĐIỂM MỚI (Tỷ lệ: 5000 VNĐ = 1 Điểm)
                    if (!string.IsNullOrEmpty(hoaDon.MaKhachHang))
                    {
                        var khachHang = await _context.KhachHangs.FindAsync(hoaDon.MaKhachHang);
                        if (khachHang != null)
                        {
                            // Tỷ lệ mới: Lấy tổng thanh toán chia cho 5000
                            int diemTichLuyMoi = (int)Math.Floor(tongThanhToan / 5000);
                            khachHang.DiemTichLuy += diemTichLuyMoi;
                            _context.Update(khachHang);
                        }
                    }

                    // Cập nhật trạng thái vào ghi chú
                    string trangThaiD4 = "Trạng thái: Đã thanh toán";
                    hoaDon.GhiChu = string.IsNullOrEmpty(hoaDon.GhiChu) ? trangThaiD4 : hoaDon.GhiChu + " | " + trangThaiD4;

                    _context.HoaDons.Update(hoaDon);

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
            ViewBag.KhuyenMais = _context.KhuyenMais.ToList();
        }

        // GET: HoaDon/Index
        // Hiển thị lịch sử danh sách hóa đơn bán hàng
        public IActionResult Index()
        {
            var danhSachHoaDon = _context.HoaDons
                .Include(h => h.MaKhachHangNavigation) // Đã sửa tên Navigation
                .OrderByDescending(h => h.NgayHoaDon)
                .ToList();
            return View(danhSachHoaDon);
        }

        // GET: HoaDon/Details/5
        // Hiển thị chi tiết mẫu in hóa đơn (D5)
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaKhachHangNavigation)
                .Include(h => h.ThuNganNavigation)
                .Include(h => h.MaKhuyenMaiNavigation) // Đã sửa lại đúng tên
                .Include(h => h.ChitietHoadons)
                    .ThenInclude(c => c.MaSanPhamNavigation)
                .FirstOrDefaultAsync(m => m.SoHoaDon == id);

            if (hoaDon == null) return NotFound();

            return View(hoaDon);
        }
    }
}