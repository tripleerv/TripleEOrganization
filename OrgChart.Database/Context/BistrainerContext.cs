using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OrgChart.Database.Models;

namespace OrgChart.Database.Context;

public partial class BistrainerContext : DbContext
{
    public BistrainerContext()
    {
    }

    public BistrainerContext(DbContextOptions<BistrainerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<NewLocation> NewLocations { get; set; }

    public virtual DbSet<NewTempLocation> NewTempLocations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=sql-02;Database=\"BistrainerDataWarehouse\";TrustServerCertificate=True; Integrated Security=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NewTempLocation>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_User_1");

            entity.Property(e => e.Status).IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
