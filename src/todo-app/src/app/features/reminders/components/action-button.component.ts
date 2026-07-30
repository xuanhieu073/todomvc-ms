import { Component, output } from '@angular/core';

@Component({
  selector: 'app-action-button',
  template: `
    <button class="bg-green-300 text-white px-4 py-2 rounded-full hover:bg-green-700">
      <ng-content></ng-content>
    </button>
  `,
})
export class ActionButtonComponent {}
