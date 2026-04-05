namespace VMAPP.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using System.Diagnostics;

    using VMAPP.Web.Models;

    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["Title"] = "Privacy Policy";
            ViewData["EffectiveDate"] = DateTime.UtcNow.ToString("MMMM dd, yyyy");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            return (statusCode ?? 500) switch
            {
                400 => View("Error400"),
                403 => View("Error403"),
                404 => View("Error404"),
                _   => View("Error500")
            };
        }
    }
}
