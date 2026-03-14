namespace VMAPP.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using System.Diagnostics;

    using VMAPP.Web.Models;
    public class VehicleController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
