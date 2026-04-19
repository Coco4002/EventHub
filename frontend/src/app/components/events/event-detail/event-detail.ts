import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventsService } from '../../../services/events';
import { AuthService } from '../../../services/auth';
import { InvitationsService } from '../../../services/invitations';
import { UsersService } from '../../../services/users';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { HttpClient } from '@angular/common/http';
import { EventRequestsService } from '../../../services/event-requests';

@Component({
  selector: 'app-event-detail',
  imports: [
    CommonModule,
    RouterLink,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDividerModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './event-detail.html',
  styleUrl: './event-detail.css'
})
export class EventDetailComponent implements OnInit {
  event: any = null;
  loading = true;
  currentUser: any;
  newComment = '';
  myInvitation: any = null;
  allUsers: any[] = [];
  selectedUserId: number | null = null;
  showInviteForm = false;
  myRequest: any = null;
  eventRequests: any[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventsService: EventsService,
    private authService: AuthService,
    private invitationsService: InvitationsService,
    private usersService: UsersService,
    private http: HttpClient,
    private cdr: ChangeDetectorRef,
    private eventRequestsService: EventRequestsService,
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadEvent(+id);
  }

  loadEvent(id: number): void {
    this.eventsService.getById(id).subscribe({
      next: (data) => {
        this.event = data;
        this.loading = false;
        if (this.currentUser) this.findMyInvitation();
        if (this.currentUser && !this.isOrganizer()) this.findMyRequest();
        if (this.isOrganizer() || this.isAdmin()) this.loadEventRequests();
        if (this.isOrganizer()) this.loadUsers();
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  findMyInvitation(): void {
    if (!this.event?.invitations) return;
    this.myInvitation = this.event.invitations.find(
      (i: any) => i.participant?.id === this.currentUser?.id
    );
  }

  loadUsers(): void {
    this.usersService.getAll().subscribe({
      next: (data) => {
        this.allUsers = data;
        this.cdr.detectChanges();
      }
    });
  }

  getUninvitedUsers(): any[] {
    const invitedIds = this.event?.invitations?.map((i: any) => i.participant.id) || [];
    return this.allUsers.filter(u =>
      u.id !== this.currentUser?.id && !invitedIds.includes(u.id)
    );
  }

  sendInvitation(): void {
    if (!this.selectedUserId) return;
    this.invitationsService.send({
      eventId: this.event.id,
      participantId: this.selectedUserId
    }).subscribe({
      next: () => {
        const user = this.allUsers.find(u => u.id === this.selectedUserId);
        this.event.invitations = [...this.event.invitations, {
          id: Date.now(),
          status: 'Pending',
          sentAt: new Date().toISOString(),
          participant: { id: user.id, fullName: user.fullName }
        }];
        this.selectedUserId = null;
        this.showInviteForm = false;
        this.cdr.detectChanges();
      }
    });
  }

  isOrganizer(): boolean {
    return this.event?.organizer?.id === this.currentUser?.id;
  }

  isAdmin(): boolean {
    return this.currentUser?.role === 'Admin';
  }

  canComment(): boolean {
    if (!this.currentUser) return false;
    if (this.isOrganizer() || this.isAdmin()) return true;
    return this.myInvitation?.status === 'Accepted';
  }

  respondInvitation(status: string): void {
    if (!this.myInvitation) return;
    this.invitationsService.respond(this.myInvitation.id, status).subscribe({
      next: () => {
        this.myInvitation.status = status;
        this.cdr.detectChanges();
      }
    });
  }

  submitComment(): void {
    if (!this.newComment.trim()) return;
    this.http.post('http://localhost:5090/api/comments', {
      eventId: this.event.id,
      content: this.newComment
    }).subscribe({
      next: (comment: any) => {
        this.event.comments = [...this.event.comments, {
          ...comment,
          user: { id: this.currentUser.id, fullName: this.currentUser.fullName }
        }];
        this.newComment = '';
        this.cdr.detectChanges();
      }
    });
  }

  deleteComment(commentId: number): void {
    this.http.delete(`http://localhost:5090/api/comments/${commentId}`).subscribe({
      next: () => {
        this.event.comments = this.event.comments.filter((c: any) => c.id !== commentId);
        this.cdr.detectChanges();
      }
    });
  }

  deleteEvent(): void {
    if (!confirm('Esti sigur ca vrei sa stergi acest eveniment?')) return;
    this.eventsService.delete(this.event.id).subscribe({
      next: () => this.router.navigate(['/events'])
    });
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('ro-RO', {
      day: '2-digit',
      month: 'long',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  findMyRequest(): void {
    this.eventRequestsService.getEventRequests(this.event.id).subscribe({
      next: () => {}
    });
    // cauta in event requests
    this.eventRequestsService.getMyRequests().subscribe({
      next: (data) => {
        this.myRequest = data.find((r: any) => r.event.id === this.event.id) || null;
        this.cdr.detectChanges();
      }
    });
  }

  loadEventRequests(): void {
    this.eventRequestsService.getEventRequests(this.event.id).subscribe({
      next: (data) => {
        this.eventRequests = data;
        this.cdr.detectChanges();
      }
    });
  }

  sendRequest(): void {
    this.eventRequestsService.sendRequest(this.event.id).subscribe({
      next: (data) => {
        this.myRequest = { id: data.id, status: 'Pending' };
        this.cdr.detectChanges();
      }
    });
  }

  respondRequest(requestId: number, status: string): void {
    this.eventRequestsService.respond(requestId, status).subscribe({
      next: () => {
        const req = this.eventRequests.find(r => r.id === requestId);
        if (req) req.status = status;
        this.cdr.detectChanges();
      }
    });
  }

  hasInvitation(): boolean {
    return !!this.myInvitation;
  }
}