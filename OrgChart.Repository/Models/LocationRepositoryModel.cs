using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart.Repository.Models
{
    public class LocationRepositoryModel
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public int ParentLocationId { get; set; }
        public string ParentLocationName { get; set; } = null!;
        public string EmployeeId { get; set; } = null!;
        public string CompanyRole { get; set; } = null!;
    }
}
