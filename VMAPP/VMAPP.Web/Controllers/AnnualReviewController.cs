using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VMAPP.Common;
using VMAPP.Data.Models;
using VMAPP.Services.DTOs.AnnualReviewDTOs;
using VMAPP.Services.Interfaces;
using VMAPP.Web.Models.AnnualReviewModels;

namespace VMAPP.Web.Controllers
{
    [Authorize(Roles = "ProgramAdministrator,AnnualReviewCompany")]
    public class AnnualReviewController : Controller
    {
        private readonly IVSAnnualReviewService annualReviewService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AnnualReviewController> logger;

        public AnnualReviewController(
            IVSAnnualReviewService annualReviewService,
            UserManager<ApplicationUser> userManager,
            ILogger<AnnualReviewController> logger)
        {
            this.annualReviewService = annualReviewService;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if (IsAnnualReviewCompanyUser())
            {
                var companyId = await GetCurrentUserAnnualReviewCompanyIdAsync();
                if (companyId == null)
                {
                    return View("Error");
                }

                return RedirectToAction(nameof(Details), new { id = companyId.Value });
            }

            var dtos = await annualReviewService.GetAllAsync();

            var companies = dtos
                .Select(c => new AnnualReviewCompanyViewModel
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
        [Authorize(Roles = GlobalConstant.AdministratorRoleName)]
        public IActionResult AddAnnualReviewCompany()
        {
            return View(new AddAnnualReviewCompanyViewModel());
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstant.AdministratorRoleName)]
        public async Task<IActionResult> AddAnnualReviewCompany(AddAnnualReviewCompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new AnnualReviewCompanyDto
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

            await annualReviewService.CreateAsync(dto);
            logger.LogInformation("Annual review company '{Name}' was successfully created.", dto.Name);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (IsAnnualReviewCompanyUser())
            {
                var userCompanyId = await GetCurrentUserAnnualReviewCompanyIdAsync();
                if (userCompanyId != id)
                {
                    return Forbid();
                }
            }

            var dto = await annualReviewService.GetCompanyWithVehiclesAsync(id);

            if (dto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new AnnualReviewCompanyDetailsViewModel
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
                    .Select(v => new VehicleWithReportRowViewModel
                    {
                        Id = v.Id,
                        ReportId = v.ReportId,
                        VIN = v.VIN,
                        CarBrand = v.CarBrand,
                        CarModel = v.CarModel,
                        CreatedOnYear = v.CreatedOnYear,
                        Color = v.Color,
                        VehicleType = v.VehicleType,
                        ReportNumber = v.ReportNumber
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ReportDetails(int id)
        {
            var dto = await annualReviewService.GetReportDetailsAsync(id);

            if (dto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (IsAnnualReviewCompanyUser())
            {
                var userCompanyId = await GetCurrentUserAnnualReviewCompanyIdAsync();
                if (userCompanyId != dto.AnnualReviewCompanyId)
                {
                    return Forbid();
                }
            }

            var model = new AnnualReportDetailsViewModel
            {
                Id = dto.Id,
                ReportNumber = dto.ReportNumber,
                InspectionDate = dto.InspectionDate,
                ExpiryDate = dto.ExpiryDate,
                Passed = dto.Passed,
                Notes = dto.Notes,
                VehicleId = dto.VehicleId,
                VehicleVIN = dto.VehicleVIN,
                VehicleBrand = dto.VehicleBrand,
                VehicleModel = dto.VehicleModel,
                AnnualReviewCompanyId = dto.AnnualReviewCompanyId,
                AnnualReviewCompanyName = dto.AnnualReviewCompanyName
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstant.AdministratorRoleName)]
        public async Task<IActionResult> EditAnnualReviewCompany(int id)
        {
            var dto = await annualReviewService.GetByIdAsync(id);

            if (dto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new EditAnnualReviewCompanyViewModel
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
        [Authorize(Roles = GlobalConstant.AdministratorRoleName)]
        public async Task<IActionResult> EditAnnualReviewCompany(EditAnnualReviewCompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new AnnualReviewCompanyDto
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

            await annualReviewService.UpdateAsync(dto);
            logger.LogInformation("Annual review company '{Name}' was successfully updated.", dto.Name);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstant.AdministratorRoleName)]
        public async Task<IActionResult> DeleteAnnualReviewCompany(int id)
        {
            await annualReviewService.DeleteAsync(id);
            logger.LogInformation("Annual review company with id {Id} was successfully deleted.", id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> SearchVehicleByVin(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin))
            {
                return Json(new { found = false, message = "Please enter a VIN to search." });
            }

            var vehicle = await annualReviewService.GetVehicleByVinAsync(vin.Trim().ToUpperInvariant());

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
        public async Task<IActionResult> AddReport([FromBody] AnnualReportFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            if (IsAnnualReviewCompanyUser())
            {
                var userCompanyId = await GetCurrentUserAnnualReviewCompanyIdAsync();
                if (userCompanyId != model.AnnualReviewCompanyId)
                {
                    return Json(new { success = false, message = "Access denied." });
                }
            }

            var dto = new AnnualReportFormDto
            {
                VehicleId = model.VehicleId,
                AnnualReviewCompanyId = model.AnnualReviewCompanyId,
                ReportNumber = model.ReportNumber,
                InspectionDate = model.InspectionDate,
                ExpiryDate = model.ExpiryDate,
                Passed = model.Passed,
                Notes = model.Notes,
                CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            await annualReviewService.AddReportAsync(dto);
            logger.LogInformation("Annual report '{ReportNumber}' for vehicle {VehicleId} was successfully created.", dto.ReportNumber, dto.VehicleId);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReport([FromBody] DeleteReportRequest request)
        {
            if (IsAnnualReviewCompanyUser())
            {
                var userCompanyId = await GetCurrentUserAnnualReviewCompanyIdAsync();
                var reportCompanyId = await annualReviewService.GetCompanyIdByReportIdAsync(request.Id);
                if (userCompanyId != reportCompanyId)
                {
                    return Json(new { success = false, message = "Access denied." });
                }
            }

            var deleted = await annualReviewService.DeleteReportAsync(request.Id);
            if (!deleted)
            {
                return Json(new { success = false, message = "Report not found." });
            }

            logger.LogInformation("Annual report with id {Id} was successfully deleted.", request.Id);
            return Json(new { success = true });
        }

        private bool IsAnnualReviewCompanyUser()
            => User.IsInRole(GlobalConstant.AnnualReviewCompanyRoleName);

        private async Task<int?> GetCurrentUserAnnualReviewCompanyIdAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return null;
            }

            var user = await userManager.FindByIdAsync(userId);
            return user?.AnnualReviewCompanyId;
        }
    }
}
