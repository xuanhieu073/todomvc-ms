import { Component, inject } from '@angular/core';
import { AuthStore } from '../../../core/store/auth.store';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-user-menu',
  template: `
    @if (isLoggedIn()) {
      <button class="cursor-pointer" (click)="Logout()">logout ➜]</button>
    }
  `,
})
export class UserMenuComponent {
  private readonly authStore = inject(AuthStore);
  isLoggedIn = toSignal(this.authStore.isLoggedIn$);

  Logout() {
    this.authStore.logoutEffect();
  }
}
