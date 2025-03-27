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
  
  // Estados locales de reconocimiento (actualizado con dryPond)
  localAckStates = {
    highTemp: false,
    lowTemp: false,
    disconnection: false,
    dryPond: false
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

  // Método solo para interfaz (actualizado con dryPond)
  acknowledgeLocalAlert(type: 'highTemp' | 'lowTemp' | 'disconnection' | 'dryPond'): void {
    this.localAckStates[type] = true;
    
    // Opcional: Aquí podrías añadir lógica para notificar al backend
    // cuando una alerta es reconocida localmente
    console.log(`Alarma ${type} reconocida localmente`);
  }

  // Verifica estado local (actualizado con dryPond)
  isLocalAcknowledged(type: 'highTemp' | 'lowTemp' | 'disconnection' | 'dryPond'): boolean {
    return this.localAckStates[type];
  }

  // Método para activación manual (actualizado con dryPond)
  activateManualAlert(alertType: 'highTemp' | 'lowTemp' | 'disconnection' | 'dryPond') {
    console.log(`Activando manualmente alarma: ${alertType}`);
    
    // Resetear el estado de reconocimiento
    this.localAckStates[alertType] = false;
    
    // Aquí puedes agregar lógica para simular la activación
    // o conectar con tu backend cuando esté listo
    
    // Ejemplo de simulación:
    this.simulateAlert(alertType);
  }

  // Método opcional para simular alertas (puedes eliminarlo en producción)
  private simulateAlert(type: string): void {
    console.log(`Simulando alerta de ${type}`);
    // Aquí podrías añadir lógica para simular la alerta
    // Por ejemplo, mostrar un mensaje o cambiar algún estado temporal
  }
}