import {Injectable} from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class LocalStorageService {
    private readonly _storageEngine = localStorage;
    constructor() {}

    setItem(key: string, value: string) {
        this._storageEngine.setItem(key, value)
    }

    getItem(key: string): string | null {
        return this._storageEngine.getItem(key);
    }

    removeItem(key: string) {
        this._storageEngine.removeItem(key);
    }
}
