import {Component, computed, inject} from '@angular/core';
import {ReservationsService} from '../../_services/reservations.service';
import {MatCardModule} from '@angular/material/card';
import {MatButtonModule} from '@angular/material/button';
import {ErrorService} from '../../_services/error.service';
import {AlertService} from '../../_services/alert.service';

@Component({
    selector: 'app-check-in',
    imports: [MatCardModule, MatButtonModule],
    templateUrl: './check-in.component.html',
    styleUrl: './check-in.component.scss'
})
export class CheckInComponent {
    private readonly _reservation = inject(ReservationsService);
    private readonly _error = inject(ErrorService);
    private readonly _alert = inject(AlertService);

    isBusy = false;

    currentReservation = this._reservation.currentReservation.asReadonly();
    activeReservations = this._reservation.activeReservations.asReadonly();
    canCheckIn = computed(() => {
        if (!this.currentReservation()) return false;

        const sorted = this.activeReservations()
            .filter(x => x.seatingStatus === 'TableReady')
            .sort((a, b) => a.createdAt > b.createdAt ? -1 : 1);

        if (!sorted.length) return false;
        return sorted[0].id === this.currentReservation()!.id;
    });

    async checkIn() {
        this.isBusy = true;

        try {
            if (!this.currentReservation()) return;
            await this._reservation.checkIn(this.currentReservation()!.id);
            this._alert.inform('Check-in successful!');
        } catch (e) {
            this._error.handle(e);
        } finally {
            this.isBusy = false;
        }
    }

    newReservation() {
        this._reservation.newReservation();
    }
}
