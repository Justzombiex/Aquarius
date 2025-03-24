import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ReadingService } from '../../services/reading.service';
import { Reading } from '../../models/reading.model';
import Chart from 'chart.js/auto'; // Importación directa de Chart.js

@Component({
  selector: 'app-reading-graph',
  standalone: true,
  templateUrl: './reading.component.html',
  styleUrls: ['./reading.component.css']
})
export class ReadingGraphComponent implements OnInit {
  @ViewChild('chartCanvas', { static: true }) chartCanvas!: ElementRef<HTMLCanvasElement>; // Referencia al canvas
  private chart!: Chart; // Instancia del gráfico

  constructor(private readingService: ReadingService) {}

  ngOnInit(): void {
    this.fetchReadings(); // Obtenemos los datos al inicializar
  }

  fetchReadings(): void {
    this.readingService.getReadings().subscribe({
      next: (data: Reading[]) => {
        const temperatureValues = data.map(reading => reading.value); // Extraemos los valores de temperatura
        const timestamps = data.map(reading => new Date(reading.timestamp).toLocaleString()); // Extraemos las fechas

        this.renderLineChart(temperatureValues, timestamps); // Renderizamos el gráfico con líneas
      },
      error: (err) => {
        console.error('Error al obtener lecturas:', err);
      },
    });
  }

  renderLineChart(temperatureValues: number[], timestamps: string[]): void {
    this.chart = new Chart(this.chartCanvas.nativeElement, {
      type: 'line', // Tipo de gráfico: línea
      data: {
        labels: timestamps,
        datasets: [
          {
            label: 'Temperatura',
            data: temperatureValues,
            backgroundColor: 'rgba(54, 162, 235, 0.2)', // Azul claro para el área
            borderColor: 'rgba(54, 162, 235, 1)', // Azul oscuro para las líneas
            borderWidth: 2, // Anchura de las líneas
            pointBackgroundColor: 'rgba(54, 162, 235, 1)', // Azul oscuro para los puntos
            pointBorderColor: '#ffffff', // Blanco para los bordes de puntos
            tension: 0.4, // Suaviza las líneas entre puntos
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false, // Permite ajustar el tamaño sin deformaciones
        plugins: {
          legend: { display: true }, // Mostramos la leyenda
        },
        scales: {
          x: {
            title: {
              display: true,
              text: 'Tiempo',
            },
          },
          y: {
            beginAtZero: true, // Comienza en 0 el eje Y
            title: {
              display: true,
              text: 'Temperatura',
            },
          },
        },
      },
    });
  }
}
