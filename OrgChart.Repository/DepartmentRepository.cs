using System;
using System.Linq;
using System.Collections.Generic;
using OrgChart.Repository.Models;
using OrgChart.Database.Context;

namespace OrgChart.Repository
{
    public interface IDepartmentRepository
    {
        List<DepartmentModel> Departments { get; }
    }

    public class DepartmentRepository : IDepartmentRepository
    {
        public List<DepartmentModel> Departments
        {
            get
            {
                using (HRContext dc = new HRContext())
                {
                    return dc.Departments.Select(s => new DepartmentModel
                    {
                        DepartmentId = s.Id,
                        DepartmentName = s.Name
                    }).ToList();
                }
            }
        }
    }
}
