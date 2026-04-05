using Serilog;

using VMAPP.Web.Extensions;
using VMAPP.Web.Extensions.Middleware;

namespace VMAPP.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.AddSerilog();

                var services = builder.Services;

                services
                    .AddDatabase(builder.Configuration)
                    .AddIdentity()
                    .AddControllersAndViews()
                    .AddCookieConsent()
                    .AddControllersAndViews()
                    .AddApplicationServices(builder.Configuration)
                    .AddSecurity()
                    .AddRazorPages();

                var app = builder.Build();

                app
                    .UseRequestLocalization()
                    .AddErrorHandler()
                    .UseHsts()
                    .UseHttpsRedirection()
                    .UseCookiePolicy()
                    .UseStaticFiles()
                    .UseRouting()
                    .UseAuthentication()
                    .UseAuthorization()
                    .InsertUserInLog()
                    .UseSerilogRequestLogging()
                    .UseMiddleware<GlobalExceptionMiddleware>()
                    .UseMiddleware<EnforceAdmin2FAMiddleware>()
                    .InsertEndpoints();

                Log.Information("Starting up the VMApp!");

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There was a problem starting the service!");
                return;
            }
            finally
            {
                Log.Fatal("The process was killed!");
                Log.CloseAndFlush();
            }
        }
    }
}
