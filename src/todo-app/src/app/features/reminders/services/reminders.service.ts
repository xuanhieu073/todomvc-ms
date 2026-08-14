import { inject, Service } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Reminder } from '../../todos/models/reminder';

@Service()
export class RemindersService {
  private readonly apiUrl = environment.apiUrl;
  private readonly http = inject(HttpClient);

  getReminders(state: string) {
    return this.http.get<Reminder[]>(`${this.apiUrl}/bff/reminders`, { params: { state } });
  }

  snoozeReminder(Id: string, minutes: number) {
    return this.http.patch<Reminder>(`${this.apiUrl}/bff/reminders/${Id}/snooze`, { minutes });
  }

  dimissReminder(Id: string) {
    return this.http.patch<Reminder>(`${this.apiUrl}/bff/reminders/${Id}/dimiss`, null);
  }
}
