import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService) as AuthService;
  const router = inject(Router);

  if (authService.getRole() === 'Admin') {
    return true;
  }

  router.navigate(['/events']);
  return false;
};