import { AsyncPipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { map, take } from 'rxjs';
import { FilterButtonComponent } from './components/filter-button/filter-button.component';
import { TodoItemComponent } from './components/todo-item/todo-item.component';
import { TodosService } from './services/todos.service';
import { TodosStore } from './todos.store';
import { TodoFooterComponent } from './components/todo-footer/todo-footer.component';
import { ErrorMessageComponent } from './components/error-message/error-message.component';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';

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

    @if (vm$ | async; as vm) {
      <div class="flex flex-col shadow-2xl relative">
        <div class="flex min-w-175 border border-gray-200 shadow-2xl">
          @if (
            vm.todos.filter(
              (todo) =>
                vm?.filter === 'all' ||
                (todo.isCompleted === false && vm?.filter === 'active') ||
                (todo.isCompleted === true && vm?.filter === 'completed')
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
        </div>
        @if (newTodoTitle.touched && newTodoTitle.hasError('maxlength')) {
          <div class="bg-white px-4 border border-gray-200">
            Title length shoud not greater than 200
          </div>
        }
        <ul>
          @for (todo of vm.todos; track todo.id) {
            @if (
              vm.filter === 'all' ||
              (todo.isCompleted === false && vm.filter === 'active') ||
              (todo.isCompleted === true && vm.filter === 'completed')
            ) {
              <app-todo-item [todo]="todo" />
            }
          }
        </ul>
        @if (vm.todos.length > 0 || vm.filter !== 'all') {
          <app-todo-footer />
        }
      </div>

      @if (vm.isLoading) {
        <div>Loading...</div>
      }

      @if (vm.error) {
        <app-error-message [title]="vm.error.title" (onDataSent)="CloseNofity($event)" />
      }
    }
  </div>`,
  styles: ``,
})
export class TodosComponent implements OnInit {
  private readonly todosStore = inject(TodosStore);
  private readonly route = inject(ActivatedRoute);

  todos$ = this.todosStore.todos$;
  vm$ = this.todosStore.vm$;
  newTodoTitle = new FormControl('', {
    validators: [Validators.required, Validators.min(2), Validators.maxLength(200)],
  });

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
      const el = event.target as HTMLInputElement;
      this.todosStore.createEffect({ title: el.value });
      this.newTodoTitle.reset();
    }
  }

  ToggleCompletedAll() {
    this.todos$.pipe(take(1)).subscribe((todos) => {
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
