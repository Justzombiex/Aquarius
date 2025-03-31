import { Component, OnInit, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AlertService } from '../../../../services/alert.service';
import { Alert } from '../../../../models/alert.model';
import { AlertDialogComponent } from '../alert-dialog/alert-dialog.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';

// Enum para representar los tipos de alarmas
enum AlarmType {
  HighTemperature = 0,
  LowTemperature = 1,
  Disconnection = 2,
  LowLevel = 3,
}

@Component({
  selector: 'app-alert-list',
  templateUrl: './alert.list.component.html',
  styleUrls: ['./alert.list.component.css'],
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule],
})
export class AlertListComponent implements OnInit {
  private alertService = inject(AlertService);
  private dialog = inject(MatDialog);
  private cdr = inject(ChangeDetectorRef); // Para manejar cambios en la interfaz

  alerts: Alert[] = [];
  localAckStates = {
    highTemp: false,
    lowTemp: false,
    disconnection: false,
    lowLevel: false,
  };

  ngOnInit(): void {
    this.alertService.getAlerts().subscribe({
      next: (data) => {
        this.alerts = data;
        console.log('Alertas cargadas:', this.alerts);
      },
      error: (err) => {
        console.error('Error al cargar las alertas:', err);
      },
    });

    // Monitoreo constante
    this.startAlertMonitoring();
  }

  private startAlertMonitoring(): void {
    setInterval(() => {
      this.alertService.getAlerts().subscribe({
        next: (data) => {
          this.alerts = data;
          console.log('Alertas actualizadas:', this.alerts);
        },
        error: (err) => {
          console.error('Error al actualizar las alertas:', err);
        },
      });
    }, 2000); // Intervalo de 2 segundos
  }

  openAlertDialog(): void {
    this.dialog.open(AlertDialogComponent, {
      width: '90%',
      height: '60%',
      data: this.alerts,
    });
  }

  hasActiveHighTemperatureAlarms(): boolean {
    return this.alerts.some(
      (alert) => alert.isActive && Number(alert.alarmType) === AlarmType.HighTemperature
    );
  }

  hasActiveLowTemperatureAlarms(): boolean {
    return this.alerts.some(
      (alert) => alert.isActive && Number(alert.alarmType) === AlarmType.LowTemperature
    );
  }

  hasActiveDisconnectionAlarms(): boolean {
    return this.alerts.some(
      (alert) => alert.isActive && Number(alert.alarmType) === AlarmType.Disconnection
    );
  }

  hasActiveLowLevelAlarms(): boolean {
    return this.alerts.some(
      (alert) => alert.isActive && Number(alert.alarmType) === AlarmType.LowLevel
    );
  }
}
