import {Component, inject} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {CONSTANTS} from '../../_dictionary/constants';
import {ReservationsService} from '../../_services/reservations.service';
import {AlertService} from '../../_services/alert.service';
import {CreateReservationDto} from '../../_dtos/create-reservation-dto';
import {RestaurantsService} from '../../_services/restaurants.service';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatButtonModule} from '@angular/material/button';
import {MatCardModule} from '@angular/material/card';
import {ErrorService} from '../../_services/error.service';
import {MatDividerModule} from '@angular/material/divider';
import {RouterModule} from '@angular/router';
import {MatIconModule} from '@angular/material/icon';

@Component({
    selector: 'app-create-reservation-form',
    imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatCardModule, MatDividerModule, RouterModule, MatIconModule],
    templateUrl: './create-reservation-form.component.html',
    styleUrl: './create-reservation-form.component.scss',
})
export class CreateReservationFormComponent {
    private readonly _reservationService = inject(ReservationsService);
    private readonly _errorService = inject(ErrorService);
    private readonly _alertService = inject(AlertService);
    private readonly _restaurantService = inject(RestaurantsService);
    protected readonly CONSTANTS = CONSTANTS;

    restaurant = this._restaurantService.restaurant.asReadonly();
    currentReservation = this._reservationService.currentReservation.asReadonly();

    isBusy = false;
    form = new FormGroup({
        Name: new FormControl<string>('', [Validators.required, Validators.maxLength(64)]),
        PartySize: new FormControl<number>(1, [Validators.required, Validators.min(1), Validators.max(this.CONSTANTS.MAX_SEATS)]),
    });

    async submit() {
        this.isBusy = true;

        try {
            if (this.form.invalid) {
                this._alertService.inform('Please double check your input');
                return;
            }

            if (!this.restaurant()) {
                return;
            }

            const dto: CreateReservationDto = {
                leadGuestName: this.form.value.Name!,
                numberOfDiners: this.form.value.PartySize!,
                restaurantId: this.restaurant()!.id
            }

            await this._reservationService.createReservation(dto);
            this._alertService.inform("Reservation created successfully");
            this.form.reset();
        } catch (e) {
            this._errorService.handle(e);
        } finally {
            this.isBusy = false;
        }
    }
}
