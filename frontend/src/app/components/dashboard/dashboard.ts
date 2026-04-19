import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { EventsService } from '../../services/events';
import { CategoriesService } from '../../services/categories';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent implements OnInit {
  currentUser: any;
  myEvents: any[] = [];
  myCategories: any[] = [];
  allCategories: any[] = [];
  loading = true;

  constructor(
    private authService: AuthService,
    private eventsService: EventsService,
    private categoriesService: CategoriesService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.loadMyCategories();
    this.loadAllCategories();

    if (this.isOrganizer()) {
      this.loadMyEvents();
    } else {
      this.loading = false;
    }
  }

  loadMyEvents(): void {
    this.eventsService.getMyEvents().subscribe({
      next: (data) => {
        this.myEvents = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadMyCategories(): void {
    this.categoriesService.getMyCategories().subscribe({
      next: (data) => {
        this.myCategories = data;
        this.cdr.detectChanges();
      }
    });
  }

  loadAllCategories(): void {
    this.categoriesService.getAll().subscribe({
      next: (data) => {
        this.allCategories = data;
        this.cdr.detectChanges();
      }
    });
  }

  isSubscribed(categoryId: number): boolean {
    return this.myCategories.some(c => c.id === categoryId);
  }

  subscribeCategory(categoryId: number): void {
    this.categoriesService.subscribe(categoryId).subscribe({
      next: () => {
        const cat = this.allCategories.find(c => c.id === categoryId);
        if (cat) {
          this.myCategories = [...this.myCategories, cat];
        }
        this.cdr.detectChanges();
      }
    });
  }

  unsubscribeCategory(categoryId: number): void {
    this.categoriesService.unsubscribe(categoryId).subscribe({
      next: () => {
        this.myCategories = this.myCategories.filter(c => c.id !== categoryId);
        this.cdr.detectChanges();
      }
    });
  }

  isOrganizer(): boolean {
    return this.currentUser?.role === 'Organizator' || this.currentUser?.role === 'Admin';
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('ro-RO', {
      day: '2-digit',
      month: 'long',
      year: 'numeric'
    });
  }
}