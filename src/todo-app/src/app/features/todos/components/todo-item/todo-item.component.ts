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
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-todo-item',
  imports: [FormsModule, ReactiveFormsModule, DatePipe],
  template: `
    <div
      class="todo-item border border-gray-200 px-4 bg-white flex gap-4"
      [class.py-4]="!inEditMode()"
      /* (clickOutside)="UpdateTodo($event)" */
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
        <div class="w-full flex" (dblclick)="ToggleEditMode()">
          <p class="text-2xl text-gray-600 w-full">
            {{ todo().title }}
          </p>
          <p class="w-1/3">{{ todo().dueAt | date: 'short' }}</p>
        </div>
      } @else {
        <input
          class="text-2xl px-4 py-4 ml-8 w-full outline-green-700"
          type="text"
          #editTitle
          (keydown.enter)="UpdateTodo($event)"
          (keydown.esc)="CancelEdit()"
          [formControl]="updateTodoTitle"
        />

        <input
          class="outline-green-600 px-6 py-4 w-1/3"
          type="datetime-local"
          id="appointment"
          name="appointment"
          [formControl]="updateTodoDueAt"
          (keydown.enter)="UpdateTodo($event)"
        />

        <button (click)="UpdateTodo($event)">✔️</button>
      }
      @if (!inEditMode()) {}
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

  editTitleInput = viewChild<ElementRef<HTMLInputElement>>('editTitle');
  inEditMode = signal(false);
  updateTodoTitle = new FormControl('', {
    validators: [Validators.required, Validators.min(2), Validators.maxLength(200)],
  });
  updateTodoDueAt = new FormControl<Date>(new Date(), {
    validators: [Validators.required],
  });

  constructor() {
    afterRenderEffect(() => {
      if (this.inEditMode()) {
        this.editTitleInput()?.nativeElement.focus();
      }
    });
  }

  ngAfterViewInit(): void {
    const dateTimeLocal: any = this.todo().dueAt.toString().substring(0, 16);
    this.updateTodoTitle.setValue(this.todo().title);
    this.updateTodoDueAt.setValue(dateTimeLocal);
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
    if (this.updateTodoTitle.valid && this.updateTodoDueAt.valid) {
      const updateTodoRequest: UpdateTodoRequest = {
        // title: this.title(),
        title: this.updateTodoTitle.value!,
        isCompleted: this.todo().isCompleted,
        dueAt: this.updateTodoDueAt.value!,
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
