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
        const timestamps = data.map(reading => 
          new Date(reading.timestamp).toLocaleTimeString() // Extraemos únicamente la hora
        );

        const pointColors = temperatureValues.map(value => 
          value > 35 
            ? 'rgba(255, 0, 0, 1)' // Rojo si supera 35
            : value < 25 
              ? 'rgb(95, 244, 255)' // Azul claro (hielo) si es menor a 25
              : 'rgb(5, 114, 187)' // Azul oscuro para el resto
        );
        

        this.renderLineChart(temperatureValues, timestamps, pointColors); // Renderizamos el gráfico con lógica de colores
      },
      error: (err) => {
        console.error('Error al obtener lecturas:', err);
      },
    });
  }

  renderLineChart(temperatureValues: number[], timestamps: string[], pointColors: string[]): void {
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
            pointBackgroundColor: pointColors, // Color dinámico para los puntos
            pointBorderColor: '#ffffff', // Blanco para bordes de los puntos
            pointRadius: 6, // Tamaño de los puntos
            pointHoverRadius: 8, // Tamaño más grande al pasar el mouse
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
              text: 'Hora',
              color: '#004b6b', // Azul profundo
              font: {
                size: 16,
                weight: 'bold',
              },
            },
            ticks: {
              color: '#0277bd', // Azul claro para las etiquetas
            },
          },
          y: {
            beginAtZero: true,
            title: {
              display: true,
              text: 'Temperatura',
              color: '#004b6b',
              font: {
                size: 16,
                weight: 'bold',
              },
            },
            ticks: {
              color: '#0277bd',
            },
            grid: {
              color: 'rgba(54, 162, 235, 0.2)', // Líneas del grid en azul traslúcido
            },
          },
        },
      },
    });
  }
}
