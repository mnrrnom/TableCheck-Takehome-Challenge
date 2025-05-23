using lequeuer.api.Models;
using lequeuer.api.Modules.RestaurantModule.Dtos;

namespace lequeuer.api.Modules.RestaurantModule.Extensions;

public static class RestaurantExtensions
{
    public static RestaurantDto ToDto(this Restaurant restaurant)
    {
        return new (restaurant.Id, restaurant.Name, restaurant.AvailableSeats);
    }
}