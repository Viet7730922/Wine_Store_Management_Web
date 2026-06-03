using Microsoft.EntityFrameworkCore;
using Wine_Store_Management_Web.Data;
using Wine_Store_Management_Web.Models;

var builder = WebApplication.CreateBuilder(args);

// LƯU Ý KỸ THUẬT: Đảm bảo bạn đã cài đặt các gói NuGet sau thông qua Package Manager Console:
// Install-Package Microsoft.EntityFrameworkCore.SqlServer
// Install-Package Microsoft.EntityFrameworkCore.Tools

// 1. Thêm dịch vụ MVC vào vùng chứa (Container)
builder.Services.AddControllersWithViews();

// 2. Đăng ký kết nối Cơ sở dữ liệu SQL Server với Connection String "DefaultConnection"
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Cấu hình đường ống xử lý yêu cầu HTTP (HTTP request pipeline).
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Giá trị HSTS mặc định là 30 ngày. Bạn có thể thay đổi giá trị này cho phù hợp thực tế.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

// Ánh xạ tài nguyên tĩnh (Tương thích với .NET 8/9 MVC)
app.MapStaticAssets();

// Cấu hình định tuyến mặc định của ứng dụng MVC (Mặc định sẽ hướng vào HomeController và Action Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();