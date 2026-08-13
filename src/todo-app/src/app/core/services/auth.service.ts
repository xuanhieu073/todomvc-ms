import { inject, Service } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Login } from '../models/login';
import { User } from '../models/user';
import { Register } from '../models/register';

@Service()
export class AuthService {
  private readonly apiUrl = environment.apiUrl;
  private readonly http = inject(HttpClient);

  login(loginModel: Login) {
    return this.http.post<User>(`${this.apiUrl}/bff/auth/login`, loginModel);
  }

  register(registerModel: Register) {
    return this.http.post<User>(`${this.apiUrl}/bff/auth/register`, registerModel);
  }
}
