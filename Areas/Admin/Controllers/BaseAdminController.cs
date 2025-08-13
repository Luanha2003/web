using Microsoft.AspNetCore.Mvc;

namespace WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaseAdminController : Controller
    {
        protected bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return !string.IsNullOrEmpty(role) && role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        protected IActionResult? OnlyAdmin()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });
            return null;
        }
    }
}
