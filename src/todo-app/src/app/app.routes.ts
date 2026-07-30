import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'todos',
    loadComponent: () =>
      import('./shared/todos-layout/todos-layout.component').then((c) => c.TodoLayoutComponent),
  },
  {
    path: 'statistics',
    loadComponent: () =>
      import('./features/statistics/statistics.component').then((c) => c.StatisticsComponent),
  },
];
