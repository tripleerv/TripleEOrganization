using System;
using System.Linq;
using System.Collections.Generic;
using OrgChart.Repository.Models;
using OrgChart.Database.Context;

namespace OrgChart.Repository
{
    public interface IJobRepository
    {
        List<JobModel> Jobs { get; }
    }

    public class JobRepository : IJobRepository
    {
        public List<JobModel> Jobs
        {
            get
            {
                using (HRContext dc = new HRContext())
                {
                    return dc.Jobs.Select(s => new JobModel 
                    { 
                        Id = s.JobId,
                        Name = s.Name
                    }).ToList();
                }
            }
        }
    }
}
