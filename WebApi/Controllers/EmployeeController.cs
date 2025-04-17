using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using WebApi.Models;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // Obtenir tous les employés
        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _employeeService.GetByIdAsync(1);
            return Ok(employees); // OData va gérer la requête
        }

        // Obtenir un employé par ID
        [EnableQuery]
        [HttpGet("({id})")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        // Enregistrer un nouvel employé
        [HttpPost]
        public async Task<IActionResult> RegisterEmployee([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest();
            }

            var result = await _employeeService.RegisterAsync(employee);
            if (result)
            {
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            return BadRequest();
        }

        // Mettre à jour un employé existant
        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            var result = await _employeeService.UpdateEmployeeAsync(employee);
            if (result)
            {
                return Ok(employee);
            }
            return NotFound();
        }
    }
}
