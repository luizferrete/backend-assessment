using EmployeeMaintenance.DL.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMaintenance.DAL.EF
{
    public class EntityContext : DbContext
    {
        public EntityContext(DbContextOptions<EntityContext> options) : base(options) 
        {

        }

        public DbSet<Employee> Employees { get; set; } = default!;
        public DbSet<Department> Departments { get; set; } = default!;

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            EnableForeignKeysIfSQLite();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            EnableForeignKeysIfSQLite();
            return base.SaveChanges();
        }

        private void EnableForeignKeysIfSQLite()
        {
            // This is needed because SQLite does not apply foreign key integrity by default
            if (IsSqliteDatabase())
            {
                Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");
            }
        }

        private bool IsSqliteDatabase()
        {
            return Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite";
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Initial seeding only in the "production" database so it doesnt affect tests
            if(IsSqliteDatabase())
                InitialSeeding(modelBuilder);
        }

        private static DataBuilder<Department> InitialSeeding(ModelBuilder modelBuilder)
        {
            return modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    Name = "Human Resources",
                    FlagActive = true
                },
                new Department
                {
                    Id = 2,
                    Name = "IT",
                    FlagActive = true
                },
                new Department
                {
                    Id = 3,
                    Name = "Finance",
                    FlagActive = true
                }
            );
        }
    }
}
