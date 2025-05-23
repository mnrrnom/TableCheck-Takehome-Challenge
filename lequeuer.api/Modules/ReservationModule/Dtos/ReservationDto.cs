namespace lequeuer.api.Modules.ReservationModule.Dtos;

public record ReservationDto(
    int Id,
    string LeadGuestName,
    int NumberOfDiners,
    string SeatingStatus,
    string CreatedAt,
    int RestaurantId
);