using System;
using System.Linq;
using System.Collections.Generic;
using OrgChart.Database.Context;
using OrgChart.Repository.Models;
using Microsoft.Extensions.Caching.Memory;

namespace OrgChart.Repository
{
    public interface IEmployeeRepository
    {
        List<EmployeeModel> Employees { get; }
        EmployeeModel Employee(int id);
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IMemoryCache memoryCache;

        public EmployeeRepository(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public List<EmployeeModel> Employees
        {
            get
            {
                List<EmployeeModel> results = new List<EmployeeModel>();

                if (!memoryCache.TryGetValue("Employees", out results))
                {
                    using (HRContext dc = new HRContext())
                    {
                        //results = dc.Employees.Where(x => x.Active == true).Select(s => new EmployeeModel
                        results = dc.Employees.Select(s => new EmployeeModel
                        {
                            Id = s.Id,
                            Number = s.Number.Trim(),
                            FirstName = s.FirstName.Trim(),
                            LastName = s.LastName.Trim(),
                            SubDepartmentId = (int)s.DepartmentId,
                            DepartmentId = 0,
                            Position = s.Position,
                            Active = s.Active == true ? true : false,
                            ImageUrl = s.Photo,
                            HireDate = s.HireDate
                        }).ToList();
                    }

                    memoryCache.Set("Employees", results,
                            new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromSeconds(5)));
                }

                return (List<EmployeeModel>)memoryCache.Get("Employees");
            }
        }

        public EmployeeModel Employee(int id)
        {
            return Employees.FirstOrDefault(x => x.Id == id);
        }
    }
}
