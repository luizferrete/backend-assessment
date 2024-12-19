using EmployeeMaintenance.DAL.EF;
using EmployeeMaintenance.DL.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeMaintenance.Tests.Fixtures
{
    public class DbContextFixture : IDisposable
    {
        public EntityContext Context { get; private set; }

        public DbContextFixture()
        {
            ResetDatabase();
        }

        /// <summary>
        /// This ensures a fresh database is used in every test.
        /// </summary>
        public void ResetDatabase()
        {
            var options = new DbContextOptionsBuilder<EntityContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;

            Context = new EntityContext(options);
            Context.Database.EnsureCreated();

            //initial seeding
            Context.Departments.Add(new Department { Name = "Test Department", FlagActive = true });
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
