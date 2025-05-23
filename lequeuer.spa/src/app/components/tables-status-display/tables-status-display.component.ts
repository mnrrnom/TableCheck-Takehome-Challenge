import {Component, computed, inject, input, OnInit} from '@angular/core';
import {RestaurantsService} from '../../_services/restaurants.service';
import {ReservationsService} from '../../_services/reservations.service';
import {NgClass} from '@angular/common';
import {MatIconModule} from '@angular/material/icon';
import {ReservationDto} from '../../_dtos/reservation-dto';
import {SeatStatus} from '../../_dictionary/reservation-status';
import {MatCardModule} from '@angular/material/card';

@Component({
    selector: 'app-tables-status-checker',
    imports: [
        NgClass,
        MatIconModule,
        MatCardModule
    ],
    templateUrl: './tables-status-display.component.html',
    styleUrl: './tables-status-display.component.scss',
})
export class TablesStatusDisplayComponent implements OnInit {
    private readonly _restaurantsService = inject(RestaurantsService);
    private readonly _reservationsService = inject(ReservationsService);

    restaurantId = input.required<number>();
    restaurant = this._restaurantsService.restaurant.asReadonly();
    reservations = this._reservationsService.activeReservations.asReadonly();

    seatStatuses = computed<SeatStatus[]>(() => {
        if (!this.restaurant()) return [];

        const isOccupied = (reservation: ReservationDto) =>
            reservation.seatingStatus === 'Seated' ||
            reservation.seatingStatus === 'TableReady';

        const occupiedSeats = this.reservations()
            .filter(isOccupied)
            .flatMap(x => Array(x.numberOfDiners).fill(x));

        return Array
            .from({length: 10}, (_, i) => i)
            .map(x => occupiedSeats[x] ? occupiedSeats[x].seatingStatus : 'Available');
    });

    async ngOnInit() {
        await this._restaurantsService.loadRestaurant(this.restaurantId());
    }
}
