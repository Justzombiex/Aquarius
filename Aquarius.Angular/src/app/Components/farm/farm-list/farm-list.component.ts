import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatButtonModule } from '@angular/material/button';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';

import { FarmService } from '../../../services/farm.service';
import { Farm } from '../../../models/farm.model';
import { signal, Signal } from '@angular/core';

@Component({
  selector: 'app-farm-list',
  imports: [CommonModule, MatButtonModule, MatTableModule, MatPaginatorModule],
  templateUrl: './farm-list.component.html',
  styleUrl: './farm-list.component.css',
})
export class FarmListComponent implements OnInit {
  private farmService = inject(FarmService);
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  farms = signal<Farm[]>([]);

  displayedColumns: string[] = ['id', 'location', 'name', 'actions'];
  dataSource = new MatTableDataSource<Farm>([]);

  ngOnInit(): void {
    this.loadFarm();
    /*  this.farmService.getFarms().subscribe({
        next:(resp)=>{
            console.log('API response', resp);

        },
        error:(err)=>{
            console.log('Error getting farms', err)

        },
        complete:()=>{

        }
     }) */
  }

  loadFarm() {
    this.farmService.getFarms().subscribe({
      next: (farms) => {
        console.log('Received farms:', farms);
        this.farms.set(farms);
        this.updateTableData();
      },
      error: (err) => {
        console.error('Error getting farms', err);
      },
    });
  }

  updateTableData(){
    this.dataSource.data = this.farms();
    this.dataSource.paginator = this.paginator;
  }

  navigateToForm(id?: string) {}

  deleteFarm(id?: string) {}
}
