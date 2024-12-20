using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.DL.Services.BLL;
using EmployeeMaintenance.DL.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMaintenance.Tests.Fixtures
{
    public class CacheFixture
    {
        public Mock<ICacheHelper> CacheHelperMock { get; }

        public CacheFixture()
        {
            CacheHelperMock = new Mock<ICacheHelper>();

            #region Employees Mock

            CacheHelperMock
                .Setup(ch => ch.GetOrCreateAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<ICacheEntry, Task<Employee>>>(),
                    It.IsAny<CacheOptions>()))
                .Returns<string, Func<ICacheEntry, Task<Employee>>, CacheOptions>(
                    (key, factoryMethod, options) => factoryMethod(Mock.Of<ICacheEntry>()));

            CacheHelperMock
                .Setup(ch => ch.GetOrCreateAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<ICacheEntry, Task<bool>>>(),
                    It.IsAny<CacheOptions>()))
                .Returns<string, Func<ICacheEntry, Task<bool>>, CacheOptions>(
                    (key, factoryMethod, options) => factoryMethod(Mock.Of<ICacheEntry>()));

             CacheHelperMock
                .Setup(ch => ch.GetOrCreate(
                    It.IsAny<string>(),
                    It.IsAny<Func<ICacheEntry, IEnumerable<EmployeeResponse>>>(),
                    It.IsAny<CacheOptions>()))
                .Returns<string, Func<ICacheEntry, IEnumerable<EmployeeResponse>>, CacheOptions>(
                    (key, factoryMethod, options) => factoryMethod(Mock.Of<ICacheEntry>()));

            #endregion

            #region Departments Mock

            CacheHelperMock
                .Setup(ch => ch.GetOrCreate(
                    "GetAllDepartments",
                    It.IsAny<Func<ICacheEntry, IEnumerable<DepartmentResponse>>>(),
                    It.IsAny<CacheOptions>()))
                .Returns<string, Func<ICacheEntry, IEnumerable<DepartmentResponse>>, CacheOptions>(
                    (key, factoryMethod, options) => factoryMethod(Mock.Of<ICacheEntry>()));

            CacheHelperMock
                .Setup(ch => ch.GetOrCreate(
                    It.Is<string>(s => s.StartsWith("GetDepartment")),
                    It.IsAny<Func<ICacheEntry, Task<Department>>>(),
                    It.IsAny<CacheOptions>()))
                .Returns<string, Func<ICacheEntry, Task<Department>>, CacheOptions>(
                    (key, factoryMethod, options) => factoryMethod(Mock.Of<ICacheEntry>()));

            #endregion
        }
    }
}
