using Serilog;
using Serilog.Context;

namespace VMAPP.Web.Extensions
{
    public static class ApplicationsBuilderExtensions
    {
        public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });

            Log.Logger = SerilogExtensions.CreateLogger(builder.Configuration);

            builder.Host.UseSerilog();

            return builder;
        }

        public static IApplicationBuilder AddErrorHandler(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

            return app;
        }

        public static IApplicationBuilder InsertEndpoints(this IApplicationBuilder app)
        {
            var endpointApp = (IEndpointRouteBuilder)app;

            endpointApp.MapControllerRoute(
                name: "Administration",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            endpointApp.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            endpointApp.MapRazorPages();

            return app;
        }

        public static IApplicationBuilder InsertUserInLog(this IApplicationBuilder app)
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
        }
    }
}
