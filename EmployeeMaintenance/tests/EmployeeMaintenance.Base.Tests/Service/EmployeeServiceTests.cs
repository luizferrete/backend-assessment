using AutoMapper;
using Bogus;
using EmployeeMaintenance.Tests.Fixtures;
using EmployeeMaintenance.BLL.Services;
using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.DL.Services.BLL;
using EmployeeMaintenance.DL.Services.DAL.Repositories;
using EmployeeMaintenance.DL.ValueObjects;
using Moq;

namespace EmployeeMaintenance.Tests.Service
{
    public class EmployeeServiceTests : IClassFixture<EmployeeFixture>
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IDepartmentService> _departmentServiceMock;
        private readonly Faker<EmployeeRequest> _employeeRequestFaker;
        private readonly EmployeeService _employeeService;
        private readonly Faker<Employee> _employeeFaker;
        private readonly IMapper _mapper;

        public EmployeeServiceTests(EmployeeFixture fixture)
        {
            _employeeRepositoryMock = fixture.EmployeeRepositoryMock;
            _departmentServiceMock = fixture.DepartmentServiceMock;
            _mapper = fixture.Mapper;

            _employeeRepositoryMock.Invocations.Clear();
            _departmentServiceMock.Invocations.Clear();

            _employeeRequestFaker = new Faker<EmployeeRequest>()
                .RuleFor(e => e.Name, f => f.Person.FirstName)
                .RuleFor(e => e.LastName, f => f.Person.LastName)
                .RuleFor(e => e.HireDate, f => f.Date.Past())
                .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber("##-####-####"))
                .RuleFor(e => e.DepartmentId, f => f.Random.Int(1, 5))
                .RuleFor(e => e.Address, f => f.Address.StreetAddress());

            _employeeFaker = new Faker<Employee>()
               .RuleFor(e => e.Id, f => f.Random.Int(1, 1000))
               .RuleFor(e => e.Name, f => f.Person.FirstName)
               .RuleFor(e => e.LastName, f => f.Person.LastName)
               .RuleFor(e => e.HireDate, f => f.Date.Past())
               .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber("##-####-####"))
               .RuleFor(e => e.DepartmentId, f => f.Random.Int(1, 5))
               .RuleFor(e => e.FlagActive, true)
               .RuleFor(e => e.Address, f => f.Address.StreetAddress());

            _employeeService = new EmployeeService(
                _employeeRepositoryMock.Object,
                _mapper,
                _departmentServiceMock.Object
            );
        }

        [Fact]
        public async Task GetById_Should_Return_Employee_When_Found()
        {
            // Arrange
            var employee = _employeeFaker.Generate();
            SetupFindEmployeeById(employee.Id, employee);

            // Act
            var result = await _employeeService.GetById(employee.Id);

            // Assert
            Assert.Equal(employee.Id, result.Id);
            Assert.Equal(employee.Name, result.Name);
        }

        [Fact]
        public async Task Create_Should_Insert_Employee_When_Valid()
        {
            // Arrange
            var employeeRequest = _employeeRequestFaker.Generate();

            SetupDepartmentExists(employeeRequest.DepartmentId);

            // Act
            await _employeeService.Create(employeeRequest);

            // Assert
            _employeeRepositoryMock.Verify(r => r.InsertAsync(It.Is<Employee>(e =>
                e.Name == employeeRequest.Name &&
                e.LastName == employeeRequest.LastName &&
                e.Phone == employeeRequest.Phone &&
                e.DepartmentId == employeeRequest.DepartmentId
            ), false), Times.Once);
        }

        [Fact]
        public async Task Create_Should_Throw_Exception_When_HireDate_In_Future()
        {
            // Arrange
            var employeeRequest = _employeeRequestFaker.Generate();
            employeeRequest.HireDate = DateTime.Now.AddDays(1);

            SetupDepartmentExists(employeeRequest.DepartmentId);

            // Act and assert
            await AssertThrowsAsync<Exception>(() => _employeeService.Create(employeeRequest), "The hire date can't be in the future.");
            _employeeRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Employee>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Create_Should_Throw_Exception_When_Phone_Invalid()
        {
            // Arrange
            var employeeRequest = _employeeRequestFaker.Generate();
            employeeRequest.Phone = "XXX";

            SetupDepartmentExists(employeeRequest.DepartmentId);

            //Act and assert
            await AssertThrowsAsync<Exception>(() => _employeeService.Create(employeeRequest), "Invalid phone number.");
            _employeeRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Employee>(), It.IsAny<bool>()), Times.Never);
        }


        [Theory]
        [InlineData("Name", 129, "Name exceeds the maximum length of 128 characters.")]
        [InlineData("LastName", 129, "Last name exceeds the maximum length of 128 characters.")]
        [InlineData("Address", 256, "Address exceeds the maximum length of 255 characters.")]
        public async Task Create_Should_Throw_Exception_When_Field_Exceeds_MaxLength(string field, int length, string expectedMessage)
        {
            var employeeRequest = _employeeRequestFaker.Generate();
            typeof(EmployeeRequest).GetProperty(field)?.SetValue(employeeRequest, new string('A', length));
            SetupDepartmentExists(employeeRequest.DepartmentId);

            await AssertThrowsAsync<Exception>(() => _employeeService.Create(employeeRequest), expectedMessage);
        }

        [Theory]
        [InlineData("Name", null, "Name cannot be null or empty.")]
        [InlineData("LastName", "", "Last name cannot be null or empty.")]
        [InlineData("Address", "", "Address cannot be null or empty.")]
        public async Task Create_Should_Throw_Exception_When_Field_Is_NullOrEmpty(string field, string value, string expectedMessage)
        {
            var employeeRequest = _employeeRequestFaker.Generate();
            typeof(EmployeeRequest).GetProperty(field)?.SetValue(employeeRequest, value);
            SetupDepartmentExists(employeeRequest.DepartmentId);

            await AssertThrowsAsync<Exception>(() => _employeeService.Create(employeeRequest), expectedMessage);
        }

        [Fact]
        public async Task Create_Should_Throw_Exception_When_Phone_Exceeds_MaxLength()
        {
            // Arrange
            var employeeRequest = _employeeRequestFaker.Generate();
            employeeRequest.Phone = new string('1', 25);

            SetupDepartmentExists(employeeRequest.DepartmentId);

            // Act & Assert
            await AssertThrowsAsync<Exception>(() => _employeeService.Create(employeeRequest), "Phone exceeds the maximum length of 24 characters.");
            _employeeRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Employee>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Update_Should_Update_Employee_When_Exists()
        {
            // Arrange
            var employee = _employeeFaker.Generate();
            var employeeRequest = _employeeRequestFaker.Generate();

            SetupEmployeeExists(employee.Id);
            SetupDepartmentExists(employeeRequest.DepartmentId);

            // Act
            await _employeeService.Update((int)employee.Id, employeeRequest);

            // Assert
            _employeeRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Employee>(e =>
                e.Id == employee.Id &&
                e.Name == employeeRequest.Name &&
                e.LastName == employeeRequest.LastName &&
                e.Address == employeeRequest.Address &&
                e.DepartmentId == employeeRequest.DepartmentId &&
                e.Phone == employeeRequest.Phone
            ), false), Times.Once);
        }

        [Fact]
        public async Task Update_Should_Throw_Exception_When_Employee_Not_Exists()
        {
            // Arrange
            var employeeRequest = _employeeRequestFaker.Generate();

            SetupEmployeeExists(It.IsAny<int>(), false);

            // Act & Assert
            await AssertThrowsAsync<KeyNotFoundException>(() => _employeeService.Update(999, employeeRequest), "Employee not found.");
            _employeeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Employee>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Delete_Should_Set_FlagActive_False_When_Employee_Exists()
        {
            // Arrange
            var employee = _employeeFaker.Generate();

            SetupFindEmployeeById(employee.Id, employee);

            _employeeRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Employee>()))
                          .Callback((Employee e) => e.FlagActive = false)
                          .Returns(Task.CompletedTask);

            // Act
            await _employeeService.Delete(employee.Id);

            // Assert
            _employeeRepositoryMock.Verify(r => r.DeleteAsync(It.Is<Employee>(e => e.Id == employee.Id)), Times.Once);
            Assert.False(employee.FlagActive);
        }

        [Fact]
        public async Task Delete_Should_Throw_Exception_When_Employee_Not_Found()
        {
            // Arrange
            SetupFindEmployeeById(It.IsAny<int>(), (Employee)null);

            // Act & Assert
            await AssertThrowsAsync<KeyNotFoundException>(() => _employeeService.Delete(999), "Employee not found.");
            _employeeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public void GetAll_Should_Return_Active_Employees()
        {
            // Arrange
            var employees = _employeeFaker.Generate(5);
            _employeeRepositoryMock.Setup(r => r.GetAll())
                                   .Returns(employees);

            // Act
            var results = _employeeService.GetAll().ToList();

            // Assert
            Assert.Equal(5, results.Count);
            Assert.Equal(employees.Select(e => $"{e.Name} {e.LastName}"), results.Select(r => r.Name));
        }

        [Fact]
        public async Task EmployeeExists_Should_Return_True_If_Exists()
        {
            // Arrange
            long id = 10;
            SetupEmployeeExists(id);

            // Act
            var result = await _employeeService.Exists(id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task EmployeeExists_Should_Return_False_If_Not_Exists()
        {
            // Arrange
            long id = 10;
            SetupEmployeeExists(id, false);

            // Act
            var result = await _employeeService.Exists(id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateEmployeeDepartment_Should_Update_When_Valid()
        {
            //Arrange
            var employeeId = 1L;
            var departmentId = 2L;

            SetupEmployeeExists(employeeId);
            SetupDepartmentExists(departmentId);

            // Act
            await _employeeService.UpdateEmployeeDepartment(employeeId, departmentId);

            // Assert
            _employeeRepositoryMock.Verify(r => r.ExistsAsync(employeeId), Times.Once);
            _departmentServiceMock.Verify(d => d.Exists(departmentId), Times.Once);
            _employeeRepositoryMock.Verify(r => r.UpdateEmployeeDepartment(employeeId, departmentId), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployeeDepartment_Should_Throw_Exception_When_Employee_Not_Found()
        {
            // Arrange
            var employeeId = 1L;
            var departmentId = 2L;

            SetupEmployeeExists(employeeId, false);

            // Act & Assert
            await AssertThrowsAsync<KeyNotFoundException>(() => _employeeService.UpdateEmployeeDepartment(employeeId, departmentId), "Employee not found.");
            _employeeRepositoryMock.Verify(r => r.UpdateEmployeeDepartment(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task UpdateEmployeeDepartment_Should_Throw_Exception_When_Department_Not_Found()
        {
            // Arrange
            var employeeId = 1L;
            var departmentId = 2L;

            SetupEmployeeExists(employeeId);
            SetupDepartmentExists(departmentId, false);

            // Act & Assert
            await AssertThrowsAsync<KeyNotFoundException>(() => _employeeService.UpdateEmployeeDepartment(employeeId, departmentId), "Department not found.");
            _employeeRepositoryMock.Verify(r => r.UpdateEmployeeDepartment(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
        }

        #region Private methods

        private void SetupDepartmentExists(long departmentId, bool exists = true) =>
            _departmentServiceMock.Setup(d => d.Exists(departmentId)).ReturnsAsync(exists);

        private void SetupEmployeeExists(long employeeId, bool exists = true) =>
            _employeeRepositoryMock.Setup(r => r.ExistsAsync(employeeId)).ReturnsAsync(exists);

        private void SetupFindEmployeeById(long employeeId, Employee employee) =>
            _employeeRepositoryMock.Setup(r => r.FindByIdAsync(employeeId)).ReturnsAsync(employee);
        
        private async Task AssertThrowsAsync<TException>(Func<Task> action, string expectedMessage) where TException : Exception
        {
            var ex = await Assert.ThrowsAsync<TException>(action);
            Assert.Equal(expectedMessage, ex.Message);
        }

        #endregion Private methods
    }
}