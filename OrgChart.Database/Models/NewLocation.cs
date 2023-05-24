using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrgChart.Database.Models;

[Table("NEW_Location")]
public partial class NewLocation
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public int ParentId { get; set; }

    [StringLength(50)]
    public string ParentName { get; set; } = null!;

    [StringLength(50)]
    public string EmployeeId { get; set; } = null!;

    public string CompanyRole { get; set; } = null!;
}
