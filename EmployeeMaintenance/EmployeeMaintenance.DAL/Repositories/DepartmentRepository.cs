using EmployeeMaintenance.DAL.EF;
using EmployeeMaintenance.DAL.EF.Base;
using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.DL.Services.DAL.Repositories;

namespace EmployeeMaintenance.DAL.Repositories
{
    public class DepartmentRepository : EFRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(EntityContext db) : base(db)
        {
        }
    }
}
