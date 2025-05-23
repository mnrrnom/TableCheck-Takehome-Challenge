import {inject, Injectable} from '@angular/core';
import {MatSnackBar} from '@angular/material/snack-bar';

@Injectable({
    providedIn: 'root'
})
export class AlertService {
    private readonly _snackBar = inject(MatSnackBar);
    private readonly _defaultDuration = 3000;

    constructor() {}

    inform(message: string, duration: number = this._defaultDuration) {
        this._snackBar.open(message, 'OK', {duration})
    }
}
