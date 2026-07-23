import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavigationComponent } from './core/layout/navigation/navigation.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavigationComponent],
  template: `
    <div class="flex gap-4 min-h-screen">
      <div class="w-40 pt-10 px-4 bg-gray-100">
        <app-navigation />
      </div>
      <div class="flex-1">
        <router-outlet />
      </div>
    </div>
  `,
  styles: [],
})
export class App {
  protected readonly title = signal('todo-app');
}
