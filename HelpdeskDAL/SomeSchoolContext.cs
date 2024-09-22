using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExercisesDAL;

public partial class HelpdeskDb : DbContext
{
    public HelpdeskDb()
    {
    }
    
    public HelpdeskDb(DbContextOptions<HelpdeskDb> options)
        : base(options)
    {
    }

    public virtual DbSet<Division> Divisions { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\ProjectModels;Database=SomeschoolDb;Trusted_Connection=True;");
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Division>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Division");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Timer)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Student");

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.Timer)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Title)
                .HasMaxLength(4)
                .IsUnicode(false);

            entity.HasOne(d => d.Division).WithMany(p => p.Students)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentInDiv");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
