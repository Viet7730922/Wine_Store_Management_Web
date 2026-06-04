using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Wine_Store_Management_Web.Data;

namespace Wine_Store_Management_Web.Controllers
{
    public class LogHeThongController : Controller
    {
        private readonly QLChilliquerContext _context;

        public LogHeThongController(QLChilliquerContext context)
        {
            _context = context;
        }

        // GET: LogHeThong/Index (Biểu mẫu QLCH_BM4)
        [HttpGet]
        public async Task<IActionResult> Index(string username, DateTime? tuNgay, DateTime? denNgay)
        {
            // 1. Tạo danh sách Dropdown để chọn nhân viên trên giao diện
            var danhSachNhanVien = await _context.NhanViens
                .Select(n => new SelectListItem
                {
                    Value = n.TenDangNhap,
                    Text = n.HoTen + " (" + n.TenDangNhap + ")"
                }).ToListAsync();
            ViewBag.UserList = danhSachNhanVien;

            // 2. Trả lại giá trị tìm kiếm ra View để giữ nguyên text trên các ô input
            ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");
            ViewBag.SelectedUser = username;

            // 3. Xây dựng câu truy vấn động (IQueryable) để có thể nối thêm các điều kiện lọc
            var query = _context.LogHeThongs.AsQueryable();

            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(l => l.TenTaiKhoan == username);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(l => l.ThoiGian.Date >= tuNgay.Value.Date);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(l => l.ThoiGian.Date <= denNgay.Value.Date);
            }

            // 4. Sắp xếp thứ tự thời gian mới nhất đến cũ nhất theo đúng quy định DFD
            var ketQuaLog = await query.OrderByDescending(l => l.ThoiGian).ToListAsync();

            return View(ketQuaLog);
        }
    }
}