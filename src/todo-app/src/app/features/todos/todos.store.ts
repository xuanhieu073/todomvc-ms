import { inject, Injectable } from '@angular/core';
import { Todo } from './models/todo';
import { ComponentStore } from '@ngrx/component-store';
import {
  forkJoin,
  last,
  merge,
  Observable,
  of,
  switchMap,
  takeLast,
  tap,
  withLatestFrom,
} from 'rxjs';
import { TodosService } from './services/todos.service';
import { CreateTodoRequest } from './models/create-todo-request';
import { tapResponse } from '@ngrx/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { UpdateTodoRequest } from './models/update-todo-request';

export interface TodosState {
  isLoading: boolean;
  error: {
    title: string;
    details: string;
  } | null;
  filter: 'all' | 'active' | 'completed';
  todos: Todo[];
}

@Injectable()
export class TodosStore extends ComponentStore<TodosState> {
  todosService = inject(TodosService);

  readonly vm$ = this.select((s) => ({
    ...s,
    completedCount: s.todos.filter((todo) => todo.isCompleted).length,
    inCompletedCount: s.todos.filter((todo) => !todo.isCompleted).length,
  }));
  readonly todos$ = this.select((s) => s.todos);

  constructor() {
    super({
      isLoading: false,
      error: null,
      filter: 'all',
      todos: [],
    });
  }

  setFilter = this.updater((state, filter: Filter) => ({
    ...state,
    filter,
  }));

  removeError = this.updater((state) => ({ ...state, error: null }));

  fetchEffect = this.effect<Filter>((filter$) =>
    filter$.pipe(
      tap((filter) => {
        this.patchState({ isLoading: true, filter: filter });
      }),
      switchMap((filter) => {
        return this.todosService.getTodos(filter || '').pipe(
          tapResponse({
            next: (todos) => this.patchState({ isLoading: false, todos }),
            error: (error: HttpErrorResponse) =>
              this.patchState({ error: { title: 'fail to fetch', details: error.message } }),
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
            error: (error: HttpErrorResponse) =>
              this.patchState({
                error: {
                  title: `Save Fail!`,
                  details: error.message,
                },
              }),
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
            error: (error: HttpErrorResponse) =>
              this.patchState({
                error: {
                  title: `Save Fail!`,
                  details: error.message,
                },
              }),
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
            error: (error: HttpErrorResponse) =>
              this.patchState({ error: { title: 'Save Fail!', details: error.message } }),
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
            error: (error: HttpErrorResponse) =>
              this.patchState({ error: { title: 'Delete Fail!', details: error.message } }),
            finalize: () => this.patchState({ isLoading: false }),
          }),
        ),
      ),
    ),
  );

  clearCompletedEffect = this.effect((clear$) =>
    clear$.pipe(
      tap(() => this.patchState({ isLoading: true })),
      switchMap(() =>
        this.todosService.clearCompleted().pipe(
          tapResponse({
            next: () =>
              this.setState((state) => ({
                ...state,
                todos: state.todos.filter((todo) => !todo.isCompleted),
              })),
            error: (error: HttpErrorResponse) =>
              this.patchState({ error: { title: 'Delete Fail!', details: error.message } }),
            finalize: () => this.patchState({ isLoading: false }),
          }),
        ),
      ),
    ),
  );

  completedAllEffect = this.effect<boolean>((isCompleted$) =>
    isCompleted$.pipe(
      tap(() => this.patchState({ isLoading: true })),
      withLatestFrom(this.todos$),
      switchMap(([isCompleted, todos]) => {
        const toggleCompletedRequests = todos.reduce<Observable<Todo>[]>((requests, todo) => {
          if (todo.isCompleted !== isCompleted) {
            requests.push(this.todosService.toggleCompleted(todo.id));
          }
          return requests;
        }, []);
        return merge(...toggleCompletedRequests).pipe(
          tapResponse({
            next: (todo) =>
              this.setState((state) => ({
                ...state,
                todos: state.todos.map((td) => (td.id === todo.id ? todo : td)),
              })),
            error: (error: HttpErrorResponse) =>
              this.patchState({ error: { title: 'Delete Fail!', details: error.message } }),
          }),
          last(),
          tap(() => {
            this.patchState({ isLoading: false });
          }),
        );
      }),
    ),
  );
}
