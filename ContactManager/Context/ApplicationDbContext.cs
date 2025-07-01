using Microsoft.EntityFrameworkCore;
using Alfasoft.Models;

namespace Alfasoft;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Contact> Contacts { get; set; }

    // Optional: configure constraints via Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Contact>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<Contact>()
            .Property(c => c.ContactNumber)
            .IsRequired();

        modelBuilder.Entity<Contact>()
            .Property(c => c.Email)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}
