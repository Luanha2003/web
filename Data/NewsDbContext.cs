using Microsoft.EntityFrameworkCore;

namespace WEB.Models
{
    public class NewsDbContext : DbContext
    {
        public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options) { }

        public DbSet<BaiViet> BaiViets { get; set; }
        public DbSet<ChuyenMuc> ChuyenMucs { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<BinhLuan> BinhLuans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BaiViet>().Property(x => x.HomeCol1).HasDefaultValue(false);
            modelBuilder.Entity<BaiViet>().Property(x => x.HomeCol2).HasDefaultValue(false);
            modelBuilder.Entity<BaiViet>().Property(x => x.HomeCol3).HasDefaultValue(false);
        }
    }
}
