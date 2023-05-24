using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrgChart.Database.Models
{
    [Table("Hierarchy")]
    public partial class Hierarchy
    {
        [Key]
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int Type { get; set; }
        [StringLength(100)]
        public string Description { get; set; } = null!;
        public int EmployeeId { get; set; }
        public int Sequence { get; set; }
    }
}
