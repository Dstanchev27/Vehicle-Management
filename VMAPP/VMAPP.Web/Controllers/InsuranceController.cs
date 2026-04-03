using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VMAPP.Services.DTOs;
using VMAPP.Services.Interfaces;
using VMAPP.Web.Models.InsuranceModels;

namespace VMAPP.Web.Controllers
{
    public class InsuranceController : Controller
    {
        private readonly IVSInsuranceService insuranceService;

        public InsuranceController(IVSInsuranceService insuranceService)
        {
            this.insuranceService = insuranceService;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await insuranceService.GetAllAsync();

            var companies = dtos
                .Select(c => new InsuranceCompanyViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedOn = c.CreatedOn,
                    City = c.City,
                    Address = c.Address,
                    Email = c.Email,
                    Phone = c.Phone
                })
                .ToList();

            return View(companies);
        }

        [HttpGet]
        public IActionResult AddInsuranceCompany()
        {
            return View(new AddInsuranceCompanyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInsuranceCompany(AddInsuranceCompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var dto = new InsuranceCompanyDto
                {
                    Name = model.Name,
                    City = model.City,
                    Address = model.Address,
                    Email = model.Email,
                    Phone = model.Phone,
                    Description = model.Description,
                    CreatedOn = DateTime.UtcNow,
                    CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                await insuranceService.CreateAsync(dto);
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
            var dto = await insuranceService.GetByIdAsync(id);

            if (dto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new InsuranceCompanyDetailsViewModel
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

        [HttpGet]
        public async Task<IActionResult> EditInsuranceCompany(int id)
        {
            var dto = await insuranceService.GetByIdAsync(id);

            if (dto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new EditInsuranceCompanyViewModel
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
        public async Task<IActionResult> EditInsuranceCompany(EditInsuranceCompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new InsuranceCompanyDto
            {
                Id = model.Id,
                Name = model.Name,
                City = model.City,
                Address = model.Address,
                Email = model.Email,
                Phone = model.Phone,
                Description = model.Description,
                CreatedOn = model.CreatedOn,
                ModifiedById = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            await insuranceService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInsuranceCompany(int id)
        {
            await insuranceService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

