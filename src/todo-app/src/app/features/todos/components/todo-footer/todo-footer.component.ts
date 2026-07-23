import { Component, inject } from '@angular/core';
import { FilterButtonComponent } from '../filter-button/filter-button.component';
import { TodosStore } from '../../todos.store';
import { RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-todo-footer',
  imports: [FilterButtonComponent, RouterLink, AsyncPipe],
  template: `
    @if (vm$ | async; as vm) {
      <div class="border border-gray-200 px-4 py-2 relative bg-white flex justify-between">
        <p>{{ vm.inCompletedCount }} item left</p>
        <div class="flex gap-2 absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2">
          <app-filter-button
            [routerLink]="['/todos']"
            fragment="all"
            [isActive]="vm.filter === 'all'"
            text="All"
          />
          <app-filter-button
            [routerLink]="['/todos']"
            fragment="active"
            [isActive]="vm.filter === 'active'"
            text="Active"
          />
          <app-filter-button
            [routerLink]="['/todos']"
            fragment="completed"
            [isActive]="vm.filter === 'completed'"
            text="Completed"
          />
        </div>
        @if (vm.completedCount > 0) {
          <p class="hover:underline cursor-pointer" (click)="ClearCompleted()">Clear Completed</p>
        }
        <div class=" absolute -z-10 h-10 -bottom-1 left-1 right-1 bg-white shadow"></div>
        <div class=" absolute -z-20 h-10 -bottom-2 left-2 right-2 bg-white shadow"></div>
      </div>
    }
  `,
  styles: ``,
})
export class TodoFooterComponent {
  private readonly todosStore = inject(TodosStore);

  vm$ = this.todosStore.vm$;

  ClearCompleted() {
    this.todosStore.clearCompletedEffect();
  }
}
