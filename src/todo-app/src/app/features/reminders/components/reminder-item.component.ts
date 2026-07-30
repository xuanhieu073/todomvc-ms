import { Component, inject, input } from '@angular/core';
import { ActionButtonComponent } from './action-button.component';
import { Reminder } from '../../todos/models/reminder';
import { RemindersStore } from '../reminders.store';

@Component({
  selector: 'app-reminder-item',
  template: ` <li class="item border border-gray-200 px-6 py-4 shadow">
    <p>
      <ng-content></ng-content>
    </p>
    <div class="flex gap-2 mt-2">
      <app-action-button (click)="Snooze(1)">💤1m</app-action-button>
      <app-action-button (click)="Snooze(60)">💤1h</app-action-button>
      <app-action-button class="ml-auto" (click)="Dimiss()">❌</app-action-button>
    </div>
  </li>`,
  imports: [ActionButtonComponent],
  styles: `
    .item {
      transition-property: opacity, transform;
      transition-duration: 500ms;

      @starting-style {
        opacity: 0;
        transform: translateX(100%);
      }
    }
  `,
})
export class ReminderItemComponent {
  private readonly remindersStore = inject(RemindersStore);

  reminder = input.required<Reminder>();

  Snooze(minutes: number) {
    const Id = this.reminder().id;
    this.remindersStore.snoozeReminderEffect({ Id, minutes });
  }

  Dimiss() {
    this.remindersStore.dimissReminderEffect(this.reminder().id);
  }
}
