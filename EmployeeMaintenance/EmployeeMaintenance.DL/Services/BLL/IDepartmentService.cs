using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.DL.ValueObjects;

namespace EmployeeMaintenance.DL.Services.BLL
{
    public interface IDepartmentService
    {
        Task<bool> Exists(long id);
        Task<Department> GetById(long id);
        IEnumerable<DepartmentResponse> GetAll();
    }
}
