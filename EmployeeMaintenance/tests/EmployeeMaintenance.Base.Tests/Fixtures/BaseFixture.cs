using AutoMapper;
using EmployeeMaintenance.BLL.AutoMapper;
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
    public class BaseFixture
    {
        public IMapper Mapper { get; private set; }

        public BaseFixture()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ConfigMapper>();
            });
            Mapper = config.CreateMapper();
        }
    }
}
