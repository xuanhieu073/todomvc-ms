import { Component, model } from '@angular/core';
import { FormValueControl } from '@angular/forms/signals';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-todo-datepicker',
  imports: [FormsModule],
  template: `
    <div class="todo-datepicker h-full">
      <input
        class="outline-green-600 px-6 py-4 h-full"
        type="datetime-local"
        /* [value]="value()"
        (input)="value.set($event.target.value)" */
        [(ngModel)]="value"
      />
    </div>
  `,
})
export class TodoDatepickerComponent implements FormValueControl<Date | null> {
  value = model<Date | null>(null);
}
