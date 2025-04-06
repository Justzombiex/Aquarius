import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-oxygen',
  templateUrl: './oxygen.component.html',
  styleUrls: ['./oxygen.component.css'],
  imports: [MatIconModule]
})
export class OxygenComponent {
  oxygenLevel: number = 5; // Valor fijo de 5 mg/L
  status: string = 'Nivel Óptimo';
  statusIcon: string = 'check_circle';
}