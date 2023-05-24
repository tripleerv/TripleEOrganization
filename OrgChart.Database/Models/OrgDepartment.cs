using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrgChart.Database.Models
{
    [Table("OrgDepartment")]
    public partial class OrgDepartment
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
        public int DepartmentId { get; set; }
    }
}
