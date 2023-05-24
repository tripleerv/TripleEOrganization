using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OrgChart.Database.Models;

namespace OrgChart.Database.Context
{
    public partial class HRContext : DbContext
    {
        public HRContext()
        {
        }

        public HRContext(DbContextOptions<HRContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Hierarchy> Hierarchies { get; set; } = null!;
        public virtual DbSet<Job> Jobs { get; set; } = null!;
        public virtual DbSet<OrgDepartment> OrgDepartments { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=sqlsrv;Database=\"HR\";Integrated Security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Latin1_General_CI_AS");

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasDefaultValueSql("(N' ')");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Number).IsFixedLength();

                entity.Property(e => e.Plant).IsFixedLength();

                entity.Property(e => e.Province).IsFixedLength();
            });

            modelBuilder.Entity<OrgDepartment>(entity =>
            {
                entity.Property(e => e.DepartmentId).HasDefaultValueSql("('0')");

                entity.Property(e => e.Name).HasDefaultValueSql("('')");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
