using System.ComponentModel.DataAnnotations;

namespace lequeuer.api.Modules.ReservationModule.Dtos;

public record CreateReservationDto (int NumberOfDiners, string LeadGuestName, int RestaurantId);