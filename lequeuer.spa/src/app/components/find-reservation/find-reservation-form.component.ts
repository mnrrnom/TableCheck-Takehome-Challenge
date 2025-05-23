import {Component, inject, OnInit} from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {CONSTANTS} from '../../_dictionary/constants';
import {MatButton} from '@angular/material/button';
import {MatCard, MatCardContent, MatCardHeader, MatCardSubtitle, MatCardTitle} from '@angular/material/card';
import {MatError, MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {CreateReservationDto} from '../../_dtos/create-reservation-dto';
import {ReservationsService} from '../../_services/reservations.service';
import {ErrorService} from '../../_services/error.service';
import {AlertService} from '../../_services/alert.service';
import {RestaurantsService} from '../../_services/restaurants.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-find-reservation-form',
    imports: [
        FormsModule,
        MatButton,
        MatCard,
        MatCardContent,
        MatCardHeader,
        MatCardSubtitle,
        MatCardTitle,
        MatError,
        MatFormField,
        MatInput,
        MatLabel,
        ReactiveFormsModule
    ],
  templateUrl: './find-reservation-form.component.html',
  styleUrl: './find-reservation-form.component.scss'
})
export class FindReservationFormComponent implements OnInit {
    protected readonly CONSTANTS = CONSTANTS;
    private readonly _reservationService = inject(ReservationsService);
    private readonly _errorService = inject(ErrorService);
    private readonly _alertService = inject(AlertService);
    private readonly _restaurantService = inject(RestaurantsService);
    private readonly _router = inject(Router);

    restaurant = this._restaurantService.restaurant.asReadonly();

    isBusy = false;

    form = new FormGroup({
        Id: new FormControl<number | null>(null, [Validators.required, Validators.min(1)]),
        Name: new FormControl<string>('', [Validators.required, Validators.maxLength(64)]),
        PartySize: new FormControl<number>(1, [Validators.required, Validators.min(1), Validators.max(this.CONSTANTS.MAX_SEATS)]),
    });

    async ngOnInit() {

        if (!!this._reservationService.currentReservation()) {
            this._alertService.inform("You already have a reservation.");
            await this._router.navigate(['/'])
        }
    }

    async submit() {
        this.isBusy = true;

        try {
            if (this.form.invalid) {
                this._alertService.inform('Please fill in all required fields.');
                return;
            }

            const reservation = await this._reservationService
                .findExistingReservation(this.form.value.Id!, this.form.value.Name!, this.form.value.PartySize!);
            if (!reservation) return;
            await this._restaurantService.loadRestaurant(reservation.restaurantId);
            this._alertService.inform("Reservation found!");
            this.form.reset();
            await this._router.navigate(['restaurants', reservation.restaurantId]);
        } catch (e) {
            this._errorService.handle(e);
        } finally {
            this.isBusy = false;
        }
    }
}
