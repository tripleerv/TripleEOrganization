using System;
using System.Linq;
using System.Collections.Generic;
using OrgChart.Database.Context;
using OrgChart.Repository.Models;
using Microsoft.Extensions.Caching.Memory;

namespace OrgChart.Repository
{
    public interface IHierarchyRepository
    {
        List<HierarchyModel> Hierarchies { get; }
        HierarchyModel Add(int type, string description, int employee_id, int parent_id, int sequence);
        HierarchyModel Update(int id, int type, string description, int employee_id, int parent_id, int sequence);
        bool Remove(int id);
    }

    public class HierarchyRepository : IHierarchyRepository
    {
        private readonly IMemoryCache memoryCache;

        public HierarchyRepository(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public List<HierarchyModel> Hierarchies
        {
            get
            {
                List<HierarchyModel> results = new List<HierarchyModel>();

                if (!memoryCache.TryGetValue("Hierarchy", out results))
                {
                    using (HRContext dc = new HRContext())
                    {
                        results = dc.Hierarchies.Select(s => new HierarchyModel
                        {
                            Id = s.Id,
                            ParentId = s.ParentId,
                            Type = s.Type,
                            Description = s.Description,
                            EmployeeId = s.EmployeeId,
                            Sequence = s.Sequence
                        }).ToList();
                    }

                    memoryCache.Set("Hierarchy", results,
                        new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(5)));
                }

                return (List<HierarchyModel>)memoryCache.Get("Hierarchy");               
            }
        }

        public HierarchyModel Add(int type, string description, int employee_id, int parent_id, int sequence)
        {
            using (HRContext dc = new HRContext())
            {
                var add = dc.Hierarchies.Add(new Database.Models.Hierarchy 
                { 
                    Type = type,
                    Description = description,
                    EmployeeId = employee_id,
                    ParentId = parent_id,
                    Sequence = sequence
                });
                dc.SaveChanges();

                if (add == null)
                    return null;

                if (memoryCache.TryGetValue("Hierarchy", out List<HierarchyModel> temp))
                {
                    memoryCache.Remove("Hierarchy");
                }

                return new HierarchyModel
                {
                    Id = add.Entity.Id,
                    Type = add.Entity.Type,
                    Description = add.Entity.Description,
                    EmployeeId = add.Entity.EmployeeId,
                    ParentId = add.Entity.ParentId,
                    Sequence = add.Entity.Sequence
                };
            }
        }

        public bool Remove(int id)
        {
            using (HRContext dc = new HRContext())
            {
                var remove = dc.Hierarchies.Find(id);

                if (remove == null)
                    return false;

                int parent_id = remove.ParentId;

                dc.Remove(remove);
                dc.SaveChanges();

                ReOrder(dc, parent_id);
            }

            if (memoryCache.TryGetValue("Hierarchy", out List<HierarchyModel> temp))
            {
                memoryCache.Remove("Hierarchy");
            }

            return true;
        }

        public HierarchyModel Update(int id, int type, string description, int employee_id, int parent_id, int sequence)
        {
            using (HRContext dc = new HRContext())
            {
                var update = dc.Hierarchies.FirstOrDefault(x => x.Id == id);

                if (update == null)
                    return null;

                bool reorder = update.Sequence != sequence;

                update.Type = type;
                update.Description = description;
                update.EmployeeId = employee_id;
                update.ParentId = parent_id;
                update.Sequence = sequence;

                dc.SaveChanges();

                if (reorder)
                {
                    ReOrder(dc, update.ParentId);
                }

                if (memoryCache.TryGetValue("Hierarchy", out List<HierarchyModel> temp))
                {
                    memoryCache.Remove("Hierarchy");
                }

                return new HierarchyModel
                {
                    Id = update.Id,
                    Type = update.Type,
                    Description = update.Description,
                    EmployeeId = update.EmployeeId,
                    ParentId = update.ParentId,
                    Sequence = update.Sequence
                };
            }
        }

        public void ReOrder(HRContext dc, int parent_id)
        {
            var children = dc.Hierarchies.Where(x => x.ParentId == parent_id).OrderBy(o => o.Sequence).ToList();

            if (children.Count != 0)
            {
                int seq = 10;
                foreach (var child in children)
                {
                    child.Sequence = seq;
                    seq += 10;
                }

                dc.SaveChanges();
            }        
        }
    }
}
