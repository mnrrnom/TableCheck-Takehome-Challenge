import {Component} from '@angular/core';
import {RouterModule} from '@angular/router';
import {MatIconModule} from '@angular/material/icon';

@Component({
    selector: 'app-home',
    imports: [RouterModule, MatIconModule],
    templateUrl: './home.component.html',
    styleUrl: './home.component.scss',
})
export class HomeComponent {}
