using lequeuer.api.Data;
using lequeuer.api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace lequeuer.test.Utils;

public class DatabaseSeeder(IServiceProvider serviceProvider)
{
    public async Task SeedAsync()
    {
        using var scope = serviceProvider.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        await context.Database.EnsureCreatedAsync();
        await context.Restaurants.AddRangeAsync(
            new Restaurant
            {
                Id = 1,
                Name = "Restaurant 1",
                AvailableSeats = 10
            },
            new Restaurant
            {
                Id = 2,
                Name = "Restaurant 2",
                AvailableSeats = 10
            }
        );
        
        await context.SaveChangesAsync();
    }
}