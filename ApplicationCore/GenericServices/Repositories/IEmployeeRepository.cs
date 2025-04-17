using System;
using System.Threading.Tasks;

using WebApi.Models;

namespace ApplicationCore.Data.GenericServise
{
    public interface IEmployeeRepository
    {
        Task<bool> RegisterAsync(Employee utilisateur);
        Task<Employee> GetByNomAsync(string nom);
      
        Task<Employee> GetByIdAsync(int id);
      

        // Nouvelles méthodes ajoutées
        
        Task<bool> UpdateUserAsync(Employee user);
    }
}