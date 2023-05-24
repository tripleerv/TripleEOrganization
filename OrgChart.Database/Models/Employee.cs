using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrgChart.Database.Models
{
    [Table("Employee")]
    public partial class Employee
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [StringLength(10)]
        public string? Number { get; set; }
        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }
        public int? PlantId { get; set; }
        [StringLength(10)]
        public string? Plant { get; set; }
        public int? DepartmentId { get; set; }
        public bool? Active { get; set; }
        [StringLength(100)]
        public string? Photo { get; set; }
        public bool? IsEmployee { get; set; }
        public bool? IsInstructor { get; set; }
        [Column("EMail")]
        [StringLength(50)]
        public string? Email { get; set; }
        [StringLength(50)]
        public string? SecondName { get; set; }
        [StringLength(50)]
        public string? Position { get; set; }
        public int? Type { get; set; }
        [StringLength(50)]
        public string? NickName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? HireDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SeniorityDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ReviewDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
        public int? Gender { get; set; }
        public int? WorkcenterId { get; set; }
        public int? JobId { get; set; }
        public int? ReportsToId { get; set; }
        public int PeriodOffset { get; set; }
        [StringLength(50)]
        public string? Username { get; set; }
        [StringLength(50)]
        public string? Password { get; set; }
        [Column("EMail2")]
        [StringLength(50)]
        public string? Email2 { get; set; }
        [StringLength(50)]
        public string? HomePhone { get; set; }
        [StringLength(50)]
        public string? CellPhone { get; set; }
        [StringLength(50)]
        public string? SpousePhone { get; set; }
        [StringLength(100)]
        public string? Address { get; set; }
        [StringLength(50)]
        public string? City { get; set; }
        [StringLength(2)]
        public string? Province { get; set; }
        [StringLength(50)]
        public string? PostalCode { get; set; }
        [Column("EC1Name")]
        [StringLength(50)]
        public string? Ec1name { get; set; }
        [Column("EC1Relationship")]
        [StringLength(50)]
        public string? Ec1relationship { get; set; }
        [Column("EC1HomePhone")]
        [StringLength(50)]
        public string? Ec1homePhone { get; set; }
        [Column("EC1CellPhone")]
        [StringLength(50)]
        public string? Ec1cellPhone { get; set; }
        [Column("EC1WorkPhone")]
        [StringLength(50)]
        public string? Ec1workPhone { get; set; }
        [Column("EC2Name")]
        [StringLength(50)]
        public string? Ec2name { get; set; }
        [Column("EC2Relationship")]
        [StringLength(50)]
        public string? Ec2relationship { get; set; }
        [Column("EC2HomePhone")]
        [StringLength(50)]
        public string? Ec2homePhone { get; set; }
        [Column("EC2CellPhone")]
        [StringLength(50)]
        public string? Ec2cellPhone { get; set; }
        [Column("EC2WorkPhone")]
        [StringLength(50)]
        public string? Ec2workPhone { get; set; }
        public DateTime? LastConnectDate { get; set; }
    }
}
