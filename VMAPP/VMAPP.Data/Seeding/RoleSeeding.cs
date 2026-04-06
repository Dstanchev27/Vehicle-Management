namespace VMAPP.Data.Seeding
{
    using System.Linq;
    using System.Threading.Tasks;

    using VMAPP.Data.Models;
    using VMAPP.Data.Models.Enums;

    public class RoleSeeding : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            if (dbContext.Roles.Any())
            {
                return;
            }

            var roleNames = new[]
            {
                nameof(AppRole.InsuranceCompany),
                nameof(AppRole.VehicleService),
                nameof(AppRole.ProgramAdministrator),
            };

            foreach (var roleName in roleNames)
            {
                await dbContext.Roles.AddAsync(new ApplicationRole(roleName)
                {
                    NormalizedName = roleName.ToUpper()
                });
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
