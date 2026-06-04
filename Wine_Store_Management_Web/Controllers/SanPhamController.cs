using Microsoft.AspNetCore.Mvc; 
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly QLChilliquerContext _context;

        // Tiêm DbContext vào Controller để tương tác với cơ sở dữ liệu
        public SanPhamController(QLChilliquerContext context)
        {
            _context = context;
        }

        // GET: SanPham/Create
        // Hiển thị giao diện Phiếu thêm sản phẩm mới (KHO_BM1)
        [HttpGet]
        public IActionResult Create()
        {
            var sanPham = new Sanpham
            {
                NgayTao = DateOnly.FromDateTime(DateTime.Now)
            };
            return View(sanPham);
        }

        // POST: SanPham/Create
        // Xử lý luồng dữ liệu khi người dùng nhấn nút lưu sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSanPham,TenSanPham,LoaiSanPham,HangSanXuat,XuatXu,DonViTinh,GiaNhap,GiaBanDeXuat,ThoiHanBaoHanh,MoTa,NguoiLapPhieu,NgayTao")] Sanpham sanPham)
        {
            // Remove navigation properties - chỉ cần FK, không cần object
            ModelState.Remove("NguoiLapPhieuNavigation");
            ModelState.Remove("ChitietHoadons");
            ModelState.Remove("ChitietKiemkes");
            ModelState.Remove("ChitietPhieunhaps");
            ModelState.Remove("TiepnhanBaohanhs");
            ModelState.Remove("TuchoiBaohanhs");
            ModelState.Remove("PhieuBaohanhKhos");

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng lặp mã sản phẩm trong hệ thống
                    var existingProduct = await _context.SanPhams.FindAsync(sanPham.MaSanPham);
                    if (existingProduct != null)
                    {
                        ModelState.AddModelError("MaSanPham", "Mã sản phẩm này đã tồn tại trên hệ thống!");
                        return View(sanPham);
                    }

                    // Thiết lập các thông số mặc định không hiển thị trên biểu mẫu nhập liệu
                    sanPham.SoLuongTon = 0; // Sản phẩm mới tạo mặc định tồn kho bằng 0 (chờ phiếu nhập)
                    sanPham.TrangThai = "Kích hoạt"; // Trạng thái mặc định khi đưa vào kinh doanh

                    // Lưu thực thể vào bảng SANPHAM
                    _context.Add(sanPham);
                    await _context.SaveChangesAsync();

                    // Chuyển hướng về trang danh sách sau khi lưu thành công
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }
            // Trả về view kèm thông báo lỗi nếu dữ liệu không hợp lệ
            return View(sanPham);
        }

        // GET: SanPham/Index
        // Hiển thị danh sách sản phẩm hiện tại trong kho
        public IActionResult Index()
        {
            var danhSachSanPham = _context.SanPhams.ToList();
            return View(danhSachSanPham);
        }
    }
}