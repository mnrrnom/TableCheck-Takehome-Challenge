using Microsoft.AspNetCore.SignalR;

namespace lequeuer.api.Hubs;

public interface IReservationsHub
{
    Task OnReservationDataUpdated();
}

public class ReservationsHub : Hub<IReservationsHub>
{
    public async Task ReservationDataUpdated(int restaurantId)
    {
        Console.WriteLine("Reservation data updated for restaurant " + restaurantId);
        await Clients.Group(restaurantId.ToString()).OnReservationDataUpdated();
    }
    
    public async Task JoinRestaurantGroup(int restaurantId)
    {
        Console.WriteLine("Joining restaurant group " + restaurantId);
        await Groups.AddToGroupAsync(Context.ConnectionId, restaurantId.ToString());
    }
}