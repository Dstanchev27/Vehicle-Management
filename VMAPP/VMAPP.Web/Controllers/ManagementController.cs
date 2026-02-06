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

        public IActionResult Index()
        {
            List<VehicleServiceViewModel> services = 
                _dbContext.VehicleServices
                    .AsNoTracking()
                    .Select(s => new VehicleServiceViewModel()
                    {
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
                    .ToList();

            return View(services);
        }

        [HttpGet]
        public IActionResult AddService()
        {
            var newService = new VehicleServiceViewModel();
            return View(newService);
        }
/*
        [HttpPost]
        public IActionResult AddService(VehicleServiceViewModel newService)
        {
            if (!ModelState.IsValid)
            {
                return View(newService);
            }

            _dbContext.VehicleServices.Add(newService);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        /*
        [HttpGet]
        public IActionResult EditService()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EditService()
        {
            _dbContext.VehicleServices.Update(service);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteService()
        {
            _dbContext.VehicleServices.Remove(service);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        */
    }
}