using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrgChart.Database.Models;

[Table("User")]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string EmployeeId { get; set; } = null!;

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(50)]
    public string UserName { get; set; } = null!;

    public string CompanyRole { get; set; } = null!;

    public int LocationId { get; set; }

    [StringLength(10)]
    public string Status { get; set; } = null!;

    [StringLength(50)]
    public string ReportsToEmployeeId { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
