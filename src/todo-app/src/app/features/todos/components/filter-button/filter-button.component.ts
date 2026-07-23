import { Component, inject, input } from '@angular/core';
import { TodosStore } from '../../todos.store';

@Component({
  selector: 'app-filter-button',
  imports: [],
  template: ` <button class="rounded-sm px-2 text-sm border-green-700" [class.active]="isActive()">
    {{ text() }}
  </button>`,
  styles: `
    .active {
      @apply border;
    }
  `,
})
export class FilterButtonComponent {
  isActive = input<boolean>();
  text = input.required<string>();
}
