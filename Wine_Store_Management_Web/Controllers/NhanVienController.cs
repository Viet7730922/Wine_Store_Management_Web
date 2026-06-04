using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Wine_Store_Management_Web.Data;
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly QLChilliquerContext _context;

        public NhanVienController(QLChilliquerContext context)
        {
            _context = context;
        }

        // GET: NhanVien/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: NhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNhanVien,HoTen,NgaySinh,SoDienThoai,Email,TenDangNhap,MatKhau,VaiTro,TrangThai,NgayLap,NguoiThucHien")] Nhanvien nhanVien)
        {
            ModelState.Remove("NguoiThucHienNavigation");
            ModelState.Remove("InverseNguoiThucHienNavigation");
            ModelState.Remove("BiennhanGiaodiches");
            ModelState.Remove("Hoadons");
            ModelState.Remove("Khuyenmais");
            ModelState.Remove("Kiemkekhos");
            ModelState.Remove("Phieunhaps");
            ModelState.Remove("Sanphams");
            ModelState.Remove("TiepnhanBaohanhs");
            ModelState.Remove("TuchoiBaohanhs");
            ModelState.Remove("PhieuBaohanhKhos");
            if (ModelState.IsValid)
            {
                try
                {
                    // Hệ thống kiểm tra đối chiếu dữ liệu để tránh trùng lặp 
                    var isMaNhanVienExist = await _context.NhanViens.AnyAsync(n => n.MaNhanVien == nhanVien.MaNhanVien);
                    if (isMaNhanVienExist)
                    {
                        ModelState.AddModelError("MaNhanVien", "Mã nhân viên này đã tồn tại trong hệ thống.");
                        return View(nhanVien);
                    }

                    var isTenDangNhapExist = await _context.NhanViens.AnyAsync(n => n.TenDangNhap == nhanVien.TenDangNhap);
                    if (isTenDangNhapExist)
                    {
                        ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã được người khác sử dụng.");
                        return View(nhanVien);
                    }

                    // Thực hiện mã hóa mật khẩu  (Sử dụng SHA256 cơ bản)
                    nhanVien.MatKhau = HashPassword(nhanVien.MatKhau);

                    // Ghi dữ liệu mới vào kho dữ liệu NHANVIEN 
                    _context.Add(nhanVien);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Tạo tài khoản và phân quyền thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi thiết lập tài khoản: " + ex.Message);
                }
            }
            return View(nhanVien);
        }

        // GET: NhanVien/Index
        public IActionResult Index()
        {
            var danhSachNhanVien = _context.NhanViens.ToList();
            return View(danhSachNhanVien);
        }

        // Hàm hỗ trợ mã hóa mật khẩu
        private string HashPassword(string rawPassword)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawPassword));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}