import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CategoriesService {
  private apiUrl = 'http://localhost:5090/api/categories';

  constructor(private http: HttpClient) {}

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getMyCategories(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my`);
  }

  subscribe(id: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/subscribe`, {});
  }

  unsubscribe(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}/unsubscribe`);
  }
}