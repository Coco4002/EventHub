import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatSelectModule } from '@angular/material/select';
import { MatDividerModule } from '@angular/material/divider';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin',
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatSelectModule,
    MatDividerModule
  ],
  templateUrl: './admin.html',
  styleUrl: './admin.css'
})
export class AdminComponent implements OnInit {
  users: any[] = [];
  stats: any = null;
  loading = true;
  private apiUrl = 'http://localhost:5090/api/admin';

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadStats();
    this.loadUsers();
  }

  loadStats(): void {
    this.http.get(`${this.apiUrl}/stats`).subscribe({
      next: (data) => {
        this.stats = data;
        this.cdr.detectChanges();
      }
    });
  }

  loadUsers(): void {
    this.http.get<any[]>(`${this.apiUrl}/users`).subscribe({
      next: (data) => {
        this.users = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  changeRole(userId: number, role: string): void {
    this.http.put(`${this.apiUrl}/users/${userId}/role`, JSON.stringify(role), {
      headers: { 'Content-Type': 'application/json' }
    }).subscribe({
      next: () => {
        const user = this.users.find(u => u.id === userId);
        if (user) user.role = role;
        this.cdr.detectChanges();
      }
    });
  }

  toggleActive(user: any): void {
    const endpoint = user.isActive ? 'deactivate' : 'activate';
    this.http.put(`${this.apiUrl}/users/${user.id}/${endpoint}`, {}).subscribe({
      next: () => {
        user.isActive = !user.isActive;
        this.cdr.detectChanges();
      }
    });
  }

  deleteUser(userId: number): void {
    if (!confirm('Esti sigur ca vrei sa stergi acest cont?')) return;
    this.http.delete(`${this.apiUrl}/users/${userId}`).subscribe({
      next: () => {
        this.users = this.users.filter(u => u.id !== userId);
        this.cdr.detectChanges();
      }
    });
  }

  getCurrentUserId(): number {
    return this.authService.getCurrentUser()?.id;
  }
}