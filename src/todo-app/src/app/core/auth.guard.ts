import { Injectable } from '@angular/core';
import { CanMatch, Route, Router, UrlSegment } from '@angular/router';
import { AuthStore } from './store/auth.store';
import { map, Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanMatch {
  constructor(
    private auth: AuthStore,
    private router: Router,
  ) {}
  canMatch(route: Route, segments: UrlSegment[]): Observable<boolean> {
    return this.auth.vm$.pipe(
      map((s) => s.isLoggedIn),
      tap((isLoggedIn) => {
        if (!isLoggedIn) this.router.navigate(['/login']);
      }),
    );
  }
}
