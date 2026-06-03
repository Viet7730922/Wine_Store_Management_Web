using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wine_Store_Management_Web.Data;
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class KhuyenMaiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhuyenMaiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: KhuyenMai/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: KhuyenMai/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaChuongTrinh,TenChuongTrinh,HinhThucApDung,MucGiamGia,DieuKienApDung,TuNgay,DenNgay,NgayDuyet,NguoiPheDuyet")] KhuyenMai khuyenMai)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Kiểm tra logic thời gian cơ bản
                    if (khuyenMai.TuNgay > khuyenMai.DenNgay)
                    {
                        ModelState.AddModelError("DenNgay", "Ngày kết thúc không được nhỏ hơn ngày bắt đầu!");
                        return View(khuyenMai);
                    }

                    // 2. Kiểm tra trùng lặp mã chương trình (Tránh lỗi khóa chính)
                    var isExist = await _context.KhuyenMais.AnyAsync(k => k.MaChuongTrinh == khuyenMai.MaChuongTrinh);
                    if (isExist)
                    {
                        ModelState.AddModelError("MaChuongTrinh", "Mã chương trình ưu đãi này đã tồn tại!");
                        return View(khuyenMai);
                    }

                    // 3. Đối chiếu dữ liệu D3 để đảm bảo không bị xung đột thời gian ưu đãi (DFD 1.3.15 - Bước 3)
                    // Kiểm tra xem có chương trình nào đang chạy chồng chéo thời gian không
                    var isConflict = await _context.KhuyenMais.AnyAsync(k =>
                        (khuyenMai.TuNgay >= k.TuNgay && khuyenMai.TuNgay <= k.DenNgay) ||
                        (khuyenMai.DenNgay >= k.TuNgay && khuyenMai.DenNgay <= k.DenNgay));

                    if (isConflict)
                    {
                        ModelState.AddModelError("", "Cảnh báo: Khoảng thời gian này đang có một chương trình khuyến mãi khác hoạt động. Vui lòng kiểm tra lại để tránh xung đột!");
                        return View(khuyenMai);
                    }

                    // 4. Lưu dữ liệu vào CSDL
                    _context.Add(khuyenMai);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Thiết lập chương trình khuyến mãi thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống khi thiết lập khuyến mãi: " + ex.Message);
                }
            }
            return View(khuyenMai);
        }

        // GET: KhuyenMai/Index
        public IActionResult Index()
        {
            var danhSachKhuyenMai = _context.KhuyenMais.OrderByDescending(k => k.NgayDuyet).ToList();
            return View(danhSachKhuyenMai);
        }
    }
}