using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NgayIn,GioIn,LoaiBienNhan,MaDoiChieuHeThong,TenKhachHang,SoDienThoai,ChiTietSanPham,TongGiaTriDoiChieu,ThuNganXuatPhieu")] BiennhanGiaodich bienNhan)
        {
            ModelState.Remove("ThuNganXuatPhieuNavigation");
            if (ModelState.IsValid)
            {
                try
                {
                    // Lưu lại bản sao biên nhận đã in để đối chiếu về sau
                    _context.Add(bienNhan);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đã xuất lệnh in biên nhận giao dịch!";
                    // Quay về trang chủ dashboard hoặc danh sách tùy thiết kế
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi in biên nhận: " + ex.Message);
                }
            }
            return View(bienNhan);
        }
    }
}