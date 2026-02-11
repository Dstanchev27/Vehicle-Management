using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMAPP.Data;
using VMAPP.Web.Models.VehicleServiceCars;

namespace VMAPP.Web.Controllers
{
    public class VehicleServiceCars : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public VehicleServiceCars(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        // GET: /VehicleServiceCars
        // optional q parameter for service name search
        [HttpGet]
        public IActionResult Index(string q)
        {
            // Query services and project related vehicles into ServiceIndexViewModel -> VehicleServiceCarModel
            var query = _dbContext.VehicleServices.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(s => EF.Functions.Like(s.Name, $"%{q}%"));
            }

            var model = query
                .Select(s => new ServiceIndexViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Cars = s.VehicleVehicleServices
                        .Select(vvs => new VehicleServiceCarModel
                        {
                            Id = vvs.Vehicle.VehicleId,
                            VIN = vvs.Vehicle.VIN,
                            Make = vvs.Vehicle.CarBrand,
                            Model = vvs.Vehicle.CarModel,
                            Year = vvs.Vehicle.CreatedOnYear.Year
                        })
                        .ToList()
                })
                .OrderBy(s => s.Name)
                .ToList();

            ViewBag.Query = q ?? string.Empty;
            return View(model);
        }

        [HttpGet]
        public IActionResult AddCar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCar(string make, string model, int year)
        {
            ViewBag.Message = $"Car added: {year} {make} {model}";
            return View();
        }

        [HttpGet]
        public IActionResult EditVehicle()
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
