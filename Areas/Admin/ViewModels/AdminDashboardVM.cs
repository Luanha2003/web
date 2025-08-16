using System.Collections.Generic;

namespace WEB.Areas.Admin.ViewModels
{
    public class AdminDashboardVM
    {
        public int TotalPosts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalUsers { get; set; }
        public int PostsToday { get; set; }

        public List<string> RecentDates { get; set; } = new();
        public List<int> RecentPostCounts { get; set; } = new();

        public List<string> RoleNames { get; set; } = new();
        public List<int> RoleCounts { get; set; } = new();

        public List<string> CategoryNames { get; set; } = new();
        public List<int> CategoryCounts { get; set; } = new();
    }
}
