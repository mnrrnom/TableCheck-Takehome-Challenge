using lequeuer.api.Hubs;
using lequeuer.api.Models;
using lequeuer.api.Modules.ReservationModule;
using lequeuer.api.Utils;

using Microsoft.AspNetCore.SignalR;

namespace lequeuer.api;

public class Dequeuer(IServiceProvider serviceProvider) : BackgroundService
{
    private static readonly int SecondsToDequeue = Constants.TimeToDequeue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000 * SecondsToDequeue, stoppingToken);

            using var scope = serviceProvider.CreateScope();
            var reservationService = scope.ServiceProvider.GetRequiredService<IReservationsService>();
            var rtcHub = scope.ServiceProvider.GetRequiredService<IHubContext<ReservationsHub, IReservationsHub>>();
            
            var dequeuedReservations = await reservationService.DequeueOlderThanAndQueueNextAsync(SecondsToDequeue);
            if (dequeuedReservations.Count <= 0) continue;
            
            foreach (Reservation dequeuedReservation in dequeuedReservations)
            {
                await rtcHub.Clients.Group(dequeuedReservation.RestaurantId.ToString()).OnReservationDataUpdated();
                Console.WriteLine("Dequeued reservation: {0}", dequeuedReservation.Id);
            }
        }
    }
}