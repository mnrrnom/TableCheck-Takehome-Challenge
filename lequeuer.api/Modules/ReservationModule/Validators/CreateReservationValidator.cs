using lequeuer.api.Models;
using lequeuer.api.Modules.ReservationModule.Dtos;

namespace lequeuer.api.Modules.ReservationModule.Validators;

public static class CreateReservationValidator
{
    public static string? GetValidationError(this CreateReservationDto dto)
    {
        switch (dto.LeadGuestName.Length)
        {
            case > 64:
                return "Lead guest name is too long. Maximum length is 64 characters.";
            case < 1:
                return "Lead guest name is too short. Minimum length is 1 character.";
        }

        if (dto.NumberOfDiners is < 1 or > 10) return "Number of diners must be between 1 and 10.";
        if (dto.RestaurantId <= 0) return "Restaurant ID is required.";
        
        return null;
    }
}