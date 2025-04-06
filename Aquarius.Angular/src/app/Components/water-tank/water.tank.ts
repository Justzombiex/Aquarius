import { Component, inject, OnInit, OnDestroy} from '@angular/core';
import { LevelSensorService } from '../../services/levelsensor.service';
import { LevelSensor } from '../../models/levelsensor.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-tank',
  standalone: true,
  imports: [CommonModule], // Importa CommonModule para habilitar directivas como *ngIf
  templateUrl: './water.tank.html',
  styleUrls: ['./water.tank.css'],
})
export class TanqueComponent implements OnInit, OnDestroy { // Implementa OnDestroy
  private levelSensorService = inject(LevelSensorService); // Inyección directa del servicio

  isTankFull: boolean = false; // Estado inicial del tanque
  private intervalId: any; // ID del intervalo para poder limpiarlo

  ngOnInit(): void {
    this.getTankStatus(); // Llamada inicial
    // Ejecutar cada 5 segundos
    this.intervalId = setInterval(() => {
      this.getTankStatus();
    }, 2000);
  }

  ngOnDestroy(): void {
    // Limpiar el intervalo cuando el componente se destruya
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

  // Método para obtener el estado del tanque desde todos los sensores
  getTankStatus(): void {
    this.levelSensorService.getLevelSensors()
      .subscribe({
        next: (sensors: LevelSensor[]) => {
          if (sensors.length > 0) {
            const sensor = sensors[0]; // Obtenemos el primer sensor
            console.log('Valor recibido de isFull:', sensor.fullPond); // Verifica qué valor llega
            this.isTankFull = sensor.fullPond; // Actualizamos el estado
          } else {
            console.error('No se encontraron sensores de nivel.');
          }
        },
        error: (err) => {
          console.error('Error al obtener los sensores de nivel:', err);
        },
      });
  }
}
