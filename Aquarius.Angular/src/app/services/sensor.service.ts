import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TemperatureSensor } from '../models/sensor.model';

@Injectable({
  providedIn: 'root'
})
export class TemperatureSensorService {
  private apiUrl = 'https://localhost:7185/api/TemperatureSensors';

  constructor(private http: HttpClient) { }

  getTemperatureSensors(): Observable<TemperatureSensor[]> {
    return this.http.get<TemperatureSensor[]>(this.apiUrl);
  }

  getTemperatureSensor(id: string): Observable<TemperatureSensor> {
    return this.http.get<TemperatureSensor>(`${this.apiUrl}/${id}`);
  }

  createTemperatureSensor(temperatureSensor: TemperatureSensor): Observable<TemperatureSensor> {
    return this.http.post<TemperatureSensor>(this.apiUrl, temperatureSensor);
  }

  updateTemperatureSensor(id: string, temperatureSensor: TemperatureSensor): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, temperatureSensor);
  }

  deleteTemperatureSensor(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
