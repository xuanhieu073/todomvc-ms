import { Routes } from '@angular/router';
import { AuthGuard } from './core/auth.guard';

export const routes: Routes = [
  {
    path: 'todos',
    loadComponent: () =>
      import('./shared/layout/todos-layout/todos-layout.component').then(
        (c) => c.TodoLayoutComponent,
      ),
    canMatch: [AuthGuard],
  },
  {
    path: 'statistics',
    loadComponent: () =>
      import('./features/statistics/statistics.component').then((c) => c.StatisticsComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register.component').then((c) => c.RegisterComponent),
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login.component').then((c) => c.LoginComponent),
  },
];
