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
            return View();
        }

        // POST: HoaDon/Create
        // Xử lý lưu hóa đơn và toàn bộ danh sách chi tiết hàng hóa đi kèm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoaDon hoaDon, List<ChiTietHoaDon> ChiTietHoaDons)
        {
            // Kiểm tra tính hợp lệ của thông tin hóa đơn tổng và danh sách sản phẩm mua
            if (ModelState.IsValid && ChiTietHoaDons != null && ChiTietHoaDons.Count > 0)
            {
                // Sử dụng Transaction để đảm bảo tính toàn vẹn dữ liệu (nếu lỗi một bước sẽ rollback toàn bộ)
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Gán cố định các thông tin chung của cửa hàng theo đúng quy định biểu mẫu
                    hoaDon.DonViBanHang = "Cửa Hàng Rượu Chilliquer";
                    hoaDon.DiaChiBanHang = "138 Mai Xuân Thưởng, Nha Trang, Khánh Hòa";
                    hoaDon.DienThoaiBanHang = "0905XXXXXX";

                    decimal tongTienHoaDon = 0;

                    // 2. Thêm bản ghi hóa đơn gốc (Master) vào trước để sinh khóa ngoại
                    _context.HoaDons.Add(hoaDon);
                    await _context.SaveChangesAsync();

                    // 3. Vòng lặp xử lý từng dòng hàng hóa trong Chi tiết hóa đơn (Detail)
                    foreach (var chiTiet in ChiTietHoaDons)
                    {
                        // Gán mối quan hệ khóa ngoại kết nối với hóa đơn vừa tạo
                        chiTiet.SoHoaDon = hoaDon.SoHoaDon;
                        _context.ChiTietHoaDons.Add(chiTiet);

                        // Nghiệp vụ đối chiếu và trừ kho sản phẩm (DFD Chức năng 1.3.3 / 1.3.8)
                        var sanPham = await _context.SanPhams.FindAsync(chiTiet.MaSanPham);
                        if (sanPham == null)
                        {
                            ModelState.AddModelError("", $"Mã sản phẩm {chiTiet.MaSanPham} không tồn tại trên hệ thống!");
                            return View(hoaDon);
                        }

                        // Kiểm tra nếu số lượng tồn trong kho nhỏ hơn số lượng khách mua
                        if (sanPham.SoLuongTon < chiTiet.SoLuong)
                        {
                            ModelState.AddModelError("", $"Sản phẩm '{sanPham.TenSanPham}' không đủ số lượng để xuất kho! (Tồn hiện tại: {sanPham.SoLuongTon})");
                            return View(hoaDon);
                        }

                        // Thực hiện trừ tồn kho vật lý của sản phẩm
                        sanPham.SoLuongTon -= chiTiet.SoLuong;
                        _context.Update(sanPham);

                        // Tính tổng tiền lũy tiến = Số lượng x Đơn giá từng sản phẩm
                        tongTienHoaDon += (chiTiet.SoLuong * chiTiet.DonGia);
                    }

                    // 4. Nghiệp vụ xử lý thẻ khách hàng thân thiết và cộng điểm tích lũy (DFD Chức năng 1.3.8)
                    if (!string.IsNullOrEmpty(hoaDon.MaKhachHang))
                    {
                        var khachHang = await _context.KhachHangs.FindAsync(hoaDon.MaKhachHang);
                        if (khachHang != null)
                        {
                            // Công thức tính điểm quy định: Điểm = Tổng tiền / 500 (Làm tròn xuống)
                            int diemTichLuyMoi = (int)Math.Floor(tongTienHoaDon / 500);
                            khachHang.DiemTichLuy += diemTichLuyMoi;
                            _context.Update(khachHang);
                        }
                    }

                    // Lưu toàn bộ các thay đổi (Chi tiết hóa đơn, Tồn kho sản phẩm, Điểm khách hàng) vào CSDL
                    await _context.SaveChangesAsync();

                    // Xác nhận hoàn tất giao dịch an toàn
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Hủy bỏ mọi thao tác lưu trữ trước đó nếu phát sinh lỗi giữa chừng
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Lỗi hệ thống khi tạo hóa đơn: " + ex.Message);
                }
            }

            // Trả lại giao diện hóa đơn kèm dữ liệu đã nhập nếu quá trình xử lý thất bại
            return View(hoaDon);
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
    }
}