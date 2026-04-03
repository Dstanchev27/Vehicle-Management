using Microsoft.AspNetCore.Mvc;
using VMAPP.Services.DTOs;
using VMAPP.Services.DTOs.VehicleServiceDTOs;
using VMAPP.Services.Interfaces;
using VMAPP.Web.Models.VehicleServiceModels;
using VMAPP.Web.Models.VehicleServiceCars;
using VMAPP.Web.Models.ServiceRecordModels;

namespace VMAPP.Web.Controllers
{
    public class VehicleServicesController : Controller
    {
        private readonly IVSManagementService vsManagementService;
        private readonly IVSCarsService vsCarsService;

        public VehicleServicesController(IVSManagementService vsManagementService, IVSCarsService vsCarsService)
        {
            this.vsManagementService = vsManagementService;
            this.vsCarsService = vsCarsService;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await vsManagementService.GetAllAsync();

            var services = dtos
                .Select(s => new VehicleServiceViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    CreatedOn = s.CreatedOn,
                    City = s.City,
                    Address = s.Address,
                    Email = s.Email,
                    Phone = s.Phone
                })
                .ToList();

            return View(services);
        }

        [HttpGet]
        public IActionResult AddService()
        {
            return View(new AddServiceViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddService(AddServiceViewModel newService)
        {
            if (!ModelState.IsValid)
            {
                return View(newService);
            }
            try
            {
                var dto = new VehicleServiceDto
                {
                    Name = newService.Name,
                    City = newService.City,
                    Address = newService.Address,
                    Email = newService.Email,
                    Phone = newService.Phone,
                    Description = newService.Description,
                    CreatedOn = DateTime.UtcNow
                };

                await vsManagementService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var dto = await vsManagementService.GetVehiclesServiceDetailsByIdAsync(id);

            if (dto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new VehicleServiceDetailsViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                City = dto.City,
                Address = dto.Address,
                Email = dto.Email,
                Phone = dto.Phone,
                Description = dto.Description,
                CreatedOn = dto.CreatedOn,
                Vehicles = dto.Vehicles
                    .Select(v => new VehicleRowViewModel
                    {
                        Id = v.Id,
                        VIN = v.VIN,
                        CarBrand = v.CarBrand,
                        CarModel = v.CarModel,
                        CreatedOnYear = v.CreatedOnYear,
                        Color = v.Color,
                        VehicleType = v.VehicleType
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SearchVehicleByVin(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin))
            {
                return Json(new { found = false, message = "Please enter a VIN to search." });
            }

            var vehicle = await vsManagementService.GetVehicleByVinAsync(vin.Trim().ToUpperInvariant());

            if (vehicle == null)
            {
                return Json(new { found = false, message = $"No vehicle found with VIN \"{vin.Trim().ToUpperInvariant()}\"." });
            }

            return Json(new
            {
                found = true,
                vehicle = new
                {
                    id = vehicle.Id,
                    vin = vehicle.VIN,
                    carBrand = vehicle.CarBrand,
                    carModel = vehicle.CarModel,
                    createdOnYear = vehicle.CreatedOnYear,
                    color = vehicle.Color,
                    vehicleType = vehicle.VehicleType.ToString()
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicleToService([FromBody] AddVehicleToServiceRequest request)
        {
            var success = await vsManagementService.AddVehicleToServiceAsync(request.ServiceId, request.VehicleId);

            if (!success)
            {
                return Json(new { success = false, message = "This vehicle is already assigned to the service." });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveVehicleFromService([FromBody] AddVehicleToServiceRequest request)
        {
            var (success, message) = await vsManagementService.RemoveVehicleFromServiceAsync(
                request.ServiceId, request.VehicleId);

            if (!success)
            {
                return Json(new { success = false, message });
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> EditService(int id)
        {
            var dto = await vsManagementService.GetByIdAsync(id);

            if (dto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new EditViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                City = dto.City,
                Address = dto.Address,
                Email = dto.Email,
                Phone = dto.Phone,
                Description = dto.Description,
                CreatedOn = dto.CreatedOn
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditService(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new VehicleServiceDto
            {
                Id = model.Id,
                Name = model.Name,
                City = model.City,
                Address = model.Address,
                Email = model.Email,
                Phone = model.Phone,
                Description = model.Description,
                CreatedOn = model.CreatedOn
            };

            await vsManagementService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteService(int id)
        {
            await vsManagementService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ServiceVehicle(int vehicleId, int serviceId)
        {
            var dto = await vsCarsService.GetVehicleWithServiceRecordsAsync(vehicleId, serviceId);

            if (dto == null)
            {
                return RedirectToAction(nameof(Details), new { id = serviceId });
            }

            var model = new ServiceVehicleViewModel
            {
                VehicleId = dto.Id,
                ServiceId = serviceId,
                VIN = dto.VIN,
                CarBrand = dto.CarBrand,
                CarModel = dto.CarModel,
                CreatedOnYear = dto.CreatedOnYear,
                Color = dto.Color,
                VehicleType = dto.VehicleType,
                ServiceRecords = dto.ServiceRecords
                    .Select(sr => new ServiceRecordRowViewModel
                    {
                        Id = sr.Id,
                        Cost = sr.Cost,
                        Description = sr.Description,
                        ServiceDate = sr.ServiceDate
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetServiceRecord(int id)
        {
            var dto = await vsCarsService.GetServiceRecordByIdAsync(id);

            if (dto == null)
            {
                return NotFound();
            }

            return Json(new
            {
                id = dto.Id,
                cost = dto.Cost,
                description = dto.Description,
                serviceDate = dto.ServiceDate.ToString("yyyy-MM-dd"),
                vehicleId = dto.VehicleId,
                vehicleServiceId = dto.VehicleServiceId
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddServiceRecord([FromBody] ServiceRecordFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            var dto = new ServiceRecordDto
            {
                Cost = model.Cost,
                Description = model.Description,
                ServiceDate = model.ServiceDate,
                VehicleId = model.VehicleId,
                VehicleServiceId = model.VehicleServiceId
            };

            await vsCarsService.AddServiceRecordAsync(dto);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> EditServiceRecord([FromBody] ServiceRecordFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            var dto = new ServiceRecordDto
            {
                Id = model.Id,
                Cost = model.Cost,
                Description = model.Description,
                ServiceDate = model.ServiceDate,
                VehicleId = model.VehicleId,
                VehicleServiceId = model.VehicleServiceId
            };

            var updated = await vsCarsService.UpdateServiceRecordAsync(dto);

            if (!updated)
            {
                return Json(new { success = false, message = "Record not found." });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteServiceRecord([FromBody] DeleteServiceRecordRequest request)
        {
            var deleted = await vsCarsService.DeleteServiceRecordAsync(request.Id);
            if (!deleted)
            {
                return Json(new { success = false, message = "Record not found." });
            }

            return Json(new { success = true });
        }
    }
}