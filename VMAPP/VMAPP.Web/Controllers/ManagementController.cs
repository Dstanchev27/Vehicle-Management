using Microsoft.AspNetCore.Mvc;
using VMAPP.Services.DTOs;
using VMAPP.Services.Interfaces;
using VMAPP.Web.Models.VehicleServiceModels;

namespace VMAPP.Web.Controllers
{
    public class ManagementController : Controller
    {
        private readonly IVSManagementService _vsManagementService;

        public ManagementController(IVSManagementService vsManagementService)
        {
            _vsManagementService = vsManagementService;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _vsManagementService.GetAllAsync();

            var services = dtos
                .Select(s => new EditViewModel
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
            var newService = new AddServiceViewModel();
            return View(newService);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

                await _vsManagementService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditService(int id)
        {
            var dto = await _vsManagementService.GetByIdAsync(id);
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
        [ValidateAntiForgeryToken]
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

            var updated = await _vsManagementService.UpdateAsync(dto);
            if (!updated)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int id)
        {
            await _vsManagementService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}