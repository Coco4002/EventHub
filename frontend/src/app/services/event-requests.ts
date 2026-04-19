import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EventRequestsService {
  private apiUrl = 'http://localhost:5090/api/eventrequests';

  constructor(private http: HttpClient) {}

  sendRequest(eventId: number): Observable<any> {
    return this.http.post(this.apiUrl, eventId);
  }

  getEventRequests(eventId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/event/${eventId}`);
  }

  getMyRequests(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my`);
  }

  respond(id: number, status: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/respond`, JSON.stringify(status), {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}