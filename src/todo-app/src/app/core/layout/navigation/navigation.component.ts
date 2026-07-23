import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-navigation',
  imports: [RouterLink, RouterLinkActive],
  template: `
    <ul class="flex flex-col gap-2 text-gray-700">
      <nav routerLink="/todos" class="cursor-pointer" routerLinkActive="text-green-700 font-medium">
        Todos
      </nav>
      <nav
        routerLink="/reminders"
        class="cursor-pointer "
        routerLinkActive="text-green-700 font-medium"
      >
        Reminders
      </nav>
      <nav
        routerLink="/statistics"
        class="cursor-pointer "
        routerLinkActive="text-green-700 font-medium"
      >
        Statistics
      </nav>
    </ul>
  `,
  styles: ``,
})
export class NavigationComponent {}
