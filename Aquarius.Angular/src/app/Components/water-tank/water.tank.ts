import { Component } from '@angular/core';

@Component({
  selector: 'app-tank',
  templateUrl: './water.tank.html',
  styleUrls: ['./water.tank.css']
})
export class TanqueComponent {
  nivelAgua: number = 50; // Nivel inicial del agua en porcentaje

  // Cambia el nivel del agua
  cambiarNivel(nuevoNivel: number): void {
    this.nivelAgua = nuevoNivel; // Actualiza el nivel dinámicamente
  }
}
