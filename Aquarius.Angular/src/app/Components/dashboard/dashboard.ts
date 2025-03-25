import {Component} from '@angular/core';
import {MatGridListModule} from '@angular/material/grid-list';
import { ReadingGraphComponent } from "../reading/reading.component";
import { TanqueComponent } from "../water-tank/water.tank";

/**
 * @title Basic grid-list
 */
@Component({
  selector: 'dashboard',
  styleUrl: 'dashboard.css',
  templateUrl: 'dashboard.html',
  imports: [MatGridListModule, ReadingGraphComponent, TanqueComponent],
})
export class DashBoard {}
