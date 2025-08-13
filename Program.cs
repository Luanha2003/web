using Microsoft.EntityFrameworkCore;
using WEB.Models; // NewsDbContext

var builder = WebApplication.CreateBuilder(args);

// ========== Services ==========
builder.Services.AddDbContext<NewsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session cần cache phân tán (in-memory) + cấu hình cookie
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // phiên hết hạn sau 30'
    options.Cookie.HttpOnly = true;                 // tăng bảo mật
    options.Cookie.IsEssential = true;              // cần thiết (GDPR)
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ========== Middleware Pipeline ==========
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// BẬT SESSION trước Authorization và trước MapControllerRoute
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
