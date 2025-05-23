using lequeuer.api.Models;
using lequeuer.api.Modules.ReservationModule.Dtos;

namespace lequeuer.api.Modules.ReservationModule.Extensions;

public static class ReservationsDtoMapper
{
    public static ReservationDto ToDto(this Reservation reservation)
    {
        return new (
            Id: reservation.Id,
            LeadGuestName: reservation.LeadGuestName,
            NumberOfDiners: reservation.NumberOfDiners,
            SeatingStatus: reservation.Status.ToString(),
            CreatedAt: reservation.CreatedAt.ToString("yyyy-M-dd HH:mm"),
            RestaurantId: reservation.RestaurantId
        );
    }
}