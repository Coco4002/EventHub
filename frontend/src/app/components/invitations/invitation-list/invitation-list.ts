import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { InvitationsService } from '../../../services/invitations';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-invitation-list',
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDividerModule,
    MatTabsModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './invitation-list.html',
  styleUrl: './invitation-list.css'
})
export class InvitationListComponent implements OnInit {
  invitations: any[] = [];
  loading = true;

  constructor(
    private invitationsService: InvitationsService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadInvitations();
  }

  loadInvitations(): void {
    this.invitationsService.getMyInvitations().subscribe({
      next: (data) => {
        this.invitations = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  getPending(): any[] {
    return this.invitations.filter(i => i.status === 'Pending');
  }

  getAccepted(): any[] {
    return this.invitations.filter(i => i.status === 'Accepted');
  }

  getDeclined(): any[] {
    return this.invitations.filter(i => i.status === 'Declined');
  }

  respond(invitationId: number, status: string): void {
    this.invitationsService.respond(invitationId, status).subscribe({
      next: () => {
        const inv = this.invitations.find(i => i.id === invitationId);
        if (inv) inv.status = status;
        this.cdr.detectChanges();
      }
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