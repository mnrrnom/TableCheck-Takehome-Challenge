using lequeuer.api.Configuration;
using lequeuer.api.Data;
using lequeuer.api.Dictionary;
using lequeuer.api.Hubs;
using lequeuer.api.Models;
using lequeuer.api.Modules.ReservationModule.Dtos;
using lequeuer.api.Modules.ReservationModule.Validators;
using lequeuer.api.Utils;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace lequeuer.api.Modules.ReservationModule;

public interface IReservationsService
{
    Task<Reservation> CreateReservationAsync(CreateReservationDto dto);
    Task<ICollection<Reservation>> DequeueOlderThanAndQueueNextAsync(int seconds);
    Task<ICollection<Reservation>> GetActiveReservationsAsync(int restaurantId);
    Task<Reservation> CheckInAsync(int reservationId);
    Task<Reservation?> GetActiveReservationByIdAsync(int id);
}

public class ReservationsService(
    DataContext db,
    ILogger<ReservationsService> logger,
    IHubContext<ReservationsHub, IReservationsHub> reservationsHub
) : IReservationsService
{
    private const int _maxRetries = 3;

    public async Task<Reservation?> GetActiveReservationByIdAsync(int id)
    {
        return await db.Reservations
            .Include(x => x.Restaurant)
            .FirstOrDefaultAsync(x => x.Id == id &&
                                      (x.Status == ReservationStatus.TableReady ||
                                       x.Status == ReservationStatus.Seated ||
                                       x.Status == ReservationStatus.Vacated ||
                                       x.Status == ReservationStatus.Waiting));
    }

    public async Task<ICollection<Reservation>> GetActiveReservationsAsync(int restaurantId)
    {
        return await db.Reservations
            .Where(x => x.RestaurantId == restaurantId &&
                        (x.Status == ReservationStatus.TableReady ||
                         x.Status == ReservationStatus.Seated ||
                         x.Status == ReservationStatus.Waiting))
            .ToListAsync();
    }

    public async Task<Reservation> CreateReservationAsync(CreateReservationDto dto)
    {
        var validationError = dto.GetValidationError();
        if (validationError != null) throw new ValidationProblemException("Invalid input", validationError);

        return await _executeWithRetryAsync("Create reservation", async () =>
        {
            var restaurant = await db.Restaurants.FirstOrDefaultAsync(x => x.Id == dto.RestaurantId);
            if (restaurant == null) throw new ValidationProblemException("Invalid input", "Restaurant not found");

            var hasEnoughSeats = restaurant.AvailableSeats - dto.NumberOfDiners >= 0;
            var hasPriorWaitingParty = await db.Reservations
                .AnyAsync(x => x.RestaurantId == dto.RestaurantId && x.Status == ReservationStatus.Waiting);
            var canBeSeated = hasEnoughSeats && !hasPriorWaitingParty;

            var reservation = new Reservation
            {
                RestaurantId = dto.RestaurantId,
                LeadGuestName = dto.LeadGuestName,
                NumberOfDiners = dto.NumberOfDiners,
                Status = canBeSeated ? ReservationStatus.TableReady : ReservationStatus.Waiting,
                CreatedAt = DateTimeUtils.JapanTime()
            };

            if (canBeSeated) restaurant.AvailableSeats -= dto.NumberOfDiners;

            await db.Reservations.AddAsync(reservation);
            await db.SaveChangesAsync();
            await reservationsHub.Clients.Group(dto.RestaurantId.ToString()).OnReservationDataUpdated();

            return reservation;
        });
    }

    public async Task<ICollection<Reservation>> DequeueOlderThanAndQueueNextAsync(int seconds)
    {
        if (seconds < 0) seconds = 0;

        return await _executeWithRetryAsync("Dequeue reservations", async () =>
        {
            var forDequeue = await db.Reservations
                .Include(x => x.Restaurant)
                .Where(x => x.SeatedAt != null &&
                            x.Status == ReservationStatus.Seated &&
                            x.SeatedAt.Value.AddSeconds(seconds * x.NumberOfDiners) <= DateTimeUtils.JapanTime())
                .ToListAsync();

            if (forDequeue.Count <= 0) return [];

            foreach (var r in forDequeue)
            {
                r.Status = ReservationStatus.Vacated;
                r.Restaurant!.AvailableSeats += r.NumberOfDiners;
                r.VacatedAt = DateTimeUtils.JapanTime();
            }

            await _tryQueueNextWaitingReservationAsync(forDequeue.First().RestaurantId);
            await db.SaveChangesAsync();
            return forDequeue;
        });
    }

    public async Task<Reservation> CheckInAsync(int reservationId)
    {
        var nextReservation = await db.Reservations
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefaultAsync(x => x.Status == ReservationStatus.TableReady);
        if (nextReservation == null) throw new ValidationProblemException("Invalid input", "No reservation found");
        if (nextReservation.Id != reservationId)
            throw new ValidationProblemException("Invalid input", "Sorry, there is a reservation ahead of you");

        nextReservation.Status = ReservationStatus.Seated;
        nextReservation.SeatedAt = DateTimeUtils.JapanTime();
        await db.SaveChangesAsync();
        await reservationsHub.Clients.Group(nextReservation.RestaurantId.ToString()).OnReservationDataUpdated();
        return nextReservation;
    }

    private async Task<T> _executeWithRetryAsync<T>(string errorContext, Func<Task<T>> action)
    {
        int retries = 0;
        while (retries < _maxRetries)
        {
            try
            {
                return await action();
            }
            catch (DbUpdateConcurrencyException)
            {
                retries++;
                if (retries >= _maxRetries)
                {
                    logger.LogError("Failed to {ErrorContext} after {MaxRetries} retries", errorContext, _maxRetries);
                }
                else
                {
                    logger.LogWarning("Concurrency problem, retrying... {Retries}/{MaxRetries}", retries, _maxRetries);
                }
            }
            catch (Exception)
            {
                throw new ProblemException("Unexpected Exception", $"Failed to {errorContext}");
            }
        }

        throw new ProblemException("Concurrency Problem", $"Failed to {errorContext} after {_maxRetries} retries");
    }

    private async Task _tryQueueNextWaitingReservationAsync(int restaurantId)
    {
        var reservation = await db.Reservations
            .Include(x => x.Restaurant)
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefaultAsync(x => x.Status == ReservationStatus.Waiting && x.RestaurantId == restaurantId);
        if (reservation == null) return;

        var hasEnoughSeats = reservation.Restaurant!.AvailableSeats - reservation.NumberOfDiners >= 0;
        if (!hasEnoughSeats) return;
        reservation.Status = ReservationStatus.TableReady;
        reservation.Restaurant.AvailableSeats -= reservation.NumberOfDiners;
    }
}