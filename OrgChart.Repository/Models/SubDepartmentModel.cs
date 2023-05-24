using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart.Repository.Models
{
    public class SubDepartmentModel
    {
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }
        public string SubDepartmentName { get; set; } = null!;
    }
}
