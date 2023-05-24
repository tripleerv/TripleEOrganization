using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart.Business.Models
{
    public class OrgChartModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public OrgEmployeeModel Employee { get; set; } = null!;
        public OrgDepartmentModel Department { get; set; } = null!;
    }
}
