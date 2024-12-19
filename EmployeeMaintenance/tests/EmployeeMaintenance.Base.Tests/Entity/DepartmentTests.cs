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
    public class DepartmentTests
    {
        private readonly Faker _faker = new Faker();

        [Fact]
        public void Department_WithoutName_ShouldFailValidation()
        {
            // Arrange
            var department = new Department
            {
                Name = null // Campo obrigatório ausente
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(department);

            // Act
            var isValid = Validator.TryValidateObject(department, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Department_WithNameExceedingMaxLength_ShouldFailValidation()
        {
            // Arrange
            var department = new Department
            {
                Name = _faker.Random.String(129) // Excede os 128 caracteres
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(department);

            // Act
            var isValid = Validator.TryValidateObject(department, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Department_WithValidName_ShouldPassValidation()
        {
            // Arrange
            var department = new Department
            {
                Name = _faker.Lorem.Word() // Gera um nome válido com Faker
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(department);

            // Act
            var isValid = Validator.TryValidateObject(department, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }
    }
}
