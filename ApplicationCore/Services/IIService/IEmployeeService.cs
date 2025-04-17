using System.Threading.Tasks;
using WebApi.Models;

namespace ApplicationCore.Services
{
    public interface IEmployeeService
    {
        Task<bool> RegisterAsync(Employee employee);
        Task<Employee> GetByNomAsync(string nom);
        Task<Employee> GetByIdAsync(int id);
        Task<bool> UpdateEmployeeAsync(Employee employee);
    }
}
