using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart.Business.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public DepartmentModel Department { get; set; } = null!;
        public int? ParentId { get; set; }
        public string Position { get; set; } = null!;
        public bool Active { get; set; }
        public string ImageUrl { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public string str_HireDate { get { return HireDate.ToString("yyyy-MM-dd"); } }        
    }
}
