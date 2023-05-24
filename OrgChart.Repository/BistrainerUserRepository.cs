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
using static Azure.Core.HttpHeader;

namespace OrgChart.Repository
{
    public interface IBistrainerUserRepository
    {
        public List<UserRepositoryModel> Users { get; }
    }
    public class BistrainerUserRepository : IBistrainerUserRepository
    {
        private readonly IMemoryCache _memoryCache;

        public BistrainerUserRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public List<UserRepositoryModel> Users
        {
            get
            {
                List<UserRepositoryModel> results = null!;

                if (!_memoryCache.TryGetValue("BistrainerUsers", out results))
                {
                    using (Database.Context.BistrainerContext dc = new Database.Context.BistrainerContext())
                    {
                        results = dc.Users.Select(s => new UserRepositoryModel
                        {
                            Id = s.Id,
                            FirstName = s.FirstName,
                            LastName = s.LastName,
                            EmployeeId = s.EmployeeId,
                            LocationId = s.LocationId,
                            ReportsToEmployeeId = s.ReportsToEmployeeId,
                            Status = s.Status,
                            CompanyRole = s.CompanyRole
                        }).ToList();

                        _memoryCache.Set("BistrainerUsers", results,
                            new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));

                    }
                }

                return (List<UserRepositoryModel>)_memoryCache.Get("BistrainerUsers");
            }
        }
    }
}