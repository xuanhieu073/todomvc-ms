import { Component, inject, OnDestroy, signal } from '@angular/core';
import { ReminderItemComponent } from './components/reminder-item.component';
import { RemindersStore } from './reminders.store';
import { AsyncPipe } from '@angular/common';
import { Reminder } from '../todos/models/reminder';
import { AuthStore } from '../../core/store/auth.store';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { take } from 'rxjs';

@Component({
  selector: 'app-reminders',
  imports: [ReminderItemComponent, AsyncPipe],
  template: `
    <div class="px-6 flex flex-col gap-4 overflow-y-auto overflow-x-hidden">
      @if (pendingReminders$ | async; as pendingReminders) {
        <div class="flex justify-end w-full text-2xl">
          <span>{{ pendingReminders.length }}</span>
          <span>🔔</span>
        </div>
        <ul class="items">
          @for (reminder of pendingReminders; track reminder.id) {
            <app-reminder-item [reminder]="reminder" class="item-container" animate.leave="fade"
              >Overdue task: {{ reminder.title }}
            </app-reminder-item>
          }
        </ul>
      }
    </div>
  `,
  styles: `
    .items .item-container {
      transition-property: opacity, transform;
      transition-duration: 500ms;
      @starting-style {
        opacity: 0;
        transform: translateX(-10px);
      }
    }

    .items .item-container.fade {
      animation: fade-out 500ms;
    }

    @keyframes fade-out {
      from {
        opacity: 1;
      }

      to {
        opacity: 0;
      }
    }
  `,
})
export class RemindersComponent implements OnDestroy {
  private readonly authStore = inject(AuthStore);
  private readonly remindersStore = inject(RemindersStore);
  pendingReminders$ = this.remindersStore.pendingReminders$;
  items = ['stuff', 'things', 'cheese', 'paper'];

  eventSource: EventSource | null = null;
  handleAddReminder = (event: MessageEvent) => {
    const addReminders = JSON.parse(event.data) as Reminder[];
    console.log('Reminder Update received:', addReminders);
    if (addReminders.length > 0) this.remindersStore.addPendingReminder(addReminders);
  };

  handleRemoveReminder = (event: MessageEvent) => {
    console.log('Reminder Dimissed received:', event.data);
    const removeReminders = JSON.parse(event.data) as Reminder[];
    if (removeReminders.length > 0)
      this.remindersStore.removePendingReminder(removeReminders.map((r) => r.id));
  };

  constructor() {
    this.authStore.accessToken$.pipe(take(1)).subscribe((token) => {
      console.log(token);
      this.eventSource = new EventSource(
        `https://localhost:7160/bff/reminders/stream?token=${encodeURIComponent(token || '')}`,
      );

      this.eventSource.onmessage = (event) => {
        console.log('Generic message received:', event.data);
      };

      this.eventSource.addEventListener('reminder-fired', this.handleAddReminder);
      this.eventSource.addEventListener('reminder-removed', this.handleRemoveReminder);

      this.eventSource.onerror = (error) => {
        console.error('EventSource failed:', error);
      };
    });
  }

  ngOnDestroy(): void {
    this.eventSource?.close();
    this.eventSource?.removeEventListener('receive', this.handleAddReminder);
    this.eventSource?.removeEventListener('remove', this.handleRemoveReminder);
  }
}
