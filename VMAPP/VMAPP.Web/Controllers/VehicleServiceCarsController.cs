using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Data.Models.Enums;
using VMAPP.Web.Models.ServiceRecordModels;
using VMAPP.Web.Models.VehicleServiceCars;
using VMAPP.Web.Models.VehicleServiceModels;

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
        public async Task<IActionResult> Index(string serviceName = null)
        {
            var model = new ServiceIndexViewModel();

            if (string.IsNullOrEmpty(serviceName))
            {
                return View(model);
            }

            model = await _dbContext.VehicleServices
                .Where(s => s.Name == serviceName)
                .Select(s => new ServiceIndexViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Cars = s.VehicleVehicleServices.Select(vs => new VehicleServiceCarModel
                    {
                        Id = vs.Vehicle.VehicleId,
                        VIN = vs.Vehicle.VIN,
                        Make = vs.Vehicle.CarModel,
                        Model = vs.Vehicle.CarBrand,
                        CreatedOnYear = vs.Vehicle.CreatedOnYear,
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                model = new ServiceIndexViewModel();
            }

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
        public IActionResult AddVehicle(int serviceId, string serviceName)
        {
            var newVehicle = new AddVehicleViewModel()
            {
                ServiceId = serviceId,
                ServiceName = serviceName
            };
            return View(newVehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVehicle(AddVehicleViewModel newVehicle)
        {
            if (!ModelState.IsValid)
            {
                return View(newVehicle);
            }

            try
            {
                var dbVehicle = new Vehicle()
                {
                    VIN = newVehicle.VIN,
                    CarBrand = newVehicle.CarBrand,
                    CarModel = newVehicle.CarModel,
                    CreatedOnYear = newVehicle.CreatedOnYear,
                    Color = newVehicle.Color,
                    VehicleType = newVehicle.VehicleType
                };

                var vehicleServiceVehicle = new VehicleVehicleService()
                {
                    VehicleServiceId = newVehicle.ServiceId,
                    Vehicle = dbVehicle
                };

                dbVehicle.VehicleVehicleServices.Add(vehicleServiceVehicle);

                await _dbContext.Vehicles.AddAsync(dbVehicle);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { serviceName = newVehicle.ServiceName });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save vehicle to the database. Please try again.");
                return View(newVehicle);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return View(newVehicle);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditVehicle(int id, int serviceId, string serviceName)
        {
            var entity = await _dbContext.Vehicles
                .AsNoTracking()
                .Include(v => v.VehicleVehicleServices)
                .FirstOrDefaultAsync(v => v.VehicleId == id)!;

            if (entity == null)
            {
                return NotFound();
            }

            var model = new EditVehicleViewModel()
            {
                ServiceId = serviceId,
                VehicleId = entity.VehicleId,
                VIN = entity.VIN,
                CarBrand = entity.CarBrand,
                CarModel = entity.CarModel,
                CreatedOnYear = entity.CreatedOnYear,
                Color = entity.Color,
                VehicleType = entity.VehicleType,
                ServiceName = serviceName
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVehicle(EditVehicleViewModel model, int id)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dbVehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == id);

            if (dbVehicle == null)
            {
                return NotFound();
            }

            dbVehicle.VIN = model.VIN;
            dbVehicle.CarBrand = model.CarBrand;
            dbVehicle.CarModel = model.CarModel;
            dbVehicle.CreatedOnYear = model.CreatedOnYear;
            dbVehicle.Color = model.Color;
            dbVehicle.VehicleType = model.VehicleType;

            _dbContext.Vehicles.Update(dbVehicle);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { serviceName = model.ServiceName });
        }

        //[HttpGet]
        //public async Task<IActionResult> AddRecord(int id, int vehicleId)
        //{
        //    var record = await _dbContext.ServiceRecords
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(v => v.VehicleId == id);

        //    if (record == null)
        //    {
        //        return NotFound();
        //    }

        //    var model = new AddRecordViewModel()
        //    {
        //        Description = record.Description,
        //        Cost = record.Cost
        //    };

        //    return View(model);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRecord(AddRecordViewModel model, int id, int vehicleId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var dbRecord = new ServiceRecord()
            {
                Description = model.Description,
                Cost = model.Cost,
                VehicleId = vehicleId
            };

            await _dbContext.ServiceRecords.AddAsync(dbRecord);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(EditVehicle), new {vehicleId = model.VehicleId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteVehicle(EditVehicleViewModel model,int id)
        {
            var vehicle = _dbContext.Vehicles.FirstOrDefault(v => v.VehicleId == id);
            if (vehicle == null)
            {
                return NotFound();
            }
            try
            {
                _dbContext.Vehicles.Remove(vehicle);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index), new { serviceName = model.ServiceName });
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
