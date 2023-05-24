using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart.Repository.Models
{
    public class UserRepositoryModel
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string CompanyRole { get; set; } = null!;
        public int LocationId { get; set; }
        public string Status { get; set; } = null!;
        public string ReportsToEmployeeId { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
