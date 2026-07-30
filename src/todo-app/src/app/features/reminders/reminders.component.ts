import { Component, inject, OnDestroy, signal } from '@angular/core';
import { ReminderItemComponent } from './components/reminder-item.component';
import { RemindersStore } from './reminders.store';
import { AsyncPipe } from '@angular/common';
import { Reminder } from '../todos/models/reminder';

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
            <app-reminder-item [reminder]="reminder" class="item-container" animate.leave="fade">{{
              reminder.title
            }}</app-reminder-item>
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
  remindersStore = inject(RemindersStore);
  pendingReminders$ = this.remindersStore.pendingReminders$;
  items = ['stuff', 'things', 'cheese', 'paper'];

  eventSource: EventSource;
  handleMessage: (event: MessageEvent) => void;

  constructor() {
    // Connect to the .NET SSE endpoint
    this.eventSource = new EventSource('https://localhost:7160/bff/reminders/stream');
    this.handleMessage = (event) => {
      const reminder = JSON.parse(event.data) as Reminder[];
      console.log('Reminder Update received:', reminder);
      if (reminder.length > 0) this.remindersStore.addPendingReminder(reminder);
    };

    // Handle default/nameless events ("message" event)
    this.eventSource.onmessage = (event) => {
      console.log('Generic message received:', event.data);
    };

    // Handle custom named events (like 'weatherUpdate' declared in our .NET 10 code)
    this.eventSource.addEventListener('remindersUpdate', this.handleMessage);
    this.eventSource.addEventListener('removeDimiss', (event) => {
      console.log('remove reminder', event.data);
    });

    // Capture network or connectivity issues
    this.eventSource.onerror = (error) => {
      console.error('EventSource failed:', error);
    };
  }

  ngOnDestroy(): void {
    this.eventSource.close();
    this.eventSource.removeEventListener('remindersUpdate', this.handleMessage);
  }
}
