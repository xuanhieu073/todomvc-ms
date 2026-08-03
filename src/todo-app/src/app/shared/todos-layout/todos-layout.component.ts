import { Component } from '@angular/core';
import { TodosComponent } from '../../features/todos/todos.component';
import { RemindersComponent } from '../../features/reminders/reminders.component';

@Component({
  selector: 'app-todo-layout',
  imports: [TodosComponent, RemindersComponent],
  template: ` <div class="flex flex-col-reverse lg:flex-row mt-6 gap-8">
    <div class="flex-1">
      <app-todos />
    </div>

    <div class="lg:w-1/3">
      <app-reminders />
    </div>
  </div>`,
  styles: ``,
})
export class TodoLayoutComponent {}
