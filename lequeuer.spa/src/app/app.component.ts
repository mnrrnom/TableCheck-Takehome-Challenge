import {Component, inject, OnInit} from '@angular/core';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {RouterLink, RouterOutlet} from '@angular/router';
import {RtcService} from './_services/rtc.service';

@Component({
    selector: 'app-root',
    imports: [MatToolbarModule, MatIconModule, MatButtonModule, RouterOutlet, RouterLink],
    templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
    private readonly _rtc = inject(RtcService);

    async ngOnInit() {
        await this._rtc.start();
    }
}
