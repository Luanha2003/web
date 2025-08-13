using Microsoft.EntityFrameworkCore;
using WEB.Models; // NewsDbContext

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
app.UseSession(); // BẬT SESSION trước Authorization
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

// === Optional: Seed an Admin account (runs on startup) ===
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NewsDbContext>();
    try { db.Database.Migrate(); } catch { /* ignore */ }
    if (!db.NguoiDungs.Any(u => u.Email == "admin@site.com"))
    {
        db.NguoiDungs.Add(new NguoiDung {
            HoTen = "Quản trị",
            Email = "admin@site.com",
            MatKhau = "123456", // DEMO ONLY
            VaiTro = "Admin",
            NgayTao = DateTime.Now
        });
        db.SaveChanges();
    }
}

app.Run();
