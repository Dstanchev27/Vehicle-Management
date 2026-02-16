using Microsoft.AspNetCore.Mvc;

using VMAPP.Data.Models.Enums;
using VMAPP.Services.DTOs;
using VMAPP.Services.Interfaces;
using VMAPP.Web.Models.ServiceRecordModels;
using VMAPP.Web.Models.VehicleServiceCars;
using VMAPP.Web.Models.VehicleServiceModels;

namespace VMAPP.Web.Controllers
{
    public class VehicleServiceCarsController : Controller
    {
        private readonly IVSCarsService _vsCarsService;

        public VehicleServiceCarsController(IVSCarsService vsCarsService)
        {
            _vsCarsService = vsCarsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string serviceName = null)
        {
            var model = new ServiceIndexViewModel();

            if (string.IsNullOrEmpty(serviceName))
            {
                return View(model);
            }

            var serviceDto = await _vsCarsService.GetServiceWithVehiclesByNameAsync(serviceName);
            if (serviceDto == null)
            {
                return View(model);
            }

            model.Id = serviceDto.Id;
            model.Name = serviceDto.Name;
            model.Cars = serviceDto.Vehicles
                .Select(v => new VehicleServiceCarModel
                {
                    Id = v.Id,
                    VIN = v.VIN,
                    Make = v.CarBrand,
                    Model = v.CarModel,
                    CreatedOnYear = v.CreatedOnYear
                })
                .ToList();

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
                var vehicleDto = new VehicleDto
                {
                    VIN = newVehicle.VIN,
                    CarBrand = newVehicle.CarBrand,
                    CarModel = newVehicle.CarModel,
                    CreatedOnYear = newVehicle.CreatedOnYear,
                    Color = newVehicle.Color,
                    VehicleType = (int)newVehicle.VehicleType
                };

                await _vsCarsService.AddVehicleToServiceAsync(newVehicle.ServiceId, vehicleDto);
                return RedirectToAction(nameof(Index), new { serviceName = newVehicle.ServiceName });
            }
            catch
            {
                ModelState.AddModelError("", "Unable to save vehicle to the database. Please try again.");
                return View(newVehicle);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditVehicle(int id, int serviceId, string serviceName)
        {
            var details = await _vsCarsService.GetVehicleDetailsAsync(id);
            if (details == null)
            {
                return NotFound();
            }

            var v = details.Vehicle;

            var model = new EditVehicleViewModel()
            {
                ServiceId = serviceId,
                VehicleId = v.Id,
                VIN = v.VIN,
                CarBrand = v.CarBrand,
                CarModel = v.CarModel,
                CreatedOnYear = v.CreatedOnYear,
                Color = v.Color,
                VehicleType = (VehicleType)v.VehicleType,
                ServiceName = serviceName,
                ServiceRecords = details.ServiceRecords
                    .Select(r => new AddRecordViewModel
                    {
                        RecordId = r.Id,
                        VehicleId = r.VehicleId,
                        ServiceId = serviceId,
                        ServiceName = serviceName,
                        ServiceDate = r.ServiceDate,
                        RecordCost = r.Cost,
                        Description = r.Description
                    })
                    .ToList()
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

            var vehicleDto = new VehicleDto
            {
                Id = id,
                VIN = model.VIN,
                CarBrand = model.CarBrand,
                CarModel = model.CarModel,
                CreatedOnYear = model.CreatedOnYear ?? DateTime.Now.Year,
                Color = model.Color,
                VehicleType = (int)model.VehicleType
            };

            var updated = await _vsCarsService.UpdateVehicleAsync(vehicleDto);
            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index), new { serviceName = model.ServiceName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRecord(AddRecordViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(EditVehicle), new { id = model.VehicleId, serviceId = model.ServiceId, serviceName = model.ServiceName });
            }

            var recordDto = new ServiceRecordDto
            {
                VehicleId = model.VehicleId,
                ServiceDate = model.ServiceDate,
                Cost = model.RecordCost,
                Description = model.Description
            };

            await _vsCarsService.AddServiceRecordAsync(recordDto);

            return RedirectToAction(nameof(EditVehicle), new { id = model.VehicleId, serviceId = model.ServiceId, serviceName = model.ServiceName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRecord(int id, AddRecordViewModel model)
        {
            if (model == null || id == 0)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(EditVehicle), new { id = model.VehicleId, serviceId = model.ServiceId, serviceName = model.ServiceName });
            }

            var recordDto = new ServiceRecordDto
            {
                Id = id,
                VehicleId = model.VehicleId,
                ServiceDate = model.ServiceDate,
                Cost = model.RecordCost,
                Description = model.Description
            };

            var updated = await _vsCarsService.UpdateServiceRecordAsync(recordDto);
            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(EditVehicle), new { id = model.VehicleId, serviceId = model.ServiceId, serviceName = model.ServiceName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRecord(int id, int vehicleId, string serviceName)
        {
            await _vsCarsService.DeleteServiceRecordAsync(id);
            return RedirectToAction(nameof(EditVehicle), new { id = vehicleId, serviceId = (int?)null, serviceName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVehicle(EditVehicleViewModel model, int id)
        {
            await _vsCarsService.DeleteVehicleAsync(id);
            return RedirectToAction(nameof(Index), new { serviceName = model.ServiceName });
        }
    }
}
