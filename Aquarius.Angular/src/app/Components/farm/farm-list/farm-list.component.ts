import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatButtonModule } from '@angular/material/button';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';

import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatExpansionModule } from '@angular/material/expansion';

import { AlertListComponent } from "../../alert/alert-list/alert.list.component";

@Component({
  selector: 'app-farm-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSidenavModule,
    MatListModule,
    MatToolbarModule,
    MatExpansionModule,
    AlertListComponent,
],
  templateUrl: './farm-list.component.html',
  styleUrls: ['./farm-list.component.css'],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class FarmListComponent {
  selectedOption: string | null = null;

  selectOption(option: string) {
    this.selectedOption = option;
    console.log(`Opción seleccionada: ${option}`);
  }
}
