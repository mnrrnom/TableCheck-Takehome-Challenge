using lequeuer.api.Modules.ReservationModule.Dtos;
using lequeuer.api.Modules.ReservationModule.Extensions;

using Microsoft.AspNetCore.Mvc;

namespace lequeuer.api.Modules.ReservationModule;

public static class ReservationEndpoint
{
    public static void MapReservationsEndpoint(this RouteGroupBuilder routeBuilder)
    {
        routeBuilder.MapGet("", _getReservationsHandler);
        routeBuilder.MapGet("{id:int}", _getByIdHandler);
        routeBuilder.MapGet("find", _findExistingReservation);
        routeBuilder.MapPost("", _createReservationHandler);
        routeBuilder.MapPatch("/checkin", _checkInHandler);
    }

    private static async Task<IResult> _getReservationsHandler(
        [FromQuery] int restaurantId,
        IReservationsService reservationsService
    )
    {
        var reservations = await reservationsService.GetActiveReservationsAsync(restaurantId);
        return Results.Ok(
            reservations.Select(x => new
            {
                x.Id,
                x.NumberOfDiners, 
                SeatingStatus = x.Status.ToString(),
                x.RestaurantId
            })
        );
    }
    
    private static async Task<IResult> _getByIdHandler(
        [FromRoute] int id,
        IReservationsService reservationsService
    )
    {
        var reservation = await reservationsService.GetActiveReservationByIdAsync(id);
        if (reservation == null) return Results.NotFound();
        return Results.Ok(reservation.ToDto());
    }
    
    private static async Task<IResult> _findExistingReservation(
        [FromQuery] int reservationId,
        [FromQuery] string leadGuestName,
        [FromQuery] int partySize,
        IReservationsService reservationsService
    )
    {
        var reservation = await reservationsService.GetActiveReservationByIdAsync(reservationId);
        if (reservation == null) return Results.Problem("Could not find reservation with the given details");
        
        if (!string.Equals(reservation.LeadGuestName, leadGuestName, StringComparison.CurrentCultureIgnoreCase) ||
            reservation.NumberOfDiners != partySize)
            return Results.Problem("Could not find reservation with the given details");
        
        return Results.Ok(reservation.ToDto());
    }
    
    private static async Task<IResult> _createReservationHandler(
        [FromBody] CreateReservationDto dto,
        IReservationsService reservationsService
    )
    {
        var reservation = await reservationsService.CreateReservationAsync(dto);
        return Results.Created($"/reservations/{reservation.Id}", reservation.ToDto());
    }

    private static async Task<IResult> _checkInHandler(
        [FromQuery] int reservationId,
        IReservationsService reservationsService
    )
    {
        var reservation = await reservationsService.CheckInAsync(reservationId);
        return Results.Ok(reservation.ToDto());
    }
}