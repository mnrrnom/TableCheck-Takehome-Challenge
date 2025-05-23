using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace lequeuer.api.Models;

[Table("restaurants")]
[Index(nameof(Name), IsUnique = true)]
[Index(nameof(DeletedAt))]
public class Restaurant : SoftDeletable
{
    public int Id { get; set; }

    [StringLength(64)]
    public required string Name { get; set; }

    [Range(1, int.MaxValue)]
    public required int AvailableSeats { get; set; }

    [Timestamp]
    public byte[]? Version { get; set; }
}