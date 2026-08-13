import { inject, Injectable } from '@angular/core';
import { User } from '../models/user';
import { ComponentStore } from '@ngrx/component-store';
import { filter, switchMap, tap } from 'rxjs';
import { Login } from '../models/login';
import { AuthService } from '../services/auth.service';
import { tapResponse } from '@ngrx/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { Register } from '../models/register';

export interface AuthState {
  user: User | null;
  isLoading: boolean;
  error: {
    title: string;
    details: string;
  } | null;
}

@Injectable({ providedIn: 'root' })
export class AuthStore extends ComponentStore<AuthState> {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  user$ = this.select((s) => s.user);
  accessToken$ = this.select((s) => s.user?.accessToken);
  isLoggedIn$ = this.select((s) => !!s.user);
  vm$ = this.select((s) => ({ user: s.user, isLoggedIn: !!s.user }));

  constructor() {
    super({
      user: null,
      isLoading: false,
      error: null,
    });

    this.initializeEffect();
    this.saveUserEffect(this.user$);
  }

  initializeEffect = this.effect((trigger$) =>
    trigger$.pipe(
      tap(() => {
        const user = localStorage.getItem('user')
          ? JSON.parse(localStorage.getItem('user') || '')
          : null;
        if (user) this.patchState({ user });
      }),
    ),
  );

  saveUserEffect = this.effect<User | null>((user$) =>
    user$.pipe(
      tap((user) => {
        if (user) localStorage.setItem('user', JSON.stringify(user));
        else localStorage.removeItem('user');
      }),
    ),
  );

  registerEffect = this.effect<Register>((register$) =>
    register$.pipe(
      tap(() => this.patchState({ isLoading: true })),
      switchMap((register) =>
        this.authService.register(register).pipe(
          tapResponse({
            next: (user) => {
              this.patchState({ isLoading: false, user });
              this.router.navigate(['/todos']);
            },
            error: (error: HttpErrorResponse) =>
              this.patchState({ error: { title: 'Fail to fetch', details: error.message } }),
            finalize: () => this.patchState({ isLoading: false }),
          }),
        ),
      ),
    ),
  );

  loginEffect = this.effect<Login>((info$) =>
    info$.pipe(
      tap(() => this.patchState({ isLoading: true })),
      switchMap((info) =>
        this.authService.login(info).pipe(
          tapResponse({
            next: (user) => {
              this.patchState({ isLoading: false, user });
              this.router.navigate(['/todos']);
            },
            error: (error: HttpErrorResponse) =>
              this.patchState({ error: { title: 'Fail to fetch', details: error.message } }),
            finalize: () => this.patchState({ isLoading: false }),
          }),
        ),
      ),
    ),
  );

  logoutEffect = this.effect((logout$) =>
    logout$.pipe(
      tap(() => {
        this.patchState({ user: null });
        this.router.navigate(['/login']);
      }),
    ),
  );
}
