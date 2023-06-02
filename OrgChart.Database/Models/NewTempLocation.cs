using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrgChart.Database.Models;

[Table("NEW_TEMP_Location")]
public partial class NewTempLocation
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int ParentId { get; set; }

    [StringLength(20)]
    public string EmployeeId { get; set; } = null!;

    public string CompanyRole { get; set; } = null!;
}
