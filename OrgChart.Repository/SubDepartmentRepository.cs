using System;
using System.Linq;
using System.Collections.Generic;
using OrgChart.Repository.Models;
using OrgChart.Database.Context;

namespace OrgChart.Repository
{
    public interface ISubDepartmentRepository
    {
        List<SubDepartmentModel> SubDepartments { get; }
    }

    public class SubDepartmentRepository : ISubDepartmentRepository
    {
        public List<SubDepartmentModel> SubDepartments
        {
            get
            {
                using (HRContext dc = new HRContext())
                {
                    return dc.OrgDepartments.Select(s => new SubDepartmentModel
                    {
                        SubDepartmentId = s.Id,
                        SubDepartmentName = s.Name,
                        DepartmentId = s.DepartmentId
                    }).ToList();
                }
            }
        }
    }
}
