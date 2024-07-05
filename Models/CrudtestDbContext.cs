using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ApiTest.Models;

public partial class CrudtestDbContext : DbContext
{
    public CrudtestDbContext()
    {
    }

    public CrudtestDbContext(DbContextOptions<CrudtestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblComment> TblComments { get; set; }

    public virtual DbSet<TblPost> TblPosts { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=127.0.0.1;Database=CRUDTestDB;User Id=SA;Password=MsSql123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK_cmt");

            entity.HasOne(d => d.Post).WithMany(p => p.TblComments)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_post");

            entity.HasOne(d => d.User).WithMany(p => p.TblComments)
                .HasConstraintName("FK_cmmtusr");
        });

        modelBuilder.Entity<TblPost>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK_post");

            entity.HasOne(d => d.User).WithMany(p => p.TblPosts)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_usr");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_user");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
