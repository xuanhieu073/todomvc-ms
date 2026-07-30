import { ComponentStore } from '@ngrx/component-store';
import { Reminder } from '../todos/models/reminder';
import { Injectable } from '@angular/core';
import { tap } from 'rxjs';

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
        if (reminders.length) localStorage.setItem('pendingReminders', JSON.stringify(reminders));
      }),
    ),
  );

  snoozeReminderEffect = this.effect<{ Id: string; snoozeTime: string }>((sonozeInfo$) =>
    sonozeInfo$.pipe(
      tap((snoozeInfo) => {
        console.log(snoozeInfo);
      }),
    ),
  );
}
