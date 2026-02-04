using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using VMAPP.Data;
using VMAPP.Data.Models;

namespace VMAPP.SandBox
{
    public class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(
                            context.Configuration.GetConnectionString("DefaultConnection")));
                })
                .Build();

            // Use DbContext
            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();



            GiveInformation(db);
        }
        public static void GiveInformation(ApplicationDbContext db)
        {
            List<Vehicle> vehicles = db.Vehicles.ToList();
            foreach (var vehicle in vehicles)
            {
                Console.WriteLine(
                    $"Vehicle ID: {vehicle.VehicleId}, VIN: {vehicle.VIN}, Brand: {vehicle.CarBrand}, Model: {vehicle.CarModel}, Year: {vehicle.CreatedOnYear.Year}, Color: {vehicle.Color}, Type: {vehicle.VehicleType}");
            }
        }
    }
}
