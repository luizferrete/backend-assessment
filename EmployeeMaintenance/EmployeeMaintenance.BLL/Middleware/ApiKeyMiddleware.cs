using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMaintenance.BLL.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEYNAME = "EM-API-KEY";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            if(!context.Request.Headers.TryGetValue(APIKEYNAME.ToLower(), out var extractedKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized.");
                return;
            }

            var apiKey = configuration.GetSection("ApiKeySettings:ApiKey").Value;

            if(!apiKey.Equals(extractedKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized.");
                return;
            }

            await _next(context);
        }
    }
}
