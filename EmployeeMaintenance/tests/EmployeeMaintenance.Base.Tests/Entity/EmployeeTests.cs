using Bogus;
using EmployeeMaintenance.DL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace EmployeeMaintenance.Tests.Entity
{
    public class EmployeeTests
    {
        [Theory]
        [InlineData("2022-08-01", "2024-12-17", "2y – 4m – 16d")]
        [InlineData("2024-08-01", "2024-12-17", "0y – 4m – 16d")]
        [InlineData("2023-01-15", "2024-12-17", "1y – 11m – 2d")]
        [InlineData("2024-01-01", "2024-12-17", "0y – 11m – 16d")]
        [InlineData("2024-12-12", "2024-12-12", "0y – 0m – 0d")]
        public void CalculateHiredTime_Should_Return_Correct_String(string hireDateString, string nowString, string expected)
        {
            // Arrange
            var hireDate = DateTime.Parse(hireDateString);
            var now = DateTime.Parse(nowString);
            var employee = new Employee();

            // Act
            var result = employee.CalculateHiredTime(now, hireDate);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Employee_WithoutRequiredFields_ShouldFailValidation()
        {
            // Arrange
            var employee = new Employee();
            var validationContext = new ValidationContext(employee);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(employee, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("LastName"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Phone"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Address"));
        }

        [Fact]
        public void Employee_WithFieldsExceedingMaxLength_ShouldFailValidation()
        {
            // Arrange
            var employee = new Faker<Employee>()
                .RuleFor(e => e.Name, f => f.Random.String(129))
                .RuleFor(e => e.LastName, f => f.Random.String(129))
                .RuleFor(e => e.Address, f => f.Random.String(256))
                .RuleFor(e => e.Phone, f => f.Random.String(25))
                .Generate();

            var validationContext = new ValidationContext(employee);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(employee, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("LastName"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Phone"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Address"));
        }
    }
}
