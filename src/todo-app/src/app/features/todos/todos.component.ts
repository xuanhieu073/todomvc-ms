import { AsyncPipe, JsonPipe } from '@angular/common';
import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { map, take } from 'rxjs';
import { TodoItemComponent } from './components/todo-item/todo-item.component';
import { TodosService } from './services/todos.service';
import { TodosStore } from './todos.store';
import { TodoFooterComponent } from './components/todo-footer/todo-footer.component';
import { ErrorMessageComponent } from './components/error-message/error-message.component';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { TodoInput } from './components/form/todo-input.component';
import { TodoDatepickerComponent } from './components/form/todo-datepicker.component';
import { form, maxLength, required, FormField } from '@angular/forms/signals';

@Component({
  selector: 'app-todos',
  imports: [
    TodoItemComponent,
    AsyncPipe,
    TodoFooterComponent,
    ErrorMessageComponent,
    ReactiveFormsModule,
    TodoInput,
    TodoDatepickerComponent,
    FormField,
  ],
  providers: [TodosStore, TodosService],
  template: ` <div class="flex flex-col items-center mt-6 gap-8">
    <h1 class="text-7xl text-green-700 font-medium">Todos</h1>

    <div class="flex flex-col shadow-2xl relative">
      <div class="flex w-full border border-gray-200 shadow-2xl">
        @if (
          todos$().filter(
            (todo) =>
              filter$() === 'all' ||
              (todo.isCompleted === false && filter$() === 'active') ||
              (todo.isCompleted === true && filter$() === 'completed')
          ).length > 0
        ) {
          <button
            class="absolute top-4 left-4 text-2xl cursor-pointer"
            (click)="ToggleCompletedAll()"
          >
            🔻
          </button>
        }
        <form novalidate class="flex w-full">
          <app-todo-input [formField]="newTodoForm.title" />
          <app-todo-datepicker [formField]="newTodoForm.dueAt" />
          <button
            class="px-4 py-2 cursor-pointer disabled:opacity-10"
            [disabled]="newTodoForm().invalid()"
            (click)="CreateTodo($event)"
          >
            ✔️
          </button>
        </form>
      </div>
      @if (newTodoForm.title().touched() && newTodoForm.title().invalid()) {
        <ul class="error-list">
          @for (error of newTodoForm.title().errors(); track error) {
            <li class="bg-white px-4 border border-gray-200">{{ error.message }}</li>
          }
        </ul>
      }
      <ul>
        @for (todo of todos$(); track todo.id) {
          @if (
            filter$() === 'all' ||
            (todo.isCompleted === false && filter$() === 'active') ||
            (todo.isCompleted === true && filter$() === 'completed')
          ) {
            <app-todo-item [todo]="todo" />
          }
        }
      </ul>
      @if (todos$().length > 0 || filter$() !== 'all') {
        <app-todo-footer />
      }
    </div>

    @if (isLoading$ | async; as isLoading) {
      <div>Loading...</div>
    }

    @if (error$ | async; as error) {
      <app-error-message [title]="error.title" (onClose)="CloseNofity($event)" />
    }
  </div>`,
  styles: ``,
})
export class TodosComponent implements OnInit {
  private readonly todosStore = inject(TodosStore);
  private readonly route = inject(ActivatedRoute);

  filter$ = toSignal(this.todosStore.filter$);
  todos$ = toSignal(this.todosStore.todos$, { initialValue: [] });
  isLoading$ = this.todosStore.isLoading$;
  error$ = this.todosStore.error$;

  newTodoModel = signal({
    title: '',
    dueAt: null as Date | null,
  });

  newTodoForm = form(this.newTodoModel, (schemaPath) => {
    required(schemaPath.title, { message: 'Title is required' });
    maxLength(schemaPath.title, 200, { message: 'Title must be less than 200 characters' });
    required(schemaPath.dueAt, { message: 'Due date is required' });
  });

  constructor() {}

  ngOnInit(): void {
    this.todosStore.fetchEffect(
      this.route.fragment.pipe(
        map((filter) =>
          ['all', 'active', 'completed'].indexOf(filter || '') >= 0
            ? (filter as 'all' | 'active' | 'completed')
            : 'all',
        ),
      ),
    );
  }

  CreateTodo(event: Event) {
    event.preventDefault();
    this.todosStore.createEffect(this.newTodoForm().value() as { title: string; dueAt: Date });
  }

  ToggleCompletedAll() {
    this.todosStore.todos$.pipe(take(1)).subscribe((todos) => {
      if (todos.filter((todo) => !todo.isCompleted).length > 0) {
        this.todosStore.completedAllEffect(true);
      } else {
        this.todosStore.completedAllEffect(false);
      }
    });
  }

  CloseNofity(event: string) {
    this.todosStore.removeError();
  }
}
