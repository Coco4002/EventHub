import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EventsService {
  private apiUrl = 'http://localhost:5090/api/events';

  constructor(private http: HttpClient) {}

  getAll(categoryId?: number): Observable<any[]> {
    const url = categoryId ? `${this.apiUrl}?categoryId=${categoryId}` : this.apiUrl;
    return this.http.get<any[]>(url);
  }

  getById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  getMyEvents(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my`);
  }

  create(data: any): Observable<any> {
    return this.http.post(this.apiUrl, data);
  }

  update(id: number, data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, data);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}