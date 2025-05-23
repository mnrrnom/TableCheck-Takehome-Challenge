using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using lequeuer.api.Dictionary;

using Microsoft.EntityFrameworkCore;

namespace lequeuer.api.Models;

[Table("reservations")]
[Index(nameof(DeletedAt))]
[Index(nameof(Status))]
[Index(nameof(CreatedAt))]
public class Reservation : SoftDeletable
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? SeatedAt { get; set; }
    
    public DateTime? VacatedAt { get; set; }

    [StringLength(64)] 
    public required string LeadGuestName { get; set; }

    public required int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    [Range(1, int.MaxValue)] 
    public required int NumberOfDiners { get; set; }

    [StringLength(16)]
    public ReservationStatus Status { get; set; }
}