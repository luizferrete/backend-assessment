using EmployeeMaintenance.DL.Services.BLL;
using EmployeeMaintenance.DL.Services.DAL.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMaintenance.Tests.Fixtures
{
    public class EmployeeFixture : BaseFixture
    {
        public Mock<IEmployeeRepository> EmployeeRepositoryMock { get; private set; }
        public Mock<IDepartmentService> DepartmentServiceMock { get; private set; }

        public EmployeeFixture()
        {
            EmployeeRepositoryMock = new Mock<IEmployeeRepository>();
            DepartmentServiceMock = new Mock<IDepartmentService>();
        }
    }
}
