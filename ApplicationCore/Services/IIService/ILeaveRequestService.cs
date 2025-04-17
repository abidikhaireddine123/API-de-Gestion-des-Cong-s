using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Data.Entities;
using WebApi.Models;

namespace ApplicationCore.Services.IService
{
    public interface ILeaveRequestService
    {
        Task<IEnumerable<LeaveRequest>> GetAllAsync();
        Task<LeaveRequest> GetByIdAsync(int id);
        Task<LeaveRequest> CreateAsync(LeaveRequest leaveRequest);
        Task<bool> UpdateAsync(int id, LeaveRequest leaveRequest);
        Task<PagedResult<LeaveRequest>> GetFilteredAsync(
            int? employeeId,
            string? leaveType,
            string? status,
            DateTime? startDate,
            DateTime? endDate,
            string? keyword,
            int page,
            int pageSize,
            string sortBy,
            string sortOrder);
        Task<bool> DeleteAsync(int id);
        Task<LeaveReportResponse> GetLeaveReportAsync(
            int year,
            string? department
        );
        Task<bool> ApproveAsync(int id);

    }
}