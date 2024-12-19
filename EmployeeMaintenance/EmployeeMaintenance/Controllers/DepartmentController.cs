using EmployeeMaintenance.DL.Services.BLL;
using EmployeeMaintenance.DL.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeMaintenance.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentsService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentsService = departmentService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<DepartmentResponse>> GetAllDepartments()
        {
            try
            {
                return Ok(_departmentsService.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
