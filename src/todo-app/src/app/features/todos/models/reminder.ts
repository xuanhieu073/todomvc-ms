export interface Reminder {
  id: string;
  todoId: string;
  title: string;
  fireAt: Date;
  dueAt: Date;
  state: number;
  snoozeUntil: Date;
}
