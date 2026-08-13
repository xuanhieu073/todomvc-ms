import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavigationComponent } from './shared/layout/navigation/navigation.component';
import { UserMenuComponent } from './shared/layout/user-menu/user-menu.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavigationComponent, UserMenuComponent],
  template: `
    <div class="flex gap-4 min-h-screen">
      <div class="w-40 py-10 px-4 bg-gray-100 flex flex-col justify-between">
        <app-navigation />
        <app-user-menu />
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
