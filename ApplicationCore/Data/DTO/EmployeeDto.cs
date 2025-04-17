using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class EmployeeDto
{


    public string FullName { get; set; } = null!;

    public string Department { get; set; } = null!;

    public DateOnly JoiningDate { get; set; }

   // public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
}
