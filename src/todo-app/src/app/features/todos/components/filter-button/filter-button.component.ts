import { Component, inject, input } from '@angular/core';
import { TodosStore } from '../../todos.store';

@Component({
  selector: 'app-filter-button',
  imports: [],
  template: ` <button
    class="border rounded-sm px-2 text-sm cursor-pointer border-white hover:border-green-500"
    [class.active]="isActive()"
  >
    {{ text() }}
  </button>`,
  styles: `
    .active {
      border-color: var(--color-green-700);
    }
  `,
})
export class FilterButtonComponent {
  isActive = input<boolean>();
  text = input.required<string>();
}
