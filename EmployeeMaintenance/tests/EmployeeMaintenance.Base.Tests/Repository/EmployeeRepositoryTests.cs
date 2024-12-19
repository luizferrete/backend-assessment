using Bogus;
using EmployeeMaintenance.DAL.Repositories;
using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.Tests.Fixtures;
using EmployeeMaintenance.Tests.Repository.Base;

namespace EmployeeMaintenance.Tests.Repository
{
    public class EmployeeRepositoryTests : EFRepositoryTestsBase<Employee, EmployeeRepository>, IClassFixture<DbContextFixture>
    {
        
        public EmployeeRepositoryTests(DbContextFixture fixture) : base(fixture, new EmployeeRepository(fixture.Context))
        {
        }

        protected override Employee CreateEntity(bool isActive = true)
        {
            var department = _context.Departments.First();
            return new Faker<Employee>()
                .RuleFor(e => e.Name, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.Address, f => f.Address.StreetAddress())
                .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber("##-####-####"))
                .RuleFor(e => e.DepartmentId, f => department.Id)
                .RuleFor(e => e.FlagActive, _ => isActive)
                .RuleFor(e => e.HireDate, f => f.Date.Past(5))
                .Generate();
        }
    }
}
