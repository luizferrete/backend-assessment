using EmployeeMaintenance.BLL.Services;
using EmployeeMaintenance.DAL.EF.Base;
using EmployeeMaintenance.DAL.EF;
using EmployeeMaintenance.DAL.Repositories;
using EmployeeMaintenance.DL.Services.BLL;
using EmployeeMaintenance.DL.Services.DAL.Repositories;
using EmployeeMaintenance.DL.Services.DAL;
using Microsoft.EntityFrameworkCore;
using EmployeeMaintenance.BLL.Helpers;

namespace EmployeeMaintenance.API.Injections
{
    public class DependencyInjections
    {
        public static void Config(IServiceCollection servicesContainer, IConfigurationManager configuration)
        {
            servicesContainer.AddDbContext<EntityContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
                options.UseLazyLoadingProxies();
                options.EnableSensitiveDataLogging();
            });

            servicesContainer.AddSingleton<ICacheHelper, CacheHelper>();

            servicesContainer.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));

            servicesContainer.AddTransient<IEmployeeRepository, EmployeeRepository>();
            servicesContainer.AddTransient<IEmployeeService, EmployeeService>();
            servicesContainer.AddTransient<IDepartmentRepository, DepartmentRepository>();
            servicesContainer.AddTransient<IDepartmentService, DepartmentService>();
        }
    }
}
