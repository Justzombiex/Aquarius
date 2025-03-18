import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Pond } from '../models/pond.model';

@Injectable({
  providedIn: 'root'
})
export class PondService {
  private apiUrl = 'https://localhost:5001/api/Ponds';

  constructor(private http: HttpClient) { }

  getPonds(): Observable<Pond[]> {
    return this.http.get<Pond[]>(this.apiUrl);
  }

  getPond(id: string): Observable<Pond> {
    return this.http.get<Pond>(`${this.apiUrl}/${id}`);
  }

  createPond(pond: Pond): Observable<Pond> {
    return this.http.post<Pond>(this.apiUrl, pond);
  }

  updatePond(id: string, pond: Pond): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, pond);
  }

  deletePond(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}