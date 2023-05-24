using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart.Business.Models
{
    public class BistrainerLocationModel
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public int ParentLocationId { get; set; }
        public string ParentLocationName { get; set; } = null!;
        public int Level { get; set; }
        public BistrainerUserModel User { get; set; } = null!;

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

        public BistrainerLocationModel Parent { get; set; } = null!;
        public List<BistrainerLocationModel> Children { get; set; } = null!;

        public BistrainerLocationModel()
        {
            this.Children = new List<BistrainerLocationModel>();
        }
    }
}
