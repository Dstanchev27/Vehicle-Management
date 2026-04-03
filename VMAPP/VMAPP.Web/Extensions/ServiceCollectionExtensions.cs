using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

using System.Globalization;

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
                .AddIdentity<ApplicationUser, IdentityRole>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IVSManagementService, VSManagementService>();
            services.AddScoped<IVSCarsService, VSCarsService>();
            services.AddScoped<IVSService, VSService>();

            var supportedCultures = new[] { new CultureInfo("en-US") };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            return services;
        }

        public static IServiceCollection AddCookieConsent(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(
                options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.Lax;
                });

            return services;
        }

        public static IServiceCollection AddControllersAndViews(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AllowAnonymousFilter());
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
                        //context.HttpContext.Response.Redirect("https://" + context.HttpContext.Request.Host + "/Login");

                        var request = context.HttpContext.Request;
                        var redirectUrl = new UriBuilder
                        {
                            Scheme = "https",
                            Host = request.Host.Host,
                            Port = 441,
                            Path = "/Login"
                        };

                        context.HttpContext.Response.Redirect(redirectUrl.ToString());
                        return Task.CompletedTask;
                    };
                    option.Events.OnRedirectToAccessDenied = context =>
                    {
                        var request = context.HttpContext.Request;
                        var redirectUrl = new UriBuilder
                        {
                            Scheme = "https",
                            Host = request.Host.Host,
                            Port = 441,
                            Path = "/Home/Error403"
                        };

                        context.HttpContext.Response.Redirect(redirectUrl.ToString());
                        return Task.CompletedTask;
                    };
                });

            return services;
        }
    }
}
