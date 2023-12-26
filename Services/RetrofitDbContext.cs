using Microsoft.EntityFrameworkCore;
using Retrofit.Data;

namespace Retrofit.Services;

public class RetrofitDbContext : DbContext
{
    public DbSet<RetrofitEntry> Entries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configure your PostgreSQL connection string here
        optionsBuilder.UseNpgsql("Host=localhost;Database=retrofitdb;Username=postgres;Password=postgres",
            o => o.UseVector());
    }
}