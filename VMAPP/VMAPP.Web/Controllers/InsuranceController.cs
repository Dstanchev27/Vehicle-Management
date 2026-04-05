using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VMAPP.Common;
using VMAPP.Services.DTOs;
using VMAPP.Services.DTOs.InsuranceDTOs;
using VMAPP.Services.Interfaces;
using VMAPP.Web.Models.InsuranceModels;

namespace VMAPP.Web.Controllers
{
    [Authorize(Roles = GlobalConstant.AdministratorRoleName + "," + GlobalConstant.InsuranceCompanyRoleName)]
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
            var dto = await insuranceService.GetCompanyWithVehiclesAsync(id);

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
                CreatedOn = dto.CreatedOn,
                Vehicles = dto.Vehicles
                    .Select(v => new VehicleWithPolicyRowViewModel
                    {
                        Id = v.Id,
                        PolicyId = v.PolicyId,
                        VIN = v.VIN,
                        CarBrand = v.CarBrand,
                        CarModel = v.CarModel,
                        CreatedOnYear = v.CreatedOnYear,
                        Color = v.Color,
                        VehicleType = v.VehicleType,
                        PolicyNumber = v.PolicyNumber
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PolicyDetails(int id)
        {
            var dto = await insuranceService.GetPolicyDetailsAsync(id);

            if (dto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new InsurancePolicyDetailsViewModel
            {
                Id = dto.Id,
                PolicyNumber = dto.PolicyNumber,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                VehicleId = dto.VehicleId,
                VehicleVIN = dto.VehicleVIN,
                VehicleBrand = dto.VehicleBrand,
                VehicleModel = dto.VehicleModel,
                InsuranceCompanyId = dto.InsuranceCompanyId,
                InsuranceCompanyName = dto.InsuranceCompanyName,
                Claims = dto.Claims
                    .Select(c => new InsuranceClaimRowViewModel
                    {
                        Id = c.Id,
                        ClaimDate = c.ClaimDate,
                        Description = c.Description,
                        Amount = c.Amount
                    })
                    .ToList()
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
        public async Task<IActionResult> DeleteInsuranceCompany(int id)
        {
            await insuranceService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> SearchVehicleByVin(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin))
            {
                return Json(new { found = false, message = "Please enter a VIN to search." });
            }

            var vehicle = await insuranceService.GetVehicleByVinAsync(vin.Trim().ToUpperInvariant());

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
        public async Task<IActionResult> AddPolicy([FromBody] InsurancePolicyFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            var dto = new InsurancePolicyFormDto
            {
                VehicleId = model.VehicleId,
                InsuranceCompanyId = model.InsuranceCompanyId,
                PolicyNumber = model.PolicyNumber,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            await insuranceService.AddPolicyAsync(dto);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePolicy([FromBody] DeletePolicyRequest request)
        {
            var deleted = await insuranceService.DeletePolicyAsync(request.Id);
            if (!deleted)
            {
                return Json(new { success = false, message = "Policy not found." });
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetClaim(int id)
        {
            var dto = await insuranceService.GetClaimByIdAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            return Json(new
            {
                id = dto.Id,
                claimDate = dto.ClaimDate.ToString("yyyy-MM-dd"),
                description = dto.Description,
                amount = dto.Amount
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddClaim([FromBody] InsuranceClaimFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            var dto = new InsuranceClaimFormDto
            {
                InsurancePolicyId = model.InsurancePolicyId,
                ClaimDate = model.ClaimDate,
                Description = model.Description,
                Amount = model.Amount
            };

            await insuranceService.AddClaimAsync(dto);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteClaim([FromBody] DeleteClaimRequest request)
        {
            var deleted = await insuranceService.DeleteClaimAsync(request.Id);
            if (!deleted)
            {
                return Json(new { success = false, message = "Claim not found." });
            }

            return Json(new { success = true });
        }
    }
}

