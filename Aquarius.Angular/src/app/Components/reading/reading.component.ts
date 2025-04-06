import { Component, ElementRef, OnInit, OnDestroy, ViewChild } from '@angular/core'; // Agrega OnDestroy
import { ReadingService } from '../../services/reading.service';
import { Reading } from '../../models/reading.model';
import Chart from 'chart.js/auto'; // Importación directa de Chart.js

@Component({
  selector: 'app-reading-graph',
  standalone: true,
  templateUrl: './reading.component.html',
  styleUrls: ['./reading.component.css'],
})
export class ReadingGraphComponent implements OnInit, OnDestroy { // Implementa OnDestroy
  @ViewChild('chartCanvas', { static: true }) chartCanvas!: ElementRef<HTMLCanvasElement>; // Referencia al canvas
  private chart!: Chart; // Instancia del gráfico
  private intervalId: any; // ID del intervalo

  constructor(private readingService: ReadingService) {}

  ngOnInit(): void {
    this.fetchReadings(); // Llamada inicial
    // Ejecutar fetchReadings cada 2 segundos
    this.intervalId = setInterval(() => {
      this.fetchReadings();
    }, 2000); // 2000 ms = 2 segundos
  }

  ngOnDestroy(): void {
    // Limpiar el intervalo para evitar fugas de memoria
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

  fetchReadings(): void {
    this.readingService.getReadings().subscribe({
      next: (data: Reading[]) => {
        // Ordenar los datos por marca de tiempo y tomar los últimos 10 valores
        const recentData = data
          .slice(-10) // Toma los últimos 10 elementos
          .sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()); // Ordena por timestamp ascendente
  
        const temperatureValues = recentData.map((reading) => reading.value); // Extraemos los valores de temperatura
        const timestamps = recentData.map((reading) =>
          new Date(reading.timestamp).toLocaleTimeString() // Extraemos únicamente la hora
        );
  
        const pointColors = temperatureValues.map((value) =>
          value > 33
            ? 'rgba(255, 0, 0, 1)' // Rojo si supera 35
            : value < 24
            ? 'rgb(95, 244, 255)' // Azul claro si es menor a 25
            : 'rgb(5, 114, 187)' // Azul oscuro para el resto
        );
  
        this.renderLineChart(temperatureValues, timestamps, pointColors); // Renderizamos el gráfico
      },
      error: (err) => {
        console.error('Error al obtener lecturas:', err);
      },
    });
  }
  
  

  renderLineChart(temperatureValues: number[], timestamps: string[], pointColors: string[]): void {
    if (this.chart) {
      // Actualiza los datos y etiquetas del gráfico existente
      this.chart.data.labels = timestamps; // Actualiza las etiquetas
      this.chart.data.datasets[0].data = temperatureValues; // Actualiza los valores
      this.chart.data.datasets[0].backgroundColor = pointColors; // Actualiza el color de los puntos
      this.chart.update(); // Aplica los cambios al gráfico
    } else {
      // Crea el gráfico solo si no existe
      this.chart = new Chart(this.chartCanvas.nativeElement, {
        type: 'line',
        data: {
          labels: timestamps,
          datasets: [
            {
              label: 'Temperatura',
              data: temperatureValues,
              backgroundColor: pointColors, // Uso de backgroundColor para el color de los puntos
              borderColor: 'rgba(54, 162, 235, 1)', // Azul oscuro para las líneas
              borderWidth: 2, // Anchura de las líneas
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
                color: '#004b6b',
                font: { size: 16, weight: 'bold' },
              },
              ticks: { color: '#0277bd' },
            },
            y: {
              beginAtZero: true,
              title: {
                display: true,
                text: 'Temperatura',
                color: '#004b6b',
                font: { size: 16, weight: 'bold' },
              },
              ticks: { color: '#0277bd' },
              grid: { color: 'rgba(54, 162, 235, 0.2)' },
            },
          },
        },
      });
    }
  }
  
}
