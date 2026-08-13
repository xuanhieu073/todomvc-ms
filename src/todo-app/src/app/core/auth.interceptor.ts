import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthStore } from './store/auth.store';
import { catchError, switchMap, take, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authStore = inject(AuthStore);

  return authStore.accessToken$.pipe(
    take(1),
    switchMap((token) => {
      if (token) {
        const clonedReq = req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`,
          },
        });
        return next(clonedReq).pipe(
          catchError((error: HttpErrorResponse) => {
            if (error.status === 401) {
              authStore.logoutEffect();
            }
            return throwError(() => error);
          }),
        );
      }

      return next(req);
    }),
  );
};
