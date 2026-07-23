import {
  afterRenderEffect,
  AfterViewInit,
  Component,
  ElementRef,
  inject,
  input,
  model,
  signal,
  viewChild,
} from '@angular/core';
import { Todo } from '../../models/todo';
import { TodosStore } from '../../todos.store';
import { UpdateTodoRequest } from '../../models/updateTodoRequest';
import { ClickOutsideDirective } from './todo-item.clickoutside.directive';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-todo-item',
  imports: [ClickOutsideDirective, FormsModule],
  template: `
    <div
      class="todo-item border border-gray-200 px-4 bg-white flex gap-4"
      [class.py-4]="!inEditMode()"
      (dblclick)="ToggleEditMode()"
    >
      @if (!inEditMode()) {
        <button
          (click)="ToggleCompleted(todo().id)"
          class="border border-gray-500 rounded-full aspect-square h-8 flex items-center justify-center cursor-pointer"
        >
          @if (todo().isCompleted) {
            <span>✔️</span>
          }
        </button>
      }
      @if (!inEditMode()) {
        <p class="text-2xl text-gray-600">{{ todo().title }}</p>
      } @else {
        <input
          class="text-2xl px-4 py-4 ml-10 w-full"
          type="text"
          #editTitle
          [(ngModel)]="title"
          (keydown.enter)="UpdateTodo($event)"
          (clickOutside)="UpdateTodo($event)"
        />
      }
      @if (!inEditMode()) {
        <button class="hidden delete-button ml-auto" (click)="DeleteTodo(todo().id)">❌</button>
      }
    </div>
  `,
  styles: `
    .todo-item:hover .delete-button {
      @apply block;
    }
  `,
})
export class TodoItemComponent implements AfterViewInit {
  todo = input.required<Todo>();
  todosStore = inject(TodosStore);
  title = model('');

  editTitleInput = viewChild<ElementRef<HTMLInputElement>>('editTitle');
  inEditMode = signal(false);

  constructor() {
    afterRenderEffect(() => {
      if (this.inEditMode()) {
        this.editTitleInput()?.nativeElement.focus();
      }
    });
  }

  ngAfterViewInit(): void {
    this.title.set(this.todo().title);
  }

  ToggleEditMode() {
    this.inEditMode.set(!this.inEditMode());
    this.editTitleInput()?.nativeElement.focus();
  }

  UpdateTodo(event: Event) {
    console.log('update todo');
    const updateTodoRequest: UpdateTodoRequest = {
      title: this.title(),
      isCompleted: this.todo().isCompleted,
    };
    this.todosStore.updateEffect({ id: this.todo().id, updateTodoRequest });
    this.inEditMode.set(false);
  }

  ToggleCompleted(id: string) {
    this.todosStore.toggleCompletedEffect(id);
  }

  DeleteTodo(id: string) {
    this.todosStore.deleteEffect(id);
  }
}
