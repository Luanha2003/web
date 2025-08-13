using Microsoft.EntityFrameworkCore;
using WEB.Models; // thêm để nhận NewsDbContext
namespace WEB.Models
{
    public class NewsDbContext : DbContext
    {
        public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options) { }

        public DbSet<ChuyenMuc> ChuyenMucs { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<BaiViet> BaiViets { get; set; }
        public DbSet<BinhLuan> BinhLuans { get; set; }
    }
}
