using System;
using System.Linq;
using System.Threading.Tasks;

using ApplicationCore.Data.GenericServise;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApplication1.Repositories.Implementations
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> RegisterAsync(LeaveRequest LeaveRequest)
        {
            if (await _context.LeaveRequests.AnyAsync(u => u.Id == LeaveRequest.Id))
                return false;

            _context.LeaveRequests.Add(LeaveRequest);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<LeaveRequest> GetByemploi(int id)
        {
            return await _context.LeaveRequests.FirstOrDefaultAsync(u => u.EmployeeId == id);
        }


        public async Task<LeaveRequest> GetByIdAsync(int id)
        {
            return await _context.LeaveRequests.FindAsync(id);
        }





        public async Task<bool> UpdateUserAsync(LeaveRequest LeaveRequest)
        {
            _context.LeaveRequests.Update(LeaveRequest);
            return await _context.SaveChangesAsync() > 0;
        }
    }

}