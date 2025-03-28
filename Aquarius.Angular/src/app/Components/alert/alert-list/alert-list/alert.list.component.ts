import { Component, OnInit, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AlertService } from '../../../../services/alert.service';
import { Alert } from '../../../../models/alert.model';
import { AlertDialogComponent } from '../alert-dialog/alert-dialog.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';

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

  alerts: Alert[] = [];
  localAckStates = {
    highTemp: false,
    lowTemp: false,
    disconnection: false,
    lowLevel: false,
  };

  ngOnInit(): void {
    // Cargar las alertas desde el servicio
    this.alertService.getAlerts().subscribe({
      next: (data) => {
        this.alerts = data; // Guardar las alertas en la variable local
        console.log('Alertas cargadas:', this.alerts);

        // Verificar el estado de las alarmas usando métodos
        console.log('Alta temperatura activa:', this.hasActiveHighTemperatureAlarms());
        console.log('Baja temperatura activa:', this.hasActiveLowTemperatureAlarms());
        console.log('Desconexión activa:', this.hasActiveDisconnectionAlarms());
        console.log('Nivel bajo activo:', this.hasActiveLowLevelAlarms());
      },
      error: (err) => {
        console.error('Error al cargar las alertas:', err);
      },
    });
  }

  openAlertDialog(): void {
    this.dialog.open(AlertDialogComponent, {
      width: '90%',
      height: '60%',
      data: this.alerts,
    });
  }

  // Métodos para verificar el estado de cada tipo de alarma
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

  acknowledgeLocalAlert(type: 'highTemp' | 'lowTemp' | 'disconnection' | 'lowLevel'): void {
    this.localAckStates[type] = true;
    console.log(`Alarma ${type} reconocida localmente`);
  }

  isLocalAcknowledged(type: 'highTemp' | 'lowTemp' | 'disconnection' | 'lowLevel'): boolean {
    return this.localAckStates[type];
  }

  activateManualAlert(alertType: 'highTemp' | 'lowTemp' | 'disconnection' | 'lowLevel'): void {
    console.log(`Activando manualmente alarma: ${alertType}`);
    this.localAckStates[alertType] = false;
    this.simulateAlert(alertType);
  }

  private simulateAlert(type: string): void {
    console.log(`Simulando alerta de ${type}`);
  }
}
