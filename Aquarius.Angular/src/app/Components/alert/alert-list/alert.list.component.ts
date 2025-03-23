import { Component, inject, OnInit, signal, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { AlertService } from '../../../services/alert.service';
import { Alert } from '../../../models/alert.model';

@Component({
  selector: 'app-alert-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
  ],
  templateUrl: './alert.list.component.html',
  styleUrls: ['./alert.list.component.css'],
  schemas: [CUSTOM_ELEMENTS_SCHEMA], // Aquí se agrega el esquema
})
export class AlertListComponent implements OnInit {
  alerts = signal<Alert[]>([]); // Signal para manejar alertas
  displayedColumns: string[] = ['message']; // Columnas mostradas en la tabla
  dataSource = new MatTableDataSource<Alert>(); // Fuente de datos de la tabla

  private alertService = inject(AlertService);

  ngOnInit(): void {
    this.fetchAlerts();
  }

  fetchAlerts(): void {
    this.alertService.getAlerts().subscribe({
      next: (data) => {
        this.alerts.set(data); // Actualiza las señales
        this.dataSource.data = data; // Actualiza la tabla
      },
      error: (err) => {
        console.error('Error al obtener las alertas:', err);
      },
    });
  }
}
