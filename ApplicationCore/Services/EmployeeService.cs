using System.Threading.Tasks;
using WebApi.Models;

using ApplicationCore.Data.GenericServise;

namespace ApplicationCore.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<bool> RegisterAsync(Employee employee)
        {
            // Vous pouvez ajouter ici des validations ou de la logique métier supplémentaire
            return await _employeeRepository.RegisterAsync(employee);
        }

        public async Task<Employee> GetByNomAsync(string nom)
        {
            return await _employeeRepository.GetByNomAsync(nom);
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            // Vous pouvez ajouter des vérifications ou de la logique métier
            return await _employeeRepository.UpdateUserAsync(employee);
        }
    }
}
