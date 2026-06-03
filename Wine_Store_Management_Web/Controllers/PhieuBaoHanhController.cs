using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wine_Store_Management_Web.Data;
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class PhieuBaoHanhKhoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhieuBaoHanhKhoController(ApplicationDbContext context)
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
        public async Task<IActionResult> Create([Bind("MaPhieuBH,MaKhachHang,MaSanPham,SoSeri,NoiDungLoiGhiNhan,NgayTiepNhanHeThong,ThoiGianXuLyDuKien,NgayHenTra,NhanVienThucHien")] PhieuBaoHanhKho phieuBaoHanh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng lặp mã phiếu bảo hành
                    var isExist = await _context.PhieuBaoHanhKhos.AnyAsync(p => p.MaPhieuBH == phieuBaoHanh.MaPhieuBH);
                    if (isExist)
                    {
                        ModelState.AddModelError("MaPhieuBH", "Mã phiếu bảo hành này đã tồn tại trên hệ thống!");
                        return View(phieuBaoHanh);
                    }

                    _context.Add(phieuBaoHanh);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Kết xuất phiếu bảo hành thành công!";
                    // Thường in xong sẽ quay về giao diện lịch sử bảo hành
                    return RedirectToAction("Index", "TiepNhanBaoHanh");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống khi kết xuất phiếu bảo hành: " + ex.Message);
                }
            }
            return View(phieuBaoHanh);
        }
    }
}