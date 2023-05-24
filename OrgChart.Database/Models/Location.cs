using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrgChart.Database.Models
{
    [Table("Location")]
    public partial class Location
    {
        [Key]
        public int LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public int ParentLocationId { get; set; }
        public string ParentLocationName { get; set; } = null!;
    }
}
