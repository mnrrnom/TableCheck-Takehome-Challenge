import { Component } from '@angular/core';
import {FindReservationFormComponent} from '../../components/find-reservation/find-reservation-form.component';

@Component({
  selector: 'app-find-reservation-page',
    imports: [
        FindReservationFormComponent
    ],
  templateUrl: './find-reservation-page.component.html',
  styleUrl: './find-reservation-page.component.scss'
})
export class FindReservationPage {

}
