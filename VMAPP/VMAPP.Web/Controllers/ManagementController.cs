using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Web.Models.VehicleServiceModels;

namespace VMAPP.Web.Controllers
{
    public class ManagementController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ManagementController(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var services = 
               await _dbContext.VehicleServices
                    .AsNoTracking()
                    .Select(s => new EditViewModel()
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
                    .OrderBy(n => n.Name)
                    .ThenBy(cr => cr.CreatedOn)
                    .ToListAsync();

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
                var dbService = new VehicleService()
                {
                    Address = newService.Address,
                    City = newService.City,
                    Description = newService.Description,
                    Email = newService.Email,
                    Name = newService.Name,
                    Phone = newService.Phone,
                    CreatedOn = DateTime.UtcNow,
                };

                await _dbContext.VehicleServices.AddAsync(dbService);
                await _dbContext.SaveChangesAsync();
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
            var entity = await _dbContext.VehicleServices
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (entity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new EditViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                City = entity.City,
                Address = entity.Address,
                Email = entity.Email,
                Phone = entity.Phone,
                Description = entity.Description,
                CreatedOn = entity.CreatedOn
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

            var entity = await _dbContext.VehicleServices.FirstOrDefaultAsync(s => s.Id == model.Id);
            if (entity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            entity.Name = model.Name;
            entity.City = model.City;
            entity.Address = model.Address;
            entity.Email = model.Email;
            entity.Phone = model.Phone;
            entity.Description = model.Description;

            _dbContext.VehicleServices.Update(entity);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int id)
        {
            var entity = await _dbContext.VehicleServices.FirstOrDefaultAsync(s => s.Id == id);

            if (entity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _dbContext.VehicleServices.Remove(entity);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}