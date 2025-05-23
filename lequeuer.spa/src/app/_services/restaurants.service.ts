import {computed, inject, Injectable, signal} from '@angular/core';
import {environment} from '../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {lastValueFrom} from 'rxjs';
import {RestaurantDto} from '../_dtos/restaurant-dto';
import {RtcService} from './rtc.service';
import {takeUntilDestroyed, toObservable} from '@angular/core/rxjs-interop';

@Injectable({
    providedIn: 'root'
})
export class RestaurantsService {
    private readonly baseUrl = environment.baseUrl + '/api/restaurants';
    private readonly _http = inject(HttpClient);
    private readonly _rtc = inject(RtcService);

    restaurant = signal<RestaurantDto | undefined>(undefined);
    isReadyToJoin = computed(() => {
        return this._rtc.connectionState() === 'Connected' && !!this.restaurant();
    });
    constructor() {
        toObservable(this.isReadyToJoin)
            .pipe(takeUntilDestroyed())
            .subscribe(async (isReady) => {
                if (!isReady) return;
                await this._rtc.joinRestaurantGroup(this.restaurant()!.id);
            });
    }

    async loadRestaurant(id: number) {
        const restaurant = await lastValueFrom(this._http.get<RestaurantDto>(`${this.baseUrl}/${id}`));
        this.restaurant.set(restaurant);
    }
}
