using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMAPP.Data;
using VMAPP.Web.Models.VehicleServiceCars;

namespace VMAPP.Web.Controllers
{
    public class VehicleServiceCarsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public VehicleServiceCarsController(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index(string serviceName = null)
        {
            var model = new ServiceIndexViewModel();

            if (string.IsNullOrEmpty(serviceName))
            {
                return View(model);
            }

            model = _dbContext.VehicleServices
                .Where(s => s.Name == serviceName)
                .Select(s => new ServiceIndexViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Cars = s.VehicleVehicleServices.Select(vs => new VehicleServiceCarModel
                    {
                        Id = vs.Vehicle.VehicleId,
                        Make = vs.Vehicle.CarModel,
                        Model = vs.Vehicle.CarBrand,
                    }).ToList()
                })
                .FirstOrDefault();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FilterService(ServiceIndexViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Name))
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index), new { serviceName = model.Name });
        }

        [HttpGet]
        public IActionResult AddVehicle()
        {
            var newVehicle = new AddVehicleViewModel();
            return View(newVehicle);
        }

        [HttpPost]
        public IActionResult AddVehicle(string make, string model, int year)
        {
            ViewBag.Message = $"Car added: {year} {make} {model}";
            return View();
        }

        [HttpGet]
        public IActionResult EditVehicles()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditVehicle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteVehicle()
        {
            return View();
        }
    }
}
