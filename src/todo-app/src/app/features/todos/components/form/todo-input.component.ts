import { Component, input, model, output } from '@angular/core';
import { FormValueControl } from '@angular/forms/signals';

@Component({
  selector: 'app-todo-input',
  template: `
    <div class="todo-input">
      <input
        class="py-4 px-14 text-2xl italic outline-green-600 w-full"
        placeholder="What needs to be done?"
        type="text"
        [value]="value()"
        (input)="value.set($event.target.value)"
        (blur)="touch.emit()"
        placeholder="What needs to be done?"
      />
    </div>
  `,
})
export class TodoInput implements FormValueControl<string> {
  value = model('');

  touched = input<boolean>(false);
  touch = output<void>();
}
