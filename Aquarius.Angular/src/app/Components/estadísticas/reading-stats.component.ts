import { Component, OnInit, OnDestroy } from '@angular/core';
import { ReadingService } from '../../services/reading.service';
import { Reading } from '../../models/reading.model';
import { DatePipe, TitleCasePipe, NgIf, NgFor } from '@angular/common';

@Component({
  selector: 'app-reading-stats',
  standalone: true,
  templateUrl: './reading-stats.component.html',
  styleUrls: ['./reading-stats.component.css'],
  imports: [
    DatePipe,
    TitleCasePipe,
    NgIf,
    NgFor
  ]
})
export class ReadingStatsComponent implements OnInit, OnDestroy {
  private intervalId: any;
  
  // Estadísticas
  maxTemp: number | null = null;
  minTemp: number | null = null;
  dailyAverages: {date: string, average: number}[] = [];
  
  
  // Fechas para el filtro
  currentMonth: Date = new Date();
  loading: boolean = true;

  constructor(private readingService: ReadingService) {}

  ngOnInit(): void {
    this.loadStats();
    // Actualizar estadísticas cada 30 segundos (ajustable)
    this.intervalId = setInterval(() => {
      this.loadStats();
    }, 30000);
  }

  ngOnDestroy(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

  // Propiedad calculada para el promedio mensual
  get monthlyAverage(): string {
    if (this.dailyAverages.length === 0) return 'Sin datos';
    const total = this.dailyAverages.reduce((sum, day) => sum + day.average, 0);
    const average = total / this.dailyAverages.length;
    return `${average.toFixed(1)}°C`;  // Usando toFixed() en lugar de DecimalPipe
  }

  loadStats(): void {
    this.loading = true;
    this.readingService.getReadings().subscribe({
      next: (data: Reading[]) => {
        this.filterAndCalculateStats(data);
        this.loading = false;
      },
      error: (err) => {
        console.error('Error al obtener lecturas:', err);
        this.loading = false;
      }
    });
  }

  private filterAndCalculateStats(allReadings: Reading[]): void {
    // Obtener el rango de fechas del mes actual
    const startDate = new Date(
      this.currentMonth.getFullYear(), 
      this.currentMonth.getMonth(), 
      1
    );
    const endDate = new Date(
      this.currentMonth.getFullYear(), 
      this.currentMonth.getMonth() + 1, 
      0, 23, 59, 59
    );

    // Filtrar lecturas del mes actual
    const readings = allReadings.filter(reading => {
      const readingDate = new Date(reading.timestamp);
      return readingDate >= startDate && readingDate <= endDate;
    });

    if (readings.length === 0) {
      this.maxTemp = null;
      this.minTemp = null;
      this.dailyAverages = [];
      return;
    }

    // Calcular máximos y mínimos
    this.maxTemp = Math.max(...readings.map(r => r.value));
    this.minTemp = Math.min(...readings.map(r => r.value));

    // Calcular promedios diarios
    const readingsByDay: {[key: string]: number[]} = {};
    
    readings.forEach(reading => {
      const date = new Date(reading.timestamp).toISOString().split('T')[0];
      if (!readingsByDay[date]) {
        readingsByDay[date] = [];
      }
      readingsByDay[date].push(reading.value);
    });

    this.dailyAverages = Object.keys(readingsByDay).map(date => {
      const values = readingsByDay[date];
      const sum = values.reduce((a, b) => a + b, 0);
      const average = sum / values.length;
      return { date, average: parseFloat(average.toFixed(2)) };
    }).sort((a, b) => a.date.localeCompare(b.date));
  }

  changeMonth(offset: number): void {
    this.currentMonth = new Date(
      this.currentMonth.getFullYear(),
      this.currentMonth.getMonth() + offset,
      1
    );
    this.loadStats();
  }
}