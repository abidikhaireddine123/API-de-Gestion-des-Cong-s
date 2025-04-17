using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;
using ApplicationCore.Services.IService;
using AutoMapper;
using ApplicationCore.Data.Entities;

namespace WebApi.Controllers
{
    [Route("odata/[controller]")]
    public class LeaveRequestController : ODataController
    {
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly IMapper _mapper;

        public LeaveRequestController(ILeaveRequestService leaveRequestService, IMapper mapper)
        {
            _leaveRequestService = leaveRequestService;
            _mapper = mapper;
        }

        // GET odata/LeaveRequest
        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _leaveRequestService.GetAllAsync();
            return Ok(results.AsQueryable()); // pour OData
        }

        // GET odata/LeaveRequest(1)
        [EnableQuery]
        [HttpGet("({key})")]
        public async Task<IActionResult> GetById([FromRoute] int key)
        {
            var leaveRequest = await _leaveRequestService.GetByIdAsync(key);
            if (leaveRequest == null)
                return NotFound();

            return Ok(leaveRequest);
        }

        // POST odata/LeaveRequest
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LeaveRequestDto leaveRequestDto)
        {
            var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestDto);
            leaveRequest.CreatedAt = DateTime.UtcNow;

            var created = await _leaveRequestService.CreateAsync(leaveRequest);
            return Ok(created);
        }

        // PUT odata/LeaveRequest(1)
        [HttpPut("({key})")]
        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] LeaveRequest updatedRequest)
        {
            var success = await _leaveRequestService.UpdateAsync(key, updatedRequest);
            if (!success)
                return NotFound();

            return Updated(updatedRequest);
        }

        // DELETE odata/LeaveRequest(1)
        [HttpDelete("({key})")]
        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var success = await _leaveRequestService.DeleteAsync(key);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered(
            [FromQuery] int? employeeId,
            [FromQuery] string? leaveType,
            [FromQuery] string? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] string sortOrder = "desc")
        {
            var result = await _leaveRequestService.GetFilteredAsync(
                employeeId, leaveType, status, startDate, endDate,
                keyword, page, pageSize, sortBy, sortOrder);

            return Ok(result);
        }
        [HttpGet("report")]
        public async Task<ActionResult<LeaveReportResponse>> GetLeaveReport(
            [FromQuery] int year,
            [FromQuery] string? department = null)
      
        {
            // Valider l'année
            if (year < 2000 || year > DateTime.UtcNow.Year + 1)
                return BadRequest("Année invalide.");

            var report = await _leaveRequestService.GetLeaveReportAsync(year, department);
            return Ok(report);
        }


        [HttpPost("{id}/approve")]
        public async Task<ActionResult> ApproveLeaveRequest(int id)
        {
            var success = await _leaveRequestService.ApproveAsync(id);
            if (!success)
                return BadRequest("La demande n'existe pas ou n'est pas en attente.");

            return Ok("Demande de congé approuvée.");
        }

    }
}
