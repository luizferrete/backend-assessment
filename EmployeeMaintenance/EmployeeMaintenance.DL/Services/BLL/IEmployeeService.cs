using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.DL.ValueObjects;

namespace EmployeeMaintenance.DL.Services.BLL
{
    public interface IEmployeeService
    {
        Task<Employee> GetById(long id);

        IEnumerable<EmployeeResponse> GetAll();

        Task Create(EmployeeRequest employeeRequest);

        Task Update(int id, EmployeeRequest employeeRequest);

        Task<bool> Exists(long id);

        Task Delete(long id);
        Task UpdateEmployeeDepartment(long employeeId, long departmentId);
    }
}
