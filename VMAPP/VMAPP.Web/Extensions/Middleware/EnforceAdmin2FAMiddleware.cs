namespace VMAPP.Web.Extensions.Middleware
{
    using Microsoft.AspNetCore.Identity;

    using VMAPP.Data.Models;

    public class EnforceAdmin2FAMiddleware
    {
        private readonly RequestDelegate _next;

        public EnforceAdmin2FAMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            if (signInManager.IsSignedIn(context.User))
            {
                var user = await userManager.GetUserAsync(context.User);
                var isAdmin = user != null && await userManager.IsInRoleAsync(user, "Administrator");
                var has2FA = user != null && await userManager.GetTwoFactorEnabledAsync(user);

                var path = context.Request.Path.Value ?? "";


                if (isAdmin && !has2FA &&
                    !path.StartsWith("/Identity/Account/Manage/EnableAuthenticator", StringComparison.OrdinalIgnoreCase) &&
                    !path.StartsWith("/Identity/Account/Logout", StringComparison.OrdinalIgnoreCase) &&
                    !path.StartsWith("/css") && !path.StartsWith("/js") && !path.StartsWith("/images"))
                {
                    context.Response.Redirect("/Identity/Account/Manage/EnableAuthenticator");
                    return;
                }
            }

            await _next(context);
        }
    }
}
