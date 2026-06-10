using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wine_Store_Management_Web.Data;
using Wine_Store_Management_Web.Models;

namespace Wine_Store_Management_Web.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly QLChilliquerContext _context;

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
        // GET: SanPham/Details/MãSP
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var sanPham = await _context.SanPhams
                .Include(s => s.NguoiLapPhieuNavigation)
                .FirstOrDefaultAsync(m => m.MaSanPham == id);

            if (sanPham == null) return NotFound();

            return View(sanPham);
        }

        // POST: SanPham/Create
        // Xử lý luồng dữ liệu khi người dùng nhấn nút lưu sản phẩm
        // POST: SanPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSanPham,SoSeri,TenSanPham,LoaiSanPham,HangSanXuat,XuatXu,DonViTinh,GiaNhap,GiaBanDeXuat,ThoiHanBaoHanh,MoTa,NguoiLapPhieu,NgayTao")] Sanpham sanPham)
        {
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
                    if (!string.IsNullOrEmpty(sanPham.SoSeri) && (sanPham.SoSeri.Length != 16 || !sanPham.SoSeri.All(char.IsDigit)))
                    {
                        ModelState.AddModelError("SoSeri", "Số Seri sản phẩm phải đúng 16 ký số!");
                        return View(sanPham);
                    }

                    var existingProduct = await _context.SanPhams.FindAsync(sanPham.MaSanPham);
                    if (existingProduct != null)
                    {
                        ModelState.AddModelError("MaSanPham", "Mã sản phẩm này đã tồn tại!");
                        return View(sanPham);
                    }

                    sanPham.SoLuongTon = 0;
                    sanPham.TrangThai = "Kích hoạt";

                    _context.Add(sanPham);
                    await _context.SaveChangesAsync();

                    // Chuyển sang trang xem phiếu in sau khi lưu thành công
                    TempData["SuccessMessage"] = "Thêm sản phẩm mới thành công!";
                    return RedirectToAction(nameof(Details), new { id = sanPham.MaSanPham });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu: " + ex.Message);
                }
            }

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