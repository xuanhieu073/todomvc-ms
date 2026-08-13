import { Component, inject, signal } from '@angular/core';
import { email, form, FormField, maxLength, minLength, required } from '@angular/forms/signals';
import { TodoInput } from '../../core/component/input.component';
import { RouterLink } from '@angular/router';
import { AuthStore } from '../../core/store/auth.store';

@Component({
  template: `<div class="flex justify-center items-center h-screen">
    <form novalidate class="max-w-100 flex flex-col gap-4 px-6 py-4 shadow-2xl">
      <div>
        <label for="email">Email</label>
        <app-input [formField]="registerForm.email"></app-input>
      </div>
      @if (registerForm.email().touched() && registerForm.email().invalid()) {
        <ul class="error-list">
          @for (error of registerForm.email().errors(); track error.message) {
            <li class="text-red-400">{{ error.message }}</li>
          }
        </ul>
      }
      <div>
        <label for="password">Password</label>
        <app-input type="password" [formField]="registerForm.password"></app-input>
      </div>
      @if (registerForm.password().touched() && registerForm.password().invalid()) {
        <ul class="error-list">
          @for (error of registerForm.password().errors(); track error.message) {
            <li class="text-red-400">{{ error.message }}</li>
          }
        </ul>
      }
      <div>
        <label for="confirmPassword">Confirm Password</label>
        <app-input type="password" [formField]="registerForm.confirmPassword"></app-input>
      </div>
      @if (registerForm.confirmPassword().touched() && registerForm.confirmPassword().invalid()) {
        <ul class="error-list">
          @for (error of registerForm.confirmPassword().errors(); track error.message) {
            <li class="text-red-400">{{ error.message }}</li>
          }
        </ul>
      }
      <button
        class="cursor-pointer rounded bg-green-700 py-4 text-white"
        (click)="Register($event)"
      >
        Register
      </button>
      <a class="text-blue-500 cursor-pointer self-center" routerLink="/login">Go to login?</a>
    </form>
  </div>`,
  imports: [TodoInput, FormField, RouterLink],
})
export class RegisterComponent {
  private readonly authStore = inject(AuthStore);

  registerModel = signal({
    email: '',
    password: '',
    confirmPassword: '',
  });

  registerForm = form(this.registerModel, (schemaPath) => {
    required(schemaPath.email, { message: 'Email is required' });
    email(schemaPath.email, { message: 'Invalid email format' });
    required(schemaPath.password, { message: 'Password is required' });
    minLength(schemaPath.password, 6, { message: 'Minimum length of password is 6' });
    maxLength(schemaPath.password, 64, { message: 'Maximum length of password is 64' });
    required(schemaPath.confirmPassword, { message: 'confirmPassword is required' });
    minLength(schemaPath.confirmPassword, 6, { message: 'Minimum length of confirmPassword is 6' });
    maxLength(schemaPath.confirmPassword, 64, {
      message: 'Maximum length of confirmPassword is 64',
    });
  });

  Register(event: Event) {
    event.preventDefault();
    this.authStore.registerEffect(this.registerForm().value());
  }
}
