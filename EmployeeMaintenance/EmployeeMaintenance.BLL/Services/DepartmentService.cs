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

        public DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public async Task<bool> Exists(long id)
        {
            return await _departmentRepository.ExistsAsync(id);
        }

        public IEnumerable<DepartmentResponse> GetAll()
        {
            return _departmentRepository.GetAll().Select(department => _mapper.Map<DepartmentResponse>(department));
        }

        public async Task<Department> GetById(long id)
        {
            return await _departmentRepository.FindByIdAsync(id);
        }
    }
}
