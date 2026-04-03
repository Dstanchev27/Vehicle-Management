using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

using VMAPP.Data;
using VMAPP.Data.Seeding;

namespace VMAPP.Web.Extensions
{
    public static class ApplicationsBuilderExtensions
    {
        public static IApplicationBuilder AddErrorHandler(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            return app;
        }

        public static IApplicationBuilder InsertEndpoints(this IApplicationBuilder app)
        {
            var endpointApp = (IEndpointRouteBuilder)app;

            endpointApp.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            endpointApp.MapRazorPages();

            return app;
        }

       /* public static IApplicationBuilder InsertUserInLog(this IApplicationBuilder app)
        {
            app.Use(async (httpContext, next) =>
            {
                var username = httpContext.User.Identity?.IsAuthenticated == true
                    ? httpContext.User.Identity.Name
                    : "Anonymous";

                LogContext.PushProperty("Username", username);
                await next.Invoke();
            });

            return app;
        }*/
    }
}
