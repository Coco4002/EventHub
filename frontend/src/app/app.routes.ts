import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { RegisterComponent } from './components/register/register';
import { EventListComponent } from './components/events/event-list/event-list';
import { EventDetailComponent } from './components/events/event-detail/event-detail';
import { EventFormComponent } from './components/events/event-form/event-form';
import { InvitationListComponent } from './components/invitations/invitation-list/invitation-list';
import { DashboardComponent } from './components/dashboard/dashboard';
import { authGuard } from './guards/auth.guard';
import { AdminComponent } from './components/admin/admin';
import { ProfileComponent } from './components/profile/profile';  

export const routes: Routes = [
  { path: '', redirectTo: '/events', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'events', component: EventListComponent }, 
  { path: 'events/new', component: EventFormComponent, canActivate: [authGuard] },
  { path: 'events/:id', component: EventDetailComponent }, 
  { path: 'events/:id/edit', component: EventFormComponent, canActivate: [authGuard] },
  { path: 'invitations', component: InvitationListComponent, canActivate: [authGuard] },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [authGuard] },
  { path: 'admin', component: AdminComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: '/events' },
];