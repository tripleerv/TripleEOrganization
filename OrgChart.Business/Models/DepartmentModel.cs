using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart.Business.Models
{
    public class DepartmentModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public int SubDepartmentId { get; set; }
        public string SubDepartmentName { get; set; } = null!;

        public DepartmentModel()
        {
            DepartmentId = 0;
            DepartmentName = string.Empty;
            SubDepartmentId = 0;
            SubDepartmentName = string.Empty;
        }
    }
}
