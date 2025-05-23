import {Component, computed, inject} from '@angular/core';
import {ReservationsService} from '../../_services/reservations.service';
import {NgClass} from '@angular/common';
import {MatIconModule} from '@angular/material/icon';
import {MatCardModule} from '@angular/material/card';
import {ReservationDto} from '../../_dtos/reservation-dto';

type LocalReservation = ReservationDto & {
    icons: string[]
}

@Component({
  selector: 'app-queue-position-display',
    imports: [
        NgClass, MatIconModule, MatCardModule
    ],
  templateUrl: './queue-position-display.component.html',
  styleUrl: './queue-position-display.component.scss'
})
export class QueuePositionDisplayComponent {
    protected readonly Array = Array;
    private _reservationService = inject(ReservationsService);
    private _icons = ['elderly_woman', 'sports_martial_arts', 'pregnant_woman', 'sports_gymnastics', 'emoji_people',
        'directions_walk', 'sports_handball', 'scuba_diving', 'surfing', 'hiking'];

    sortedReservations = computed<LocalReservation[]>(() => {
        const reservations = [...this._reservationService.activeReservations()];
        return reservations
            .sort((a, b) => a.createdAt > b.createdAt ? -1 : 1)
            .map(x => {
                const icons = Array
                    .from({length: x.numberOfDiners}, (_, i) => i)
                    .map(m => {
                        const randomIndex = Math.floor(Math.random() * this._icons.length);
                        return this._icons[randomIndex];
                    });

                return {
                    ...x,
                    icons: icons
                } as LocalReservation;
            });
    });
    currentReservation = this._reservationService.currentReservation.asReadonly();
}
