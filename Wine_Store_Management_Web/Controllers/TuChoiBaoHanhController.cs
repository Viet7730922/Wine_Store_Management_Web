using Microsoft.AspNetCore.Mvc; 
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class TuChoiBaoHanhController : Controller
    {
        private readonly QLChilliquerContext _context;

        public TuChoiBaoHanhController(QLChilliquerContext context)
        {
            _context = context;
        }

        // GET: TuChoiBaoHanh/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TuChoiBaoHanh/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NgayLapPhieu,MaKhachHang,MaSanPham,SoSeri,LyDoTuChoi,BangChungDinhKem,ThuNganXacNhan")] TuchoiBaohanh tuChoi)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Lưu phiếu từ chối bảo hành vào hệ thống để lưu trữ lịch sử vi phạm
                    _context.Add(tuChoi);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đã lập và in Phiếu từ chối bảo hành thành công!";
                    return RedirectToAction("Index", "TiepNhanBaoHanh"); // Quay về danh sách bảo hành chung
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lập phiếu từ chối: " + ex.Message);
                }
            }
            return View(tuChoi);
        }
    }
}