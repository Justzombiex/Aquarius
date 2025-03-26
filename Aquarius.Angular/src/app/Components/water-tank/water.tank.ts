import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LevelSensorService } from '../../services/levelsensor.service';
import { LevelSensor } from '../../models/levelsensor.model';

@Component({
  selector: 'app-tank',
  templateUrl: './water.tank.html',
  styleUrls: ['./water.tank.css'],
  imports: [CommonModule]
})
export class TanqueComponent implements OnInit {
  isTankFull: boolean = false; // Estado inicial del tanque
  mensaje: string = 'Cargando estado del tanque...'; // Mensaje inicial

  constructor(private levelSensorService: LevelSensorService) {}

  ngOnInit(): void {
    this.getTankStatus();
  }

  // Obtiene el estado del tanque desde el sensor de nivel
  getTankStatus(): void {
    this.levelSensorService.getLevelSensor('your-sensor-id') // Reemplaza 'your-sensor-id' con el ID del sensor
      .subscribe((sensor: LevelSensor) => {
        this.isTankFull = sensor.isFull;
        this.mensaje = this.isTankFull ? 'El tanque está lleno' : 'El tanque está vacío';
      }, error => {
        console.error('Error al obtener el estado del tanque:', error);
        this.mensaje = 'No se pudo obtener el estado del tanque.';
      });
  }
}
