import { Component, inject, OnInit, ViewChild, CUSTOM_ELEMENTS_SCHEMA, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { AlertService } from '../../../../services/alert.service';
import { Alert } from '../../../../models/alert.model';

@Component({
  selector: 'app-alert-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
  ],
  templateUrl: './alert-dialog.component.html',
  styleUrls: ['./alert-dialog.component.css'],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class AlertDialogComponent implements OnInit {
  alerts = signal<Alert[]>([]); // Manejo de alertas
  displayedColumns: string[] = ['message', 'timeStamp']; // Añade "timeStamp" para incluir la columna de la fecha
  dataSource = new MatTableDataSource<Alert>(); // Fuente de datos de la tabla

  @ViewChild(MatPaginator) paginator!: MatPaginator; // Conecta el paginador

  private alertService = inject(AlertService);

  ngOnInit(): void {
    this.fetchAlerts();
  }

  fetchAlerts(): void {
    this.alertService.getAlerts().subscribe({
      next: (data) => {
        this.alerts.set(data); // Actualiza el signal
        this.dataSource.data = data; // Actualiza la tabla con los datos
        this.dataSource.paginator = this.paginator; // Conecta el paginador
        this.paginator.pageSize = 5; // Muestra solo 5 elementos por página
      },
      error: (err) => {
        console.error('Error al obtener las alertas:', err);
      },
    });
  }
}
