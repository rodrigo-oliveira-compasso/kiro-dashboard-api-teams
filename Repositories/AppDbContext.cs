using Microsoft.EntityFrameworkCore;
using TeamsApi.Models.Entities;

namespace TeamsApi.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Professional> Professionals => Set<Professional>();
    public DbSet<Squad> Squads => Set<Squad>();
    public DbSet<SquadProfessional> SquadProfessionals => Set<SquadProfessional>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Professional>(entity =>
        {
            entity.ToTable("professionals");
            entity.HasIndex(e => e.AwsUserId).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<Squad>(entity =>
        {
            entity.ToTable("squads");
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<SquadProfessional>(entity =>
        {
            entity.ToTable("squad_professionals");
            entity.HasKey(e => new { e.SquadId, e.ProfessionalId });

            entity.HasOne(e => e.Squad)
                .WithMany(s => s.SquadProfessionals)
                .HasForeignKey(e => e.SquadId);

            entity.HasOne(e => e.Professional)
                .WithMany(p => p.SquadProfessionals)
                .HasForeignKey(e => e.ProfessionalId);

            entity.Property(e => e.AssignedAt).HasDefaultValueSql("now()");
        });
    }
}
