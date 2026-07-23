import { Component, inject } from '@angular/core';
import { TodoItemComponent } from './components/todo-item/todo-item.component';
import { TodosStore } from './todos.store';
import { TodosService } from './services/todos.service';
import { AsyncPipe } from '@angular/common';
import { FilterButtonComponent } from './components/filter-button/filter-button.component';

@Component({
  selector: 'app-todos',
  imports: [TodoItemComponent, AsyncPipe, FilterButtonComponent],
  providers: [TodosStore, TodosService],
  template: ` <div class="flex flex-col items-center mt-6 gap-8">
    <h1 class="text-7xl text-green-700 font-medium">Todos</h1>

    <div class="flex flex-col shadow-2xl">
      <input
        class="px-14 py-4 shadow-2xl min-w-175 text-2xl italic border border-gray-200 outline-green-600"
        type="text"
        placeholder="What need to be done?"
        (keydown.enter)="CreateTodo($event)"
      />
      @if (todos$ | async; as todos) {
        <ul>
          @for (todo of todos; track todo.id) {
            <app-todo-item [todo]="todo" />
          }
        </ul>
      }
      <div class="border border-gray-200 px-4 py-2 relative bg-white">
        <p>1 item left</p>
        <div class="flex gap-2 absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2">
          @if (filter$ | async; as filter) {
            <app-filter-button
              [isActive]="filter === 'all'"
              (click)="SetFilter('all')"
              text="All"
            />
            <app-filter-button
              [isActive]="filter === 'active'"
              (click)="SetFilter('active')"
              text="Active"
            />
            <app-filter-button
              [isActive]="filter === 'completed'"
              (click)="SetFilter('completed')"
              text="Completed"
            />
          }
        </div>
      </div>
    </div>
  </div>`,
  styles: ``,
})
export class TodosComponent {
  private readonly todosStore = inject(TodosStore);
  todos$ = this.todosStore.todos$;
  filter$ = this.todosStore.filter$;

  SetFilter(filter: 'all' | 'active' | 'completed') {
    this.todosStore.setFilter(filter);
  }

  CreateTodo(event: Event) {
    const el = event.target as HTMLInputElement;
    this.todosStore.createEffect({ title: el.value });
    el.value = '';
  }
}
