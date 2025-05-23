import { Routes } from '@angular/router';
import {HomeComponent} from './pages/home/home.component';
import {ReservationComponent} from './pages/reservation/reservation.component';
import {FindReservationPage} from './pages/find-reservation/find-reservation-page.component';

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'reservations/find', component: FindReservationPage},
    { path: 'restaurants', component: HomeComponent},
    { path: 'restaurants/:id', component: ReservationComponent}
];
