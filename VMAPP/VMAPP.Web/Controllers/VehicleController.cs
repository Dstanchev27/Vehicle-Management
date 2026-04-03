using Microsoft.AspNetCore.Mvc;
using VMAPP.Services.DTOs;
using VMAPP.Services.DTOs.VehicleDTOs;
using VMAPP.Services.Interfaces;
using VMAPP.Web.Models.VehicleViewModels;

namespace VMAPP.Web.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IVSService vehicleService;

        public VehicleController(IVSService vehicleService)
        {
            this.vehicleService = vehicleService;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await this.vehicleService.GetAllAsync();

            var vehicles = dtos
                .Select(v => new VehicleIndexViewModel
                {
                    Id = v.Id,
                    VIN = v.VIN,
                    CarBrand = v.CarBrand,
                    CarModel = v.CarModel,
                    CreatedOnYear = v.CreatedOnYear,
                    Color = v.Color,
                    VehicleType = v.VehicleType,
                    CreatedOn = v.CreatedOn
                })
                .ToList();

            return View(vehicles);
        }

        [HttpGet]
        public IActionResult AddVehicle()
        {
            return View(new AddVehicleViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle(AddVehicleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new VehicleDto
            {
                VIN = model.VIN.ToUpperInvariant(),
                CarBrand = model.CarBrand,
                CarModel = model.CarModel,
                CreatedOnYear = model.CreatedOnYear,
                Color = model.Color,
                VehicleType = model.VehicleType
            };

            await this.vehicleService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditVehicle(int id)
        {
            var dto = await this.vehicleService.GetByIdAsync(id);

            if (dto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new EditVehicleViewModel
            {
                Id = dto.Id,
                VIN = dto.VIN,
                CarBrand = dto.CarBrand,
                CarModel = dto.CarModel,
                CreatedOnYear = dto.CreatedOnYear,
                Color = dto.Color,
                VehicleType = dto.VehicleType
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditVehicle(EditVehicleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new VehicleDto
            {
                Id = model.Id,
                VIN = model.VIN.ToUpperInvariant(),
                CarBrand = model.CarBrand,
                CarModel = model.CarModel,
                CreatedOnYear = model.CreatedOnYear,
                Color = model.Color,
                VehicleType = model.VehicleType
            };

            await this.vehicleService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            await this.vehicleService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
