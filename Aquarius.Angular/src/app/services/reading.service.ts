import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Reading } from '../models/reading.model';

@Injectable({
  providedIn: 'root'
})
export class ReadingService {
  private apiUrl = 'https://localhost:5001/api/Readings';

  constructor(private http: HttpClient) { }

  getReadings(): Observable<Reading[]> {
    return this.http.get<Reading[]>(this.apiUrl);
  }

  getReading(id: string): Observable<Reading> {
    return this.http.get<Reading>(`${this.apiUrl}/${id}`);
  }

  createReading(reading: Reading): Observable<Reading> {
    return this.http.post<Reading>(this.apiUrl, reading);
  }

  updateReading(id: string, reading: Reading): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, reading);
  }

  deleteReading(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}