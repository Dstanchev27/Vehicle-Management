using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

using System.Globalization;
using Microsoft.AspNetCore.Identity.UI.Services;
using VMAPP.Data;
using VMAPP.Data.Models;

using VMAPP.Services;
using VMAPP.Services.Interfaces;

namespace VMAPP.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddIdentity<ApplicationUser, ApplicationRole>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IVSManagementService, VSManagementService>();
            services.AddScoped<IVSCarsService, VSCarsService>();
            services.AddScoped<IVSService, VSService>();
            services.AddScoped<IVSInsuranceService, VSInsurance>();

            var supportedCultures = new[] { new CultureInfo("en-US") };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddTransient<ICustomEmailSender>(
                serviceProvider => new SendGridEmailSender(configuration["SendGridApiKey:ApiKey"]));
            services.AddTransient<IEmailSender>(
                serviceProvider => new Services.IdentityEmailSenderAdapter(
                    serviceProvider.GetRequiredService<ICustomEmailSender>(),
                    configuration["SendGridApiKey:SenderEmail"],
                    configuration["SendGridApiKey:SenderName"]));

            return services;
        }

        public static IServiceCollection AddCookieConsent(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(
                options =>
                {
                    options.CheckConsentNeeded = context => false;
                    options.MinimumSameSitePolicy = SameSiteMode.Lax;
                });

            return services;
        }

        public static IServiceCollection AddControllersAndViews(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            return services;
        }

        public static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            services
                .AddAntiforgery(options =>
                {
                    options.HeaderName = "X-CSRF-TOKEN";
                })
                .PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, option =>
                {
                    option.Cookie.Name = "VMA_Auth_Cookie";
                    option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    option.SlidingExpiration = true;
                    option.LoginPath = new PathString("/Login");
                    option.Events.OnRedirectToLogin = context =>
                    {
                        context.HttpContext.Response.Redirect("/Login");
                        return Task.CompletedTask;
                    };
                    option.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.HttpContext.Response.Redirect("/Home/Error?statusCode=403");
                        return Task.CompletedTask;
                    };
                });

            return services;
        }
    }
}
