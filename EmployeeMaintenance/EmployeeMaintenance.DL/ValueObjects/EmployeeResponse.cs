namespace EmployeeMaintenance.DL.ValueObjects
{
    public class EmployeeResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string HireDate { get; set; }
        public required string HiredTime { get; set; }
        public required string Department { get; set; }
        public int DepartmentId { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
    }
}
