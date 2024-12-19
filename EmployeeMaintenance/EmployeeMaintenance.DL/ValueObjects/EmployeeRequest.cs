using EmployeeMaintenance.DL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMaintenance.DL.ValueObjects
{
    public class EmployeeRequest
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        public DateTime HireDate { get; set; }

        public long DepartmentId { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }
    }
}
