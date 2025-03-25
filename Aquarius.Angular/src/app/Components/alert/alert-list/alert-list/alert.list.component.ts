import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AlertService } from '../../../../services/alert.service';
import { Alert } from '../../../../models/alert.model';
import { AlertDialogComponent } from '../alert-dialog/alert-dialog.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-alert-list',
  templateUrl: './alert.list.component.html',
  styleUrls: ['./alert.list.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
  ],
})
export class AlertListComponent implements OnInit {
  private alertService = inject(AlertService);
  private dialog = inject(MatDialog);

  alerts: Alert[] = [];
  isAlarmActive: boolean = false;
  
  // Estados locales de reconocimiento (solo interfaz)
  localAckStates = {
    highTemp: false,
    lowTemp: false,
    disconnection: false
  };

  ngOnInit(): void {
    this.fetchAlerts();
  }

  fetchAlerts(): void {
    this.alertService.getAlerts().subscribe({
      next: (data) => {
        this.alerts = data;
        this.isAlarmActive = this.alerts.length > 0;
      },
      error: (err) => {
        console.error('Error al obtener las alertas:', err);
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

  // Método solo para interfaz
  acknowledgeLocalAlert(type: 'highTemp' | 'lowTemp' | 'disconnection'): void {
    this.localAckStates[type] = true;
  }

  // Verifica estado local
  isLocalAcknowledged(type: 'highTemp' | 'lowTemp' | 'disconnection'): boolean {
    return this.localAckStates[type];
  }

  activateManualAlert(alertType: 'highTemp' | 'lowTemp' | 'disconnection') {
    // Lógica para activación manual
    console.log(`Activando manualmente alarma: ${alertType}`);
    
    // Ejemplo: Resetear el estado de reconocimiento
    this.localAckStates[alertType] = false;
    
    // Aquí puedes agregar lógica para simular la activación
    // o conectar con tu backend cuando esté listo
  }
}