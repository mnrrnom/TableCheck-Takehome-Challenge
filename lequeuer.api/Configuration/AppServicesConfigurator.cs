using lequeuer.api.Modules.ReservationModule;
using lequeuer.api.Modules.RestaurantModule;

namespace lequeuer.api.Configuration;

public static class AppServicesConfigurator
{
    public static void ConfigureAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IReservationsService, ReservationsService>();
        builder.Services.AddScoped<IRestaurantsService, RestaurantsService>();
    }
}