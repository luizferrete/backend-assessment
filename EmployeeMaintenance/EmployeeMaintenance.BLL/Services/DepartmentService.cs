using AutoMapper;
using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.DL.Services.BLL;
using EmployeeMaintenance.DL.Services.DAL.Repositories;
using EmployeeMaintenance.DL.ValueObjects;

namespace EmployeeMaintenance.BLL.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly ICacheHelper _cacheHelper;

        public DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper, ICacheHelper cacheHelper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            _cacheHelper = cacheHelper;
        }

        public async Task<bool> Exists(long id)
        {
            return await _cacheHelper.GetOrCreateAsync("ExistsDepartment" + id, async item =>
            {
                return await _departmentRepository.ExistsAsync(id);
            });
        }

        public IEnumerable<DepartmentResponse> GetAll()
        {
            return _cacheHelper.GetOrCreate("GetAllDepartments", item =>
            {
                return _departmentRepository.GetAll().ToList()
                    .Select(department => _mapper.Map<DepartmentResponse>(department));
            }, new CacheOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            });
        }

        public async Task<Department> GetById(long id)
        {
            return await _cacheHelper.GetOrCreate("GetDepartment" + id, async item =>
            {
                return await _departmentRepository.FindByIdAsync(id);
            });
        }
    }
}
