import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Farm } from '../models/farm.model';
import { environment } from '../../environments/environment.farm';

@Injectable({
  providedIn: 'root'
})
export class FarmService {

  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7185/api/Farms';


  getFarms(): Observable<Farm[]> {
    return this.http.get<Farm[]>(this.apiUrl);
  }

  getFarm(id: string): Observable<Farm> {
    return this.http.get<Farm>(`${this.apiUrl}/${id}`);
  }

  createFarm(farm: Farm): Observable<Farm> {
    return this.http.post<Farm>(this.apiUrl, farm);
  }

  updateFarm(id: string, farm: Farm): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, farm);
  }

  deleteFarm(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}