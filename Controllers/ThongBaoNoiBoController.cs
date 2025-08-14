using Microsoft.AspNetCore.Mvc;

namespace WEB.Controllers
{
    public class ThongBaoNoiBoController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "THÔNG BÁO - BÁO CÁO";
            return View();
        }
    }
}
