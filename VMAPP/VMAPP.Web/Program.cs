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

                var services = builder.Services;

                services
                    .AddDatabase(builder.Configuration)
                    .AddIdentity()
                    .AddControllersAndViews()
                    .AddCookieConsent()
                    .AddControllersAndViews()
                    .AddApplicationServices()
                    .AddSecurity()
                    .AddRazorPages();

                var app = builder.Build();

                app
                    .UseRequestLocalization()
                    .AddErrorHandler()
                    //.InsertUserInLog()
                    .UseHsts()
                    .UseHttpsRedirection()
                    .UseCookiePolicy()
                    .UseStaticFiles()
                    .UseRouting()
                    .UseAuthentication()
                    .UseAuthorization()
                    .UseMiddleware<EnforceAdmin2FAMiddleware>()
                    .InsertEndpoints();

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"Application failed to start: {ex}");
                throw;
            }
        }
    }
}
