import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InvitationsService {
  private apiUrl = 'http://localhost:5090/api/invitations';

  constructor(private http: HttpClient) {}

  getMyInvitations(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my`);
  }

  getEventInvitations(eventId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/event/${eventId}`);
  }

  send(data: { eventId: number; participantId: number }): Observable<any> {
    return this.http.post(this.apiUrl, data);
  }

  respond(id: number, status: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/respond`, { status });
  }
}