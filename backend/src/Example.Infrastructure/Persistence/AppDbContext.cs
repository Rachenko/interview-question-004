using Example.Domain.Persons;
using Microsoft.EntityFrameworkCore;

namespace Example.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Person> Persons => Set<Person>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("persons");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(p => p.LastName).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Email).HasMaxLength(255).IsRequired();
            entity.Property(p => p.Phone).HasMaxLength(30).IsRequired();
            entity.Property(p => p.ProfileBase64).HasColumnType("text").IsRequired();
            entity.Property(p => p.BirthDay).HasColumnType("date").IsRequired();
            entity.Property(p => p.Occupation).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Sex).HasConversion<string>().HasMaxLength(10).IsRequired();
            entity.Property(p => p.CreatedAt).IsRequired();
        });
    }
}
