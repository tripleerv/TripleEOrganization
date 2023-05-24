using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart.Business.Models
{
    public class BistrainerUserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public string EmployeeId { get; set; } = null!;
        public string ReportsToEmployeeId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int LocationId { get; set; }
        public BistrainerLocationModel Location { get; set; } = null!;
        public int Level { get; set; }

        public string CompanyRole { get; set; } = string.Empty;
        public List<string> CompanyRoles
        {
            get
            {
                if (string.IsNullOrEmpty(CompanyRole))
                {
                    return new List<string>();
                }
                else
                {
                    var temp = CompanyRole.Split("|");

                    if (temp != null)
                        return temp.ToList();

                    return new List<string>();
                }
            }
        }

        public BistrainerUserModel Parent { get; set; } = null!;
        public List<BistrainerUserModel> Children { get; set; } = null!;

        public BistrainerUserModel(string employeeId, string firstName, string lastName, string companyRole)
        {
            EmployeeId = employeeId;
            FirstName = firstName;
            LastName = lastName;
            CompanyRole = companyRole;

            Children = new List<BistrainerUserModel>();
        }
    }
}
