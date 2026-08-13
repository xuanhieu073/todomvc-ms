import { Component, input, model, output } from '@angular/core';
import { FormValueControl } from '@angular/forms/signals';

@Component({
  selector: 'app-input',
  template: `
    <div class="app-input">
      <input
        class="py-4 px-14 text-2xl italic outline-green-600 border border-gray-400 w-full rounded"
        [placeholder]="placeholder()"
        type="text"
        [value]="value()"
        (input)="value.set($event.target.value)"
        (blur)="touch.emit()"
        placeholder="What needs to be done?"
        [type]="type()"
      />
    </div>
  `,
})
export class TodoInput implements FormValueControl<string> {
  placeholder = input('');
  type = input('text');
  value = model('');

  touched = input<boolean>(false);
  touch = output<void>();
}
