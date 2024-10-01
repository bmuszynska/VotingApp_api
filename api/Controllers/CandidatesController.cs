using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class CandidatesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
