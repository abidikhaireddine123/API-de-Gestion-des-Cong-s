﻿using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class LeaveRequest
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string? LeaveType { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? Status { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }
    public virtual Employee Employee { get; set; } = null!;
    // public virtual Employee Employee { get; set; } = null!;
}
