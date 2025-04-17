using Microsoft.AspNetCore.OData.Results;
using System;
using System.Threading.Tasks;

using WebApi.Models;
using WebApplication1.Repositories.Implementations;

namespace ApplicationCore.Data.GenericServise
{
    public interface ILeaveRequestRepository
    {
        Task<bool> RegisterAsync(LeaveRequest LeaveRequest);
        Task<LeaveRequest> GetByemploi(int id);
      
        Task<LeaveRequest> GetByIdAsync(int id);

  

        // Nouvelles méthodes ajoutées

        Task<bool> UpdateUserAsync(LeaveRequest LeaveRequest);
    }
}