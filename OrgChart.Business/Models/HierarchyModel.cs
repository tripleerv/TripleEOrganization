using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart.Business.Models
{
    public class HierarchyModel
    {
        public int Id { get; set; }
        public EHierarchyType Type { get; set; }
        public string Description { get; set; }
        public EmployeeModel Employee { get; set; }
        public int Level { get; set; }
        public int Sequence { get; set; }
        public bool Duplicate { get; set; }

        public HierarchyModel Parent { get; set; }
        public List<HierarchyModel> Children { get; set; }

        public HierarchyModel()
        {
            this.Children = new List<HierarchyModel>();
        }
    }
}
