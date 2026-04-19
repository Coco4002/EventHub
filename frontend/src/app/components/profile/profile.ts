import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-profile',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDividerModule,
    MatSnackBarModule
  ],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class ProfileComponent implements OnInit {
  profile: any = null;
  nameForm: FormGroup;
  passwordForm: FormGroup;
  nameSuccess = '';
  nameError = '';
  passwordSuccess = '';
  passwordError = '';
  private apiUrl = 'http://localhost:5090/api/profile';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private http: HttpClient,
    private cdr: ChangeDetectorRef,
    private snackBar: MatSnackBar
  ) {
    this.nameForm = this.fb.group({
      fullName: ['', Validators.required]
    });

    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.http.get(this.apiUrl).subscribe({
      next: (data) => {
        this.profile = data;
        this.nameForm.patchValue({ fullName: this.profile.fullName });
        this.cdr.detectChanges();
      }
    });
  }

  passwordMatchValidator(form: FormGroup) {
    const newPassword = form.get('newPassword')?.value;
    const confirm = form.get('confirmPassword')?.value;
    return newPassword === confirm ? null : { passwordMismatch: true };
  }

  updateName(): void {
    if (this.nameForm.invalid) return;
    this.nameSuccess = '';
    this.nameError = '';

    this.http.put(this.apiUrl + '/name',
      JSON.stringify(this.nameForm.value.fullName),
      { headers: { 'Content-Type': 'application/json' } }
    ).subscribe({
      next: (data: any) => {
        this.snackBar.open('Numele a fost actualizat!', 'OK', { duration: 3000 });
        this.profile.fullName = data.fullName;

        const user = this.authService.getCurrentUser();
        user.fullName = data.fullName;
        localStorage.setItem('user', JSON.stringify(user));

        this.cdr.detectChanges();
      },
      error: () => {
        this.nameError = 'Eroare la actualizare.';
        this.cdr.detectChanges();
      }
    });
  }

  updatePassword(): void {
    if (this.passwordForm.invalid) return;
    this.passwordSuccess = '';
    this.passwordError = '';

    const { currentPassword, newPassword } = this.passwordForm.value;

    this.http.put(this.apiUrl + '/password', { currentPassword, newPassword }).subscribe({
      next: () => {
        this.snackBar.open('Parola a fost actualizata!', 'OK', { duration: 3000 });
        this.passwordForm.reset();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.passwordError = err.error || 'Eroare la actualizare.';
        this.cdr.detectChanges();
      }
    });
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('ro-RO', {
      day: '2-digit', month: 'long', year: 'numeric'
    });
  }
}