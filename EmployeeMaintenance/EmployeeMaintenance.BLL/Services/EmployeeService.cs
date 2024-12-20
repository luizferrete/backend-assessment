using AutoMapper;
using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.DL.Services.BLL;
using EmployeeMaintenance.DL.Services.DAL.Repositories;
using EmployeeMaintenance.DL.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace EmployeeMaintenance.BLL.Services
{
    public class EmployeeService : IEmployeeService
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;
        private readonly ICacheHelper _cacheHelper;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper, IDepartmentService departmentService, ICacheHelper cacheHelper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _departmentService = departmentService;
            _cacheHelper = cacheHelper;
        }

        public async Task Create(EmployeeRequest employeeRequest)
        {
            await ValidateEmployee(employeeRequest);

            Employee employee = _mapper.Map<Employee>(employeeRequest);

            await _employeeRepository.InsertAsync(employee);

            ClearCache();
        }

        public async Task Delete(long id)
        {
            var employee = await GetById(id);

            if (employee == null)
            {
                throw new KeyNotFoundException("Employee not found.");
            }

            await _employeeRepository.DeleteAsync(employee);
            ClearCache(
                id: (int)id,
                clearDistinct: true,
                clearExists: true);
        }

        public async Task<bool> Exists(long id)
        {
            return await _cacheHelper.GetOrCreateAsync("ExistsEmployee"+id, async item =>
            {
                return await _employeeRepository.ExistsAsync(id);
            });
        }

        public IEnumerable<EmployeeResponse> GetAll()
        {
            return _cacheHelper.GetOrCreate("GetAllEmployees", item =>
            {
                return _employeeRepository.GetAll().ToList()
                    .Select(employee => _mapper.Map<EmployeeResponse>(employee));
            }, new CacheOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            });
        }

        public async Task<Employee> GetById(long id)
        {
            return await _cacheHelper.GetOrCreateAsync("GetEmployee" + id, async item =>
            {
                item.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                item.SlidingExpiration = TimeSpan.FromMinutes(10);

                return await _employeeRepository.FindByIdAsync(id) ?? throw new KeyNotFoundException("Employee not found.");
            });
        }
        
        public async Task Update(int id, EmployeeRequest employeeRequest)
        {
            await ValidateEmployeeExists(id);

            await ValidateEmployee(employeeRequest);

            Employee employee = _mapper.Map<Employee>(employeeRequest);
            employee.Id = id;

            await _employeeRepository.UpdateAsync(employee);

            ClearCache(
                clearDistinct: true,
                id: id);

        }

        private async Task ValidateEmployeeExists(long id)
        {
            var employeeExists = await Exists(id);

            if (!employeeExists)
            {
                throw new KeyNotFoundException("Employee not found.");
            }
        }

        public async Task UpdateEmployeeDepartment(long employeeId, long departmentId)
        {
            await ValidateEmployeeExists(employeeId);
            await ValidateDepartment(departmentId);

            await _employeeRepository.UpdateEmployeeDepartment(employeeId, departmentId);
            
            ClearCache(
                clearDistinct: true,
                id: (int)employeeId);
        }

        private async Task ValidateEmployee(EmployeeRequest employee)
        {
            await ValidateDepartment(employee.DepartmentId);

            if (string.IsNullOrWhiteSpace(employee.Name))
                throw new Exception("Name cannot be null or empty.");

            if (employee.Name.Length > 128)
                throw new Exception("Name exceeds the maximum length of 128 characters.");

            if (string.IsNullOrWhiteSpace(employee.LastName))
                throw new Exception("Last name cannot be null or empty.");

            if (employee.LastName.Length > 128)
                throw new Exception("Last name exceeds the maximum length of 128 characters.");

            if (employee.HireDate > DateTime.Now)
                throw new Exception("The hire date can't be in the future.");

            if (string.IsNullOrWhiteSpace(employee.Phone))
                throw new Exception("Phone cannot be null or empty.");

            string phonePattern = @"^[0-9\s\-\(\)\+]+$";
            if (string.IsNullOrWhiteSpace(employee.Phone) || !Regex.IsMatch(employee.Phone, phonePattern))
                throw new Exception("Invalid phone number.");

            if (employee.Phone.Length > 24)
                throw new Exception("Phone exceeds the maximum length of 24 characters.");

            if (string.IsNullOrWhiteSpace(employee.Address))
                throw new Exception("Address cannot be null or empty.");

            if (employee.Address.Length > 255)
                throw new Exception("Address exceeds the maximum length of 255 characters.");
        }

        private async Task ValidateDepartment(long departmentId)
        {
            var hasDepartment = await _departmentService.Exists(departmentId);

            if (!hasDepartment)
                throw new KeyNotFoundException("Department not found.");
        }

        private void ClearCache(bool clearDistinct = false, int? id = null, bool clearExists = false)
        {
            if (clearDistinct && id.HasValue)
                _cacheHelper.Remove("GetEmployee" + id);

            if (clearExists && id.HasValue)
                _cacheHelper.Remove("ExistsEmployee" + id);

            _cacheHelper.Remove("GetAllEmployees");
        }
    }
}
