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

        // POST: SanPham/Create
        // Xử lý luồng dữ liệu khi người dùng nhấn nút lưu sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        // BỔ SUNG: Đã thêm SoSeri vào danh sách gán dữ liệu tự động Bind
        public async Task<IActionResult> Create([Bind("MaSanPham,SoSeri,TenSanPham,LoaiSanPham,HangSanXuat,XuatXu,DonViTinh,GiaNhap,GiaBanDeXuat,ThoiHanBaoHanh,MoTa,NguoiLapPhieu,NgayTao")] Sanpham sanPham)
        {
            // Remove các thuộc tính điều hướng ảo để tránh lỗi ModelState ngầm
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
                    // 1. Kiểm tra định dạng số Seri (Chỉ chấp nhận chuỗi chứa đúng 16 ký số)
                    if (!string.IsNullOrEmpty(sanPham.SoSeri) && (sanPham.SoSeri.Length != 16 || !sanPham.SoSeri.All(char.IsDigit)))
                    {
                        ModelState.AddModelError("SoSeri", "Số Seri sản phẩm bắt buộc phải bao gồm đúng 16 ký số liên tiếp (không chứa chữ hay ký tự đặc biệt)!");
                        return View(sanPham);
                    }

                    // 2. Kiểm tra trùng lặp mã sản phẩm trong hệ thống
                    var existingProduct = await _context.SanPhams.FindAsync(sanPham.MaSanPham);
                    if (existingProduct != null)
                    {
                        ModelState.AddModelError("MaSanPham", "Mã sản phẩm này đã tồn tại trên hệ thống!");
                        return View(sanPham);
                    }

                    // Thiết lập thông số mặc định hệ thống
                    sanPham.SoLuongTon = 0;
                    sanPham.TrangThai = "Kích hoạt";

                    // Lưu thực thể vào bảng SANPHAM
                    _context.Add(sanPham);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message);
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