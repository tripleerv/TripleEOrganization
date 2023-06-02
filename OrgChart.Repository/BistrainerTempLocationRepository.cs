using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using OrgChart.Database.Context;
using OrgChart.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OrgChart.Repository
{
    public interface IBistrainerTempLocationRepository
    {
        public List<LocationRepositoryModel> Locations { get; }
        public List<string> Departments { get; }

        LocationRepositoryModel Add(int id, string name, int parent_id, string parent_name);
        LocationRepositoryModel Add(string name, int parent_id, string parent_name);
        LocationRepositoryModel Update(int id, string name, int parent_id, string parent_name);
        LocationRepositoryModel UpdateEmployee(int locationId, string employeeId);
        LocationRepositoryModel UpdateCompanyRole(int locationId, string companyRoles);
        bool Remove(int id);
    }
    public class BistrainerTempLocationRepository : IBistrainerTempLocationRepository
    {
        private readonly IMemoryCache _memoryCache;

        public BistrainerTempLocationRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public List<LocationRepositoryModel> Locations
        {
            get
            {
                List<LocationRepositoryModel> results = null!;

                if (!_memoryCache.TryGetValue("BistrainerLocations", out results))
                {
                    using (Database.Context.BistrainerContext dc = new Database.Context.BistrainerContext())
                    {
                        results = dc.NewTempLocations.Select(s => new LocationRepositoryModel
                        {
                            LocationId = s.Id,
                            LocationName = s.Name,
                            ParentLocationId = s.ParentId,
                            ParentLocationName = string.Empty,
                            EmployeeId = s.EmployeeId,
                            CompanyRole = s.CompanyRole
                        }).ToList();

                        _memoryCache.Set("BistrainerLocations", results,
                            new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromSeconds(5)));

                    }
                }

                return (List<LocationRepositoryModel>)_memoryCache.Get("BistrainerLocations");
            }
        }

        public List<string> Departments
        {
            get
            {
                List<string> results = new List<string>();

                foreach (var location in Locations)
                {
                    if (location.LocationName.Contains(":"))
                    {
                        var department = location.LocationName.Split(":")[0];

                        if (!string.IsNullOrEmpty(department))
                        {
                            if (department.StartsWith("D"))
                            {
                                if (!results.Any(x => x.Equals(department)))
                                {
                                    results.Add(department);
                                }
                            }
                        }
                    }
                }

                return results;
            }
        }

        public LocationRepositoryModel Add(string name, int parent_id, string parent_name)
        {
            using (BistrainerContext dc = new BistrainerContext())
            {
                var add = dc.NewTempLocations.Add(new Database.Models.NewTempLocation
                {
                    Name = name,
                    ParentId = parent_id,
                    EmployeeId = string.Empty,
                    CompanyRole = string.Empty
                });
                dc.SaveChanges();

                if (add == null)
                    return null!;

                if (_memoryCache.TryGetValue("BistrainerLocations", out var temp))
                    _memoryCache.Remove("BistrainerLocations");

                return new LocationRepositoryModel
                {
                    LocationId = add.Entity.Id,
                    LocationName = add.Entity.Name,
                    ParentLocationId = add.Entity.ParentId,
                    ParentLocationName = string.Empty,
                    EmployeeId = add.Entity.EmployeeId,
                    CompanyRole = add.Entity.CompanyRole
                };
            }
        }

        public LocationRepositoryModel Add(int id, string name, int parent_id, string parent_name)
        {
            using (BistrainerContext dc = new BistrainerContext())
            {
                var add = dc.NewTempLocations.Add(new Database.Models.NewTempLocation
                {
                    Id = id,
                    Name = name,
                    ParentId = parent_id,
                    EmployeeId = string.Empty,
                    CompanyRole = string.Empty
                });
                dc.SaveChanges();

                if (add == null)
                    return null!;

                if (_memoryCache.TryGetValue("BistrainerLocations", out var temp))
                    _memoryCache.Remove("BistrainerLocations");

                return new LocationRepositoryModel
                {
                    LocationId = add.Entity.Id,
                    LocationName = add.Entity.Name,
                    ParentLocationId = add.Entity.ParentId,
                    ParentLocationName = string.Empty,
                    EmployeeId = add.Entity.EmployeeId,
                    CompanyRole = add.Entity.CompanyRole
                };
            }
        }

        public bool Remove(int id)
        {
            using (BistrainerContext dc = new BistrainerContext())
            {
                var dc_location = dc.NewTempLocations.Find(id);

                if (dc_location == null)
                    return false;

                dc.NewTempLocations.Remove(dc_location);
                dc.SaveChanges();

                if (_memoryCache.TryGetValue("BistrainerLocations", out var temp))
                    _memoryCache.Remove("BistrainerLocations");

                return true;
            }
        }

        public LocationRepositoryModel Update(int id, string name, int parent_id, string parent_name)
        {
            using (BistrainerContext dc = new BistrainerContext())
            {
                var dc_location = dc.NewTempLocations.Find(id);

                if (dc_location == null)
                    return null!;

                dc_location.Name = name;
                dc_location.ParentId = parent_id;
                dc.SaveChanges();

                if (_memoryCache.TryGetValue("BistrainerLocations", out var temp))
                    _memoryCache.Remove("BistrainerLocations");

                return new LocationRepositoryModel
                {
                    LocationId = dc_location.Id,
                    LocationName = dc_location.Name,
                    ParentLocationId = dc_location.ParentId,
                    ParentLocationName = string.Empty,
                    EmployeeId = dc_location.EmployeeId,
                    CompanyRole = dc_location.CompanyRole
                };
            }
        }

        public LocationRepositoryModel UpdateEmployee(int locationId, string employeeId)
        {
            using (BistrainerContext dc = new BistrainerContext())
            {
                var dc_location = dc.NewTempLocations.Find(locationId);

                if (dc_location == null)
                    return null!;

                dc_location.EmployeeId = employeeId.Equals("0") ? string.Empty : employeeId;
                dc.SaveChanges();

                if (_memoryCache.TryGetValue("BistrainerLocations", out var temp))
                    _memoryCache.Remove("BistrainerLocations");

                return new LocationRepositoryModel
                {
                    LocationId = dc_location.Id,
                    LocationName = dc_location.Name,
                    ParentLocationId = dc_location.ParentId,
                    ParentLocationName = string.Empty,
                    EmployeeId = dc_location.EmployeeId,
                    CompanyRole = dc_location.CompanyRole
                };
            }
        }

        public LocationRepositoryModel UpdateCompanyRole(int locationId, string companyRoles)
        {
            using (BistrainerContext dc = new BistrainerContext())
            {
                var dc_location = dc.NewTempLocations.Find(locationId);

                if (dc_location == null)
                    return null!;

                dc_location.CompanyRole = companyRoles;
                dc.SaveChanges();

                if (_memoryCache.TryGetValue("BistrainerLocations", out var temp))
                    _memoryCache.Remove("BistrainerLocations");

                return new LocationRepositoryModel
                {
                    LocationId = dc_location.Id,
                    LocationName = dc_location.Name,
                    ParentLocationId = dc_location.ParentId,
                    ParentLocationName = string.Empty,
                    EmployeeId = dc_location.EmployeeId,
                    CompanyRole = dc_location.CompanyRole
                };
            }
        }

    }
}
