using EmployeeMaintenance.DL.Services.DAL.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMaintenance.Tests.Fixtures
{
    public class DepartmentFixture : BaseFixture
    {
        public Mock<IDepartmentRepository> DepartmentRepositoryMock { get; private set; }

        public DepartmentFixture()
        {
            DepartmentRepositoryMock = new Mock<IDepartmentRepository>();
        }
    }
}
