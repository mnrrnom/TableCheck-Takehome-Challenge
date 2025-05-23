import {Component, inject, OnInit} from '@angular/core';
import {
    CreateReservationFormComponent
} from "../../components/create-reservation-form/create-reservation-form.component";
import {ActivatedRoute, Router} from '@angular/router';
import {TablesStatusDisplayComponent} from '../../components/tables-status-display/tables-status-display.component';
import {RtcService} from '../../_services/rtc.service';
import {CheckInComponent} from '../../components/check-in/check-in.component';
import {QueuePositionDisplayComponent} from '../../components/queue-position-display/queue-position-display.component';

@Component({
  selector: 'app-reservation',
    imports: [
        CreateReservationFormComponent,
        TablesStatusDisplayComponent,
        CheckInComponent,
        QueuePositionDisplayComponent
    ],
  templateUrl: './reservation.component.html',
  styleUrl: './reservation.component.scss'
})
export class ReservationComponent implements OnInit {
    private readonly _route = inject(ActivatedRoute);
    private readonly _router = inject(Router);
    restaurantId = 0;

    async ngOnInit() {
        this.restaurantId = parseInt(this._route.snapshot.paramMap.get('id') ?? '');
        if (isNaN(this.restaurantId)) {
            this._router.navigate(['/']);
            return;
        }
    }
}
