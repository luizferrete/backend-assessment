using Bogus;
using EmployeeMaintenance.DAL.Repositories;
using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.Tests.Fixtures;
using EmployeeMaintenance.Tests.Repository.Base;

namespace EmployeeMaintenance.Tests.Repository
{
    public class DepartmentRepositoryTests : EFRepositoryTestsBase<Department, DepartmentRepository>, IClassFixture<DbContextFixture>
    {
        public DepartmentRepositoryTests(DbContextFixture fixture) : base(fixture, new DepartmentRepository(fixture.Context))
        {
        }

        protected override Department CreateEntity(bool isActive = true)
        {
            return new Faker<Department>()
                .RuleFor(e => e.Name, f => f.Name.FirstName())
                .RuleFor(e => e.FlagActive, _ => isActive)
                .Generate();
        }

    }
}
