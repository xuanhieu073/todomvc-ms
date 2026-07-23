import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'todos',
    loadComponent: () => import('./features/todos/todos.component').then((c) => c.TodosComponent),
  },
  {
    path: 'reminders',
    loadComponent: () =>
      import('./features/reminders/reminders.component').then((c) => c.RemindersComponent),
  },
  {
    path: 'statistics',
    loadComponent: () =>
      import('./features/statistics/statistics.component').then((c) => c.StatisticsComponent),
  },
];
