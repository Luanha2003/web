using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WEB.Models;
using WEB.Areas.Admin.ViewModels;

namespace WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : BaseAdminController
    {
        private readonly NewsDbContext _db;
        public DashboardController(NewsDbContext db) { _db = db; }

        public IActionResult Index()
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var vm = new AdminDashboardVM();

            // ===== Tổng quan
            vm.TotalPosts      = _db.BaiViets.Count();
            vm.TotalCategories = _db.ChuyenMucs.Count();
            vm.TotalUsers      = _db.NguoiDungs.Count();

            var today = DateTime.Today;
            vm.PostsToday = _db.BaiViets.Count(b => b.NgayDang.Date == today);

            // ===== Bài viết 7 ngày gần nhất
            var start = today.AddDays(-6);
            var last7 = _db.BaiViets
                           .AsNoTracking()
                           .Where(b => b.NgayDang.Date >= start)
                           .ToList()                               // tránh lỗi translate .Date
                           .Select(b => new { Day = b.NgayDang.Date })
                           .ToList();

            var byDay = last7.GroupBy(x => x.Day)
                             .ToDictionary(g => g.Key, g => g.Count());

            for (int i = 0; i < 7; i++)
            {
                var d = start.AddDays(i).Date;
                vm.RecentDates.Add(d.ToString("dd/MM"));
                vm.RecentPostCounts.Add(byDay.TryGetValue(d, out var c) ? c : 0);
            }

            // ===== Phân bổ vai trò
            var roles = _db.NguoiDungs.AsNoTracking()
                          .GroupBy(u => string.IsNullOrWhiteSpace(u.VaiTro) ? "Khác" : u.VaiTro)
                          .Select(g => new { Role = g.Key, Count = g.Count() })
                          .ToList();
            foreach (var r in roles)
            {
                vm.RoleNames.Add(r.Role);
                vm.RoleCounts.Add(r.Count);
            }

            // ===== Bài viết theo chuyên mục
            var cats = _db.ChuyenMucs.AsNoTracking()
                        .ToDictionary(c => c.ChuyenMucID, c => c.TenChuyenMuc);

            var catAgg = _db.BaiViets.AsNoTracking()
                          .GroupBy(b => b.ChuyenMucID)
                          .Select(g => new { ChuyenMucID = g.Key, Count = g.Count() })
                          .ToList();

            foreach (var kv in catAgg)
            {
                vm.CategoryNames.Add(cats.TryGetValue(kv.ChuyenMucID, out var name) ? name : "Khác");
                vm.CategoryCounts.Add(kv.Count);
            }

            return View(vm);
        }
    }
}
