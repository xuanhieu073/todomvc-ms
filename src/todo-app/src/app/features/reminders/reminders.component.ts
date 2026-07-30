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
      <div class="flex justify-end w-full text-2xl">🔔</div>
      <button type="button" class="toggle-btn" (click)="reandomize()">Randomize</button>
      <ul class="items">
        @if (pendingReminders$ | async; as pendingReminders) {
          @for (reminder of pendingReminders; track reminder.id) {
            <app-reminder-item [reminder]="reminder" class="item-container" animate.leave="fade">{{
              reminder.todoId
            }}</app-reminder-item>
          }
        }
      </ul>
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

  reandomize() {
    // const randItems = [...this.items];
    // const newItems = [];
    // for (let i of this.items) {
    //   const max: number = this.items.length - newItems.length;
    //   const randNum = Math.floor(Math.random() * max);
    //   newItems.push(...randItems.splice(randNum, 1));
    // }
    // this.items = newItems;
    // this.items = this.items.filter((item) => item !== 'stuff');
    this.items.splice(0, 1);
    this.items = [...this.items, 'table' + Math.random()];
  }

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
