using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart.Repository.Models
{
    public class HierarchyModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int Type { get; set; }
        public string Description { get; set; } = null!;
        public int EmployeeId { get; set; }
        public int Sequence { get; set; }
    }
}
