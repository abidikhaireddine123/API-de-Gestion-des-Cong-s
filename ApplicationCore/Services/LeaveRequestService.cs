

using ApplicationCore.Data.Entities;
using Microsoft.EntityFrameworkCore;

using WebApi.Models;
using System.Linq.Dynamic.Core;
using ApplicationCore.Services.IService;
namespace ApplicationCore.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeaveRequest>> GetAllAsync()
        {
            return await _context.LeaveRequests.ToListAsync();
        }

        public async Task<LeaveRequest> GetByIdAsync(int id)
        {
            return await _context.LeaveRequests.FindAsync(id);
        }

        public async Task<LeaveRequest> CreateAsync(LeaveRequest leaveRequest)
        {
            if (leaveRequest == null)
                throw new ArgumentNullException(nameof(leaveRequest), "La demande de congé ne peut pas être nulle.");

            // Validation des entrées
            if (leaveRequest.StartDate > leaveRequest.EndDate)
                throw new ArgumentException("La date de début ne peut pas être postérieure à la date de fin.");

            // Validation 1 : Pas de chevauchement des dates de congé par employé
            var hasOverlap = await _context.LeaveRequests
                .AnyAsync(lr => lr.EmployeeId == leaveRequest.EmployeeId
                             && lr.Status != "Rejeté" 
                             && lr.StartDate <= leaveRequest.EndDate
                             && lr.EndDate >= leaveRequest.StartDate);

            if (hasOverlap)
                throw new InvalidOperationException("La demande de congé chevauche une demande existante pour cet employé.");

            // Validation 2 : Maximum 20 jours de congé annuel par an (pour le type "Annuel")
            if (leaveRequest.LeaveType == "Annuel")
            {
                int currentYear = leaveRequest.StartDate.Year;
                var existingAnnualLeaves = await _context.LeaveRequests
                    .Where(lr => lr.EmployeeId == lr.EmployeeId
                              && lr.LeaveType == "Annuel"
                              && lr.Status == "Approuvé" // Optionnel : Ne compter que les congés approuvés
                              && lr.StartDate.Year == currentYear)
                    .ToListAsync();

                // Calculer le total des jours de congé annuel déjà pris
                int totalDaysTaken = existingAnnualLeaves
                    .Sum(lr => (lr.EndDate - lr.StartDate).Days + 1); // Inclusif des dates de début et de fin

                // Calculer les jours pour la nouvelle demande
                int newRequestDays = (leaveRequest.EndDate - leaveRequest.StartDate).Days + 1;

                if (totalDaysTaken + newRequestDays > 20)
                    throw new InvalidOperationException($"Impossible de dépasser 20 jours de congé annuel par an. Jours déjà pris : {totalDaysTaken}.");
            }

            // Validation 3 : Congé maladie nécessite une raison non vide
            if (leaveRequest.LeaveType == "Maladie" && string.IsNullOrWhiteSpace(leaveRequest.Reason))
                throw new ArgumentException("Une raison est requise pour les demandes de congé maladie.");

            // Définir l'horodatage de création et ajouter au contexte
            leaveRequest.CreatedAt = DateTime.UtcNow;
            leaveRequest.Status = leaveRequest.Status ?? "En attente"; // Optionnel : Statut par défaut
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            return leaveRequest;
        }

        public async Task<bool> UpdateAsync(int id, LeaveRequest updated)
        {
            if (updated == null)
                return false;

            var existing = await _context.LeaveRequests.FindAsync(id);
            if (existing == null)
                return false;

            existing.EmployeeId = updated.EmployeeId;
            existing.LeaveType = updated.LeaveType;
            existing.StartDate = updated.StartDate;
            existing.EndDate = updated.EndDate;
            existing.Status = updated.Status;
            existing.Reason = updated.Reason;
            existing.CreatedAt = DateTime.UtcNow; // Optional: Track update time

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request == null)
                return false;

            _context.LeaveRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Data.Entities.PagedResult<LeaveRequest>> GetFilteredAsync(
            int? employeeId,
            string? leaveType,
            string? status,
            DateTime? startDate,
            DateTime? endDate,
            string? keyword,
            int page,
            int pageSize,
            string sortBy,
            string sortOrder)
        {
            // Input validation for pagination
            if (page < 1)
                page = 1;
            if (pageSize < 1)
                pageSize = 10; // Default page size

            var query = _context.LeaveRequests.AsQueryable();

            // ----- Filtering
            if (employeeId.HasValue)
                query = query.Where(l => l.EmployeeId == employeeId);

            if (!string.IsNullOrWhiteSpace(leaveType))
                query = query.Where(l => l.LeaveType == leaveType);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(l => l.Status == status);

            if (startDate.HasValue)
                query = query.Where(l => l.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.EndDate <= endDate.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(l => l.Reason.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            // ----- Sorting (via Dynamic LINQ)
            var validProperties = typeof(LeaveRequest).GetProperties()
                .Select(p => p.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!validProperties.Contains(sortBy))
                sortBy = "CreatedAt";

            sortOrder = sortOrder?.ToLower() == "asc" ? "asc" : "desc";
            query = query.OrderBy($"{sortBy} {sortOrder}");

            // ----- Pagination
            var total = await query.CountAsync();
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Data.Entities.PagedResult<LeaveRequest>
            {
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                Items = data
            };
        }





        public async Task<LeaveReportResponse> GetLeaveReportAsync(
            int year,
            string? department
        )
        {
    
            var query = _context.Employees
                .Include(e => e.LeaveRequests)
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(department))
                query = query.Where(e => e.Department == department);

         
            var employees = await query.ToListAsync();

       
            var reportItems = employees.Select(employee =>
            {
               
                var leaveRequests = employee.LeaveRequests
                    .Where(lr => lr.Status == "Approved"
                              && lr.StartDate.Year == year)
               
                    .ToList();

              
                int totalLeaves = leaveRequests
                    .Sum(lr => (lr.EndDate - lr.StartDate).Days + 1); 
                int annualLeaves = leaveRequests
                    .Where(lr => lr.LeaveType == "Annual")
                    .Sum(lr => (lr.EndDate - lr.StartDate).Days + 1);
                int sickLeaves = leaveRequests
                    .Where(lr => lr.LeaveType == "Sick")
                    .Sum(lr => (lr.EndDate - lr.StartDate).Days + 1);

                return new LeaveReportItem
                {
                    EmployeeName = employee.FullName,
                    TotalLeaves = totalLeaves,
                    AnnualLeaves = annualLeaves,
                    SickLeaves = sickLeaves
                };
            })
            .Where(item => item.TotalLeaves > 0) 
            .OrderBy(item => item.EmployeeName) 
            .ToList();

            return new LeaveReportResponse
            {
                Items = reportItems
            };
        }





        public async Task<bool> ApproveAsync(int id)
        {
            // Rechercher la demande de congé
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null || leaveRequest.Status != "Pending")
                return false;

            // Mettre à jour le statut
            leaveRequest.Status = "Approved";


            await _context.SaveChangesAsync();
            return true;
        }
    }
}