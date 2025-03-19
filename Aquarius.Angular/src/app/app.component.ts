import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { FarmComponent } from "./Components/farm/farm.component";
import { FarmListComponent } from "./Components/farm/farm-list/farm-list.component";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports: [CommonModule, RouterModule, MatToolbarModule, MatButtonModule, FarmListComponent]
})
export class AppComponent {
  title = 'AquariusFrontend';
}