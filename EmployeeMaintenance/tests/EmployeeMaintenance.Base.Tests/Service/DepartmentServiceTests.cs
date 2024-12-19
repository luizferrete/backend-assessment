using AutoMapper;
using Bogus;
using EmployeeMaintenance.BLL.Services;
using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.DL.Services.BLL;
using EmployeeMaintenance.DL.Services.DAL.Repositories;
using EmployeeMaintenance.DL.ValueObjects;
using EmployeeMaintenance.Tests.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMaintenance.Tests.Service
{
    public class DepartmentServiceTests : IClassFixture<DepartmentFixture>
    {
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;
        private readonly Faker<Department> _departmentFaker;

        public DepartmentServiceTests(DepartmentFixture fixture)
        {
            _departmentRepositoryMock = fixture.DepartmentRepositoryMock;
            _departmentRepositoryMock.Invocations.Clear();
            _mapper = fixture.Mapper;

            _departmentService = new DepartmentService(_departmentRepositoryMock.Object, _mapper);

            _departmentFaker = new Faker<Department>()
                .RuleFor(e => e.Id, f => f.Random.Int(1, 1000))
                .RuleFor(e => e.Name, f => f.Lorem.Word())
                .RuleFor(e => e.FlagActive, true);
        }


        [Fact]
        public async Task GetById_Should_Return_Department_When_Found()
        {
            // Arrange
            var department = _departmentFaker.Generate();
            _departmentRepositoryMock.Setup(r => r.FindByIdAsync(department.Id))
                                 .ReturnsAsync(department);

            // Act
            var result = await _departmentService.GetById(department.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(department.Id, result.Id);
            Assert.Equal(department.Name, result.Name);
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_NotFound()
        {
            // Arrange
            _departmentRepositoryMock.Setup(r => r.FindByIdAsync(It.IsAny<long>()))
                                     .ReturnsAsync((Department)null);

            // Act
            var result = await _departmentService.GetById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Exists_Should_Return_True_If_Department_Exists()
        {
            // Arrange
            long id = 10;
            _departmentRepositoryMock.Setup(r => r.ExistsAsync(id))
                                     .ReturnsAsync(true);

            // Act
            var result = await _departmentService.Exists(id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Exists_Should_Return_False_If_Department_Not_Exists()
        {
            // Arrange
            long id = 999;
            _departmentRepositoryMock.Setup(r => r.ExistsAsync(id))
                                     .ReturnsAsync(false);

            // Act
            var result = await _departmentService.Exists(id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetAll_Should_Return_Active_Departments()
        {
            // Arrange
            var departments = _departmentFaker.Generate(5);
            _departmentRepositoryMock.Setup(r => r.GetAll())
                                     .Returns(departments);

            // Act
            var results = _departmentService.GetAll().ToList();

            // Assert
            Assert.Equal(5, results.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(departments[i].Name, results[i].Name);
            }
        }

        [Fact]
        public void GetAll_Should_Return_Empty_When_Only_Inactive_Departments()
        {
            //Arrange
            var departments = _departmentFaker.Generate(5);
            foreach (var department in departments)
            {
                department.FlagActive = false;
            }
            _departmentRepositoryMock.Setup(r => r.GetAll())
                         .Returns(departments.Where(d => d.FlagActive).ToList());

            // Atc
            var results = _departmentService.GetAll().ToList();

            // Assert
            Assert.Empty(results);
        }
    }
}
