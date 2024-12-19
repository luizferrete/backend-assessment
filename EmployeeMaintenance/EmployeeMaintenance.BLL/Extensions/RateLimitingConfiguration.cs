
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace EmployeeMaintenance.BLL.Extensions
{
    public static class RateLimitingConfiguration
    {
        public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    // Extract API Key from the request header
                    if (context.Request.Headers.TryGetValue("EM-API-KEY", out var apiKey))
                    {
                        var key = apiKey.ToString();

                        return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 200,
                            Window = TimeSpan.FromMinutes(15),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
                    }
                    else
                    {
                        return RateLimitPartition.GetNoLimiter("no-limit");
                    }
                });

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = 429; // Too Many Requests
                    context.HttpContext.Response.ContentType = "application/json";
                    var response = new
                    {
                        Message = "Too many requests. Please try again later."
                    };
                    await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
                };
            });

            return services;
        }
    }
}
