using EmployeeMaintenance.DL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMaintenance.DL.Services.DAL.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task UpdateEmployeeDepartment(long employeeId, long departmentId);
    }
}
