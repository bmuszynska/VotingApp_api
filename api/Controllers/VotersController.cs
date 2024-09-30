using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class VotersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
