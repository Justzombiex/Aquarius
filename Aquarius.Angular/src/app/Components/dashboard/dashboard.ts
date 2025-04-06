import { Component } from '@angular/core';
import { MatGridListModule } from '@angular/material/grid-list';
import { ReadingGraphComponent } from "../reading/reading.component";
import { TanqueComponent } from "../water-tank/water.tank";
import { OxygenComponent } from '../Oxygen/oxygen.component';
import { PhDisplayComponent } from "../pH/ph-display.component";

/**
 * @title Basic grid-list
 */
@Component({
  selector: 'dashboard',
  styleUrls: ['dashboard.css'], // Corrección: styleUrls en plural
  templateUrl: 'dashboard.html',
  imports: [MatGridListModule, ReadingGraphComponent, TanqueComponent, OxygenComponent, PhDisplayComponent],
})
export class DashBoard {
  dissolvedOxygen = 5; // Valor inicial de oxígeno
}
