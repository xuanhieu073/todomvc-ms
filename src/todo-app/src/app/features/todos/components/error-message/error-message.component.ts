import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-error-message',
  imports: [],
  template: ` <div class="error-message bg-red-300 z-30 shadow px-4 py-2 flex gap-4 ">
    <p>{{ title() }}</p>
    <button class="close-button opacity-0" (click)="CloseMessage()">❌</button>
  </div>`,
  styles: `
    .error-message:hover .close-button {
      @apply opacity-100;
    }
  `,
})
export class ErrorMessageComponent {
  title = input.required<string>();
  onClose = output<string>();

  CloseMessage() {
    this.onClose.emit('Hello from the child component!');
  }
}
