import { ComponentStore } from '@ngrx/component-store';
import { Reminder } from '../todos/models/reminder';
import { inject, Injectable } from '@angular/core';
import { switchMap, tap } from 'rxjs';
import { RemindersService } from './services/reminders.service';
import { tapResponse } from '@ngrx/operators';
import { HttpErrorResponse } from '@angular/common/http';

export interface RemindersState {
  isLoading: boolean;
  error: {
    title: string;
    details: string;
  } | null;
  pendingReminders: Reminder[];
}

@Injectable({ providedIn: 'root' })
export class RemindersStore extends ComponentStore<RemindersState> {
  reminderSerivce = inject(RemindersService);

  readonly pendingReminders$ = this.select((s) => s.pendingReminders);

  constructor() {
    super({
      isLoading: false,
      error: null,
      pendingReminders: [],
    });

    this.initializeEffect();
    this.localSaveReminderEffect(this.pendingReminders$);
  }

  readonly addPendingReminder = this.updater((state, reminder: Reminder[]) => {
    console.log('Adding pending reminder:', reminder);
    return {
      ...state,
      pendingReminders: [...state.pendingReminders, ...reminder],
    };
  });

  readonly removePendingReminder = this.updater((state, reminderIds: string[]) => {
    return {
      ...state,
      pendingReminders: state.pendingReminders.filter((r) => !reminderIds.includes(r.id)),
    };
  });

  initializeEffect = this.effect((trigger$) =>
    trigger$.pipe(
      tap(() => {
        const pendingReminders = localStorage.getItem('pendingReminders')
          ? (JSON.parse(localStorage.getItem('pendingReminders')!) as Reminder[])
          : null;
        if (pendingReminders?.length) this.patchState({ pendingReminders });
      }),
    ),
  );

  localSaveReminderEffect = this.effect<Reminder[]>((reminders) =>
    reminders.pipe(
      tap((reminders) => {
        console.log('saving pending reminders', reminders);
        localStorage.setItem('pendingReminders', JSON.stringify(reminders));
      }),
    ),
  );

  snoozeReminderEffect = this.effect<{ Id: string; minutes: number }>((sonozeInfo$) =>
    sonozeInfo$.pipe(
      tap((snoozeInfo) => {
        console.log(snoozeInfo);
        this.patchState({ isLoading: true });
      }),
      switchMap(({ Id, minutes }) =>
        this.reminderSerivce.snoozeReminder(Id, minutes).pipe(
          tapResponse({
            next: (reminder) =>
              this.setState((state) => ({
                ...state,
                pendingReminders: state.pendingReminders.filter((r) => r.id !== reminder.id),
              })),
            error: (error: HttpErrorResponse) =>
              this.patchState({ error: { title: 'Fail to fetch', details: error.message } }),
            finalize: () => this.patchState({ isLoading: false }),
          }),
        ),
      ),
    ),
  );

  dimissReminderEffect = this.effect<string>((Id$) =>
    Id$.pipe(
      tap(() => this.patchState({ isLoading: true })),
      switchMap((Id) =>
        this.reminderSerivce.dimissReminder(Id).pipe(
          tapResponse({
            next: (reminder) =>
              this.setState((state) => ({
                ...state,
                pendingReminders: state.pendingReminders.filter((r) => r.id != reminder.id),
              })),
            error: (error: HttpErrorResponse) =>
              this.patchState({ error: { title: 'Fail to fetch', details: error.message } }),
            finalize: () => this.patchState({ isLoading: false }),
          }),
        ),
      ),
    ),
  );
}
