using System;
using System.Linq;
using System.Threading.Tasks;

using ApplicationCore.Data.GenericServise;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApplication1.Repositories.Implementations
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> RegisterAsync(Employee utilisateur)
        {
            if (await _context.Employees.AnyAsync(u => u.FullName == utilisateur.FullName))
                return false;

            _context.Employees.Add(utilisateur);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Employee> GetByNomAsync(string Fullname)
        {
            return await _context.Employees.FirstOrDefaultAsync(u => u.FullName == Fullname);
        }

  
        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

       

        public async Task<Employee> GetByEmailAsync(int id)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(u => u.Id == id );
        }

        public async Task<bool> UpdateUserAsync(Employee user)
        {
            _context.Employees.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}