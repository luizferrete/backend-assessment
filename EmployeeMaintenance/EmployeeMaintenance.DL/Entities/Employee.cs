using EmployeeMaintenance.DL.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeMaintenance.DL.Entities
{
    public class Employee : EntityBase
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [Required]
        [MaxLength(128)]
        public string LastName { get; set; }

        public DateTime HireDate { get; set; }

        [ForeignKey("Department")]
        public long DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        [Required]
        [MaxLength(24)] 
        public string Phone { get; set; }

        [Required]
        [MaxLength(255)]
        public string Address { get; set; }

        public string CalculateHiredTime(DateTime now, DateTime hireDate)
        {
            var years = now.Year - hireDate.Year;
            var months = now.Month - hireDate.Month;
            var days = now.Day - hireDate.Day;

            if (days < 0)
            {
                months--;
                days += DateTime.DaysInMonth(hireDate.Year, hireDate.Month);
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            return $"{years}y – {months}m – {days}d";
        }
    }
}
