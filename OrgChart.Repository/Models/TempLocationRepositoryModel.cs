using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart.Repository.Models
{
    public class TempLocationRepositoryModel
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int ParentLocationId { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string CompanyRole { get; set; } = string.Empty;
    }
}
