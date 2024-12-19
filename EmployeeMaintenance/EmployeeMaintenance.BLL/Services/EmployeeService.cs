﻿using AutoMapper;
using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.DL.Services.BLL;
using EmployeeMaintenance.DL.Services.DAL.Repositories;
using EmployeeMaintenance.DL.ValueObjects;
using System.Text.RegularExpressions;

namespace EmployeeMaintenance.BLL.Services
{
    public class EmployeeService : IEmployeeService
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper, IDepartmentService departmentService)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _departmentService = departmentService;
        }

        public async Task Create(EmployeeRequest employeeRequest)
        {
            await ValidateEmployee(employeeRequest);

            Employee employee = _mapper.Map<Employee>(employeeRequest);

            await _employeeRepository.InsertAsync(employee);
        }

        public async Task Delete(long id)
        {
            var employee = await GetById(id);

            if (employee == null)
            {
                throw new KeyNotFoundException("Employee not found.");
            }

            await _employeeRepository.DeleteAsync(employee);
        }

        public async Task<bool> Exists(long id)
        {
            return await _employeeRepository.ExistsAsync(id);
        }

        public IEnumerable<EmployeeResponse> GetAll()
        {
            return _employeeRepository.GetAll()
                .Select(employee => _mapper.Map<EmployeeResponse>(employee));
        }

        public async Task<Employee> GetById(long id)
        {
            return await _employeeRepository.FindByIdAsync(id);
        }
        
        public async Task Update(int id, EmployeeRequest employeeRequest)
        {
            await ValidateEmployeeExists(id);

            await ValidateEmployee(employeeRequest);

            Employee employee = _mapper.Map<Employee>(employeeRequest);
            employee.Id = id;

            await _employeeRepository.UpdateAsync(employee);
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
    }
}