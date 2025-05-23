using lequeuer.api.Modules.RestaurantModule.Dtos;
using lequeuer.api.Modules.RestaurantModule.Extensions;

using Microsoft.AspNetCore.Mvc;

namespace lequeuer.api.Modules.RestaurantModule;

public static class RestaurantsEndpoint
{
    public static void MapRestaurantsEndpoint(this RouteGroupBuilder routeBuilder)
    {
        routeBuilder.MapGet("/{restaurantId:int}", _getRestaurantByIdHandler);
    }
    
    private static async Task<IResult> _getRestaurantByIdHandler(
        [FromRoute] int restaurantId,
        IRestaurantsService restaurantsService
    )
    {
        var restaurant = await restaurantsService.GetRestaurantByIdAsync(restaurantId);
        if (restaurant == null) return Results.NotFound();
        
        return Results.Ok(restaurant.ToDto());
    }
}