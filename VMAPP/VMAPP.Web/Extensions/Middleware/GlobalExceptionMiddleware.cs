namespace VMAPP.Web.Extensions.Middleware
{
    using Serilog;

    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Unhandled exception | {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                if (_env.IsDevelopment())
                {
                    throw;
                }

                if (!context.Response.HasStarted)
                {
                    context.Response.Redirect("/Home/Error?statusCode=500");
                }
            }
        }
    }
}
