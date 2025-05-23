using lequeuer.api.Data;
using lequeuer.api.Models;

using Microsoft.EntityFrameworkCore;

namespace lequeuer.api.Modules.RestaurantModule;

public interface IRestaurantsService
{
    Task<Restaurant?> GetRestaurantByIdAsync(int restaurantId);
}

public class RestaurantsService(DataContext db) : IRestaurantsService
{
    public async Task<Restaurant?> GetRestaurantByIdAsync(int restaurantId)
    {
        return await db.Restaurants.FirstOrDefaultAsync(x => x.Id == restaurantId);
    }
}