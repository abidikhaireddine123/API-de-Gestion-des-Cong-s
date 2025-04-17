using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Data.Entities
{
  
        public class PagedResult<T>
        {
            public int TotalCount { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public IEnumerable<T> Items { get; set; } = new List<T>();
        }
    public class LeaveReportItem
    {
        public string EmployeeName { get; set; } = string.Empty; 
        public int TotalLeaves { get; set; } 
        public int AnnualLeaves { get; set; }
        public int SickLeaves { get; set; } 
    }

    public class LeaveReportResponse
    {
        public List<LeaveReportItem> Items { get; set; } = new List<LeaveReportItem>(); 
    }
}
