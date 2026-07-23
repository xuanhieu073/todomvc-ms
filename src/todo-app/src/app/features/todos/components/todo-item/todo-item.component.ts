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
import { UpdateTodoRequest } from '../../models/update-todo-request';
import { ClickOutsideDirective } from './todo-item.clickoutside.directive';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-todo-item',
  imports: [ClickOutsideDirective, FormsModule, ReactiveFormsModule],
  template: `
    <div
      class="todo-item border border-gray-200 px-4 bg-white flex gap-4"
      [class.py-4]="!inEditMode()"
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
        <p class="text-2xl text-gray-600 w-full" (dblclick)="ToggleEditMode()">
          {{ todo().title }}
        </p>
      } @else {
        <input
          class="text-2xl px-4 py-4 ml-8 w-full outline-green-700"
          type="text"
          #editTitle
          (keydown.enter)="UpdateTodo($event)"
          (clickOutside)="UpdateTodo($event)"
          (keydown.esc)="CancelEdit()"
          [formControl]="updateTodoTitle"
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
  // title = model('');

  editTitleInput = viewChild<ElementRef<HTMLInputElement>>('editTitle');
  inEditMode = signal(false);
  updateTodoTitle = new FormControl('', {
    validators: [Validators.required, Validators.min(2), Validators.maxLength(200)],
  });

  constructor() {
    afterRenderEffect(() => {
      if (this.inEditMode()) {
        this.editTitleInput()?.nativeElement.focus();
      }
    });
  }

  ngAfterViewInit(): void {
    // this.title.set(this.todo().title);
    this.updateTodoTitle.setValue(this.todo().title);
  }

  ToggleEditMode() {
    this.inEditMode.set(!this.inEditMode());
  }

  CancelEdit() {
    this.inEditMode.set(false);
    // this.title.set(this.todo().title);
    this.updateTodoTitle.setValue(this.todo().title);
  }

  UpdateTodo(event: Event) {
    if (this.updateTodoTitle.valid) {
      const updateTodoRequest: UpdateTodoRequest = {
        // title: this.title(),
        title: this.updateTodoTitle.value!,
        isCompleted: this.todo().isCompleted,
      };
      this.todosStore.updateEffect({ id: this.todo().id, updateTodoRequest });
      this.inEditMode.set(false);
    }
  }

  ToggleCompleted(id: string) {
    this.todosStore.toggleCompletedEffect(id);
  }

  DeleteTodo(id: string) {
    this.todosStore.deleteEffect(id);
  }
}
