import {inject, Injectable} from '@angular/core';
import {environment} from '../../environments/environment';
import {AlertService} from './alert.service';

type ApplicationError = {
    error: {
        detail: string;
        title: string;
    }
}

@Injectable({
    providedIn: 'root'
})
export class ErrorService {
    private readonly _alert = inject(AlertService);

    constructor() {}

    handle(error: ApplicationError | any) {
        if (!environment.production) console.log(error);
        if (error.error.detail) {
            this._alert.inform(error.error.detail);
        } else {
            this._alert.inform('An unexpected error occurred. Please try again later.');
        }
    }
}
