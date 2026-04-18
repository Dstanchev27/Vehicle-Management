using Serilog;
using Serilog.Events;

using VMAPP.Web.Extensions;
using VMAPP.Web.Extensions.Middleware;

namespace VMAPP.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("System", LogEventLevel.Error)
                .WriteTo.Console()
                .CreateBootstrapLogger();

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

                await app.InitializeDatabaseAsync();

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
            }
            finally
            {
                Log.Fatal("The process was killed!");
                await Log.CloseAndFlushAsync();
            }
        }
    }
}
