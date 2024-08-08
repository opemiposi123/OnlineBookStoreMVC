using Microsoft.AspNetCore.Mvc;

namespace OnlineBookStoreMVC.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
