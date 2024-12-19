using AutoMapper;
using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.DL.ValueObjects;
using System.Globalization;

namespace EmployeeMaintenance.BLL.AutoMapper
{
    public class ConfigMapper : Profile
    {
        public ConfigMapper()
        {
            CreateMap<EmployeeRequest, Employee>()
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
                .ForMember(dest => dest.LastName, opts => opts.MapFrom(src => src.LastName))
                .ForMember(dest => dest.HireDate, opts => opts.MapFrom(src => src.HireDate))
                .ForMember(dest => dest.Address, opts => opts.MapFrom(src => src.Address))
                .ForMember(dest => dest.Phone, opts => opts.MapFrom(src => src.Phone))
                .ForMember(dest => dest.DepartmentId, opts => opts.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.FlagActive, opts => opts.MapFrom(src => true));

            CreateMap<Employee, EmployeeResponse>()
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => $"{src.Name} {src.LastName}"))
                .ForMember(dest => dest.HireDate, opts => opts.MapFrom(src => src.HireDate.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.HiredTime, opts => opts.MapFrom(src => src.CalculateHiredTime(DateTime.UtcNow, src.HireDate)))
                .ForMember(dest => dest.Department, opts => opts.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.DepartmentId, opts => opts.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.Address, opts => opts.MapFrom(src => src.Address))
                .ForMember(dest => dest.Phone, opts => opts.MapFrom(src => src.Phone));

            CreateMap<Department, DepartmentResponse>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name));
        }
    }
}
