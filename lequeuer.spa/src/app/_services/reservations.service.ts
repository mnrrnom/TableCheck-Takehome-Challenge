import { HttpClient } from '@angular/common/http';
import {inject, Injectable, signal} from '@angular/core';
import {ReservationDto} from '../_dtos/reservation-dto';
import {RestaurantsService} from './restaurants.service';
import {lastValueFrom} from 'rxjs';
import {takeUntilDestroyed, toObservable} from '@angular/core/rxjs-interop';
import {environment} from '../../environments/environment';
import {RtcService} from './rtc.service';
import {CreateReservationDto} from '../_dtos/create-reservation-dto';
import {LocalStorageService} from './local-storage.service';

@Injectable({
    providedIn: 'root'
})
export class ReservationsService {
    private readonly _baseUrl = environment.baseUrl + '/api/reservations';
    private readonly _http = inject(HttpClient);
    private readonly _restaurantsService = inject(RestaurantsService);
    private readonly _rtc = inject(RtcService);
    private readonly _localStorageKey = 'current-reservation';
    private readonly _storage = inject(LocalStorageService);

    activeReservations = signal<ReservationDto[]>([]);
    currentReservation = signal<ReservationDto | null>(null);

    constructor() {
        toObservable(this._restaurantsService.restaurant)
            .pipe(takeUntilDestroyed())
            .subscribe(() => this.loadActiveReservations());

        this._rtc.onUpdateReservationData$
            .pipe(takeUntilDestroyed())
            .subscribe(async () => await this.loadActiveReservations());

        const storedReservation = this._storage.getItem(this._localStorageKey);
        if (storedReservation) {
            const reservation = JSON.parse(storedReservation) as ReservationDto;
            if (reservation.seatingStatus === 'Vacated') {
                this._storage.removeItem(this._localStorageKey);
                return;
            }
            this.loadReservation(reservation.id);
        }
    }

    async loadActiveReservations() {
        const restaurantId = this._restaurantsService.restaurant()?.id;
        if (!restaurantId) return;

        const reservations = await lastValueFrom(this._http.get<ReservationDto[]>(`${this._baseUrl}?restaurantId=${restaurantId}`));
        this.activeReservations.set(reservations);

        if (this.currentReservation()) await this.loadReservation(this.currentReservation()!.id);
    }

    async findExistingReservation(id: number, leadGuestName: string, partySize: number) {
        if (this.currentReservation()) return;
        const url = `${this._baseUrl}/find?reservationId=${id}&leadGuestName=${leadGuestName}&partySize=${partySize}`;
        const reservation = await lastValueFrom(this._http.get<ReservationDto>(url));
        console.log(reservation);
        this.currentReservation.set(reservation);
        this._storage.setItem(this._localStorageKey, JSON.stringify(reservation));
        return reservation;
    }

    async loadReservation(reservationId: number) {
        const reservation = await lastValueFrom(this._http.get<ReservationDto>(`${this._baseUrl}/${reservationId}`));
        this.currentReservation.set(reservation);
        this._storage.setItem(this._localStorageKey, JSON.stringify(reservation));
    }

    async createReservation(dto: CreateReservationDto) {
        const reservation = await lastValueFrom(this._http.post<ReservationDto>(`${this._baseUrl}`, dto));

        // we do not need to update the active reservations list
        // here since creating a reservation will trigger an
        // OnUpdateReservationData event on the RTC hub
        // which will update the active reservations list

        this.currentReservation.set(reservation);
        this._storage.setItem(this._localStorageKey, JSON.stringify(reservation));
        return reservation;
    }

    async checkIn(reservationId: number): Promise<ReservationDto> {
        const reservation = await lastValueFrom(
            this._http.patch<ReservationDto>(`${this._baseUrl}/checkin?reservationId=${reservationId}`, {})
        );

        this.currentReservation.set(reservation);
        this._storage.setItem(this._localStorageKey, JSON.stringify(reservation));
        return reservation;
    }

    newReservation() {
        if (this.currentReservation()?.seatingStatus !== 'Vacated') return;
        this.currentReservation.set(null);
        this._storage.removeItem(this._localStorageKey);
    }
}
