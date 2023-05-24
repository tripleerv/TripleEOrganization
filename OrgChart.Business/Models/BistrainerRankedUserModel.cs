using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart.Business.Models
{
    public class BistrainerRankedUserModel : BistrainerUserModel
    {
        private int _totalLocationCompanyRoles { get; set; }
        private int _totalUserMatchedCompanyRoles { get; set; }
        private double _totalMatchedPercentageCompanyRoles { get; set; }

        public double MatchedPercentage { get { return _totalMatchedPercentageCompanyRoles; } }
        public List<string> MatchedCompanyRoles { get; set; } = null!;
        public List<string> MissingCompanyRoles { get; set; } = null!;

        public BistrainerRankedUserModel(string employeeId, string firstName, string lastName, string companyRole, List<string> location_company_roles) 
            : base(employeeId, firstName, lastName, companyRole)
        {
            if (location_company_roles != null)
            {
                _totalLocationCompanyRoles = location_company_roles.Count;

                MatchedCompanyRoles = location_company_roles.Intersect(base.CompanyRoles).ToList();
                MissingCompanyRoles = location_company_roles.Except(base.CompanyRoles).ToList();

                _totalUserMatchedCompanyRoles = MatchedCompanyRoles.Count;
                _totalMatchedPercentageCompanyRoles = (double)_totalUserMatchedCompanyRoles / _totalLocationCompanyRoles * 100;

            }
        }
    }
}
