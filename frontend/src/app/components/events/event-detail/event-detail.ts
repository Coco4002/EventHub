import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventsService } from '../../../services/events';
import { AuthService } from '../../../services/auth';
import { InvitationsService } from '../../../services/invitations';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { HttpClient } from '@angular/common/http';

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
    MatInputModule
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

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventsService: EventsService,
    private authService: AuthService,
    private invitationsService: InvitationsService,
    private http: HttpClient,
    private cdr: ChangeDetectorRef
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
        this.event.comments.push({
          ...comment,
          user: { id: this.currentUser.id, fullName: this.currentUser.fullName }
        });
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
}