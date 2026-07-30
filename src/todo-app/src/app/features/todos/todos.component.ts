import { AsyncPipe, JsonPipe } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { map, take } from 'rxjs';
import { TodoItemComponent } from './components/todo-item/todo-item.component';
import { TodosService } from './services/todos.service';
import { TodosStore } from './todos.store';
import { TodoFooterComponent } from './components/todo-footer/todo-footer.component';
import { ErrorMessageComponent } from './components/error-message/error-message.component';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-todos',
  imports: [
    TodoItemComponent,
    AsyncPipe,
    TodoFooterComponent,
    ErrorMessageComponent,
    ReactiveFormsModule,
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
        <input
          class="py-4 px-14 text-2xl italic outline-green-600 w-full"
          type="text"
          placeholder="What needs to be done?"
          (keydown.enter)="CreateTodo($event)"
          [formControl]="newTodoTitle"
        />

        <input
          class="outline-green-600 px-6 py-4"
          type="datetime-local"
          id="appointment"
          name="appointment"
          [formControl]="newTodoDueAt"
          (keydown.enter)="CreateTodo($event)"
        />
        <button class="px-4 py-2" (click)="CreateTodo($event)">✔️</button>
      </div>
      @if (newTodoTitle.touched && newTodoTitle.hasError('maxlength')) {
        <div class="bg-white px-4 border border-gray-200">
          Title length shoud not greater than 200
        </div>
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

  newTodoTitle = new FormControl('', {
    validators: [Validators.required, Validators.min(2), Validators.maxLength(200)],
  });

  newTodoDueAt = new FormControl<Date>(new Date(), {
    validators: [Validators.required],
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
    if (this.newTodoTitle.valid) {
      if (this.newTodoDueAt.valid && this.newTodoTitle.valid) {
        const title = this.newTodoTitle.value;
        const dueAt = this.newTodoDueAt.value;
        this.todosStore.createEffect({ title: title!, dueAt: dueAt! });
        this.newTodoTitle.reset();
      }
    }
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
