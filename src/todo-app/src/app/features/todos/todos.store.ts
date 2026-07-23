import { inject, Injectable } from '@angular/core';
import { Todo } from './models/todo';
import { ComponentStore } from '@ngrx/component-store';
import { switchMap, tap } from 'rxjs';
import { TodosService } from './services/todos.service';
import { CreateTodoRequest } from './models/createTodoRequest';
import { tapResponse } from '@ngrx/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { UpdateTodoRequest } from './models/updateTodoRequest';

export interface TodosState {
  isLoading: boolean;
  error: string;
  filter: 'all' | 'active' | 'completed';
  todos: Todo[];
}

@Injectable()
export class TodosStore extends ComponentStore<TodosState> {
  todosService = inject(TodosService);

  readonly todos$ = this.select((s) => s.todos);
  readonly filter$ = this.select((s) => s.filter);
  readonly isLoading$ = this.select((s) => s.isLoading);

  constructor() {
    super({
      isLoading: false,
      error: '',
      filter: 'all',
      todos: [],
    });
    this.fetchEffect(this.filter$);
  }

  readonly addMovie = this.updater((state, todo: Todo) => ({
    ...state,
    todos: [...state.todos, todo],
  }));

  setFilter = this.updater((state, filter: 'all' | 'active' | 'completed') => ({
    ...state,
    filter,
  }));

  fetchEffect = this.effect<string>((filter$) =>
    filter$.pipe(
      tap(() => {
        this.patchState({ isLoading: true });
      }),
      switchMap((filter) => {
        return this.todosService.getTodos(filter).pipe(
          tapResponse({
            next: (todos) => this.patchState({ isLoading: false, todos }),
            error: (error: HttpErrorResponse) => this.patchState({ error: error.message }),
            finalize: () => this.patchState({ isLoading: false }),
          }),
        );
      }),
    ),
  );

  createEffect = this.effect<CreateTodoRequest>((createTodoRequest$) =>
    createTodoRequest$.pipe(
      tap(() => {
        this.patchState({ isLoading: true });
      }),
      switchMap((createTodoRequest) =>
        this.todosService.createTodo(createTodoRequest).pipe(
          tapResponse({
            next: (todo) =>
              this.setState((state) => ({
                ...state,
                isLoading: false,
                todos: [...state.todos, todo],
              })),
            error: (error: HttpErrorResponse) => this.patchState({ error: error.message }),
            finalize: () => this.patchState({ isLoading: false }),
          }),
        ),
      ),
    ),
  );

  updateEffect = this.effect<{ id: string; updateTodoRequest: UpdateTodoRequest }>((updateInfo$) =>
    updateInfo$.pipe(
      tap(() => this.patchState({ isLoading: true })),
      switchMap(({ id, updateTodoRequest }) =>
        this.todosService.updateTodo(id, updateTodoRequest).pipe(
          tapResponse({
            next: (todo) =>
              this.setState((state) => ({
                ...state,
                todos: state.todos.map((td) => (td.id === todo.id ? todo : td)),
              })),
            error: (error: HttpErrorResponse) => this.patchState({ error: error.message }),
            finalize: () => this.patchState({ isLoading: false }),
          }),
        ),
      ),
    ),
  );

  toggleCompletedEffect = this.effect<string>((id$) =>
    id$.pipe(
      tap(() => this.patchState({ isLoading: true })),
      switchMap((id) =>
        this.todosService.toggleCompleted(id).pipe(
          tapResponse({
            next: (todo) =>
              this.setState((state) => ({
                ...state,
                todos: state.todos.map((td) => (td.id === todo.id ? todo : td)),
              })),
            error: (error: HttpErrorResponse) => this.patchState({ error: error.message }),
            finalize: () => this.patchState({ isLoading: false }),
          }),
        ),
      ),
    ),
  );

  deleteEffect = this.effect<string>((id$) =>
    id$.pipe(
      tap(() => this.patchState({ isLoading: true })),
      switchMap((id) =>
        this.todosService.deleteTodo(id).pipe(
          tapResponse({
            next: (_) =>
              this.setState((state) => ({
                ...state,
                todos: state.todos.filter((todo) => todo.id !== id),
              })),
            error: (error: HttpErrorResponse) => this.patchState({ error: error.message }),
            finalize: () => this.patchState({ isLoading: false }),
          }),
        ),
      ),
    ),
  );
}
