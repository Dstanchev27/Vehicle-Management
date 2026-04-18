using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VMAPP.Data.Seeding
{
    public class ApplicationDbContextSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            var seeders = new List<ISeeder>
            {
                new RoleSeeding(),
                new AdminUserSeeding(),       
                new VehicleSeeding(),
                new VehicleServiceSeeding(),   
                new InsuranceCompanySeeding(), 
                new UserSeeding(),             
                new ServiceRecordSeeding(),
                new InsurancePolicySeeding(),
                new InsuranceClaimSeeding(),
                new AnnualReviewCompanySeeding(),
                new AnnualReportSeeding(),
            };

            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync(dbContext);
            }
        }
    }
}
