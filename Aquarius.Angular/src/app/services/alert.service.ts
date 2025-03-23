import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Alert } from '../models/alert.model';
import { environment } from '../../environments/environment.alert';

@Injectable({
  providedIn: 'root'
})
export class AlertService {

  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7185/api/Alerts';

  getAlerts(): Observable<Alert[]> {
    return this.http.get<Alert[]>(this.apiUrl);
  }

  getAlert(id: string): Observable<Alert> {
    return this.http.get<Alert>(`${this.apiUrl}/${id}`);
  }

  createAlert(alert: Alert): Observable<Alert> {
    return this.http.post<Alert>(this.apiUrl, alert);
  }

  updateAlert(id: string, alert: Alert): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, alert);
  }

  deleteAlert(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
