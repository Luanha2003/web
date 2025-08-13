using Microsoft.AspNetCore.Mvc;

namespace WEB.Areas.Admin.Controllers
{
    public class DashboardController : BaseAdminController
    {
        public IActionResult Index()
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;
            return View();
        }
    }
}
