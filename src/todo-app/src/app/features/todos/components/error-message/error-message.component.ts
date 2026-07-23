import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-error-message',
  imports: [],
  template: ` <div
    class="error-message border border-gray-200 bg-white z-10 shadow px-4 py-2 flex gap-4 "
  >
    <p>{{ title() }}</p>
    <button class="close-button opacity-0" (click)="triggerAction()">❌</button>
  </div>`,
  styles: `
    .error-message:hover .close-button {
      @apply opacity-100;
    }
  `,
})
export class ErrorMessageComponent {
  title = input.required<string>();
  onDataSent = output<string>();

  triggerAction() {
    // Emit the event to the parent
    this.onDataSent.emit('Hello from the child component!');
  }
}
