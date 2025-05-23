using lequeuer.api.Models;

using Microsoft.EntityFrameworkCore;

namespace lequeuer.api.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public required DbSet<Restaurant> Restaurants { get; set; }
    public required DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        ConfigureGlobalFilters(b);
        base.OnModelCreating(b);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder b)
    {
        b.Properties<decimal>().HavePrecision(15, 5);
        b.Properties<DateTime>().HavePrecision(0);
        b.Properties<Enum>().HaveConversion<string>();
        base.ConfigureConventions(b);
    }
    
    private static void ConfigureGlobalFilters(ModelBuilder b)
    {
        b.Entity<Reservation>().HasQueryFilter(x => x.DeletedAt == null);
        b.Entity<Restaurant>().HasQueryFilter(x => x.DeletedAt == null);
    }
}