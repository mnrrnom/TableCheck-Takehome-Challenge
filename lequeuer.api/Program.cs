using lequeuer.api;
using lequeuer.api.Configuration;
using lequeuer.api.Hubs;
using lequeuer.api.Modules.ReservationModule;
using lequeuer.api.Modules.RestaurantModule;

using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.ConfigureAppSettings();
builder.ConfigureDataContext(settings);
builder.ConfigureCors();
builder.ConfigureProblemDetails();
builder.ConfigureAppServices();
builder.Services.AddSignalR();
builder.Services.AddHostedService<Dequeuer>();

var app = builder.Build();

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseConfiguredCors();
app.UseStaticFileServer();

app.MapGroup("/api/reservations").MapReservationsEndpoint();
app.MapGroup("/api/restaurants").MapRestaurantsEndpoint();
app.MapHub<ReservationsHub>("/hubs/reservations");
app.MapCustomFallbackToFile();

app.Run();
