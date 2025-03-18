import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Sensor } from '../models/sensor.model';

@Injectable({
  providedIn: 'root'
})
export class SensorService {
  private apiUrl = 'https://localhost:5001/api/Sensors';

  constructor(private http: HttpClient) { }

  getSensors(): Observable<Sensor[]> {
    return this.http.get<Sensor[]>(this.apiUrl);
  }

  getSensor(id: string): Observable<Sensor> {
    return this.http.get<Sensor>(`${this.apiUrl}/${id}`);
  }

  createSensor(sensor: Sensor): Observable<Sensor> {
    return this.http.post<Sensor>(this.apiUrl, sensor);
  }

  updateSensor(id: string, sensor: Sensor): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, sensor);
  }

  deleteSensor(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}