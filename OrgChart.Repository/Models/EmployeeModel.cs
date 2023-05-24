using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart.Repository.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }
        public int? ParentId { get; set; }
        public string Position { get; set; } = null!;
        public bool Active { get; set; }
        public string ImageUrl { get; set; } = null!;
        public DateTime? HireDate { get; set; }
    }
}
