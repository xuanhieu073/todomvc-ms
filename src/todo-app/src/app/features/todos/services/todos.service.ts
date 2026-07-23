import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Todo } from '../models/todo';
import { CreateTodoRequest } from '../models/createTodoRequest';
import { UpdateTodoRequest } from '../models/updateTodoRequest';

@Service()
export class TodosService {
  private readonly http = inject(HttpClient);
  private apiUrl = 'https://localhost:7160';

  getTodos(filter: string = '') {
    return filter
      ? this.http.get<Todo[]>(`${this.apiUrl}/bff/todos`, { params: { filter } })
      : this.http.get<Todo[]>(`${this.apiUrl}/bff/todos`);
  }

  getTodo(id: string) {
    return this.http.get<Todo>(`${this.apiUrl}/bff/todos/{id}`);
  }

  createTodo(createTodoRequest: CreateTodoRequest) {
    return this.http.post<Todo>(`${this.apiUrl}/bff/todos`, createTodoRequest);
  }

  updateTodo(id: string, updateTodoRequest: UpdateTodoRequest) {
    return this.http.put<Todo>(`${this.apiUrl}/bff/todos/${id}`, updateTodoRequest);
  }

  toggleCompleted(id: string) {
    return this.http.patch<Todo>(`${this.apiUrl}/bff/todos/${id}/toggle`, null);
  }

  deleteTodo(id: string) {
    return this.http.delete<string>(`${this.apiUrl}/bff/todos/${id}`);
  }

  clearCompleted() {
    return this.http.delete<string>(`${this.apiUrl}/bff/todos/completed`);
  }
}
