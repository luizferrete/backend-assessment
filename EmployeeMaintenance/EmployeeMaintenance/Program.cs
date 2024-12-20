using EmployeeMaintenance.API.Injections;
using EmployeeMaintenance.BLL.AutoMapper;
using EmployeeMaintenance.BLL.Extensions;
using EmployeeMaintenance.BLL.Middleware;
using EmployeeMaintenance.DAL.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCustomRateLimiter(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0",
        Title = "Employee Maintenance API",
        Description = "",
        Contact = new OpenApiContact
        {
            Name = "Luiz Augusto Ferrete",
            Email = "l.a.ferrete@gmail.com",
            Url = new Uri("https://www.linkedin.com/in/luiz-augusto-ferrete-990713b8/")
        }
    });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Provide the API Key in the header (example: 'EM-API-KEY: {key}')",
        Name = "EM-API-KEY",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKey"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });

});

builder.Services.AddApiVersioning(options =>
{
    // Report the API versions supported for the particular endpoint
    options.ReportApiVersions = true;

    // Assume default version when unspecified
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);

    // Use the "apiVersion" route parameter to determine the API version
    options.ApiVersionReader = new Microsoft.AspNetCore.Mvc.Versioning.UrlSegmentApiVersionReader();
});

builder.Services.AddMemoryCache();
builder.Services.AddDbContext<EntityContext>();
builder.Services.AddAutoMapper(typeof(ConfigMapper));

DependencyInjections.Config(builder.Services, builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

// Run migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<EntityContext>();
    context.Database.Migrate();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseRateLimiter();

app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
