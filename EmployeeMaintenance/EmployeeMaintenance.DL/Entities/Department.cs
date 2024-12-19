using EmployeeMaintenance.DL.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMaintenance.DL.Entities
{
    public class Department : EntityBase
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }
    }
}
