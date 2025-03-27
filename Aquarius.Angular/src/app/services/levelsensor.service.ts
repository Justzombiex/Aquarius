import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LevelSensor } from '../models/levelsensor.model';

@Injectable({
  providedIn: 'root'
})
export class LevelSensorService {
  private apiUrl = 'https://localhost:7185/api/LevelSensors';

  constructor(private http: HttpClient) { }

  getLevelSensors(): Observable<LevelSensor[]> {
    return this.http.get<LevelSensor[]>(this.apiUrl);
  }

  getLevelSensor(id: string): Observable<LevelSensor> {
    return this.http.get<LevelSensor>(`${this.apiUrl}/${id}`);
  }

  createLevelSensor(levelSensor: LevelSensor): Observable<LevelSensor> {
    return this.http.post<LevelSensor>(this.apiUrl, levelSensor);
  }

  updateLevelSensor(id: string, levelSensor: LevelSensor): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, levelSensor);
  }

  deleteLevelSensor(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
