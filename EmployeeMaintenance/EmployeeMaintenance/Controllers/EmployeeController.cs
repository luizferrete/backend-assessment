using EmployeeMaintenance.DL.Entities;
using EmployeeMaintenance.DL.Services.BLL;
using EmployeeMaintenance.DL.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeMaintenance.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class EmployeeController : ControllerBase
    {

        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            try
            {
                return Ok(await _employeeService.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<EmployeeResponse>> GetAllEmployees()
        {
            try
            {
                return Ok(_employeeService.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeRequest employee)
        {
            try
            {
                await _employeeService.Create(employee);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeRequest employee)
        {
            try
            {
                await _employeeService.Update(id, employee);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{employeeId}/department/{departmentId}")]
        public async Task<IActionResult> UpdateEmployeeDepartment(int employeeId, int departmentId)
        {
            try
            {
                await _employeeService.UpdateEmployeeDepartment(employeeId, departmentId);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                await _employeeService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
