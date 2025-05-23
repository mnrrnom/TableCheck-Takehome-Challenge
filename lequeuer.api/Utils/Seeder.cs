using lequeuer.api.Data;
using lequeuer.api.Models;

namespace lequeuer.api.Utils;

public class Seeder(DataContext context)
{
    public void Seed()
    {
        context.Database.EnsureCreated();

        if (context.Restaurants.Any()) return;
        
        context.Restaurants.AddRange(
            new Restaurant { Id = 1, Name = "Restaurant 1", AvailableSeats = 10 },
            new Restaurant { Id = 2, Name = "Restaurant 2", AvailableSeats = 10 }
        );

        context.SaveChanges();
    }
}