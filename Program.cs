using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WEB.Models; // NewsDbContext, NguoiDung

var builder = WebApplication.CreateBuilder(args);

// ========== Services ==========
builder.Services.AddDbContext<NewsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // Nếu deploy HTTPS, có thể bật:
    // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ========== Pipeline ==========
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// BẬT SESSION trước Authorization
app.UseSession();
app.UseAuthorization();

// ========== Routes ==========
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// ========== Seed Admin (tùy chọn) ==========
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NewsDbContext>();

    try
    {
        db.Database.Migrate();
    }
    catch
    {
        // Bỏ qua lỗi migration khi khởi động (tùy chọn)
    }

    if (!db.NguoiDungs.Any(u => u.Email == "admin@site.com"))
    {
        db.NguoiDungs.Add(new NguoiDung
        {
            HoTen = "Quản trị",
            Email = "admin@site.com",
            MatKhau = "123456", // CHỈ DEMO — production phải hash mật khẩu
            VaiTro = "Admin",
            NgayTao = DateTime.Now
        });
        db.SaveChanges();
    }
}

app.Run();
    