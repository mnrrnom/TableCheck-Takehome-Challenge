import {Injectable, signal} from '@angular/core';
import {HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel} from '@microsoft/signalr';
import {environment} from '../../environments/environment';
import {Subject} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class RtcService {
    private _connection: HubConnection;
    private _hubEndpoint = `${environment.baseUrl}/Hubs/Reservations`;

    onUpdateReservationData$ = new Subject<Date>();
    connectionState = signal<HubConnectionState>(HubConnectionState.Disconnected);

    constructor() {
        this._connection = new HubConnectionBuilder()
            .withUrl(this._hubEndpoint)
            .configureLogging(environment.production ? LogLevel.None : LogLevel.Error)
            .withAutomaticReconnect()
            .build();

        this._connection.onclose(() => this.connectionState.set(this._connection.state));
        this._connection.onreconnecting(() => this.connectionState.set(this._connection.state));
        this._connection.onreconnected(() => this.connectionState.set(this._connection.state));

        this._connection.on('OnReservationDataUpdated', () => this._onReservationDataUpdated());
    }

    async start() {
        await this._connection.start();
        this.connectionState.set(this._connection.state);
    }

    async joinRestaurantGroup(restaurantId: number) {
        await this._connection.invoke('JoinRestaurantGroup', restaurantId)
            .catch(err => console.error('Error joining restaurant group:', err));
    }

    private _onReservationDataUpdated() {
        this.onUpdateReservationData$.next(new Date());
    }
}
