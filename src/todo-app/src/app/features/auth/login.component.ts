import { Component, inject, signal } from '@angular/core';
import { TodoInput } from '../../core/component/input.component';
import { email, form, FormField, maxLength, minLength, required } from '@angular/forms/signals';
import { AuthService } from '../../core/services/auth.service';
import { AuthStore } from '../../core/store/auth.store';
import { RouterLink } from '@angular/router';

@Component({
  template: `
    <div class="flex justify-center items-center h-screen">
      <form novalidate class="max-w-100 flex flex-col gap-4 px-6 py-4 shadow-2xl">
        <div>
          <label for="email">Email</label>
          <app-input [formField]="loginUserForm.email"></app-input>
        </div>
        @if (loginUserForm.email().touched() && loginUserForm.email().invalid()) {
          <ul class="error-list">
            @for (error of loginUserForm.email().errors(); track error.message) {
              <li class="text-red-400">{{ error.message }}</li>
            }
          </ul>
        }
        <div>
          <label for="password">Password</label>
          <app-input type="password" [formField]="loginUserForm.password"></app-input>
        </div>
        @if (loginUserForm.password().touched() && loginUserForm.password().invalid()) {
          <ul class="error-list">
            @for (error of loginUserForm.password().errors(); track error.message) {
              <li class="text-red-400">{{ error.message }}</li>
            }
          </ul>
        }
        <button class="cursor-pointer rounded bg-green-700 py-4 text-white" (click)="Login($event)">
          Login
        </button>

        <a class="text-blue-500 cursor-pointer self-center" routerLink="/register"
          >Doesn't have account?</a
        >
      </form>
    </div>
  `,
  imports: [TodoInput, FormField, RouterLink],
})
export class LoginComponent {
  authService = inject(AuthService);
  authStore = inject(AuthStore);

  loginUserModel = signal({
    email: '',
    password: '',
  });

  loginUserForm = form(this.loginUserModel, (schemaPath) => {
    required(schemaPath.email, { message: 'Email is required' });
    email(schemaPath.email, { message: 'Invalid email format' });
    required(schemaPath.password, { message: 'Password is required' });
    minLength(schemaPath.password, 6, { message: 'Minimum length of password is 6' });
    maxLength(schemaPath.password, 64, { message: 'Maximum length of password is 64' });
  });

  Login(event: Event) {
    event.preventDefault();
    this.authStore.loginEffect(this.loginUserForm().value());
  }
}
