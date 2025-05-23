using System.Net;
using System.Net.Http.Json;
using Bogus;
using lequeuer.api.Data;
using lequeuer.api.Dictionary;
using lequeuer.api.Models;
using lequeuer.api.Modules.ReservationModule;
using lequeuer.api.Modules.ReservationModule.Dtos;
using lequeuer.api.Modules.RestaurantModule.Dtos;
using lequeuer.api.Utils;
using lequeuer.test.Fixtures;
using lequeuer.test.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace lequeuer.test.ReservationsTests;

public class ReservationsEndpointTests(ApiTestFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.HttpClient;
    
    [Fact]
    public async Task When_enough_seats_available_create_with_table_ready_status()
    {
        // Max seats are hard coded to 10
        // Create 2 reservations with 3 diners.
        // Both should have the status `TableReady`
        // Lastly, restaurant should have 4 available seats.
        
        const int restaurantId = 1;
        var faker = new Faker();
        var dto1 = new CreateReservationDto(3, faker.Name.FirstName(), restaurantId);
        var dto2 = new CreateReservationDto(3, faker.Name.FirstName(), restaurantId);
        var ct = TestContext.Current.CancellationToken;
        
        var postReservation1 = await _client.PostAsJsonAsync("/api/reservations", dto1, ct);
        var postReservation2 = await _client.PostAsJsonAsync("/api/reservations", dto2, ct); 
        var getRestaurantResponse = await _client.GetAsync($"/api/restaurants/{restaurantId}", ct);
        
        var reservation1 = await postReservation1.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);
        var reservation2 = await postReservation2.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);
        var restaurant = await getRestaurantResponse.Content.ReadFromJsonAsync<RestaurantDto>(cancellationToken: ct);
        
        Assert.NotNull(reservation1);
        Assert.NotNull(reservation2);
        Assert.NotNull(restaurant);
        
        Assert.True(postReservation1.IsSuccessStatusCode);
        Assert.True(postReservation2.IsSuccessStatusCode);
        
        Assert.Equal(nameof(ReservationStatus.TableReady), reservation1.SeatingStatus);
        Assert.Equal(nameof(ReservationStatus.TableReady), reservation2.SeatingStatus);
        Assert.Equal(4, restaurant.AvailableSeats);
    }
    
    [Fact]
    public async Task When_not_enough_seats_available_create_second_with_waiting_status()
    {
        // Max seats are hard coded to 10
        // Create 2 reservations with 6 diners.
        // First one should have the status `TableReady` and the second one should be `Waiting`
        // Lastly, restaurant should have 4 available seats.
            
        const int restaurantId = 1;
        var faker = new Faker();
        var dto1 = new CreateReservationDto(6, faker.Name.FirstName(), restaurantId);
        var dto2 = new CreateReservationDto(6, faker.Name.FirstName(), restaurantId);
        var ct = TestContext.Current.CancellationToken;
        
        var postReservation1 = await _client.PostAsJsonAsync("/api/reservations", dto1, ct);
        var postReservation2 = await _client.PostAsJsonAsync("/api/reservations", dto2, ct); 
        var getRestaurantResponse = await _client.GetAsync($"/api/restaurants/{restaurantId}", ct);
        
        var reservation1 = await postReservation1.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);
        var reservation2 = await postReservation2.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);
        var restaurant = await getRestaurantResponse.Content.ReadFromJsonAsync<RestaurantDto>(cancellationToken: ct);
        
        Assert.NotNull(reservation1);
        Assert.NotNull(reservation2);
        Assert.NotNull(restaurant);
        
        Assert.True(postReservation1.IsSuccessStatusCode);
        Assert.True(postReservation2.IsSuccessStatusCode);
        
        Assert.Equal(nameof(ReservationStatus.TableReady), reservation1.SeatingStatus);
        Assert.Equal(nameof(ReservationStatus.Waiting), reservation2.SeatingStatus);
        Assert.Equal(4, restaurant.AvailableSeats);
    }

    [Fact]
    public async Task When_turn_to_check_in_should_be_able_to_check_in()
    {
        const int restaurantId = 1;
        var faker = new Faker();
        var dto1 = new CreateReservationDto(3, faker.Name.FirstName(), restaurantId);
        var dto2 = new CreateReservationDto(3, faker.Name.FirstName(), restaurantId);
        var ct = TestContext.Current.CancellationToken;
        
        var postReservation1 = await _client.PostAsJsonAsync("/api/reservations", dto1, ct);
        var postReservation2 = await _client.PostAsJsonAsync("/api/reservations", dto2, ct);
        
        var reservation1 = await postReservation1.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);
        var reservation2 = await postReservation2.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);

        var checkInResponse = await _client.PatchAsync($"/api/reservations/checkin?reservationId={reservation1?.Id}", null, ct);
        reservation1 = await checkInResponse.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);

        Assert.NotNull(reservation1);
        Assert.NotNull(reservation2);
        
        Assert.True(postReservation1.IsSuccessStatusCode);
        Assert.True(postReservation2.IsSuccessStatusCode);
        Assert.True(checkInResponse.IsSuccessStatusCode);
        Assert.Equal(nameof(ReservationStatus.Seated), reservation1.SeatingStatus);
        Assert.Equal(nameof(ReservationStatus.TableReady), reservation2.SeatingStatus);
    }
    
    [Fact]
    public async Task When_not_turn_to_check_in_should_not_be_able_to_check_in()
    {
        const int restaurantId = 1;
        var faker = new Faker();
        var dto1 = new CreateReservationDto(3, faker.Name.FirstName(), restaurantId);
        var dto2 = new CreateReservationDto(3, faker.Name.FirstName(), restaurantId);
        var ct = TestContext.Current.CancellationToken;
        
        var postReservation1 = await _client.PostAsJsonAsync("/api/reservations", dto1, ct);
        var postReservation2 = await _client.PostAsJsonAsync("/api/reservations", dto2, ct);
        
        var reservation1 = await postReservation1.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);
        var reservation2 = await postReservation2.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);

        var checkInResponse = await _client.PatchAsync($"/api/reservations/checkin?reservationId={reservation2?.Id}", null, ct);
        var problemDetails = await checkInResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: ct);
        
        Assert.NotNull(reservation1);
        Assert.NotNull(reservation2);
        Assert.NotNull(problemDetails);
        
        Assert.True(postReservation1.IsSuccessStatusCode);
        Assert.True(postReservation2.IsSuccessStatusCode);
        Assert.False(checkInResponse.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, checkInResponse.StatusCode);
        Assert.Equal("Sorry, there is a reservation ahead of you", problemDetails.Detail);
    }

    [Fact]
    public async Task When_party_finishes_service_next_party_should_be_queued()
    {
        const int restaurantId = 1;
        var faker = new Faker();
        var dto1 = new CreateReservationDto(1, faker.Name.FirstName(), restaurantId);
        var dto2 = new CreateReservationDto(10, faker.Name.FirstName(), restaurantId);
        var ct = TestContext.Current.CancellationToken;
        await using var scope = fixture.Services.CreateAsyncScope();
        var reservationsService = scope.ServiceProvider.GetRequiredService<IReservationsService>();
        
        // create 2 reservations with 1 and 10 diners.
        var postReservation1 = await _client.PostAsJsonAsync("/api/reservations", dto1, ct);
        var postReservation2 = await _client.PostAsJsonAsync("/api/reservations", dto2, ct);
        var reservation1 = await postReservation1.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);
        var reservation2 = await postReservation2.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);
        
        // Assert that the first one should have the status `TableReady` and the second one should be `Waiting`
        Assert.NotNull(reservation1);
        Assert.NotNull(reservation2);
        Assert.True(postReservation1.IsSuccessStatusCode);
        Assert.True(postReservation2.IsSuccessStatusCode);
        Assert.Equal(nameof(ReservationStatus.TableReady), reservation1.SeatingStatus);
        Assert.Equal(nameof(ReservationStatus.Waiting), reservation2.SeatingStatus);

        // Check in the first reservation
        var checkInResponse = await _client.PatchAsync($"/api/reservations/checkin?reservationId={reservation1?.Id}", null, ct);
        reservation1 = await checkInResponse.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);

        // Assert that the first one should have the status `Seated`.
        Assert.True(checkInResponse.IsSuccessStatusCode);
        Assert.NotNull(reservation1);
        Assert.Equal(nameof(ReservationStatus.Seated), reservation1.SeatingStatus);
        
        // Simulate dequeuer calling dequeue method.
        // Dequeuer actual functionality should be tested in another test.
        await Task.Delay(1000, ct);
        var dequeuedReservations = await reservationsService.DequeueOlderThanAndQueueNextAsync(0);
        
        // Assert that the dequeuer would have dequeued only the first reservation.
        Assert.Single(dequeuedReservations);
        Assert.Equal(reservation1.Id, dequeuedReservations.First().Id);

        // Get a fresh copy of both reservations.
        var getReservationResponse1 = await _client.GetAsync($"/api/reservations/{reservation1.Id}", ct);
        var getReservationResponse2 = await _client.GetAsync($"/api/reservations/{reservation2.Id}", ct);
        reservation1 = await getReservationResponse1.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);
        reservation2 = await getReservationResponse2.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);
        
        // Assert that the first one should have the status `Vacated` and the second one should be `TableReady`.
        Assert.True(getReservationResponse1.IsSuccessStatusCode);
        Assert.True(getReservationResponse2.IsSuccessStatusCode);
        Assert.NotNull(reservation1);
        Assert.NotNull(reservation2);
        Assert.Equal(nameof(ReservationStatus.Vacated), reservation1.SeatingStatus);
        Assert.Equal(nameof(ReservationStatus.TableReady), reservation2.SeatingStatus);
    }
    
    [Fact]
    public async Task When_party_finishes_service_should_be_dequeued_automatically()
    {
        const int restaurantId = 1;
        var faker = new Faker();
        var dto = new CreateReservationDto(1, faker.Name.FirstName(), restaurantId);
        var ct = TestContext.Current.CancellationToken;
        
        // create a reservation.
        var postReservation = await _client.PostAsJsonAsync("/api/reservations", dto, ct);
        var reservation = await postReservation.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);
        
        // Check in the reservation.
        var checkInResponse = await _client.PatchAsync($"/api/reservations/checkin?reservationId={reservation?.Id}", null, ct);
        reservation = await checkInResponse.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);

        // Assert that the status is `Seated`.
        Assert.True(checkInResponse.IsSuccessStatusCode);
        Assert.NotNull(reservation);
        Assert.Equal(nameof(ReservationStatus.Seated), reservation.SeatingStatus);
        
        // Give the Deququer time to dequeue the reservation.
        await Task.Delay(Constants.TimeToDequeue * 2 * 1000, ct);
        
        // Get a fresh copy of the reservations.
        var getReservationResponse = await _client.GetAsync($"/api/reservations/{reservation.Id}", ct);
        reservation = await getReservationResponse.Content.ReadFromJsonAsync<ReservationDto>(cancellationToken: ct);
        
        // Assert that the status is updated to `Vacated`.
        Assert.True(getReservationResponse.IsSuccessStatusCode);
        Assert.NotNull(reservation);
        Assert.Equal(nameof(ReservationStatus.Vacated), reservation.SeatingStatus);
    }
    
    [Fact]
    public async Task GetActiveReservationsAsync_ShouldReturnActiveReservations()
    {
        const int restaurantId = 1;
        var faker = new Faker();
        await using var scope = fixture.Services.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        var ct = TestContext.Current.CancellationToken;
        var sut = scope.ServiceProvider.GetRequiredService<IReservationsService>();
        
        List<Reservation> reservations =
        [
            _createReservation(faker, status: ReservationStatus.Seated),
            _createReservation(faker, status: ReservationStatus.Waiting),
            _createReservation(faker, status: ReservationStatus.NoShow),
            _createReservation(faker, status: ReservationStatus.Unset),
            _createReservation(faker, status: ReservationStatus.Vacated),
            _createReservation(faker, status: ReservationStatus.TableReady),
        ];
        
        db.Reservations.AddRange(reservations);
        await db.SaveChangesAsync(ct);
        
        var result = await sut.GetActiveReservationsAsync(restaurantId);

        Assert.Equal(3, result.Count);
        Assert.All(result, r => Assert.Contains(r.Status, new[]
        {
            ReservationStatus.Seated,
            ReservationStatus.Waiting,
            ReservationStatus.TableReady,
        }));
    }

    [Fact]
    public async Task DequeueOlderThanAndQueueNextAsync_ShouldDequeueExpectedReservations()
    {
        var faker = new Faker();
        await using var scope = fixture.Services.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        var ct = TestContext.Current.CancellationToken;
        var sut = scope.ServiceProvider.GetRequiredService<IReservationsService>();
        
        List<Reservation> reservations =
        [
            _createReservation(faker, status: ReservationStatus.Seated, 
                createdAt: DateTimeUtils.JapanTime(),
                seatedAt: DateTimeUtils.JapanTime().AddSeconds(-10)),
            _createReservation(faker, status: ReservationStatus.Seated, 
                createdAt: DateTimeUtils.JapanTime(),
                seatedAt: DateTimeUtils.JapanTime().AddSeconds(-10)),
            _createReservation(faker),
            _createReservation(faker),
            _createReservation(faker),
        ];
        
        db.Reservations.AddRange(reservations);
        await db.SaveChangesAsync(ct);
        
        var result = await sut.DequeueOlderThanAndQueueNextAsync(0);
        
        Assert.Equal(2, result.Count);
    }
    
    private static Reservation _createReservation(
        Faker faker, 
        ReservationStatus status = ReservationStatus.Unset,
        DateTime? createdAt = null,
        DateTime? seatedAt = null,
        int restaurantId = 1,
        int numberOfDiners = 1
    )
    {
        return new ()
        {
            RestaurantId = restaurantId,
            Status = status,
            CreatedAt = createdAt ?? DateTimeUtils.JapanTime(),
            LeadGuestName = faker.Name.FirstName(),
            NumberOfDiners = numberOfDiners,
            SeatedAt = seatedAt ?? DateTimeUtils.JapanTime(),
        };
    }
    
    public async ValueTask DisposeAsync()
    {
        await using var scope = fixture.Services.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        await db.Database.EnsureDeletedAsync(TestContext.Current.CancellationToken);
        await new DatabaseSeeder(fixture.Services).SeedAsync();
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }
}