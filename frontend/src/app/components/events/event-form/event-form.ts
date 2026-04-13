import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { EventsService } from '../../../services/events';
import { CategoriesService } from '../../../services/categories';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-event-form',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule
  ],
  templateUrl: './event-form.html',
  styleUrl: './event-form.css'
})
export class EventFormComponent implements OnInit {
  eventForm: FormGroup;
  categories: any[] = [];
  isEditMode = false;
  eventId: number | null = null;
  loading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private eventsService: EventsService,
    private categoriesService: CategoriesService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    this.eventForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      location: ['', Validators.required],
      eventDate: ['', Validators.required],
      categoryId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadCategories();

    // Verifica daca suntem in modul editare
    this.eventId = this.route.snapshot.paramMap.get('id')
      ? +this.route.snapshot.paramMap.get('id')!
      : null;

    if (this.eventId) {
      this.isEditMode = true;
      this.loadEvent(this.eventId);
    }
  }

  loadCategories(): void {
    this.categoriesService.getAll().subscribe({
      next: (data) => {
        this.categories = data;
        this.cdr.detectChanges();
      }
    });
  }

  loadEvent(id: number): void {
    this.eventsService.getById(id).subscribe({
      next: (data) => {
        this.eventForm.patchValue({
          title: data.title,
          description: data.description,
          location: data.location,
          eventDate: new Date(data.eventDate),
          categoryId: data.category.id
        });
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit(): void {
    if (this.eventForm.invalid) return;

    this.loading = true;
    this.errorMessage = '';

    const formValue = {
      ...this.eventForm.value,
      eventDate: new Date(this.eventForm.value.eventDate).toISOString()
    };

    if (this.isEditMode && this.eventId) {
      this.eventsService.update(this.eventId, formValue).subscribe({
        next: () => {
          this.router.navigate(['/events', this.eventId]);
        },
        error: (err) => {
          this.errorMessage = 'Eroare la actualizare.';
          this.loading = false;
          this.cdr.detectChanges();
        }
      });
    } else {
      this.eventsService.create(formValue).subscribe({
        next: (data) => {
          this.router.navigate(['/events', data.id]);
        },
        error: (err) => {
          this.errorMessage = 'Eroare la creare.';
          this.loading = false;
          this.cdr.detectChanges();
        }
      });
    }
  }
}