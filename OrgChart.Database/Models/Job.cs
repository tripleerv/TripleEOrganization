using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrgChart.Database.Models
{
    [Table("Job")]
    public partial class Job
    {
        [Key]
        public int JobId { get; set; }
        public int DepartmentId { get; set; }
        public int WorkcenterId { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
    }
}
