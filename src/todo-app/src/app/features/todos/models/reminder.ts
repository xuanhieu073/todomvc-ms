export interface Reminder {
  id: string;
  todoId: string;
  fireAt: Date;
  dueAt: Date;
  state: number;
  snoozeUntil: Date;
}
