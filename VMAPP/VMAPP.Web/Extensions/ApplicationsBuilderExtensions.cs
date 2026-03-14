using VMAPP.Data;
using VMAPP.Data.Seeding;

namespace VMAPP.Web.Extensions
{
    public static class ApplicationsBuilderExtensions
    {
        public static IApplicationBuilder SeedDatabase (this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                new ApplicationDbContextSeeder()
                    .SeedAsync(dbContext)
                    .GetAwaiter()
                    .GetResult();
            }
            return app;
        }
    }
}
