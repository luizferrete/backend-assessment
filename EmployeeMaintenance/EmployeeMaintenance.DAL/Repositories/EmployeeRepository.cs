using EmployeeMaintenance.DAL.EF;
using EmployeeMaintenance.DAL.EF.Base;
using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.DL.Services.DAL.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EmployeeMaintenance.DAL.Repositories
{
    public class EmployeeRepository : EFRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(EntityContext context) : base(context)
        {
            
        }

        public async Task UpdateEmployeeDepartment(long employeeId, long departmentId)
        {
            string sql = @"UPDATE Employees SET DepartmentId = @departmentId WHERE Id = @employeeId AND FlagActive = 1";

            await _dbContext.Database.ExecuteSqlRawAsync(sql, new[]
            {
                new SqliteParameter("@employeeId", employeeId),
                new SqliteParameter("@departmentId", departmentId)
            });
        }
    }
}
